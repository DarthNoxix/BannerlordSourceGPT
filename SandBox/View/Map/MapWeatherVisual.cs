using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.View.Map;

public class MapWeatherVisual
{
	private readonly WeatherNode _weatherNode;

	public GameEntity Prefab;

	private MapWeatherModel.WeatherEvent _previousWeatherEvent;

	private int _maskPixelIndex = -1;

	public Vec2 Position => _weatherNode.Position;

	public Vec2 PrefabSpawnOffset
	{
		get
		{
			Vec2 terrainSize = Campaign.Current.MapSceneWrapper.GetTerrainSize();
			float num = terrainSize.X / (float)Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension;
			float num2 = terrainSize.Y / (float)Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension;
			return new Vec2(num * 0.5f, num2 * 0.5f);
		}
	}

	public int MaskPixelIndex
	{
		get
		{
			if (_maskPixelIndex == -1)
			{
				Vec2 terrainSize = Campaign.Current.MapSceneWrapper.GetTerrainSize();
				float num = terrainSize.X / (float)Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension;
				float num2 = terrainSize.Y / (float)Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension;
				int num3 = (int)(Position.x / num);
				int num4 = (int)(Position.y / num2);
				_maskPixelIndex = num4 * Campaign.Current.Models.MapWeatherModel.DefaultWeatherNodeDimension + num3;
			}
			return _maskPixelIndex;
		}
	}

	public override string ToString()
	{
		return Position.ToString();
	}

	public MapWeatherVisual(WeatherNode weatherNode)
	{
		_weatherNode = weatherNode;
		_previousWeatherEvent = MapWeatherModel.WeatherEvent.Clear;
	}

	public void Tick()
	{
		if (!_weatherNode.IsVisuallyDirty)
		{
			return;
		}
		bool flag = _previousWeatherEvent == MapWeatherModel.WeatherEvent.HeavyRain;
		bool flag2 = _previousWeatherEvent == MapWeatherModel.WeatherEvent.Blizzard;
		MapWeatherModel.WeatherEvent weatherEventInPosition = Campaign.Current.Models.MapWeatherModel.GetWeatherEventInPosition(Position);
		bool flag3 = weatherEventInPosition == MapWeatherModel.WeatherEvent.HeavyRain;
		bool num = weatherEventInPosition == MapWeatherModel.WeatherEvent.LightRain;
		bool flag4 = weatherEventInPosition == MapWeatherModel.WeatherEvent.Blizzard;
		byte b = (byte)(num ? 125u : (flag3 ? 200u : 0u));
		byte value = (byte)Math.Max(b, flag4 ? 200 : 0);
		MapWeatherVisualManager.Current.SetRainData(MaskPixelIndex, b);
		MapWeatherVisualManager.Current.SetCloudData(MaskPixelIndex, value);
		if (Prefab == null)
		{
			if (flag3)
			{
				AttachNewRainPrefabToVisual();
			}
			else if (flag4)
			{
				AttachNewBlizzardPrefabToVisual();
			}
			else if (MBRandom.RandomFloat < 0.1f)
			{
				MapWeatherVisualManager.Current.SetCloudData(MaskPixelIndex, 200);
			}
		}
		else
		{
			if (flag && !flag3 && flag4)
			{
				MapWeatherVisualManager.Current.ReleaseRainPrefab(Prefab);
				AttachNewBlizzardPrefabToVisual();
			}
			else if (flag2 && !flag4 && flag3)
			{
				MapWeatherVisualManager.Current.ReleaseBlizzardPrefab(Prefab);
				AttachNewRainPrefabToVisual();
			}
			if (!flag3 && !flag4)
			{
				if (flag)
				{
					MapWeatherVisualManager.Current.ReleaseRainPrefab(Prefab);
				}
				else if (flag2)
				{
					MapWeatherVisualManager.Current.ReleaseBlizzardPrefab(Prefab);
				}
				Prefab = null;
			}
		}
		_previousWeatherEvent = weatherEventInPosition;
		_weatherNode.OnVisualUpdated();
	}

	private void AttachNewRainPrefabToVisual()
	{
		MatrixFrame frame = MatrixFrame.Identity;
		frame.origin = new Vec3(Position + PrefabSpawnOffset, 26f);
		GameEntity rainPrefabFromPool = MapWeatherVisualManager.Current.GetRainPrefabFromPool();
		rainPrefabFromPool.SetVisibilityExcludeParents(visible: true);
		rainPrefabFromPool.SetGlobalFrame(in frame);
		Prefab = rainPrefabFromPool;
	}

	private void AttachNewBlizzardPrefabToVisual()
	{
		MatrixFrame frame = MatrixFrame.Identity;
		frame.origin = new Vec3(Position + PrefabSpawnOffset, 26f);
		GameEntity blizzardPrefabFromPool = MapWeatherVisualManager.Current.GetBlizzardPrefabFromPool();
		blizzardPrefabFromPool.SetVisibilityExcludeParents(visible: true);
		blizzardPrefabFromPool.SetGlobalFrame(in frame);
		Prefab = blizzardPrefabFromPool;
	}
}
