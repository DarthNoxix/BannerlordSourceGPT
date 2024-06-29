using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class TalkToNotableTutorialStep1 : TutorialItemBase
{
	private bool _wantedCharacterPopupOpened;

	public TalkToNotableTutorialStep1()
	{
		base.Type = "TalkToNotableTutorialStep1";
		base.Placement = TutorialItemVM.ItemPlacements.TopRight;
		base.HighlightedVisualElementID = "ApplicableNotable";
		base.MouseRequired = true;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.MapWindow;
	}

	public override bool IsConditionsMetForActivation()
	{
		if (!TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == TutorialContexts.MapWindow)
		{
			return TutorialHelper.VillageMenuIsOpen;
		}
		return false;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _wantedCharacterPopupOpened;
	}

	public override void OnCharacterPortraitPopUpOpened(CharacterObject obj)
	{
		_wantedCharacterPopupOpened = obj != null && obj.HeroObject?.IsHeadman == true;
	}
}
