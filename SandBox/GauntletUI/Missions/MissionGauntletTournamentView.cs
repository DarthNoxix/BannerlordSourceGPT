using SandBox.Tournaments.MissionLogics;
using SandBox.View.Missions.Tournaments;
using SandBox.ViewModelCollection.Tournament;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Missions;

[OverrideView(typeof(MissionTournamentView))]
public class MissionGauntletTournamentView : MissionView
{
	private TournamentBehavior _behavior;

	private Camera _customCamera;

	private bool _viewEnabled = true;

	private IGauntletMovie _gauntletMovie;

	private GauntletLayer _gauntletLayer;

	private TournamentVM _dataSource;

	public MissionGauntletTournamentView()
	{
		base.ViewOrderPriority = 48;
	}

	public override void OnMissionScreenInitialize()
	{
		((MissionView)this).OnMissionScreenInitialize();
		_dataSource = new TournamentVM(DisableUi, _behavior);
		_gauntletLayer = new GauntletLayer(base.ViewOrderPriority);
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		_gauntletLayer.InputRestrictions.SetInputRestrictions();
		_gauntletLayer.IsFocusLayer = true;
		ScreenManager.TrySetFocus(_gauntletLayer);
		_dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		_dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		_gauntletMovie = _gauntletLayer.LoadMovie("Tournament", _dataSource);
		((MissionView)this).MissionScreen.CustomCamera = _customCamera;
		((ScreenBase)(object)((MissionView)this).MissionScreen).AddLayer((ScreenLayer)_gauntletLayer);
	}

	public override void OnMissionScreenFinalize()
	{
		_gauntletLayer.IsFocusLayer = false;
		ScreenManager.TryLoseFocus(_gauntletLayer);
		_gauntletLayer.InputRestrictions.ResetInputRestrictions();
		_gauntletMovie = null;
		_gauntletLayer = null;
		_dataSource.OnFinalize();
		_dataSource = null;
		((MissionView)this).OnMissionScreenFinalize();
	}

	public override void AfterStart()
	{
		_behavior = ((MissionBehavior)this).Mission.GetMissionBehavior<TournamentBehavior>();
		GameEntity gameEntity = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("camera_instance");
		_customCamera = Camera.CreateCamera();
		Vec3 dofParams = default(Vec3);
		gameEntity.GetCameraParamsFromCameraScript(_customCamera, ref dofParams);
	}

	public override void OnMissionTick(float dt)
	{
		if (_behavior == null)
		{
			return;
		}
		if (_gauntletLayer.IsFocusLayer && _dataSource.IsCurrentMatchActive)
		{
			_gauntletLayer.InputRestrictions.ResetInputRestrictions();
			_gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(_gauntletLayer);
		}
		else if (!_gauntletLayer.IsFocusLayer && !_dataSource.IsCurrentMatchActive)
		{
			_gauntletLayer.InputRestrictions.SetInputRestrictions();
			_gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(_gauntletLayer);
		}
		if (_dataSource.IsBetWindowEnabled)
		{
			if (_gauntletLayer.Input.IsHotKeyReleased("Confirm"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_dataSource.ExecuteBet();
				_dataSource.IsBetWindowEnabled = false;
			}
			else if (_gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_dataSource.IsBetWindowEnabled = false;
			}
		}
		if (!_viewEnabled && ((_behavior.LastMatch != null && _behavior.CurrentMatch == null) || _behavior.CurrentMatch.IsReady))
		{
			_dataSource.Refresh();
			ShowUi();
		}
		if (!_viewEnabled && _dataSource.CurrentMatch.IsValid)
		{
			TournamentMatch currentMatch = _behavior.CurrentMatch;
			if (currentMatch != null && currentMatch.State == TournamentMatch.MatchState.Started)
			{
				_dataSource.CurrentMatch.RefreshActiveMatch();
			}
		}
		if (_dataSource.IsOver && _viewEnabled && !((MissionBehavior)this).DebugInput.IsControlDown() && ((MissionBehavior)this).DebugInput.IsHotKeyPressed("ShowHighlightsSummary"))
		{
			((MissionBehavior)this).Mission.GetMissionBehavior<HighlightsController>()?.ShowSummary();
		}
	}

	private void DisableUi()
	{
		if (_viewEnabled)
		{
			((MissionView)this).MissionScreen.UpdateFreeCamera(_customCamera.Frame);
			((MissionView)this).MissionScreen.CustomCamera = null;
			_viewEnabled = false;
			_gauntletLayer.InputRestrictions.ResetInputRestrictions();
		}
	}

	private void ShowUi()
	{
		if (!_viewEnabled)
		{
			((MissionView)this).MissionScreen.CustomCamera = _customCamera;
			_viewEnabled = true;
			_gauntletLayer.InputRestrictions.SetInputRestrictions();
		}
	}

	public override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
	{
		return !_viewEnabled;
	}

	public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
	{
		((MissionBehavior)this).OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
		_dataSource.OnAgentRemoved(affectedAgent);
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
