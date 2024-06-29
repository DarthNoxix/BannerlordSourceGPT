using SandBox.View.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map;

[OverrideView(typeof(MapBasicView))]
public class GauntletMapBasicView : MapView
{
	public GauntletLayer GauntletLayer { get; private set; }

	public GauntletLayer GauntletNameplateLayer { get; private set; }

	protected override void CreateLayout()
	{
		base.CreateLayout();
		GauntletLayer = new GauntletLayer(100);
		GauntletLayer.InputRestrictions.SetInputRestrictions(isMouseVisible: false);
		GauntletLayer.Name = "BasicLayer";
		base.MapScreen.AddLayer(GauntletLayer);
		GauntletNameplateLayer = new GauntletLayer(90);
		GauntletNameplateLayer.InputRestrictions.SetInputRestrictions(isMouseVisible: false, InputUsageMask.MouseButtons | InputUsageMask.Keyboardkeys);
		base.MapScreen.AddLayer(GauntletNameplateLayer);
	}

	protected override void OnMapConversationStart()
	{
		base.OnMapConversationStart();
		GauntletLayer.TwoDimensionView.SetEnable(value: false);
		GauntletNameplateLayer.TwoDimensionView.SetEnable(value: false);
	}

	protected override void OnMapConversationOver()
	{
		base.OnMapConversationOver();
		GauntletLayer.TwoDimensionView.SetEnable(value: true);
		GauntletNameplateLayer.TwoDimensionView.SetEnable(value: true);
	}

	protected override void OnFinalize()
	{
		base.MapScreen.RemoveLayer(GauntletLayer);
		GauntletLayer = null;
		base.OnFinalize();
	}
}
