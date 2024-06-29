using System.ComponentModel;
using System.Numerics;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper;

namespace SandBox.GauntletUI.AutoGenerated1;

public class CharacterDeveloper__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterDeveloper_CharacterDeveloperVM_Dependency_8_ItemTemplate : CharacterDeveloperPerkSelectionItemButtonWidget
{
	private CharacterDeveloperPerkSelectionItemButtonWidget _widget;

	private BrushWidget _widget_0;

	private ListPanel _widget_1;

	private TextWidget _widget_1_0;

	private TextWidget _widget_1_1;

	private TextWidget _widget_1_2;

	private TextWidget _widget_1_3;

	private PerkSelectionItemVM _datasource_Root;

	public CharacterDeveloper__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterDeveloper_CharacterDeveloperVM_Dependency_8_ItemTemplate(UIContext context)
		: base(context)
	{
	}

	private VisualDefinition CreateVisualDefinitionBottomMenu()
	{
		VisualDefinition visualDefinition = new VisualDefinition("BottomMenu", 0.45f, 0f, easeIn: true);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionYOffset = 6f
		});
		return visualDefinition;
	}

	private VisualDefinition CreateVisualDefinitionTopPanel()
	{
		VisualDefinition visualDefinition = new VisualDefinition("TopPanel", 0.45f, 0f, easeIn: true);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionYOffset = -6f
		});
		return visualDefinition;
	}

	private VisualDefinition CreateVisualDefinitionTopPanelExtension()
	{
		VisualDefinition visualDefinition = new VisualDefinition("TopPanelExtension", 0.3f, 0.7f, easeIn: true);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionYOffset = 0f
		});
		return visualDefinition;
	}

	private VisualDefinition CreateVisualDefinitionLeftPanel()
	{
		VisualDefinition visualDefinition = new VisualDefinition("LeftPanel", 0.4f, 0f, easeIn: true);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionXOffset = 0f
		});
		return visualDefinition;
	}

	private VisualDefinition CreateVisualDefinitionRightPanel()
	{
		VisualDefinition visualDefinition = new VisualDefinition("RightPanel", 0.4f, 0f, easeIn: true);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionXOffset = 0f
		});
		return visualDefinition;
	}

	public void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new BrushWidget(base.Context);
		_widget.AddChild(_widget_0);
		_widget_1 = new ListPanel(base.Context);
		_widget.AddChild(_widget_1);
		_widget_1_0 = new TextWidget(base.Context);
		_widget_1.AddChild(_widget_1_0);
		_widget_1_1 = new TextWidget(base.Context);
		_widget_1.AddChild(_widget_1_1);
		_widget_1_2 = new TextWidget(base.Context);
		_widget_1.AddChild(_widget_1_2);
		_widget_1_3 = new TextWidget(base.Context);
		_widget_1.AddChild(_widget_1_3);
	}

	public void SetIds()
	{
		_widget_0.Id = "PerkSelectionIndicatorWidget";
	}

	public void SetAttributes()
	{
		base.WidthSizePolicy = SizePolicy.Fixed;
		base.HeightSizePolicy = SizePolicy.Fixed;
		base.SuggestedWidth = 250f;
		base.SuggestedHeight = 340f;
		base.Brush = base.Context.GetBrush("CharacterDeveloper.PerkSelection.Button");
		base.MarginTop = 0f;
		base.MarginBottom = 0f;
		base.DoNotPassEventsToChildren = true;
		base.UpdateChildrenStates = true;
		base.PerkSelectionIndicatorWidget = _widget_0;
		_widget_0.HeightSizePolicy = SizePolicy.Fixed;
		_widget_0.SuggestedWidth = 65f;
		_widget_0.SuggestedHeight = 72f;
		_widget_0.HorizontalAlignment = HorizontalAlignment.Left;
		_widget_0.VerticalAlignment = VerticalAlignment.Bottom;
		_widget_0.PositionXOffset = -81f;
		_widget_0.MarginBottom = 3f;
		_widget_0.Brush = base.Context.GetBrush("CharacterDeveloper.PerkSelection.Indicator");
		_widget_0.MarginTop = 5f;
		_widget_1.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_1.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_1.StackLayout.LayoutMethod = LayoutMethod.VerticalBottomToTop;
		_widget_1.MarginTop = 5f;
		_widget_1.MarginBottom = 5f;
		_widget_1.MarginRight = 10f;
		_widget_1.MarginLeft = 10f;
		_widget_1_0.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_1_0.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_1_0.Brush = base.Context.GetBrush("CharacterDeveloper.PerkSelection.Pick.Text");
		_widget_1_0.MarginTop = 15f;
		_widget_1_1.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_1_1.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_1_1.Brush = base.Context.GetBrush("CharacterDeveloper.PerkSelection.PerkName.Text");
		_widget_1_1.MarginTop = 5f;
		_widget_1_2.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_1_2.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_1_2.Brush = base.Context.GetBrush("CharacterDeveloper.PerkSelection.Description.Text");
		_widget_1_2.MarginTop = 5f;
		_widget_1_3.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_1_3.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_1_3.Brush = base.Context.GetBrush("CharacterDeveloper.PerkSelection.Description.Text");
		_widget_1_3.MarginTop = 5f;
		_widget_1_3.MarginBottom = 15f;
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
			_widget_1_0.PropertyChanged -= PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_0;
			_widget_1_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_0;
			_widget_1_1.PropertyChanged -= PropertyChangedListenerOf_widget_1_1;
			_widget_1_1.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_1;
			_widget_1_1.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_1;
			_widget_1_1.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_1;
			_widget_1_1.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_1;
			_widget_1_2.PropertyChanged -= PropertyChangedListenerOf_widget_1_2;
			_widget_1_2.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_2;
			_widget_1_2.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_2;
			_widget_1_2.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_2;
			_widget_1_2.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_2;
			_widget_1_3.PropertyChanged -= PropertyChangedListenerOf_widget_1_3;
			_widget_1_3.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_3;
			_widget_1_3.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_3;
			_widget_1_3.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_3;
			_widget_1_3.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_3;
			_datasource_Root = null;
		}
	}

	public void SetDataSource(PerkSelectionItemVM dataSource)
	{
		RefreshDataSource_datasource_Root(dataSource);
	}

	private void EventListenerOf_widget(Widget widget, string commandName, object[] args)
	{
		if (commandName == "Click")
		{
			_datasource_Root.ExecuteSelection();
		}
	}

	private void PropertyChangedListenerOf_widget_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_1_0(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_1_0(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_1_0(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_0(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_0(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_1_0(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_1_0(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_1_0(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_1_0(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_1_0(string propertyName)
	{
		if (propertyName == "Text")
		{
			_datasource_Root.PickText = _widget_1_0.Text;
		}
	}

	private void PropertyChangedListenerOf_widget_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_1_1(string propertyName)
	{
		if (propertyName == "Text")
		{
			_datasource_Root.PerkName = _widget_1_1.Text;
		}
	}

	private void PropertyChangedListenerOf_widget_1_2(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_1_2(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_1_2(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_1_2(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_1_2(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_1_2(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_1_2(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_2(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_1_2(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_2(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_1_2(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_1_2(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_1_2(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_1_2(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_1_2(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_1_2(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_1_2(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_1_2(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_1_2(string propertyName)
	{
		if (propertyName == "Text")
		{
			_datasource_Root.PerkDescription = _widget_1_2.Text;
		}
	}

	private void PropertyChangedListenerOf_widget_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_1_3(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_1_3(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_1_3(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_3(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_3(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_1_3(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_1_3(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_1_3(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_1_3(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_1_3(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_1_3(string propertyName)
	{
		if (propertyName == "Text")
		{
			_datasource_Root.PerkRole = _widget_1_3.Text;
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
		case "PickText":
			_widget_1_0.Text = _datasource_Root.PickText;
			break;
		case "PerkName":
			_widget_1_1.Text = _datasource_Root.PerkName;
			break;
		case "PerkDescription":
			_widget_1_2.Text = _datasource_Root.PerkDescription;
			break;
		case "PerkRole":
			_widget_1_3.Text = _datasource_Root.PerkRole;
			break;
		}
	}

	private void RefreshDataSource_datasource_Root(PerkSelectionItemVM newDataSource)
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
			_widget_1_0.PropertyChanged -= PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_0;
			_widget_1_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_0;
			_widget_1_1.PropertyChanged -= PropertyChangedListenerOf_widget_1_1;
			_widget_1_1.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_1;
			_widget_1_1.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_1;
			_widget_1_1.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_1;
			_widget_1_1.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_1;
			_widget_1_2.PropertyChanged -= PropertyChangedListenerOf_widget_1_2;
			_widget_1_2.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_2;
			_widget_1_2.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_2;
			_widget_1_2.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_2;
			_widget_1_2.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_2;
			_widget_1_3.PropertyChanged -= PropertyChangedListenerOf_widget_1_3;
			_widget_1_3.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_3;
			_widget_1_3.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_3;
			_widget_1_3.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_3;
			_widget_1_3.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_3;
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
			_widget.EventFire += EventListenerOf_widget;
			_widget_1_0.Text = _datasource_Root.PickText;
			_widget_1_0.PropertyChanged += PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.boolPropertyChanged += boolPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.floatPropertyChanged += floatPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.doublePropertyChanged += doublePropertyChangedListenerOf_widget_1_0;
			_widget_1_0.intPropertyChanged += intPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.uintPropertyChanged += uintPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_1_0;
			_widget_1_1.Text = _datasource_Root.PerkName;
			_widget_1_1.PropertyChanged += PropertyChangedListenerOf_widget_1_1;
			_widget_1_1.boolPropertyChanged += boolPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.floatPropertyChanged += floatPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_1_1;
			_widget_1_1.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_1_1;
			_widget_1_1.doublePropertyChanged += doublePropertyChangedListenerOf_widget_1_1;
			_widget_1_1.intPropertyChanged += intPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.uintPropertyChanged += uintPropertyChangedListenerOf_widget_1_1;
			_widget_1_1.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_1_1;
			_widget_1_2.Text = _datasource_Root.PerkDescription;
			_widget_1_2.PropertyChanged += PropertyChangedListenerOf_widget_1_2;
			_widget_1_2.boolPropertyChanged += boolPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.floatPropertyChanged += floatPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_1_2;
			_widget_1_2.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_1_2;
			_widget_1_2.doublePropertyChanged += doublePropertyChangedListenerOf_widget_1_2;
			_widget_1_2.intPropertyChanged += intPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.uintPropertyChanged += uintPropertyChangedListenerOf_widget_1_2;
			_widget_1_2.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_1_2;
			_widget_1_3.Text = _datasource_Root.PerkRole;
			_widget_1_3.PropertyChanged += PropertyChangedListenerOf_widget_1_3;
			_widget_1_3.boolPropertyChanged += boolPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.floatPropertyChanged += floatPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_1_3;
			_widget_1_3.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_1_3;
			_widget_1_3.doublePropertyChanged += doublePropertyChangedListenerOf_widget_1_3;
			_widget_1_3.intPropertyChanged += intPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.uintPropertyChanged += uintPropertyChangedListenerOf_widget_1_3;
			_widget_1_3.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_1_3;
		}
	}
}