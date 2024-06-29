using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class UpgradingTroopsStep3Tutorial : TutorialItemBase
{
	private bool _playerUpgradedTroop;

	public UpgradingTroopsStep3Tutorial()
	{
		base.Type = "UpgradingTroopsStep3";
		base.Placement = TutorialItemVM.ItemPlacements.Right;
		base.HighlightedVisualElementID = "UpgradeButton";
		base.MouseRequired = true;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _playerUpgradedTroop;
	}

	public override void OnPlayerUpgradeTroop(CharacterObject arg1, CharacterObject arg2, int arg3)
	{
		_playerUpgradedTroop = true;
	}

	public override bool IsConditionsMetForActivation()
	{
		if (Hero.MainHero.Gold > 100 && TutorialHelper.CurrentContext == TutorialContexts.PartyScreen)
		{
			PartyScreenManager instance = PartyScreenManager.Instance;
			if (instance != null && instance.CurrentMode == PartyScreenMode.Normal)
			{
				return TutorialHelper.PlayerHasAnyUpgradeableTroop;
			}
		}
		return false;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.PartyScreen;
	}
}
