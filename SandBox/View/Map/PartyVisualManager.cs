using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace SandBox.View.Map;

public class PartyVisualManager : CampaignEntityVisualComponent
{
	private readonly Dictionary<PartyBase, PartyVisual> _partiesAndVisuals = new Dictionary<PartyBase, PartyVisual>();

	private readonly List<PartyVisual> _visualsFlattened = new List<PartyVisual>();

	private int _dirtyPartyVisualCount;

	private PartyVisual[] _dirtyPartiesList = new PartyVisual[2500];

	private readonly List<PartyVisual> _fadingPartiesFlatten = new List<PartyVisual>();

	private readonly HashSet<PartyVisual> _fadingPartiesSet = new HashSet<PartyVisual>();

	public static PartyVisualManager Current => Campaign.Current.GetEntityComponent<PartyVisualManager>();

	protected override void OnInitialize()
	{
		base.OnInitialize();
		foreach (MobileParty item in MobileParty.All)
		{
			AddNewPartyVisualForParty(item.Party);
		}
		foreach (Settlement item2 in Settlement.All)
		{
			AddNewPartyVisualForParty(item2.Party);
		}
		CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, OnMobilePartyCreated);
		CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnMobilePartyDestroyed);
	}

	private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
	{
		_partiesAndVisuals[mobileParty.Party].OnPartyRemoved();
		_visualsFlattened.Remove(_partiesAndVisuals[mobileParty.Party]);
		_partiesAndVisuals.Remove(mobileParty.Party);
	}

	private void OnMobilePartyCreated(MobileParty mobileParty)
	{
		AddNewPartyVisualForParty(mobileParty.Party);
	}

	private void AddNewPartyVisualForParty(PartyBase partyBase)
	{
		PartyVisual partyVisual = new PartyVisual(partyBase);
		partyVisual.OnStartup();
		_partiesAndVisuals.Add(partyBase, partyVisual);
		_visualsFlattened.Add(partyVisual);
	}

	public PartyVisual GetVisualOfParty(PartyBase partyBase)
	{
		return _partiesAndVisuals[partyBase];
	}

	public void OnFinalized()
	{
		foreach (PartyVisual value in _partiesAndVisuals.Values)
		{
			value.ReleaseResources();
		}
	}

	public override void OnTick(float realDt, float dt)
	{
		_dirtyPartyVisualCount = -1;
		TWParallel.For(0, _visualsFlattened.Count, delegate(int startInclusive, int endExclusive)
		{
			for (int j = startInclusive; j < endExclusive; j++)
			{
				_visualsFlattened[j].Tick(dt, ref _dirtyPartyVisualCount, ref _dirtyPartiesList);
			}
		});
		for (int i = 0; i < _dirtyPartyVisualCount + 1; i++)
		{
			_dirtyPartiesList[i].ValidateIsDirty(realDt, dt);
		}
		for (int num = _fadingPartiesFlatten.Count - 1; num >= 0; num--)
		{
			_fadingPartiesFlatten[num].TickFadingState(realDt, dt);
		}
	}

	internal void RegisterFadingVisual(PartyVisual visual)
	{
		if (!_fadingPartiesSet.Contains(visual))
		{
			_fadingPartiesFlatten.Add(visual);
			_fadingPartiesSet.Add(visual);
		}
	}

	internal void UnRegisterFadingVisual(PartyVisual visual)
	{
		if (_fadingPartiesSet.Contains(visual))
		{
			int index = _fadingPartiesFlatten.IndexOf(visual);
			_fadingPartiesFlatten[index] = _fadingPartiesFlatten[_fadingPartiesFlatten.Count - 1];
			_fadingPartiesFlatten.Remove(_fadingPartiesFlatten[_fadingPartiesFlatten.Count - 1]);
			_fadingPartiesSet.Remove(visual);
		}
	}
}
