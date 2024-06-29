using System.ComponentModel;
using System.Numerics;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;

namespace SandBox.GauntletUI.AutoGenerated1;

public class EncyclopediaClanPage__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaClanPageVM_Dependency_3_ItemTemplate : ButtonWidget
{
	private ButtonWidget _widget;

	private Widget _widget_0;

	private HintWidget _widget_1;

	private TextWidget _widget_2;

	private EncyclopediaSettlementVM _datasource_Root;

	public EncyclopediaClanPage__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaClanPageVM_Dependency_3_ItemTemplate(UIContext context)
		: base(context)
	{
	}

	public void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new Widget(base.Context);
		_widget.AddChild(_widget_0);
		_widget_1 = new HintWidget(base.Context);
		_widget.AddChild(_widget_1);
		_widget_2 = new TextWidget(base.Context);
		_widget.AddChild(_widget_2);
	}

	public void SetIds()
	{
		_widget_0.Id = "VillageImage";
	}

	public void SetAttributes()
	{
		base.DoNotPassEventsToChildren = true;
		base.WidthSizePolicy = SizePolicy.Fixed;
		base.HeightSizePolicy = SizePolicy.Fixed;
		base.SuggestedHeight = 90f;
		base.SuggestedWidth = 90f;
		base.Brush = base.Context.GetBrush("Encyclopedia.SubPage.Element");
		_widget_0.WidthSizePolicy = SizePolicy.Fixed;
		_widget_0.HeightSizePolicy = SizePolicy.Fixed;
		_widget_0.SuggestedHeight = 80f;
		_widget_0.SuggestedWidth = 80f;
		_widget_0.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_0.VerticalAlignment = VerticalAlignment.Center;
		_widget_1.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_1.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_2.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2.VerticalAlignment = VerticalAlignment.Top;
		_widget_2.Brush = base.Context.GetBrush("Encyclopedia.SubPage.Element.Name.Text");
		_widget_2.PositionYOffset = 95f;
		_widget_2.Brush.FontSize = 18;
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
			_widget_0.PropertyChanged -= PropertyChangedListenerOf_widget_0;
			_widget_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_0;
			_widget_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_0;
			_widget_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_0;
			_widget_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_0;
			_widget_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_0;
			_widget_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_0;
			_widget_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_0;
			_widget_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_0;
			_widget_1.EventFire -= EventListenerOf_widget_1;
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

	public void SetDataSource(EncyclopediaSettlementVM dataSource)
	{
		RefreshDataSource_datasource_Root(dataSource);
	}

	private void EventListenerOf_widget(Widget widget, string commandName, object[] args)
	{
		if (commandName == "Click")
		{
			_datasource_Root.ExecuteLink();
		}
	}

	private void EventListenerOf_widget_1(Widget widget, string commandName, object[] args)
	{
		if (commandName == "HoverBegin")
		{
			_datasource_Root.ExecuteBeginHint();
		}
		if (commandName == "HoverEnd")
		{
			_datasource_Root.ExecuteEndHint();
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
		if (propertyName == "Sprite")
		{
			_datasource_Root.FileName = _widget_0.Sprite.Name;
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
			_datasource_Root.NameText = _widget_2.Text;
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
		if (propertyName == "FileName")
		{
			if (_datasource_Root.FileName != null)
			{
				_widget_0.Sprite = base.Context.SpriteData.GetSprite(_datasource_Root.FileName);
			}
		}
		else if (propertyName == "NameText")
		{
			_widget_2.Text = _datasource_Root.NameText;
		}
	}

	private void RefreshDataSource_datasource_Root(EncyclopediaSettlementVM newDataSource)
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
			_widget_0.PropertyChanged -= PropertyChangedListenerOf_widget_0;
			_widget_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_0;
			_widget_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_0;
			_widget_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_0;
			_widget_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_0;
			_widget_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_0;
			_widget_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_0;
			_widget_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_0;
			_widget_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_0;
			_widget_1.EventFire -= EventListenerOf_widget_1;
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
			_widget.EventFire += EventListenerOf_widget;
			if (_datasource_Root.FileName != null)
			{
				_widget_0.Sprite = base.Context.SpriteData.GetSprite(_datasource_Root.FileName);
			}
			_widget_0.PropertyChanged += PropertyChangedListenerOf_widget_0;
			_widget_0.boolPropertyChanged += boolPropertyChangedListenerOf_widget_0;
			_widget_0.floatPropertyChanged += floatPropertyChangedListenerOf_widget_0;
			_widget_0.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_0;
			_widget_0.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_0;
			_widget_0.doublePropertyChanged += doublePropertyChangedListenerOf_widget_0;
			_widget_0.intPropertyChanged += intPropertyChangedListenerOf_widget_0;
			_widget_0.uintPropertyChanged += uintPropertyChangedListenerOf_widget_0;
			_widget_0.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_0;
			_widget_1.EventFire += EventListenerOf_widget_1;
			_widget_2.Text = _datasource_Root.NameText;
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
