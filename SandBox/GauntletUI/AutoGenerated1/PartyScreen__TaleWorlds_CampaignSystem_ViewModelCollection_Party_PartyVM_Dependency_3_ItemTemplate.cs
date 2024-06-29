using System.ComponentModel;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace SandBox.GauntletUI.AutoGenerated1;

public class PartyScreen__TaleWorlds_CampaignSystem_ViewModelCollection_Party_PartyVM_Dependency_3_ItemTemplate : PartyScreen__TaleWorlds_CampaignSystem_ViewModelCollection_Party_PartyVM_Dependency_14_PartyTroopTuple__InheritedPrefab
{
	private PartyScreen__TaleWorlds_CampaignSystem_ViewModelCollection_Party_PartyVM_Dependency_14_PartyTroopTuple__InheritedPrefab _widget;

	private PartyCharacterVM _datasource_Root;

	public PartyScreen__TaleWorlds_CampaignSystem_ViewModelCollection_Party_PartyVM_Dependency_3_ItemTemplate(UIContext context)
		: base(context)
	{
	}

	private VisualDefinition CreateVisualDefinitionLeftMenu()
	{
		VisualDefinition visualDefinition = new VisualDefinition("LeftMenu", 0.45f, 0f, easeIn: true);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionXOffset = -5f
		});
		return visualDefinition;
	}

	private VisualDefinition CreateVisualDefinitionRightMenu()
	{
		VisualDefinition visualDefinition = new VisualDefinition("RightMenu", 0.45f, 0f, easeIn: true);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionXOffset = 5f
		});
		return visualDefinition;
	}

	private VisualDefinition CreateVisualDefinitionTopMenu()
	{
		VisualDefinition visualDefinition = new VisualDefinition("TopMenu", 0.45f, 0f, easeIn: true);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionYOffset = -6f
		});
		return visualDefinition;
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

	private VisualDefinition CreateVisualDefinitionFormationListPanel()
	{
		VisualDefinition visualDefinition = new VisualDefinition("FormationListPanel", 0.2f, 0f, easeIn: false);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			PositionYOffset = -522f
		});
		visualDefinition.AddVisualState(new VisualState("Closed")
		{
			PositionYOffset = -522f
		});
		visualDefinition.AddVisualState(new VisualState("Opened")
		{
			PositionYOffset = 0f
		});
		return visualDefinition;
	}

	private VisualDefinition CreateVisualDefinitionFormationSeperator()
	{
		VisualDefinition visualDefinition = new VisualDefinition("FormationSeperator", 0.01f, 0f, easeIn: false);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			SuggestedWidth = 0f
		});
		visualDefinition.AddVisualState(new VisualState("Closed")
		{
			SuggestedWidth = 26f
		});
		visualDefinition.AddVisualState(new VisualState("Opened")
		{
			SuggestedWidth = 93f
		});
		return visualDefinition;
	}

	private VisualDefinition CreateVisualDefinitionToggle()
	{
		VisualDefinition visualDefinition = new VisualDefinition("Toggle", 0.045f, 0f, easeIn: false);
		visualDefinition.AddVisualState(new VisualState("Default")
		{
			SuggestedWidth = 626f
		});
		visualDefinition.AddVisualState(new VisualState("Pressed")
		{
			SuggestedWidth = 619f
		});
		visualDefinition.AddVisualState(new VisualState("Selected")
		{
			SuggestedWidth = 626f
		});
		visualDefinition.AddVisualState(new VisualState("Hovered")
		{
			SuggestedWidth = 626f
		});
		return visualDefinition;
	}

	public override void CreateWidgets()
	{
		base.CreateWidgets();
		_widget = this;
	}

	public override void SetIds()
	{
		base.SetIds();
	}

	public override void SetAttributes()
	{
		base.SetAttributes();
		base.ButtonType = ButtonType.Radio;
	}

	public override void DestroyDataSource()
	{
		base.DestroyDataSource();
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
			_datasource_Root = null;
		}
	}

	public override void SetDataSource(PartyCharacterVM dataSource)
	{
		base.SetDataSource(dataSource);
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
	}

	private void RefreshDataSource_datasource_Root(PartyCharacterVM newDataSource)
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
		}
	}
}
