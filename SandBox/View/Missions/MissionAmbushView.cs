using SandBox.Missions.AgentControllers;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.SiegeWeapon;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;

namespace SandBox.View.Missions;

public class MissionAmbushView : MissionView
{
	private AmbushMissionController _ambushMissionController;

	private MissionDeploymentBoundaryMarker _deploymentBoundaryMarkerHandler;

	private MissionAmbushDeploymentView _ambushDeploymentView;

	private bool _firstTick = true;

	public override void AfterStart()
	{
		((MissionBehavior)this).AfterStart();
		_ambushMissionController = ((MissionBehavior)this).Mission.GetMissionBehavior<AmbushMissionController>();
		_deploymentBoundaryMarkerHandler = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionDeploymentBoundaryMarker>();
		_ambushMissionController.PlayerDeploymentFinish += OnPlayerDeploymentFinish;
		_ambushMissionController.IntroFinish += OnIntroFinish;
	}

	public override void OnMissionTick(float dt)
	{
		if (_firstTick)
		{
			_firstTick = false;
			if (_ambushMissionController.IsPlayerAmbusher)
			{
				_ambushDeploymentView = new MissionAmbushDeploymentView();
				((MissionView)this).MissionScreen.AddMissionView((MissionView)(object)_ambushDeploymentView);
				((MissionBehavior)(object)_ambushDeploymentView).OnBehaviorInitialize();
				((MissionBehavior)(object)_ambushDeploymentView).EarlyStart();
				((MissionBehavior)(object)_ambushDeploymentView).AfterStart();
			}
		}
	}

	public void OnIntroFinish()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		if (_deploymentBoundaryMarkerHandler != null)
		{
			((MissionBehavior)this).Mission.RemoveMissionBehavior((MissionBehavior)(object)_deploymentBoundaryMarkerHandler);
		}
		((MissionView)this).MissionScreen.AddMissionView(ViewCreator.CreateMissionAgentStatusUIHandler((Mission)null));
		((MissionView)this).MissionScreen.AddMissionView(ViewCreator.CreateMissionMainAgentEquipmentController((Mission)null));
		((MissionView)this).MissionScreen.AddMissionView(ViewCreator.CreateMissionMainAgentCheerBarkControllerView((Mission)null));
		((MissionView)this).MissionScreen.AddMissionView(ViewCreator.CreateMissionAgentLockVisualizerView((Mission)null));
		((MissionView)this).MissionScreen.AddMissionView(ViewCreator.CreateMissionBoundaryCrossingView());
		((MissionView)this).MissionScreen.AddMissionView((MissionView)new MissionBoundaryWallView());
		((MissionView)this).MissionScreen.AddMissionView((MissionView)new MissionMainAgentController());
		((MissionView)this).MissionScreen.AddMissionView((MissionView)new MissionCrosshair());
		((MissionView)this).MissionScreen.AddMissionView((MissionView)new RangedSiegeWeaponViewController());
	}

	public void OnPlayerDeploymentFinish()
	{
		if (_ambushMissionController.IsPlayerAmbusher)
		{
			((MissionBehavior)this).Mission.RemoveMissionBehavior((MissionBehavior)(object)_ambushDeploymentView);
		}
	}
}
