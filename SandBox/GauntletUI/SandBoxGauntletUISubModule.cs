using SandBox.GauntletUI.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.GauntletUI.SceneNotification;

namespace SandBox.GauntletUI;

public class SandBoxGauntletUISubModule : MBSubModuleBase
{
	private bool _gameStarted;

	private bool _initialized;

	public override void OnCampaignStart(Game game, object starterObject)
	{
		base.OnCampaignStart(game, starterObject);
		if (!_gameStarted && game.GameType is Campaign)
		{
			_gameStarted = true;
		}
	}

	public override void OnGameEnd(Game game)
	{
		base.OnGameEnd(game);
		if (_gameStarted && game.GameType is Campaign)
		{
			_gameStarted = false;
			GauntletGameNotification.OnFinalize();
		}
	}

	public override void BeginGameStart(Game game)
	{
		base.BeginGameStart(game);
		if (Campaign.Current != null)
		{
			Campaign.Current.VisualCreator.MapEventVisualCreator = new GauntletMapEventVisualCreator();
		}
	}

	protected override void OnBeforeInitialModuleScreenSetAsRoot()
	{
		base.OnBeforeInitialModuleScreenSetAsRoot();
		if (!_initialized)
		{
			if (!Utilities.CommandLineArgumentExists("VisualTests"))
			{
				GauntletSceneNotification.Current.RegisterContextProvider((ISceneNotificationContextProvider)new SandboxSceneNotificationContextProvider());
			}
			_initialized = true;
		}
	}

	protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
	{
		base.OnGameStart(game, gameStarterObject);
		if (!_gameStarted && game.GameType is Campaign)
		{
			_gameStarted = true;
		}
	}
}
