using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;

namespace StoryMode.GameComponents;

public class StoryModeBannerItemModel : DefaultBannerItemModel
{
	public override IEnumerable<ItemObject> GetPossibleRewardBannerItems()
	{
		if (!StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted)
		{
			return new List<ItemObject>();
		}
		return base.GetPossibleRewardBannerItems().WhereQ((ItemObject i) => !IsItemDragonBanner(i));
	}

	public override bool CanBannerBeUpdated(ItemObject item)
	{
		if (IsItemDragonBanner(item))
		{
			return false;
		}
		return base.CanBannerBeUpdated(item);
	}

	private bool IsItemDragonBanner(ItemObject item)
	{
		if (!(item.StringId == "dragon_banner") && !(item.StringId == "dragon_banner_center") && !(item.StringId == "dragon_banner_dragonhead"))
		{
			return item.StringId == "dragon_banner_handle";
		}
		return true;
	}
}
