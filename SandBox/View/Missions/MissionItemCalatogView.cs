using SandBox.Missions.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions;

public class MissionItemCalatogView : MissionView
{
	private ItemCatalogController _itemCatalogController;

	public override void AfterStart()
	{
		((MissionBehavior)this).AfterStart();
		_itemCatalogController = ((MissionBehavior)this).Mission.GetMissionBehavior<ItemCatalogController>();
		_itemCatalogController.BeforeCatalogTick += OnBeforeCatalogTick;
		_itemCatalogController.AfterCatalogTick += OnAfterCatalogTick;
	}

	private void OnBeforeCatalogTick(int currentItemIndex)
	{
		Utilities.TakeScreenshot(string.Concat("ItemCatalog/", _itemCatalogController.AllItems[currentItemIndex - 1].Name, ".bmp"));
	}

	private void OnAfterCatalogTick()
	{
		MatrixFrame cameraFrame = default(MatrixFrame);
		Vec3 lookDirection = ((MissionBehavior)this).Mission.MainAgent.LookDirection;
		cameraFrame.origin = ((MissionBehavior)this).Mission.MainAgent.Position + lookDirection * 2f + new Vec3(0f, 0f, 1.273f);
		cameraFrame.rotation.u = lookDirection;
		cameraFrame.rotation.s = new Vec3(1f);
		cameraFrame.rotation.f = new Vec3(0f, 0f, 1f);
		cameraFrame.rotation.Orthonormalize();
		((MissionBehavior)this).Mission.SetCameraFrame(ref cameraFrame, 1f);
		Camera camera = Camera.CreateCamera();
		camera.Frame = cameraFrame;
		((MissionView)this).MissionScreen.CustomCamera = camera;
	}
}
