using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions;

[DefaultView]
public class MissionSettlementPrepareView : MissionView
{
	public override void AfterStart()
	{
		((MissionBehavior)this).AfterStart();
		SetOwnerBanner();
	}

	private void SetOwnerBanner()
	{
		Campaign current = Campaign.Current;
		if (current == null || current.GameMode != CampaignGameMode.Campaign || Settlement.CurrentSettlement?.OwnerClan?.Banner == null || !(((MissionBehavior)this).Mission.Scene != null))
		{
			return;
		}
		bool flag = false;
		foreach (GameEntity item in ((MissionBehavior)this).Mission.Scene.FindEntitiesWithTag("bd_banner_b"))
		{
			_ = item;
			Action<Texture> action = delegate(Texture tex)
			{
				Material material = Mesh.GetFromResource("bd_banner_b").GetMaterial();
				uint num = (uint)material.GetShader().GetMaterialShaderFlagMask("use_tableau_blending");
				ulong shaderFlags = material.GetShaderFlags();
				material.SetShaderFlags(shaderFlags | num);
				material.SetTexture(Material.MBTextureType.DiffuseMap2, tex);
			};
			BannerVisualExtensions.GetTableauTextureLarge(Settlement.CurrentSettlement.OwnerClan.Banner, action);
			flag = true;
		}
		if (flag)
		{
			((MissionBehavior)this).Mission.Scene.SetClothSimulationState(state: false);
		}
	}
}
