using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class ChoosingSkillFocusStep1Tutorial : TutorialItemBase
{
	private bool _characterWindowOpened;

	public ChoosingSkillFocusStep1Tutorial()
	{
		base.Type = "ChoosingSkillFocusStep1";
		base.Placement = TutorialItemVM.ItemPlacements.Right;
		base.HighlightedVisualElementID = "CharacterButton";
		base.MouseRequired = true;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _characterWindowOpened;
	}

	public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
	{
		_characterWindowOpened = obj.NewContext == TutorialContexts.CharacterScreen;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.MapWindow;
	}

	public override bool IsConditionsMetForActivation()
	{
		if (Settlement.CurrentSettlement == null && Hero.MainHero.HeroDeveloper.UnspentFocusPoints > 1 && (TutorialHelper.PlayerIsInAnySettlement || TutorialHelper.PlayerIsSafeOnMap))
		{
			return TutorialHelper.CurrentContext == TutorialContexts.MapWindow;
		}
		return false;
	}
}
