using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.ThirdPhase;

public class DefeatTheConspiracyQuestBehavior : CampaignBehaviorBase
{
	internal class OppositionData
	{
		[SaveableField(10)]
		public float InitialWarScore;

		[SaveableField(20)]
		public float ReinforcedWarScore;

		[SaveableField(30)]
		public JournalLog QuestLog;

		[SaveableField(40)]
		public CampaignTime LastPeaceOfferDate = CampaignTime.Zero;

		internal static void AutoGeneratedStaticCollectObjectsOppositionData(object o, List<object> collectedObjects)
		{
			((OppositionData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(QuestLog);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(LastPeaceOfferDate, collectedObjects);
		}

		internal static object AutoGeneratedGetMemberValueInitialWarScore(object o)
		{
			return ((OppositionData)o).InitialWarScore;
		}

		internal static object AutoGeneratedGetMemberValueReinforcedWarScore(object o)
		{
			return ((OppositionData)o).ReinforcedWarScore;
		}

		internal static object AutoGeneratedGetMemberValueQuestLog(object o)
		{
			return ((OppositionData)o).QuestLog;
		}

		internal static object AutoGeneratedGetMemberValueLastPeaceOfferDate(object o)
		{
			return ((OppositionData)o).LastPeaceOfferDate;
		}
	}

	public class DefeatTheConspiracyQuestBehaviorTypeDefiner : SaveableTypeDefiner
	{
		public DefeatTheConspiracyQuestBehaviorTypeDefiner()
			: base(16000)
		{
		}

		protected override void DefineClassTypes()
		{
			AddClassDefinition(typeof(OppositionData), 1);
			AddClassDefinition(typeof(DefeatTheConspiracyQuest), 2);
		}
	}

	public class DefeatTheConspiracyQuest : StoryModeQuestBase
	{
		private const int ProgressTrackerRange = 100;

		[SaveableField(100)]
		private Kingdom _oppositionKingdom;

		[SaveableField(110)]
		private OppositionData _oppositionData;

		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=Dfmg8stq}Eliminate {FACTION}");
				textObject.SetTextVariable("FACTION", _oppositionKingdom.Name);
				return textObject;
			}
		}

		private TextObject _questCanceledLogText => new TextObject("{=tVlZTOst}You have chosen a different path.");

		private TextObject _defeatOpposingKingdomsQuestLogText => new TextObject("{=ib2TKPUa}The ruler of the {FACTION} is leading the alliance against you. Defeat their armies or force them to make peace by capturing their settlements and destroying their parties to achieve victory.");

		private TextObject _defeatOpposingKingdomsProgressLogText => new TextObject("{=3Io5vmOk}War Progress with the {FACTION}");

		private TextObject _imperialKingdomDefeatedPopUpTitleText => new TextObject("{=XWL3XcIq}{FACTION} Defeated");

		private TextObject _imperialKingdomDefeatedPopUpDescriptionText => new TextObject("{=4SnDe0rA}The clans of the {FACTION} have defected to surrounding kingdoms as their leader has given up all hopes of restoring the Empire.");

		private TextObject _imperialKingdomDefeatedQuestLogText => new TextObject("{=OwcgxRXB}Weakened by the war, the {FACTION} has collapsed and its clans have defected to surrounding kingdoms.");

		private TextObject _antiImperialKingdomDefeatedPopUpTitleText => new TextObject("{=L3Qb6lbp}Peace Offer from the {FACTION}");

		private TextObject _antiImperialKingdomDefeatedPopUpKingDescriptionText => new TextObject("{=E87miqTI}Exhausted from the war, the clans of the {FACTION} offer to make peace with the {PLAYER_SUPPORTED_FACTION}.");

		private TextObject _antiImperialKingdomDefeatedPopUpSubjectDescriptionText => new TextObject("{=hGPdLssq}The ruler of the {PLAYER_SUPPORTED_FACTION} has accepted the peace offered by the war-ravaged {FACTION}.");

		private TextObject _antiImperialKingdomDefeatedQuestLogText => new TextObject("{=weS3DJKA}Weakened by war, the ruler of the {FACTION} offers to make peace with {PLAYER_FACTION}.");

		private TextObject _playerSupportedKingdomDestroyedLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=eH8z6Fws}Despite its Dragon Banner, the {PLAYER_SUPPORTED_KINGDOM} has been destroyed!");
				textObject.SetTextVariable("PLAYER_SUPPORTED_KINGDOM", StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		public DefeatTheConspiracyQuest(string questId, Kingdom oppositionKingdom)
			: base(questId, StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? StoryModeHeroes.ImperialMentor : StoryModeHeroes.AntiImperialMentor, CampaignTime.Never)
		{
			_oppositionKingdom = oppositionKingdom;
			SetDialogs();
			InitializeQuestOnCreation();
		}

		protected override void SetDialogs()
		{
		}

		protected override void InitializeQuestOnGameLoad()
		{
			SetDialogs();
		}

		protected override void HourlyTick()
		{
			UpdateWarProgressWithKingdom();
		}

		private void UpdateWarProgressWithKingdom()
		{
			float num = CalculateWarScoreForKingdom(_oppositionKingdom);
			float reinforcedWarScore = _oppositionData.ReinforcedWarScore;
			float num2 = _oppositionData.InitialWarScore / 2f;
			int progress = (int)TaleWorlds.Library.MathF.Clamp((reinforcedWarScore - num) / (reinforcedWarScore - num2) * 100f, -100f, 100f);
			_oppositionData.QuestLog.UpdateCurrentProgress(progress);
			if (num <= _oppositionData.InitialWarScore / 2f && _oppositionData.LastPeaceOfferDate.ElapsedDaysUntilNow >= 7f)
			{
				_oppositionData.LastPeaceOfferDate = CampaignTime.Now;
				InitializeKingdomDefeatedPopUp(_oppositionKingdom);
			}
		}

		private void InitializeKingdomDefeatedPopUp(Kingdom kingdom)
		{
			Kingdom playerSupportedKingdom = StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom;
			TextObject textObject = null;
			TextObject textObject2 = null;
			if (StoryModeData.IsKingdomImperial(kingdom))
			{
				textObject = _imperialKingdomDefeatedPopUpTitleText;
				textObject.SetTextVariable("FACTION", kingdom.Name);
				textObject2 = _imperialKingdomDefeatedPopUpDescriptionText;
				textObject2.SetTextVariable("FACTION", kingdom.Name);
				textObject2.SetTextVariable("PLAYER_SUPPORTED_FACTION", playerSupportedKingdom.Name);
				TextObject textObject3 = new TextObject("{=DM6luo3c}Continue");
				InformationManager.ShowInquiry(new InquiryData(textObject.ToString(), textObject2.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, textObject3.ToString(), "", delegate
				{
					OnKingdomDefeated(kingdom);
				}, null), pauseGameActiveState: true);
				return;
			}
			textObject = _antiImperialKingdomDefeatedPopUpTitleText;
			textObject.SetTextVariable("FACTION", kingdom.Name);
			if (playerSupportedKingdom.Leader == Hero.MainHero)
			{
				textObject2 = _antiImperialKingdomDefeatedPopUpKingDescriptionText;
				textObject2.SetTextVariable("FACTION", kingdom.Name);
				textObject2.SetTextVariable("PLAYER_SUPPORTED_FACTION", playerSupportedKingdom.Name);
				TextObject textObject4 = new TextObject("{=Y94H6XnK}Accept");
				TextObject textObject5 = new TextObject("{=cOgmdp9e}Decline");
				InformationManager.ShowInquiry(new InquiryData(textObject.ToString(), textObject2.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, textObject4.ToString(), textObject5.ToString(), delegate
				{
					OnKingdomDefeated(kingdom);
				}, delegate
				{
					OnKingdomDefeated(kingdom, makePeace: false);
				}), pauseGameActiveState: true);
			}
			else
			{
				textObject2 = _antiImperialKingdomDefeatedPopUpSubjectDescriptionText;
				textObject2.SetTextVariable("FACTION", kingdom.Name);
				textObject2.SetTextVariable("PLAYER_SUPPORTED_FACTION", playerSupportedKingdom.Name);
				TextObject textObject6 = new TextObject("{=DM6luo3c}Continue");
				InformationManager.ShowInquiry(new InquiryData(textObject.ToString(), textObject2.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, textObject6.ToString(), "", delegate
				{
					OnKingdomDefeated(kingdom);
				}, null), pauseGameActiveState: true);
			}
		}

		private void OnKingdomDefeated(Kingdom kingdom, bool makePeace = true)
		{
			if (_oppositionKingdom != kingdom)
			{
				return;
			}
			Kingdom playerSupportedKingdom = StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom;
			if (StoryModeData.IsKingdomImperial(kingdom))
			{
				StoryModeManager.Current.MainStoryLine.ThirdPhase.RemoveOppositionKingdom(kingdom);
				RemoveLog(_oppositionData.QuestLog);
				TextObject imperialKingdomDefeatedQuestLogText = _imperialKingdomDefeatedQuestLogText;
				imperialKingdomDefeatedQuestLogText.SetTextVariable("FACTION", kingdom.EncyclopediaLinkWithName);
				AddLog(imperialKingdomDefeatedQuestLogText);
				Kingdom kingdom2 = null;
				if (!StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms.IsEmpty())
				{
					kingdom2 = StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms.OrderBy((Kingdom t) => Campaign.Current.Models.MapDistanceModel.GetDistance(kingdom.FactionMidSettlement, t.FactionMidSettlement)).FirstOrDefault();
				}
				else
				{
					List<Kingdom> source = Kingdom.All.Where((Kingdom t) => !StoryModeData.IsKingdomImperial(t) && !t.IsEliminated && t != playerSupportedKingdom).ToList();
					if (source.IsEmpty())
					{
						kingdom2 = playerSupportedKingdom;
					}
					else
					{
						kingdom2 = source.OrderBy((Kingdom t) => Campaign.Current.Models.MapDistanceModel.GetDistance(kingdom.FactionMidSettlement, t.FactionMidSettlement)).FirstOrDefault();
						foreach (Settlement item in new List<Settlement>(kingdom.Settlements))
						{
							ChangeOwnerOfSettlementAction.ApplyByLeaveFaction(playerSupportedKingdom.Leader, item);
						}
					}
				}
				if (kingdom2 != null)
				{
					DefectClansOfKingdomToKingdom(kingdom, kingdom2);
				}
				else
				{
					Debug.FailedAssert("Kingdom to defect can't be found", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\Quests\\ThirdPhase\\DefeatTheConspiracyQuestBehavior.cs", "OnKingdomDefeated", 371);
				}
			}
			else
			{
				StoryModeManager.Current.MainStoryLine.ThirdPhase.RemoveOppositionKingdom(kingdom);
				RemoveLog(_oppositionData.QuestLog);
				TextObject antiImperialKingdomDefeatedQuestLogText = _antiImperialKingdomDefeatedQuestLogText;
				antiImperialKingdomDefeatedQuestLogText.SetTextVariable("FACTION", kingdom.EncyclopediaLinkWithName);
				antiImperialKingdomDefeatedQuestLogText.SetTextVariable("PLAYER_FACTION", playerSupportedKingdom.EncyclopediaLinkWithName);
				AddLog(antiImperialKingdomDefeatedQuestLogText);
				if (makePeace)
				{
					MakePeaceAction.Apply(playerSupportedKingdom, kingdom);
				}
				if (StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms.IsEmpty())
				{
					Kingdom kingdom3 = playerSupportedKingdom;
					foreach (Kingdom item2 in Kingdom.All.Where((Kingdom t) => !t.IsEliminated && StoryModeData.IsKingdomImperial(t)))
					{
						if (item2 != kingdom3)
						{
							DefectClansOfKingdomToKingdom(item2, kingdom3);
						}
					}
				}
			}
			CompleteQuestWithSuccess();
		}

		private void DefectClansOfKingdomToKingdom(Kingdom defectorKingdom, Kingdom targetKingdom)
		{
			foreach (Clan item in new List<Clan>(defectorKingdom.Clans))
			{
				if (item == Clan.PlayerClan)
				{
					ChangeKingdomAction.ApplyByLeaveKingdom(Clan.PlayerClan);
				}
				else if (item.IsUnderMercenaryService)
				{
					ChangeKingdomAction.ApplyByJoinFactionAsMercenary(item, targetKingdom, item.MercenaryAwardMultiplier, showNotification: false);
				}
				else
				{
					ChangeKingdomAction.ApplyByJoinToKingdom(item, targetKingdom, showNotification: false);
				}
			}
			DestroyKingdomAction.Apply(defectorKingdom);
		}

		private void OnKingdomDestroyed(Kingdom kingdom)
		{
			if (StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom == kingdom)
			{
				AddLog(_playerSupportedKingdomDestroyedLogText);
				CompleteQuestWithFail();
				TextObject textObject = new TextObject("{=atnUtXdO}The {KINGDOM_NAME} has been defeated. Your quest to restore the Empire has failed.");
				if (!StoryModeManager.Current.MainStoryLine.IsOnAntiImperialQuestLine)
				{
					textObject = new TextObject("{=r48aEAbq}The {KINGDOM_NAME} has been defeated. Your quest to destroy the Empire has failed.");
				}
				textObject.SetTextVariable("KINGDOM_NAME", StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom.Name);
				MBInformationManager.AddQuickInformation(textObject);
			}
		}

		private float CalculateWarScoreForKingdom(Kingdom kingdom)
		{
			float num = 0f;
			foreach (Settlement settlement in kingdom.Settlements)
			{
				num += GetWarScoreOfSettlement(settlement);
			}
			return num + kingdom.TotalStrength;
		}

		private float GetWarScoreOfSettlement(Settlement settlement)
		{
			float result = 0f;
			if (settlement.IsTown)
			{
				result = 3000f;
			}
			else if (settlement.IsCastle)
			{
				result = 1000f;
			}
			return result;
		}

		protected override void RegisterEvents()
		{
			CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, OnKingdomDestroyed);
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, OnCampaignQuestCompleted);
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, SettlementOwnerChanged);
			CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, OnClanChangedKingdom);
		}

		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			if (clan == Clan.PlayerClan && oldKingdom == StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom)
			{
				CompleteQuestWithCancel(_questCanceledLogText);
				StoryModeManager.Current.MainStoryLine.CancelSecondAndThirdPhase();
			}
		}

		private void OnCampaignQuestCompleted(QuestBase completedQuest, QuestCompleteDetails detail)
		{
			if (completedQuest != this && completedQuest is DefeatTheConspiracyQuest)
			{
				UpdateWarProgressWithKingdom();
				StoryModeManager.Current.MainStoryLine.ThirdPhase.CompleteThirdPhase(detail);
			}
		}

		private void SettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if (((newOwner != null && newOwner.MapFaction == _oppositionKingdom) || (oldOwner != null && oldOwner.MapFaction == _oppositionKingdom)) && GetWarScoreOfSettlement(settlement) > 0f)
			{
				UpdateWarProgressWithKingdom();
			}
		}

		protected override void OnStartQuest()
		{
			OppositionData oppositionData = new OppositionData();
			oppositionData.InitialWarScore = CalculateWarScoreForKingdom(_oppositionKingdom);
			oppositionData.ReinforcedWarScore = 0f;
			TextObject defeatOpposingKingdomsQuestLogText = _defeatOpposingKingdomsQuestLogText;
			TextObject defeatOpposingKingdomsProgressLogText = _defeatOpposingKingdomsProgressLogText;
			defeatOpposingKingdomsQuestLogText.SetTextVariable("FACTION", _oppositionKingdom.EncyclopediaLinkWithName);
			defeatOpposingKingdomsProgressLogText.SetTextVariable("FACTION", _oppositionKingdom.EncyclopediaLinkWithName);
			oppositionData.QuestLog = AddTwoWayContinuousLog(defeatOpposingKingdomsQuestLogText, defeatOpposingKingdomsProgressLogText, 0, 100);
			_oppositionData = oppositionData;
		}

		public void CalculateReinforcedWarScore()
		{
			_oppositionData.ReinforcedWarScore = CalculateWarScoreForKingdom(_oppositionKingdom);
		}

		internal static void AutoGeneratedStaticCollectObjectsDefeatTheConspiracyQuest(object o, List<object> collectedObjects)
		{
			((DefeatTheConspiracyQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(_oppositionKingdom);
			collectedObjects.Add(_oppositionData);
		}

		internal static object AutoGeneratedGetMemberValue_oppositionKingdom(object o)
		{
			return ((DefeatTheConspiracyQuest)o)._oppositionKingdom;
		}

		internal static object AutoGeneratedGetMemberValue_oppositionData(object o)
		{
			return ((DefeatTheConspiracyQuest)o)._oppositionData;
		}
	}

	private const int TroopCountPerNewLord = 200;

	private bool _hasBeenFinalized;

	private TextObject _empireUnitedText => new TextObject("{=zTYd6Qai}The Empire has been united under the {FACTION}!");

	private TextObject _empireDefeatedText => new TextObject("{=rCX81DDR}The Empire has been destroyed!");

	public override void RegisterEvents()
	{
		StoryModeEvents.OnConspiracyActivatedEvent.AddNonSerializedListener(this, OnConspiracyActivated);
		CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, HourlyTick);
	}

	private void OnConspiracyActivated()
	{
		InitializeFinalPhase();
	}

	private void HourlyTick()
	{
		if (StoryModeManager.Current.MainStoryLine.ThirdPhase == null || !StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms.IsEmpty() || _hasBeenFinalized)
		{
			return;
		}
		_hasBeenFinalized = true;
		TextObject textObject = new TextObject("{=R4Gqskgq}Victory");
		TextObject textObject2 = (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? _empireUnitedText : _empireDefeatedText);
		textObject2.SetTextVariable("FACTION", StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom.Name);
		InformationManager.ShowInquiry(new InquiryData(affirmativeText: new TextObject("{=DM6luo3c}Continue").ToString(), titleText: textObject.ToString(), text: textObject2.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, negativeText: "", affirmativeAction: delegate
		{
			string videoFile;
			string audioFile;
			string subtitleFile;
			if (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine)
			{
				videoFile = "imperial_outro";
				audioFile = "imperial_outro";
				subtitleFile = "imperial_outro";
			}
			else
			{
				videoFile = "anti_imperial_outro";
				audioFile = "anti_imperial_outro";
				subtitleFile = "anti_imperial_outro";
			}
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			PlayOutroCinematic(videoFile, audioFile, subtitleFile, ShowGameStatistics);
		}, negativeAction: null), pauseGameActiveState: true);
	}

	protected void InitializeFinalPhase()
	{
		bool isOnImperialQuestLine = StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine;
		List<Kingdom> list = Kingdom.All.Where((Kingdom t) => StoryModeData.IsKingdomImperial(t) && t != StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom && !t.IsEliminated).ToList();
		List<Kingdom> list2 = Kingdom.All.Where((Kingdom t) => !StoryModeData.IsKingdomImperial(t) && t != StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom && !t.IsEliminated).ToList();
		List<DefeatTheConspiracyQuest> list3 = new List<DefeatTheConspiracyQuest>();
		List<Kingdom> list4;
		List<Kingdom> list5;
		if (isOnImperialQuestLine)
		{
			if (list2.IsEmpty())
			{
				Kingdom randomElementWithPredicate = Kingdom.All.GetRandomElementWithPredicate((Kingdom t) => !StoryModeData.IsKingdomImperial(t));
				randomElementWithPredicate.ReactivateKingdom();
				list2.Add(randomElementWithPredicate);
			}
			list4 = list2.OrderByDescending((Kingdom t) => t.TotalStrength).Take(3).ToList();
			list5 = new List<Kingdom> { StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom };
			for (int i = 0; i < list2.Count; i++)
			{
				for (int j = i + 1; j < list2.Count; j++)
				{
					if (!FactionManager.IsAlliedWithFaction(list2[i], list2[j]))
					{
						MakePeaceAction.Apply(list2[i], list2[j]);
					}
				}
			}
		}
		else
		{
			if (list.IsEmpty())
			{
				Kingdom randomElementWithPredicate2 = Kingdom.All.GetRandomElementWithPredicate(StoryModeData.IsKingdomImperial);
				randomElementWithPredicate2.ReactivateKingdom();
				list.Add(randomElementWithPredicate2);
			}
			list4 = list.ToList();
			list5 = new List<Kingdom> { StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom };
			for (int l = 0; l < list.Count; l++)
			{
				for (int m = l + 1; m < list.Count; m++)
				{
					if (!FactionManager.IsAlliedWithFaction(list[l], list[m]))
					{
						MakePeaceAction.Apply(list[l], list[m]);
					}
				}
			}
		}
		foreach (Kingdom item in list5)
		{
			StoryModeManager.Current.MainStoryLine.ThirdPhase.AddAllyKingdom(item);
		}
		int num = 0;
		foreach (Kingdom item2 in list4)
		{
			StoryModeManager.Current.MainStoryLine.ThirdPhase.AddOppositionKingdom(item2);
			DefeatTheConspiracyQuest defeatTheConspiracyQuest = new DefeatTheConspiracyQuest("defeat_the_conspiracy_quest_" + num, item2);
			num++;
			defeatTheConspiracyQuest.StartQuest();
			list3.Add(defeatTheConspiracyQuest);
		}
		Dictionary<Kingdom, int> dictionary = new Dictionary<Kingdom, int>();
		float conspiracyStrength = StoryModeManager.Current.MainStoryLine.SecondPhase.ConspiracyStrength;
		List<float> list6 = new List<float> { 0.5f, 0.3f, 0.2f };
		int num2 = 0;
		for (int n = 0; n < list6.Count; n++)
		{
			if (num2 > list4.Count - 1)
			{
				num2 = 0;
			}
			Kingdom key = list4[num2];
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, 0);
			}
			dictionary[key] += (int)(conspiracyStrength * list6[n]);
			num2++;
		}
		foreach (KeyValuePair<Kingdom, int> item3 in dictionary)
		{
			Kingdom kingdom = item3.Key;
			MBList<MobileParty> mBList = new MBList<MobileParty>();
			foreach (WarPartyComponent warPartyComponent in kingdom.WarPartyComponents)
			{
				if (warPartyComponent.Leader != null)
				{
					mBList.Add(warPartyComponent.MobileParty);
				}
			}
			MBList<CharacterObject> e = CharacterHelper.GetTroopTree(kingdom.Culture.BasicTroop, 3f, 6f).ToMBList();
			int num3 = item3.Value / 2;
			int num4 = num3 / 200;
			List<Hero> list7 = new List<Hero>();
			Clan clan = null;
			for (int num5 = 0; num5 < num4; num5++)
			{
				if (clan == null || clan.Lords.Count >= clan.CommanderLimit)
				{
					TextObject clanName = NameGenerator.Current.GenerateClanName(kingdom.Culture, Settlement.All.GetRandomElementWithPredicate((Settlement t) => t.Culture == kingdom.Culture && t.IsVillage));
					clan = Clan.CreateClan("main_storyline_clan_" + clanName.ToString() + "_" + Clan.All.Count((Clan t) => t.Name == clanName));
					clan.InitializeClan(clanName, clanName, kingdom.Culture, Banner.CreateRandomClanBanner());
					clan.IsNoble = true;
				}
				MBList<Settlement> mBList2 = kingdom.Settlements.Where((Settlement t) => !t.IsUnderSiege && !t.IsUnderRaid).ToMBList();
				Settlement settlement = null;
				if (!mBList2.IsEmpty())
				{
					settlement = mBList2.GetRandomElementWithPredicate((Settlement t) => t.IsTown);
					if (settlement == null)
					{
						settlement = mBList2.GetRandomElement();
					}
				}
				Hero hero = HeroCreator.CreateSpecialHero(((mBList.Count > 0) ? mBList.GetRandomElement().LeaderHero : Hero.AllAliveHeroes.GetRandomElementWithPredicate((Hero t) => t.Occupation == Occupation.Lord)).CharacterObject, settlement, clan);
				GiveGoldAction.ApplyBetweenCharacters(null, hero, 2000, disableNotification: true);
				hero.ChangeState(Hero.CharacterStates.Active);
				if (clan.Leader == null)
				{
					clan.SetLeader(hero);
				}
				if (clan.Kingdom != kingdom)
				{
					ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom, showNotification: false);
				}
				if (clan.Kingdom.RulingClan == null || clan.Kingdom.RulingClan.IsEliminated)
				{
					ChangeRulingClanAction.Apply(clan.Kingdom, clan);
				}
				MobileParty mobileParty;
				if (settlement != null)
				{
					hero.BornSettlement = settlement;
					EnterSettlementAction.ApplyForCharacterOnly(hero, settlement);
					mobileParty = clan.CreateNewMobileParty(hero);
				}
				else
				{
					Vec2 vec = kingdom.RulingClan?.InitialPosition ?? kingdom.InitialPosition;
					if (!PartyBase.IsPositionOkForTraveling(vec))
					{
						vec = Campaign.Current.MapSceneWrapper.GetAccessiblePointNearPosition(vec, 50f);
					}
					mobileParty = clan.CreateNewMobilePartyAtPosition(hero, vec);
					hero.BornSettlement = Settlement.All.GetRandomElementWithPredicate((Settlement t) => t.IsTown || t.IsVillage);
				}
				mobileParty.MemberRoster.AddToCounts(e.GetRandomElement(), 200);
				mobileParty.ItemRoster.AddToCounts(DefaultItems.Grain, 10);
				mobileParty.ItemRoster.AddToCounts(DefaultItems.Meat, 5);
				list7.Add(hero);
			}
			if (mBList.IsEmpty())
			{
				mBList.AddRange(list7.Select((Hero t) => t.PartyBelongedTo));
			}
			if (mBList.IsEmpty())
			{
				continue;
			}
			int num6 = item3.Value - num3;
			int num7 = num3 - list7.Count * 200;
			int num8 = num6 + num7;
			int num9 = num8 / mBList.Count;
			int num10 = num8 % mBList.Count;
			if (num9 > 0)
			{
				foreach (MobileParty item4 in mBList)
				{
					for (int num11 = 0; num11 < num9; num11++)
					{
						item4.MemberRoster.AddToCounts(e.GetRandomElement(), 1);
					}
				}
			}
			if (num10 > 0)
			{
				MobileParty randomElement = mBList.GetRandomElement();
				for (int num12 = 0; num12 < num10; num12++)
				{
					randomElement.MemberRoster.AddToCounts(e.GetRandomElement(), 1);
				}
			}
		}
		for (int num13 = 0; num13 < list4.Count; num13++)
		{
			foreach (Clan clan2 in list4[num13].Clans)
			{
				clan2.UpdateStrength();
			}
			if (!list4[num13].IsAtWarWith(StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom))
			{
				ChangeRelationAction.ApplyPlayerRelation(list4[num13].Leader, -10);
				DeclareWarAction.ApplyByPlayerHostility(list4[num13], StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom);
			}
		}
		foreach (DefeatTheConspiracyQuest item5 in list3)
		{
			item5.CalculateReinforcedWarScore();
		}
		Hero leader = list4[list4.IndexOfMax((Kingdom k) => (int)k.TotalStrength)].Leader;
		SceneNotificationData data = ((!StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine) ? ((EmpireConspiracySupportsSceneNotificationItemBase)new ProEmpireConspiracyBeginsSceneNotificationItem(leader)) : ((EmpireConspiracySupportsSceneNotificationItemBase)new AntiEmpireConspiracyBeginsSceneNotificationItem(leader, StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms.ToList())));
		MBInformationManager.ShowSceneNotification(data);
	}

	public override void SyncData(IDataStore dataStore)
	{
		dataStore.SyncData("_hasBeenFinalized", ref _hasBeenFinalized);
	}

	private static void PlayOutroCinematic(string videoFile, string audioFile, string subtitleFile, Action onVideoFinished)
	{
		VideoPlaybackState videoPlaybackState = Game.Current.GameStateManager.CreateState<VideoPlaybackState>();
		string text = ModuleHelper.GetModuleFullPath("SandBox") + "Videos/CampaignOutro/";
		string videoPath = text + videoFile + ".ivf";
		string audioPath = text + audioFile + ".ogg";
		string subtitleFileBasePath = text + subtitleFile;
		videoPlaybackState.SetStartingParameters(videoPath, audioPath, subtitleFileBasePath);
		videoPlaybackState.SetOnVideoFinisedDelegate(onVideoFinished);
		Game.Current.GameStateManager.PushState(videoPlaybackState);
	}

	private void ShowGameStatistics()
	{
		GameOverState gameState = Game.Current.GameStateManager.CreateState<GameOverState>(new object[1] { GameOverState.GameOverReason.Victory });
		Game.Current.GameStateManager.PopState();
		Game.Current.GameStateManager.PushState(gameState);
	}
}
