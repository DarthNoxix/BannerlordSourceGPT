using System;
using SandBox.Missions.AgentControllers;
using SandBox.Missions.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions;

public class MissionAmbushIntroView : MissionView
{
	private AmbushMissionController _ambushMissionController;

	private AmbushIntroLogic _ambushIntroLogic;

	private bool _isPlayerAmbusher;

	private MatrixFrame _cameraStart;

	private MatrixFrame _cameraEnd;

	private float _cameraMoveSpeed = 0.1f;

	private float _cameraLerping;

	private Camera _camera;

	public Action IntroEndAction;

	private bool _started;

	private bool _firstTick = true;

	public override void AfterStart()
	{
		((MissionBehavior)this).AfterStart();
		_ambushMissionController = ((MissionBehavior)this).Mission.GetMissionBehavior<AmbushMissionController>();
		_ambushIntroLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<AmbushIntroLogic>();
		_isPlayerAmbusher = _ambushMissionController.IsPlayerAmbusher;
		_cameraStart = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag(_isPlayerAmbusher ? "intro_camera_attacker_start" : "intro_camera_defender_start").GetGlobalFrame();
		_cameraEnd = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag(_isPlayerAmbusher ? "intro_camera_attacker_end" : "intro_camera_defender_end").GetGlobalFrame();
		IntroEndAction = _ambushIntroLogic.OnIntroEnded;
		_ambushIntroLogic.StartIntroAction = StartIntro;
	}

	public void StartIntro()
	{
		_started = true;
		_camera = Camera.CreateCamera();
		_camera.FillParametersFrom(((MissionView)this).MissionScreen.CombatCamera);
		_camera.Frame = _cameraStart;
		((MissionView)this).MissionScreen.CustomCamera = _camera;
	}

	public override void OnMissionTick(float dt)
	{
		if (_firstTick)
		{
			_firstTick = false;
		}
		if (_started)
		{
			if (_cameraLerping < 1f)
			{
				MatrixFrame frame = default(MatrixFrame);
				frame.origin = MBMath.Lerp(_cameraStart.origin, _cameraEnd.origin, _cameraLerping, 1E-05f);
				frame.rotation = MBMath.Lerp(ref _cameraStart.rotation, ref _cameraEnd.rotation, _cameraLerping, 1E-05f);
				_camera.Frame = frame;
				_cameraLerping += _cameraMoveSpeed * dt;
			}
			else
			{
				_camera.Frame = _cameraEnd;
				CleanUp();
			}
		}
	}

	private void CleanUp()
	{
		((MissionView)this).MissionScreen.CustomCamera = null;
		IntroEndAction();
		((MissionBehavior)this).Mission.RemoveMissionBehavior((MissionBehavior)(object)this);
	}
}
