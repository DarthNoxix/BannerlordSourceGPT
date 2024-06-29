using SandBox.AI;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.Usables;

public class UsablePlace : UsableMachine
{
	public override string GetDescriptionText(GameEntity gameEntity = null)
	{
		return base.PilotStandingPoint?.DescriptionMessage.ToString();
	}

	public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
	{
		return base.PilotStandingPoint?.ActionMessage;
	}

	public override UsableMachineAIBase CreateAIBehaviorObject()
	{
		return new UsablePlaceAI(this);
	}
}
