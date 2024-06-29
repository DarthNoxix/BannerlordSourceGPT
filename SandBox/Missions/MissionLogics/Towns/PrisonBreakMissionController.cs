using System.Collections.Generic;
using System.Linq;
using SandBox.CampaignBehaviors;
using SandBox.Missions.AgentBehaviors;
using SandBox.Objects.AnimationPoints;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;

namespace SandBox.Missions.MissionLogics.Towns;

public class PrisonBreakMissionController : MissionLogic
{
	private const int PrisonerSwitchToAlarmedDistance = 3;

	private readonly CharacterObject _prisonerCharacter;

	private readonly CharacterObject _companionCharacter;

	private List<Agent> _guardAgents;

	private List<Agent> _agentsToRemove;

	private Agent _prisonerAgent;

	private List<AreaMarker> _areaMarkers;

	private bool _isPrisonerFollowing;

	public PrisonBreakMissionController(CharacterObject prisonerCharacter, CharacterObject companionCharacter)
	{
		_prisonerCharacter = prisonerCharacter;
		_companionCharacter = companionCharacter;
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Mission.DoesMissionRequireCivilianEquipment = true;
	}

	public override void OnBehaviorInitialize()
	{
		base.Mission.IsAgentInteractionAllowed_AdditionalCondition += IsAgentInteractionAllowed_AdditionalCondition;
	}

	public override void AfterStart()
	{
		_isPrisonerFollowing = true;
		MBTextManager.SetTextVariable("IS_PRISONER_FOLLOWING", _isPrisonerFollowing ? 1 : 0);
		base.Mission.SetMissionMode(MissionMode.Stealth, atStart: true);
		base.Mission.IsInventoryAccessible = false;
		base.Mission.IsQuestScreenAccessible = true;
		LocationCharacter firstLocationCharacterOfCharacter = LocationComplex.Current.GetFirstLocationCharacterOfCharacter(_prisonerCharacter);
		PlayerEncounter.LocationEncounter.AddAccompanyingCharacter(firstLocationCharacterOfCharacter, isFollowing: true);
		_areaMarkers = (from area in base.Mission.ActiveMissionObjects.FindAllWithType<AreaMarker>()
			orderby area.AreaIndex
			select area).ToList();
		MissionAgentHandler missionBehavior = base.Mission.GetMissionBehavior<MissionAgentHandler>();
		foreach (UsableMachine townPassageProp in missionBehavior.TownPassageProps)
		{
			townPassageProp.Deactivate();
		}
		missionBehavior.SpawnPlayer(base.Mission.DoesMissionRequireCivilianEquipment, noHorses: true);
		missionBehavior.SpawnLocationCharacters();
		ArrangeGuardCount();
		_prisonerAgent = base.Mission.Agents.First((Agent x) => x.Character == _prisonerCharacter);
		PreparePrisonAgent();
		missionBehavior.SimulateAgent(_prisonerAgent);
		for (int i = 0; i < _guardAgents.Count; i++)
		{
			Agent agent = _guardAgents[i];
			agent.GetComponent<CampaignAgentComponent>().AgentNavigator.SpecialTargetTag = _areaMarkers[i % _areaMarkers.Count].Tag;
			missionBehavior.SimulateAgent(agent);
		}
		SetTeams();
	}

	public override void OnMissionTick(float dt)
	{
		SandBoxHelpers.MissionHelper.FadeOutAgents(_agentsToRemove, hideInstantly: true, hideMount: true);
		_agentsToRemove.Clear();
		if (_prisonerAgent != null)
		{
			CheckPrisonerSwitchToAlarmState();
		}
	}

	public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
	{
		if (_guardAgents != null && usedObject is AnimationPoint && _guardAgents.Contains(userAgent))
		{
			userAgent.StopUsingGameObject();
		}
	}

	public override void OnAgentInteraction(Agent userAgent, Agent agent)
	{
		if (userAgent == Agent.Main && agent == _prisonerAgent)
		{
			SwitchPrisonerFollowingState();
		}
	}

	public override bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
	{
		if (userAgent == Agent.Main)
		{
			return otherAgent == _prisonerAgent;
		}
		return false;
	}

	private void PreparePrisonAgent()
	{
		_prisonerAgent.Health = _prisonerAgent.HealthLimit;
		_prisonerAgent.Defensiveness = 2f;
		AgentNavigator agentNavigator = _prisonerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator;
		agentNavigator.RemoveBehaviorGroup<AlarmedBehaviorGroup>();
		agentNavigator.SpecialTargetTag = "sp_prison_break_prisoner";
		ItemObject item = Items.All.Where((ItemObject x) => x.IsCraftedWeapon && x.Type == ItemObject.ItemTypeEnum.OneHandedWeapon && x.WeaponComponent.GetItemType() == ItemObject.ItemTypeEnum.OneHandedWeapon && x.IsCivilian).MinBy((ItemObject x) => x.Value);
		MissionWeapon weapon = new MissionWeapon(item, null, _prisonerCharacter.HeroObject.ClanBanner);
		_prisonerAgent.EquipWeaponWithNewEntity(EquipmentIndex.WeaponItemBeginSlot, ref weapon);
	}

	public override void OnAgentAlarmedStateChanged(Agent agent, Agent.AIStateFlag flag)
	{
		if (agent == _prisonerAgent && flag != Agent.AIStateFlag.Cautious)
		{
			SwitchPrisonerFollowingState(forceFollow: true);
		}
		UpdateDoorPermission();
	}

	private void ArrangeGuardCount()
	{
		int num = 2 + Settlement.CurrentSettlement.Town.GetWallLevel();
		float security = Settlement.CurrentSettlement.Town.Security;
		if (security < 40f)
		{
			num--;
		}
		else if (security > 70f)
		{
			num++;
		}
		_guardAgents = base.Mission.Agents.Where((Agent x) => x.Character is CharacterObject characterObject && characterObject.IsSoldier).ToList();
		_agentsToRemove = new List<Agent>();
		int count = _guardAgents.Count;
		if (count > num)
		{
			int num2 = count - num;
			for (int i = 0; i < count; i++)
			{
				if (num2 <= 0)
				{
					break;
				}
				Agent agent = _guardAgents[i];
				if (!agent.Character.IsHero)
				{
					_agentsToRemove.Add(agent);
					num2--;
				}
			}
		}
		else if (count < num)
		{
			List<LocationCharacter> list = (from x in LocationComplex.Current.GetListOfCharactersInLocation("prison")
				where !x.Character.IsHero && x.Character.IsSoldier
				select x).ToList();
			if (list.IsEmpty())
			{
				AgentData agentData = GuardsCampaignBehavior.PrepareGuardAgentDataFromGarrison(PlayerEncounter.LocationEncounter.Settlement.Culture.Guard, overrideWeaponWithSpear: true);
				LocationCharacter item = new LocationCharacter(agentData, SandBoxManager.Instance.AgentBehaviorManager.AddStandGuardBehaviors, "sp_guard", fixedLocation: true, LocationCharacter.CharacterRelations.Neutral, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_guard"), useCivilianEquipment: false);
				list.Add(item);
			}
			int count2 = list.Count;
			Location locationWithId = LocationComplex.Current.GetLocationWithId("prison");
			int num3 = num - count;
			for (int j = 0; j < num3; j++)
			{
				LocationCharacter locationCharacter = list[j % count2];
				LocationCharacter locationCharacter2 = new LocationCharacter(new AgentData(new SimpleAgentOrigin(locationCharacter.Character, -1, locationCharacter.AgentData.AgentOrigin.Banner)).Equipment(locationCharacter.AgentData.AgentOverridenEquipment).Monster(locationCharacter.AgentData.AgentMonster).NoHorses(noHorses: true), locationCharacter.AddBehaviors, _areaMarkers[j % _areaMarkers.Count].Tag, fixedLocation: true, LocationCharacter.CharacterRelations.Enemy, locationCharacter.ActionSetCode, locationCharacter.UseCivilianEquipment);
				LocationComplex.Current.ChangeLocation(locationCharacter2, null, locationWithId);
			}
		}
		_guardAgents = base.Mission.Agents.Where((Agent x) => x.Character is CharacterObject && ((CharacterObject)x.Character).IsSoldier && !_agentsToRemove.Contains(x)).ToList();
	}

	public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
	{
		if (_guardAgents.Contains(affectedAgent))
		{
			_guardAgents.Remove(affectedAgent);
			UpdateDoorPermission();
			if (_prisonerAgent != null)
			{
				AgentFlag agentFlags = _prisonerAgent.GetAgentFlags();
				_prisonerAgent.SetAgentFlags(agentFlags & ~AgentFlag.CanGetAlarmed);
			}
		}
		else if (_prisonerAgent == affectedAgent)
		{
			_prisonerAgent = null;
		}
	}

	private void CheckPrisonerSwitchToAlarmState()
	{
		foreach (Agent guardAgent in _guardAgents)
		{
			if (_prisonerAgent.Position.DistanceSquared(guardAgent.Position) < 3f)
			{
				AgentFlag agentFlags = _prisonerAgent.GetAgentFlags();
				_prisonerAgent.SetAgentFlags(agentFlags | AgentFlag.CanGetAlarmed);
			}
		}
	}

	private void SwitchPrisonerFollowingState(bool forceFollow = false)
	{
		_isPrisonerFollowing = forceFollow || !_isPrisonerFollowing;
		MBTextManager.SetTextVariable("IS_PRISONER_FOLLOWING", _isPrisonerFollowing ? 1 : 0);
		FollowAgentBehavior behavior = _prisonerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().GetBehavior<FollowAgentBehavior>();
		if (_isPrisonerFollowing)
		{
			_prisonerAgent.SetCrouchMode(set: false);
			behavior.SetTargetAgent(Agent.Main);
			AgentFlag agentFlags = _prisonerAgent.GetAgentFlags();
			_prisonerAgent.SetAgentFlags(agentFlags & ~AgentFlag.CanGetAlarmed);
		}
		else
		{
			behavior.SetTargetAgent(null);
			_prisonerAgent.SetCrouchMode(set: true);
		}
		_prisonerAgent.AIStateFlags = Agent.AIStateFlag.None;
	}

	public override InquiryData OnEndMissionRequest(out bool canLeave)
	{
		canLeave = Agent.Main == null || !Agent.Main.IsActive() || _guardAgents.IsEmpty() || _guardAgents.All((Agent x) => !x.IsActive());
		if (!canLeave)
		{
			MBInformationManager.AddQuickInformation(GameTexts.FindText("str_can_not_retreat"));
		}
		return null;
	}

	private void SetTeams()
	{
		base.Mission.PlayerTeam.SetIsEnemyOf(base.Mission.PlayerEnemyTeam, isEnemyOf: true);
		_prisonerAgent.SetTeam(base.Mission.PlayerTeam, sync: true);
		if (_companionCharacter != null)
		{
			base.Mission.Agents.First((Agent x) => x.Character == _companionCharacter).SetTeam(base.Mission.PlayerTeam, sync: true);
		}
		foreach (Agent guardAgent in _guardAgents)
		{
			guardAgent.SetTeam(base.Mission.PlayerEnemyTeam, sync: true);
			AgentFlag agentFlags = guardAgent.GetAgentFlags();
			guardAgent.SetAgentFlags((agentFlags | AgentFlag.CanGetAlarmed) & ~AgentFlag.CanRetreat);
		}
	}

	protected override void OnEndMission()
	{
		if (PlayerEncounter.LocationEncounter.CharactersAccompanyingPlayer.Any((AccompanyingCharacter x) => x.LocationCharacter.Character == _prisonerCharacter))
		{
			PlayerEncounter.LocationEncounter.RemoveAccompanyingCharacter(_prisonerCharacter.HeroObject);
		}
		if (Agent.Main == null || !Agent.Main.IsActive())
		{
			GameMenu.SwitchToMenu("settlement_prison_break_fail_player_unconscious");
		}
		else if (_prisonerAgent == null || !_prisonerAgent.IsActive())
		{
			GameMenu.SwitchToMenu("settlement_prison_break_fail_prisoner_unconscious");
		}
		else
		{
			GameMenu.SwitchToMenu("settlement_prison_break_success");
		}
		Campaign.Current.GameMenuManager.NextLocation = null;
		Campaign.Current.GameMenuManager.PreviousLocation = null;
		base.Mission.IsAgentInteractionAllowed_AdditionalCondition -= IsAgentInteractionAllowed_AdditionalCondition;
	}

	private void UpdateDoorPermission()
	{
		bool flag = _guardAgents.IsEmpty() || _guardAgents.All((Agent x) => x.CurrentWatchState != Agent.WatchState.Alarmed);
		foreach (UsableMachine townPassageProp in base.Mission.GetMissionBehavior<MissionAgentHandler>().TownPassageProps)
		{
			if (flag)
			{
				townPassageProp.Activate();
			}
			else
			{
				townPassageProp.Deactivate();
			}
		}
	}

	private bool IsAgentInteractionAllowed_AdditionalCondition()
	{
		return true;
	}
}
