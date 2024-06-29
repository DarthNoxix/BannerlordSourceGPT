using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate;

public class SettlementNameplatesVM : ViewModel
{
	private readonly Camera _mapCamera;

	private Vec3 _cachedCameraPosition;

	private readonly TWParallel.ParallelForAuxPredicate UpdateNameplateAuxMTPredicate;

	private readonly Action<Vec2> _fastMoveCameraToPosition;

	private IEnumerable<Tuple<Settlement, GameEntity>> _allHideouts;

	private IEnumerable<Tuple<Settlement, GameEntity>> _allRetreats;

	private MBBindingList<SettlementNameplateVM> _nameplates;

	[DataSourceProperty]
	public MBBindingList<SettlementNameplateVM> Nameplates
	{
		get
		{
			return _nameplates;
		}
		set
		{
			if (_nameplates != value)
			{
				_nameplates = value;
				OnPropertyChangedWithValue(value, "Nameplates");
			}
		}
	}

	public SettlementNameplatesVM(Camera mapCamera, Action<Vec2> fastMoveCameraToPosition)
	{
		Nameplates = new MBBindingList<SettlementNameplateVM>();
		_mapCamera = mapCamera;
		_fastMoveCameraToPosition = fastMoveCameraToPosition;
		CampaignEvents.PartyVisibilityChangedEvent.AddNonSerializedListener(this, OnPartyBaseVisibilityChange);
		CampaignEvents.WarDeclared.AddNonSerializedListener(this, OnWarDeclared);
		CampaignEvents.MakePeace.AddNonSerializedListener(this, OnPeaceDeclared);
		CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, OnClanChangeKingdom);
		CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, OnSettlementOwnerChanged);
		CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, OnSiegeEventStartedOnSettlement);
		CampaignEvents.OnSiegeEventEndedEvent.AddNonSerializedListener(this, OnSiegeEventEndedOnSettlement);
		CampaignEvents.RebelliousClanDisbandedAtSettlement.AddNonSerializedListener(this, OnRebelliousClanDisbandedAtSettlement);
		UpdateNameplateAuxMTPredicate = UpdateNameplateAuxMT;
	}

	public override void RefreshValues()
	{
		base.RefreshValues();
		Nameplates.ApplyActionOnAllItems(delegate(SettlementNameplateVM x)
		{
			x.RefreshValues();
		});
	}

	public void Initialize(IEnumerable<Tuple<Settlement, GameEntity>> settlements)
	{
		IEnumerable<Tuple<Settlement, GameEntity>> enumerable = settlements.Where((Tuple<Settlement, GameEntity> x) => !x.Item1.IsHideout && !(x.Item1.SettlementComponent is RetirementSettlementComponent));
		_allHideouts = settlements.Where((Tuple<Settlement, GameEntity> x) => x.Item1.IsHideout && !(x.Item1.SettlementComponent is RetirementSettlementComponent));
		_allRetreats = settlements.Where((Tuple<Settlement, GameEntity> x) => !x.Item1.IsHideout && x.Item1.SettlementComponent is RetirementSettlementComponent);
		foreach (Tuple<Settlement, GameEntity> item4 in enumerable)
		{
			SettlementNameplateVM item = new SettlementNameplateVM(item4.Item1, item4.Item2, _mapCamera, _fastMoveCameraToPosition);
			Nameplates.Add(item);
		}
		foreach (Tuple<Settlement, GameEntity> allHideout in _allHideouts)
		{
			if (allHideout.Item1.Hideout.IsSpotted)
			{
				SettlementNameplateVM item2 = new SettlementNameplateVM(allHideout.Item1, allHideout.Item2, _mapCamera, _fastMoveCameraToPosition);
				Nameplates.Add(item2);
			}
		}
		foreach (Tuple<Settlement, GameEntity> allRetreat in _allRetreats)
		{
			if (allRetreat.Item1.SettlementComponent is RetirementSettlementComponent retirementSettlementComponent)
			{
				if (retirementSettlementComponent.IsSpotted)
				{
					SettlementNameplateVM item3 = new SettlementNameplateVM(allRetreat.Item1, allRetreat.Item2, _mapCamera, _fastMoveCameraToPosition);
					Nameplates.Add(item3);
				}
			}
			else
			{
				Debug.FailedAssert("A seetlement which is IsRetreat doesn't have a retirement component.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\Nameplate\\SettlementNameplatesVM.cs", "Initialize", 83);
			}
		}
		foreach (SettlementNameplateVM nameplate in Nameplates)
		{
			if (nameplate.Settlement?.SiegeEvent != null)
			{
				nameplate.OnSiegeEventStartedOnSettlement(nameplate.Settlement?.SiegeEvent);
			}
			else if (nameplate.Settlement.IsTown || nameplate.Settlement.IsCastle)
			{
				Clan ownerClan = nameplate.Settlement.OwnerClan;
				if (ownerClan != null && ownerClan.IsRebelClan)
				{
					nameplate.OnRebelliousClanFormed(nameplate.Settlement.OwnerClan);
				}
			}
		}
		RefreshRelationsOfNameplates();
	}

	private void UpdateNameplateAuxMT(int startInclusive, int endExclusive)
	{
		for (int i = startInclusive; i < endExclusive; i++)
		{
			Nameplates[i].UpdateNameplateMT(_cachedCameraPosition);
		}
	}

	public void Update()
	{
		_cachedCameraPosition = _mapCamera.Position;
		TWParallel.For(0, Nameplates.Count, UpdateNameplateAuxMTPredicate);
		for (int i = 0; i < Nameplates.Count; i++)
		{
			Nameplates[i].RefreshBindValues();
		}
	}

	private void OnSiegeEventStartedOnSettlement(SiegeEvent siegeEvent)
	{
		Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == siegeEvent.BesiegedSettlement)?.OnSiegeEventStartedOnSettlement(siegeEvent);
	}

	private void OnSiegeEventEndedOnSettlement(SiegeEvent siegeEvent)
	{
		Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == siegeEvent.BesiegedSettlement)?.OnSiegeEventEndedOnSettlement(siegeEvent);
	}

	private void OnMapEventStartedOnSettlement(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
	{
		Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == mapEvent.MapEventSettlement)?.OnMapEventStartedOnSettlement(mapEvent);
	}

	private void OnMapEventEndedOnSettlement(MapEvent mapEvent)
	{
		Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == mapEvent.MapEventSettlement)?.OnMapEventEndedOnSettlement();
	}

	private void OnPartyBaseVisibilityChange(PartyBase party)
	{
		if (!party.IsSettlement)
		{
			return;
		}
		Tuple<Settlement, GameEntity> desiredSettlementTuple = null;
		if (party.Settlement.IsHideout)
		{
			desiredSettlementTuple = _allHideouts.SingleOrDefault((Tuple<Settlement, GameEntity> h) => h.Item1.Hideout == party.Settlement.Hideout);
		}
		else if (party.Settlement.SettlementComponent is RetirementSettlementComponent)
		{
			desiredSettlementTuple = _allRetreats.SingleOrDefault((Tuple<Settlement, GameEntity> h) => h.Item1.SettlementComponent as RetirementSettlementComponent == party.Settlement.SettlementComponent as RetirementSettlementComponent);
		}
		else
		{
			Debug.FailedAssert("We don't support hiding non retreat or non hideout settlements.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\Nameplate\\SettlementNameplatesVM.cs", "OnPartyBaseVisibilityChange", 176);
		}
		if (desiredSettlementTuple != null)
		{
			SettlementNameplateVM settlementNameplateVM = Nameplates.SingleOrDefault((SettlementNameplateVM n) => n.Settlement == desiredSettlementTuple.Item1);
			if (party.IsVisible && settlementNameplateVM == null)
			{
				SettlementNameplateVM settlementNameplateVM2 = new SettlementNameplateVM(desiredSettlementTuple.Item1, desiredSettlementTuple.Item2, _mapCamera, _fastMoveCameraToPosition);
				Nameplates.Add(settlementNameplateVM2);
				settlementNameplateVM2.RefreshRelationStatus();
			}
			else if (!party.IsVisible && settlementNameplateVM != null)
			{
				Nameplates.Remove(settlementNameplateVM);
			}
		}
	}

	private void OnPeaceDeclared(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
	{
		OnPeaceOrWarDeclared(faction1, faction2);
	}

	private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail arg3)
	{
		OnPeaceOrWarDeclared(faction1, faction2);
	}

	private void OnPeaceOrWarDeclared(IFaction faction1, IFaction faction2)
	{
		if (faction1 == Hero.MainHero.MapFaction || faction1 == Hero.MainHero.Clan || faction2 == Hero.MainHero.MapFaction || faction2 == Hero.MainHero.Clan)
		{
			RefreshRelationsOfNameplates();
		}
	}

	private void OnClanChangeKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
	{
		RefreshRelationsOfNameplates();
	}

	private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero previousOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
	{
		SettlementNameplateVM settlementNameplateVM = Nameplates.SingleOrDefault((SettlementNameplateVM n) => n.Settlement == settlement);
		settlementNameplateVM?.RefreshDynamicProperties(forceUpdate: true);
		settlementNameplateVM?.RefreshRelationStatus();
		foreach (Village village in settlement.BoundVillages)
		{
			SettlementNameplateVM settlementNameplateVM2 = Nameplates.SingleOrDefault((SettlementNameplateVM n) => n.Settlement.IsVillage && n.Settlement.Village == village);
			settlementNameplateVM2?.RefreshDynamicProperties(forceUpdate: true);
			settlementNameplateVM2?.RefreshRelationStatus();
		}
		if (detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByRebellion)
		{
			Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == settlement)?.OnRebelliousClanFormed(newOwner.Clan);
		}
		else if (previousOwner != null && previousOwner.IsRebel)
		{
			Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == settlement)?.OnRebelliousClanDisbanded(previousOwner.Clan);
		}
	}

	private void OnRebelliousClanDisbandedAtSettlement(Settlement settlement, Clan clan)
	{
		Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == settlement)?.OnRebelliousClanDisbanded(clan);
	}

	private void RefreshRelationsOfNameplates()
	{
		foreach (SettlementNameplateVM nameplate in Nameplates)
		{
			nameplate.RefreshRelationStatus();
		}
	}

	private void RefreshDynamicPropertiesOfNameplates()
	{
		foreach (SettlementNameplateVM nameplate in Nameplates)
		{
			nameplate.RefreshDynamicProperties(forceUpdate: false);
		}
	}

	public override void OnFinalize()
	{
		base.OnFinalize();
		CampaignEvents.PartyVisibilityChangedEvent.ClearListeners(this);
		CampaignEvents.WarDeclared.ClearListeners(this);
		CampaignEvents.MakePeace.ClearListeners(this);
		CampaignEvents.OnClanChangedKingdomEvent.ClearListeners(this);
		CampaignEvents.OnSettlementOwnerChangedEvent.ClearListeners(this);
		CampaignEvents.OnSiegeEventStartedEvent.ClearListeners(this);
		CampaignEvents.OnSiegeEventEndedEvent.ClearListeners(this);
		CampaignEvents.RebelliousClanDisbandedAtSettlement.ClearListeners(this);
		Nameplates.ApplyActionOnAllItems(delegate(SettlementNameplateVM n)
		{
			n.OnFinalize();
		});
	}
}
