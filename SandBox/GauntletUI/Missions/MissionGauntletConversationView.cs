using SandBox.Conversation.MissionLogics;
using SandBox.View.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.ViewModelCollection.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Missions;

[OverrideView(typeof(MissionConversationView))]
public class MissionGauntletConversationView : MissionView, IConversationStateHandler
{
	private MissionConversationVM _dataSource;

	private GauntletLayer _gauntletLayer;

	private ConversationManager _conversationManager;

	private MissionConversationCameraView _conversationCameraView;

	private MissionGauntletEscapeMenuBase _escapeView;

	private SpriteCategory _conversationCategory;

	public MissionConversationLogic ConversationHandler { get; private set; }

	public MissionGauntletConversationView()
	{
		base.ViewOrderPriority = 49;
	}

	public override void OnMissionScreenTick(float dt)
	{
		((MissionView)this).OnMissionScreenTick(dt);
		MissionGauntletEscapeMenuBase escapeView = _escapeView;
		if ((escapeView != null && ((MissionEscapeMenuView)escapeView).IsActive) || _gauntletLayer == null)
		{
			return;
		}
		MissionConversationVM dataSource = _dataSource;
		if (dataSource != null && dataSource.AnswerList.Count <= 0 && ((MissionBehavior)this).Mission.Mode != MissionMode.Barter)
		{
			if (IsReleasedInSceneLayer("ContinueClick", isDownAndReleased: false))
			{
				goto IL_008c;
			}
			if (IsReleasedInGauntletLayer("ContinueKey", isDownAndReleased: true))
			{
				MissionConversationVM dataSource2 = _dataSource;
				if (dataSource2 != null && !dataSource2.SelectedAnOptionOrLinkThisFrame)
				{
					goto IL_008c;
				}
			}
		}
		goto IL_009d;
		IL_008c:
		_dataSource?.ExecuteContinue();
		goto IL_009d;
		IL_009d:
		if (_gauntletLayer != null && IsGameKeyReleasedInAnyLayer("ToggleEscapeMenu", isDownAndReleased: true))
		{
			((MissionView)this).MissionScreen.OnEscape();
		}
		if (_dataSource != null)
		{
			_dataSource.SelectedAnOptionOrLinkThisFrame = false;
		}
		if (((MissionView)this).MissionScreen.SceneLayer.Input.IsKeyDown(InputKey.RightMouseButton))
		{
			_gauntletLayer?.InputRestrictions.SetMouseVisibility(isVisible: false);
		}
		else
		{
			_gauntletLayer?.InputRestrictions.SetInputRestrictions();
		}
	}

	public override void OnMissionScreenFinalize()
	{
		Campaign.Current.ConversationManager.Handler = null;
		if (_dataSource != null)
		{
			_dataSource?.OnFinalize();
			_dataSource = null;
		}
		_gauntletLayer = null;
		ConversationHandler = null;
		((MissionView)this).OnMissionScreenFinalize();
	}

	public override void EarlyStart()
	{
		((MissionBehavior)this).EarlyStart();
		ConversationHandler = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionConversationLogic>();
		_conversationCameraView = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionConversationCameraView>();
		Campaign.Current.ConversationManager.Handler = this;
	}

	public override void OnMissionScreenActivate()
	{
		((MissionView)this).OnMissionScreenActivate();
		if (_dataSource != null)
		{
			((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesStateAndDeactivateOthers(new string[2] { "Conversation", "SceneLayer" }, isActive: true);
			ScreenManager.TrySetFocus(_gauntletLayer);
		}
	}

	void IConversationStateHandler.OnConversationInstall()
	{
		((MissionView)this).MissionScreen.SetConversationActive(true);
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_conversationCategory = spriteData.SpriteCategories["ui_conversation"];
		_conversationCategory.Load(resourceContext, uIResourceDepot);
		_dataSource = new MissionConversationVM(GetContinueKeyText);
		_gauntletLayer = new GauntletLayer(base.ViewOrderPriority, "Conversation");
		_gauntletLayer.LoadMovie("SPConversation", _dataSource);
		GameKeyContext category = HotKeyManager.GetCategory("ConversationHotKeyCategory");
		_gauntletLayer.Input.RegisterHotKeyCategory(category);
		if (!((MissionView)this).MissionScreen.SceneLayer.Input.IsCategoryRegistered(category))
		{
			((MissionView)this).MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(category);
		}
		_gauntletLayer.IsFocusLayer = true;
		_gauntletLayer.InputRestrictions.SetInputRestrictions();
		_escapeView = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionGauntletEscapeMenuBase>();
		((ScreenBase)(object)((MissionView)this).MissionScreen).AddLayer((ScreenLayer)_gauntletLayer);
		((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesStateAndDeactivateOthers(new string[2] { "Conversation", "SceneLayer" }, isActive: true);
		ScreenManager.TrySetFocus(_gauntletLayer);
		_conversationManager = Campaign.Current.ConversationManager;
		InformationManager.ClearAllMessages();
	}

	public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
	{
		((MissionBehavior)this).OnMissionModeChange(oldMissionMode, atStart);
		if (oldMissionMode == MissionMode.Barter && ((MissionBehavior)this).Mission.Mode == MissionMode.Conversation)
		{
			ScreenManager.TrySetFocus(_gauntletLayer);
		}
	}

	void IConversationStateHandler.OnConversationUninstall()
	{
		((MissionView)this).MissionScreen.SetConversationActive(false);
		if (_dataSource != null)
		{
			_dataSource?.OnFinalize();
			_dataSource = null;
		}
		_conversationCategory.Unload();
		_gauntletLayer.IsFocusLayer = false;
		ScreenManager.TryLoseFocus(_gauntletLayer);
		_gauntletLayer.InputRestrictions.ResetInputRestrictions();
		((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesStateAndToggleOthers(new string[1] { "Conversation" }, isActive: false);
		((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesState(new string[1] { "SceneLayer" }, isActive: true);
		((ScreenBase)(object)((MissionView)this).MissionScreen).RemoveLayer((ScreenLayer)_gauntletLayer);
		_gauntletLayer = null;
		_escapeView = null;
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

	void IConversationStateHandler.OnConversationActivate()
	{
		((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesStateAndDeactivateOthers(new string[2] { "Conversation", "SceneLayer" }, isActive: true);
	}

	void IConversationStateHandler.OnConversationDeactivate()
	{
		MBInformationManager.HideInformations();
	}

	void IConversationStateHandler.OnConversationContinue()
	{
		_dataSource.OnConversationContinue();
	}

	void IConversationStateHandler.ExecuteConversationContinue()
	{
		_dataSource.ExecuteContinue();
	}

	private bool IsGameKeyReleasedInAnyLayer(string hotKeyID, bool isDownAndReleased)
	{
		bool num = IsReleasedInSceneLayer(hotKeyID, isDownAndReleased);
		bool flag = IsReleasedInGauntletLayer(hotKeyID, isDownAndReleased);
		return num || flag;
	}

	private bool IsReleasedInSceneLayer(string hotKeyID, bool isDownAndReleased)
	{
		if (isDownAndReleased)
		{
			return ((MissionView)this).MissionScreen.SceneLayer?.Input.IsHotKeyDownAndReleased(hotKeyID) ?? false;
		}
		return ((MissionView)this).MissionScreen.SceneLayer?.Input.IsHotKeyReleased(hotKeyID) ?? false;
	}

	private bool IsReleasedInGauntletLayer(string hotKeyID, bool isDownAndReleased)
	{
		if (isDownAndReleased)
		{
			return _gauntletLayer?.Input.IsHotKeyDownAndReleased(hotKeyID) ?? false;
		}
		return _gauntletLayer?.Input.IsHotKeyReleased(hotKeyID) ?? false;
	}
}
