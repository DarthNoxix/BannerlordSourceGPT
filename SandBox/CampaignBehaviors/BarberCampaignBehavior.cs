using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SandBox.CampaignBehaviors;

public class BarberCampaignBehavior : CampaignBehaviorBase, IFacegenCampaignBehavior, ICampaignBehavior
{
	private class BarberFaceGeneratorCustomFilter : IFaceGeneratorCustomFilter
	{
		private readonly int[] _haircutIndices;

		private readonly int[] _facialHairIndices;

		private readonly bool _defaultStages;

		public BarberFaceGeneratorCustomFilter(bool useDefaultStages, int[] haircutIndices, int[] faircutIndices)
		{
			_haircutIndices = haircutIndices;
			_facialHairIndices = faircutIndices;
			_defaultStages = useDefaultStages;
		}

		public int[] GetHaircutIndices(BasicCharacterObject character)
		{
			return _haircutIndices;
		}

		public int[] GetFacialHairIndices(BasicCharacterObject character)
		{
			return _facialHairIndices;
		}

		public FaceGeneratorStage[] GetAvailableStages()
		{
			if (!_defaultStages)
			{
				return new FaceGeneratorStage[1] { FaceGeneratorStage.Hair };
			}
			return new FaceGeneratorStage[7]
			{
				FaceGeneratorStage.Body,
				FaceGeneratorStage.Face,
				FaceGeneratorStage.Eyes,
				FaceGeneratorStage.Nose,
				FaceGeneratorStage.Mouth,
				FaceGeneratorStage.Hair,
				FaceGeneratorStage.Taint
			};
		}
	}

	private const int BarberCost = 100;

	private bool _isOpenedFromBarberDialogue;

	private StaticBodyProperties _previousBodyProperties;

	public override void RegisterEvents()
	{
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
		CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, LocationCharactersAreReadyToSpawn);
	}

	public override void SyncData(IDataStore store)
	{
	}

	private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
	{
		AddDialogs(campaignGameStarter);
	}

	private void AddDialogs(CampaignGameStarter campaignGameStarter)
	{
		campaignGameStarter.AddDialogLine("barber_start_talk_beggar", "start", "close_window", "{=pWzdxd7O}May the Heavens bless you, my poor {?PLAYER.GENDER}lady{?}fellow{\\?}, but I can't spare a coin right now.", InDisguiseSpeakingToBarber, InitializeBarberConversation);
		campaignGameStarter.AddDialogLine("barber_start_talk", "start", "barber_question1", "{=2aXYYNBG}Come to have your hair cut, {?PLAYER.GENDER}my lady{?}my lord{\\?}? A new look for a new day?", IsConversationAgentBarber, InitializeBarberConversation);
		campaignGameStarter.AddPlayerLine("player_accept_haircut", "barber_question1", "start_cut_token", "{=Q7wBRXtR}Yes, I have. ({GOLD_COST} {GOLD_ICON})", GivePlayerAHaircutCondition, GivePlayerAHaircut, 100, DoesPlayerHaveEnoughGold);
		campaignGameStarter.AddPlayerLine("player_refuse_haircut", "barber_question1", "no_haircut_conversation_token", "{=xPAAZAaI}My hair is fine as it is, thank you.", null, null);
		campaignGameStarter.AddDialogLine("barber_ask_if_done", "start_cut_token", "finish_cut_token", "{=M3K8wUOO}So... Does this please you, {?PLAYER.GENDER}my lady{?}my lord{\\?}?", null, null);
		campaignGameStarter.AddPlayerLine("player_done_with_haircut", "finish_cut_token", "finish_barber", "{=zTF4bJm0}Yes, it's fine.", null, null);
		campaignGameStarter.AddPlayerLine("player_not_done_with_haircut", "finish_cut_token", "start_cut_token", "{=BnoSOi3r}Actually...", GivePlayerAHaircutCondition, GivePlayerAHaircut, 100, DoesPlayerHaveEnoughGold);
		campaignGameStarter.AddDialogLine("barber_no_haircut_talk", "no_haircut_conversation_token", "close_window", "{=BusYGTrN}Excellent! Have a good day, then, {?PLAYER.GENDER}my lady{?}my lord{\\?}.", null, null);
		campaignGameStarter.AddDialogLine("barber_haircut_finished", "finish_barber", "player_had_a_haircut_token", "{=akqJbZpH}Marvellous! You cut a splendid appearance, {?PLAYER.GENDER}my lady{?}my lord{\\?}, if you don't mind my saying. Most splendid.", DidPlayerHaveAHaircut, ChargeThePlayer);
		campaignGameStarter.AddDialogLine("barber_haircut_no_change", "finish_barber", "player_did_not_cut_token", "{=yLIZlaS1}Very well. Do come back when you're ready, {?PLAYER.GENDER}my lady{?}my lord{\\?}.", DidPlayerNotHaveAHaircut, null);
		campaignGameStarter.AddPlayerLine("player_no_haircut_finish_talk", "player_did_not_cut_token", "close_window", "{=oPUVNuhN}I'll keep you in mind", null, null);
		campaignGameStarter.AddPlayerLine("player_haircut_finish_talk", "player_had_a_haircut_token", "close_window", "{=F9Xjbchh}Thank you.", null, null);
	}

	private bool InDisguiseSpeakingToBarber()
	{
		if (IsConversationAgentBarber())
		{
			return Campaign.Current.IsMainHeroDisguised;
		}
		return false;
	}

	private bool DoesPlayerHaveEnoughGold(out TextObject explanation)
	{
		if (Hero.MainHero.Gold < 100)
		{
			explanation = new TextObject("{=RYJdU43V}Not Enough Gold");
			return false;
		}
		explanation = TextObject.Empty;
		return true;
	}

	private void ChargeThePlayer()
	{
		GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, 100);
	}

	private bool DidPlayerNotHaveAHaircut()
	{
		return !DidPlayerHaveAHaircut();
	}

	private bool DidPlayerHaveAHaircut()
	{
		return Hero.MainHero.BodyProperties.StaticProperties != _previousBodyProperties;
	}

	private bool IsConversationAgentBarber()
	{
		return Settlement.CurrentSettlement?.Culture.Barber == CharacterObject.OneToOneConversationCharacter;
	}

	private bool GivePlayerAHaircutCondition()
	{
		MBTextManager.SetTextVariable("GOLD_COST", 100);
		return true;
	}

	private void GivePlayerAHaircut()
	{
		_isOpenedFromBarberDialogue = true;
		BarberState gameState = Game.Current.GameStateManager.CreateState<BarberState>(new object[2]
		{
			Hero.MainHero.CharacterObject,
			GetFaceGenFilter()
		});
		_isOpenedFromBarberDialogue = false;
		GameStateManager.Current.PushState(gameState);
	}

	private void InitializeBarberConversation()
	{
		_previousBodyProperties = Hero.MainHero.BodyProperties.StaticProperties;
	}

	private LocationCharacter CreateBarber(CultureObject culture, LocationCharacter.CharacterRelations relation)
	{
		CharacterObject barber = culture.Barber;
		Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(barber, out var minimumAge, out var maximumAge, "Barber");
		return new LocationCharacter(new AgentData(new SimpleAgentOrigin(barber)).Monster(FaceGen.GetMonsterWithSuffix(barber.Race, "_settlement_slow")).Age(MBRandom.RandomInt(minimumAge, maximumAge)), SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors, "sp_barber", fixedLocation: true, relation, null, useCivilianEquipment: true);
	}

	private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
	{
		Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("center");
		if (CampaignMission.Current.Location == locationWithId && Campaign.Current.IsDay && unusedUsablePointCount.TryGetValue("sp_merchant_notary", out var _))
		{
			locationWithId.AddLocationCharacters(CreateBarber, Settlement.CurrentSettlement.Culture, LocationCharacter.CharacterRelations.Neutral, 1);
		}
	}

	public IFaceGeneratorCustomFilter GetFaceGenFilter()
	{
		return new BarberFaceGeneratorCustomFilter(!_isOpenedFromBarberDialogue, GetAvailableHaircuts(), GetAvailableFacialHairs());
	}

	private int[] GetAvailableFacialHairs()
	{
		List<int> list = new List<int>();
		CultureCode cultureCode = ((_isOpenedFromBarberDialogue && Settlement.CurrentSettlement != null) ? Settlement.CurrentSettlement.Culture.GetCultureCode() : CultureCode.Invalid);
		if (!Hero.MainHero.IsFemale)
		{
			switch (cultureCode)
			{
			case CultureCode.Aserai:
				list.AddRange(new int[6] { 36, 37, 38, 39, 40, 41 });
				break;
			case CultureCode.Battania:
				list.AddRange(new int[24]
				{
					0, 1, 2, 4, 8, 10, 11, 12, 13, 14,
					15, 16, 17, 18, 19, 20, 21, 22, 24, 29,
					31, 32, 34, 35
				});
				break;
			case CultureCode.Empire:
				list.AddRange(new int[8] { 5, 6, 9, 12, 23, 24, 25, 26 });
				break;
			case CultureCode.Khuzait:
				list.AddRange(new int[6] { 0, 28, 29, 31, 32, 33 });
				break;
			case CultureCode.Sturgia:
				list.AddRange(new int[25]
				{
					1, 2, 4, 8, 9, 10, 11, 12, 13, 14,
					15, 16, 17, 18, 19, 20, 21, 22, 24, 25,
					26, 29, 32, 34, 35
				});
				break;
			case CultureCode.Vlandia:
				list.AddRange(new int[18]
				{
					1, 2, 3, 5, 6, 7, 8, 9, 10, 12,
					13, 14, 22, 23, 24, 25, 26, 32
				});
				break;
			}
			list.AddRange(new int[1]);
		}
		return list.Distinct().ToArray();
	}

	private int[] GetAvailableHaircuts()
	{
		List<int> list = new List<int>();
		CultureCode cultureCode = ((_isOpenedFromBarberDialogue && Settlement.CurrentSettlement != null) ? Settlement.CurrentSettlement.Culture.GetCultureCode() : CultureCode.Invalid);
		if (Hero.MainHero.IsFemale)
		{
			switch (cultureCode)
			{
			case CultureCode.Aserai:
				list.AddRange(new int[5] { 13, 17, 18, 19, 20 });
				break;
			case CultureCode.Battania:
				list.AddRange(new int[3] { 8, 9, 15 });
				break;
			case CultureCode.Empire:
				list.AddRange(new int[0]);
				break;
			case CultureCode.Khuzait:
				list.AddRange(new int[3] { 7, 12, 13 });
				break;
			case CultureCode.Sturgia:
				list.AddRange(new int[4] { 8, 9, 13, 15 });
				break;
			case CultureCode.Vlandia:
				list.AddRange(new int[0]);
				break;
			}
			list.AddRange(new int[12]
			{
				0, 1, 2, 3, 4, 5, 6, 10, 11, 14,
				16, 21
			});
		}
		else
		{
			switch (cultureCode)
			{
			case CultureCode.Aserai:
				list.AddRange(new int[11]
				{
					1, 2, 3, 4, 5, 21, 22, 23, 24, 25,
					26
				});
				break;
			case CultureCode.Battania:
				list.AddRange(new int[13]
				{
					1, 2, 3, 4, 5, 7, 8, 10, 16, 17,
					18, 19, 20
				});
				break;
			case CultureCode.Empire:
				list.AddRange(new int[6] { 1, 4, 5, 8, 14, 15 });
				break;
			case CultureCode.Khuzait:
				list.AddRange(new int[3] { 12, 17, 28 });
				break;
			case CultureCode.Sturgia:
				list.AddRange(new int[11]
				{
					1, 2, 3, 4, 5, 8, 10, 16, 18, 20,
					27
				});
				break;
			case CultureCode.Vlandia:
				list.AddRange(new int[8] { 1, 4, 5, 8, 11, 14, 15, 28 });
				break;
			}
			list.AddRange(new int[4] { 0, 6, 9, 13 });
		}
		return list.Distinct().ToArray();
	}
}
