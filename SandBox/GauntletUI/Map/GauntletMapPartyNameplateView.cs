using SandBox.View.Map;
using SandBox.ViewModelCollection.Nameplate;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map;

[OverrideView(typeof(MapPartyNameplateView))]
public class GauntletMapPartyNameplateView : MapView
{
	private GauntletLayer _layerAsGauntletLayer;

	private PartyNameplatesVM _dataSource;

	private IGauntletMovie _movie;

	protected override void CreateLayout()
	{
		base.CreateLayout();
		_dataSource = new PartyNameplatesVM(base.MapScreen._mapCameraView.Camera, base.MapScreen.FastMoveCameraToMainParty, IsShowPartyNamesEnabled);
		GauntletMapBasicView mapView = base.MapScreen.GetMapView<GauntletMapBasicView>();
		base.Layer = mapView.GauntletNameplateLayer;
		_layerAsGauntletLayer = base.Layer as GauntletLayer;
		_movie = _layerAsGauntletLayer.LoadMovie("PartyNameplate", _dataSource);
		_dataSource.Initialize();
	}

	protected override void OnMapScreenUpdate(float dt)
	{
		base.OnMapScreenUpdate(dt);
		_dataSource.Update();
	}

	protected override void OnResume()
	{
		base.OnResume();
		foreach (PartyNameplateVM nameplate in _dataSource.Nameplates)
		{
			nameplate.RefreshDynamicProperties(forceUpdate: true);
		}
	}

	protected override void OnFinalize()
	{
		_layerAsGauntletLayer.ReleaseMovie(_movie);
		_dataSource.OnFinalize();
		_layerAsGauntletLayer = null;
		base.Layer = null;
		_movie = null;
		_dataSource = null;
		base.OnFinalize();
	}

	private bool IsShowPartyNamesEnabled()
	{
		return base.MapScreen.SceneLayer.Input.IsGameKeyDown(5);
	}
}
