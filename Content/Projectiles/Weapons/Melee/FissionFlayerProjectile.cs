using System;
using System.IO;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Items.Weapons.Melee;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Melee
{
    public class FissionFlayerProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.manualDirectionChange = true;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 5;

            //if (ModLoader.HasMod(HUtils.CalamityMod)) {
            //    DamageClass d;
            //    Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
            //    calamity.TryFind("RogueDamageClass", out d);
            //    Projectile.DamageType = d;
            //}
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Mode => ref Projectile.ai[1];
        public ref float HitDelay => ref Projectile.ai[2];

        public Player Player => Main.player[Projectile.owner];

        public Vector2 rodPosition;
        public Vector2 rodDirection;
        public float rodRotation;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((sbyte)Projectile.direction);
            writer.Write(rodRotation);
            writer.WriteVector2(rodPosition);
            writer.WriteVector2(rodDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.direction = reader.ReadSByte();
            rodRotation = reader.ReadSingle();
            rodPosition = reader.ReadVector2();
            rodDirection = reader.ReadVector2();
        }

        public LoopingSound sawSound;
        public float sawSoundVolume;
        public float sawSoundPitch;

        public override void AI()
        {
            Player.heldProj = Projectile.whoAmI;
            Projectile.scale = Player.GetAdjustedItemScale(Player.HeldItem);
            float speed = 1.3f / (0.5f + Player.GetAttackSpeed(Projectile.DamageType) * 0.5f);

            switch (Mode) {
                default:
                    if (Main.myPlayer == Projectile.owner && PlayerInput.MouseInfo.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) {
                        if (Player.HeldItem.ModItem is FissionFlayer rightClickItem) {
                            rightClickItem.spin = Math.Clamp(rightClickItem.spin + Utils.GetLerpValue(-10, 500, Time, true), 0f, 1f);
                        }

                        Projectile.timeLeft = 6;
                    }

                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Player.MountedCenter.DirectionTo(Main.MouseWorld), 0.1f);
                    Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
                    Player.ChangeDir(Projectile.direction);
                    Player.SetDummyItemTime(1);

                    Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, (Projectile.velocity.ToRotation() + MathHelper.PiOver2) * Player.gravDir - (MathHelper.PiOver2 * Player.gravDir + 0.4f) * Projectile.direction);
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, (Projectile.velocity.ToRotation() + MathHelper.PiOver2) * Player.gravDir - (MathHelper.PiOver2 * Player.gravDir - 0.5f) * Projectile.direction);

                    rodPosition = Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.velocity.ToRotation() + MathHelper.PiOver2 - (MathHelper.PiOver2 - 0.5f) * Projectile.direction * Player.gravDir) + new Vector2(0, Player.gfxOffY);
                    rodDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    rodRotation = rodDirection.ToRotation() - 0.1f * Projectile.direction * Player.gravDir;

                    Projectile.Center = GetSawbladeHomePos(rodPosition);
                    Projectile.netUpdate = true;

                    sawSoundVolume = Utils.GetLerpValue(0, 4f, Time, true) * Utils.GetLerpValue(0, 4f, Projectile.timeLeft, true);
                    sawSoundPitch = Utils.GetLerpValue(0, 4f, Time, true) * Utils.GetLerpValue(0, 4f, Projectile.timeLeft, true) * 0.2f;

                    sawSound ??= new LoopingSound(AssetDirectory.Sounds.TestLoop, new Terraria.Audio.ProjectileAudioTracker(Projectile).IsActiveAndInGame);
                    sawSound.PlaySound(() => Projectile.Center, () => sawSoundVolume, () => sawSoundPitch - 1f);

                    break;

                case 0:

                    Vector2 newPos = Main.MouseWorld;
                    Player.LimitPointToPlayerReachableArea(ref newPos);

                    if (Time <= 0) {
                        newPos += Main.rand.NextVector2Circular(50, 50).RotatedBy(Player.MountedCenter.AngleTo(newPos));
                        float distance = Projectile.Distance(newPos) * 1.05f;
                        Projectile.velocity = Projectile.DirectionTo(newPos).RotatedBy(-0.3f * Projectile.direction) * Math.Clamp(distance, 400, 900) / 12f;
                        rodDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    }

                    Projectile.direction = rodDirection.X > 0 ? 1 : -1;
                    Player.ChangeDir(Projectile.direction);

                    float swingProgress = SwingEase(Utils.GetLerpValue(0f, 30f * speed, Time, true) * Utils.GetLerpValue(55f * speed, 25f * speed, Time, true));
                    float returnProgress = Utils.GetLerpValue(65f * speed, 55f * speed, Time, true) * Utils.GetLerpValue(53f * speed, 55f * speed, Time, true) * 0.5f;
                    rodDirection = Vector2.Lerp(rodDirection, Player.MountedCenter.DirectionTo(newPos), 0.01f);
                    rodRotation = rodDirection.ToRotation() + (MathHelper.Lerp(-MathHelper.ToRadians(140), MathHelper.ToRadians(110), swingProgress) - returnProgress) * Projectile.direction * Player.gravDir;
                    float rodRotationForHand = rodDirection.ToRotation() + (MathHelper.Lerp(-MathHelper.ToRadians(100), MathHelper.ToRadians(100), swingProgress) - returnProgress) * Projectile.direction * Player.gravDir;

                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rodRotationForHand - MathHelper.PiOver2);

                    rodPosition = Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rodRotationForHand - MathHelper.PiOver2) + new Vector2(0, Player.gfxOffY);

                    Vector2 home = GetSawbladeHomePos(rodPosition);

                    Projectile.Center = home - Projectile.velocity;

                    if (Time == (int)(55 * speed)) {
                        SoundEngine.PlaySound(SoundID.Tink.WithPitchOffset(1f), Projectile.Center);
                    }

                    if (Time < (int)(55 * speed)) {
                        Projectile.timeLeft = (int)(15 * speed);
                    }

                    break;

                case 1:

                    newPos = Main.MouseWorld;
                    Player.LimitPointToPlayerReachableArea(ref newPos);

                    if (Time <= 0) {
                        newPos += Main.rand.NextVector2Circular(50, 50).RotatedBy(Player.MountedCenter.AngleTo(newPos));
                        float distance = Projectile.Distance(newPos) * 1.05f;
                        Projectile.velocity = Projectile.DirectionTo(newPos).RotatedBy(-0.3f * Projectile.direction) * Math.Clamp(distance, 400, 900) / 12f;
                        rodDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    }

                    Projectile.direction = rodDirection.X > 0 ? 1 : -1;
                    Player.ChangeDir(Projectile.direction);

                    float flingProgress = SwingEase(Utils.GetLerpValue(0f, 40f * speed, Time, true));
                    float fullRotationProgress = MathHelper.SmoothStep(0f, MathHelper.TwoPi, Utils.GetLerpValue(2f * speed, 20f * speed, Time, true)) + MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(15f * speed, 45f * speed, Time, true)) - 1f;
                    returnProgress = Utils.GetLerpValue(55 * speed, 45 * speed, Time, true) * Utils.GetLerpValue(43f * speed, 44f * speed, Time, true) * 0.5f;

                    rodDirection = Vector2.Lerp(rodDirection, Player.MountedCenter.DirectionTo(newPos), 0.01f);
                    rodRotation = rodDirection.ToRotation() + (MathHelper.Lerp(-MathHelper.ToRadians(140), MathHelper.ToRadians(110), flingProgress) + fullRotationProgress + returnProgress) * Projectile.direction * Player.gravDir;
                    rodRotationForHand = rodDirection.ToRotation() + (MathHelper.Lerp(-MathHelper.ToRadians(100), MathHelper.ToRadians(100), flingProgress) + returnProgress) * Projectile.direction * Player.gravDir;

                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rodRotationForHand - MathHelper.PiOver2);

                    rodPosition = Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rodRotationForHand - MathHelper.PiOver2) + new Vector2(0, Player.gfxOffY);

                    home = GetSawbladeHomePos(rodPosition);

                    if (Main.myPlayer == Projectile.owner) {

                        if (Time < (int)(20 * speed)) {
                            Projectile.Center = home - Projectile.velocity;
                        }
                        else {
                            Projectile.velocity = Projectile.velocity.RotatedBy(0.04f * Projectile.direction) * 0.98f;
                            Projectile.Center = Vector2.SmoothStep(Projectile.Center, GetSawbladeHomePos(rodPosition) - Projectile.velocity, MathF.Pow(Utils.GetLerpValue(26f * speed, 45f * speed, Time, true), 2f));
                        }

                        //Projectile.Center = Vector2.SmoothStep(GetSawbladeHomePos(rodPosition), Player.MountedCenter + Projectile.velocity, sawbladeProgress) - Projectile.velocity;
                    }

                    if (Time == (int)(43 * speed)) {
                        SoundEngine.PlaySound(SoundID.Tink.WithPitchOffset(1f), Projectile.Center);
                    }
                   
                    if (Time < (int)(45 * speed)) {
                        Projectile.timeLeft = (int)(15 * speed);
                    }

                    break;
            }

            if (Player.HeldItem.ModItem is FissionFlayer item) {
                Projectile.rotation += MathF.Max(item.spin, 0.2f) * Projectile.direction * Player.gravDir;
                item.spin = Math.Max(0, item.spin - 0.005f);
                sawSoundPitch += 0.05f + item.spin * 0.5f;
                rodPosition += Main.rand.NextVector2Circular(2, 2) * MathF.Cbrt(item.spin);
            }

            if (HitDelay > 0) {
                HitDelay--;
            }
            else {
                Time++;
                Projectile.localAI[0]++;
            }
        }

        public static float SwingEase(float x)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;
            return x < 0.5f
              ? (MathF.Pow(2f * x, 2f) * ((c2 + 1) * 2f * x - c2)) / 2f
              : (MathF.Pow(2f * x - 2f, 2f) * ((c2 + 1f) * (x * 2f - 2f) + c2) + 2f) / 2f;
        }

        public Vector2 GetSawbladeHomePos(Vector2 rodPos) => rodPos + new Vector2(63f, 0).RotatedBy(rodRotation) * Projectile.scale;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 180);
            HitDelay += 5;
        }

        public override void Load()
        {
            rodTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Rod").Value;
        }

        public static Texture2D rodTexture;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow[0].Value;

            SpriteEffects spriteEffects = Projectile.direction * Player.gravDir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.2f, 0, 0);

            Main.EntitySpriteDraw(rodTexture, rodPosition - Main.screenPosition, rodTexture.Frame(), Color.White, rodRotation + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.direction * Player.gravDir, rodTexture.Size() * new Vector2(0.5f - 0.13f * Player.direction * Player.gravDir, 0.63f), Projectile.scale * 1.2f, spriteEffects, 0);

            return false;
        }
    }
}
