using System.ComponentModel;
using System.Numerics;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party;

namespace SandBox.GauntletUI.AutoGenerated1;

public class EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_4_EncyclopediaFilterGroup__InheritedPrefab : Widget
{
	private Widget _widget;

	private NavigationScopeTargeter _widget_0;

	private NavigationAutoScrollWidget _widget_1;

	private PartyHeaderToggleWidget _widget_2;

	private ListPanel _widget_2_0;

	private ImageWidget _widget_2_0_0;

	private TextWidget _widget_2_0_1;

	private ImageWidget _widget_2_0_2;

	private NavigatableListPanel _widget_3;

	private EncyclopediaFilterGroupVM _datasource_Root;

	private MBBindingList<EncyclopediaListFilterVM> _datasource_Root_Filters;

	public EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_4_EncyclopediaFilterGroup__InheritedPrefab(UIContext context)
		: base(context)
	{
	}

	public virtual void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new NavigationScopeTargeter(base.Context);
		_widget.AddChild(_widget_0);
		_widget_1 = new NavigationAutoScrollWidget(base.Context);
		_widget.AddChild(_widget_1);
		_widget_2 = new PartyHeaderToggleWidget(base.Context);
		_widget.AddChild(_widget_2);
		_widget_2_0 = new ListPanel(base.Context);
		_widget_2.AddChild(_widget_2_0);
		_widget_2_0_0 = new ImageWidget(base.Context);
		_widget_2_0.AddChild(_widget_2_0_0);
		_widget_2_0_1 = new TextWidget(base.Context);
		_widget_2_0.AddChild(_widget_2_0_1);
		_widget_2_0_2 = new ImageWidget(base.Context);
		_widget_2_0.AddChild(_widget_2_0_2);
		_widget_3 = new NavigatableListPanel(base.Context);
		_widget.AddChild(_widget_3);
	}

	public virtual void SetIds()
	{
		_widget_2.Id = "HeaderToggle";
		_widget_2_0.Id = "Description";
		_widget_2_0_0.Id = "CollapseIndicator";
		_widget_3.Id = "Filters";
	}

	public virtual void SetAttributes()
	{
		base.HeightSizePolicy = SizePolicy.CoverChildren;
		base.WidthSizePolicy = SizePolicy.StretchToParent;
		base.DoNotAcceptEvents = true;
		_widget_0.ScopeID = "EncyclopediaFilterGroupScope";
		_widget_0.ScopeParent = _widget;
		_widget_0.ScopeMovements = GamepadNavigationTypes.Vertical;
		_widget_1.TrackedWidget = _widget_2;
		_widget_2.DoNotPassEventsToChildren = true;
		_widget_2.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_2.HorizontalAlignment = HorizontalAlignment.Left;
		_widget_2.VerticalAlignment = VerticalAlignment.Top;
		_widget_2.CollapseIndicator = _widget_2_0_0;
		_widget_2.ListPanel = _widget_3;
		_widget_2.RenderLate = true;
		_widget_2.WidgetToClose = _widget_3;
		_widget_2.UpdateChildrenStates = true;
		_widget_2.GamepadNavigationIndex = 0;
		_widget_2_0.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2_0.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_2_0.HorizontalAlignment = HorizontalAlignment.Left;
		_widget_2_0.VerticalAlignment = VerticalAlignment.Center;
		_widget_2_0.DoNotAcceptEvents = true;
		_widget_2_0.UpdateChildrenStates = true;
		_widget_2_0.StackLayout.LayoutMethod = LayoutMethod.HorizontalLeftToRight;
		_widget_2_0_0.WidthSizePolicy = SizePolicy.Fixed;
		_widget_2_0_0.HeightSizePolicy = SizePolicy.Fixed;
		_widget_2_0_0.SuggestedHeight = 19f;
		_widget_2_0_0.SuggestedWidth = 19f;
		_widget_2_0_0.VerticalAlignment = VerticalAlignment.Center;
		_widget_2_0_0.PositionXOffset = -10f;
		_widget_2_0_0.PositionYOffset = -3f;
		_widget_2_0_0.Brush = base.Context.GetBrush("SPOptions.GameKeysgroup.ExpandIndicator");
		_widget_2_0_0.OverrideDefaultStateSwitchingEnabled = true;
		_widget_2_0_1.WidthSizePolicy = SizePolicy.CoverChildren;
		_widget_2_0_1.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_2_0_1.Brush = base.Context.GetBrush("SPOptions.GameKeysGroup.Title.Text");
		_widget_2_0_1.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_2_0_2.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_2_0_2.HeightSizePolicy = SizePolicy.Fixed;
		_widget_2_0_2.SuggestedHeight = 2f;
		_widget_2_0_2.VerticalAlignment = VerticalAlignment.Center;
		_widget_2_0_2.MarginLeft = 5f;
		_widget_2_0_2.Brush = base.Context.GetBrush("SPOptions.CollapserLine");
		_widget_3.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_3.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_3.StackLayout.LayoutMethod = LayoutMethod.VerticalTopToBottom;
		_widget_3.MarginTop = 40f;
		_widget_3.DoNotAcceptEvents = true;
		_widget_3.MarginLeft = 5f;
		_widget_3.MaxIndex = 10000;
	}

	public virtual void DestroyDataSource()
	{
		if (_datasource_Root == null)
		{
			return;
		}
		_datasource_Root.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
		_widget_2_0_1.PropertyChanged -= PropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.intPropertyChanged -= intPropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_2_0_1;
		if (_datasource_Root_Filters != null)
		{
			_datasource_Root_Filters.ListChanged -= OnList_datasource_Root_FiltersChanged;
			for (int num = _widget_3.ChildCount - 1; num >= 0; num--)
			{
				Widget child = _widget_3.GetChild(num);
				((EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate)child).OnBeforeRemovedChild(child);
				((EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate)_widget_3.GetChild(num)).DestroyDataSource();
			}
			_datasource_Root_Filters = null;
		}
		_datasource_Root = null;
	}

	public virtual void SetDataSource(EncyclopediaFilterGroupVM dataSource)
	{
		RefreshDataSource_datasource_Root(dataSource);
	}

	private void PropertyChangedListenerOf_widget_2_0_1(PropertyOwnerObject propertyOwnerObject, string propertyName, object e)
	{
		HandleWidgetPropertyChangeOf_widget_2_0_1(propertyName);
	}

	private void boolPropertyChangedListenerOf_widget_2_0_1(PropertyOwnerObject propertyOwnerObject, string propertyName, bool e)
	{
		HandleWidgetPropertyChangeOf_widget_2_0_1(propertyName);
	}

	private void floatPropertyChangedListenerOf_widget_2_0_1(PropertyOwnerObject propertyOwnerObject, string propertyName, float e)
	{
		HandleWidgetPropertyChangeOf_widget_2_0_1(propertyName);
	}

	private void Vec2PropertyChangedListenerOf_widget_2_0_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 e)
	{
		HandleWidgetPropertyChangeOf_widget_2_0_1(propertyName);
	}

	private void Vector2PropertyChangedListenerOf_widget_2_0_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 e)
	{
		HandleWidgetPropertyChangeOf_widget_2_0_1(propertyName);
	}

	private void doublePropertyChangedListenerOf_widget_2_0_1(PropertyOwnerObject propertyOwnerObject, string propertyName, double e)
	{
		HandleWidgetPropertyChangeOf_widget_2_0_1(propertyName);
	}

	private void intPropertyChangedListenerOf_widget_2_0_1(PropertyOwnerObject propertyOwnerObject, string propertyName, int e)
	{
		HandleWidgetPropertyChangeOf_widget_2_0_1(propertyName);
	}

	private void uintPropertyChangedListenerOf_widget_2_0_1(PropertyOwnerObject propertyOwnerObject, string propertyName, uint e)
	{
		HandleWidgetPropertyChangeOf_widget_2_0_1(propertyName);
	}

	private void ColorPropertyChangedListenerOf_widget_2_0_1(PropertyOwnerObject propertyOwnerObject, string propertyName, Color e)
	{
		HandleWidgetPropertyChangeOf_widget_2_0_1(propertyName);
	}

	private void HandleWidgetPropertyChangeOf_widget_2_0_1(string propertyName)
	{
		if (propertyName == "Text")
		{
			_datasource_Root.FilterName = _widget_2_0_1.Text;
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
		if (propertyName == "Filters")
		{
			RefreshDataSource_datasource_Root_Filters(_datasource_Root.Filters);
		}
		else if (propertyName == "FilterName")
		{
			_widget_2_0_1.Text = _datasource_Root.FilterName;
		}
	}

	public void OnList_datasource_Root_FiltersChanged(object sender, TaleWorlds.Library.ListChangedEventArgs e)
	{
		switch (e.ListChangedType)
		{
		case TaleWorlds.Library.ListChangedType.Reset:
		{
			for (int num = _widget_3.ChildCount - 1; num >= 0; num--)
			{
				Widget child3 = _widget_3.GetChild(num);
				((EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate)child3).OnBeforeRemovedChild(child3);
				Widget child4 = _widget_3.GetChild(num);
				((EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate)child4).SetDataSource(null);
				_widget_3.RemoveChild(child4);
			}
			break;
		}
		case TaleWorlds.Library.ListChangedType.Sorted:
		{
			for (int i = 0; i < _datasource_Root_Filters.Count; i++)
			{
				EncyclopediaListFilterVM bindingObject = _datasource_Root_Filters[i];
				_widget_3.FindChild((Widget widget) => widget.GetComponent<GeneratedWidgetData>().Data == bindingObject).SetSiblingIndex(i);
			}
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemAdded:
		{
			EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate = new EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate(base.Context);
			GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate);
			EncyclopediaListFilterVM dataSource = (EncyclopediaListFilterVM)(generatedWidgetData.Data = _datasource_Root_Filters[e.NewIndex]);
			encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.AddComponent(generatedWidgetData);
			_widget_3.AddChildAtIndex(encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate, e.NewIndex);
			encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.CreateWidgets();
			encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.SetIds();
			encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.SetAttributes();
			encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.SetDataSource(dataSource);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemBeforeDeleted:
		{
			Widget child2 = _widget_3.GetChild(e.NewIndex);
			((EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate)child2).OnBeforeRemovedChild(child2);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemDeleted:
		{
			Widget child = _widget_3.GetChild(e.NewIndex);
			((EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate)child).SetDataSource(null);
			_widget_3.RemoveChild(child);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemChanged:
			break;
		}
	}

	private void RefreshDataSource_datasource_Root(EncyclopediaFilterGroupVM newDataSource)
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
			_widget_2_0_1.PropertyChanged -= PropertyChangedListenerOf_widget_2_0_1;
			_widget_2_0_1.boolPropertyChanged -= boolPropertyChangedListenerOf_widget_2_0_1;
			_widget_2_0_1.floatPropertyChanged -= floatPropertyChangedListenerOf_widget_2_0_1;
			_widget_2_0_1.Vec2PropertyChanged -= Vec2PropertyChangedListenerOf_widget_2_0_1;
			_widget_2_0_1.Vector2PropertyChanged -= Vector2PropertyChangedListenerOf_widget_2_0_1;
			_widget_2_0_1.doublePropertyChanged -= doublePropertyChangedListenerOf_widget_2_0_1;
			_widget_2_0_1.intPropertyChanged -= intPropertyChangedListenerOf_widget_2_0_1;
			_widget_2_0_1.uintPropertyChanged -= uintPropertyChangedListenerOf_widget_2_0_1;
			_widget_2_0_1.ColorPropertyChanged -= ColorPropertyChangedListenerOf_widget_2_0_1;
			if (_datasource_Root_Filters != null)
			{
				_datasource_Root_Filters.ListChanged -= OnList_datasource_Root_FiltersChanged;
				for (int num = _widget_3.ChildCount - 1; num >= 0; num--)
				{
					Widget child = _widget_3.GetChild(num);
					((EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate)child).OnBeforeRemovedChild(child);
					Widget child2 = _widget_3.GetChild(num);
					((EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate)child2).SetDataSource(null);
					_widget_3.RemoveChild(child2);
				}
				_datasource_Root_Filters = null;
			}
			_datasource_Root = null;
		}
		_datasource_Root = newDataSource;
		if (_datasource_Root == null)
		{
			return;
		}
		_datasource_Root.PropertyChanged += ViewModelPropertyChangedListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithValue += ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithBoolValue += ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithIntValue += ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithFloatValue += ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithUIntValue += ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithColorValue += ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithDoubleValue += ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
		_datasource_Root.PropertyChangedWithVec2Value += ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
		_widget_2_0_1.Text = _datasource_Root.FilterName;
		_widget_2_0_1.PropertyChanged += PropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.boolPropertyChanged += boolPropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.floatPropertyChanged += floatPropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.Vec2PropertyChanged += Vec2PropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.Vector2PropertyChanged += Vector2PropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.doublePropertyChanged += doublePropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.intPropertyChanged += intPropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.uintPropertyChanged += uintPropertyChangedListenerOf_widget_2_0_1;
		_widget_2_0_1.ColorPropertyChanged += ColorPropertyChangedListenerOf_widget_2_0_1;
		_datasource_Root_Filters = _datasource_Root.Filters;
		if (_datasource_Root_Filters != null)
		{
			_datasource_Root_Filters.ListChanged += OnList_datasource_Root_FiltersChanged;
			for (int i = 0; i < _datasource_Root_Filters.Count; i++)
			{
				EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate = new EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate(base.Context);
				GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate);
				EncyclopediaListFilterVM dataSource = (EncyclopediaListFilterVM)(generatedWidgetData.Data = _datasource_Root_Filters[i]);
				encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.AddComponent(generatedWidgetData);
				_widget_3.AddChildAtIndex(encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate, i);
				encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.CreateWidgets();
				encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.SetIds();
				encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.SetAttributes();
				encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.SetDataSource(dataSource);
			}
		}
	}

	private void RefreshDataSource_datasource_Root_Filters(MBBindingList<EncyclopediaListFilterVM> newDataSource)
	{
		if (_datasource_Root_Filters != null)
		{
			_datasource_Root_Filters.ListChanged -= OnList_datasource_Root_FiltersChanged;
			for (int num = _widget_3.ChildCount - 1; num >= 0; num--)
			{
				Widget child = _widget_3.GetChild(num);
				((EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate)child).OnBeforeRemovedChild(child);
				Widget child2 = _widget_3.GetChild(num);
				((EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate)child2).SetDataSource(null);
				_widget_3.RemoveChild(child2);
			}
			_datasource_Root_Filters = null;
		}
		_datasource_Root_Filters = newDataSource;
		_datasource_Root_Filters = _datasource_Root.Filters;
		if (_datasource_Root_Filters != null)
		{
			_datasource_Root_Filters.ListChanged += OnList_datasource_Root_FiltersChanged;
			for (int i = 0; i < _datasource_Root_Filters.Count; i++)
			{
				EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate = new EncyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate(base.Context);
				GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate);
				EncyclopediaListFilterVM dataSource = (EncyclopediaListFilterVM)(generatedWidgetData.Data = _datasource_Root_Filters[i]);
				encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.AddComponent(generatedWidgetData);
				_widget_3.AddChildAtIndex(encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate, i);
				encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.CreateWidgets();
				encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.SetIds();
				encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.SetAttributes();
				encyclopediaItemList__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_Pages_EncyclopediaFactionPageVM_Dependency_7_ItemTemplate.SetDataSource(dataSource);
			}
		}
	}
}
