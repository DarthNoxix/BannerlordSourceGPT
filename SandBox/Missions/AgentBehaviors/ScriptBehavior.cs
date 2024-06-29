using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors;

public class ScriptBehavior : AgentBehavior
{
	public delegate bool SelectTargetDelegate(Agent agent, ref Agent targetAgent, ref UsableMachine targetUsableMachine, ref WorldFrame targetFrame);

	public delegate bool OnTargetReachedDelegate(Agent agent, ref Agent targetAgent, ref UsableMachine targetUsableMachine, ref WorldFrame targetFrame);

	private enum State
	{
		NoTarget,
		GoToUsableMachine,
		GoToAgent,
		GoToTargetFrame,
		NearAgent,
		NearStationaryTarget
	}

	private UsableMachine _targetUsableMachine;

	private Agent _targetAgent;

	private WorldFrame _targetFrame;

	private State _state;

	private bool _sentToTarget;

	private SelectTargetDelegate _selectTargetDelegate;

	private OnTargetReachedDelegate _onTargetReachedDelegate;

	public ScriptBehavior(AgentBehaviorGroup behaviorGroup)
		: base(behaviorGroup)
	{
	}

	public static void AddUsableMachineTarget(Agent ownerAgent, UsableMachine targetUsableMachine)
	{
		DailyBehaviorGroup behaviorGroup = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
		ScriptBehavior scriptBehavior = behaviorGroup.GetBehavior<ScriptBehavior>() ?? behaviorGroup.AddBehavior<ScriptBehavior>();
		bool num = behaviorGroup.ScriptedBehavior != scriptBehavior;
		scriptBehavior._targetUsableMachine = targetUsableMachine;
		scriptBehavior._state = State.GoToUsableMachine;
		scriptBehavior._sentToTarget = false;
		if (num)
		{
			behaviorGroup.SetScriptedBehavior<ScriptBehavior>();
		}
	}

	public static void AddAgentTarget(Agent ownerAgent, Agent targetAgent)
	{
		DailyBehaviorGroup behaviorGroup = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
		ScriptBehavior scriptBehavior = behaviorGroup.GetBehavior<ScriptBehavior>() ?? behaviorGroup.AddBehavior<ScriptBehavior>();
		bool num = behaviorGroup.ScriptedBehavior != scriptBehavior;
		scriptBehavior._targetAgent = targetAgent;
		scriptBehavior._state = State.GoToAgent;
		scriptBehavior._sentToTarget = false;
		if (num)
		{
			behaviorGroup.SetScriptedBehavior<ScriptBehavior>();
		}
	}

	public static void AddWorldFrameTarget(Agent ownerAgent, WorldFrame targetWorldFrame)
	{
		DailyBehaviorGroup behaviorGroup = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
		ScriptBehavior scriptBehavior = behaviorGroup.GetBehavior<ScriptBehavior>() ?? behaviorGroup.AddBehavior<ScriptBehavior>();
		bool num = behaviorGroup.ScriptedBehavior != scriptBehavior;
		scriptBehavior._targetFrame = targetWorldFrame;
		scriptBehavior._state = State.GoToTargetFrame;
		scriptBehavior._sentToTarget = false;
		if (num)
		{
			behaviorGroup.SetScriptedBehavior<ScriptBehavior>();
		}
	}

	public static void AddTargetWithDelegate(Agent ownerAgent, SelectTargetDelegate selectTargetDelegate, OnTargetReachedDelegate onTargetReachedDelegate)
	{
		DailyBehaviorGroup behaviorGroup = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
		ScriptBehavior scriptBehavior = behaviorGroup.GetBehavior<ScriptBehavior>() ?? behaviorGroup.AddBehavior<ScriptBehavior>();
		bool num = behaviorGroup.ScriptedBehavior != scriptBehavior;
		scriptBehavior._selectTargetDelegate = selectTargetDelegate;
		scriptBehavior._onTargetReachedDelegate = onTargetReachedDelegate;
		scriptBehavior._state = State.NoTarget;
		scriptBehavior._sentToTarget = false;
		if (num)
		{
			behaviorGroup.SetScriptedBehavior<ScriptBehavior>();
		}
	}

	public bool IsNearTarget(Agent targetAgent)
	{
		if (_targetAgent == targetAgent)
		{
			if (_state != State.NearAgent)
			{
				return _state == State.NearStationaryTarget;
			}
			return true;
		}
		return false;
	}

	public override void Tick(float dt, bool isSimulation)
	{
		if (_state == State.NoTarget)
		{
			if (_selectTargetDelegate == null)
			{
				if (BehaviorGroup.ScriptedBehavior == this)
				{
					BehaviorGroup.DisableScriptedBehavior();
				}
				return;
			}
			SearchForNewTarget();
		}
		switch (_state)
		{
		case State.GoToUsableMachine:
			if (!_sentToTarget)
			{
				base.Navigator.SetTarget(_targetUsableMachine);
				_sentToTarget = true;
			}
			else if (base.OwnerAgent.IsUsingGameObject && base.OwnerAgent.Position.DistanceSquared(_targetUsableMachine.GameEntity.GetGlobalFrame().origin) < 1f)
			{
				if (CheckForSearchNewTarget(State.NearStationaryTarget))
				{
					base.OwnerAgent.StopUsingGameObject(isSuccessful: false);
				}
				else
				{
					RemoveTargets();
				}
			}
			break;
		case State.GoToAgent:
		{
			float interactionDistanceToUsable = base.OwnerAgent.GetInteractionDistanceToUsable(_targetAgent);
			if (base.OwnerAgent.Position.DistanceSquared(_targetAgent.Position) < interactionDistanceToUsable * interactionDistanceToUsable)
			{
				if (!CheckForSearchNewTarget(State.NearAgent))
				{
					base.Navigator.SetTargetFrame(base.OwnerAgent.GetWorldPosition(), base.OwnerAgent.Frame.rotation.f.AsVec2.RotationInRadians, 1f, 1f);
					RemoveTargets();
				}
			}
			else
			{
				base.Navigator.SetTargetFrame(_targetAgent.GetWorldPosition(), _targetAgent.Frame.rotation.f.AsVec2.RotationInRadians, 1f, 1f);
			}
			break;
		}
		case State.GoToTargetFrame:
			if (!_sentToTarget)
			{
				base.Navigator.SetTargetFrame(_targetFrame.Origin, _targetFrame.Rotation.f.AsVec2.RotationInRadians, 1f, 1f, Agent.AIScriptedFrameFlags.DoNotRun);
				_sentToTarget = true;
			}
			else if (base.Navigator.IsTargetReached() && !CheckForSearchNewTarget(State.NearStationaryTarget))
			{
				RemoveTargets();
			}
			break;
		case State.NearAgent:
			if (base.OwnerAgent.Position.DistanceSquared(_targetAgent.Position) >= 1f)
			{
				_state = State.GoToAgent;
				break;
			}
			base.Navigator.SetTargetFrame(base.OwnerAgent.GetWorldPosition(), base.OwnerAgent.Frame.rotation.f.AsVec2.RotationInRadians, 1f, 1f);
			RemoveTargets();
			break;
		}
	}

	private bool CheckForSearchNewTarget(State endState)
	{
		bool flag = false;
		if (_onTargetReachedDelegate != null)
		{
			flag = _onTargetReachedDelegate(base.OwnerAgent, ref _targetAgent, ref _targetUsableMachine, ref _targetFrame);
		}
		if (flag)
		{
			SearchForNewTarget();
		}
		else
		{
			_state = endState;
		}
		return flag;
	}

	private void SearchForNewTarget()
	{
		Agent targetAgent = null;
		UsableMachine targetUsableMachine = null;
		WorldFrame targetFrame = WorldFrame.Invalid;
		if (_selectTargetDelegate(base.OwnerAgent, ref targetAgent, ref targetUsableMachine, ref targetFrame))
		{
			if (targetAgent != null)
			{
				_targetAgent = targetAgent;
				_state = State.GoToAgent;
			}
			else if (targetUsableMachine != null)
			{
				_targetUsableMachine = targetUsableMachine;
				_state = State.GoToUsableMachine;
			}
			else
			{
				_targetFrame = targetFrame;
				_state = State.GoToTargetFrame;
			}
		}
	}

	public override float GetAvailability(bool isSimulation)
	{
		return (_state != 0) ? 1 : 0;
	}

	protected override void OnDeactivate()
	{
		base.Navigator.ClearTarget();
		RemoveTargets();
	}

	private void RemoveTargets()
	{
		_targetUsableMachine = null;
		_targetAgent = null;
		_targetFrame = WorldFrame.Invalid;
		_state = State.NoTarget;
		_selectTargetDelegate = null;
		_onTargetReachedDelegate = null;
		_sentToTarget = false;
	}

	public override string GetDebugInfo()
	{
		return "Scripted";
	}
}
