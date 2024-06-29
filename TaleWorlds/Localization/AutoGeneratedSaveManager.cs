using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.Localization;

internal class AutoGeneratedSaveManager : IAutoGeneratedSaveManager
{
	public void Initialize(DefinitionContext definitionContext)
	{
		TypeDefinition obj = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(20001));
		CollectObjectsDelegate collectObjectsDelegate = TextObject.AutoGeneratedStaticCollectObjectsTextObject;
		obj.InitializeForAutoGeneration(collectObjectsDelegate);
		obj.GetPropertyDefinitionWithId(new MemberTypeId(2, 2)).InitializeForAutoGeneration(TextObject.AutoGeneratedGetMemberValueAttributes);
		obj.GetFieldDefinitionWithId(new MemberTypeId(2, 1)).InitializeForAutoGeneration(TextObject.AutoGeneratedGetMemberValueValue);
	}
}
