using System.ComponentModel;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;

namespace TaleWorlds.MountAndBlade.GauntletUI.AutoGenerated1;

public class OrderBar__TaleWorlds_MountAndBlade_ViewModelCollection_Order_MissionOrderVM_Dependency_7_ItemTemplate : OrderSiegeDeploymentItemButtonWidget
{
	private OrderSiegeDeploymentItemButtonWidget _widget;

	private OrderSiegeMachineItemButtonWidget _widget_0;

	private BrushWidget _widget_0_0;

	private ImageWidget _widget_1;

	private TextWidget _widget_2;

	private DeploymentSiegeMachineVM _datasource_Root;

	public OrderBar__TaleWorlds_MountAndBlade_ViewModelCollection_Order_MissionOrderVM_Dependency_7_ItemTemplate(UIContext context)
		: base(context)
	{
	}

	public void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new OrderSiegeMachineItemButtonWidget(base.Context);
		_widget.AddChild(_widget_0);
		_widget_0_0 = new BrushWidget(base.Context);
		_widget_0.AddChild(_widget_0_0);
		_widget_1 = new ImageWidget(base.Context);
		_widget.AddChild(_widget_1);
		_widget_2 = new TextWidget(base.Context);
		_widget.AddChild(_widget_2);
	}

	public void SetIds()
	{
		_widget_0.Id = "MachineItemWidget";
		_widget_0_0.Id = "Icon";
		_widget_1.Id = "TypeIcon";
		_widget_2.Id = "BreachedText";
	}

	public void SetAttributes()
	{
		base.DoNotPassEventsToChildren = true;
		base.WidthSizePolicy = SizePolicy.Fixed;
		base.HeightSizePolicy = SizePolicy.Fixed;
		base.SuggestedWidth = 60f;
		base.SuggestedHeight = 60f;
		base.Brush = base.Context.GetBrush("Order.Siege.Deployment.DeployPoint");
		base.BreachedTextWidget = _widget_2;
		base.ScreenWidget = _widget.FindChild(new BindingPath("..\\..\\.")) as OrderSiegeDeploymentScreenWidget;
		base.TypeIconWidget = _widget_1;
		base.UpdateChildrenStates = true;
		_widget_0.WidthSizePolicy = SizePolicy.Fixed;
		_widget_0.HeightSizePolicy = SizePolicy.Fixed;
		_widget_0.SuggestedWidth = 60f;
		_widget_0.SuggestedHeight = 60f;
		_widget_0.Brush = base.Context.GetBrush("Order.Siege.Deployment.MachineItem");
		_widget_0.IsDisabled = true;
		_widget_0.MachineIconWidget = _widget_0_0;
		_widget_0_0.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_0_0.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_0_0.Brush = base.Context.GetBrush("Order.Siege.Deployment.MachineIcon");
		_widget_1.WidthSizePolicy = SizePolicy.Fixed;
		_widget_1.HeightSizePolicy = SizePolicy.Fixed;
		_widget_1.SuggestedWidth = 45f;
		_widget_1.SuggestedHeight = 45f;
		_widget_1.HorizontalAlignment = HorizontalAlignment.Right;
		_widget_1.PositionXOffset = 20f;
		_widget_1.PositionYOffset = -20f;
		_widget_1.Brush = base.Context.GetBrush("Order.Siege.Deployment.TypeIcon");
		_widget_1.OverrideDefaultStateSwitchingEnabled = true;
		_widget_2.WidthSizePolicy = SizePolicy.CoverChildren;
		_widget_2.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_2.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_2.PositionYOffset = 60f;
		_widget_2.Brush = base.Context.GetBrush("Order.Siege.Breaced.Text");
		_widget_2.ClipContents = false;
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
			_widget.EventFire -= EventListenerOf_widget;
			_widget.PropertyChanged -= PropertyChangedListenerOf_widget;
			_widget.boolPropertyChanged -= boolPropertyChangedListenerOf_widget;
			_widget.floatPropertyChanged -= floatPropertyChangedListenerOf_widget;
			_widget.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget;
			_widget.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget;
			_widget.doublePropertyChanged -= doublePropertyChangedListenerOf_widget;
			_widget.intPropertyChanged -= intPropertyChangedListenerOf_widget;
			_widget.uintPropertyChanged -= uintPropertyChangedListenerOf_widget;
			_widget.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget;
			_widget_0.PropertyChanged -= PropertyChangedListenerOf_widget_0;
			_widget_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_0;
			_widget_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_0;
			_widget_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_0;
			_widget_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_0;
			_widget_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_0;
			_widget_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_0;
			_widget_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_0;
			_widget_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_0;
			_widget_2.PropertyChanged -= PropertyChangedListenerOf_widget_2;
			_widget_2.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_2;
			_widget_2.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_2;
			_widget_2.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_2;
			_widget_2.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_2;
			_widget_2.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_2;
			_widget_2.intPropertyChanged -= intPropertyChangedListenerOf_widget_2;
			_widget_2.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_2;
			_widget_2.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_2;
			_datasource_Root = null;
		}
	}

	public void SetDataSource(DeploymentSiegeMachineVM dataSource)
	{
		RefreshDataSource_datasource_Root(dataSource);
	}

	private void EventListenerOf_widget(Widget widget, string commandName, object[] args)
	{
		if (commandName == "Click")
		{
			_datasource_Root.ExecuteAction();
		}
		if (commandName == "HoverBegin")
		{
			_datasource_Root.ExecuteFocusBegin();
		}
		if (commandName == "HoverEnd")
		{
			_datasource_Root.ExecuteFocusEnd();
		}
	}

	private void PropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void intPropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget(string propertyName)
	{
		switch (propertyName)
		{
		case "IsPlayerGeneral":
			_datasource_Root.IsPlayerGeneral = _widget.IsPlayerGeneral;
			break;
		case "IsInFront":
			_datasource_Root.IsInFront = _widget.IsInFront;
			break;
		case "IsInsideWindow":
			_datasource_Root.IsInside = _widget.IsInsideWindow;
			break;
		case "IsSelected":
			_datasource_Root.IsSelected = _widget.IsSelected;
			break;
		case "PointType":
			_datasource_Root.Type = _widget.PointType;
			break;
		case "Position":
			_datasource_Root.Position = _widget.Position;
			break;
		}
	}

	private void PropertyChangedListenerOf_widget_0(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_0(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_0(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_0(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_0(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_0(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_0(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_0(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_0(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_0(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_0(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_0(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_0(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_0(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_0(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_0(string propertyName)
	{
		if (propertyName == "MachineClass")
		{
			_datasource_Root.MachineClass = _widget_0.MachineClass;
		}
	}

	private void PropertyChangedListenerOf_widget_2(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_2(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_2(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_2(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_2(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_2(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_2(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_2(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_2(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_2(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_2(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_2(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_2(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_2(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_2(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_2(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_2(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_2(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_2(string propertyName)
	{
		if (propertyName == "Text")
		{
			_datasource_Root.BreachedText = _widget_2.Text;
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
		switch (propertyName)
		{
		case "IsPlayerGeneral":
			_widget.IsPlayerGeneral = _datasource_Root.IsPlayerGeneral;
			break;
		case "IsInFront":
			_widget.IsInFront = _datasource_Root.IsInFront;
			break;
		case "IsInside":
			_widget.IsInsideWindow = _datasource_Root.IsInside;
			break;
		case "IsSelected":
			_widget.IsSelected = _datasource_Root.IsSelected;
			break;
		case "Type":
			_widget.PointType = _datasource_Root.Type;
			break;
		case "Position":
			_widget.Position = _datasource_Root.Position;
			break;
		case "MachineClass":
			_widget_0.MachineClass = _datasource_Root.MachineClass;
			break;
		case "BreachedText":
			_widget_2.Text = _datasource_Root.BreachedText;
			break;
		}
	}

	private void RefreshDataSource_datasource_Root(DeploymentSiegeMachineVM newDataSource)
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
			_widget.EventFire -= EventListenerOf_widget;
			_widget.PropertyChanged -= PropertyChangedListenerOf_widget;
			_widget.boolPropertyChanged -= boolPropertyChangedListenerOf_widget;
			_widget.floatPropertyChanged -= floatPropertyChangedListenerOf_widget;
			_widget.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget;
			_widget.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget;
			_widget.doublePropertyChanged -= doublePropertyChangedListenerOf_widget;
			_widget.intPropertyChanged -= intPropertyChangedListenerOf_widget;
			_widget.uintPropertyChanged -= uintPropertyChangedListenerOf_widget;
			_widget.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget;
			_widget_0.PropertyChanged -= PropertyChangedListenerOf_widget_0;
			_widget_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_0;
			_widget_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_0;
			_widget_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_0;
			_widget_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_0;
			_widget_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_0;
			_widget_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_0;
			_widget_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_0;
			_widget_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_0;
			_widget_2.PropertyChanged -= PropertyChangedListenerOf_widget_2;
			_widget_2.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_2;
			_widget_2.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_2;
			_widget_2.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_2;
			_widget_2.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_2;
			_widget_2.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_2;
			_widget_2.intPropertyChanged -= intPropertyChangedListenerOf_widget_2;
			_widget_2.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_2;
			_widget_2.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_2;
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
			_widget.IsPlayerGeneral = _datasource_Root.IsPlayerGeneral;
			_widget.IsInFront = _datasource_Root.IsInFront;
			_widget.IsInsideWindow = _datasource_Root.IsInside;
			_widget.IsSelected = _datasource_Root.IsSelected;
			_widget.PointType = _datasource_Root.Type;
			_widget.Position = _datasource_Root.Position;
			_widget.EventFire += EventListenerOf_widget;
			_widget.PropertyChanged += PropertyChangedListenerOf_widget;
			_widget.boolPropertyChanged += boolPropertyChangedListenerOf_widget;
			_widget.floatPropertyChanged += floatPropertyChangedListenerOf_widget;
			_widget.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget;
			_widget.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget;
			_widget.doublePropertyChanged += doublePropertyChangedListenerOf_widget;
			_widget.intPropertyChanged += intPropertyChangedListenerOf_widget;
			_widget.uintPropertyChanged += uintPropertyChangedListenerOf_widget;
			_widget.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget;
			_widget_0.MachineClass = _datasource_Root.MachineClass;
			_widget_0.PropertyChanged += PropertyChangedListenerOf_widget_0;
			_widget_0.boolPropertyChanged += boolPropertyChangedListenerOf_widget_0;
			_widget_0.floatPropertyChanged += floatPropertyChangedListenerOf_widget_0;
			_widget_0.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_0;
			_widget_0.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_0;
			_widget_0.doublePropertyChanged += doublePropertyChangedListenerOf_widget_0;
			_widget_0.intPropertyChanged += intPropertyChangedListenerOf_widget_0;
			_widget_0.uintPropertyChanged += uintPropertyChangedListenerOf_widget_0;
			_widget_0.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_0;
			_widget_2.Text = _datasource_Root.BreachedText;
			_widget_2.PropertyChanged += PropertyChangedListenerOf_widget_2;
			_widget_2.boolPropertyChanged += boolPropertyChangedListenerOf_widget_2;
			_widget_2.floatPropertyChanged += floatPropertyChangedListenerOf_widget_2;
			_widget_2.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_2;
			_widget_2.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_2;
			_widget_2.doublePropertyChanged += doublePropertyChangedListenerOf_widget_2;
			_widget_2.intPropertyChanged += intPropertyChangedListenerOf_widget_2;
			_widget_2.uintPropertyChanged += uintPropertyChangedListenerOf_widget_2;
			_widget_2.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_2;
		}
	}
}