using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.View.Map;

public class MapWeatherVisualManager : CampaignEntityVisualComponent
{
	public const int DefaultCloudHeight = 26;

	private MapWeatherVisual[] _allWeatherNodeVisuals;

	private const string RainPrefabName = "campaign_rain_prefab";

	private const string BlizzardPrefabName = "campaign_snow_prefab";

	private const string RainSoundPath = "event:/map/ambient/bed/rain";

	private const string SnowSoundPath = "event:/map/ambient/bed/snow";

	private const string WeatherEventParameterName = "Rainfall";

	private const string CameraRainPrefabName = "map_camera_rain_prefab";

	private const int DefaultRainObjectPoolCount = 5;

	private const int DefaultBlizzardObjectPoolCount = 5;

	private const int WeatherCheckOriginZDelta = 25;

	private readonly List<GameEntity> _unusedRainPrefabEntityPool;

	private readonly List<GameEntity> _unusedBlizzardPrefabEntityPool;

	private readonly Scene _mapScene;

	private readonly byte[] _rainData = new byte[Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension * Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension * 2];

	private readonly byte[] _rainDataTemporal = new byte[Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension * Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension * 2];

	private SoundEvent _currentRainSound;

	private SoundEvent _currentBlizzardSound;

	private GameEntity _cameraRainEffect;

	public static MapWeatherVisualManager Current => Campaign.Current.GetEntityComponent<MapWeatherVisualManager>();

	private int DimensionSquared => Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension * Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension;

	public MapWeatherVisualManager()
	{
		_unusedRainPrefabEntityPool = new List<GameEntity>();
		_unusedBlizzardPrefabEntityPool = new List<GameEntity>();
		for (int i = 0; i < DimensionSquared * 2; i++)
		{
			_rainData[i] = 0;
			_rainDataTemporal[i] = 0;
		}
		_allWeatherNodeVisuals = new MapWeatherVisual[DimensionSquared];
		_mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
		WeatherNode[] allWeatherNodes = Campaign.Current.GetCampaignBehavior<MapWeatherCampaignBehavior>().AllWeatherNodes;
		for (int j = 0; j < allWeatherNodes.Length; j++)
		{
			_allWeatherNodeVisuals[j] = new MapWeatherVisual(allWeatherNodes[j]);
		}
	}

	public override void OnVisualTick(MapScreen screen, float realDt, float dt)
	{
		for (int i = 0; i < _allWeatherNodeVisuals.Length; i++)
		{
			_allWeatherNodeVisuals[i].Tick();
		}
		TWParallel.For(0, DimensionSquared, delegate(int startInclusive, int endExclusive)
		{
			for (int j = startInclusive; j < endExclusive; j++)
			{
				int num = j * 2;
				_rainDataTemporal[num] = (byte)MBMath.Lerp((int)_rainDataTemporal[num], (int)_rainData[num], 1f - (float)Math.Exp(-1.8f * (realDt + dt)));
				_rainDataTemporal[num + 1] = (byte)MBMath.Lerp((int)_rainDataTemporal[num + 1], (int)_rainData[num + 1], 1f - (float)Math.Exp(-1.8f * (realDt + dt)));
			}
		});
		_mapScene.SetLandscapeRainMaskData(_rainDataTemporal);
		WeatherAudioTick();
	}

	public void SetRainData(int dataIndex, byte value)
	{
		_rainData[dataIndex * 2] = value;
	}

	public void SetCloudData(int dataIndex, byte value)
	{
		_rainData[dataIndex * 2 + 1] = value;
	}

	private void WeatherAudioTick()
	{
		SoundManager.SetGlobalParameter("Rainfall", 0.5f);
		float num = 1f;
		float height = 0f;
		Campaign.Current.MapSceneWrapper.GetHeightAtPoint(_mapScene.LastFinalRenderCameraFrame.origin.AsVec2, ref height);
		float num2 = _mapScene.LastFinalRenderCameraFrame.origin.Z - height;
		if (26f > num2)
		{
			num = num2 / 26f;
		}
		Vec3 origin = _mapScene.LastFinalRenderCameraFrame.Elevate(-25f * num).origin;
		switch (Campaign.Current.Models.MapWeatherModel.GetWeatherEventInPosition(origin.AsVec2))
		{
		case MapWeatherModel.WeatherEvent.HeavyRain:
			if (_mapScene.LastFinalRenderCameraPosition.z < 65f)
			{
				_cameraRainEffect.SetVisibilityExcludeParents(visible: true);
				MatrixFrame frame = _mapScene.LastFinalRenderCameraFrame.Elevate(-5f);
				_cameraRainEffect.SetFrame(ref frame);
			}
			else
			{
				_cameraRainEffect.SetVisibilityExcludeParents(visible: false);
			}
			DestroyBlizzardSound();
			StartRainSoundIfNeeded();
			MBMapScene.ApplyRainColorGrade = true;
			break;
		case MapWeatherModel.WeatherEvent.Blizzard:
			DestroyRainSound();
			StartBlizzardSoundIfNeeded();
			_cameraRainEffect.SetVisibilityExcludeParents(visible: false);
			MBMapScene.ApplyRainColorGrade = false;
			break;
		case MapWeatherModel.WeatherEvent.Clear:
			DestroyBlizzardSound();
			DestroyRainSound();
			_cameraRainEffect.SetVisibilityExcludeParents(visible: false);
			MBMapScene.ApplyRainColorGrade = false;
			break;
		}
	}

	private void DestroyRainSound()
	{
		if (_currentRainSound != null)
		{
			_currentRainSound.Stop();
			_currentRainSound = null;
		}
	}

	private void DestroyBlizzardSound()
	{
		if (_currentBlizzardSound != null)
		{
			_currentBlizzardSound.Stop();
			_currentBlizzardSound = null;
		}
	}

	private void StartRainSoundIfNeeded()
	{
		if (_currentRainSound == null)
		{
			_currentRainSound = SoundManager.CreateEvent("event:/map/ambient/bed/rain", _mapScene);
			_currentRainSound.Play();
		}
	}

	private void StartBlizzardSoundIfNeeded()
	{
		if (_currentBlizzardSound == null)
		{
			_currentBlizzardSound = SoundManager.CreateEvent("event:/map/ambient/bed/snow", _mapScene);
			_currentBlizzardSound.Play();
		}
	}

	protected override void OnInitialize()
	{
		base.OnInitialize();
		InitializeObjectPoolWithDefaultCount();
		_cameraRainEffect = GameEntity.Instantiate(_mapScene, "map_camera_rain_prefab", MatrixFrame.Identity);
	}

	public GameEntity GetRainPrefabFromPool()
	{
		if (_unusedRainPrefabEntityPool.IsEmpty())
		{
			_unusedRainPrefabEntityPool.AddRange(CreateNewWeatherPrefabPoolElements("campaign_rain_prefab", 5));
		}
		GameEntity gameEntity = _unusedRainPrefabEntityPool[0];
		_unusedRainPrefabEntityPool.Remove(gameEntity);
		return gameEntity;
	}

	public GameEntity GetBlizzardPrefabFromPool()
	{
		if (_unusedBlizzardPrefabEntityPool.IsEmpty())
		{
			_unusedBlizzardPrefabEntityPool.AddRange(CreateNewWeatherPrefabPoolElements("campaign_snow_prefab", 5));
		}
		GameEntity gameEntity = _unusedBlizzardPrefabEntityPool[0];
		_unusedBlizzardPrefabEntityPool.Remove(gameEntity);
		return gameEntity;
	}

	public void ReleaseRainPrefab(GameEntity prefab)
	{
		_unusedRainPrefabEntityPool.Add(prefab);
		prefab.SetVisibilityExcludeParents(visible: false);
	}

	public void ReleaseBlizzardPrefab(GameEntity prefab)
	{
		_unusedBlizzardPrefabEntityPool.Add(prefab);
		prefab.SetVisibilityExcludeParents(visible: false);
	}

	private void InitializeObjectPoolWithDefaultCount()
	{
		_unusedRainPrefabEntityPool.AddRange(CreateNewWeatherPrefabPoolElements("campaign_rain_prefab", 5));
		_unusedBlizzardPrefabEntityPool.AddRange(CreateNewWeatherPrefabPoolElements("campaign_snow_prefab", 5));
	}

	private List<GameEntity> CreateNewWeatherPrefabPoolElements(string prefabName, int delta)
	{
		List<GameEntity> list = new List<GameEntity>();
		for (int i = 0; i < delta; i++)
		{
			GameEntity gameEntity = GameEntity.Instantiate(_mapScene, prefabName, MatrixFrame.Identity);
			gameEntity.SetVisibilityExcludeParents(visible: false);
			list.Add(gameEntity);
		}
		return list;
	}
}
