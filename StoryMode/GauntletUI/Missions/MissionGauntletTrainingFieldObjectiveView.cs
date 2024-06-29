using System;
using StoryMode.Missions;
using StoryMode.View.Missions;
using StoryMode.ViewModelCollection.Missions;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace StoryMode.GauntletUI.Missions;

[OverrideView(typeof(MissionTrainingFieldObjectiveView))]
public class MissionGauntletTrainingFieldObjectiveView : MissionView
{
	private TrainingFieldObjectivesVM _dataSource;

	private GauntletLayer _layer;

	private float _beginningTime;

	private bool _isTimerActive;

	public override void OnMissionScreenInitialize()
	{
		((MissionView)this).OnMissionScreenInitialize();
		TrainingFieldMissionController missionBehavior = ((MissionBehavior)this).Mission.GetMissionBehavior<TrainingFieldMissionController>();
		_dataSource = new TrainingFieldObjectivesVM();
		_dataSource.UpdateCurrentObjectiveText(missionBehavior.InitialCurrentObjective);
		_layer = new GauntletLayer(2);
		_layer.LoadMovie("TrainingFieldObjectives", _dataSource);
		((ScreenBase)(object)((MissionView)this).MissionScreen).AddLayer((ScreenLayer)_layer);
		missionBehavior.TimerTick = _dataSource.UpdateTimerText;
		missionBehavior.CurrentObjectiveTick = _dataSource.UpdateCurrentObjectiveText;
		missionBehavior.AllObjectivesTick = _dataSource.UpdateObjectivesWith;
		missionBehavior.UIStartTimer = BeginTimer;
		missionBehavior.UIEndTimer = EndTimer;
		missionBehavior.CurrentMouseObjectiveTick = _dataSource.UpdateCurrentMouseObjective;
		Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(OnGamepadActiveStateChanged));
	}

	public override void OnMissionScreenTick(float dt)
	{
		((MissionView)this).OnMissionScreenTick(dt);
		if (_isTimerActive)
		{
			_dataSource.UpdateTimerText((((MissionBehavior)this).Mission.CurrentTime - _beginningTime).ToString("0.0"));
		}
	}

	private void BeginTimer()
	{
		_isTimerActive = true;
		_beginningTime = ((MissionBehavior)this).Mission.CurrentTime;
	}

	private float EndTimer()
	{
		_isTimerActive = false;
		_dataSource.UpdateTimerText("");
		return ((MissionBehavior)this).Mission.CurrentTime - _beginningTime;
	}

	public override void OnMissionScreenFinalize()
	{
		((MissionView)this).OnMissionScreenFinalize();
		((ScreenBase)(object)((MissionView)this).MissionScreen).RemoveLayer((ScreenLayer)_layer);
		_dataSource = null;
		_layer = null;
		Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(OnGamepadActiveStateChanged));
	}

	public override void OnPhotoModeActivated()
	{
		((MissionView)this).OnPhotoModeActivated();
		_layer.UIContext.ContextAlpha = 0f;
	}

	public override void OnPhotoModeDeactivated()
	{
		((MissionView)this).OnPhotoModeDeactivated();
		_layer.UIContext.ContextAlpha = 1f;
	}

	private void OnGamepadActiveStateChanged()
	{
		_dataSource?.UpdateIsGamepadActive();
	}
}
