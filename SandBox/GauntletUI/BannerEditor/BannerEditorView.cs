using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.BannerEditor;

public class BannerEditorView
{
	private IGauntletMovie _gauntletmovie;

	private readonly SpriteCategory _spriteCategory;

	private bool _isFinalized;

	private float _cameraCurrentRotation;

	private float _cameraTargetRotation;

	private float _cameraCurrentDistanceAdder;

	private float _cameraTargetDistanceAdder;

	private float _cameraCurrentElevationAdder;

	private float _cameraTargetElevationAdder;

	private readonly BasicCharacterObject _character;

	private readonly ActionIndexCache _idleAction = ActionIndexCache.Create("act_walk_idle_1h_with_shield_left_stance");

	private Scene _scene;

	private MBAgentRendererSceneController _agentRendererSceneController;

	private AgentVisuals[] _agentVisuals;

	private int _agentVisualToShowIndex;

	private bool _checkWhetherAgentVisualIsReady;

	private bool _firstCharacterRender = true;

	private bool _refreshBannersNextFrame;

	private bool _refreshCharacterAndShieldNextFrame;

	private BannerCode _previousBannerCode;

	private MatrixFrame _characterFrame;

	private Equipment _weaponEquipment;

	private BannerCode _currentBannerCode;

	private Camera _camera;

	private bool _isOpenedFromCharacterCreation;

	private ControlCharacterCreationStage _affirmativeAction;

	private ControlCharacterCreationStage _negativeAction;

	private ControlCharacterCreationStageWithInt _goToIndexAction;

	public GauntletLayer GauntletLayer { get; private set; }

	public BannerEditorVM DataSource { get; private set; }

	public Banner Banner { get; private set; }

	private ItemRosterElement ShieldRosterElement => DataSource.ShieldRosterElement;

	private int ShieldSlotIndex => DataSource.ShieldSlotIndex;

	public SceneLayer SceneLayer { get; private set; }

	public BannerEditorView(BasicCharacterObject character, Banner banner, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage onRefresh = null, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction = null, ControlCharacterCreationStageReturnInt getTotalStageCountAction = null, ControlCharacterCreationStageReturnInt getFurthestIndexAction = null, ControlCharacterCreationStageWithInt goToIndexAction = null)
	{
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_spriteCategory = spriteData.SpriteCategories["ui_bannericons"];
		_spriteCategory.Load(resourceContext, uIResourceDepot);
		_character = character;
		Banner = banner;
		_goToIndexAction = goToIndexAction;
		if (getCurrentStageIndexAction == null || getTotalStageCountAction == null || getFurthestIndexAction == null)
		{
			DataSource = new BannerEditorVM(_character, Banner, Exit, RefreshShieldAndCharacter, 0, 0, 0, GoToIndex);
			DataSource.Description = new TextObject("{=3ZO5cMLu}Customize your banner's sigil").ToString();
			_isOpenedFromCharacterCreation = true;
		}
		else
		{
			DataSource = new BannerEditorVM(_character, Banner, Exit, RefreshShieldAndCharacter, getCurrentStageIndexAction(), getTotalStageCountAction(), getFurthestIndexAction(), GoToIndex);
			DataSource.Description = new TextObject("{=312lNJTM}Customize your personal banner by choosing your clan's sigil").ToString();
			_isOpenedFromCharacterCreation = false;
		}
		DataSource.DoneText = affirmativeActionText.ToString();
		DataSource.CancelText = negativeActionText.ToString();
		GauntletLayer = new GauntletLayer(1);
		_gauntletmovie = GauntletLayer.LoadMovie("BannerEditor", DataSource);
		GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
		GauntletLayer.InputRestrictions.SetInputRestrictions();
		GauntletLayer.IsFocusLayer = true;
		ScreenManager.TrySetFocus(GauntletLayer);
		DataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		DataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		_affirmativeAction = affirmativeAction;
		_negativeAction = negativeAction;
		_agentVisuals = (AgentVisuals[])(object)new AgentVisuals[2];
		_currentBannerCode = BannerCode.CreateFrom(Banner);
		CreateScene();
		Input.ClearKeys();
		_weaponEquipment.AddEquipmentToSlotWithoutAgent((EquipmentIndex)ShieldSlotIndex, ShieldRosterElement.EquipmentElement);
		AgentVisualsData copyAgentVisualsData = _agentVisuals[0].GetCopyAgentVisualsData();
		copyAgentVisualsData.Equipment(_weaponEquipment).RightWieldedItemIndex(-1).LeftWieldedItemIndex(ShieldSlotIndex)
			.Banner(Banner)
			.ClothColor1(Banner.GetPrimaryColor())
			.ClothColor2(Banner.GetFirstIconColor());
		_agentVisuals[0].Refresh(false, copyAgentVisualsData, true);
		MissionWeapon shieldWeapon = new MissionWeapon(ShieldRosterElement.EquipmentElement.Item, ShieldRosterElement.EquipmentElement.ItemModifier, Banner);
		Action<TaleWorlds.Engine.Texture> action = delegate(TaleWorlds.Engine.Texture tex)
		{
			shieldWeapon.GetWeaponData(needBatchedVersionForMeshes: false).TableauMaterial.SetTexture(TaleWorlds.Engine.Material.MBTextureType.DiffuseMap2, tex);
		};
		BannerVisualExtensions.GetTableauTextureLarge(Banner, action);
		_agentVisuals[0].SetVisible(false);
		_agentVisuals[0].GetEntity().CheckResources(addToQueue: true, checkFaceResources: true);
		AgentVisualsData copyAgentVisualsData2 = _agentVisuals[1].GetCopyAgentVisualsData();
		copyAgentVisualsData2.Equipment(_weaponEquipment).RightWieldedItemIndex(-1).LeftWieldedItemIndex(ShieldSlotIndex)
			.Banner(Banner)
			.ClothColor1(Banner.GetPrimaryColor())
			.ClothColor2(Banner.GetFirstIconColor());
		_agentVisuals[1].Refresh(false, copyAgentVisualsData2, true);
		_agentVisuals[1].SetVisible(false);
		_agentVisuals[1].GetEntity().CheckResources(addToQueue: true, checkFaceResources: true);
		_checkWhetherAgentVisualIsReady = true;
		_firstCharacterRender = true;
	}

	public void OnTick(float dt)
	{
		if (_isFinalized)
		{
			return;
		}
		HandleUserInput();
		if (_isFinalized)
		{
			return;
		}
		UpdateCamera(dt);
		SceneLayer sceneLayer = SceneLayer;
		if (sceneLayer != null && sceneLayer.ReadyToRender())
		{
			LoadingWindow.DisableGlobalLoadingWindow();
		}
		_scene?.Tick(dt);
		if (_refreshBannersNextFrame)
		{
			UpdateBanners();
			_refreshBannersNextFrame = false;
		}
		if (_refreshCharacterAndShieldNextFrame)
		{
			RefreshShieldAndCharacterAux();
			_refreshCharacterAndShieldNextFrame = false;
		}
		if (!_checkWhetherAgentVisualIsReady)
		{
			return;
		}
		int num = (_agentVisualToShowIndex + 1) % 2;
		if (_agentVisuals[_agentVisualToShowIndex].GetEntity().CheckResources(_firstCharacterRender, checkFaceResources: true))
		{
			_agentVisuals[num].SetVisible(false);
			_agentVisuals[_agentVisualToShowIndex].SetVisible(true);
			_checkWhetherAgentVisualIsReady = false;
			_firstCharacterRender = false;
		}
		else
		{
			if (!_firstCharacterRender)
			{
				_agentVisuals[num].SetVisible(true);
			}
			_agentVisuals[_agentVisualToShowIndex].SetVisible(false);
		}
	}

	public void OnFinalize()
	{
		if (!_isOpenedFromCharacterCreation)
		{
			_spriteCategory.Unload();
		}
		DataSource?.OnFinalize();
		_isFinalized = true;
	}

	public void Exit(bool isCancel)
	{
		MouseManager.ActivateMouseCursor(CursorType.Default);
		_gauntletmovie = null;
		if (isCancel)
		{
			_negativeAction();
			return;
		}
		SetMapIconAsDirtyForAllPlayerClanParties();
		_affirmativeAction();
	}

	private void SetMapIconAsDirtyForAllPlayerClanParties()
	{
		foreach (Hero lord in Clan.PlayerClan.Lords)
		{
			foreach (CaravanPartyComponent ownedCaravan in lord.OwnedCaravans)
			{
				ownedCaravan.MobileParty.Party?.SetVisualAsDirty();
			}
		}
		foreach (Hero companion in Clan.PlayerClan.Companions)
		{
			foreach (CaravanPartyComponent ownedCaravan2 in companion.OwnedCaravans)
			{
				ownedCaravan2.MobileParty.Party?.SetVisualAsDirty();
			}
		}
		foreach (WarPartyComponent warPartyComponent in Clan.PlayerClan.WarPartyComponents)
		{
			warPartyComponent.MobileParty.Party?.SetVisualAsDirty();
		}
		foreach (Settlement settlement in Clan.PlayerClan.Settlements)
		{
			if (settlement.IsVillage && settlement.Village.VillagerPartyComponent != null)
			{
				settlement.Village.VillagerPartyComponent.MobileParty.Party?.SetVisualAsDirty();
			}
			else if ((settlement.IsCastle || settlement.IsTown) && settlement.Town.GarrisonParty != null)
			{
				settlement.Town.GarrisonParty.Party?.SetVisualAsDirty();
			}
		}
	}

	private void CreateScene()
	{
		_scene = Scene.CreateNewScene(initialize_physics: true, enable_decals: true, DecalAtlasGroup.Battle);
		_scene.SetName("MBBannerEditorScreen");
		SceneInitializationData initData = default(SceneInitializationData);
		initData.InitPhysicsWorld = false;
		_scene.Read("banner_editor_scene", ref initData);
		_scene.SetShadow(shadowEnabled: true);
		_scene.DisableStaticShadows(value: true);
		_scene.SetDynamicShadowmapCascadesRadiusMultiplier(0.1f);
		_agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(_scene, 32);
		float aspectRatio = Screen.AspectRatio;
		GameEntity gameEntity = _scene.FindEntityWithTag("spawnpoint_player");
		_characterFrame = gameEntity.GetFrame();
		_characterFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		_cameraTargetDistanceAdder = 3.5f;
		_cameraCurrentDistanceAdder = _cameraTargetDistanceAdder;
		_cameraTargetElevationAdder = 1.15f;
		_cameraCurrentElevationAdder = _cameraTargetElevationAdder;
		_camera = Camera.CreateCamera();
		_camera.SetFovVertical(0.6981317f, aspectRatio, 0.2f, 200f);
		SceneLayer = new SceneLayer();
		SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
		SceneLayer.SetScene(_scene);
		UpdateCamera(0f);
		SceneLayer.SetSceneUsesShadows(value: true);
		SceneLayer.SceneView.SetResolutionScaling(value: true);
		int num = -1;
		num &= -5;
		SceneLayer.SetPostfxConfigParams(num);
		AddCharacterEntity(_idleAction);
	}

	private void AddCharacterEntity(ActionIndexCache action)
	{
		_weaponEquipment = new Equipment();
		for (int i = 0; i < 12; i++)
		{
			EquipmentElement equipmentFromSlot = _character.Equipment.GetEquipmentFromSlot((EquipmentIndex)i);
			if (equipmentFromSlot.Item?.PrimaryWeapon == null || (equipmentFromSlot.Item?.PrimaryWeapon != null && !equipmentFromSlot.Item.PrimaryWeapon.IsShield && !equipmentFromSlot.Item.ItemFlags.HasAllFlags(ItemFlags.DropOnWeaponChange)))
			{
				_weaponEquipment.AddEquipmentToSlotWithoutAgent((EquipmentIndex)i, equipmentFromSlot);
			}
		}
		Monster baseMonsterFromRace = TaleWorlds.Core.FaceGen.GetBaseMonsterFromRace(_character.Race);
		_agentVisuals[0] = AgentVisuals.Create(new AgentVisualsData().Equipment(_weaponEquipment).BodyProperties(_character.GetBodyProperties(_weaponEquipment)).Frame(_characterFrame)
			.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, _character.IsFemale, "_facegen"))
			.ActionCode(action)
			.Scene(_scene)
			.Monster(baseMonsterFromRace)
			.SkeletonType(_character.IsFemale ? SkeletonType.Female : SkeletonType.Male)
			.Race(_character.Race)
			.PrepareImmediately(prepareImmediately: true)
			.UseMorphAnims(useMorphAnims: true), "BannerEditorChar", false, false, true);
		_agentVisuals[0].SetAgentLodZeroOrMaxExternal(true);
		_agentVisuals[0].GetEntity().CheckResources(addToQueue: true, checkFaceResources: true);
		_agentVisuals[1] = AgentVisuals.Create(new AgentVisualsData().Equipment(_weaponEquipment).BodyProperties(_character.GetBodyProperties(_weaponEquipment)).Frame(_characterFrame)
			.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, _character.IsFemale, "_facegen"))
			.ActionCode(action)
			.Scene(_scene)
			.Race(_character.Race)
			.Monster(baseMonsterFromRace)
			.SkeletonType(_character.IsFemale ? SkeletonType.Female : SkeletonType.Male)
			.PrepareImmediately(prepareImmediately: true)
			.UseMorphAnims(useMorphAnims: true), "BannerEditorChar", false, false, true);
		_agentVisuals[1].SetAgentLodZeroOrMaxExternal(true);
		_agentVisuals[1].GetEntity().CheckResources(addToQueue: true, checkFaceResources: true);
		UpdateBanners();
	}

	private void UpdateBanners()
	{
		BannerCode currentBannerCode = BannerCode.CreateFrom(Banner);
		BannerVisualExtensions.GetTableauTextureLarge(Banner, (Action<TaleWorlds.Engine.Texture>)delegate(TaleWorlds.Engine.Texture resultTexture)
		{
			OnNewBannerReadyForBanners(currentBannerCode, resultTexture);
		});
		if (_previousBannerCode != null)
		{
			TableauCacheManager current = TableauCacheManager.Current;
			if (current != null)
			{
				current.ForceReleaseBanner(_previousBannerCode, true, true);
			}
			TableauCacheManager current2 = TableauCacheManager.Current;
			if (current2 != null)
			{
				current2.ForceReleaseBanner(_previousBannerCode, true, false);
			}
		}
		_previousBannerCode = BannerCode.CreateFrom(Banner);
	}

	private void OnNewBannerReadyForBanners(BannerCode bannerCodeOfTexture, TaleWorlds.Engine.Texture newTexture)
	{
		if (_isFinalized || !(_scene != null) || !(_currentBannerCode == bannerCodeOfTexture))
		{
			return;
		}
		GameEntity gameEntity = _scene.FindEntityWithTag("banner");
		if (gameEntity != null)
		{
			Mesh firstMesh = gameEntity.GetFirstMesh();
			if (firstMesh != null && Banner != null)
			{
				firstMesh.GetMaterial().SetTexture(TaleWorlds.Engine.Material.MBTextureType.DiffuseMap2, newTexture);
			}
		}
		else
		{
			gameEntity = _scene.FindEntityWithTag("banner_2");
			Mesh firstMesh2 = gameEntity.GetFirstMesh();
			if (firstMesh2 != null && Banner != null)
			{
				firstMesh2.GetMaterial().SetTexture(TaleWorlds.Engine.Material.MBTextureType.DiffuseMap2, newTexture);
			}
		}
		_refreshCharacterAndShieldNextFrame = true;
	}

	private void RefreshShieldAndCharacter()
	{
		_currentBannerCode = BannerCode.CreateFrom(Banner);
		_refreshBannersNextFrame = true;
	}

	private void RefreshShieldAndCharacterAux()
	{
		_ = _agentVisualToShowIndex;
		_agentVisualToShowIndex = (_agentVisualToShowIndex + 1) % 2;
		AgentVisualsData copyAgentVisualsData = _agentVisuals[_agentVisualToShowIndex].GetCopyAgentVisualsData();
		copyAgentVisualsData.Equipment(_weaponEquipment).RightWieldedItemIndex(-1).LeftWieldedItemIndex(ShieldSlotIndex)
			.Banner(Banner)
			.Frame(_characterFrame)
			.BodyProperties(_character.GetBodyProperties(_weaponEquipment))
			.ClothColor1(Banner.GetPrimaryColor())
			.ClothColor2(Banner.GetFirstIconColor());
		_agentVisuals[_agentVisualToShowIndex].Refresh(false, copyAgentVisualsData, true);
		_agentVisuals[_agentVisualToShowIndex].GetEntity().CheckResources(addToQueue: true, checkFaceResources: true);
		_agentVisuals[_agentVisualToShowIndex].GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.001f, _characterFrame, tickAnimsForChildren: true);
		_agentVisuals[_agentVisualToShowIndex].SetVisible(false);
		_agentVisuals[_agentVisualToShowIndex].SetVisible(true);
		_checkWhetherAgentVisualIsReady = true;
	}

	private void HandleUserInput()
	{
		if (GauntletLayer.Input.IsHotKeyReleased("Confirm"))
		{
			DataSource.ExecuteDone();
			UISoundsHelper.PlayUISound("event:/ui/panels/next");
			return;
		}
		if (GauntletLayer.Input.IsHotKeyReleased("Exit"))
		{
			DataSource.ExecuteCancel();
			UISoundsHelper.PlayUISound("event:/ui/panels/next");
			return;
		}
		if (SceneLayer.Input.IsHotKeyReleased("Ascend") || SceneLayer.Input.IsHotKeyReleased("Rotate") || SceneLayer.Input.IsHotKeyReleased("Zoom"))
		{
			GauntletLayer.InputRestrictions.SetMouseVisibility(isVisible: true);
		}
		Vec2 vec = new Vec2(0f - SceneLayer.Input.GetMouseMoveX(), 0f - SceneLayer.Input.GetMouseMoveY());
		if (SceneLayer.Input.IsHotKeyDown("Zoom"))
		{
			_cameraTargetDistanceAdder = MBMath.ClampFloat(_cameraTargetDistanceAdder + vec.y * 0.002f, 1.5f, 5f);
			MBWindowManager.DontChangeCursorPos();
			GauntletLayer.InputRestrictions.SetMouseVisibility(isVisible: false);
		}
		if (SceneLayer.Input.IsHotKeyDown("Rotate"))
		{
			_cameraTargetRotation = MBMath.WrapAngle(_cameraTargetRotation - vec.x * 0.004f);
			MBWindowManager.DontChangeCursorPos();
			GauntletLayer.InputRestrictions.SetMouseVisibility(isVisible: false);
		}
		if (SceneLayer.Input.IsHotKeyDown("Ascend"))
		{
			_cameraTargetElevationAdder = MBMath.ClampFloat(_cameraTargetElevationAdder - vec.y * 0.002f, 0.5f, 1.9f * _agentVisuals[0].GetScale());
			MBWindowManager.DontChangeCursorPos();
			GauntletLayer.InputRestrictions.SetMouseVisibility(isVisible: false);
		}
		if (SceneLayer.Input.GetDeltaMouseScroll() != 0f)
		{
			_cameraTargetDistanceAdder = MBMath.ClampFloat(_cameraTargetDistanceAdder - SceneLayer.Input.GetDeltaMouseScroll() * 0.001f, 1.5f, 5f);
		}
	}

	private void UpdateCamera(float dt)
	{
		_cameraCurrentRotation += MBMath.WrapAngle(_cameraTargetRotation - _cameraCurrentRotation) * TaleWorlds.Library.MathF.Min(1f, 10f * dt);
		_cameraCurrentElevationAdder += MBMath.WrapAngle(_cameraTargetElevationAdder - _cameraCurrentElevationAdder) * TaleWorlds.Library.MathF.Min(1f, 10f * dt);
		_cameraCurrentDistanceAdder += MBMath.WrapAngle(_cameraTargetDistanceAdder - _cameraCurrentDistanceAdder) * TaleWorlds.Library.MathF.Min(1f, 10f * dt);
		MatrixFrame characterFrame = _characterFrame;
		characterFrame.rotation.RotateAboutUp(_cameraCurrentRotation);
		characterFrame.origin += _cameraCurrentElevationAdder * characterFrame.rotation.u + _cameraCurrentDistanceAdder * characterFrame.rotation.f;
		characterFrame.rotation.RotateAboutSide(-(float)Math.PI / 2f);
		characterFrame.rotation.RotateAboutUp((float)Math.PI);
		characterFrame.rotation.RotateAboutForward((float)Math.PI * -3f / 50f);
		_camera.Frame = characterFrame;
		SceneLayer.SetCamera(_camera);
		SoundManager.SetListenerFrame(characterFrame);
	}

	public void OnDeactivate()
	{
		_agentVisuals[0].Reset();
		_agentVisuals[1].Reset();
		MBAgentRendererSceneController.DestructAgentRendererSceneController(_scene, _agentRendererSceneController, deleteThisFrame: false);
		_agentRendererSceneController = null;
		_scene.ClearAll();
		_scene = null;
	}

	public void GoToIndex(int index)
	{
		_goToIndexAction(index);
	}
}
