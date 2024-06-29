using System.ComponentModel;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;

namespace SandBox.GauntletUI.AutoGenerated0;

public class EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM : BrushWidget, IGeneratedGauntletMovieRoot
{
	private BrushWidget _widget;

	private Widget _widget_0;

	private NavigationScopeTargeter _widget_0_0;

	private NavigatableListPanel _widget_0_1;

	private EncyclopediaHomeVM _datasource_Root;

	private MBBindingList<ListTypeVM> _datasource_Root_Lists;

	public EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM(UIContext context)
		: base(context)
	{
	}

	public void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new Widget(base.Context);
		_widget.AddChild(_widget_0);
		_widget_0_0 = new NavigationScopeTargeter(base.Context);
		_widget_0.AddChild(_widget_0_0);
		_widget_0_1 = new NavigatableListPanel(base.Context);
		_widget_0.AddChild(_widget_0_1);
	}

	public void SetIds()
	{
		_widget_0_1.Id = "EncyclopediaHomeList";
	}

	public void SetAttributes()
	{
		base.HeightSizePolicy = SizePolicy.StretchToParent;
		base.WidthSizePolicy = SizePolicy.StretchToParent;
		base.DoNotAcceptEvents = true;
		base.Brush = base.Context.GetBrush("Encyclopedia.Page.SoundBrush");
		_widget_0.HeightSizePolicy = SizePolicy.Fixed;
		_widget_0.WidthSizePolicy = SizePolicy.Fixed;
		_widget_0.SuggestedHeight = 762f;
		_widget_0.SuggestedWidth = 1465f;
		_widget_0.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_0.VerticalAlignment = VerticalAlignment.Center;
		_widget_0.DoNotAcceptEvents = true;
		_widget_0_0.ScopeID = "EncyclopediaHomeScope";
		_widget_0_0.ScopeParent = _widget_0_1;
		_widget_0_0.ScopeMovements = GamepadNavigationTypes.Horizontal;
		_widget_0_0.IsDefaultNavigationScope = true;
		_widget_0_0.HasCircularMovement = false;
		_widget_0_1.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_0_1.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_0_1.MarginLeft = 15f;
		_widget_0_1.HorizontalAlignment = HorizontalAlignment.Left;
		_widget_0_1.VerticalAlignment = VerticalAlignment.Center;
	}

	public void RefreshBindingWithChildren()
	{
		EncyclopediaHomeVM datasource_Root = _datasource_Root;
		SetDataSource(null);
		SetDataSource(datasource_Root);
	}

	public void DestroyDataSource()
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
		if (_datasource_Root_Lists != null)
		{
			_datasource_Root_Lists.ListChanged -= OnList_datasource_Root_ListsChanged;
			for (int num = _widget_0_1.ChildCount - 1; num >= 0; num--)
			{
				Widget child = _widget_0_1.GetChild(num);
				((EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate)child).OnBeforeRemovedChild(child);
				((EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate)_widget_0_1.GetChild(num)).DestroyDataSource();
			}
			_datasource_Root_Lists = null;
		}
		_datasource_Root = null;
	}

	public void SetDataSource(EncyclopediaHomeVM dataSource)
	{
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
		if (propertyName == "Lists")
		{
			RefreshDataSource_datasource_Root_Lists(_datasource_Root.Lists);
		}
	}

	public void OnList_datasource_Root_ListsChanged(object sender, TaleWorlds.Library.ListChangedEventArgs e)
	{
		switch (e.ListChangedType)
		{
		case TaleWorlds.Library.ListChangedType.Reset:
		{
			for (int num = _widget_0_1.ChildCount - 1; num >= 0; num--)
			{
				Widget child3 = _widget_0_1.GetChild(num);
				((EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate)child3).OnBeforeRemovedChild(child3);
				Widget child4 = _widget_0_1.GetChild(num);
				((EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate)child4).SetDataSource(null);
				_widget_0_1.RemoveChild(child4);
			}
			break;
		}
		case TaleWorlds.Library.ListChangedType.Sorted:
		{
			for (int i = 0; i < _datasource_Root_Lists.Count; i++)
			{
				ListTypeVM bindingObject = _datasource_Root_Lists[i];
				_widget_0_1.FindChild((Widget widget) => widget.GetComponent<GeneratedWidgetData>().Data == bindingObject).SetSiblingIndex(i);
			}
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemAdded:
		{
			EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate = new EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate(base.Context);
			GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate);
			ListTypeVM dataSource = (ListTypeVM)(generatedWidgetData.Data = _datasource_Root_Lists[e.NewIndex]);
			encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.AddComponent(generatedWidgetData);
			_widget_0_1.AddChildAtIndex(encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate, e.NewIndex);
			encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.CreateWidgets();
			encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.SetIds();
			encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.SetAttributes();
			encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.SetDataSource(dataSource);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemBeforeDeleted:
		{
			Widget child2 = _widget_0_1.GetChild(e.NewIndex);
			((EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate)child2).OnBeforeRemovedChild(child2);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemDeleted:
		{
			Widget child = _widget_0_1.GetChild(e.NewIndex);
			((EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate)child).SetDataSource(null);
			_widget_0_1.RemoveChild(child);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemChanged:
			break;
		}
	}

	private void RefreshDataSource_datasource_Root(EncyclopediaHomeVM newDataSource)
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
			if (_datasource_Root_Lists != null)
			{
				_datasource_Root_Lists.ListChanged -= OnList_datasource_Root_ListsChanged;
				for (int num = _widget_0_1.ChildCount - 1; num >= 0; num--)
				{
					Widget child = _widget_0_1.GetChild(num);
					((EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate)child).OnBeforeRemovedChild(child);
					Widget child2 = _widget_0_1.GetChild(num);
					((EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate)child2).SetDataSource(null);
					_widget_0_1.RemoveChild(child2);
				}
				_datasource_Root_Lists = null;
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
		_datasource_Root_Lists = _datasource_Root.Lists;
		if (_datasource_Root_Lists != null)
		{
			_datasource_Root_Lists.ListChanged += OnList_datasource_Root_ListsChanged;
			for (int i = 0; i < _datasource_Root_Lists.Count; i++)
			{
				EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate = new EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate(base.Context);
				GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate);
				ListTypeVM dataSource = (ListTypeVM)(generatedWidgetData.Data = _datasource_Root_Lists[i]);
				encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.AddComponent(generatedWidgetData);
				_widget_0_1.AddChildAtIndex(encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate, i);
				encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.CreateWidgets();
				encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.SetIds();
				encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.SetAttributes();
				encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.SetDataSource(dataSource);
			}
		}
	}

	private void RefreshDataSource_datasource_Root_Lists(MBBindingList<ListTypeVM> newDataSource)
	{
		if (_datasource_Root_Lists != null)
		{
			_datasource_Root_Lists.ListChanged -= OnList_datasource_Root_ListsChanged;
			for (int num = _widget_0_1.ChildCount - 1; num >= 0; num--)
			{
				Widget child = _widget_0_1.GetChild(num);
				((EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate)child).OnBeforeRemovedChild(child);
				Widget child2 = _widget_0_1.GetChild(num);
				((EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate)child2).SetDataSource(null);
				_widget_0_1.RemoveChild(child2);
			}
			_datasource_Root_Lists = null;
		}
		_datasource_Root_Lists = newDataSource;
		_datasource_Root_Lists = _datasource_Root.Lists;
		if (_datasource_Root_Lists != null)
		{
			_datasource_Root_Lists.ListChanged += OnList_datasource_Root_ListsChanged;
			for (int i = 0; i < _datasource_Root_Lists.Count; i++)
			{
				EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate = new EncyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate(base.Context);
				GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate);
				ListTypeVM dataSource = (ListTypeVM)(generatedWidgetData.Data = _datasource_Root_Lists[i]);
				encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.AddComponent(generatedWidgetData);
				_widget_0_1.AddChildAtIndex(encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate, i);
				encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.CreateWidgets();
				encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.SetIds();
				encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.SetAttributes();
				encyclopediaHome__TaleWorlds_CampaignSystem_ViewModelCollection_Encyclopedia_EncyclopediaHomeVM_Dependency_1_ItemTemplate.SetDataSource(dataSource);
			}
		}
	}
}
