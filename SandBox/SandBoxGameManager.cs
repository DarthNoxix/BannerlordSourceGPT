using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem.Load;

namespace SandBox;

public class SandBoxGameManager : MBGameManager
{
	private bool _loadingSavedGame;

	private LoadResult _loadedGameResult;

	public SandBoxGameManager()
	{
		_loadingSavedGame = false;
	}

	public SandBoxGameManager(LoadResult loadedGameResult)
	{
		_loadingSavedGame = true;
		_loadedGameResult = loadedGameResult;
	}

	public override void OnGameEnd(Game game)
	{
		MBDebug.SetErrorReportScene(null);
		base.OnGameEnd(game);
	}

	protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
	{
		nextStep = GameManagerLoadingSteps.None;
		switch (gameManagerLoadingStep)
		{
		case GameManagerLoadingSteps.PreInitializeZerothStep:
			nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
			break;
		case GameManagerLoadingSteps.FirstInitializeFirstStep:
			MBGameManager.LoadModuleData(_loadingSavedGame);
			nextStep = GameManagerLoadingSteps.WaitSecondStep;
			break;
		case GameManagerLoadingSteps.WaitSecondStep:
			if (!_loadingSavedGame)
			{
				MBGameManager.StartNewGame();
			}
			nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
			break;
		case GameManagerLoadingSteps.SecondInitializeThirdState:
			MBGlobals.InitializeReferences();
			if (!_loadingSavedGame)
			{
				MBDebug.Print("Initializing new game begin...");
				Campaign campaign = new Campaign(CampaignGameMode.Campaign);
				Game.CreateGame(campaign, this);
				campaign.SetLoadingParameters(Campaign.GameLoadingType.NewCampaign);
				MBDebug.Print("Initializing new game end...");
			}
			else
			{
				MBDebug.Print("Initializing saved game begin...");
				((Campaign)Game.LoadSaveGame(_loadedGameResult, this).GameType).SetLoadingParameters(Campaign.GameLoadingType.SavedCampaign);
				_loadedGameResult = null;
				Common.MemoryCleanupGC();
				MBDebug.Print("Initializing saved game end...");
			}
			Game.Current.DoLoading();
			nextStep = GameManagerLoadingSteps.PostInitializeFourthState;
			break;
		case GameManagerLoadingSteps.PostInitializeFourthState:
		{
			bool flag = true;
			foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
			{
				flag = flag && subModule.DoLoading(Game.Current);
			}
			nextStep = (flag ? GameManagerLoadingSteps.FinishLoadingFifthStep : GameManagerLoadingSteps.PostInitializeFourthState);
			break;
		}
		case GameManagerLoadingSteps.FinishLoadingFifthStep:
			nextStep = (Game.Current.DoLoading() ? GameManagerLoadingSteps.None : GameManagerLoadingSteps.FinishLoadingFifthStep);
			break;
		}
	}

	public override void OnAfterCampaignStart(Game game)
	{
	}

	public override void OnLoadFinished()
	{
		if (!_loadingSavedGame)
		{
			MBDebug.Print("Switching to menu window...");
			if (!Game.Current.IsDevelopmentMode)
			{
				VideoPlaybackState videoPlaybackState = Game.Current.GameStateManager.CreateState<VideoPlaybackState>();
				string text = ModuleHelper.GetModuleFullPath("SandBox") + "Videos/CampaignIntro/";
				string subtitleFileBasePath = text + "campaign_intro";
				string videoPath = text + "campaign_intro.ivf";
				string audioPath = text + "campaign_intro.ogg";
				videoPlaybackState.SetStartingParameters(videoPath, audioPath, subtitleFileBasePath);
				videoPlaybackState.SetOnVideoFinisedDelegate(LaunchSandboxCharacterCreation);
				Game.Current.GameStateManager.CleanAndPushState(videoPlaybackState);
			}
			else
			{
				LaunchSandboxCharacterCreation();
			}
		}
		else
		{
			Game.Current.GameStateManager.OnSavedGameLoadFinished();
			Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<MapState>());
			MapState mapState = Game.Current.GameStateManager.ActiveState as MapState;
			string text2 = mapState?.GameMenuId;
			if (!string.IsNullOrEmpty(text2))
			{
				PlayerEncounter.Current?.OnLoad();
				Campaign.Current.GameMenuManager.SetNextMenu(text2);
			}
			PartyBase.MainParty.SetVisualAsDirty();
			Campaign.Current.CampaignInformationManager.OnGameLoaded();
			foreach (Settlement item in Settlement.All)
			{
				item.Party.SetLevelMaskIsDirty();
			}
			CampaignEventDispatcher.Instance.OnGameLoadFinished();
			mapState?.OnLoadingFinished();
		}
		base.IsLoaded = true;
	}

	private void LaunchSandboxCharacterCreation()
	{
		CharacterCreationState gameState = Game.Current.GameStateManager.CreateState<CharacterCreationState>(new object[1]
		{
			new SandboxCharacterCreationContent()
		});
		Game.Current.GameStateManager.CleanAndPushState(gameState);
	}

	[CrashInformationCollector.CrashInformationProvider]
	private static CrashInformationCollector.CrashInformation UsedModuleInfoCrashCallback()
	{
		if (Campaign.Current?.PreviouslyUsedModules != null)
		{
			string[] moduleNames = SandBoxManager.Instance.ModuleManager.ModuleNames;
			MBList<(string, string)> mBList = new MBList<(string, string)>();
			foreach (string module in Campaign.Current.PreviouslyUsedModules)
			{
				bool flag = moduleNames.FindIndex((string x) => x == module) != -1;
				mBList.Add((module, flag ? "1" : "0"));
			}
			return new CrashInformationCollector.CrashInformation("Used Mods", mBList);
		}
		return null;
	}

	[CrashInformationCollector.CrashInformationProvider]
	private static CrashInformationCollector.CrashInformation UsedGameVersionsCallback()
	{
		if (Campaign.Current?.UsedGameVersions != null)
		{
			MBList<(string, string)> mBList = new MBList<(string, string)>();
			for (int i = 0; i < Campaign.Current.UsedGameVersions.Count; i++)
			{
				string item = "";
				if (i < Campaign.Current.UsedGameVersions.Count - 1 && ApplicationVersion.FromString(Campaign.Current.UsedGameVersions[i]) > ApplicationVersion.FromString(Campaign.Current.UsedGameVersions[i + 1]))
				{
					item = "Error";
				}
				mBList.Add((Campaign.Current.UsedGameVersions[i], item));
			}
			return new CrashInformationCollector.CrashInformation("Used Game Versions", mBList);
		}
		return null;
	}
}
