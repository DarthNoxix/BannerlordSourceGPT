using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace StoryMode;

public class CampaignStoryMode : Campaign
{
	[SaveableProperty(9999)]
	public StoryModeManager StoryMode { get; private set; }

	public CampaignStoryMode(CampaignGameMode gameMode)
		: base(gameMode)
	{
		StoryMode = new StoryModeManager();
	}

	protected override void BeforeRegisterTypes(MBObjectManager objectManager)
	{
		base.BeforeRegisterTypes(objectManager);
		objectManager.RegisterType<TrainingField>("TrainingField", "TrainingFields", 1u);
	}

	protected override void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState)
	{
		base.DoLoadingForGameType(gameTypeLoadingState, out nextState);
		if (gameTypeLoadingState == GameTypeLoadingStates.InitializeFirstStep && nextState != gameTypeLoadingState)
		{
			StoryMode.InitializeStoryModeObjects();
		}
		if (gameTypeLoadingState == GameTypeLoadingStates.LoadVisualsThirdState && nextState != gameTypeLoadingState)
		{
			Settlement settlement = Settlement.Find("tutorial_training_field");
			if (settlement != null)
			{
				base.MapSceneWrapper.AddNewEntityToMapScene(settlement.StringId, settlement.Position2D);
			}
		}
		if (gameTypeLoadingState == GameTypeLoadingStates.PostInitializeFourthState)
		{
			OnSessionLaunched();
		}
	}

	private void OnSessionLaunched()
	{
		StoryMode.MainStoryLine.OnSessionLaunched();
	}

	internal static void AutoGeneratedStaticCollectObjectsCampaignStoryMode(object o, List<object> collectedObjects)
	{
		((CampaignStoryMode)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		collectedObjects.Add(StoryMode);
	}

	internal static object AutoGeneratedGetMemberValueStoryMode(object o)
	{
		return ((CampaignStoryMode)o).StoryMode;
	}
}