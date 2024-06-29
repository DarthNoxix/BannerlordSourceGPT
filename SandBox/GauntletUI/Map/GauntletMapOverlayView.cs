using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Map;

[OverrideView(typeof(MapOverlayView))]
public class GauntletMapOverlayView : MapView
{
	private GauntletLayer _layerAsGauntletLayer;

	private GameMenuOverlay _overlayDataSource;

	private readonly GameOverlays.MapOverlayType _type;

	private IGauntletMovie _movie;

	private bool _isContextMenuEnabled;

	private GauntletLayer _armyManagementLayer;

	private SpriteCategory _armyManagementCategory;

	private ArmyManagementVM _armyManagementDatasource;

	private IGauntletMovie _gauntletArmyManagementMovie;

	private CampaignTimeControlMode _timeControlModeBeforeArmyManagementOpened;

	public GauntletMapOverlayView(GameOverlays.MapOverlayType type)
	{
		_type = type;
	}

	protected override void CreateLayout()
	{
		base.CreateLayout();
		_overlayDataSource = GetOverlay(_type);
		base.Layer = new GauntletLayer(201);
		_layerAsGauntletLayer = base.Layer as GauntletLayer;
		base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		base.Layer.InputRestrictions.SetInputRestrictions(isMouseVisible: false);
		GameOverlays.MapOverlayType type = _type;
		if (type == GameOverlays.MapOverlayType.Army)
		{
			_movie = _layerAsGauntletLayer.LoadMovie("ArmyOverlay", _overlayDataSource);
			(_overlayDataSource as ArmyMenuOverlayVM).OpenArmyManagement = OpenArmyManagement;
		}
		else
		{
			Debug.FailedAssert("This kind of overlay not supported in gauntlet", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.GauntletUI\\Map\\GauntletMapOverlayView.cs", "CreateLayout", 63);
		}
		base.MapScreen.AddLayer(base.Layer);
	}

	public GameMenuOverlay GetOverlay(GameOverlays.MapOverlayType mapOverlayType)
	{
		if (mapOverlayType == GameOverlays.MapOverlayType.Army)
		{
			return new ArmyMenuOverlayVM();
		}
		Debug.FailedAssert("Game menu overlay: " + mapOverlayType.ToString() + " could not be found", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.GauntletUI\\Map\\GauntletMapOverlayView.cs", "GetOverlay", 76);
		return null;
	}

	protected override void OnArmyLeft()
	{
		base.MapScreen.RemoveArmyOverlay();
	}

	protected override void OnFinalize()
	{
		if (_armyManagementLayer != null)
		{
			CloseArmyManagement();
		}
		_layerAsGauntletLayer.ReleaseMovie(_movie);
		if (_gauntletArmyManagementMovie != null)
		{
			_layerAsGauntletLayer.ReleaseMovie(_gauntletArmyManagementMovie);
		}
		base.MapScreen.RemoveLayer(base.Layer);
		_movie = null;
		_overlayDataSource = null;
		base.Layer = null;
		_layerAsGauntletLayer = null;
		_armyManagementCategory?.Unload();
		base.OnFinalize();
	}

	protected override void OnHourlyTick()
	{
		base.OnHourlyTick();
		_overlayDataSource?.HourlyTick();
	}

	protected override void OnFrameTick(float dt)
	{
		base.OnFrameTick(dt);
		if (ScreenManager.TopScreen is MapScreen mapScreen && _overlayDataSource != null)
		{
			_overlayDataSource.IsInfoBarExtended = mapScreen.IsBarExtended;
		}
		_overlayDataSource?.OnFrameTick(dt);
	}

	protected override bool IsEscaped()
	{
		if (_armyManagementDatasource != null)
		{
			_armyManagementDatasource.ExecuteCancel();
			return true;
		}
		return false;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		_overlayDataSource?.Refresh();
	}

	protected override void OnResume()
	{
		base.OnResume();
		_overlayDataSource?.Refresh();
	}

	protected override void OnMapScreenUpdate(float dt)
	{
		base.OnMapScreenUpdate(dt);
		if (!_isContextMenuEnabled && _overlayDataSource.IsContextMenuEnabled)
		{
			_isContextMenuEnabled = true;
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(base.Layer);
		}
		else if (_isContextMenuEnabled && !_overlayDataSource.IsContextMenuEnabled)
		{
			_isContextMenuEnabled = false;
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
		}
		if (_isContextMenuEnabled && base.Layer.Input.IsHotKeyReleased("Exit"))
		{
			UISoundsHelper.PlayUISound("event:/ui/default");
			_overlayDataSource.IsContextMenuEnabled = false;
		}
		HandleArmyManagementInput();
	}

	protected override void OnMenuModeTick(float dt)
	{
		base.OnMenuModeTick(dt);
		if (ScreenManager.TopScreen is MapScreen mapScreen && _overlayDataSource != null)
		{
			_overlayDataSource.IsInfoBarExtended = mapScreen.IsBarExtended;
		}
		_overlayDataSource?.OnFrameTick(dt);
	}

	private void OpenArmyManagement()
	{
		_armyManagementDatasource = new ArmyManagementVM(CloseArmyManagement);
		_armyManagementDatasource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		_armyManagementDatasource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		_armyManagementDatasource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
		_armyManagementDatasource.SetRemoveInputKey(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory").GetHotKey("RemoveParty"));
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_armyManagementCategory = spriteData.SpriteCategories["ui_armymanagement"];
		_armyManagementCategory.Load(resourceContext, uIResourceDepot);
		_armyManagementLayer = new GauntletLayer(300);
		_gauntletArmyManagementMovie = _armyManagementLayer.LoadMovie("ArmyManagement", _armyManagementDatasource);
		_armyManagementLayer.InputRestrictions.SetInputRestrictions();
		_armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		_armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory"));
		_armyManagementLayer.IsFocusLayer = true;
		ScreenManager.TrySetFocus(_armyManagementLayer);
		base.MapScreen.AddLayer(_armyManagementLayer);
		_timeControlModeBeforeArmyManagementOpened = Campaign.Current.TimeControlMode;
		Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
		Campaign.Current.SetTimeControlModeLock(isLocked: true);
		if (ScreenManager.TopScreen is MapScreen mapScreen)
		{
			mapScreen.SetIsInArmyManagement(isInArmyManagement: true);
		}
	}

	private void CloseArmyManagement()
	{
		if (_armyManagementLayer != null && _gauntletArmyManagementMovie != null)
		{
			_armyManagementLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(_armyManagementLayer);
			_armyManagementLayer.InputRestrictions.ResetInputRestrictions();
			_armyManagementLayer.ReleaseMovie(_gauntletArmyManagementMovie);
			base.MapScreen.RemoveLayer(_armyManagementLayer);
		}
		_armyManagementDatasource?.OnFinalize();
		_gauntletArmyManagementMovie = null;
		_armyManagementDatasource = null;
		_armyManagementLayer = null;
		_overlayDataSource?.Refresh();
		if (ScreenManager.TopScreen is MapScreen mapScreen)
		{
			mapScreen.SetIsInArmyManagement(isInArmyManagement: false);
		}
		Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.MapWindow));
		Campaign.Current.SetTimeControlModeLock(isLocked: false);
		Campaign.Current.TimeControlMode = _timeControlModeBeforeArmyManagementOpened;
	}

	private void HandleArmyManagementInput()
	{
		if (_armyManagementLayer != null && _armyManagementDatasource != null)
		{
			if (_armyManagementLayer.Input.IsHotKeyReleased("Exit"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_armyManagementDatasource.ExecuteCancel();
			}
			else if (_armyManagementLayer.Input.IsHotKeyReleased("Confirm"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_armyManagementDatasource.ExecuteDone();
			}
			else if (_armyManagementLayer.Input.IsHotKeyReleased("Reset"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_armyManagementDatasource.ExecuteReset();
			}
			else if (_armyManagementLayer.Input.IsHotKeyReleased("RemoveParty") && _armyManagementDatasource.FocusedItem != null)
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_armyManagementDatasource.FocusedItem.ExecuteAction();
			}
		}
	}
}
