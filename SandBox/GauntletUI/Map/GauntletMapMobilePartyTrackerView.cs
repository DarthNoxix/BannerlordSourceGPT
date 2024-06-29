using SandBox.View.Map;
using SandBox.ViewModelCollection.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map;

[OverrideView(typeof(MapMobilePartyTrackerView))]
public class GauntletMapMobilePartyTrackerView : MapView
{
	private GauntletLayer _layerAsGauntletLayer;

	private IGauntletMovie _movie;

	private MapMobilePartyTrackerVM _dataSource;

	protected override void CreateLayout()
	{
		base.CreateLayout();
		_dataSource = new MapMobilePartyTrackerVM(base.MapScreen._mapCameraView.Camera, base.MapScreen.FastMoveCameraToPosition);
		GauntletMapBasicView mapView = base.MapScreen.GetMapView<GauntletMapBasicView>();
		base.Layer = mapView.GauntletNameplateLayer;
		_layerAsGauntletLayer = base.Layer as GauntletLayer;
		_movie = _layerAsGauntletLayer.LoadMovie("MapMobilePartyTracker", _dataSource);
	}

	protected override void OnResume()
	{
		base.OnResume();
		_dataSource.UpdateProperties();
	}

	protected override void OnMapScreenUpdate(float dt)
	{
		base.OnMapScreenUpdate(dt);
		_dataSource.Update();
	}

	protected override void OnFinalize()
	{
		_dataSource.OnFinalize();
		_layerAsGauntletLayer.ReleaseMovie(_movie);
		_layerAsGauntletLayer = null;
		base.Layer = null;
		_movie = null;
		_dataSource = null;
		base.OnFinalize();
	}
}
