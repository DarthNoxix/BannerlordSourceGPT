using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core;

public class WeaponComponent : ItemComponent
{
	private readonly MBList<WeaponComponentData> _weaponList = new MBList<WeaponComponentData>();

	public MBReadOnlyList<WeaponComponentData> Weapons => _weaponList;

	public WeaponComponentData PrimaryWeapon => _weaponList[0];

	internal static void AutoGeneratedStaticCollectObjectsWeaponComponent(object o, List<object> collectedObjects)
	{
		((WeaponComponent)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	public void AddWeapon(WeaponComponentData weaponComponentData, ItemModifierGroup itemModifierGroup)
	{
		base.ItemModifierGroup = itemModifierGroup;
		_weaponList.Add(weaponComponentData);
	}

	public override ItemComponent GetCopy()
	{
		return new WeaponComponent(base.Item);
	}

	public WeaponComponent(ItemObject item)
	{
		base.Item = item;
	}

	public ItemObject.ItemTypeEnum GetItemType()
	{
		return WeaponComponentData.GetItemTypeFromWeaponClass(_weaponList[0].WeaponClass);
	}

	public override void Deserialize(MBObjectManager objectManager, XmlNode node)
	{
		base.Deserialize(objectManager, node);
		_ = node.Attributes["modifier_group"]?.Value;
		WeaponComponentData weaponComponentData = new WeaponComponentData(base.Item, WeaponClass.Undefined, (WeaponFlags)0uL);
		weaponComponentData.Deserialize(base.Item, node);
		_weaponList.Add(weaponComponentData);
	}
}