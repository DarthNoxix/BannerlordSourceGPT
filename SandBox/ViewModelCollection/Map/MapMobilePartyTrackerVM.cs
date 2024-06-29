using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Map;

public class MapMobilePartyTrackerVM : ViewModel
{
	private readonly Camera _mapCamera;

	private readonly Action<Vec2> _fastMoveCameraToPosition;

	private readonly TWParallel.ParallelForAuxPredicate UpdateTrackerPropertiesAuxPredicate;

	private MBBindingList<MobilePartyTrackItemVM> _trackers;

	public MBBindingList<MobilePartyTrackItemVM> Trackers
	{
		get
		{
			return _trackers;
		}
		set
		{
			if (value != _trackers)
			{
				_trackers = value;
				OnPropertyChangedWithValue(value, "Trackers");
			}
		}
	}

	public MapMobilePartyTrackerVM(Camera mapCamera, Action<Vec2> fastMoveCameraToPosition)
	{
		_mapCamera = mapCamera;
		_fastMoveCameraToPosition = fastMoveCameraToPosition;
		UpdateTrackerPropertiesAuxPredicate = UpdateTrackerPropertiesAux;
		Trackers = new MBBindingList<MobilePartyTrackItemVM>();
		InitList();
		CampaignEvents.ArmyCreated.AddNonSerializedListener(this, OnArmyCreated);
		CampaignEvents.ArmyDispersed.AddNonSerializedListener(this, OnArmyDispersed);
		CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, OnMobilePartyCreated);
		CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnPartyDestroyed);
		CampaignEvents.MobilePartyQuestStatusChanged.AddNonSerializedListener(this, OnPartyQuestStatusChanged);
		CampaignEvents.OnPartyDisbandedEvent.AddNonSerializedListener(this, OnPartyDisbanded);
		CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, OnClanChangedKingdom);
		CampaignEvents.OnCompanionClanCreatedEvent.AddNonSerializedListener(this, OnCompanionClanCreated);
	}

	public override void OnFinalize()
	{
		base.OnFinalize();
		CampaignEventDispatcher.Instance.RemoveListeners(this);
	}

	private void InitList()
	{
		Trackers.Clear();
		foreach (WarPartyComponent warPartyComponent in Clan.PlayerClan.WarPartyComponents)
		{
			if (CanAddParty(warPartyComponent.MobileParty))
			{
				Trackers.Add(new MobilePartyTrackItemVM(warPartyComponent.MobileParty, _mapCamera, _fastMoveCameraToPosition));
			}
		}
		foreach (CaravanPartyComponent item in Clan.PlayerClan.Heroes.SelectMany((Hero h) => h.OwnedCaravans))
		{
			if (CanAddParty(item.MobileParty))
			{
				Trackers.Add(new MobilePartyTrackItemVM(item.MobileParty, _mapCamera, _fastMoveCameraToPosition));
			}
		}
		if (Clan.PlayerClan.Kingdom != null)
		{
			foreach (Army army in Clan.PlayerClan.Kingdom.Armies)
			{
				Trackers.Add(new MobilePartyTrackItemVM(army, _mapCamera, _fastMoveCameraToPosition));
			}
		}
		foreach (TrackedObject trackedObject in Campaign.Current.VisualTrackerManager.TrackedObjects)
		{
			if (trackedObject.Object is MobileParty mobileParty && mobileParty.LeaderHero == null && mobileParty.IsCurrentlyUsedByAQuest)
			{
				Trackers.Add(new MobilePartyTrackItemVM(mobileParty, _mapCamera, _fastMoveCameraToPosition));
			}
		}
	}

	private void UpdateTrackerPropertiesAux(int startInclusive, int endExclusive)
	{
		for (int i = startInclusive; i < endExclusive; i++)
		{
			Trackers[i].UpdateProperties();
			Trackers[i].UpdatePosition();
		}
	}

	public void Update()
	{
		TWParallel.For(0, Trackers.Count, UpdateTrackerPropertiesAuxPredicate);
		Trackers.ApplyActionOnAllItems(delegate(MobilePartyTrackItemVM t)
		{
			t.RefreshBinding();
		});
	}

	public void UpdateProperties()
	{
		Trackers.ApplyActionOnAllItems(delegate(MobilePartyTrackItemVM t)
		{
			t.UpdateProperties();
		});
	}

	private bool CanAddParty(MobileParty party)
	{
		if (party != null && !party.IsMainParty && !party.IsMilitia && !party.IsGarrison && !party.IsVillager && !party.IsBandit && !party.IsBanditBossParty && !party.IsCurrentlyUsedByAQuest)
		{
			if (party.IsCaravan)
			{
				return party.CaravanPartyComponent.Owner == Hero.MainHero;
			}
			return true;
		}
		return false;
	}

	private void AddIfNotAdded(Army army)
	{
		if (Trackers.FirstOrDefault((MobilePartyTrackItemVM t) => t.TrackedArmy == army) == null)
		{
			Trackers.Add(new MobilePartyTrackItemVM(army, _mapCamera, _fastMoveCameraToPosition));
		}
	}

	private void AddIfNotAdded(MobileParty party)
	{
		for (int i = 0; i < Trackers.Count; i++)
		{
			if (Trackers[i].TrackedParty == party)
			{
				return;
			}
		}
		Trackers.Add(new MobilePartyTrackItemVM(party, _mapCamera, _fastMoveCameraToPosition));
	}

	private void RemoveIfExists(Army army)
	{
		MobilePartyTrackItemVM mobilePartyTrackItemVM = Trackers.FirstOrDefault((MobilePartyTrackItemVM t) => t.TrackedArmy == army);
		if (mobilePartyTrackItemVM != null)
		{
			Trackers.Remove(mobilePartyTrackItemVM);
		}
	}

	private void RemoveIfExists(MobileParty party)
	{
		for (int i = 0; i < Trackers.Count; i++)
		{
			if (Trackers[i].TrackedParty == party)
			{
				Trackers.RemoveAt(i);
				break;
			}
		}
	}

	private void OnPartyDestroyed(MobileParty mobileParty, PartyBase arg2)
	{
		RemoveIfExists(mobileParty);
	}

	private void OnPartyQuestStatusChanged(MobileParty mobileParty, bool isUsedByQuest)
	{
		if (isUsedByQuest)
		{
			RemoveIfExists(mobileParty);
		}
		else if (CanAddParty(mobileParty))
		{
			AddIfNotAdded(mobileParty);
		}
	}

	private void OnPartyDisbanded(MobileParty disbandedParty, Settlement relatedSettlement)
	{
		RemoveIfExists(disbandedParty);
	}

	private void OnMobilePartyCreated(MobileParty party)
	{
		if (party.IsLordParty)
		{
			if (Clan.PlayerClan.WarPartyComponents.Contains(party.WarPartyComponent))
			{
				AddIfNotAdded(party);
			}
		}
		else if (CanAddParty(party))
		{
			AddIfNotAdded(party);
		}
	}

	private void OnArmyDispersed(Army army, Army.ArmyDispersionReason arg2, bool arg3)
	{
		if (army.Kingdom == Hero.MainHero.MapFaction)
		{
			RemoveIfExists(army);
		}
	}

	private void OnArmyCreated(Army army)
	{
		if (army.Kingdom == Hero.MainHero.MapFaction)
		{
			AddIfNotAdded(army);
		}
	}

	private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
	{
		if (clan == Clan.PlayerClan)
		{
			InitList();
		}
	}

	private void OnCompanionClanCreated(Clan clan)
	{
		RemoveIfExists(clan.Leader.PartyBelongedTo);
	}
}
