using System;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace SandBox.ViewModelCollection.Nameplate;

public class PartyNameplateVM : NameplateVM
{
	public static string PositiveIndicator = Color.FromUint(4285650500u).ToString();

	public static string PositiveArmyIndicator = Color.FromUint(4288804731u).ToString();

	public static string NegativeIndicator = Color.FromUint(4292232774u).ToString();

	public static string NegativeArmyIndicator = Color.FromUint(4294931829u).ToString();

	public static string NeutralIndicator = Color.FromUint(4291877096u).ToString();

	public static string NeutralArmyIndicator = Color.FromUint(4294573055u).ToString();

	public static string MainPartyIndicator = Color.FromUint(4287421380u).ToString();

	public static string MainPartyArmyIndicator = Color.FromUint(4289593317u).ToString();

	private float _latestX;

	private float _latestY;

	private float _latestW;

	private float _cachedSpeed;

	private readonly Camera _mapCamera;

	private readonly Action _resetCamera;

	private readonly Func<bool> _getShouldShowFullName;

	private int _latestPrisonerAmount = -1;

	private int _latestWoundedAmount = -1;

	private int _latestTotalCount = -1;

	private bool _isPartyBannerDirty;

	private bool _isPartyHeroVisualDirty;

	private float _latestMainHeroAge = -1f;

	private TextObject _latestNameTextObject;

	private SandBoxUIHelper.IssueQuestFlags _questsBind;

	private Vec2 _partyPositionBind;

	private Vec2 _headPositionBind;

	private ImageIdentifierVM _mainHeroVisualBind;

	private bool _isMainPartyBind;

	private bool _isHighBind;

	private bool _isBehindBind;

	private bool _isInArmyBind;

	private bool _isInSettlementBind;

	private bool _isVisibleOnMapBind;

	private bool _shouldShowFullNameBind;

	private bool _isArmyBind;

	private bool _isPrisonerBind;

	private bool _isDisorganizedBind;

	private string _factionColorBind;

	private string _countBind;

	private string _woundedBind;

	private string _prisonerBind;

	private string _extraInfoTextBind;

	private string _fullNameBind;

	private string _movementSpeedTextBind;

	private string _count;

	private string _wounded;

	private string _prisoner;

	private MBBindingList<QuestMarkerVM> _quests;

	private string _fullName;

	private string _extraInfoText;

	private string _movementSpeedText;

	private bool _isMainParty;

	private bool _isBehind;

	private bool _isHigh;

	private bool _shouldShowFullName;

	private bool _isInArmy;

	private bool _isArmy;

	private bool _isPrisoner;

	private bool _isInSettlement;

	private bool _isDisorganized;

	private ImageIdentifierVM _mainHeroVisual;

	private ImageIdentifierVM _partyBanner;

	private Vec2 _headPosition;

	public bool GetIsMainParty
	{
		get
		{
			if (!_isMainPartyBind)
			{
				return IsMainParty;
			}
			return true;
		}
	}

	public MobileParty Party { get; }

	private IFaction _mainFaction => Hero.MainHero.MapFaction;

	public Vec2 HeadPosition
	{
		get
		{
			return _headPosition;
		}
		set
		{
			if (value != _headPosition)
			{
				_headPosition = value;
				OnPropertyChangedWithValue(value, "HeadPosition");
			}
		}
	}

	public string Count
	{
		get
		{
			return _count;
		}
		set
		{
			if (value != _count)
			{
				_count = value;
				OnPropertyChangedWithValue(value, "Count");
			}
		}
	}

	public string Prisoner
	{
		get
		{
			return _prisoner;
		}
		set
		{
			if (value != _prisoner)
			{
				_prisoner = value;
				OnPropertyChangedWithValue(value, "Prisoner");
			}
		}
	}

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

	public string Wounded
	{
		get
		{
			return _wounded;
		}
		set
		{
			if (value != _wounded)
			{
				_wounded = value;
				OnPropertyChangedWithValue(value, "Wounded");
			}
		}
	}

	public string ExtraInfoText
	{
		get
		{
			return _extraInfoText;
		}
		set
		{
			if (value != _extraInfoText)
			{
				_extraInfoText = value;
				OnPropertyChangedWithValue(value, "ExtraInfoText");
			}
		}
	}

	public string MovementSpeedText
	{
		get
		{
			return _movementSpeedText;
		}
		set
		{
			if (value != _movementSpeedText)
			{
				_movementSpeedText = value;
				OnPropertyChangedWithValue(value, "MovementSpeedText");
			}
		}
	}

	public string FullName
	{
		get
		{
			return _fullName;
		}
		set
		{
			if (value != _fullName)
			{
				_fullName = value;
				OnPropertyChangedWithValue(value, "FullName");
			}
		}
	}

	public bool IsMainParty
	{
		get
		{
			return _isMainParty;
		}
		set
		{
			if (value != _isMainParty)
			{
				_isMainParty = value;
				OnPropertyChangedWithValue(value, "IsMainParty");
			}
		}
	}

	public bool IsInArmy
	{
		get
		{
			return _isInArmy;
		}
		set
		{
			if (value != _isInArmy)
			{
				_isInArmy = value;
				OnPropertyChangedWithValue(value, "IsInArmy");
			}
		}
	}

	public bool IsInSettlement
	{
		get
		{
			return _isInSettlement;
		}
		set
		{
			if (value != _isInSettlement)
			{
				_isInSettlement = value;
				OnPropertyChangedWithValue(value, "IsInSettlement");
			}
		}
	}

	public bool IsDisorganized
	{
		get
		{
			return _isDisorganized;
		}
		set
		{
			if (value != _isDisorganized)
			{
				_isDisorganized = value;
				OnPropertyChangedWithValue(value, "IsDisorganized");
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

	public bool IsHigh
	{
		get
		{
			return _isHigh;
		}
		set
		{
			if (value != _isHigh)
			{
				_isHigh = value;
				OnPropertyChangedWithValue(value, "IsHigh");
			}
		}
	}

	public bool IsPrisoner
	{
		get
		{
			return _isPrisoner;
		}
		set
		{
			if (value != _isPrisoner)
			{
				_isPrisoner = value;
				OnPropertyChangedWithValue(value, "IsPrisoner");
			}
		}
	}

	public bool ShouldShowFullName
	{
		get
		{
			if (!_shouldShowFullName)
			{
				return _bindIsTargetedByTutorial;
			}
			return true;
		}
		set
		{
			if (value != _shouldShowFullName)
			{
				_shouldShowFullName = value;
				OnPropertyChangedWithValue(value, "ShouldShowFullName");
			}
		}
	}

	public ImageIdentifierVM MainHeroVisual
	{
		get
		{
			return _mainHeroVisual;
		}
		set
		{
			if (value != _mainHeroVisual)
			{
				_mainHeroVisual = value;
				OnPropertyChangedWithValue(value, "MainHeroVisual");
			}
		}
	}

	public ImageIdentifierVM PartyBanner
	{
		get
		{
			return _partyBanner;
		}
		set
		{
			if (value != _partyBanner)
			{
				_partyBanner = value;
				OnPropertyChangedWithValue(value, "PartyBanner");
			}
		}
	}

	public PartyNameplateVM(MobileParty party, Camera mapCamera, Action resetCamera, Func<bool> getShouldShowFullName)
	{
		_resetCamera = resetCamera;
		_mapCamera = mapCamera;
		_getShouldShowFullName = getShouldShowFullName;
		Party = party;
		_isMainPartyBind = party.IsMainParty;
		MainHeroVisual = (_isMainPartyBind ? new ImageIdentifierVM(SandBoxUIHelper.GetCharacterCode(Hero.MainHero.CharacterObject)) : null);
		_isPartyHeroVisualDirty = true;
		_isPartyBannerDirty = true;
		Quests = new MBBindingList<QuestMarkerVM>();
		RegisterEvents();
	}

	public override void OnFinalize()
	{
		UnregisterEvents();
		base.OnFinalize();
	}

	public override void RefreshValues()
	{
		base.RefreshValues();
		RefreshDynamicProperties(forceUpdate: true);
	}

	private void AddQuestBindFlagsForParty(MobileParty party)
	{
		if (party == MobileParty.MainParty || party == Party)
		{
			return;
		}
		if (party.LeaderHero?.Issue != null && (_questsBind & SandBoxUIHelper.IssueQuestFlags.TrackedIssue) == 0 && ((_questsBind & SandBoxUIHelper.IssueQuestFlags.AvailableIssue) == 0 || (_questsBind & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) == 0))
		{
			_questsBind |= SandBoxUIHelper.GetIssueType(party.LeaderHero.Issue);
		}
		if (((_questsBind & SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest) != 0 || (_questsBind & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) != 0) && (_questsBind & SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest) != 0)
		{
			return;
		}
		foreach (QuestBase item in SandBoxUIHelper.GetQuestsRelatedToParty(party))
		{
			if (party.LeaderHero != null && item.QuestGiver == party.LeaderHero)
			{
				if (item.IsSpecialQuest && (_questsBind & SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest) == 0)
				{
					_questsBind |= SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest;
				}
				else if (!item.IsSpecialQuest && (_questsBind & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) == 0)
				{
					_questsBind |= SandBoxUIHelper.IssueQuestFlags.ActiveIssue;
				}
			}
			else if (item.IsSpecialQuest && (_questsBind & SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest) == 0)
			{
				_questsBind |= SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest;
			}
			else if (!item.IsSpecialQuest && (_questsBind & SandBoxUIHelper.IssueQuestFlags.TrackedIssue) == 0)
			{
				_questsBind |= SandBoxUIHelper.IssueQuestFlags.TrackedIssue;
			}
		}
	}

	public override void RefreshDynamicProperties(bool forceUpdate)
	{
		base.RefreshDynamicProperties(forceUpdate);
		if ((IsMainParty && TaleWorlds.Library.MathF.Abs(Hero.MainHero.Age - _latestMainHeroAge) >= 1f) || forceUpdate)
		{
			_latestMainHeroAge = Hero.MainHero.Age;
			_isPartyHeroVisualDirty = true;
		}
		if (_isPartyHeroVisualDirty || forceUpdate)
		{
			_mainHeroVisualBind = (_isMainPartyBind ? new ImageIdentifierVM(SandBoxUIHelper.GetCharacterCode(Hero.MainHero.CharacterObject)) : null);
			_isPartyHeroVisualDirty = false;
		}
		if (_isVisibleOnMapBind || forceUpdate)
		{
			IssueBase issueBase = Party?.LeaderHero?.Issue;
			_questsBind = SandBoxUIHelper.IssueQuestFlags.None;
			if (Party != MobileParty.MainParty)
			{
				if (issueBase != null)
				{
					_questsBind |= SandBoxUIHelper.GetIssueType(issueBase);
				}
				foreach (QuestBase item in SandBoxUIHelper.GetQuestsRelatedToParty(Party))
				{
					if (item.QuestGiver != null && item.QuestGiver == Party.LeaderHero)
					{
						_questsBind |= (SandBoxUIHelper.IssueQuestFlags)(item.IsSpecialQuest ? 4 : 2);
					}
					else
					{
						_questsBind |= (SandBoxUIHelper.IssueQuestFlags)(item.IsSpecialQuest ? 16 : 8);
					}
				}
			}
		}
		_isInArmyBind = Party.Army != null && Party.AttachedTo != null;
		_isArmyBind = Party.Army != null && Party.Army.LeaderParty == Party;
		_isInSettlementBind = Party?.CurrentSettlement != null;
		if (_isArmyBind && (_isVisibleOnMapBind || forceUpdate))
		{
			AddQuestBindFlagsForParty(Party.Army.LeaderParty);
			foreach (MobileParty attachedParty in Party.Army.LeaderParty.AttachedParties)
			{
				AddQuestBindFlagsForParty(attachedParty);
			}
		}
		if (_isArmyBind || !_isInArmy || forceUpdate)
		{
			int partyHealthyCount = SandBoxUIHelper.GetPartyHealthyCount(Party);
			if (partyHealthyCount != _latestTotalCount)
			{
				_latestTotalCount = partyHealthyCount;
				_countBind = partyHealthyCount.ToString();
			}
			int allWoundedMembersAmount = SandBoxUIHelper.GetAllWoundedMembersAmount(Party);
			int allPrisonerMembersAmount = SandBoxUIHelper.GetAllPrisonerMembersAmount(Party);
			if (_latestWoundedAmount != allWoundedMembersAmount || _latestPrisonerAmount != allPrisonerMembersAmount)
			{
				if (_latestWoundedAmount != allWoundedMembersAmount)
				{
					_woundedBind = ((allWoundedMembersAmount == 0) ? "" : SandBoxUIHelper.GetPartyWoundedText(allWoundedMembersAmount));
					_latestWoundedAmount = allWoundedMembersAmount;
				}
				if (_latestPrisonerAmount != allPrisonerMembersAmount)
				{
					_prisonerBind = ((allPrisonerMembersAmount == 0) ? "" : SandBoxUIHelper.GetPartyPrisonerText(allPrisonerMembersAmount));
					_latestPrisonerAmount = allPrisonerMembersAmount;
				}
				_extraInfoTextBind = _woundedBind + _prisonerBind;
			}
		}
		if (!Party.IsMainParty)
		{
			Army army = Party.Army;
			if (army == null || !army.LeaderParty.AttachedParties.Contains(MobileParty.MainParty) || !Party.Army.LeaderParty.AttachedParties.Contains(Party))
			{
				if (FactionManager.IsAtWarAgainstFaction(Party.MapFaction, _mainFaction))
				{
					_factionColorBind = ((Party.Army != null && Party.Army.LeaderParty == Party) ? NegativeArmyIndicator : NegativeIndicator);
				}
				else if (FactionManager.IsAlliedWithFaction(Party.MapFaction, Hero.MainHero.MapFaction))
				{
					_factionColorBind = ((Party.Army != null && Party.Army.LeaderParty == Party) ? PositiveArmyIndicator : PositiveIndicator);
				}
				else
				{
					_factionColorBind = ((Party.Army != null && Party.Army.LeaderParty == Party) ? NeutralArmyIndicator : NeutralIndicator);
				}
				goto IL_0491;
			}
		}
		_factionColorBind = ((Party.Army != null && Party.Army.LeaderParty == Party) ? MainPartyArmyIndicator : MainPartyIndicator);
		goto IL_0491;
		IL_0491:
		if (_isPartyBannerDirty || forceUpdate)
		{
			if (Party.Party.Banner != null)
			{
				PartyBanner = new ImageIdentifierVM(BannerCode.CreateFrom(Party.Party.Banner), nineGrid: true);
			}
			else
			{
				PartyBanner = new ImageIdentifierVM(BannerCode.CreateFrom(Party.MapFaction?.Banner), nineGrid: true);
			}
			_isPartyBannerDirty = false;
		}
		if (_isVisibleOnMapBind && (_isInArmyBind || _isInSettlementBind))
		{
			_isVisibleOnMapBind = false;
		}
		Army army2 = Party.Army;
		TextObject textObject = ((army2 != null && army2.DoesLeaderPartyAndAttachedPartiesContain(Party)) ? Party.ArmyName : ((Party.LeaderHero == null) ? Party.Name : Party.LeaderHero.Name));
		_isPrisonerBind = IsMainParty && Party.LeaderHero == null && (Hero.MainHero?.IsAlive ?? false);
		_isDisorganizedBind = Party.IsDisorganized;
		_shouldShowFullNameBind = _getShouldShowFullName != null && _getShouldShowFullName();
		if (_latestNameTextObject != textObject || forceUpdate)
		{
			_latestNameTextObject = textObject;
			_fullNameBind = _latestNameTextObject.ToString();
		}
		if (!_cachedSpeed.ApproximatelyEqualsTo(Party.Speed, 0.01f))
		{
			_cachedSpeed = Party.Speed;
			_movementSpeedTextBind = _cachedSpeed.ToString("F1");
		}
	}

	public override void RefreshPosition()
	{
		base.RefreshPosition();
		float height = 0f;
		Campaign.Current.MapSceneWrapper.GetHeightAtPoint(Party.VisualPosition2DWithoutError, ref height);
		Vec3 vec = Party.VisualPosition2DWithoutError.ToVec3(height);
		Vec3 worldSpacePosition = vec + new Vec3(0f, 0f, 0.8f);
		if (_isMainPartyBind)
		{
			RefreshMainPartyScreenPosition(vec);
			MBWindowManager.WorldToScreenInsideUsableArea(_mapCamera, worldSpacePosition, ref _latestX, ref _latestY, ref _latestW);
			_headPositionBind = new Vec2(_latestX, _latestY);
		}
		else
		{
			_latestX = 0f;
			_latestY = 0f;
			_latestW = 0f;
			MBWindowManager.WorldToScreenInsideUsableArea(_mapCamera, vec, ref _latestX, ref _latestY, ref _latestW);
			_partyPositionBind = new Vec2(_latestX, _latestY);
			MBWindowManager.WorldToScreenInsideUsableArea(_mapCamera, worldSpacePosition, ref _latestX, ref _latestY, ref _latestW);
			_headPositionBind = new Vec2(_latestX, _latestY);
		}
		base.DistanceToCamera = vec.Distance(_mapCamera.Position);
	}

	public override void RefreshTutorialStatus(string newTutorialHighlightElementID)
	{
		base.RefreshTutorialStatus(newTutorialHighlightElementID);
		if (Party?.Party?.Id == null)
		{
			Debug.FailedAssert("Mobile party id is null when refreshing tutorial status", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\Nameplate\\PartyNameplateVM.cs", "RefreshTutorialStatus", 344);
		}
		else
		{
			_bindIsTargetedByTutorial = ((Party.Party.Id == newTutorialHighlightElementID) ? true : false);
		}
	}

	private void RefreshMainPartyScreenPosition(Vec3 partyWorldPosition)
	{
		_latestX = 0f;
		_latestY = 0f;
		_latestW = 0f;
		MatrixFrame viewProj = MatrixFrame.Identity;
		_mapCamera.GetViewProjMatrix(ref viewProj);
		Vec3 vec = partyWorldPosition;
		vec.w = 1f;
		Vec3 vec2 = vec * viewProj;
		_isBehindBind = vec2.w < 0f;
		vec2.w = TaleWorlds.Library.MathF.Abs(vec2.w);
		vec2.x /= vec2.w;
		vec2.y /= vec2.w;
		vec2.z /= vec2.w;
		vec2.w /= vec2.w;
		vec2 *= 0.5f;
		vec2.x += 0.5f;
		vec2.y += 0.5f;
		vec2.y = 1f - vec2.y;
		if (_isBehindBind)
		{
			vec2.y = 1f;
		}
		int num = (int)(Screen.RealScreenResolutionWidth * ScreenManager.UsableArea.X);
		int num2 = (int)(Screen.RealScreenResolutionHeight * ScreenManager.UsableArea.Y);
		_latestX = vec2.x * (float)num;
		_latestY = vec2.y * (float)num2;
		_latestW = (IsBehind ? (0f - vec2.w) : vec2.w);
		_isHighBind = _mapCamera.Position.Distance(vec) >= 110f;
		_partyPositionBind = new Vec2(_latestX, _latestY);
	}

	public void DetermineIsVisibleOnMap()
	{
		_isVisibleOnMapBind = _latestW < 100f && _latestW > 0f && _mapCamera.Position.z < 200f;
	}

	public void OnPlayerCharacterChanged(Hero newPlayer)
	{
		if (IsMainParty && Party.LeaderHero != newPlayer)
		{
			_isMainPartyBind = false;
			_mainHeroVisualBind = new ImageIdentifierVM();
		}
		else if (Party.LeaderHero == newPlayer)
		{
			_isMainPartyBind = true;
			_mainHeroVisualBind = new ImageIdentifierVM(CharacterCode.CreateFrom(Hero.MainHero.CharacterObject));
		}
		_isPartyHeroVisualDirty = true;
	}

	private bool IsInsideWindow()
	{
		if (!(_latestX > Screen.RealScreenResolutionWidth) && !(_latestY > Screen.RealScreenResolutionHeight) && !(_latestX + 100f < 0f))
		{
			return !(_latestY + 30f < 0f);
		}
		return false;
	}

	public void RefreshBinding()
	{
		base.Position = _partyPositionBind;
		HeadPosition = _headPositionBind;
		base.IsVisibleOnMap = _isVisibleOnMapBind;
		IsInSettlement = _isInSettlementBind;
		IsMainParty = _isMainPartyBind;
		base.FactionColor = _factionColorBind;
		MainHeroVisual = _mainHeroVisualBind;
		IsHigh = _isHighBind;
		Count = _countBind;
		Prisoner = _prisonerBind;
		Wounded = _woundedBind;
		IsBehind = _isBehindBind;
		FullName = _fullNameBind;
		base.IsTargetedByTutorial = _bindIsTargetedByTutorial;
		ShouldShowFullName = _shouldShowFullNameBind;
		IsInArmy = _isInArmyBind;
		IsArmy = _isArmyBind;
		ExtraInfoText = _extraInfoTextBind;
		IsPrisoner = _isPrisonerBind;
		IsDisorganized = _isDisorganizedBind;
		MovementSpeedText = _movementSpeedTextBind;
		Quests.Clear();
		SandBoxUIHelper.IssueQuestFlags[] issueQuestFlagsValues = SandBoxUIHelper.IssueQuestFlagsValues;
		foreach (SandBoxUIHelper.IssueQuestFlags issueQuestFlags in issueQuestFlagsValues)
		{
			if (issueQuestFlags != 0 && (_questsBind & issueQuestFlags) != 0)
			{
				Quests.Add(new QuestMarkerVM(issueQuestFlags));
			}
		}
	}

	public void ExecuteSetCameraPosition()
	{
		if (IsMainParty)
		{
			_resetCamera();
		}
	}

	private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
	{
		bool flag = Party.HomeSettlement != null && (Party.HomeSettlement.IsVillage ? settlement.BoundVillages.Contains(Party.HomeSettlement.Village) : (Party.HomeSettlement == settlement));
		if ((Party.IsCaravan || Party.IsVillager) && flag)
		{
			_isPartyBannerDirty = true;
		}
	}

	private void OnClanChangeKingdom(Clan arg1, Kingdom arg2, Kingdom arg3, ChangeKingdomAction.ChangeKingdomActionDetail arg4, bool showNotification)
	{
		if (Party.LeaderHero?.Clan == arg1)
		{
			_isPartyBannerDirty = true;
		}
	}

	private void OnClanLeaderChanged(Hero arg1, Hero arg2)
	{
		if (arg2.MapFaction == Party.MapFaction)
		{
			_isPartyBannerDirty = true;
		}
	}

	public void RegisterEvents()
	{
		CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, OnSettlementOwnerChanged);
		CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, OnClanChangeKingdom);
		CampaignEvents.OnClanLeaderChangedEvent.AddNonSerializedListener(this, OnClanLeaderChanged);
	}

	public void UnregisterEvents()
	{
		CampaignEvents.OnSettlementOwnerChangedEvent.ClearListeners(this);
		CampaignEvents.OnClanChangedKingdomEvent.ClearListeners(this);
		CampaignEvents.OnClanLeaderChangedEvent.ClearListeners(this);
	}
}
