using StoryMode.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace StoryMode.GameComponents;

public class StoryModeCombatXpModel : DefaultCombatXpModel
{
	public override void GetXpFromHit(CharacterObject attackerTroop, CharacterObject captain, CharacterObject attackedTroop, PartyBase party, int damage, bool isFatal, MissionTypeEnum missionType, out int xpAmount)
	{
		if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTrainingField())
		{
			xpAmount = 0;
		}
		else
		{
			base.GetXpFromHit(attackerTroop, captain, attackedTroop, party, damage, isFatal, missionType, out xpAmount);
		}
	}
}
