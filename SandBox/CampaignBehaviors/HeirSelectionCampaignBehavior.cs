using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.CampaignBehaviors;

public class HeirSelectionCampaignBehavior : CampaignBehaviorBase
{
	private readonly ItemRoster _itemsThatWillBeInherited = new ItemRoster();

	private readonly ItemRoster _equipmentsThatWillBeInherited = new ItemRoster();

	public override void RegisterEvents()
	{
		CampaignEvents.OnBeforeMainCharacterDiedEvent.AddNonSerializedListener(this, OnBeforeMainCharacterDied);
		CampaignEvents.OnBeforePlayerCharacterChangedEvent.AddNonSerializedListener(this, OnBeforePlayerCharacterChanged);
		CampaignEvents.OnPlayerCharacterChangedEvent.AddNonSerializedListener(this, OnPlayerCharacterChanged);
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	private void OnBeforePlayerCharacterChanged(Hero oldPlayer, Hero newPlayer)
	{
		foreach (ItemRosterElement item in MobileParty.MainParty.ItemRoster)
		{
			_itemsThatWillBeInherited.Add(item);
		}
		for (int i = 0; i < 12; i++)
		{
			if (!oldPlayer.BattleEquipment[i].IsEmpty)
			{
				_equipmentsThatWillBeInherited.AddToCounts(oldPlayer.BattleEquipment[i], 1);
			}
			if (!oldPlayer.CivilianEquipment[i].IsEmpty)
			{
				_equipmentsThatWillBeInherited.AddToCounts(oldPlayer.CivilianEquipment[i], 1);
			}
		}
	}

	private void OnPlayerCharacterChanged(Hero oldPlayer, Hero newPlayer, MobileParty newMainParty, bool isMainPartyChanged)
	{
		if (isMainPartyChanged)
		{
			newMainParty.ItemRoster.Add(_itemsThatWillBeInherited);
		}
		newMainParty.ItemRoster.Add(_equipmentsThatWillBeInherited);
		_itemsThatWillBeInherited.Clear();
		_equipmentsThatWillBeInherited.Clear();
	}

	private void OnBeforeMainCharacterDied(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
	{
		Dictionary<Hero, int> heirApparents = Hero.MainHero.Clan.GetHeirApparents();
		if (heirApparents.Count == 0)
		{
			if (PlayerEncounter.Current != null && (PlayerEncounter.Battle == null || !PlayerEncounter.Battle.IsFinalized))
			{
				PlayerEncounter.Finish();
			}
			Dictionary<TroopRosterElement, int> dictionary = new Dictionary<TroopRosterElement, int>();
			foreach (TroopRosterElement item in MobileParty.MainParty.Party.MemberRoster.GetTroopRoster())
			{
				if (item.Character != CharacterObject.PlayerCharacter)
				{
					dictionary.Add(item, item.Number);
				}
			}
			foreach (KeyValuePair<TroopRosterElement, int> item2 in dictionary)
			{
				MobileParty.MainParty.Party.MemberRoster.RemoveTroop(item2.Key.Character, item2.Value);
			}
			Hero.MainHero.AddDeathMark(null, detail);
			CampaignEventDispatcher.Instance.OnGameOver();
			GameOverCleanup();
			ShowGameStatistics();
			Campaign.Current.OnGameOver();
			return;
		}
		if (Hero.MainHero.IsPrisoner)
		{
			EndCaptivityAction.ApplyByDeath(Hero.MainHero);
		}
		if (PlayerEncounter.Current != null && (PlayerEncounter.Battle == null || !PlayerEncounter.Battle.IsFinalized))
		{
			PlayerEncounter.Finish();
		}
		List<InquiryElement> list = new List<InquiryElement>();
		foreach (KeyValuePair<Hero, int> item3 in heirApparents.OrderBy((KeyValuePair<Hero, int> x) => x.Value))
		{
			TextObject textObject = new TextObject("{=!}{HERO.NAME}");
			StringHelpers.SetCharacterProperties("HERO", item3.Key.CharacterObject, textObject);
			textObject.SetTextVariable("POINT", item3.Value);
			string heroPropertiesHint = GetHeroPropertiesHint(item3.Key);
			list.Add(new InquiryElement(item3.Key, textObject.ToString(), new ImageIdentifier(CharacterCode.CreateFrom(item3.Key.CharacterObject)), isEnabled: true, heroPropertiesHint));
		}
		MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=iHYAEEfv}SELECT AN HEIR").ToString(), string.Empty, list, isExitShown: false, 1, 1, GameTexts.FindText("str_done").ToString(), string.Empty, OnHeirSelectionOver, null));
		Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
	}

	private static string GetHeroPropertiesHint(Hero hero)
	{
		GameTexts.SetVariable("newline", "\n");
		string content = hero.Name.ToString();
		TextObject textObject = GameTexts.FindText("str_STR1_space_STR2");
		textObject.SetTextVariable("STR1", GameTexts.FindText("str_enc_sf_age").ToString());
		textObject.SetTextVariable("STR2", ((int)hero.Age).ToString());
		string content2 = GameTexts.FindText("str_attributes").ToString();
		foreach (CharacterAttribute item in Attributes.All)
		{
			GameTexts.SetVariable("LEFT", item.Name.ToString());
			GameTexts.SetVariable("RIGHT", hero.GetAttributeValue(item));
			string content3 = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon").ToString();
			GameTexts.SetVariable("STR1", content2);
			GameTexts.SetVariable("STR2", content3);
			content2 = GameTexts.FindText("str_string_newline_string").ToString();
		}
		int num = 0;
		string content4 = GameTexts.FindText("str_skills").ToString();
		foreach (SkillObject item2 in Skills.All)
		{
			int skillValue = hero.GetSkillValue(item2);
			if (skillValue > 50)
			{
				GameTexts.SetVariable("LEFT", item2.Name.ToString());
				GameTexts.SetVariable("RIGHT", skillValue);
				string content5 = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon").ToString();
				GameTexts.SetVariable("STR1", content4);
				GameTexts.SetVariable("STR2", content5);
				content4 = GameTexts.FindText("str_string_newline_string").ToString();
				num++;
			}
		}
		GameTexts.SetVariable("STR1", content);
		GameTexts.SetVariable("STR2", textObject.ToString());
		string content6 = GameTexts.FindText("str_string_newline_string").ToString();
		GameTexts.SetVariable("newline", "\n \n");
		GameTexts.SetVariable("STR1", content6);
		GameTexts.SetVariable("STR2", content2);
		content6 = GameTexts.FindText("str_string_newline_string").ToString();
		if (num > 0)
		{
			GameTexts.SetVariable("STR1", content6);
			GameTexts.SetVariable("STR2", content4);
			content6 = GameTexts.FindText("str_string_newline_string").ToString();
		}
		GameTexts.SetVariable("newline", "\n");
		return content6;
	}

	private static void OnHeirSelectionOver(List<InquiryElement> element)
	{
		ApplyHeirSelectionAction.ApplyByDeath(element[0].Identifier as Hero);
	}

	private void ShowGameStatistics()
	{
		TextObject textObject = new TextObject("{=oxb2FVz5}Clan Destroyed");
		TextObject textObject2 = new TextObject("{=T2GbF6lK}With no suitable heirs, the {CLAN_NAME} clan is no more. Your journey ends here.");
		textObject2.SetTextVariable("CLAN_NAME", Clan.PlayerClan.Name);
		InformationManager.ShowInquiry(new InquiryData(affirmativeText: new TextObject("{=DM6luo3c}Continue").ToString(), titleText: textObject.ToString(), text: textObject2.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, negativeText: "", affirmativeAction: delegate
		{
			GameOverState gameState = Game.Current.GameStateManager.CreateState<GameOverState>(new object[1] { GameOverState.GameOverReason.ClanDestroyed });
			Game.Current.GameStateManager.CleanAndPushState(gameState);
		}, negativeAction: null), pauseGameActiveState: true);
	}

	private void GameOverCleanup()
	{
		GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, Hero.MainHero.Gold, disableNotification: true);
		Campaign.Current.MainParty.Party.ItemRoster.Clear();
		Campaign.Current.MainParty.Party.MemberRoster.Clear();
		Campaign.Current.MainParty.Party.PrisonRoster.Clear();
		Campaign.Current.MainParty.IsVisible = false;
		Campaign.Current.CameraFollowParty = null;
		Campaign.Current.MainParty.IsActive = false;
		PartyBase.MainParty.SetVisualAsDirty();
		if (Hero.MainHero.MapFaction.IsKingdomFaction && Clan.PlayerClan.Kingdom.Leader == Hero.MainHero)
		{
			DestroyKingdomAction.ApplyByKingdomLeaderDeath(Clan.PlayerClan.Kingdom);
		}
	}
}
