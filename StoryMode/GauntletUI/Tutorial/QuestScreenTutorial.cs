using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace StoryMode.GauntletUI.Tutorial;

public class QuestScreenTutorial : TutorialItemBase
{
	private bool _contextChangedToQuestsScreen;

	public QuestScreenTutorial()
	{
		base.Type = "GetQuestTutorial";
		base.Placement = TutorialItemVM.ItemPlacements.Right;
		base.HighlightedVisualElementID = "QuestsButton";
		base.MouseRequired = true;
	}

	public override TutorialContexts GetTutorialsRelevantContext()
	{
		return TutorialContexts.MapWindow;
	}

	public override bool IsConditionsMetForActivation()
	{
		if (Mission.Current == null)
		{
			return TutorialHelper.CurrentContext == TutorialContexts.QuestsScreen;
		}
		return false;
	}

	public override bool IsConditionsMetForCompletion()
	{
		return _contextChangedToQuestsScreen;
	}

	public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
	{
		_contextChangedToQuestsScreen = obj.NewContext == TutorialContexts.QuestsScreen;
	}
}
