using SandBox.View.Map;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map;

[OverrideView(typeof(MapReadyView))]
public class GauntletMapReadyView : MapReadyView
{
	private BoolItemWithActionVM _dataSource;

	protected override void CreateLayout()
	{
		base.CreateLayout();
		_dataSource = new BoolItemWithActionVM(null, isActive: true, null);
		base.Layer = new GauntletLayer(9999);
		(base.Layer as GauntletLayer).LoadMovie("MapReadyBlocker", _dataSource);
		base.MapScreen.AddLayer(base.Layer);
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		_dataSource.OnFinalize();
		base.MapScreen.RemoveLayer(base.Layer);
		base.Layer = null;
		_dataSource = null;
	}

	public override void SetIsMapSceneReady(bool isReady)
	{
		base.SetIsMapSceneReady(isReady);
		_dataSource.IsActive = !isReady;
	}
}
