using SandBox.View;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI;

[GameStateScreen(typeof(InventoryState))]
public class GauntletInventoryScreen : ScreenBase, IInventoryStateHandler, IGameStateListener, IChangeableScreen
{
	private IGauntletMovie _gauntletMovie;

	private SPInventoryVM _dataSource;

	private GauntletLayer _gauntletLayer;

	private bool _closed;

	private bool _openedFromMission;

	private SpriteCategory _inventoryCategory;

	public InventoryState InventoryState { get; private set; }

	protected override void OnFrameTick(float dt)
	{
		base.OnFrameTick(dt);
		if (!_closed)
		{
			LoadingWindow.DisableGlobalLoadingWindow();
		}
		_dataSource.IsFiveStackModifierActive = _gauntletLayer.Input.IsHotKeyDown("FiveStackModifier");
		_dataSource.IsEntireStackModifierActive = _gauntletLayer.Input.IsHotKeyDown("EntireStackModifier");
		if (_dataSource.IsSearchAvailable && _gauntletLayer.IsFocusedOnInput())
		{
			return;
		}
		if (_gauntletLayer.Input.IsHotKeyReleased("SwitchAlternative") && _dataSource != null)
		{
			_dataSource.CompareNextItem();
		}
		else if (_gauntletLayer.Input.IsHotKeyDownAndReleased("Exit") || _gauntletLayer.Input.IsGameKeyDownAndReleased(38))
		{
			ExecuteCancel();
		}
		else if (_gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
		{
			ExecuteConfirm();
		}
		else if (_gauntletLayer.Input.IsHotKeyDownAndReleased("Reset"))
		{
			HandleResetInput();
		}
		else if (_gauntletLayer.Input.IsHotKeyPressed("SwitchToPreviousTab"))
		{
			if (_dataSource.IsFocusedOnItemList && Input.IsGamepadActive)
			{
				if (_dataSource.CurrentFocusedItem != null && _dataSource.CurrentFocusedItem.IsTransferable && _dataSource.CurrentFocusedItem.InventorySide == InventoryLogic.InventorySide.OtherInventory)
				{
					ExecuteBuySingle();
				}
			}
			else
			{
				ExecuteSwitchToPreviousTab();
			}
		}
		else if (_gauntletLayer.Input.IsHotKeyPressed("SwitchToNextTab"))
		{
			if (_dataSource.IsFocusedOnItemList && Input.IsGamepadActive)
			{
				if (_dataSource.CurrentFocusedItem != null && _dataSource.CurrentFocusedItem.IsTransferable && _dataSource.CurrentFocusedItem.InventorySide == InventoryLogic.InventorySide.PlayerInventory)
				{
					ExecuteSellSingle();
				}
			}
			else
			{
				ExecuteSwitchToNextTab();
			}
		}
		else if (_gauntletLayer.Input.IsHotKeyPressed("TakeAll"))
		{
			ExecuteTakeAll();
		}
		else if (_gauntletLayer.Input.IsHotKeyPressed("GiveAll"))
		{
			ExecuteGiveAll();
		}
	}

	public GauntletInventoryScreen(InventoryState inventoryState)
	{
		InventoryState = inventoryState;
		InventoryState.Handler = this;
	}

	protected override void OnInitialize()
	{
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_inventoryCategory = spriteData.SpriteCategories["ui_inventory"];
		_inventoryCategory.Load(resourceContext, uIResourceDepot);
		_dataSource = new SPInventoryVM(InventoryState.InventoryLogic, Mission.Current?.DoesMissionRequireCivilianEquipment ?? false, GetItemUsageSetFlag, GetFiveStackShortcutkeyText(), GetEntireStackShortcutkeyText());
		_dataSource.SetGetKeyTextFromKeyIDFunc(Game.Current.GameTextManager.GetHotKeyGameTextFromKeyID);
		_dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
		_dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		_dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		_dataSource.SetPreviousCharacterInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
		_dataSource.SetNextCharacterInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
		_dataSource.SetBuyAllInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("TakeAll"));
		_dataSource.SetSellAllInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("GiveAll"));
		_gauntletLayer = new GauntletLayer(15, "GauntletLayer", shouldClear: true)
		{
			IsFocusLayer = true
		};
		_gauntletLayer.InputRestrictions.SetInputRestrictions();
		AddLayer(_gauntletLayer);
		ScreenManager.TrySetFocus(_gauntletLayer);
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("InventoryHotKeyCategory"));
		_gauntletMovie = _gauntletLayer.LoadMovie("Inventory", _dataSource);
		_openedFromMission = InventoryState.Predecessor is MissionState;
		InformationManager.ClearAllMessages();
		UISoundsHelper.PlayUISound("event:/ui/panels/panel_inventory_open");
		_gauntletLayer.GamepadNavigationContext.GainNavigationAfterFrames(2, null);
	}

	private string GetFiveStackShortcutkeyText()
	{
		if (!Input.IsControllerConnected || Input.IsMouseActive)
		{
			return Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", "anyshift").ToString();
		}
		return string.Empty;
	}

	private string GetEntireStackShortcutkeyText()
	{
		if (!Input.IsControllerConnected || Input.IsMouseActive)
		{
			return Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", "anycontrol").ToString();
		}
		return null;
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		_closed = true;
		MBInformationManager.HideInformations();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		_dataSource?.RefreshCallbacks();
		if (_gauntletLayer != null)
		{
			ScreenManager.TrySetFocus(_gauntletLayer);
		}
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		_gauntletMovie = null;
		_inventoryCategory.Unload();
		_dataSource.OnFinalize();
		_dataSource = null;
		_gauntletLayer = null;
	}

	void IGameStateListener.OnActivate()
	{
		Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.InventoryScreen));
	}

	void IGameStateListener.OnDeactivate()
	{
		Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.None));
	}

	void IGameStateListener.OnInitialize()
	{
	}

	void IGameStateListener.OnFinalize()
	{
	}

	void IInventoryStateHandler.FilterInventoryAtOpening(InventoryManager.InventoryCategoryType inventoryCategoryType)
	{
		if (_dataSource == null)
		{
			Debug.FailedAssert("Data source is not initialized when filtering inventory", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.GauntletUI\\GauntletInventoryScreen.cs", "FilterInventoryAtOpening", 234);
			return;
		}
		switch (inventoryCategoryType)
		{
		case InventoryManager.InventoryCategoryType.Weapon:
			_dataSource.ExecuteFilterWeapons();
			break;
		case InventoryManager.InventoryCategoryType.Armors:
			_dataSource.ExecuteFilterArmors();
			break;
		case InventoryManager.InventoryCategoryType.HorseCategory:
			_dataSource.ExecuteFilterMounts();
			break;
		case InventoryManager.InventoryCategoryType.Goods:
			_dataSource.ExecuteFilterMisc();
			break;
		case InventoryManager.InventoryCategoryType.Shield:
			break;
		}
	}

	public void ExecuteLootingScript()
	{
		_dataSource.ExecuteBuyAllItems();
	}

	public void ExecuteSellAllLoot()
	{
		_dataSource.ExecuteSellAllItems();
	}

	private void HandleResetInput()
	{
		if (!_dataSource.ItemPreview.IsSelected)
		{
			_dataSource.ExecuteResetTranstactions();
			UISoundsHelper.PlayUISound("event:/ui/default");
		}
	}

	public void ExecuteCancel()
	{
		if (_dataSource.ItemPreview.IsSelected)
		{
			UISoundsHelper.PlayUISound("event:/ui/default");
			_dataSource.ClosePreview();
		}
		else if (_dataSource.IsExtendedEquipmentControlsEnabled)
		{
			_dataSource.IsExtendedEquipmentControlsEnabled = false;
		}
		else
		{
			UISoundsHelper.PlayUISound("event:/ui/default");
			_dataSource.ExecuteResetAndCompleteTranstactions();
		}
	}

	public void ExecuteConfirm()
	{
		if (!_dataSource.ItemPreview.IsSelected && !_dataSource.IsDoneDisabled)
		{
			_dataSource.ExecuteCompleteTranstactions();
			UISoundsHelper.PlayUISound("event:/ui/default");
		}
	}

	public void ExecuteSwitchToPreviousTab()
	{
		if (!_dataSource.ItemPreview.IsSelected)
		{
			MBBindingList<InventoryCharacterSelectorItemVM> itemList = _dataSource.CharacterList.ItemList;
			if (itemList != null && itemList.Count > 1)
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
			}
			_dataSource.CharacterList.ExecuteSelectPreviousItem();
		}
	}

	public void ExecuteSwitchToNextTab()
	{
		if (!_dataSource.ItemPreview.IsSelected)
		{
			MBBindingList<InventoryCharacterSelectorItemVM> itemList = _dataSource.CharacterList.ItemList;
			if (itemList != null && itemList.Count > 1)
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
			}
			_dataSource.CharacterList.ExecuteSelectNextItem();
		}
	}

	public void ExecuteBuySingle()
	{
		_dataSource.CurrentFocusedItem.ExecuteBuySingle();
		UISoundsHelper.PlayUISound("event:/ui/transfer");
	}

	public void ExecuteSellSingle()
	{
		_dataSource.CurrentFocusedItem.ExecuteSellSingle();
		UISoundsHelper.PlayUISound("event:/ui/transfer");
	}

	public void ExecuteTakeAll()
	{
		if (!_dataSource.ItemPreview.IsSelected)
		{
			_dataSource.ExecuteBuyAllItems();
			UISoundsHelper.PlayUISound("event:/ui/inventory/take_all");
		}
	}

	public void ExecuteGiveAll()
	{
		if (!_dataSource.ItemPreview.IsSelected)
		{
			_dataSource.ExecuteSellAllItems();
			UISoundsHelper.PlayUISound("event:/ui/inventory/take_all");
		}
	}

	public void ExecuteBuyConsumableItem()
	{
		_dataSource.ExecuteBuyItemTest();
	}

	private ItemObject.ItemUsageSetFlags GetItemUsageSetFlag(WeaponComponentData item)
	{
		if (!string.IsNullOrEmpty(item.ItemUsage))
		{
			return MBItem.GetItemUsageSetFlags(item.ItemUsage);
		}
		return (ItemObject.ItemUsageSetFlags)0;
	}

	private void CloseInventoryScreen()
	{
		InventoryManager.Instance.CloseInventoryPresentation(fromCancel: false);
	}

	bool IChangeableScreen.AnyUnsavedChanges()
	{
		return InventoryState.InventoryLogic.IsThereAnyChanges();
	}

	bool IChangeableScreen.CanChangesBeApplied()
	{
		return InventoryState.InventoryLogic.CanPlayerCompleteTransaction();
	}

	void IChangeableScreen.ApplyChanges()
	{
		_dataSource.ItemPreview.Close();
		InventoryState.InventoryLogic.DoneLogic();
	}

	void IChangeableScreen.ResetChanges()
	{
		InventoryState.InventoryLogic.Reset(fromCancel: true);
	}
}
