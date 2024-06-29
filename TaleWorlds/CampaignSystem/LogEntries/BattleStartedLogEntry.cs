using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries;

public class BattleStartedLogEntry : LogEntry, IEncyclopediaLog, IChatNotification
{
	private enum BattleDetail
	{
		AttackerPartyDefendingVillage,
		AttackerPartyRaidingVillage,
		AttackerPartyDefendingFortification,
		AttackerPartyBesiegingFortification,
		ArmyBattle
	}

	[SaveableField(40)]
	private readonly uint _notificationTextColor;

	[SaveableField(41)]
	private readonly Settlement _settlement;

	[SaveableField(42)]
	private readonly TextObject _attackerName;

	[SaveableField(43)]
	private readonly TextObject _defenderName;

	[SaveableField(44)]
	private readonly bool _isVisibleNotification;

	[SaveableField(45)]
	private readonly uint _battleDetail;

	[SaveableField(46)]
	private readonly IFaction _attackerFaction;

	[SaveableField(47)]
	private readonly Hero _attackerLord;

	[SaveableField(48)]
	private readonly bool _attackerPartyHasArmy;

	public bool IsVisibleNotification => _isVisibleNotification;

	public override ChatNotificationType NotificationType => AdversityNotification(null, null);

	internal static void AutoGeneratedStaticCollectObjectsBattleStartedLogEntry(object o, List<object> collectedObjects)
	{
		((BattleStartedLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		collectedObjects.Add(_settlement);
		collectedObjects.Add(_attackerName);
		collectedObjects.Add(_defenderName);
		collectedObjects.Add(_attackerFaction);
		collectedObjects.Add(_attackerLord);
	}

	internal static object AutoGeneratedGetMemberValue_notificationTextColor(object o)
	{
		return ((BattleStartedLogEntry)o)._notificationTextColor;
	}

	internal static object AutoGeneratedGetMemberValue_settlement(object o)
	{
		return ((BattleStartedLogEntry)o)._settlement;
	}

	internal static object AutoGeneratedGetMemberValue_attackerName(object o)
	{
		return ((BattleStartedLogEntry)o)._attackerName;
	}

	internal static object AutoGeneratedGetMemberValue_defenderName(object o)
	{
		return ((BattleStartedLogEntry)o)._defenderName;
	}

	internal static object AutoGeneratedGetMemberValue_isVisibleNotification(object o)
	{
		return ((BattleStartedLogEntry)o)._isVisibleNotification;
	}

	internal static object AutoGeneratedGetMemberValue_battleDetail(object o)
	{
		return ((BattleStartedLogEntry)o)._battleDetail;
	}

	internal static object AutoGeneratedGetMemberValue_attackerFaction(object o)
	{
		return ((BattleStartedLogEntry)o)._attackerFaction;
	}

	internal static object AutoGeneratedGetMemberValue_attackerLord(object o)
	{
		return ((BattleStartedLogEntry)o)._attackerLord;
	}

	internal static object AutoGeneratedGetMemberValue_attackerPartyHasArmy(object o)
	{
		return ((BattleStartedLogEntry)o)._attackerPartyHasArmy;
	}

	public BattleStartedLogEntry(PartyBase attackerParty, PartyBase defenderParty, object subject)
	{
		_notificationTextColor = defenderParty.MapFaction.LabelColor;
		_attackerName = ((attackerParty.IsMobile && attackerParty.MobileParty.Army != null) ? attackerParty.MobileParty.ArmyName : attackerParty.Name);
		_defenderName = ((defenderParty.IsMobile && defenderParty.MobileParty.Army != null) ? defenderParty.MobileParty.ArmyName : defenderParty.Name);
		_attackerFaction = attackerParty.MapFaction;
		_attackerPartyHasArmy = attackerParty.IsMobile && attackerParty.MobileParty.Army != null && attackerParty.MobileParty.LeaderHero != null;
		if (attackerParty.IsMobile && attackerParty.MobileParty.LeaderHero != null)
		{
			_attackerLord = attackerParty.MobileParty.LeaderHero;
		}
		_settlement = subject as Settlement;
		if (_settlement != null)
		{
			_isVisibleNotification = true;
			if (_settlement.IsVillage)
			{
				_battleDetail = (_settlement.MapFaction.IsAtWarWith(attackerParty.MapFaction) ? 1u : 0u);
			}
			else if (_settlement.IsFortification)
			{
				_battleDetail = (_settlement.MapFaction.IsAtWarWith(attackerParty.MapFaction) ? 3u : 2u);
			}
		}
		else if (subject is Army)
		{
			_battleDetail = 4u;
		}
	}

	public override string ToString()
	{
		return GetEncyclopediaText().ToString();
	}

	public TextObject GetNotificationText()
	{
		return GetEncyclopediaText();
	}

	public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
	{
		return obj == _settlement;
	}

	public TextObject GetEncyclopediaText()
	{
		TextObject textObject = TextObject.Empty;
		TextObject variable = ((_settlement != null) ? _settlement.EncyclopediaLinkWithName : TextObject.Empty);
		switch ((BattleDetail)_battleDetail)
		{
		case BattleDetail.AttackerPartyDefendingVillage:
			textObject = GameTexts.FindText("str_army_defend_from_raiding_news");
			textObject.SetTextVariable("PARTY", _attackerName);
			textObject.SetTextVariable("SETTLEMENT", variable);
			break;
		case BattleDetail.AttackerPartyRaidingVillage:
			if (_attackerFaction == null || _attackerLord == null)
			{
				textObject = new TextObject("{=xss7eP0f}{PARTY} is raiding {SETTLEMENT}");
				textObject.SetTextVariable("PARTY", _attackerName);
				textObject.SetTextVariable("SETTLEMENT", variable);
			}
			else
			{
				textObject = ((!_attackerPartyHasArmy) ? GameTexts.FindText("str_party_raiding_news") : GameTexts.FindText("str_army_raiding_news"));
				StringHelpers.SetCharacterProperties("LORD", _attackerLord.CharacterObject, textObject);
				textObject.SetTextVariable("FACTION_NAME", _attackerFaction.EncyclopediaLinkWithName);
				textObject.SetTextVariable("SETTLEMENT", variable);
			}
			break;
		case BattleDetail.AttackerPartyDefendingFortification:
			textObject = GameTexts.FindText("str_defend_from_assault_news");
			textObject.SetTextVariable("PARTY", _attackerName);
			textObject.SetTextVariable("SETTLEMENT", variable);
			break;
		case BattleDetail.AttackerPartyBesiegingFortification:
			textObject = GameTexts.FindText("str_assault_news");
			textObject.SetTextVariable("PARTY", _attackerName);
			textObject.SetTextVariable("SETTLEMENT", variable);
			break;
		case BattleDetail.ArmyBattle:
			textObject = GameTexts.FindText("str_army_battle_news");
			textObject.SetTextVariable("PARTY1", _attackerName);
			textObject.SetTextVariable("PARTY2", _defenderName);
			break;
		}
		return textObject;
	}
}
