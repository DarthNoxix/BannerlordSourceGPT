using System.Collections.Generic;
using SandBox.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace SandBox.Objects.AnimationPoints;

public class AnimationPoint : StandingPoint
{
	protected struct ItemForBone
	{
		public HumanBone HumanBone;

		public string ItemPrefabName;

		public bool IsVisible;

		public bool OldVisibility;

		public ItemForBone(HumanBone bone, string name, bool isVisible)
		{
			HumanBone = bone;
			ItemPrefabName = name;
			IsVisible = isVisible;
			OldVisibility = isVisible;
		}
	}

	private enum State
	{
		NotUsing,
		StartToUse,
		Using
	}

	private enum PairState
	{
		NoPair,
		BecomePair,
		Greeting,
		StartPairAnimation,
		Pair
	}

	private const string AlternativeTag = "alternative";

	private const float RangeThreshold = 0.2f;

	private const float RotationScoreThreshold = 0.99f;

	private const float ActionSpeedRandomMinValue = 0.8f;

	private const float AnimationRandomProgressMaxValue = 0.5f;

	public string ArriveAction = "";

	public string LoopStartAction = "";

	public string PairLoopStartAction = "";

	public string LeaveAction = "";

	public int GroupId = -1;

	public string RightHandItem = "";

	public HumanBone RightHandItemBone = HumanBone.ItemR;

	public string LeftHandItem = "";

	public HumanBone LeftHandItemBone = HumanBone.ItemL;

	public GameEntity PairEntity;

	public int MinUserToStartInteraction = 1;

	public bool ActivatePairs;

	public float MinWaitinSeconds = 30f;

	public float MaxWaitInSeconds = 120f;

	public float ForwardDistanceToPivotPoint;

	public float SideDistanceToPivotPoint;

	private bool _startPairAnimationWithGreeting;

	protected float ActionSpeed = 1f;

	public bool KeepOldVisibility;

	private ActionIndexCache ArriveActionCode;

	protected ActionIndexCache LoopStartActionCode;

	protected ActionIndexCache PairLoopStartActionCode;

	private ActionIndexCache LeaveActionCode;

	protected ActionIndexCache DefaultActionCode;

	private bool _resyncAnimations;

	private string _selectedRightHandItem;

	private string _selectedLeftHandItem;

	private State _state;

	private PairState _pairState;

	private Vec3 _pointRotation;

	private List<AnimationPoint> _pairPoints;

	private List<ItemForBone> _itemsForBones;

	private ActionIndexValueCache _lastAction;

	private Timer _greetingTimer;

	private GameEntity _animatedEntity;

	private Vec3 _animatedEntityDisplacement = Vec3.Zero;

	private EquipmentIndex _equipmentIndexMainHand;

	private EquipmentIndex _equipmentIndexOffHand;

	private readonly ActionIndexCache[] _greetingFrontActions = new ActionIndexCache[4]
	{
		ActionIndexCache.Create("act_greeting_front_1"),
		ActionIndexCache.Create("act_greeting_front_2"),
		ActionIndexCache.Create("act_greeting_front_3"),
		ActionIndexCache.Create("act_greeting_front_4")
	};

	private readonly ActionIndexCache[] _greetingRightActions = new ActionIndexCache[4]
	{
		ActionIndexCache.Create("act_greeting_right_1"),
		ActionIndexCache.Create("act_greeting_right_2"),
		ActionIndexCache.Create("act_greeting_right_3"),
		ActionIndexCache.Create("act_greeting_right_4")
	};

	private readonly ActionIndexCache[] _greetingLeftActions = new ActionIndexCache[4]
	{
		ActionIndexCache.Create("act_greeting_left_1"),
		ActionIndexCache.Create("act_greeting_left_2"),
		ActionIndexCache.Create("act_greeting_left_3"),
		ActionIndexCache.Create("act_greeting_left_4")
	};

	public override bool PlayerStopsUsingWhenInteractsWithOther => false;

	public bool IsArriveActionFinished { get; private set; }

	protected string SelectedRightHandItem
	{
		get
		{
			return _selectedRightHandItem;
		}
		set
		{
			if (value != _selectedRightHandItem)
			{
				ItemForBone newItem = new ItemForBone(RightHandItemBone, value, isVisible: false);
				AssignItemToBone(newItem);
				_selectedRightHandItem = value;
			}
		}
	}

	protected string SelectedLeftHandItem
	{
		get
		{
			return _selectedLeftHandItem;
		}
		set
		{
			if (value != _selectedLeftHandItem)
			{
				ItemForBone newItem = new ItemForBone(LeftHandItemBone, value, isVisible: false);
				AssignItemToBone(newItem);
				_selectedLeftHandItem = value;
			}
		}
	}

	public bool IsActive { get; private set; } = true;


	public override bool DisableCombatActionsOnUse => !base.IsInstantUse;

	public AnimationPoint()
	{
		_greetingTimer = null;
	}

	private void CreateVisualizer()
	{
		if (!(PairLoopStartActionCode != ActionIndexCache.act_none) && !(LoopStartActionCode != ActionIndexCache.act_none))
		{
			return;
		}
		_animatedEntity = GameEntity.CreateEmpty(base.GameEntity.Scene, isModifiableFromEditor: false);
		_animatedEntity.EntityFlags |= EntityFlags.DontSaveToScene;
		_animatedEntity.Name = "ap_visual_entity";
		MBActionSet actionSet = MBActionSet.GetActionSetWithIndex(0);
		ActionIndexCache actionIndexCache = ActionIndexCache.act_none;
		int numberOfActionSets = MBActionSet.GetNumberOfActionSets();
		for (int i = 0; i < numberOfActionSets; i++)
		{
			MBActionSet actionSetWithIndex = MBActionSet.GetActionSetWithIndex(i);
			if (ArriveActionCode == ActionIndexCache.act_none || MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, ArriveActionCode))
			{
				if (PairLoopStartActionCode != ActionIndexCache.act_none && MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, PairLoopStartActionCode))
				{
					actionSet = actionSetWithIndex;
					actionIndexCache = PairLoopStartActionCode;
					break;
				}
				if (LoopStartActionCode != ActionIndexCache.act_none && MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, LoopStartActionCode))
				{
					actionSet = actionSetWithIndex;
					actionIndexCache = LoopStartActionCode;
					break;
				}
			}
		}
		if (actionIndexCache == null || actionIndexCache == ActionIndexCache.act_none)
		{
			actionIndexCache = ActionIndexCache.Create("act_jump_loop");
		}
		_animatedEntity.CreateAgentSkeleton("human_skeleton", isHumanoid: true, actionSet, "human", MBObjectManager.Instance.GetObject<Monster>("human"));
		_animatedEntity.Skeleton.SetAgentActionChannel(0, actionIndexCache);
		_animatedEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("roman_cloth_tunic_a"));
		_animatedEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("casual_02_boots"));
		_animatedEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("hands_male_a"));
		_animatedEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("head_male_a"));
		_animatedEntityDisplacement = Vec3.Zero;
		if (ArriveActionCode != ActionIndexCache.act_none && (MBActionSet.GetActionAnimationFlags(actionSet, ArriveActionCode) & AnimFlags.anf_displace_position) != (AnimFlags)0uL)
		{
			_animatedEntityDisplacement = MBActionSet.GetActionDisplacementVector(actionSet, ArriveActionCode);
		}
		UpdateAnimatedEntityFrame();
	}

	private void UpdateAnimatedEntityFrame()
	{
		MatrixFrame frame = base.GameEntity.GetGlobalFrame();
		MatrixFrame m = default(MatrixFrame);
		m.rotation = Mat3.Identity;
		m.origin = _animatedEntityDisplacement;
		frame.origin = frame.TransformToParent(m).origin;
		_animatedEntity.SetFrame(ref frame);
	}

	protected override void OnEditModeVisibilityChanged(bool currentVisibility)
	{
		if (_animatedEntity != null)
		{
			_animatedEntity.SetVisibilityExcludeParents(currentVisibility);
			if (!base.GameEntity.IsGhostObject())
			{
				_resyncAnimations = true;
			}
		}
	}

	protected override void OnEditorTick(float dt)
	{
		if (_animatedEntity != null)
		{
			if (_resyncAnimations)
			{
				ResetAnimations();
				_resyncAnimations = false;
			}
			bool flag = _animatedEntity.IsVisibleIncludeParents();
			if (flag && !MBEditor.HelpersEnabled())
			{
				_animatedEntity.SetVisibilityExcludeParents(visible: false);
				flag = false;
			}
			if (flag)
			{
				UpdateAnimatedEntityFrame();
			}
		}
	}

	protected override void OnEditorInit()
	{
		_itemsForBones = new List<ItemForBone>();
		SetActionCodes();
		InitParameters();
		if (!base.GameEntity.IsGhostObject())
		{
			CreateVisualizer();
		}
	}

	protected override void OnRemoved(int removeReason)
	{
		base.OnRemoved(removeReason);
		if (_animatedEntity != null && _animatedEntity.Scene == base.GameEntity.Scene)
		{
			_animatedEntity.Remove(removeReason);
			_animatedEntity = null;
		}
	}

	protected void ResetAnimations()
	{
		ActionIndexCache actionIndexCache = ActionIndexCache.act_none;
		int numberOfActionSets = MBActionSet.GetNumberOfActionSets();
		for (int i = 0; i < numberOfActionSets; i++)
		{
			MBActionSet actionSetWithIndex = MBActionSet.GetActionSetWithIndex(i);
			if (ArriveActionCode == ActionIndexCache.act_none || MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, ArriveActionCode))
			{
				if (PairLoopStartActionCode != ActionIndexCache.act_none && MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, PairLoopStartActionCode))
				{
					actionIndexCache = PairLoopStartActionCode;
					break;
				}
				if (LoopStartActionCode != ActionIndexCache.act_none && MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, LoopStartActionCode))
				{
					actionIndexCache = LoopStartActionCode;
					break;
				}
			}
		}
		if (actionIndexCache != null && actionIndexCache != ActionIndexCache.act_none)
		{
			ActionIndexCache actionIndex = ActionIndexCache.Create("act_jump_loop");
			_animatedEntity.Skeleton.SetAgentActionChannel(0, actionIndex);
			_animatedEntity.Skeleton.SetAgentActionChannel(0, actionIndexCache);
		}
	}

	protected override void OnEditorVariableChanged(string variableName)
	{
		if (ShouldUpdateOnEditorVariableChanged(variableName))
		{
			if (_animatedEntity != null)
			{
				_animatedEntity.Remove(91);
			}
			SetActionCodes();
			CreateVisualizer();
		}
	}

	public void RequestResync()
	{
		_resyncAnimations = true;
	}

	public override void AfterMissionStart()
	{
		if (Agent.Main != null && LoopStartActionCode != ActionIndexCache.act_none && !MBActionSet.CheckActionAnimationClipExists(Agent.Main.ActionSet, LoopStartActionCode))
		{
			base.IsDisabledForPlayers = true;
		}
	}

	protected virtual bool ShouldUpdateOnEditorVariableChanged(string variableName)
	{
		if (!(variableName == "ArriveAction") && !(variableName == "LoopStartAction"))
		{
			return variableName == "PairLoopStartAction";
		}
		return true;
	}

	protected void ClearAssignedItems()
	{
		SetAgentItemsVisibility(isVisible: false);
		_itemsForBones.Clear();
	}

	protected void AssignItemToBone(ItemForBone newItem)
	{
		if (!string.IsNullOrEmpty(newItem.ItemPrefabName) && !_itemsForBones.Contains(newItem))
		{
			_itemsForBones.Add(newItem);
		}
	}

	public override bool IsDisabledForAgent(Agent agent)
	{
		if (base.HasUser && base.UserAgent == agent)
		{
			if (IsActive)
			{
				return base.IsDeactivated;
			}
			return true;
		}
		if (!IsActive || agent.MountAgent != null || base.IsDeactivated || !agent.IsOnLand() || (!agent.IsAIControlled && (base.IsDisabledForPlayers || base.HasUser)))
		{
			return true;
		}
		GameEntity parent = base.GameEntity.Parent;
		if (parent == null || !parent.HasScriptOfType<UsableMachine>() || !base.GameEntity.HasTag("alternative"))
		{
			return base.IsDisabledForAgent(agent);
		}
		if (agent.IsAIControlled && parent.HasTag("reserved"))
		{
			return true;
		}
		string text = ((agent.GetComponent<CampaignAgentComponent>()?.AgentNavigator != null) ? agent.GetComponent<CampaignAgentComponent>().AgentNavigator.SpecialTargetTag : string.Empty);
		if (!string.IsNullOrEmpty(text) && !parent.HasTag(text))
		{
			return true;
		}
		foreach (StandingPoint standingPoint in parent.GetFirstScriptOfType<UsableMachine>().StandingPoints)
		{
			if (standingPoint is AnimationPoint animationPoint && GroupId == animationPoint.GroupId && !animationPoint.IsDeactivated && (animationPoint.HasUser || (animationPoint.HasAIMovingTo && !animationPoint.IsAIMovingTo(agent))) && animationPoint.GameEntity.HasTag("alternative"))
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnInit()
	{
		base.OnInit();
		_itemsForBones = new List<ItemForBone>();
		SetActionCodes();
		InitParameters();
		SetScriptComponentToTick(GetTickRequirement());
	}

	private void InitParameters()
	{
		_greetingTimer = null;
		_pointRotation = Vec3.Zero;
		_state = State.NotUsing;
		_pairPoints = GetPairs(PairEntity);
		if (ActivatePairs)
		{
			SetPairsActivity(isActive: false);
		}
		LockUserPositions = true;
	}

	protected virtual void SetActionCodes()
	{
		ArriveActionCode = ActionIndexCache.Create(ArriveAction);
		LoopStartActionCode = ActionIndexCache.Create(LoopStartAction);
		PairLoopStartActionCode = ActionIndexCache.Create(PairLoopStartAction);
		LeaveActionCode = ActionIndexCache.Create(LeaveAction);
		SelectedRightHandItem = RightHandItem;
		SelectedLeftHandItem = LeftHandItem;
	}

	protected override bool DoesActionTypeStopUsingGameObject(Agent.ActionCodeType actionType)
	{
		return false;
	}

	public override TickRequirement GetTickRequirement()
	{
		if (base.HasUser)
		{
			return base.GetTickRequirement() | TickRequirement.Tick;
		}
		return base.GetTickRequirement();
	}

	protected override void OnTick(float dt)
	{
		base.OnTick(dt);
		Tick(dt);
	}

	private List<AnimationPoint> GetPairs(GameEntity entity)
	{
		List<AnimationPoint> list = new List<AnimationPoint>();
		if (entity != null)
		{
			if (entity.HasScriptOfType<AnimationPoint>())
			{
				AnimationPoint firstScriptOfType = entity.GetFirstScriptOfType<AnimationPoint>();
				list.Add(firstScriptOfType);
			}
			else
			{
				foreach (GameEntity child in entity.GetChildren())
				{
					list.AddRange(GetPairs(child));
				}
			}
		}
		if (list.Contains(this))
		{
			list.Remove(this);
		}
		return list;
	}

	public override WorldFrame GetUserFrameForAgent(Agent agent)
	{
		WorldFrame userFrameForAgent = base.GetUserFrameForAgent(agent);
		float agentScale = agent.AgentScale;
		userFrameForAgent.Origin.SetVec2(userFrameForAgent.Origin.AsVec2 + (userFrameForAgent.Rotation.f.AsVec2 * (0f - ForwardDistanceToPivotPoint) + userFrameForAgent.Rotation.s.AsVec2 * SideDistanceToPivotPoint) * (1f - agentScale));
		return userFrameForAgent;
	}

	private void Tick(float dt, bool isSimulation = false)
	{
		if (!base.HasUser)
		{
			return;
		}
		if (Game.Current != null && Game.Current.IsDevelopmentMode)
		{
			base.UserAgent.GetTargetPosition().IsNonZero();
		}
		ActionIndexValueCache currentActionValue = base.UserAgent.GetCurrentActionValue(0);
		switch (_state)
		{
		case State.NotUsing:
			if (IsTargetReached() && base.UserAgent.MovementVelocity.LengthSquared < 0.1f && base.UserAgent.IsOnLand())
			{
				if (ArriveActionCode != ActionIndexCache.act_none)
				{
					Agent userAgent = base.UserAgent;
					ActionIndexCache arriveActionCode = ArriveActionCode;
					long additionalFlags = 0L;
					float blendInPeriod = (isSimulation ? 0 : 0);
					userAgent.SetActionChannel(0, arriveActionCode, ignorePriority: false, (ulong)additionalFlags, 0f, MBRandom.RandomFloatRanged(0.8f, 1f), blendInPeriod);
				}
				_state = State.StartToUse;
			}
			break;
		case State.StartToUse:
			if (ArriveActionCode != ActionIndexCache.act_none && isSimulation)
			{
				SimulateAnimations(0.1f);
			}
			if (ArriveActionCode == ActionIndexCache.act_none || currentActionValue == ArriveActionCode || base.UserAgent.ActionSet.AreActionsAlternatives(currentActionValue, ArriveActionCode))
			{
				base.UserAgent.ClearTargetFrame();
				WorldFrame userFrameForAgent = GetUserFrameForAgent(base.UserAgent);
				_pointRotation = userFrameForAgent.Rotation.f;
				_pointRotation.Normalize();
				if (base.UserAgent != Agent.Main)
				{
					base.UserAgent.SetScriptedPositionAndDirection(ref userFrameForAgent.Origin, userFrameForAgent.Rotation.f.AsVec2.RotationInRadians, addHumanLikeDelay: false, Agent.AIScriptedFrameFlags.DoNotRun);
				}
				_state = State.Using;
			}
			break;
		case State.Using:
			if (isSimulation)
			{
				float dt2 = 0.1f;
				if (currentActionValue != ArriveActionCode)
				{
					dt2 = 0.01f + MBRandom.RandomFloat * 0.09f;
				}
				SimulateAnimations(dt2);
			}
			if (!IsArriveActionFinished && (ArriveActionCode == ActionIndexCache.act_none || base.UserAgent.GetCurrentActionValue(0) != ArriveActionCode))
			{
				IsArriveActionFinished = true;
				AddItemsToAgent();
			}
			if (IsRotationCorrectDuringUsage())
			{
				base.UserAgent.SetActionChannel(0, LoopStartActionCode, ignorePriority: false, 0uL, 0f, (ActionSpeed < 0.8f) ? ActionSpeed : MBRandom.RandomFloatRanged(0.8f, ActionSpeed), isSimulation ? 0 : 0, 0.4f, isSimulation ? MBRandom.RandomFloatRanged(0f, 0.5f) : 0f);
			}
			if (IsArriveActionFinished && base.UserAgent != Agent.Main)
			{
				PairTick(isSimulation);
			}
			break;
		}
	}

	private void PairTick(bool isSimulation)
	{
		MBList<Agent> pairEntityUsers = GetPairEntityUsers();
		if (PairEntity != null)
		{
			bool agentItemsVisibility = base.UserAgent != ConversationMission.OneToOneConversationAgent && pairEntityUsers.Count + 1 >= MinUserToStartInteraction;
			SetAgentItemsVisibility(agentItemsVisibility);
		}
		if (_pairState != 0 && pairEntityUsers.Count < MinUserToStartInteraction)
		{
			_pairState = PairState.NoPair;
			if (base.UserAgent != ConversationMission.OneToOneConversationAgent)
			{
				base.UserAgent.SetActionChannel(0, _lastAction, ignorePriority: false, (ulong)base.UserAgent.GetCurrentActionPriority(0));
				base.UserAgent.ResetLookAgent();
			}
			_greetingTimer = null;
		}
		else if (_pairState == PairState.NoPair && pairEntityUsers.Count >= MinUserToStartInteraction && IsRotationCorrectDuringUsage())
		{
			_lastAction = base.UserAgent.GetCurrentActionValue(0);
			if (_startPairAnimationWithGreeting)
			{
				_pairState = PairState.BecomePair;
				_greetingTimer = new Timer(Mission.Current.CurrentTime, (float)MBRandom.RandomInt(5) * 0.3f);
			}
			else
			{
				_pairState = PairState.StartPairAnimation;
			}
		}
		else if (_pairState == PairState.BecomePair && _greetingTimer.Check(Mission.Current.CurrentTime))
		{
			_greetingTimer = null;
			_pairState = PairState.Greeting;
			Vec3 eyeGlobalPosition = pairEntityUsers.GetRandomElement().GetEyeGlobalPosition();
			Vec3 eyeGlobalPosition2 = base.UserAgent.GetEyeGlobalPosition();
			Vec3 v = eyeGlobalPosition - eyeGlobalPosition2;
			v.Normalize();
			Mat3 rotation = base.UserAgent.Frame.rotation;
			if (Vec3.DotProduct(rotation.f, v) > 0f)
			{
				ActionIndexCache greetingActionId = GetGreetingActionId(eyeGlobalPosition2, eyeGlobalPosition, rotation);
				base.UserAgent.SetActionChannel(1, greetingActionId, ignorePriority: false, 0uL);
			}
		}
		else if (_pairState == PairState.Greeting && base.UserAgent.GetCurrentActionValue(1) == ActionIndexCache.act_none)
		{
			_pairState = PairState.StartPairAnimation;
		}
		if (_pairState == PairState.StartPairAnimation)
		{
			_pairState = PairState.Pair;
			base.UserAgent.SetActionChannel(0, PairLoopStartActionCode, ignorePriority: false, 0uL, 0f, MBRandom.RandomFloatRanged(0.8f, ActionSpeed), isSimulation ? 0 : 0, 0.4f, isSimulation ? MBRandom.RandomFloatRanged(0f, 0.5f) : 0f);
		}
		if (_pairState == PairState.Pair && IsRotationCorrectDuringUsage())
		{
			base.UserAgent.SetActionChannel(0, PairLoopStartActionCode, ignorePriority: false, 0uL, 0f, MBRandom.RandomFloatRanged(0.8f, ActionSpeed), isSimulation ? 0 : 0, 0.4f, isSimulation ? MBRandom.RandomFloatRanged(0f, 0.5f) : 0f);
		}
	}

	private ActionIndexCache GetGreetingActionId(Vec3 userAgentGlobalEyePoint, Vec3 lookTarget, Mat3 userAgentRot)
	{
		Vec3 vec = lookTarget - userAgentGlobalEyePoint;
		vec.Normalize();
		float num = Vec3.DotProduct(userAgentRot.f, vec);
		if (num > 0.8f)
		{
			return _greetingFrontActions[MBRandom.RandomInt(_greetingFrontActions.Length)];
		}
		if (num > 0f)
		{
			if (Vec3.DotProduct(Vec3.CrossProduct(vec, userAgentRot.f), userAgentRot.u) > 0f)
			{
				return _greetingRightActions[MBRandom.RandomInt(_greetingRightActions.Length)];
			}
			return _greetingLeftActions[MBRandom.RandomInt(_greetingLeftActions.Length)];
		}
		return ActionIndexCache.act_none;
	}

	private MBList<Agent> GetPairEntityUsers()
	{
		MBList<Agent> mBList = new MBList<Agent>();
		if (base.UserAgent != ConversationMission.OneToOneConversationAgent)
		{
			foreach (AnimationPoint pairPoint in _pairPoints)
			{
				if (pairPoint.HasUser && pairPoint._state == State.Using && pairPoint.UserAgent != ConversationMission.OneToOneConversationAgent)
				{
					mBList.Add(pairPoint.UserAgent);
				}
			}
			return mBList;
		}
		return mBList;
	}

	private void SetPairsActivity(bool isActive)
	{
		foreach (AnimationPoint pairPoint in _pairPoints)
		{
			pairPoint.IsActive = isActive;
			if (!isActive)
			{
				if (pairPoint.HasAIUser)
				{
					pairPoint.UserAgent.StopUsingGameObject();
				}
				pairPoint.MovingAgent?.StopUsingGameObject();
			}
		}
	}

	public override bool IsUsableByAgent(Agent userAgent)
	{
		if (IsActive)
		{
			return base.IsUsableByAgent(userAgent);
		}
		return false;
	}

	public override void OnUse(Agent userAgent)
	{
		base.OnUse(userAgent);
		_equipmentIndexMainHand = base.UserAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
		_equipmentIndexOffHand = base.UserAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
		_state = State.NotUsing;
		if (ActivatePairs)
		{
			SetPairsActivity(isActive: true);
		}
	}

	private void RevertWeaponWieldSheathState()
	{
		if (_equipmentIndexMainHand != EquipmentIndex.None && AutoSheathWeapons)
		{
			base.UserAgent.TryToWieldWeaponInSlot(_equipmentIndexMainHand, Agent.WeaponWieldActionType.WithAnimation, isWieldedOnSpawn: false);
		}
		else if (_equipmentIndexMainHand == EquipmentIndex.None && AutoWieldWeapons)
		{
			base.UserAgent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.WithAnimation);
		}
		if (_equipmentIndexOffHand != EquipmentIndex.None && AutoSheathWeapons)
		{
			base.UserAgent.TryToWieldWeaponInSlot(_equipmentIndexOffHand, Agent.WeaponWieldActionType.WithAnimation, isWieldedOnSpawn: false);
		}
		else if (_equipmentIndexOffHand == EquipmentIndex.None && AutoWieldWeapons)
		{
			base.UserAgent.TryToSheathWeaponInHand(Agent.HandIndex.OffHand, Agent.WeaponWieldActionType.WithAnimation);
		}
	}

	public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
	{
		SetAgentItemsVisibility(isVisible: false);
		RevertWeaponWieldSheathState();
		if (base.UserAgent.IsActive())
		{
			if (LeaveActionCode == ActionIndexCache.act_none)
			{
				base.UserAgent.SetActionChannel(0, LeaveActionCode, ignorePriority: false, (ulong)base.UserAgent.GetCurrentActionPriority(0));
			}
			else if (IsArriveActionFinished)
			{
				ActionIndexValueCache currentActionValue = base.UserAgent.GetCurrentActionValue(0);
				if (currentActionValue != LeaveActionCode && !base.UserAgent.ActionSet.AreActionsAlternatives(currentActionValue, LeaveActionCode))
				{
					base.UserAgent.SetActionChannel(0, LeaveActionCode, ignorePriority: false, (ulong)base.UserAgent.GetCurrentActionPriority(0));
				}
			}
			else
			{
				ActionIndexValueCache currentActionValue2 = userAgent.GetCurrentActionValue(0);
				if (currentActionValue2 == ArriveActionCode && ArriveActionCode != ActionIndexCache.act_none)
				{
					MBActionSet actionSet = userAgent.ActionSet;
					float currentActionProgress = userAgent.GetCurrentActionProgress(0);
					float actionBlendOutStartProgress = MBActionSet.GetActionBlendOutStartProgress(actionSet, currentActionValue2);
					if (currentActionProgress < actionBlendOutStartProgress)
					{
						float num = (actionBlendOutStartProgress - currentActionProgress) / actionBlendOutStartProgress;
						MBActionSet.GetActionBlendOutStartProgress(actionSet, LeaveActionCode);
					}
				}
			}
		}
		_pairState = PairState.NoPair;
		_lastAction = ActionIndexValueCache.act_none;
		if (base.UserAgent.GetLookAgent() != null)
		{
			base.UserAgent.ResetLookAgent();
		}
		IsArriveActionFinished = false;
		base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
		if (ActivatePairs)
		{
			SetPairsActivity(isActive: false);
		}
	}

	public override void SimulateTick(float dt)
	{
		Tick(dt, isSimulation: true);
	}

	public override bool HasAlternative()
	{
		return GroupId >= 0;
	}

	public float GetRandomWaitInSeconds()
	{
		if (MinWaitinSeconds < 0f || MaxWaitInSeconds < 0f)
		{
			return -1f;
		}
		if (!(MathF.Abs(MinWaitinSeconds - MaxWaitInSeconds) < float.Epsilon))
		{
			return MinWaitinSeconds + MBRandom.RandomFloat * (MaxWaitInSeconds - MinWaitinSeconds);
		}
		return MinWaitinSeconds;
	}

	public List<AnimationPoint> GetAlternatives()
	{
		List<AnimationPoint> list = new List<AnimationPoint>();
		IEnumerable<GameEntity> children = base.GameEntity.Parent.GetChildren();
		if (children != null)
		{
			foreach (GameEntity item in children)
			{
				AnimationPoint firstScriptOfType = item.GetFirstScriptOfType<AnimationPoint>();
				if (firstScriptOfType != null && firstScriptOfType.HasAlternative() && GroupId == firstScriptOfType.GroupId)
				{
					list.Add(firstScriptOfType);
				}
			}
			return list;
		}
		return list;
	}

	private void SimulateAnimations(float dt)
	{
		base.UserAgent.TickActionChannels(dt);
		Vec3 vec = base.UserAgent.ComputeAnimationDisplacement(dt);
		if (vec.LengthSquared > 0f)
		{
			base.UserAgent.TeleportToPosition(base.UserAgent.Position + vec);
		}
		base.UserAgent.AgentVisuals.GetSkeleton().TickAnimations(dt, base.UserAgent.AgentVisuals.GetGlobalFrame(), tickAnimsForChildren: true);
	}

	private bool IsTargetReached()
	{
		float num = Vec2.DotProduct(base.UserAgent.GetTargetDirection().AsVec2, base.UserAgent.GetMovementDirection());
		if ((base.UserAgent.Position.AsVec2 - base.UserAgent.GetTargetPosition()).LengthSquared < 0.040000003f)
		{
			return num > 0.99f;
		}
		return false;
	}

	public bool IsRotationCorrectDuringUsage()
	{
		if (!_pointRotation.IsNonZero)
		{
			return false;
		}
		return Vec2.DotProduct(_pointRotation.AsVec2, base.UserAgent.GetMovementDirection()) > 0.99f;
	}

	protected bool CanAgentUseItem(Agent agent)
	{
		if (agent.GetComponent<CampaignAgentComponent>() != null)
		{
			return agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null;
		}
		return false;
	}

	protected void AddItemsToAgent()
	{
		if (!CanAgentUseItem(base.UserAgent) || !IsArriveActionFinished)
		{
			return;
		}
		if (_itemsForBones.Count != 0)
		{
			base.UserAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.HoldAndHideRecentlyUsedMeshes();
		}
		foreach (ItemForBone itemsForBone in _itemsForBones)
		{
			ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(itemsForBone.ItemPrefabName);
			if (@object != null)
			{
				EquipmentIndex equipmentIndex = FindProperSlot(@object);
				if (!base.UserAgent.Equipment[equipmentIndex].IsEmpty)
				{
					base.UserAgent.DropItem(equipmentIndex);
				}
				MissionWeapon weapon = new MissionWeapon(@object, null, base.UserAgent.Origin?.Banner);
				base.UserAgent.EquipWeaponWithNewEntity(equipmentIndex, ref weapon);
				base.UserAgent.TryToWieldWeaponInSlot(equipmentIndex, Agent.WeaponWieldActionType.Instant, isWieldedOnSpawn: false);
			}
			else
			{
				sbyte realBoneIndex = base.UserAgent.AgentVisuals.GetRealBoneIndex(itemsForBone.HumanBone);
				base.UserAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SetPrefabVisibility(realBoneIndex, itemsForBone.ItemPrefabName, isVisible: true);
			}
		}
	}

	public override void OnUserConversationStart()
	{
		_pointRotation = base.UserAgent.Frame.rotation.f;
		_pointRotation.Normalize();
		if (KeepOldVisibility)
		{
			return;
		}
		foreach (ItemForBone itemsForBone in _itemsForBones)
		{
			ItemForBone current = itemsForBone;
			current.OldVisibility = current.IsVisible;
		}
		SetAgentItemsVisibility(isVisible: false);
	}

	public override void OnUserConversationEnd()
	{
		base.UserAgent.ResetLookAgent();
		base.UserAgent.LookDirection = _pointRotation;
		base.UserAgent.SetActionChannel(0, LoopStartActionCode, ignorePriority: false, 0uL);
		foreach (ItemForBone itemsForBone in _itemsForBones)
		{
			if (itemsForBone.OldVisibility)
			{
				SetAgentItemVisibility(itemsForBone, isVisible: true);
			}
		}
	}

	public void SetAgentItemsVisibility(bool isVisible)
	{
		if (base.UserAgent.IsMainAgent)
		{
			return;
		}
		foreach (ItemForBone itemsForBone in _itemsForBones)
		{
			sbyte realBoneIndex = base.UserAgent.AgentVisuals.GetRealBoneIndex(itemsForBone.HumanBone);
			base.UserAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SetPrefabVisibility(realBoneIndex, itemsForBone.ItemPrefabName, isVisible);
			ItemForBone itemForBone = itemsForBone;
			itemForBone.IsVisible = isVisible;
		}
	}

	private void SetAgentItemVisibility(ItemForBone item, bool isVisible)
	{
		sbyte realBoneIndex = base.UserAgent.AgentVisuals.GetRealBoneIndex(item.HumanBone);
		base.UserAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SetPrefabVisibility(realBoneIndex, item.ItemPrefabName, isVisible);
		item.IsVisible = isVisible;
	}

	private EquipmentIndex FindProperSlot(ItemObject item)
	{
		EquipmentIndex result = EquipmentIndex.Weapon3;
		for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex <= EquipmentIndex.Weapon3; equipmentIndex++)
		{
			if (base.UserAgent.Equipment[equipmentIndex].IsEmpty)
			{
				result = equipmentIndex;
			}
			else if (base.UserAgent.Equipment[equipmentIndex].Item == item)
			{
				return equipmentIndex;
			}
		}
		return result;
	}
}
