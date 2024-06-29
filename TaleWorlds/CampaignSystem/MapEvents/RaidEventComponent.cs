using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapEvents;

public class RaidEventComponent : MapEventComponent
{
	[SaveableField(10)]
	private float _nextSettlementDamage;

	[SaveableField(30)]
	private int _lootedItemCount;

	[SaveableField(40)]
	private Dictionary<ItemObject, float> _raidProductionRewards = new Dictionary<ItemObject, float>();

	[SaveableProperty(20)]
	public float RaidDamage { get; private set; }

	public MapEventSide DefenderSide => base.MapEvent.DefenderSide;

	public MapEventSide AttackerSide => base.MapEvent.AttackerSide;

	public Settlement MapEventSettlement => base.MapEvent.MapEventSettlement;

	public bool IsPlayerMapEvent => base.MapEvent.IsPlayerMapEvent;

	public BattleState BattleState => base.MapEvent.BattleState;

	internal static void AutoGeneratedStaticCollectObjectsRaidEventComponent(object o, List<object> collectedObjects)
	{
		((RaidEventComponent)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		collectedObjects.Add(_raidProductionRewards);
	}

	internal static object AutoGeneratedGetMemberValueRaidDamage(object o)
	{
		return ((RaidEventComponent)o).RaidDamage;
	}

	internal static object AutoGeneratedGetMemberValue_nextSettlementDamage(object o)
	{
		return ((RaidEventComponent)o)._nextSettlementDamage;
	}

	internal static object AutoGeneratedGetMemberValue_lootedItemCount(object o)
	{
		return ((RaidEventComponent)o)._lootedItemCount;
	}

	internal static object AutoGeneratedGetMemberValue_raidProductionRewards(object o)
	{
		return ((RaidEventComponent)o)._raidProductionRewards;
	}

	protected RaidEventComponent(MapEvent mapEvent)
		: base(mapEvent)
	{
	}

	public static RaidEventComponent CreateRaidEvent(PartyBase attackerParty, PartyBase defenderParty)
	{
		MapEvent mapEvent = new MapEvent();
		RaidEventComponent raidEventComponent = new RaidEventComponent(mapEvent);
		mapEvent.Initialize(attackerParty, defenderParty, raidEventComponent, MapEvent.BattleTypes.Raid);
		if (defenderParty.Settlement?.MilitiaPartyComponent != null)
		{
			defenderParty.Settlement.MilitiaPartyComponent.MobileParty.MapEventSide = mapEvent.DefenderSide;
		}
		Campaign.Current.MapEventManager.OnMapEventCreated(mapEvent);
		return raidEventComponent;
	}

	public static RaidEventComponent CreateComponentForOldSaves(MapEvent mapEvent, float nextSettlementDamage, int lootedItemCount, float raidDamage)
	{
		return new RaidEventComponent(mapEvent)
		{
			_nextSettlementDamage = nextSettlementDamage,
			_lootedItemCount = lootedItemCount,
			RaidDamage = raidDamage
		};
	}

	protected override void OnInitialize()
	{
		_nextSettlementDamage = 0f;
		RaidDamage = 0f;
		ChangeVillageStateAction.ApplyBySettingToBeingRaided(MapEventSettlement, AttackerSide.LeaderParty.MobileParty);
		if (_raidProductionRewards == null)
		{
			_raidProductionRewards = new Dictionary<ItemObject, float>();
		}
		else
		{
			_raidProductionRewards.Clear();
		}
	}

	protected override void OnFinalize()
	{
		if (DefenderSide.Parties.Count <= 1)
		{
			CampaignEventDispatcher.Instance.RaidCompleted((BattleState == BattleState.AttackerVictory) ? BattleSideEnum.Attacker : BattleSideEnum.Defender, this);
		}
		if (MapEventSettlement.SettlementHitPoints > 0f)
		{
			ChangeVillageStateAction.ApplyBySettingToNormal(MapEventSettlement);
		}
		if (_raidProductionRewards != null)
		{
			_raidProductionRewards.Clear();
		}
	}

	internal override void Finish()
	{
		if ((double?)MapEventSettlement?.SettlementHitPoints <= 1E-05)
		{
			ChangeVillageStateAction.ApplyBySettingToLooted(MapEventSettlement, AttackerSide.LeaderParty.MobileParty);
		}
		if (IsPlayerMapEvent && PlayerEncounter.Current != null)
		{
			PlayerEncounter.Current.FinishRaid();
		}
	}

	internal override void Update(ref bool finish)
	{
		if (DefenderSide.TroopCount == 0)
		{
			_nextSettlementDamage += Campaign.Current.Models.RaidModel.CalculateHitDamage(AttackerSide, MapEventSettlement.SettlementHitPoints);
			if (!((double)_nextSettlementDamage > 0.05))
			{
				return;
			}
			float num = -1f;
			if (MapEventSettlement.IsVillage)
			{
				num = _nextSettlementDamage * 0.5f * MapEventSettlement.Village.Hearth;
				MapEventSettlement.Village.Hearth -= num;
				Hero leaderHero = AttackerSide.LeaderParty.LeaderHero;
				if (leaderHero != null)
				{
					int num2 = (int)(num * (float)Campaign.Current.Models.RaidModel.GoldRewardForEachLostHearth);
					if (num2 > 0)
					{
						if (leaderHero == Hero.MainHero)
						{
							TextObject textObject = GameTexts.FindText("str_plunder_gain_message");
							textObject.SetTextVariable("GOLD", num2);
							MBInformationManager.AddQuickInformation(textObject);
						}
						GiveGoldAction.ApplyBetweenCharacters(null, leaderHero, num2, disableNotification: true);
					}
				}
			}
			MapEventSettlement.SettlementHitPoints -= _nextSettlementDamage;
			RaidDamage += _nextSettlementDamage;
			RaidDamage = MathF.Min(RaidDamage, 1f);
			int num3 = 0;
			foreach (MapEventParty party in AttackerSide.Parties)
			{
				num3 += party.Party.MemberRoster.TotalManCount;
			}
			if (num3 > 0)
			{
				float nextSettlementDamage = _nextSettlementDamage;
				int num4 = 0;
				foreach (ItemRosterElement item3 in MapEventSettlement.ItemRoster)
				{
					num4 += item3.Amount;
				}
				ItemRoster lootedItems = new ItemRoster();
				PartyBase partyBase = AttackerSide.LeaderParty;
				if (num4 > 0)
				{
					int num5 = MBRandom.RoundRandomized((float)num4 * nextSettlementDamage);
					if (num5 > num4)
					{
						num5 = num4;
					}
					EquipmentElement rosterElement = MapEventSettlement.ItemRoster.First().EquipmentElement;
					for (int i = 0; i < num5; i++)
					{
						int num6 = (int)(MBRandom.RandomFloat * (float)num4);
						bool flag = false;
						foreach (ItemRosterElement item4 in MapEventSettlement.ItemRoster)
						{
							int amount = item4.Amount;
							num6 -= amount;
							if (num6 < 0)
							{
								rosterElement = item4.EquipmentElement;
								int num7 = (int)(MBRandom.RandomFloat * (float)num3);
								foreach (MapEventParty party2 in AttackerSide.Parties)
								{
									num7 -= party2.Party.MemberRoster.TotalManCount;
									if (num7 < 0)
									{
										partyBase = party2.Party;
										flag = true;
										break;
									}
								}
							}
							if (flag)
							{
								break;
							}
						}
						if (rosterElement.IsEqualTo(EquipmentElement.Invalid))
						{
							continue;
						}
						MapEventSettlement.ItemRoster.AddToCounts(rosterElement, -1);
						if (_lootedItemCount % 2 == 1)
						{
							int num8 = 1;
							if (rosterElement.Item.IsFood)
							{
								Hero leaderHero2 = partyBase.LeaderHero;
								if (leaderHero2 != null && leaderHero2.GetPerkValue(DefaultPerks.Steward.EfficientCampaigner))
								{
									num8++;
								}
							}
							partyBase.ItemRoster.AddToCounts(rosterElement, num8);
							lootedItems.AddToCounts(rosterElement, num8);
						}
						_lootedItemCount++;
						num4--;
						rosterElement = EquipmentElement.Invalid;
					}
				}
				foreach (var production in MapEventSettlement.Village.VillageType.Productions)
				{
					if ((MapEventSettlement.Village.VillageType.PrimaryProduction == DefaultItems.Grain && production.Item1 == DefaultItems.Grain) || production.Item1 != DefaultItems.Grain)
					{
						float item = production.Item2;
						float num9 = num * item / 60f;
						if (_raidProductionRewards.TryGetValue(production.Item1, out var value))
						{
							_raidProductionRewards[production.Item1] = value + num9;
						}
						else
						{
							_raidProductionRewards.Add(production.Item1, num9);
						}
						if (_raidProductionRewards[production.Item1] >= 1f)
						{
							int num10 = (int)_raidProductionRewards[production.Item1];
							_raidProductionRewards[production.Item1] = _raidProductionRewards[production.Item1] - (float)num10;
							LootItemInRaid(AttackerSide.LeaderParty, production.Item1, num10, ref lootedItems);
						}
					}
				}
				for (int j = 0; j < (int)num; j++)
				{
					if (MBRandom.RandomFloat < 0.25f)
					{
						ItemObject item2 = MBRandom.ChooseWeighted(Campaign.Current.Models.RaidModel.GetCommonLootItemScores());
						LootItemInRaid(AttackerSide.LeaderParty, item2, 1, ref lootedItems);
					}
				}
				if (lootedItems.Any() && partyBase?.MobileParty != null)
				{
					CampaignEventDispatcher.Instance.OnItemsLooted(partyBase.MobileParty, lootedItems);
				}
			}
			if (MapEventSettlement.SettlementHitPoints <= 1E-05f)
			{
				MapEventSettlement.SettlementHitPoints = 0f;
				base.MapEvent.BattleState = BattleState.AttackerVictory;
				finish = true;
				MapEventSettlement.Party.SetLevelMaskIsDirty();
			}
			_nextSettlementDamage = 0f;
		}
		else if (AttackerSide.TroopCount == 0)
		{
			finish = true;
		}
	}

	internal override void OnAfterLoad()
	{
		if (_raidProductionRewards == null)
		{
			_raidProductionRewards = new Dictionary<ItemObject, float>();
		}
	}

	private void LootItemInRaid(PartyBase party, ItemObject item, int count, ref ItemRoster lootedItems)
	{
		if (item.IsFood)
		{
			MobileParty mobileParty = party.MobileParty;
			if (mobileParty != null && mobileParty.HasPerk(DefaultPerks.Steward.EfficientCampaigner))
			{
				count++;
			}
		}
		lootedItems.AddToCounts(item, count);
		party.ItemRoster.AddToCounts(item, count);
	}
}
