using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial;

public class PartySpeedTutorial : TutorialItemBase
{
	private bool _isPlayerInspectedPartySpeed;

	private bool _isActivated;

	public PartySpeedTutorial()
	{
		base.Type = "PartySpeed";
		base.Placement = TutorialItemVM.ItemPlacements.Right;
		base.HighlightedVisualElementID = "PartySpeedLabel";
		base.MouseRequired = true;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _isPlayerInspectedPartySpeed;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.MapWindow;
	}

	public override void OnPlayerInspectedPartySpeed(PlayerInspectedPartySpeedEvent obj)
	{
		if (_isActivated)
		{
			_isPlayerInspectedPartySpeed = true;
		}
	}

	public override bool IsConditionsMetForActivation()
	{
		_isActivated = TutorialHelper.CurrentContext == TutorialContexts.MapWindow && Campaign.Current.CurrentMenuContext == null && MobileParty.MainParty.Ai.PartyMoveMode != 0 && MobileParty.MainParty.Speed < TutorialHelper.MaximumSpeedForPartyForSpeedTutorial && (float)MobileParty.MainParty.InventoryCapacity < MobileParty.MainParty.ItemRoster.TotalWeight;
		return _isActivated;
	}
}
