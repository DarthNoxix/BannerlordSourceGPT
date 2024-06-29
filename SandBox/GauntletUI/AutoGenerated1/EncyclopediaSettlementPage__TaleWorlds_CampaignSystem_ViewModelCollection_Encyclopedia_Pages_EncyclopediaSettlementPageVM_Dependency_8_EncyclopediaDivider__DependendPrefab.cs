using System.ComponentModel;
using System.Numerics;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia;

namespace SandBox.GauntletUI.AutoGenerated1;

public class EncyclopediaSettlementPage__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaSettlementPageVM_Dependency_8_EncyclopediaDivider__DependendPrefab : EncyclopediaDividerButtonWidget
{
	private EncyclopediaDividerButtonWidget _widget;

	private NavigationScopeTargeter _widget_0;

	private NavigationAutoScrollWidget _widget_1;

	private ListPanel _widget_2;

	private BrushWidget _widget_2_0;

	private TextWidget _widget_2_1;

	private ImageWidget _widget_2_2;

	private EncyclopediaSettlementPageVM _datasource_Root;

	public EncyclopediaSettlementPage__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaSettlementPageVM_Dependency_8_EncyclopediaDivider__DependendPrefab(UIContext context)
		: base(context)
	{
	}

	public void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new NavigationScopeTargeter(base.Context);
		_widget.AddChild(_widget_0);
		_widget_1 = new NavigationAutoScrollWidget(base.Context);
		_widget.AddChild(_widget_1);
		_widget_2 = new ListPanel(base.Context);
		_widget.AddChild(_widget_2);
		_widget_2_0 = new BrushWidget(base.Context);
		_widget_2.AddChild(_widget_2_0);
		_widget_2_1 = new TextWidget(base.Context);
		_widget_2.AddChild(_widget_2_1);
		_widget_2_2 = new ImageWidget(base.Context);
		_widget_2.AddChild(_widget_2_2);
	}

	public void SetIds()
	{
		_widget_2.Id = "PlacementListPanel";
		_widget_2_0.Id = "CollapseIndicator";
		_widget_2_1.Id = "Text";
	}

	public void SetAttributes()
	{
		base.WidthSizePolicy = SizePolicy.StretchToParent;
		base.HeightSizePolicy = SizePolicy.CoverChildren;
		base.CollapseIndicator = _widget_2_0;
		base.ItemListWidget = _widget.FindChild(new BindingPath("..\\SettlementsGrid"));
		base.UpdateChildrenStates = true;
		base.DoNotPassEventsToChildren = true;
		base.ExtendCursorAreaLeft = 5f;
		base.ExtendCursorAreaRight = 5f;
		base.ExtendCursorAreaTop = 5f;
		base.ExtendCursorAreaBottom = 5f;
		base.GamepadNavigationIndex = 0;
		_widget_0.ScopeID = "EncyclopediaDividerScope";
		_widget_0.ScopeParent = _widget;
		_widget_0.NavigateFromScopeEdges = true;
		_widget_0.UseDiscoveryAreaAsScopeEdges = true;
		_widget_1.TrackedWidget = _widget;
		_widget_1.ScrollYOffset = 35;
		_widget_2.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_2.UpdateChildrenStates = true;
		_widget_2_0.WidthSizePolicy = SizePolicy.Fixed;
		_widget_2_0.HeightSizePolicy = SizePolicy.Fixed;
		_widget_2_0.SuggestedHeight = 19f;
		_widget_2_0.SuggestedWidth = 19f;
		_widget_2_0.VerticalAlignment = VerticalAlignment.Center;
		_widget_2_0.PositionYOffset = -3f;
		_widget_2_0.Brush = base.Context.GetBrush("SPOptions.GameKeysgroup.ExpandIndicator");
		_widget_2_1.WidthSizePolicy = SizePolicy.CoverChildren;
		_widget_2_1.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_2_1.Brush = base.Context.GetBrush("Encyclopedia.SubPage.Header.Text");
		_widget_2_1.MarginLeft = 10f;
		_widget_2_2.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2_2.HeightSizePolicy = SizePolicy.Fixed;
		_widget_2_2.SuggestedHeight = 2f;
		_widget_2_2.VerticalAlignment = VerticalAlignment.Center;
		_widget_2_2.HorizontalAlignment = HorizontalAlignment.Left;
		_widget_2_2.MarginLeft = 15f;
		_widget_2_2.Brush = base.Context.GetBrush("SPOptions.CollapserLine");
	}

	public void DestroyDataSource()
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
			_widget_2_1.PropertyChanged -= PropertyChangedListenerOf_widget_2_1;
			_widget_2_1.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_2_1;
			_widget_2_1.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_2_1;
			_widget_2_1.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_2_1;
			_widget_2_1.intPropertyChanged -= intPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_2_1;
			_datasource_Root = null;
		}
	}

	public void SetDataSource(EncyclopediaSettlementPageVM dataSource)
	{
		RefreshDataSource_datasource_Root(dataSource);
	}

	private void PropertyChangedListenerOf_widget_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_2_1(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_2_1(string propertyName)
	{
		if (propertyName == "Text")
		{
			_datasource_Root.SettlementsText = _widget_2_1.Text;
		}
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
		if (propertyName == "SettlementsText")
		{
			_widget_2_1.Text = _datasource_Root.SettlementsText;
		}
	}

	private void RefreshDataSource_datasource_Root(EncyclopediaSettlementPageVM newDataSource)
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
			_widget_2_1.PropertyChanged -= PropertyChangedListenerOf_widget_2_1;
			_widget_2_1.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_2_1;
			_widget_2_1.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_2_1;
			_widget_2_1.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_2_1;
			_widget_2_1.intPropertyChanged -= intPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_2_1;
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
			_widget_2_1.Text = _datasource_Root.SettlementsText;
			_widget_2_1.PropertyChanged += PropertyChangedListenerOf_widget_2_1;
			_widget_2_1.boolPropertyChanged += boolPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.floatPropertyChanged += floatPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_2_1;
			_widget_2_1.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_2_1;
			_widget_2_1.doublePropertyChanged += doublePropertyChangedListenerOf_widget_2_1;
			_widget_2_1.intPropertyChanged += intPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.uintPropertyChanged += uintPropertyChangedListenerOf_widget_2_1;
			_widget_2_1.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_2_1;
		}
	}
}
