using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class CraftingOrdersTutorial : TutorialItemBase
{
	private bool _craftingCategorySelectionOpened;

	private bool _craftingOrderSelectionOpened;

	private bool _craftingOrderResultOpened;

	public CraftingOrdersTutorial()
	{
		base.Type = "CraftingOrdersTutorial";
		base.Placement = TutorialItemVM.ItemPlacements.Top;
		base.HighlightedVisualElementID = "CraftingOrderSelectionButton";
		base.MouseRequired = false;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.CraftingScreen;
	}

	public override void OnCraftingWeaponClassSelectionOpened(CraftingWeaponClassSelectionOpenedEvent obj)
	{
		_craftingCategorySelectionOpened = obj.IsOpen;
	}

	public override void OnCraftingOrderSelectionOpened(CraftingOrderSelectionOpenedEvent obj)
	{
		_craftingOrderSelectionOpened = obj.IsOpen;
	}

	public override void OnCraftingOnWeaponResultPopupOpened(CraftingWeaponResultPopupToggledEvent obj)
	{
		_craftingOrderResultOpened = obj.IsOpen;
	}

	public override bool IsConditionsMetForActivation()
	{
		if (!_craftingCategorySelectionOpened && !_craftingOrderResultOpened)
		{
			return TutorialHelper.IsCurrentTownHaveDoableCraftingOrder;
		}
		return false;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _craftingOrderSelectionOpened;
	}
}
