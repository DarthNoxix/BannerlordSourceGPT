using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI;

[GameStateScreen(typeof(CraftingState))]
public class GauntletCraftingScreen : ScreenBase, ICraftingStateHandler, IGameStateListener
{
	private const float _controllerRotationSensitivity = 2f;

	private Scene _craftingScene;

	private SceneLayer _sceneLayer;

	private readonly CraftingState _craftingState;

	private CraftingVM _dataSource;

	private GauntletLayer _gauntletLayer;

	private IGauntletMovie _gauntletMovie;

	private SpriteCategory _craftingCategory;

	private Camera _camera;

	private MatrixFrame _cameraFrame;

	private MatrixFrame _initialCameraFrame;

	private Vec3 _dofParams;

	private Vec2 _curCamSpeed;

	private float _zoomAmount;

	private GameEntity _craftingEntity;

	private MatrixFrame _craftingEntityFrame = MatrixFrame.Identity;

	private MatrixFrame _initialEntityFrame;

	private WeaponDesign _craftedData;

	private bool _isInitialized;

	private static KeyValuePair<string, string> _reloadXmlPath;

	private SceneView SceneView => _sceneLayer.SceneView;

	public GauntletCraftingScreen(CraftingState craftingState)
	{
		_craftingState = craftingState;
		_craftingState.Handler = this;
	}

	private void ReloadPieces()
	{
		string key = _reloadXmlPath.Key;
		string text = _reloadXmlPath.Value;
		if (!text.EndsWith(".xml"))
		{
			text += ".xml";
		}
		_reloadXmlPath = new KeyValuePair<string, string>(null, null);
		XmlDocument xmlDocument = Game.Current.ObjectManager.LoadXMLFromFileSkipValidation(ModuleHelper.GetModuleFullPath(key) + "ModuleData/" + text, "");
		if (xmlDocument == null)
		{
			return;
		}
		foreach (XmlNode childNode in xmlDocument.ChildNodes[1].ChildNodes)
		{
			XmlAttributeCollection attributes = childNode.Attributes;
			if (attributes != null)
			{
				string innerText = attributes["id"].InnerText;
				Game.Current.ObjectManager.GetObject<CraftingPiece>(innerText)?.Deserialize(Game.Current.ObjectManager, childNode);
			}
		}
		_craftingState.CraftingLogic.ReIndex(enforceReCreation: true);
		RefreshItemEntity(_dataSource.IsInCraftingMode);
		_dataSource.WeaponDesign.RefreshItem();
	}

	public void Initialize()
	{
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_craftingCategory = spriteData.SpriteCategories["ui_crafting"];
		_craftingCategory.Load(resourceContext, uIResourceDepot);
		_gauntletLayer = new GauntletLayer(1);
		_gauntletMovie = _gauntletLayer.LoadMovie("Crafting", _dataSource);
		_gauntletLayer.InputRestrictions.SetInputRestrictions();
		_gauntletLayer.InputRestrictions.SetCanOverrideFocusOnHit(canOverrideFocusOnHit: true);
		AddLayer(_gauntletLayer);
		OpenScene();
		RefreshItemEntity(isItemVisible: true);
		_isInitialized = true;
		Game.Current?.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.CraftingScreen));
	}

	protected override void OnInitialize()
	{
		Initialize();
		_sceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
		_sceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("CraftingHotkeyCategory"));
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("CraftingHotkeyCategory"));
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		Game.Current?.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.None));
		SceneView.ClearAll(clearScene: true, removeTerrain: true);
		_craftingCategory.Unload();
		_dataSource?.OnFinalize();
	}

	protected override void OnFrameTick(float dt)
	{
		LoadingWindow.DisableGlobalLoadingWindow();
		base.OnFrameTick(dt);
		_dataSource.CanSwitchTabs = !Input.IsGamepadActive || !InformationManager.GetIsAnyTooltipActiveAndExtended();
		_dataSource.AreGamepadControlHintsEnabled = Input.IsGamepadActive && _sceneLayer.IsHitThisFrame && _dataSource.IsInCraftingMode;
		if (_dataSource.IsInCraftingMode)
		{
			_dataSource.WeaponDesign.WeaponControlsEnabled = _sceneLayer.IsHitThisFrame;
		}
		if (_sceneLayer.Input.IsControlDown() || _gauntletLayer.Input.IsControlDown())
		{
			if (_sceneLayer.Input.IsHotKeyPressed("Copy") || _gauntletLayer.Input.IsHotKeyPressed("Copy"))
			{
				CopyXmlCode();
			}
			else if (_sceneLayer.Input.IsHotKeyPressed("Paste") || _gauntletLayer.Input.IsHotKeyPressed("Paste"))
			{
				PasteXmlCode();
			}
		}
		if (_craftingState.CraftingLogic.CurrentCraftingTemplate == null)
		{
			return;
		}
		if (!_sceneLayer.Input.IsHotKeyDown("Rotate") && !_sceneLayer.Input.IsHotKeyDown("Zoom"))
		{
			_sceneLayer.InputRestrictions.SetMouseVisibility(isVisible: true);
		}
		_craftingScene.Tick(dt);
		if (Input.IsGamepadActive || (!_gauntletLayer.IsFocusedOnInput() && !_sceneLayer.IsFocusedOnInput()))
		{
			if (IsHotKeyReleasedInAnyLayer("Exit"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_dataSource.ExecuteCancel();
			}
			else if (IsHotKeyReleasedInAnyLayer("Confirm"))
			{
				bool isInCraftingMode = _dataSource.IsInCraftingMode;
				bool isInRefinementMode = _dataSource.IsInRefinementMode;
				bool isInSmeltingMode = _dataSource.IsInSmeltingMode;
				var (flag, flag2) = _dataSource.ExecuteConfirm();
				if (flag)
				{
					if (flag2)
					{
						if (isInCraftingMode)
						{
							UISoundsHelper.PlayUISound("event:/ui/crafting/craft_success");
						}
						else if (isInRefinementMode)
						{
							UISoundsHelper.PlayUISound("event:/ui/crafting/refine_success");
						}
						else if (isInSmeltingMode)
						{
							UISoundsHelper.PlayUISound("event:/ui/crafting/smelt_success");
						}
					}
					else
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
					}
				}
			}
			else if (_dataSource.CanSwitchTabs)
			{
				if (IsHotKeyReleasedInAnyLayer("SwitchToPreviousTab"))
				{
					if (_dataSource.IsInSmeltingMode)
					{
						UISoundsHelper.PlayUISound("event:/ui/crafting/refine_tab");
						_dataSource.ExecuteSwitchToRefinement();
					}
					else if (_dataSource.IsInCraftingMode)
					{
						UISoundsHelper.PlayUISound("event:/ui/crafting/smelt_tab");
						_dataSource.ExecuteSwitchToSmelting();
					}
					else if (_dataSource.IsInRefinementMode)
					{
						UISoundsHelper.PlayUISound("event:/ui/crafting/craft_tab");
						_dataSource.ExecuteSwitchToCrafting();
					}
				}
				else if (IsHotKeyReleasedInAnyLayer("SwitchToNextTab"))
				{
					if (_dataSource.IsInSmeltingMode)
					{
						UISoundsHelper.PlayUISound("event:/ui/crafting/craft_tab");
						_dataSource.ExecuteSwitchToCrafting();
					}
					else if (_dataSource.IsInCraftingMode)
					{
						UISoundsHelper.PlayUISound("event:/ui/crafting/refine_tab");
						_dataSource.ExecuteSwitchToRefinement();
					}
					else if (_dataSource.IsInRefinementMode)
					{
						UISoundsHelper.PlayUISound("event:/ui/crafting/smelt_tab");
						_dataSource.ExecuteSwitchToSmelting();
					}
				}
			}
		}
		bool flag3 = false;
		if (_reloadXmlPath.Key != null && _reloadXmlPath.Value != null)
		{
			ReloadPieces();
			flag3 = true;
		}
		if (flag3)
		{
			return;
		}
		if (base.DebugInput.IsHotKeyPressed("Reset"))
		{
			OnResetCamera();
		}
		if (_dataSource.IsInCraftingMode)
		{
			float num = 0f;
			float num2 = 0f;
			if (Input.IsGamepadActive)
			{
				num = _sceneLayer.Input.GetGameKeyAxis("CameraAxisX");
				num2 = _sceneLayer.Input.GetGameKeyAxis("CameraAxisY");
			}
			else if (_sceneLayer.Input.IsHotKeyDown("Rotate") || _sceneLayer.Input.IsHotKeyDown("Zoom"))
			{
				num = _sceneLayer.Input.GetMouseMoveX();
				num2 = _sceneLayer.Input.GetMouseMoveY();
			}
			if (num != 0f || num2 != 0f)
			{
				OnMouseMove(num, num2, dt);
			}
			ZoomTick(dt);
		}
		_craftingScene.SetDepthOfFieldParameters(_dofParams.x, _dofParams.z, isVignetteOn: false);
		_craftingScene.SetDepthOfFieldFocus(_initialEntityFrame.origin.Distance(_cameraFrame.origin));
		if (_dataSource.IsInCraftingMode)
		{
			_craftingEntity.SetFrame(ref _craftingEntityFrame);
		}
		SceneView.SetCamera(_camera);
	}

	private void OnClose()
	{
		CampaignMission.Current?.EndMission();
		Game.Current.GameStateManager.PopState();
	}

	private void OnResetCamera()
	{
		_sceneLayer.InputRestrictions.SetMouseVisibility(isVisible: true);
		ResetEntityAndCamera();
	}

	private void OnWeaponCrafted()
	{
		_dataSource.WeaponDesign.CraftingResultPopup?.SetDoneInputKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetHotKey("Confirm"));
	}

	public void OnCraftingLogicInitialized()
	{
		_dataSource?.OnFinalize();
		_dataSource = new CraftingVM(_craftingState.CraftingLogic, OnClose, OnResetCamera, OnWeaponCrafted, GetItemUsageSetFlag)
		{
			OnItemRefreshed = RefreshItemEntity
		};
		_dataSource.WeaponDesign.CraftingHistory.SetDoneKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetHotKey("Confirm"));
		_dataSource.WeaponDesign.CraftingHistory.SetCancelKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetHotKey("Exit"));
		_dataSource.SetConfirmInputKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetHotKey("Confirm"));
		_dataSource.SetExitInputKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetHotKey("Exit"));
		_dataSource.SetPreviousTabInputKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetHotKey("SwitchToPreviousTab"));
		_dataSource.SetNextTabInputKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetHotKey("SwitchToNextTab"));
		_dataSource.AddCameraControlInputKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetGameKey(55));
		_dataSource.AddCameraControlInputKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetGameKey(56));
		_dataSource.AddCameraControlInputKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").RegisteredGameAxisKeys.FirstOrDefault((GameAxisKey x) => x.Id == "CameraAxisX"));
	}

	public void OnCraftingLogicRefreshed()
	{
		_dataSource.OnCraftingLogicRefreshed(_craftingState.CraftingLogic);
		if (_isInitialized)
		{
			RefreshItemEntity(isItemVisible: true);
		}
	}

	private void OpenScene()
	{
		_craftingScene = Scene.CreateNewScene(initialize_physics: true, enable_decals: false);
		_craftingScene.SetName("GauntletCraftingScreen");
		SceneInitializationData initData = default(SceneInitializationData);
		initData.InitPhysicsWorld = false;
		_craftingScene.Read("crafting_menu_outdoor", ref initData);
		_craftingScene.DisableStaticShadows(value: true);
		_craftingScene.SetShadow(shadowEnabled: true);
		_craftingScene.SetClothSimulationState(state: true);
		InitializeEntityAndCamera();
		_sceneLayer = new SceneLayer();
		_sceneLayer.IsFocusLayer = true;
		_sceneLayer.InputRestrictions.SetCanOverrideFocusOnHit(canOverrideFocusOnHit: true);
		AddLayer(_sceneLayer);
		SceneView.SetScene(_craftingScene);
		SceneView.SetCamera(_camera);
		SceneView.SetSceneUsesShadows(value: true);
		SceneView.SetAcceptGlobalDebugRenderObjects(value: true);
		SceneView.SetRenderWithPostfx(value: true);
		SceneView.SetResolutionScaling(value: true);
	}

	private void InitializeEntityAndCamera()
	{
		GameEntity gameEntity = _craftingScene.FindEntityWithTag("weapon_point");
		MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
		_craftingScene.RemoveEntity(gameEntity, 114);
		globalFrame.Elevate(1.6f);
		_craftingEntityFrame = globalFrame;
		_initialEntityFrame = _craftingEntityFrame;
		_craftingEntity = GameEntity.CreateEmpty(_craftingScene);
		_craftingEntity.SetFrame(ref _craftingEntityFrame);
		_camera = Camera.CreateCamera();
		_dofParams = default(Vec3);
		_curCamSpeed = new Vec2(0f, 0f);
		GameEntity gameEntity2 = _craftingScene.FindEntityWithTag("camera_point");
		gameEntity2.GetCameraParamsFromCameraScript(_camera, ref _dofParams);
		float fovVertical = _camera.GetFovVertical();
		float aspectRatio = Screen.AspectRatio;
		float near = _camera.Near;
		float far = _camera.Far;
		_camera.SetFovVertical(fovVertical, aspectRatio, near, far);
		_craftingScene.SetDepthOfFieldParameters(_dofParams.x, _dofParams.z, isVignetteOn: false);
		_craftingScene.SetDepthOfFieldFocus(_dofParams.y);
		_cameraFrame = gameEntity2.GetFrame();
		_initialCameraFrame = _cameraFrame;
	}

	private void RefreshItemEntity(bool isItemVisible)
	{
		_dataSource.WeaponDesign.CurrentWeaponHasScabbard = false;
		if (_craftingEntity != null)
		{
			_craftingEntityFrame = _craftingEntity.GetFrame();
			_craftingEntity.Remove(115);
			_craftingEntity = null;
		}
		if (!isItemVisible)
		{
			return;
		}
		_craftingEntity = GameEntity.CreateEmpty(_craftingScene);
		_craftingEntity.SetFrame(ref _craftingEntityFrame);
		_craftedData = _craftingState.CraftingLogic.CurrentWeaponDesign;
		if (!(_craftedData != null))
		{
			return;
		}
		_craftingEntityFrame = _craftingEntity.GetFrame();
		float num = _craftedData.CraftedWeaponLength / 2f;
		_craftingEntity.SetFrame(ref _craftingEntityFrame);
		BladeData bladeData = _craftedData.UsedPieces[0].CraftingPiece.BladeData;
		_dataSource.WeaponDesign.CurrentWeaponHasScabbard = !string.IsNullOrEmpty(bladeData.HolsterMeshName);
		MetaMesh metaMesh;
		if (!_dataSource.WeaponDesign.IsScabbardVisible)
		{
			metaMesh = CraftedDataView.BuildWeaponMesh(_craftedData, 0f - num, false, false);
		}
		else
		{
			metaMesh = CraftedDataView.BuildHolsterMeshWithWeapon(_craftedData, 0f - num, false);
			if (metaMesh == null)
			{
				metaMesh = CraftedDataView.BuildWeaponMesh(_craftedData, 0f - num, false, false);
			}
		}
		_craftingEntity = _craftingScene.AddItemEntity(ref _craftingEntityFrame, metaMesh);
	}

	private void OnMouseMove(float deltaX, float deltaY, float dT)
	{
		if (base.DebugInput.IsControlDown() || base.DebugInput.IsAltDown())
		{
			return;
		}
		if (Input.IsGamepadActive)
		{
			if (Mathf.Abs(deltaX) > 0.1f)
			{
				deltaX = (deltaX - Mathf.Sign(deltaX) * 0.1f) / 0.9f;
				_craftingEntityFrame.rotation.RotateAboutUp(2f * deltaX * (float)Math.PI / 180f);
			}
			if (Mathf.Abs(deltaY) > 0.1f)
			{
				deltaY = (deltaY - Mathf.Sign(deltaY) * 0.1f) / 0.9f;
				_craftingEntityFrame.rotation.RotateAboutSide(2f * deltaY * (float)Math.PI / 180f);
			}
		}
		else if (_sceneLayer.Input.IsHotKeyDown("Rotate"))
		{
			Vec2 vec = new Vec2(0.02f, 0.02f);
			Vec2 vec2 = new Vec2(deltaX, 0f - deltaY);
			Vec2 vec3 = new Vec2(vec2.x / vec.x, vec2.y / vec.y);
			Vec2 vec4 = new Vec2(dT * vec3.x, dT * vec3.y);
			float num = 0.95f;
			_curCamSpeed = _curCamSpeed * num + vec4;
			Vec2 vec5 = new Vec2(_curCamSpeed.x * dT, _curCamSpeed.y * dT);
			_craftingEntityFrame.rotation.RotateAboutAnArbitraryVector(Vec3.Side, vec5.y * (float)Math.PI / 180f);
			_craftingEntityFrame.rotation.RotateAboutAnArbitraryVector(Vec3.Up, vec5.x * (float)Math.PI / 180f);
			MBWindowManager.DontChangeCursorPos();
			_sceneLayer.InputRestrictions.SetMouseVisibility(isVisible: false);
			_gauntletLayer.InputRestrictions.SetMouseVisibility(isVisible: false);
		}
		else if (_sceneLayer.Input.IsHotKeyDown("Zoom"))
		{
			float num2 = ((TaleWorlds.Library.MathF.Abs(deltaX) >= TaleWorlds.Library.MathF.Abs(deltaY)) ? deltaX : deltaY);
			_craftingEntityFrame.rotation.RotateAboutUp(num2 * (float)Math.PI / 180f * 0.15f);
			MBWindowManager.DontChangeCursorPos();
			_sceneLayer.InputRestrictions.SetMouseVisibility(isVisible: false);
			_gauntletLayer.InputRestrictions.SetMouseVisibility(isVisible: false);
		}
		if (_sceneLayer.Input.IsHotKeyDown("Rotate") && _sceneLayer.Input.IsHotKeyDown("Zoom"))
		{
			ResetEntityAndCamera();
		}
	}

	private float GetActiveZoomAmount()
	{
		if (Input.IsGamepadActive)
		{
			float gameKeyState = _sceneLayer.Input.GetGameKeyState(55);
			return _sceneLayer.Input.GetGameKeyState(56) - gameKeyState;
		}
		return MBMath.ClampFloat(_zoomAmount - (float)TaleWorlds.Library.MathF.Sign(_sceneLayer.Input.GetDeltaMouseScroll()) * 0.05f, -0.6f, 0.5f);
	}

	private void ZoomTick(float dt)
	{
		_zoomAmount = GetActiveZoomAmount();
		if (TaleWorlds.Library.MathF.Abs(_zoomAmount) < 1E-05f)
		{
			_zoomAmount = 0f;
			return;
		}
		int num = TaleWorlds.Library.MathF.Sign(_zoomAmount);
		Vec3 vec = -num * (_initialEntityFrame.origin - _cameraFrame.origin);
		vec.Normalize();
		float num2 = (Input.IsGamepadActive ? 2f : 5f);
		float num3 = dt * num2;
		_cameraFrame.origin += vec * num3;
		_zoomAmount += (float)(-num) * num3;
		float num4 = _initialEntityFrame.origin.Distance(_cameraFrame.origin);
		if (num4 > 3.3f)
		{
			_cameraFrame.origin += -num * vec * (num4 - 3.3f);
			num4 = _initialEntityFrame.origin.Distance(_cameraFrame.origin);
			_zoomAmount = 0f;
		}
		else if (num4 < 0.55f)
		{
			_cameraFrame.origin += -num * vec * (num4 - 0.55f);
			num4 = _initialEntityFrame.origin.Distance(_cameraFrame.origin);
			_zoomAmount = 0f;
		}
		else if (num != TaleWorlds.Library.MathF.Sign(_zoomAmount))
		{
			_zoomAmount = 0f;
		}
		_camera.Frame = _cameraFrame;
	}

	private void ResetEntityAndCamera()
	{
		_zoomAmount = 0f;
		_craftingEntityFrame = _initialEntityFrame;
		_cameraFrame = _initialCameraFrame;
		_camera.Frame = _cameraFrame;
	}

	private void CopyXmlCode()
	{
		Input.SetClipboardText(_craftingState.CraftingLogic.GetXmlCodeForCurrentItem(_craftingState.CraftingLogic.GetCurrentCraftedItemObject()));
	}

	private void PasteXmlCode()
	{
		string clipboardText = Input.GetClipboardText();
		if (!string.IsNullOrEmpty(clipboardText))
		{
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(clipboardText);
			CraftingTemplate craftingTemplate;
			(CraftingPiece, int)[] pieces;
			if (@object != null)
			{
				SwithToCraftedItem(@object);
			}
			else if (_craftingState.CraftingLogic.TryGetWeaponPropertiesFromXmlCode(clipboardText, out craftingTemplate, out pieces))
			{
				_dataSource.SetCurrentDesignManually(craftingTemplate, pieces);
			}
		}
	}

	private void SwithToCraftedItem(ItemObject itemObject)
	{
		if (itemObject == null || !itemObject.IsCraftedWeapon)
		{
			return;
		}
		if (!_dataSource.IsInCraftingMode)
		{
			_dataSource.ExecuteSwitchToCrafting();
		}
		WeaponDesign weaponDesign = itemObject.WeaponDesign;
		if (_craftingState.CraftingLogic.CurrentCraftingTemplate != weaponDesign.Template)
		{
			_dataSource.WeaponDesign.SelectPrimaryWeaponClass(weaponDesign.Template);
		}
		WeaponDesignElement[] usedPieces = weaponDesign.UsedPieces;
		foreach (WeaponDesignElement weaponDesignElement in usedPieces)
		{
			if (weaponDesignElement.IsValid)
			{
				_dataSource.WeaponDesign.SwitchToPiece(weaponDesignElement);
			}
		}
	}

	private ItemObject.ItemUsageSetFlags GetItemUsageSetFlag(WeaponComponentData item)
	{
		if (!string.IsNullOrEmpty(item.ItemUsage))
		{
			return MBItem.GetItemUsageSetFlags(item.ItemUsage);
		}
		return (ItemObject.ItemUsageSetFlags)0;
	}

	private bool IsHotKeyReleasedInAnyLayer(string hotKeyId)
	{
		if (!_sceneLayer.Input.IsHotKeyReleased(hotKeyId))
		{
			return _gauntletLayer.Input.IsHotKeyReleased(hotKeyId);
		}
		return true;
	}

	void IGameStateListener.OnInitialize()
	{
	}

	void IGameStateListener.OnFinalize()
	{
	}

	void IGameStateListener.OnActivate()
	{
	}

	void IGameStateListener.OnDeactivate()
	{
	}
}
