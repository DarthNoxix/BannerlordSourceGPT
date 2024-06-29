using System;
using System.Collections.Generic;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.ViewModelCollection.Barter;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapConversation;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Map;

[OverrideView(typeof(MapConversationView))]
public class GauntletMapConversationView : MapConversationView, IConversationStateHandler
{
	private enum ConversationStates
	{
		OnInstall,
		OnUninstall,
		OnActivate,
		OnDeactivate,
		OnContinue,
		ExecuteContinue
	}

	private GauntletLayer _layerAsGauntletLayer;

	private MapConversationVM _dataSource;

	private SpriteCategory _conversationCategory;

	private MapConversationTableauData _tableauData;

	private bool _isBarterActive;

	private Queue<ConversationStates> _conversationStateQueue;

	private ConversationCharacterData? _playerCharacterData;

	private ConversationCharacterData? _conversationPartnerData;

	private bool _isConversationInstalled;

	private BarterManager _barter;

	private SpriteCategory _barterCategory;

	private BarterVM _barterDataSource;

	private IGauntletMovie _barterMovie;

	public GauntletMapConversationView(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData)
	{
		_conversationStateQueue = new Queue<ConversationStates>();
		_playerCharacterData = playerCharacterData;
		_conversationPartnerData = conversationPartnerData;
		_barter = Campaign.Current.BarterManager;
		BarterManager barter = _barter;
		barter.BarterBegin = (BarterManager.BarterBeginEventDelegate)Delegate.Combine(barter.BarterBegin, new BarterManager.BarterBeginEventDelegate(OnBarterBegin));
		BarterManager barter2 = _barter;
		barter2.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Combine(barter2.Closed, new BarterManager.BarterCloseEventDelegate(OnBarterClosed));
	}

	private void OnBarterClosed()
	{
		_layerAsGauntletLayer.ReleaseMovie(_barterMovie);
		_barterCategory.Unload();
		_barterDataSource = null;
		_isBarterActive = false;
		_dataSource.IsBarterActive = false;
		BarterItemVM.IsFiveStackModifierActive = false;
		BarterItemVM.IsEntireStackModifierActive = false;
	}

	private void OnBarterBegin(BarterData args)
	{
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_barterCategory = spriteData.SpriteCategories["ui_barter"];
		_barterCategory.Load(resourceContext, uIResourceDepot);
		_barterDataSource = new BarterVM(args);
		_barterDataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
		_barterDataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		_barterDataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		_barterMovie = _layerAsGauntletLayer.LoadMovie("BarterScreen", _barterDataSource);
		_isBarterActive = true;
		_dataSource.IsBarterActive = true;
	}

	protected override void CreateLayout()
	{
		base.CreateLayout();
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_conversationCategory = spriteData.SpriteCategories["ui_conversation"];
		_conversationCategory.Load(resourceContext, uIResourceDepot);
		Campaign.Current.ConversationManager.Handler = this;
		Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
	}

	private void OnContinue()
	{
		MapConversationVM dataSource = _dataSource;
		if (dataSource != null && dataSource.DialogController?.AnswerList.Count <= 0 && !_isBarterActive && _isConversationInstalled)
		{
			((IConversationStateHandler)this).ExecuteConversationContinue();
		}
	}

	protected override void OnFrameTick(float dt)
	{
		base.OnFrameTick(dt);
		Tick();
	}

	protected override void OnIdleTick(float dt)
	{
		base.OnIdleTick(dt);
		Tick();
	}

	protected override bool IsEscaped()
	{
		return !_isConversationInstalled;
	}

	protected override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
	{
		return true;
	}

	private void OnAfterConversationInstalled()
	{
		if (_conversationStateQueue.Count > 0)
		{
			ConversationStates conversationStates = _conversationStateQueue.Peek();
			if (conversationStates == ConversationStates.OnActivate || conversationStates == ConversationStates.OnContinue)
			{
				ProcessConversationState(_conversationStateQueue.Dequeue());
				OnAfterConversationInstalled();
			}
		}
	}

	private void Tick()
	{
		if (_conversationStateQueue.Count > 0)
		{
			ConversationStates state = _conversationStateQueue.Dequeue();
			ProcessConversationState(state);
		}
		if (_isConversationInstalled && ScreenManager.TopScreen == base.MapScreen && ScreenManager.FocusedLayer != base.Layer)
		{
			ScreenManager.TrySetFocus(base.Layer);
		}
		MapConversationVM dataSource = _dataSource;
		if (dataSource != null && dataSource.DialogController?.AnswerList.Count <= 0 && !_isBarterActive && _isConversationInstalled && IsReleasedInGauntletLayer("ContinueKey"))
		{
			UISoundsHelper.PlayUISound("event:/ui/default");
			((IConversationStateHandler)this).ExecuteConversationContinue();
		}
		if (_barterDataSource != null)
		{
			if (IsReleasedInGauntletLayer("Exit"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_barterDataSource.ExecuteCancel();
			}
			else
			{
				if (IsReleasedInGauntletLayer("Confirm"))
				{
					BarterVM barterDataSource = _barterDataSource;
					if (barterDataSource != null && !barterDataSource.IsOfferDisabled)
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						_barterDataSource.ExecuteOffer();
						goto IL_0195;
					}
				}
				if (IsReleasedInGauntletLayer("Reset"))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					_barterDataSource.ExecuteReset();
				}
			}
		}
		else if (IsReleasedInGauntletLayer("ToggleEscapeMenu"))
		{
			MapScreen mapScreen = base.MapScreen;
			if (mapScreen != null && mapScreen.IsEscapeMenuOpened)
			{
				base.MapScreen.CloseEscapeMenu();
			}
			else
			{
				base.MapScreen?.OpenEscapeMenu();
			}
		}
		goto IL_0195;
		IL_0195:
		BarterItemVM.IsFiveStackModifierActive = IsDownInGauntletLayer("FiveStackModifier");
		BarterItemVM.IsEntireStackModifierActive = IsDownInGauntletLayer("EntireStackModifier");
	}

	protected override void OnMenuModeTick(float dt)
	{
		base.OnMenuModeTick(dt);
		Tick();
	}

	protected override void OnMapConversationUpdate(ConversationCharacterData playerConversationData, ConversationCharacterData partnerConversationData)
	{
		base.OnMapConversationUpdate(playerConversationData, partnerConversationData);
		float timeOfDay = CampaignTime.Now.CurrentHourInDay * 1f;
		MapWeatherModel.WeatherEvent weatherEventInPosition = Campaign.Current.Models.MapWeatherModel.GetWeatherEventInPosition(MobileParty.MainParty.Position2D);
		bool isCurrentTerrainUnderSnow = weatherEventInPosition == MapWeatherModel.WeatherEvent.Snowy || weatherEventInPosition == MapWeatherModel.WeatherEvent.Blizzard;
		bool isInside = false;
		if (partnerConversationData.Character.HeroObject != null)
		{
			string text = LocationComplex.Current?.GetLocationOfCharacter(partnerConversationData.Character.HeroObject)?.StringId;
			isInside = Hero.MainHero.CurrentSettlement != null && (text == "lordshall" || text == "tavern");
		}
		MapConversationTableauData mapConversationTableauData = MapConversationTableauData.CreateFrom(playerConversationData, partnerConversationData, Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace), timeOfDay, isCurrentTerrainUnderSnow, Hero.MainHero.CurrentSettlement, isInside, weatherEventInPosition == MapWeatherModel.WeatherEvent.HeavyRain, weatherEventInPosition == MapWeatherModel.WeatherEvent.Blizzard);
		if (!IsSame(mapConversationTableauData, _tableauData))
		{
			_dataSource.TableauData = mapConversationTableauData;
		}
	}

	private bool IsReleasedInGauntletLayer(string hotKeyID)
	{
		return base.Layer?.Input.IsHotKeyReleased(hotKeyID) ?? false;
	}

	private bool IsDownInGauntletLayer(string hotKeyID)
	{
		return base.Layer?.Input.IsHotKeyDown(hotKeyID) ?? false;
	}

	private void OnClose()
	{
		Game.Current.GameStateManager.LastOrDefault<MapState>()?.OnMapConversationOver();
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		base.Layer.IsFocusLayer = false;
		ScreenManager.TryLoseFocus(base.Layer);
		BarterManager barter = _barter;
		barter.BarterBegin = (BarterManager.BarterBeginEventDelegate)Delegate.Remove(barter.BarterBegin, new BarterManager.BarterBeginEventDelegate(OnBarterBegin));
		BarterManager barter2 = _barter;
		barter2.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Remove(barter2.Closed, new BarterManager.BarterCloseEventDelegate(OnBarterClosed));
		_dataSource.OnFinalize();
		_barterDataSource?.OnFinalize();
		base.MapScreen.RemoveLayer(base.Layer);
		_conversationCategory?.Unload();
		base.Layer = null;
		_barterMovie = null;
		_dataSource = null;
		_barterDataSource = null;
		Campaign.Current.ConversationManager.Handler = null;
		Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
	}

	private string GetContinueKeyText()
	{
		if (Input.IsGamepadActive)
		{
			GameTexts.SetVariable("CONSOLE_KEY_NAME", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("ConversationHotKeyCategory", "ContinueKey")));
			return GameTexts.FindText("str_click_to_continue_console").ToString();
		}
		return GameTexts.FindText("str_click_to_continue").ToString();
	}

	private void ProcessConversationState(ConversationStates state)
	{
		switch (state)
		{
		case ConversationStates.OnInstall:
			CreateConversationTableau();
			OnAfterConversationInstalled();
			break;
		case ConversationStates.OnUninstall:
			UninstallConversation();
			break;
		case ConversationStates.OnDeactivate:
			MBInformationManager.HideInformations();
			break;
		case ConversationStates.OnContinue:
			_dataSource.DialogController.OnConversationContinue();
			break;
		case ConversationStates.ExecuteContinue:
			_dataSource.DialogController.ExecuteContinue();
			break;
		case ConversationStates.OnActivate:
			break;
		}
	}

	private void CreateConversationTableau()
	{
		float timeOfDay = CampaignTime.Now.CurrentHourInDay * 1f;
		MapWeatherModel.WeatherEvent weatherEventInPosition = Campaign.Current.Models.MapWeatherModel.GetWeatherEventInPosition(MobileParty.MainParty.Position2D);
		bool isCurrentTerrainUnderSnow = weatherEventInPosition == MapWeatherModel.WeatherEvent.Snowy || weatherEventInPosition == MapWeatherModel.WeatherEvent.Blizzard;
		bool isInside = false;
		if (_conversationPartnerData.Value.Character.HeroObject != null)
		{
			string text = LocationComplex.Current?.GetLocationOfCharacter(_conversationPartnerData.Value.Character.HeroObject)?.StringId;
			isInside = Hero.MainHero.CurrentSettlement != null && (text == "lordshall" || text == "tavern");
		}
		_tableauData = MapConversationTableauData.CreateFrom(_playerCharacterData.Value, _conversationPartnerData.Value, Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace), timeOfDay, isCurrentTerrainUnderSnow, Hero.MainHero.CurrentSettlement, isInside, weatherEventInPosition == MapWeatherModel.WeatherEvent.HeavyRain, weatherEventInPosition == MapWeatherModel.WeatherEvent.Blizzard);
		_dataSource.TableauData = _tableauData;
		_dataSource.IsTableauEnabled = true;
		_layerAsGauntletLayer.GamepadNavigationContext.GainNavigationAfterFrames(1, null);
	}

	private void UninstallConversation()
	{
		if (_isConversationInstalled)
		{
			OnClose();
			_isConversationInstalled = false;
		}
	}

	void IConversationStateHandler.OnConversationInstall()
	{
		if (!_isConversationInstalled)
		{
			CreateConversationMission();
			_dataSource = new MapConversationVM(OnContinue, GetContinueKeyText);
			base.Layer = new GauntletLayer(205);
			_layerAsGauntletLayer = base.Layer as GauntletLayer;
			_layerAsGauntletLayer.LoadMovie("MapConversation", _dataSource);
			base.Layer.InputRestrictions.SetInputRestrictions();
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ConversationHotKeyCategory"));
			base.MapScreen.AddLayer(base.Layer);
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(base.Layer);
			_conversationStateQueue.Enqueue(ConversationStates.OnInstall);
			_isConversationInstalled = true;
		}
	}

	void IConversationStateHandler.OnConversationUninstall()
	{
		_conversationStateQueue.Enqueue(ConversationStates.OnUninstall);
	}

	void IConversationStateHandler.OnConversationActivate()
	{
		if (_conversationStateQueue.Count > 0)
		{
			_conversationStateQueue.Enqueue(ConversationStates.OnActivate);
		}
		else
		{
			ProcessConversationState(ConversationStates.OnActivate);
		}
	}

	void IConversationStateHandler.OnConversationDeactivate()
	{
		if (_conversationStateQueue.Count > 0)
		{
			_conversationStateQueue.Enqueue(ConversationStates.OnDeactivate);
		}
		else
		{
			ProcessConversationState(ConversationStates.OnDeactivate);
		}
	}

	void IConversationStateHandler.OnConversationContinue()
	{
		if (_conversationStateQueue.Count > 0)
		{
			_conversationStateQueue.Enqueue(ConversationStates.OnContinue);
		}
		else
		{
			ProcessConversationState(ConversationStates.OnContinue);
		}
	}

	void IConversationStateHandler.ExecuteConversationContinue()
	{
		if (_conversationStateQueue.Count > 0)
		{
			_conversationStateQueue.Enqueue(ConversationStates.ExecuteContinue);
		}
		else
		{
			ProcessConversationState(ConversationStates.ExecuteContinue);
		}
	}

	private static bool IsSame(MapConversationTableauData first, MapConversationTableauData second)
	{
		if (first == null || second == null)
		{
			return false;
		}
		if (IsSame(first.PlayerCharacterData, second.PlayerCharacterData) && IsSame(first.ConversationPartnerData, second.ConversationPartnerData) && first.ConversationTerrainType == second.ConversationTerrainType && first.IsCurrentTerrainUnderSnow == second.IsCurrentTerrainUnderSnow)
		{
			return first.TimeOfDay == second.TimeOfDay;
		}
		return false;
	}

	private static bool IsSame(ConversationCharacterData first, ConversationCharacterData second)
	{
		if (first.Character == second.Character && first.NoHorse == second.NoHorse && first.NoWeapon == second.NoWeapon && first.Party == second.Party && first.SpawnedAfterFight == second.SpawnedAfterFight)
		{
			return first.IsCivilianEquipmentRequiredForLeader == second.IsCivilianEquipmentRequiredForLeader;
		}
		return false;
	}
}
