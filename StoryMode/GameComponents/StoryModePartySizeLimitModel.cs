using System.Linq;
using StoryMode.Quests.SecondPhase.ConspiracyQuests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace StoryMode.GameComponents;

public class StoryModePartySizeLimitModel : DefaultPartySizeLimitModel
{
	public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false)
	{
		if (party.IsMobile)
		{
			QuestBase questBase = Campaign.Current.QuestManager.Quests.FirstOrDefault((QuestBase q) => !q.IsFinalized && q.GetType() == typeof(DisruptSupplyLinesConspiracyQuest));
			if (questBase != null && ((DisruptSupplyLinesConspiracyQuest)questBase).ConspiracyCaravan?.Party == party)
			{
				return new ExplainedNumber(((DisruptSupplyLinesConspiracyQuest)questBase).CaravanPartySize);
			}
		}
		return base.GetPartyMemberSizeLimit(party, includeDescriptions);
	}
}
