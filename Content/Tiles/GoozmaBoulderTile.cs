using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityHunt.Content.Tiles
{
    public class GoozmaBoulderTile : ModTile
    {
        public override void Load()
        {
            On_Liquid.tilesIgnoreWater += LiquidBehindGoozmaBoulder;
            On_TileDrawing.DrawSingleTile += GoozmaBoulderDrawIgnoreHammering;
        }

        public override void Unload()
        {
            On_Liquid.tilesIgnoreWater -= LiquidBehindGoozmaBoulder;
            On_TileDrawing.DrawSingleTile -= GoozmaBoulderDrawIgnoreHammering;
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.Boulders[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.DontDrawTileSliced[Type] = true;
            TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;

            AddMapEntry(new Color(66, 70, 87));

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 6;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16, 16, 16, 16, 16, 18
            };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.Origin = new Point16(3, 5);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0) {
                //Actuate corners
                Main.tile[i, j].Get<TileWallWireStateData>().IsActuated = true;
                Main.tile[i + 5, j].Get<TileWallWireStateData>().IsActuated = true;
                Main.tile[i, j + 5].Get<TileWallWireStateData>().IsActuated = true;
                Main.tile[i + 5, j + 5].Get<TileWallWireStateData>().IsActuated = true;

                //Make top a slab
                Main.tile[i + 1, j].Get<TileWallWireStateData>().IsHalfBlock = true;
                Main.tile[i + 2, j].Get<TileWallWireStateData>().IsHalfBlock = true;
                Main.tile[i + 3, j].Get<TileWallWireStateData>().IsHalfBlock = true;
                Main.tile[i + 4, j].Get<TileWallWireStateData>().IsHalfBlock = true;

                //Set "corners" to be slopes
                Main.tile[i, j + 1].Get<TileWallWireStateData>().Slope = SlopeType.SlopeDownRight;
                Main.tile[i + 5, j + 1].Get<TileWallWireStateData>().Slope = SlopeType.SlopeDownLeft;
                Main.tile[i, j + 4].Get<TileWallWireStateData>().Slope = SlopeType.SlopeUpRight;
                Main.tile[i + 1, j + 5].Get<TileWallWireStateData>().Slope = SlopeType.SlopeUpRight;
                Main.tile[i + 5, j + 4].Get<TileWallWireStateData>().Slope = SlopeType.SlopeUpLeft;
                Main.tile[i + 4, j + 5].Get<TileWallWireStateData>().Slope = SlopeType.SlopeUpLeft;
            }
            return true;
        }

        public override bool Slope(int i, int j) => false;

        public override bool CanDrop(int i, int j) => false;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Projectile.NewProjectile(Entity.GetSource_NaturalSpawn(), new Vector2((i + 2.5f) * 16 + 8, (j + 2.5f) * 16 + 8 + 4), Vector2.Zero, ModContent.ProjectileType<GoozmaBoulderProjectile>(), 1, 0);
            }
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        private void LiquidBehindGoozmaBoulder(On_Liquid.orig_tilesIgnoreWater orig, bool ignoreSolids)
        {
            Main.tileSolid[ModContent.TileType<GoozmaBoulderTile>()] = !ignoreSolids;
            orig(ignoreSolids);
        }

        private void GoozmaBoulderDrawIgnoreHammering(On_TileDrawing.orig_DrawSingleTile orig, TileDrawing self, TileDrawInfo drawData, bool solidLayer, int waterStyleOverride, Vector2 screenPosition, Vector2 screenOffset, int tileX, int tileY)
        {
            bool halfBrick = Main.tile[tileX, tileY].IsHalfBlock;
            SlopeType slope = Main.tile[tileX, tileY].Slope;
            if (Main.tile[tileX, tileY].TileType == Type) {
                Main.tile[tileX, tileY].Get<TileWallWireStateData>().IsHalfBlock = false;
                Main.tile[tileX, tileY].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
            }

            orig(self, drawData, solidLayer, waterStyleOverride, screenPosition, screenOffset, tileX, tileY);

            if (Main.tile[tileX, tileY].TileType == Type) {
                Main.tile[tileX, tileY].Get<TileWallWireStateData>().IsHalfBlock = halfBrick;
                Main.tile[tileX, tileY].Get<TileWallWireStateData>().Slope = slope;
            }
        }
    }
}
