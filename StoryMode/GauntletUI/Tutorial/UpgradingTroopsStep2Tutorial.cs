using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class UpgradingTroopsStep2Tutorial : TutorialItemBase
{
	private bool _playerUpgradedTroop;

	private bool _playerOpenedUpgradePopup;

	public UpgradingTroopsStep2Tutorial()
	{
		base.Type = "UpgradingTroopsStep2";
		base.Placement = TutorialItemVM.ItemPlacements.Left;
		base.HighlightedVisualElementID = "UpgradePopupButton";
		base.MouseRequired = true;
	}

	public override bool IsConditionsMetForCompletion()
	{
		if (!_playerUpgradedTroop)
		{
			return _playerOpenedUpgradePopup;
		}
		return true;
	}

	public override void OnPlayerToggledUpgradePopup(PlayerToggledUpgradePopupEvent obj)
	{
		if (obj.IsOpened)
		{
			_playerOpenedUpgradePopup = true;
		}
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
