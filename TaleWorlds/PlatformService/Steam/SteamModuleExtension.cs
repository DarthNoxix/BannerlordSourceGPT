using System.Collections.Generic;
using Steamworks;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.PlatformService.Steam;

public class SteamModuleExtension : IPlatformModuleExtension
{
	private List<string> _modulePaths;

	public SteamModuleExtension()
	{
		_modulePaths = new List<string>();
	}

	public void Initialize()
	{
		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
		PublishedFileId_t[] array = null;
		if (numSubscribedItems == 0)
		{
			return;
		}
		array = new PublishedFileId_t[numSubscribedItems];
		SteamUGC.GetSubscribedItems(array, numSubscribedItems);
		for (int i = 0; i < numSubscribedItems; i++)
		{
			if (SteamUGC.GetItemInstallInfo(array[i], out var _, out var pchFolder, 4096u, out var _))
			{
				_modulePaths.Add(pchFolder);
			}
		}
	}

	public string[] GetModulePaths()
	{
		return _modulePaths.ToArray();
	}

	public void Destroy()
	{
		_modulePaths.Clear();
	}

	public void SetLauncherMode(bool isLauncherModeActive)
	{
		SteamUtils.SetGameLauncherMode(isLauncherModeActive);
	}
}
