using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics;

public class SandBoxSallyOutMissionController : SallyOutMissionController
{
	private MapEvent _mapEvent;

	public override void OnBehaviorInitialize()
	{
		base.OnBehaviorInitialize();
		_mapEvent = MapEvent.PlayerMapEvent;
	}

	protected override void GetInitialTroopCounts(out int besiegedTotalTroopCount, out int besiegerTotalTroopCount)
	{
		besiegedTotalTroopCount = _mapEvent.GetNumberOfInvolvedMen(BattleSideEnum.Defender);
		besiegerTotalTroopCount = _mapEvent.GetNumberOfInvolvedMen(BattleSideEnum.Attacker);
	}
}
