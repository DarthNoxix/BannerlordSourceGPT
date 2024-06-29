using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Missions.AgentBehaviors;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;

namespace SandBox.Conversation.MissionLogics;

public class MissionConversationLogic : MissionLogic
{
	private const string CenterConversationPointMappingTag = "CenterConversationPoint";

	private MissionMode _oldMissionMode;

	private readonly CharacterObject _teleportNearCharacter;

	private GameEntity _selectedConversationPoint;

	private bool _conversationStarted;

	private bool _teleported;

	private bool _conversationAgentFound;

	private bool _disableStartConversation;

	private readonly Dictionary<string, MBList<GameEntity>> _conversationPoints;

	private string _customSpawnTag;

	public static MissionConversationLogic Current => Mission.Current.GetMissionBehavior<MissionConversationLogic>();

	public MissionState State { get; private set; }

	public ConversationManager ConversationManager { get; private set; }

	public bool IsReadyForConversation
	{
		get
		{
			if (ConversationAgent != null && ConversationManager != null)
			{
				if (Agent.Main != null)
				{
					return Agent.Main.IsActive();
				}
				return false;
			}
			return false;
		}
	}

	public Agent ConversationAgent { get; private set; }

	public MissionConversationLogic(CharacterObject teleportNearChar)
	{
		_teleportNearCharacter = teleportNearChar;
		_conversationPoints = new Dictionary<string, MBList<GameEntity>>();
	}

	public MissionConversationLogic()
	{
	}

	public override void OnBehaviorInitialize()
	{
		base.OnBehaviorInitialize();
		CampaignEvents.LocationCharactersSimulatedEvent.AddNonSerializedListener(this, OnLocationCharactersSimulated);
	}

	public override void OnRemoveBehavior()
	{
		base.OnRemoveBehavior();
		CampaignEvents.LocationCharactersAreReadyToSpawnEvent.ClearListeners(this);
	}

	public override void OnAgentBuild(Agent agent, Banner banner)
	{
		if (_teleportNearCharacter != null && agent.Character == _teleportNearCharacter)
		{
			ConversationAgent = agent;
			_conversationAgentFound = true;
		}
	}

	public void SetSpawnArea(Alley alley)
	{
		_customSpawnTag = alley.Tag;
	}

	public void SetSpawnArea(Workshop workshop)
	{
		_customSpawnTag = workshop.Tag;
	}

	public void SetSpawnArea(string customTag)
	{
		_customSpawnTag = customTag;
	}

	private void OnLocationCharactersSimulated()
	{
		if (_conversationAgentFound)
		{
			if (FillConversationPointList())
			{
				DetermineSpawnPoint();
				_teleported = TryToTeleportBothToCertainPoints();
			}
			else
			{
				base.Mission.GetMissionBehavior<MissionAgentHandler>()?.TeleportTargetAgentNearReferenceAgent(ConversationAgent, Agent.Main, teleportFollowers: true, teleportOpposite: false);
			}
		}
	}

	public override void OnMissionTick(float dt)
	{
		base.OnMissionTick(dt);
		if (!IsReadyForConversation)
		{
			return;
		}
		if (!_teleported)
		{
			base.Mission.GetMissionBehavior<MissionAgentHandler>().TeleportTargetAgentNearReferenceAgent(ConversationAgent, Agent.Main, teleportFollowers: true, teleportOpposite: false);
			_teleported = true;
		}
		if (_teleportNearCharacter != null && !_conversationStarted)
		{
			StartConversation(ConversationAgent, setActionsInstantly: true, isInitialization: true);
			if (ConversationManager.NeedsToActivateForMapConversation && !GameNetwork.IsReplay)
			{
				ConversationManager.BeginConversation();
			}
		}
	}

	private bool TryToTeleportBothToCertainPoints()
	{
		MissionAgentHandler missionBehavior = base.Mission.GetMissionBehavior<MissionAgentHandler>();
		bool flag = Agent.Main.MountAgent != null;
		WorldFrame worldFrame = new WorldFrame(_selectedConversationPoint.GetGlobalFrame().rotation, new WorldPosition(Agent.Main.Mission.Scene, _selectedConversationPoint.GetGlobalFrame().origin));
		worldFrame.Origin.SetVec2(worldFrame.Origin.AsVec2 + worldFrame.Rotation.f.AsVec2 * (flag ? 1f : 0.5f));
		WorldFrame worldFrame2 = new WorldFrame(_selectedConversationPoint.GetGlobalFrame().rotation, new WorldPosition(Agent.Main.Mission.Scene, _selectedConversationPoint.GetGlobalFrame().origin));
		worldFrame2.Origin.SetVec2(worldFrame2.Origin.AsVec2 - worldFrame2.Rotation.f.AsVec2 * (flag ? 1f : 0.5f));
		Vec3 vec = new Vec3(worldFrame.Origin.AsVec2 - worldFrame2.Origin.AsVec2);
		Vec3 vec2 = new Vec3(worldFrame2.Origin.AsVec2 - worldFrame.Origin.AsVec2);
		worldFrame.Rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		ConversationAgent.LookDirection = vec2.NormalizedCopy();
		ConversationAgent.TeleportToPosition(worldFrame.Origin.GetGroundVec3());
		worldFrame2.Rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		if (Agent.Main.MountAgent != null)
		{
			Vec3 vec3 = vec2.AsVec2.RightVec().ToVec3();
			Agent.Main.MountAgent.LookDirection = vec3.NormalizedCopy();
		}
		base.Mission.MainAgent.LookDirection = vec.NormalizedCopy();
		base.Mission.MainAgent.TeleportToPosition(worldFrame2.Origin.GetGroundVec3());
		SetConversationAgentAnimations(ConversationAgent);
		WorldPosition origin = worldFrame2.Origin;
		origin.SetVec2(origin.AsVec2 - worldFrame2.Rotation.s.AsVec2 * 2f);
		if (missionBehavior != null)
		{
			foreach (Agent agent in base.Mission.Agents)
			{
				LocationCharacter locationCharacter = LocationComplex.Current.FindCharacter(agent);
				AccompanyingCharacter accompanyingCharacter = PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(locationCharacter);
				if (accompanyingCharacter != null && accompanyingCharacter.IsFollowingPlayerAtMissionStart)
				{
					if (agent.MountAgent != null && Agent.Main.MountAgent != null)
					{
						agent.MountAgent.LookDirection = Agent.Main.MountAgent.LookDirection;
					}
					if (accompanyingCharacter.LocationCharacter.Character == _teleportNearCharacter)
					{
						agent.LookDirection = vec2.NormalizedCopy();
						Vec2 direction = worldFrame2.Rotation.f.AsVec2;
						agent.SetMovementDirection(in direction);
						agent.TeleportToPosition(worldFrame.Origin.GetGroundVec3());
					}
					else
					{
						agent.LookDirection = vec.NormalizedCopy();
						Vec2 direction = worldFrame.Rotation.f.AsVec2;
						agent.SetMovementDirection(in direction);
						agent.TeleportToPosition(origin.GetGroundVec3());
					}
				}
			}
		}
		_teleported = true;
		return true;
	}

	private void SetConversationAgentAnimations(Agent conversationAgent)
	{
		CampaignAgentComponent component = conversationAgent.GetComponent<CampaignAgentComponent>();
		AgentBehavior agentBehavior = component.AgentNavigator?.GetActiveBehavior();
		if (agentBehavior != null)
		{
			agentBehavior.IsActive = false;
			component.AgentNavigator.ForceThink(0f);
			conversationAgent.SetActionChannel(0, ActionIndexCache.act_none, ignorePriority: false, (ulong)conversationAgent.GetCurrentActionPriority(0), 0f, 1f, 0f);
			conversationAgent.TickActionChannels(0.1f);
			conversationAgent.AgentVisuals.GetSkeleton().TickAnimationsAndForceUpdate(0.1f, conversationAgent.AgentVisuals.GetGlobalFrame(), tickAnimsForChildren: true);
		}
	}

	private void OnConversationEnd()
	{
		foreach (Agent conversationAgent in ConversationManager.ConversationAgents)
		{
			conversationAgent.AgentVisuals.SetVisible(value: true);
			conversationAgent.AgentVisuals.SetClothComponentKeepStateOfAllMeshes(keepState: false);
			conversationAgent.MountAgent?.AgentVisuals.SetVisible(value: true);
		}
		if (base.Mission.Mode == MissionMode.Conversation)
		{
			base.Mission.SetMissionMode(_oldMissionMode, atStart: false);
		}
		if (Agent.Main != null)
		{
			Agent.Main.AgentVisuals.SetVisible(value: true);
			Agent.Main.AgentVisuals.SetClothComponentKeepStateOfAllMeshes(keepState: false);
			if (Agent.Main.MountAgent != null)
			{
				Agent.Main.MountAgent.AgentVisuals.SetVisible(value: true);
			}
		}
		base.Mission.MainAgentServer.Controller = Agent.ControllerType.Player;
		ConversationManager.ConversationEnd -= OnConversationEnd;
	}

	public override void EarlyStart()
	{
		State = Game.Current.GameStateManager.ActiveState as MissionState;
	}

	protected override void OnEndMission()
	{
		if (ConversationManager != null && ConversationManager.IsConversationInProgress)
		{
			ConversationManager.EndConversation();
		}
		State = null;
		CampaignEvents.LocationCharactersSimulatedEvent.ClearListeners(this);
	}

	public override void OnAgentInteraction(Agent userAgent, Agent agent)
	{
		if (Campaign.Current.GameMode != CampaignGameMode.Campaign)
		{
			return;
		}
		if (Game.Current.GameStateManager.ActiveState is MissionState)
		{
			if (IsThereAgentAction(userAgent, agent))
			{
				StartConversation(agent, setActionsInstantly: false);
			}
		}
		else
		{
			Debug.FailedAssert("Agent interaction must occur in MissionState.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Conversation\\Logics\\MissionConversationLogic.cs", "OnAgentInteraction", 288);
		}
	}

	public void StartConversation(Agent agent, bool setActionsInstantly, bool isInitialization = false)
	{
		_oldMissionMode = base.Mission.Mode;
		ConversationManager = Campaign.Current.ConversationManager;
		ConversationManager.SetupAndStartMissionConversation(agent, base.Mission.MainAgent, setActionsInstantly);
		ConversationManager.ConversationEnd += OnConversationEnd;
		_conversationStarted = true;
		foreach (Agent conversationAgent in ConversationManager.ConversationAgents)
		{
			conversationAgent.ForceAiBehaviorSelection();
			conversationAgent.AgentVisuals.SetClothComponentKeepStateOfAllMeshes(keepState: true);
		}
		base.Mission.MainAgentServer.AgentVisuals.SetClothComponentKeepStateOfAllMeshes(keepState: true);
		base.Mission.SetMissionMode(MissionMode.Conversation, setActionsInstantly);
	}

	public override bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
	{
		if (base.Mission.Mode != MissionMode.Battle && base.Mission.Mode != MissionMode.Duel && base.Mission.Mode != MissionMode.Conversation && !_disableStartConversation && otherAgent.IsActive())
		{
			return !otherAgent.IsEnemyOf(userAgent);
		}
		return false;
	}

	public override void OnRenderingStarted()
	{
		ConversationManager = Campaign.Current.ConversationManager;
		if (ConversationManager == null)
		{
			throw new ArgumentNullException("conversationManager");
		}
	}

	public void DisableStartConversation(bool isDisabled)
	{
		_disableStartConversation = isDisabled;
	}

	private bool FillConversationPointList()
	{
		List<GameEntity> list = base.Mission.Scene.FindEntitiesWithTag("sp_player_conversation").ToList();
		bool result = false;
		if (!list.IsEmpty())
		{
			List<AreaMarker> list2 = base.Mission.ActiveMissionObjects.FindAllWithType<AreaMarker>().ToList();
			foreach (GameEntity item in list)
			{
				bool flag = false;
				foreach (AreaMarker item2 in list2)
				{
					if (item2.IsPositionInRange(item.GlobalPosition))
					{
						if (_conversationPoints.ContainsKey(item2.Tag))
						{
							_conversationPoints[item2.Tag].Add(item);
						}
						else
						{
							_conversationPoints.Add(item2.Tag, new MBList<GameEntity> { item });
						}
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (_conversationPoints.ContainsKey("CenterConversationPoint"))
					{
						_conversationPoints["CenterConversationPoint"].Add(item);
						continue;
					}
					_conversationPoints.Add("CenterConversationPoint", new MBList<GameEntity> { item });
				}
			}
			result = true;
		}
		else
		{
			Debug.FailedAssert("Scene must have at least one 'sp_player_conversation' game entity. Scene Name: " + Mission.Current.Scene.GetName(), "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Conversation\\Logics\\MissionConversationLogic.cs", "FillConversationPointList", 382);
		}
		return result;
	}

	private void DetermineSpawnPoint()
	{
		if (_customSpawnTag != null && _conversationPoints.TryGetValue(_customSpawnTag, out var value))
		{
			_selectedConversationPoint = value.GetRandomElement();
			return;
		}
		string agentsTag = ConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SpecialTargetTag;
		if (agentsTag != null)
		{
			_selectedConversationPoint = _conversationPoints.FirstOrDefault((KeyValuePair<string, MBList<GameEntity>> x) => agentsTag.Contains(x.Key)).Value?.GetRandomElement();
		}
		if (_selectedConversationPoint == null)
		{
			if (_conversationPoints.ContainsKey("CenterConversationPoint"))
			{
				_selectedConversationPoint = _conversationPoints["CenterConversationPoint"].GetRandomElement();
			}
			else
			{
				_selectedConversationPoint = _conversationPoints.GetRandomElementInefficiently().Value.GetRandomElement();
			}
		}
	}
}
