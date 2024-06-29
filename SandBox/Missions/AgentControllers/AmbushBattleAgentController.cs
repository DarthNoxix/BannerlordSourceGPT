using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentControllers;

public class AmbushBattleAgentController : AgentController
{
	private enum AgentState
	{
		None,
		SearchingForBoulder,
		MovingToBoulder,
		PickingUpBoulder,
		MovingBackToSpawn
	}

	private readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create("act_pickup_boulder_begin");

	private readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create("act_pickup_boulder_end");

	public bool IsAttacker;

	private bool _aggressive;

	public bool IsLeader;

	public GameEntity BoulderTarget;

	public bool HasBeenPlaced;

	public MatrixFrame OriginalSpawnFrame;

	private bool _boulderAddedToEquip;

	private AgentState _agentState = AgentState.SearchingForBoulder;

	public bool Aggressive
	{
		get
		{
			return _aggressive;
		}
		set
		{
			_aggressive = value;
			if (_aggressive)
			{
				base.Owner.SetWatchState(Agent.WatchState.Alarmed);
			}
		}
	}

	public override void OnInitialize()
	{
		Aggressive = false;
	}

	public bool CheckArrivedAtWayPoint(GameEntity waypoint)
	{
		return waypoint.CheckPointWithOrientedBoundingBox(base.Owner.Position);
	}

	public void UpdateState()
	{
		if (!IsAttacker)
		{
			UpdateDefendingAIAgentState();
		}
		else
		{
			UpdateAttackingAIAgentState();
		}
	}

	private void UpdateDefendingAIAgentState()
	{
	}

	private void UpdateAttackingAIAgentState()
	{
		if (_agentState == AgentState.MovingToBoulder || _agentState == AgentState.SearchingForBoulder)
		{
			if (base.Owner.Character != Game.Current.PlayerTroop && !base.Owner.Character.IsPlayerCharacter && _agentState != AgentState.SearchingForBoulder)
			{
				Vec3 origin = base.Owner.AgentVisuals.GetGlobalFrame().origin;
				Vec3 globalPosition = BoulderTarget.GlobalPosition;
				if (origin.DistanceSquared(globalPosition) < 0.16000001f)
				{
					MBDebug.Print("Picking up a boulder");
					_agentState = AgentState.PickingUpBoulder;
					base.Owner.DisableScriptedMovement();
					MatrixFrame globalFrame = BoulderTarget.GetGlobalFrame();
					Vec2 targetPosition = globalFrame.origin.AsVec2;
					base.Owner.SetTargetPositionAndDirectionSynched(ref targetPosition, ref globalFrame.rotation.f);
				}
			}
		}
		else if (_agentState == AgentState.PickingUpBoulder)
		{
			PickUpBoulderWithAnimation();
		}
		if (_agentState == AgentState.MovingBackToSpawn)
		{
			base.Owner.DisableScriptedMovement();
			_agentState = AgentState.None;
		}
	}

	private void PickUpBoulderWithAnimation()
	{
		ActionIndexValueCache currentActionValue = base.Owner.GetCurrentActionValue(0);
		if (!_boulderAddedToEquip && currentActionValue != act_pickup_boulder_begin)
		{
			base.Owner.SetActionChannel(0, act_pickup_boulder_begin, ignorePriority: true, 0uL);
		}
		else if (!_boulderAddedToEquip && currentActionValue == act_pickup_boulder_begin)
		{
			if (base.Owner.GetCurrentActionProgress(0) >= 0.7f)
			{
				_boulderAddedToEquip = true;
			}
		}
		else if (!base.Owner.IsMainAgent && _agentState == AgentState.PickingUpBoulder && currentActionValue != act_pickup_boulder_end && currentActionValue != act_pickup_boulder_begin)
		{
			base.Owner.ClearTargetFrame();
			if (!Aggressive)
			{
				WorldPosition position = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, OriginalSpawnFrame.origin, hasValidZ: false);
				base.Owner.SetScriptedPosition(ref position, addHumanLikeDelay: false);
				_agentState = AgentState.MovingBackToSpawn;
			}
			else
			{
				_agentState = AgentState.None;
			}
		}
	}
}
