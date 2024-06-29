using System.ComponentModel;
using System.Numerics;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;

namespace SandBox.GauntletUI.AutoGenerated1;

public class PartyScreen__TaleWorlds_CampaignSystem_ViewModelCollection_Party_PartyVM_Dependency_23_Standard_PopupCloseButton__DependendPrefab : ButtonWidget
{
	private ButtonWidget _widget;

	private TextWidget _widget_0;

	private InputKeyVisualWidget _widget_0_0;

	private PartyRecruitTroopVM _datasource_Root;

	private InputKeyItemVM _datasource_Root_DoneInputKey;

	public PartyScreen__TaleWorlds_CampaignSystem_ViewModelCollection_Party_PartyVM_Dependency_23_Standard_PopupCloseButton__DependendPrefab(UIContext context)
		: base(context)
	{
	}

	public void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new TextWidget(base.Context);
		_widget.AddChild(_widget_0);
		_widget_0_0 = new InputKeyVisualWidget(base.Context);
		_widget_0.AddChild(_widget_0_0);
	}

	public void SetIds()
	{
	}

	public void SetAttributes()
	{
		base.DoNotPassEventsToChildren = true;
		base.WidthSizePolicy = SizePolicy.Fixed;
		base.HeightSizePolicy = SizePolicy.Fixed;
		base.SuggestedWidth = 286f;
		base.SuggestedHeight = 82f;
		base.HorizontalAlignment = HorizontalAlignment.Center;
		base.VerticalAlignment = VerticalAlignment.Top;
		base.IsEnabled = true;
		base.Brush = base.Context.GetBrush("Standard.PopupCloseButton");
		base.UpdateChildrenStates = true;
		_widget_0.DoNotAcceptEvents = true;
		_widget_0.WidthSizePolicy = SizePolicy.CoverChildren;
		_widget_0.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_0.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_0.VerticalAlignment = VerticalAlignment.Center;
		_widget_0.Brush = base.Context.GetBrush("Standard.DialogCloseButtons.ButtonText");
		_widget_0_0.DoNotAcceptEvents = true;
		_widget_0_0.WidthSizePolicy = SizePolicy.Fixed;
		_widget_0_0.HeightSizePolicy = SizePolicy.Fixed;
		_widget_0_0.SuggestedWidth = 40f;
		_widget_0_0.SuggestedHeight = 40f;
		_widget_0_0.HorizontalAlignment = HorizontalAlignment.Left;
		_widget_0_0.VerticalAlignment = VerticalAlignment.Center;
		_widget_0_0.PositionXOffset = -40f;
		_widget_0_0.PositionYOffset = -1f;
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
			if (_datasource_Root_DoneInputKey != null)
			{
				_datasource_Root_DoneInputKey.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_DoneInputKey;
				_widget_0_0.PropertyChanged -= PropertyChangedListenerOf_widget_0_0;
				_widget_0_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_0_0;
				_widget_0_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_0_0;
				_widget_0_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_0_0;
				_widget_0_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_0_0;
				_datasource_Root_DoneInputKey = null;
			}
			_datasource_Root = null;
		}
	}

	public void SetDataSource(PartyRecruitTroopVM dataSource)
	{
		RefreshDataSource_datasource_Root(dataSource);
	}

	private void EventListenerOf_widget(Widget widget, string commandName, object[] args)
	{
		if (commandName == "Click")
		{
			_datasource_Root.ExecuteDone();
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
		if (propertyName == "Text")
		{
			_datasource_Root.DoneLbl = _widget_0.Text;
		}
	}

	private void PropertyChangedListenerOf_widget_0_0(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_0_0(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_0_0(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_0_0(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_0_0(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_0_0(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_0_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_0_0(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_0_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_0_0(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_0_0(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_0_0(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_0_0(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_0_0(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_0_0(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_0_0(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_0_0(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_0_0(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_0_0(string propertyName)
	{
		if (propertyName == "KeyID")
		{
			_datasource_Root_DoneInputKey.KeyID = _widget_0_0.KeyID;
		}
		else if (propertyName == "IsVisible")
		{
			_datasource_Root_DoneInputKey.IsVisible = _widget_0_0.IsVisible;
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
		if (propertyName == "DoneInputKey")
		{
			RefreshDataSource_datasource_Root_DoneInputKey(_datasource_Root.DoneInputKey);
		}
		else if (propertyName == "DoneLbl")
		{
			_widget_0.Text = _datasource_Root.DoneLbl;
		}
	}

	private void ViewModelPropertyChangedListenerOf_datasource_Root_DoneInputKey(object sender, PropertyChangedEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_DoneInputKey(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithValueListenerOf_datasource_Root_DoneInputKey(object sender, PropertyChangedWithValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_DoneInputKey(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_DoneInputKey(object sender, PropertyChangedWithBoolValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_DoneInputKey(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_DoneInputKey(object sender, PropertyChangedWithIntValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_DoneInputKey(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_DoneInputKey(object sender, PropertyChangedWithFloatValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_DoneInputKey(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_DoneInputKey(object sender, PropertyChangedWithUIntValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_DoneInputKey(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_DoneInputKey(object sender, PropertyChangedWithColorValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_DoneInputKey(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_DoneInputKey(object sender, PropertyChangedWithDoubleValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_DoneInputKey(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_DoneInputKey(object sender, PropertyChangedWithVec2ValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_DoneInputKey(e.PropertyName);
	}

	private void HandleViewModelPropertyChangeOf_datasource_Root_DoneInputKey(string propertyName)
	{
		if (propertyName == "KeyID")
		{
			_widget_0_0.KeyID = _datasource_Root_DoneInputKey.KeyID;
		}
		else if (propertyName == "IsVisible")
		{
			_widget_0_0.IsVisible = _datasource_Root_DoneInputKey.IsVisible;
		}
	}

	private void RefreshDataSource_datasource_Root(PartyRecruitTroopVM newDataSource)
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
			if (_datasource_Root_DoneInputKey != null)
			{
				_datasource_Root_DoneInputKey.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_DoneInputKey;
				_widget_0_0.PropertyChanged -= PropertyChangedListenerOf_widget_0_0;
				_widget_0_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_0_0;
				_widget_0_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_0_0;
				_widget_0_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_0_0;
				_widget_0_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_0_0;
				_datasource_Root_DoneInputKey = null;
			}
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
			_widget_0.Text = _datasource_Root.DoneLbl;
			_widget_0.PropertyChanged += PropertyChangedListenerOf_widget_0;
			_widget_0.boolPropertyChanged += boolPropertyChangedListenerOf_widget_0;
			_widget_0.floatPropertyChanged += floatPropertyChangedListenerOf_widget_0;
			_widget_0.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_0;
			_widget_0.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_0;
			_widget_0.doublePropertyChanged += doublePropertyChangedListenerOf_widget_0;
			_widget_0.intPropertyChanged += intPropertyChangedListenerOf_widget_0;
			_widget_0.uintPropertyChanged += uintPropertyChangedListenerOf_widget_0;
			_widget_0.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_0;
			_datasource_Root_DoneInputKey = _datasource_Root.DoneInputKey;
			if (_datasource_Root_DoneInputKey != null)
			{
				_datasource_Root_DoneInputKey.PropertyChanged += ViewModelPropertyChangedListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithValue += ViewModelPropertyChangedWithValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithBoolValue += ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithIntValue += ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithFloatValue += ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithUIntValue += ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithColorValue += ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithDoubleValue += ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_DoneInputKey;
				_datasource_Root_DoneInputKey.PropertyChangedWithVec2Value += ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_DoneInputKey;
				_widget_0_0.KeyID = _datasource_Root_DoneInputKey.KeyID;
				_widget_0_0.IsVisible = _datasource_Root_DoneInputKey.IsVisible;
				_widget_0_0.PropertyChanged += PropertyChangedListenerOf_widget_0_0;
				_widget_0_0.boolPropertyChanged += boolPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.floatPropertyChanged += floatPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_0_0;
				_widget_0_0.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_0_0;
				_widget_0_0.doublePropertyChanged += doublePropertyChangedListenerOf_widget_0_0;
				_widget_0_0.intPropertyChanged += intPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.uintPropertyChanged += uintPropertyChangedListenerOf_widget_0_0;
				_widget_0_0.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_0_0;
			}
		}
	}

	private void RefreshDataSource_datasource_Root_DoneInputKey(InputKeyItemVM newDataSource)
	{
		if (_datasource_Root_DoneInputKey != null)
		{
			_datasource_Root_DoneInputKey.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_DoneInputKey;
			_widget_0_0.PropertyChanged -= PropertyChangedListenerOf_widget_0_0;
			_widget_0_0.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_0_0;
			_widget_0_0.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_0_0;
			_widget_0_0.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_0_0;
			_widget_0_0.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_0_0;
			_widget_0_0.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_0_0;
			_widget_0_0.intPropertyChanged -= intPropertyChangedListenerOf_widget_0_0;
			_widget_0_0.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_0_0;
			_widget_0_0.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_0_0;
			_datasource_Root_DoneInputKey = null;
		}
		_datasource_Root_DoneInputKey = newDataSource;
		_datasource_Root_DoneInputKey = _datasource_Root.DoneInputKey;
		if (_datasource_Root_DoneInputKey != null)
		{
			_datasource_Root_DoneInputKey.PropertyChanged += ViewModelPropertyChangedListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithValue += ViewModelPropertyChangedWithValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithBoolValue += ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithIntValue += ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithFloatValue += ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithUIntValue += ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithColorValue += ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithDoubleValue += ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_DoneInputKey;
			_datasource_Root_DoneInputKey.PropertyChangedWithVec2Value += ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_DoneInputKey;
			_widget_0_0.KeyID = _datasource_Root_DoneInputKey.KeyID;
			_widget_0_0.IsVisible = _datasource_Root_DoneInputKey.IsVisible;
			_widget_0_0.PropertyChanged += PropertyChangedListenerOf_widget_0_0;
			_widget_0_0.boolPropertyChanged += boolPropertyChangedListenerOf_widget_0_0;
			_widget_0_0.floatPropertyChanged += floatPropertyChangedListenerOf_widget_0_0;
			_widget_0_0.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_0_0;
			_widget_0_0.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_0_0;
			_widget_0_0.doublePropertyChanged += doublePropertyChangedListenerOf_widget_0_0;
			_widget_0_0.intPropertyChanged += intPropertyChangedListenerOf_widget_0_0;
			_widget_0_0.uintPropertyChanged += uintPropertyChangedListenerOf_widget_0_0;
			_widget_0_0.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_0_0;
		}
	}
}
