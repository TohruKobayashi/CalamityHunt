using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Items.Armor.Shogun;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players
{
    public class ShogunArmorPlayer : ModPlayer
    {
        public bool active;
        public float slamPower;
        public bool slamming;
        public int bunnyHopCounter;
        public int inertiaTimer;
        public int dashTime;

        public List<IShogunSpinModifier> spinEffects = new List<IShogunSpinModifier>();

        public override void Initialize()
        {
            spinEffects ??= new List<IShogunSpinModifier>();
            spinEffects.Clear();

            spinEffects.Add(new ShogunDashSpinModifier());
            spinEffects.Add(new ShogunSlamSpinModifier());
            spinEffects.Add(new ShogunBHopSpinModifier());
        }

        public override void FrameEffects()
        {
            bool deactivatedEffect = false;
            float rotation = 0;

            foreach (IShogunSpinModifier spinEffect in spinEffects)
            {
                bool previousState = spinEffect.ActiveState;
                spinEffect.ActiveState = spinEffect.Active(Player, this);

                if (spinEffect.ActiveState != previousState)
                {
                    //If we turned active while before we weren't, reactivate the effect
                    if (!previousState)
                        spinEffect.Reactivate(Player, this);
                    else
                        deactivatedEffect = true;
                }

                if (spinEffect.ActiveState)
                    spinEffect.Update(Player, this, ref rotation);
            }

            //Set the rotation if we have any active effects
            if (spinEffects.Any(s => s.ActiveState))
            {
                Player.fullRotation = rotation;
                Player.fullRotationOrigin = Player.Size * 0.5f;
            }
            //If there's no more active effects after we deactivated one, that means we have to reset the rotation
            else if (deactivatedEffect)
                 Player.fullRotation = 0; 

            if (active)
            {
                if (Player.dashDelay < 0)
                {
                    //Main.SetCameraLerp(0.1f, 25);
                    for (int i = 0; i < 6; i++)
                        Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(25, 25), DustID.TintableDust, Player.velocity * -Main.rand.NextFloat(-0.5f, 1f), 100, Color.Black, 1f + Main.rand.NextFloat(1.5f)).noGravity = true;

                    ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Player.Center + Main.rand.NextVector2Circular(25, 25), Player.velocity * -Main.rand.NextFloat(-0.6f, 0.6f), Player.shirtColor, 0.5f + Main.rand.NextFloat());
                }
                //else if (slamPower > 0)
                //{
                //    Main.SetCameraLerp(0.1f, 25);
                //}



                //If the wing logic isnt the same as the visual wings that means its vanity. Vanilla does the same to check if the wings are real or not
                bool playerHasVanityWings = Player.wings > 0 && Player.wingsLogic != Player.wings;
                if (!playerHasVanityWings)
                {
                    Player.wings = EquipLoader.GetEquipSlot(Mod, "ShogunChestplate", EquipType.Wings);
                }
            }
        }

        public override void PostUpdateRunSpeeds()
        {
            bool inAir = !WorldGen.SolidOrSlopedTile(Main.tile[(Player.Bottom / 16f).ToPoint()]) && !Collision.SolidCollision(Player.position, Player.width, Player.height);
            if (active)
            {
                if (Player.controlDown && !Player.mount.Active)
                    Player.gravity *= 1.1111f;

                if (Player.controlDown && Player.velocity.Y > 1f && !Player.mount.Active)
                    slamPower++;
                else
                    slamPower = 0;

                slamPower = Math.Clamp(slamPower, 0, 10);

                Player.maxFallSpeed += slamPower;

                if (inertiaTimer > 0)
                {
                    inertiaTimer--;
                    Player.runSlowdown *= 0.33f;
                }

                if (Player.dashDelay < 0)
                {
                    inertiaTimer = 1;

                    if (Player.controlJump && Player.releaseJump)
                    {
                        Player.dashDelay = 0;
                        inertiaTimer = 60;
                    }
                }
                else if (slamPower > 0)
                    Player.velocity.X += (Player.direction * 0.2f - Player.velocity.X * 0.01f) * Math.Clamp(Player.velocity.X, 0, 1);

                if (Player.dashDelay > 0)
                    Player.dashDelay--;

                if (!inAir && slamPower > 6)
                    slamming = true;

                if (slamming)
                {
                    bunnyHopCounter += 25;
                    for (int i = 0; i < 40; i++)
                        Dust.NewDustPerfect(Player.Bottom + Main.rand.NextVector2Circular(20, 5), DustID.TintableDust, Main.rand.NextVector2Circular(10, 1) - Vector2.UnitY * Main.rand.NextFloat(5f), 100, Color.Black, 1f + Main.rand.NextFloat(1.5f)).noGravity = true;

                    for (int i = 0; i < 5; i++)
                        ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Player.Bottom + Main.rand.NextVector2Circular(30, 5), Main.rand.NextVector2Circular(6, 1) - Vector2.UnitY * Main.rand.NextFloat(2f), Player.shirtColor, 1f);

                    slamPower = 0;
                }

                if (bunnyHopCounter > 0)
                {
                    bunnyHopCounter--;

                    if (Player.controlJump || Player.dashDelay < 0)
                    {
                        bunnyHopCounter = -20;
                        Player.velocity.X *= 2f;
                        for (int i = 0; i < 40; i++)
                            Dust.NewDustPerfect(Player.Bottom + Main.rand.NextVector2Circular(20, 5), DustID.TintableDust, -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(7f) * (i / 40f) - new Vector2(Player.direction * 10f, 0f), 100, Color.Black, 1f + Main.rand.NextFloat(1.5f)).noGravity = true;

                        for (int i = 0; i < 5; i++)
                            ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Player.Bottom + Main.rand.NextVector2Circular(20, 5), -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(2f) * (i / 40f) - new Vector2(Player.direction * 10f, 0f), Player.shirtColor, 0.5f + Main.rand.NextFloat());

                    }
                }
                if (bunnyHopCounter < 0)
                {
                    bunnyHopCounter++;
                }

                if (inAir)
                    Player.maxRunSpeed *= 1.2f;
            }
            else
            {
                bunnyHopCounter = 0;
                slamPower = 0;
            }

            if (dashTime > 0)
                dashTime--;
        }

        public override void ResetEffects()
        {
            active = false;
            slamming = false;
        }

        public override void Load()
        {
            On_Player.DashMovement += ShogunDash;
        }

        public override void PostUpdateEquips()
        {
            if (active)
            {
                int wingSlot = EquipLoader.GetEquipSlot(Mod, "ShogunChestplate", EquipType.Wings);

                if (Player.equippedWings == null)
                {
                    Player.wingsLogic = wingSlot;
                    Player.wingTime = 1;
                    Player.wingTimeMax = 1;
                    Player.equippedWings = Player.armor[1];

                    if (ModLoader.HasMod("CalamityMod"))
                        ModLoader.GetMod("CalamityMod").Call("ToggleInfiniteFlight", Player, true);

                    if (Player.controlJump && Player.wingTime > 0f && !Player.GetJumpState(ExtraJump.CloudInABottle).Available && Player.jump == 0)
                    {
                        bool hovering = Player.TryingToHoverDown && !Player.merman;
                        if (hovering)
                        {
                            Player.runAcceleration += 5;
                            Player.maxRunSpeed += 5;

                            Player.velocity.Y *= 0.7f;
                            if (Player.velocity.Y > -2f && Player.velocity.Y < 1f)
                                Player.velocity.Y = 1E-05f;
                        }
                    }

                    if (Player.TryingToHoverUp && !Player.mount.Active)
                        Player.velocity.Y -= 1f;
                }

                Player.noFallDmg = true;
            }
        }

        private void ShogunDash(On_Player.orig_DashMovement orig, Player self)
        {
            if (self.GetModPlayer<ShogunArmorPlayer>().active)
            {
                //if (self.dashDelay > 0)
                //{
                //    if (self.eocDash > 0)
                //        self.eocDash--;

                //    if (self.eocDash == 0)
                //        self.eocHit = -1;

                //    self.dashDelay--;
                //}

                //else if (self.dashDelay < 0)
                //{
                //    self.StopVanityActions();

                //    self.doorHelper.AllowOpeningDoorsByVelocityAloneForATime(60);
                //    self.vortexStealthActive = false;

                //    self.dashDelay = 10;
                //    if (self.velocity.X < 0f)
                //        self.velocity.X = -Math.Max(self.accRunSpeed, self.maxRunSpeed) * 2f;
                //    else if (self.velocity.X > 0f)
                //        self.velocity.X = Math.Max(self.accRunSpeed, self.maxRunSpeed) * 2f;

                //    //self.dashType = 5;
                //}
                //else
                //{
                //    object[] parameters = new object[] { 0, false, null };
                //    self.GetType().GetMethod("DoCommonDashHandle", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, parameters);
                //    int dir = (int)parameters[0];
                //    bool dashing = (bool)parameters[1];
                //    if (dashing)
                //    {
                //        if (self.mount.Active)
                //            self.mount.Dismount(self);

                //        self.dashDelay = -1;
                //        self.velocity.X = dir * 20;
                //    }
                //}

                self.dashType = 1;
            }
           
            orig(self);
        }

        public override void PostUpdateMiscEffects()
        {
            if (active)
            {
            Player.buffImmune[BuffID.Silenced] = true;
            Player.buffImmune[BuffID.Cursed] = true;
            Player.buffImmune[BuffID.OgreSpit] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Webbed] = true;
            Player.buffImmune[BuffID.Stoned] = true;
            Player.buffImmune[BuffID.VortexDebuff] = true;
            Player.buffImmune[BuffID.Electrified] = true;
            Player.buffImmune[BuffID.Burning] = true;
            Player.buffImmune[BuffID.Stinky] = true;
            Player.buffImmune[BuffID.Dazed] = true;
            Player.buffImmune[BuffID.Venom] = true;
            Player.buffImmune[BuffID.CursedInferno] = true;
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                Player.buffImmune[calamity.Find<ModBuff>("Clamity").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("Dragonfire").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("DoGExtremeGravity").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("FishAlert").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("GlacialState").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("GodSlayerInferno").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("HolyFlames").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("IcarusFolly").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("MiracleBlight").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("Nightwither").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("Plague").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("VulnerabilityHex").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("Warped").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("WeakPetrification").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("WhisperingDeath").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("FabsolVodkaBuff").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("FrozenLungs").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("PopoNoselessBuff").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("SearingLava").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("ShellfishClaps").Type] = true;
                Player.buffImmune[calamity.Find<ModBuff>("BrimstoneFlames").Type] = true;
                calamity.Call("SetWearingRogueArmor", Player, true);
                calamity.Call("SetWearingPostMLSummonerArmor", Player, true);
                }
          }
          }
    }

    public interface IShogunSpinModifier
    {
        public bool ActiveState { get; set; }
        public bool Active(Player player, ShogunArmorPlayer modPlayer);
        public void Update(Player player, ShogunArmorPlayer modPlayer, ref float rotation);
        public void Reactivate(Player player, ShogunArmorPlayer modPlayer) { }
    }

    public class ShogunBHopSpinModifier : IShogunSpinModifier
    {
        public int smoothingTimer = 50;

        public bool ActiveState { get; set; }
        public bool Active(Player player, ShogunArmorPlayer modPlayer) => (modPlayer.bunnyHopCounter < 0 || smoothingTimer > 0) && modPlayer.slamPower <= 0;

        public void Update(Player player, ShogunArmorPlayer modPlayer, ref float rotation)
        {
            smoothingTimer--;
            rotation += player.velocity.X * 0.02f * (float)Math.Pow(Utils.GetLerpValue(0, 30, smoothingTimer, true), 0.6f);
        }

        public void Reactivate(Player player, ShogunArmorPlayer modPlayer)
        {
            smoothingTimer = 50;
        }
    }

    public class ShogunSlamSpinModifier : IShogunSpinModifier
    {
        public float lerpedRotation = 0;

        public bool ActiveState { get; set; }
        public bool Active(Player player, ShogunArmorPlayer modPlayer) => modPlayer.slamPower > 0;

        public void Update(Player player, ShogunArmorPlayer modPlayer, ref float rotation)
        {
            lerpedRotation = lerpedRotation.AngleLerp(-player.velocity.X * 0.05f, 0.1f);
            rotation += lerpedRotation;
        }

        public void Reactivate(Player player, ShogunArmorPlayer modPlayer)
        {
            lerpedRotation = player.fullRotation;
        }
    }


    public class ShogunDashSpinModifier : IShogunSpinModifier
    {
        public float accumulatedSpin = 0;

        public bool ActiveState { get; set; }
        public bool Active(Player player, ShogunArmorPlayer modPlayer) => modPlayer.active && player.dashDelay < 0;

        public void Update(Player player, ShogunArmorPlayer modPlayer, ref float rotation)
        {
            accumulatedSpin += (float)Math.Cbrt(player.velocity.X) * 0.2f * (1 + modPlayer.dashTime);
            rotation += accumulatedSpin;
        }

        public void Reactivate(Player player, ShogunArmorPlayer modPlayer)
        {
            accumulatedSpin = 0;
        }
    }
}
