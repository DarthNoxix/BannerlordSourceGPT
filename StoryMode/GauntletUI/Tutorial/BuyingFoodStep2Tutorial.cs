using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class BuyingFoodStep2Tutorial : TutorialItemBase
{
	private bool _filterChangedToMisc;

	public BuyingFoodStep2Tutorial()
	{
		base.Type = "GetSuppliesTutorialStep2";
		base.Placement = TutorialItemVM.ItemPlacements.Right;
		base.HighlightedVisualElementID = "InventoryMicsFilter";
		base.MouseRequired = true;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _filterChangedToMisc;
	}

	public override bool IsConditionsMetForActivation()
	{
		if (TutorialHelper.BuyingFoodBaseConditions)
		{
			return TutorialHelper.CurrentContext == TutorialContexts.InventoryScreen;
		}
		return false;
	}

	public override void OnInventoryFilterChanged(InventoryFilterChangedEvent obj)
	{
		_filterChangedToMisc = obj.NewFilter == SPInventoryVM.Filters.Miscellaneous;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.InventoryScreen;
	}
}
