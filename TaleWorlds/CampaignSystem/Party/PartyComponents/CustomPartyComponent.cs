using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Party.PartyComponents;

public class CustomPartyComponent : PartyComponent
{
	private const string StringId = "quest_party_template_1";

	[SaveableField(10)]
	private TextObject _name;

	[SaveableField(20)]
	private Settlement _homeSettlement;

	[SaveableField(30)]
	private Hero _owner;

	[SaveableField(40)]
	private float _customPartyBaseSpeed;

	[SaveableField(50)]
	private string _partyMountStringId;

	[SaveableField(60)]
	private string _partyHarnessStringId;

	[SaveableField(70)]
	public bool _avoidHostileActions;

	public float CustomPartyBaseSpeed
	{
		get
		{
			return _customPartyBaseSpeed;
		}
		set
		{
			_customPartyBaseSpeed = value;
		}
	}

	public override bool AvoidHostileActions => _avoidHostileActions;

	public float BaseSpeed => _customPartyBaseSpeed;

	public override Hero PartyOwner => _owner;

	public override TextObject Name => _name;

	public override Settlement HomeSettlement => _homeSettlement;

	internal static void AutoGeneratedStaticCollectObjectsCustomPartyComponent(object o, List<object> collectedObjects)
	{
		((CustomPartyComponent)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		collectedObjects.Add(_name);
		collectedObjects.Add(_homeSettlement);
		collectedObjects.Add(_owner);
	}

	internal static object AutoGeneratedGetMemberValue_avoidHostileActions(object o)
	{
		return ((CustomPartyComponent)o)._avoidHostileActions;
	}

	internal static object AutoGeneratedGetMemberValue_name(object o)
	{
		return ((CustomPartyComponent)o)._name;
	}

	internal static object AutoGeneratedGetMemberValue_homeSettlement(object o)
	{
		return ((CustomPartyComponent)o)._homeSettlement;
	}

	internal static object AutoGeneratedGetMemberValue_owner(object o)
	{
		return ((CustomPartyComponent)o)._owner;
	}

	internal static object AutoGeneratedGetMemberValue_customPartyBaseSpeed(object o)
	{
		return ((CustomPartyComponent)o)._customPartyBaseSpeed;
	}

	internal static object AutoGeneratedGetMemberValue_partyMountStringId(object o)
	{
		return ((CustomPartyComponent)o)._partyMountStringId;
	}

	internal static object AutoGeneratedGetMemberValue_partyHarnessStringId(object o)
	{
		return ((CustomPartyComponent)o)._partyHarnessStringId;
	}

	public static MobileParty CreateQuestParty(Vec2 position, float spawnRadius, Settlement homeSettlement, TextObject name, Clan clan, PartyTemplateObject partyTemplate, Hero owner, int troopLimit = 0, string partyMountStringId = "", string partyHarnessStringId = "", float customPartyBaseSpeed = 0f, bool avoidHostileActions = false)
	{
		return MobileParty.CreateParty("quest_party_template_1", new CustomPartyComponent(), delegate(MobileParty mobileParty)
		{
			(mobileParty.PartyComponent as CustomPartyComponent).InitializeQuestPartyProperties(mobileParty, position, spawnRadius, homeSettlement, name, partyTemplate, owner, troopLimit, partyMountStringId, partyHarnessStringId, customPartyBaseSpeed, avoidHostileActions);
			mobileParty.ActualClan = clan;
		});
	}

	public static MobileParty CreateQuestParty(Vec2 position, float spawnRadius, Settlement homeSettlement, TextObject name, Clan clan, TroopRoster troopRoster, TroopRoster prisonerRoster, Hero owner, string partyMountStringId = "", string partyHarnessStringId = "", float customPartyBaseSpeed = 0f, bool avoidHostileActions = false)
	{
		return MobileParty.CreateParty("quest_party_template_1", new CustomPartyComponent(), delegate(MobileParty mobileParty)
		{
			(mobileParty.PartyComponent as CustomPartyComponent).InitializeQuestPartyProperties(mobileParty, position, spawnRadius, homeSettlement, name, troopRoster, prisonerRoster, owner, partyMountStringId, partyHarnessStringId, customPartyBaseSpeed, avoidHostileActions);
			mobileParty.ActualClan = clan;
		});
	}

	private void InitializeQuestPartyProperties(MobileParty mobileParty, Vec2 position, float spawnRadius, Settlement homeSettlement, TextObject name, PartyTemplateObject partyTemplate, Hero owner, int troopLimit, string partyMountStringId, string partyHarnessStringId, float customPartyBaseSpeed, bool avoidHostileActions)
	{
		_name = name;
		_homeSettlement = homeSettlement;
		_owner = owner;
		mobileParty.InitializeMobilePartyAroundPosition(partyTemplate, position, spawnRadius, 0f, troopLimit);
		mobileParty.Party.SetVisualAsDirty();
		_partyMountStringId = partyMountStringId;
		_partyHarnessStringId = partyHarnessStringId;
		SetBaseSpeed(customPartyBaseSpeed);
		_avoidHostileActions = avoidHostileActions;
	}

	private void InitializeQuestPartyProperties(MobileParty mobileParty, Vec2 position, float spawnRadius, Settlement homeSettlement, TextObject name, TroopRoster troopRoster, TroopRoster prisonerRoster, Hero owner, string partyMountStringId, string partyHarnessStringId, float customPartyBaseSpeed, bool avoidHostileActions)
	{
		_name = name;
		_homeSettlement = homeSettlement;
		_owner = owner;
		mobileParty.InitializeMobilePartyAroundPosition(troopRoster, prisonerRoster, position, spawnRadius);
		mobileParty.Party.SetVisualAsDirty();
		_partyMountStringId = partyMountStringId;
		_partyHarnessStringId = partyHarnessStringId;
		SetBaseSpeed(customPartyBaseSpeed);
		_avoidHostileActions = avoidHostileActions;
	}

	public void SetBaseSpeed(float speed)
	{
		_customPartyBaseSpeed = speed;
		base.MobileParty?.UpdateVersionNo();
	}

	public override void GetMountAndHarnessVisualIdsForPartyIcon(PartyBase party, out string mountStringId, out string harnessStringId)
	{
		mountStringId = _partyMountStringId;
		harnessStringId = _partyHarnessStringId;
	}
}
