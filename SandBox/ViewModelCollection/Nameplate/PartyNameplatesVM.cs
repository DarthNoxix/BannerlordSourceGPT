using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace SandBox.ViewModelCollection.Nameplate;

public class PartyNameplatesVM : ViewModel
{
	public class NameplateDistanceComparer : IComparer<PartyNameplateVM>
	{
		public int Compare(PartyNameplateVM x, PartyNameplateVM y)
		{
			return y.DistanceToCamera.CompareTo(x.DistanceToCamera);
		}
	}

	private readonly Camera _mapCamera;

	private readonly Action _resetCamera;

	private readonly Func<bool> _isShowPartyNamesEnabled;

	private readonly NameplateDistanceComparer _nameplateComparer;

	private MBBindingList<PartyNameplateVM> _nameplates;

	[DataSourceProperty]
	public MBBindingList<PartyNameplateVM> Nameplates
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

	public PartyNameplatesVM(Camera mapCamera, Action resetCamera, Func<bool> isShowPartyNamesEnabled)
	{
		Nameplates = new MBBindingList<PartyNameplateVM>();
		_nameplateComparer = new NameplateDistanceComparer();
		_mapCamera = mapCamera;
		_resetCamera = resetCamera;
		_isShowPartyNamesEnabled = isShowPartyNamesEnabled;
		RegisterEvents();
	}

	public override void RefreshValues()
	{
		base.RefreshValues();
		Nameplates.ApplyActionOnAllItems(delegate(PartyNameplateVM x)
		{
			x.RefreshValues();
		});
	}

	public void Initialize()
	{
		foreach (MobileParty item2 in MobileParty.All.Where((MobileParty p) => p.IsSpotted() && p.CurrentSettlement == null))
		{
			PartyNameplateVM item = new PartyNameplateVM(item2, _mapCamera, _resetCamera, _isShowPartyNamesEnabled);
			Nameplates.Add(item);
		}
	}

	private void OnClanChangeKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
	{
		foreach (PartyNameplateVM item in Nameplates.Where((PartyNameplateVM p) => p.Party.LeaderHero != null && p.Party.LeaderHero.Clan == clan))
		{
			item.RefreshDynamicProperties(forceUpdate: true);
		}
	}

	private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
	{
		for (int i = 0; i < Nameplates.Count; i++)
		{
			PartyNameplateVM partyNameplateVM = Nameplates[i];
			if (partyNameplateVM.Party == party)
			{
				partyNameplateVM.OnFinalize();
				Nameplates.RemoveAt(i);
				break;
			}
		}
	}

	private void OnSettlementLeft(MobileParty party, Settlement settlement)
	{
		if (party.Army != null && party.Army.LeaderParty == party)
		{
			foreach (MobileParty armyParty in party.Army.Parties)
			{
				if (armyParty.IsSpotted() && Nameplates.All((PartyNameplateVM p) => p.Party != armyParty))
				{
					Nameplates.Add(new PartyNameplateVM(armyParty, _mapCamera, _resetCamera, _isShowPartyNamesEnabled));
				}
			}
			return;
		}
		if (party.IsSpotted() && Nameplates.All((PartyNameplateVM p) => p.Party != party))
		{
			Nameplates.Add(new PartyNameplateVM(party, _mapCamera, _resetCamera, _isShowPartyNamesEnabled));
		}
	}

	private void OnPartyVisibilityChanged(PartyBase party)
	{
		if (party.IsMobile)
		{
			PartyNameplateVM partyNameplateVM;
			if (party.MobileParty.IsSpotted() && party.MobileParty.CurrentSettlement == null && Nameplates.All((PartyNameplateVM p) => p.Party != party.MobileParty))
			{
				Nameplates.Add(new PartyNameplateVM(party.MobileParty, _mapCamera, _resetCamera, _isShowPartyNamesEnabled));
			}
			else if ((!party.MobileParty.IsSpotted() || party.MobileParty.CurrentSettlement != null) && (partyNameplateVM = Nameplates.FirstOrDefault((PartyNameplateVM p) => p.Party == party.MobileParty)) != null && !partyNameplateVM.IsMainParty)
			{
				partyNameplateVM.OnFinalize();
				Nameplates.Remove(partyNameplateVM);
			}
		}
	}

	public void Update()
	{
		for (int i = 0; i < Nameplates.Count; i++)
		{
			PartyNameplateVM partyNameplateVM = Nameplates[i];
			partyNameplateVM.RefreshPosition();
			partyNameplateVM.DetermineIsVisibleOnMap();
			partyNameplateVM.RefreshDynamicProperties(forceUpdate: false);
		}
		for (int j = 0; j < Nameplates.Count; j++)
		{
			Nameplates[j].RefreshBinding();
		}
		Nameplates.Sort(_nameplateComparer);
	}

	private void OnPlayerCharacterChangedEvent(Hero oldPlayer, Hero newPlayer, MobileParty newMainParty, bool isMainPartyChanged)
	{
		PartyNameplateVM partyNameplateVM = Nameplates.FirstOrDefault((PartyNameplateVM n) => n.GetIsMainParty);
		if (partyNameplateVM != null)
		{
			partyNameplateVM.OnFinalize();
			Nameplates.Remove(partyNameplateVM);
		}
		if (Nameplates.AllQ((PartyNameplateVM p) => p.Party.LeaderHero != newPlayer))
		{
			Nameplates.Add(new PartyNameplateVM(newMainParty, _mapCamera, _resetCamera, _isShowPartyNamesEnabled));
		}
		foreach (PartyNameplateVM nameplate in Nameplates)
		{
			nameplate.OnPlayerCharacterChanged(newPlayer);
		}
	}

	private void OnGameOver()
	{
		PartyNameplateVM partyNameplateVM = Nameplates.FirstOrDefault((PartyNameplateVM n) => n.IsMainParty);
		if (partyNameplateVM != null)
		{
			partyNameplateVM.OnFinalize();
			Nameplates.Remove(partyNameplateVM);
		}
	}

	public override void OnFinalize()
	{
		base.OnFinalize();
		Nameplates.Clear();
		UnregisterEvents();
	}

	private void RegisterEvents()
	{
		CampaignEvents.SettlementEntered.AddNonSerializedListener(this, OnSettlementEntered);
		CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, OnSettlementLeft);
		CampaignEvents.PartyVisibilityChangedEvent.AddNonSerializedListener(this, OnPartyVisibilityChanged);
		CampaignEvents.OnPlayerCharacterChangedEvent.AddNonSerializedListener(this, OnPlayerCharacterChangedEvent);
		CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, OnClanChangeKingdom);
		CampaignEvents.OnGameOverEvent.AddNonSerializedListener(this, OnGameOver);
	}

	private void UnregisterEvents()
	{
		CampaignEvents.SettlementEntered.ClearListeners(this);
		CampaignEvents.OnSettlementLeftEvent.ClearListeners(this);
		CampaignEvents.PartyVisibilityChangedEvent.ClearListeners(this);
		CampaignEvents.OnPlayerCharacterChangedEvent.ClearListeners(this);
		CampaignEvents.OnClanChangedKingdomEvent.ClearListeners(this);
		CampaignEvents.OnGameOverEvent.ClearListeners(this);
	}
}
