using System.ComponentModel;
using System.Numerics;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;
using TaleWorlds.MountAndBlade.ViewModelCollection.FaceGenerator;

namespace TaleWorlds.MountAndBlade.GauntletUI.AutoGenerated1;

public class FaceGen__TaleWorlds_MountAndBlade_ViewModelCollection_FaceGenerator_FaceGenVM_Dependency_7_FaceGenHair__DependendPrefab : ListPanel
{
	private ListPanel _widget;

	private NavigationScopeTargeter _widget_0;

	private FaceGen__TaleWorlds_MountAndBlade_ViewModelCollection_FaceGenerator_FaceGenVM_Dependency_16_FaceGenColorSelector__DependendPrefab _widget_1;

	private Widget _widget_2;

	private NavigationScopeTargeter _widget_3;

	private FaceGen__TaleWorlds_MountAndBlade_ViewModelCollection_FaceGenerator_FaceGenVM_Dependency_17_FaceGenGrid__DependendPrefab _widget_4;

	private Widget _widget_5;

	private NavigationScopeTargeter _widget_6;

	private FaceGen__TaleWorlds_MountAndBlade_ViewModelCollection_FaceGenerator_FaceGenVM_Dependency_18_FaceGenGrid__DependendPrefab _widget_7;

	private FaceGenVM _datasource_Root;

	private SelectorVM<SelectorItemVM> _datasource_Root_HairColorSelector;

	public FaceGen__TaleWorlds_MountAndBlade_ViewModelCollection_FaceGenerator_FaceGenVM_Dependency_7_FaceGenHair__DependendPrefab(UIContext context)
		: base(context)
	{
	}

	public void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new NavigationScopeTargeter(base.Context);
		_widget.AddChild(_widget_0);
		_widget_1 = new FaceGen__TaleWorlds_MountAndBlade_ViewModelCollection_FaceGenerator_FaceGenVM_Dependency_16_FaceGenColorSelector__DependendPrefab(base.Context);
		_widget.AddChild(_widget_1);
		_widget_1.CreateWidgets();
		_widget_2 = new Widget(base.Context);
		_widget.AddChild(_widget_2);
		_widget_3 = new NavigationScopeTargeter(base.Context);
		_widget.AddChild(_widget_3);
		_widget_4 = new FaceGen__TaleWorlds_MountAndBlade_ViewModelCollection_FaceGenerator_FaceGenVM_Dependency_17_FaceGenGrid__DependendPrefab(base.Context);
		_widget.AddChild(_widget_4);
		_widget_4.CreateWidgets();
		_widget_5 = new Widget(base.Context);
		_widget.AddChild(_widget_5);
		_widget_6 = new NavigationScopeTargeter(base.Context);
		_widget.AddChild(_widget_6);
		_widget_7 = new FaceGen__TaleWorlds_MountAndBlade_ViewModelCollection_FaceGenerator_FaceGenVM_Dependency_18_FaceGenGrid__DependendPrefab(base.Context);
		_widget.AddChild(_widget_7);
		_widget_7.CreateWidgets();
	}

	public void SetIds()
	{
		_widget_1.SetIds();
		_widget_1.Id = "HairColor";
		_widget_4.SetIds();
		_widget_4.Id = "HairTypes";
		_widget_7.SetIds();
		_widget_7.Id = "BeardTypes";
	}

	public void SetAttributes()
	{
		base.WidthSizePolicy = SizePolicy.StretchToParent;
		base.HeightSizePolicy = SizePolicy.StretchToParent;
		base.StackLayout.LayoutMethod = LayoutMethod.VerticalBottomToTop;
		_widget_0.ScopeID = "HairColorScope";
		_widget_0.ScopeParent = _widget_1;
		_widget_0.ScopeMovements = GamepadNavigationTypes.Horizontal;
		_widget_0.AlternateScopeMovements = GamepadNavigationTypes.Vertical;
		_widget_0.AlternateMovementStepSize = 12;
		_widget_0.HasCircularMovement = false;
		_widget_1.SetAttributes();
		_widget_1.MarginTop = 10f;
		_widget_2.WidthSizePolicy = SizePolicy.Fixed;
		_widget_2.SuggestedWidth = 581f;
		_widget_2.HeightSizePolicy = SizePolicy.Fixed;
		_widget_2.SuggestedHeight = 14f;
		_widget_2.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_2.Sprite = base.Context.SpriteData.GetSprite("General\\CharacterCreation\\stone_divider");
		_widget_3.ScopeID = "HairTypeScope";
		_widget_3.ScopeParent = _widget_4;
		_widget_3.ScopeMovements = GamepadNavigationTypes.Horizontal;
		_widget_3.AlternateScopeMovements = GamepadNavigationTypes.Vertical;
		_widget_3.AlternateMovementStepSize = 5;
		_widget_3.HasCircularMovement = false;
		_widget_4.SetAttributes();
		_widget_4.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_4.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_4.MarginRight = 3f;
		_widget_5.WidthSizePolicy = SizePolicy.Fixed;
		_widget_5.SuggestedWidth = 580f;
		_widget_5.HeightSizePolicy = SizePolicy.Fixed;
		_widget_5.SuggestedHeight = 11f;
		_widget_5.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_5.Sprite = base.Context.SpriteData.GetSprite("General\\CharacterCreation\\stone_divider_thin");
		_widget_5.ExtendTop = 19f;
		_widget_5.ExtendBottom = 20f;
		_widget_6.ScopeID = "BeardTypeScope";
		_widget_6.ScopeParent = _widget_7;
		_widget_6.ScopeMovements = GamepadNavigationTypes.Horizontal;
		_widget_6.AlternateScopeMovements = GamepadNavigationTypes.Vertical;
		_widget_6.AlternateMovementStepSize = 5;
		_widget_6.HasCircularMovement = false;
		_widget_7.SetAttributes();
		_widget_7.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_7.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_7.MarginRight = 3f;
	}

	public void DestroyDataSource()
	{
		if (_datasource_Root != null)
		{
			_widget_4.DestroyDataSource();
			_widget_7.DestroyDataSource();
			_datasource_Root.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
			_widget_7.PropertyChanged -= PropertyChangedListenerOf_widget_7;
			_widget_7.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_7;
			_widget_7.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_7;
			_widget_7.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_7;
			_widget_7.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_7;
			_widget_7.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_7;
			_widget_7.intPropertyChanged -= intPropertyChangedListenerOf_widget_7;
			_widget_7.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_7;
			_widget_7.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_7;
			if (_datasource_Root_HairColorSelector != null)
			{
				_widget_1.DestroyDataSource();
				_datasource_Root_HairColorSelector.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector = null;
			}
			_datasource_Root = null;
		}
	}

	public void SetDataSource(FaceGenVM dataSource)
	{
		RefreshDataSource_datasource_Root(dataSource);
	}

	private void PropertyChangedListenerOf_widget_7(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_7(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_7(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_7(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_7(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_7(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_7(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_7(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_7(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_7(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_7(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_7(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_7(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_7(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_7(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_7(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_7(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_7(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_7(string propertyName)
	{
		_ = propertyName == "IsHidden";
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
		if (propertyName == "HairColorSelector")
		{
			RefreshDataSource_datasource_Root_HairColorSelector(_datasource_Root.HairColorSelector);
		}
		else if (propertyName == "IsFemale")
		{
			_widget_7.IsHidden = _datasource_Root.IsFemale;
		}
	}

	private void ViewModelPropertyChangedListenerOf_datasource_Root_HairColorSelector(object sender, PropertyChangedEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_HairColorSelector(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithValueListenerOf_datasource_Root_HairColorSelector(object sender, PropertyChangedWithValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_HairColorSelector(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_HairColorSelector(object sender, PropertyChangedWithBoolValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_HairColorSelector(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_HairColorSelector(object sender, PropertyChangedWithIntValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_HairColorSelector(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_HairColorSelector(object sender, PropertyChangedWithFloatValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_HairColorSelector(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_HairColorSelector(object sender, PropertyChangedWithUIntValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_HairColorSelector(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_HairColorSelector(object sender, PropertyChangedWithColorValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_HairColorSelector(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_HairColorSelector(object sender, PropertyChangedWithDoubleValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_HairColorSelector(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_HairColorSelector(object sender, PropertyChangedWithVec2ValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root_HairColorSelector(e.PropertyName);
	}

	private void HandleViewModelPropertyChangeOf_datasource_Root_HairColorSelector(string propertyName)
	{
	}

	private void RefreshDataSource_datasource_Root(FaceGenVM newDataSource)
	{
		if (_datasource_Root != null)
		{
			_widget_4.SetDataSource(null);
			_widget_7.SetDataSource(null);
			_datasource_Root.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
			_widget_7.PropertyChanged -= PropertyChangedListenerOf_widget_7;
			_widget_7.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_7;
			_widget_7.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_7;
			_widget_7.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_7;
			_widget_7.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_7;
			_widget_7.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_7;
			_widget_7.intPropertyChanged -= intPropertyChangedListenerOf_widget_7;
			_widget_7.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_7;
			_widget_7.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_7;
			if (_datasource_Root_HairColorSelector != null)
			{
				_widget_1.SetDataSource(null);
				_datasource_Root_HairColorSelector.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector = null;
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
			_widget_7.IsHidden = _datasource_Root.IsFemale;
			_widget_7.PropertyChanged += PropertyChangedListenerOf_widget_7;
			_widget_7.boolPropertyChanged += boolPropertyChangedListenerOf_widget_7;
			_widget_7.floatPropertyChanged += floatPropertyChangedListenerOf_widget_7;
			_widget_7.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_7;
			_widget_7.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_7;
			_widget_7.doublePropertyChanged += doublePropertyChangedListenerOf_widget_7;
			_widget_7.intPropertyChanged += intPropertyChangedListenerOf_widget_7;
			_widget_7.uintPropertyChanged += uintPropertyChangedListenerOf_widget_7;
			_widget_7.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_7;
			_datasource_Root_HairColorSelector = _datasource_Root.HairColorSelector;
			if (_datasource_Root_HairColorSelector != null)
			{
				_datasource_Root_HairColorSelector.PropertyChanged += ViewModelPropertyChangedListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithValue += ViewModelPropertyChangedWithValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithBoolValue += ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithIntValue += ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithFloatValue += ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithUIntValue += ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithColorValue += ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithDoubleValue += ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_HairColorSelector;
				_datasource_Root_HairColorSelector.PropertyChangedWithVec2Value += ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_HairColorSelector;
				_widget_1.SetDataSource(_datasource_Root_HairColorSelector);
			}
			_widget_4.SetDataSource(_datasource_Root);
			_widget_7.SetDataSource(_datasource_Root);
		}
	}

	private void RefreshDataSource_datasource_Root_HairColorSelector(SelectorVM<SelectorItemVM> newDataSource)
	{
		if (_datasource_Root_HairColorSelector != null)
		{
			_widget_1.SetDataSource(null);
			_datasource_Root_HairColorSelector.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector = null;
		}
		_datasource_Root_HairColorSelector = newDataSource;
		_datasource_Root_HairColorSelector = _datasource_Root.HairColorSelector;
		if (_datasource_Root_HairColorSelector != null)
		{
			_datasource_Root_HairColorSelector.PropertyChanged += ViewModelPropertyChangedListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithValue += ViewModelPropertyChangedWithValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithBoolValue += ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithIntValue += ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithFloatValue += ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithUIntValue += ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithColorValue += ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithDoubleValue += ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root_HairColorSelector;
			_datasource_Root_HairColorSelector.PropertyChangedWithVec2Value += ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root_HairColorSelector;
			_widget_1.SetDataSource(_datasource_Root_HairColorSelector);
		}
	}
}