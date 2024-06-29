using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Missions.MissionLogics.Towns;
using SandBox.Objects;
using SandBox.Objects.AreaMarkers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Missions.NameMarker;

public class MissionNameMarkerVM : ViewModel
{
	private class MarkerDistanceComparer : IComparer<MissionNameMarkerTargetVM>
	{
		public int Compare(MissionNameMarkerTargetVM x, MissionNameMarkerTargetVM y)
		{
			return y.Distance.CompareTo(x.Distance);
		}
	}

	private readonly Camera _missionCamera;

	private readonly Mission _mission;

	private Vec3 _agentHeightOffset = new Vec3(0f, 0f, 0.35f);

	private Vec3 _defaultHeightOffset = new Vec3(0f, 0f, 2f);

	private bool _prevEnabledState;

	private bool _fadeOutTimerStarted;

	private float _fadeOutTimer;

	private Dictionary<Agent, SandBoxUIHelper.IssueQuestFlags> _additionalTargetAgents;

	private Dictionary<string, (Vec3, string, string)> _additionalGenericTargets;

	private Dictionary<string, MissionNameMarkerTargetVM> _genericTargets;

	private readonly MarkerDistanceComparer _distanceComparer;

	private readonly List<string> PassagePointFilter = new List<string> { "Empty Shop" };

	private MBBindingList<MissionNameMarkerTargetVM> _targets;

	private bool _isEnabled;

	public bool IsTargetsAdded { get; private set; }

	[DataSourceProperty]
	public MBBindingList<MissionNameMarkerTargetVM> Targets
	{
		get
		{
			return _targets;
		}
		set
		{
			if (value != _targets)
			{
				_targets = value;
				OnPropertyChangedWithValue(value, "Targets");
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
				UpdateTargetStates(value);
				Game.Current.EventManager.TriggerEvent(new MissionNameMarkerToggleEvent(value));
			}
		}
	}

	public MissionNameMarkerVM(Mission mission, Camera missionCamera, Dictionary<Agent, SandBoxUIHelper.IssueQuestFlags> additionalTargetAgents, Dictionary<string, (Vec3, string, string)> additionalGenericTargets)
	{
		Targets = new MBBindingList<MissionNameMarkerTargetVM>();
		_distanceComparer = new MarkerDistanceComparer();
		_missionCamera = missionCamera;
		_additionalTargetAgents = additionalTargetAgents;
		_additionalGenericTargets = additionalGenericTargets;
		_genericTargets = new Dictionary<string, MissionNameMarkerTargetVM>();
		_mission = mission;
	}

	public override void RefreshValues()
	{
		base.RefreshValues();
		Targets.ApplyActionOnAllItems(delegate(MissionNameMarkerTargetVM x)
		{
			x.RefreshValues();
		});
	}

	public override void OnFinalize()
	{
		base.OnFinalize();
		Targets.ApplyActionOnAllItems(delegate(MissionNameMarkerTargetVM x)
		{
			x.OnFinalize();
		});
	}

	public void Tick(float dt)
	{
		if (!IsTargetsAdded)
		{
			if (_mission.MainAgent != null)
			{
				if (_additionalTargetAgents != null)
				{
					foreach (KeyValuePair<Agent, SandBoxUIHelper.IssueQuestFlags> additionalTargetAgent in _additionalTargetAgents)
					{
						AddAgentTarget(additionalTargetAgent.Key, isAdditional: true);
						UpdateAdditionalTargetAgentQuestStatus(additionalTargetAgent.Key, additionalTargetAgent.Value);
					}
				}
				if (_additionalGenericTargets != null)
				{
					foreach (KeyValuePair<string, (Vec3, string, string)> additionalGenericTarget in _additionalGenericTargets)
					{
						AddGenericMarker(additionalGenericTarget.Key, additionalGenericTarget.Value.Item1, additionalGenericTarget.Value.Item2, additionalGenericTarget.Value.Item3);
					}
				}
				foreach (Agent agent in _mission.Agents)
				{
					AddAgentTarget(agent);
				}
				if (Hero.MainHero.CurrentSettlement != null)
				{
					List<CommonAreaMarker> list = (from x in _mission.ActiveMissionObjects.FindAllWithType<CommonAreaMarker>()
						where x.GameEntity.HasTag("alley_marker")
						select x).ToList();
					if (Hero.MainHero.CurrentSettlement.Alleys.Count > 0)
					{
						foreach (CommonAreaMarker item in list)
						{
							Alley alley = item.GetAlley();
							if (alley != null && alley.Owner != null)
							{
								Targets.Add(new MissionNameMarkerTargetVM(item));
							}
						}
					}
					foreach (PassageUsePoint item2 in from passage in _mission.ActiveMissionObjects.FindAllWithType<PassageUsePoint>().ToList()
						where passage.ToLocation != null && !PassagePointFilter.Exists((string s) => passage.ToLocation.Name.Contains(s))
						select passage)
					{
						if (!item2.ToLocation.CanBeReserved || item2.ToLocation.IsReserved)
						{
							Targets.Add(new MissionNameMarkerTargetVM(item2));
						}
					}
					if (_mission.HasMissionBehavior<WorkshopMissionHandler>())
					{
						foreach (Tuple<Workshop, GameEntity> item3 in from s in _mission.GetMissionBehavior<WorkshopMissionHandler>().WorkshopSignEntities.ToList()
							where s.Item1.WorkshopType != null
							select s)
						{
							Targets.Add(new MissionNameMarkerTargetVM(item3.Item1.WorkshopType, item3.Item2.GlobalPosition - _defaultHeightOffset));
						}
					}
				}
			}
			IsTargetsAdded = true;
		}
		if (IsEnabled)
		{
			UpdateTargetScreenPositions();
			_fadeOutTimerStarted = false;
			_fadeOutTimer = 0f;
			_prevEnabledState = IsEnabled;
		}
		else
		{
			if (_prevEnabledState)
			{
				_fadeOutTimerStarted = true;
			}
			if (_fadeOutTimerStarted)
			{
				_fadeOutTimer += dt;
			}
			if (_fadeOutTimer < 2f)
			{
				UpdateTargetScreenPositions();
			}
			else
			{
				_fadeOutTimerStarted = false;
			}
		}
		_prevEnabledState = IsEnabled;
	}

	private void UpdateTargetScreenPositions()
	{
		foreach (MissionNameMarkerTargetVM target in Targets)
		{
			float screenX = -100f;
			float screenY = -100f;
			float w = 0f;
			Vec3 vec = ((target.TargetAgent != null) ? _agentHeightOffset : _defaultHeightOffset);
			MBWindowManager.WorldToScreenInsideUsableArea(_missionCamera, target.WorldPosition + vec, ref screenX, ref screenY, ref w);
			if (w > 0f)
			{
				target.ScreenPosition = new Vec2(screenX, screenY);
				target.Distance = (int)(target.WorldPosition - _missionCamera.Position).Length;
			}
			else
			{
				target.Distance = -1;
				target.ScreenPosition = new Vec2(-100f, -100f);
			}
		}
		Targets.Sort(_distanceComparer);
	}

	public void OnConversationEnd()
	{
		foreach (Agent agent in _mission.Agents)
		{
			AddAgentTarget(agent);
		}
		foreach (MissionNameMarkerTargetVM target in Targets)
		{
			if (!target.IsAdditionalTargetAgent)
			{
				target.UpdateQuestStatus();
			}
		}
	}

	public void OnAgentBuild(Agent agent)
	{
		AddAgentTarget(agent);
	}

	public void OnAgentRemoved(Agent agent)
	{
		RemoveAgentTarget(agent);
	}

	public void OnAgentDeleted(Agent agent)
	{
		RemoveAgentTarget(agent);
	}

	public void UpdateAdditionalTargetAgentQuestStatus(Agent agent, SandBoxUIHelper.IssueQuestFlags issueQuestFlags)
	{
		Targets.FirstOrDefault((MissionNameMarkerTargetVM t) => t.TargetAgent == agent)?.UpdateQuestStatus(issueQuestFlags);
	}

	public void AddGenericMarker(string markerIdentifier, Vec3 markerPosition, string name, string iconType)
	{
		if (_genericTargets.TryGetValue(markerIdentifier, out var _))
		{
			Debug.FailedAssert("Marker with identifier: " + markerIdentifier + " already exists", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\Missions\\NameMarker\\MissionNameMarkerVM.cs", "AddGenericMarker", 229);
			return;
		}
		MissionNameMarkerTargetVM missionNameMarkerTargetVM = new MissionNameMarkerTargetVM(markerPosition, name, iconType);
		_genericTargets.Add(markerIdentifier, missionNameMarkerTargetVM);
		Targets.Add(missionNameMarkerTargetVM);
	}

	public void RemoveGenericMarker(string markerIdentifier)
	{
		if (_genericTargets.TryGetValue(markerIdentifier, out var value))
		{
			_genericTargets.Remove(markerIdentifier);
			Targets.Remove(value);
		}
		else
		{
			Debug.FailedAssert("Marker with identifier: " + markerIdentifier + " does not exist", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\Missions\\NameMarker\\MissionNameMarkerVM.cs", "RemoveGenericMarker", 248);
		}
	}

	public void AddAgentTarget(Agent agent, bool isAdditional = false)
	{
		if (agent?.Character == null || agent == Agent.Main || !agent.IsActive() || Targets.Any((MissionNameMarkerTargetVM t) => t.TargetAgent == agent))
		{
			return;
		}
		if (!isAdditional && !agent.Character.IsHero)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if ((currentSettlement == null || currentSettlement.LocationComplex?.FindCharacter(agent)?.IsVisualTracked != true) && (!(agent.Character is CharacterObject characterObject) || (characterObject.Occupation != Occupation.RansomBroker && characterObject.Occupation != Occupation.Tavernkeeper)) && agent.Character != Settlement.CurrentSettlement?.Culture?.Blacksmith && agent.Character != Settlement.CurrentSettlement?.Culture?.Barber && agent.Character != Settlement.CurrentSettlement?.Culture?.TavernGamehost && !(agent.Character.StringId == "sp_hermit"))
			{
				return;
			}
		}
		MissionNameMarkerTargetVM item = new MissionNameMarkerTargetVM(agent, isAdditional);
		Targets.Add(item);
	}

	public void RemoveAgentTarget(Agent agent)
	{
		if (Targets.SingleOrDefault((MissionNameMarkerTargetVM t) => t.TargetAgent == agent) != null)
		{
			Targets.Remove(Targets.Single((MissionNameMarkerTargetVM t) => t.TargetAgent == agent));
		}
	}

	private void UpdateTargetStates(bool state)
	{
		foreach (MissionNameMarkerTargetVM target in Targets)
		{
			target.IsEnabled = state;
		}
	}
}
