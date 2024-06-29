using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace StoryMode.GameComponents;

public class StoryModeCutsceneSelectionModel : DefaultCutsceneSelectionModel
{
	public override SceneNotificationData GetKingdomDestroyedSceneNotification(Kingdom kingdom)
	{
		if (StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom == kingdom)
		{
			return new SupportedFactionDefeatedSceneNotificationItem(kingdom, StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine);
		}
		return new KingdomDestroyedSceneNotificationItem(kingdom, CampaignTime.Now);
	}
}
