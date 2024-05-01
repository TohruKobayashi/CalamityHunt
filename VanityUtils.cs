using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace CalamityHunt;

public static class VanityUtils
{
    public static Vector2 HeadPosition(this PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetHelmetDrawOffset() + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.headPosition + drawInfo.headVect;

    public static Vector2 BodyPosition(this PlayerDrawSet drawInfo) => new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);

    public static void ApplyVerticalOffset(ref this Vector2 drawPos, PlayerDrawSet drawInfo)
    {
        Vector2 value = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
        value.Y -= 2f;
        drawPos += value * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
    }

    public static Vector2 GetCompositeOffset_BackArm(ref PlayerDrawSet drawinfo) => new Vector2(6 * ((!drawinfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)) ? 1 : (-1)), 2 * ((!drawinfo.playerEffect.HasFlag(SpriteEffects.FlipVertically)) ? 1 : (-1)));
    public static Vector2 GetCompositeOffset_FrontArm(ref PlayerDrawSet drawinfo) => new Vector2(-5 * ((!drawinfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)) ? 1 : (-1)), 0f);

    public static bool NoBackpackOn(ref PlayerDrawSet drawinfo) => !drawinfo.drawPlayer.turtleArmor && drawinfo.drawPlayer.body != 106 && drawinfo.drawPlayer.body != 170 && drawinfo.drawPlayer.backpack <= 0 && !drawinfo.drawPlayer.mount.Active;
}
