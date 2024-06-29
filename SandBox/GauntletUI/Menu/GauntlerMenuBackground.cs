using SandBox.View.Menu;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Menu;

[OverrideView(typeof(MenuBackgroundView))]
public class GauntlerMenuBackground : MenuView
{
	private IGauntletMovie _movie;

	protected override void OnInitialize()
	{
		base.OnInitialize();
		base.Layer = base.MenuViewContext.FindLayer<GauntletLayer>("BasicLayer");
		if (base.Layer == null)
		{
			base.Layer = new GauntletLayer(100)
			{
				Name = "BasicLayer"
			};
			base.MenuViewContext.AddLayer(base.Layer);
		}
		GauntletLayer gauntletLayer = base.Layer as GauntletLayer;
		_movie = gauntletLayer.LoadMovie("GameMenuBackground", null);
		base.Layer.InputRestrictions.SetInputRestrictions();
	}

	protected override void OnFinalize()
	{
		(base.Layer as GauntletLayer)?.ReleaseMovie(_movie);
		base.Layer = null;
		_movie = null;
		base.OnFinalize();
	}
}
