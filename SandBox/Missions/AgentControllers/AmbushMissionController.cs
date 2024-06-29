using System;
using System.Collections.Generic;
using SandBox.Missions.Handlers;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace SandBox.Missions.AgentControllers;

public class AmbushMissionController : BaseBattleMissionController
{
	private AmbushBattleDeploymentHandler _ambushDeploymentHandler;

	private AmbushIntroLogic _ambushIntroLogic;

	private readonly List<GameEntity> _checkPoints;

	private readonly List<GameEntity> _defenderSpawnPoints;

	private int _currentPositionIndex;

	private int _columns;

	private const float UnitSpread = 1f;

	private Team _playerSoloTeam;

	private bool _firstTick = true;

	public bool IsPlayerAmbusher
	{
		get
		{
			return base.IsPlayerAttacker;
		}
		private set
		{
		}
	}

	public event AmbushMissionEventDelegate PlayerDeploymentFinish;

	public event AmbushMissionEventDelegate IntroFinish;

	public AmbushMissionController(bool isPlayerAttacker)
		: base(isPlayerAttacker)
	{
		_checkPoints = new List<GameEntity>();
		_defenderSpawnPoints = new List<GameEntity>();
	}

	public override void OnBehaviorInitialize()
	{
		base.OnBehaviorInitialize();
		_ambushDeploymentHandler = base.Mission.GetMissionBehavior<AmbushBattleDeploymentHandler>();
		_ambushIntroLogic = base.Mission.GetMissionBehavior<AmbushIntroLogic>();
	}

	public override void AfterStart()
	{
		base.AfterStart();
		base.Mission.SetMissionMode(MissionMode.Stealth, atStart: true);
		int num = 0;
		GameEntity gameEntity = null;
		do
		{
			gameEntity = Mission.Current.Scene.FindEntityWithTag("checkpoint_" + num);
			if (gameEntity != null)
			{
				_checkPoints.Add(gameEntity);
				num++;
			}
		}
		while (gameEntity != null);
		num = 0;
		do
		{
			gameEntity = Mission.Current.Scene.FindEntityWithTag("spawnpoint_defender_" + num);
			if (gameEntity != null)
			{
				_defenderSpawnPoints.Add(gameEntity);
				num++;
			}
		}
		while (gameEntity != null);
		if (base.Mission.PlayerTeam.Side == BattleSideEnum.Attacker)
		{
			SetupTeam(base.Mission.AttackerTeam);
		}
		else
		{
			SetupTeam(base.Mission.AttackerTeam);
			SetupTeam(base.Mission.DefenderTeam);
		}
		_playerSoloTeam = base.Mission.Teams.Add(base.Mission.PlayerTeam.Side, uint.MaxValue, uint.MaxValue, null, isPlayerGeneral: true, isPlayerSergeant: false, isSettingRelations: false);
		base.Mission.AttackerTeam.SetIsEnemyOf(base.Mission.DefenderTeam, isEnemyOf: false);
		base.Mission.DefenderTeam.SetIsEnemyOf(base.Mission.AttackerTeam, isEnemyOf: false);
		base.Mission.AttackerTeam.SetIsEnemyOf(_playerSoloTeam, isEnemyOf: false);
		base.Mission.DefenderTeam.SetIsEnemyOf(_playerSoloTeam, isEnemyOf: true);
		base.Mission.AttackerTeam.ExpireAIQuerySystem();
		base.Mission.DefenderTeam.ExpireAIQuerySystem();
		Agent.Main.Controller = Agent.ControllerType.AI;
	}

	public override void OnMissionTick(float dt)
	{
		if (_firstTick)
		{
			_firstTick = false;
			if (!IsPlayerAmbusher)
			{
				base.Mission.AddMissionBehavior(new MissionBoundaryCrossingHandler());
				_ambushIntroLogic.StartIntro();
			}
		}
		base.OnMissionTick(dt);
		UpdateAgents();
	}

	protected override void CreateDefenderTroops()
	{
		CreateTroop("guard", base.Mission.DefenderTeam, 30);
	}

	protected override void CreateAttackerTroops()
	{
		CreateTroop("guard", base.Mission.AttackerTeam, 10);
		CreateTroop("archer", base.Mission.AttackerTeam, 15);
	}

	protected void CreateTroop(string troopName, Team troopTeam, int troopCount, bool isReinforcement = false)
	{
		if (troopTeam.Side == BattleSideEnum.Attacker)
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(troopName);
			FormationClass defaultFormationClass = @object.DefaultFormationClass;
			Formation formation = troopTeam.GetFormation(defaultFormationClass);
			base.Mission.GetFormationSpawnFrame(troopTeam.Side, defaultFormationClass, isReinforcement, out var spawnPosition, out var spawnDirection);
			formation.SetPositioning(spawnPosition, spawnDirection);
			for (int i = 0; i < troopCount; i++)
			{
				(base.Mission.SpawnAgent(new AgentBuildData(@object).Team(troopTeam).Formation(formation).FormationTroopSpawnCount(troopCount)
					.FormationTroopSpawnIndex(i)).AddController(typeof(AmbushBattleAgentController)) as AmbushBattleAgentController).IsAttacker = true;
				IncrementDeploymedTroops(BattleSideEnum.Attacker);
			}
			return;
		}
		CharacterObject object2 = game.ObjectManager.GetObject<CharacterObject>(troopName);
		for (int j = 0; j < troopCount; j++)
		{
			int count = _defenderSpawnPoints.Count;
			_columns = TaleWorlds.Library.MathF.Ceiling((float)troopCount / (float)count);
			int num = base.DeployedDefenderTroopCount - base.DeployedDefenderTroopCount / _columns * _columns;
			MatrixFrame globalFrame = _defenderSpawnPoints[base.DeployedDefenderTroopCount / _columns].GetGlobalFrame();
			globalFrame.origin = globalFrame.TransformToParent(new Vec3(1f) * num * 1f);
			Mission mission = base.Mission;
			AgentBuildData agentBuildData = new AgentBuildData(object2).Team(troopTeam).InitialPosition(in globalFrame.origin);
			Vec2 direction = globalFrame.rotation.f.AsVec2.Normalized();
			(mission.SpawnAgent(agentBuildData.InitialDirection(in direction)).AddController(typeof(AmbushBattleAgentController)) as AmbushBattleAgentController).IsAttacker = false;
			IncrementDeploymedTroops(BattleSideEnum.Defender);
		}
	}

	public void OnPlayerDeploymentFinish(bool doDebugPause = false)
	{
		if (base.Mission.PlayerTeam.Side == BattleSideEnum.Attacker)
		{
			SetupTeam(base.Mission.DefenderTeam);
		}
		base.Mission.RemoveMissionBehavior(_ambushDeploymentHandler);
		base.Mission.AddMissionBehavior(new MissionBoundaryCrossingHandler());
		_ambushIntroLogic.StartIntro();
		if (this.PlayerDeploymentFinish != null)
		{
			this.PlayerDeploymentFinish();
		}
		Agent.Main.SetTeam(_playerSoloTeam, sync: true);
	}

	public void OnIntroductionFinish()
	{
		if (!base.IsPlayerAttacker)
		{
			StartFighting();
		}
		if (this.IntroFinish != null)
		{
			this.IntroFinish();
		}
		Agent.Main.Controller = Agent.ControllerType.Player;
	}

	private void UpdateAgents()
	{
		int num = 0;
		int num2 = 0;
		foreach (Agent agent3 in base.Mission.Agents)
		{
			if (base.Mission.Mode == MissionMode.Stealth && agent3.IsAIControlled && agent3.CurrentWatchState == Agent.WatchState.Cautious && agent3.IsHuman)
			{
				StartFighting();
			}
			if (!IsPlayerAmbusher && Agent.Main.IsAIControlled)
			{
				Vec2 movementDirection = Agent.Main.GetMovementDirection();
				WorldPosition position = Agent.Main.GetWorldPosition();
				position.SetVec2(position.AsVec2 + movementDirection * 5f);
				Agent.Main.DisableScriptedMovement();
				Agent.Main.SetScriptedPosition(ref position, addHumanLikeDelay: false, Agent.AIScriptedFrameFlags.DoNotRun);
			}
			AmbushBattleAgentController controller = agent3.GetController<AmbushBattleAgentController>();
			if (controller != null)
			{
				controller.UpdateState();
				if (!controller.IsAttacker && !controller.Aggressive)
				{
					if (num == 0)
					{
						if (controller.CheckArrivedAtWayPoint(_checkPoints[_currentPositionIndex]))
						{
							_currentPositionIndex++;
							if (_currentPositionIndex >= _checkPoints.Count)
							{
								MBDebug.ShowWarning("The enemy has gotten away.");
							}
							else
							{
								WorldPosition position2 = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, _checkPoints[_currentPositionIndex].GlobalPosition, hasValidZ: false);
								agent3.SetScriptedPosition(ref position2, addHumanLikeDelay: false, Agent.AIScriptedFrameFlags.DoNotRun);
							}
						}
					}
					else
					{
						WorldPosition position3;
						Vec2 vec;
						if (num % _columns != 0)
						{
							Agent agent = base.Mission.Agents[num2 - 1];
							position3 = agent.GetWorldPosition();
							vec = agent.GetMovementDirection();
							vec.RotateCCW(-(float)Math.PI / 2f);
						}
						else
						{
							Agent agent2 = base.Mission.Agents[num2 - _columns];
							position3 = agent2.GetWorldPosition();
							vec = agent3.Position.AsVec2 - agent2.Position.AsVec2;
							vec.Normalize();
						}
						position3.SetVec2(position3.AsVec2 + vec * 1f);
						agent3.DisableScriptedMovement();
						agent3.SetScriptedPosition(ref position3, addHumanLikeDelay: false, Agent.AIScriptedFrameFlags.DoNotRun);
					}
					num++;
				}
			}
			num2++;
		}
	}

	private void StartFighting()
	{
		base.Mission.AttackerTeam.SetIsEnemyOf(base.Mission.DefenderTeam, isEnemyOf: true);
		base.Mission.DefenderTeam.SetIsEnemyOf(base.Mission.AttackerTeam, isEnemyOf: true);
		if (base.Mission.PlayerAllyTeam != null)
		{
			base.Mission.PlayerAllyTeam.SetIsEnemyOf(base.Mission.PlayerEnemyTeam, isEnemyOf: true);
			base.Mission.PlayerEnemyTeam.SetIsEnemyOf(base.Mission.PlayerAllyTeam, isEnemyOf: true);
		}
		base.Mission.SetMissionMode(MissionMode.Battle, atStart: false);
		foreach (Agent agent in base.Mission.Agents)
		{
			AmbushBattleAgentController controller = agent.GetController<AmbushBattleAgentController>();
			if (controller != null)
			{
				controller.Aggressive = true;
				if (!controller.IsAttacker)
				{
					agent.DisableScriptedMovement();
					FormationClass agentTroopClass = base.Mission.GetAgentTroopClass(BattleSideEnum.Defender, agent.Character);
					agent.Formation = base.Mission.DefenderTeam.GetFormation(agentTroopClass);
					agent.Formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
				}
			}
			if (agent.IsPlayerControlled)
			{
				agent.SetTeam(base.Mission.PlayerTeam, sync: true);
			}
		}
		base.Mission.DefenderTeam.MasterOrderController.SelectAllFormations();
		base.Mission.DefenderTeam.MasterOrderController.SetOrder(OrderType.StandYourGround);
		base.Mission.DefenderTeam.MasterOrderController.SetOrder(OrderType.Charge);
		foreach (Formation item in base.Mission.DefenderTeam.FormationsIncludingSpecialAndEmpty)
		{
			if (item.CountOfUnits > 0)
			{
				base.Mission.DefenderTeam.MasterOrderController.DeselectFormation(item);
			}
		}
	}
}
