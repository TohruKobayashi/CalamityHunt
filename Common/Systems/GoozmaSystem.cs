using System.Collections.Generic;
using System.Linq;
using CalamityHunt.Common.Utilities.Interfaces;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.NPCs;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class GoozmaSystem : ModSystem
    {
        public override void PostUpdateNPCs()
        {
            if (Main.zenithWorld) {
                SpawnGoozmaOld();
            }

            //stop black hole shader
            if (!Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<StellarBlackHole>())) {
                if (Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].Active) {
                    float intensity = Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].GetShader().Intensity;
                    Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].GetShader().UseIntensity(intensity * 0.5f);
                    Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].Deactivate();
                }
            }

            Filters.Scene["HuntOfTheOldGods:PluripotentSpawn"].GetShader().UseProgress(Main.GlobalTimeWrappedHourly * 0.05f % 1f);
            //stop pluripotent spawn shader
            if (!Main.npc.Any(n => n.active && n.type == ModContent.NPCType<PluripotentSpawn>())) {
                if (Filters.Scene["HuntOfTheOldGods:PluripotentSpawn"].Active) {
                    float intensity = Filters.Scene["HuntOfTheOldGods:PluripotentSpawn"].GetShader().Intensity;
                    Filters.Scene["HuntOfTheOldGods:PluripotentSpawn"].GetShader().UseIntensity(intensity * 0.5f);
                    Filters.Scene["HuntOfTheOldGods:PluripotentSpawn"].Deactivate();
                }
            }
        }

        public static bool GoozmaActive => Main.npc.Any(n => n is ISubjectOfNPC<Goozma> && n.active);

        public static Vector2 ninjaStatuePoint;
        public static Vector2[] slimeStatuePoints;

        public static bool FindSlimeStatues(int iCenter, int jCenter, int halfWidth = 20, int halfHeight = 20)
        {
            List<Vector2> count = new List<Vector2>();
            List<Point> ignore = new List<Point>();
            for (int i = iCenter - halfWidth; i <= iCenter + halfWidth; i++) {
                for (int j = jCenter - halfHeight; j <= jCenter + halfHeight; j++) {
                    Tile checkTile = Framing.GetTileSafely(i, j);

                    if (ignore.Contains(new Point(i, j))) {
                        continue;
                    }

                    if (checkTile.HasTile && checkTile.TileType == TileID.Statues && checkTile.TileFrameX / 18 == 8) {
                        ignore.Add(new Point(i + 1, j));
                        ignore.Add(new Point(i, j + 1));
                        ignore.Add(new Point(i, j + 2));
                        ignore.Add(new Point(i + 1, j + 1));
                        ignore.Add(new Point(i + 1, j + 2));

                        count.Add(new Vector2((i + 1) * 16, (j + 1) * 16));
                    }
                }
            }

            if (count.Count > 3) {
                ninjaStatuePoint = new Vector2(iCenter * 16, (jCenter - 1) * 16);
                slimeStatuePoints = count.ToArray();
                return true;
            }
            else {
                slimeStatuePoints = new Vector2[4];

                //text
                return false;
            }
        }

        private void SpawnGoozmaOld()
        {
            Vector2 spawnPos = Vector2.Zero;
            bool conditionsMet = false;
            int slimeBoss = -1;
            bool king = true;

            if (Main.slimeRain && (Main.hardMode || NPC.downedPlantBoss)) {
                foreach (NPC nPC in Main.npc.Where(n => (n.type == NPCID.KingSlime || n.type == NPCID.QueenSlimeBoss) && n.boss && n.active)) {
                    slimeBoss = nPC.whoAmI;
                    if (nPC.type == NPCID.QueenSlimeBoss) {
                        king = false;
                    }

                    break;
                }
                if (slimeBoss > -1) {
                    ModLoader.TryGetMod(HUtils.CalamityMod, out Mod calamity);
                    if (calamity != null) {
                        foreach (Item item in Main.item.Where(n => n.active && n.type == calamity.Find<ModItem>("OverloadedSludge").Type)) {
                            if (Main.npc[slimeBoss].Hitbox.Intersects(item.Hitbox)) {
                                spawnPos = item.Center;
                                item.active = false;
                                conditionsMet = true;
                            }
                        }
                    }

                    foreach (Item item in Main.item.Where(n => n.active && n.type == ModContent.ItemType<OverloadedSludge>())) {
                        if (Main.npc[slimeBoss].Hitbox.Intersects(item.Hitbox)) {
                            spawnPos = item.Center;
                            item.active = false;
                            conditionsMet = true;
                        }
                    }


                }
            }

            if (conditionsMet) {
                if (king) {
                    for (int i = 0; i < 200; i++) {
                        Dust slime = Dust.NewDustPerfect(Main.npc[slimeBoss].Center + Main.rand.NextVector2Circular(200, 110), 4, Main.rand.NextVector2Circular(15, 12) - Vector2.UnitY * 6f, 150, new Color(78, 136, 255, 80), 2f);
                        slime.noGravity = true;
                        slime.velocity *= 2f;
                    }
                    Gore.NewGore(Entity.GetSource_NaturalSpawn(), Main.npc[slimeBoss].Top, -Vector2.UnitY, GoreID.KingSlimeCrown);
                }
                else {
                    for (int i = 0; i < 200; i++) {
                        Color qsColor = NPC.AI_121_QueenSlime_GetDustColor();
                        qsColor.A = 150;
                        Dust slime = Dust.NewDustPerfect(Main.npc[slimeBoss].Center + Main.rand.NextVector2Circular(200, 110), 4, Main.rand.NextVector2Circular(15, 12) - Vector2.UnitY * 6f, 50, qsColor, 2f);
                        slime.noGravity = true;
                        slime.velocity *= 2f;
                    }
                    Gore.NewGore(Entity.GetSource_NaturalSpawn(), Main.npc[slimeBoss].Top, -Vector2.UnitY, GoreID.QueenSlimeCrown);
                }

                SoundEngine.PlaySound(SoundID.NPCDeath1.WithPitchOffset(-0.5f), spawnPos);

                Main.npc[slimeBoss].active = false;

                if (Main.netMode != NetmodeID.MultiplayerClient) {
                    NPC.NewNPCDirect(Entity.GetSource_NaturalSpawn(), spawnPos, ModContent.NPCType<PluripotentSpawn>());
                }
            }
        }
    }
}
