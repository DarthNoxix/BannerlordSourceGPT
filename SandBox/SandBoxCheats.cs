using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SandBox.BoardGames.MissionLogics;
using SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox;

public static class SandBoxCheats
{
	[CommandLineFunctionality.CommandLineArgumentFunction("spawn_new_alley_attack", "campaign")]
	public static string SpawnNewAlleyAttack(List<string> strings)
	{
		AlleyCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<AlleyCampaignBehavior>();
		if (campaignBehavior == null)
		{
			return "Alley Campaign Behavior not found";
		}
		foreach (AlleyCampaignBehavior.PlayerAlleyData item in (List<AlleyCampaignBehavior.PlayerAlleyData>)typeof(AlleyCampaignBehavior).GetField("_playerOwnedCommonAreaData", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(campaignBehavior))
		{
			if (!item.IsUnderAttack && item.Alley.Settlement.Alleys.Any((Alley x) => x.State == Alley.AreaState.OccupiedByGangLeader))
			{
				typeof(AlleyCampaignBehavior).GetMethod("StartNewAlleyAttack", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(campaignBehavior, new object[1] { item });
				return "Success";
			}
		}
		return "There is no suitable alley for spawning an alley attack.";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("win_board_game", "campaign")]
	public static string WinCurrentGame(List<string> strings)
	{
		MissionBoardGameLogic missionBoardGameLogic = Mission.Current?.GetMissionBehavior<MissionBoardGameLogic>();
		if (missionBoardGameLogic == null)
		{
			return "There is no board game.";
		}
		missionBoardGameLogic.PlayerOneWon();
		return "Success";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("refresh_battle_scene_index_map", "campaign")]
	public static string RefreshBattleSceneIndexMap(List<string> strings)
	{
		MapScene obj = Campaign.Current.MapSceneWrapper as MapScene;
		Type typeFromHandle = typeof(MapScene);
		FieldInfo field = typeFromHandle.GetField("_scene", BindingFlags.Instance | BindingFlags.NonPublic);
		FieldInfo field2 = typeFromHandle.GetField("_battleTerrainIndexMap", BindingFlags.Instance | BindingFlags.NonPublic);
		FieldInfo field3 = typeFromHandle.GetField("_battleTerrainIndexMapWidth", BindingFlags.Instance | BindingFlags.NonPublic);
		FieldInfo field4 = typeFromHandle.GetField("_battleTerrainIndexMapHeight", BindingFlags.Instance | BindingFlags.NonPublic);
		byte[] indexData = null;
		int width = 0;
		int height = 0;
		Scene scene = (Scene)field.GetValue(obj);
		MBMapScene.GetBattleSceneIndexMap(scene, ref indexData, ref width, ref height);
		field.SetValue(obj, scene);
		field2.SetValue(obj, indexData);
		field3.SetValue(obj, width);
		field4.SetValue(obj, height);
		return "Success";
	}
}
