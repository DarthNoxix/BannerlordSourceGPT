using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace StoryMode.StoryModePhases;

public class FirstPhase
{
	public const int NeededBannerPieceCount = 3;

	public const int FirstPhaseDurationAsYears = 20;

	public static FirstPhase Instance => StoryModeManager.Current.MainStoryLine.FirstPhase;

	[SaveableProperty(1)]
	public int CollectedBannerPieceCount { get; private set; }

	[SaveableProperty(2)]
	public CampaignTime FirstPhaseStartTime { get; private set; }

	public CampaignTime FirstPhaseEndTime => CampaignTime.Years(20f) + FirstPhaseStartTime;

	public bool AllPiecesCollected => CollectedBannerPieceCount == 3;

	internal static void AutoGeneratedStaticCollectObjectsFirstPhase(object o, List<object> collectedObjects)
	{
		((FirstPhase)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(FirstPhaseStartTime, collectedObjects);
	}

	internal static object AutoGeneratedGetMemberValueCollectedBannerPieceCount(object o)
	{
		return ((FirstPhase)o).CollectedBannerPieceCount;
	}

	internal static object AutoGeneratedGetMemberValueFirstPhaseStartTime(object o)
	{
		return ((FirstPhase)o).FirstPhaseStartTime;
	}

	public FirstPhase()
	{
		CollectedBannerPieceCount = 0;
		FirstPhaseStartTime = CampaignTime.Now;
	}

	public void CollectBannerPiece()
	{
		CollectedBannerPieceCount++;
		ItemObject itemObject = null;
		if (CollectedBannerPieceCount == 1)
		{
			itemObject = Campaign.Current.ObjectManager.GetObject<ItemObject>("dragon_banner_center");
		}
		else if (CollectedBannerPieceCount == 2)
		{
			itemObject = Campaign.Current.ObjectManager.GetObject<ItemObject>("dragon_banner_dragonhead");
			MBInformationManager.ShowSceneNotification(new FindingSecondBannerPieceSceneNotificationItem(Hero.MainHero));
		}
		else if (CollectedBannerPieceCount == 3)
		{
			itemObject = Campaign.Current.ObjectManager.GetObject<ItemObject>("dragon_banner_handle");
			MBInformationManager.ShowSceneNotification(new FindingThirdBannerPieceSceneNotificationItem());
		}
		if (itemObject != null)
		{
			MobileParty.MainParty.ItemRoster.AddToCounts(new EquipmentElement(itemObject, null, null, isQuestItem: true), 1);
		}
		StoryModeEvents.Instance.OnBannerPieceCollected();
	}

	public void MergeDragonBanner()
	{
		MobileParty.MainParty.ItemRoster.AddToCounts(new EquipmentElement(Campaign.Current.ObjectManager.GetObject<ItemObject>("dragon_banner_center"), null, null, isQuestItem: true), -1);
		MobileParty.MainParty.ItemRoster.AddToCounts(new EquipmentElement(Campaign.Current.ObjectManager.GetObject<ItemObject>("dragon_banner_dragonhead"), null, null, isQuestItem: true), -1);
		MobileParty.MainParty.ItemRoster.AddToCounts(new EquipmentElement(Campaign.Current.ObjectManager.GetObject<ItemObject>("dragon_banner_handle"), null, null, isQuestItem: true), -1);
		MobileParty.MainParty.ItemRoster.AddToCounts(new EquipmentElement(Campaign.Current.ObjectManager.GetObject<ItemObject>("dragon_banner"), null, null, isQuestItem: true), 1);
	}
}
