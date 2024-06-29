using System.ComponentModel;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;

namespace SandBox.GauntletUI.AutoGenerated0;

public class CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_3_CharacterCreationGainedProperties__DependendPrefab : ListPanel
{
	private ListPanel _widget;

	private NavigationScopeTargeter _widget_0;

	private NavigatableListPanel _widget_1;

	private CharacterCreationGainedPropertiesVM _datasource_Root;

	private MBBindingList<CharacterCreationGainGroupItemVM> _datasource_Root_GainGroups;

	public CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_3_CharacterCreationGainedProperties__DependendPrefab(UIContext context)
		: base(context)
	{
	}

	public void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new NavigationScopeTargeter(base.Context);
		_widget.AddChild(_widget_0);
		_widget_1 = new NavigatableListPanel(base.Context);
		_widget.AddChild(_widget_1);
	}

	public void SetIds()
	{
		_widget_1.Id = "GainedProperties";
	}

	public void SetAttributes()
	{
		base.WidthSizePolicy = SizePolicy.CoverChildren;
		base.HeightSizePolicy = SizePolicy.CoverChildren;
		base.VerticalAlignment = VerticalAlignment.Center;
		base.MarginLeft = 20f;
		base.DoNotUseCustomScaleAndChildren = true;
		base.StackLayout.LayoutMethod = LayoutMethod.VerticalBottomToTop;
		_widget_0.ScopeID = "GainedPropertiesScope";
		_widget_0.ScopeParent = _widget_1;
		_widget_0.ScopeMovements = GamepadNavigationTypes.Horizontal;
		_widget_0.AlternateScopeMovements = GamepadNavigationTypes.Vertical;
		_widget_0.AlternateMovementStepSize = 3;
		_widget_1.WidthSizePolicy = SizePolicy.CoverChildren;
		_widget_1.HeightSizePolicy = SizePolicy.CoverChildren;
		_widget_1.StackLayout.LayoutMethod = LayoutMethod.VerticalBottomToTop;
		_widget_1.StepSize = 100;
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
		if (_datasource_Root_GainGroups != null)
		{
			_datasource_Root_GainGroups.ListChanged -= OnList_datasource_Root_GainGroupsChanged;
			for (int num = _widget_1.ChildCount - 1; num >= 0; num--)
			{
				Widget child = _widget_1.GetChild(num);
				((CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate)child).OnBeforeRemovedChild(child);
				((CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate)_widget_1.GetChild(num)).DestroyDataSource();
			}
			_datasource_Root_GainGroups = null;
		}
		_datasource_Root = null;
	}

	public void SetDataSource(CharacterCreationGainedPropertiesVM dataSource)
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
		if (propertyName == "GainGroups")
		{
			RefreshDataSource_datasource_Root_GainGroups(_datasource_Root.GainGroups);
		}
	}

	public void OnList_datasource_Root_GainGroupsChanged(object sender, TaleWorlds.Library.ListChangedEventArgs e)
	{
		switch (e.ListChangedType)
		{
		case TaleWorlds.Library.ListChangedType.Reset:
		{
			for (int num = _widget_1.ChildCount - 1; num >= 0; num--)
			{
				Widget child3 = _widget_1.GetChild(num);
				((CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate)child3).OnBeforeRemovedChild(child3);
				Widget child4 = _widget_1.GetChild(num);
				((CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate)child4).SetDataSource(null);
				_widget_1.RemoveChild(child4);
			}
			break;
		}
		case TaleWorlds.Library.ListChangedType.Sorted:
		{
			for (int i = 0; i < _datasource_Root_GainGroups.Count; i++)
			{
				CharacterCreationGainGroupItemVM bindingObject = _datasource_Root_GainGroups[i];
				_widget_1.FindChild((Widget widget) => widget.GetComponent<GeneratedWidgetData>().Data == bindingObject).SetSiblingIndex(i);
			}
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemAdded:
		{
			CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate = new CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate(base.Context);
			GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate);
			CharacterCreationGainGroupItemVM dataSource = (CharacterCreationGainGroupItemVM)(generatedWidgetData.Data = _datasource_Root_GainGroups[e.NewIndex]);
			characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.AddComponent(generatedWidgetData);
			_widget_1.AddChildAtIndex(characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate, e.NewIndex);
			characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.CreateWidgets();
			characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.SetIds();
			characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.SetAttributes();
			characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.SetDataSource(dataSource);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemBeforeDeleted:
		{
			Widget child2 = _widget_1.GetChild(e.NewIndex);
			((CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate)child2).OnBeforeRemovedChild(child2);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemDeleted:
		{
			Widget child = _widget_1.GetChild(e.NewIndex);
			((CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate)child).SetDataSource(null);
			_widget_1.RemoveChild(child);
			break;
		}
		case TaleWorlds.Library.ListChangedType.ItemChanged:
			break;
		}
	}

	private void RefreshDataSource_datasource_Root(CharacterCreationGainedPropertiesVM newDataSource)
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
			if (_datasource_Root_GainGroups != null)
			{
				_datasource_Root_GainGroups.ListChanged -= OnList_datasource_Root_GainGroupsChanged;
				for (int num = _widget_1.ChildCount - 1; num >= 0; num--)
				{
					Widget child = _widget_1.GetChild(num);
					((CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate)child).OnBeforeRemovedChild(child);
					Widget child2 = _widget_1.GetChild(num);
					((CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate)child2).SetDataSource(null);
					_widget_1.RemoveChild(child2);
				}
				_datasource_Root_GainGroups = null;
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
		_datasource_Root_GainGroups = _datasource_Root.GainGroups;
		if (_datasource_Root_GainGroups != null)
		{
			_datasource_Root_GainGroups.ListChanged += OnList_datasource_Root_GainGroupsChanged;
			for (int i = 0; i < _datasource_Root_GainGroups.Count; i++)
			{
				CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate = new CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate(base.Context);
				GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate);
				CharacterCreationGainGroupItemVM dataSource = (CharacterCreationGainGroupItemVM)(generatedWidgetData.Data = _datasource_Root_GainGroups[i]);
				characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.AddComponent(generatedWidgetData);
				_widget_1.AddChildAtIndex(characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate, i);
				characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.CreateWidgets();
				characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.SetIds();
				characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.SetAttributes();
				characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.SetDataSource(dataSource);
			}
		}
	}

	private void RefreshDataSource_datasource_Root_GainGroups(MBBindingList<CharacterCreationGainGroupItemVM> newDataSource)
	{
		if (_datasource_Root_GainGroups != null)
		{
			_datasource_Root_GainGroups.ListChanged -= OnList_datasource_Root_GainGroupsChanged;
			for (int num = _widget_1.ChildCount - 1; num >= 0; num--)
			{
				Widget child = _widget_1.GetChild(num);
				((CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate)child).OnBeforeRemovedChild(child);
				Widget child2 = _widget_1.GetChild(num);
				((CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate)child2).SetDataSource(null);
				_widget_1.RemoveChild(child2);
			}
			_datasource_Root_GainGroups = null;
		}
		_datasource_Root_GainGroups = newDataSource;
		_datasource_Root_GainGroups = _datasource_Root.GainGroups;
		if (_datasource_Root_GainGroups != null)
		{
			_datasource_Root_GainGroups.ListChanged += OnList_datasource_Root_GainGroupsChanged;
			for (int i = 0; i < _datasource_Root_GainGroups.Count; i++)
			{
				CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate = new CharacterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate(base.Context);
				GeneratedWidgetData generatedWidgetData = new GeneratedWidgetData(characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate);
				CharacterCreationGainGroupItemVM dataSource = (CharacterCreationGainGroupItemVM)(generatedWidgetData.Data = _datasource_Root_GainGroups[i]);
				characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.AddComponent(generatedWidgetData);
				_widget_1.AddChildAtIndex(characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate, i);
				characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.CreateWidgets();
				characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.SetIds();
				characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.SetAttributes();
				characterCreationReviewStage__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterCreation_CharacterCreationReviewStageVM_Dependency_4_ItemTemplate.SetDataSource(dataSource);
			}
		}
	}
}
