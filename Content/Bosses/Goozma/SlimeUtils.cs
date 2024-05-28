using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma
{
    public static class SlimeUtils
    {
        public static Vector2 FindSmashSpot(this NPC NPC, Vector2 target)
        {
            Vector2 pos = target;
            for (int j = 0; j < 36; j++)
            {
                Point world = new Point(pos.ToTileCoordinates().X, pos.ToTileCoordinates().Y + j);
                if (WorldGen.InWorld(world.X, world.Y))
                {
                    if (WorldGen.SolidTileAllowTopSlope(world.X, world.Y + j))
                    {
                        pos.Y = (int)(pos.Y / 16f) * 16f + j * 16 - NPC.height / 2f - 8;
                        break;
                    }
                }
            }
            return pos;
        }

        public static Vector2 FindSmashSpot(this Projectile Projectile, Vector2 target)
        {
            Vector2 pos = target;
            for (int j = 0; j < 32; j++)
            {
                Point world = new Point(pos.ToTileCoordinates().X, pos.ToTileCoordinates().Y + j);
                if (WorldGen.InWorld(world.X, world.Y))
                {
                    if (WorldGen.SolidTileAllowTopSlope(world.X, world.Y + j))
                    {
                        pos.Y = (int)(pos.Y / 16f) * 16f + j * 16 - Projectile.height / 2f + 16;
                        break;
                    }
                }
            }
            return pos;
        }

        public static Color[] GoozColors => new Color[]
        {
            new Color(GoozColorsVector3[3]),
            new Color(GoozColorsVector3[4]),
            new Color(GoozColorsVector3[5]),
            new Color(GoozColorsVector3[6]),
            new Color(GoozColorsVector3[7]),
        };
                
        
        public static Color[] GoozOilColors => new Color[]
        {
            new Color(GoozmaColorUtils.Oil[3]),
            new Color(GoozmaColorUtils.Oil[4]),
            new Color(GoozmaColorUtils.Oil[5]),
            new Color(GoozmaColorUtils.Oil[6]),
            new Color(GoozmaColorUtils.Oil[7]),
        };

        public static Vector3[] GoozColorsVector3
        {
            get
            {
                if (Main.drunkWorld)
                    return ColorsForFunnyWorlds(GoozmaColorType);                
                
                if ((Main.getGoodWorld && Main.masterMode) || Main.zenithWorld)
                    return GoozmaColorUtils.Nuclear;

                else if (Main.getGoodWorld)
                    return GoozmaColorUtils.Masterful;

                if (Main.notTheBeesWorld)
                    return GoozmaColorUtils.Honey;

                return GoozmaColorUtils.Oil;
            }
        }

        public static int GoozmaColorType;
        
        private static Vector3[] ColorsForFunnyWorlds(int type)
        {
            switch (GoozmaColorType)
            {
                default:
                    return GoozmaColorUtils.Oil;                
                case 0:
                    return GoozmaColorUtils.Nuclear;
                case 1:
                    return GoozmaColorUtils.Gold;
                case 2:
                    return GoozmaColorUtils.Grayscale;
                case 3:
                    return GoozmaColorUtils.Honey;
                case 4:
                    return GoozmaColorUtils.Masterful;
                case 5:
                    return GoozmaColorUtils.DarkEnergy;
                case 6:
                    return GoozmaColorUtils.Frigid;
                case 7:
                    return GoozmaColorUtils.Tritanopic;
                case 8:
                    return GoozmaColorUtils.Caliginous;
                case 9:
                    return GoozmaColorUtils.Spectrum;
                case 10:
                    return GoozmaColorUtils.Evil;
                case 11:
                    return GoozmaColorUtils.Shadowflame;
                case 12:
                    return GoozmaColorUtils.Dogmatic;
                case 13:
                    return GoozmaColorUtils.FIREBLU;
                case 14:
                    return GoozmaColorUtils.Distorted;
                case 15:
                    return GoozmaColorUtils.Electric;
                case 16:
                    return GoozmaColorUtils.Polterplasmic;
                case 17:
                    return GoozmaColorUtils.Exhumed;
                case 18:
                    return GoozmaColorUtils.Raptured;
                case 19:
                    return GoozmaColorUtils.Ibanical;
                case 20:
                    return GoozmaColorUtils.AcidRainbow;
                case 21:
                    return GoozmaColorUtils.Rubicon;
                case 22:
                    return GoozmaColorUtils.DoubleRainbow;
                case 23:
                    return GoozmaColorUtils.Unpleasant;
                case 24:
                    return GoozmaColorUtils.OceanJasper;
                case 25:
                    return GoozmaColorUtils.Seashell;
                case 26:
                    return GoozmaColorUtils.JackOLantern;
                case 27:
                    return GoozmaColorUtils.CottonCandy;
                case 28:
                    return GoozmaColorUtils.MeanGreen;
                case 29:
                    return GoozmaColorUtils.ColorCalibration;
                case 30:
                    return GoozmaColorUtils.Enchanted;
                case 31:
                    return GoozmaColorUtils.Sisyphan;
                case 32:
                    return GoozmaColorUtils.Babil;
                case 33:
                    return GoozmaColorUtils.Doomsday;
                case 34:
                    return GoozmaColorUtils.Rugamarian;
                case 35:
                    return GoozmaColorUtils.Poozma;
                case 36:
                    return GoozmaColorUtils.Festive;
                case 37:
                    return GoozmaColorUtils.MoonCarpet;
                case 39:
                    return GoozmaColorUtils.Zeromus;
                case 40:
                    return GoozmaColorUtils.Water;
                case 41:
                    return GoozmaColorUtils.Stellar;
                case 42:
                    return GoozmaColorUtils.Crimson;
                case 43:
                    return GoozmaColorUtils.Corruption;
                case 44:
                    return GoozmaColorUtils.Hallow;
                case 45:
                    return GoozmaColorUtils.TheDragon;
                case 46:
                    return GoozmaColorUtils.Speevil;
                case 47:
                    return GoozmaColorUtils.HyperrealisticBloody;
                case 48:
                    return GoozmaColorUtils.AuricTesla;
                case 49:
                    return GoozmaColorUtils.Pikmin;
                case 50:
                    return GoozmaColorUtils.Flame;
                case 51:
                    return GoozmaColorUtils.Poland;
                case 52:
                    return GoozmaColorUtils.Kindergarten;
                case 53:
                    return GoozmaColorUtils.Daniel;
                case 54:
                    return GoozmaColorUtils.Autumnal;
                case 55:
                    return GoozmaColorUtils.Subworld;
            }
        }

        public static void DrawWormhole(Vector2 position, Color innerColor, float rotation, float scale)
        {
            Asset<Texture2D> wormhole = AssetDirectory.Textures.Goozma.Wormhole;
            Main.EntitySpriteDraw(wormhole.Value, position - Main.screenPosition, null, innerColor, rotation, wormhole.Size() * 0.5f, scale * 2f, 0, 0);

        }
    }
}
