using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;

namespace SandBox.View;

public static class SandBoxViewCheats
{
	[CommandLineFunctionality.CommandLineArgumentFunction("illumination", "global")]
	private static string TryGlobalIllumination(List<string> values)
	{
		string text = "";
		foreach (Settlement objectType in MBObjectManager.Instance.GetObjectTypeList<Settlement>())
		{
			if (objectType.Culture != null && objectType.MapFaction != null)
			{
				text = text + objectType.Position2D.x + "," + objectType.Position2D.y + ",";
				text += objectType.MapFaction.Color;
				text += "-";
			}
		}
		MapScreen obj = ScreenManager.TopScreen as MapScreen;
		MBMapScene.GetGlobalIlluminationOfString((Scene)typeof(MapScreen).GetField("_mapScene", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj), text);
		return "Success";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("remove_all_circle_notifications", "campaign")]
	public static string ClearAllCircleNotifications(List<string> strings)
	{
		if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
		{
			return CampaignCheats.ErrorType;
		}
		MapScreen.Instance.MapNotificationView.ResetNotifications();
		return "Cleared";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("set_custom_maximum_map_height", "campaign")]
	private static string SetCustomMaximumHeight(List<string> strings)
	{
		string result = $"Format is \"campaign.set_custom_maximum_map_height [MaxHeight]\".\n If the given number is below the current base maximum: {Campaign.MapMaximumHeight}, it won't be used.";
		if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
		{
			return CampaignCheats.ErrorType;
		}
		if (CampaignCheats.CheckHelp(strings))
		{
			return result;
		}
		if (CampaignCheats.CheckParameters(strings, 1) && int.TryParse(strings[0], out var result2))
		{
			Type typeFromHandle = typeof(MapCameraView);
			PropertyInfo property = typeFromHandle.GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic);
			MapCameraView mapCameraView = (MapCameraView)property.GetValue(null);
			typeFromHandle.GetField("_customMaximumCameraHeight", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(mapCameraView, (float)result2);
			property.SetValue(null, mapCameraView);
		}
		return "Success";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("focus_tournament", "campaign")]
	public static string FocusTournament(List<string> strings)
	{
		if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
		{
			return CampaignCheats.ErrorType;
		}
		if (CampaignCheats.CheckHelp(strings))
		{
			return "Format is \"campaign.focus_tournament\".";
		}
		Settlement settlement = Settlement.FindFirst((Settlement x) => x.IsTown && Campaign.Current.TournamentManager.GetTournamentGame(x.Town) != null);
		if (settlement == null)
		{
			return "There isn't any tournament right now.";
		}
		((MapCameraView)typeof(MapCameraView).GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
		settlement.Party.SetAsCameraFollowParty();
		return "Success";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("focus_hostile_army", "campaign")]
	public static string FocusHostileArmy(List<string> strings)
	{
		if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
		{
			return CampaignCheats.ErrorType;
		}
		if (CampaignCheats.CheckHelp(strings))
		{
			return "Format is \"campaign.focus_hostile_army\".";
		}
		Army army = null;
		foreach (Kingdom item in Kingdom.All)
		{
			if (item != Clan.PlayerClan.MapFaction && !item.Armies.IsEmpty() && item.IsAtWarWith(Clan.PlayerClan.MapFaction))
			{
				army = item.Armies.GetRandomElement();
			}
			if (army != null)
			{
				break;
			}
		}
		if (army == null)
		{
			return "There isn't any hostile army right now.";
		}
		((MapCameraView)typeof(MapCameraView).GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
		army.LeaderParty.Party.SetAsCameraFollowParty();
		return "Success";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("focus_mobile_party", "campaign")]
	public static string FocusMobileParty(List<string> strings)
	{
		if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
		{
			return CampaignCheats.ErrorType;
		}
		string text = "Format is \"campaign.focus_mobile_party [PartyName]\".";
		if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
		{
			return text;
		}
		string text2 = CampaignCheats.ConcatenateString(strings);
		foreach (MobileParty item in MobileParty.All)
		{
			if (string.Equals(text2.Replace(" ", ""), item.Name.ToString().Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
			{
				((MapCameraView)typeof(MapCameraView).GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
				item.Party.SetAsCameraFollowParty();
				return "Success";
			}
		}
		return "Party is not found: " + text2 + "\n" + text;
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("focus_hero", "campaign")]
	public static string FocusHero(List<string> strings)
	{
		if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
		{
			return CampaignCheats.ErrorType;
		}
		string text = "Format is \"campaign.focus_hero [HeroName]\".";
		if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
		{
			return text;
		}
		string text2 = CampaignCheats.ConcatenateString(strings);
		Hero hero = CampaignCheats.GetHero(text2);
		if (hero != null)
		{
			MapCameraView mapCameraView = (MapCameraView)typeof(MapCameraView).GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			if (hero.CurrentSettlement != null)
			{
				mapCameraView.SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
				hero.CurrentSettlement.Party.SetAsCameraFollowParty();
				return "Success";
			}
			if (hero.PartyBelongedTo != null)
			{
				mapCameraView.SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
				hero.PartyBelongedTo.Party.SetAsCameraFollowParty();
				return "Success";
			}
			if (hero.PartyBelongedToAsPrisoner != null)
			{
				mapCameraView.SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
				hero.PartyBelongedToAsPrisoner.SetAsCameraFollowParty();
				return "Success";
			}
			return "Party is not found: " + text2 + "\n" + text;
		}
		return "Hero is not found: " + text2 + "\n" + text;
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("focus_infested_hideout", "campaign")]
	public static string FocusInfestedHideout(List<string> strings)
	{
		if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
		{
			return CampaignCheats.ErrorType;
		}
		string text = "Format is \"campaign.focus_infested_hideout [Optional: Number of troops]\".";
		if (CampaignCheats.CheckHelp(strings))
		{
			return text;
		}
		MBList<Settlement> mBList = Settlement.All.Where((Settlement t) => t.IsHideout && t.Parties.Count > 0).ToMBList();
		Settlement settlement = null;
		if (mBList.IsEmpty())
		{
			return "All hideouts are empty!";
		}
		if (strings.Count > 0)
		{
			int troopCount = -1;
			int.TryParse(strings[0], out troopCount);
			if (troopCount == -1)
			{
				return "Incorrect input.\n" + text;
			}
			MBList<Settlement> mBList2 = mBList.Where((Settlement t) => t.Parties.Sum((MobileParty p) => p.MemberRoster.TotalManCount) >= troopCount).ToMBList();
			if (mBList2.IsEmpty())
			{
				return "Can't find suitable hideout.";
			}
			settlement = mBList2.GetRandomElement();
		}
		else
		{
			settlement = mBList.GetRandomElement();
		}
		if (settlement != null)
		{
			((MapCameraView)typeof(MapCameraView).GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
			settlement.Party.SetAsCameraFollowParty();
			return "Success";
		}
		return "Unable to find such a hideout.";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("focus_issue", "campaign")]
	public static string FocusIssues(List<string> strings)
	{
		if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
		{
			return CampaignCheats.ErrorType;
		}
		string text = "Format is \"campaign.focus_issue [IssueName]\".";
		if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
		{
			return text;
		}
		MapCameraView mapCameraView = (MapCameraView)typeof(MapCameraView).GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
		string text2 = CampaignCheats.ConcatenateString(strings);
		IssueBase issueBase = null;
		foreach (KeyValuePair<Hero, IssueBase> issue in Campaign.Current.IssueManager.Issues)
		{
			issue.Value.Title.ToString();
			if (issue.Value.Title.ToString().ToLower().Replace(" ", "")
				.Contains(text2.ToLower().Replace(" ", "")))
			{
				if (issue.Value.IssueSettlement != null)
				{
					issueBase = issue.Value;
					mapCameraView.SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
					issue.Value.IssueSettlement.Party.SetAsCameraFollowParty();
				}
				else if (issue.Value.IssueOwner.PartyBelongedTo != null)
				{
					issueBase = issue.Value;
					mapCameraView.SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
					issue.Value.IssueOwner.PartyBelongedTo?.Party.SetAsCameraFollowParty();
				}
				else if (issue.Value.IssueOwner.CurrentSettlement != null)
				{
					issueBase = issue.Value;
					mapCameraView.SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
					issue.Value.IssueOwner.CurrentSettlement.Party.SetAsCameraFollowParty();
				}
				if (issueBase != null)
				{
					return "Found issue: " + issueBase.Title.ToString() + ". Issue Owner: " + issueBase.IssueOwner.Name.ToString();
				}
			}
		}
		return "Issue Not Found.\n" + text;
	}
}
