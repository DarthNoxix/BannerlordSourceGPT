using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors;

public class AlarmedBehaviorGroup : AgentBehaviorGroup
{
	private static readonly ActionIndexCache act_scared_to_normal_1 = ActionIndexCache.Create("act_scared_to_normal_1");

	public const float SafetyDistance = 15f;

	public const float SafetyDistanceSquared = 225f;

	private readonly MissionAgentHandler _missionAgentHandler;

	private readonly MissionFightHandler _missionFightHandler;

	public bool DisableCalmDown;

	private readonly BasicMissionTimer _alarmedTimer;

	private readonly BasicMissionTimer _checkCalmDownTimer;

	private bool _isCalmingDown;

	public AlarmedBehaviorGroup(AgentNavigator navigator, Mission mission)
		: base(navigator, mission)
	{
		_alarmedTimer = new BasicMissionTimer();
		_checkCalmDownTimer = new BasicMissionTimer();
		_missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
		_missionFightHandler = base.Mission.GetMissionBehavior<MissionFightHandler>();
	}

	public override void Tick(float dt, bool isSimulation)
	{
		if (base.ScriptedBehavior != null)
		{
			if (!base.ScriptedBehavior.IsActive)
			{
				DisableAllBehaviors();
				base.ScriptedBehavior.IsActive = true;
			}
		}
		else
		{
			float num = 0f;
			int num2 = -1;
			for (int i = 0; i < Behaviors.Count; i++)
			{
				float availability = Behaviors[i].GetAvailability(isSimulation);
				if (availability > num)
				{
					num = availability;
					num2 = i;
				}
			}
			if (num > 0f && num2 != -1 && !Behaviors[num2].IsActive)
			{
				DisableAllBehaviors();
				Behaviors[num2].IsActive = true;
			}
		}
		TickActiveBehaviors(dt, isSimulation);
	}

	private void TickActiveBehaviors(float dt, bool isSimulation)
	{
		foreach (AgentBehavior behavior in Behaviors)
		{
			if (behavior.IsActive)
			{
				behavior.Tick(dt, isSimulation);
			}
		}
	}

	public override float GetScore(bool isSimulation)
	{
		if (base.OwnerAgent.CurrentWatchState == Agent.WatchState.Alarmed)
		{
			if (!DisableCalmDown && _alarmedTimer.ElapsedTime > 10f && _checkCalmDownTimer.ElapsedTime > 1f)
			{
				if (!_isCalmingDown)
				{
					_checkCalmDownTimer.Reset();
					if (!IsNearDanger())
					{
						_isCalmingDown = true;
						base.OwnerAgent.DisableScriptedMovement();
						base.OwnerAgent.SetActionChannel(0, act_scared_to_normal_1, ignorePriority: false, 0uL, 0f, 1f, -0.2f, 0.4f, MBRandom.RandomFloat);
					}
				}
				else if (!base.OwnerAgent.ActionSet.AreActionsAlternatives(base.OwnerAgent.GetCurrentActionValue(0), act_scared_to_normal_1))
				{
					_isCalmingDown = false;
					return 0f;
				}
			}
			return 1f;
		}
		if (IsNearDanger())
		{
			AlarmAgent(base.OwnerAgent);
			return 1f;
		}
		return 0f;
	}

	private bool IsNearDanger()
	{
		float distanceSquared;
		Agent closestAlarmSource = GetClosestAlarmSource(out distanceSquared);
		if (closestAlarmSource != null)
		{
			if (!(distanceSquared < 225f))
			{
				return Navigator.CanSeeAgent(closestAlarmSource);
			}
			return true;
		}
		return false;
	}

	public Agent GetClosestAlarmSource(out float distanceSquared)
	{
		distanceSquared = float.MaxValue;
		if (_missionFightHandler == null || !_missionFightHandler.IsThereActiveFight())
		{
			return null;
		}
		Agent result = null;
		foreach (Agent dangerSource in _missionFightHandler.GetDangerSources(base.OwnerAgent))
		{
			float num = dangerSource.Position.DistanceSquared(base.OwnerAgent.Position);
			if (num < distanceSquared)
			{
				distanceSquared = num;
				result = dangerSource;
			}
		}
		return result;
	}

	public static void AlarmAgent(Agent agent)
	{
		agent.SetWatchState(Agent.WatchState.Alarmed);
	}

	protected override void OnActivate()
	{
		TextObject textObject = new TextObject("{=!}{p0} {p1} activate alarmed behavior group.");
		textObject.SetTextVariable("p0", base.OwnerAgent.Name);
		textObject.SetTextVariable("p1", base.OwnerAgent.Index);
		_isCalmingDown = false;
		_alarmedTimer.Reset();
		_checkCalmDownTimer.Reset();
		base.OwnerAgent.DisableScriptedMovement();
		base.OwnerAgent.ClearTargetFrame();
		Navigator.SetItemsVisibility(isVisible: false);
		LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(base.OwnerAgent.Origin);
		if (locationCharacter.ActionSetCode != locationCharacter.AlarmedActionSetCode)
		{
			AnimationSystemData animationSystemData = locationCharacter.GetAgentBuildData().AgentMonster.FillAnimationSystemData(MBGlobals.GetActionSet(locationCharacter.AlarmedActionSetCode), locationCharacter.Character.GetStepSize(), hasClippingPlane: false);
			base.OwnerAgent.SetActionSet(ref animationSystemData);
		}
		if (Navigator.MemberOfAlley != null || MissionFightHandler.IsAgentAggressive(base.OwnerAgent))
		{
			DisableCalmDown = true;
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		_isCalmingDown = false;
		if (base.OwnerAgent.IsActive())
		{
			base.OwnerAgent.TryToSheathWeaponInHand(Agent.HandIndex.OffHand, Agent.WeaponWieldActionType.WithAnimationUninterruptible);
			base.OwnerAgent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.WithAnimationUninterruptible);
			if (base.OwnerAgent.Team.IsValid && base.OwnerAgent.Team == base.Mission.PlayerEnemyTeam)
			{
				base.OwnerAgent.SetTeam(new Team(MBTeam.InvalidTeam, BattleSideEnum.None, null), sync: true);
			}
			base.OwnerAgent.SetWatchState(Agent.WatchState.Patrolling);
			base.OwnerAgent.ResetLookAgent();
			base.OwnerAgent.SetActionChannel(0, ActionIndexCache.act_none, ignorePriority: true, 0uL);
			base.OwnerAgent.SetActionChannel(1, ActionIndexCache.act_none, ignorePriority: true, 0uL);
		}
	}

	public override void ForceThink(float inSeconds)
	{
	}
}
