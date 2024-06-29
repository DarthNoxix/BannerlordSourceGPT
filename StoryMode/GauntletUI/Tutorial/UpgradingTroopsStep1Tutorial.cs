using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class UpgradingTroopsStep1Tutorial : TutorialItemBase
{
	private bool _partyScreenOpened;

	private bool _playerUpgradedTroop;

	public UpgradingTroopsStep1Tutorial()
	{
		base.Type = "UpgradingTroopsStep1";
		base.Placement = TutorialItemVM.ItemPlacements.Right;
		base.HighlightedVisualElementID = "PartyButton";
		base.MouseRequired = true;
	}

	public override bool IsConditionsMetForCompletion()
	{
		if (!_partyScreenOpened)
		{
			return _playerUpgradedTroop;
		}
		return true;
	}

	public override void OnPlayerUpgradeTroop(CharacterObject arg1, CharacterObject arg2, int arg3)
	{
		_playerUpgradedTroop = true;
	}

	public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
	{
		_partyScreenOpened = obj.NewContext == TutorialContexts.PartyScreen;
	}

	public override bool IsConditionsMetForActivation()
	{
		if (Hero.MainHero.Gold < 100 || TutorialHelper.CurrentContext != TutorialContexts.MapWindow || TutorialHelper.PlayerIsInAnySettlement || !TutorialHelper.PlayerIsSafeOnMap)
		{
			return false;
		}
		return TutorialHelper.PlayerHasAnyUpgradeableTroop;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.MapWindow;
	}
}
