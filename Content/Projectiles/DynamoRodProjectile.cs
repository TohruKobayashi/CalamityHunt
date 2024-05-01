using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityHunt.Content.Projectiles
{
    public class DynamoRodProjectile : ModProjectile
    {
        public override string Texture => $"{nameof(CalamityHunt)}/Content/Items/Misc/DynamoRod";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            // explosives don't scale with any damage
            Projectile.damage = DynamoRod.Damage;
            Projectile.knockBack = DynamoRod.Knockback;

            Projectile.rotation += Projectile.velocity.Length() * 0.09f * Projectile.direction;
            Projectile.velocity.Y += 0.05f;
            Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y, int.MinValue, 10);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 36; i++)
            {
                var smoke = ParticleBehavior.NewParticle(ModContent.GetInstance<CosmicSmokeParticleBehavior>(), Projectile.Center + Main.rand.NextVector2Circular(1200, 1200) * Projectile.scale + Projectile.velocity * (i / 6f) * 0.5f, (Main.rand.NextVector2Circular(4, 4) + Projectile.velocity * (i / 8f)) * Projectile.scale, Color.White, (1f + Main.rand.NextFloat(12f)) * Projectile.scale);
                smoke.Add(new ParticleData<string> { Value = "Cosmos" });
            }

            if (Projectile.owner == Main.myPlayer)
            ExplodeandDestroyTiles(Projectile, 32, false, new List<int>() { }, new List<int>() { });
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 0)
                return true;
            Asset<Texture2D> shadowTexture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Bosses/Goozma/Projectiles/BlackHoleBlenderShadow");

            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black * 0.5f, -Projectile.rotation * 0.7f, shadowTexture.Size() * 0.5f, Projectile.scale * 0.3f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black * 0.2f, -Projectile.rotation * 0.5f, shadowTexture.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black, Projectile.rotation * 0.33f, shadowTexture.Size() * 0.5f, Projectile.scale * 0.2f, 0, 0);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? CanDamage()
        {
            return Projectile.timeLeft <= 1;
        }
        public static void ExplodeandDestroyTiles(Projectile projectile, int explosionRadius, bool checkExplosions, List<int> tilesToCheck, List<int> wallsToCheck)
        {
            int minTileX = (int)projectile.position.X / 16 - explosionRadius;
            int maxTileX = (int)projectile.position.X / 16 + explosionRadius;
            int minTileY = (int)projectile.position.Y / 16 - explosionRadius;
            int maxTileY = (int)projectile.position.Y / 16 + explosionRadius;
            if (minTileX < 0)
            {
                minTileX = 0;
            }
            if (maxTileX > Main.maxTilesX)
            {
                maxTileX = Main.maxTilesX;
            }
            if (minTileY < 0)
            {
                minTileY = 0;
            }
            if (maxTileY > Main.maxTilesY)
            {
                maxTileY = Main.maxTilesY;
            }

            bool canKillWalls = false;
            float projectilePositionX = projectile.position.X / 16f;
            float projectilePositionY = projectile.position.Y / 16f;
            for (int x = minTileX; x <= maxTileX; x++)
            {
                for (int y = minTileY; y <= maxTileY; y++)
                {
                    Vector2 explodeArea = new Vector2(Math.Abs(x - projectilePositionX), Math.Abs(y - projectilePositionY));
                    float distance = explodeArea.Length();
                    if (distance < explosionRadius && Main.tile[x, y] != null && Main.tile[x, y].WallType == WallID.None)
                    {
                        canKillWalls = true;
                        break;
                    }
                }
            }

            List<int> tileExcludeList = new List<int>()
            {
                TileID.DemonAltar,
                TileID.ElderCrystalStand
            };
            for (int i = 0; i < tilesToCheck.Count; ++i)
                tileExcludeList.Add(tilesToCheck[i]);
            List<int> wallExcludeList = new List<int>();
            for (int i = 0; i < wallsToCheck.Count; ++i)
                wallExcludeList.Add(wallsToCheck[i]);

            for (int i = minTileX; i <= maxTileX; i++)
            {
                for (int j = minTileY; j <= maxTileY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    bool t = 1 == 1; bool f = 1 == 2;

                    Vector2 explodeArea = new Vector2(Math.Abs(i - projectilePositionX), Math.Abs(j - projectilePositionY));
                    float distance = explodeArea.Length();
                    if (distance < explosionRadius)
                    {
                        bool canKillTile = true;
                        if (tile != null && tile.HasTile)
                        {
                            if (checkExplosions)
                            {
                                if (!TileLoader.CanExplode(i, j))
                                    canKillTile = false;
                            }

                            if (Main.tileContainer[tile.TileType])
                                canKillTile = false;
                            if (!TileLoader.CanKillTile(i, j, tile.TileType, ref t) || !TileLoader.CanKillTile(i, j, tile.TileType, ref f))
                                canKillTile = false;
                            if (tileExcludeList.Contains(tile.TileType))
                                canKillTile = false;

                            if (canKillTile)
                            {
                                WorldGen.KillTile(i, j, false, false, false);
                                if (!tile.HasTile && Main.netMode != NetmodeID.SinglePlayer)
                                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j, 0f, 0, 0, 0);
                            }
                        }

                        if (canKillTile)
                        {
                            for (int x = i - 1; x <= i + 1; x++)
                            {
                                for (int y = j - 1; y <= j + 1; y++)
                                {
                                    bool canExplode = true;
                                    if (checkExplosions)
                                        canExplode = WallLoader.CanExplode(x, y, Main.tile[x, y].WallType);
                                    if (wallExcludeList.Contains(Main.tile[x, y].WallType))
                                        canKillWalls = false;

                                    if (Main.tile[x, y] != null && Main.tile[x, y].WallType > WallID.None && canKillWalls && canExplode)
                                    {
                                        WorldGen.KillWall(x, y, false);
                                        if (Main.tile[x, y].WallType == WallID.None && Main.netMode != NetmodeID.SinglePlayer)
                                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, x, y, 0f, 0, 0, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
