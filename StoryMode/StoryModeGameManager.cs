using SandBox;
using StoryMode.CharacterCreationContent;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;

namespace StoryMode;

public class StoryModeGameManager : SandBoxGameManager
{
	protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
	{
		if (gameManagerLoadingStep != GameManagerLoadingSteps.SecondInitializeThirdState)
		{
			((SandBoxGameManager)this).DoLoadingForGameManager(gameManagerLoadingStep, ref nextStep);
			return;
		}
		MBGlobals.InitializeReferences();
		MBDebug.Print("Initializing new game begin...");
		CampaignStoryMode campaignStoryMode = new CampaignStoryMode(CampaignGameMode.Campaign);
		Game.CreateGame(campaignStoryMode, (GameManagerBase)(object)this);
		campaignStoryMode.SetLoadingParameters(Campaign.GameLoadingType.NewCampaign);
		MBDebug.Print("Initializing new game end...");
		Game.Current.DoLoading();
		nextStep = GameManagerLoadingSteps.PostInitializeFourthState;
	}

	public override void OnLoadFinished()
	{
		VideoPlaybackState videoPlaybackState = Game.Current.GameStateManager.CreateState<VideoPlaybackState>();
		string text = ModuleHelper.GetModuleFullPath("SandBox") + "Videos/CampaignIntro/";
		string subtitleFileBasePath = text + "campaign_intro";
		string videoPath = text + "campaign_intro.ivf";
		string audioPath = text + "campaign_intro.ogg";
		videoPlaybackState.SetStartingParameters(videoPath, audioPath, subtitleFileBasePath);
		videoPlaybackState.SetOnVideoFinisedDelegate(LaunchStoryModeCharacterCreation);
		Game.Current.GameStateManager.CleanAndPushState(videoPlaybackState);
		((MBGameManager)this).IsLoaded = true;
	}

	private void LaunchStoryModeCharacterCreation()
	{
		CharacterCreationState gameState = Game.Current.GameStateManager.CreateState<CharacterCreationState>(new object[1]
		{
			new StoryModeCharacterCreationContent()
		});
		Game.Current.GameStateManager.CleanAndPushState(gameState);
	}
}
