using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate;

public class NameplateVM : ViewModel
{
	protected enum NameplateSize
	{
		Small,
		Normal,
		Big
	}

	protected bool _bindIsTargetedByTutorial;

	private Vec2 _position;

	private bool _isVisibleOnMap;

	private string _factionColor;

	private int _sizeType;

	private bool _isTargetedByTutorial;

	private float _distanceToCamera;

	public double Scale { get; set; }

	public int NameplateOrder { get; set; }

	public int SizeType
	{
		get
		{
			return _sizeType;
		}
		set
		{
			if (value != _sizeType)
			{
				_sizeType = value;
				OnPropertyChangedWithValue(value, "SizeType");
			}
		}
	}

	public string FactionColor
	{
		get
		{
			return _factionColor;
		}
		set
		{
			if (value != _factionColor)
			{
				_factionColor = value;
				OnPropertyChangedWithValue(value, "FactionColor");
			}
		}
	}

	public float DistanceToCamera
	{
		get
		{
			return _distanceToCamera;
		}
		set
		{
			if (value != _distanceToCamera)
			{
				_distanceToCamera = value;
				OnPropertyChangedWithValue(value, "DistanceToCamera");
			}
		}
	}

	public bool IsVisibleOnMap
	{
		get
		{
			return _isVisibleOnMap;
		}
		set
		{
			if (value != _isVisibleOnMap)
			{
				_isVisibleOnMap = value;
				OnPropertyChangedWithValue(value, "IsVisibleOnMap");
			}
		}
	}

	public bool IsTargetedByTutorial
	{
		get
		{
			return _isTargetedByTutorial;
		}
		set
		{
			if (value != _isTargetedByTutorial)
			{
				_isTargetedByTutorial = value;
				OnPropertyChangedWithValue(value, "IsTargetedByTutorial");
				OnPropertyChanged("ShouldShowFullName");
				OnPropertyChanged("IsTracked");
			}
		}
	}

	public Vec2 Position
	{
		get
		{
			return _position;
		}
		set
		{
			if (_position != value)
			{
				_position = value;
				OnPropertyChangedWithValue(value, "Position");
			}
		}
	}

	public NameplateVM()
	{
		if (Game.Current != null)
		{
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(OnTutorialNotificationElementChanged);
		}
	}

	public override void OnFinalize()
	{
		base.OnFinalize();
		Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(OnTutorialNotificationElementChanged);
	}

	private void OnTutorialNotificationElementChanged(TutorialNotificationElementChangeEvent obj)
	{
		RefreshTutorialStatus(obj?.NewNotificationElementID ?? string.Empty);
	}

	public virtual void Initialize(GameEntity strategicEntity)
	{
		SizeType = 1;
	}

	public virtual void RefreshDynamicProperties(bool forceUpdate)
	{
	}

	public virtual void RefreshPosition()
	{
	}

	public virtual void RefreshRelationStatus()
	{
	}

	public virtual void RefreshTutorialStatus(string newTutorialHighlightElementID)
	{
	}
}
