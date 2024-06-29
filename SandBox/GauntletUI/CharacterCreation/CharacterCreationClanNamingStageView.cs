using System;
using System.Collections.Generic;
using SandBox.View.CharacterCreation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation;
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
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.CharacterCreation;

[CharacterCreationStageView(typeof(CharacterCreationClanNamingStage))]
public class CharacterCreationClanNamingStageView : CharacterCreationStageViewBase
{
	private TaleWorlds.CampaignSystem.CharacterCreationContent.CharacterCreation _characterCreation;

	private GauntletLayer GauntletLayer;

	private CharacterCreationClanNamingStageVM _dataSource;

	private IGauntletMovie _clanNamingStageMovie;

	private TextObject _affirmativeActionText;

	private TextObject _negativeActionText;

	private Banner _banner;

	private float _cameraCurrentRotation;

	private float _cameraTargetRotation;

	private float _cameraCurrentDistanceAdder;

	private float _cameraTargetDistanceAdder;

	private float _cameraCurrentElevationAdder;

	private float _cameraTargetElevationAdder;

	private readonly BasicCharacterObject _character;

	private Scene _scene;

	private readonly ActionIndexCache _idleAction = ActionIndexCache.Create("act_walk_idle_1h_with_shield_left_stance");

	private MBAgentRendererSceneController _agentRendererSceneController;

	private AgentVisuals _agentVisuals;

	private MatrixFrame _characterFrame;

	private Equipment _weaponEquipment;

	private Camera _camera;

	private EscapeMenuVM _escapeMenuDatasource;

	private IGauntletMovie _escapeMenuMovie;

	private ItemRosterElement ShieldRosterElement => _dataSource.ShieldRosterElement;

	private int ShieldSlotIndex => _dataSource.ShieldSlotIndex;

	public SceneLayer SceneLayer { get; private set; }

	public CharacterCreationClanNamingStageView(TaleWorlds.CampaignSystem.CharacterCreationContent.CharacterCreation characterCreation, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage refreshAction, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction, ControlCharacterCreationStageReturnInt getTotalStageCountAction, ControlCharacterCreationStageReturnInt getFurthestIndexAction, ControlCharacterCreationStageWithInt goToIndexAction)
		: base(affirmativeAction, negativeAction, refreshAction, getCurrentStageIndexAction, getTotalStageCountAction, getFurthestIndexAction, goToIndexAction)
	{
		_characterCreation = characterCreation;
		_affirmativeActionText = affirmativeActionText;
		_negativeActionText = negativeActionText;
		GauntletLayer = new GauntletLayer(1)
		{
			IsFocusLayer = true
		};
		GauntletLayer.InputRestrictions.SetInputRestrictions();
		GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		ScreenManager.TrySetFocus(GauntletLayer);
		_character = CharacterObject.PlayerCharacter;
		_banner = Clan.PlayerClan.Banner;
		_dataSource = new CharacterCreationClanNamingStageVM(_character, _banner, _characterCreation, NextStage, _affirmativeActionText, PreviousStage, _negativeActionText, getCurrentStageIndexAction(), getTotalStageCountAction(), getFurthestIndexAction(), GoToIndex);
		_dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		_dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		_clanNamingStageMovie = GauntletLayer.LoadMovie("CharacterCreationClanNamingStage", _dataSource);
		CreateScene();
		RefreshCharacterEntity();
	}

	public override void Tick(float dt)
	{
		HandleUserInput();
		UpdateCamera(dt);
		if (SceneLayer != null && SceneLayer.ReadyToRender())
		{
			LoadingWindow.DisableGlobalLoadingWindow();
		}
		if (_scene != null)
		{
			_scene.Tick(dt);
		}
		HandleEscapeMenu(this, GauntletLayer);
		HandleLayerInput();
	}

	private void CreateScene()
	{
		_scene = Scene.CreateNewScene(initialize_physics: true, enable_decals: false);
		_scene.SetName("MBBannerEditorScreen");
		SceneInitializationData initData = new SceneInitializationData(initializeWithDefaults: true);
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
		SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
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
			if (equipmentFromSlot.Item?.PrimaryWeapon == null || (equipmentFromSlot.Item?.PrimaryWeapon != null && !equipmentFromSlot.Item.PrimaryWeapon.IsShield))
			{
				_weaponEquipment.AddEquipmentToSlotWithoutAgent((EquipmentIndex)i, equipmentFromSlot);
			}
		}
		Monster baseMonsterFromRace = TaleWorlds.Core.FaceGen.GetBaseMonsterFromRace(_character.Race);
		_agentVisuals = AgentVisuals.Create(new AgentVisualsData().Equipment(_weaponEquipment).BodyProperties(_character.GetBodyProperties(_weaponEquipment)).Frame(_characterFrame)
			.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, _character.IsFemale, "_facegen"))
			.ActionCode(action)
			.Scene(_scene)
			.Race(_character.Race)
			.Monster(baseMonsterFromRace)
			.SkeletonType(_character.IsFemale ? SkeletonType.Female : SkeletonType.Male)
			.PrepareImmediately(prepareImmediately: true)
			.UseMorphAnims(useMorphAnims: true), "BannerEditorChar", false, false, true);
		_agentVisuals.SetAgentLodZeroOrMaxExternal(true);
		UpdateBanners();
	}

	private void UpdateBanners()
	{
		BannerVisualExtensions.GetTableauTextureLarge(_banner, (Action<Texture>)OnNewBannerReadyForBanners);
	}

	private void OnNewBannerReadyForBanners(Texture newTexture)
	{
		if (_scene == null)
		{
			return;
		}
		GameEntity gameEntity = _scene.FindEntityWithTag("banner");
		if (gameEntity == null)
		{
			return;
		}
		Mesh firstMesh = gameEntity.GetFirstMesh();
		if (firstMesh != null && _banner != null)
		{
			firstMesh.GetMaterial().SetTexture(Material.MBTextureType.DiffuseMap2, newTexture);
		}
		gameEntity = _scene.FindEntityWithTag("banner_2");
		if (!(gameEntity == null))
		{
			firstMesh = gameEntity.GetFirstMesh();
			if (firstMesh != null && _banner != null)
			{
				firstMesh.GetMaterial().SetTexture(Material.MBTextureType.DiffuseMap2, newTexture);
			}
		}
	}

	private void RefreshCharacterEntity()
	{
		_weaponEquipment.AddEquipmentToSlotWithoutAgent((EquipmentIndex)ShieldSlotIndex, ShieldRosterElement.EquipmentElement);
		AgentVisualsData copyAgentVisualsData = _agentVisuals.GetCopyAgentVisualsData();
		copyAgentVisualsData.Equipment(_weaponEquipment).RightWieldedItemIndex(-1).LeftWieldedItemIndex(ShieldSlotIndex)
			.Banner(_banner)
			.ClothColor1(_banner.GetPrimaryColor())
			.ClothColor2(_banner.GetFirstIconColor());
		_agentVisuals.Refresh(false, copyAgentVisualsData, false);
		MissionWeapon shieldWeapon = new MissionWeapon(ShieldRosterElement.EquipmentElement.Item, ShieldRosterElement.EquipmentElement.ItemModifier, _banner);
		Action<Texture> action = delegate(Texture tex)
		{
			shieldWeapon.GetWeaponData(needBatchedVersionForMeshes: false).TableauMaterial.SetTexture(Material.MBTextureType.DiffuseMap2, tex);
		};
		BannerVisualExtensions.GetTableauTextureLarge(_banner, action);
	}

	private void HandleLayerInput()
	{
		if (IsGameKeyReleasedInAnyLayer("Exit"))
		{
			UISoundsHelper.PlayUISound("event:/ui/panels/next");
			_dataSource.OnPreviousStage();
		}
		else if (IsGameKeyReleasedInAnyLayer("Confirm") && _dataSource.CanAdvance)
		{
			UISoundsHelper.PlayUISound("event:/ui/panels/next");
			_dataSource.OnNextStage();
		}
	}

	private void HandleUserInput()
	{
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
			_cameraTargetElevationAdder = MBMath.ClampFloat(_cameraTargetElevationAdder - vec.y * 0.002f, 0.5f, 1.9f * _agentVisuals.GetScale());
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

	public override IEnumerable<ScreenLayer> GetLayers()
	{
		return new List<ScreenLayer> { SceneLayer, GauntletLayer };
	}

	public override int GetVirtualStageCount()
	{
		return 1;
	}

	public override void NextStage()
	{
		TextObject variable = new TextObject(_dataSource.ClanName);
		TextObject textObject = GameTexts.FindText("str_generic_clan_name");
		textObject.SetTextVariable("CLAN_NAME", variable);
		Clan.PlayerClan.ChangeClanName(textObject, textObject);
		_affirmativeAction?.Invoke();
	}

	public override void PreviousStage()
	{
		_negativeAction?.Invoke();
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		SceneLayer.SceneView.SetEnable(value: false);
		SceneLayer.SceneView.ClearAll(clearScene: true, removeTerrain: true);
		GauntletLayer = null;
		SceneLayer = null;
		_dataSource?.OnFinalize();
		_dataSource = null;
		_clanNamingStageMovie = null;
		_agentVisuals.Reset();
		_agentVisuals = null;
		MBAgentRendererSceneController.DestructAgentRendererSceneController(_scene, _agentRendererSceneController, deleteThisFrame: false);
		_agentRendererSceneController = null;
		_scene = null;
	}

	public override void LoadEscapeMenuMovie()
	{
		_escapeMenuDatasource = new EscapeMenuVM(GetEscapeMenuItems(this));
		_escapeMenuMovie = GauntletLayer.LoadMovie("EscapeMenu", _escapeMenuDatasource);
	}

	public override void ReleaseEscapeMenuMovie()
	{
		GauntletLayer.ReleaseMovie(_escapeMenuMovie);
		_escapeMenuDatasource = null;
		_escapeMenuMovie = null;
	}

	private bool IsGameKeyReleasedInAnyLayer(string hotKeyID)
	{
		bool num = IsReleasedInSceneLayer(hotKeyID);
		bool flag = IsReleasedInGauntletLayer(hotKeyID);
		return num || flag;
	}

	private bool IsReleasedInSceneLayer(string hotKeyID)
	{
		return SceneLayer?.Input.IsHotKeyReleased(hotKeyID) ?? false;
	}

	private bool IsReleasedInGauntletLayer(string hotKeyID)
	{
		return GauntletLayer?.Input.IsHotKeyReleased(hotKeyID) ?? false;
	}
}
