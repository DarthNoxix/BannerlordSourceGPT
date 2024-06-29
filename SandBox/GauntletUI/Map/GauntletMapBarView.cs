using SandBox.View.Map;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map;

[OverrideView(typeof(MapBarView))]
public class GauntletMapBarView : MapView
{
	private GauntletMapBarGlobalLayer _gauntletMapBarGlobalLayer;

	protected override void CreateLayout()
	{
		base.CreateLayout();
		_gauntletMapBarGlobalLayer = new GauntletMapBarGlobalLayer();
		_gauntletMapBarGlobalLayer.Initialize(base.MapScreen, 8.5f);
		ScreenManager.AddGlobalLayer(_gauntletMapBarGlobalLayer, isFocusable: true);
	}

	protected override void OnFinalize()
	{
		_gauntletMapBarGlobalLayer.OnFinalize();
		ScreenManager.RemoveGlobalLayer(_gauntletMapBarGlobalLayer);
		base.OnFinalize();
	}

	protected override void OnResume()
	{
		base.OnResume();
		_gauntletMapBarGlobalLayer.Refresh();
	}

	protected override bool IsEscaped()
	{
		return _gauntletMapBarGlobalLayer.IsEscaped();
	}

	protected override void OnMapConversationStart()
	{
		_gauntletMapBarGlobalLayer.OnMapConversationStart();
	}

	protected override void OnMapConversationOver()
	{
		_gauntletMapBarGlobalLayer.OnMapConversationEnd();
	}
}
