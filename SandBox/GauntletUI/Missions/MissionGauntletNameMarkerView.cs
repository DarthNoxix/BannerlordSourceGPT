using System.Collections.Generic;
using SandBox.View.Missions;
using SandBox.ViewModelCollection;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Missions;

[OverrideView(typeof(MissionNameMarkerUIHandler))]
public class MissionGauntletNameMarkerView : MissionView
{
	private GauntletLayer _gauntletLayer;

	private MissionNameMarkerVM _dataSource;

	private readonly Dictionary<Agent, SandBoxUIHelper.IssueQuestFlags> _additionalTargetAgents;

	private Dictionary<string, (Vec3, string, string)> _additionalGenericTargets;

	public MissionGauntletNameMarkerView()
	{
		_additionalTargetAgents = new Dictionary<Agent, SandBoxUIHelper.IssueQuestFlags>();
		_additionalGenericTargets = new Dictionary<string, (Vec3, string, string)>();
	}

	public override void OnMissionScreenInitialize()
	{
		((MissionView)this).OnMissionScreenInitialize();
		_dataSource = new MissionNameMarkerVM(((MissionBehavior)this).Mission, ((MissionView)this).MissionScreen.CombatCamera, _additionalTargetAgents, _additionalGenericTargets);
		_gauntletLayer = new GauntletLayer(1);
		_gauntletLayer.LoadMovie("NameMarker", _dataSource);
		((ScreenBase)(object)((MissionView)this).MissionScreen).AddLayer((ScreenLayer)_gauntletLayer);
		CampaignEvents.ConversationEnded.AddNonSerializedListener(this, OnConversationEnd);
	}

	public override void OnMissionScreenFinalize()
	{
		((MissionView)this).OnMissionScreenFinalize();
		((ScreenBase)(object)((MissionView)this).MissionScreen).RemoveLayer((ScreenLayer)_gauntletLayer);
		_gauntletLayer = null;
		_dataSource.OnFinalize();
		_dataSource = null;
		_additionalTargetAgents.Clear();
		CampaignEvents.ConversationEnded.ClearListeners(this);
		InformationManager.ClearAllMessages();
	}

	public override void OnMissionScreenTick(float dt)
	{
		((MissionView)this).OnMissionScreenTick(dt);
		if (((MissionView)this).Input.IsGameKeyDown(5))
		{
			_dataSource.IsEnabled = true;
		}
		else
		{
			_dataSource.IsEnabled = false;
		}
		_dataSource.Tick(dt);
	}

	public override void OnAgentBuild(Agent affectedAgent, Banner banner)
	{
		((MissionBehavior)this).OnAgentBuild(affectedAgent, banner);
		_dataSource?.OnAgentBuild(affectedAgent);
	}

	public override void OnAgentDeleted(Agent affectedAgent)
	{
		_dataSource.OnAgentDeleted(affectedAgent);
	}

	public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
	{
		_dataSource.OnAgentRemoved(affectedAgent);
	}

	private void OnConversationEnd(IEnumerable<CharacterObject> conversationCharacters)
	{
		_dataSource.OnConversationEnd();
	}

	public override void OnPhotoModeActivated()
	{
		((MissionView)this).OnPhotoModeActivated();
		_gauntletLayer.UIContext.ContextAlpha = 0f;
	}

	public override void OnPhotoModeDeactivated()
	{
		((MissionView)this).OnPhotoModeDeactivated();
		_gauntletLayer.UIContext.ContextAlpha = 1f;
	}

	public void UpdateAgentTargetQuestStatus(Agent agent, SandBoxUIHelper.IssueQuestFlags issueQuestFlags)
	{
		if (agent != null)
		{
			MissionNameMarkerVM dataSource = _dataSource;
			SandBoxUIHelper.IssueQuestFlags value;
			if (dataSource != null && dataSource.IsTargetsAdded)
			{
				_dataSource.UpdateAdditionalTargetAgentQuestStatus(agent, issueQuestFlags);
			}
			else if (_additionalTargetAgents.TryGetValue(agent, out value))
			{
				_additionalTargetAgents[agent] = issueQuestFlags;
			}
		}
	}

	public void AddGenericMarker(string markerIdentifier, Vec3 globalPosition, string name, string iconType)
	{
		MissionNameMarkerVM dataSource = _dataSource;
		if (dataSource != null && dataSource.IsTargetsAdded)
		{
			_dataSource.AddGenericMarker(markerIdentifier, globalPosition, name, iconType);
		}
		else
		{
			_additionalGenericTargets.Add(markerIdentifier, (globalPosition, name, iconType));
		}
	}

	public void RemoveGenericMarker(string markerIdentifier)
	{
		MissionNameMarkerVM dataSource = _dataSource;
		if (dataSource != null && dataSource.IsTargetsAdded)
		{
			_dataSource.RemoveGenericMarker(markerIdentifier);
		}
		else
		{
			_additionalGenericTargets.Remove(markerIdentifier);
		}
	}

	public void AddAgentTarget(Agent agent, SandBoxUIHelper.IssueQuestFlags issueQuestFlags)
	{
		if (agent != null)
		{
			MissionNameMarkerVM dataSource = _dataSource;
			SandBoxUIHelper.IssueQuestFlags value;
			if (dataSource != null && dataSource.IsTargetsAdded)
			{
				_dataSource.AddAgentTarget(agent, isAdditional: true);
				_dataSource.UpdateAdditionalTargetAgentQuestStatus(agent, issueQuestFlags);
			}
			else if (!_additionalTargetAgents.TryGetValue(agent, out value))
			{
				_additionalTargetAgents.Add(agent, issueQuestFlags);
			}
		}
	}

	public void RemoveAgentTarget(Agent agent)
	{
		if (agent != null)
		{
			MissionNameMarkerVM dataSource = _dataSource;
			SandBoxUIHelper.IssueQuestFlags value;
			if (dataSource != null && dataSource.IsTargetsAdded)
			{
				_dataSource.RemoveAgentTarget(agent);
			}
			else if (_additionalTargetAgents.TryGetValue(agent, out value))
			{
				_additionalTargetAgents.Remove(agent);
			}
		}
	}
}
