using System;
using System.Collections.Generic;
using CalamityHunt.Common.Players;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner
{
    public class Goozmoem : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.manualDirectionChange = true;
            Projectile.hide = true;
        }

        public override bool? CanDamage() => State == (int)SlimeMinionState.Attacking && Time > 0;

        public ref float State => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public ref float AttackCD => ref Projectile.ai[2];

        public Player Player => Main.player[Projectile.owner];

        public Rope cordRope;

        public override void AI()
        {
            if (!Player.GetModPlayer<SlimeCanePlayer>().slimes || Player.dead || Player.GetModPlayer<SlimeCanePlayer>().SlimeRank() < SlimeCanePlayer.HighestRank) {
                Projectile.Kill();
            }
            else {
                Projectile.timeLeft = 2;
            }

            int target = -1;
            Projectile.Minion_FindTargetInRange(1200, ref target, true);
            bool hasTarget = false;
            if (target > -1) {
                hasTarget = true;
                if (Main.npc[target].active && Main.npc[target].CanBeChasedBy(Projectile)) {
                    Attack(target);
                }
                else {
                    hasTarget = false;
                }
            }
            if (!hasTarget) {
                Idle();
            }

            Dust gas = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 25), DustID.TintableDust, Main.rand.NextVector2Circular(1, 1), 150, Color.Black, 0.5f + Main.rand.NextFloat());
            gas.noGravity = true;
            gas.shader = GameShaders.Armor.GetSecondaryShader(Player.cMinion, Player);

            if (AttackCD > 0) {
                AttackCD--;
            }

            if (Projectile.frameCounter++ > 3) {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 6;
            }

            Vector2 cordStart = new Vector2(512) + new Vector2(10 * Projectile.direction, 2).RotatedBy(Projectile.rotation) * Projectile.scale * 0.5f;
            Vector2 cordEnd = new Vector2(512) + (Player.MountedCenter - Projectile.Center) * 0.5f;
            cordRope ??= new Rope(cordStart, cordEnd, 16, 2f, Vector2.Zero, 0.5f, 10);

            cordRope.segmentLength = MathF.Sqrt(Projectile.Distance(Player.MountedCenter)) * 0.5f;
            cordRope.StartPos = cordStart;
            cordRope.EndPos = cordEnd;
            cordRope.gravity = new Vector2(Player.direction * 1, -0.5f) * 0.2f;
            cordRope.Update();
        }

        public Vector2 HomePosition => Player.MountedCenter + new Vector2(-20 * Player.direction, -32);

        public void Idle()
        {
            Time = 0;

            if (Math.Abs(Projectile.velocity.X) < 1f) {
                Projectile.direction = Player.direction;
            }
            else {
                Projectile.direction = Math.Sign(Projectile.velocity.X);
            }

            if (Projectile.Distance(HomePosition) > 14) {
                Projectile.velocity += Projectile.DirectionTo(HomePosition).SafeNormalize(Vector2.Zero) * MathF.Max(0.1f, Projectile.Distance(HomePosition) * 0.03f);
            }
            else {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Main.rand.NextVector2Circular(2, 2), 0.3f);
                Projectile.velocity *= 0.9f;
                Projectile.netUpdate = true;
            }
            Projectile.velocity *= 0.9f;

            eyeOffset = Vector2.Lerp(eyeOffset, new Vector2(4 * Projectile.direction, 0) + Main.rand.NextVector2Circular(4, 4), 0.3f);
        }

        public void Attack(int whoAmI)
        {
            NPC target = Main.npc[whoAmI];

            if (Projectile.Distance(HomePosition) > 14) {
                Projectile.velocity += Projectile.DirectionTo(HomePosition).SafeNormalize(Vector2.Zero) * MathF.Max(0.1f, Projectile.Distance(HomePosition) * 0.03f);
            }
            else {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Main.rand.NextVector2Circular(2, 2), 0.3f);
                Projectile.velocity *= 0.9f;
                Projectile.netUpdate = true;
            }

            if (AttackCD == 0) {
                Time++;

                if (Time > 100) {
                    SoundStyle deep = SoundID.Item15 with { MaxInstances = 0, Pitch = -1f, PitchVariance = 0.2f, Volume = 0.5f };
                    SoundEngine.PlaySound(deep, Projectile.Center);
                    SoundStyle shootSound = AssetDirectory.Sounds.Weapons.GoozmoemRay;
                    SoundEngine.PlaySound(shootSound, Projectile.Center);

                    int direction = Main.rand.NextBool().ToDirectionInt();
                    Projectile laser = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<GoozmoemRay>(), Projectile.damage, Projectile.knockBack, Player.whoAmI);
                    laser.ai[0] = Projectile.AngleTo(target.Center) + Main.rand.NextFloat(0.5f, 1.5f) * direction;
                    laser.ai[1] = Projectile.AngleTo(target.Center) - Main.rand.NextFloat(0.5f, 1.5f) * direction;
                    laser.ai[2] = Projectile.whoAmI;
                    Time = 0;
                    AttackCD = 60;
                }
            }
            else {
                Projectile.direction = Projectile.Center.X > target.Center.X ? -1 : 1;
                eyeOffset = Vector2.Lerp(eyeOffset, new Vector2(4 * Projectile.direction, 0) + Main.rand.NextVector2Circular(4, 4), 0.3f);
            }

            Projectile.velocity *= 0.9f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindProjectiles.Add(index);

        public Vector2 eyeOffset;

        public static Texture2D eyeTexture;
        public static Texture2D crownTexture;
        public static RenderTargetDrawContent cordContent;
        public static RenderTargetDrawContent creatureContent;
        public VertexStrip cordStrip;

        public override void Load()
        {
            eyeTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Eye").Value;
            crownTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Crown").Value;
            Main.ContentThatNeedsRenderTargets.Add(creatureContent = new RenderTargetDrawContent());
            Main.ContentThatNeedsRenderTargets.Add(cordContent = new RenderTargetDrawContent());
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 scale = Projectile.scale * Vector2.One;

            cordContent.Request(1024, 1024, Projectile.whoAmI, DrawCord);

            creatureContent.Request(1024, 1024, Projectile.whoAmI, spriteBatch => {

                Vector2 creaturePos = new Vector2(512);
                GetGradientMapValues(out float[] brightnesses, out Vector3[] colors);
                Effect effect = AssetDirectory.Effects.HolographicGel.Value;
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
                effect.Parameters["colors"].SetValue(colors);
                effect.Parameters["brightnesses"].SetValue(brightnesses);
                effect.Parameters["baseToScreenPercent"].SetValue(1.05f);
                effect.Parameters["baseToMapPercent"].SetValue(-0.05f);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, effect);
                SpriteEffects direction = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                Rectangle frame = texture.Frame(1, 7, 0, Projectile.frame, -2, -2);
                if (cordRope != null) {

                    if (cordContent.IsTargetReady(Projectile.whoAmI)) {
                        Texture2D goozmoemCord = cordContent.GetTarget(Projectile.whoAmI);
                        Main.EntitySpriteDraw(goozmoemCord, creaturePos, goozmoemCord.Frame(), Color.White, Projectile.rotation, goozmoemCord.Size() * 0.5f, 2f, 0, 0);
                    }

                    Main.EntitySpriteDraw(texture, creaturePos, frame, Color.White, Projectile.rotation, new Vector2(frame.Width * 0.5f, 22), 1f, direction, 0);
                    Main.EntitySpriteDraw(eyeTexture, creaturePos + eyeOffset + new Vector2(0, -4).RotatedBy(Projectile.rotation), eyeTexture.Frame(), Color.White, Projectile.rotation, eyeTexture.Size() * 0.5f, 1f, direction, 0);
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
                Main.EntitySpriteDraw(crownTexture, creaturePos, frame, Color.White, Projectile.rotation, new Vector2(frame.Width * 0.5f, 22), 1f, direction, 0);
                spriteBatch.End();
            });

            if (creatureContent.IsTargetReady(Projectile.whoAmI)) {
                Texture2D goozmoem = creatureContent.GetTarget(Projectile.whoAmI);
                Main.EntitySpriteDraw(goozmoem, Projectile.Center - Main.screenPosition, goozmoem.Frame(), Color.White, Projectile.rotation, goozmoem.Size() * 0.5f, scale, 0, 0);
            }

            return false;
        }

        public void DrawCord(SpriteBatch spriteBatch)
        {
            List<Vector2> points = cordRope.GetPoints();
            points.Add(new Vector2(512) + (Player.MountedCenter - Projectile.Center) * 0.5f);
            Vector2[] positions = points.ToArray();
            float[] rotations = new float[positions.Length];
            for (int i = 1; i < positions.Length; i++) {
                rotations[i] = positions[i - 1].AngleTo(positions[i]);
            }

            rotations[positions.Length - 1] = rotations[positions.Length - 2];

            Effect effect = AssetDirectory.Effects.GoozmaCordMap.Value;
            effect.Parameters["uTransformMatrix"].SetValue(Matrix.Invert(Main.GameViewMatrix.EffectMatrix) * Matrix.CreateOrthographicOffCenter(0, 1024, 1024, 0, -1, 1));
            if (!Main.gameInactive) {
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f);
            }
            effect.Parameters["uTexture"].SetValue(AssetDirectory.Textures.Goozma.LiquidTrail.Value);
            effect.Parameters["uMap"].SetValue(AssetDirectory.Textures.ColorMap[0].Value);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            effect.CurrentTechnique.Passes[0].Apply();

            cordStrip ??= new VertexStrip();

            Color ColorFunc(float progress) => Color.White;
            float WidthFunc(float progress) => MathF.Pow(Utils.GetLerpValue(0f, 0.1f, progress, true) * Utils.GetLerpValue(1.2f, 0.5f, progress, true), 0.7f) * 7f;
            cordStrip.PrepareStrip(positions, rotations, ColorFunc, WidthFunc, Vector2.Zero, positions.Length, true);
            cordStrip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            spriteBatch.End();
        }

        private void GetGradientMapValues(out float[] brightnesses, out Vector3[] colors)
        {
            brightnesses = new float[10];
            colors = new Vector3[10];

            float maxBright = 0.667f;
            float rainbowStartOffset = 0.35f + Main.GlobalTimeWrappedHourly * 0.5f % (maxBright * 2f);
            //Calculate and store every non-modulo brightness, with the shifting offset. 
            //The first brightness is ignored for the moment, it will be relevant later. Setting it to -1 temporarily
            brightnesses[0] = -1;
            brightnesses[1] = rainbowStartOffset + 0.35f;
            brightnesses[2] = rainbowStartOffset + 0.42f;
            brightnesses[3] = rainbowStartOffset + 0.47f;
            brightnesses[4] = rainbowStartOffset + 0.51f;
            brightnesses[5] = rainbowStartOffset + 0.56f;
            brightnesses[6] = rainbowStartOffset + 0.61f;
            brightnesses[7] = rainbowStartOffset + 0.64f;
            brightnesses[8] = rainbowStartOffset + 0.72f;
            brightnesses[9] = rainbowStartOffset + 0.75f;

            //Pass the entire rainbow through modulo 1
            for (int i = 1; i < 10; i++) {
                brightnesses[i] = HUtils.Modulo(brightnesses[i], maxBright) * maxBright;
            }

            //Store the first element's value so we can find it again later
            float firstBrightnessValue = brightnesses[1];

            //Sort the values from lowest to highest
            Array.Sort(brightnesses);

            //Find the new index of the original first element after the list being sorted
            int rainbowStartIndex = Array.IndexOf(brightnesses, firstBrightnessValue);
            //Substract 1 from the index, because we are ignoring the currently negative first array slot.
            rainbowStartIndex--;

            //9 loop, filling a list of colors in a array of 10 elements (ignoring the first one)
            for (int i = 0; i < 9; i++) {
                colors[1 + (rainbowStartIndex + i) % 9] = GoozmaColorUtils.Oil[i];
            }

            //We always want a brightness at index 0 to be the lower bound
            brightnesses[0] = 0;
            //Make the color at index 0 be a mix between the first and last colors in the list, based on the distance between the 2.
            float interpolant = (1 - brightnesses[9]) / (brightnesses[1] + (1 - brightnesses[9]));
            colors[0] = Vector3.Lerp(colors[9], colors[0], interpolant);
        }
    }
}
