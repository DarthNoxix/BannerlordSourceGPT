using System.ComponentModel;
using System.Numerics;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting;

namespace SandBox.GauntletUI.AutoGenerated1;

public class Crafting__TaleWorlds_CampaignSystem_ViewModelCollection_WeaponCrafting_CraftingVM_Dependency_31_ItemTemplate : ButtonWidget
{
	private ButtonWidget _widget;

	private Widget _widget_0;

	private Widget _widget_1;

	private CraftingCardHighlightBrushWidget _widget_1_0;

	private ListPanel _widget_1_1;

	private TextWidget _widget_1_1_0;

	private CraftingWeaponTypeIconWidget _widget_1_1_1;

	private ListPanel _widget_1_1_2;

	private TextWidget _widget_1_1_2_0;

	private TextWidget _widget_1_1_2_1;

	private WeaponClassVM _datasource_Root;

	public Crafting__TaleWorlds_CampaignSystem_ViewModelCollection_WeaponCrafting_CraftingVM_Dependency_31_ItemTemplate(UIContext context)
		: base(context)
	{
	}

	public void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new Widget(base.Context);
		_widget.AddChild(_widget_0);
		_widget_1 = new Widget(base.Context);
		_widget.AddChild(_widget_1);
		_widget_1_0 = new CraftingCardHighlightBrushWidget(base.Context);
		_widget_1.AddChild(_widget_1_0);
		_widget_1_1 = new ListPanel(base.Context);
		_widget_1.AddChild(_widget_1_1);
		_widget_1_1_0 = new TextWidget(base.Context);
		_widget_1_1.AddChild(_widget_1_1_0);
		_widget_1_1_1 = new CraftingWeaponTypeIconWidget(base.Context);
		_widget_1_1.AddChild(_widget_1_1_1);
		_widget_1_1_2 = new ListPanel(base.Context);
		_widget_1_1.AddChild(_widget_1_1_2);
		_widget_1_1_2_0 = new TextWidget(base.Context);
		_widget_1_1_2.AddChild(_widget_1_1_2_0);
		_widget_1_1_2_1 = new TextWidget(base.Context);
		_widget_1_1_2.AddChild(_widget_1_1_2_1);
	}

	public void SetIds()
	{
	}

	public void SetAttributes()
	{
		base.WidthSizePolicy = SizePolicy.StretchToParent;
		base.HeightSizePolicy = SizePolicy.StretchToParent;
		base.DoNotPassEventsToChildren = true;
		base.Brush = base.Context.GetBrush("Crafting.Order.Card");
		base.MarginTop = 6f;
		base.MarginBottom = 6f;
		base.MarginLeft = 6f;
		base.MarginRight = 6f;
		base.UpdateChildrenStates = true;
		_widget_0.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_0.HeightSizePolicy = SizePolicy.Fixed;
		_widget_0.SuggestedHeight = 150f;
		_widget_0.VerticalAlignment = VerticalAlignment.Bottom;
		_widget_0.MarginRight = 2f;
		_widget_0.MarginLeft = 2f;
		_widget_0.MarginBottom = 2f;
		_widget_0.Sprite = base.Context.SpriteData.GetSprite("scrollable_field_gradient_9");
		_widget_1.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_1.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_1.MarginTop = 10f;
		_widget_1.MarginBottom = 10f;
		_widget_1.MarginLeft = 15f;
		_widget_1.MarginRight = 15f;
		_widget_1.UpdateChildrenStates = true;
		_widget_1_0.WidthSizePolicy = SizePolicy.Fixed;
		_widget_1_0.HeightSizePolicy = SizePolicy.Fixed;
		_widget_1_0.SuggestedWidth = 15f;
		_widget_1_0.SuggestedHeight = 15f;
		_widget_1_0.VerticalAlignment = VerticalAlignment.Top;
		_widget_1_0.HorizontalAlignment = HorizontalAlignment.Right;
		_widget_1_0.PositionXOffset = 7f;
		_widget_1_0.PositionYOffset = -3f;
		_widget_1_0.Brush = base.Context.GetBrush("Crafting.Piece.Card.NewlyUnlocked.Dot");
		_widget_1_1.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_1_1.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_1_1.StackLayout.LayoutMethod = LayoutMethod.VerticalBottomToTop;
		_widget_1_1.MarginTop = 10f;
		_widget_1_1.UpdateChildrenStates = true;
		_widget_1_1_0.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_1_1_0.HeightSizePolicy = SizePolicy.Fixed;
		_widget_1_1_0.SuggestedHeight = 40f;
		_widget_1_1_0.Brush = base.Context.GetBrush("Crafting.Order.Card.Header");
		_widget_1_1_1.WidthSizePolicy = SizePolicy.Fixed;
		_widget_1_1_1.HeightSizePolicy = SizePolicy.Fixed;
		_widget_1_1_1.SuggestedWidth = 110f;
		_widget_1_1_1.SuggestedHeight = 110f;
		_widget_1_1_1.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_1_1_1.MarginTop = 15f;
		_widget_1_1_1.Color = new Color(1f, 0.8784314f, 0.6196079f);
		_widget_1_1_2.WidthSizePolicy = SizePolicy.CoverChildren;
		_widget_1_1_2.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_1_1_2.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_1_1_2.StackLayout.LayoutMethod = LayoutMethod.HorizontalLeftToRight;
		_widget_1_1_2.MarginBottom = 4f;
		_widget_1_1_2_0.WidthSizePolicy = SizePolicy.CoverChildren;
		_widget_1_1_2_0.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_1_1_2_0.MaxWidth = 150f;
		_widget_1_1_2_0.Brush = base.Context.GetBrush("Crafting.TemplateSelection.Card.Label");
		_widget_1_1_2_1.WidthSizePolicy = SizePolicy.CoverChildren;
		_widget_1_1_2_1.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_1_1_2_1.MarginLeft = 5f;
		_widget_1_1_2_1.Brush = base.Context.GetBrush("Crafting.TemplateSelection.Card.Value");
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
			_widget_1_0.PropertyChanged -= PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_0;
			_widget_1_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_0;
			_widget_1_1_0.PropertyChanged -= PropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_1.PropertyChanged -= PropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_2_0.PropertyChanged -= PropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_1.PropertyChanged -= PropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_1_2_1;
			_datasource_Root = null;
		}
	}

	public void SetDataSource(WeaponClassVM dataSource)
	{
		RefreshDataSource_datasource_Root(dataSource);
	}

	private void EventListenerOf_widget(Widget widget, string commandName, object[] args)
	{
		if (commandName == "Click")
		{
			_datasource_Root.ExecuteSelect();
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
		if (propertyName == "IsSelected")
		{
			_datasource_Root.IsSelected = _widget.IsSelected;
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
		if (propertyName == "IsVisible")
		{
			_datasource_Root.HasNewlyUnlockedPieces = _widget_1_0.IsVisible;
		}
	}

	private void PropertyChangedListenerOf_widget_1_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_0(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_1_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_0(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_1_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_0(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_1_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_0(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_1_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_0(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_1_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_0(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_1_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_0(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_1_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_0(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_1_1_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_0(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_1_1_0(string propertyName)
	{
		if (propertyName == "Text")
		{
			_datasource_Root.TemplateName = _widget_1_1_0.Text;
		}
	}

	private void PropertyChangedListenerOf_widget_1_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_1(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_1_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_1(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_1_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_1(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_1_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_1(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_1_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_1(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_1_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_1(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_1_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_1(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_1_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_1(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_1_1_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_1(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_1_1_1(string propertyName)
	{
		if (propertyName == "WeaponType")
		{
			_datasource_Root.WeaponType = _widget_1_1_1.WeaponType;
		}
	}

	private void PropertyChangedListenerOf_widget_1_1_2_0(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_0(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_1_1_2_0(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_0(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_1_1_2_0(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_0(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_1_1_2_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_0(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_1_1_2_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_0(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_1_1_2_0(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_0(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_1_1_2_0(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_0(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_1_1_2_0(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_0(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_1_1_2_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_0(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_1_1_2_0(string propertyName)
	{
		if (propertyName == "Text")
		{
			_datasource_Root.UnlockedPiecesLabelText = _widget_1_1_2_0.Text;
		}
	}

	private void PropertyChangedListenerOf_widget_1_1_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_1(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_1_1_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_1(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_1_1_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_1(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_1_1_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_1(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_1_1_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_1(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_1_1_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_1(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_1_1_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_1(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_1_1_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_1(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_1_1_2_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_1_1_2_1(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_1_1_2_1(string propertyName)
	{
		if (propertyName == "IntText")
		{
			_datasource_Root.UnlockedPiecesCount = _widget_1_1_2_1.IntText;
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
		case "IsSelected":
			_widget.IsSelected = _datasource_Root.IsSelected;
			break;
		case "HasNewlyUnlockedPieces":
			_widget_1_0.IsVisible = _datasource_Root.HasNewlyUnlockedPieces;
			break;
		case "TemplateName":
			_widget_1_1_0.Text = _datasource_Root.TemplateName;
			break;
		case "WeaponType":
			_widget_1_1_1.WeaponType = _datasource_Root.WeaponType;
			break;
		case "UnlockedPiecesLabelText":
			_widget_1_1_2_0.Text = _datasource_Root.UnlockedPiecesLabelText;
			break;
		case "UnlockedPiecesCount":
			_widget_1_1_2_1.IntText = _datasource_Root.UnlockedPiecesCount;
			break;
		}
	}

	private void RefreshDataSource_datasource_Root(WeaponClassVM newDataSource)
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
			_widget_1_0.PropertyChanged -= PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_0;
			_widget_1_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_0;
			_widget_1_1_0.PropertyChanged -= PropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_1.PropertyChanged -= PropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_2_0.PropertyChanged -= PropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_1.PropertyChanged -= PropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.intPropertyChanged -= intPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_1_1_2_1;
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
			_widget.IsSelected = _datasource_Root.IsSelected;
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
			_widget_1_0.IsVisible = _datasource_Root.HasNewlyUnlockedPieces;
			_widget_1_0.PropertyChanged += PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.boolPropertyChanged += boolPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.floatPropertyChanged += floatPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_1_0;
			_widget_1_0.doublePropertyChanged += doublePropertyChangedListenerOf_widget_1_0;
			_widget_1_0.intPropertyChanged += intPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.uintPropertyChanged += uintPropertyChangedListenerOf_widget_1_0;
			_widget_1_0.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_1_0;
			_widget_1_1_0.Text = _datasource_Root.TemplateName;
			_widget_1_1_0.PropertyChanged += PropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.boolPropertyChanged += boolPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.floatPropertyChanged += floatPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.doublePropertyChanged += doublePropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.intPropertyChanged += intPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.uintPropertyChanged += uintPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_0.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_1_1_0;
			_widget_1_1_1.WeaponType = _datasource_Root.WeaponType;
			_widget_1_1_1.PropertyChanged += PropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.boolPropertyChanged += boolPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.floatPropertyChanged += floatPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.doublePropertyChanged += doublePropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.intPropertyChanged += intPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.uintPropertyChanged += uintPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_1.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_1_1_1;
			_widget_1_1_2_0.Text = _datasource_Root.UnlockedPiecesLabelText;
			_widget_1_1_2_0.PropertyChanged += PropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.boolPropertyChanged += boolPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.floatPropertyChanged += floatPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.doublePropertyChanged += doublePropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.intPropertyChanged += intPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.uintPropertyChanged += uintPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_0.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_1_1_2_0;
			_widget_1_1_2_1.IntText = _datasource_Root.UnlockedPiecesCount;
			_widget_1_1_2_1.PropertyChanged += PropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.boolPropertyChanged += boolPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.floatPropertyChanged += floatPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.doublePropertyChanged += doublePropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.intPropertyChanged += intPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.uintPropertyChanged += uintPropertyChangedListenerOf_widget_1_1_2_1;
			_widget_1_1_2_1.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_1_1_2_1;
		}
	}
}
