using System;
using TaleWorlds.Core;

namespace SandBox.View.Map;

public class DefaultMapConversationDataProvider : IMapConversationDataProvider
{
	string IMapConversationDataProvider.GetAtmosphereNameFromData(MapConversationTableauData data)
	{
		string text = ((data.TimeOfDay <= 3f || data.TimeOfDay >= 21f) ? "night" : ((!(data.TimeOfDay > 8f) || !(data.TimeOfDay < 16f)) ? "sunset" : "noon"));
		if (data.Settlement == null || data.Settlement.IsHideout)
		{
			if (data.IsCurrentTerrainUnderSnow)
			{
				return "conv_snow_" + text + "_0";
			}
			return data.ConversationTerrainType switch
			{
				TerrainType.Desert => "conv_desert_" + text + "_0", 
				TerrainType.Steppe => "conv_steppe_" + text + "_0", 
				TerrainType.Forest => "conv_forest_" + text + "_0", 
				_ => "conv_plains_" + text + "_0", 
			};
		}
		string text2 = Enum.GetName(typeof(CultureCode), data.Settlement.Culture.GetCultureCode()).ToLower();
		if (data.IsInside)
		{
			return "conv_" + text2 + "_lordshall_0";
		}
		return "conv_" + text2 + "_town_" + text + "_0";
	}
}
