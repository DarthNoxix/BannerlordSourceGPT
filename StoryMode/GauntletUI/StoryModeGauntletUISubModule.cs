using SandBox.View.Map;
using StoryMode.GauntletUI.Permissions;
using StoryMode.GauntletUI.Tutorial;
using StoryMode.ViewModelCollection.Map;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace StoryMode.GauntletUI;

public class StoryModeGauntletUISubModule : MBSubModuleBase
{
	private bool _registered;

	public override void OnGameInitializationFinished(Game game)
	{
		base.OnGameInitializationFinished(game);
		if (game.GameType.RequiresTutorial)
		{
			GauntletTutorialSystem.OnInitialize();
			StoryModePermissionsSystem.OnInitialize();
			ScreenManager.OnPushScreen += OnScreenManagerPushScreen;
		}
	}

	private void OnScreenManagerPushScreen(ScreenBase pushedScreen)
	{
		MapScreen val;
		if (!_registered && (val = (MapScreen)(object)((pushedScreen is MapScreen) ? pushedScreen : null)) != null)
		{
			val.MapNotificationView.RegisterMapNotificationType(typeof(ConspiracyQuestMapNotification), typeof(ConspiracyQuestMapNotificationItemVM));
			_registered = true;
		}
	}

	public override void OnGameEnd(Game game)
	{
		base.OnGameEnd(game);
		if (game.GameType.RequiresTutorial)
		{
			GauntletTutorialSystem.OnUnload();
			StoryModePermissionsSystem.OnUnload();
			ScreenManager.OnPushScreen -= OnScreenManagerPushScreen;
		}
		_registered = false;
	}
}
