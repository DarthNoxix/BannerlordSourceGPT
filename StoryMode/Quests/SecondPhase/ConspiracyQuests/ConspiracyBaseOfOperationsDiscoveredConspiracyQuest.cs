using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.SecondPhase.ConspiracyQuests;

internal class ConspiracyBaseOfOperationsDiscoveredConspiracyQuest : ConspiracyQuestBase
{
	private const string AntiImperialHideoutBossStringId = "anti_imperial_conspiracy_boss";

	private const string ImperialHideoutBossStringId = "imperial_conspiracy_boss";

	private const int RaiderPartySize = 6;

	private const int RaiderPartyCount = 2;

	[SaveableField(1)]
	private readonly Settlement _hideout;

	private Settlement _baseLocation;

	private bool _dueledWithHideoutBoss;

	private bool _isSuccess;

	private bool _isDone;

	private float _conspiracyStrengthDecreaseAmount;

	[SaveableField(2)]
	private readonly List<MobileParty> _raiderParties;

	public override TextObject Title => new TextObject("{=3Pq58i2u}Conspiracy base of operations discovered");

	public override TextObject SideNotificationText
	{
		get
		{
			TextObject textObject = new TextObject("{=aY4zWYpg}You have have received an important message from {MENTOR.LINK}.");
			StringHelpers.SetCharacterProperties("MENTOR", base.Mentor.CharacterObject, textObject);
			return textObject;
		}
	}

	public override TextObject StartMessageLogFromMentor
	{
		get
		{
			TextObject textObject = new TextObject("{=XQrmVPKL}{PLAYER.LINK} I hope this letter finds you well. I have learned from a spy in {LOCATION_LINK} that our adversaries have set up a camp in its environs. She could not tell me what they plan to do, but if you raided the camp, stole some of their supplies, and brought it back to me, we could get some idea of their wicked intentions. Search around {LOCATION_LINK} to find the hideout.");
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject);
			textObject.SetTextVariable("LOCATION_LINK", _baseLocation.EncyclopediaLinkWithName);
			return textObject;
		}
	}

	public override TextObject StartLog
	{
		get
		{
			TextObject textObject = new TextObject("{=rTYNL1LB}{MENTOR.LINK} told you about a group of conspirators operating in a hideout in the vicinity of {LOCATION_LINK}. You should go there and raid the hideout with a small group of fighters and take the bandits by surprise.");
			StringHelpers.SetCharacterProperties("MENTOR", base.Mentor.CharacterObject, textObject);
			textObject.SetTextVariable("LOCATION_LINK", _baseLocation.EncyclopediaLinkWithName);
			return textObject;
		}
	}

	public override float ConspiracyStrengthDecreaseAmount => _conspiracyStrengthDecreaseAmount;

	private TextObject HideoutBossName
	{
		get
		{
			MobileParty mobileParty = _hideout.Parties.FirstOrDefault((MobileParty p) => p.IsBanditBossParty);
			if (mobileParty != null && mobileParty.MemberRoster.TotalManCount > 0)
			{
				return mobileParty.MemberRoster.GetCharacterAtIndex(0).Name;
			}
			return new TextObject("{=izCbZEZg}Conspiracy Commander{%Commander is male.}");
		}
	}

	private TextObject HideoutSpottedLog => new TextObject("{=nrdl5QaF}My spy spotted some conspirators at the camp, and some local bandits have joined them. My spy does not know if they are expecting an attack, so I implore you to be cautious and to be ready for anything. Needless to say, I'm sure you will send any documents you can find to me so I can study them. Go quickly and return safely.");

	private TextObject HideoutRemovedLog => new TextObject("{=cLZWjrZP}They have moved to another hiding place.");

	private TextObject NotDueledWithHideoutBossAndDefeatLog
	{
		get
		{
			TextObject textObject = new TextObject("{=nOLFHL3x}You and your men have defeated {BOSS_NAME} and the rest of the conspirators as {MENTOR.LINK} asked you to do.");
			StringHelpers.SetCharacterProperties("MENTOR", base.Mentor.CharacterObject, textObject);
			textObject.SetTextVariable("BOSS_NAME", HideoutBossName);
			return textObject;
		}
	}

	private TextObject NotDueledWithHideoutBossAndDefeatedLog
	{
		get
		{
			TextObject textObject = new TextObject("{=EV5ykPuT}You and your men were defeated by {BOSS_NAME} and his conspirators. Rest of your men finds your broken body among the bloodied pile of corpses. Yet you live to fight another day.");
			textObject.SetTextVariable("BOSS_NAME", HideoutBossName);
			return textObject;
		}
	}

	private TextObject DueledWithHideoutBossAndDefeatLog
	{
		get
		{
			TextObject textObject = new TextObject("{=LKiREaFZ}You have defeated {BOSS_NAME} in a fair duel his men the conspirators scatters and runs away in shame.");
			textObject.SetTextVariable("BOSS_NAME", HideoutBossName);
			return textObject;
		}
	}

	private TextObject DueledWithHideoutBossAndDefeatedLog
	{
		get
		{
			TextObject textObject = new TextObject("{=Uk7F483P}You were defeated by the {BOSS_NAME} in the duel. Your men takes your wounded body to the safety. As agreed, conspirators quickly leave and disappear without a trace.");
			textObject.SetTextVariable("BOSS_NAME", HideoutBossName);
			return textObject;
		}
	}

	public ConspiracyBaseOfOperationsDiscoveredConspiracyQuest(string questId, Hero questGiver)
		: base(questId, questGiver)
	{
		_raiderParties = new List<MobileParty>();
		_hideout = SelectHideout();
		if (_hideout.Hideout.IsSpotted)
		{
			AddLog(HideoutSpottedLog);
			AddTrackedObject(_hideout);
		}
		_baseLocation = SettlementHelper.FindNearestSettlement((Settlement p) => p.IsFortification, _hideout);
		_conspiracyStrengthDecreaseAmount = 0f;
		InitializeHideout();
		_isDone = false;
	}

	private Settlement SelectHideout()
	{
		Settlement settlement = SettlementHelper.FindRandomHideout((Settlement s) => s.Hideout.IsInfested && ((!StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine) ? StoryModeData.IsKingdomImperial(SettlementHelper.FindNearestFortification(null, s).OwnerClan.Kingdom) : (!StoryModeData.IsKingdomImperial(SettlementHelper.FindNearestFortification(null, s).OwnerClan.Kingdom))));
		if (settlement == null)
		{
			settlement = SettlementHelper.FindRandomHideout((Settlement s) => (!StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine) ? StoryModeData.IsKingdomImperial(SettlementHelper.FindNearestFortification(null, s).OwnerClan.Kingdom) : (!StoryModeData.IsKingdomImperial(SettlementHelper.FindNearestFortification(null, s).OwnerClan.Kingdom)));
			if (settlement == null)
			{
				settlement = SettlementHelper.FindRandomHideout((Settlement s) => s.Hideout.IsInfested);
				if (settlement == null)
				{
					settlement = SettlementHelper.FindRandomHideout();
				}
			}
		}
		if (!settlement.Hideout.IsInfested)
		{
			for (int i = 0; i < 2; i++)
			{
				if (!settlement.Hideout.IsInfested)
				{
					_raiderParties.Add(CreateRaiderParty(settlement, isBanditBossParty: false, i));
				}
			}
		}
		return settlement;
	}

	private MobileParty CreateRaiderParty(Settlement hideout, bool isBanditBossParty, int partyIndex)
	{
		MobileParty mobileParty = BanditPartyComponent.CreateBanditParty("conspiracy_discovered_quest_raider_party_" + partyIndex, hideout.OwnerClan, hideout.Hideout, isBanditBossParty);
		TroopRoster troopRoster = new TroopRoster(mobileParty.Party);
		CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>(hideout.Culture.StringId + "_bandit");
		troopRoster.AddToCounts(@object, 6 - troopRoster.TotalManCount);
		TroopRoster prisonerRoster = new TroopRoster(mobileParty.Party);
		mobileParty.InitializeMobilePartyAtPosition(troopRoster, prisonerRoster, hideout.Position2D);
		mobileParty.SetCustomName(new TextObject("{=u1Pkt4HC}Raiders"));
		mobileParty.ActualClan = hideout.OwnerClan;
		mobileParty.Position2D = hideout.Position2D;
		mobileParty.Party.SetVisualAsDirty();
		EnterSettlementAction.ApplyForParty(mobileParty, hideout);
		float totalStrength = mobileParty.Party.TotalStrength;
		int initialGold = (int)(1f * MBRandom.RandomFloat * 20f * totalStrength + 50f);
		mobileParty.InitializePartyTrade(initialGold);
		mobileParty.Ai.SetMoveGoToSettlement(hideout);
		EnterSettlementAction.ApplyForParty(mobileParty, hideout);
		mobileParty.SetPartyUsedByQuest(isActivelyUsed: true);
		return mobileParty;
	}

	protected override void InitializeQuestOnGameLoad()
	{
		_baseLocation = SettlementHelper.FindNearestFortification(null, _hideout);
		SetDialogs();
	}

	protected override void HourlyTick()
	{
	}

	private void InitializeHideout()
	{
		AddTrackedObject(_baseLocation);
	}

	private void ChangeHideoutParties()
	{
		PartyTemplateObject raiderTemplate = (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("conspiracy_anti_imperial_special_raider_party_template") : Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("conspiracy_imperial_special_raider_party_template"));
		foreach (MobileParty party in _hideout.Parties)
		{
			if (party.IsBandit)
			{
				party.SetCustomName(new TextObject("{=FRSas4xT}Conspiracy Troops"));
				party.SetPartyUsedByQuest(isActivelyUsed: true);
				if (party.IsBanditBossParty)
				{
					int troopCountLimit = party.MemberRoster.TotalManCount - 1;
					party.MemberRoster.Clear();
					DistributeConspiracyRaiderTroopsByLevel(raiderTemplate, party.Party, troopCountLimit);
					CharacterObject characterObject = (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? Campaign.Current.ObjectManager.GetObject<CharacterObject>("anti_imperial_conspiracy_boss") : Campaign.Current.ObjectManager.GetObject<CharacterObject>("imperial_conspiracy_boss"));
					characterObject.SetTransferableInPartyScreen(isTransferable: false);
					party.MemberRoster.AddToCounts(characterObject, 1, insertAtFront: true);
				}
				else
				{
					int totalManCount = party.MemberRoster.TotalManCount;
					party.MemberRoster.Clear();
					DistributeConspiracyRaiderTroopsByLevel(raiderTemplate, party.Party, totalManCount);
				}
			}
		}
	}

	protected override void RegisterEvents()
	{
		base.RegisterEvents();
		CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, OnGameMenuOpened);
		CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, OnMissionEnded);
		CampaignEvents.OnHideoutSpottedEvent.AddNonSerializedListener(this, OnHideoutSpotted);
	}

	private void OnGameMenuOpened(MenuCallbackArgs args)
	{
		if (Settlement.CurrentSettlement == _hideout && !_isDone)
		{
			MobileParty mobileParty = _hideout.Parties.FirstOrDefault((MobileParty p) => p.IsBanditBossParty);
			if (mobileParty != null && mobileParty.IsActive && (mobileParty.MemberRoster.TotalManCount <= 0 || (mobileParty.MemberRoster.GetCharacterAtIndex(0) != null && mobileParty.MemberRoster.GetCharacterAtIndex(0).StringId != (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? "anti_imperial_conspiracy_boss" : "imperial_conspiracy_boss"))))
			{
				ChangeHideoutParties();
			}
		}
		if (!_isDone)
		{
			return;
		}
		if (Hero.MainHero.IsPrisoner)
		{
			EndCaptivityAction.ApplyByPeace(Hero.MainHero);
		}
		foreach (MobileParty item in _hideout.Parties.ToList())
		{
			if (item.IsBandit)
			{
				DestroyPartyAction.Apply(null, item);
			}
		}
		if (_isSuccess)
		{
			CompleteQuestWithSuccess();
			return;
		}
		AddLog(HideoutRemovedLog);
		CompleteQuestWithFail();
	}

	private void OnMissionEnded(IMission mission)
	{
		if (Settlement.CurrentSettlement != _hideout || PlayerEncounter.Current == null)
		{
			return;
		}
		MapEvent playerMapEvent = MapEvent.PlayerMapEvent;
		if (playerMapEvent == null)
		{
			return;
		}
		if (playerMapEvent.WinningSide == playerMapEvent.PlayerSide)
		{
			if (_dueledWithHideoutBoss)
			{
				DueledWithHideoutBossAndDefeatedCaravan();
			}
			else
			{
				NotDueledWithHideoutBossAndDefeatedCaravan();
			}
			_isSuccess = true;
		}
		else
		{
			if (playerMapEvent.WinningSide != BattleSideEnum.None)
			{
				if (_dueledWithHideoutBoss)
				{
					DueledWithHideoutBossAndDefeatedByCaravan();
				}
				else
				{
					NotDueledWithHideoutBossAndDefeatedByCaravan();
				}
			}
			_isSuccess = false;
		}
		_isDone = true;
	}

	private void OnHideoutSpotted(PartyBase party, PartyBase hideoutParty)
	{
		if (party == PartyBase.MainParty && hideoutParty.Settlement == _hideout)
		{
			AddLog(HideoutSpottedLog);
			AddTrackedObject(_hideout);
		}
	}

	private void NotDueledWithHideoutBossAndDefeatedCaravan()
	{
		AddLog(NotDueledWithHideoutBossAndDefeatLog);
		_conspiracyStrengthDecreaseAmount = 50f;
	}

	private void NotDueledWithHideoutBossAndDefeatedByCaravan()
	{
		AddLog(NotDueledWithHideoutBossAndDefeatedLog);
	}

	private void DueledWithHideoutBossAndDefeatedCaravan()
	{
		AddLog(DueledWithHideoutBossAndDefeatLog);
		_conspiracyStrengthDecreaseAmount = 75f;
	}

	private void DueledWithHideoutBossAndDefeatedByCaravan()
	{
		AddLog(DueledWithHideoutBossAndDefeatedLog);
	}

	protected override void SetDialogs()
	{
		Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000015).NpcLine(new TextObject("{=UdHL9YZC}Well well, isn't this the famous {PLAYER.LINK}! You have been a thorn at our side for a while now. It's good that you are here now. It spares us from searching for you.[if:convo_confused_annoyed][ib:hip]")).Condition(bandit_hideout_boss_fight_start_on_condition)
			.BeginPlayerOptions()
			.PlayerOption(new TextObject("{=bZI82WMt}Let's get this over with! Men Attack!"))
			.NpcLine(new TextObject("{=H2FMIJmw}My wolves! Kill them![ib:aggressive][if:convo_furious]"))
			.Consequence(bandit_hideout_continue_battle_on_consequence)
			.CloseDialog()
			.PlayerOption(new TextObject("{=5PGokzW1}Talk is cheap. If you really want me that bad, I challenge you to a duel."))
			.NpcLine(new TextObject("{=karjORwI}To hell with that! Why would I want to duel with you?"))
			.PlayerLine(new TextObject("{=MU2O1SaZ}There is an army waiting for you outside."))
			.PlayerLine(new TextObject("{=tF6VeYaA}If you win, I promise my army won't crush you."))
			.PlayerLine(new TextObject("{=fUcwKbW8}If I win I will just kill you and let these poor excuses you call conspirators run away."))
			.NpcLine(new TextObject("{=C0xbbPqE}I will duel you for your insolence! Die dog![ib:warrior][if:convo_furious]"))
			.Consequence(bandit_hideout_start_duel_fight_on_consequence)
			.CloseDialog()
			.EndPlayerOptions()
			.CloseDialog(), this);
	}

	private bool bandit_hideout_boss_fight_start_on_condition()
	{
		PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
		StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter);
		if (encounteredParty == null || encounteredParty.IsMobile || encounteredParty.MapFaction == null || !encounteredParty.MapFaction.IsBanditFaction)
		{
			return false;
		}
		if (encounteredParty.IsSettlement && encounteredParty.Settlement.IsHideout && encounteredParty.Settlement == _hideout && Mission.Current != null && Mission.Current.GetMissionBehavior<HideoutMissionController>() != null && CharacterObject.OneToOneConversationCharacter != null)
		{
			return CharacterObject.OneToOneConversationCharacter.StringId == (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? "anti_imperial_conspiracy_boss" : "imperial_conspiracy_boss");
		}
		return false;
	}

	private void bandit_hideout_start_duel_fight_on_consequence()
	{
		_dueledWithHideoutBoss = true;
		Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightDuelMode;
	}

	private void bandit_hideout_continue_battle_on_consequence()
	{
		_dueledWithHideoutBoss = false;
		Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightBattleMode;
	}

	protected override void OnStartQuest()
	{
		base.OnStartQuest();
		SetDialogs();
	}

	protected override void OnCompleteWithSuccess()
	{
		base.OnCompleteWithSuccess();
		AddLog(new TextObject("{=6Dd3Pa07}You managed to thwart the conspiracy."));
		foreach (MobileParty raiderParty in _raiderParties)
		{
			if (raiderParty.IsActive)
			{
				DestroyPartyAction.Apply(null, raiderParty);
			}
		}
		_raiderParties.Clear();
	}

	protected override void OnTimedOut()
	{
		base.OnTimedOut();
		AddLog(new TextObject("{=S5Dn2K3m}You couldn't stop the conspiracy."));
	}

	internal static void AutoGeneratedStaticCollectObjectsConspiracyBaseOfOperationsDiscoveredConspiracyQuest(object o, List<object> collectedObjects)
	{
		((ConspiracyBaseOfOperationsDiscoveredConspiracyQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		collectedObjects.Add(_hideout);
		collectedObjects.Add(_raiderParties);
	}

	internal static object AutoGeneratedGetMemberValue_hideout(object o)
	{
		return ((ConspiracyBaseOfOperationsDiscoveredConspiracyQuest)o)._hideout;
	}

	internal static object AutoGeneratedGetMemberValue_raiderParties(object o)
	{
		return ((ConspiracyBaseOfOperationsDiscoveredConspiracyQuest)o)._raiderParties;
	}
}
