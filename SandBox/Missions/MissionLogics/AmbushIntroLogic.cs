using System;
using SandBox.Missions.AgentControllers;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics;

public class AmbushIntroLogic : MissionLogic
{
	private AmbushMissionController _ambushMission;

	public Action StartIntroAction;

	public override void OnCreated()
	{
		_ambushMission = base.Mission.GetMissionBehavior<AmbushMissionController>();
	}

	public void StartIntro()
	{
		StartIntroAction?.Invoke();
	}

	public void OnIntroEnded()
	{
		_ambushMission.OnIntroductionFinish();
		base.Mission.RemoveMissionBehavior(this);
	}
}
