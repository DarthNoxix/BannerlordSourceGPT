using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Conversation.MissionLogics;

public class ConversationMissionLogic : MissionLogic
{
	private static readonly ActionIndexCache act_conversation_normal_loop = ActionIndexCache.Create("act_conversation_normal_loop");

	private ConversationCharacterData _otherSideConversationData;

	private ConversationCharacterData _playerConversationData;

	private readonly List<Agent> _addBloodToAgents;

	private Agent _curConversationPartnerAgent;

	private bool _isRenderingStarted;

	private bool _conversationStarted;

	private bool _isCivilianEquipmentRequiredForLeader;

	private bool _isCivilianEquipmentRequiredForBodyGuards;

	private List<GameEntity> _usedSpawnPoints;

	private GameEntity _conversationSet;

	private bool _realCameraController;

	private bool IsReadyForConversation
	{
		get
		{
			if (_isRenderingStarted && Agent.Main != null)
			{
				return Agent.Main.IsActive();
			}
			return false;
		}
	}

	public ConversationMissionLogic(ConversationCharacterData playerCharacterData, ConversationCharacterData otherCharacterData)
	{
		_playerConversationData = playerCharacterData;
		_otherSideConversationData = otherCharacterData;
		_isCivilianEquipmentRequiredForLeader = otherCharacterData.IsCivilianEquipmentRequiredForLeader;
		_isCivilianEquipmentRequiredForBodyGuards = otherCharacterData.IsCivilianEquipmentRequiredForBodyGuardCharacters;
		_addBloodToAgents = new List<Agent>();
	}

	public override void AfterStart()
	{
		base.AfterStart();
		_realCameraController = base.Mission.CameraIsFirstPerson;
		base.Mission.CameraIsFirstPerson = true;
		IEnumerable<GameEntity> source = base.Mission.Scene.FindEntitiesWithTag("binary_conversation_point");
		if (source.Any())
		{
			_conversationSet = source.ToMBList().GetRandomElement();
		}
		_usedSpawnPoints = new List<GameEntity>();
		BattleSideEnum battleSideEnum = BattleSideEnum.Attacker;
		if (PlayerSiege.PlayerSiegeEvent != null)
		{
			battleSideEnum = PlayerSiege.PlayerSide;
		}
		else if (PlayerEncounter.Current != null)
		{
			battleSideEnum = ((!PlayerEncounter.InsideSettlement || PlayerEncounter.Current.OpponentSide == BattleSideEnum.Defender) ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
			if (PlayerEncounter.Current.EncounterSettlementAux != null && PlayerEncounter.Current.EncounterSettlementAux.MapFaction == Hero.MainHero.MapFaction)
			{
				battleSideEnum = ((!PlayerEncounter.Current.EncounterSettlementAux.IsUnderSiege) ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
			}
		}
		base.Mission.PlayerTeam = base.Mission.Teams.Add(battleSideEnum, Hero.MainHero.MapFaction.Color, Hero.MainHero.MapFaction.Color2);
		bool flag = _otherSideConversationData.Character.Equipment[10].Item != null && _otherSideConversationData.Character.Equipment[10].Item.HasHorseComponent && battleSideEnum == BattleSideEnum.Defender;
		MatrixFrame matrixFrame;
		MatrixFrame initialFrame;
		if (_conversationSet != null)
		{
			if (base.Mission.PlayerTeam.IsDefender)
			{
				matrixFrame = GetDefenderSideSpawnFrame();
				initialFrame = GetAttackerSideSpawnFrame(flag);
			}
			else
			{
				matrixFrame = GetAttackerSideSpawnFrame(flag);
				initialFrame = GetDefenderSideSpawnFrame();
			}
		}
		else
		{
			matrixFrame = GetPlayerSideSpawnFrameInSettlement();
			initialFrame = GetOtherSideSpawnFrameInSettlement(matrixFrame);
		}
		SpawnPlayer(_playerConversationData, matrixFrame);
		SpawnOtherSide(_otherSideConversationData, initialFrame, flag, !base.Mission.PlayerTeam.IsDefender);
	}

	private void SpawnPlayer(ConversationCharacterData playerConversationData, MatrixFrame initialFrame)
	{
		MatrixFrame initialFrame2 = new MatrixFrame(initialFrame.rotation, initialFrame.origin);
		initialFrame2.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		SpawnCharacter(CharacterObject.PlayerCharacter, playerConversationData, initialFrame2, act_conversation_normal_loop);
	}

	private void SpawnOtherSide(ConversationCharacterData characterData, MatrixFrame initialFrame, bool spawnWithHorse, bool isDefenderSide)
	{
		MatrixFrame matrixFrame = new MatrixFrame(initialFrame.rotation, initialFrame.origin);
		if (Agent.Main != null)
		{
			matrixFrame.rotation.f = Agent.Main.Position - matrixFrame.origin;
		}
		matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		Monster monsterWithSuffix = TaleWorlds.Core.FaceGen.GetMonsterWithSuffix(characterData.Character.Race, "_settlement");
		AgentBuildData agentBuildData = new AgentBuildData(characterData.Character).TroopOrigin(new SimpleAgentOrigin(characterData.Character)).Team(base.Mission.PlayerTeam).Monster(monsterWithSuffix)
			.InitialPosition(in matrixFrame.origin);
		Vec2 direction = matrixFrame.rotation.f.AsVec2;
		AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(in direction).NoHorses(!spawnWithHorse).CivilianEquipment(_isCivilianEquipmentRequiredForLeader);
		if (characterData.Party?.LeaderHero?.ClanBanner != null)
		{
			agentBuildData2.Banner(characterData.Party.LeaderHero.ClanBanner);
		}
		else if (characterData.Party != null && characterData.Party.MapFaction != null)
		{
			agentBuildData2.Banner(characterData.Party?.MapFaction?.Banner);
		}
		if (spawnWithHorse)
		{
			agentBuildData2.MountKey(MountCreationKey.GetRandomMountKeyString(characterData.Character.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, characterData.Character.GetMountKeySeed()));
		}
		if (characterData.Party != null)
		{
			agentBuildData2.TroopOrigin(new PartyAgentOrigin(characterData.Party, characterData.Character, 0, new UniqueTroopDescriptor(FlattenedTroopRoster.GenerateUniqueNoFromParty(characterData.Party.MobileParty, 0))));
			agentBuildData2.ClothingColor1(characterData.Party.MapFaction.Color).ClothingColor2(characterData.Party.MapFaction.Color2);
		}
		Agent agent = base.Mission.SpawnAgent(agentBuildData2);
		if (characterData.SpawnedAfterFight)
		{
			_addBloodToAgents.Add(agent);
		}
		if (agent.MountAgent == null)
		{
			agent.SetActionChannel(0, act_conversation_normal_loop, ignorePriority: false, 0uL, 0f, 1f, 0f, 0.4f, MBRandom.RandomFloat);
		}
		agent.AgentVisuals.SetAgentLodZeroOrMax(makeZero: true);
		_curConversationPartnerAgent = agent;
		bool flag = characterData.Character.HeroObject != null && characterData.Character.HeroObject.IsPlayerCompanion;
		if (!characterData.NoBodyguards && !flag)
		{
			SpawnBodyguards(isDefenderSide);
		}
	}

	private MatrixFrame GetDefenderSideSpawnFrame()
	{
		MatrixFrame result = MatrixFrame.Identity;
		foreach (GameEntity child in _conversationSet.GetChildren())
		{
			if (child.HasTag("opponent_infantry_spawn"))
			{
				result = child.GetGlobalFrame();
				break;
			}
		}
		result.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		return result;
	}

	private MatrixFrame GetAttackerSideSpawnFrame(bool hasHorse)
	{
		MatrixFrame result = MatrixFrame.Identity;
		foreach (GameEntity child in _conversationSet.GetChildren())
		{
			if (hasHorse && child.HasTag("player_cavalry_spawn"))
			{
				result = child.GetGlobalFrame();
				break;
			}
			if (child.HasTag("player_infantry_spawn"))
			{
				result = child.GetGlobalFrame();
				break;
			}
		}
		result.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		return result;
	}

	private MatrixFrame GetPlayerSideSpawnFrameInSettlement()
	{
		MatrixFrame result = (base.Mission.Scene.FindEntityWithTag("spawnpoint_player") ?? base.Mission.Scene.FindEntitiesWithTag("sp_player_conversation").FirstOrDefault() ?? base.Mission.Scene.FindEntityWithTag("spawnpoint_player_outside"))?.GetFrame() ?? MatrixFrame.Identity;
		result.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		return result;
	}

	private MatrixFrame GetOtherSideSpawnFrameInSettlement(MatrixFrame playerFrame)
	{
		MatrixFrame result = playerFrame;
		Vec3 vec = new Vec3(playerFrame.rotation.f);
		vec.Normalize();
		result.origin = playerFrame.origin + 4f * vec;
		result.rotation.RotateAboutUp((float)Math.PI);
		return result;
	}

	public override void OnRenderingStarted()
	{
		_isRenderingStarted = true;
		Debug.Print("\n ConversationMissionLogic::OnRenderingStarted\n", 0, Debug.DebugColor.Cyan, 64uL);
	}

	private void InitializeAfterCreation(Agent conversationPartnerAgent, PartyBase conversationPartnerParty)
	{
		Campaign.Current.ConversationManager.SetupAndStartMapConversation(conversationPartnerParty?.MobileParty, conversationPartnerAgent, Mission.Current.MainAgentServer);
		base.Mission.SetMissionMode(MissionMode.Conversation, atStart: true);
	}

	public override void OnMissionTick(float dt)
	{
		if (_addBloodToAgents.Count > 0)
		{
			foreach (Agent addBloodToAgent in _addBloodToAgents)
			{
				(sbyte, sbyte) randomPairOfRealBloodBurstBoneIndices = addBloodToAgent.GetRandomPairOfRealBloodBurstBoneIndices();
				if (randomPairOfRealBloodBurstBoneIndices.Item1 != -1 && randomPairOfRealBloodBurstBoneIndices.Item2 != -1)
				{
					addBloodToAgent.CreateBloodBurstAtLimb(randomPairOfRealBloodBurstBoneIndices.Item1, 0.1f + MBRandom.RandomFloat * 0.1f);
					addBloodToAgent.CreateBloodBurstAtLimb(randomPairOfRealBloodBurstBoneIndices.Item2, 0.2f + MBRandom.RandomFloat * 0.2f);
				}
			}
			_addBloodToAgents.Clear();
		}
		if (!_conversationStarted)
		{
			if (!IsReadyForConversation)
			{
				return;
			}
			InitializeAfterCreation(_curConversationPartnerAgent, _otherSideConversationData.Party);
			_conversationStarted = true;
		}
		if (base.Mission.InputManager.IsGameKeyPressed(4))
		{
			Campaign.Current.ConversationManager.EndConversation();
		}
		if (!Campaign.Current.ConversationManager.IsConversationInProgress)
		{
			base.Mission.EndMission();
		}
	}

	private void SpawnBodyguards(bool isDefenderSide)
	{
		int num = 2;
		ConversationCharacterData otherSideConversationData = _otherSideConversationData;
		if (otherSideConversationData.Party == null)
		{
			return;
		}
		TroopRoster memberRoster = otherSideConversationData.Party.MemberRoster;
		int num2 = memberRoster.TotalManCount;
		if (memberRoster.Contains(CharacterObject.PlayerCharacter))
		{
			num2--;
		}
		if (num2 < num + 1)
		{
			return;
		}
		List<CharacterObject> list = new List<CharacterObject>();
		foreach (TroopRosterElement item in memberRoster.GetTroopRoster())
		{
			if (item.Character.IsHero && otherSideConversationData.Character != item.Character && !list.Contains(item.Character) && item.Character.HeroObject.IsWounded && !item.Character.IsPlayerCharacter)
			{
				list.Add(item.Character);
			}
		}
		while (list.Count < num)
		{
			foreach (TroopRosterElement item2 in from k in memberRoster.GetTroopRoster()
				orderby k.Character.Level descending
				select k)
			{
				if ((!otherSideConversationData.Character.IsHero || otherSideConversationData.Character != item2.Character) && !item2.Character.IsPlayerCharacter)
				{
					list.Add(item2.Character);
				}
				if (list.Count == num)
				{
					break;
				}
			}
		}
		List<ActionIndexCache> list2 = new List<ActionIndexCache>
		{
			ActionIndexCache.Create("act_stand_1"),
			ActionIndexCache.Create("act_inventory_idle_start"),
			ActionIndexCache.Create("act_inventory_idle"),
			act_conversation_normal_loop,
			ActionIndexCache.Create("act_conversation_warrior_loop"),
			ActionIndexCache.Create("act_conversation_hip_loop"),
			ActionIndexCache.Create("act_conversation_closed_loop"),
			ActionIndexCache.Create("act_conversation_demure_loop")
		};
		for (int i = 0; i < num; i++)
		{
			int index = new Random().Next(0, list.Count);
			int index2 = MBRandom.RandomInt(0, list2.Count);
			SpawnCharacter(list[index], otherSideConversationData, GetBodyguardSpawnFrame(list[index].HasMount(), isDefenderSide), list2[index2]);
			list2.RemoveAt(index2);
			list.RemoveAt(index);
		}
	}

	private void SpawnCharacter(CharacterObject character, ConversationCharacterData characterData, MatrixFrame initialFrame, ActionIndexCache conversationAction)
	{
		Monster monsterWithSuffix = TaleWorlds.Core.FaceGen.GetMonsterWithSuffix(character.Race, "_settlement");
		AgentBuildData agentBuildData = new AgentBuildData(character).TroopOrigin(new SimpleAgentOrigin(character)).Team(base.Mission.PlayerTeam).Monster(monsterWithSuffix)
			.InitialPosition(in initialFrame.origin);
		Vec2 direction = initialFrame.rotation.f.AsVec2.Normalized();
		AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(in direction).NoHorses(character.HasMount()).NoWeapons(characterData.NoWeapon)
			.CivilianEquipment((character == CharacterObject.PlayerCharacter) ? _isCivilianEquipmentRequiredForLeader : _isCivilianEquipmentRequiredForBodyGuards);
		if (characterData.Party?.LeaderHero?.ClanBanner != null)
		{
			agentBuildData2.Banner(characterData.Party.LeaderHero.ClanBanner);
		}
		else if (characterData.Party != null && characterData.Party?.MapFaction != null)
		{
			agentBuildData2.Banner(characterData.Party.MapFaction.Banner);
		}
		if (characterData.Party != null)
		{
			agentBuildData2.ClothingColor1(characterData.Party.MapFaction.Color).ClothingColor2(characterData.Party.MapFaction.Color2);
		}
		if (characterData.Character == CharacterObject.PlayerCharacter)
		{
			agentBuildData2.Controller(Agent.ControllerType.Player);
		}
		Agent agent = base.Mission.SpawnAgent(agentBuildData2);
		agent.AgentVisuals.SetAgentLodZeroOrMax(makeZero: true);
		agent.SetLookAgent(Agent.Main);
		AnimationSystemData animationSystemData = agentBuildData2.AgentMonster.FillAnimationSystemData(MBGlobals.GetActionSetWithSuffix(agentBuildData2.AgentMonster, agentBuildData2.AgentIsFemale, "_poses"), character.GetStepSize(), hasClippingPlane: false);
		agent.SetActionSet(ref animationSystemData);
		if (characterData.Character == CharacterObject.PlayerCharacter)
		{
			agent.AgentVisuals.GetSkeleton().TickAnimationsAndForceUpdate(0.1f, initialFrame, tickAnimsForChildren: true);
		}
		if (characterData.SpawnedAfterFight)
		{
			_addBloodToAgents.Add(agent);
		}
		else if (agent.MountAgent == null)
		{
			agent.SetActionChannel(0, conversationAction, ignorePriority: false, 0uL, 0f, 1f, 0f, 0.4f, MBRandom.RandomFloat * 0.8f);
		}
	}

	private MatrixFrame GetBodyguardSpawnFrame(bool spawnWithHorse, bool isDefenderSide)
	{
		MatrixFrame result = MatrixFrame.Identity;
		foreach (GameEntity child in _conversationSet.GetChildren())
		{
			if (!isDefenderSide)
			{
				if (spawnWithHorse && child.HasTag("player_bodyguard_cavalry_spawn") && !_usedSpawnPoints.Contains(child))
				{
					_usedSpawnPoints.Add(child);
					result = child.GetGlobalFrame();
					break;
				}
				if (child.HasTag("player_bodyguard_infantry_spawn") && !_usedSpawnPoints.Contains(child))
				{
					_usedSpawnPoints.Add(child);
					result = child.GetGlobalFrame();
					break;
				}
			}
			else
			{
				if (spawnWithHorse && child.HasTag("opponent_bodyguard_cavalry_spawn") && !_usedSpawnPoints.Contains(child))
				{
					_usedSpawnPoints.Add(child);
					result = child.GetGlobalFrame();
					break;
				}
				if (child.HasTag("opponent_bodyguard_infantry_spawn") && !_usedSpawnPoints.Contains(child))
				{
					_usedSpawnPoints.Add(child);
					result = child.GetGlobalFrame();
					break;
				}
			}
		}
		result.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		return result;
	}

	protected override void OnEndMission()
	{
		_conversationSet = null;
		base.Mission.CameraIsFirstPerson = _realCameraController;
	}
}
