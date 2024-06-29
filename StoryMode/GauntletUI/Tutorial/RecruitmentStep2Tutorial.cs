using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class RecruitmentStep2Tutorial : TutorialItemBase
{
	private int _recruitedTroopCount;

	public RecruitmentStep2Tutorial()
	{
		base.Type = "RecruitmentTutorialStep2";
		base.Placement = TutorialItemVM.ItemPlacements.Right;
		base.HighlightedVisualElementID = "AvailableTroops";
		base.MouseRequired = true;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _recruitedTroopCount >= TutorialHelper.RecruitTroopAmount;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.RecruitmentWindow;
	}

	public override void OnPlayerRecruitedUnit(CharacterObject obj, int count)
	{
		_recruitedTroopCount += count;
	}

	public override bool IsConditionsMetForActivation()
	{
		if (TutorialHelper.PlayerCanRecruit)
		{
			return TutorialHelper.CurrentContext == TutorialContexts.RecruitmentWindow;
		}
		return false;
	}
}
