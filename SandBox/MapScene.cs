using System;
using System.Collections.Generic;
using System.Threading;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox;

public class MapScene : IMapScene
{
	private Scene _scene;

	private MBAgentRendererSceneController _agentRendererSceneController;

	private Dictionary<string, uint> _sceneLevels;

	private int _battleTerrainIndexMapWidth;

	private int _battleTerrainIndexMapHeight;

	private byte[] _battleTerrainIndexMap;

	private Vec2 _terrainSize;

	private ReaderWriterLockSlim _sharedLock;

	public Scene Scene => _scene;

	public MapScene()
	{
		_sharedLock = new ReaderWriterLockSlim();
		_sceneLevels = new Dictionary<string, uint>();
	}

	public Vec2 GetTerrainSize()
	{
		return _terrainSize;
	}

	public uint GetSceneLevel(string name)
	{
		_sharedLock.EnterReadLock();
		uint value;
		bool num = _sceneLevels.TryGetValue(name, out value) && value != int.MaxValue;
		_sharedLock.ExitReadLock();
		if (num)
		{
			return value;
		}
		uint upgradeLevelMaskOfLevelName = _scene.GetUpgradeLevelMaskOfLevelName(name);
		_sharedLock.EnterWriteLock();
		_sceneLevels[name] = upgradeLevelMaskOfLevelName;
		_sharedLock.ExitWriteLock();
		return upgradeLevelMaskOfLevelName;
	}

	public void SetSceneLevels(List<string> levels)
	{
		foreach (string level in levels)
		{
			_sceneLevels.Add(level, 2147483647u);
		}
	}

	public List<AtmosphereState> GetAtmosphereStates()
	{
		List<AtmosphereState> list = new List<AtmosphereState>();
		foreach (GameEntity item2 in Scene.FindEntitiesWithTag("atmosphere_probe"))
		{
			MapAtmosphereProbe firstScriptOfType = item2.GetFirstScriptOfType<MapAtmosphereProbe>();
			Vec3 globalPosition = item2.GlobalPosition;
			AtmosphereState item = new AtmosphereState
			{
				Position = globalPosition,
				HumidityAverage = firstScriptOfType.rainDensity,
				HumidityVariance = 5f,
				TemperatureAverage = firstScriptOfType.temperature,
				TemperatureVariance = 5f,
				distanceForMaxWeight = firstScriptOfType.minRadius,
				distanceForMinWeight = firstScriptOfType.maxRadius,
				ColorGradeTexture = firstScriptOfType.colorGrade
			};
			list.Add(item);
		}
		return list;
	}

	public void ValidateAgentVisualsReseted()
	{
		if (_scene != null && _agentRendererSceneController != null)
		{
			MBAgentRendererSceneController.ValidateAgentVisualsReseted(_scene, _agentRendererSceneController);
		}
	}

	public void SetAtmosphereColorgrade(TerrainType terrainType)
	{
	}

	public void AddNewEntityToMapScene(string entityId, Vec2 position)
	{
		GameEntity gameEntity = GameEntity.Instantiate(_scene, entityId, callScriptCallbacks: true);
		if (gameEntity != null)
		{
			Vec3 localPosition = new Vec3(position.x, position.y);
			localPosition.z = _scene.GetGroundHeightAtPosition(position.ToVec3());
			gameEntity.SetLocalPosition(localPosition);
		}
	}

	public void GetFaceIndexForMultiplePositions(int movedPartyCount, float[] positionArray, PathFaceRecord[] resultArray)
	{
		MBMapScene.GetFaceIndexForMultiplePositions(_scene, movedPartyCount, positionArray, resultArray, check_if_disabled: false, check_height: true);
	}

	public void GetMapBorders(out Vec2 minimumPosition, out Vec2 maximumPosition, out float maximumHeight)
	{
		GameEntity firstEntityWithName = _scene.GetFirstEntityWithName("border_min");
		GameEntity firstEntityWithName2 = _scene.GetFirstEntityWithName("border_max");
		minimumPosition = ((firstEntityWithName != null) ? firstEntityWithName.GetGlobalFrame().origin.AsVec2 : Vec2.Zero);
		maximumPosition = ((firstEntityWithName2 != null) ? firstEntityWithName2.GetGlobalFrame().origin.AsVec2 : new Vec2(900f, 900f));
		maximumHeight = ((firstEntityWithName2 != null) ? firstEntityWithName2.GetGlobalFrame().origin.z : 670f);
	}

	public void Load()
	{
		Debug.Print("Creating map scene");
		_scene = Scene.CreateNewScene(initialize_physics: false, enable_decals: true, DecalAtlasGroup.Worldmap, "MapScene");
		_scene.SetClothSimulationState(state: true);
		_agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(_scene, 4096);
		_agentRendererSceneController.SetDoTimerBasedForcedSkeletonUpdates(value: false);
		_scene.SetOcclusionMode(mode: true);
		SceneInitializationData initData = new SceneInitializationData(initializeWithDefaults: true);
		initData.UsePhysicsMaterials = false;
		initData.EnableFloraPhysics = false;
		initData.UseTerrainMeshBlending = false;
		initData.CreateOros = false;
		Debug.Print("Reading map scene");
		_scene.Read("Main_map", ref initData);
		Utilities.SetAllocationAlwaysValidScene(_scene);
		_scene.DisableStaticShadows(value: true);
		_scene.InvalidateTerrainPhysicsMaterials();
		LoadAtmosphereData(_scene);
		DisableUnwalkableNavigationMeshes();
		MBMapScene.ValidateTerrainSoundIds();
		_scene.OptimizeScene();
		_scene.GetTerrainData(out var nodeDimension, out var nodeSize, out var _, out var _);
		_terrainSize.x = (float)nodeDimension.X * nodeSize;
		_terrainSize.y = (float)nodeDimension.Y * nodeSize;
		MBMapScene.GetBattleSceneIndexMap(_scene, ref _battleTerrainIndexMap, ref _battleTerrainIndexMapWidth, ref _battleTerrainIndexMapHeight);
		Debug.Print("Ticking map scene for first initialization");
		_scene.Tick(0.1f);
		AsyncTask campaignLateAITickTask = AsyncTask.CreateWithDelegate(new ManagedDelegate
		{
			Instance = Campaign.LateAITick
		}, isBackground: false);
		Campaign.Current.CampaignLateAITickTask = campaignLateAITickTask;
	}

	public void Destroy()
	{
		MBAgentRendererSceneController.DestructAgentRendererSceneController(_scene, _agentRendererSceneController, deleteThisFrame: false);
		_agentRendererSceneController = null;
	}

	public void DisableUnwalkableNavigationMeshes()
	{
		Scene.SetAbilityOfFacesWithId(GetNavigationMeshIndexOfTerrainType(TerrainType.Mountain), isEnabled: false);
		Scene.SetAbilityOfFacesWithId(GetNavigationMeshIndexOfTerrainType(TerrainType.Lake), isEnabled: false);
		Scene.SetAbilityOfFacesWithId(GetNavigationMeshIndexOfTerrainType(TerrainType.Water), isEnabled: false);
		Scene.SetAbilityOfFacesWithId(GetNavigationMeshIndexOfTerrainType(TerrainType.River), isEnabled: false);
		Scene.SetAbilityOfFacesWithId(GetNavigationMeshIndexOfTerrainType(TerrainType.Canyon), isEnabled: false);
		Scene.SetAbilityOfFacesWithId(GetNavigationMeshIndexOfTerrainType(TerrainType.RuralArea), isEnabled: false);
	}

	public PathFaceRecord GetFaceIndex(Vec2 position)
	{
		PathFaceRecord record = new PathFaceRecord(-1, -1, -1);
		_scene.GetNavMeshFaceIndex(ref record, position, checkIfDisabled: false, ignoreHeight: true);
		return record;
	}

	public bool AreFacesOnSameIsland(PathFaceRecord startingFace, PathFaceRecord endFace, bool ignoreDisabled)
	{
		return _scene.DoesPathExistBetweenFaces(startingFace.FaceIndex, endFace.FaceIndex, ignoreDisabled);
	}

	private void LoadAtmosphereData(Scene mapScene)
	{
		MBMapScene.LoadAtmosphereData(mapScene);
	}

	public TerrainType GetTerrainTypeAtPosition(Vec2 position)
	{
		PathFaceRecord faceIndex = GetFaceIndex(position);
		return GetFaceTerrainType(faceIndex);
	}

	public TerrainType GetFaceTerrainType(PathFaceRecord navMeshFace)
	{
		if (!navMeshFace.IsValid())
		{
			Debug.FailedAssert("Null nav mesh face tried to get terrain type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\MapScene.cs", "GetFaceTerrainType", 255);
			return TerrainType.Plain;
		}
		return navMeshFace.FaceGroupIndex switch
		{
			1 => TerrainType.Plain, 
			2 => TerrainType.Desert, 
			3 => TerrainType.Snow, 
			4 => TerrainType.Forest, 
			5 => TerrainType.Steppe, 
			6 => TerrainType.Fording, 
			7 => TerrainType.Mountain, 
			8 => TerrainType.Lake, 
			10 => TerrainType.Water, 
			11 => TerrainType.River, 
			13 => TerrainType.Canyon, 
			14 => TerrainType.RuralArea, 
			_ => TerrainType.Plain, 
		};
	}

	public static int GetNavigationMeshIndexOfTerrainType(TerrainType terrainType)
	{
		return terrainType switch
		{
			TerrainType.Plain => 1, 
			TerrainType.Desert => 2, 
			TerrainType.Snow => 3, 
			TerrainType.Forest => 4, 
			TerrainType.Steppe => 5, 
			TerrainType.Fording => 6, 
			TerrainType.Mountain => 7, 
			TerrainType.Lake => 8, 
			TerrainType.Water => 10, 
			TerrainType.River => 11, 
			TerrainType.Canyon => 13, 
			TerrainType.RuralArea => 14, 
			_ => -1, 
		};
	}

	public List<TerrainType> GetEnvironmentTerrainTypes(Vec2 position)
	{
		List<TerrainType> list = new List<TerrainType>();
		Vec2 vec = new Vec2(1f, 0f);
		list.Add(GetTerrainTypeAtPosition(position));
		for (int i = 0; i < 8; i++)
		{
			vec.RotateCCW((float)Math.PI / 4f * (float)i);
			for (int j = 1; j < 7; j++)
			{
				TerrainType terrainTypeAtPosition = GetTerrainTypeAtPosition(position + j * vec);
				GetFaceIndex(position + j * vec);
				if (!list.Contains(terrainTypeAtPosition))
				{
					list.Add(terrainTypeAtPosition);
				}
			}
		}
		return list;
	}

	public List<TerrainType> GetEnvironmentTerrainTypesCount(Vec2 position, out TerrainType currentPositionTerrainType)
	{
		List<TerrainType> list = new List<TerrainType>();
		Vec2 vec = new Vec2(1f, 0f);
		currentPositionTerrainType = GetTerrainTypeAtPosition(position);
		list.Add(currentPositionTerrainType);
		for (int i = 0; i < 8; i++)
		{
			vec.RotateCCW((float)Math.PI / 4f * (float)i);
			for (int j = 1; j < 7; j++)
			{
				PathFaceRecord faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(position + j * vec);
				if (faceIndex.IsValid())
				{
					TerrainType faceTerrainType = GetFaceTerrainType(faceIndex);
					list.Add(faceTerrainType);
				}
			}
		}
		return list;
	}

	public MapPatchData GetMapPatchAtPosition(Vec2 position)
	{
		if (_battleTerrainIndexMap != null)
		{
			int value = TaleWorlds.Library.MathF.Floor(position.x / _terrainSize.X * (float)_battleTerrainIndexMapWidth);
			int value2 = TaleWorlds.Library.MathF.Floor(position.y / _terrainSize.Y * (float)_battleTerrainIndexMapHeight);
			value = MBMath.ClampIndex(value, 0, _battleTerrainIndexMapWidth);
			int num = (MBMath.ClampIndex(value2, 0, _battleTerrainIndexMapHeight) * _battleTerrainIndexMapWidth + value) * 2;
			byte sceneIndex = _battleTerrainIndexMap[num];
			byte b = _battleTerrainIndexMap[num + 1];
			Vec2 normalizedCoordinates = new Vec2((float)(b & 0xF) / 15f, (float)((b >> 4) & 0xF) / 15f);
			MapPatchData result = default(MapPatchData);
			result.sceneIndex = sceneIndex;
			result.normalizedCoordinates = normalizedCoordinates;
			return result;
		}
		return default(MapPatchData);
	}

	public Vec2 GetAccessiblePointNearPosition(Vec2 position, float radius)
	{
		return MBMapScene.GetAccessiblePointNearPosition(_scene, position, radius);
	}

	public bool GetPathBetweenAIFaces(PathFaceRecord startingFace, PathFaceRecord endingFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, NavigationPath path, int[] excludedFaceIds = null)
	{
		return _scene.GetPathBetweenAIFaces(startingFace.FaceIndex, endingFace.FaceIndex, startingPosition, endingPosition, agentRadius, path, excludedFaceIds);
	}

	public bool GetPathDistanceBetweenAIFaces(PathFaceRecord startingAiFace, PathFaceRecord endingAiFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, float distanceLimit, out float distance)
	{
		return _scene.GetPathDistanceBetweenAIFaces(startingAiFace.FaceIndex, endingAiFace.FaceIndex, startingPosition, endingPosition, agentRadius, distanceLimit, out distance);
	}

	public bool IsLineToPointClear(PathFaceRecord startingFace, Vec2 position, Vec2 destination, float agentRadius)
	{
		return _scene.IsLineToPointClear(startingFace.FaceIndex, position, destination, agentRadius);
	}

	public Vec2 GetLastPointOnNavigationMeshFromPositionToDestination(PathFaceRecord startingFace, Vec2 position, Vec2 destination)
	{
		return _scene.GetLastPointOnNavigationMeshFromPositionToDestination(startingFace.FaceIndex, position, destination);
	}

	public Vec2 GetNavigationMeshCenterPosition(PathFaceRecord face)
	{
		Vec3 centerPosition = Vec3.Zero;
		_scene.GetNavMeshCenterPosition(face.FaceIndex, ref centerPosition);
		return centerPosition.AsVec2;
	}

	public int GetNumberOfNavigationMeshFaces()
	{
		return _scene.GetNavMeshFaceCount();
	}

	public bool GetHeightAtPoint(Vec2 point, ref float height)
	{
		return _scene.GetHeightAtPoint(point, BodyFlags.CommonCollisionExcludeFlags, ref height);
	}

	public float GetWinterTimeFactor()
	{
		return _scene.GetWinterTimeFactor();
	}

	public float GetFaceVertexZ(PathFaceRecord navMeshFace)
	{
		return _scene.GetNavMeshFaceFirstVertexZ(navMeshFace.FaceIndex);
	}

	public Vec3 GetGroundNormal(Vec2 position)
	{
		return _scene.GetNormalAt(position);
	}

	public void GetTerrainHeightAndNormal(Vec2 position, out float height, out Vec3 normal)
	{
		_scene.GetTerrainHeightAndNormal(position, out height, out normal);
	}

	public string GetTerrainTypeName(TerrainType type)
	{
		string result = "Invalid";
		switch (type)
		{
		case TerrainType.Water:
			result = "Water";
			break;
		case TerrainType.Mountain:
			result = "Mountain";
			break;
		case TerrainType.Snow:
			result = "Snow";
			break;
		case TerrainType.Steppe:
			result = "Steppe";
			break;
		case TerrainType.Plain:
			result = "Plain";
			break;
		case TerrainType.Desert:
			result = "Desert";
			break;
		case TerrainType.Swamp:
			result = "Swamp";
			break;
		case TerrainType.Dune:
			result = "Dune";
			break;
		case TerrainType.Bridge:
			result = "Bridge";
			break;
		case TerrainType.River:
			result = "River";
			break;
		case TerrainType.Forest:
			result = "Forest";
			break;
		case TerrainType.Fording:
			result = "Fording";
			break;
		case TerrainType.Lake:
			result = "Lake";
			break;
		case TerrainType.Canyon:
			result = "Canyon";
			break;
		}
		return result;
	}

	public void GetSnowAmountData(byte[] snowData)
	{
		_scene.GetSnowAmountData(snowData);
	}
}
