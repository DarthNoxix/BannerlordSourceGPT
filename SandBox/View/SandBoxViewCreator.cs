using System;
using System.Collections.Generic;
using System.Reflection;
using SandBox.View.Map;
using SandBox.View.Menu;
using SandBox.View.Missions;
using SandBox.View.Missions.Tournaments;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.ScreenSystem;

namespace SandBox.View;

public static class SandBoxViewCreator
{
	private static Dictionary<Type, Type> _actualViewTypes;

	static SandBoxViewCreator()
	{
		_actualViewTypes = new Dictionary<Type, Type>();
		Assembly[] viewAssemblies = GetViewAssemblies();
		_ = typeof(ViewCreatorModule).Assembly;
		Assembly[] array = viewAssemblies;
		for (int i = 0; i < array.Length; i++)
		{
			CheckOverridenViews(array[i]);
		}
	}

	private static void CheckOverridenViews(Assembly assembly)
	{
		foreach (Type item in assembly.GetTypesSafe())
		{
			if (!typeof(MapView).IsAssignableFrom(item) && !typeof(MenuView).IsAssignableFrom(item) && !typeof(MissionView).IsAssignableFrom(item) && !typeof(ScreenBase).IsAssignableFrom(item))
			{
				continue;
			}
			object[] customAttributesSafe = item.GetCustomAttributesSafe(typeof(OverrideView), inherit: false);
			if (customAttributesSafe == null || customAttributesSafe.Length != 1)
			{
				continue;
			}
			object obj = customAttributesSafe[0];
			OverrideView val = (OverrideView)((obj is OverrideView) ? obj : null);
			if (val != null)
			{
				if (!_actualViewTypes.ContainsKey(val.BaseType))
				{
					_actualViewTypes.Add(val.BaseType, item);
				}
				else
				{
					_actualViewTypes[val.BaseType] = item;
				}
			}
		}
	}

	private static Assembly[] GetViewAssemblies()
	{
		List<Assembly> list = new List<Assembly>();
		Assembly assembly = typeof(ViewCreatorModule).Assembly;
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly2 in assemblies)
		{
			AssemblyName[] referencedAssemblies = assembly2.GetReferencedAssemblies();
			for (int j = 0; j < referencedAssemblies.Length; j++)
			{
				if (referencedAssemblies[j].ToString() == assembly.GetName().ToString())
				{
					list.Add(assembly2);
					break;
				}
			}
		}
		return list.ToArray();
	}

	public static ScreenBase CreateSaveLoadScreen(bool isSaving)
	{
		return ViewCreatorManager.CreateScreenView<SaveLoadScreen>(new object[1] { isSaving });
	}

	public static MissionView CreateMissionCraftingView()
	{
		return null;
	}

	public static MissionView CreateMissionNameMarkerUIHandler(Mission mission = null)
	{
		return ViewCreatorManager.CreateMissionView<MissionNameMarkerUIHandler>(mission != null, mission, Array.Empty<object>());
	}

	public static MissionView CreateMissionConversationView(Mission mission)
	{
		return ViewCreatorManager.CreateMissionView<MissionConversationView>(true, mission, Array.Empty<object>());
	}

	public static MissionView CreateMissionBarterView()
	{
		return ViewCreatorManager.CreateMissionView<BarterView>(false, (Mission)null, Array.Empty<object>());
	}

	public static MissionView CreateMissionTournamentView()
	{
		return ViewCreatorManager.CreateMissionView<MissionTournamentView>(false, (Mission)null, Array.Empty<object>());
	}

	public static MapView CreateMapView<T>(params object[] parameters) where T : MapView
	{
		Type type = typeof(T);
		if (_actualViewTypes.ContainsKey(typeof(T)))
		{
			type = _actualViewTypes[typeof(T)];
		}
		return Activator.CreateInstance(type, parameters) as MapView;
	}

	public static MenuView CreateMenuView<T>(params object[] parameters) where T : MenuView
	{
		Type type = typeof(T);
		if (_actualViewTypes.ContainsKey(typeof(T)))
		{
			type = _actualViewTypes[typeof(T)];
		}
		return Activator.CreateInstance(type, parameters) as MenuView;
	}

	public static MissionView CreateBoardGameView()
	{
		return ViewCreatorManager.CreateMissionView<BoardGameView>(false, (Mission)null, Array.Empty<object>());
	}

	public static MissionView CreateMissionAmbushDeploymentView()
	{
		return ViewCreatorManager.CreateMissionView<MissionAmbushDeploymentView>(false, (Mission)null, Array.Empty<object>());
	}

	public static MissionView CreateMissionArenaPracticeFightView()
	{
		return ViewCreatorManager.CreateMissionView<MissionArenaPracticeFightView>(false, (Mission)null, Array.Empty<object>());
	}
}
