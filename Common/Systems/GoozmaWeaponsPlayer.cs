using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Content.Items.Weapons.Summoner;
using CalamityHunt.Content.Projectiles.Weapons.Magic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace CalamityHunt.Common.Systems
{
    public class GoozmaWeaponsPlayer : ModPlayer
    {
        public static int ParasolBloodMax = 8000;
        public int parasolBlood;
        public int parasolBloodWaitTime;
        public float ParasolBloodPercent => Math.Clamp((float)parasolBlood / ParasolBloodMax, 0, 1);

        public bool gobfloggerBuff;

        public bool crystalGauntletsHeld;
        public bool crystalGauntletsUseFingers;

        public int crystalGauntletsWaitTime;
        public float CrystalGauntletsCharge;

        public int crystalGauntletsClapTime;
        public Vector2 crystalGauntletsClapDir;

        public override void PostUpdateMiscEffects()
        {
            if (parasolBloodWaitTime > 0)
                parasolBloodWaitTime--;            
            
            if (crystalGauntletsWaitTime > 0)
                crystalGauntletsWaitTime--;

            if (parasolBlood > 0 && parasolBloodWaitTime <= 0)
                parasolBlood -= 100;            
            
            if (CrystalGauntletsCharge > 0 && crystalGauntletsWaitTime <= 0)
                CrystalGauntletsCharge *= 0.85f;

            if (CrystalGauntletsCharge < 0.002f)
                CrystalGauntletsCharge = 0f;

            parasolBlood = Math.Clamp(parasolBlood, 0, ParasolBloodMax);
            CrystalGauntletsCharge = Math.Clamp(CrystalGauntletsCharge, 0f, 1f);

            if (crystalGauntletsClapTime == 10)
            {
                SoundStyle clapSound = AssetDirectory.Sounds.Weapon.CrystalGauntletClap;
                SoundEngine.PlaySound(clapSound.WithVolumeScale(0.5f), Player.Center);

                Projectile boom = Projectile.NewProjectileDirect(Player.GetSource_ItemUse(Player.HeldItem), Player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<CrystalBoom>(), Player.HeldItem.damage, 0, Player.whoAmI);
                boom.ai[1] = 1f;
                boom.localAI[0] = Main.GlobalTimeWrappedHourly * 7f;

                foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<CrystalGauntletBallThrown>() && n.owner == Player.whoAmI))
                {
                    NPC target = projectile.FindTargetWithinRange(2400);
                    Vector2 dir = Vector2.Zero;
                    if (target != null)
                        dir = target.Center - projectile.Center;

                    projectile.velocity = Vector2.Zero;
                    projectile.timeLeft = 17;
                    SoundEngine.PlaySound(clapSound.WithVolumeScale(0.2f), Player.Center);

                    Vector2 oldDir = dir;
                    for (int i = 0; i < 10; i++)
                    {
                        if (oldDir.LengthSquared() < 5)
                            dir = new Vector2(700, 0).RotatedBy(MathHelper.TwoPi / 10f * i);

                        Projectile shock = Projectile.NewProjectileDirect(projectile.GetSource_Death(), projectile.Center, dir.SafeNormalize(Vector2.Zero), ModContent.ProjectileType<CrystalLightning>(), Player.HeldItem.damage, 1f, Player.whoAmI, ai1: projectile.whoAmI);
                        shock.ai[2] = dir.Length();
                    }
                }
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow == 0f)
            {
                if (crystalGauntletsClapTime > 0)
                {
                    float clapRotation = -0.4f + MathF.Sqrt(Utils.GetLerpValue(40, 10, crystalGauntletsClapTime, true)) * Utils.GetLerpValue(10, 15, crystalGauntletsClapTime, true) * 2f + MathHelper.SmoothStep(0, 0.5f, MathF.Sqrt(Utils.GetLerpValue(10, 0, crystalGauntletsClapTime, true)));
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (-MathHelper.PiOver2 + clapRotation) * Player.direction);
                    Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, (-MathHelper.PiOver2 - clapRotation) * Player.direction);
                }
            }
        }

        public override float UseSpeedMultiplier(Item item)
        {
            if (gobfloggerBuff && item.type == ModContent.ItemType<Gobflogger>())
                return 6.66f;
            return 1f;
        }

        public override void ResetEffects()
        {
            gobfloggerBuff = false;

            crystalGauntletsHeld = false;

            if (crystalGauntletsClapTime > 0)
                crystalGauntletsClapTime--;
        }
    }
}
