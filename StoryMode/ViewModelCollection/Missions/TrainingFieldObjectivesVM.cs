using System.Collections.Generic;
using StoryMode.Missions;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.ViewModelCollection.Missions;

public class TrainingFieldObjectivesVM : ViewModel
{
	private TextObject _objectiveText;

	private MBBindingList<TrainingFieldObjectiveItemVM> _objectiveItems;

	private string _currentObjectiveText;

	private string _leaveAnyTimeText;

	private string _timerText;

	private string _rightStickAbbreviatedText;

	private int _currentMouseObjective;

	private bool _isGamepadActive;

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

	[DataSourceProperty]
	public string CurrentObjectiveText
	{
		get
		{
			return _currentObjectiveText;
		}
		set
		{
			if (value != _currentObjectiveText)
			{
				_currentObjectiveText = value;
				OnPropertyChangedWithValue(value, "CurrentObjectiveText");
			}
		}
	}

	[DataSourceProperty]
	public string TimerText
	{
		get
		{
			return _timerText;
		}
		set
		{
			if (value != _timerText)
			{
				_timerText = value;
				OnPropertyChangedWithValue(value, "TimerText");
			}
		}
	}

	[DataSourceProperty]
	public string LeaveAnyTimeText
	{
		get
		{
			return _leaveAnyTimeText;
		}
		set
		{
			if (value != _leaveAnyTimeText)
			{
				_leaveAnyTimeText = value;
				OnPropertyChangedWithValue(value, "LeaveAnyTimeText");
			}
		}
	}

	[DataSourceProperty]
	public int CurrentMouseObjective
	{
		get
		{
			return _currentMouseObjective;
		}
		set
		{
			if (value != _currentMouseObjective)
			{
				_currentMouseObjective = value;
				OnPropertyChangedWithValue(value, "CurrentMouseObjective");
			}
		}
	}

	[DataSourceProperty]
	public string RightStickAbbreviatedText
	{
		get
		{
			return _rightStickAbbreviatedText;
		}
		set
		{
			if (value != _rightStickAbbreviatedText)
			{
				_rightStickAbbreviatedText = value;
				OnPropertyChangedWithValue(value, "RightStickAbbreviatedText");
			}
		}
	}

	[DataSourceProperty]
	public bool IsGamepadActive
	{
		get
		{
			return _isGamepadActive;
		}
		set
		{
			if (value != _isGamepadActive)
			{
				_isGamepadActive = value;
				OnPropertyChangedWithValue(value, "IsGamepadActive");
			}
		}
	}

	public TrainingFieldObjectivesVM()
	{
		ObjectiveItems = new MBBindingList<TrainingFieldObjectiveItemVM>();
		RefreshValues();
		UpdateIsGamepadActive();
	}

	public override void RefreshValues()
	{
		base.RefreshValues();
		CurrentObjectiveText = ((_objectiveText == null) ? "" : _objectiveText.ToString());
		string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4));
		GameTexts.SetVariable("LEAVE_KEY", keyHyperlinkText);
		GameTexts.SetVariable("newline", "\n");
		LeaveAnyTimeText = GameTexts.FindText("str_leave_training_field").ToString();
		RightStickAbbreviatedText = new TextObject("{=rightstickabbreviated}RS").ToString();
		ObjectiveItems.ApplyActionOnAllItems(delegate(TrainingFieldObjectiveItemVM o)
		{
			o.RefreshValues();
		});
	}

	public void UpdateObjectivesWith(List<TrainingFieldMissionController.TutorialObjective> objectives)
	{
		ObjectiveItems.Clear();
		foreach (TrainingFieldMissionController.TutorialObjective objective in objectives)
		{
			ObjectiveItems.Add(TrainingFieldObjectiveItemVM.CreateFromObjective(objective));
		}
	}

	public void UpdateCurrentObjectiveText(TextObject currentObjectiveText)
	{
		_objectiveText = currentObjectiveText;
		CurrentObjectiveText = ((_objectiveText == null) ? "" : _objectiveText.ToString());
	}

	public void UpdateCurrentMouseObjective(TrainingFieldMissionController.MouseObjectives currentMouseObjective)
	{
		CurrentMouseObjective = (int)currentMouseObjective;
	}

	public void UpdateTimerText(string timerText)
	{
		TimerText = (string.IsNullOrEmpty(timerText) ? "" : timerText);
	}

	public void UpdateIsGamepadActive()
	{
		IsGamepadActive = Input.IsGamepadActive;
	}
}
