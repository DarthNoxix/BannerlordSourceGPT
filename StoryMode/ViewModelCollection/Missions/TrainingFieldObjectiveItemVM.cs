using StoryMode.Missions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.ViewModelCollection.Missions;

public class TrainingFieldObjectiveItemVM : ViewModel
{
	private string _textObjectString;

	private string _objectiveText;

	private bool _isCompleted;

	private bool _isActive;

	private float _score;

	private MBBindingList<TrainingFieldObjectiveItemVM> _objectiveItems;

	[DataSourceProperty]
	public string ObjectiveText
	{
		get
		{
			return _objectiveText;
		}
		set
		{
			if (value != _objectiveText)
			{
				_objectiveText = value;
				OnPropertyChangedWithValue(value, "ObjectiveText");
			}
		}
	}

	[DataSourceProperty]
	public bool IsCompleted
	{
		get
		{
			return _isCompleted;
		}
		set
		{
			if (value != _isCompleted)
			{
				_isCompleted = value;
				OnPropertyChangedWithValue(value, "IsCompleted");
			}
		}
	}

	[DataSourceProperty]
	public bool IsActive
	{
		get
		{
			return _isActive;
		}
		set
		{
			if (value != _isActive)
			{
				_isActive = value;
				OnPropertyChangedWithValue(value, "IsActive");
			}
		}
	}

	[DataSourceProperty]
	public MBBindingList<TrainingFieldObjectiveItemVM> ObjectiveItems
	{
		get
		{
			return _objectiveItems;
		}
		set
		{
			if (value != _objectiveItems)
			{
				_objectiveItems = value;
				OnPropertyChangedWithValue(value, "ObjectiveItems");
			}
		}
	}

	private TrainingFieldObjectiveItemVM()
	{
	}

	private TrainingFieldObjectiveItemVM(TrainingFieldMissionController.TutorialObjective objective)
	{
		_textObjectString = objective.GetNameString();
		IsActive = objective.IsActive;
		IsCompleted = objective.IsFinished;
		_score = objective.Score;
		ObjectiveItems = new MBBindingList<TrainingFieldObjectiveItemVM>();
		if (objective.SubTasks != null)
		{
			foreach (TrainingFieldMissionController.TutorialObjective subTask in objective.SubTasks)
			{
				ObjectiveItems.Add(CreateFromObjective(subTask));
			}
		}
		RefreshValues();
	}

	public override void RefreshValues()
	{
		base.RefreshValues();
		if (_textObjectString != "")
		{
			ObjectiveText = _textObjectString;
			if (_score != 0f)
			{
				TextObject textObject = GameTexts.FindText("str_tutorial_time_score");
				textObject.SetTextVariable("TIME_SCORE", _score.ToString("0.0"));
				ObjectiveText += textObject.ToString();
			}
		}
	}

	public static TrainingFieldObjectiveItemVM CreateFromObjective(TrainingFieldMissionController.TutorialObjective objective)
	{
		return new TrainingFieldObjectiveItemVM(objective);
	}
}
