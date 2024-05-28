using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityHunt.Content.Tiles
{
    public class SlimeMonsoonMonolithTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(22, 18, 22));

            DustType = DustID.Cobalt;
            AnimationFrameHeight = 56;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameY < 56) {
                return;
            }

            Player player = Main.LocalPlayer;
            if (player is null) {
                return;
            }

            if (player.active) {
                Main.LocalPlayer.GetModPlayer<SceneEffectPlayer>().effectActive[(ushort)SceneEffectPlayer.EffectorType.SlimeMonsoon] = 30;
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frame = Main.tileFrame[TileID.LunarMonolith];
            frameCounter = Main.tileFrameCounter[TileID.LunarMonolith];
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<SlimeMonsoonMonolith>();
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            HitWire(i, j);
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
            return true;
        }

        public override void HitWire(int i, int j)
        {
            Main.tile[i, j].GetTopLeftTile(ref i, ref j, out _, out _);

            for (int l = i; l < i + 2; l++) {
                for (int m = j; m < j + 3; m++) {
                    if (Main.tile[l, m].TileType == Type) {
                        if (Main.tile[l, m].TileFrameY < 56) {
                            Main.tile[l, m].TileFrameY += 56;
                        }
                        else {
                            Main.tile[l, m].TileFrameY -= 56;
                        }
                    }
                }
            }
            if (Wiring.running) {
                Wiring.SkipWire(i, j);
                Wiring.SkipWire(i, j + 1);
                Wiring.SkipWire(i, j + 2);
                Wiring.SkipWire(i + 1, j);
                Wiring.SkipWire(i + 1, j + 1);
                Wiring.SkipWire(i + 1, j + 2);
            }
            NetMessage.SendTileSquare(-1, i, j + 1, 3);
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY >= 56) {
                frameYOffset = Main.tileFrame[type] * 56;
            }
            else {
                frameYOffset = 0;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return true;

            //We can just use animateindividualtile
            //Leaving this here as reference to how to draw tiles accounting for paint and echo

            /*
            Tile tile = Main.tile[i, j];

            //I don't know why TileDrawing.IsVisible is private. Weird
            if (tile.IsTileInvisible && !Main.ShouldShowInvisibleWalls())
                return false;

            Texture2D texture = TextureAssets.Tile[Type].Value;
            if (tile.TileColor != 0)
            {
                Texture2D paintedTexture = Main.instance.TilePaintSystem.TryGetTileAndRequestIfNotReady(Type, 0, tile.TileColor);
                texture = paintedTexture ?? texture;
            }

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = (tile.TileFrameX % 56) / 3 == 2 ? 18 : 16;
            int animate = 0;
            if (tile.TileFrameY >= 56)
                animate = Main.tileFrame[Type] * AnimationFrameHeight;

            Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + animate, 16, height), Lighting.GetColor(i, j), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            return false;
            */
        }
    }
}
