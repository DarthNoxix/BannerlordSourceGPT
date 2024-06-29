using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Siege;

public class SiegeStrategy : MBObjectBase
{
	public static MBReadOnlyList<SiegeStrategy> All => Campaign.Current.AllSiegeStrategies;

	public TextObject Name { get; private set; }

	public TextObject Description { get; private set; }

	internal static void AutoGeneratedStaticCollectObjectsSiegeStrategy(object o, List<object> collectedObjects)
	{
		((SiegeStrategy)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
		base.AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	public SiegeStrategy(string stringId)
		: base(stringId)
	{
	}

	public void Initialize(TextObject name, TextObject description)
	{
		base.Initialize();
		Name = name;
		Description = description;
		AfterInitialized();
	}
}
