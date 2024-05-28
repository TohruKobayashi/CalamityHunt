using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Melee
{
    public class ParasanguineDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Backpacks);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem.ModItem is Parasanguine && drawInfo.drawPlayer.ownedProjectileCounts[ModContent.ProjectileType<ParasanguineHeld>()] <= 0;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D texture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Items/Weapons/Melee/Parasanguine").Value;
            Vector2 drawCenter = drawInfo.drawPlayer.MountedCenter + new Vector2(-5 * drawInfo.drawPlayer.direction, 5).RotatedBy(drawInfo.drawPlayer.fullRotation);
            float drawRotation = 2.66f * drawInfo.drawPlayer.direction + drawInfo.drawPlayer.velocity.X * 0.01f;
            
            if (drawInfo.shadow <= 0.1f)
            {
                float strength = Utils.GetLerpValue(0.5f, 0.6f, drawInfo.drawPlayer.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent, true);

                if (drawInfo.drawPlayer.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 off = new Vector2(3).RotatedBy(MathHelper.TwoPi / 6f * i + drawRotation);
                        DrawData outlineDarkData = new DrawData(texture, drawCenter + off - Main.screenPosition, texture.Frame(), Color.Black * 0.5f * strength, drawRotation + MathHelper.PiOver4 * drawInfo.drawPlayer.direction, texture.Size() * 0.5f, 1f, drawInfo.playerEffect, 0);
                        drawInfo.DrawDataCache.Add(outlineDarkData);
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 off = new Vector2(3).RotatedBy(MathHelper.TwoPi / 6f * i + drawRotation);
                        DrawData outlineData = new DrawData(texture, drawCenter + off - Main.screenPosition, texture.Frame(), new Color(211, 10, 15, 0) * strength, drawRotation + MathHelper.PiOver4 * drawInfo.drawPlayer.direction, texture.Size() * 0.5f, 1f, drawInfo.playerEffect, 0);
                        drawInfo.DrawDataCache.Add(outlineData);
                    }
                }
                DrawData data = new DrawData(texture, drawCenter - Main.screenPosition, texture.Frame(), drawInfo.colorArmorBody, drawRotation + MathHelper.PiOver4 * drawInfo.drawPlayer.direction, texture.Size() * 0.5f, 1f, drawInfo.playerEffect, 0);
                drawInfo.DrawDataCache.Add(data);
            }
        }
    }
}
