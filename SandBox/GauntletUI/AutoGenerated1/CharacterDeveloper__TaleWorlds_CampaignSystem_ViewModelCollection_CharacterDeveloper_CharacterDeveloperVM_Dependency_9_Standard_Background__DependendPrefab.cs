using System.ComponentModel;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;

namespace SandBox.GauntletUI.AutoGenerated1;

public class CharacterDeveloper__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterDeveloper_CharacterDeveloperVM_Dependency_9_Standard_Background__DependendPrefab : ScreenBackgroundBrushWidget
{
	private ScreenBackgroundBrushWidget _widget;

	private Widget _widget_0;

	private Widget _widget_1;

	private Widget _widget_2;

	private Widget _widget_3;

	private Widget _widget_4;

	private CharacterDeveloperVM _datasource_Root;

	public CharacterDeveloper__TaleWorlds_CampaignSystem_ViewModelCollection_CharacterDeveloper_CharacterDeveloperVM_Dependency_9_Standard_Background__DependendPrefab(UIContext context)
		: base(context)
	{
	}

	public void AddChildToLogicalLocation(Widget widget)
	{
		_widget_0.AddChild(widget);
	}

	public void CreateWidgets()
	{
		_widget = this;
		_widget_0 = new Widget(base.Context);
		_widget.AddChild(_widget_0);
		_widget_1 = new Widget(base.Context);
		_widget.AddChild(_widget_1);
		_widget_2 = new Widget(base.Context);
		_widget.AddChild(_widget_2);
		_widget_3 = new Widget(base.Context);
		_widget.AddChild(_widget_3);
		_widget_4 = new Widget(base.Context);
		_widget.AddChild(_widget_4);
	}

	public void SetIds()
	{
		_widget_1.Id = "SmokeWidget1";
		_widget_2.Id = "SmokeWidget2";
		_widget_3.Id = "ParticleWidget1";
		_widget_4.Id = "ParticleWidget2";
	}

	public void SetAttributes()
	{
		base.WidthSizePolicy = SizePolicy.StretchToParent;
		base.HeightSizePolicy = SizePolicy.StretchToParent;
		base.ParticleWidget1 = _widget_3;
		base.ParticleWidget2 = _widget_4;
		base.IsParticleVisible = true;
		base.IsSmokeVisible = true;
		base.SmokeWidget1 = _widget_1;
		base.SmokeWidget2 = _widget_2;
		base.AnimEnabled = true;
		base.SmokeSpeedModifier = 5f;
		base.ParticleSpeedModifier = 3f;
		base.IsFullscreenImageEnabled = true;
		base.Brush = base.Context.GetBrush("Standard.Background.Brush");
		base.DoNotUseCustomScaleAndChildren = true;
		_widget_0.WidthSizePolicy = SizePolicy.StretchToParent;
		_widget_0.HeightSizePolicy = SizePolicy.StretchToParent;
		_widget_1.WidthSizePolicy = SizePolicy.Fixed;
		_widget_1.HeightSizePolicy = SizePolicy.Fixed;
		_widget_1.SuggestedWidth = 1920f;
		_widget_1.SuggestedHeight = 700f;
		_widget_1.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_1.VerticalAlignment = VerticalAlignment.Bottom;
		_widget_1.Sprite = base.Context.SpriteData.GetSprite("fog_smoke");
		_widget_1.ColorFactor = 1.1f;
		_widget_2.WidthSizePolicy = SizePolicy.Fixed;
		_widget_2.HeightSizePolicy = SizePolicy.Fixed;
		_widget_2.SuggestedWidth = 1920f;
		_widget_2.SuggestedHeight = 700f;
		_widget_2.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_2.VerticalAlignment = VerticalAlignment.Bottom;
		_widget_2.Sprite = base.Context.SpriteData.GetSprite("fog_smoke");
		_widget_2.ColorFactor = 1.1f;
		_widget_3.WidthSizePolicy = SizePolicy.Fixed;
		_widget_3.HeightSizePolicy = SizePolicy.Fixed;
		_widget_3.SuggestedWidth = 1920f;
		_widget_3.SuggestedHeight = 1080f;
		_widget_3.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_3.VerticalAlignment = VerticalAlignment.Center;
		_widget_3.Sprite = base.Context.SpriteData.GetSprite("fog_particles");
		_widget_3.AlphaFactor = 0.8f;
		_widget_4.WidthSizePolicy = SizePolicy.Fixed;
		_widget_4.HeightSizePolicy = SizePolicy.Fixed;
		_widget_4.SuggestedWidth = 1920f;
		_widget_4.SuggestedHeight = 1080f;
		_widget_4.HorizontalAlignment = HorizontalAlignment.Center;
		_widget_4.VerticalAlignment = VerticalAlignment.Center;
		_widget_4.Sprite = base.Context.SpriteData.GetSprite("fog_particles");
		_widget_4.AlphaFactor = 0.8f;
	}

	public void DestroyDataSource()
	{
		if (_datasource_Root != null)
		{
			_datasource_Root.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
			_datasource_Root = null;
		}
	}

	public void SetDataSource(CharacterDeveloperVM dataSource)
	{
		RefreshDataSource_datasource_Root(dataSource);
	}

	private void ViewModelPropertyChangedListenerOf_datasource_Root(object sender, PropertyChangedEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithValueListenerOf_datasource_Root(object sender, PropertyChangedWithValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root(object sender, PropertyChangedWithBoolValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root(object sender, PropertyChangedWithIntValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root(object sender, PropertyChangedWithFloatValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root(object sender, PropertyChangedWithUIntValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root(object sender, PropertyChangedWithColorValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root(object sender, PropertyChangedWithDoubleValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root(object sender, PropertyChangedWithVec2ValueEventArgs e)
	{
		HandleViewModelPropertyChangeOf_datasource_Root(e.PropertyName);
	}

	private void HandleViewModelPropertyChangeOf_datasource_Root(string propertyName)
	{
	}

	private void RefreshDataSource_datasource_Root(CharacterDeveloperVM newDataSource)
	{
		if (_datasource_Root != null)
		{
			_datasource_Root.PropertyChanged -= ViewModelPropertyChangedListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithValue -= ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithBoolValue -= ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithIntValue -= ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithFloatValue -= ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithUIntValue -= ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithColorValue -= ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithDoubleValue -= ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithVec2Value -= ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
			_datasource_Root = null;
		}
		_datasource_Root = newDataSource;
		if (_datasource_Root != null)
		{
			_datasource_Root.PropertyChanged += ViewModelPropertyChangedListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithValue += ViewModelPropertyChangedWithValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithBoolValue += ViewModelPropertyChangedWithBoolValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithIntValue += ViewModelPropertyChangedWithIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithFloatValue += ViewModelPropertyChangedWithFloatValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithUIntValue += ViewModelPropertyChangedWithUIntValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithColorValue += ViewModelPropertyChangedWithColorValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithDoubleValue += ViewModelPropertyChangedWithDoubleValueListenerOf_datasource_Root;
			_datasource_Root.PropertyChangedWithVec2Value += ViewModelPropertyChangedWithVec2ValueListenerOf_datasource_Root;
		}
	}
}
