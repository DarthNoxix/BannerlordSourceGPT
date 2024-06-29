using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace StoryMode.GauntletUI.Tutorial;

public class OrderOfBattleTutorialStep3Tutorial : TutorialItemBase
{
	private bool _playerAssignedACaptainToFormationInOoB;

	public OrderOfBattleTutorialStep3Tutorial()
	{
		base.Type = "OrderOfBattleTutorialStep3";
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
		if (TutorialHelper.IsOrderOfBattleOpenAndReady && !TutorialHelper.IsPlayerEncounterLeader)
		{
			return TutorialHelper.CanPlayerAssignHimselfToFormation;
		}
		return false;
	}

	public override void OnOrderOfBattleHeroAssignedToFormation(OrderOfBattleHeroAssignedToFormationEvent obj)
	{
		if (!TutorialHelper.IsPlayerEncounterLeader)
		{
			_playerAssignedACaptainToFormationInOoB = obj.AssignedHero == Agent.Main;
		}
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _playerAssignedACaptainToFormationInOoB;
	}
}
