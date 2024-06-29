using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;
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

[GameStateScreen(typeof(ClanState))]
public class GauntletClanScreen : ScreenBase, IGameStateListener
{
	private ClanManagementVM _dataSource;

	private GauntletLayer _gauntletLayer;

	private SpriteCategory _clanCategory;

	private readonly ClanState _clanState;

	private bool _isCreatingPartyWithMembers;

	public GauntletClanScreen(ClanState clanState)
	{
		_clanState = clanState;
	}

	protected override void OnFrameTick(float dt)
	{
		base.OnFrameTick(dt);
		LoadingWindow.DisableGlobalLoadingWindow();
		_dataSource.CanSwitchTabs = !Input.IsGamepadActive || (!InformationManager.GetIsAnyTooltipActiveAndExtended() && _gauntletLayer.IsHitThisFrame);
		ClanManagementVM dataSource = _dataSource;
		if (dataSource != null && dataSource.CardSelectionPopup?.IsVisible == true)
		{
			if (_gauntletLayer.Input.IsHotKeyReleased("Confirm"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_dataSource.CardSelectionPopup.ExecuteDone();
			}
			else if (_gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_dataSource.CardSelectionPopup.ExecuteCancel();
			}
		}
		else if (_gauntletLayer.Input.IsHotKeyReleased("Exit") || _gauntletLayer.Input.IsGameKeyPressed(41) || _gauntletLayer.Input.IsHotKeyReleased("Confirm"))
		{
			CloseClanScreen();
		}
		else if (_dataSource.CanSwitchTabs)
		{
			if (_gauntletLayer.Input.IsHotKeyReleased("SwitchToPreviousTab"))
			{
				UISoundsHelper.PlayUISound("event:/ui/tab");
				_dataSource.SelectPreviousCategory();
			}
			else if (_gauntletLayer.Input.IsHotKeyReleased("SwitchToNextTab"))
			{
				UISoundsHelper.PlayUISound("event:/ui/tab");
				_dataSource.SelectNextCategory();
			}
		}
	}

	private void OpenPartyScreenForNewClanParty(Hero hero)
	{
		_isCreatingPartyWithMembers = true;
		PartyScreenManager.OpenScreenAsCreateClanPartyForHero(hero);
	}

	private void OpenBannerEditorWithPlayerClan()
	{
		Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<BannerEditorState>());
	}

	void IGameStateListener.OnActivate()
	{
		base.OnActivate();
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_clanCategory = spriteData.SpriteCategories["ui_clan"];
		_clanCategory.Load(resourceContext, uIResourceDepot);
		_gauntletLayer = new GauntletLayer(1, "GauntletLayer", shouldClear: true);
		_gauntletLayer.InputRestrictions.SetInputRestrictions();
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
		_gauntletLayer.IsFocusLayer = true;
		ScreenManager.TrySetFocus(_gauntletLayer);
		AddLayer(_gauntletLayer);
		_dataSource = new ClanManagementVM(CloseClanScreen, ShowHeroOnMap, OpenPartyScreenForNewClanParty, OpenBannerEditorWithPlayerClan);
		_dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		_dataSource.SetPreviousTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
		_dataSource.SetNextTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
		if (_isCreatingPartyWithMembers)
		{
			_dataSource.SelectParty(PartyBase.MainParty);
			_isCreatingPartyWithMembers = false;
		}
		else if (_clanState.InitialSelectedHero != null)
		{
			_dataSource.SelectHero(_clanState.InitialSelectedHero);
		}
		else if (_clanState.InitialSelectedParty != null)
		{
			_dataSource.SelectParty(_clanState.InitialSelectedParty);
			if (_clanState.InitialSelectedParty.LeaderHero == null)
			{
				ClanPartiesVM clanParties = _dataSource.ClanParties;
				if (clanParties != null && clanParties.CurrentSelectedParty?.IsChangeLeaderEnabled == true)
				{
					_dataSource.ClanParties.OnShowChangeLeaderPopup();
				}
			}
		}
		else if (_clanState.InitialSelectedSettlement != null)
		{
			_dataSource.SelectSettlement(_clanState.InitialSelectedSettlement);
		}
		else if (_clanState.InitialSelectedWorkshop != null)
		{
			_dataSource.SelectWorkshop(_clanState.InitialSelectedWorkshop);
		}
		else if (_clanState.InitialSelectedAlley != null)
		{
			_dataSource.SelectAlley(_clanState.InitialSelectedAlley);
		}
		_gauntletLayer.LoadMovie("ClanScreen", _dataSource);
		Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.ClanScreen));
		UISoundsHelper.PlayUISound("event:/ui/panels/panel_clan_open");
		_gauntletLayer.GamepadNavigationContext.GainNavigationAfterFrames(2, null);
	}

	private void ShowHeroOnMap(Hero hero)
	{
		Vec2 asVec = hero.GetPosition().AsVec2;
		CloseClanScreen();
		MapScreen.Instance.FastMoveCameraToPosition(asVec);
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
		_clanCategory.Unload();
		_dataSource.OnFinalize();
		_dataSource = null;
		_gauntletLayer = null;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		_dataSource?.UpdateBannerVisuals();
		_dataSource?.RefreshCategoryValues();
	}

	private void CloseClanScreen()
	{
		Game.Current.GameStateManager.PopState();
		UISoundsHelper.PlayUISound("event:/ui/default");
	}
}
