using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Map;

public class MobilePartyTrackItemVM : ViewModel
{
	private float _latestX;

	private float _latestY;

	private float _latestW;

	private readonly Camera _mapCamera;

	private readonly Action<Vec2> _fastMoveCameraToPosition;

	private Vec2 _partyPositionBind;

	private ImageIdentifierVM _factionVisualBind;

	private bool _isVisibleOnMapBind;

	private bool _isBehindBind;

	private string _nameBind;

	private Vec2 _partyPosition;

	private ImageIdentifierVM _factionVisual;

	private string _name;

	private bool _isArmy;

	private bool _isTracked;

	private bool _isEnabled;

	private bool _isBehind;

	public MobileParty TrackedParty { get; private set; }

	public Army TrackedArmy { get; private set; }

	private MobileParty _concernedMobileParty => TrackedArmy?.LeaderParty ?? TrackedParty;

	public Vec2 PartyPosition
	{
		get
		{
			return _partyPosition;
		}
		set
		{
			if (value != _partyPosition)
			{
				_partyPosition = value;
				OnPropertyChangedWithValue(value, "PartyPosition");
			}
		}
	}

	public ImageIdentifierVM FactionVisual
	{
		get
		{
			return _factionVisual;
		}
		set
		{
			if (value != _factionVisual)
			{
				_factionVisual = value;
				OnPropertyChangedWithValue(value, "FactionVisual");
			}
		}
	}

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			if (value != _name)
			{
				_name = value;
				OnPropertyChangedWithValue(value, "Name");
			}
		}
	}

	public bool IsArmy
	{
		get
		{
			return _isArmy;
		}
		set
		{
			if (value != _isArmy)
			{
				_isArmy = value;
				OnPropertyChangedWithValue(value, "IsArmy");
			}
		}
	}

	public bool IsTracked
	{
		get
		{
			return _isTracked;
		}
		set
		{
			if (value != _isTracked)
			{
				_isTracked = value;
				OnPropertyChangedWithValue(value, "IsTracked");
			}
		}
	}

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			if (value != _isEnabled)
			{
				_isEnabled = value;
				OnPropertyChangedWithValue(value, "IsEnabled");
			}
		}
	}

	public bool IsBehind
	{
		get
		{
			return _isBehind;
		}
		set
		{
			if (value != _isBehind)
			{
				_isBehind = value;
				OnPropertyChangedWithValue(value, "IsBehind");
			}
		}
	}

	public MobilePartyTrackItemVM(MobileParty trackedParty, Camera mapCamera, Action<Vec2> fastMoveCameraToPosition)
	{
		_mapCamera = mapCamera;
		_fastMoveCameraToPosition = fastMoveCameraToPosition;
		TrackedParty = trackedParty;
		IsTracked = Campaign.Current.VisualTrackerManager.CheckTracked(_concernedMobileParty);
		UpdateProperties();
		IsArmy = false;
	}

	public MobilePartyTrackItemVM(Army trackedArmy, Camera mapCamera, Action<Vec2> fastMoveCameraToPosition)
	{
		_mapCamera = mapCamera;
		_fastMoveCameraToPosition = fastMoveCameraToPosition;
		TrackedArmy = trackedArmy;
		IsTracked = Campaign.Current.VisualTrackerManager.CheckTracked(_concernedMobileParty);
		UpdateProperties();
		IsArmy = true;
	}

	internal void UpdateProperties()
	{
		if (TrackedArmy != null)
		{
			_nameBind = TrackedArmy?.Name.ToString();
		}
		else if (TrackedParty != null)
		{
			if (TrackedParty.IsCaravan && TrackedParty.LeaderHero != null)
			{
				_nameBind = TrackedParty.LeaderHero?.Name.ToString();
			}
			else
			{
				_nameBind = TrackedParty.Name.ToString();
			}
		}
		else
		{
			_nameBind = "";
		}
		_isVisibleOnMapBind = GetIsVisibleOnMap();
		if (_concernedMobileParty.LeaderHero?.Clan != null)
		{
			_factionVisualBind = new ImageIdentifierVM(BannerCode.CreateFrom(_concernedMobileParty.LeaderHero.Clan.Banner), nineGrid: true);
		}
		else
		{
			_factionVisualBind = new ImageIdentifierVM(BannerCode.CreateFrom(_concernedMobileParty.MapFaction?.Banner), nineGrid: true);
		}
	}

	private bool GetIsVisibleOnMap()
	{
		MobileParty concernedMobileParty = _concernedMobileParty;
		if (concernedMobileParty != null && concernedMobileParty.IsVisible)
		{
			return false;
		}
		if (TrackedArmy != null)
		{
			return true;
		}
		if (TrackedParty != null && TrackedParty.IsActive)
		{
			return TrackedParty.AttachedTo == null;
		}
		return false;
	}

	internal void UpdatePosition()
	{
		if (_concernedMobileParty != null)
		{
			float height = 0f;
			Campaign.Current.MapSceneWrapper.GetHeightAtPoint(_concernedMobileParty.VisualPosition2DWithoutError, ref height);
			Vec3 worldSpacePosition = _concernedMobileParty.VisualPosition2DWithoutError.ToVec3(height) + new Vec3(0f, 0f, 1f);
			_latestX = 0f;
			_latestY = 0f;
			_latestW = 0f;
			MBWindowManager.WorldToScreenInsideUsableArea(_mapCamera, worldSpacePosition, ref _latestX, ref _latestY, ref _latestW);
			_partyPositionBind = new Vec2(_latestX, _latestY);
			_isBehindBind = _latestW < 0f;
		}
	}

	public void ExecuteToggleTrack()
	{
		if (IsTracked)
		{
			Untrack();
		}
		else
		{
			Track();
		}
	}

	private void Track()
	{
		IsTracked = true;
		if (!Campaign.Current.VisualTrackerManager.CheckTracked(_concernedMobileParty))
		{
			Campaign.Current.VisualTrackerManager.RegisterObject(_concernedMobileParty);
		}
	}

	private void Untrack()
	{
		IsTracked = false;
		if (Campaign.Current.VisualTrackerManager.CheckTracked(_concernedMobileParty))
		{
			Campaign.Current.VisualTrackerManager.RemoveTrackedObject(_concernedMobileParty);
		}
	}

	public void ExecuteGoToPosition()
	{
		if (_concernedMobileParty != null)
		{
			_fastMoveCameraToPosition?.Invoke(_concernedMobileParty.GetLogicalPosition().AsVec2);
		}
	}

	public void ExecuteShowTooltip()
	{
		if (TrackedArmy != null)
		{
			InformationManager.ShowTooltip(typeof(Army), TrackedArmy, true, false);
		}
		else if (TrackedParty != null)
		{
			InformationManager.ShowTooltip(typeof(MobileParty), TrackedParty, true, false);
		}
	}

	public void ExecuteHideTooltip()
	{
		MBInformationManager.HideInformations();
	}

	public void RefreshBinding()
	{
		PartyPosition = _partyPositionBind;
		Name = _nameBind;
		IsEnabled = _isVisibleOnMapBind;
		IsBehind = _isBehindBind;
		FactionVisual = _factionVisualBind;
	}
}
