using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics;

public class SneakTeam3MissionController : MissionLogic
{
	private Game _game;

	private List<List<GameEntity>> _townRegionProps;

	private Agent _playerAgent;

	private const string _targetEntityTag = "khuzait_scroll";

	private bool _isScrollObtained;

	public SneakTeam3MissionController()
	{
		_game = Game.Current;
		_townRegionProps = new List<List<GameEntity>>();
		_isScrollObtained = false;
	}

	public override void AfterStart()
	{
		base.AfterStart();
		base.Mission.SetMissionMode(MissionMode.Stealth, atStart: true);
		base.Mission.Scene.TimeOfDay = 20.5f;
		GetAllProps();
		RandomizeScrollPosition();
		GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("spawnpoint_player");
		MatrixFrame matrixFrame = ((gameEntity != null) ? gameEntity.GetGlobalFrame() : MatrixFrame.Identity);
		if (gameEntity != null)
		{
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		}
		Mission mission = base.Mission;
		AgentBuildData agentBuildData = new AgentBuildData(_game.PlayerTroop).Team(base.Mission.PlayerTeam).InitialPosition(in matrixFrame.origin);
		Vec2 direction = matrixFrame.rotation.f.AsVec2.Normalized();
		_playerAgent = mission.SpawnAgent(agentBuildData.InitialDirection(in direction).NoHorses(noHorses: true).Controller(Agent.ControllerType.Player));
		_playerAgent.WieldInitialWeapons();
	}

	private void GetAllProps()
	{
		for (int i = 0; i < 5; i++)
		{
			List<GameEntity> list = new List<GameEntity>();
			IEnumerable<GameEntity> collection = base.Mission.Scene.FindEntitiesWithTag("patrol_region_" + i);
			list.AddRange(collection);
			_townRegionProps.Add(list);
		}
	}

	private void RandomizeScrollPosition()
	{
		int num = MBRandom.RandomInt(3);
		GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("scroll_" + num);
		if (gameEntity != null)
		{
			GameEntity gameEntity2 = base.Mission.Scene.FindEntityWithTag("khuzait_scroll");
			if (gameEntity2 != null)
			{
				MatrixFrame frame = gameEntity.GetFrame();
				frame.origin.z += 0.9f;
				gameEntity2.SetFrame(ref frame);
			}
		}
	}

	public override void OnMissionTick(float dt)
	{
		base.OnMissionTick(dt);
	}

	private bool IsPlayerDead()
	{
		if (base.Mission.MainAgent != null)
		{
			return !base.Mission.MainAgent.IsActive();
		}
		return true;
	}

	public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
	{
		if (usedObject.GameEntity.HasTag("khuzait_scroll"))
		{
			_isScrollObtained = true;
		}
	}

	public override bool MissionEnded(ref MissionResult missionResult)
	{
		if (!_isScrollObtained)
		{
			return IsPlayerDead();
		}
		return true;
	}
}
