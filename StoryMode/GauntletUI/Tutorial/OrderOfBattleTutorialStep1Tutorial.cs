using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace StoryMode.GauntletUI.Tutorial;

public class OrderOfBattleTutorialStep1Tutorial : TutorialItemBase
{
	private bool _playerAssignedACaptainToFormationInOoB;

	public OrderOfBattleTutorialStep1Tutorial()
	{
		base.Type = "OrderOfBattleTutorialStep1";
		base.Placement = TutorialItemVM.ItemPlacements.Center;
		base.HighlightedVisualElementID = "AssignCaptain";
		base.MouseRequired = false;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.Mission;
	}

	public override bool IsConditionsMetForActivation()
	{
		if (TutorialHelper.IsOrderOfBattleOpenAndReady)
		{
			return TutorialHelper.IsPlayerEncounterLeader;
		}
		return false;
	}

	public override void OnOrderOfBattleHeroAssignedToFormation(OrderOfBattleHeroAssignedToFormationEvent obj)
	{
		_playerAssignedACaptainToFormationInOoB = true;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _playerAssignedACaptainToFormationInOoB;
	}
}
