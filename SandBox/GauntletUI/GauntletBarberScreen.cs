using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI;

[GameStateScreen(typeof(BarberState))]
public class GauntletBarberScreen : ScreenBase, IGameStateListener, IFaceGeneratorScreen
{
	private readonly BodyGeneratorView _facegenLayer;

	public IFaceGeneratorHandler Handler => (IFaceGeneratorHandler)_facegenLayer;

	public GauntletBarberScreen(BarberState state)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		LoadingWindow.EnableGlobalLoadingWindow();
		_facegenLayer = new BodyGeneratorView((ControlCharacterCreationStage)OnExit, GameTexts.FindText("str_done"), (ControlCharacterCreationStage)OnExit, GameTexts.FindText("str_cancel"), (BasicCharacterObject)Hero.MainHero.CharacterObject, false, state.Filter, (Equipment)null, (ControlCharacterCreationStageReturnInt)null, (ControlCharacterCreationStageReturnInt)null, (ControlCharacterCreationStageReturnInt)null, (ControlCharacterCreationStageWithInt)null);
	}

	protected override void OnFrameTick(float dt)
	{
		base.OnFrameTick(dt);
		_facegenLayer.OnTick(dt);
	}

	public void OnExit()
	{
		Game.Current.GameStateManager.PopState();
	}

	protected override void OnInitialize()
	{
		base.OnInitialize();
		Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
		AddLayer(_facegenLayer.GauntletLayer);
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		if (LoadingWindow.GetGlobalLoadingWindowState())
		{
			LoadingWindow.DisableGlobalLoadingWindow();
		}
		Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		AddLayer(_facegenLayer.SceneLayer);
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		_facegenLayer.SceneLayer.SceneView.SetEnable(value: false);
		_facegenLayer.OnFinalize();
		LoadingWindow.EnableGlobalLoadingWindow();
		MBInformationManager.HideInformations();
	}

	void IGameStateListener.OnActivate()
	{
	}

	void IGameStateListener.OnDeactivate()
	{
	}

	void IGameStateListener.OnInitialize()
	{
	}

	void IGameStateListener.OnFinalize()
	{
	}
}
