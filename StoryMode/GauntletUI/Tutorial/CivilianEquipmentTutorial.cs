using StoryMode.StoryModePhases;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class CivilianEquipmentTutorial : TutorialItemBase
{
	private bool _playerFilteredToCivilianEquipment;

	public CivilianEquipmentTutorial()
	{
		base.Type = "CivilianEquipment";
		base.Placement = TutorialItemVM.ItemPlacements.Right;
		base.HighlightedVisualElementID = "CivilianFilter";
		base.MouseRequired = true;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _playerFilteredToCivilianEquipment;
	}

	public override void OnInventoryEquipmentTypeChange(InventoryEquipmentTypeChangedEvent obj)
	{
		_playerFilteredToCivilianEquipment = !obj.IsCurrentlyWarSet;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.InventoryScreen;
	}

	public override bool IsConditionsMetForActivation()
	{
		if (TutorialPhase.Instance.IsCompleted)
		{
			return TutorialHelper.CurrentContext == TutorialContexts.InventoryScreen;
		}
		return false;
	}
}
