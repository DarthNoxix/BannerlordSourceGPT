using Helpers;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class ChoosingPerkUpgradesStep3Tutorial : TutorialItemBase
{
	private bool _perkSelectedByPlayer;

	public ChoosingPerkUpgradesStep3Tutorial()
	{
		base.Type = "ChoosingPerkUpgradesStep3";
		base.Placement = TutorialItemVM.ItemPlacements.BottomRight;
		base.HighlightedVisualElementID = "PerkSelectionContainer";
		base.MouseRequired = true;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _perkSelectedByPlayer;
	}

	public override void OnPerkSelectedByPlayer(PerkSelectedByPlayerEvent obj)
	{
		_perkSelectedByPlayer = true;
	}

	public override bool IsConditionsMetForActivation()
	{
		if ((TutorialHelper.PlayerIsInAnySettlement || TutorialHelper.PlayerIsSafeOnMap) && PerkHelper.AvailablePerkCountOfHero(Hero.MainHero) > 1)
		{
			return TutorialHelper.CurrentContext == TutorialContexts.CharacterScreen;
		}
		return false;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.CharacterScreen;
	}
}
