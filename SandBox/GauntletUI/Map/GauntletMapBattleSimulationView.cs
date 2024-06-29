using SandBox.View.Map;
using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map;

[OverrideView(typeof(BattleSimulationMapView))]
public class GauntletMapBattleSimulationView : MapView
{
	private GauntletLayer _layerAsGauntletLayer;

	private IGauntletMovie _gauntletMovie;

	private SPScoreboardVM _dataSource;

	private readonly BattleSimulation _battleSimulation;

	public GauntletMapBattleSimulationView(BattleSimulation battleSimulation)
	{
		_battleSimulation = battleSimulation;
	}

	protected override void CreateLayout()
	{
		base.CreateLayout();
		_dataSource = new SPScoreboardVM(_battleSimulation);
		_dataSource.Initialize(null, null, base.MapState.EndBattleSimulation, null);
		_dataSource.SetShortcuts(new ScoreboardHotkeys
		{
			ShowMouseHotkey = null,
			ShowScoreboardHotkey = null,
			DoneInputKey = HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"),
			FastForwardKey = HotKeyManager.GetCategory("ScoreboardHotKeyCategory").GetHotKey("ToggleFastForward")
		});
		base.Layer = new GauntletLayer(101);
		_layerAsGauntletLayer = base.Layer as GauntletLayer;
		base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ScoreboardHotKeyCategory"));
		_gauntletMovie = _layerAsGauntletLayer.LoadMovie("SPScoreboard", _dataSource);
		_dataSource.ExecutePlayAction();
		base.Layer.IsFocusLayer = true;
		base.Layer.InputRestrictions.SetInputRestrictions();
		base.MapScreen.AddLayer(base.Layer);
		ScreenManager.TrySetFocus(base.Layer);
	}

	protected override void OnFinalize()
	{
		_dataSource.OnFinalize();
		base.MapScreen.RemoveLayer(base.Layer);
		base.Layer.IsFocusLayer = false;
		base.Layer.InputRestrictions.ResetInputRestrictions();
		ScreenManager.TryLoseFocus(base.Layer);
		_layerAsGauntletLayer = null;
		base.Layer = null;
		_gauntletMovie = null;
	}

	protected override void OnFrameTick(float dt)
	{
		base.OnFrameTick(dt);
		if (_dataSource != null && base.Layer != null)
		{
			if (!_dataSource.IsOver && base.Layer.Input.IsHotKeyReleased("ToggleFastForward"))
			{
				_dataSource.IsFastForwarding = !_dataSource.IsFastForwarding;
				_dataSource.ExecuteFastForwardAction();
			}
			else if (_dataSource.IsOver && _dataSource.ShowScoreboard && base.Layer.Input.IsHotKeyPressed("Confirm"))
			{
				_dataSource.ExecuteQuitAction();
			}
		}
	}
}
