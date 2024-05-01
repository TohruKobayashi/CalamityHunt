using CalamityHunt.Common.Systems;
using CalamityHunt.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Melee
{
    public class ParasanguineHeld : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.noEnchantmentVisuals = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Mode => ref Projectile.ai[1];

        public ref Player Player => ref Main.player[Projectile.owner];

        private float slashRotation;

        public override void AI()
        {
            if (!Player.active || Player.dead || Player.noItems || Player.CCed)
            {
                Projectile.Kill();
                return;
            }

            if (Time <= 1 && Mode != 3)
            {
                Projectile.velocity = Player.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 5f;
                Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
                Projectile.scale = 1.3f * Player.GetAdjustedItemScale(Player.HeldItem);
                Projectile.localNPCHitCooldown = 12;
            }

            float rotation = 0;
            float addRot = 0;
            bool canKill = false;

            SoundStyle swingSound = SoundID.DD2_MonkStaffSwing;
            swingSound.MaxInstances = 0;
            swingSound.Volume = 1.5f;

            float speed = (Player.itemAnimationMax / 35f) / (0.9f + Player.GetTotalAttackSpeed(DamageClass.Melee) * 0.1f);

            switch (Mode)
            {
                default:
                case 0:
                    rotation = -2.1f + SwingProgress(MathF.Pow(Utils.GetLerpValue(0, (int)(45 * speed), Time, true), 1.5f)) * 4f;

                    if (Time > (int)(15 * speed) && Time < (int)(40 * speed))
                    {
                        Projectile.damage = Player.GetWeaponDamage(Player.HeldItem);

                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 70f + Main.rand.NextVector2Circular(70, 30).RotatedBy(Projectile.rotation);
                            Dust blood = Dust.NewDustPerfect(pos, DustID.Blood, Main.rand.NextVector2Circular(4, 4), 0, Color.DarkRed, 1f + Main.rand.NextFloat());
                            blood.noGravity = true;
                        }
                    }
                    else
                        Projectile.damage = 0;

                    if (Time > (int)(25 * speed) && Time < (int)(33 * speed))
                    {
                        Vector2 stickyVelocity = (Projectile.velocity.ToRotation() + (rotation * 0.3f - 0.15f) * Projectile.direction).ToRotationVector2() * Main.rand.NextFloat(16f, 25f) * Utils.GetLerpValue(0, 35, Time, true);
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 60f, stickyVelocity + Player.velocity, ModContent.ProjectileType<ParasanguineBlood>(), Projectile.damage / 3, 0.5f, Player.whoAmI);
                    }

                    if (Time > (int)(45 * speed))
                    {
                        if (Player.controlUseItem)
                        {
                            Time = -1;
                            Mode++;
                        }
                        else if (Time > (int)(65 * speed))
                            canKill = true;

                        if (Player.altFunctionUse == 1 && Player.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f)
                        {
                            Time = -1;
                            Mode = 2;
                        }
                    }

                    if (Time == 22)
                        SoundEngine.PlaySound(swingSound.WithPitchOffset(0.2f), Projectile.Center);

                    Projectile.spriteDirection = -Projectile.direction;

                    break;

                case 1:

                    rotation = 2f - SwingProgress(MathF.Pow(Utils.GetLerpValue(0, (int)(35 * speed), Time, true), 3f)) * 4f;

                    if (Time > (int)(20 * speed) && Time < (int)(32 * speed))
                    {
                        Vector2 stickyVelocity = (Projectile.velocity.ToRotation() + (rotation * 0.3f - 0.77f) * Projectile.direction).ToRotationVector2() * Main.rand.NextFloat(17f, 27f) * Utils.GetLerpValue(0, (int)(35 * speed), Time, true) * Utils.GetLerpValue((int)(35 * speed), (int)(30 * speed), Time, true) * 1.1f;
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 70f, stickyVelocity + Player.velocity, ModContent.ProjectileType<ParasanguineBlood>(), Projectile.damage / 3, 0.5f, Player.whoAmI);
                    }

                    if (Time > (int)(40 * speed))
                    {
                        Projectile.damage = 0;

                        if (Player.controlUseItem)
                        {
                            Time = -1;
                            Mode--;
                        }
                        else if (Time > (int)(55 * speed))
                            canKill = true;

                        if (Player.altFunctionUse == 1 && Player.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f)
                        {
                            Time = -1;
                            Mode = 2;
                        }
                    }
                    else
                    {
                        Projectile.damage = Player.GetWeaponDamage(Player.HeldItem);

                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 70f + Main.rand.NextVector2Circular(70, 30).RotatedBy(Projectile.rotation);
                            Dust blood = Dust.NewDustPerfect(pos, DustID.Blood, Main.rand.NextVector2Circular(4, 4), 0, Color.DarkRed, 1f + Main.rand.NextFloat());
                            blood.noGravity = true;
                        }
                    }
                    if (Time == (int)(15 * speed))
                        SoundEngine.PlaySound(swingSound.WithPitchOffset(0.2f), Projectile.Center);
                    
                    Projectile.spriteDirection = Projectile.direction;

                    break;

                case 2:

                    rotation = 0;
                    Projectile.damage = 0;
                    Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;

                    if (Time < 25)
                    {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Player.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 5f, 0.3f * Utils.GetLerpValue(25, 0, Time, true));

                        Projectile.frame = (int)(Utils.GetLerpValue(5, 15, Time, true) * 2);
                        Projectile.scale = 1.4f * Player.GetAdjustedItemScale(Player.HeldItem) * (0.7f + MathF.Pow(Utils.GetLerpValue(25, 0, Time, true), 2f) * 0.2f);
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(120, 120) + Projectile.velocity * 20;
                            Dust blood = Dust.NewDustPerfect(pos, DustID.Blood, pos.DirectionTo(Projectile.Center) * Main.rand.NextFloat(5f), 100, Color.DarkRed, 1f + Main.rand.NextFloat(2f));
                            blood.noGravity = true;
                        }
                    }
                    else
                    {
                        Player.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood -= 100;
                        Projectile.frame = 3;
                        Projectile.scale = 1.4f * Player.GetAdjustedItemScale(Player.HeldItem) * (0.7f + MathF.Sqrt(Utils.GetLerpValue(25, 27, Time, true)) * 0.3f);
                    }

                    if (Time == 27)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath1.WithPitchOffset(-0.5f).WithVolumeScale(0.5f), Projectile.Center);
                        SoundEngine.PlaySound(SoundID.Item100, Projectile.Center);
                    }
                    if (Time > 25 && Time < 35)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 stickyVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(1f) * Main.rand.NextFloat(18f, 32f) * Utils.GetLerpValue(0, 35, Time, true);
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 70f, stickyVelocity + Player.velocity, ModContent.ProjectileType<ParasanguineBlood>(), Player.GetWeaponDamage(Player.HeldItem) / 2, 0.5f, Player.whoAmI);
                        }
                    }

                    if (Time > 40)
                    {
                        if (Player.controlUseItem && Player.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f)
                        {
                            Time = -1;
                            Mode++;
                        }
                        else if (Time > 50)
                            canKill = true;
                    }


                    break;                
                
                case 3:

                    addRot = -MathHelper.TwoPi * 4f * MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(15, 75, Time, true));
                    rotation = (1f - SwingProgress(MathF.Pow(Utils.GetLerpValue(0, 90, Time, true), 1.5f)) * 3f) * MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(0, 10, Time, true));
                    Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(Projectile.direction * 5, 0), 0.05f);
                    Projectile.frame = (int)(Utils.GetLerpValue(93, 87, Time, true) * 3);
                    Projectile.scale = 1.4f * Player.GetAdjustedItemScale(Player.HeldItem) * (1f + MathF.Sqrt(Utils.GetLerpValue(25, 0, Time, true) * Utils.GetLerpValue(95, 75, Time, true)) * 0.2f);

                    if (Time < 30 && Time > 70)
                        Projectile.damage = 0;
                    else
                    {
                        Player.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood -= 100;
                        Projectile.damage = Player.GetWeaponDamage(Player.HeldItem);

                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 80f + Main.rand.NextVector2Circular(120, 40).RotatedBy(Projectile.rotation);
                            Vector2 vel = (Projectile.rotation - MathHelper.PiOver2 * Projectile.spriteDirection).ToRotationVector2() * Main.rand.NextFloat(5f);
                            Dust blood = Dust.NewDustPerfect(pos, DustID.Blood, vel.RotatedByRandom(1f), 0, Color.DarkRed, 1f + Main.rand.NextFloat());
                            blood.noGravity = true;
                        }
                    }

                    if (Time >= 30 && Time <= 65)
                    {
                        Projectile.localNPCHitCooldown = 10;

                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 stickyVelocity = (Projectile.rotation + i * 0.2f * Projectile.direction).ToRotationVector2() * Main.rand.NextFloat(18f, 32f) * Utils.GetLerpValue(0, 35, Time, true);
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 60f, stickyVelocity + Player.velocity, ModContent.ProjectileType<ParasanguineBlood>(), Projectile.damage / 2, 0.5f, Player.whoAmI);
                        }

                        if ((Time - 20) % 10 == 0)
                            SoundEngine.PlaySound(swingSound.WithPitchOffset(0.5f), Projectile.Center);

                    }

                    if (Time > 95)
                    {
                        if (Player.controlUseItem)
                        {
                            Time = -1;
                            Mode = 0;
                        }
                        else
                            canKill = true;
                    }

                    Projectile.spriteDirection = Projectile.direction;

                    break;
            }

            if (Projectile.damage > 0)
            {
                Projectile.EmitEnchantmentVisualsAt(Projectile.Center + Projectile.rotation.ToRotationVector2() * 100 - new Vector2(50), 100, 100);

                if (Mode < 2)
                {
                    WeaponBars.DisplayBar();
                    Player.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood += 10;
                    Player.GetModPlayer<GoozmaWeaponsPlayer>().parasolBloodWaitTime += 20;
                }
            }

            if (canKill)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation() + (rotation + MathHelper.WrapAngle(addRot)) * Projectile.direction;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Vector2 position = Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            if (Mode == 3)
            {
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() + (rotation * 0.5f - MathF.Sin(MathHelper.WrapAngle(addRot)) * 0.3f) * Projectile.direction - MathHelper.PiOver2);
                position = Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() + (rotation * 0.7f - MathF.Sin(MathHelper.WrapAngle(addRot)) * 0.05f) * Projectile.direction - MathHelper.PiOver2);
            }
            position.Y += Player.gfxOffY;
            Projectile.Center = position;

            Player.itemAnimation = 2;
            Player.itemTime = 2;
            Player.ChangeDir(Projectile.direction);
            Player.heldProj = Projectile.whoAmI;

            Time++;

            slashRotation = slashRotation.AngleLerp(Projectile.rotation, 0.6f);
            for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
                Projectile.oldRot[i] = Projectile.oldRot[i].AngleLerp(Projectile.oldRot[i - 1], 0.4f);

            Projectile.oldRot[0] = Projectile.oldRot[0].AngleLerp(slashRotation, 0.5f);

            if (hitCD > 0)
                hitCD--;
        }

        public float SwingProgress(float x)
        {
            float[] functions = new float[]
            {
                -0.1f * MathF.Cbrt(x / 0.15f),
                MathF.Pow(2.1f * x - 0.315f, 3f),
                MathF.Pow(3.3f * x - 2.475f, 3f) + 1f,
                -2f * MathF.Pow(x - 0.85f, 2f) + 1.05f
            };

            if (x < 0f)
                return 0f;
            if (x < 0.25f)
                return MathHelper.Lerp(functions[0], functions[1], Utils.GetLerpValue(0.1f, 0.25f, x, true));
            if (x < 0.6f)
                return MathHelper.Lerp(functions[1], functions[2], Utils.GetLerpValue(0.5f, 0.6f, x, true));
            if (x < 0.7f)
                return MathHelper.Lerp(functions[2], functions[3], Utils.GetLerpValue(0.6f, 0.7f, x, true));
            else
                return MathHelper.Lerp(functions[3], 1f, Utils.GetLerpValue(0.7f, 0.97f, x, true));
        }

        public int hitCD;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Mode < 2 && hitCD <= 0)
            {
                hitCD = 12;
                Player.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood += 200 + (int)(damageDone * 0.01f);
                Player.GetModPlayer<GoozmaWeaponsPlayer>().parasolBloodWaitTime += 30;

                CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y - 30, Player.width, 30), new Color(150, 10, 0), 200 + (int)(damageDone * 0.01f), true, true);
            }

            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.Center);

            WeaponBars.DisplayBar();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Mode < 2 && hitCD <= 0)
            {
                hitCD = 12;

                Player.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood += 200 + (int)(info.Damage * 0.01f);
                Player.GetModPlayer<GoozmaWeaponsPlayer>().parasolBloodWaitTime += 30;

                CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y - 30, Player.width, 30), new Color(150, 10, 0), 200 + (int)(info.Damage * 0.01f), true, true);
            }

            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.Center);

            WeaponBars.DisplayBar();
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)(180 * Player.GetAdjustedItemScale(Player.HeldItem));

            hitbox.Width = size;
            hitbox.Height = size;
            hitbox.Location = (Projectile.Center + Projectile.rotation.ToRotationVector2() * 120f - new Vector2(size * 0.5f)).ToPoint();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D flare = TextureAssets.Extra[89].Value;
            Texture2D slash = AssetDirectory.Textures.SwordSwing.Bloody.Value;
            Texture2D splatTexture = ModContent.Request<Texture2D>(Texture + "Splat").Value;
            Rectangle frame = texture.Frame(1, 4, 0, Projectile.frame);
            Vector2 origin = frame.Size() * new Vector2(0.1f, 0.5f - 0.35f * Projectile.direction);
            SpriteEffects spriteEffects = Projectile.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            if (Mode != 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    float speed = (Player.itemAnimationMax / 35f) / Player.GetTotalAttackSpeed(DamageClass.Melee);
                    float slashPower = 0f;
                    switch (Mode)
                    {
                        case 0:

                            slashPower = Utils.GetLerpValue((int)(20 * speed), (int)(35 * speed), Time + i * 2f - 6f, true) * Utils.GetLerpValue((int)(40 * speed), (int)(35 * speed), Time - i * 3f + 2f, true);

                            break;

                        case 1:

                            slashPower = Utils.GetLerpValue((int)(20 * speed), (int)(35 * speed), Time + i * 2f - 6f, true) * Utils.GetLerpValue((int)(40 * speed), (int)(35 * speed), Time - i * 3f + 2f, true);

                            break;

                        case 3:

                            slashPower = Utils.GetLerpValue(25, 40, Time + i * 2f - 6f, true) * Utils.GetLerpValue(90, 70, Time - i * 3f + 2f, true);

                            break;
                    }

                    SpriteEffects slashEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Rectangle slashFrame = slash.Frame(1, 4, 0, i);
                    Color slashColor = Color.Lerp(new Color(8, 0, 0), new Color(255, 0, 0), (i / 3f + 0.01f) * (0.3f + slashPower * 0.7f)) * 0.7f;
                    Main.EntitySpriteDraw(slash, Projectile.Center - Main.screenPosition, slashFrame, slashColor * slashPower, Projectile.oldRot[Math.Clamp(i, 0, 3)] - MathHelper.PiOver2 - (MathHelper.PiOver2 - 0.3f + (i == 3 ? 0.3f : 0)) * Projectile.spriteDirection, slashFrame.Size() * new Vector2(0.5f + 0.02f * Projectile.spriteDirection, 0.53f), Projectile.scale * 2.2f, slashEffects, 0);
                }
            }

            float strength = Utils.GetLerpValue(0.5f, 0.6f, Player.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent, true);
            if (Mode > 1)
                strength = 1f;
            if (Player.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f || Mode > 1)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 off = new Vector2(3).RotatedBy(MathHelper.TwoPi / 6f * i + Projectile.rotation);
                    Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, frame, Color.Black * 0.5f * strength, Projectile.rotation - MathHelper.PiOver4 * Projectile.direction, origin, Projectile.scale, spriteEffects, 0);
                }                
                for (int i = 0; i < 6; i++)
                {
                    Vector2 off = new Vector2(3).RotatedBy(MathHelper.TwoPi / 6f * i + Projectile.rotation);
                    Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, frame, new Color(211, 10, 15, 0) * strength, Projectile.rotation - MathHelper.PiOver4 * Projectile.direction, origin, Projectile.scale, spriteEffects, 0);
                }
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation - MathHelper.PiOver4 * Projectile.direction, origin, Projectile.scale, spriteEffects, 0);

            if (Player.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f && Mode < 2)
            {
                Vector2 tip = new Vector2(Projectile.scale * 92, -5 * Projectile.direction).RotatedBy(Projectile.rotation);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), Color.Black * 0.9f * strength, 0, flare.Size() * 0.5f, new Vector2(0.4f, 1.5f), spriteEffects, 0);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), Color.Black * 0.9f * strength, MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.4f, 2f), spriteEffects, 0);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), new Color(211, 10, 15, 0) * strength, 0, flare.Size() * 0.5f, new Vector2(0.5f, 1.5f), spriteEffects, 0);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), new Color(211, 10, 15, 0) * strength, MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.5f, 2f), spriteEffects, 0);
            }
       
            if (Mode == 2 && Time >= 25 && Time <= 45)
            {
                Rectangle splatFrame = splatTexture.Frame(1, 6, 0, (int)(Utils.GetLerpValue(25, 45, Time, true) * 6f));
                Vector2 tip = new Vector2(Projectile.scale * 60, -5 * Projectile.direction).RotatedBy(Projectile.rotation);
                Main.EntitySpriteDraw(splatTexture, Projectile.Center + tip * 1.03f - Main.screenPosition, splatFrame, Color.Black * 0.8f, Projectile.rotation + MathHelper.PiOver2, splatFrame.Size() * new Vector2(0.5f, 1f), 0.7f + Projectile.scale * 0.5f, 0, 0);
                Main.EntitySpriteDraw(splatTexture, Projectile.Center + tip - Main.screenPosition, splatFrame, new Color(255, 8, 20) * 0.6f, Projectile.rotation + MathHelper.PiOver2, splatFrame.Size() * new Vector2(0.5f, 1f), 0.66f + Projectile.scale * 0.5f, 0, 0);
            }

            return false;
        }
    }
}
