using SandBox.AI;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.Usables;

public class PatrolArea : UsableMachine
{
	public int AreaIndex;

	private int _activeIndex;

	private int ActiveIndex
	{
		get
		{
			return _activeIndex;
		}
		set
		{
			if (_activeIndex != value)
			{
				base.StandingPoints[value].IsDeactivated = false;
				base.StandingPoints[_activeIndex].IsDeactivated = true;
				_activeIndex = value;
			}
		}
	}

	public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
	{
		return base.PilotStandingPoint?.ActionMessage;
	}

	public override string GetDescriptionText(GameEntity gameEntity = null)
	{
		return base.PilotStandingPoint?.DescriptionMessage.ToString();
	}

	public override UsableMachineAIBase CreateAIBehaviorObject()
	{
		return new UsablePlaceAI(this);
	}

	protected override void OnInit()
	{
		base.OnInit();
		foreach (StandingPoint standingPoint in base.StandingPoints)
		{
			standingPoint.IsDeactivated = true;
		}
		ActiveIndex = base.StandingPoints.Count - 1;
		SetScriptComponentToTick(GetTickRequirement());
	}

	public override TickRequirement GetTickRequirement()
	{
		return TickRequirement.Tick | base.GetTickRequirement();
	}

	protected override void OnTick(float dt)
	{
		base.OnTick(dt);
		if (base.StandingPoints[ActiveIndex].HasAIUser)
		{
			ActiveIndex = ((ActiveIndex == 0) ? (base.StandingPoints.Count - 1) : (ActiveIndex - 1));
		}
	}
}
