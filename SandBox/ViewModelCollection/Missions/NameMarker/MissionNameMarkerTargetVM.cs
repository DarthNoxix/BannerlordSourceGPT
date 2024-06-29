using System;
using System.Collections.Generic;
using SandBox.Objects;
using SandBox.Objects.AreaMarkers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Missions.NameMarker;

public class MissionNameMarkerTargetVM : ViewModel
{
	private class QuestMarkerComparer : IComparer<QuestMarkerVM>
	{
		public int Compare(QuestMarkerVM x, QuestMarkerVM y)
		{
			return x.QuestMarkerType.CompareTo(y.QuestMarkerType);
		}
	}

	public const string NameTypeNeutral = "Normal";

	public const string NameTypeFriendly = "Friendly";

	public const string NameTypeEnemy = "Enemy";

	public const string NameTypeNoble = "Noble";

	public const string NameTypePassage = "Passage";

	public const string NameTypeEnemyPassage = "Passage";

	public const string IconTypeCommonArea = "common_area";

	public const string IconTypeCharacter = "character";

	public const string IconTypePrisoner = "prisoner";

	public const string IconTypeNoble = "noble";

	public const string IconTypeBarber = "barber";

	public const string IconTypeBlacksmith = "blacksmith";

	public const string IconTypeGameHost = "game_host";

	public const string IconTypeHermit = "hermit";

	private Func<Vec3> _getPosition = () => Vec3.Zero;

	private Func<string> _getMarkerObjectName = () => string.Empty;

	private MBBindingList<QuestMarkerVM> _quests;

	private Vec2 _screenPosition;

	private int _distance;

	private string _name;

	private string _iconType = string.Empty;

	private string _nameType = string.Empty;

	private bool _isEnabled;

	private bool _isTracked;

	private bool _isQuestMainStory;

	private bool _isEnemy;

	private bool _isFriendly;

	public bool IsAdditionalTargetAgent { get; private set; }

	public bool IsMovingTarget { get; }

	public Agent TargetAgent { get; }

	public Alley TargetAlley { get; }

	public PassageUsePoint TargetPassageUsePoint { get; private set; }

	public Vec3 WorldPosition => _getPosition();

	[DataSourceProperty]
	public MBBindingList<QuestMarkerVM> Quests
	{
		get
		{
			return _quests;
		}
		set
		{
			if (value != _quests)
			{
				_quests = value;
				OnPropertyChangedWithValue(value, "Quests");
			}
		}
	}

	[DataSourceProperty]
	public Vec2 ScreenPosition
	{
		get
		{
			return _screenPosition;
		}
		set
		{
			if (value.x != _screenPosition.x || value.y != _screenPosition.y)
			{
				_screenPosition = value;
				OnPropertyChangedWithValue(value, "ScreenPosition");
			}
		}
	}

	[DataSourceProperty]
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

	[DataSourceProperty]
	public string IconType
	{
		get
		{
			return _iconType;
		}
		set
		{
			if (value != _iconType)
			{
				_iconType = value;
				OnPropertyChangedWithValue(value, "IconType");
			}
		}
	}

	[DataSourceProperty]
	public string NameType
	{
		get
		{
			return _nameType;
		}
		set
		{
			if (value != _nameType)
			{
				_nameType = value;
				OnPropertyChangedWithValue(value, "NameType");
			}
		}
	}

	[DataSourceProperty]
	public int Distance
	{
		get
		{
			return _distance;
		}
		set
		{
			if (value != _distance)
			{
				_distance = value;
				OnPropertyChangedWithValue(value, "Distance");
			}
		}
	}

	[DataSourceProperty]
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

	[DataSourceProperty]
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

	[DataSourceProperty]
	public bool IsQuestMainStory
	{
		get
		{
			return _isQuestMainStory;
		}
		set
		{
			if (value != _isQuestMainStory)
			{
				_isQuestMainStory = value;
				OnPropertyChangedWithValue(value, "IsQuestMainStory");
			}
		}
	}

	[DataSourceProperty]
	public bool IsEnemy
	{
		get
		{
			return _isEnemy;
		}
		set
		{
			if (value != _isEnemy)
			{
				_isEnemy = value;
				OnPropertyChangedWithValue(value, "IsEnemy");
			}
		}
	}

	[DataSourceProperty]
	public bool IsFriendly
	{
		get
		{
			return _isFriendly;
		}
		set
		{
			if (value != _isFriendly)
			{
				_isFriendly = value;
				OnPropertyChangedWithValue(value, "IsFriendly");
			}
		}
	}

	public MissionNameMarkerTargetVM(CommonAreaMarker commonAreaMarker)
	{
		IsMovingTarget = false;
		NameType = "Passage";
		IconType = "common_area";
		Quests = new MBBindingList<QuestMarkerVM>();
		TargetAlley = Hero.MainHero.CurrentSettlement.Alleys[commonAreaMarker.AreaIndex - 1];
		UpdateAlleyStatus();
		_getPosition = () => commonAreaMarker.GetPosition();
		_getMarkerObjectName = () => commonAreaMarker.GetName().ToString();
		CampaignEvents.AlleyOwnerChanged.AddNonSerializedListener(this, OnAlleyOwnerChanged);
		RefreshValues();
	}

	public MissionNameMarkerTargetVM(WorkshopType workshopType, Vec3 signPosition)
	{
		IsMovingTarget = false;
		NameType = "Passage";
		IconType = workshopType.StringId;
		Quests = new MBBindingList<QuestMarkerVM>();
		_getPosition = () => signPosition;
		_getMarkerObjectName = () => workshopType.Name.ToString();
		RefreshValues();
	}

	public MissionNameMarkerTargetVM(PassageUsePoint passageUsePoint)
	{
		TargetPassageUsePoint = passageUsePoint;
		IsMovingTarget = false;
		NameType = "Passage";
		IconType = passageUsePoint.ToLocation.StringId;
		Quests = new MBBindingList<QuestMarkerVM>();
		_getPosition = () => passageUsePoint.GameEntity.GlobalPosition;
		_getMarkerObjectName = () => passageUsePoint.ToLocation.Name.ToString();
		RefreshValues();
	}

	public MissionNameMarkerTargetVM(Agent agent, bool isAdditionalTargetAgent)
	{
		IsMovingTarget = true;
		TargetAgent = agent;
		NameType = "Normal";
		IconType = "character";
		IsAdditionalTargetAgent = isAdditionalTargetAgent;
		Quests = new MBBindingList<QuestMarkerVM>();
		CharacterObject characterObject = (CharacterObject)agent.Character;
		if (characterObject != null)
		{
			Hero heroObject = characterObject.HeroObject;
			if (heroObject != null && heroObject.IsLord)
			{
				IconType = "noble";
				NameType = "Noble";
				if (FactionManager.IsAtWarAgainstFaction(characterObject.HeroObject.MapFaction, Hero.MainHero.MapFaction))
				{
					NameType = "Enemy";
					IsEnemy = true;
				}
				else if (FactionManager.IsAlliedWithFaction(characterObject.HeroObject.MapFaction, Hero.MainHero.MapFaction))
				{
					NameType = "Friendly";
					IsFriendly = true;
				}
			}
			if (characterObject.HeroObject != null && characterObject.HeroObject.IsPrisoner)
			{
				IconType = "prisoner";
			}
			if (agent.IsHuman && agent != Agent.Main && !IsAdditionalTargetAgent)
			{
				UpdateQuestStatus();
			}
			if (characterObject == Settlement.CurrentSettlement?.Culture?.Barber)
			{
				IconType = "barber";
			}
			else if (characterObject == Settlement.CurrentSettlement?.Culture?.Blacksmith)
			{
				IconType = "blacksmith";
			}
			else if (characterObject == Settlement.CurrentSettlement?.Culture?.TavernGamehost)
			{
				IconType = "game_host";
			}
			else if (characterObject.StringId == "sp_hermit")
			{
				IconType = "hermit";
			}
		}
		_getPosition = delegate
		{
			Vec3 position = agent.Position;
			position.z = agent.GetEyeGlobalPosition().Z;
			return position;
		};
		_getMarkerObjectName = () => agent.Name;
		RefreshValues();
	}

	public MissionNameMarkerTargetVM(Vec3 position, string name, string iconType)
	{
		NameType = "Passage";
		IconType = iconType;
		Quests = new MBBindingList<QuestMarkerVM>();
		_getPosition = () => position;
		_getMarkerObjectName = () => name;
		RefreshValues();
	}

	public override void RefreshValues()
	{
		base.RefreshValues();
		Name = _getMarkerObjectName();
	}

	public override void OnFinalize()
	{
		base.OnFinalize();
		CampaignEvents.AlleyOwnerChanged.ClearListeners(this);
	}

	private void OnAlleyOwnerChanged(Alley alley, Hero newOwner, Hero oldOwner)
	{
		if (TargetAlley == alley && (newOwner == Hero.MainHero || oldOwner == Hero.MainHero))
		{
			UpdateAlleyStatus();
		}
	}

	private void UpdateAlleyStatus()
	{
		if (TargetAlley == null)
		{
			return;
		}
		Hero owner = TargetAlley.Owner;
		if (owner != null)
		{
			if (owner == Hero.MainHero)
			{
				NameType = "Friendly";
				IsFriendly = true;
				IsEnemy = false;
			}
			else
			{
				NameType = "Passage";
				IsFriendly = false;
				IsEnemy = true;
			}
		}
		else
		{
			NameType = "Normal";
			IsFriendly = false;
			IsEnemy = false;
		}
	}

	public void UpdateQuestStatus(SandBoxUIHelper.IssueQuestFlags issueQuestFlags)
	{
		SandBoxUIHelper.IssueQuestFlags[] issueQuestFlagsValues = SandBoxUIHelper.IssueQuestFlagsValues;
		foreach (SandBoxUIHelper.IssueQuestFlags questFlag in issueQuestFlagsValues)
		{
			if (questFlag != 0 && (issueQuestFlags & questFlag) != 0 && Quests.AllQ((QuestMarkerVM q) => q.IssueQuestFlag != questFlag))
			{
				Quests.Add(new QuestMarkerVM(questFlag));
				if ((questFlag & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) != 0 && (questFlag & SandBoxUIHelper.IssueQuestFlags.AvailableIssue) != 0 && (questFlag & SandBoxUIHelper.IssueQuestFlags.TrackedIssue) != 0)
				{
					IsTracked = true;
				}
				else if ((questFlag & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) != 0 && (questFlag & SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest) != 0 && (questFlag & SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest) != 0)
				{
					IsQuestMainStory = true;
				}
			}
		}
		Quests.Sort(new QuestMarkerComparer());
	}

	public void UpdateQuestStatus()
	{
		Quests.Clear();
		SandBoxUIHelper.IssueQuestFlags issueQuestFlags = SandBoxUIHelper.IssueQuestFlags.None;
		Hero hero = ((CharacterObject)(TargetAgent?.Character))?.HeroObject;
		if (hero != null)
		{
			List<(SandBoxUIHelper.IssueQuestFlags, TextObject, TextObject)> questStateOfHero = SandBoxUIHelper.GetQuestStateOfHero(hero);
			for (int i = 0; i < questStateOfHero.Count; i++)
			{
				issueQuestFlags |= questStateOfHero[i].Item1;
			}
		}
		if (TargetAgent != null && (TargetAgent.Character as CharacterObject)?.HeroObject?.Clan?.Leader != Hero.MainHero)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement != null && currentSettlement.LocationComplex?.FindCharacter(TargetAgent)?.IsVisualTracked == true)
			{
				issueQuestFlags |= SandBoxUIHelper.IssueQuestFlags.TrackedIssue;
			}
		}
		SandBoxUIHelper.IssueQuestFlags[] issueQuestFlagsValues = SandBoxUIHelper.IssueQuestFlagsValues;
		foreach (SandBoxUIHelper.IssueQuestFlags issueQuestFlags2 in issueQuestFlagsValues)
		{
			if (issueQuestFlags2 != 0 && (issueQuestFlags & issueQuestFlags2) != 0)
			{
				Quests.Add(new QuestMarkerVM(issueQuestFlags2));
				if ((issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) != 0 && (issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.AvailableIssue) != 0 && (issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.TrackedIssue) != 0)
				{
					IsTracked = true;
				}
				else if ((issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) != 0 && (issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest) != 0 && (issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest) != 0)
				{
					IsQuestMainStory = true;
				}
			}
		}
		Quests.Sort(new QuestMarkerComparer());
	}
}
