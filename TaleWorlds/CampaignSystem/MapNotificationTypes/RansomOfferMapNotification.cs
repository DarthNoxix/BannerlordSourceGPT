using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapNotificationTypes;

public class RansomOfferMapNotification : InformationData
{
	[SaveableProperty(1)]
	public Hero CaptiveHero { get; private set; }

	public override TextObject TitleText => new TextObject("{=LUNcUV2I}Ransom Offer");

	public override string SoundEventPath => "event:/ui/notification/ransom_offer";

	internal static void AutoGeneratedStaticCollectObjectsRansomOfferMapNotification(object o, List<object> collectedObjects)
	{
		((RansomOfferMapNotification)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		collectedObjects.Add(CaptiveHero);
	}

	internal static object AutoGeneratedGetMemberValueCaptiveHero(object o)
	{
		return ((RansomOfferMapNotification)o).CaptiveHero;
	}

	public RansomOfferMapNotification(Hero captiveHero, TextObject descriptionText)
		: base(descriptionText)
	{
		CaptiveHero = captiveHero;
	}

	public RansomOfferMapNotification(TextObject description)
		: base(description)
	{
	}

	public override bool IsValid()
	{
		if (CaptiveHero.IsAlive)
		{
			return CaptiveHero.IsPrisoner;
		}
		return false;
	}
}