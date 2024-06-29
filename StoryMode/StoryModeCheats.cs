using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.Quests.SecondPhase;
using StoryMode.Quests.SecondPhase.ConspiracyQuests;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace StoryMode;

public static class StoryModeCheats
{
	public static bool CheckGameMode(out string message)
	{
		message = string.Empty;
		if (StoryModeManager.Current != null)
		{
			return false;
		}
		message = "Game mode is not correct!";
		return true;
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("activate_conspiracy_quest", "storymode")]
	public static string ActivateConspiracyQuest(List<string> strings)
	{
		if (CheckGameMode(out var message))
		{
			return message;
		}
		if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
		{
			return CampaignCheats.ErrorType;
		}
		if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
		{
			return "Format is \"storymode.activate_conspiracy_quest\".";
		}
		if (StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom == null)
		{
			return " Player supported kingdom doesn't exist.";
		}
		foreach (QuestBase item in Campaign.Current.QuestManager.Quests.Where((QuestBase t) => t is WeakenEmpireQuestBehavior.WeakenEmpireQuest || t is AssembleEmpireQuestBehavior.AssembleEmpireQuest).ToList())
		{
			item.CompleteQuestWithCancel();
		}
		StoryModeManager.Current.MainStoryLine.CompleteSecondPhase();
		return "Success";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("add_family_members", "storymode")]
	public static string AddFamilyMembers(List<string> strings)
	{
		if (CheckGameMode(out var message))
		{
			return message;
		}
		foreach (Hero item in new List<Hero>
		{
			StoryModeHeroes.LittleBrother,
			StoryModeHeroes.ElderBrother,
			StoryModeHeroes.LittleSister
		})
		{
			AddHeroToPartyAction.Apply(item, MobileParty.MainParty);
			item.Clan = Clan.PlayerClan;
		}
		return "Success";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("weaken_kingdom", "storymode")]
	public static string WeakenKingdom(List<string> strings)
	{
		if (CheckGameMode(out var message))
		{
			return message;
		}
		if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
		{
			return CampaignCheats.ErrorType;
		}
		string text = "Format is \"storymode.weaken_kingdom [KingdomName]\".";
		if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
		{
			return text;
		}
		string text2 = CampaignCheats.ConcatenateString(strings);
		Kingdom kingdom = null;
		foreach (Kingdom item in Kingdom.All)
		{
			if (item.Name.ToString().Replace(" ", "").Equals(text2.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
			{
				kingdom = item;
				break;
			}
			if (text2.Length >= 2 && item.Name.ToString().ToLower().Substring(0, 2)
				.Equals(text2.ToLower().Substring(0, 2)))
			{
				kingdom = item;
				break;
			}
		}
		if (kingdom != null)
		{
			foreach (Settlement item2 in kingdom.Settlements.Where((Settlement t) => t.IsTown || t.IsCastle).Take(3).ToList())
			{
				ChangeOwnerOfSettlementAction.ApplyByDefault(Hero.MainHero, item2);
			}
			foreach (MobileParty item3 in kingdom.AllParties.Where((MobileParty t) => t.MapEvent == null))
			{
				foreach (TroopRosterElement item4 in item3.MemberRoster.GetTroopRoster())
				{
					item3.MemberRoster.RemoveTroop(item4.Character, item3.MemberRoster.GetTroopCount(item4.Character) / 2);
				}
			}
			return "Success";
		}
		return "Kingdom is not found\n" + text;
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("reinforce_kingdom", "storymode")]
	public static string ReinforceKingdom(List<string> strings)
	{
		if (CheckGameMode(out var message))
		{
			return message;
		}
		if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
		{
			return CampaignCheats.ErrorType;
		}
		string text = "Format is \"storymode.reinforce_kingdom [KingdomName]\".";
		if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
		{
			return text;
		}
		string text2 = CampaignCheats.ConcatenateString(strings);
		Kingdom kingdom = null;
		foreach (Kingdom item in Kingdom.All)
		{
			if (item.Name.ToString().Replace(" ", "").Equals(text2.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
			{
				kingdom = item;
				break;
			}
		}
		if (kingdom != null)
		{
			foreach (Settlement item2 in Settlement.All.Where((Settlement t) => (t.IsTown || t.IsCastle) && t.MapFaction != kingdom).Take(3).ToList())
			{
				ChangeOwnerOfSettlementAction.ApplyByDefault(kingdom.Leader, item2);
			}
			foreach (MobileParty item3 in kingdom.AllParties.Where((MobileParty t) => t.MapEvent == null))
			{
				foreach (TroopRosterElement item4 in item3.MemberRoster.GetTroopRoster())
				{
					item3.MemberRoster.AddToCounts(item4.Character, 200);
				}
			}
			return "Success";
		}
		return "Kingdom is not found\n" + text;
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("start_conspiracy_quest_destroy_raiders", "storymode")]
	public static string StartDestroyRaidersConspiracyQuest(List<string> strings)
	{
		string ErrorType = "";
		if (!CampaignCheats.CheckCheatUsage(ref ErrorType))
		{
			return ErrorType;
		}
		new DestroyRaidersConspiracyQuest("cheat_quest", StoryModeHeroes.ImperialMentor).StartQuest();
		return "Success";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("start_next_second_phase_quest", "storymode")]
	public static string SecondPhaseStartNextQuest(List<string> strings)
	{
		string ErrorType = "";
		if (!CampaignCheats.CheckCheatUsage(ref ErrorType))
		{
			return ErrorType;
		}
		if (SecondPhase.Instance != null)
		{
			SecondPhase.Instance.CreateNextConspiracyQuest();
			return "Success";
		}
		return "Second phase not found.";
	}
}
