using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions;

public class MissionConversationCameraView : MissionView
{
	private MissionMainAgentController _missionMainAgentController;

	private Agent _speakerAgent;

	private Agent _listenerAgent;

	public override void AfterStart()
	{
		_missionMainAgentController = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionMainAgentController>();
	}

	public override bool UpdateOverridenCamera(float dt)
	{
		MissionMode mode = ((MissionBehavior)this).Mission.Mode;
		if ((mode == MissionMode.Conversation || mode == MissionMode.Barter) && !((MissionView)this).MissionScreen.IsCheatGhostMode)
		{
			UpdateAgentLooksForConversation();
		}
		else if (_missionMainAgentController.CustomLookDir.IsNonZero)
		{
			_missionMainAgentController.CustomLookDir = Vec3.Zero;
		}
		return false;
	}

	private void UpdateAgentLooksForConversation()
	{
		Agent agent = null;
		ConversationManager conversationManager = Campaign.Current.ConversationManager;
		if (conversationManager.ConversationAgents != null && conversationManager.ConversationAgents.Count > 0)
		{
			_speakerAgent = (Agent)conversationManager.SpeakerAgent;
			_listenerAgent = (Agent)conversationManager.ListenerAgent;
			agent = Agent.Main.GetLookAgent();
			if (_speakerAgent == null)
			{
				return;
			}
			foreach (IAgent conversationAgent in conversationManager.ConversationAgents)
			{
				if (conversationAgent != _speakerAgent)
				{
					MakeAgentLookToSpeaker((Agent)conversationAgent);
				}
			}
			MakeSpeakerLookToListener();
		}
		SetFocusedObjectForCameraFocus();
		if (Agent.Main.GetLookAgent() != agent && _speakerAgent != null)
		{
			SpeakerAgentIsChanged();
		}
	}

	private void SpeakerAgentIsChanged()
	{
		Mission.Current.ConversationCharacterChanged();
	}

	private void SetFocusedObjectForCameraFocus()
	{
		if (_speakerAgent == Agent.Main)
		{
			_missionMainAgentController.InteractionComponent.SetCurrentFocusedObject((IFocusable)_listenerAgent, (IFocusable)null, true);
			_missionMainAgentController.CustomLookDir = (_listenerAgent.Position - Agent.Main.Position).NormalizedCopy();
			Agent.Main.SetLookAgent(_listenerAgent);
		}
		else
		{
			_missionMainAgentController.InteractionComponent.SetCurrentFocusedObject((IFocusable)_speakerAgent, (IFocusable)null, true);
			_missionMainAgentController.CustomLookDir = (_speakerAgent.Position - Agent.Main.Position).NormalizedCopy();
			Agent.Main.SetLookAgent(_speakerAgent);
		}
	}

	private void MakeAgentLookToSpeaker(Agent agent)
	{
		Vec3 position = agent.Position;
		Vec3 position2 = _speakerAgent.Position;
		position.z = agent.AgentVisuals.GetGlobalStableEyePoint(isHumanoid: true).z;
		position2.z = _speakerAgent.AgentVisuals.GetGlobalStableEyePoint(isHumanoid: true).z;
		agent.SetLookToPointOfInterest(_speakerAgent.AgentVisuals.GetGlobalStableEyePoint(isHumanoid: true));
		agent.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
		agent.LookDirection = (position2 - position).NormalizedCopy();
		agent.SetLookAgent(_speakerAgent);
	}

	private void MakeSpeakerLookToListener()
	{
		Vec3 position = _speakerAgent.Position;
		Vec3 position2 = _listenerAgent.Position;
		position.z = _speakerAgent.AgentVisuals.GetGlobalStableEyePoint(isHumanoid: true).z;
		position2.z = _listenerAgent.AgentVisuals.GetGlobalStableEyePoint(isHumanoid: true).z;
		_speakerAgent.SetLookToPointOfInterest(_listenerAgent.AgentVisuals.GetGlobalStableEyePoint(isHumanoid: true));
		_speakerAgent.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
		_speakerAgent.LookDirection = (position2 - position).NormalizedCopy();
		_speakerAgent.SetLookAgent(_listenerAgent);
	}

	private void SetConversationLook(Agent agent1, Agent agent2)
	{
		Vec3 position = agent2.Position;
		Vec3 position2 = agent1.Position;
		agent2.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
		agent1.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
		position.z = agent2.AgentVisuals.GetGlobalStableEyePoint(isHumanoid: true).z;
		position2.z = agent1.AgentVisuals.GetGlobalStableEyePoint(isHumanoid: true).z;
		agent1.SetLookToPointOfInterest(agent2.AgentVisuals.GetGlobalStableEyePoint(isHumanoid: true));
		agent2.SetLookToPointOfInterest(agent1.AgentVisuals.GetGlobalStableEyePoint(isHumanoid: true));
		agent1.LookDirection = (position2 - position).NormalizedCopy();
		agent2.LookDirection = (position - position2).NormalizedCopy();
		agent2.SetLookAgent(agent1);
		agent1.SetLookAgent(agent2);
	}
}
