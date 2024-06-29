using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.CampaignBehaviors;

public class DumpIntegrityCampaignBehavior : CampaignBehaviorBase
{
	private readonly List<KeyValuePair<string, string>> _saveIntegrityDumpInfo = new List<KeyValuePair<string, string>>();

	private readonly List<KeyValuePair<string, string>> _usedModulesDumpInfo = new List<KeyValuePair<string, string>>();

	private readonly List<KeyValuePair<string, string>> _usedVersionsDumpInfo = new List<KeyValuePair<string, string>>();

	public override void SyncData(IDataStore dataStore)
	{
		IsGameIntegrityAchieved(out var _);
		UpdateDumpInfo();
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnConfigChangedEvent.AddNonSerializedListener(this, OnConfigChanged);
		CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, OnNewGameCreatedPartialFollowUpEnd);
	}

	private void OnConfigChanged()
	{
		IsGameIntegrityAchieved(out var _);
		UpdateDumpInfo();
	}

	private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter campaignGameStarter)
	{
		IsGameIntegrityAchieved(out var _);
		UpdateDumpInfo();
	}

	private void UpdateDumpInfo()
	{
		_saveIntegrityDumpInfo.Clear();
		_usedModulesDumpInfo.Clear();
		_usedVersionsDumpInfo.Clear();
		if (Campaign.Current?.PreviouslyUsedModules != null && Campaign.Current.UsedGameVersions != null && Campaign.Current.NewGameVersion != null)
		{
			_saveIntegrityDumpInfo.Add(new KeyValuePair<string, string>("New Game Version", Campaign.Current.NewGameVersion));
			_saveIntegrityDumpInfo.Add(new KeyValuePair<string, string>("Has Used Cheats", (!CheckCheatUsage()).ToString()));
			_saveIntegrityDumpInfo.Add(new KeyValuePair<string, string>("Has Installed Unofficial Modules", (!CheckIfModulesAreDefault()).ToString()));
			_saveIntegrityDumpInfo.Add(new KeyValuePair<string, string>("Has Reverted to Older Versions", (!CheckIfVersionIntegrityIsAchieved()).ToString()));
			_saveIntegrityDumpInfo.Add(new KeyValuePair<string, string>("Game Integrity is Achieved", IsGameIntegrityAchieved(out var _).ToString()));
		}
		if (Campaign.Current?.PreviouslyUsedModules != null)
		{
			string[] moduleNames = SandBoxManager.Instance.ModuleManager.ModuleNames;
			foreach (string module in Campaign.Current.PreviouslyUsedModules)
			{
				bool flag = moduleNames.FindIndex((string x) => x == module) != -1;
				_usedModulesDumpInfo.Add(new KeyValuePair<string, string>(module, flag.ToString()));
			}
		}
		if (Campaign.Current?.UsedGameVersions != null && Campaign.Current.UsedGameVersions.Count > 0)
		{
			foreach (string usedGameVersion in Campaign.Current.UsedGameVersions)
			{
				_usedVersionsDumpInfo.Add(new KeyValuePair<string, string>(usedGameVersion, ""));
			}
		}
		SendDataToWatchdog();
	}

	private void SendDataToWatchdog()
	{
		foreach (KeyValuePair<string, string> item in _saveIntegrityDumpInfo)
		{
			Utilities.SetWatchdogValue("crash_tags.txt", "Campaign Dump Integrity", item.Key, item.Value);
		}
		foreach (KeyValuePair<string, string> item2 in _usedModulesDumpInfo)
		{
			Utilities.SetWatchdogValue("crash_tags.txt", "Used Mods", item2.Key, item2.Value);
		}
		foreach (KeyValuePair<string, string> item3 in _usedVersionsDumpInfo)
		{
			Utilities.SetWatchdogValue("crash_tags.txt", "Used Game Versions", item3.Key, item3.Value);
		}
	}

	public static bool IsGameIntegrityAchieved(out TextObject reason)
	{
		reason = TextObject.Empty;
		bool result = true;
		if (!CheckCheatUsage())
		{
			reason = new TextObject("{=sO8Zh3ZH}Achievements are disabled due to cheat usage.");
			result = false;
		}
		else if (!CheckIfModulesAreDefault())
		{
			reason = new TextObject("{=R0AbAxqX}Achievements are disabled due to unofficial modules.");
			result = false;
		}
		else if (!CheckIfVersionIntegrityIsAchieved())
		{
			reason = new TextObject("{=dt00CQCM}Achievements are disabled due to version downgrade.");
			result = false;
		}
		return result;
	}

	private static bool CheckIfVersionIntegrityIsAchieved()
	{
		for (int i = 0; i < Campaign.Current.UsedGameVersions.Count; i++)
		{
			if (i < Campaign.Current.UsedGameVersions.Count - 1 && ApplicationVersion.FromString(Campaign.Current.UsedGameVersions[i]) > ApplicationVersion.FromString(Campaign.Current.UsedGameVersions[i + 1]))
			{
				Debug.Print("Dump integrity is compromised due to version downgrade", 0, Debug.DebugColor.DarkRed);
				return false;
			}
		}
		return true;
	}

	private static bool CheckIfModulesAreDefault()
	{
		bool num = Campaign.Current.PreviouslyUsedModules.All((string x) => x.Equals("Native", StringComparison.OrdinalIgnoreCase) || x.Equals("SandBoxCore", StringComparison.OrdinalIgnoreCase) || x.Equals("CustomBattle", StringComparison.OrdinalIgnoreCase) || x.Equals("SandBox", StringComparison.OrdinalIgnoreCase) || x.Equals("Multiplayer", StringComparison.OrdinalIgnoreCase) || x.Equals("BirthAndDeath", StringComparison.OrdinalIgnoreCase) || x.Equals("StoryMode", StringComparison.OrdinalIgnoreCase));
		if (!num)
		{
			Debug.Print("Dump integrity is compromised due to non-default modules being used", 0, Debug.DebugColor.DarkRed);
		}
		return num;
	}

	private static bool CheckCheatUsage()
	{
		if (!Campaign.Current.EnabledCheatsBefore && Game.Current.CheatMode)
		{
			Campaign.Current.EnabledCheatsBefore = Game.Current.CheatMode;
		}
		if (Campaign.Current.EnabledCheatsBefore)
		{
			Debug.Print("Dump integrity is compromised due to cheat usage", 0, Debug.DebugColor.DarkRed);
		}
		return !Campaign.Current.EnabledCheatsBefore;
	}
}
