using System.ComponentModel;
using TaleWorlds.CampaignSystem.ViewModelCollection.Barter;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace SandBox.GauntletUI.AutoGenerated1;

public class BarterScreen__TaleWorlds_CampaignSystem_ViewModelCollection_Barter_BarterVM_Dependency_7_ItemTemplate : BarterScreen__TaleWorlds_CampaignSystem_ViewModelCollection_Barter_BarterVM_Dependency_28_BarterOfferItemTuple__InheritedPrefab
{
	private BarterScreen__TaleWorlds_CampaignSystem_ViewModelCollection_Barter_BarterVM_Dependency_28_BarterOfferItemTuple__InheritedPrefab _widget;

	private BarterItemVM _datasource_Root;

	public BarterScreen__TaleWorlds_CampaignSystem_ViewModelCollection_Barter_BarterVM_Dependency_7_ItemTemplate(UIContext context)
		: base(context)
	{
	}

	private VisualDefinition CreateVisualDefinitionLeftMenu()
	{
		VisualDefinition visualDefinition = new VisualDefinition("LeftMenu", 0.2f, 0f, easeIn: false);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionXOffset = 0f
		});
		return visualDefinition;
	}

	private VisualDefinition CreateVisualDefinitionRightMenu()
	{
		VisualDefinition visualDefinition = new VisualDefinition("RightMenu", 0.2f, 0f, easeIn: false);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionXOffset = 0f
		});
		return visualDefinition;
	}

	private VisualDefinition CreateVisualDefinitionTopMenu()
	{
		VisualDefinition visualDefinition = new VisualDefinition("TopMenu", 0.2f, 0f, easeIn: false);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionYOffset = 0f
		});
		return visualDefinition;
	}

	private VisualDefinition CreateVisualDefinitionBottomMenu()
	{
		VisualDefinition visualDefinition = new VisualDefinition("BottomMenu", 0.2f, 0f, easeIn: false);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionYOffset = 19f
		});
		return visualDefinition;
	}

	public override void CreateWidgets()
	{
		base.CreateWidgets();
		_widget = this;
	}

	public override void SetIds()
	{
		base.SetIds();
	}

	public override void SetAttributes()
	{
		base.SetAttributes();
	}

	public override void DestroyDataSource()
	{
		base.DestroyDataSource();
		if (_datasource_Root != null)
		{
			_datasource_Root.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
			_datasource_Root = null;
		}
	}

	public override void SetDataSource(BarterItemVM dataSource)
	{
		base.SetDataSource(dataSource);
		RefreshDataSource_datasource_Root(dataSource);
	}

	private void ViewModelPropertyChangedListenerOf_datasource_Root(object sender, PropertyChangedEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithValueListenerOf_datasource_Root(object sender, PropertyChangedWithValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root(object sender, PropertyChangedWithBoolValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root(object sender, PropertyChangedWithIntValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root(object sender, PropertyChangedWithFloatValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root(object sender, PropertyChangedWithUIntValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root(object sender, PropertyChangedWithColorValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root(object sender, PropertyChangedWithDoubleValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root(object sender, PropertyChangedWithVec2ValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void HandleViewModelPropertyChangeOf_datasource_Root(string propertyName)
	{
	}

	private void RefreshDataSource_datasource_Root(BarterItemVM newDataSource)
	{
		if (_datasource_Root != null)
		{
			_datasource_Root.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
			_datasource_Root = null;
		}
		_datasource_Root = newDataSource;
		if (_datasource_Root != null)
		{
			_datasource_Root.PropertyChanged += ViewModelPropertyChangedListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithValue += ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithBoolValue += ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithIntValue += ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithFloatValue += ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithUIntValue += ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithColorValue += ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithDoubleValue += ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithVec2Value += ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
		}
	}
}