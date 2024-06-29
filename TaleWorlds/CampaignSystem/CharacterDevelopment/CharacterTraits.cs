using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment;

public class CharacterTraits : PropertyOwner<TraitObject>
{
	public const int MaxTraitValue = 20;

	public int Mercy => GetPropertyValue(DefaultTraits.Mercy);

	public int Valor => GetPropertyValue(DefaultTraits.Valor);

	public int Honor => GetPropertyValue(DefaultTraits.Honor);

	public int Generosity => GetPropertyValue(DefaultTraits.Generosity);

	public int Calculating => GetPropertyValue(DefaultTraits.Calculating);

	internal static void AutoGeneratedStaticCollectObjectsCharacterTraits(object o, List<object> collectedObjects)
	{
		((CharacterTraits)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	public CharacterTraits(CharacterTraits propertyOwner)
		: base((PropertyOwner<TraitObject>)propertyOwner)
	{
	}

	public CharacterTraits()
	{
	}

	public override void Deserialize(MBObjectManager objectManager, XmlNode node)
	{
		Initialize();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Comment)
			{
				XmlAttributeCollection attributes = childNode.Attributes;
				string value = attributes["id"].Value;
				string value2 = attributes["value"].Value;
				TraitObject @object = Game.Current.ObjectManager.GetObject<TraitObject>(value);
				if (@object != null)
				{
					int value3 = ((value2 == null) ? 1 : Convert.ToInt32(value2));
					SetPropertyValue(@object, value3);
				}
			}
		}
		foreach (XmlNode childNode2 in node.ChildNodes)
		{
			if (childNode2.NodeType == XmlNodeType.Comment)
			{
				continue;
			}
			string text = childNode2.Name;
			int value4 = 1;
			if (text == "Trait")
			{
				text = null;
				if (childNode2.Attributes != null)
				{
					XmlAttribute xmlAttribute = childNode2.Attributes["id"];
					if (xmlAttribute != null)
					{
						text = xmlAttribute.InnerText;
					}
				}
			}
			if (text == null)
			{
				continue;
			}
			TraitObject object2 = Game.Current.ObjectManager.GetObject<TraitObject>(text);
			if (object2 != null)
			{
				XmlAttribute xmlAttribute2 = childNode2.Attributes["value"];
				if (xmlAttribute2 != null)
				{
					string value5 = xmlAttribute2.Value;
					value4 = ((value5 == null) ? 1 : Convert.ToInt32(value5));
				}
				SetPropertyValue(object2, value4);
			}
		}
	}

	protected override void AfterLoad()
	{
		if (!(MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("e1.8.1.0")))
		{
			return;
		}
		MBList<KeyValuePair<TraitObject, int>> mBList = _attributes.ToMBList();
		int num = 0;
		foreach (KeyValuePair<TraitObject, int> item in mBList)
		{
			if (item.Key.StringId == "DesertScoutSkills")
			{
				_attributes.Remove(item.Key);
				num = Math.Max(item.Value, num);
			}
			if (item.Key.StringId == "WoodsScoutSkills")
			{
				_attributes.Remove(item.Key);
				num = Math.Max(item.Value, num);
			}
			if (item.Key.StringId == "HillScoutSkills")
			{
				_attributes.Remove(item.Key);
				num = Math.Max(item.Value, num);
			}
			if (item.Key.StringId == "MarshesLore")
			{
				_attributes.Remove(item.Key);
				num = Math.Max(item.Value, num);
			}
			if (item.Key.StringId == "SteppeScoutSkills")
			{
				_attributes.Remove(item.Key);
				num = Math.Max(item.Value, num);
			}
		}
		if (num > 0)
		{
			_attributes.Add(DefaultTraits.ScoutSkills, num);
		}
		List<string> source = new List<string> { "Engineer", "SergeantCommandSkills", "Manager", "Surgeon", "ScoutSkills", "Commander", "Politician", "RogueSkills", "Blacksmith" };
		foreach (KeyValuePair<TraitObject, int> item2 in _attributes.ToMBList())
		{
			if (source.ContainsQ(item2.Key.StringId) && item2.Value > 0)
			{
				_attributes[item2.Key] = item2.Value * 2;
			}
		}
	}
}
