using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map;

public readonly struct PanelScreenStatus
{
	public readonly bool IsCharacterScreenOpen;

	public readonly bool IsPartyScreenOpen;

	public readonly bool IsQuestsScreenOpen;

	public readonly bool IsInventoryScreenOpen;

	public readonly bool IsClanScreenOpen;

	public readonly bool IsKingdomScreenOpen;

	public readonly bool IsAnyPanelScreenOpen;

	public readonly bool IsCurrentScreenLocksNavigation;

	public PanelScreenStatus(ScreenBase screen)
	{
		IsCharacterScreenOpen = false;
		IsPartyScreenOpen = false;
		IsQuestsScreenOpen = false;
		IsInventoryScreenOpen = false;
		IsClanScreenOpen = false;
		IsKingdomScreenOpen = false;
		IsAnyPanelScreenOpen = true;
		IsCurrentScreenLocksNavigation = false;
		if (screen is GauntletCharacterDeveloperScreen)
		{
			IsCharacterScreenOpen = true;
		}
		else if (screen is GauntletPartyScreen)
		{
			IsPartyScreenOpen = true;
		}
		else if (screen is GauntletQuestsScreen)
		{
			IsQuestsScreenOpen = true;
		}
		else if (screen is GauntletInventoryScreen)
		{
			IsInventoryScreenOpen = true;
		}
		else if (screen is GauntletClanScreen)
		{
			IsClanScreenOpen = true;
		}
		else if (screen is GauntletKingdomScreen gauntletKingdomScreen)
		{
			IsKingdomScreenOpen = true;
			IsCurrentScreenLocksNavigation = gauntletKingdomScreen?.IsMakingDecision ?? false;
		}
		else
		{
			IsAnyPanelScreenOpen = false;
		}
	}
}
