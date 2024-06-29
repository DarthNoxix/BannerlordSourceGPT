using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI;

public static class SandBoxGauntletUICheats
{
	[CommandLineFunctionality.CommandLineArgumentFunction("set_inventory_search_enabled", "ui")]
	public static string SetInventorySearchEnabled(List<string> args)
	{
		string result = "Format is \"ui.set_inventory_search_enabled [1/0]\".";
		if (ScreenManager.TopScreen is GauntletInventoryScreen obj)
		{
			if (args.Count == 1)
			{
				if (int.TryParse(args[0], out var result2) && (result2 == 1 || result2 == 0))
				{
					FieldInfo field = typeof(GauntletInventoryScreen).GetField("_dataSource", BindingFlags.Instance | BindingFlags.NonPublic);
					SPInventoryVM sPInventoryVM = (SPInventoryVM)field.GetValue(obj);
					sPInventoryVM.IsSearchAvailable = result2 == 1;
					field.SetValue(obj, sPInventoryVM);
					return "Success";
				}
				return result;
			}
			return result;
		}
		return "Inventory screen is not open!";
	}

	[CommandLineFunctionality.CommandLineArgumentFunction("reload_pieces", "crafting")]
	public static string ReloadCraftingPieces(List<string> strings)
	{
		if (strings.Count != 2)
		{
			return "Usage: crafting.reload_pieces {MODULE_NAME} {XML_NAME}";
		}
		typeof(GauntletCraftingScreen).GetField("_reloadXmlPath", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, new KeyValuePair<string, string>(strings[0], strings[1]));
		return "Reloading crafting pieces...";
	}
}
