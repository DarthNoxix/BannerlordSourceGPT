using SandBox;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;

namespace StoryMode.Missions;

[MissionManager]
public static class StoryModeMissions
{
	[MissionMethod]
	public static Mission OpenTrainingFieldMission(string scene, Location location, CharacterObject talkToChar = null, string sceneLevels = null)
	{
		return MissionState.OpenNew("TrainingField", SandBoxMissions.CreateSandBoxTrainingMissionInitializerRecord(scene, sceneLevels, false), (Mission mission) => new MissionBehavior[23]
		{
			new MissionOptionsComponent(),
			(MissionBehavior)new CampaignMissionComponent(),
			(MissionBehavior)new MissionBasicTeamLogic(),
			new TrainingFieldMissionController(),
			new BasicLeaveMissionLogic(),
			(MissionBehavior)new LeaveMissionLogic(),
			(MissionBehavior)new MissionAgentLookHandler(),
			(MissionBehavior)new SandBoxMissionHandler(),
			(MissionBehavior)new MissionConversationLogic(talkToChar),
			(MissionBehavior)new MissionFightHandler(),
			(MissionBehavior)new MissionAgentHandler(location, talkToChar, (string)null),
			(MissionBehavior)new MissionAlleyHandler(),
			(MissionBehavior)new HeroSkillHandler(),
			new MissionFacialAnimationHandler(),
			new MissionAgentPanicHandler(),
			(MissionBehavior)new BattleAgentLogic(),
			new AgentHumanAILogic(),
			(MissionBehavior)new MissionCrimeHandler(),
			new MissionHardBorderPlacer(),
			new MissionBoundaryPlacer(),
			new MissionBoundaryCrossingHandler(),
			(MissionBehavior)new VisualTrackerMissionBehavior(),
			new EquipmentControllerLeaveLogic()
		});
	}
}
