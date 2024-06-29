using StoryMode.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace StoryMode.GameComponents;

public class StoryModeGenericXpModel : DefaultGenericXpModel
{
	public override float GetXpMultiplier(Hero hero)
	{
		if (hero?.CurrentSettlement != null && hero.CurrentSettlement.IsTrainingField())
		{
			return 0f;
		}
		return base.GetXpMultiplier(hero);
	}
}
