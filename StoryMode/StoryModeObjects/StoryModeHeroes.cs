using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace StoryMode.StoryModeObjects;

public class StoryModeHeroes
{
	private const string BrotherStringId = "tutorial_npc_brother";

	private const string LittleBrotherStringId = "storymode_little_brother";

	private const string LittleSisterStringId = "storymode_little_sister";

	private const string TacitusStringId = "tutorial_npc_tacitus";

	private const string RadagosStringId = "tutorial_npc_radagos";

	private const string IstianaStringId = "storymode_imperial_mentor_istiana";

	private const string ArzagosStringId = "storymode_imperial_mentor_arzagos";

	private const string GalterStringId = "radagos_henchman";

	private Hero _elderBrother;

	private Hero _littleBrother;

	private Hero _littleSister;

	private Hero _tacitus;

	private Hero _radagos;

	private Hero _imperialMentor;

	private Hero _antiImperialMentor;

	private Hero _radagosHenchman;

	public static Hero ElderBrother => StoryModeManager.Current.StoryModeHeroes._elderBrother;

	public static Hero LittleBrother => StoryModeManager.Current.StoryModeHeroes._littleBrother;

	public static Hero LittleSister => StoryModeManager.Current.StoryModeHeroes._littleSister;

	public static Hero Tacitus => StoryModeManager.Current.StoryModeHeroes._tacitus;

	public static Hero Radagos => StoryModeManager.Current.StoryModeHeroes._radagos;

	public static Hero ImperialMentor => StoryModeManager.Current.StoryModeHeroes._imperialMentor;

	public static Hero AntiImperialMentor => StoryModeManager.Current.StoryModeHeroes._antiImperialMentor;

	public static Hero RadagosHencman => StoryModeManager.Current.StoryModeHeroes._radagosHenchman;

	internal StoryModeHeroes()
	{
		RegisterAll();
	}

	private void RegisterAll()
	{
		Clan clan = Campaign.Current.CampaignObjectManager.Find<Clan>("player_faction");
		CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>("main_hero_mother");
		CharacterObject object2 = Game.Current.ObjectManager.GetObject<CharacterObject>("main_hero_father");
		if (HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("tutorial_npc_brother"), out _elderBrother, "tutorial_npc_brother"))
		{
			_elderBrother.Clan = clan;
			TextObject textObject = GameTexts.FindText("str_player_brother_name", @object.Culture.StringId);
			_elderBrother.SetName(textObject, textObject);
			_elderBrother.Mother = @object.HeroObject;
			_elderBrother.Father = object2.HeroObject;
		}
		if (HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("storymode_little_brother"), out _littleBrother, "storymode_little_brother"))
		{
			TextObject textObject2 = GameTexts.FindText("str_player_little_brother_name", @object.Culture.StringId);
			_littleBrother.SetName(textObject2, textObject2);
			_littleBrother.Mother = @object.HeroObject;
			_littleBrother.Father = object2.HeroObject;
		}
		if (HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("storymode_little_sister"), out _littleSister, "storymode_little_sister"))
		{
			TextObject textObject3 = GameTexts.FindText("str_player_little_sister_name", @object.Culture.StringId);
			_littleSister.SetName(textObject3, textObject3);
			_littleSister.Mother = @object.HeroObject;
			_littleSister.Father = object2.HeroObject;
		}
		HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("tutorial_npc_tacitus"), out _tacitus, "tutorial_npc_tacitus");
		HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("tutorial_npc_radagos"), out _radagos, "tutorial_npc_radagos");
		HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("storymode_imperial_mentor_istiana"), out _imperialMentor, "storymode_imperial_mentor_istiana");
		HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("storymode_imperial_mentor_arzagos"), out _antiImperialMentor, "storymode_imperial_mentor_arzagos");
		HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("radagos_henchman"), out _radagosHenchman, "radagos_henchman");
	}
}
