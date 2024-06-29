using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection;

public class TournamentRewardVM : ViewModel
{
	private string _text;

	private ImageIdentifierVM _imageIdentifier;

	private bool _gotImageIdentifier;

	[DataSourceProperty]
	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			if (value != _text)
			{
				_text = value;
				OnPropertyChangedWithValue(value, "Text");
			}
		}
	}

	[DataSourceProperty]
	public bool GotImageIdentifier
	{
		get
		{
			return _gotImageIdentifier;
		}
		set
		{
			if (value != _gotImageIdentifier)
			{
				_gotImageIdentifier = value;
				OnPropertyChangedWithValue(value, "GotImageIdentifier");
			}
		}
	}

	[DataSourceProperty]
	public ImageIdentifierVM ImageIdentifier
	{
		get
		{
			return _imageIdentifier;
		}
		set
		{
			if (value != _imageIdentifier)
			{
				_imageIdentifier = value;
				OnPropertyChangedWithValue(value, "ImageIdentifier");
			}
		}
	}

	public TournamentRewardVM(string text)
	{
		Text = text;
		GotImageIdentifier = false;
		ImageIdentifier = new ImageIdentifierVM();
	}

	public TournamentRewardVM(string text, ImageIdentifierVM imageIdentifierVM)
	{
		Text = text;
		GotImageIdentifier = true;
		ImageIdentifier = imageIdentifierVM;
	}
}
