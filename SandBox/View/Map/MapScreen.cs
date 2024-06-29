using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Helpers;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.View.Scripts;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;

namespace SandBox.View.Map;

[GameStateScreen(typeof(MapState))]
public class MapScreen : ScreenBase, IMapStateHandler, IGameStateListener
{
	private enum TerrainTypeSoundSlot
	{
		dismounted,
		mounted,
		mounted_slow,
		caravan,
		ambient
	}

	private class TempConversationStateHandler : IConversationStateHandler
	{
		private Queue<Action> _actionQueue = new Queue<Action>();

		private IConversationStateHandler _tempHandler;

		void IConversationStateHandler.ExecuteConversationContinue()
		{
			_actionQueue.Enqueue(delegate
			{
				_tempHandler?.ExecuteConversationContinue();
			});
		}

		void IConversationStateHandler.OnConversationActivate()
		{
			_actionQueue.Enqueue(delegate
			{
				_tempHandler?.OnConversationActivate();
			});
		}

		void IConversationStateHandler.OnConversationContinue()
		{
			_actionQueue.Enqueue(delegate
			{
				_tempHandler?.OnConversationContinue();
			});
		}

		void IConversationStateHandler.OnConversationDeactivate()
		{
			_actionQueue.Enqueue(delegate
			{
				_tempHandler?.OnConversationDeactivate();
			});
		}

		void IConversationStateHandler.OnConversationInstall()
		{
			_actionQueue.Enqueue(delegate
			{
				_tempHandler?.OnConversationInstall();
			});
		}

		void IConversationStateHandler.OnConversationUninstall()
		{
			_actionQueue.Enqueue(delegate
			{
				_tempHandler?.OnConversationUninstall();
			});
		}

		public void ApplyHandlerChangesTo(IConversationStateHandler newHandler)
		{
			_tempHandler = newHandler;
			while (_actionQueue.Count > 0)
			{
				_actionQueue.Dequeue()?.Invoke();
			}
			_tempHandler = null;
		}
	}

	private const float DoubleClickTimeLimit = 0.3f;

	private MenuViewContext _menuViewContext;

	private MenuContext _latestMenuContext;

	private bool _partyIconNeedsRefreshing;

	private uint _tooltipTargetHash;

	private object _tooltipTargetObject;

	private readonly ObservableCollection<MapView> _mapViews;

	private MapView[] _mapViewsCopyCache;

	private MapView _encounterOverlay;

	private MapView _armyOverlay;

	private MapReadyView _mapReadyView;

	private MapView _escapeMenuView;

	private MapView _battleSimulationView;

	private MapView _mapSiegeOverlayView;

	private MapView _campaignOptionsView;

	private MapView _mapConversationView;

	private MapView _marriageOfferPopupView;

	private MapView _mapCheatsView;

	public MapCameraView _mapCameraView;

	private MapNavigationHandler _navigationHandler = new MapNavigationHandler();

	private const int _frameDelayAmountForRenderActivation = 5;

	private float _timeSinceCreation;

	private bool _leftButtonDraggingMode;

	private UIntPtr _preSelectedSiegeEntityID;

	private Vec2 _oldMousePosition;

	private Vec2 _clickedPositionPixel;

	private Vec3 _clickedPosition;

	private Ray _mouseRay;

	private PartyVisual _preVisualOfSelectedEntity;

	private int _activatedFrameNo = Utilities.EngineFrameNo;

	public Dictionary<Tuple<Material, BannerCode>, Material> _characterBannerMaterialCache = new Dictionary<Tuple<Material, BannerCode>, Material>();

	private Tuple<ConversationCharacterData, ConversationCharacterData, TempConversationStateHandler> _conversationDataCache;

	private readonly int _displayedContextMenuType = -1;

	private double _lastReleaseTime;

	private double _lastPressTime;

	private double _secondLastPressTime;

	private bool _leftButtonDoubleClickOnSceneWidget;

	private float _waitForDoubleClickUntilTime;

	private float _timeToggleTimer = float.MaxValue;

	private bool _ignoreNextTimeToggle;

	private bool _exitOnSaveOver;

	private Scene _mapScene;

	private Campaign _campaign;

	private readonly MapState _mapState;

	private bool _isSceneViewEnabled;

	private bool _isReadyForRender;

	private bool _gpuMemoryCleared;

	private bool _focusLost;

	private bool _isKingdomDecisionsDirty;

	private bool _conversationOverThisFrame;

	private float _cheatPressTimer;

	private Dictionary<Tuple<Material, BannerCode>, Material> _bannerTexturedMaterialCache;

	private GameEntity _targetCircleEntitySmall;

	private GameEntity _targetCircleEntityBig;

	private GameEntity _targetCircleTown;

	private GameEntity _partyOutlineEntity;

	private GameEntity _townOutlineEntity;

	private Decal _targetDecalMeshSmall;

	private Decal _targetDecalMeshBig;

	private Decal _partyOutlineMesh;

	private Decal _settlementOutlineMesh;

	private Decal _targetTownMesh;

	private float _targetCircleRotationStartTime;

	private MapCursor _mapCursor = new MapCursor();

	private bool _mapSceneCursorWanted = true;

	private bool _mapSceneCursorActive;

	public IMapTracksCampaignBehavior MapTracksCampaignBehavior;

	private bool _isSoundOn = true;

	private float _soundCalculationTime;

	private const float SoundCalculationInterval = 0.2f;

	private uint _enemyPartyDecalColor = 4281663744u;

	private uint _allyPartyDecalColor = 4279308800u;

	private uint _neutralPartyDecalColor = 4294919959u;

	private MapColorGradeManager _colorGradeManager;

	private bool _playerSiegeMachineSlotMeshesAdded;

	private GameEntity[] _defenderMachinesCircleEntities;

	private GameEntity[] _attackerRamMachinesCircleEntities;

	private GameEntity[] _attackerTowerMachinesCircleEntities;

	private GameEntity[] _attackerRangedMachinesCircleEntities;

	private string _emptyAttackerRangedDecalMaterialName = "decal_siege_ranged";

	private string _attackerRamMachineDecalMaterialName = "decal_siege_ram";

	private string _attackerTowerMachineDecalMaterialName = "decal_siege_tower";

	private string _attackerRangedMachineDecalMaterialName = "decal_siege_ranged";

	private string _defenderRangedMachineDecalMaterialName = "decal_defender_ranged_siege";

	private uint _preperationOrEnemySiegeEngineDecalColor = 4287064638u;

	private uint _normalStartSiegeEngineDecalColor = 4278394186u;

	private float _defenderMachineCircleDecalScale = 0.25f;

	private float _attackerMachineDecalScale = 0.38f;

	private bool _isNewDecalScaleImplementationEnabled;

	private uint _normalEndSiegeEngineDecalColor = 4284320212u;

	private uint _hoveredSiegeEngineDecalColor = 4293956364u;

	private uint _withMachineSiegeEngineDecalColor = 4283683126u;

	private float _machineDecalAnimLoopTime = 0.5f;

	public bool TooltipHandlingDisabled;

	private readonly UIntPtr[] _intersectedEntityIDs = new UIntPtr[128];

	private readonly Intersection[] _intersectionInfos = new Intersection[128];

	private GameEntity[] _tickedMapEntities;

	private Mesh[] _tickedMapMeshes;

	private readonly List<MBCampaignEvent> _periodicCampaignUIEvents;

	private bool _ignoreLeftMouseRelease;

	public static MapScreen Instance { get; private set; }

	public CampaignMapSiegePrefabEntityCache PrefabEntityCache { get; private set; }

	public MapEncyclopediaView EncyclopediaScreenManager { get; private set; }

	public MapNotificationView MapNotificationView { get; private set; }

	public bool IsInMenu => _menuViewContext != null;

	public bool IsEscapeMenuOpened { get; private set; }

	public PartyVisual CurrentVisualOfTooltip { get; private set; }

	public SceneLayer SceneLayer { get; private set; }

	public IInputContext Input => SceneLayer.Input;

	public bool IsReady => _isReadyForRender;

	public bool IsInBattleSimulation { get; private set; }

	public bool IsInTownManagement { get; private set; }

	public bool IsInHideoutTroopManage { get; private set; }

	public bool IsInArmyManagement { get; private set; }

	public bool IsInRecruitment { get; private set; }

	public bool IsBarExtended { get; private set; }

	public bool IsInCampaignOptions { get; private set; }

	public bool IsMarriageOfferPopupActive { get; private set; }

	public bool IsMapCheatsActive { get; private set; }

	public Dictionary<Tuple<Material, BannerCode>, Material> BannerTexturedMaterialCache => _bannerTexturedMaterialCache ?? (_bannerTexturedMaterialCache = new Dictionary<Tuple<Material, BannerCode>, Material>());

	public bool MapSceneCursorActive
	{
		get
		{
			return _mapSceneCursorActive;
		}
		set
		{
			if (_mapSceneCursorActive != value)
			{
				_mapSceneCursorActive = value;
			}
		}
	}

	public GameEntity ContourMaskEntity { get; private set; }

	public List<Mesh> InactiveLightMeshes { get; private set; }

	public List<Mesh> ActiveLightMeshes { get; private set; }

	public static Dictionary<UIntPtr, PartyVisual> VisualsOfEntities => SandBoxViewSubModule.VisualsOfEntities;

	internal static Dictionary<UIntPtr, Tuple<MatrixFrame, PartyVisual>> FrameAndVisualOfEngines => SandBoxViewSubModule.FrameAndVisualOfEngines;

	public MapScreen(MapState mapState)
	{
		_mapState = mapState;
		mapState.Handler = this;
		_periodicCampaignUIEvents = new List<MBCampaignEvent>();
		InitializeVisuals();
		CampaignMusicHandler.Create();
		_mapViews = new ObservableCollection<MapView>();
		_mapViewsCopyCache = new MapView[0];
		_mapCameraView = (MapCameraView)AddMapView<MapCameraView>(Array.Empty<object>());
		MapTracksCampaignBehavior = Campaign.Current.GetCampaignBehavior<IMapTracksCampaignBehavior>();
	}

	public void OnHoverMapEntity(IMapEntity mapEntity)
	{
		uint hashCode = (uint)mapEntity.GetHashCode();
		if (_tooltipTargetHash != hashCode)
		{
			_tooltipTargetHash = hashCode;
			_tooltipTargetObject = null;
			mapEntity.OnHover();
		}
	}

	public void SetupMapTooltipForTrack(Track track)
	{
		if (_tooltipTargetObject != track)
		{
			_tooltipTargetObject = track;
			_tooltipTargetHash = 0u;
			InformationManager.ShowTooltip(typeof(Track), track);
		}
	}

	public void RemoveMapTooltip()
	{
		if (_tooltipTargetObject != null || _tooltipTargetHash != 0)
		{
			_tooltipTargetObject = null;
			_tooltipTargetHash = 0u;
			MBInformationManager.HideInformations();
		}
	}

	private static void PreloadTextures()
	{
		List<string> list = new List<string>();
		list.Add("gui_map_circle_enemy");
		list.Add("gui_map_circle_enemy_selected");
		list.Add("gui_map_circle_neutral");
		list.Add("gui_map_circle_neutral_selected");
		for (int i = 2; i <= 5; i++)
		{
			list.Add("gui_map_circle_enemy_selected_" + i);
			list.Add("gui_map_circle_neutral_selected_" + i);
		}
		for (int j = 0; j < list.Count; j++)
		{
			Texture.GetFromResource(list[j]).PreloadTexture(blocking: false);
		}
		list.Clear();
	}

	private void HandleSiegeEngineHoverEnd()
	{
		if (_preSelectedSiegeEntityID != UIntPtr.Zero)
		{
			FrameAndVisualOfEngines[_preSelectedSiegeEntityID].Item2.OnMapHoverSiegeEngineEnd();
			_preSelectedSiegeEntityID = UIntPtr.Zero;
		}
	}

	private void SetCameraOfSceneLayer()
	{
		SceneLayer.SetCamera(_mapCameraView.Camera);
		Vec3 center = _mapCameraView.CameraFrame.origin;
		center.z = 0f;
		SceneLayer.SetFocusedShadowmap(enable: false, ref center, 0f);
	}

	protected override void OnResume()
	{
		base.OnResume();
		PreloadTextures();
		_isSoundOn = true;
		RestartAmbientSounds();
		if (_gpuMemoryCleared)
		{
			_gpuMemoryCleared = false;
		}
		for (int num = _mapViews.Count - 1; num >= 0; num--)
		{
			_mapViews[num].OnResume();
		}
		MenuContext menuContext = _mapState.MenuContext;
		if (_menuViewContext != null)
		{
			if (menuContext != null && menuContext != _menuViewContext.MenuContext)
			{
				_menuViewContext.UpdateMenuContext(menuContext);
			}
			else if (menuContext == null)
			{
				ExitMenuContext();
			}
		}
		_menuViewContext?.OnResume();
		(Campaign.Current.MapSceneWrapper as MapScene).ValidateAgentVisualsReseted();
	}

	protected override void OnPause()
	{
		base.OnPause();
		MBInformationManager.HideInformations();
		PauseAmbientSounds();
		_isSoundOn = false;
		_activatedFrameNo = Utilities.EngineFrameNo;
		HandleIfSceneIsReady();
		_conversationOverThisFrame = false;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		_mapCameraView.OnActivate(_leftButtonDraggingMode, _clickedPosition);
		_activatedFrameNo = Utilities.EngineFrameNo;
		HandleIfSceneIsReady();
		Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.MapWindow));
		SetCameraOfSceneLayer();
		RestartAmbientSounds();
		PartyBase.MainParty.SetVisualAsDirty();
	}

	public void ClearGPUMemory()
	{
		if (true)
		{
			SceneLayer.ClearRuntimeGPUMemory(remove_terrain: true);
		}
		Texture.ReleaseGpuMemories();
		_gpuMemoryCleared = true;
	}

	protected override void OnDeactivate()
	{
		Game.Current?.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.None));
		PauseAmbientSounds();
		_menuViewContext?.StopAllSounds();
		MBInformationManager.HideInformations();
		for (int num = _mapViews.Count - 1; num >= 0; num--)
		{
			_mapViews[num].OnDeactivate();
		}
		base.OnDeactivate();
	}

	public override void OnFocusChangeOnGameWindow(bool focusGained)
	{
		base.OnFocusChangeOnGameWindow(focusGained);
		if (!focusGained && BannerlordConfig.StopGameOnFocusLost)
		{
			Func<bool> isAnyInquiryActive = InformationManager.IsAnyInquiryActive;
			if (isAnyInquiryActive != null && !isAnyInquiryActive())
			{
				MapEncyclopediaView encyclopediaScreenManager = EncyclopediaScreenManager;
				if ((encyclopediaScreenManager == null || !encyclopediaScreenManager.IsEncyclopediaOpen) && _mapViews.All((MapView m) => m.IsOpeningEscapeMenuOnFocusChangeAllowed()))
				{
					OnEscapeMenuToggled(isOpened: true);
				}
			}
		}
		_focusLost = !focusGained;
	}

	public MapView AddMapView<T>(params object[] parameters) where T : MapView, new()
	{
		for (int i = 0; i < _mapViews.Count; i++)
		{
			if (_mapViews[i] is T result)
			{
				Debug.FailedAssert("Map view already added to the list", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\MapScreen.cs", "AddMapView", 492);
				Debug.Print("Map view already added to the list: " + typeof(T).Name + ". Returning existing view instead of creating new one.");
				return result;
			}
		}
		MapView mapView = SandBoxViewCreator.CreateMapView<T>(parameters);
		mapView.MapScreen = this;
		mapView.MapState = _mapState;
		_mapViews.Add(mapView);
		mapView.CreateLayout();
		return mapView;
	}

	public T GetMapView<T>() where T : MapView
	{
		foreach (MapView mapView in _mapViews)
		{
			if (mapView is T)
			{
				return (T)mapView;
			}
		}
		return null;
	}

	public void RemoveMapView(MapView mapView)
	{
		mapView.OnFinalize();
		_mapViews.Remove(mapView);
	}

	public void AddEncounterOverlay(GameOverlays.MenuOverlayType type)
	{
		if (_encounterOverlay == null)
		{
			_encounterOverlay = AddMapView<MapOverlayView>(new object[1] { type });
			for (int num = _mapViews.Count - 1; num >= 0; num--)
			{
				_mapViews[num].OnOverlayCreated();
			}
		}
	}

	public void AddArmyOverlay(GameOverlays.MapOverlayType type)
	{
		if (_armyOverlay == null)
		{
			_armyOverlay = AddMapView<MapOverlayView>(new object[1] { type });
			for (int num = _mapViews.Count - 1; num >= 0; num--)
			{
				_mapViews[num].OnOverlayCreated();
			}
		}
	}

	public void RemoveEncounterOverlay()
	{
		if (_encounterOverlay != null)
		{
			RemoveMapView(_encounterOverlay);
			_encounterOverlay = null;
			for (int num = _mapViews.Count - 1; num >= 0; num--)
			{
				_mapViews[num].OnOverlayClosed();
			}
		}
	}

	public void RemoveArmyOverlay()
	{
		if (_armyOverlay != null)
		{
			RemoveMapView(_armyOverlay);
			_armyOverlay = null;
			for (int num = _mapViews.Count - 1; num >= 0; num--)
			{
				_mapViews[num].OnOverlayClosed();
			}
		}
	}

	protected override void OnInitialize()
	{
		base.OnInitialize();
		if (MBDebug.TestModeEnabled)
		{
			CheckValidityOfItems();
		}
		Instance = this;
		_mapCameraView.Initialize();
		ViewSubModule.BannerTexturedMaterialCache = BannerTexturedMaterialCache;
		SceneLayer = new SceneLayer("SceneLayer", clearSceneOnFinalize: true, autoToggleSceneView: false);
		SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
		SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
		SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("MapHotKeyCategory"));
		AddLayer(SceneLayer);
		_mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
		Utilities.SetAllocationAlwaysValidScene(null);
		SceneLayer.SetScene(_mapScene);
		SceneLayer.SceneView.SetEnable(value: false);
		SceneLayer.SetSceneUsesShadows(value: true);
		SceneLayer.SetRenderWithPostfx(value: true);
		SceneLayer.SetSceneUsesContour(value: true);
		SceneLayer.SceneView.SetAcceptGlobalDebugRenderObjects(value: true);
		SceneLayer.SceneView.SetResolutionScaling(value: true);
		CollectTickableMapMeshes();
		MapNotificationView = AddMapView<MapNotificationView>(Array.Empty<object>()) as MapNotificationView;
		AddMapView<MapBasicView>(Array.Empty<object>());
		AddMapView<MapSettlementNameplateView>(Array.Empty<object>());
		AddMapView<MapPartyNameplateView>(Array.Empty<object>());
		AddMapView<MapEventVisualsView>(Array.Empty<object>());
		AddMapView<MapMobilePartyTrackerView>(Array.Empty<object>());
		AddMapView<MapSaveView>(Array.Empty<object>());
		AddMapView<MapGamepadEffectsView>(Array.Empty<object>());
		EncyclopediaScreenManager = AddMapView<MapEncyclopediaView>(Array.Empty<object>()) as MapEncyclopediaView;
		AddMapView<MapBarView>(Array.Empty<object>());
		_mapReadyView = AddMapView<MapReadyView>(Array.Empty<object>()) as MapReadyView;
		_mapReadyView.SetIsMapSceneReady(isReady: false);
		_mouseRay = new Ray(Vec3.Zero, Vec3.Up);
		if (PlayerSiege.PlayerSiegeEvent != null)
		{
			((IMapStateHandler)this)?.OnPlayerSiegeActivated();
		}
		PrefabEntityCache = SceneLayer.SceneView.GetScene().GetFirstEntityWithScriptComponent<CampaignMapSiegePrefabEntityCache>().GetFirstScriptOfType<CampaignMapSiegePrefabEntityCache>();
		CampaignEvents.OnSaveOverEvent.AddNonSerializedListener(this, OnSaveOver);
		CampaignEvents.OnMarriageOfferedToPlayerEvent.AddNonSerializedListener(this, OnMarriageOfferedToPlayer);
		CampaignEvents.OnMarriageOfferCanceledEvent.AddNonSerializedListener(this, OnMarriageOfferCanceled);
		GameEntity firstEntityWithScriptComponent = _mapScene.GetFirstEntityWithScriptComponent<MapColorGradeManager>();
		if (firstEntityWithScriptComponent != null)
		{
			_colorGradeManager = firstEntityWithScriptComponent.GetFirstScriptOfType<MapColorGradeManager>();
		}
	}

	private void OnSaveOver(bool isSuccessful, string newSaveGameName)
	{
		if (_exitOnSaveOver)
		{
			if (isSuccessful)
			{
				OnExit();
			}
			_exitOnSaveOver = false;
		}
	}

	private void OnMarriageOfferedToPlayer(Hero suitor, Hero maiden)
	{
		_marriageOfferPopupView = AddMapView<MarriageOfferPopupView>(new object[2] { suitor, maiden });
	}

	private void OnMarriageOfferCanceled(Hero suitor, Hero maiden)
	{
		if (_marriageOfferPopupView != null)
		{
			RemoveMapView(_marriageOfferPopupView);
			_marriageOfferPopupView = null;
		}
	}

	protected override void OnFinalize()
	{
		for (int num = _mapViews.Count - 1; num >= 0; num--)
		{
			_mapViews[num].OnFinalize();
		}
		PartyVisualManager.Current.OnFinalized();
		base.OnFinalize();
		if (_mapScene != null)
		{
			_mapScene.ClearAll();
		}
		Common.MemoryCleanupGC();
		_characterBannerMaterialCache.Clear();
		_characterBannerMaterialCache = null;
		ViewSubModule.BannerTexturedMaterialCache = null;
		MBMusicManager.Current.DeactivateCampaignMode();
		MBMusicManager.Current.OnCampaignMusicHandlerFinalize();
		CampaignEvents.OnSaveOverEvent.ClearListeners(this);
		CampaignEvents.OnMarriageOfferedToPlayerEvent.ClearListeners(this);
		CampaignEvents.OnMarriageOfferCanceledEvent.ClearListeners(this);
		_mapScene = null;
		_campaign = null;
		_navigationHandler = null;
		_mapCameraView = null;
		Instance = null;
	}

	public void OnHourlyTick()
	{
		for (int num = _mapViews.Count - 1; num >= 0; num--)
		{
			_mapViews[num].OnHourlyTick();
		}
		_isKingdomDecisionsDirty = Clan.PlayerClan.Kingdom?.UnresolvedDecisions.FirstOrDefault((KingdomDecision d) => d.NotifyPlayer && d.IsEnforced && d.IsPlayerParticipant && !d.ShouldBeCancelled()) != null;
	}

	private void OnRenderingStateChanged(bool startedRendering)
	{
		if (startedRendering && _isSceneViewEnabled && _conversationDataCache != null)
		{
			Campaign.Current.ConversationManager.Handler = null;
			Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
			HandleMapConversationInit(_conversationDataCache.Item1, _conversationDataCache.Item2);
			_conversationDataCache.Item3.ApplyHandlerChangesTo(_mapConversationView as IConversationStateHandler);
			_conversationDataCache = null;
		}
	}

	private void ShowNextKingdomDecisionPopup()
	{
		KingdomDecision kingdomDecision = Clan.PlayerClan.Kingdom?.UnresolvedDecisions.FirstOrDefault((KingdomDecision d) => d.NotifyPlayer && d.IsEnforced && d.IsPlayerParticipant && !d.ShouldBeCancelled());
		if (kingdomDecision != null)
		{
			InquiryData data = new InquiryData(new TextObject("{=A7349NHy}Critical Kingdom Decision").ToString(), kingdomDecision.GetChooseTitle().ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, new TextObject("{=bFzZwwjT}Examine").ToString(), "", delegate
			{
				OpenKingdom();
			}, null);
			kingdomDecision.NotifyPlayer = false;
			InformationManager.ShowInquiry(data, pauseGameActiveState: true);
			_isKingdomDecisionsDirty = false;
		}
		else
		{
			Debug.FailedAssert("There is no dirty decision but still demanded one", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\MapScreen.cs", "ShowNextKingdomDecisionPopup", 773);
		}
	}

	void IMapStateHandler.OnMenuModeTick(float dt)
	{
		for (int num = _mapViews.Count - 1; num >= 0; num--)
		{
			_mapViews[num].OnMenuModeTick(dt);
		}
	}

	private void HandleIfBlockerStatesDisabled()
	{
		bool isReadyForRender = _isReadyForRender;
		bool flag = SceneLayer.SceneView.ReadyToRender() && SceneLayer.SceneView.CheckSceneReadyToRender();
		bool flag2 = (_isSceneViewEnabled || _mapConversationView != null) && flag;
		if (LoadingWindow.IsLoadingWindowActive && flag2)
		{
			LoadingWindow.DisableGlobalLoadingWindow();
		}
		_mapReadyView.SetIsMapSceneReady(flag2);
		_isReadyForRender = flag2;
		if (isReadyForRender != _isReadyForRender)
		{
			OnRenderingStateChanged(_isReadyForRender);
		}
	}

	private void CheckCursorState()
	{
		Vec3 worldMouseNear = Vec3.Zero;
		Vec3 worldMouseFar = Vec3.Zero;
		SceneLayer.SceneView.TranslateMouse(ref worldMouseNear, ref worldMouseFar);
		Vec3 clippedMouseNear = worldMouseNear;
		Vec3 clippedMouseFar = worldMouseFar;
		PathFaceRecord currentFace = PathFaceRecord.NullFaceRecord;
		GetCursorIntersectionPoint(ref clippedMouseNear, ref clippedMouseFar, out var _, out var _, ref currentFace);
		bool flag = Campaign.Current.MapSceneWrapper.AreFacesOnSameIsland(currentFace, MobileParty.MainParty.CurrentNavigationFace, ignoreDisabled: false);
		SceneLayer.ActiveCursor = (flag ? CursorType.Default : CursorType.Disabled);
	}

	private void HandleIfSceneIsReady()
	{
		int num = Utilities.EngineFrameNo - _activatedFrameNo;
		bool isSceneViewEnabled = _isSceneViewEnabled;
		if (num < 5)
		{
			isSceneViewEnabled = false;
			MapColorGradeManager colorGradeManager = _colorGradeManager;
			if (colorGradeManager != null)
			{
				colorGradeManager.ApplyAtmosphere(true);
			}
		}
		else
		{
			bool num2 = _mapConversationView != null;
			bool flag = ScreenManager.TopScreen == this;
			isSceneViewEnabled = !num2 && flag;
		}
		if (isSceneViewEnabled != _isSceneViewEnabled)
		{
			_isSceneViewEnabled = isSceneViewEnabled;
			SceneLayer.SceneView.SetEnable(_isSceneViewEnabled);
			if (_isSceneViewEnabled)
			{
				_mapScene.CheckResources();
				if (_focusLost && !IsEscapeMenuOpened)
				{
					OnFocusChangeOnGameWindow(focusGained: false);
				}
			}
		}
		HandleIfBlockerStatesDisabled();
	}

	void IMapStateHandler.StartCameraAnimation(Vec2 targetPosition, float animationStopDuration)
	{
		_mapCameraView.StartCameraAnimation(targetPosition, animationStopDuration);
	}

	void IMapStateHandler.BeforeTick(float dt)
	{
		HandleIfSceneIsReady();
		bool flag = MobileParty.MainParty != null && PartyBase.MainParty.IsValid;
		if (flag && !_mapCameraView.CameraAnimationInProgress)
		{
			if (!IsInMenu && SceneLayer.Input.IsHotKeyPressed("MapChangeCursorMode"))
			{
				_mapSceneCursorWanted = !_mapSceneCursorWanted;
			}
			if (SceneLayer.Input.IsHotKeyPressed("MapClick"))
			{
				_secondLastPressTime = _lastPressTime;
				_lastPressTime = Time.ApplicationTime;
			}
			_leftButtonDoubleClickOnSceneWidget = false;
			if (SceneLayer.Input.IsHotKeyReleased("MapClick"))
			{
				Vec2 mousePositionPixel = SceneLayer.Input.GetMousePositionPixel();
				float applicationTime = Time.ApplicationTime;
				_leftButtonDoubleClickOnSceneWidget = (double)applicationTime - _lastReleaseTime < 0.30000001192092896 && (double)applicationTime - _secondLastPressTime < 0.44999998807907104 && mousePositionPixel.Distance(_oldMousePosition) < 10f;
				if (_leftButtonDoubleClickOnSceneWidget)
				{
					_waitForDoubleClickUntilTime = 0f;
				}
				_oldMousePosition = SceneLayer.Input.GetMousePositionPixel();
				_lastReleaseTime = applicationTime;
			}
			if (IsReady)
			{
				HandleMouse(dt);
			}
		}
		float deltaMouseScroll = SceneLayer.Input.GetDeltaMouseScroll();
		Vec3 worldMouseNear = Vec3.Zero;
		Vec3 worldMouseFar = Vec3.Zero;
		SceneLayer.SceneView.TranslateMouse(ref worldMouseNear, ref worldMouseFar);
		float gameKeyAxis = SceneLayer.Input.GetGameKeyAxis("CameraAxisX");
		float collisionDistance;
		Vec3 closestPoint;
		bool rayCastForClosestEntityOrTerrainCondition = _mapScene.RayCastForClosestEntityOrTerrain(worldMouseNear, worldMouseFar, out collisionDistance, out closestPoint, 0.01f, BodyFlags.CameraCollisionRayCastExludeFlags);
		float rX = 0f;
		float rY = 0f;
		float num = 1f;
		bool num2 = !TaleWorlds.InputSystem.Input.IsGamepadActive && !IsInMenu && ScreenManager.FocusedLayer == SceneLayer;
		bool flag2 = TaleWorlds.InputSystem.Input.IsGamepadActive && MapSceneCursorActive;
		if (num2 || flag2)
		{
			if (SceneLayer.Input.IsGameKeyDown(54))
			{
				num = _mapCameraView.CameraFastMoveMultiplier;
			}
			rX = SceneLayer.Input.GetGameKeyAxis("MapMovementAxisX") * num;
			rY = SceneLayer.Input.GetGameKeyAxis("MapMovementAxisY") * num;
		}
		_ignoreLeftMouseRelease = false;
		if (SceneLayer.Input.IsKeyPressed(InputKey.LeftMouseButton))
		{
			_clickedPositionPixel = SceneLayer.Input.GetMousePositionPixel();
			_mapScene.RayCastForClosestEntityOrTerrain(_mouseRay.Origin, _mouseRay.EndPoint, out collisionDistance, out _clickedPosition, 0.01f, BodyFlags.CameraCollisionRayCastExludeFlags);
			if (CurrentVisualOfTooltip != null)
			{
				RemoveMapTooltip();
			}
			_leftButtonDraggingMode = false;
		}
		else if (SceneLayer.Input.IsKeyDown(InputKey.LeftMouseButton) && !SceneLayer.Input.IsKeyReleased(InputKey.LeftMouseButton) && (SceneLayer.Input.GetMousePositionPixel().DistanceSquared(_clickedPositionPixel) > 300f || _leftButtonDraggingMode) && !IsInMenu)
		{
			_leftButtonDraggingMode = true;
		}
		else if (_leftButtonDraggingMode)
		{
			_leftButtonDraggingMode = false;
			_ignoreLeftMouseRelease = true;
		}
		if (SceneLayer.Input.IsKeyDown(InputKey.MiddleMouseButton))
		{
			MBWindowManager.DontChangeCursorPos();
		}
		if (SceneLayer.Input.IsKeyReleased(InputKey.LeftMouseButton))
		{
			_clickedPositionPixel = SceneLayer.Input.GetMousePositionPixel();
		}
		MapSceneCursorActive = !SceneLayer.Input.GetIsMouseActive() && !IsInMenu && ScreenManager.FocusedLayer == SceneLayer && _mapSceneCursorWanted;
		MapCameraView.InputInformation inputInformation = default(MapCameraView.InputInformation);
		inputInformation.IsMainPartyValid = flag;
		inputInformation.IsMapReady = IsReady;
		inputInformation.IsControlDown = SceneLayer.Input.IsControlDown();
		inputInformation.IsMouseActive = SceneLayer.Input.GetIsMouseActive();
		inputInformation.CheatModeEnabled = Game.Current.CheatMode;
		inputInformation.DeltaMouseScroll = deltaMouseScroll;
		inputInformation.LeftMouseButtonPressed = SceneLayer.Input.IsKeyPressed(InputKey.LeftMouseButton);
		inputInformation.LeftMouseButtonDown = SceneLayer.Input.IsKeyDown(InputKey.LeftMouseButton);
		inputInformation.LeftMouseButtonReleased = SceneLayer.Input.IsKeyReleased(InputKey.LeftMouseButton);
		inputInformation.MiddleMouseButtonDown = SceneLayer.Input.IsKeyDown(InputKey.MiddleMouseButton);
		inputInformation.RightMouseButtonDown = SceneLayer.Input.IsKeyDown(InputKey.RightMouseButton);
		inputInformation.RotateLeftKeyDown = SceneLayer.Input.IsGameKeyDown(57);
		inputInformation.RotateRightKeyDown = SceneLayer.Input.IsGameKeyDown(58);
		inputInformation.PartyMoveUpKey = SceneLayer.Input.IsGameKeyDown(49);
		inputInformation.PartyMoveDownKey = SceneLayer.Input.IsGameKeyDown(50);
		inputInformation.PartyMoveLeftKey = SceneLayer.Input.IsGameKeyDown(52);
		inputInformation.PartyMoveRightKey = SceneLayer.Input.IsGameKeyDown(51);
		inputInformation.MapZoomIn = SceneLayer.Input.GetGameKeyState(55);
		inputInformation.MapZoomOut = SceneLayer.Input.GetGameKeyState(56);
		inputInformation.CameraFollowModeKeyPressed = SceneLayer.Input.IsGameKeyPressed(63);
		inputInformation.MousePositionPixel = SceneLayer.Input.GetMousePositionPixel();
		inputInformation.ClickedPositionPixel = _clickedPositionPixel;
		inputInformation.ClickedPosition = _clickedPosition;
		inputInformation.LeftButtonDraggingMode = _leftButtonDraggingMode;
		inputInformation.IsInMenu = IsInMenu;
		inputInformation.WorldMouseNear = worldMouseNear;
		inputInformation.WorldMouseFar = worldMouseFar;
		inputInformation.MouseSensitivity = SceneLayer.Input.GetMouseSensitivity();
		inputInformation.MouseMoveX = SceneLayer.Input.GetMouseMoveX();
		inputInformation.MouseMoveY = SceneLayer.Input.GetMouseMoveY();
		inputInformation.HorizontalCameraInput = gameKeyAxis;
		inputInformation.RayCastForClosestEntityOrTerrainCondition = rayCastForClosestEntityOrTerrainCondition;
		inputInformation.ProjectedPosition = closestPoint;
		inputInformation.RX = rX;
		inputInformation.RY = rY;
		inputInformation.RS = num;
		inputInformation.Dt = dt;
		_mapCameraView.OnBeforeTick(in inputInformation);
		_mapCursor.SetVisible(MapSceneCursorActive);
		if (flag && !_campaign.TimeControlModeLock)
		{
			if (!_mapState.AtMenu)
			{
				goto IL_06f5;
			}
			if (Campaign.Current.CurrentMenuContext != null)
			{
				GameMenu gameMenu = Campaign.Current.CurrentMenuContext.GameMenu;
				if (gameMenu != null && gameMenu.IsWaitActive)
				{
					goto IL_06f5;
				}
			}
		}
		goto IL_09ad;
		IL_06f5:
		float applicationTime2 = Time.ApplicationTime;
		if (SceneLayer.Input.IsGameKeyPressed(62) && _timeToggleTimer == float.MaxValue)
		{
			_timeToggleTimer = applicationTime2;
		}
		if (SceneLayer.Input.IsGameKeyPressed(62) && applicationTime2 - _timeToggleTimer > 0.4f)
		{
			if (_campaign.TimeControlMode == CampaignTimeControlMode.StoppablePlay || _campaign.TimeControlMode == CampaignTimeControlMode.UnstoppablePlay)
			{
				_campaign.SetTimeSpeed(2);
			}
			else if (_campaign.TimeControlMode == CampaignTimeControlMode.StoppableFastForward || _campaign.TimeControlMode == CampaignTimeControlMode.UnstoppableFastForward)
			{
				_campaign.SetTimeSpeed(1);
			}
			else if (_campaign.TimeControlMode == CampaignTimeControlMode.Stop)
			{
				_campaign.SetTimeSpeed(1);
			}
			else if (_campaign.TimeControlMode == CampaignTimeControlMode.FastForwardStop)
			{
				_campaign.SetTimeSpeed(2);
			}
			_timeToggleTimer = float.MaxValue;
			_ignoreNextTimeToggle = true;
		}
		else if (SceneLayer.Input.IsGameKeyPressed(62))
		{
			if (_ignoreNextTimeToggle)
			{
				_ignoreNextTimeToggle = false;
			}
			else
			{
				_waitForDoubleClickUntilTime = 0f;
				if (_campaign.TimeControlMode == CampaignTimeControlMode.UnstoppableFastForward || _campaign.TimeControlMode == CampaignTimeControlMode.UnstoppablePlay || ((_campaign.TimeControlMode == CampaignTimeControlMode.StoppableFastForward || _campaign.TimeControlMode == CampaignTimeControlMode.StoppablePlay) && !_campaign.IsMainPartyWaiting))
				{
					_campaign.SetTimeSpeed(0);
				}
				else if (_campaign.TimeControlMode == CampaignTimeControlMode.Stop || _campaign.TimeControlMode == CampaignTimeControlMode.StoppablePlay)
				{
					_campaign.SetTimeSpeed(1);
				}
				else if (_campaign.TimeControlMode == CampaignTimeControlMode.FastForwardStop || _campaign.TimeControlMode == CampaignTimeControlMode.StoppableFastForward)
				{
					_campaign.SetTimeSpeed(2);
				}
			}
			_timeToggleTimer = float.MaxValue;
		}
		else if (SceneLayer.Input.IsGameKeyPressed(59))
		{
			_waitForDoubleClickUntilTime = 0f;
			_campaign.SetTimeSpeed(0);
		}
		else if (SceneLayer.Input.IsGameKeyPressed(60))
		{
			_waitForDoubleClickUntilTime = 0f;
			_campaign.SetTimeSpeed(1);
		}
		else if (SceneLayer.Input.IsGameKeyPressed(61))
		{
			_waitForDoubleClickUntilTime = 0f;
			_campaign.SetTimeSpeed(2);
		}
		else if (SceneLayer.Input.IsGameKeyPressed(64))
		{
			if (_campaign.TimeControlMode == CampaignTimeControlMode.UnstoppableFastForward || _campaign.TimeControlMode == CampaignTimeControlMode.StoppableFastForward)
			{
				_campaign.SetTimeSpeed(0);
			}
			else
			{
				_campaign.SetTimeSpeed(2);
			}
		}
		goto IL_09ad;
		IL_09ad:
		if (!flag && CurrentVisualOfTooltip != null)
		{
			CurrentVisualOfTooltip = null;
			RemoveMapTooltip();
		}
		SetCameraOfSceneLayer();
		if (!SceneLayer.Input.GetIsMouseActive() && Campaign.Current.GameStarted)
		{
			_mapCursor.BeforeTick(dt);
		}
	}

	void IMapStateHandler.Tick(float dt)
	{
		if (_mapViewsCopyCache.Length != _mapViews.Count || !_mapViewsCopyCache.SequenceEqual(_mapViews))
		{
			_mapViewsCopyCache = new MapView[_mapViews.Count];
			_mapViews.CopyTo(_mapViewsCopyCache, 0);
		}
		if (!IsInMenu)
		{
			if (_isKingdomDecisionsDirty)
			{
				ShowNextKingdomDecisionPopup();
			}
			else
			{
				if (ViewModel.UIDebugMode && base.DebugInput.IsHotKeyDown("UIExtendedDebugKey") && base.DebugInput.IsHotKeyPressed("MapScreenHotkeyOpenEncyclopedia"))
				{
					OpenEncyclopedia();
				}
				bool cheatMode = Game.Current.CheatMode;
				if (cheatMode && base.DebugInput.IsHotKeyPressed("MapScreenHotkeySwitchCampaignTrueSight"))
				{
					_campaign.TrueSight = !_campaign.TrueSight;
				}
				if (cheatMode)
				{
					base.DebugInput.IsHotKeyPressed("MapScreenPrintMultiLineText");
				}
				for (int num = _mapViewsCopyCache.Length - 1; num >= 0; num--)
				{
					if (!_mapViewsCopyCache[num].IsFinalized)
					{
						_mapViewsCopyCache[num].OnFrameTick(dt);
					}
				}
			}
		}
		_conversationOverThisFrame = false;
	}

	void IMapStateHandler.OnIdleTick(float dt)
	{
		HandleIfSceneIsReady();
		RemoveMapTooltip();
		if (_mapViewsCopyCache.Length != _mapViews.Count || !_mapViewsCopyCache.SequenceEqual(_mapViews))
		{
			_mapViewsCopyCache = new MapView[_mapViews.Count];
			_mapViews.CopyTo(_mapViewsCopyCache, 0);
		}
		for (int num = _mapViewsCopyCache.Length - 1; num >= 0; num--)
		{
			if (!_mapViewsCopyCache[num].IsFinalized)
			{
				_mapViewsCopyCache[num].OnIdleTick(dt);
			}
		}
		_conversationOverThisFrame = false;
	}

	protected override void OnFrameTick(float dt)
	{
		base.OnFrameTick(dt);
		MBDebug.SetErrorReportScene(_mapScene);
		UpdateMenuView();
		if (IsInMenu)
		{
			_menuViewContext.OnFrameTick(dt);
			if (SceneLayer.Input.IsGameKeyPressed(4))
			{
				GameMenuOption leaveMenuOption = Campaign.Current.GameMenuManager.GetLeaveMenuOption(_menuViewContext.MenuContext);
				if (leaveMenuOption != null)
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					if (_menuViewContext.MenuContext.GameMenu.IsWaitMenu)
					{
						_menuViewContext.MenuContext.GameMenu.EndWait();
					}
					leaveMenuOption.RunConsequence(_menuViewContext.MenuContext);
				}
			}
		}
		else if (Campaign.Current != null && !IsInBattleSimulation && !IsInArmyManagement && !IsMarriageOfferPopupActive && !IsMapCheatsActive && Clan.PlayerClan.Kingdom?.UnresolvedDecisions?.FirstOrDefault((KingdomDecision d) => d.NeedsPlayerResolution && !d.ShouldBeCancelled()) != null)
		{
			OpenKingdom();
		}
		if (_partyIconNeedsRefreshing)
		{
			_partyIconNeedsRefreshing = false;
			PartyBase.MainParty.SetVisualAsDirty();
		}
		for (int num = _mapViews.Count - 1; num >= 0; num--)
		{
			_mapViews[num].OnMapScreenUpdate(dt);
		}
		RefreshMapSiegeOverlayRequired();
		if (PlayerSiege.PlayerSiegeEvent != null && _playerSiegeMachineSlotMeshesAdded)
		{
			TickSiegeMachineCircles();
		}
		_timeSinceCreation += dt;
	}

	private void UpdateMenuView()
	{
		if (_latestMenuContext == null && IsInMenu)
		{
			ExitMenuContext();
		}
		else if ((!IsInMenu && _latestMenuContext != null) || (IsInMenu && _menuViewContext.MenuContext != _latestMenuContext))
		{
			EnterMenuContext(_latestMenuContext);
		}
	}

	private void EnterMenuContext(MenuContext menuContext)
	{
		_mapCameraView.SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
		Campaign.Current.CameraFollowParty = PartyBase.MainParty;
		if (!IsInMenu)
		{
			_menuViewContext = new MenuViewContext(this, menuContext);
		}
		else
		{
			_menuViewContext.UpdateMenuContext(menuContext);
		}
		_menuViewContext.OnInitialize();
		_menuViewContext.OnActivate();
		if (_mapConversationView != null)
		{
			_menuViewContext.OnMapConversationActivated();
		}
	}

	private void ExitMenuContext()
	{
		_menuViewContext.OnGameStateDeactivate();
		_menuViewContext.OnDeactivate();
		_menuViewContext.OnFinalize();
		_menuViewContext = null;
	}

	private void OpenBannerEditorScreen()
	{
		if (Campaign.Current.IsBannerEditorEnabled)
		{
			_partyIconNeedsRefreshing = true;
			Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<BannerEditorState>());
		}
	}

	private void OpenFaceGeneratorScreen()
	{
		if (Campaign.Current.IsFaceGenEnabled)
		{
			IFaceGeneratorCustomFilter faceGeneratorFilter = CharacterHelper.GetFaceGeneratorFilter();
			BarberState gameState = Game.Current.GameStateManager.CreateState<BarberState>(new object[2]
			{
				Hero.MainHero.CharacterObject,
				faceGeneratorFilter
			});
			GameStateManager.Current.PushState(gameState);
		}
	}

	public void OnExit()
	{
		_mapCameraView.OnExit();
		MBGameManager.EndGame();
	}

	private void SetMapSiegeOverlayState(bool isActive)
	{
		_mapCameraView.OnSetMapSiegeOverlayState(isActive, _mapSiegeOverlayView == null);
		if (_mapSiegeOverlayView != null && !isActive)
		{
			RemoveMapView(_mapSiegeOverlayView);
			_mapSiegeOverlayView = null;
		}
		else if (_mapSiegeOverlayView == null && isActive && PlayerSiege.PlayerSiegeEvent != null)
		{
			_mapSiegeOverlayView = AddMapView<MapSiegeOverlayView>(Array.Empty<object>());
			if (!_playerSiegeMachineSlotMeshesAdded)
			{
				InitializeSiegeCircleVisuals();
				_playerSiegeMachineSlotMeshesAdded = true;
			}
		}
	}

	private void RefreshMapSiegeOverlayRequired()
	{
		_mapCameraView.OnRefreshMapSiegeOverlayRequired(_mapSiegeOverlayView == null);
		if (_playerSiegeMachineSlotMeshesAdded && PlayerSiege.PlayerSiegeEvent != null)
		{
			Settlement besiegedSettlement = PlayerSiege.PlayerSiegeEvent.BesiegedSettlement;
			if (besiegedSettlement != null && besiegedSettlement.CurrentSiegeState == Settlement.SiegeState.InTheLordsHall)
			{
				RemoveSiegeCircleVisuals();
				_playerSiegeMachineSlotMeshesAdded = false;
				return;
			}
		}
		if (PlayerSiege.PlayerSiegeEvent == null && _mapSiegeOverlayView != null)
		{
			RemoveMapView(_mapSiegeOverlayView);
			_mapSiegeOverlayView = null;
			if (_playerSiegeMachineSlotMeshesAdded)
			{
				RemoveSiegeCircleVisuals();
				_playerSiegeMachineSlotMeshesAdded = false;
			}
		}
		else if (PlayerSiege.PlayerSiegeEvent != null && _mapSiegeOverlayView == null)
		{
			_mapSiegeOverlayView = AddMapView<MapSiegeOverlayView>(Array.Empty<object>());
			if (!_playerSiegeMachineSlotMeshesAdded)
			{
				InitializeSiegeCircleVisuals();
				_playerSiegeMachineSlotMeshesAdded = true;
			}
		}
	}

	private void OnEscapeMenuToggled(bool isOpened = false)
	{
		_mapCameraView.OnEscapeMenuToggled(isOpened);
		if (IsEscapeMenuOpened != isOpened)
		{
			IsEscapeMenuOpened = isOpened;
			if (isOpened)
			{
				List<EscapeMenuItemVM> escapeMenuItems = GetEscapeMenuItems();
				Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
				_escapeMenuView = AddMapView<MapEscapeMenuView>(new object[1] { escapeMenuItems });
			}
			else
			{
				RemoveMapView(_escapeMenuView);
				_escapeMenuView = null;
				Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
			}
		}
	}

	private void CheckValidityOfItems()
	{
		foreach (ItemObject objectType in MBObjectManager.Instance.GetObjectTypeList<ItemObject>())
		{
			if (!objectType.IsUsingTeamColor)
			{
				continue;
			}
			MetaMesh copy = MetaMesh.GetCopy(objectType.MultiMeshName, showErrors: false);
			for (int i = 0; i < copy.MeshCount; i++)
			{
				Material material = copy.GetMeshAtIndex(i).GetMaterial();
				if (material.Name != "vertex_color_lighting_skinned" && material.Name != "vertex_color_lighting" && material.GetTexture(Material.MBTextureType.DiffuseMap2) == null)
				{
					MBDebug.ShowWarning(string.Concat("Item object(", objectType.Name, ") has 'Using Team Color' flag but does not have a mask texture in diffuse2 slot. "));
					break;
				}
			}
		}
	}

	public void GetCursorIntersectionPoint(ref Vec3 clippedMouseNear, ref Vec3 clippedMouseFar, out float closestDistanceSquared, out Vec3 intersectionPoint, ref PathFaceRecord currentFace, BodyFlags excludedBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
	{
		(clippedMouseFar - clippedMouseNear).Normalize();
		Vec3 vec = clippedMouseFar - clippedMouseNear;
		float maxDistance = vec.Normalize();
		_mouseRay.Reset(clippedMouseNear, vec, maxDistance);
		intersectionPoint = Vec3.Zero;
		closestDistanceSquared = 1E+12f;
		if (SceneLayer.SceneView.RayCastForClosestEntityOrTerrain(clippedMouseNear, clippedMouseFar, out var collisionDistance, out var _, 0.01f, excludedBodyFlags))
		{
			closestDistanceSquared = collisionDistance * collisionDistance;
			intersectionPoint = clippedMouseNear + vec * collisionDistance;
		}
		currentFace = Campaign.Current.MapSceneWrapper.GetFaceIndex(intersectionPoint.AsVec2);
	}

	public void FastMoveCameraToPosition(Vec2 target)
	{
		_mapCameraView.FastMoveCameraToPosition(target, IsInMenu);
	}

	private void HandleMouse(float dt)
	{
		if (!Campaign.Current.GameStarted)
		{
			return;
		}
		Track track = null;
		Vec3 worldMouseNear = Vec3.Zero;
		Vec3 worldMouseFar = Vec3.Zero;
		SceneLayer.SceneView.TranslateMouse(ref worldMouseNear, ref worldMouseFar);
		Vec3 clippedMouseNear = worldMouseNear;
		Vec3 clippedMouseFar = worldMouseFar;
		PathFaceRecord currentFace = PathFaceRecord.NullFaceRecord;
		CheckCursorState();
		GetCursorIntersectionPoint(ref clippedMouseNear, ref clippedMouseFar, out var closestDistanceSquared, out var _, ref currentFace);
		GetCursorIntersectionPoint(ref clippedMouseNear, ref clippedMouseFar, out var _, out var intersectionPoint2, ref currentFace, BodyFlags.CommonFocusRayCastExcludeFlags | BodyFlags.Moveable);
		int num = _mapScene.SelectEntitiesCollidedWith(ref _mouseRay, _intersectionInfos, _intersectedEntityIDs);
		bool flag = false;
		float num2 = TaleWorlds.Library.MathF.Sqrt(closestDistanceSquared) + 1f;
		float num3 = num2;
		PartyVisual partyVisual = null;
		PartyVisual partyVisual2 = null;
		bool flag2 = false;
		for (int num4 = num - 1; num4 >= 0; num4--)
		{
			UIntPtr uIntPtr = _intersectedEntityIDs[num4];
			if (uIntPtr != UIntPtr.Zero)
			{
				if (VisualsOfEntities.TryGetValue(uIntPtr, out var value) && value.IsVisibleOrFadingOut())
				{
					PartyVisual partyVisual3 = value;
					Intersection intersection = _intersectionInfos[num4];
					float num5 = (worldMouseNear - intersection.IntersectionPoint).Length;
					if (partyVisual3.PartyBase.IsMobile)
					{
						num5 -= 1.5f;
					}
					if (num5 < num3)
					{
						num3 = num5;
						partyVisual = ((partyVisual3.PartyBase.IsMobile && partyVisual3.PartyBase.MobileParty.AttachedTo != null) ? PartyVisualManager.Current.GetVisualOfParty(partyVisual3.PartyBase.MobileParty.AttachedTo.Party) : value);
						flag = true;
					}
					if (num5 < num2 && (!partyVisual3.PartyBase.IsMobile || (partyVisual3.PartyBase != PartyBase.MainParty && (partyVisual3.PartyBase.MobileParty.AttachedTo == null || partyVisual3.PartyBase.MobileParty.AttachedTo != MobileParty.MainParty))))
					{
						num2 = num5;
						partyVisual2 = ((!partyVisual3.PartyBase.IsMobile || partyVisual3.PartyBase.MobileParty.AttachedTo == null) ? partyVisual3 : PartyVisualManager.Current.GetVisualOfParty(partyVisual3.PartyBase.MobileParty.AttachedTo.Party));
					}
				}
				else if (ScreenManager.FirstHitLayer == SceneLayer && FrameAndVisualOfEngines.ContainsKey(uIntPtr))
				{
					flag2 = true;
					if (_preSelectedSiegeEntityID != uIntPtr)
					{
						Tuple<MatrixFrame, PartyVisual> tuple = FrameAndVisualOfEngines[uIntPtr];
						tuple.Item2.OnMapHoverSiegeEngine(tuple.Item1);
						_preSelectedSiegeEntityID = uIntPtr;
					}
				}
			}
		}
		if (!flag2)
		{
			HandleSiegeEngineHoverEnd();
		}
		Array.Clear(_intersectedEntityIDs, 0, num);
		Array.Clear(_intersectionInfos, 0, num);
		if (flag)
		{
			if (_displayedContextMenuType < 0)
			{
				SceneLayer.ActiveCursor = CursorType.Default;
			}
		}
		else
		{
			track = _campaign.GetEntityComponent<MapTracksVisual>().GetTrackOnMouse(_mouseRay, intersectionPoint2);
		}
		float gameKeyAxis = SceneLayer.Input.GetGameKeyAxis("CameraAxisY");
		_mapCameraView.HandleMouse(SceneLayer.Input.IsKeyDown(InputKey.RightMouseButton), gameKeyAxis, SceneLayer.Input.GetMouseMoveY(), dt);
		if (SceneLayer.Input.IsKeyDown(InputKey.RightMouseButton))
		{
			MBWindowManager.DontChangeCursorPos();
		}
		if (ScreenManager.FirstHitLayer == SceneLayer && SceneLayer.Input.IsHotKeyReleased("MapClick") && !_leftButtonDraggingMode && !_ignoreLeftMouseRelease)
		{
			if (_leftButtonDoubleClickOnSceneWidget)
			{
				HandleLeftMouseButtonClick(_preSelectedSiegeEntityID, _preVisualOfSelectedEntity, intersectionPoint2, currentFace);
			}
			else
			{
				HandleLeftMouseButtonClick(_preSelectedSiegeEntityID, partyVisual2, intersectionPoint2, currentFace);
				_preVisualOfSelectedEntity = partyVisual2;
			}
		}
		if (Campaign.Current.TimeControlMode == CampaignTimeControlMode.StoppableFastForward && _waitForDoubleClickUntilTime > 0f && _waitForDoubleClickUntilTime < Time.ApplicationTime)
		{
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.StoppablePlay;
			_waitForDoubleClickUntilTime = 0f;
		}
		if (ScreenManager.FirstHitLayer == SceneLayer)
		{
			if (partyVisual != null)
			{
				if (CurrentVisualOfTooltip != partyVisual)
				{
					RemoveMapTooltip();
				}
				IMapEntity mapEntity = partyVisual.GetMapEntity();
				if (SceneLayer.Input.IsGameKeyPressed(66))
				{
					mapEntity.OnOpenEncyclopedia();
					_mapCursor.SetVisible(value: false);
				}
				if (mapEntity is ITrackableCampaignObject obj && SceneLayer.Input.IsGameKeyPressed(65))
				{
					if (Campaign.Current.VisualTrackerManager.CheckTracked(obj))
					{
						Campaign.Current.VisualTrackerManager.RemoveTrackedObject(obj);
					}
					else
					{
						Campaign.Current.VisualTrackerManager.RegisterObject(obj);
					}
				}
				OnHoverMapEntity(mapEntity);
				CurrentVisualOfTooltip = partyVisual;
			}
			else if (track != null)
			{
				CurrentVisualOfTooltip = null;
				SetupMapTooltipForTrack(track);
			}
			else if (!TooltipHandlingDisabled)
			{
				CurrentVisualOfTooltip = null;
				RemoveMapTooltip();
			}
		}
		else
		{
			CurrentVisualOfTooltip = null;
			RemoveMapTooltip();
			HandleSiegeEngineHoverEnd();
		}
	}

	private void HandleLeftMouseButtonClick(UIntPtr selectedSiegeEntityID, PartyVisual visualOfSelectedEntity, Vec3 intersectionPoint, PathFaceRecord mouseOverFaceIndex)
	{
		_mapCameraView.HandleLeftMouseButtonClick(SceneLayer.Input.GetIsMouseActive());
		if (!_mapState.AtMenu)
		{
			if (visualOfSelectedEntity?.GetMapEntity() != null)
			{
				IMapEntity mapEntity = visualOfSelectedEntity.GetMapEntity();
				if (visualOfSelectedEntity.PartyBase == PartyBase.MainParty)
				{
					MobileParty.MainParty.Ai.SetMoveModeHold();
					return;
				}
				PathFaceRecord faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(mapEntity.InteractionPosition);
				if (!_mapScene.DoesPathExistBetweenFaces(faceIndex.FaceIndex, MobileParty.MainParty.CurrentNavigationFace.FaceIndex, ignoreDisabled: false) || !_mapCameraView.ProcessCameraInput || PartyBase.MainParty.MapEvent != null)
				{
					return;
				}
				if (mapEntity.OnMapClick(SceneLayer.Input.IsHotKeyDown("MapFollowModifier")))
				{
					if (!_leftButtonDoubleClickOnSceneWidget && Campaign.Current.TimeControlMode == CampaignTimeControlMode.StoppableFastForward)
					{
						_waitForDoubleClickUntilTime = Time.ApplicationTime + 0.3f;
						Campaign.Current.TimeControlMode = CampaignTimeControlMode.StoppableFastForward;
					}
					else
					{
						Campaign.Current.TimeControlMode = (_leftButtonDoubleClickOnSceneWidget ? CampaignTimeControlMode.StoppableFastForward : CampaignTimeControlMode.StoppablePlay);
					}
					if (TaleWorlds.InputSystem.Input.IsGamepadActive)
					{
						if (mapEntity.IsMobileEntity)
						{
							if (mapEntity.IsAllyOf(PartyBase.MainParty.MapFaction))
							{
								UISoundsHelper.PlayUISound("event:/ui/campaign/click_party");
							}
							else
							{
								UISoundsHelper.PlayUISound("event:/ui/campaign/click_party_enemy");
							}
						}
						else if (mapEntity.IsAllyOf(PartyBase.MainParty.MapFaction))
						{
							UISoundsHelper.PlayUISound("event:/ui/campaign/click_settlement");
						}
						else
						{
							UISoundsHelper.PlayUISound("event:/ui/campaign/click_settlement_enemy");
						}
					}
				}
				MobileParty.MainParty.Ai.ForceAiNoPathMode = false;
			}
			else
			{
				if (!mouseOverFaceIndex.IsValid())
				{
					return;
				}
				bool flag;
				if (Input.IsControlDown() && Game.Current.CheatMode)
				{
					if (MobileParty.MainParty.Army != null)
					{
						foreach (MobileParty attachedParty in MobileParty.MainParty.Army.LeaderParty.AttachedParties)
						{
							attachedParty.Position2D += intersectionPoint.AsVec2 - MobileParty.MainParty.Position2D;
						}
					}
					MobileParty.MainParty.Position2D = intersectionPoint.AsVec2;
					MobileParty.MainParty.Ai.SetMoveModeHold();
					foreach (MobileParty item in MobileParty.All)
					{
						item.Party.UpdateVisibilityAndInspected();
					}
					foreach (Settlement item2 in Settlement.All)
					{
						item2.Party.UpdateVisibilityAndInspected();
					}
					MBDebug.Print("main party cheat move! - " + intersectionPoint.x + " " + intersectionPoint.y);
					flag = true;
				}
				else
				{
					flag = Campaign.Current.MapSceneWrapper.AreFacesOnSameIsland(mouseOverFaceIndex, MobileParty.MainParty.CurrentNavigationFace, ignoreDisabled: false);
				}
				if (flag && _mapCameraView.ProcessCameraInput && MobileParty.MainParty.MapEvent == null)
				{
					_mapState.ProcessTravel(intersectionPoint.AsVec2);
					if (!_leftButtonDoubleClickOnSceneWidget && Campaign.Current.TimeControlMode == CampaignTimeControlMode.StoppableFastForward)
					{
						_waitForDoubleClickUntilTime = Time.ApplicationTime + 0.3f;
						Campaign.Current.TimeControlMode = CampaignTimeControlMode.StoppableFastForward;
					}
					else
					{
						Campaign.Current.TimeControlMode = (_leftButtonDoubleClickOnSceneWidget ? CampaignTimeControlMode.StoppableFastForward : CampaignTimeControlMode.StoppablePlay);
					}
				}
				OnTerrainClick();
			}
		}
		else if (selectedSiegeEntityID != UIntPtr.Zero)
		{
			Tuple<MatrixFrame, PartyVisual> tuple = FrameAndVisualOfEngines[selectedSiegeEntityID];
			OnSiegeEngineFrameClick(tuple.Item1);
		}
		else
		{
			OnTerrainClick();
		}
	}

	private void OnTerrainClick()
	{
		foreach (MapView mapView in _mapViews)
		{
			mapView.OnMapTerrainClick();
		}
		_mapCursor.OnMapTerrainClick();
	}

	private void OnSiegeEngineFrameClick(MatrixFrame siegeFrame)
	{
		foreach (MapView mapView in _mapViews)
		{
			mapView.OnSiegeEngineClick(siegeFrame);
		}
	}

	private void InitializeSiegeCircleVisuals()
	{
		Settlement besiegedSettlement = PlayerSiege.PlayerSiegeEvent.BesiegedSettlement;
		PartyVisual visualOfParty = PartyVisualManager.Current.GetVisualOfParty(besiegedSettlement.Party);
		MapScene mapScene = Campaign.Current.MapSceneWrapper as MapScene;
		MatrixFrame[] defenderRangedSiegeEngineFrames = visualOfParty.GetDefenderRangedSiegeEngineFrames();
		_defenderMachinesCircleEntities = new GameEntity[defenderRangedSiegeEngineFrames.Length];
		for (int i = 0; i < defenderRangedSiegeEngineFrames.Length; i++)
		{
			MatrixFrame matrixFrame = defenderRangedSiegeEngineFrames[i];
			_defenderMachinesCircleEntities[i] = GameEntity.CreateEmpty(mapScene.Scene);
			_defenderMachinesCircleEntities[i].Name = "dRangedMachineCircle_" + i;
			Decal decal = Decal.CreateDecal();
			decal.SetMaterial(Material.GetFromResource(_defenderRangedMachineDecalMaterialName));
			decal.SetFactor1Linear(_preperationOrEnemySiegeEngineDecalColor);
			_defenderMachinesCircleEntities[i].AddComponent(decal);
			MatrixFrame frame = matrixFrame;
			if (_isNewDecalScaleImplementationEnabled)
			{
				frame.Scale(new Vec3(_defenderMachineCircleDecalScale, _defenderMachineCircleDecalScale, _defenderMachineCircleDecalScale));
			}
			_defenderMachinesCircleEntities[i].SetGlobalFrame(in frame);
			_defenderMachinesCircleEntities[i].SetVisibilityExcludeParents(visible: true);
			mapScene.Scene.AddDecalInstance(decal, "editor_set", deletable: true);
		}
		defenderRangedSiegeEngineFrames = visualOfParty.GetAttackerBatteringRamSiegeEngineFrames();
		_attackerRamMachinesCircleEntities = new GameEntity[defenderRangedSiegeEngineFrames.Length];
		for (int j = 0; j < defenderRangedSiegeEngineFrames.Length; j++)
		{
			MatrixFrame matrixFrame2 = defenderRangedSiegeEngineFrames[j];
			_attackerRamMachinesCircleEntities[j] = GameEntity.CreateEmpty(mapScene.Scene);
			_attackerRamMachinesCircleEntities[j].Name = "InitializeSiegeCircleVisuals";
			_attackerRamMachinesCircleEntities[j].Name = "aRamMachineCircle_" + j;
			Decal decal2 = Decal.CreateDecal();
			decal2.SetMaterial(Material.GetFromResource(_attackerRamMachineDecalMaterialName));
			decal2.SetFactor1Linear(_preperationOrEnemySiegeEngineDecalColor);
			_attackerRamMachinesCircleEntities[j].AddComponent(decal2);
			MatrixFrame frame2 = matrixFrame2;
			if (_isNewDecalScaleImplementationEnabled)
			{
				frame2.Scale(new Vec3(_attackerMachineDecalScale, _attackerMachineDecalScale, _attackerMachineDecalScale));
			}
			_attackerRamMachinesCircleEntities[j].SetGlobalFrame(in frame2);
			_attackerRamMachinesCircleEntities[j].SetVisibilityExcludeParents(visible: true);
			mapScene.Scene.AddDecalInstance(decal2, "editor_set", deletable: true);
		}
		defenderRangedSiegeEngineFrames = visualOfParty.GetAttackerTowerSiegeEngineFrames();
		_attackerTowerMachinesCircleEntities = new GameEntity[defenderRangedSiegeEngineFrames.Length];
		for (int k = 0; k < defenderRangedSiegeEngineFrames.Length; k++)
		{
			MatrixFrame matrixFrame3 = defenderRangedSiegeEngineFrames[k];
			_attackerTowerMachinesCircleEntities[k] = GameEntity.CreateEmpty(mapScene.Scene);
			_attackerTowerMachinesCircleEntities[k].Name = "aTowerMachineCircle_" + k;
			Decal decal3 = Decal.CreateDecal();
			decal3.SetMaterial(Material.GetFromResource(_attackerTowerMachineDecalMaterialName));
			decal3.SetFactor1Linear(_preperationOrEnemySiegeEngineDecalColor);
			_attackerTowerMachinesCircleEntities[k].AddComponent(decal3);
			MatrixFrame frame3 = matrixFrame3;
			if (_isNewDecalScaleImplementationEnabled)
			{
				frame3.Scale(new Vec3(_attackerMachineDecalScale, _attackerMachineDecalScale, _attackerMachineDecalScale));
			}
			_attackerTowerMachinesCircleEntities[k].SetGlobalFrame(in frame3);
			_attackerTowerMachinesCircleEntities[k].SetVisibilityExcludeParents(visible: true);
			mapScene.Scene.AddDecalInstance(decal3, "editor_set", deletable: true);
		}
		defenderRangedSiegeEngineFrames = visualOfParty.GetAttackerRangedSiegeEngineFrames();
		_attackerRangedMachinesCircleEntities = new GameEntity[defenderRangedSiegeEngineFrames.Length];
		for (int l = 0; l < defenderRangedSiegeEngineFrames.Length; l++)
		{
			MatrixFrame matrixFrame4 = defenderRangedSiegeEngineFrames[l];
			_attackerRangedMachinesCircleEntities[l] = GameEntity.CreateEmpty(mapScene.Scene);
			_attackerRangedMachinesCircleEntities[l].Name = "aRangedMachineCircle_" + l;
			Decal decal4 = Decal.CreateDecal();
			decal4.SetMaterial(Material.GetFromResource(_emptyAttackerRangedDecalMaterialName));
			decal4.SetFactor1Linear(_preperationOrEnemySiegeEngineDecalColor);
			_attackerRangedMachinesCircleEntities[l].AddComponent(decal4);
			MatrixFrame frame4 = matrixFrame4;
			if (_isNewDecalScaleImplementationEnabled)
			{
				frame4.Scale(new Vec3(_attackerMachineDecalScale, _attackerMachineDecalScale, _attackerMachineDecalScale));
			}
			_attackerRangedMachinesCircleEntities[l].SetGlobalFrame(in frame4);
			_attackerRangedMachinesCircleEntities[l].SetVisibilityExcludeParents(visible: true);
			mapScene.Scene.AddDecalInstance(decal4, "editor_set", deletable: true);
		}
	}

	private void TickSiegeMachineCircles()
	{
		SiegeEvent playerSiegeEvent = PlayerSiege.PlayerSiegeEvent;
		bool isPlayerLeader = playerSiegeEvent != null && playerSiegeEvent.IsPlayerSiegeEvent && Campaign.Current.Models.EncounterModel.GetLeaderOfSiegeEvent(playerSiegeEvent, PlayerSiege.PlayerSide) == Hero.MainHero;
		bool isPreparationComplete = playerSiegeEvent.BesiegerCamp.IsPreparationComplete;
		Settlement besiegedSettlement = playerSiegeEvent.BesiegedSettlement;
		PartyVisual visualOfParty = PartyVisualManager.Current.GetVisualOfParty(besiegedSettlement.Party);
		Tuple<MatrixFrame, PartyVisual> tuple = null;
		if (_preSelectedSiegeEntityID != UIntPtr.Zero)
		{
			tuple = FrameAndVisualOfEngines[_preSelectedSiegeEntityID];
		}
		for (int i = 0; i < visualOfParty.GetDefenderRangedSiegeEngineFrames().Length; i++)
		{
			bool isEmpty = playerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Defender).SiegeEngines.DeployedRangedSiegeEngines[i] == null;
			bool isEnemy = PlayerSiege.PlayerSide != BattleSideEnum.Defender;
			string desiredMaterialName = GetDesiredMaterialName(isRanged: true, isAttacker: false, isEmpty, isTower: false);
			Decal decal = _defenderMachinesCircleEntities[i].GetComponentAtIndex(0, GameEntity.ComponentType.Decal) as Decal;
			if (decal.GetMaterial()?.Name != desiredMaterialName)
			{
				decal.SetMaterial(Material.GetFromResource(desiredMaterialName));
			}
			bool isHovered = tuple != null && _defenderMachinesCircleEntities[i].GetGlobalFrame().NearlyEquals(tuple.Item1);
			uint desiredDecalColor = GetDesiredDecalColor(isPreparationComplete, isHovered, isEnemy, isEmpty, isPlayerLeader);
			if (desiredDecalColor != decal.GetFactor1())
			{
				decal.SetFactor1(desiredDecalColor);
			}
		}
		for (int j = 0; j < visualOfParty.GetAttackerRangedSiegeEngineFrames().Length; j++)
		{
			bool isEmpty2 = playerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.DeployedRangedSiegeEngines[j] == null;
			bool isEnemy2 = PlayerSiege.PlayerSide != BattleSideEnum.Attacker;
			string desiredMaterialName2 = GetDesiredMaterialName(isRanged: true, isAttacker: true, isEmpty2, isTower: false);
			Decal decal2 = _attackerRangedMachinesCircleEntities[j].GetComponentAtIndex(0, GameEntity.ComponentType.Decal) as Decal;
			if (decal2.GetMaterial()?.Name != desiredMaterialName2)
			{
				decal2.SetMaterial(Material.GetFromResource(desiredMaterialName2));
			}
			bool isHovered2 = tuple != null && _attackerRangedMachinesCircleEntities[j].GetGlobalFrame().NearlyEquals(tuple.Item1);
			uint desiredDecalColor2 = GetDesiredDecalColor(isPreparationComplete, isHovered2, isEnemy2, isEmpty2, isPlayerLeader);
			if (desiredDecalColor2 != decal2.GetFactor1())
			{
				decal2.SetFactor1(desiredDecalColor2);
			}
		}
		for (int k = 0; k < visualOfParty.GetAttackerBatteringRamSiegeEngineFrames().Length; k++)
		{
			bool isEmpty3 = playerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.DeployedMeleeSiegeEngines[k] == null;
			bool isEnemy3 = PlayerSiege.PlayerSide != BattleSideEnum.Attacker;
			string desiredMaterialName3 = GetDesiredMaterialName(isRanged: false, isAttacker: true, isEmpty3, isTower: false);
			Decal decal3 = _attackerRamMachinesCircleEntities[k].GetComponentAtIndex(0, GameEntity.ComponentType.Decal) as Decal;
			if (decal3.GetMaterial()?.Name != desiredMaterialName3)
			{
				decal3.SetMaterial(Material.GetFromResource(desiredMaterialName3));
			}
			bool isHovered3 = tuple != null && _attackerRamMachinesCircleEntities[k].GetGlobalFrame().NearlyEquals(tuple.Item1);
			uint desiredDecalColor3 = GetDesiredDecalColor(isPreparationComplete, isHovered3, isEnemy3, isEmpty3, isPlayerLeader);
			if (desiredDecalColor3 != decal3.GetFactor1())
			{
				decal3.SetFactor1(desiredDecalColor3);
			}
		}
		for (int l = 0; l < visualOfParty.GetAttackerTowerSiegeEngineFrames().Length; l++)
		{
			bool isEmpty4 = playerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.DeployedMeleeSiegeEngines[visualOfParty.GetAttackerBatteringRamSiegeEngineFrames().Length + l] == null;
			bool isEnemy4 = PlayerSiege.PlayerSide != BattleSideEnum.Attacker;
			string desiredMaterialName4 = GetDesiredMaterialName(isRanged: false, isAttacker: true, isEmpty4, isTower: true);
			Decal decal4 = _attackerTowerMachinesCircleEntities[l].GetComponentAtIndex(0, GameEntity.ComponentType.Decal) as Decal;
			if (decal4.GetMaterial()?.Name != desiredMaterialName4)
			{
				decal4.SetMaterial(Material.GetFromResource(desiredMaterialName4));
			}
			bool isHovered4 = tuple != null && _attackerTowerMachinesCircleEntities[l].GetGlobalFrame().NearlyEquals(tuple.Item1);
			uint desiredDecalColor4 = GetDesiredDecalColor(isPreparationComplete, isHovered4, isEnemy4, isEmpty4, isPlayerLeader);
			if (desiredDecalColor4 != decal4.GetFactor1())
			{
				decal4.SetFactor1(desiredDecalColor4);
			}
		}
	}

	private uint GetDesiredDecalColor(bool isPrepOver, bool isHovered, bool isEnemy, bool isEmpty, bool isPlayerLeader)
	{
		isPrepOver = true;
		if (isPrepOver && !isEnemy)
		{
			if (isHovered && isPlayerLeader)
			{
				return _hoveredSiegeEngineDecalColor;
			}
			if (!isEmpty)
			{
				return _withMachineSiegeEngineDecalColor;
			}
			if (isPlayerLeader)
			{
				float ratio = TaleWorlds.Library.MathF.PingPong(0f, _machineDecalAnimLoopTime, _timeSinceCreation) / _machineDecalAnimLoopTime;
				Color start = Color.FromUint(_normalStartSiegeEngineDecalColor);
				Color end = Color.FromUint(_normalEndSiegeEngineDecalColor);
				return Color.Lerp(start, end, ratio).ToUnsignedInteger();
			}
			return _normalStartSiegeEngineDecalColor;
		}
		return _preperationOrEnemySiegeEngineDecalColor;
	}

	private string GetDesiredMaterialName(bool isRanged, bool isAttacker, bool isEmpty, bool isTower)
	{
		if (isRanged)
		{
			if (!isAttacker)
			{
				return _defenderRangedMachineDecalMaterialName;
			}
			return _attackerRangedMachineDecalMaterialName;
		}
		if (!isTower)
		{
			return _attackerRamMachineDecalMaterialName;
		}
		return _attackerTowerMachineDecalMaterialName;
	}

	private void RemoveSiegeCircleVisuals()
	{
		if (_playerSiegeMachineSlotMeshesAdded)
		{
			MapScene mapScene = Campaign.Current.MapSceneWrapper as MapScene;
			for (int i = 0; i < _defenderMachinesCircleEntities.Length; i++)
			{
				_defenderMachinesCircleEntities[i].SetVisibilityExcludeParents(visible: false);
				mapScene.Scene.RemoveEntity(_defenderMachinesCircleEntities[i], 107);
				_defenderMachinesCircleEntities[i] = null;
			}
			for (int j = 0; j < _attackerRamMachinesCircleEntities.Length; j++)
			{
				_attackerRamMachinesCircleEntities[j].SetVisibilityExcludeParents(visible: false);
				mapScene.Scene.RemoveEntity(_attackerRamMachinesCircleEntities[j], 108);
				_attackerRamMachinesCircleEntities[j] = null;
			}
			for (int k = 0; k < _attackerTowerMachinesCircleEntities.Length; k++)
			{
				_attackerTowerMachinesCircleEntities[k].SetVisibilityExcludeParents(visible: false);
				mapScene.Scene.RemoveEntity(_attackerTowerMachinesCircleEntities[k], 109);
				_attackerTowerMachinesCircleEntities[k] = null;
			}
			for (int l = 0; l < _attackerRangedMachinesCircleEntities.Length; l++)
			{
				_attackerRangedMachinesCircleEntities[l].SetVisibilityExcludeParents(visible: false);
				mapScene.Scene.RemoveEntity(_attackerRangedMachinesCircleEntities[l], 110);
				_attackerRangedMachinesCircleEntities[l] = null;
			}
			_playerSiegeMachineSlotMeshesAdded = false;
		}
	}

	void IMapStateHandler.AfterTick(float dt)
	{
		if (ScreenManager.TopScreen == this)
		{
			TickVisuals(dt);
			SceneLayer sceneLayer = SceneLayer;
			if (sceneLayer != null && sceneLayer.Input.IsGameKeyPressed(53))
			{
				Campaign.Current.SaveHandler.QuickSaveCurrentGame();
			}
		}
		base.DebugInput.IsHotKeyPressed("MapScreenHotkeyShowPos");
	}

	void IMapStateHandler.AfterWaitTick(float dt)
	{
		if (SceneLayer.Input.IsShiftDown() || SceneLayer.Input.IsControlDown())
		{
			return;
		}
		bool flag = false;
		if (SceneLayer.Input.IsGameKeyPressed(38) && _navigationHandler.InventoryEnabled)
		{
			OpenInventory();
			flag = true;
		}
		else if (SceneLayer.Input.IsGameKeyPressed(43) && _navigationHandler.PartyEnabled)
		{
			OpenParty();
			flag = true;
		}
		else if (SceneLayer.Input.IsGameKeyPressed(39) && !IsInArmyManagement && !IsMapCheatsActive)
		{
			OpenEncyclopedia();
			flag = true;
		}
		else if (SceneLayer.Input.IsGameKeyPressed(36) && !IsInArmyManagement && !IsMarriageOfferPopupActive && !IsMapCheatsActive)
		{
			OpenBannerEditorScreen();
			flag = true;
		}
		else if (SceneLayer.Input.IsGameKeyPressed(40) && _navigationHandler.KingdomPermission.IsAuthorized)
		{
			OpenKingdom();
			flag = true;
		}
		else if (SceneLayer.Input.IsGameKeyPressed(42) && _navigationHandler.QuestsEnabled)
		{
			OpenQuestsScreen();
			flag = true;
		}
		else if (SceneLayer.Input.IsGameKeyPressed(41) && _navigationHandler.ClanPermission.IsAuthorized)
		{
			OpenClanScreen();
			flag = true;
		}
		else if (SceneLayer.Input.IsGameKeyPressed(37) && _navigationHandler.CharacterDeveloperEnabled)
		{
			OpenCharacterDevelopmentScreen();
			flag = true;
		}
		else if (SceneLayer.Input.IsHotKeyReleased("ToggleEscapeMenu"))
		{
			if (!_mapViews.Any((MapView m) => m.IsEscaped()))
			{
				OpenEscapeMenu();
				flag = true;
			}
		}
		else if (SceneLayer.Input.IsGameKeyPressed(44))
		{
			OpenFaceGeneratorScreen();
			flag = true;
		}
		else if (TaleWorlds.InputSystem.Input.IsGamepadActive)
		{
			HandleCheatMenuInput(dt);
		}
		if (flag)
		{
			_mapCursor.SetVisible(value: false);
		}
	}

	private void HandleCheatMenuInput(float dt)
	{
		if (!IsMapCheatsActive && Input.IsKeyDown(InputKey.ControllerLBumper) && Input.IsKeyDown(InputKey.ControllerRTrigger) && Input.IsKeyDown(InputKey.ControllerLDown))
		{
			_cheatPressTimer += dt;
			if (_cheatPressTimer > 0.55f)
			{
				OpenGameplayCheats();
			}
		}
		else
		{
			_cheatPressTimer = 0f;
		}
	}

	void IMapStateHandler.OnRefreshState()
	{
		if (!(Game.Current.GameStateManager.ActiveState is MapState))
		{
			return;
		}
		if (MobileParty.MainParty.Army != null && _armyOverlay == null)
		{
			AddArmyOverlay(GameOverlays.MapOverlayType.Army);
		}
		else if (MobileParty.MainParty.Army == null && _armyOverlay != null)
		{
			for (int num = _mapViews.Count - 1; num >= 0; num--)
			{
				_mapViews[num].OnArmyLeft();
			}
			for (int num2 = _mapViews.Count - 1; num2 >= 0; num2--)
			{
				_mapViews[num2].OnDispersePlayerLeadedArmy();
			}
		}
	}

	void IMapStateHandler.OnExitingMenuMode()
	{
		_latestMenuContext = null;
	}

	void IMapStateHandler.OnEnteringMenuMode(MenuContext menuContext)
	{
		_latestMenuContext = menuContext;
	}

	void IMapStateHandler.OnMainPartyEncounter()
	{
		for (int num = _mapViews.Count - 1; num >= 0; num--)
		{
			_mapViews[num].OnMainPartyEncounter();
		}
	}

	void IMapStateHandler.OnSignalPeriodicEvents()
	{
		DeleteMarkedPeriodicEvents();
	}

	void IMapStateHandler.OnBattleSimulationStarted(BattleSimulation battleSimulation)
	{
		IsInBattleSimulation = true;
		_battleSimulationView = AddMapView<BattleSimulationMapView>(new object[1] { battleSimulation });
	}

	void IMapStateHandler.OnBattleSimulationEnded()
	{
		IsInBattleSimulation = false;
		RemoveMapView(_battleSimulationView);
		_battleSimulationView = null;
	}

	void IMapStateHandler.OnSiegeEngineClick(MatrixFrame siegeEngineFrame)
	{
		_mapCameraView.SiegeEngineClick(siegeEngineFrame);
	}

	void IGameStateListener.OnInitialize()
	{
	}

	void IMapStateHandler.OnPlayerSiegeActivated()
	{
	}

	void IMapStateHandler.OnPlayerSiegeDeactivated()
	{
	}

	void IMapStateHandler.OnGameplayCheatsEnabled()
	{
		OpenGameplayCheats();
	}

	void IGameStateListener.OnActivate()
	{
	}

	void IGameStateListener.OnDeactivate()
	{
	}

	void IMapStateHandler.OnMapConversationStarts(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData)
	{
		if (_isReadyForRender || _conversationOverThisFrame)
		{
			HandleMapConversationInit(playerCharacterData, conversationPartnerData);
			return;
		}
		TempConversationStateHandler tempConversationStateHandler = new TempConversationStateHandler();
		_conversationDataCache = new Tuple<ConversationCharacterData, ConversationCharacterData, TempConversationStateHandler>(playerCharacterData, conversationPartnerData, tempConversationStateHandler);
		Campaign.Current.ConversationManager.Handler = tempConversationStateHandler;
		Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
	}

	private void HandleMapConversationInit(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData)
	{
		if (_mapConversationView == null)
		{
			for (int num = _mapViews.Count - 1; num >= 0; num--)
			{
				_mapViews[num].OnMapConversationStart();
			}
		}
		_menuViewContext?.OnMapConversationActivated();
		if (_mapConversationView == null)
		{
			_mapConversationView = AddMapView<MapConversationView>(new object[2] { playerCharacterData, conversationPartnerData });
		}
		else
		{
			for (int num2 = _mapViews.Count - 1; num2 >= 0; num2--)
			{
				_mapViews[num2].OnMapConversationUpdate(playerCharacterData, conversationPartnerData);
			}
		}
		_mapCursor.SetVisible(value: false);
		HandleIfSceneIsReady();
	}

	void IMapStateHandler.OnMapConversationOver()
	{
		_conversationOverThisFrame = true;
		for (int num = _mapViews.Count - 1; num >= 0; num--)
		{
			_mapViews[num].OnMapConversationOver();
		}
		_menuViewContext?.OnMapConversationDeactivated();
		HandleMapConversationOver();
		_activatedFrameNo = Utilities.EngineFrameNo;
		HandleIfSceneIsReady();
	}

	private void HandleMapConversationOver()
	{
		if (_mapConversationView != null)
		{
			RemoveMapView(_mapConversationView);
		}
		_mapConversationView = null;
	}

	private void InitializeVisuals()
	{
		InactiveLightMeshes = new List<Mesh>();
		ActiveLightMeshes = new List<Mesh>();
		MapScene mapScene = Campaign.Current.MapSceneWrapper as MapScene;
		_targetCircleEntitySmall = GameEntity.CreateEmpty(mapScene.Scene);
		_targetCircleEntitySmall.Name = "tCircleSmall";
		_targetCircleEntityBig = GameEntity.CreateEmpty(mapScene.Scene);
		_targetCircleEntityBig.Name = "tCircleBig";
		_targetCircleTown = GameEntity.CreateEmpty(mapScene.Scene);
		_targetCircleTown.Name = "tTown";
		_partyOutlineEntity = GameEntity.CreateEmpty(mapScene.Scene);
		_partyOutlineEntity.Name = "sCircle";
		_townOutlineEntity = GameEntity.CreateEmpty(mapScene.Scene);
		_townOutlineEntity.Name = "sSettlementOutline";
		_targetDecalMeshSmall = Decal.CreateDecal();
		if (_targetDecalMeshSmall != null)
		{
			_settlementOutlineMesh = _targetDecalMeshSmall.CreateCopy();
			Material fromResource = Material.GetFromResource("decal_city_circle_a");
			if (fromResource != null)
			{
				_settlementOutlineMesh.SetMaterial(fromResource);
			}
			_targetTownMesh = _settlementOutlineMesh.CreateCopy();
			_targetDecalMeshSmall = _targetDecalMeshSmall.CreateCopy();
			Material fromResource2 = Material.GetFromResource("map_circle_decal");
			if (fromResource2 != null)
			{
				_targetDecalMeshSmall.SetMaterial(fromResource2);
			}
			else
			{
				MBDebug.ShowWarning("Material(map_circle_decal) for party circles could not be found.");
			}
			_targetDecalMeshBig = _targetDecalMeshSmall.CreateCopy();
			_partyOutlineMesh = _targetDecalMeshSmall.CreateCopy();
			mapScene.Scene.AddDecalInstance(_targetDecalMeshSmall, "editor_set", deletable: false);
			mapScene.Scene.AddDecalInstance(_targetDecalMeshBig, "editor_set", deletable: false);
			mapScene.Scene.AddDecalInstance(_partyOutlineMesh, "editor_set", deletable: false);
			mapScene.Scene.AddDecalInstance(_settlementOutlineMesh, "editor_set", deletable: false);
			mapScene.Scene.AddDecalInstance(_targetTownMesh, "editor_set", deletable: false);
			_targetCircleEntitySmall.AddComponent(_targetDecalMeshSmall);
			_targetCircleEntityBig.AddComponent(_targetDecalMeshBig);
			_partyOutlineEntity.AddComponent(_partyOutlineMesh);
			_townOutlineEntity.AddComponent(_settlementOutlineMesh);
			_targetCircleTown.AddComponent(_targetTownMesh);
		}
		else
		{
			MBDebug.ShowWarning("Mesh(decal_mesh) for party circles could not be found.");
		}
		_mapCursor.Initialize(this);
		_campaign = Campaign.Current;
		_campaign.AddEntityComponent<MapTracksVisual>();
		_campaign.AddEntityComponent<MapWeatherVisualManager>();
		_campaign.AddEntityComponent<MapAudioManager>();
		_campaign.AddEntityComponent<PartyVisualManager>();
		ContourMaskEntity = GameEntity.CreateEmpty(mapScene.Scene);
		ContourMaskEntity.Name = "aContourMask";
	}

	internal void TickCircles(float realDt)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		float num = 0.5f;
		float num2 = 0.5f;
		int num3 = 0;
		int num4 = 0;
		uint factor1Linear = 4293199122u;
		uint factor1Linear2 = 4293199122u;
		uint factor1Linear3 = 4293199122u;
		bool flag4 = false;
		bool flag5 = false;
		MatrixFrame frame = MatrixFrame.Identity;
		PartyBase partyBase = null;
		if (MobileParty.MainParty.Ai.PartyMoveMode == MoveModeType.Point && MobileParty.MainParty.DefaultBehavior != AiBehavior.GoToSettlement && MobileParty.MainParty.DefaultBehavior != 0 && !MobileParty.MainParty.Ai.ForceAiNoPathMode && MobileParty.MainParty.MapEvent == null && MobileParty.MainParty.TargetPosition.DistanceSquared(MobileParty.MainParty.Position2D) > 0.01f)
		{
			flag = true;
			flag2 = true;
			num = 0.238846f;
			num2 = 0.278584f;
			num3 = 4;
			num4 = 5;
			factor1Linear = 4293993473u;
			factor1Linear2 = 4293993473u;
			frame.origin = new Vec3(MobileParty.MainParty.TargetPosition);
			flag5 = true;
		}
		else
		{
			if (MobileParty.MainParty.Ai.PartyMoveMode == MoveModeType.Party && MobileParty.MainParty.Ai.MoveTargetParty != null && MobileParty.MainParty.Ai.MoveTargetParty.IsVisible)
			{
				partyBase = ((MobileParty.MainParty.Ai.MoveTargetParty.CurrentSettlement != null && !MobileParty.MainParty.Ai.MoveTargetParty.CurrentSettlement.IsHideout) ? MobileParty.MainParty.Ai.MoveTargetParty.CurrentSettlement.Party : MobileParty.MainParty.Ai.MoveTargetParty.Party);
			}
			else if (MobileParty.MainParty.DefaultBehavior == AiBehavior.GoToSettlement && MobileParty.MainParty.TargetSettlement != null)
			{
				partyBase = MobileParty.MainParty.TargetSettlement.Party;
			}
			if (partyBase != null)
			{
				bool flag6 = FactionManager.IsAtWarAgainstFaction(partyBase.MapFaction, Hero.MainHero.MapFaction);
				bool flag7 = FactionManager.IsAlliedWithFaction(partyBase.MapFaction, Hero.MainHero.MapFaction);
				frame = PartyVisualManager.Current.GetVisualOfParty(partyBase).CircleLocalFrame;
				if (partyBase.IsMobile)
				{
					flag = true;
					num3 = GetCircleIndex();
					factor1Linear = (flag6 ? _enemyPartyDecalColor : (flag7 ? _allyPartyDecalColor : _neutralPartyDecalColor));
					num = frame.rotation.GetScaleVector().x * 1.2f;
				}
				else if (partyBase.IsSettlement && (partyBase.Settlement.IsTown || partyBase.Settlement.IsCastle))
				{
					flag4 = true;
					flag3 = true;
					factor1Linear3 = (flag6 ? _enemyPartyDecalColor : (flag7 ? _allyPartyDecalColor : _neutralPartyDecalColor));
					num = frame.rotation.GetScaleVector().x * 1.2f;
				}
				else
				{
					flag = true;
					num3 = 5;
					factor1Linear = (flag6 ? _enemyPartyDecalColor : (flag7 ? _allyPartyDecalColor : _neutralPartyDecalColor));
					num = frame.rotation.GetScaleVector().x * 1.2f;
				}
				if (!flag4)
				{
					frame.origin += new Vec3(partyBase.Position2D + (partyBase.IsMobile ? (partyBase.MobileParty.EventPositionAdder + partyBase.MobileParty.ArmyPositionAdder) : Vec2.Zero));
				}
			}
		}
		if (flag5)
		{
			float value = (_mapCameraView.CameraDistance + 80f) * (_mapCameraView.CameraDistance + 80f) / 5000f;
			value = TaleWorlds.Library.MathF.Clamp(value, 0.2f, 45f);
			num *= value;
			num2 *= value;
		}
		if (partyBase == null)
		{
			_targetCircleRotationStartTime = 0f;
		}
		else if (_targetCircleRotationStartTime == 0f)
		{
			_targetCircleRotationStartTime = MBCommon.GetApplicationTime();
		}
		Vec3 normalAt = _mapScene.GetNormalAt(frame.origin.AsVec2);
		if (!flag4)
		{
			Vec3 origin = _targetCircleTown.GetGlobalFrame().origin;
			frame.origin.z = ((origin.AsVec2 != frame.origin.AsVec2) ? _mapScene.GetTerrainHeight(frame.origin.AsVec2) : origin.z);
		}
		MatrixFrame frame2 = MatrixFrame.Identity;
		frame2.origin = frame.origin;
		frame2.rotation.u = normalAt;
		MatrixFrame frame3 = frame2;
		frame2.rotation.ApplyScaleLocal(new Vec3(num, num, num));
		frame3.rotation.ApplyScaleLocal(new Vec3(num2, num2, num2));
		_targetCircleEntitySmall.SetVisibilityExcludeParents(flag);
		_targetCircleEntityBig.SetVisibilityExcludeParents(flag2);
		_targetCircleTown.SetVisibilityExcludeParents(flag3);
		if (flag)
		{
			_targetDecalMeshSmall.SetVectorArgument(0.166f, 1f, 0.166f * (float)num3, 0f);
			_targetDecalMeshSmall.SetFactor1Linear(factor1Linear);
			_targetCircleEntitySmall.SetGlobalFrame(in frame2);
		}
		if (flag2)
		{
			_targetDecalMeshBig.SetVectorArgument(0.166f, 1f, 0.166f * (float)num4, 0f);
			_targetDecalMeshBig.SetFactor1Linear(factor1Linear2);
			_targetCircleEntityBig.SetGlobalFrame(in frame3);
		}
		if (flag3)
		{
			_targetTownMesh.SetVectorArgument(1f, 1f, 0f, 0f);
			_targetTownMesh.SetFactor1Linear(factor1Linear3);
			_targetCircleTown.SetGlobalFrame(in frame);
		}
		MatrixFrame frame4 = MatrixFrame.Identity;
		if (CurrentVisualOfTooltip != null && partyBase?.MapEntity != CurrentVisualOfTooltip.GetMapEntity())
		{
			_mapCursor.OnAnotherEntityHighlighted();
			IMapEntity mapEntity = CurrentVisualOfTooltip.GetMapEntity();
			if (mapEntity != null && mapEntity.ShowCircleAroundEntity)
			{
				bool flag8 = mapEntity.IsEnemyOf(Hero.MainHero.MapFaction);
				bool flag9 = mapEntity.IsAllyOf(Hero.MainHero.MapFaction);
				flag4 = mapEntity is Settlement settlement && settlement.IsFortification;
				Vec3 origin2;
				if (flag4)
				{
					origin2 = _townOutlineEntity.GetGlobalFrame().origin;
					frame4 = CurrentVisualOfTooltip.CircleLocalFrame;
					if (flag8)
					{
						_settlementOutlineMesh.SetFactor1Linear(_enemyPartyDecalColor);
					}
					else if (flag9)
					{
						_settlementOutlineMesh.SetFactor1Linear(_allyPartyDecalColor);
					}
					else
					{
						_settlementOutlineMesh.SetFactor1Linear(_neutralPartyDecalColor);
					}
				}
				else
				{
					origin2 = _partyOutlineEntity.GetGlobalFrame().origin;
					frame4.origin = CurrentVisualOfTooltip.GetVisualPosition() + CurrentVisualOfTooltip.CircleLocalFrame.origin;
					frame4.rotation = CurrentVisualOfTooltip.CircleLocalFrame.rotation;
					_partyOutlineMesh.SetFactor1Linear(flag8 ? _enemyPartyDecalColor : (flag9 ? _allyPartyDecalColor : _neutralPartyDecalColor));
					_partyOutlineMesh.SetVectorArgument(0.166f, 1f, 0.83f, 0f);
				}
				frame4.origin.z = ((origin2.AsVec2 != frame4.origin.AsVec2) ? _mapScene.GetTerrainHeight(frame4.origin.AsVec2) : origin2.z);
				if (flag4)
				{
					frame4.rotation.u = normalAt * frame4.rotation.u.Length;
					_townOutlineEntity.SetGlobalFrame(in frame4);
					_townOutlineEntity.SetVisibilityExcludeParents(visible: true);
					_partyOutlineEntity.SetVisibilityExcludeParents(visible: false);
				}
				else
				{
					_partyOutlineEntity.SetGlobalFrame(in frame4);
					_townOutlineEntity.SetVisibilityExcludeParents(visible: false);
					_partyOutlineEntity.SetVisibilityExcludeParents(visible: true);
				}
			}
			else
			{
				_townOutlineEntity.SetVisibilityExcludeParents(visible: false);
				_partyOutlineEntity.SetVisibilityExcludeParents(visible: false);
			}
		}
		else
		{
			_townOutlineEntity.SetVisibilityExcludeParents(visible: false);
			_partyOutlineEntity.SetVisibilityExcludeParents(visible: false);
		}
	}

	public void SetIsInTownManagement(bool isInTownManagement)
	{
		if (IsInTownManagement != isInTownManagement)
		{
			IsInTownManagement = isInTownManagement;
		}
	}

	public void SetIsInHideoutTroopManage(bool isInHideoutTroopManage)
	{
		if (IsInHideoutTroopManage != isInHideoutTroopManage)
		{
			IsInHideoutTroopManage = isInHideoutTroopManage;
		}
	}

	public void SetIsInArmyManagement(bool isInArmyManagement)
	{
		if (IsInArmyManagement != isInArmyManagement)
		{
			IsInArmyManagement = isInArmyManagement;
			if (!IsInArmyManagement)
			{
				_menuViewContext?.OnResume();
			}
		}
	}

	public void SetIsInRecruitment(bool isInRecruitment)
	{
		if (IsInRecruitment != isInRecruitment)
		{
			IsInRecruitment = isInRecruitment;
		}
	}

	public void SetIsBarExtended(bool isBarExtended)
	{
		if (IsBarExtended != isBarExtended)
		{
			IsBarExtended = isBarExtended;
		}
	}

	public void SetIsInCampaignOptions(bool isInCampaignOptions)
	{
		if (IsInCampaignOptions != isInCampaignOptions)
		{
			IsInCampaignOptions = isInCampaignOptions;
		}
	}

	public void SetIsMarriageOfferPopupActive(bool isMarriageOfferPopupActive)
	{
		if (IsMarriageOfferPopupActive != isMarriageOfferPopupActive)
		{
			IsMarriageOfferPopupActive = isMarriageOfferPopupActive;
		}
	}

	public void SetIsMapCheatsActive(bool isMapCheatsActive)
	{
		if (IsMapCheatsActive != isMapCheatsActive)
		{
			IsMapCheatsActive = isMapCheatsActive;
			_cheatPressTimer = 0f;
		}
	}

	private void TickVisuals(float realDt)
	{
		if (_campaign.CampaignDt < 1E-05f)
		{
			ApplySoundSceneProps(realDt);
		}
		else
		{
			ApplySoundSceneProps(_campaign.CampaignDt);
		}
		_mapScene.TimeOfDay = CampaignTime.Now.CurrentHourInDay;
		Campaign.Current.Models.MapWeatherModel.GetSeasonTimeFactorOfCampaignTime(CampaignTime.Now, out var timeFactorForSnow, out var _, snapCampaignTimeToWeatherPeriod: false);
		MBMapScene.SetSeasonTimeFactor(_mapScene, timeFactorForSnow);
		if (!NativeConfig.DisableSound && ScreenManager.TopScreen is MapScreen)
		{
			_soundCalculationTime += realDt;
			if (_isSoundOn)
			{
				TickStepSounds();
			}
			if (_soundCalculationTime > 0.2f)
			{
				_soundCalculationTime -= 0.2f;
			}
		}
		if (IsReady)
		{
			foreach (CampaignEntityComponent campaignEntityComponent in _campaign.CampaignEntityComponents)
			{
				if (campaignEntityComponent is CampaignEntityVisualComponent campaignEntityVisualComponent)
				{
					campaignEntityVisualComponent.OnVisualTick(this, realDt, _campaign.CampaignDt);
				}
			}
		}
		MBMapScene.TickVisuals(_mapScene, Campaign.CurrentTime % 24f, _tickedMapMeshes);
		TickCircles(realDt);
		MBWindowManager.PreDisplay();
	}

	private void TickStepSounds()
	{
		if (!(Campaign.Current.CampaignDt > 0f))
		{
			return;
		}
		MobileParty mainParty = MobileParty.MainParty;
		LocatableSearchData<MobileParty> data = MobileParty.StartFindingLocatablesAroundPosition(radius: mainParty.SeeingRange + 25f, position: mainParty.Position2D);
		for (MobileParty mobileParty = MobileParty.FindNextLocatable(ref data); mobileParty != null; mobileParty = MobileParty.FindNextLocatable(ref data))
		{
			if (!mobileParty.IsMilitia && !mobileParty.IsGarrison)
			{
				StepSounds(mobileParty);
			}
		}
	}

	private void StepSounds(MobileParty party)
	{
		if (!party.IsVisible || party.MemberRoster.TotalManCount <= 0)
		{
			return;
		}
		PartyVisual visualOfParty = PartyVisualManager.Current.GetVisualOfParty(party.Party);
		if (visualOfParty.HumanAgentVisuals == null)
		{
			return;
		}
		TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(party.CurrentNavigationFace);
		AgentVisuals val = null;
		int soundType = 0;
		if (visualOfParty.CaravanMountAgentVisuals != null)
		{
			soundType = 3;
			val = visualOfParty.CaravanMountAgentVisuals;
		}
		else if (visualOfParty.HumanAgentVisuals != null)
		{
			if (visualOfParty.MountAgentVisuals != null)
			{
				soundType = 1;
				val = visualOfParty.MountAgentVisuals;
			}
			else
			{
				soundType = 0;
				val = visualOfParty.HumanAgentVisuals;
			}
		}
		MBMapScene.TickStepSound(_mapScene, val.GetVisuals(), (int)faceTerrainType, soundType);
	}

	public void SetMouseVisible(bool value)
	{
		SceneLayer.InputRestrictions.SetMouseVisibility(value);
	}

	public bool GetMouseVisible()
	{
		return MBMapScene.GetMouseVisible();
	}

	public void RestartAmbientSounds()
	{
		if (_mapScene != null)
		{
			_mapScene.ResumeSceneSounds();
		}
	}

	void IGameStateListener.OnFinalize()
	{
	}

	public void PauseAmbientSounds()
	{
		if (_mapScene != null)
		{
			_mapScene.PauseSceneSounds();
		}
	}

	public void StopSoundSceneProps()
	{
		if (_mapScene != null)
		{
			_mapScene.FinishSceneSounds();
		}
	}

	public void ApplySoundSceneProps(float dt)
	{
	}

	private void CollectTickableMapMeshes()
	{
		_tickedMapEntities = _mapScene.FindEntitiesWithTag("ticked_map_entity").ToArray();
		_tickedMapMeshes = new Mesh[_tickedMapEntities.Length];
		for (int i = 0; i < _tickedMapEntities.Length; i++)
		{
			_tickedMapMeshes[i] = _tickedMapEntities[i].GetFirstMesh();
		}
	}

	public void OnPauseTick(float dt)
	{
		ApplySoundSceneProps(dt);
	}

	public MBCampaignEvent CreatePeriodicUIEvent(CampaignTime triggerPeriod, CampaignTime initialWait)
	{
		MBCampaignEvent mBCampaignEvent = new MBCampaignEvent(triggerPeriod, initialWait);
		_periodicCampaignUIEvents.Add(mBCampaignEvent);
		return mBCampaignEvent;
	}

	private void DeleteMarkedPeriodicEvents()
	{
		for (int num = _periodicCampaignUIEvents.Count - 1; num >= 0; num--)
		{
			if (_periodicCampaignUIEvents[num].isEventDeleted)
			{
				_periodicCampaignUIEvents.RemoveAt(num);
			}
		}
	}

	public void DeletePeriodicUIEvent(MBCampaignEvent campaignEvent)
	{
		campaignEvent.isEventDeleted = true;
	}

	private static float CalculateCameraElevation(float cameraDistance)
	{
		return cameraDistance * 0.5f * 0.015f + 0.35f;
	}

	public void OpenOptions()
	{
		ScreenManager.PushScreen(ViewCreator.CreateOptionsScreen(false));
	}

	public void OpenEncyclopedia()
	{
		Campaign.Current.EncyclopediaManager.GoToLink("LastPage", "");
	}

	public void OpenSaveLoad(bool isSaving)
	{
		ScreenManager.PushScreen(SandBoxViewCreator.CreateSaveLoadScreen(isSaving));
	}

	private void OpenGameplayCheats()
	{
		_mapCheatsView = AddMapView<MapCheatsView>(Array.Empty<object>());
		IsMapCheatsActive = true;
	}

	public void CloseGameplayCheats()
	{
		if (_mapCheatsView != null)
		{
			RemoveMapView(_mapCheatsView);
		}
		else
		{
			Debug.FailedAssert("Requested remove map cheats but cheats is not enabled", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\MapScreen.cs", "CloseGameplayCheats", 3433);
		}
	}

	public void CloseEscapeMenu()
	{
		OnEscapeMenuToggled();
	}

	public void OpenEscapeMenu()
	{
		OnEscapeMenuToggled(isOpened: true);
	}

	public void CloseCampaignOptions()
	{
		if (_campaignOptionsView == null)
		{
			Debug.FailedAssert("Trying to close campaign options when it's not set", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\MapScreen.cs", "CloseCampaignOptions", 3451);
			_campaignOptionsView = GetMapView<MapCampaignOptionsView>();
			if (_campaignOptionsView == null)
			{
				Debug.FailedAssert("Trying to close campaign options when it's not open", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\MapScreen.cs", "CloseCampaignOptions", 3456);
				IsInCampaignOptions = false;
				_campaignOptionsView = null;
				return;
			}
		}
		if (_campaignOptionsView != null)
		{
			RemoveMapView(_campaignOptionsView);
		}
		_campaignOptionsView = null;
		IsInCampaignOptions = false;
	}

	private List<EscapeMenuItemVM> GetEscapeMenuItems()
	{
		bool isMapConversationActive = _mapConversationView != null;
		bool isAtSaveLimit = MBSaveLoad.IsMaxNumberOfSavesReached();
		return new List<EscapeMenuItemVM>
		{
			new EscapeMenuItemVM(new TextObject("{=e139gKZc}Return to the Game"), delegate
			{
				OnEscapeMenuToggled();
			}, null, () => new Tuple<bool, TextObject>(item1: false, TextObject.Empty), isPositiveBehaviored: true),
			new EscapeMenuItemVM(new TextObject("{=PXT6aA4J}Campaign Options"), delegate
			{
				_campaignOptionsView = AddMapView<MapCampaignOptionsView>(Array.Empty<object>());
				IsInCampaignOptions = true;
			}, null, () => new Tuple<bool, TextObject>(item1: false, TextObject.Empty)),
			new EscapeMenuItemVM(new TextObject("{=NqarFr4P}Options"), delegate
			{
				OnEscapeMenuToggled();
				OpenOptions();
			}, null, () => new Tuple<bool, TextObject>(item1: false, TextObject.Empty)),
			new EscapeMenuItemVM(new TextObject("{=bV75iwKa}Save"), delegate
			{
				OnEscapeMenuToggled();
				Campaign.Current.SaveHandler.QuickSaveCurrentGame();
			}, null, () => GetIsEscapeMenuOptionDisabledReason(isMapConversationActive, isIronmanMode: false, isAtSaveLimit: false)),
			new EscapeMenuItemVM(new TextObject("{=e0KdfaNe}Save As"), delegate
			{
				OnEscapeMenuToggled();
				OpenSaveLoad(isSaving: true);
			}, null, () => GetIsEscapeMenuOptionDisabledReason(isMapConversationActive, CampaignOptions.IsIronmanMode, isAtSaveLimit: false)),
			new EscapeMenuItemVM(new TextObject("{=9NuttOBC}Load"), delegate
			{
				OnEscapeMenuToggled();
				OpenSaveLoad(isSaving: false);
			}, null, () => GetIsEscapeMenuOptionDisabledReason(isMapConversationActive, CampaignOptions.IsIronmanMode, isAtSaveLimit: false)),
			new EscapeMenuItemVM(new TextObject("{=AbEh2y8o}Save And Exit"), delegate
			{
				Campaign.Current.SaveHandler.QuickSaveCurrentGame();
				OnEscapeMenuToggled();
				InformationManager.HideInquiry();
				_exitOnSaveOver = true;
			}, null, () => GetIsEscapeMenuOptionDisabledReason(isMapConversationActive, isIronmanMode: false, isAtSaveLimit)),
			new EscapeMenuItemVM(new TextObject("{=RamV6yLM}Exit to Main Menu"), delegate
			{
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_exit").ToString(), GameTexts.FindText("str_mission_exit_query").ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_yes").ToString(), GameTexts.FindText("str_no").ToString(), OnExitToMainMenu, delegate
				{
					OnEscapeMenuToggled();
				}));
			}, null, () => GetIsEscapeMenuOptionDisabledReason(isMapConversationActive: false, CampaignOptions.IsIronmanMode, isAtSaveLimit: false))
		};
	}

	private Tuple<bool, TextObject> GetIsEscapeMenuOptionDisabledReason(bool isMapConversationActive, bool isIronmanMode, bool isAtSaveLimit)
	{
		if (isIronmanMode)
		{
			return new Tuple<bool, TextObject>(item1: true, GameTexts.FindText("str_pause_menu_disabled_hint", "IronmanMode"));
		}
		if (isMapConversationActive)
		{
			return new Tuple<bool, TextObject>(item1: true, GameTexts.FindText("str_pause_menu_disabled_hint", "OngoingConversation"));
		}
		if (isAtSaveLimit)
		{
			return new Tuple<bool, TextObject>(item1: true, GameTexts.FindText("str_pause_menu_disabled_hint", "SaveLimitReached"));
		}
		return new Tuple<bool, TextObject>(item1: false, TextObject.Empty);
	}

	private void OpenParty()
	{
		if (Hero.MainHero.HeroState != Hero.CharacterStates.Prisoner && Hero.MainHero != null)
		{
			Hero mainHero = Hero.MainHero;
			if (mainHero != null && !mainHero.IsDead)
			{
				PartyScreenManager.OpenScreenAsNormal();
			}
		}
	}

	public void OpenInventory()
	{
		if (Hero.MainHero != null)
		{
			Hero mainHero = Hero.MainHero;
			if (mainHero != null && !mainHero.IsDead)
			{
				InventoryManager.OpenScreenAsInventory();
			}
		}
	}

	private void OpenKingdom()
	{
		if (Hero.MainHero != null)
		{
			Hero mainHero = Hero.MainHero;
			if (mainHero != null && !mainHero.IsDead && Hero.MainHero.MapFaction.IsKingdomFaction)
			{
				KingdomState gameState = Game.Current.GameStateManager.CreateState<KingdomState>();
				Game.Current.GameStateManager.PushState(gameState);
			}
		}
	}

	private void OnExitToMainMenu()
	{
		OnEscapeMenuToggled();
		InformationManager.HideInquiry();
		OnExit();
	}

	private void OpenQuestsScreen()
	{
		if (Hero.MainHero != null)
		{
			Hero mainHero = Hero.MainHero;
			if (mainHero != null && !mainHero.IsDead)
			{
				Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<QuestsState>());
			}
		}
	}

	private void OpenClanScreen()
	{
		if (Hero.MainHero != null)
		{
			Hero mainHero = Hero.MainHero;
			if (mainHero != null && !mainHero.IsDead)
			{
				Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<ClanState>());
			}
		}
	}

	private void OpenCharacterDevelopmentScreen()
	{
		if (Hero.MainHero != null)
		{
			Hero mainHero = Hero.MainHero;
			if (mainHero != null && !mainHero.IsDead)
			{
				Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<CharacterDeveloperState>());
			}
		}
	}

	public void OpenFacegenScreenAux()
	{
		OpenFaceGeneratorScreen();
	}

	private int GetCircleIndex()
	{
		int num = (int)((MBCommon.GetApplicationTime() - _targetCircleRotationStartTime) / 0.1f) % 10;
		if (num >= 5)
		{
			num = 10 - num - 1;
		}
		return num;
	}

	public void FastMoveCameraToMainParty()
	{
		_mapCameraView.FastMoveCameraToMainParty();
	}

	public void ResetCamera(bool resetDistance, bool teleportToMainParty)
	{
		_mapCameraView.ResetCamera(resetDistance, teleportToMainParty);
	}

	public void TeleportCameraToMainParty()
	{
		_mapCameraView.TeleportCameraToMainParty();
	}

	public bool IsCameraLockedToPlayerParty()
	{
		return _mapCameraView.IsCameraLockedToPlayerParty();
	}
}
