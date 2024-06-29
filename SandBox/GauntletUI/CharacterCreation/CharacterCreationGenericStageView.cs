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

[CharacterCreationStageView(typeof(CharacterCreationGenericStage))]
public class CharacterCreationGenericStageView : CharacterCreationStageViewBase
{
	protected readonly TextObject _affirmativeActionText;

	protected readonly TextObject _negativeActionText;

	private IGauntletMovie _movie;

	private GauntletLayer GauntletLayer;

	private CharacterCreationGenericStageVM _dataSource;

	private int _stageIndex;

	private readonly ActionIndexCache act_inventory_idle_start = ActionIndexCache.Create("act_inventory_idle_start");

	private readonly ActionIndexCache act_horse_stand_1 = ActionIndexCache.Create("act_horse_stand_1");

	private readonly TaleWorlds.CampaignSystem.CharacterCreationContent.CharacterCreation _characterCreation;

	private Scene _characterScene;

	private Camera _camera;

	private MatrixFrame _initialCharacterFrame;

	private List<AgentVisuals> _playerOrParentAgentVisuals;

	private List<AgentVisuals> _playerOrParentAgentVisualsPrevious;

	private int _checkForVisualVisibility;

	private GameEntity _mountEntityToPrepare;

	private GameEntity _mountEntityToShow;

	private EscapeMenuVM _escapeMenuDatasource;

	private IGauntletMovie _escapeMenuMovie;

	public SceneLayer CharacterLayer { get; private set; }

	public CharacterCreationGenericStageView(TaleWorlds.CampaignSystem.CharacterCreationContent.CharacterCreation characterCreation, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage onRefresh, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction, ControlCharacterCreationStageReturnInt getTotalStageCountAction, ControlCharacterCreationStageReturnInt getFurthestIndexAction, ControlCharacterCreationStageWithInt goToIndexAction)
		: base(affirmativeAction, negativeAction, onRefresh, getCurrentStageIndexAction, getTotalStageCountAction, getFurthestIndexAction, goToIndexAction)
	{
		_characterCreation = characterCreation;
		_affirmativeActionText = affirmativeActionText;
		_negativeActionText = negativeActionText;
		GauntletLayer = new GauntletLayer(1);
		GauntletLayer.InputRestrictions.SetInputRestrictions();
		GauntletLayer.IsFocusLayer = true;
		GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		ScreenManager.TrySetFocus(GauntletLayer);
		_dataSource = new CharacterCreationGenericStageVM(_characterCreation, NextStage, _affirmativeActionText, PreviousStage, _negativeActionText, _stageIndex, getCurrentStageIndexAction(), getTotalStageCountAction(), getFurthestIndexAction(), GoToIndex)
		{
			OnOptionSelection = OnSelectionChanged
		};
		CreateHotKeyVisuals();
		_movie = GauntletLayer.LoadMovie("CharacterCreationGenericStage", _dataSource);
	}

	public override void SetGenericScene(Scene scene)
	{
		OpenScene(scene);
		RefreshCharacterEntity();
		RefreshMountEntity();
	}

	private void CreateHotKeyVisuals()
	{
		_dataSource?.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		_dataSource?.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
	}

	private void OpenScene(Scene cachedScene)
	{
		_characterScene = cachedScene;
		_characterScene.SetShadow(shadowEnabled: true);
		_characterScene.SetDynamicShadowmapCascadesRadiusMultiplier(0.1f);
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
		if (!CharacterLayer.Input.IsCategoryRegistered(HotKeyManager.GetCategory("FaceGenHotkeyCategory")))
		{
			CharacterLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
		}
		int num = -1;
		num &= -5;
		CharacterLayer.SetPostfxConfigParams(num);
		CharacterLayer.SetPostfxFromConfig();
		_characterScene.FindEntityWithName("cradle")?.SetVisibilityExcludeParents(visible: false);
	}

	private void RefreshCharacterEntity()
	{
		List<float> list = new List<float>();
		bool isPlayerAlone = _characterCreation.IsPlayerAlone;
		bool hasSecondaryCharacter = _characterCreation.HasSecondaryCharacter;
		if (_playerOrParentAgentVisuals != null && _characterCreation.FaceGenChars.Count == 1)
		{
			foreach (AgentVisuals playerOrParentAgentVisual in _playerOrParentAgentVisuals)
			{
				Skeleton skeleton = playerOrParentAgentVisual.GetVisuals().GetSkeleton();
				list.Add(skeleton.GetAnimationParameterAtChannel(0));
			}
		}
		if (_playerOrParentAgentVisualsPrevious != null)
		{
			foreach (AgentVisuals playerOrParentAgentVisualsPreviou in _playerOrParentAgentVisualsPrevious)
			{
				playerOrParentAgentVisualsPreviou.Reset();
			}
		}
		_playerOrParentAgentVisualsPrevious = new List<AgentVisuals>();
		if (_playerOrParentAgentVisuals != null)
		{
			foreach (AgentVisuals playerOrParentAgentVisual2 in _playerOrParentAgentVisuals)
			{
				_playerOrParentAgentVisualsPrevious.Add(playerOrParentAgentVisual2);
			}
		}
		_checkForVisualVisibility = 1;
		if (_characterCreation.FaceGenChars.Count > 0)
		{
			_playerOrParentAgentVisuals = new List<AgentVisuals>();
			int num = _characterCreation.FaceGenChars.Count;
			int num2 = 0;
			{
				foreach (FaceGenChar faceGenChar in _characterCreation.FaceGenChars)
				{
					string tag = (isPlayerAlone ? "spawnpoint_player_1" : "spawnpoint_player_3");
					if (hasSecondaryCharacter)
					{
						if (_characterCreation.FaceGenChars.ElementAt(num2).ActionName.ToString().Contains("horse"))
						{
							tag = "spawnpoint_mount_1";
						}
						else
						{
							switch (num2)
							{
							case 0:
								tag = "spawnpoint_player_brother_stage";
								break;
							case 1:
								tag = "spawnpoint_brother_brother_stage";
								break;
							}
						}
					}
					GameEntity gameEntity = _characterScene.FindEntityWithTag(tag);
					_initialCharacterFrame = gameEntity.GetFrame();
					_initialCharacterFrame.origin.z = 0f;
					AgentVisuals val = AgentVisuals.Create(CreateAgentVisual(faceGenChar, _initialCharacterFrame, isPlayerAlone, (GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent.GetSelectedParentType(), num2 == 2), "facegenvisual" + num, false, false, false);
					val.SetVisible(false);
					_playerOrParentAgentVisuals.Add(val);
					_playerOrParentAgentVisuals[num2].GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.001f, _initialCharacterFrame, tickAnimsForChildren: true);
					if (isPlayerAlone || hasSecondaryCharacter)
					{
						ActionIndexCache actionIndex = ActionIndexCache.Create(_characterCreation.FaceGenChars?.ElementAt(num2).ActionName);
						_playerOrParentAgentVisuals[num2].GetVisuals().GetSkeleton().SetAgentActionChannel(0, actionIndex);
					}
					if (num2 == 0 && !string.IsNullOrEmpty(_characterCreation.PrefabId) && GameEntity.Instantiate(_characterScene, _characterCreation.PrefabId, callScriptCallbacks: true) != null)
					{
						_playerOrParentAgentVisuals[num2].AddPrefabToAgentVisualBoneByRealBoneIndex(_characterCreation.PrefabId, _characterCreation.PrefabBoneUsage);
					}
					_playerOrParentAgentVisuals[num2].SetAgentLodZeroOrMax(true);
					_playerOrParentAgentVisuals[num2].GetEntity().SetEnforcedMaximumLodLevel(0);
					_playerOrParentAgentVisuals[num2].GetEntity().CheckResources(addToQueue: true, checkFaceResources: true);
					CharacterLayer.SetFocusedShadowmap(enable: true, ref _initialCharacterFrame.origin, 0.59999996f);
					num++;
					num2++;
				}
				return;
			}
		}
		_playerOrParentAgentVisuals = null;
	}

	private void RefreshMountEntity()
	{
		RemoveShownMount();
		if (_characterCreation.FaceGenMount != null)
		{
			GameEntity gameEntity = _characterScene.FindEntityWithTag("spawnpoint_mount_1");
			HorseComponent horseComponent = _characterCreation.FaceGenMount.HorseItem.HorseComponent;
			Monster monster = horseComponent.Monster;
			_mountEntityToPrepare = GameEntity.CreateEmpty(_characterScene);
			AnimationSystemData animationSystemData = monster.FillAnimationSystemData(MBGlobals.GetActionSet(horseComponent.Monster.ActionSetCode), 1f, hasClippingPlane: false);
			_mountEntityToPrepare.CreateSkeletonWithActionSet(ref animationSystemData);
			MBSkeletonExtensions.SetAgentActionChannel(actionIndex: ActionIndexCache.Create(_characterCreation.FaceGenMount.ActionName), skeleton: _mountEntityToPrepare.Skeleton, actionChannelNo: 0);
			_mountEntityToPrepare.EntityFlags |= EntityFlags.AnimateWhenVisible;
			MountVisualCreator.AddMountMeshToEntity(_mountEntityToPrepare, _characterCreation.FaceGenMount.HorseItem, _characterCreation.FaceGenMount.HarnessItem, _characterCreation.FaceGenMount.MountKey.ToString(), (Agent)null);
			MatrixFrame frame = gameEntity.GetGlobalFrame();
			_mountEntityToPrepare.SetFrame(ref frame);
			_mountEntityToPrepare.SetVisibilityExcludeParents(visible: false);
			_mountEntityToPrepare.SetEnforcedMaximumLodLevel(0);
			_mountEntityToPrepare.CheckResources(addToQueue: true, checkFaceResources: false);
		}
	}

	private void RemoveShownMount()
	{
		if (_mountEntityToShow != null)
		{
			_mountEntityToShow.Remove(116);
		}
		_mountEntityToShow = _mountEntityToPrepare;
		_mountEntityToPrepare = null;
	}

	private AgentVisualsData CreateAgentVisual(FaceGenChar character, MatrixFrame characterFrame, bool isPlayerEntity, int selectedParentType = 0, bool isChildAgent = false)
	{
		ActionIndexCache actionCode = (isChildAgent ? ActionIndexCache.Create("act_character_creation_toddler_" + selectedParentType) : ActionIndexCache.Create(character.IsFemale ? ("act_character_creation_female_default_" + selectedParentType) : ("act_character_creation_male_default_" + selectedParentType)));
		Monster baseMonsterFromRace = TaleWorlds.Core.FaceGen.GetBaseMonsterFromRace(character.Race);
		AgentVisualsData agentVisualsData = new AgentVisualsData().UseMorphAnims(useMorphAnims: true).Equipment(character.Equipment).BodyProperties(character.BodyProperties)
			.Frame(characterFrame)
			.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, character.IsFemale, "_facegen"))
			.ActionCode(actionCode)
			.Scene(_characterScene)
			.Monster(baseMonsterFromRace)
			.UseTranslucency(useTranslucency: true)
			.UseTesselation(useTesselation: true)
			.RightWieldedItemIndex(0)
			.LeftWieldedItemIndex(1)
			.Race(CharacterObject.PlayerCharacter.Race)
			.SkeletonType(character.IsFemale ? SkeletonType.Female : SkeletonType.Male);
		CharacterCreationContentBase currentCharacterCreationContent = ((CharacterCreationState)GameStateManager.Current.ActiveState).CurrentCharacterCreationContent;
		if (currentCharacterCreationContent.GetSelectedCulture() != null)
		{
			agentVisualsData.ClothColor1(currentCharacterCreationContent.GetSelectedCulture().Color);
			agentVisualsData.ClothColor2(currentCharacterCreationContent.GetSelectedCulture().Color2);
		}
		if (!isPlayerEntity && !isChildAgent)
		{
			agentVisualsData.Scale(character.IsFemale ? 0.99f : 1f);
		}
		if (!isPlayerEntity && isChildAgent)
		{
			agentVisualsData.Scale(0.5f);
		}
		return agentVisualsData;
	}

	private void OnSelectionChanged()
	{
		RefreshCharacterEntity();
		RefreshMountEntity();
	}

	public override void Tick(float dt)
	{
		base.Tick(dt);
		HandleEscapeMenu(this, CharacterLayer);
		_characterScene?.Tick(dt);
		foreach (AgentVisuals playerOrParentAgentVisual in _playerOrParentAgentVisuals)
		{
			playerOrParentAgentVisual.TickVisuals();
		}
		if (_playerOrParentAgentVisuals != null && _checkForVisualVisibility > 0)
		{
			bool flag = true;
			foreach (AgentVisuals playerOrParentAgentVisual2 in _playerOrParentAgentVisuals)
			{
				if (!playerOrParentAgentVisual2.GetEntity().CheckResources(addToQueue: false, checkFaceResources: true))
				{
					flag = false;
				}
			}
			if (_mountEntityToPrepare != null && !_mountEntityToPrepare.CheckResources(addToQueue: false, checkFaceResources: true))
			{
				flag = false;
			}
			if (flag)
			{
				_checkForVisualVisibility--;
				if (_checkForVisualVisibility == 0)
				{
					foreach (AgentVisuals playerOrParentAgentVisual3 in _playerOrParentAgentVisuals)
					{
						playerOrParentAgentVisual3.SetVisible(true);
					}
					foreach (AgentVisuals playerOrParentAgentVisualsPreviou in _playerOrParentAgentVisualsPrevious)
					{
						playerOrParentAgentVisualsPreviou.SetVisible(false);
						playerOrParentAgentVisualsPreviou.Reset();
					}
					if (_mountEntityToPrepare != null)
					{
						_mountEntityToPrepare.SetVisibilityExcludeParents(visible: true);
					}
					if (_mountEntityToShow != null)
					{
						_mountEntityToShow.SetVisibilityExcludeParents(visible: false);
						_characterScene.RemoveEntity(_mountEntityToShow, 116);
					}
					_mountEntityToShow = _mountEntityToPrepare;
					_mountEntityToPrepare = null;
					_playerOrParentAgentVisualsPrevious.Clear();
				}
			}
		}
		if (CharacterLayer.Input.IsHotKeyReleased("Ascend") || CharacterLayer.Input.IsHotKeyReleased("Rotate") || CharacterLayer.Input.IsHotKeyReleased("Zoom"))
		{
			GauntletLayer.InputRestrictions.SetMouseVisibility(isVisible: true);
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
		_stageIndex++;
		if (_stageIndex < _characterCreation.CharacterCreationMenuCount)
		{
			if (_movie != null)
			{
				GauntletLayer.ReleaseMovie(_movie);
				_movie = null;
			}
			if (_dataSource != null)
			{
				_dataSource.OnOptionSelection = null;
			}
			_dataSource = new CharacterCreationGenericStageVM(_characterCreation, NextStage, _affirmativeActionText, PreviousStage, _negativeActionText, _stageIndex, _getCurrentStageIndexAction(), _getTotalStageCountAction(), _getFurthestIndexAction(), GoToIndex)
			{
				OnOptionSelection = OnSelectionChanged
			};
			CreateHotKeyVisuals();
			_movie = GauntletLayer.LoadMovie("CharacterCreationGenericStage", _dataSource);
			RefreshCharacterEntity();
			RefreshMountEntity();
		}
		else
		{
			RefreshMountEntity();
			_affirmativeAction();
		}
	}

	public override void PreviousStage()
	{
		_stageIndex--;
		if (_stageIndex >= 0)
		{
			if (_movie != null)
			{
				GauntletLayer.ReleaseMovie(_movie);
				_movie = null;
			}
			if (_dataSource != null)
			{
				_dataSource.OnOptionSelection = null;
			}
			_dataSource = new CharacterCreationGenericStageVM(_characterCreation, NextStage, _affirmativeActionText, PreviousStage, _negativeActionText, _stageIndex, _getCurrentStageIndexAction(), _getTotalStageCountAction(), _getFurthestIndexAction(), GoToIndex)
			{
				OnOptionSelection = OnSelectionChanged
			};
			CreateHotKeyVisuals();
			_movie = GauntletLayer.LoadMovie("CharacterCreationGenericStage", _dataSource);
			RefreshCharacterEntity();
			RefreshMountEntity();
		}
		else
		{
			RefreshMountEntity();
			_negativeAction();
		}
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		if (_playerOrParentAgentVisuals != null)
		{
			foreach (AgentVisuals playerOrParentAgentVisual in _playerOrParentAgentVisuals)
			{
				playerOrParentAgentVisual.Reset();
			}
		}
		if (_playerOrParentAgentVisualsPrevious != null)
		{
			foreach (AgentVisuals playerOrParentAgentVisualsPreviou in _playerOrParentAgentVisualsPrevious)
			{
				playerOrParentAgentVisualsPreviou.Reset();
			}
		}
		CharacterLayer.SceneView.SetEnable(value: false);
		CharacterLayer.SceneView.ClearAll(clearScene: false, removeTerrain: false);
		_playerOrParentAgentVisuals = null;
		_playerOrParentAgentVisualsPrevious = null;
		GauntletLayer = null;
		_dataSource?.OnFinalize();
		_dataSource = null;
		CharacterLayer = null;
		_characterScene = null;
	}

	public override int GetVirtualStageCount()
	{
		return _characterCreation.CharacterCreationMenuCount;
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
