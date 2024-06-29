using SandBox.View;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI;

[GameStateScreen(typeof(PartyState))]
public class GauntletPartyScreen : ScreenBase, IGameStateListener, IChangeableScreen
{
	private PartyVM _dataSource;

	private GauntletLayer _gauntletLayer;

	private SpriteCategory _partyscreenCategory;

	private readonly PartyState _partyState;

	public GauntletPartyScreen(PartyState partyState)
	{
		_partyState = partyState;
	}

	protected override void OnFrameTick(float dt)
	{
		base.OnFrameTick(dt);
		LoadingWindow.DisableGlobalLoadingWindow();
		_dataSource.IsFiveStackModifierActive = _gauntletLayer.Input.IsHotKeyDown("FiveStackModifier");
		_dataSource.IsEntireStackModifierActive = _gauntletLayer.Input.IsHotKeyDown("EntireStackModifier");
		if (!_partyState.IsActive || _gauntletLayer.Input.IsHotKeyDownAndReleased("Exit") || (!_gauntletLayer.Input.IsControlDown() && _gauntletLayer.Input.IsGameKeyDownAndReleased(43)))
		{
			HandleCancelInput();
		}
		else if (_gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
		{
			HandleDoneInput();
		}
		else if (_gauntletLayer.Input.IsHotKeyDownAndReleased("Reset"))
		{
			HandleResetInput();
		}
		else if (!_dataSource.IsAnyPopUpOpen)
		{
			if (_gauntletLayer.Input.IsHotKeyPressed("TakeAllTroops"))
			{
				if (_dataSource.IsOtherTroopsHaveTransferableTroops)
				{
					UISoundsHelper.PlayUISound("event:/ui/inventory/take_all");
					_dataSource.ExecuteTransferAllOtherTroops();
				}
			}
			else if (_gauntletLayer.Input.IsHotKeyPressed("GiveAllTroops"))
			{
				if (_dataSource.IsMainTroopsHaveTransferableTroops)
				{
					UISoundsHelper.PlayUISound("event:/ui/inventory/take_all");
					_dataSource.ExecuteTransferAllMainTroops();
				}
			}
			else if (_gauntletLayer.Input.IsHotKeyPressed("TakeAllPrisoners"))
			{
				if (_dataSource.CurrentFocusedCharacter != null && Input.IsGamepadActive)
				{
					if (_dataSource.CurrentFocusedCharacter.IsTroopTransferrable && _dataSource.CurrentFocusedCharacter.Side == PartyScreenLogic.PartyRosterSide.Left)
					{
						_dataSource.CurrentFocusedCharacter.ExecuteTransferSingle();
						UISoundsHelper.PlayUISound("event:/ui/transfer");
					}
				}
				else if (_dataSource.IsOtherPrisonersHaveTransferableTroops)
				{
					UISoundsHelper.PlayUISound("event:/ui/inventory/take_all");
					_dataSource.ExecuteTransferAllOtherPrisoners();
				}
			}
			else if (_gauntletLayer.Input.IsHotKeyPressed("GiveAllPrisoners"))
			{
				if (_dataSource.CurrentFocusedCharacter != null && Input.IsGamepadActive)
				{
					if (_dataSource.CurrentFocusedCharacter.IsTroopTransferrable && _dataSource.CurrentFocusedCharacter.Side == PartyScreenLogic.PartyRosterSide.Right)
					{
						_dataSource.CurrentFocusedCharacter.ExecuteTransferSingle();
						UISoundsHelper.PlayUISound("event:/ui/transfer");
					}
				}
				else if (_dataSource.IsMainPrisonersHaveTransferableTroops)
				{
					UISoundsHelper.PlayUISound("event:/ui/inventory/take_all");
					_dataSource.ExecuteTransferAllMainPrisoners();
				}
			}
			else if (_gauntletLayer.Input.IsHotKeyPressed("OpenUpgradePopup"))
			{
				if (!_dataSource.IsUpgradePopUpDisabled)
				{
					_dataSource.ExecuteOpenUpgradePopUp();
					UISoundsHelper.PlayUISound("event:/ui/default");
				}
			}
			else if (_gauntletLayer.Input.IsHotKeyPressed("OpenRecruitPopup"))
			{
				if (!_dataSource.IsRecruitPopUpDisabled)
				{
					_dataSource.ExecuteOpenRecruitPopUp();
					UISoundsHelper.PlayUISound("event:/ui/default");
				}
			}
			else if (_gauntletLayer.Input.IsGameKeyReleased(39) && _dataSource.CurrentFocusedCharacter != null && Input.IsGamepadActive)
			{
				_dataSource.CurrentFocusedCharacter.ExecuteOpenTroopEncyclopedia();
			}
		}
		else if (_gauntletLayer.Input.IsHotKeyPressed("PopupItemPrimaryAction"))
		{
			PartyUpgradeTroopVM upgradePopUp = _dataSource.UpgradePopUp;
			if (upgradePopUp != null && upgradePopUp.IsOpen)
			{
				if (_dataSource.UpgradePopUp.IsFocusedOnACharacter && _dataSource.UpgradePopUp.FocusedTroop.PartyCharacter.Upgrades.Count > 0 && _dataSource.UpgradePopUp.FocusedTroop.PartyCharacter.Upgrades[0].IsAvailable)
				{
					UISoundsHelper.PlayUISound("event:/ui/party/upgrade");
				}
				_dataSource.UpgradePopUp.ExecuteItemPrimaryAction();
			}
			else
			{
				PartyRecruitTroopVM recruitPopUp = _dataSource.RecruitPopUp;
				if (recruitPopUp != null && recruitPopUp.IsOpen)
				{
					_dataSource.RecruitPopUp.ExecuteItemPrimaryAction();
				}
			}
		}
		else if (_gauntletLayer.Input.IsHotKeyDownAndReleased("PopupItemSecondaryAction"))
		{
			PartyUpgradeTroopVM upgradePopUp2 = _dataSource.UpgradePopUp;
			if (upgradePopUp2 != null && upgradePopUp2.IsOpen)
			{
				if (_dataSource.UpgradePopUp.IsFocusedOnACharacter && _dataSource.UpgradePopUp.FocusedTroop.PartyCharacter.Upgrades.Count > 1 && _dataSource.UpgradePopUp.FocusedTroop.PartyCharacter.Upgrades[1].IsAvailable)
				{
					UISoundsHelper.PlayUISound("event:/ui/party/upgrade");
				}
				_dataSource.UpgradePopUp.ExecuteItemSecondaryAction();
				return;
			}
			PartyRecruitTroopVM recruitPopUp2 = _dataSource.RecruitPopUp;
			if (recruitPopUp2 != null && recruitPopUp2.IsOpen)
			{
				PartyTroopManagerItemVM focusedTroop = _dataSource.RecruitPopUp.FocusedTroop;
				if (focusedTroop != null && focusedTroop.PartyCharacter.IsTroopRecruitable)
				{
					UISoundsHelper.PlayUISound("event:/ui/party/recruit_prisoner");
				}
				_dataSource.RecruitPopUp.ExecuteItemSecondaryAction();
			}
		}
		else
		{
			if (!Input.IsGamepadActive || !_gauntletLayer.Input.IsGameKeyReleased(39))
			{
				return;
			}
			PartyRecruitTroopVM recruitPopUp3 = _dataSource.RecruitPopUp;
			if (recruitPopUp3 != null && recruitPopUp3.IsOpen && _dataSource.RecruitPopUp.FocusedTroop != null)
			{
				_dataSource.RecruitPopUp.FocusedTroop.PartyCharacter.ExecuteOpenTroopEncyclopedia();
				return;
			}
			PartyUpgradeTroopVM upgradePopUp3 = _dataSource.UpgradePopUp;
			if (upgradePopUp3 != null && upgradePopUp3.IsOpen)
			{
				if (_dataSource.UpgradePopUp.FocusedTroop != null)
				{
					_dataSource.UpgradePopUp.FocusedTroop.ExecuteOpenTroopEncyclopedia();
				}
				else if (_dataSource.CurrentFocusedUpgrade != null)
				{
					_dataSource.CurrentFocusedUpgrade.ExecuteUpgradeEncyclopediaLink();
				}
			}
		}
	}

	void IGameStateListener.OnActivate()
	{
		base.OnActivate();
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_partyscreenCategory = spriteData.SpriteCategories["ui_partyscreen"];
		_partyscreenCategory.Load(resourceContext, uIResourceDepot);
		_gauntletLayer = new GauntletLayer(1, "GauntletLayer", shouldClear: true);
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("PartyHotKeyCategory"));
		_dataSource = new PartyVM(_partyState.PartyScreenLogic);
		_dataSource.SetGetKeyTextFromKeyIDFunc(Game.Current.GameTextManager.GetHotKeyGameTextFromKeyID);
		_dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
		_dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		_dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		_dataSource.SetTakeAllTroopsInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("TakeAllTroops"));
		_dataSource.SetDismissAllTroopsInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("GiveAllTroops"));
		_dataSource.SetTakeAllPrisonersInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("TakeAllPrisoners"));
		_dataSource.SetDismissAllPrisonersInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("GiveAllPrisoners"));
		_dataSource.SetOpenUpgradePanelInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("OpenUpgradePopup"));
		_dataSource.SetOpenRecruitPanelInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("OpenRecruitPopup"));
		_dataSource.UpgradePopUp.SetPrimaryActionInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("PopupItemPrimaryAction"));
		_dataSource.UpgradePopUp.SetSecondaryActionInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("PopupItemSecondaryAction"));
		_dataSource.RecruitPopUp.SetPrimaryActionInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("PopupItemPrimaryAction"));
		_dataSource.RecruitPopUp.SetSecondaryActionInputKey(HotKeyManager.GetCategory("PartyHotKeyCategory").GetHotKey("PopupItemSecondaryAction"));
		string fiveStackShortcutkeyText = GetFiveStackShortcutkeyText();
		_dataSource.SetFiveStackShortcutKeyText(fiveStackShortcutkeyText);
		string entireStackShortcutkeyText = GetEntireStackShortcutkeyText();
		_dataSource.SetEntireStackShortcutKeyText(entireStackShortcutkeyText);
		_partyState.Handler = _dataSource;
		_gauntletLayer.LoadMovie("PartyScreen", _dataSource);
		AddLayer(_gauntletLayer);
		_gauntletLayer.InputRestrictions.SetInputRestrictions();
		_gauntletLayer.IsFocusLayer = true;
		ScreenManager.TrySetFocus(_gauntletLayer);
		Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.PartyScreen));
		UISoundsHelper.PlayUISound("event:/ui/panels/panel_party_open");
		_gauntletLayer.GamepadNavigationContext.GainNavigationAfterFrames(2, null);
	}

	void IGameStateListener.OnDeactivate()
	{
		base.OnDeactivate();
		PartyBase.MainParty.SetVisualAsDirty();
		_gauntletLayer.IsFocusLayer = false;
		_gauntletLayer.InputRestrictions.ResetInputRestrictions();
		RemoveLayer(_gauntletLayer);
		ScreenManager.TryLoseFocus(_gauntletLayer);
		Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.None));
		if (Campaign.Current.ConversationManager.IsConversationInProgress && !Campaign.Current.ConversationManager.IsConversationFlowActive)
		{
			Campaign.Current.ConversationManager.OnConversationActivate();
		}
	}

	void IGameStateListener.OnInitialize()
	{
		CampaignEvents.CompanionRemoved.AddNonSerializedListener(this, OnCompanionRemoved);
	}

	void IGameStateListener.OnFinalize()
	{
		CampaignEvents.CompanionRemoved.ClearListeners(this);
		_dataSource.OnFinalize();
		_partyscreenCategory.Unload();
		_dataSource = null;
		_gauntletLayer = null;
	}

	protected override void OnResume()
	{
		base.OnResume();
		PartyVM dataSource = _dataSource;
		if (dataSource != null && dataSource.IsInConversation)
		{
			_dataSource.IsInConversation = false;
			if (_dataSource.PartyScreenLogic.IsDoneActive())
			{
				_dataSource.PartyScreenLogic.DoneLogic(isForced: false);
			}
		}
	}

	private void HandleResetInput()
	{
		if (!_dataSource.IsAnyPopUpOpen)
		{
			_dataSource.ExecuteReset();
			UISoundsHelper.PlayUISound("event:/ui/default");
		}
	}

	private void HandleCancelInput()
	{
		PartyUpgradeTroopVM upgradePopUp = _dataSource.UpgradePopUp;
		if (upgradePopUp != null && upgradePopUp.IsOpen)
		{
			_dataSource.UpgradePopUp.ExecuteCancel();
		}
		else
		{
			PartyRecruitTroopVM recruitPopUp = _dataSource.RecruitPopUp;
			if (recruitPopUp != null && recruitPopUp.IsOpen)
			{
				_dataSource.RecruitPopUp.ExecuteCancel();
			}
			else
			{
				_dataSource.ExecuteCancel();
			}
		}
		UISoundsHelper.PlayUISound("event:/ui/default");
	}

	private void HandleDoneInput()
	{
		PartyUpgradeTroopVM upgradePopUp = _dataSource.UpgradePopUp;
		if (upgradePopUp != null && upgradePopUp.IsOpen)
		{
			_dataSource.UpgradePopUp.ExecuteDone();
		}
		else
		{
			PartyRecruitTroopVM recruitPopUp = _dataSource.RecruitPopUp;
			if (recruitPopUp != null && recruitPopUp.IsOpen)
			{
				_dataSource.RecruitPopUp.ExecuteDone();
			}
			else
			{
				_dataSource.ExecuteDone();
			}
		}
		UISoundsHelper.PlayUISound("event:/ui/default");
	}

	private void OnCompanionRemoved(Hero arg1, RemoveCompanionAction.RemoveCompanionDetail arg2)
	{
		((IChangeableScreen)this).ApplyChanges();
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

	bool IChangeableScreen.AnyUnsavedChanges()
	{
		return _partyState.PartyScreenLogic.IsThereAnyChanges();
	}

	bool IChangeableScreen.CanChangesBeApplied()
	{
		return _partyState.PartyScreenLogic.IsDoneActive();
	}

	void IChangeableScreen.ApplyChanges()
	{
		_partyState.PartyScreenLogic.DoneLogic(isForced: true);
	}

	void IChangeableScreen.ResetChanges()
	{
		_partyState.PartyScreenLogic.Reset(fromCancel: true);
	}
}
