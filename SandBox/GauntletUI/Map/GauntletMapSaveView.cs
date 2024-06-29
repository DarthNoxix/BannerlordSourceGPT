using SandBox.View.Map;
using SandBox.ViewModelCollection.SaveLoad;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map;

[OverrideView(typeof(MapSaveView))]
public class GauntletMapSaveView : MapView
{
	private MapSaveVM _dataSource;

	protected override void CreateLayout()
	{
		base.CreateLayout();
		_dataSource = new MapSaveVM(OnStateChange);
		base.Layer = new GauntletLayer(10000);
		(base.Layer as GauntletLayer).LoadMovie("MapSave", _dataSource);
		base.Layer.InputRestrictions.SetInputRestrictions(isMouseVisible: false, InputUsageMask.MouseButtons | InputUsageMask.Keyboardkeys);
		base.MapScreen.AddLayer(base.Layer);
	}

	private void OnStateChange(bool isActive)
	{
		if (isActive)
		{
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(base.Layer);
			base.Layer.InputRestrictions.SetInputRestrictions(isMouseVisible: false);
		}
		else
		{
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			base.Layer.InputRestrictions.ResetInputRestrictions();
		}
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		_dataSource.OnFinalize();
		base.MapScreen.RemoveLayer(base.Layer);
		base.Layer = null;
		_dataSource = null;
	}
}
