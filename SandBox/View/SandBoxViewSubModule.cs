using System;
using System.Collections.Generic;
using SandBox.View.Conversation;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Information.RundownTooltip;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.ScreenSystem;

namespace SandBox.View;

public class SandBoxViewSubModule : MBSubModuleBase
{
	private TextObject _sandBoxAchievementsHint = new TextObject("{=j09m7S2E}Achievements are disabled in SandBox mode!");

	private bool _isInitialized;

	private ConversationViewManager _conversationViewManager;

	private IMapConversationDataProvider _mapConversationDataProvider;

	private Dictionary<UIntPtr, PartyVisual> _visualsOfEntities;

	private Dictionary<UIntPtr, Tuple<MatrixFrame, PartyVisual>> _frameAndVisualOfEngines;

	private static SandBoxViewSubModule _instance;

	public static ConversationViewManager ConversationViewManager => _instance._conversationViewManager;

	public static IMapConversationDataProvider MapConversationDataProvider => _instance._mapConversationDataProvider;

	internal static Dictionary<UIntPtr, PartyVisual> VisualsOfEntities => _instance._visualsOfEntities;

	internal static Dictionary<UIntPtr, Tuple<MatrixFrame, PartyVisual>> FrameAndVisualOfEngines => _instance._frameAndVisualOfEngines;

	protected override void OnSubModuleLoad()
	{
		base.OnSubModuleLoad();
		_instance = this;
		SandBoxSaveHelper.OnStateChange += OnSaveHelperStateChange;
		RegisterTooltipTypes();
		Module.CurrentModule.AddInitialStateOption(new InitialStateOption("CampaignResumeGame", new TextObject("{=6mN03uTP}Saved Games"), 0, delegate
		{
			ScreenManager.PushScreen(SandBoxViewCreator.CreateSaveLoadScreen(isSaving: false));
		}, () => IsSavedGamesDisabled()));
		Module.CurrentModule.AddInitialStateOption(new InitialStateOption("ContinueCampaign", new TextObject("{=0tJ1oarX}Continue Campaign"), 1, ContinueCampaign, () => IsContinueCampaignDisabled()));
		Module.CurrentModule.AddInitialStateOption(new InitialStateOption("SandBoxNewGame", new TextObject("{=171fTtIN}SandBox"), 3, delegate
		{
			MBGameManager.StartNewGame(new SandBoxGameManager());
		}, () => IsSandboxDisabled(), _sandBoxAchievementsHint));
		Module.CurrentModule.ImguiProfilerTick += OnImguiProfilerTick;
		_mapConversationDataProvider = new DefaultMapConversationDataProvider();
	}

	protected override void OnSubModuleUnloaded()
	{
		Module.CurrentModule.ImguiProfilerTick -= OnImguiProfilerTick;
		SandBoxSaveHelper.OnStateChange -= OnSaveHelperStateChange;
		UnregisterTooltipTypes();
		_instance = null;
		base.OnSubModuleUnloaded();
	}

	protected override void OnBeforeInitialModuleScreenSetAsRoot()
	{
		base.OnBeforeInitialModuleScreenSetAsRoot();
		if (!_isInitialized)
		{
			CampaignOptionsManager.Initialize();
			_isInitialized = true;
		}
	}

	public override void OnCampaignStart(Game game, object starterObject)
	{
		base.OnCampaignStart(game, starterObject);
		if (Campaign.Current != null)
		{
			_conversationViewManager = new ConversationViewManager();
		}
	}

	public override void OnGameLoaded(Game game, object initializerObject)
	{
		_conversationViewManager = new ConversationViewManager();
	}

	public override void OnAfterGameInitializationFinished(Game game, object starterObject)
	{
		base.OnAfterGameInitializationFinished(game, starterObject);
	}

	public override void BeginGameStart(Game game)
	{
		base.BeginGameStart(game);
		if (Campaign.Current != null)
		{
			_visualsOfEntities = new Dictionary<UIntPtr, PartyVisual>();
			_frameAndVisualOfEngines = new Dictionary<UIntPtr, Tuple<MatrixFrame, PartyVisual>>();
			Campaign.Current.SaveHandler.MainHeroVisualSupplier = new MainHeroSaveVisualSupplier();
			TableauCacheManager.InitializeSandboxValues();
		}
	}

	public override void OnGameEnd(Game game)
	{
		if (_visualsOfEntities != null)
		{
			foreach (PartyVisual value in _visualsOfEntities.Values)
			{
				value.ReleaseResources();
			}
		}
		_visualsOfEntities = null;
		_frameAndVisualOfEngines = null;
		_conversationViewManager = null;
		if (Campaign.Current != null)
		{
			Campaign.Current.SaveHandler.MainHeroVisualSupplier = null;
			TableauCacheManager.ReleaseSandboxValues();
		}
	}

	private (bool, TextObject) IsSavedGamesDisabled()
	{
		if (Module.CurrentModule.IsOnlyCoreContentEnabled)
		{
			return (true, new TextObject("{=V8BXjyYq}Disabled during installation."));
		}
		if (MBSaveLoad.NumberOfCurrentSaves == 0)
		{
			return (true, new TextObject("{=XcVVE1mp}No saved games found."));
		}
		return (false, TextObject.Empty);
	}

	private (bool, TextObject) IsContinueCampaignDisabled()
	{
		if (Module.CurrentModule.IsOnlyCoreContentEnabled)
		{
			return (true, new TextObject("{=V8BXjyYq}Disabled during installation."));
		}
		if (string.IsNullOrEmpty(BannerlordConfig.LatestSaveGameName))
		{
			return (true, new TextObject("{=aWMZQKXZ}Save the game at least once to continue"));
		}
		SaveGameFileInfo saveFileWithName = MBSaveLoad.GetSaveFileWithName(BannerlordConfig.LatestSaveGameName);
		if (saveFileWithName == null)
		{
			return (true, new TextObject("{=60LTq0tQ}Can't find the save file for the latest save game."));
		}
		if (saveFileWithName.IsCorrupted)
		{
			return (true, new TextObject("{=t6W3UjG0}Save game file appear to be corrupted. Try starting a new campaign or load another one from Saved Games menu."));
		}
		return (false, TextObject.Empty);
	}

	private (bool, TextObject) IsSandboxDisabled()
	{
		if (Module.CurrentModule.IsOnlyCoreContentEnabled)
		{
			return (true, new TextObject("{=V8BXjyYq}Disabled during installation."));
		}
		return (false, TextObject.Empty);
	}

	private void ContinueCampaign()
	{
		SaveGameFileInfo saveFileWithName = MBSaveLoad.GetSaveFileWithName(BannerlordConfig.LatestSaveGameName);
		if (saveFileWithName != null && !saveFileWithName.IsCorrupted)
		{
			SandBoxSaveHelper.TryLoadSave(saveFileWithName, StartGame);
		}
		else
		{
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=oZrVNUOk}Error").ToString(), new TextObject("{=t6W3UjG0}Save game file appear to be corrupted. Try starting a new campaign or load another one from Saved Games menu.").ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, new TextObject("{=yS7PvrTD}OK").ToString(), null, null, null));
		}
	}

	private void StartGame(LoadResult loadResult)
	{
		MBGameManager.StartNewGame(new SandBoxGameManager(loadResult));
	}

	private void OnImguiProfilerTick()
	{
		if (Campaign.Current == null)
		{
			return;
		}
		MBReadOnlyList<MobileParty> all = MobileParty.All;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (MobileParty item in all)
		{
			if (item.IsVisible)
			{
				num++;
			}
			PartyVisual visualOfParty = PartyVisualManager.Current.GetVisualOfParty(item.Party);
			if (visualOfParty.HumanAgentVisuals != null)
			{
				num2++;
			}
			if (visualOfParty.MountAgentVisuals != null)
			{
				num2++;
			}
			if (visualOfParty.CaravanMountAgentVisuals != null)
			{
				num2++;
			}
			num3++;
		}
		Imgui.BeginMainThreadScope();
		Imgui.Begin("Bannerlord Campaign Statistics");
		Imgui.Columns(2);
		Imgui.Text("Name");
		Imgui.NextColumn();
		Imgui.Text("Count");
		Imgui.NextColumn();
		Imgui.Separator();
		Imgui.Text("Total Mobile Party");
		Imgui.NextColumn();
		Imgui.Text(num3.ToString());
		Imgui.NextColumn();
		Imgui.Text("Visible Mobile Party");
		Imgui.NextColumn();
		Imgui.Text(num.ToString());
		Imgui.NextColumn();
		Imgui.Text("Total Agent Visuals");
		Imgui.NextColumn();
		Imgui.Text(num2.ToString());
		Imgui.NextColumn();
		Imgui.End();
		Imgui.EndMainThreadScope();
	}

	private void RegisterTooltipTypes()
	{
		InformationManager.RegisterTooltip<List<MobileParty>, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshEncounterTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<Track, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshTrackTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<MapEvent, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshMapEventTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<Army, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshArmyTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<MobileParty, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshMobilePartyTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<Hero, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshHeroTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<Settlement, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshSettlementTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<CharacterObject, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshCharacterTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<WeaponDesignElement, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshCraftingPartTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<InventoryLogic, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshInventoryTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<ItemObject, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshItemTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<Building, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshBuildingTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<Workshop, PropertyBasedTooltipVM>(TooltipRefresherCollection.RefreshWorkshopTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<ExplainedNumber, RundownTooltipVM>(TooltipRefresherCollection.RefreshExplainedNumberTooltip, "RundownTooltip");
	}

	private void UnregisterTooltipTypes()
	{
		InformationManager.UnregisterTooltip<List<MobileParty>>();
		InformationManager.UnregisterTooltip<Track>();
		InformationManager.UnregisterTooltip<MapEvent>();
		InformationManager.UnregisterTooltip<Army>();
		InformationManager.UnregisterTooltip<MobileParty>();
		InformationManager.UnregisterTooltip<Hero>();
		InformationManager.UnregisterTooltip<Settlement>();
		InformationManager.UnregisterTooltip<CharacterObject>();
		InformationManager.UnregisterTooltip<WeaponDesignElement>();
		InformationManager.UnregisterTooltip<InventoryLogic>();
		InformationManager.UnregisterTooltip<ItemObject>();
		InformationManager.UnregisterTooltip<Building>();
		InformationManager.UnregisterTooltip<Workshop>();
		InformationManager.UnregisterTooltip<ExplainedNumber>();
	}

	public static void SetMapConversationDataProvider(IMapConversationDataProvider mapConversationDataProvider)
	{
		_instance._mapConversationDataProvider = mapConversationDataProvider;
	}

	private static void OnSaveHelperStateChange(SandBoxSaveHelper.SaveHelperState currentState)
	{
		switch (currentState)
		{
		case SandBoxSaveHelper.SaveHelperState.Start:
		case SandBoxSaveHelper.SaveHelperState.LoadGame:
			LoadingWindow.EnableGlobalLoadingWindow();
			break;
		case SandBoxSaveHelper.SaveHelperState.Inquiry:
			LoadingWindow.DisableGlobalLoadingWindow();
			break;
		default:
			Debug.FailedAssert("Undefined save state for listener!", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\SandBoxViewSubModule.cs", "OnSaveHelperStateChange", 426);
			break;
		}
	}
}
