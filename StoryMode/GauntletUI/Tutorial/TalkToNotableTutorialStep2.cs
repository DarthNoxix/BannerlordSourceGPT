using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class TalkToNotableTutorialStep2 : TutorialItemBase
{
	private bool _hasTalkedToNotable;

	public TalkToNotableTutorialStep2()
	{
		base.Type = "TalkToNotableTutorialStep2";
		base.Placement = TutorialItemVM.ItemPlacements.Right;
		base.HighlightedVisualElementID = "OverlayTalkButton";
		base.MouseRequired = true;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _hasTalkedToNotable;
	}

	public override void OnPlayerStartTalkFromMenuOverlay(Hero hero)
	{
		_hasTalkedToNotable = hero.IsHeadman;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.MapWindow;
	}

	public override bool IsConditionsMetForActivation()
	{
		if (TutorialHelper.CurrentContext == TutorialContexts.MapWindow)
		{
			return TutorialHelper.IsCharacterPopUpWindowOpen;
		}
		return false;
	}
}
