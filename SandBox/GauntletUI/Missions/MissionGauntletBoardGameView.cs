using System;
using SandBox.BoardGames.MissionLogics;
using SandBox.ViewModelCollection.BoardGame;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Missions;

[OverrideView(typeof(BoardGameView))]
public class MissionGauntletBoardGameView : MissionView, IBoardGameHandler
{
	private BoardGameVM _dataSource;

	private GauntletLayer _gauntletLayer;

	private IGauntletMovie _gauntletMovie;

	private GameEntity _cameraHolder;

	private SpriteCategory _spriteCategory;

	private bool _missionMouseVisibilityState;

	private InputUsageMask _missionInputRestrictions;

	public MissionBoardGameLogic _missionBoardGameHandler { get; private set; }

	public Camera Camera { get; private set; }

	public MissionGauntletBoardGameView()
	{
		base.ViewOrderPriority = 2;
	}

	public override void OnMissionScreenInitialize()
	{
		((MissionView)this).OnMissionScreenInitialize();
		((MissionView)this).MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("BoardGameHotkeyCategory"));
	}

	public override void OnMissionScreenActivate()
	{
		((MissionView)this).OnMissionScreenActivate();
		_missionBoardGameHandler = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionBoardGameLogic>();
		if (_missionBoardGameHandler != null)
		{
			_missionBoardGameHandler.Handler = this;
		}
	}

	void IBoardGameHandler.Activate()
	{
		_dataSource.Activate();
	}

	void IBoardGameHandler.SwitchTurns()
	{
		_dataSource?.SwitchTurns();
	}

	void IBoardGameHandler.DiceRoll(int roll)
	{
		_dataSource?.DiceRoll(roll);
	}

	void IBoardGameHandler.Install()
	{
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_spriteCategory = spriteData.SpriteCategories["ui_boardgame"];
		_spriteCategory.Load(resourceContext, uIResourceDepot);
		_dataSource = new BoardGameVM();
		_dataSource.SetRollDiceKey(HotKeyManager.GetCategory("BoardGameHotkeyCategory").GetHotKey("BoardGameRollDice"));
		_gauntletLayer = new GauntletLayer(base.ViewOrderPriority, "BoardGame");
		_gauntletMovie = _gauntletLayer.LoadMovie("BoardGame", _dataSource);
		_gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		_cameraHolder = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("camera_holder");
		CreateCamera();
		if (_cameraHolder == null)
		{
			_cameraHolder = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("camera_holder");
		}
		if (Camera == null)
		{
			CreateCamera();
		}
		_gauntletLayer.InputRestrictions.SetInputRestrictions();
		_missionMouseVisibilityState = ((MissionView)this).MissionScreen.SceneLayer.InputRestrictions.MouseVisibility;
		_missionInputRestrictions = ((MissionView)this).MissionScreen.SceneLayer.InputRestrictions.InputUsageMask;
		((MissionView)this).MissionScreen.SceneLayer.InputRestrictions.SetInputRestrictions(isMouseVisible: false);
		((MissionView)this).MissionScreen.SceneLayer.IsFocusLayer = true;
		((ScreenBase)(object)((MissionView)this).MissionScreen).AddLayer((ScreenLayer)_gauntletLayer);
		((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesStateAndDeactivateOthers(new string[2] { "SceneLayer", "BoardGame" }, isActive: true);
		ScreenManager.TrySetFocus(((MissionView)this).MissionScreen.SceneLayer);
		SetStaticCamera();
	}

	void IBoardGameHandler.Uninstall()
	{
		if (_dataSource != null)
		{
			_dataSource.OnFinalize();
			_dataSource = null;
		}
		_gauntletLayer.IsFocusLayer = false;
		ScreenManager.TryLoseFocus(_gauntletLayer);
		_gauntletLayer.InputRestrictions.ResetInputRestrictions();
		((MissionView)this).MissionScreen.SceneLayer.InputRestrictions.SetInputRestrictions(_missionMouseVisibilityState, _missionInputRestrictions);
		((ScreenBase)(object)((MissionView)this).MissionScreen).RemoveLayer((ScreenLayer)_gauntletLayer);
		_gauntletMovie = null;
		_gauntletLayer = null;
		Camera = null;
		_cameraHolder = null;
		((MissionView)this).MissionScreen.CustomCamera = null;
		((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesStateAndToggleOthers(new string[1] { "BoardGame" }, isActive: false);
		((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesState(new string[1] { "SceneLayer" }, isActive: true);
		_spriteCategory.Unload();
	}

	private bool IsHotkeyPressedInAnyLayer(string hotkeyID)
	{
		bool num = ((MissionView)this).MissionScreen.SceneLayer?.Input.IsHotKeyPressed(hotkeyID) ?? false;
		bool flag = _gauntletLayer?.Input.IsHotKeyPressed(hotkeyID) ?? false;
		return num || flag;
	}

	private bool IsHotkeyDownInAnyLayer(string hotkeyID)
	{
		bool num = ((MissionView)this).MissionScreen.SceneLayer?.Input.IsHotKeyDown(hotkeyID) ?? false;
		bool flag = _gauntletLayer?.Input.IsHotKeyDown(hotkeyID) ?? false;
		return num || flag;
	}

	private bool IsGameKeyReleasedInAnyLayer(string hotKeyID)
	{
		bool num = ((MissionView)this).MissionScreen.SceneLayer?.Input.IsHotKeyReleased(hotKeyID) ?? false;
		bool flag = _gauntletLayer?.Input.IsHotKeyReleased(hotKeyID) ?? false;
		return num || flag;
	}

	private void CreateCamera()
	{
		Camera = Camera.CreateCamera();
		if (_cameraHolder != null)
		{
			Camera.Entity = _cameraHolder;
		}
		Camera.SetFovVertical((float)Math.PI / 4f, 1.7777778f, 0.01f, 3000f);
	}

	private void SetStaticCamera()
	{
		if (_cameraHolder != null && Camera.Entity != null)
		{
			((MissionView)this).MissionScreen.CustomCamera = Camera;
		}
		else
		{
			Debug.FailedAssert("[DEBUG]Camera entities are null.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.GauntletUI\\Missions\\MissionGauntletBoardGameView.cs", "SetStaticCamera", 189);
		}
	}

	public override void OnMissionScreenTick(float dt)
	{
		MissionBoardGameLogic missionBoardGameHandler = _missionBoardGameHandler;
		if (missionBoardGameHandler == null || !missionBoardGameHandler.IsGameInProgress)
		{
			return;
		}
		MissionScreen missionScreen = ((MissionView)this).MissionScreen;
		if (missionScreen != null && missionScreen.IsPhotoModeEnabled)
		{
			return;
		}
		((MissionView)this).OnMissionScreenTick(dt);
		if (_gauntletLayer != null && _dataSource != null)
		{
			if (IsHotkeyPressedInAnyLayer("Exit"))
			{
				_dataSource.ExecuteForfeit();
			}
			else if (IsHotkeyPressedInAnyLayer("BoardGameRollDice") && _dataSource.IsGameUsingDice)
			{
				_dataSource.ExecuteRoll();
			}
		}
		if (_missionBoardGameHandler.Board != null)
		{
			Vec3 rayBegin = default(Vec3);
			Vec3 rayEnd = default(Vec3);
			((MissionView)this).MissionScreen.ScreenPointToWorldRay(((MissionView)this).Input.GetMousePositionRanged(), ref rayBegin, ref rayEnd);
			_missionBoardGameHandler.Board.SetUserRay(rayBegin, rayEnd);
		}
	}

	public override void OnMissionScreenFinalize()
	{
		if (_dataSource != null)
		{
			_dataSource.OnFinalize();
			_dataSource = null;
		}
		_gauntletLayer = null;
		_gauntletMovie = null;
		((MissionView)this).OnMissionScreenFinalize();
	}

	public override void OnPhotoModeActivated()
	{
		((MissionView)this).OnPhotoModeActivated();
		if (_gauntletLayer != null)
		{
			_gauntletLayer.UIContext.ContextAlpha = 0f;
		}
	}

	public override void OnPhotoModeDeactivated()
	{
		((MissionView)this).OnPhotoModeDeactivated();
		if (_gauntletLayer != null)
		{
			_gauntletLayer.UIContext.ContextAlpha = 1f;
		}
	}
}
