using System.Collections.Generic;
using SandBox.View.CharacterCreation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.CharacterCreation;

[CharacterCreationStageView(typeof(CharacterCreationFaceGeneratorStage))]
public class CharacterCreationFaceGeneratorView : CharacterCreationStageViewBase
{
	private BodyGeneratorView _faceGeneratorView;

	private readonly TaleWorlds.CampaignSystem.CharacterCreationContent.CharacterCreation _characterCreation;

	private EscapeMenuVM _escapeMenuDatasource;

	private IGauntletMovie _escapeMenuMovie;

	public CharacterCreationFaceGeneratorView(TaleWorlds.CampaignSystem.CharacterCreationContent.CharacterCreation characterCreation, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage onRefresh, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction, ControlCharacterCreationStageReturnInt getTotalStageCountAction, ControlCharacterCreationStageReturnInt getFurthestIndexAction, ControlCharacterCreationStageWithInt goToIndexAction)
		: base(affirmativeAction, negativeAction, onRefresh, getTotalStageCountAction, getCurrentStageIndexAction, getFurthestIndexAction, goToIndexAction)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		_characterCreation = characterCreation;
		Equipment equipment = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>("player_char_creation_show_" + CharacterObject.PlayerCharacter?.Culture?.StringId)?.DefaultEquipment;
		_faceGeneratorView = new BodyGeneratorView((ControlCharacterCreationStage)NextStage, affirmativeActionText, (ControlCharacterCreationStage)PreviousStage, negativeActionText, (BasicCharacterObject)Hero.MainHero.CharacterObject, false, (IFaceGeneratorCustomFilter)null, equipment, getCurrentStageIndexAction, getTotalStageCountAction, getFurthestIndexAction, goToIndexAction);
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		_faceGeneratorView.OnFinalize();
		_faceGeneratorView = null;
	}

	public override IEnumerable<ScreenLayer> GetLayers()
	{
		return new List<ScreenLayer> { _faceGeneratorView.SceneLayer, _faceGeneratorView.GauntletLayer };
	}

	public override void PreviousStage()
	{
		_negativeAction();
	}

	public override void NextStage()
	{
		List<FaceGenChar> newChars = new List<FaceGenChar>
		{
			new FaceGenChar(_faceGeneratorView.BodyGen.CurrentBodyProperties, _faceGeneratorView.BodyGen.Race, new Equipment(), _faceGeneratorView.BodyGen.IsFemale)
		};
		_characterCreation.ChangeFaceGenChars(newChars);
		_affirmativeAction();
	}

	public override void Tick(float dt)
	{
		_faceGeneratorView.OnTick(dt);
	}

	public override int GetVirtualStageCount()
	{
		return 1;
	}

	public override void GoToIndex(int index)
	{
		_goToIndexAction(index);
	}

	public override void LoadEscapeMenuMovie()
	{
		_escapeMenuDatasource = new EscapeMenuVM(GetEscapeMenuItems(this));
		_escapeMenuMovie = _faceGeneratorView.GauntletLayer.LoadMovie("EscapeMenu", _escapeMenuDatasource);
	}

	public override void ReleaseEscapeMenuMovie()
	{
		_faceGeneratorView.GauntletLayer.ReleaseMovie(_escapeMenuMovie);
		_escapeMenuDatasource = null;
		_escapeMenuMovie = null;
	}
}
