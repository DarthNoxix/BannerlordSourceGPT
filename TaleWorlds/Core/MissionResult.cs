using System.Collections.Generic;

namespace TaleWorlds.Core;

public class MissionResult
{
	public BattleState BattleState { get; private set; }

	public bool BattleResolved
	{
		get
		{
			if (!PlayerVictory)
			{
				return PlayerDefeated;
			}
			return true;
		}
	}

	public bool PlayerVictory { get; private set; }

	public bool PlayerDefeated { get; private set; }

	public bool EnemyRetreated { get; private set; }

	internal static void AutoGeneratedStaticCollectObjectsMissionResult(object o, List<object> collectedObjects)
	{
		((MissionResult)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
	}

	public MissionResult(BattleState battleState, bool playerVictory, bool playerDefeated, bool enemyRetreated)
	{
		BattleState = battleState;
		PlayerVictory = playerVictory;
		PlayerDefeated = playerDefeated;
		EnemyRetreated = enemyRetreated;
	}

	public MissionResult()
	{
		PlayerVictory = false;
		PlayerDefeated = false;
		EnemyRetreated = false;
	}

	public static MissionResult CreateSuccessful(IMission mission, bool enemyRetreated = false)
	{
		return new MissionResult((mission.PlayerTeam.Side != BattleSideEnum.Attacker) ? BattleState.DefenderVictory : BattleState.AttackerVictory, playerVictory: true, playerDefeated: false, enemyRetreated);
	}

	public static MissionResult CreateDefeated(IMission mission)
	{
		return new MissionResult((mission.PlayerTeam.Side == BattleSideEnum.Attacker) ? BattleState.DefenderVictory : BattleState.AttackerVictory, playerVictory: false, playerDefeated: true, enemyRetreated: false);
	}

	public static MissionResult CreateDefenderPushedBack()
	{
		return new MissionResult(BattleState.DefenderPullBack, playerVictory: false, playerDefeated: false, enemyRetreated: false);
	}
}
