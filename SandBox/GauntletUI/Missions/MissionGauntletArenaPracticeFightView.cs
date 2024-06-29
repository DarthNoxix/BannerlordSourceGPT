using SandBox.Missions.MissionLogics.Arena;
using SandBox.View.Missions;
using SandBox.ViewModelCollection.Missions;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Missions;

[OverrideView(typeof(MissionArenaPracticeFightView))]
public class MissionGauntletArenaPracticeFightView : MissionView
{
	private MissionArenaPracticeFightVM _dataSource;

	private GauntletLayer _gauntletLayer;

	private IGauntletMovie _movie;

	public override void OnMissionScreenInitialize()
	{
		((MissionView)this).OnMissionScreenInitialize();
		ArenaPracticeFightMissionController missionBehavior = ((MissionBehavior)this).Mission.GetMissionBehavior<ArenaPracticeFightMissionController>();
		_dataSource = new MissionArenaPracticeFightVM(missionBehavior);
		_gauntletLayer = new GauntletLayer(base.ViewOrderPriority);
		_movie = _gauntletLayer.LoadMovie("ArenaPracticeFight", _dataSource);
		((ScreenBase)(object)((MissionView)this).MissionScreen).AddLayer((ScreenLayer)_gauntletLayer);
	}

	public override void OnMissionScreenTick(float dt)
	{
		((MissionView)this).OnMissionScreenTick(dt);
		_dataSource.Tick();
	}

	public override void OnMissionScreenFinalize()
	{
		_dataSource.OnFinalize();
		_gauntletLayer.ReleaseMovie(_movie);
		((ScreenBase)(object)((MissionView)this).MissionScreen).RemoveLayer((ScreenLayer)_gauntletLayer);
		((MissionView)this).OnMissionScreenFinalize();
	}

	public override void OnPhotoModeActivated()
	{
		((MissionView)this).OnPhotoModeActivated();
		_gauntletLayer.UIContext.ContextAlpha = 0f;
	}

	public override void OnPhotoModeDeactivated()
	{
		((MissionView)this).OnPhotoModeDeactivated();
		_gauntletLayer.UIContext.ContextAlpha = 1f;
	}
}
