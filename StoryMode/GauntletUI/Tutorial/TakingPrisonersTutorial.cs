using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class TakingPrisonersTutorial : TutorialItemBase
{
	private bool _playerMovedOtherPrisonerTroop;

	public TakingPrisonersTutorial()
	{
		base.Type = "TakeAndRescuePrisonerTutorial";
		base.Placement = TutorialItemVM.ItemPlacements.Top;
		base.HighlightedVisualElementID = "TransferButtonOnlyOtherPrisoners";
		base.MouseRequired = true;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.PartyScreen;
	}

	public override bool IsConditionsMetForActivation()
	{
		PartyScreenManager instance = PartyScreenManager.Instance;
		if (instance != null && instance.CurrentMode == PartyScreenMode.Loot && GameStateManager.Current.ActiveState is PartyState partyState && partyState.PartyScreenLogic.PrisonerRosters[0].Count > 0)
		{
			return TutorialHelper.CurrentContext == TutorialContexts.InventoryScreen;
		}
		return false;
	}

	public override void OnPlayerMoveTroop(PlayerMoveTroopEvent obj)
	{
		base.OnPlayerMoveTroop(obj);
		if (obj.IsPrisoner && obj.ToSide == PartyScreenLogic.PartyRosterSide.Right && obj.Amount > 0)
		{
			_playerMovedOtherPrisonerTroop = true;
		}
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _playerMovedOtherPrisonerTroop;
	}
}
