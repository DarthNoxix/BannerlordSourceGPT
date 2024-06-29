using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;

namespace StoryMode.GameComponents.CampaignBehaviors;

public class ThirdPhaseCampaignBehavior : CampaignBehaviorBase
{
	private List<Tuple<Kingdom, Kingdom>> _warsToEnforcePeaceNextWeek = new List<Tuple<Kingdom, Kingdom>>();

	public override void RegisterEvents()
	{
		CampaignEvents.WarDeclared.AddNonSerializedListener(this, OnWarDeclared);
		CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, WeeklyTick);
		CampaignEvents.CanKingdomBeDiscontinuedEvent.AddNonSerializedListener(this, CanKingdomBeDiscontinued);
	}

	private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
	{
		if (faction1 is Kingdom kingdom && faction2 is Kingdom kingdom2 && StoryModeManager.Current.MainStoryLine.ThirdPhase != null)
		{
			MBReadOnlyList<Kingdom> oppositionKingdoms = StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms;
			MBReadOnlyList<Kingdom> allyKingdoms = StoryModeManager.Current.MainStoryLine.ThirdPhase.AllyKingdoms;
			if ((oppositionKingdoms.IndexOf(kingdom) >= 0 && oppositionKingdoms.IndexOf(kingdom2) >= 0) || (allyKingdoms.IndexOf(kingdom) >= 0 && allyKingdoms.IndexOf(kingdom2) >= 0))
			{
				_warsToEnforcePeaceNextWeek.Add(new Tuple<Kingdom, Kingdom>(kingdom, kingdom2));
			}
		}
	}

	private void WeeklyTick()
	{
		foreach (Tuple<Kingdom, Kingdom> item in new List<Tuple<Kingdom, Kingdom>>(_warsToEnforcePeaceNextWeek))
		{
			MakePeaceAction.Apply(item.Item1, item.Item2);
		}
	}

	private void CanKingdomBeDiscontinued(Kingdom kingdom, ref bool result)
	{
		if (StoryModeManager.Current.MainStoryLine.ThirdPhase != null && StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms.Contains(kingdom))
		{
			result = false;
		}
	}

	public override void SyncData(IDataStore dataStore)
	{
		dataStore.SyncData("_warsToEnforcePeaceNextWeek", ref _warsToEnforcePeaceNextWeek);
	}
}
