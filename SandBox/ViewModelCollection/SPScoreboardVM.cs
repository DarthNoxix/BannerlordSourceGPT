using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;

namespace SandBox.ViewModelCollection;

public class SPScoreboardVM : ScoreboardBaseVM, IBattleObserver
{
	private readonly BattleSimulation _battleSimulation;

	private static readonly TextObject _renownStr = new TextObject("{=eiWQoW9j}You gained {A0} renown.");

	private static readonly TextObject _influenceStr = new TextObject("{=5zeL8sa9}You gained {A0} influence.");

	private static readonly TextObject _moraleStr = new TextObject("{=WAKz9xX8}You gained {A0} morale.");

	private static readonly TextObject _lootStr = new TextObject("{=xu5NA6AW}You earned {A0}% of the loot.");

	private static readonly TextObject _deadLordStr = new TextObject("{=gDKhs4lD}{A0} has died on the battlefield.");

	private float _missionEndScoreboardDelayTimer;

	private MBBindingList<BattleResultVM> _battleResults;

	private string _simulationResult;

	private bool _isPlayerDefendingSiege
	{
		get
		{
			Mission current = Mission.Current;
			if (current != null && current.IsSiegeBattle)
			{
				return Mission.Current.PlayerTeam.IsDefender;
			}
			return false;
		}
	}

	[DataSourceProperty]
	public override MBBindingList<BattleResultVM> BattleResults
	{
		get
		{
			return _battleResults;
		}
		set
		{
			if (value != _battleResults)
			{
				_battleResults = value;
				OnPropertyChangedWithValue(value, "BattleResults");
			}
		}
	}

	[DataSourceProperty]
	public string SimulationResult
	{
		get
		{
			return _simulationResult;
		}
		set
		{
			if (value != _simulationResult)
			{
				_simulationResult = value;
				OnPropertyChangedWithValue(value, "SimulationResult");
			}
		}
	}

	public SPScoreboardVM(BattleSimulation simulation)
	{
		_battleSimulation = simulation;
		BattleResults = new MBBindingList<BattleResultVM>();
	}

	public override void RefreshValues()
	{
		base.RefreshValues();
		if (_isPlayerDefendingSiege)
		{
			base.QuitText = GameTexts.FindText("str_surrender").ToString();
		}
	}

	public override void Initialize(IMissionScreen missionScreen, Mission mission, Action releaseSimulationSources, Action<bool> onToggle)
	{
		base.Initialize(missionScreen, mission, releaseSimulationSources, onToggle);
		if (_battleSimulation != null)
		{
			_battleSimulation.BattleObserver = this;
			PlayerSide = (PlayerEncounter.PlayerIsAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
			base.Defenders = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "defender"), MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty.Banner);
			base.Attackers = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "attacker"), MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty.Banner);
			base.IsSimulation = true;
			base.IsMainCharacterDead = true;
			base.ShowScoreboard = true;
			_battleSimulation.ResetSimulation();
			base.PowerComparer.Update(base.Defenders.CurrentPower, base.Attackers.CurrentPower, base.Defenders.CurrentPower, base.Attackers.CurrentPower);
		}
		else
		{
			base.IsSimulation = false;
			BattleObserverMissionLogic missionBehavior = _mission.GetMissionBehavior<BattleObserverMissionLogic>();
			if (missionBehavior != null)
			{
				missionBehavior.SetObserver(this);
			}
			else
			{
				Debug.FailedAssert("SPScoreboard on CustomBattle", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\SPScoreboardVM.cs", "Initialize", 79);
			}
			if (Campaign.Current != null)
			{
				if (PlayerEncounter.Battle != null)
				{
					base.Defenders = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "defender"), MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty.Banner);
					base.Attackers = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "attacker"), MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty.Banner);
					PlayerSide = (PlayerEncounter.PlayerIsAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
				}
				else
				{
					base.Defenders = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "defender"), Mission.Current.Teams.Defender.Banner);
					base.Attackers = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "attacker"), Mission.Current.Teams.Attacker.Banner);
					PlayerSide = BattleSideEnum.Defender;
				}
			}
			else
			{
				Debug.FailedAssert("SPScoreboard on CustomBattle", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\SPScoreboardVM.cs", "Initialize", 105);
			}
		}
		string defenderColor;
		string attackerColor;
		if (MobileParty.MainParty.MapEvent != null)
		{
			defenderColor = ((!(MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty?.MapFaction is Kingdom)) ? Color.FromUint(MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty.MapFaction?.Banner.GetPrimaryColor() ?? 0).ToString() : Color.FromUint(((Kingdom)MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty.MapFaction).PrimaryBannerColor).ToString());
			attackerColor = ((!(MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty?.MapFaction is Kingdom)) ? Color.FromUint(MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty.MapFaction?.Banner.GetPrimaryColor() ?? 0).ToString() : Color.FromUint(((Kingdom)MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty.MapFaction).PrimaryBannerColor).ToString());
		}
		else
		{
			attackerColor = Color.FromUint(Mission.Current.Teams.Attacker.Color).ToString();
			defenderColor = Color.FromUint(Mission.Current.Teams.Defender.Color).ToString();
		}
		base.PowerComparer.SetColors(defenderColor, attackerColor);
		base.MissionTimeInSeconds = -1;
	}

	public override void Tick(float dt)
	{
		SallyOutEndLogic missionBehavior = Mission.Current.GetMissionBehavior<SallyOutEndLogic>();
		if (!base.IsOver)
		{
			if (!_mission.IsMissionEnding)
			{
				BattleEndLogic battleEndLogic = _battleEndLogic;
				if ((battleEndLogic == null || !battleEndLogic.IsEnemySideRetreating) && (missionBehavior == null || !missionBehavior.IsSallyOutOver))
				{
					goto IL_0062;
				}
			}
			if (_missionEndScoreboardDelayTimer < 1.5f)
			{
				_missionEndScoreboardDelayTimer += dt;
			}
			else
			{
				OnBattleOver();
			}
		}
		goto IL_0062;
		IL_0062:
		base.PowerComparer.IsEnabled = Mission.Current != null && Mission.Current.Mode != MissionMode.Deployment;
		base.IsPowerComparerEnabled = base.PowerComparer.IsEnabled && !BannerlordConfig.HideBattleUI && !MBCommon.IsPaused;
		if (!base.IsSimulation && !base.IsOver)
		{
			base.MissionTimeInSeconds = (int)Mission.Current.CurrentTime;
		}
	}

	public override void ExecutePlayAction()
	{
		if (base.IsSimulation)
		{
			_battleSimulation.Play();
		}
	}

	public override void ExecuteFastForwardAction()
	{
		if (base.IsSimulation)
		{
			if (!base.IsFastForwarding)
			{
				_battleSimulation.Play();
			}
			else
			{
				_battleSimulation.FastForward();
			}
		}
		else
		{
			Mission.Current.SetFastForwardingFromUI(base.IsFastForwarding);
		}
	}

	public override void ExecuteEndSimulationAction()
	{
		if (base.IsSimulation)
		{
			_battleSimulation.Skip();
		}
	}

	public override void ExecuteQuitAction()
	{
		OnExitBattle();
	}

	private void GetBattleRewards(bool playerVictory)
	{
		BattleResults.Clear();
		if (playerVictory)
		{
			ExplainedNumber renownExplained = new ExplainedNumber(0f, includeDescriptions: true);
			ExplainedNumber influencExplained = new ExplainedNumber(0f, includeDescriptions: true);
			ExplainedNumber moraleExplained = new ExplainedNumber(0f, includeDescriptions: true);
			PlayerEncounter.GetBattleRewards(out var renownChange, out var influenceChange, out var moraleChange, out var _, out var playerEarnedLootPercentage, ref renownExplained, ref influencExplained, ref moraleExplained);
			if (renownChange > 0.1f)
			{
				BattleResults.Add(new BattleResultVM(_renownStr.Format(renownChange), () => SandBoxUIHelper.GetExplainedNumberTooltip(ref renownExplained)));
			}
			if (influenceChange > 0.1f)
			{
				BattleResults.Add(new BattleResultVM(_influenceStr.Format(influenceChange), () => SandBoxUIHelper.GetExplainedNumberTooltip(ref influencExplained)));
			}
			if (moraleChange > 0.1f || moraleChange < -0.1f)
			{
				BattleResults.Add(new BattleResultVM(_moraleStr.Format(moraleChange), () => SandBoxUIHelper.GetExplainedNumberTooltip(ref moraleExplained)));
			}
			int num = ((PlayerSide == BattleSideEnum.Attacker) ? base.Attackers.Parties.Count : base.Defenders.Parties.Count);
			if (playerEarnedLootPercentage > 0.1f && num > 1)
			{
				BattleResults.Add(new BattleResultVM(_lootStr.Format(playerEarnedLootPercentage), () => SandBoxUIHelper.GetBattleLootAwardTooltip(playerEarnedLootPercentage)));
			}
		}
		foreach (SPScoreboardPartyVM party in base.Defenders.Parties)
		{
			foreach (SPScoreboardUnitVM item in party.Members.Where((SPScoreboardUnitVM member) => member.IsHero && member.Score.Dead > 0))
			{
				BattleResults.Add(new BattleResultVM(_deadLordStr.SetTextVariable("A0", item.Character.Name).ToString(), () => new List<TooltipProperty>(), SandBoxUIHelper.GetCharacterCode(item.Character as CharacterObject)));
			}
		}
		foreach (SPScoreboardPartyVM party2 in base.Attackers.Parties)
		{
			foreach (SPScoreboardUnitVM item2 in party2.Members.Where((SPScoreboardUnitVM member) => member.IsHero && member.Score.Dead > 0))
			{
				BattleResults.Add(new BattleResultVM(_deadLordStr.SetTextVariable("A0", item2.Character.Name).ToString(), () => new List<TooltipProperty>(), SandBoxUIHelper.GetCharacterCode(item2.Character as CharacterObject)));
			}
		}
	}

	private void UpdateSimulationResult(bool playerVictory)
	{
		if (base.IsSimulation)
		{
			if (playerVictory)
			{
				if (PlayerEncounter.Battle.PartiesOnSide(PlayerSide).Sum((MapEventParty x) => x.Party.NumberOfHealthyMembers) < 70)
				{
					SimulationResult = "SimulationVictorySmall";
				}
				else
				{
					SimulationResult = "SimulationVictoryLarge";
				}
			}
			else
			{
				SimulationResult = "SimulationDefeat";
			}
		}
		else
		{
			SimulationResult = "NotSimulation";
		}
	}

	public void OnBattleOver()
	{
		BattleResultType battleResultType = BattleResultType.NotOver;
		if (PlayerEncounter.IsActive && PlayerEncounter.Battle != null)
		{
			base.IsOver = true;
			battleResultType = ((PlayerEncounter.WinningSide == PlayerSide) ? BattleResultType.Victory : ((PlayerEncounter.CampaignBattleResult != null && PlayerEncounter.CampaignBattleResult.EnemyPulledBack) ? BattleResultType.Retreat : BattleResultType.Defeat));
			bool playerVictory = PlayerEncounter.WinningSide == PlayerSide;
			GetBattleRewards(playerVictory);
			UpdateSimulationResult(playerVictory);
		}
		else
		{
			Mission current = Mission.Current;
			if (current != null && current.MissionEnded)
			{
				base.IsOver = true;
				battleResultType = (((Mission.Current.HasMissionBehavior<SallyOutEndLogic>() && !Mission.Current.MissionResult.BattleResolved) || Mission.Current.MissionResult.PlayerVictory) ? BattleResultType.Victory : ((Mission.Current.MissionResult.BattleState == BattleState.DefenderPullBack) ? BattleResultType.Retreat : BattleResultType.Defeat));
			}
			else
			{
				BattleEndLogic battleEndLogic = _battleEndLogic;
				if (battleEndLogic != null && battleEndLogic.IsEnemySideRetreating)
				{
					base.IsOver = true;
				}
			}
		}
		switch (battleResultType)
		{
		case BattleResultType.Defeat:
			base.BattleResult = GameTexts.FindText("str_defeat").ToString();
			base.BattleResultIndex = (int)battleResultType;
			break;
		case BattleResultType.Victory:
			base.BattleResult = GameTexts.FindText("str_victory").ToString();
			base.BattleResultIndex = (int)battleResultType;
			break;
		case BattleResultType.Retreat:
			base.BattleResult = GameTexts.FindText("str_battle_result_retreat").ToString();
			base.BattleResultIndex = (int)battleResultType;
			break;
		case BattleResultType.NotOver:
			break;
		}
	}

	public void OnExitBattle()
	{
		if (base.IsSimulation)
		{
			if (_battleSimulation.IsSimulationFinished)
			{
				_releaseSimulationSources();
				_battleSimulation.OnReturn();
				return;
			}
			Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
			InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_order_Retreat").ToString(), GameTexts.FindText("str_retreat_question").ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_ok").ToString(), GameTexts.FindText("str_cancel").ToString(), delegate
			{
				Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
				_releaseSimulationSources();
				_battleSimulation.OnReturn();
			}, delegate
			{
				Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
			}));
			return;
		}
		BattleEndLogic missionBehavior = _mission.GetMissionBehavior<BattleEndLogic>();
		BasicMissionHandler missionBehavior2 = _mission.GetMissionBehavior<BasicMissionHandler>();
		BattleEndLogic.ExitResult exitResult = (BattleEndLogic.ExitResult)(((int?)missionBehavior?.TryExit()) ?? ((!_mission.MissionEnded) ? 1 : 3));
		switch (exitResult)
		{
		case BattleEndLogic.ExitResult.NeedsPlayerConfirmation:
		case BattleEndLogic.ExitResult.SurrenderSiege:
			OnToggle(obj: false);
			missionBehavior2.CreateWarningWidgetForResult(exitResult);
			return;
		case BattleEndLogic.ExitResult.False:
			InformationManager.ShowInquiry(_retreatInquiryData);
			return;
		}
		if (missionBehavior == null && exitResult == BattleEndLogic.ExitResult.True)
		{
			_mission.EndMission();
		}
	}

	public void TroopNumberChanged(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject character, int number = 0, int numberDead = 0, int numberWounded = 0, int numberRouted = 0, int numberKilled = 0, int numberReadyToUpgrade = 0)
	{
		bool isPlayerParty = (battleCombatant as PartyBase)?.Owner == Hero.MainHero;
		GetSide(side).UpdateScores(battleCombatant, isPlayerParty, character, number, numberDead, numberWounded, numberRouted, numberKilled, numberReadyToUpgrade);
		base.PowerComparer.Update(base.Defenders.CurrentPower, base.Attackers.CurrentPower, base.Defenders.InitialPower, base.Attackers.InitialPower);
	}

	public void HeroSkillIncreased(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject heroCharacter, SkillObject upgradedSkill)
	{
		bool isPlayerParty = (battleCombatant as PartyBase)?.Owner == Hero.MainHero;
		GetSide(side).UpdateHeroSkills(battleCombatant, isPlayerParty, heroCharacter, upgradedSkill);
	}

	public void BattleResultsReady()
	{
		if (!base.IsOver)
		{
			OnBattleOver();
		}
	}

	public void TroopSideChanged(BattleSideEnum prevSide, BattleSideEnum newSide, IBattleCombatant battleCombatant, BasicCharacterObject character)
	{
		SPScoreboardStatsVM scoreToBringOver = GetSide(prevSide).RemoveTroop(battleCombatant, character);
		GetSide(newSide).GetPartyAddIfNotExists(battleCombatant, (battleCombatant as PartyBase)?.Owner == Hero.MainHero);
		GetSide(newSide).AddTroop(battleCombatant, character, scoreToBringOver);
	}
}
