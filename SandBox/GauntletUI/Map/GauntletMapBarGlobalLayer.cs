using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Map;

public class GauntletMapBarGlobalLayer : GlobalLayer
{
	private MapBarVM _mapDataSource;

	private GauntletLayer _gauntletLayer;

	private IGauntletMovie _movie;

	private SpriteCategory _mapBarCategory;

	private MapScreen _mapScreen;

	private MapNavigationHandler _mapNavigationHandler;

	private INavigationHandler _mapNavigationHandlerAsInterface;

	private MapEncyclopediaView _encyclopediaManager;

	private float _contextAlphaTarget = 1f;

	private float _contextAlphaModifider;

	private GauntletLayer _armyManagementLayer;

	private SpriteCategory _armyManagementCategory;

	private ArmyManagementVM _armyManagementVM;

	private IGauntletMovie _gauntletArmyManagementMovie;

	private CampaignTimeControlMode _timeControlModeBeforeArmyManagementOpened;

	public void Initialize(MapScreen mapScreen, float contextAlphaModifider)
	{
		_mapScreen = mapScreen;
		_contextAlphaModifider = contextAlphaModifider;
		_mapNavigationHandler = new MapNavigationHandler();
		_mapNavigationHandlerAsInterface = _mapNavigationHandler;
		_mapDataSource = new MapBarVM(_mapNavigationHandler, _mapScreen, GetMapBarShortcuts, OpenArmyManagement);
		_gauntletLayer = new GauntletLayer(202);
		base.Layer = _gauntletLayer;
		SpriteData spriteData = UIResourceManager.SpriteData;
		_mapBarCategory = spriteData.SpriteCategories["ui_mapbar"];
		_mapBarCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
		_movie = _gauntletLayer.LoadMovie("MapBar", _mapDataSource);
		_encyclopediaManager = mapScreen.EncyclopediaScreenManager;
	}

	public void OnFinalize()
	{
		_armyManagementVM?.OnFinalize();
		_mapDataSource.OnFinalize();
		_gauntletArmyManagementMovie?.Release();
		_movie.Release();
		_mapBarCategory.Unload();
		_armyManagementVM = null;
		_gauntletLayer = null;
		_mapDataSource = null;
		_encyclopediaManager = null;
		_mapScreen = null;
	}

	public void Refresh()
	{
		_mapDataSource?.OnRefresh();
	}

	private MapBarShortcuts GetMapBarShortcuts()
	{
		MapBarShortcuts result = default(MapBarShortcuts);
		result.EscapeMenuHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericPanelGameKeyCategory", "ToggleEscapeMenu").ToString();
		result.CharacterHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 37).ToString();
		result.QuestHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 42).ToString();
		result.PartyHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 43).ToString();
		result.KingdomHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 40).ToString();
		result.ClanHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 41).ToString();
		result.InventoryHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 38).ToString();
		result.FastForwardHotkey = Game.Current.GameTextManager.GetHotKeyGameText("MapHotKeyCategory", 61).ToString();
		result.PauseHotkey = Game.Current.GameTextManager.GetHotKeyGameText("MapHotKeyCategory", 59).ToString();
		result.PlayHotkey = Game.Current.GameTextManager.GetHotKeyGameText("MapHotKeyCategory", 60).ToString();
		return result;
	}

	protected override void OnTick(float dt)
	{
		base.OnTick(dt);
		_gauntletLayer.UIContext.ContextAlpha = MathF.Lerp(_gauntletLayer.UIContext.ContextAlpha, _contextAlphaTarget, dt * _contextAlphaModifider);
		GameState activeState = Game.Current.GameStateManager.ActiveState;
		ScreenBase topScreen = ScreenManager.TopScreen;
		PanelScreenStatus screenStatus = new PanelScreenStatus(topScreen);
		if (_mapNavigationHandler != null)
		{
			_mapNavigationHandler.IsNavigationLocked = screenStatus.IsCurrentScreenLocksNavigation;
		}
		if (topScreen is MapScreen || screenStatus.IsAnyPanelScreenOpen)
		{
			_mapDataSource.IsEnabled = true;
			_mapDataSource.CurrentScreen = topScreen.GetType().Name;
			bool flag = ScreenManager.TopScreen is MapScreen;
			_mapDataSource.MapTimeControl.IsInMap = flag;
			base.Layer.InputRestrictions.SetInputRestrictions(isMouseVisible: false);
			if (!(activeState is MapState))
			{
				_mapDataSource.MapTimeControl.IsCenterPanelEnabled = false;
				if (screenStatus.IsAnyPanelScreenOpen)
				{
					HandlePanelSwitching(screenStatus);
				}
			}
			else
			{
				_ = (MapState)activeState;
				if (flag)
				{
					MapScreen mapScreen = ScreenManager.TopScreen as MapScreen;
					mapScreen.SetIsBarExtended(_mapDataSource.MapInfo.IsInfoBarExtended);
					_mapDataSource.MapTimeControl.IsInRecruitment = mapScreen.IsInRecruitment;
					_mapDataSource.MapTimeControl.IsInBattleSimulation = mapScreen.IsInBattleSimulation;
					_mapDataSource.MapTimeControl.IsEncyclopediaOpen = _encyclopediaManager.IsEncyclopediaOpen;
					_mapDataSource.MapTimeControl.IsInArmyManagement = mapScreen.IsInArmyManagement;
					_mapDataSource.MapTimeControl.IsInTownManagement = mapScreen.IsInTownManagement;
					_mapDataSource.MapTimeControl.IsInHideoutTroopManage = mapScreen.IsInHideoutTroopManage;
					_mapDataSource.MapTimeControl.IsInCampaignOptions = mapScreen.IsInCampaignOptions;
					_mapDataSource.MapTimeControl.IsEscapeMenuOpened = mapScreen.IsEscapeMenuOpened;
					_mapDataSource.MapTimeControl.IsMarriageOfferPopupActive = mapScreen.IsMarriageOfferPopupActive;
					_mapDataSource.MapTimeControl.IsMapCheatsActive = mapScreen.IsMapCheatsActive;
					if (_armyManagementVM != null)
					{
						HandleArmyManagementInput();
					}
				}
				else
				{
					_mapDataSource.MapTimeControl.IsCenterPanelEnabled = false;
				}
			}
			_mapDataSource.Tick(dt);
		}
		else
		{
			_mapDataSource.IsEnabled = false;
			base.Layer.InputRestrictions.ResetInputRestrictions();
		}
	}

	private void HandleArmyManagementInput()
	{
		if (_armyManagementLayer.Input.IsHotKeyReleased("Exit"))
		{
			UISoundsHelper.PlayUISound("event:/ui/default");
			_armyManagementVM.ExecuteCancel();
		}
		else if (_armyManagementLayer.Input.IsHotKeyReleased("Confirm"))
		{
			UISoundsHelper.PlayUISound("event:/ui/default");
			_armyManagementVM.ExecuteDone();
		}
		else if (_armyManagementLayer.Input.IsHotKeyReleased("Reset"))
		{
			UISoundsHelper.PlayUISound("event:/ui/default");
			_armyManagementVM.ExecuteReset();
		}
		else if (_armyManagementLayer.Input.IsHotKeyReleased("RemoveParty") && _armyManagementVM.FocusedItem != null)
		{
			UISoundsHelper.PlayUISound("event:/ui/default");
			_armyManagementVM.FocusedItem.ExecuteAction();
		}
	}

	private void HandlePanelSwitching(PanelScreenStatus screenStatus)
	{
		GauntletLayer gauntletLayer = ScreenManager.TopScreen.FindLayer<GauntletLayer>();
		if (gauntletLayer?.Input != null && !gauntletLayer.IsFocusedOnInput())
		{
			InputContext input = gauntletLayer.Input;
			if (input.IsGameKeyReleased(37) && !screenStatus.IsCharacterScreenOpen)
			{
				_mapNavigationHandlerAsInterface?.OpenCharacterDeveloper();
			}
			else if (input.IsGameKeyReleased(43) && !screenStatus.IsPartyScreenOpen)
			{
				_mapNavigationHandlerAsInterface?.OpenParty();
			}
			else if (input.IsGameKeyReleased(42) && !screenStatus.IsQuestsScreenOpen)
			{
				_mapNavigationHandlerAsInterface?.OpenQuests();
			}
			else if (input.IsGameKeyReleased(38) && !screenStatus.IsInventoryScreenOpen)
			{
				_mapNavigationHandlerAsInterface?.OpenInventory();
			}
			else if (input.IsGameKeyReleased(41) && !screenStatus.IsClanScreenOpen)
			{
				_mapNavigationHandlerAsInterface?.OpenClan();
			}
			else if (input.IsGameKeyReleased(40) && !screenStatus.IsKingdomScreenOpen)
			{
				_mapNavigationHandlerAsInterface?.OpenKingdom();
			}
		}
	}

	private void OpenArmyManagement()
	{
		if (_gauntletLayer != null)
		{
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
			_armyManagementLayer = new GauntletLayer(300);
			_armyManagementCategory = spriteData.SpriteCategories["ui_armymanagement"];
			_armyManagementCategory.Load(resourceContext, uIResourceDepot);
			_armyManagementVM = new ArmyManagementVM(CloseArmyManagement);
			_gauntletArmyManagementMovie = _armyManagementLayer.LoadMovie("ArmyManagement", _armyManagementVM);
			_armyManagementLayer.InputRestrictions.SetInputRestrictions();
			_armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
			_armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			_armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			_armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory"));
			_armyManagementLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(_armyManagementLayer);
			_mapScreen.AddLayer(_armyManagementLayer);
			_armyManagementVM.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			_armyManagementVM.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			_armyManagementVM.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			_armyManagementVM.SetRemoveInputKey(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory").GetHotKey("RemoveParty"));
			_timeControlModeBeforeArmyManagementOpened = Campaign.Current.TimeControlMode;
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			Campaign.Current.SetTimeControlModeLock(isLocked: true);
			if (ScreenManager.TopScreen is MapScreen mapScreen)
			{
				mapScreen.SetIsInArmyManagement(isInArmyManagement: true);
			}
		}
	}

	private void CloseArmyManagement()
	{
		_armyManagementVM.OnFinalize();
		_armyManagementLayer.ReleaseMovie(_gauntletArmyManagementMovie);
		_mapScreen.RemoveLayer(_armyManagementLayer);
		_armyManagementCategory.Unload();
		Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.MapWindow));
		_gauntletArmyManagementMovie = null;
		_armyManagementVM = null;
		_armyManagementLayer = null;
		Campaign.Current.SetTimeControlModeLock(isLocked: false);
		Campaign.Current.TimeControlMode = _timeControlModeBeforeArmyManagementOpened;
		if (ScreenManager.TopScreen is MapScreen mapScreen)
		{
			mapScreen.SetIsInArmyManagement(isInArmyManagement: false);
		}
	}

	internal bool IsEscaped()
	{
		if (_armyManagementVM != null)
		{
			_armyManagementVM.ExecuteCancel();
			return true;
		}
		return false;
	}

	internal void OnMapConversationStart()
	{
		_contextAlphaTarget = 0f;
	}

	internal void OnMapConversationEnd()
	{
		_contextAlphaTarget = 1f;
	}
}
