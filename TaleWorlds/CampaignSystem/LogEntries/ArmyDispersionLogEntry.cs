using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries;

public class ArmyDispersionLogEntry : LogEntry, IEncyclopediaLog, IChatNotification, IWarLog
{
	[SaveableField(30)]
	public readonly Army.ArmyDispersionReason DispersionReason;

	[SaveableField(31)]
	private readonly uint _notificationTextColor;

	[SaveableField(32)]
	private readonly bool _isVisibleNotification;

	[SaveableField(33)]
	private readonly CharacterObject _armyLeader;

	[SaveableField(34)]
	private readonly TextObject _encyclopediaLinkWithName;

	public override CampaignTime KeepInHistoryTime => CampaignTime.Weeks(1f);

	public override ChatNotificationType NotificationType => DiplomaticNotification(_armyLeader.HeroObject?.Clan ?? null, null);

	public bool IsVisibleNotification => _isVisibleNotification;

	internal static void AutoGeneratedStaticCollectObjectsArmyDispersionLogEntry(object o, List<object> collectedObjects)
	{
		((ArmyDispersionLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		collectedObjects.Add(_armyLeader);
		collectedObjects.Add(_encyclopediaLinkWithName);
	}

	internal static object AutoGeneratedGetMemberValueDispersionReason(object o)
	{
		return ((ArmyDispersionLogEntry)o).DispersionReason;
	}

	internal static object AutoGeneratedGetMemberValue_notificationTextColor(object o)
	{
		return ((ArmyDispersionLogEntry)o)._notificationTextColor;
	}

	internal static object AutoGeneratedGetMemberValue_isVisibleNotification(object o)
	{
		return ((ArmyDispersionLogEntry)o)._isVisibleNotification;
	}

	internal static object AutoGeneratedGetMemberValue_armyLeader(object o)
	{
		return ((ArmyDispersionLogEntry)o)._armyLeader;
	}

	internal static object AutoGeneratedGetMemberValue_encyclopediaLinkWithName(object o)
	{
		return ((ArmyDispersionLogEntry)o)._encyclopediaLinkWithName;
	}

	public ArmyDispersionLogEntry(Army army, Army.ArmyDispersionReason reason)
	{
		_armyLeader = army.ArmyOwner.CharacterObject;
		_encyclopediaLinkWithName = army.EncyclopediaLinkWithName;
		_notificationTextColor = army.Kingdom?.LabelColor ?? 0;
		DispersionReason = reason;
		_isVisibleNotification = army.LeaderParty.MapFaction == Hero.MainHero.MapFaction && army.Parties.IndexOf(MobileParty.MainParty) >= 0;
	}

	public override string ToString()
	{
		return GetEncyclopediaText().ToString();
	}

	public bool IsRelatedToWar(StanceLink stance, out IFaction effector, out IFaction effected)
	{
		IFaction faction = stance.Faction1;
		IFaction faction2 = stance.Faction2;
		effector = _armyLeader.HeroObject.MapFaction;
		effected = null;
		if (_armyLeader.HeroObject.MapFaction != faction)
		{
			return _armyLeader.HeroObject.MapFaction == faction2;
		}
		return true;
	}

	public TextObject GetNotificationText()
	{
		return GetEncyclopediaText();
	}

	public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
	{
		if (_armyLeader != null)
		{
			return obj == _armyLeader.HeroObject;
		}
		return false;
	}

	public TextObject GetEncyclopediaText()
	{
		TextObject textObject = DispersionReason switch
		{
			Army.ArmyDispersionReason.CohesionDepleted => new TextObject("{=ESM0NhLy}{ARMY} has been disbanded since cohesion has been depleted."), 
			Army.ArmyDispersionReason.KingdomChanged => new TextObject("{=6XAvdeEE}{ARMY} has been disbanded since kingdom has been changed."), 
			Army.ArmyDispersionReason.DismissalRequestedWithInfluence => new TextObject("{=bAVZloaC}{ARMY} has been disbanded since dismissal is requested."), 
			Army.ArmyDispersionReason.LeaderPartyRemoved => new TextObject("{=A3HdRbqf}{ARMY} has been disbanded since the leader party has left the army."), 
			Army.ArmyDispersionReason.NotEnoughParty => new TextObject("{=s2lKb22C}{ARMY} has been disbanded since other parties has left the army."), 
			Army.ArmyDispersionReason.ObjectiveFinished => new TextObject("{=6XRt8101}{ARMY} has been disbanded since the objective is finished"), 
			Army.ArmyDispersionReason.PlayerTakenPrisoner => new TextObject("{=loEI5awS}{ARMY} has been disbanded since the player has taken as a prisoner"), 
			Army.ArmyDispersionReason.CannotElectNewLeader => new TextObject("{=7DCzgGNP}{ARMY} has been disbanded since a new leader cannot be selected"), 
			Army.ArmyDispersionReason.LeaderCannotArrivePointOnTime => new TextObject("{=t5Y0gTGv}{ARMY} has been disbanded since the leader couldn't arrived to the point on time."), 
			Army.ArmyDispersionReason.ArmyLeaderIsDead => new TextObject("{=t6FNeiOC}{ARMY} has been disbanded since the army leader is dead."), 
			Army.ArmyDispersionReason.Unknown => new TextObject("{=5CJOMH90}{ARMY} has been disbanded."), 
			_ => new TextObject("{=5CJOMH90}{ARMY} has been disbanded."), 
		};
		TextObject textObject2 = new TextObject("{=nbmctMLk}{LEADER_NAME}{.o} Army");
		textObject2.SetTextVariable("LEADER_NAME", _armyLeader.Name);
		textObject.SetTextVariable("ARMY", textObject2);
		return textObject;
	}
}