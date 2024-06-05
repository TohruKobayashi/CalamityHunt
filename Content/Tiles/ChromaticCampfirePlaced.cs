using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Placeable;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace CalamityHunt.Content.Tiles
{
    public class ChromaticCampfirePlaced : ModTile
    {
        public Color rainbowGlow => new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 100f);
        
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true; // Necessary since Style3x3Wall uses AnchorWall
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 }; //
            AnimationFrameHeight = 36;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(139, 0, 0), name);
            AdjTiles = new int[] { TileID.Campfire };
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                if (Main.tile[i, j].TileFrameY >= 36)
                {
                    Main.LocalPlayer.AddBuff(BuffID.Campfire, 20);
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameY >= 36)
            {
                Tile tile = Main.tile[i, j];
                r = rainbowGlow.R / 125;
                g = rainbowGlow.G / 125;
                b = rainbowGlow.B / 125;
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frame = Main.tileFrame[TileID.Campfire];
            frameCounter = Main.tileFrameCounter[TileID.Campfire];
        }
        public override bool HasSmartInteract(int i, int j, Terraria.GameContent.ObjectInteractions.SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
            HitWire(i, j);
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType<ChromaticCampfire>();
        }

        public override void HitWire(int i, int j)
        {
            int x = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int y = j - Main.tile[i, j].TileFrameY / 18 % 2;
            for (int l = x; l < x + 3; l++)
            {
                for (int m = y; m < y + 2; m++)
                {
                    if (Main.tile[l, m].TileType == Type)
                    {
                        if (Main.tile[l, m].TileFrameY < 36)
                        {
                            Main.tile[l, m].TileFrameY += 36;
                        }
                        else
                        {
                            Main.tile[l, m].TileFrameY -= 36;
                        }
                    }
                }
            }
            if (Wiring.running)
            {
                Wiring.SkipWire(x, y);
                Wiring.SkipWire(x, y + 1);
                Wiring.SkipWire(x + 1, y);
                Wiring.SkipWire(x + 1, y + 1);
            }
            NetMessage.SendTileSquare(-1, x, y, 3);
        }
    }
}
