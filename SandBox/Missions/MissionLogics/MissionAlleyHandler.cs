using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics;

public class MissionAlleyHandler : MissionLogic
{
	private const float ConstantForInitiatingConversation = 5f;

	private static Vec3 _fightPosition = Vec3.Invalid;

	private Dictionary<Agent, AgentNavigator> _rivalThugAgentsAndAgentNavigators;

	private const int DistanceForEndingAlleyFight = 20;

	private const int GuardAgentSafeZone = 10;

	private static List<Agent> _guardAgents;

	private bool _conversationTriggeredWithRivalThug;

	private MissionFightHandler _missionFightHandler;

	public override void OnRenderingStarted()
	{
		foreach (Agent agent in base.Mission.Agents)
		{
			if (agent.IsHuman)
			{
				CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
				if (component?.AgentNavigator?.MemberOfAlley != null && component.AgentNavigator.MemberOfAlley.Owner != Hero.MainHero && !_rivalThugAgentsAndAgentNavigators.ContainsKey(agent))
				{
					_rivalThugAgentsAndAgentNavigators.Add(agent, component.AgentNavigator);
				}
			}
		}
	}

	public override void OnMissionTick(float dt)
	{
		if (Mission.Current.Mode == MissionMode.Battle)
		{
			EndFightIfPlayerIsFarAwayOrNearGuard();
		}
		else if (MBRandom.RandomFloat < dt * 10f)
		{
			CheckAndTriggerConversationWithRivalThug();
		}
	}

	private void CheckAndTriggerConversationWithRivalThug()
	{
		if (_conversationTriggeredWithRivalThug || Campaign.Current.ConversationManager.IsConversationFlowActive || Agent.Main == null)
		{
			return;
		}
		foreach (KeyValuePair<Agent, AgentNavigator> rivalThugAgentsAndAgentNavigator in _rivalThugAgentsAndAgentNavigators)
		{
			if (rivalThugAgentsAndAgentNavigator.Key.IsActive())
			{
				Agent key = rivalThugAgentsAndAgentNavigator.Key;
				if (key.GetTrackDistanceToMainAgent() < 5f && rivalThugAgentsAndAgentNavigator.Value.CanSeeAgent(Agent.Main))
				{
					Mission.Current.GetMissionBehavior<MissionConversationLogic>().StartConversation(key, setActionsInstantly: false);
					_conversationTriggeredWithRivalThug = true;
					break;
				}
			}
		}
	}

	public override void AfterStart()
	{
		_guardAgents = new List<Agent>();
		_rivalThugAgentsAndAgentNavigators = new Dictionary<Agent, AgentNavigator>();
		_fightPosition = Vec3.Invalid;
		_missionFightHandler = Mission.Current.GetMissionBehavior<MissionFightHandler>();
	}

	private void EndFightIfPlayerIsFarAwayOrNearGuard()
	{
		if (Agent.Main == null)
		{
			return;
		}
		bool flag = false;
		foreach (Agent guardAgent in _guardAgents)
		{
			if ((Agent.Main.Position - guardAgent.Position).Length <= 10f)
			{
				flag = true;
				break;
			}
		}
		if (_fightPosition != Vec3.Invalid && (Agent.Main.Position - _fightPosition).Length >= 20f)
		{
			flag = true;
		}
		if (flag)
		{
			EndFight();
		}
	}

	private (bool, string) CanPlayerOccupyTheCurrentAlley()
	{
		TextObject empty = TextObject.Empty;
		if (!Settlement.CurrentSettlement.Alleys.All((Alley x) => x.Owner != Hero.MainHero))
		{
			empty = new TextObject("{=ribkM9dl}You already own another alley in the settlement.");
			return (false, empty.ToString());
		}
		if (!Campaign.Current.Models.AlleyModel.GetClanMembersAndAvailabilityDetailsForLeadingAnAlley(CampaignMission.Current.LastVisitedAlley).Any(((Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail) x) => x.Item2 == DefaultAlleyModel.AlleyMemberAvailabilityDetail.Available || x.Item2 == DefaultAlleyModel.AlleyMemberAvailabilityDetail.AvailableWithDelay))
		{
			empty = new TextObject("{=hnhKJYbx}You don't have any suitable clan members to assign this alley. ({ROGUERY_SKILL} skill {NEEDED_SKILL_LEVEL} or higher, {TRAIT_NAME} trait {MAX_TRAIT_AMOUNT} or lower)");
			empty.SetTextVariable("ROGUERY_SKILL", DefaultSkills.Roguery.Name);
			empty.SetTextVariable("NEEDED_SKILL_LEVEL", 30);
			empty.SetTextVariable("TRAIT_NAME", DefaultTraits.Mercy.Name);
			empty.SetTextVariable("MAX_TRAIT_AMOUNT", 0);
			return (false, empty.ToString());
		}
		if (MobileParty.MainParty.MemberRoster.TotalRegulars < Campaign.Current.Models.AlleyModel.MinimumTroopCountInPlayerOwnedAlley)
		{
			empty = new TextObject("{=zLnqZdIK}You don't have enough troops to assign this alley. (Needed at least {NEEDED_TROOP_NUMBER})");
			empty.SetTextVariable("NEEDED_TROOP_NUMBER", Campaign.Current.Models.AlleyModel.MinimumTroopCountInPlayerOwnedAlley);
			return (false, empty.ToString());
		}
		return (true, null);
	}

	private void EndFight()
	{
		_missionFightHandler.EndFight();
		foreach (Agent guardAgent in _guardAgents)
		{
			guardAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().GetBehavior<FightBehavior>().IsActive = false;
		}
		_guardAgents.Clear();
		Mission.Current.SetMissionMode(MissionMode.StartUp, atStart: false);
	}

	private void OnTakeOverTheAlley()
	{
		AlleyHelper.CreateMultiSelectionInquiryForSelectingClanMemberToAlley(CampaignMission.Current.LastVisitedAlley, OnCompanionSelectedForNewAlley, OnCompanionSelectionCancel);
	}

	private void OnCompanionSelectionCancel(List<InquiryElement> obj)
	{
		OnLeaveItEmpty();
	}

	private void OnCompanionSelectedForNewAlley(List<InquiryElement> companion)
	{
		CharacterObject character = companion.First().Identifier as CharacterObject;
		TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
		troopRoster.AddToCounts(character, 1);
		AlleyHelper.OpenScreenForManagingAlley(troopRoster, OnPartyScreenDoneClicked, new TextObject("{=s8dsW6m0}New Alley"), OnPartyScreenCancel);
	}

	private void OnPartyScreenCancel()
	{
		OnLeaveItEmpty();
	}

	public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
	{
		if (affectedAgent.IsHuman && affectorAgent != null && affectorAgent == Agent.Main && affectorAgent.IsHuman && affectedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
		{
			affectedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>().GetBehavior<TalkBehavior>()?.Disable();
			if (!affectedAgent.IsEnemyOf(affectorAgent) && affectedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.MemberOfAlley != null)
			{
				StartCommonAreaBattle(affectedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.MemberOfAlley);
			}
		}
	}

	private bool OnPartyScreenDoneClicked(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, FlattenedTroopRoster takenPrisonerRoster, FlattenedTroopRoster releasedPrisonerRoster, bool isForced, PartyBase leftParty, PartyBase rightParty)
	{
		CampaignEventDispatcher.Instance.OnAlleyOccupiedByPlayer(CampaignMission.Current.LastVisitedAlley, leftMemberRoster);
		return true;
	}

	public void StartCommonAreaBattle(Alley alley)
	{
		_guardAgents.Clear();
		_conversationTriggeredWithRivalThug = true;
		List<Agent> accompanyingAgents = new List<Agent>();
		foreach (Agent agent in Mission.Current.Agents)
		{
			LocationCharacter locationCharacter = LocationComplex.Current.FindCharacter(agent);
			AccompanyingCharacter accompanyingCharacter = PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(locationCharacter);
			CharacterObject characterObject = (CharacterObject)agent.Character;
			if (accompanyingCharacter != null && accompanyingCharacter.IsFollowingPlayerAtMissionStart)
			{
				accompanyingAgents.Add(agent);
			}
			else if (characterObject != null && (characterObject.Occupation == Occupation.Guard || characterObject.Occupation == Occupation.Soldier))
			{
				_guardAgents.Add(agent);
			}
		}
		List<Agent> playerSideAgents = Mission.Current.Agents.Where((Agent agent) => agent.IsHuman && agent.Character.IsHero && (agent.IsPlayerControlled || accompanyingAgents.Contains(agent))).ToList();
		List<Agent> opponentSideAgents = Mission.Current.Agents.Where((Agent agent) => agent.IsHuman && agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null && agent.GetComponent<CampaignAgentComponent>().AgentNavigator.MemberOfAlley == alley).ToList();
		_fightPosition = Agent.Main.Position;
		Mission.Current.GetMissionBehavior<MissionFightHandler>().StartCustomFight(playerSideAgents, opponentSideAgents, dropWeapons: false, isItemUseDisabled: false, OnAlleyFightEnd);
	}

	private void OnLeaveItEmpty()
	{
		CampaignEventDispatcher.Instance.OnAlleyClearedByPlayer(CampaignMission.Current.LastVisitedAlley);
	}

	private void OnAlleyFightEnd(bool isPlayerSideWon)
	{
		if (isPlayerSideWon)
		{
			TextObject textObject = new TextObject("{=4QfQBi2k}Alley fight won");
			TextObject textObject2 = new TextObject("{=8SK2BZum}You have cleared an alley which belonged to a gang leader. Now, you can either take it over for your own benefit or leave it empty to help the town. To own an alley, you will need to assign a suitable clan member and some troops to watch over it. This will provide denars to your clan, but also increase your crime rating.");
			TextObject textObject3 = new TextObject("{=qxY2ASqp}Take over the alley");
			InformationManager.ShowInquiry(new InquiryData(negativeText: new TextObject("{=jjEzdO0Y}Leave it empty").ToString(), titleText: textObject.ToString(), text: textObject2.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, affirmativeText: textObject3.ToString(), affirmativeAction: OnTakeOverTheAlley, negativeAction: OnLeaveItEmpty, soundEventPath: "", expireTime: 0f, timeoutAction: null, isAffirmativeOptionEnabled: CanPlayerOccupyTheCurrentAlley), pauseGameActiveState: true);
		}
		else if (Agent.Main == null || !Agent.Main.IsActive())
		{
			Mission.Current.NextCheckTimeEndMission = 0f;
			Campaign.Current.GameMenuManager.SetNextMenu("settlement_player_unconscious");
		}
		_fightPosition = Vec3.Invalid;
	}
}
