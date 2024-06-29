using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Settlements.Buildings;

public sealed class BuildingType : MBObjectBase
{
	public struct EffectInfo
	{
		public BuildingEffectEnum BuildingEffect { get; }

		public float Level1Effect { get; }

		public float Level2Effect { get; }

		public float Level3Effect { get; }

		public float GetEffectValue(int i)
		{
			return i switch
			{
				2 => Level2Effect, 
				1 => Level1Effect, 
				_ => Level3Effect, 
			};
		}

		public EffectInfo(BuildingEffectEnum effect, float[] effectValues)
		{
			BuildingEffect = effect;
			Level1Effect = effectValues[0];
			Level2Effect = effectValues[1];
			Level3Effect = effectValues[2];
		}

		public EffectInfo(BuildingEffectEnum effect, float effectValue1, float effectValue2, float effectValue3)
		{
			BuildingEffect = effect;
			Level1Effect = effectValue1;
			Level2Effect = effectValue2;
			Level3Effect = effectValue3;
		}
	}

	public const int MaxLevel = 3;

	public bool IsDefaultProject;

	private int[] _productionCosts = new int[3];

	public int StartLevel;

	public BuildingLocation BuildingLocation;

	private EffectInfo[] _effects;

	public static MBReadOnlyList<BuildingType> All => Campaign.Current.AllBuildingTypes;

	public TextObject Name { get; private set; }

	public TextObject Explanation { get; private set; }

	internal static void AutoGeneratedStaticCollectObjectsBuildingType(object o, List<object> collectedObjects)
	{
		((BuildingType)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	public BuildingType(string stringId)
		: base(stringId)
	{
	}

	public void Initialize(TextObject name, TextObject explanation, int[] productionCosts, BuildingLocation buildingLocation, Tuple<BuildingEffectEnum, float, float, float>[] effects, int startLevel = 0)
	{
		base.Initialize();
		Name = name;
		Explanation = explanation;
		_productionCosts = productionCosts;
		IsDefaultProject = buildingLocation == BuildingLocation.Daily;
		_effects = effects.Select((Tuple<BuildingEffectEnum, float, float, float> x) => new EffectInfo(x.Item1, x.Item2, x.Item3, x.Item4)).ToArray();
		StartLevel = startLevel;
		BuildingLocation = buildingLocation;
		AfterInitialized();
	}

	public override string ToString()
	{
		return Name.ToString();
	}

	public int GetProductionCost(int level)
	{
		if (level < StartLevel || level >= 3)
		{
			return 0;
		}
		return _productionCosts[level];
	}

	public float GetBaseBuildingEffectAmount(BuildingEffectEnum effect, int level)
	{
		for (int i = 0; i < _effects.Length; i++)
		{
			if (_effects[i].BuildingEffect == effect)
			{
				return _effects[i].GetEffectValue(level);
			}
		}
		return 0f;
	}

	public TextObject GetExplanationAtLevel(int level)
	{
		if (level == 0 || level > 3)
		{
			return TextObject.Empty;
		}
		TextObject textObject = TextObject.Empty;
		if (_effects.Length == 1)
		{
			textObject = GameTexts.FindText("str_building_effect_explanation", Enum.GetName(typeof(BuildingEffectEnum), _effects[0].BuildingEffect));
			textObject.SetTextVariable("BONUS_AMOUNT", _effects[0].GetEffectValue(level));
		}
		else if (_effects.Length >= 2)
		{
			textObject = GameTexts.FindText("str_string_newline_string");
			TextObject textObject2 = GameTexts.FindText("str_building_effect_explanation", Enum.GetName(typeof(BuildingEffectEnum), _effects[0].BuildingEffect));
			textObject2.SetTextVariable("BONUS_AMOUNT", _effects[0].GetEffectValue(level));
			TextObject textObject3 = GameTexts.FindText("str_building_effect_explanation", Enum.GetName(typeof(BuildingEffectEnum), _effects[1].BuildingEffect));
			textObject3.SetTextVariable("BONUS_AMOUNT", _effects[1].GetEffectValue(level));
			textObject.SetTextVariable("STR1", textObject2);
			textObject.SetTextVariable("STR2", textObject3);
			textObject.SetTextVariable("newline", "\n");
			for (int i = 2; i < _effects.Length; i++)
			{
				TextObject textObject4 = GameTexts.FindText("str_string_newline_string");
				textObject4.SetTextVariable("STR1", textObject);
				TextObject textObject5 = GameTexts.FindText("str_building_effect_explanation", Enum.GetName(typeof(BuildingEffectEnum), _effects[i].BuildingEffect));
				textObject5.SetTextVariable("BONUS_AMOUNT", _effects[i].GetEffectValue(level));
				textObject4.SetTextVariable("STR2", textObject5);
				textObject4.SetTextVariable("newline", "\n");
				textObject = textObject4;
			}
		}
		return textObject;
	}
}
