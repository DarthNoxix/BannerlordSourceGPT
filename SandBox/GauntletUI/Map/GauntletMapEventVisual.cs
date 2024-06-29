using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.GauntletUI.Map;

public class GauntletMapEventVisual : IMapEventVisual
{
	private static int _battleSoundEventIndex = -1;

	private const string BattleSoundPath = "event:/map/ambient/node/battle";

	private const string RaidSoundPath = "event:/map/ambient/node/battle_raid";

	private const string SiegeSoundPath = "event:/map/ambient/node/battle_siege";

	private SoundEvent _siegeSoundEvent;

	private SoundEvent _raidedSoundEvent;

	private SoundEvent _battleSound;

	private readonly Action<GauntletMapEventVisual> _onDeactivate;

	private readonly Action<GauntletMapEventVisual> _onInitialized;

	private readonly Action<GauntletMapEventVisual> _onVisibilityChanged;

	private Scene _mapScene;

	public MapEvent MapEvent { get; private set; }

	public Vec2 WorldPosition { get; private set; }

	public bool IsVisible { get; private set; }

	private Scene MapScene
	{
		get
		{
			if (_mapScene == null && Campaign.Current?.MapSceneWrapper != null)
			{
				_mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
			}
			return _mapScene;
		}
	}

	public GauntletMapEventVisual(MapEvent mapEvent, Action<GauntletMapEventVisual> onInitialized, Action<GauntletMapEventVisual> onVisibilityChanged, Action<GauntletMapEventVisual> onDeactivate)
	{
		_onDeactivate = onDeactivate;
		_onInitialized = onInitialized;
		_onVisibilityChanged = onVisibilityChanged;
		MapEvent = mapEvent;
	}

	public void Initialize(Vec2 position, int battleSizeValue, bool hasSound, bool isVisible)
	{
		WorldPosition = position;
		IsVisible = isVisible;
		_onInitialized?.Invoke(this);
		if (!hasSound)
		{
			return;
		}
		if (MapEvent.IsFieldBattle)
		{
			if (_battleSoundEventIndex == -1)
			{
				_battleSoundEventIndex = SoundEvent.GetEventIdFromString("event:/map/ambient/node/battle");
			}
			_battleSound = SoundEvent.CreateEvent(_battleSoundEventIndex, MapScene);
			_battleSound.SetParameter("battle_size", battleSizeValue);
			float height = 0f;
			MapScene.GetHeightAtPoint(position, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref height);
			_battleSound.PlayInPosition(new Vec3(position.x, position.y, height + 2f));
			if (!isVisible)
			{
				_battleSound.Pause();
			}
		}
		else if (MapEvent.IsSiegeAssault || MapEvent.IsSiegeOutside || MapEvent.IsSiegeAmbush)
		{
			float height2 = 0f;
			Vec2 point = ((MapEvent.MapEventSettlement != null) ? MapEvent.MapEventSettlement.GatePosition : MapEvent.Position);
			Campaign.Current.MapSceneWrapper.GetHeightAtPoint(point, ref height2);
			Vec3 position2 = new Vec3(point.X, point.Y, height2);
			_siegeSoundEvent?.Stop();
			_siegeSoundEvent = SoundEvent.CreateEventFromString("event:/map/ambient/node/battle_siege", MapScene);
			_siegeSoundEvent.SetParameter("battle_size", 4f);
			_siegeSoundEvent.SetPosition(position2);
			_siegeSoundEvent.Play();
		}
		else if (MapEvent.IsRaid)
		{
			if (MapEvent.MapEventSettlement.IsRaided && _raidedSoundEvent == null)
			{
				_raidedSoundEvent = SoundEvent.CreateEventFromString("event:/map/ambient/node/burning_village", MapScene);
				_raidedSoundEvent.SetParameter("battle_size", 4f);
				_raidedSoundEvent.SetPosition(MapEvent.MapEventSettlement.GetPosition());
				_raidedSoundEvent.Play();
			}
			else if (!MapEvent.MapEventSettlement.IsRaided && _raidedSoundEvent != null)
			{
				_raidedSoundEvent.Stop();
				_raidedSoundEvent = null;
			}
		}
	}

	public void OnMapEventEnd()
	{
		_onDeactivate?.Invoke(this);
		if (_battleSound != null)
		{
			_battleSound.Stop();
			_battleSound = null;
		}
		if (_siegeSoundEvent != null)
		{
			_siegeSoundEvent.Stop();
			_siegeSoundEvent = null;
		}
		if (_raidedSoundEvent != null)
		{
			_raidedSoundEvent.Stop();
			_raidedSoundEvent = null;
		}
	}

	public void SetVisibility(bool isVisible)
	{
		IsVisible = isVisible;
		_onVisibilityChanged?.Invoke(this);
		SoundEvent battleSound = _battleSound;
		if (battleSound != null && battleSound.IsValid)
		{
			if (isVisible && _battleSound.IsPaused())
			{
				_battleSound.Resume();
			}
			else if (!isVisible && !_battleSound.IsPaused())
			{
				_battleSound.Pause();
			}
		}
		SoundEvent siegeSoundEvent = _siegeSoundEvent;
		if (siegeSoundEvent != null && siegeSoundEvent.IsValid)
		{
			if (isVisible && _siegeSoundEvent.IsPaused())
			{
				_siegeSoundEvent.Resume();
			}
			else if (!isVisible && !_siegeSoundEvent.IsPaused())
			{
				_siegeSoundEvent.Pause();
			}
		}
		SoundEvent raidedSoundEvent = _raidedSoundEvent;
		if (raidedSoundEvent != null && raidedSoundEvent.IsValid)
		{
			if (isVisible && _raidedSoundEvent.IsPaused())
			{
				_raidedSoundEvent.Resume();
			}
			else if (!isVisible && !_raidedSoundEvent.IsPaused())
			{
				_raidedSoundEvent.Pause();
			}
		}
	}
}
