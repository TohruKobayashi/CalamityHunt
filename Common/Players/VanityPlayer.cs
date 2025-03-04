using System;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players;

public class VanityPlayer : ModPlayer
{
    public bool tendrilCursor;
    public int tendrilCount;
    public int cTendril;
    public int tendrilSlot;

    public bool trailblazerGoggles;
    public bool trailblazerBackpack;
    public bool crystalGauntlets;

    public override void Load()
    {
        On_Player.UpdateVisibleAccessory += UpdateTendrilCount;
    }

    public override void UpdateDyes()
    {
        if (tendrilSlot > -1) {
            cTendril = Player.dye[tendrilSlot % 10].dye;
        }
    }

    private void UpdateTendrilCount(On_Player.orig_UpdateVisibleAccessory orig, Player self, int itemSlot, Item item, bool modded)
    {
        if (item.type == ModContent.ItemType<TendrilCursorAttachment>()) {
            int tendrilCount = 7;
            if (!modded) {
                tendrilCount = Math.Clamp((itemSlot - 1) % 10, 1, 8);
            }
            self.GetModPlayer<VanityPlayer>().tendrilCount = tendrilCount;
            self.GetModPlayer<VanityPlayer>().tendrilSlot = itemSlot;
        }

        orig(self, itemSlot, item, modded);
    }

    public override void ResetEffects()
    {
        tendrilCursor = false;

        trailblazerGoggles = false;
        trailblazerBackpack = false;
        //TODO: implement this better
        crystalGauntlets = false;  
    }
}

public class TrailblazerGogglesLayer : PlayerDrawLayer
{
    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.FaceAcc);

    public override bool IsHeadLayer => true;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<VanityPlayer>().trailblazerGoggles;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Texture2D goggleTexture = TrailblazerGoggles.goggleTexture.Value;

        Vector2 vec5 = drawInfo.HeadPosition();
        vec5 = vec5.Floor();
        vec5.ApplyVerticalOffset(drawInfo);

        DrawData item = new DrawData(goggleTexture, vec5, goggleTexture.Frame(), drawInfo.colorArmorHead * (1f - drawInfo.shadow), drawInfo.drawPlayer.headRotation, new Vector2((int)drawInfo.headVect.X, (int)drawInfo.headVect.Y), 1f, drawInfo.playerEffect);
        drawInfo.DrawDataCache.Add(item);
    }
}

public class TrailblazerBackpackLayer : PlayerDrawLayer
{
    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Backpacks);

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<VanityPlayer>().trailblazerBackpack && VanityUtilities.NoBackpackOn(ref drawInfo);

    private int frame;

    private int frameCounter;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Texture2D texture = TrailblazerBackpack.backTexture.Value;
        Texture2D swirlTexture = TrailblazerBackpack.backSwirlTexture.Value;
        Texture2D antennaTexture = TrailblazerBackpack.backAntennaTexture.Value;

        Vector2 vec5 = drawInfo.BodyPosition() + new Vector2(-16 * drawInfo.drawPlayer.direction, -1 * drawInfo.drawPlayer.gravDir);
        vec5 = vec5.Floor();
        vec5.ApplyVerticalOffset(drawInfo);

        Vector2 aPos = vec5 + new Vector2(9 * drawInfo.drawPlayer.direction, -18 * drawInfo.drawPlayer.gravDir);
        for (int i = 0; i < 9; i++) {
            Color aColor = Lighting.GetColor(drawInfo.drawPlayer.MountedCenter.ToTileCoordinates());
            Rectangle aFrame = antennaTexture.Frame(1, 3, 0, 2);
            if (i == 8) {
                aColor = Color.White;
                aFrame = antennaTexture.Frame(1, 3, 0, 0);
            }
            else if (i > 4) {
                aFrame = antennaTexture.Frame(1, 3, 0, 1);
            }

            float rot = -drawInfo.drawPlayer.velocity.X * (0.03f * (i + 1) / 12f) - Math.Abs(drawInfo.drawPlayer.velocity.Y) * drawInfo.drawPlayer.velocity.X * (0.01f * i / 12f);
            DrawData antenna = new DrawData(antennaTexture, aPos, aFrame, aColor * (1f - drawInfo.shadow), drawInfo.drawPlayer.bodyRotation + rot, aFrame.Size() * new Vector2(0.5f, 1f) - Vector2.UnitY, 1f, drawInfo.playerEffect);
            drawInfo.DrawDataCache.Add(antenna);
            aPos += new Vector2(0, -aFrame.Height).RotatedBy(rot);
        }

        if (drawInfo.shadow == 0f) {
            if (frameCounter++ > 5) {
                frame = (frame + 1) % 5;
                frameCounter = 0;
            }
        }

        DrawData swirl = new DrawData(swirlTexture, vec5, swirlTexture.Frame(1, 5, 0, frame), Color.White * (1f - drawInfo.shadow), drawInfo.drawPlayer.bodyRotation, swirlTexture.Frame(1, 5, 0, frame).Size() * 0.5f, 1f, drawInfo.playerEffect);
        drawInfo.DrawDataCache.Add(swirl);

        Rectangle itemFrame = texture.Frame(1, 20, 0, (int)(drawInfo.drawPlayer.legFrame.Y / drawInfo.drawPlayer.legFrame.Height));

        DrawData item = new DrawData(texture, vec5, itemFrame, Lighting.GetColor(drawInfo.drawPlayer.MountedCenter.ToTileCoordinates()) * (1f - drawInfo.shadow), drawInfo.drawPlayer.bodyRotation, itemFrame.Size() * 0.5f, 1f, drawInfo.playerEffect);
        drawInfo.DrawDataCache.Add(item);
    }
}

public class TrailblazerStrapLayer : PlayerDrawLayer
{
    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.FrontAccFront);

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<VanityPlayer>().trailblazerBackpack && VanityUtilities.NoBackpackOn(ref drawInfo);

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Texture2D texture = TrailblazerBackpack.strapTexture.Value;

        Vector2 vec5 = drawInfo.BodyPosition();//drawInfo.Position - Main.screenPosition + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.width / 2, drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height / 2) + new Vector2(0f, -4f);
        vec5 = vec5.Floor();

        DrawData item = new DrawData(texture, vec5, drawInfo.drawPlayer.bodyFrame, drawInfo.colorArmorBody * (1f - drawInfo.shadow), drawInfo.drawPlayer.bodyRotation, new Vector2(texture.Width * 0.5f, drawInfo.bodyVect.Y), 1f, drawInfo.playerEffect);
        drawInfo.DrawDataCache.Add(item);
    }
}
