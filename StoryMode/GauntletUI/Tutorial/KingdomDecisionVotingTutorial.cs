using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class KingdomDecisionVotingTutorial : TutorialItemBase
{
	private bool _playerSelectedAnOption;

	public KingdomDecisionVotingTutorial()
	{
		base.Type = "KingdomDecisionVotingTutorial";
		base.Placement = TutorialItemVM.ItemPlacements.Left;
		base.HighlightedVisualElementID = "DecisionOptions";
		base.MouseRequired = false;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.KingdomScreen;
	}

	public override void OnPlayerSelectedAKingdomDecisionOption(PlayerSelectedAKingdomDecisionOptionEvent obj)
	{
		_playerSelectedAnOption = true;
	}

	public override bool IsConditionsMetForActivation()
	{
		return TutorialHelper.IsKingdomDecisionPanelActiveAndHasOptions;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _playerSelectedAnOption;
	}
}
