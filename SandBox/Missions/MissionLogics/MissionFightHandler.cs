using System.Collections.Generic;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics;

public class MissionFightHandler : MissionLogic
{
	private enum State
	{
		NoFight,
		Fighting,
		FightEnded
	}

	public delegate void OnFightEndDelegate(bool isPlayerSideWon);

	private static OnFightEndDelegate _onFightEnd;

	private List<Agent> _playerSideAgents;

	private List<Agent> _opponentSideAgents;

	private Dictionary<Agent, Team> _playerSideAgentsOldTeamData;

	private Dictionary<Agent, Team> _opponentSideAgentsOldTeamData;

	private State _state;

	private BasicMissionTimer _finishTimer;

	private BasicMissionTimer _prepareTimer;

	private bool _isPlayerSideWon;

	private MissionMode _oldMissionMode;

	private static MissionFightHandler _current => Mission.Current.GetMissionBehavior<MissionFightHandler>();

	public IEnumerable<Agent> PlayerSideAgents => _playerSideAgents.AsReadOnly();

	public IEnumerable<Agent> OpponentSideAgents => _opponentSideAgents.AsReadOnly();

	public bool IsPlayerSideWon => _isPlayerSideWon;

	public override void OnBehaviorInitialize()
	{
		base.Mission.IsAgentInteractionAllowed_AdditionalCondition += IsAgentInteractionAllowed_AdditionalCondition;
	}

	public override void EarlyStart()
	{
		_playerSideAgents = new List<Agent>();
		_opponentSideAgents = new List<Agent>();
	}

	public override void AfterStart()
	{
	}

	public override void OnMissionTick(float dt)
	{
		if (_finishTimer != null && _finishTimer.ElapsedTime > 5f)
		{
			_finishTimer = null;
			EndFight();
			_prepareTimer = new BasicMissionTimer();
		}
		if (_prepareTimer != null && _prepareTimer.ElapsedTime > 3f)
		{
			_prepareTimer = null;
		}
	}

	public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
	{
		if (_state != State.Fighting)
		{
			return;
		}
		if (affectedAgent == Agent.Main)
		{
			Mission.Current.NextCheckTimeEndMission += 8f;
		}
		if (affectorAgent != null && _playerSideAgents.Contains(affectedAgent))
		{
			_playerSideAgents.Remove(affectedAgent);
			if (_playerSideAgents.Count == 0)
			{
				_isPlayerSideWon = false;
				ResetScriptedBehaviors();
				_finishTimer = new BasicMissionTimer();
			}
		}
		else if (affectorAgent != null && _opponentSideAgents.Contains(affectedAgent))
		{
			_opponentSideAgents.Remove(affectedAgent);
			if (_opponentSideAgents.Count == 0)
			{
				_isPlayerSideWon = true;
				ResetScriptedBehaviors();
				_finishTimer = new BasicMissionTimer();
			}
		}
	}

	public void StartCustomFight(List<Agent> playerSideAgents, List<Agent> opponentSideAgents, bool dropWeapons, bool isItemUseDisabled, OnFightEndDelegate onFightEndDelegate)
	{
		_state = State.Fighting;
		_opponentSideAgents = opponentSideAgents;
		_playerSideAgents = playerSideAgents;
		_playerSideAgentsOldTeamData = new Dictionary<Agent, Team>();
		_opponentSideAgentsOldTeamData = new Dictionary<Agent, Team>();
		_onFightEnd = onFightEndDelegate;
		Mission.Current.MainAgent.IsItemUseDisabled = isItemUseDisabled;
		foreach (Agent opponentSideAgent in _opponentSideAgents)
		{
			if (dropWeapons)
			{
				DropAllWeapons(opponentSideAgent);
			}
			_opponentSideAgentsOldTeamData.Add(opponentSideAgent, opponentSideAgent.Team);
			ForceAgentForFight(opponentSideAgent);
		}
		foreach (Agent playerSideAgent in _playerSideAgents)
		{
			if (dropWeapons)
			{
				DropAllWeapons(playerSideAgent);
			}
			_playerSideAgentsOldTeamData.Add(playerSideAgent, playerSideAgent.Team);
			ForceAgentForFight(playerSideAgent);
		}
		SetTeamsForFightAndDuel();
		_oldMissionMode = Mission.Current.Mode;
		Mission.Current.SetMissionMode(MissionMode.Battle, atStart: false);
	}

	public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
	{
		canPlayerLeave = true;
		if (_state == State.Fighting && (_opponentSideAgents.Count > 0 || _playerSideAgents.Count > 0))
		{
			MBInformationManager.AddQuickInformation(new TextObject("{=Fpk3BUBs}Your duel has not ended yet!"));
			canPlayerLeave = false;
		}
		return null;
	}

	private void ForceAgentForFight(Agent agent)
	{
		if (agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
		{
			AlarmedBehaviorGroup behaviorGroup = agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.DisableCalmDown = true;
			behaviorGroup.AddBehavior<FightBehavior>();
			behaviorGroup.SetScriptedBehavior<FightBehavior>();
		}
	}

	protected override void OnEndMission()
	{
		base.Mission.IsAgentInteractionAllowed_AdditionalCondition -= IsAgentInteractionAllowed_AdditionalCondition;
	}

	private void SetTeamsForFightAndDuel()
	{
		Mission.Current.PlayerEnemyTeam.SetIsEnemyOf(Mission.Current.PlayerTeam, isEnemyOf: true);
		foreach (Agent playerSideAgent in _playerSideAgents)
		{
			if (playerSideAgent.IsHuman)
			{
				if (playerSideAgent.IsAIControlled)
				{
					playerSideAgent.SetWatchState(Agent.WatchState.Alarmed);
				}
				playerSideAgent.SetTeam(Mission.Current.PlayerTeam, sync: true);
			}
		}
		foreach (Agent opponentSideAgent in _opponentSideAgents)
		{
			if (opponentSideAgent.IsHuman)
			{
				if (opponentSideAgent.IsAIControlled)
				{
					opponentSideAgent.SetWatchState(Agent.WatchState.Alarmed);
				}
				opponentSideAgent.SetTeam(Mission.Current.PlayerEnemyTeam, sync: true);
			}
		}
	}

	private void ResetTeamsForFightAndDuel()
	{
		foreach (Agent playerSideAgent in _playerSideAgents)
		{
			if (playerSideAgent.IsAIControlled)
			{
				playerSideAgent.ResetEnemyCaches();
				playerSideAgent.InvalidateTargetAgent();
				playerSideAgent.InvalidateAIWeaponSelections();
				playerSideAgent.SetWatchState(Agent.WatchState.Patrolling);
			}
			playerSideAgent.SetTeam(new Team(_playerSideAgentsOldTeamData[playerSideAgent].MBTeam, BattleSideEnum.None, base.Mission), sync: true);
		}
		foreach (Agent opponentSideAgent in _opponentSideAgents)
		{
			if (opponentSideAgent.IsAIControlled)
			{
				opponentSideAgent.ResetEnemyCaches();
				opponentSideAgent.InvalidateTargetAgent();
				opponentSideAgent.InvalidateAIWeaponSelections();
				opponentSideAgent.SetWatchState(Agent.WatchState.Patrolling);
			}
			opponentSideAgent.SetTeam(new Team(_opponentSideAgentsOldTeamData[opponentSideAgent].MBTeam, BattleSideEnum.None, base.Mission), sync: true);
		}
	}

	private bool IsAgentInteractionAllowed_AdditionalCondition()
	{
		return _state != State.Fighting;
	}

	public static Agent GetAgentToSpectate()
	{
		MissionFightHandler current = _current;
		if (current._playerSideAgents.Count > 0)
		{
			return current._playerSideAgents[0];
		}
		if (current._opponentSideAgents.Count > 0)
		{
			return current._opponentSideAgents[0];
		}
		return null;
	}

	private void DropAllWeapons(Agent agent)
	{
		for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
		{
			if (!agent.Equipment[equipmentIndex].IsEmpty)
			{
				agent.DropItem(equipmentIndex);
			}
		}
	}

	private void ResetScriptedBehaviors()
	{
		foreach (Agent playerSideAgent in _playerSideAgents)
		{
			if (playerSideAgent.IsActive() && playerSideAgent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
			{
				playerSideAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().DisableScriptedBehavior();
			}
		}
		foreach (Agent opponentSideAgent in _opponentSideAgents)
		{
			if (opponentSideAgent.IsActive() && opponentSideAgent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
			{
				opponentSideAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().DisableScriptedBehavior();
			}
		}
	}

	public void EndFight()
	{
		ResetTeamsForFightAndDuel();
		_state = State.FightEnded;
		foreach (Agent playerSideAgent in _playerSideAgents)
		{
			playerSideAgent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.WithAnimationUninterruptible);
			playerSideAgent.TryToSheathWeaponInHand(Agent.HandIndex.OffHand, Agent.WeaponWieldActionType.WithAnimationUninterruptible);
		}
		foreach (Agent opponentSideAgent in _opponentSideAgents)
		{
			opponentSideAgent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.WithAnimationUninterruptible);
			opponentSideAgent.TryToSheathWeaponInHand(Agent.HandIndex.OffHand, Agent.WeaponWieldActionType.WithAnimationUninterruptible);
		}
		_playerSideAgents.Clear();
		_opponentSideAgents.Clear();
		if (Mission.Current.MainAgent != null)
		{
			Mission.Current.MainAgent.IsItemUseDisabled = false;
		}
		if (_oldMissionMode == MissionMode.Conversation && !Campaign.Current.ConversationManager.IsConversationFlowActive)
		{
			_oldMissionMode = MissionMode.StartUp;
		}
		Mission.Current.SetMissionMode(_oldMissionMode, atStart: false);
		if (_onFightEnd != null)
		{
			_onFightEnd(_isPlayerSideWon);
			_onFightEnd = null;
		}
	}

	public bool IsThereActiveFight()
	{
		return _state == State.Fighting;
	}

	public void AddAgentToSide(Agent agent, bool isPlayerSide)
	{
		if (IsThereActiveFight() && !_playerSideAgents.Contains(agent) && !_opponentSideAgents.Contains(agent))
		{
			if (isPlayerSide)
			{
				agent.SetTeam(Mission.Current.PlayerTeam, sync: true);
				_playerSideAgents.Add(agent);
			}
			else
			{
				agent.SetTeam(Mission.Current.PlayerEnemyTeam, sync: true);
				_opponentSideAgents.Add(agent);
			}
		}
	}

	public IEnumerable<Agent> GetDangerSources(Agent ownerAgent)
	{
		if (!(ownerAgent.Character is CharacterObject))
		{
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\MissionFightHandler.cs", "GetDangerSources", 370);
			return new List<Agent>();
		}
		if (IsThereActiveFight() && !IsAgentAggressive(ownerAgent) && Agent.Main != null)
		{
			return new List<Agent> { Agent.Main };
		}
		return new List<Agent>();
	}

	public static bool IsAgentAggressive(Agent agent)
	{
		CharacterObject characterObject = agent.Character as CharacterObject;
		if (!agent.HasWeapon())
		{
			if (characterObject != null)
			{
				if (characterObject.Occupation != Occupation.Mercenary && !IsAgentVillian(characterObject))
				{
					return IsAgentJusticeWarrior(characterObject);
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public static bool IsAgentJusticeWarrior(CharacterObject character)
	{
		if (character.Occupation != Occupation.Soldier && character.Occupation != Occupation.Guard)
		{
			return character.Occupation == Occupation.PrisonGuard;
		}
		return true;
	}

	public static bool IsAgentVillian(CharacterObject character)
	{
		if (character.Occupation != Occupation.Gangster && character.Occupation != Occupation.GangLeader)
		{
			return character.Occupation == Occupation.Bandit;
		}
		return true;
	}
}
