using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Magic
{
    public class CrystalGauntletsOnDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HandOnAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem.ModItem is CrystalGauntlets;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D texture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Items/Weapons/Magic/CrystalGauntlets_HandsOn").Value;
            if (drawInfo.drawPlayer.GetModPlayer<GoozmaWeaponsPlayer>().crystalGauntletsUseFingers) {
                texture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Items/Weapons/Magic/CrystalGauntlets_HandsOnPoint").Value;
            }

            Vector2 position = drawInfo.BodyPosition() + drawInfo.frontShoulderOffset + VanityUtilities.GetCompositeOffset_FrontArm(ref drawInfo);
            position.ApplyVerticalOffset(drawInfo);
            Vector2 origin = drawInfo.bodyVect + drawInfo.frontShoulderOffset + VanityUtilities.GetCompositeOffset_FrontArm(ref drawInfo);
            if (drawInfo.compFrontArmFrame.X / drawInfo.compFrontArmFrame.Width >= 7) {
                position += new Vector2((!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)) ? 1 : (-1), (!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically)) ? 1 : (-1));
            }

            DrawData drawData = new DrawData(texture, position, drawInfo.compFrontArmFrame, Color.White * (1f - drawInfo.shadow), drawInfo.compositeFrontArmRotation, origin, 1f, drawInfo.playerEffect, 0);
            drawInfo.DrawDataCache.Add(drawData);
        }
    }

    public class CrystalGauntletsOffDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Torso);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem.ModItem is CrystalGauntlets;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D texture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Items/Weapons/Magic/CrystalGauntlets_HandsOff").Value;
            if (drawInfo.drawPlayer.GetModPlayer<GoozmaWeaponsPlayer>().crystalGauntletsUseFingers) {
                texture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Items/Weapons/Magic/CrystalGauntlets_HandsOffPoint").Value;
            }

            Vector2 position = drawInfo.BodyPosition() + drawInfo.backShoulderOffset + VanityUtilities.GetCompositeOffset_BackArm(ref drawInfo);
            position.ApplyVerticalOffset(drawInfo);
            Vector2 origin = drawInfo.bodyVect + drawInfo.backShoulderOffset + VanityUtilities.GetCompositeOffset_BackArm(ref drawInfo);
            DrawData drawData = new DrawData(texture, position, drawInfo.compBackArmFrame, Color.White * (1f - drawInfo.shadow), drawInfo.compositeBackArmRotation, origin, 1f, drawInfo.playerEffect, 0);
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}
