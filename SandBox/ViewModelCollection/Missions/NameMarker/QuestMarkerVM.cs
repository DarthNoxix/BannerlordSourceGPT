using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Missions.NameMarker;

public class QuestMarkerVM : ViewModel
{
	private int _questMarkerType;

	public SandBoxUIHelper.IssueQuestFlags IssueQuestFlag { get; }

	[DataSourceProperty]
	public int QuestMarkerType
	{
		get
		{
			return _questMarkerType;
		}
		set
		{
			if (value != _questMarkerType)
			{
				_questMarkerType = value;
				OnPropertyChangedWithValue(value, "QuestMarkerType");
			}
		}
	}

	public QuestMarkerVM(SandBoxUIHelper.IssueQuestFlags issueQuestFlag)
	{
		QuestMarkerType = (int)issueQuestFlag;
		IssueQuestFlag = issueQuestFlag;
	}
}
