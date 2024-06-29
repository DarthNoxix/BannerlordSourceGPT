using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI;

[GameStateScreen(typeof(KingdomState))]
public class GauntletKingdomScreen : ScreenBase, IGameStateListener
{
	private GauntletLayer _gauntletLayer;

	private readonly KingdomState _kingdomState;

	private GauntletLayer _armyManagementLayer;

	private ArmyManagementVM _armyManagementDatasource;

	private SpriteCategory _kingdomCategory;

	private SpriteCategory _armyManagementCategory;

	public KingdomManagementVM DataSource { get; private set; }

	public bool IsMakingDecision => DataSource.Decision.IsActive;

	public GauntletKingdomScreen(KingdomState kingdomState)
	{
		_kingdomState = kingdomState;
	}

	protected override void OnFrameTick(float dt)
	{
		base.OnFrameTick(dt);
		LoadingWindow.DisableGlobalLoadingWindow();
		DataSource.CanSwitchTabs = !InformationManager.GetIsAnyTooltipActiveAndExtended();
		if (DataSource.Decision.IsActive)
		{
			if (_gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
			{
				DataSource.Decision.CurrentDecision?.ExecuteFinalSelection();
			}
		}
		else if (_armyManagementDatasource != null)
		{
			if (_armyManagementLayer.Input.IsHotKeyDownAndReleased("Exit"))
			{
				_armyManagementDatasource.ExecuteCancel();
				UISoundsHelper.PlayUISound("event:/ui/default");
			}
			else if (_armyManagementLayer.Input.IsHotKeyDownAndReleased("Confirm"))
			{
				_armyManagementDatasource.ExecuteDone();
				UISoundsHelper.PlayUISound("event:/ui/default");
			}
			else if (_armyManagementLayer.Input.IsHotKeyDownAndReleased("Reset"))
			{
				_armyManagementDatasource.ExecuteReset();
				UISoundsHelper.PlayUISound("event:/ui/default");
			}
			else if (_armyManagementLayer.Input.IsHotKeyReleased("RemoveParty") && _armyManagementDatasource.FocusedItem != null)
			{
				_armyManagementDatasource.FocusedItem.ExecuteAction();
				UISoundsHelper.PlayUISound("event:/ui/default");
			}
		}
		else if (_gauntletLayer.Input.IsHotKeyReleased("Exit") || _gauntletLayer.Input.IsGameKeyPressed(40) || _gauntletLayer.Input.IsHotKeyReleased("Confirm"))
		{
			CloseKingdomScreen();
		}
		else if (DataSource.CanSwitchTabs)
		{
			if (_gauntletLayer.Input.IsHotKeyReleased("SwitchToPreviousTab"))
			{
				DataSource.SelectPreviousCategory();
				UISoundsHelper.PlayUISound("event:/ui/tab");
			}
			else if (_gauntletLayer.Input.IsHotKeyReleased("SwitchToNextTab"))
			{
				DataSource.SelectNextCategory();
				UISoundsHelper.PlayUISound("event:/ui/tab");
			}
		}
		DataSource?.OnFrameTick();
	}

	void IGameStateListener.OnActivate()
	{
		base.OnActivate();
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_kingdomCategory = spriteData.SpriteCategories["ui_kingdom"];
		_kingdomCategory.Load(resourceContext, uIResourceDepot);
		_gauntletLayer = new GauntletLayer(1, "GauntletLayer", shouldClear: true);
		_gauntletLayer.InputRestrictions.SetInputRestrictions();
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
		_gauntletLayer.IsFocusLayer = true;
		ScreenManager.TrySetFocus(_gauntletLayer);
		AddLayer(_gauntletLayer);
		DataSource = new KingdomManagementVM(CloseKingdomScreen, OpenArmyManagement, ShowArmyOnMap);
		DataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		DataSource.SetPreviousTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
		DataSource.SetNextTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
		if (_kingdomState.InitialSelectedDecision != null)
		{
			DataSource.Decision.HandleDecision(_kingdomState.InitialSelectedDecision);
		}
		else if (_kingdomState.InitialSelectedArmy != null)
		{
			DataSource.SelectArmy(_kingdomState.InitialSelectedArmy);
		}
		else if (_kingdomState.InitialSelectedSettlement != null)
		{
			DataSource.SelectSettlement(_kingdomState.InitialSelectedSettlement);
		}
		else if (_kingdomState.InitialSelectedClan != null)
		{
			DataSource.SelectClan(_kingdomState.InitialSelectedClan);
		}
		else if (_kingdomState.InitialSelectedPolicy != null)
		{
			DataSource.SelectPolicy(_kingdomState.InitialSelectedPolicy);
		}
		else if (_kingdomState.InitialSelectedKingdom != null)
		{
			DataSource.SelectKingdom(_kingdomState.InitialSelectedKingdom);
		}
		_gauntletLayer.LoadMovie("KingdomManagement", DataSource);
		Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.KingdomScreen));
		UISoundsHelper.PlayUISound("event:/ui/panels/panel_kingdom_open");
		_gauntletLayer.GamepadNavigationContext.GainNavigationAfterFrames(2, null);
	}

	void IGameStateListener.OnDeactivate()
	{
		base.OnDeactivate();
		RemoveLayer(_gauntletLayer);
		_gauntletLayer.IsFocusLayer = false;
		ScreenManager.TryLoseFocus(_gauntletLayer);
		Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.None));
	}

	void IGameStateListener.OnInitialize()
	{
	}

	void IGameStateListener.OnFinalize()
	{
		_kingdomCategory.Unload();
		DataSource.OnFinalize();
		DataSource = null;
		_gauntletLayer = null;
	}

	private void ShowArmyOnMap(Army army)
	{
		Vec2 position2D = army.LeaderParty.Position2D;
		CloseKingdomScreen();
		MapScreen.Instance.FastMoveCameraToPosition(position2D);
	}

	private void OpenArmyManagement()
	{
		if (_gauntletLayer != null)
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
			_armyManagementLayer = new GauntletLayer(2);
			_armyManagementLayer.LoadMovie("ArmyManagement", _armyManagementDatasource);
			_armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			_armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			_armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory"));
			_armyManagementLayer.InputRestrictions.SetInputRestrictions();
			_armyManagementLayer.IsFocusLayer = true;
			AddLayer(_armyManagementLayer);
			ScreenManager.TrySetFocus(_armyManagementLayer);
		}
	}

	private void CloseArmyManagement()
	{
		if (_armyManagementLayer != null)
		{
			_armyManagementLayer.InputRestrictions.ResetInputRestrictions();
			_armyManagementLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(_armyManagementLayer);
			RemoveLayer(_armyManagementLayer);
			_armyManagementLayer = null;
		}
		if (_armyManagementDatasource != null)
		{
			_armyManagementDatasource.OnFinalize();
			_armyManagementDatasource = null;
		}
		if (_armyManagementCategory != null)
		{
			_armyManagementCategory.Unload();
			_armyManagementCategory = null;
		}
		Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.KingdomScreen));
		DataSource.OnRefresh();
	}

	private void CloseKingdomScreen()
	{
		Game.Current.GameStateManager.PopState();
		UISoundsHelper.PlayUISound("event:/ui/default");
	}
}
