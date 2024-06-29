using System.Linq;
using SandBox.View.Map;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Menu;

[OverrideView(typeof(MenuTownManagementView))]
public class GauntletMenuTownManagementView : MenuView
{
	private GauntletLayer _layerAsGauntletLayer;

	private TownManagementVM _dataSource;

	protected override void OnInitialize()
	{
		base.OnInitialize();
		_dataSource = new TownManagementVM();
		base.Layer = new GauntletLayer(206)
		{
			Name = "TownManagementLayer"
		};
		_layerAsGauntletLayer = base.Layer as GauntletLayer;
		base.Layer.InputRestrictions.SetInputRestrictions();
		base.MenuViewContext.AddLayer(base.Layer);
		if (!base.Layer.Input.IsCategoryRegistered(HotKeyManager.GetCategory("GenericPanelGameKeyCategory")))
		{
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		}
		_dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		_layerAsGauntletLayer.LoadMovie("TownManagement", _dataSource);
		base.Layer.IsFocusLayer = true;
		ScreenManager.TrySetFocus(base.Layer);
		_dataSource.Show = true;
		if (ScreenManager.TopScreen is MapScreen mapScreen)
		{
			mapScreen.SetIsInTownManagement(isInTownManagement: true);
		}
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		base.MenuViewContext.RemoveLayer(base.Layer);
		base.Layer.IsFocusLayer = false;
		ScreenManager.TryLoseFocus(base.Layer);
		if (ScreenManager.TopScreen is MapScreen mapScreen)
		{
			mapScreen.SetIsInTownManagement(isInTownManagement: false);
		}
		_dataSource.OnFinalize();
		_dataSource = null;
		_layerAsGauntletLayer = null;
		base.Layer = null;
		base.OnFinalize();
	}

	protected override void OnFrameTick(float dt)
	{
		base.OnFrameTick(dt);
		if (base.Layer.Input.IsHotKeyReleased("Confirm"))
		{
			UISoundsHelper.PlayUISound("event:/ui/default");
			if (_dataSource.ReserveControl.IsEnabled)
			{
				_dataSource.ReserveControl.ExecuteUpdateReserve();
			}
			else
			{
				_dataSource.ExecuteDone();
			}
		}
		else if (base.Layer.Input.IsHotKeyReleased("Exit"))
		{
			if (_dataSource.IsSelectingGovernor)
			{
				_dataSource.IsSelectingGovernor = false;
			}
			else if (_dataSource.ReserveControl.IsEnabled)
			{
				_dataSource.ReserveControl.IsEnabled = false;
			}
			else
			{
				SettlementBuildingProjectVM settlementBuildingProjectVM = _dataSource.ProjectSelection.AvailableProjects.FirstOrDefault((SettlementBuildingProjectVM x) => x.IsSelected);
				if (settlementBuildingProjectVM != null)
				{
					settlementBuildingProjectVM.IsSelected = false;
				}
				else
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					_dataSource.ExecuteDone();
				}
			}
		}
		if (!_dataSource.Show)
		{
			base.MenuViewContext.CloseTownManagement();
		}
	}
}
