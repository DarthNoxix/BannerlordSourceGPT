using System.Collections.Generic;
using Helpers;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries;

public class PlayerRetiredLogEntry : LogEntry, IEncyclopediaLog
{
	[SaveableField(10)]
	public readonly Hero Retiree;

	internal static void AutoGeneratedStaticCollectObjectsPlayerRetiredLogEntry(object o, List<object> collectedObjects)
	{
		((PlayerRetiredLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		collectedObjects.Add(Retiree);
	}

	internal static object AutoGeneratedGetMemberValueRetiree(object o)
	{
		return ((PlayerRetiredLogEntry)o).Retiree;
	}

	public PlayerRetiredLogEntry(Hero retireeHero)
	{
		Retiree = retireeHero;
	}

	public override string ToString()
	{
		return GetEncyclopediaText().ToString();
	}

	public TextObject GetEncyclopediaText()
	{
		TextObject textObject = new TextObject("{=mg0yAzIb}{RETIREE.LINK} has retired.");
		StringHelpers.SetCharacterProperties("RETIREE", Retiree.CharacterObject, textObject);
		return textObject;
	}

	public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
	{
		return obj == Retiree;
	}
}