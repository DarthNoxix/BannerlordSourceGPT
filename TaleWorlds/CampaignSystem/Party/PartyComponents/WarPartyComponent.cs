using System.Collections.Generic;

namespace TaleWorlds.CampaignSystem.Party.PartyComponents;

public abstract class WarPartyComponent : PartyComponent
{
	public Clan Clan => base.Party.MobileParty.ActualClan;

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void OnInitialize()
	{
		Clan.OnWarPartyAdded(this);
	}

	protected override void OnFinalize()
	{
		Clan.OnWarPartyRemoved(this);
	}

	internal void OnClanChange(Clan oldClan, Clan newClan)
	{
		oldClan.OnWarPartyRemoved(this);
		newClan.OnWarPartyAdded(this);
	}
}