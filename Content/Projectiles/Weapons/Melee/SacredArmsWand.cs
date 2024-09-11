﻿using System;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Melee
{
    public class SacredArmsWand : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.noEnchantmentVisuals = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Mode => ref Projectile.ai[1];
        public ref float StickHost => ref Projectile.ai[2];

        public ref Player Owner => ref Main.player[Projectile.owner];

        public override void OnSpawn(IEntitySource source)
        {
            
        }

        public override void AI()
        {
            // if held item isnt sacred arms or the player is FUCKING DEAD, then KILL the projectile
            if (Owner.HeldItem.type != ModContent.ItemType<SacredArms>() || !Owner.active || Owner.dead || Owner.noItems || Owner.CCed) {
                Projectile.active = false;
            }

            Owner.heldProj = Projectile.whoAmI;

            // the position we want our wand to go to
            // it should be a set position a little in front of the player, between the cursor and the players center
            Vector2 idealPosition = Owner.MountedCenter + Owner.MountedCenter.DirectionTo(Main.MouseWorld) * 80;
            // warble idle effect
            // i must be honest this is kind of stupid
            Projectile.Center = new Vector2(Projectile.Center.X + (MathF.Sin(Time / 25) * 0.95f), Projectile.Center.Y + (MathF.Sin(Time / 30) * 0.95f));
            // move towards the ideal position snappy
            // safety applied because you can put ur cursor over the players center and the projectile gets nan'd
            Projectile.velocity += Projectile.SafeDirectionTo(idealPosition) * (Projectile.Distance(idealPosition) * 0.1f);
            Projectile.velocity *= 0.7f;
            // account for player movement a little bit so it doesnt lag behind as much
            Projectile.velocity += Owner.velocity * 0.2f;
            // point towards the cursor, relative to the player 
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Owner.MountedCenter.DirectionTo(Main.MouseWorld).ToRotation() + 0.75f, 0.5f);

            // if farther than 1 tile to the cursor, 
            if (Projectile.Distance(idealPosition) > 16) {
                
            }

            // if u held the wand out for like 20 years whatd happen to the timer
            // since it isnt reset when ur just holding it
            Time++;
        }

        public override bool? CanCutTiles() => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }

        //public override bool PreDraw(ref Color lightColor)
        //{
            //SpriteBatch.Draw();
            //MathHelper.SmoothStep(Projectile.rotation, Owner.AngleTo(Main.MouseWorld), 0.2f) + 0.75f
            //return false;
        //}
    }
}
