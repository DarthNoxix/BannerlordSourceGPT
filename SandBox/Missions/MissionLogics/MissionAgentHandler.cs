using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Helpers;
using SandBox.Missions.AgentBehaviors;
using SandBox.Objects.AnimationPoints;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.MountAndBlade.Source.Objects;
using TaleWorlds.ObjectSystem;

namespace SandBox.Missions.MissionLogics;

public class MissionAgentHandler : MissionLogic
{
	private const float PassageUsageDeltaTime = 30f;

	private static readonly uint[] _tournamentTeamColors = new uint[11]
	{
		4294110933u, 4290269521u, 4291535494u, 4286151096u, 4290286497u, 4291600739u, 4291868275u, 4287285710u, 4283204487u, 4287282028u,
		4290300789u
	};

	private static readonly uint[] _villagerClothColors = new uint[35]
	{
		4292860590u, 4291351206u, 4289117081u, 4288460959u, 4287541416u, 4288922566u, 4292654718u, 4289243320u, 4290286483u, 4290288531u,
		4290156159u, 4291136871u, 4289233774u, 4291205980u, 4291735684u, 4292722283u, 4293119406u, 4293911751u, 4294110933u, 4291535494u,
		4289955192u, 4289631650u, 4292133587u, 4288785593u, 4286288275u, 4286222496u, 4287601851u, 4286622134u, 4285898909u, 4285638289u,
		4289830302u, 4287593853u, 4289957781u, 4287071646u, 4284445583u
	};

	private static int _disabledFaceId = -1;

	private static int _disabledFaceIdForAnimals = 1;

	private readonly Dictionary<string, List<UsableMachine>> _usablePoints;

	private readonly Dictionary<string, List<UsableMachine>> _pairedUsablePoints;

	private List<UsableMachine> _disabledPassages;

	private readonly Location _previousLocation;

	private readonly Location _currentLocation;

	private readonly string _playerSpecialSpawnTag;

	private BasicMissionTimer _checkPossibleQuestTimer;

	private float _passageUsageTime;

	private CharacterObject _conversationCharacter;

	public List<UsableMachine> TownPassageProps
	{
		get
		{
			_usablePoints.TryGetValue("npc_passage", out var value);
			return value;
		}
	}

	public bool HasPassages()
	{
		if (_usablePoints.TryGetValue("npc_passage", out var value))
		{
			return value.Count > 0;
		}
		return false;
	}

	public MissionAgentHandler(Location location, CharacterObject conversationCharacter, string playerSpecialSpawnTag = null)
	{
		_currentLocation = location;
		_previousLocation = ((Campaign.Current.GameMode == CampaignGameMode.Campaign) ? Campaign.Current.GameMenuManager.PreviousLocation : null);
		if (_previousLocation != null && !_currentLocation.LocationsOfPassages.Contains(_previousLocation))
		{
			TaleWorlds.Library.Debug.FailedAssert(string.Concat("No passage from ", _previousLocation.DoorName, " to ", _currentLocation.DoorName), "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\MissionAgentHandler.cs", ".ctor", 77);
			_previousLocation = null;
		}
		_usablePoints = new Dictionary<string, List<UsableMachine>>();
		_pairedUsablePoints = new Dictionary<string, List<UsableMachine>>();
		_disabledPassages = new List<UsableMachine>();
		_checkPossibleQuestTimer = new BasicMissionTimer();
		_playerSpecialSpawnTag = playerSpecialSpawnTag;
		_conversationCharacter = conversationCharacter;
	}

	public override void OnCreated()
	{
		if (_currentLocation != null)
		{
			CampaignMission.Current.Location = _currentLocation;
		}
	}

	public override void EarlyStart()
	{
		_passageUsageTime = base.Mission.CurrentTime + 30f;
		GetAllProps();
		MapWeatherModel.WeatherEvent weatherEventInPosition = Campaign.Current.Models.MapWeatherModel.GetWeatherEventInPosition(Settlement.CurrentSettlement.Position2D);
		if (weatherEventInPosition != MapWeatherModel.WeatherEvent.HeavyRain && weatherEventInPosition != MapWeatherModel.WeatherEvent.Blizzard)
		{
			InitializePairedUsableObjects();
		}
		base.Mission.SetReportStuckAgentsMode(value: true);
	}

	public override void OnRenderingStarted()
	{
	}

	public override void OnMissionTick(float dt)
	{
		float currentTime = base.Mission.CurrentTime;
		if (currentTime > _passageUsageTime)
		{
			_passageUsageTime = currentTime + 30f;
			if (PlayerEncounter.LocationEncounter != null && LocationComplex.Current != null)
			{
				LocationComplex.Current.AgentPassageUsageTick();
			}
		}
	}

	public override void OnRemoveBehavior()
	{
		foreach (Location listOfLocation in LocationComplex.Current.GetListOfLocations())
		{
			if (listOfLocation.StringId == "center" || listOfLocation.StringId == "village_center" || listOfLocation.StringId == "lordshall" || listOfLocation.StringId == "prison" || listOfLocation.StringId == "tavern" || listOfLocation.StringId == "alley")
			{
				listOfLocation.RemoveAllCharacters((LocationCharacter x) => !x.Character.IsHero);
			}
		}
		base.OnRemoveBehavior();
	}

	public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
	{
		if (affectedAgent.IsHuman && (agentState == AgentState.Killed || agentState == AgentState.Unconscious))
		{
			LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(affectedAgent.Origin);
			if (locationCharacter != null)
			{
				CampaignMission.Current.Location.RemoveLocationCharacter(locationCharacter);
				if (PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(locationCharacter) != null && affectedAgent.State == AgentState.Killed)
				{
					PlayerEncounter.LocationEncounter.RemoveAccompanyingCharacter(locationCharacter);
				}
			}
		}
		foreach (Agent agent in base.Mission.Agents)
		{
			agent.GetComponent<CampaignAgentComponent>()?.OnAgentRemoved(affectedAgent);
		}
	}

	public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
	{
		if (atStart)
		{
			return;
		}
		foreach (Agent agent in base.Mission.Agents)
		{
			if (agent.IsHuman)
			{
				agent.SetAgentExcludeStateForFaceGroupId(_disabledFaceId, agent.CurrentWatchState != Agent.WatchState.Alarmed);
			}
		}
	}

	private void InitializePairedUsableObjects()
	{
		Dictionary<string, List<UsableMachine>> dictionary = new Dictionary<string, List<UsableMachine>>();
		foreach (KeyValuePair<string, List<UsableMachine>> usablePoint in _usablePoints)
		{
			foreach (UsableMachine item in usablePoint.Value)
			{
				foreach (StandingPoint standingPoint in item.StandingPoints)
				{
					if (!(standingPoint is AnimationPoint animationPoint) || !(animationPoint.PairEntity != null))
					{
						continue;
					}
					if (_pairedUsablePoints.ContainsKey(usablePoint.Key))
					{
						if (!_pairedUsablePoints[usablePoint.Key].Contains(item))
						{
							_pairedUsablePoints[usablePoint.Key].Add(item);
						}
					}
					else
					{
						_pairedUsablePoints.Add(usablePoint.Key, new List<UsableMachine> { item });
					}
					if (dictionary.ContainsKey(usablePoint.Key))
					{
						dictionary[usablePoint.Key].Add(item);
						continue;
					}
					dictionary.Add(usablePoint.Key, new List<UsableMachine> { item });
				}
			}
		}
		foreach (KeyValuePair<string, List<UsableMachine>> item2 in dictionary)
		{
			foreach (KeyValuePair<string, List<UsableMachine>> usablePoint2 in _usablePoints)
			{
				foreach (UsableMachine item3 in dictionary[item2.Key])
				{
					usablePoint2.Value.Remove(item3);
				}
			}
		}
	}

	private void GetAllProps()
	{
		GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("navigation_mesh_deactivator");
		if (gameEntity != null)
		{
			NavigationMeshDeactivator firstScriptOfType = gameEntity.GetFirstScriptOfType<NavigationMeshDeactivator>();
			_disabledFaceId = firstScriptOfType.DisableFaceWithId;
			_disabledFaceIdForAnimals = firstScriptOfType.DisableFaceWithIdForAnimals;
		}
		_usablePoints.Clear();
		foreach (UsableMachine item in base.Mission.MissionObjects.FindAllWithType<UsableMachine>())
		{
			string[] tags = item.GameEntity.Tags;
			foreach (string key in tags)
			{
				if (!_usablePoints.ContainsKey(key))
				{
					_usablePoints.Add(key, new List<UsableMachine>());
				}
				_usablePoints[key].Add(item);
			}
		}
		if (Settlement.CurrentSettlement.IsTown || Settlement.CurrentSettlement.IsVillage)
		{
			foreach (AreaMarker item2 in base.Mission.ActiveMissionObjects.FindAllWithType<AreaMarker>().ToList())
			{
				string tag = item2.Tag;
				List<UsableMachine> usableMachinesInRange = item2.GetUsableMachinesInRange(item2.Tag.Contains("workshop") ? "unaffected_by_area" : null);
				if (!_usablePoints.ContainsKey(tag))
				{
					_usablePoints.Add(tag, new List<UsableMachine>());
				}
				foreach (UsableMachine item3 in usableMachinesInRange)
				{
					foreach (KeyValuePair<string, List<UsableMachine>> usablePoint in _usablePoints)
					{
						if (usablePoint.Value.Contains(item3))
						{
							usablePoint.Value.Remove(item3);
						}
					}
					if (item3.GameEntity.HasTag("hold_tag_always"))
					{
						string text = item3.GameEntity.Tags[0] + "_" + item2.Tag;
						item3.GameEntity.AddTag(text);
						if (!_usablePoints.ContainsKey(text))
						{
							_usablePoints.Add(text, new List<UsableMachine>());
							_usablePoints[text].Add(item3);
						}
						else
						{
							_usablePoints[text].Add(item3);
						}
						continue;
					}
					foreach (UsableMachine item4 in usableMachinesInRange)
					{
						if (!item4.GameEntity.HasTag(tag))
						{
							item4.GameEntity.AddTag(tag);
						}
					}
				}
				if (_usablePoints.ContainsKey(tag))
				{
					usableMachinesInRange.RemoveAll((UsableMachine x) => _usablePoints[tag].Contains(x));
					if (usableMachinesInRange.Count > 0)
					{
						_usablePoints[tag].AddRange(usableMachinesInRange);
					}
				}
				foreach (UsableMachine item5 in item2.GetUsableMachinesWithTagInRange("unaffected_by_area"))
				{
					string key2 = item5.GameEntity.Tags[0];
					foreach (KeyValuePair<string, List<UsableMachine>> usablePoint2 in _usablePoints)
					{
						if (usablePoint2.Value.Contains(item5))
						{
							usablePoint2.Value.Remove(item5);
						}
					}
					if (_usablePoints.ContainsKey(key2))
					{
						_usablePoints[key2].Add(item5);
						continue;
					}
					_usablePoints.Add(key2, new List<UsableMachine>());
					_usablePoints[key2].Add(item5);
				}
			}
		}
		DisableUnavailableWaypoints();
		RemoveDeactivatedUsablePlacesFromList();
	}

	[Conditional("DEBUG")]
	public void DetectMissingEntities()
	{
		if (CampaignMission.Current.Location == null || Utilities.CommandLineArgumentExists("CampaignGameplayTest"))
		{
			return;
		}
		IEnumerable<LocationCharacter> characterList = CampaignMission.Current.Location.GetCharacterList();
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (LocationCharacter item in characterList)
		{
			if (item.SpecialTargetTag != null && !item.IsHidden)
			{
				if (dictionary.ContainsKey(item.SpecialTargetTag))
				{
					dictionary[item.SpecialTargetTag]++;
				}
				else
				{
					dictionary.Add(item.SpecialTargetTag, 1);
				}
			}
		}
		foreach (KeyValuePair<string, int> item2 in dictionary)
		{
			string key = item2.Key;
			int value = item2.Value;
			int num = 0;
			if (_usablePoints.ContainsKey(key))
			{
				num += _usablePoints[key].Count;
				foreach (UsableMachine item3 in _usablePoints[key])
				{
					num += GetPointCountOfUsableMachine(item3, checkForUnusedOnes: false);
				}
			}
			if (_pairedUsablePoints.ContainsKey(key))
			{
				num += _pairedUsablePoints[key].Count;
				foreach (UsableMachine item4 in _pairedUsablePoints[key])
				{
					num += GetPointCountOfUsableMachine(item4, checkForUnusedOnes: false);
				}
			}
			if (num < value)
			{
				_ = "Trying to spawn " + value + " npc with \"" + key + "\" but there are " + num + " suitable spawn points in scene " + base.Mission.SceneName;
				if (TestCommonBase.BaseInstance != null)
				{
					_ = TestCommonBase.BaseInstance.IsTestEnabled;
				}
			}
		}
	}

	public void RemoveDeactivatedUsablePlacesFromList()
	{
		Dictionary<string, List<UsableMachine>> dictionary = new Dictionary<string, List<UsableMachine>>();
		foreach (KeyValuePair<string, List<UsableMachine>> usablePoint in _usablePoints)
		{
			foreach (UsableMachine item in usablePoint.Value)
			{
				if (item.IsDeactivated)
				{
					if (dictionary.ContainsKey(usablePoint.Key))
					{
						dictionary[usablePoint.Key].Add(item);
						continue;
					}
					dictionary.Add(usablePoint.Key, new List<UsableMachine>());
					dictionary[usablePoint.Key].Add(item);
				}
			}
		}
		foreach (KeyValuePair<string, List<UsableMachine>> item2 in dictionary)
		{
			foreach (UsableMachine item3 in item2.Value)
			{
				_usablePoints[item2.Key].Remove(item3);
			}
		}
	}

	private Dictionary<string, int> FindUnusedUsablePointCount()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (KeyValuePair<string, List<UsableMachine>> usablePoint in _usablePoints)
		{
			int num = 0;
			foreach (UsableMachine item in usablePoint.Value)
			{
				num += GetPointCountOfUsableMachine(item, checkForUnusedOnes: true);
			}
			if (num > 0)
			{
				dictionary.Add(usablePoint.Key, num);
			}
		}
		foreach (KeyValuePair<string, List<UsableMachine>> pairedUsablePoint in _pairedUsablePoints)
		{
			int num2 = 0;
			foreach (UsableMachine item2 in pairedUsablePoint.Value)
			{
				num2 += GetPointCountOfUsableMachine(item2, checkForUnusedOnes: true);
			}
			if (num2 > 0)
			{
				if (!dictionary.ContainsKey(pairedUsablePoint.Key))
				{
					dictionary.Add(pairedUsablePoint.Key, num2);
				}
				else
				{
					dictionary[pairedUsablePoint.Key] += num2;
				}
			}
		}
		return dictionary;
	}

	private CharacterObject GetPlayerCharacter()
	{
		CharacterObject characterObject = CharacterObject.PlayerCharacter;
		if (characterObject == null)
		{
			characterObject = Game.Current.ObjectManager.GetObject<CharacterObject>("main_hero_for_perf");
		}
		return characterObject;
	}

	public void SpawnPlayer(bool civilianEquipment = false, bool noHorses = false, bool noWeapon = false, bool wieldInitialWeapons = false, bool isStealth = false, string spawnTag = "")
	{
		if (Campaign.Current.GameMode != CampaignGameMode.Campaign)
		{
			civilianEquipment = false;
		}
		MatrixFrame matrixFrame = MatrixFrame.Identity;
		GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("spawnpoint_player");
		if (gameEntity != null)
		{
			matrixFrame = gameEntity.GetGlobalFrame();
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		}
		bool flag = Campaign.Current.GameMode == CampaignGameMode.Campaign && PlayerEncounter.IsActive && (Settlement.CurrentSettlement.IsTown || Settlement.CurrentSettlement.IsCastle) && !Campaign.Current.IsNight && CampaignMission.Current.Location.StringId == "center" && !PlayerEncounter.LocationEncounter.IsInsideOfASettlement;
		bool flag2 = false;
		if (_playerSpecialSpawnTag != null)
		{
			GameEntity gameEntity2 = null;
			UsableMachine usableMachine = GetAllUsablePointsWithTag(_playerSpecialSpawnTag).FirstOrDefault();
			if (usableMachine != null)
			{
				gameEntity2 = usableMachine.StandingPoints.FirstOrDefault()?.GameEntity;
			}
			if (gameEntity2 == null)
			{
				gameEntity2 = base.Mission.Scene.FindEntityWithTag(_playerSpecialSpawnTag);
			}
			if (gameEntity2 != null)
			{
				matrixFrame = gameEntity2.GetGlobalFrame();
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			}
		}
		else if (CampaignMission.Current.Location.StringId == "arena")
		{
			GameEntity gameEntity3 = base.Mission.Scene.FindEntityWithTag("sp_player_near_arena_master");
			if (gameEntity3 != null)
			{
				matrixFrame = gameEntity3.GetGlobalFrame();
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			}
		}
		else if (_previousLocation != null)
		{
			matrixFrame = GetSpawnFrameOfPassage(_previousLocation);
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			noHorses = true;
			flag2 = true;
		}
		else if (flag)
		{
			GameEntity gameEntity4 = base.Mission.Scene.FindEntityWithTag(isStealth ? "sp_player_stealth" : "spawnpoint_player_outside");
			if (gameEntity4 != null)
			{
				matrixFrame = gameEntity4.GetGlobalFrame();
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			}
		}
		else
		{
			GameEntity gameEntity5 = base.Mission.Scene.FindEntityWithTag("spawnpoint_player");
			if (gameEntity5 != null)
			{
				matrixFrame = gameEntity5.GetGlobalFrame();
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			}
		}
		if (PlayerEncounter.LocationEncounter is TownEncounter)
		{
			PlayerEncounter.LocationEncounter.IsInsideOfASettlement = true;
		}
		CharacterObject playerCharacter = GetPlayerCharacter();
		AgentBuildData agentBuildData = new AgentBuildData(playerCharacter).Team(base.Mission.PlayerTeam).InitialPosition(in matrixFrame.origin);
		Vec2 direction = matrixFrame.rotation.f.AsVec2.Normalized();
		AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(in direction).CivilianEquipment(civilianEquipment).NoHorses(noHorses)
			.NoWeapons(noWeapon)
			.ClothingColor1(base.Mission.PlayerTeam.Color)
			.ClothingColor2(base.Mission.PlayerTeam.Color2)
			.TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, GetPlayerCharacter()))
			.MountKey(MountCreationKey.GetRandomMountKeyString(playerCharacter.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, playerCharacter.GetMountKeySeed()))
			.Controller(Agent.ControllerType.Player);
		if (playerCharacter.HeroObject?.ClanBanner != null)
		{
			agentBuildData2.Banner(playerCharacter.HeroObject.ClanBanner);
		}
		if (Campaign.Current.GameMode != CampaignGameMode.Campaign)
		{
			agentBuildData2.TroopOrigin(new SimpleAgentOrigin(CharacterObject.PlayerCharacter));
		}
		if (isStealth)
		{
			agentBuildData2.Equipment(GetStealthEquipmentForPlayer());
		}
		else if (Campaign.Current.IsMainHeroDisguised)
		{
			Equipment defaultEquipment = MBObjectManager.Instance.GetObject<MBEquipmentRoster>("npc_disguised_hero_equipment_template").DefaultEquipment;
			Equipment firstCivilianEquipment = CharacterObject.PlayerCharacter.FirstCivilianEquipment;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				ItemObject item = firstCivilianEquipment[equipmentIndex].Item;
				defaultEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, new EquipmentElement(item));
			}
			agentBuildData2.Equipment(defaultEquipment);
		}
		Agent agent = base.Mission.SpawnAgent(agentBuildData2);
		if (wieldInitialWeapons)
		{
			agent.WieldInitialWeapons();
		}
		if (flag2)
		{
			base.Mission.MakeSound(MiscSoundContainer.SoundCodeMovementFoleyDoorClose, matrixFrame.origin, soundCanBePredicted: true, isReliable: false, -1, -1);
		}
		SpawnCharactersAccompanyingPlayer(noHorses);
		for (int i = 0; i < 3; i++)
		{
			Agent.Main.AgentVisuals.GetSkeleton().TickAnimations(0.1f, Agent.Main.AgentVisuals.GetGlobalFrame(), tickAnimsForChildren: true);
		}
	}

	private Equipment GetStealthEquipmentForPlayer()
	{
		Equipment equipment = new Equipment();
		equipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Body, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("ragged_robes")));
		equipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.NumAllWeaponSlots, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("pilgrim_hood")));
		equipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Leg, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("ragged_boots")));
		equipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Gloves, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("ragged_armwraps")));
		for (int i = 0; i < 5; i++)
		{
			EquipmentElement equipmentFromSlot = CharacterObject.PlayerCharacter.Equipment.GetEquipmentFromSlot((EquipmentIndex)i);
			if (equipmentFromSlot.Item != null)
			{
				equipment.AddEquipmentToSlotWithoutAgent((EquipmentIndex)i, new EquipmentElement(equipmentFromSlot.Item));
			}
			else if (i >= 0 && i <= 3)
			{
				equipment.AddEquipmentToSlotWithoutAgent((EquipmentIndex)i, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>("throwing_stone")));
			}
		}
		return equipment;
	}

	private MatrixFrame GetSpawnFrameOfPassage(Location location)
	{
		MatrixFrame result = MatrixFrame.Identity;
		UsableMachine usableMachine = TownPassageProps.FirstOrDefault((UsableMachine x) => ((Passage)x).ToLocation == location) ?? _disabledPassages.FirstOrDefault((UsableMachine x) => ((Passage)x).ToLocation == location);
		if (usableMachine != null)
		{
			MatrixFrame globalFrame = usableMachine.PilotStandingPoint.GameEntity.GetGlobalFrame();
			globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			globalFrame.origin.z = base.Mission.Scene.GetGroundHeightAtPosition(globalFrame.origin);
			globalFrame.rotation.RotateAboutUp((float)Math.PI);
			result = globalFrame;
		}
		return result;
	}

	public void DisableUnavailableWaypoints()
	{
		bool isNight = Campaign.Current.IsNight;
		string text = "";
		int num = 0;
		foreach (KeyValuePair<string, List<UsableMachine>> usablePoint in _usablePoints)
		{
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < usablePoint.Value.Count; i++)
			{
				UsableMachine usableMachine = usablePoint.Value[i];
				if (!Mission.Current.IsPositionInsideBoundaries(usableMachine.GameEntity.GlobalPosition.AsVec2))
				{
					foreach (StandingPoint standingPoint in usableMachine.StandingPoints)
					{
						standingPoint.IsDeactivated = true;
						num++;
					}
				}
				if (usableMachine is Chair)
				{
					foreach (StandingPoint standingPoint2 in usableMachine.StandingPoints)
					{
						Vec3 origin = standingPoint2.GameEntity.GetGlobalFrame().origin;
						PathFaceRecord record = PathFaceRecord.NullFaceRecord;
						base.Mission.Scene.GetNavMeshFaceIndex(ref record, origin, checkIfDisabled: true);
						if (!record.IsValid() || (_disabledFaceId != -1 && record.FaceGroupIndex == _disabledFaceId))
						{
							standingPoint2.IsDeactivated = true;
							num2++;
						}
					}
				}
				else if (usableMachine is Passage)
				{
					Passage passage = usableMachine as Passage;
					if (passage.ToLocation != null && passage.ToLocation.CanPlayerSee())
					{
						continue;
					}
					foreach (StandingPoint standingPoint3 in passage.StandingPoints)
					{
						standingPoint3.IsDeactivated = true;
					}
					passage.Disable();
					_disabledPassages.Add(usableMachine);
					_ = passage.ToLocation;
					usablePoint.Value.RemoveAt(i);
					i--;
					num3++;
				}
				else
				{
					if (!(usableMachine is UsablePlace))
					{
						continue;
					}
					foreach (StandingPoint standingPoint4 in usableMachine.StandingPoints)
					{
						Vec3 origin2 = standingPoint4.GameEntity.GetGlobalFrame().origin;
						PathFaceRecord record2 = PathFaceRecord.NullFaceRecord;
						base.Mission.Scene.GetNavMeshFaceIndex(ref record2, origin2, checkIfDisabled: true);
						if (!record2.IsValid() || (_disabledFaceId != -1 && record2.FaceGroupIndex == _disabledFaceId) || (isNight && usableMachine.GameEntity.HasTag("disable_at_night")) || (!isNight && usableMachine.GameEntity.HasTag("enable_at_night")))
						{
							standingPoint4.IsDeactivated = true;
							num4++;
						}
					}
				}
			}
			if (num4 + num2 + num3 > 0)
			{
				text = text + "_____________________________________________\n\"" + usablePoint.Key + "\" :\n";
				if (num4 > 0)
				{
					text = text + "Disabled standing point : " + num4 + "\n";
				}
				if (num2 > 0)
				{
					text = text + "Disabled chair use point : " + num2 + "\n";
				}
				if (num3 > 0)
				{
					text = text + "Disabled passage info : " + num3 + "\n";
				}
			}
		}
	}

	public void SpawnLocationCharacters(string overridenTagValue = null)
	{
		Dictionary<string, int> unusedUsablePointCount = FindUnusedUsablePointCount();
		IEnumerable<LocationCharacter> characterList = CampaignMission.Current.Location.GetCharacterList();
		if (PlayerEncounter.LocationEncounter.Settlement.IsTown && CampaignMission.Current.Location == LocationComplex.Current.GetLocationWithId("center"))
		{
			foreach (LocationCharacter character in LocationComplex.Current.GetLocationWithId("alley").GetCharacterList())
			{
				characterList.Append(character);
			}
		}
		CampaignEventDispatcher.Instance.LocationCharactersAreReadyToSpawn(unusedUsablePointCount);
		foreach (LocationCharacter item in characterList)
		{
			if (!IsAlreadySpawned(item.AgentOrigin) && !item.IsHidden)
			{
				if (!string.IsNullOrEmpty(overridenTagValue))
				{
					item.SpecialTargetTag = overridenTagValue;
				}
				SetAgentExcludeFaceGroupIdAux(SpawnLocationCharacter(item), _disabledFaceId);
			}
		}
		List<Passage> list = new List<Passage>();
		if (TownPassageProps != null)
		{
			foreach (UsableMachine townPassageProp in TownPassageProps)
			{
				if (townPassageProp is Passage passage && !townPassageProp.IsDeactivated)
				{
					passage.Deactivate();
					list.Add(passage);
				}
			}
		}
		foreach (Agent agent in base.Mission.Agents)
		{
			SimulateAgent(agent);
		}
		foreach (Passage item2 in list)
		{
			item2.Activate();
		}
		CampaignEventDispatcher.Instance.LocationCharactersSimulated();
	}

	private bool IsAlreadySpawned(IAgentOriginBase agentOrigin)
	{
		if (Mission.Current != null)
		{
			return Mission.Current.Agents.Any((Agent x) => x.Origin == agentOrigin);
		}
		return false;
	}

	public Agent SpawnLocationCharacter(LocationCharacter locationCharacter, bool simulateAgentAfterSpawn = false)
	{
		Agent agent = SpawnWanderingAgent(locationCharacter);
		if (simulateAgentAfterSpawn)
		{
			SimulateAgent(agent);
		}
		if (locationCharacter.IsVisualTracked)
		{
			Mission.Current.GetMissionBehavior<VisualTrackerMissionBehavior>()?.RegisterLocalOnlyObject(agent);
		}
		return agent;
	}

	public void SimulateAgent(Agent agent)
	{
		if (!agent.IsHuman)
		{
			return;
		}
		AgentNavigator agentNavigator = agent.GetComponent<CampaignAgentComponent>().AgentNavigator;
		int num = MBRandom.RandomInt(35, 50);
		agent.PreloadForRendering();
		for (int i = 0; i < num; i++)
		{
			agentNavigator?.Tick(0.1f, isSimulation: true);
			if (agent.IsUsingGameObject)
			{
				agent.CurrentlyUsedGameObject.SimulateTick(0.1f);
			}
		}
	}

	public void SpawnThugs()
	{
		IEnumerable<LocationCharacter> characterList = CampaignMission.Current.Location.GetCharacterList();
		List<MatrixFrame> list = (from x in base.Mission.Scene.FindEntitiesWithTag("spawnpoint_thug")
			select x.GetGlobalFrame()).ToList();
		int num = 0;
		foreach (LocationCharacter item in characterList)
		{
			if (item.CharacterRelation == LocationCharacter.CharacterRelations.Enemy)
			{
				SetAgentExcludeFaceGroupIdAux(SpawnWanderingAgentWithInitialFrame(item, list[num % list.Count]), _disabledFaceId);
				num++;
			}
		}
	}

	private void GetFrameForFollowingAgent(Agent followedAgent, out MatrixFrame frame)
	{
		frame = followedAgent.Frame;
		frame.origin += -(frame.rotation.f * 1.5f);
	}

	public void SpawnCharactersAccompanyingPlayer(bool noHorse)
	{
		int num = 0;
		bool flag = PlayerEncounter.LocationEncounter.CharactersAccompanyingPlayer.Any((AccompanyingCharacter c) => c.IsFollowingPlayerAtMissionStart);
		GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("navigation_mesh_deactivator");
		foreach (AccompanyingCharacter item in PlayerEncounter.LocationEncounter.CharactersAccompanyingPlayer)
		{
			bool flag2 = item.LocationCharacter.Character.IsHero && item.LocationCharacter.Character.HeroObject.IsWounded;
			if ((!_currentLocation.GetCharacterList().Contains(item.LocationCharacter) && flag2) || !item.CanEnterLocation(_currentLocation))
			{
				continue;
			}
			_currentLocation.AddCharacter(item.LocationCharacter);
			if (item.IsFollowingPlayerAtMissionStart || (!flag && num == 0))
			{
				WorldFrame worldFrame = base.Mission.MainAgent.GetWorldFrame();
				worldFrame.Origin.SetVec2(base.Mission.GetRandomPositionAroundPoint(worldFrame.Origin.GetNavMeshVec3(), 0.5f, 2f).AsVec2);
				Agent agent = SpawnWanderingAgentWithInitialFrame(item.LocationCharacter, worldFrame.ToGroundMatrixFrame(), noHorse);
				if (gameEntity != null)
				{
					int disableFaceWithId = gameEntity.GetFirstScriptOfType<NavigationMeshDeactivator>().DisableFaceWithId;
					if (disableFaceWithId != -1)
					{
						agent.SetAgentExcludeStateForFaceGroupId(disableFaceWithId, isExcluded: false);
					}
				}
				int num2 = 0;
				while (true)
				{
					Vec2 position = base.Mission.MainAgent.Position.AsVec2;
					if (agent.CanMoveDirectlyToPosition(in position) || num2 >= 50)
					{
						break;
					}
					worldFrame.Origin.SetVec2(base.Mission.GetRandomPositionAroundPoint(worldFrame.Origin.GetNavMeshVec3(), 0.5f, 4f).AsVec2);
					agent.TeleportToPosition(worldFrame.ToGroundMatrixFrame().origin);
					num2++;
				}
				agent.SetTeam(base.Mission.PlayerTeam, sync: true);
				num++;
			}
			else
			{
				SpawnWanderingAgent(item.LocationCharacter).SetTeam(base.Mission.PlayerTeam, sync: true);
			}
			foreach (Agent agent2 in base.Mission.Agents)
			{
				LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(agent2.Origin);
				AccompanyingCharacter accompanyingCharacter = PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(locationCharacter);
				if (agent2.GetComponent<CampaignAgentComponent>().AgentNavigator != null && accompanyingCharacter != null)
				{
					DailyBehaviorGroup behaviorGroup = agent2.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
					if (item.IsFollowingPlayerAtMissionStart)
					{
						FollowAgentBehavior obj = behaviorGroup.GetBehavior<FollowAgentBehavior>() ?? behaviorGroup.AddBehavior<FollowAgentBehavior>();
						behaviorGroup.SetScriptedBehavior<FollowAgentBehavior>();
						obj.SetTargetAgent(Agent.Main);
					}
					else
					{
						behaviorGroup.Behaviors.Clear();
					}
				}
			}
		}
	}

	public void FadeoutExitingLocationCharacter(LocationCharacter locationCharacter)
	{
		foreach (Agent agent in Mission.Current.Agents)
		{
			if ((CharacterObject)agent.Character == locationCharacter.Character)
			{
				agent.FadeOut(hideInstantly: false, hideMount: true);
				break;
			}
		}
	}

	public void SpawnEnteringLocationCharacter(LocationCharacter locationCharacter, Location fromLocation)
	{
		if (fromLocation != null)
		{
			bool flag = false;
			{
				foreach (UsableMachine townPassageProp in TownPassageProps)
				{
					Passage passage = townPassageProp as Passage;
					if (passage.ToLocation == fromLocation)
					{
						MatrixFrame globalFrame = passage.PilotStandingPoint.GameEntity.GetGlobalFrame();
						globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
						globalFrame.origin.z = base.Mission.Scene.GetGroundHeightAtPosition(globalFrame.origin);
						Vec3 f = globalFrame.rotation.f;
						f.Normalize();
						globalFrame.origin -= 0.3f * f;
						globalFrame.rotation.RotateAboutUp((float)Math.PI);
						Agent agent = SpawnWanderingAgentWithInitialFrame(locationCharacter, globalFrame);
						SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceId);
						base.Mission.MakeSound(MiscSoundContainer.SoundCodeMovementFoleyDoorClose, globalFrame.origin, soundCanBePredicted: true, isReliable: false, -1, -1);
						agent.FadeIn();
						flag = true;
						break;
					}
				}
				return;
			}
		}
		SpawnLocationCharacter(locationCharacter, simulateAgentAfterSpawn: true);
	}

	private static void SimulateAnimalAnimations(Agent agent)
	{
		int num = 10 + MBRandom.RandomInt(90);
		for (int i = 0; i < num; i++)
		{
			agent.TickActionChannels(0.1f);
			Vec3 vec = agent.ComputeAnimationDisplacement(0.1f);
			if (vec.LengthSquared > 0f)
			{
				agent.TeleportToPosition(agent.Position + vec);
			}
			agent.AgentVisuals.GetSkeleton().TickAnimations(0.1f, agent.AgentVisuals.GetGlobalFrame(), tickAnimsForChildren: true);
		}
	}

	public static void SpawnSheeps()
	{
		foreach (GameEntity item in Mission.Current.Scene.FindEntitiesWithTag("sp_sheep"))
		{
			MatrixFrame globalFrame = item.GetGlobalFrame();
			ItemRosterElement rosterElement = new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>("sheep"));
			globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			Mission current2 = Mission.Current;
			ref Vec3 origin = ref globalFrame.origin;
			Vec2 initialDirection = globalFrame.rotation.f.AsVec2;
			Agent agent = current2.SpawnMonster(rosterElement, default(ItemRosterElement), in origin, in initialDirection);
			SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceId);
			SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceIdForAnimals);
			AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(item, agent);
			SimulateAnimalAnimations(agent);
		}
	}

	public static void SpawnCows()
	{
		foreach (GameEntity item in Mission.Current.Scene.FindEntitiesWithTag("sp_cow"))
		{
			MatrixFrame globalFrame = item.GetGlobalFrame();
			ItemRosterElement rosterElement = new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>("cow"));
			globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			Mission current2 = Mission.Current;
			ref Vec3 origin = ref globalFrame.origin;
			Vec2 initialDirection = globalFrame.rotation.f.AsVec2;
			Agent agent = current2.SpawnMonster(rosterElement, default(ItemRosterElement), in origin, in initialDirection);
			SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceId);
			SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceIdForAnimals);
			AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(item, agent);
			SimulateAnimalAnimations(agent);
		}
	}

	public static void SpawnGeese()
	{
		foreach (GameEntity item in Mission.Current.Scene.FindEntitiesWithTag("sp_goose"))
		{
			MatrixFrame globalFrame = item.GetGlobalFrame();
			ItemRosterElement rosterElement = new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>("goose"));
			globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			Mission current2 = Mission.Current;
			ref Vec3 origin = ref globalFrame.origin;
			Vec2 initialDirection = globalFrame.rotation.f.AsVec2;
			Agent agent = current2.SpawnMonster(rosterElement, default(ItemRosterElement), in origin, in initialDirection);
			SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceId);
			SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceIdForAnimals);
			AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(item, agent);
			SimulateAnimalAnimations(agent);
		}
	}

	public static void SpawnChicken()
	{
		foreach (GameEntity item in Mission.Current.Scene.FindEntitiesWithTag("sp_chicken"))
		{
			MatrixFrame globalFrame = item.GetGlobalFrame();
			ItemRosterElement rosterElement = new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>("chicken"));
			globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			Mission current2 = Mission.Current;
			ref Vec3 origin = ref globalFrame.origin;
			Vec2 initialDirection = globalFrame.rotation.f.AsVec2;
			Agent agent = current2.SpawnMonster(rosterElement, default(ItemRosterElement), in origin, in initialDirection);
			SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceId);
			SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceIdForAnimals);
			AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(item, agent);
			SimulateAnimalAnimations(agent);
		}
	}

	public static void SpawnHogs()
	{
		foreach (GameEntity item in Mission.Current.Scene.FindEntitiesWithTag("sp_hog"))
		{
			MatrixFrame globalFrame = item.GetGlobalFrame();
			ItemRosterElement rosterElement = new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>("hog"));
			globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			Mission current2 = Mission.Current;
			ref Vec3 origin = ref globalFrame.origin;
			Vec2 initialDirection = globalFrame.rotation.f.AsVec2;
			Agent agent = current2.SpawnMonster(rosterElement, default(ItemRosterElement), in origin, in initialDirection);
			SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceId);
			SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceIdForAnimals);
			AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(item, agent);
			SimulateAnimalAnimations(agent);
		}
	}

	public static List<Agent> SpawnHorses()
	{
		List<Agent> list = new List<Agent>();
		foreach (GameEntity item in Mission.Current.Scene.FindEntitiesWithTag("sp_horse"))
		{
			MatrixFrame globalFrame = item.GetGlobalFrame();
			string objectName = item.Tags[1];
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(objectName);
			ItemRosterElement rosterElement = new ItemRosterElement(@object, 1);
			ItemRosterElement harnessRosterElement = default(ItemRosterElement);
			if (@object.HasHorseComponent)
			{
				globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				Mission current2 = Mission.Current;
				ref Vec3 origin = ref globalFrame.origin;
				Vec2 initialDirection = globalFrame.rotation.f.AsVec2;
				Agent agent = current2.SpawnMonster(rosterElement, harnessRosterElement, in origin, in initialDirection);
				AnimalSpawnSettings.CheckAndSetAnimalAgentFlags(item, agent);
				SimulateAnimalAnimations(agent);
				list.Add(agent);
			}
		}
		return list;
	}

	public IEnumerable<string> GetAllSpawnTags()
	{
		return _usablePoints.Keys.ToList().Concat(_pairedUsablePoints.Keys.ToList());
	}

	public List<UsableMachine> GetAllUsablePointsWithTag(string tag)
	{
		List<UsableMachine> list = new List<UsableMachine>();
		List<UsableMachine> value = new List<UsableMachine>();
		if (_usablePoints.TryGetValue(tag, out value))
		{
			list.AddRange(value);
		}
		List<UsableMachine> value2 = new List<UsableMachine>();
		if (_pairedUsablePoints.TryGetValue(tag, out value2))
		{
			list.AddRange(value2);
		}
		return list;
	}

	private bool GetSpawnDataForTag(string targetTag, out MatrixFrame spawnFrame, out UsableMachine usableMachine)
	{
		List<UsableMachine> allUsablePointsWithTag = GetAllUsablePointsWithTag(targetTag);
		spawnFrame = MatrixFrame.Identity;
		usableMachine = null;
		if (allUsablePointsWithTag.Count > 0)
		{
			foreach (UsableMachine item in allUsablePointsWithTag)
			{
				if (GetSpawnFrameFromUsableMachine(item, out var frame))
				{
					spawnFrame = frame;
					usableMachine = item;
					return true;
				}
			}
		}
		return false;
	}

	private bool GetSpawnDataInUsablePointsList(Dictionary<string, List<UsableMachine>> list, out MatrixFrame spawnFrame, out UsableMachine usableMachine)
	{
		spawnFrame = MatrixFrame.Identity;
		usableMachine = null;
		foreach (KeyValuePair<string, List<UsableMachine>> item in list)
		{
			if (item.Value.Count <= 0)
			{
				continue;
			}
			foreach (UsableMachine item2 in item.Value)
			{
				if (GetSpawnFrameFromUsableMachine(item2, out var frame))
				{
					spawnFrame = frame;
					usableMachine = item2;
					return true;
				}
			}
		}
		return false;
	}

	public Agent SpawnWanderingAgent(LocationCharacter locationCharacter)
	{
		bool flag = false;
		MatrixFrame spawnFrame = MatrixFrame.Identity;
		UsableMachine usableMachine = null;
		if (locationCharacter.SpecialTargetTag != null)
		{
			flag = GetSpawnDataForTag(locationCharacter.SpecialTargetTag, out spawnFrame, out usableMachine);
		}
		if (!flag)
		{
			flag = GetSpawnDataForTag("npc_common_limited", out spawnFrame, out usableMachine);
		}
		if (!flag)
		{
			flag = GetSpawnDataForTag("npc_common", out spawnFrame, out usableMachine);
		}
		if (!flag && _usablePoints.Count > 0)
		{
			flag = GetSpawnDataInUsablePointsList(_usablePoints, out spawnFrame, out usableMachine);
		}
		if (!flag && _pairedUsablePoints.Count > 0)
		{
			flag = GetSpawnDataInUsablePointsList(_pairedUsablePoints, out spawnFrame, out usableMachine);
		}
		spawnFrame.rotation.f.z = 0f;
		spawnFrame.rotation.f.Normalize();
		spawnFrame.rotation.u = Vec3.Up;
		spawnFrame.rotation.s = Vec3.CrossProduct(spawnFrame.rotation.f, spawnFrame.rotation.u);
		spawnFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		Agent agent = null;
		agent = ((usableMachine == null || locationCharacter.Character == _conversationCharacter) ? SpawnWanderingAgentWithInitialFrame(locationCharacter, spawnFrame) : SpawnWanderingAgentWithUsableMachine(locationCharacter, usableMachine));
		SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceId);
		return agent;
	}

	private bool GetSpawnFrameFromUsableMachine(UsableMachine usableMachine, out MatrixFrame frame)
	{
		frame = MatrixFrame.Identity;
		StandingPoint randomElementWithPredicate = usableMachine.StandingPoints.GetRandomElementWithPredicate((StandingPoint x) => !x.HasUser && !x.IsDeactivated && !x.IsDisabled);
		if (randomElementWithPredicate != null)
		{
			frame = randomElementWithPredicate.GameEntity.GetGlobalFrame();
			return true;
		}
		return false;
	}

	private Agent SpawnWanderingAgentWithUsableMachine(LocationCharacter locationCharacter, UsableMachine usableMachine)
	{
		GetSpawnFrameFromUsableMachine(usableMachine, out var frame);
		Agent agent = SpawnWanderingAgentWithInitialFrame(locationCharacter, frame);
		agent.GetComponent<CampaignAgentComponent>().AgentNavigator.SetTarget(usableMachine, isInitialTarget: true);
		return agent;
	}

	private Agent SpawnWanderingAgentWithInitialFrame(LocationCharacter locationCharacter, MatrixFrame spawnPointFrame, bool noHorses = true)
	{
		Team team = Team.Invalid;
		switch (locationCharacter.CharacterRelation)
		{
		case LocationCharacter.CharacterRelations.Neutral:
			team = Team.Invalid;
			break;
		case LocationCharacter.CharacterRelations.Friendly:
			team = base.Mission.PlayerAllyTeam;
			break;
		case LocationCharacter.CharacterRelations.Enemy:
			team = base.Mission.PlayerEnemyTeam;
			break;
		}
		spawnPointFrame.origin.z = base.Mission.Scene.GetGroundHeightAtPosition(spawnPointFrame.origin);
		(uint, uint) agentSettlementColors = GetAgentSettlementColors(locationCharacter);
		AgentBuildData agentBuildData = locationCharacter.GetAgentBuildData().Team(team).InitialPosition(in spawnPointFrame.origin);
		Vec2 direction = spawnPointFrame.rotation.f.AsVec2.Normalized();
		AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(in direction).ClothingColor1(agentSettlementColors.Item1).ClothingColor2(agentSettlementColors.Item2)
			.CivilianEquipment(locationCharacter.UseCivilianEquipment)
			.NoHorses(noHorses)
			.Banner(locationCharacter.Character?.HeroObject?.Clan?.Banner);
		Agent agent = base.Mission.SpawnAgent(agentBuildData2);
		SetAgentExcludeFaceGroupIdAux(agent, _disabledFaceId);
		AnimationSystemData animationSystemData = agentBuildData2.AgentMonster.FillAnimationSystemData(MBGlobals.GetActionSet(locationCharacter.ActionSetCode), locationCharacter.Character.GetStepSize(), hasClippingPlane: false);
		agent.SetActionSet(ref animationSystemData);
		agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator(locationCharacter);
		locationCharacter.AddBehaviors(agent);
		return agent;
	}

	private static void SetAgentExcludeFaceGroupIdAux(Agent agent, int _disabledFaceId)
	{
		if (_disabledFaceId != -1)
		{
			agent.SetAgentExcludeStateForFaceGroupId(_disabledFaceId, isExcluded: true);
		}
	}

	public static uint GetRandomTournamentTeamColor(int teamIndex)
	{
		return _tournamentTeamColors[teamIndex % _tournamentTeamColors.Length];
	}

	public static (uint color1, uint color2) GetAgentSettlementColors(LocationCharacter locationCharacter)
	{
		CharacterObject character = locationCharacter.Character;
		if (character.IsHero)
		{
			if (character.HeroObject.Clan == CharacterObject.PlayerCharacter.HeroObject.Clan)
			{
				return (Clan.PlayerClan.Color, Clan.PlayerClan.Color2);
			}
			if (!character.HeroObject.IsNotable)
			{
				return (locationCharacter.AgentData.AgentClothingColor1, locationCharacter.AgentData.AgentClothingColor2);
			}
			return CharacterHelper.GetDeterministicColorsForCharacter(character);
		}
		if (character.IsSoldier)
		{
			return (Settlement.CurrentSettlement.MapFaction.Color, Settlement.CurrentSettlement.MapFaction.Color2);
		}
		return (_villagerClothColors[MBRandom.RandomInt(_villagerClothColors.Length)], _villagerClothColors[MBRandom.RandomInt(_villagerClothColors.Length)]);
	}

	public UsableMachine FindUnusedPointWithTagForAgent(Agent agent, string tag)
	{
		return FindUnusedPointForAgent(agent, _pairedUsablePoints, tag) ?? FindUnusedPointForAgent(agent, _usablePoints, tag);
	}

	private UsableMachine FindUnusedPointForAgent(Agent agent, Dictionary<string, List<UsableMachine>> usableMachinesList, string primaryTag)
	{
		if (usableMachinesList.TryGetValue(primaryTag, out var value) && value.Count > 0)
		{
			int num = MBRandom.RandomInt(0, value.Count);
			for (int i = 0; i < value.Count; i++)
			{
				UsableMachine usableMachine = value[(num + i) % value.Count];
				if (!usableMachine.IsDisabled && !usableMachine.IsDestroyed && usableMachine.IsStandingPointAvailableForAgent(agent))
				{
					return usableMachine;
				}
			}
		}
		return null;
	}

	public List<UsableMachine> FindAllUnusedPoints(Agent agent, string primaryTag)
	{
		List<UsableMachine> list = new List<UsableMachine>();
		List<UsableMachine> list2 = new List<UsableMachine>();
		_usablePoints.TryGetValue(primaryTag, out var value);
		_pairedUsablePoints.TryGetValue(primaryTag, out var value2);
		value2 = value2?.Distinct().ToList();
		if (value != null && value.Count > 0)
		{
			list.AddRange(value);
		}
		if (value2 != null && value2.Count > 0)
		{
			list.AddRange(value2);
		}
		if (list.Count > 0)
		{
			foreach (UsableMachine item in list)
			{
				if (item.StandingPoints.Exists((StandingPoint sp) => (sp.IsInstantUse || (!sp.HasUser && !sp.HasAIMovingTo)) && !sp.IsDisabledForAgent(agent)))
				{
					list2.Add(item);
				}
			}
			return list2;
		}
		return list2;
	}

	public void RemovePropReference(List<GameEntity> props)
	{
		foreach (KeyValuePair<string, List<UsableMachine>> usablePoint in _usablePoints)
		{
			foreach (GameEntity prop in props)
			{
				if (prop.HasTag(usablePoint.Key))
				{
					UsableMachine firstScriptOfType = prop.GetFirstScriptOfType<UsableMachine>();
					usablePoint.Value.Remove(firstScriptOfType);
				}
			}
		}
		foreach (KeyValuePair<string, List<UsableMachine>> pairedUsablePoint in _pairedUsablePoints)
		{
			foreach (GameEntity prop2 in props)
			{
				if (prop2.HasTag(pairedUsablePoint.Key))
				{
					UsableMachine firstScriptOfType2 = prop2.GetFirstScriptOfType<UsableMachine>();
					pairedUsablePoint.Value.Remove(firstScriptOfType2);
				}
			}
		}
	}

	public void AddPropReference(List<GameEntity> props)
	{
		foreach (KeyValuePair<string, List<UsableMachine>> usablePoint in _usablePoints)
		{
			foreach (GameEntity prop in props)
			{
				UsableMachine firstScriptOfType = prop.GetFirstScriptOfType<UsableMachine>();
				if (firstScriptOfType != null && prop.HasTag(usablePoint.Key))
				{
					usablePoint.Value.Add(firstScriptOfType);
				}
			}
		}
	}

	public void TeleportTargetAgentNearReferenceAgent(Agent referenceAgent, Agent teleportAgent, bool teleportFollowers, bool teleportOpposite)
	{
		Vec3 vec = referenceAgent.Position + referenceAgent.LookDirection.NormalizedCopy() * 4f;
		Vec3 position;
		if (teleportOpposite)
		{
			position = vec;
			position.z = base.Mission.Scene.GetGroundHeightAtPosition(position);
		}
		else
		{
			position = Mission.Current.GetRandomPositionAroundPoint(referenceAgent.Position, 2f, 4f, nearFirst: true);
			position.z = base.Mission.Scene.GetGroundHeightAtPosition(position);
		}
		teleportAgent.LookDirection = new Vec3(new WorldFrame(referenceAgent.Frame.rotation, new WorldPosition(base.Mission.Scene, referenceAgent.Frame.origin)).Origin.AsVec2 - position.AsVec2).NormalizedCopy();
		teleportAgent.TeleportToPosition(position);
		if (!teleportFollowers || teleportAgent.Controller != Agent.ControllerType.Player)
		{
			return;
		}
		foreach (Agent agent in base.Mission.Agents)
		{
			LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(agent.Origin);
			AccompanyingCharacter accompanyingCharacter = PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(locationCharacter);
			if (agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null && accompanyingCharacter != null && accompanyingCharacter.IsFollowingPlayerAtMissionStart)
			{
				GetFrameForFollowingAgent(teleportAgent, out var frame);
				agent.TeleportToPosition(frame.origin);
			}
		}
	}

	public static int GetPointCountOfUsableMachine(UsableMachine usableMachine, bool checkForUnusedOnes)
	{
		int num = 0;
		List<AnimationPoint> list = new List<AnimationPoint>();
		foreach (StandingPoint standingPoint in usableMachine.StandingPoints)
		{
			if (standingPoint is AnimationPoint animationPoint && animationPoint.IsActive && !standingPoint.IsDeactivated && !standingPoint.IsDisabled && !standingPoint.IsInstantUse && (!checkForUnusedOnes || (!standingPoint.HasUser && !standingPoint.HasAIMovingTo)))
			{
				List<AnimationPoint> alternatives = animationPoint.GetAlternatives();
				if (alternatives.Count == 0)
				{
					num++;
				}
				else if (!list.Contains(animationPoint) && (!checkForUnusedOnes || !alternatives.Any((AnimationPoint x) => x.HasUser && x.HasAIMovingTo)))
				{
					list.AddRange(alternatives);
					num++;
				}
			}
		}
		return num;
	}
}
