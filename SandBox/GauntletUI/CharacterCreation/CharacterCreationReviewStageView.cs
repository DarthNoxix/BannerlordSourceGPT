using System;
using System.Collections.Generic;
using System.Linq;
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
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.CharacterCreation;

[CharacterCreationStageView(typeof(CharacterCreationReviewStage))]
public class CharacterCreationReviewStageView : CharacterCreationStageViewBase
{
	protected readonly TextObject _affirmativeActionText;

	protected readonly TextObject _negativeActionText;

	private readonly IGauntletMovie _movie;

	private GauntletLayer GauntletLayer;

	private CharacterCreationReviewStageVM _dataSource;

	private readonly ActionIndexCache act_inventory_idle_start = ActionIndexCache.Create("act_inventory_idle_start");

	private readonly TaleWorlds.CampaignSystem.CharacterCreationContent.CharacterCreation _characterCreation;

	private Scene _characterScene;

	private Camera _camera;

	private MatrixFrame _initialCharacterFrame;

	private AgentVisuals _agentVisuals;

	private GameEntity _mountEntity;

	private float _charRotationAmount;

	private EscapeMenuVM _escapeMenuDatasource;

	private IGauntletMovie _escapeMenuMovie;

	public SceneLayer CharacterLayer { get; private set; }

	public CharacterCreationReviewStageView(TaleWorlds.CampaignSystem.CharacterCreationContent.CharacterCreation characterCreation, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage onRefresh, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction, ControlCharacterCreationStageReturnInt getTotalStageCountAction, ControlCharacterCreationStageReturnInt getFurthestIndexAction, ControlCharacterCreationStageWithInt goToIndexAction)
		: base(affirmativeAction, negativeAction, onRefresh, getCurrentStageIndexAction, getTotalStageCountAction, getFurthestIndexAction, goToIndexAction)
	{
		_characterCreation = characterCreation;
		_affirmativeActionText = new TextObject("{=Rvr1bcu8}Next");
		_negativeActionText = negativeActionText;
		GauntletLayer = new GauntletLayer(1);
		GauntletLayer.InputRestrictions.SetInputRestrictions();
		GauntletLayer.IsFocusLayer = true;
		GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		ScreenManager.TrySetFocus(GauntletLayer);
		CharacterCreationContentBase currentCharacterCreationContent = (GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent;
		bool isBannerAndClanNameSet = currentCharacterCreationContent.CharacterCreationStages.Contains(typeof(CharacterCreationBannerEditorStage)) && currentCharacterCreationContent.CharacterCreationStages.Contains(typeof(CharacterCreationClanNamingStage));
		_dataSource = new CharacterCreationReviewStageVM(_characterCreation, NextStage, _affirmativeActionText, PreviousStage, _negativeActionText, getCurrentStageIndexAction(), getTotalStageCountAction(), getFurthestIndexAction(), GoToIndex, isBannerAndClanNameSet);
		_dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		_dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		_movie = GauntletLayer.LoadMovie("CharacterCreationReviewStage", _dataSource);
	}

	public override void SetGenericScene(Scene scene)
	{
		OpenScene(scene);
		AddCharacterEntity();
		RefreshMountEntity();
	}

	private void OpenScene(Scene cachedScene)
	{
		_characterScene = cachedScene;
		_characterScene.SetShadow(shadowEnabled: true);
		_characterScene.SetDynamicShadowmapCascadesRadiusMultiplier(0.1f);
		_characterScene.FindEntityWithName("cradle")?.SetVisibilityExcludeParents(visible: false);
		_characterScene.SetDoNotWaitForLoadingStatesToRender(value: true);
		_characterScene.DisableStaticShadows(value: true);
		_camera = Camera.CreateCamera();
		BodyGeneratorView.InitCamera(_camera, _cameraPosition);
		CharacterLayer = new SceneLayer("SceneLayer", clearSceneOnFinalize: false);
		CharacterLayer.SetScene(_characterScene);
		CharacterLayer.SetCamera(_camera);
		CharacterLayer.SetSceneUsesShadows(value: true);
		CharacterLayer.SetRenderWithPostfx(value: true);
		CharacterLayer.SetPostfxFromConfig();
		CharacterLayer.SceneView.SetResolutionScaling(value: true);
		int num = -1;
		num &= -5;
		CharacterLayer.SetPostfxConfigParams(num);
		CharacterLayer.SetPostfxFromConfig();
		if (!CharacterLayer.Input.IsCategoryRegistered(HotKeyManager.GetCategory("FaceGenHotkeyCategory")))
		{
			CharacterLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
		}
	}

	private void AddCharacterEntity()
	{
		GameEntity gameEntity = _characterScene.FindEntityWithTag("spawnpoint_player_1");
		_initialCharacterFrame = gameEntity.GetFrame();
		_initialCharacterFrame.origin.z = 0f;
		ActionIndexCache actionCode = ActionIndexCache.Create("act_childhood_schooled");
		CharacterObject characterObject = Hero.MainHero.CharacterObject;
		Monster baseMonsterFromRace = TaleWorlds.Core.FaceGen.GetBaseMonsterFromRace(characterObject.Race);
		AgentVisualsData agentVisualsData = new AgentVisualsData().UseMorphAnims(useMorphAnims: true).Equipment(characterObject.Equipment).BodyProperties(characterObject.GetBodyProperties(characterObject.Equipment))
			.SkeletonType(characterObject.IsFemale ? SkeletonType.Female : SkeletonType.Male)
			.Frame(_initialCharacterFrame)
			.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, characterObject.IsFemale, "_facegen"))
			.ActionCode(actionCode)
			.Scene(_characterScene)
			.Race(characterObject.Race)
			.Monster(baseMonsterFromRace)
			.UseTranslucency(useTranslucency: true)
			.UseTesselation(useTesselation: true);
		CharacterCreationContentBase currentCharacterCreationContent = (GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent;
		Banner currentPlayerBanner = currentCharacterCreationContent.GetCurrentPlayerBanner();
		CultureObject selectedCulture = currentCharacterCreationContent.GetSelectedCulture();
		if (currentPlayerBanner != null)
		{
			agentVisualsData.ClothColor1(currentPlayerBanner.GetPrimaryColor());
			agentVisualsData.ClothColor2(currentPlayerBanner.GetFirstIconColor());
		}
		else if (currentCharacterCreationContent.GetSelectedCulture() != null)
		{
			agentVisualsData.ClothColor1(selectedCulture.Color);
			agentVisualsData.ClothColor2(selectedCulture.Color2);
		}
		_agentVisuals = AgentVisuals.Create(agentVisualsData, "facegenvisual", false, false, true);
		CharacterLayer.SetFocusedShadowmap(enable: true, ref _initialCharacterFrame.origin, 0.59999996f);
	}

	private void RefreshCharacterEntityFrame()
	{
		if (_agentVisuals != null)
		{
			MatrixFrame frame = _initialCharacterFrame;
			frame.rotation.RotateAboutUp(_charRotationAmount);
			frame.rotation.ApplyScaleLocal(_agentVisuals.GetScale());
			_agentVisuals.GetEntity().SetFrame(ref frame);
		}
	}

	private void RefreshMountEntity()
	{
		RemoveMount();
		if (CharacterObject.PlayerCharacter.HasMount())
		{
			FaceGenMount faceGenMount = new FaceGenMount(MountCreationKey.GetRandomMountKey(CharacterObject.PlayerCharacter.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, CharacterObject.PlayerCharacter.GetMountKeySeed()), CharacterObject.PlayerCharacter.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, CharacterObject.PlayerCharacter.Equipment[EquipmentIndex.HorseHarness].Item);
			GameEntity gameEntity = _characterScene.FindEntityWithTag("spawnpoint_mount_1");
			HorseComponent horseComponent = faceGenMount.HorseItem.HorseComponent;
			Monster monster = horseComponent.Monster;
			_mountEntity = GameEntity.CreateEmpty(_characterScene);
			AnimationSystemData animationSystemData = monster.FillAnimationSystemData(MBGlobals.GetActionSet(horseComponent.Monster.ActionSetCode), 1f, hasClippingPlane: false);
			_mountEntity.CreateSkeletonWithActionSet(ref animationSystemData);
			_mountEntity.Skeleton.SetAgentActionChannel(0, act_inventory_idle_start);
			_mountEntity.EntityFlags |= EntityFlags.AnimateWhenVisible;
			MountVisualCreator.AddMountMeshToEntity(_mountEntity, faceGenMount.HorseItem, faceGenMount.HarnessItem, faceGenMount.MountKey.ToString(), (Agent)null);
			MatrixFrame frame = gameEntity.GetGlobalFrame();
			_mountEntity.SetFrame(ref frame);
			_agentVisuals.GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.001f, _initialCharacterFrame, tickAnimsForChildren: true);
		}
	}

	private void RemoveMount()
	{
		if (_mountEntity != null)
		{
			_mountEntity.Remove(118);
		}
		_mountEntity = null;
	}

	public override void Tick(float dt)
	{
		base.Tick(dt);
		HandleEscapeMenu(this, CharacterLayer);
		_characterScene?.Tick(dt);
		AgentVisuals agentVisuals = _agentVisuals;
		if (agentVisuals != null)
		{
			agentVisuals.TickVisuals();
		}
		Vec2 vec = new Vec2(0f - CharacterLayer.Input.GetMouseMoveX(), 0f - CharacterLayer.Input.GetMouseMoveY());
		if (CharacterLayer.Input.IsHotKeyReleased("Ascend") || CharacterLayer.Input.IsHotKeyReleased("Rotate") || CharacterLayer.Input.IsHotKeyReleased("Zoom"))
		{
			GauntletLayer.InputRestrictions.SetMouseVisibility(isVisible: true);
		}
		if (CharacterLayer.Input.IsHotKeyDown("Rotate"))
		{
			_charRotationAmount = (_charRotationAmount - vec.x * 0.5f * dt) % ((float)Math.PI * 2f);
			RefreshCharacterEntityFrame();
			MBWindowManager.DontChangeCursorPos();
			GauntletLayer.InputRestrictions.SetMouseVisibility(isVisible: false);
		}
		HandleLayerInput();
	}

	private void HandleLayerInput()
	{
		if (GauntletLayer.Input.IsHotKeyReleased("Exit"))
		{
			UISoundsHelper.PlayUISound("event:/ui/panels/next");
			_dataSource.OnPreviousStage();
		}
		else if (GauntletLayer.Input.IsHotKeyReleased("Confirm") && _dataSource.CanAdvance)
		{
			UISoundsHelper.PlayUISound("event:/ui/panels/next");
			_dataSource.OnNextStage();
		}
	}

	public override void NextStage()
	{
		TextObject textObject = GameTexts.FindText("str_generic_character_firstname");
		textObject.SetTextVariable("CHARACTER_FIRSTNAME", new TextObject(_dataSource.Name));
		TextObject textObject2 = GameTexts.FindText("str_generic_character_name");
		textObject2.SetTextVariable("CHARACTER_NAME", new TextObject(_dataSource.Name));
		textObject2.SetTextVariable("CHARACTER_GENDER", Hero.MainHero.IsFemale ? 1 : 0);
		textObject.SetTextVariable("CHARACTER_GENDER", Hero.MainHero.IsFemale ? 1 : 0);
		Hero.MainHero.SetName(textObject2, textObject);
		RemoveMount();
		_affirmativeAction();
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		CharacterLayer.SceneView.SetEnable(value: false);
		CharacterLayer.SceneView.ClearAll(clearScene: false, removeTerrain: false);
		_agentVisuals.Reset();
		_agentVisuals = null;
		GauntletLayer = null;
		_dataSource?.OnFinalize();
		_dataSource = null;
		CharacterLayer = null;
		_characterScene = null;
	}

	public override int GetVirtualStageCount()
	{
		return 1;
	}

	public override void PreviousStage()
	{
		RemoveMount();
		_negativeAction();
	}

	public override IEnumerable<ScreenLayer> GetLayers()
	{
		return new List<ScreenLayer> { CharacterLayer, GauntletLayer };
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
}
