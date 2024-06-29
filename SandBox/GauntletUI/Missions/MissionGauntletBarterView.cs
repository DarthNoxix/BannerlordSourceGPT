using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Barter;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Missions;

[OverrideView(typeof(BarterView))]
public class MissionGauntletBarterView : MissionView
{
	private GauntletLayer _gauntletLayer;

	private BarterVM _dataSource;

	private BarterManager _barter;

	private SpriteCategory _barterCategory;

	public override void AfterStart()
	{
		((MissionBehavior)this).AfterStart();
		_barter = Campaign.Current.BarterManager;
		BarterManager barter = _barter;
		barter.BarterBegin = (BarterManager.BarterBeginEventDelegate)Delegate.Combine(barter.BarterBegin, new BarterManager.BarterBeginEventDelegate(OnBarterBegin));
		BarterManager barter2 = _barter;
		barter2.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Combine(barter2.Closed, new BarterManager.BarterCloseEventDelegate(OnBarterClosed));
	}

	private void OnBarterBegin(BarterData args)
	{
		SpriteData spriteData = UIResourceManager.SpriteData;
		TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
		ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
		_barterCategory = spriteData.SpriteCategories["ui_barter"];
		_barterCategory.Load(resourceContext, uIResourceDepot);
		_dataSource = new BarterVM(args);
		_dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
		_dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		_dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		_gauntletLayer = new GauntletLayer(base.ViewOrderPriority, "Barter");
		_gauntletLayer.LoadMovie("BarterScreen", _dataSource);
		GameKeyContext category = HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory");
		_gauntletLayer.Input.RegisterHotKeyCategory(category);
		GameKeyContext category2 = HotKeyManager.GetCategory("GenericPanelGameKeyCategory");
		_gauntletLayer.Input.RegisterHotKeyCategory(category2);
		_gauntletLayer.IsFocusLayer = true;
		_gauntletLayer.InputRestrictions.SetInputRestrictions();
		ScreenManager.TrySetFocus(_gauntletLayer);
		((ScreenBase)(object)((MissionView)this).MissionScreen).AddLayer((ScreenLayer)_gauntletLayer);
		((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesStateAndDeactivateOthers(new string[2] { "SceneLayer", "Barter" }, isActive: true);
	}

	public override void OnMissionScreenTick(float dt)
	{
		((MissionView)this).OnMissionScreenTick(dt);
		BarterItemVM.IsFiveStackModifierActive = IsDownInGauntletLayer("FiveStackModifier");
		BarterItemVM.IsEntireStackModifierActive = IsDownInGauntletLayer("EntireStackModifier");
		if (IsReleasedInGauntletLayer("Confirm"))
		{
			if (!_dataSource.IsOfferDisabled)
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_dataSource.ExecuteOffer();
			}
		}
		else if (IsReleasedInGauntletLayer("Exit"))
		{
			UISoundsHelper.PlayUISound("event:/ui/default");
			_dataSource.ExecuteCancel();
		}
		else if (IsReleasedInGauntletLayer("Reset"))
		{
			UISoundsHelper.PlayUISound("event:/ui/default");
			_dataSource.ExecuteReset();
		}
	}

	private bool IsDownInGauntletLayer(string hotKeyID)
	{
		return _gauntletLayer?.Input.IsHotKeyDown(hotKeyID) ?? false;
	}

	private bool IsReleasedInGauntletLayer(string hotKeyID)
	{
		return _gauntletLayer?.Input.IsHotKeyReleased(hotKeyID) ?? false;
	}

	public override void OnMissionScreenFinalize()
	{
		((MissionView)this).OnMissionScreenFinalize();
		BarterManager barter = _barter;
		barter.BarterBegin = (BarterManager.BarterBeginEventDelegate)Delegate.Remove(barter.BarterBegin, new BarterManager.BarterBeginEventDelegate(OnBarterBegin));
		BarterManager barter2 = _barter;
		barter2.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Remove(barter2.Closed, new BarterManager.BarterCloseEventDelegate(OnBarterClosed));
		_gauntletLayer = null;
		_dataSource?.OnFinalize();
		_dataSource = null;
	}

	private void OnBarterClosed()
	{
		((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesState(new string[1] { "Barter" }, isActive: false);
		((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesState(new string[1] { "Conversation" }, isActive: true);
		((ScreenBase)(object)((MissionView)this).MissionScreen).SetLayerCategoriesState(new string[1] { "SceneLayer" }, isActive: true);
		BarterItemVM.IsFiveStackModifierActive = false;
		BarterItemVM.IsEntireStackModifierActive = false;
		_barterCategory.Unload();
		_gauntletLayer.IsFocusLayer = false;
		ScreenManager.TryLoseFocus(_gauntletLayer);
		_gauntletLayer.InputRestrictions.ResetInputRestrictions();
		((ScreenBase)(object)((MissionView)this).MissionScreen).RemoveLayer((ScreenLayer)_gauntletLayer);
		_gauntletLayer = null;
		_dataSource = null;
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

	public override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
	{
		return true;
	}
}
