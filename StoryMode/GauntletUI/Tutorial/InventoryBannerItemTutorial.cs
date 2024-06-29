using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class InventoryBannerItemTutorial : TutorialItemBase
{
	private bool _inspectedOtherBannerItem;

	public InventoryBannerItemTutorial()
	{
		base.Type = "InventoryBannerItemTutorial";
		base.Placement = TutorialItemVM.ItemPlacements.Center;
		base.HighlightedVisualElementID = "InventoryOtherBannerItems";
		base.MouseRequired = false;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.InventoryScreen;
	}

	public override void OnInventoryItemInspected(InventoryItemInspectedEvent obj)
	{
		if (obj.Item.EquipmentElement.Item.IsBannerItem && obj.ItemSide == InventoryLogic.InventorySide.OtherInventory)
		{
			_inspectedOtherBannerItem = true;
		}
	}

	public override bool IsConditionsMetForActivation()
	{
		return TutorialHelper.CurrentInventoryScreenIncludesBannerItem;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _inspectedOtherBannerItem;
	}
}
