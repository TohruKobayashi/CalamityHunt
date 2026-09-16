using System;
using System.Collections.Generic;
using System.Formats.Tar;
using CalamityHunt.Common.Players;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Summoner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityHunt.Content.NPCs.Bosses.Lethe.Lethe;

namespace CalamityHunt.Content.Items.Weapons.Summoner
{
    public class SuzumiStaff : MinionStaffAbstract<SuzumiStaff_Buff, SuzumiStaff_Counter>
    {
        public const int MinionsPerUse = 8;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.width = 54;
            Item.height = 84;

            Item.damage = 300;
            Item.knockBack = 10f;

            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.value = Item.sellPrice(gold: 20);
        }
    }

    public class SuzumiStaff_Buff : MinionBuffAbstract<SuzumiStaff_Counter>
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);

            int amtOfCounters = player.ownedProjectileCounts[ModContent.ProjectileType<SuzumiStaff_Counter>()];
            int amtOfKnives = player.ownedProjectileCounts[ModContent.ProjectileType<SuzumiStaff_Knife>()];

            int counterKnifeDiff = (amtOfCounters * SuzumiStaff.MinionsPerUse) - amtOfKnives;
            if (counterKnifeDiff != 0)
            {
                int highestOriginalDamage = 0;
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.active && proj.owner == player.whoAmI)
                    {
                        if (proj.type == ModContent.ProjectileType<SuzumiStaff_Counter>() && highestOriginalDamage < proj.originalDamage)
                            highestOriginalDamage = proj.originalDamage;
                        else if (proj.type == ModContent.ProjectileType<SuzumiStaff_Knife>())
                            proj.Kill();
                    }
                }

                for (int i = 0; i < amtOfCounters * SuzumiStaff.MinionsPerUse; i++)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<SuzumiStaff_Knife>(), highestOriginalDamage, 0, player.whoAmI);
                }
            }
        }
    }

    public class SuzumiStaff_Knife : MinionProjAbstract<SuzumiStaff_Buff>
    {
        public override float MinionSlotsRequired => 0;
        public override bool MinionSacrificable => false;
        public override bool DoContactDamage => true;

        public ref float RotationLerp => ref Projectile.ai[1];
        /// <summary>
        /// which ring the knife is in. 0-1 is center, 2 is oriented, 3 is outside facing inwards, 4+ is outside spinning
        /// </summary>
        public int CurrentKnifeInterval
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override bool MinionBehavior_PreTargeting(Player owner)
        {
            #region general knive values
            Vector2 pos = owner.Center;
            List<int> knifeIDs = new List<int>();

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.owner == owner.whoAmI && proj.type == Projectile.type)
                    knifeIDs.Add(proj.whoAmI);
            }
            int currentKnifeIndex = knifeIDs.IndexOf(Projectile.whoAmI);
            int amtOfKniveSetsPerRing = 2;
            CurrentKnifeInterval = currentKnifeIndex / (SuzumiStaff.MinionsPerUse * amtOfKniveSetsPerRing);
            #endregion

            #region position values
            int rotationRate = 180;
            if (CurrentKnifeInterval == 0)
            {
                rotationRate -= 70;
                rotationRate *= -1;
            }
            int distance = 40;
            int iterationStart = 0;
            int iterationAmount = SuzumiStaff.MinionsPerUse * amtOfKniveSetsPerRing;
            if (iterationAmount > knifeIDs.Count)
                iterationAmount = knifeIDs.Count;
            float radians = MathHelper.TwoPi / iterationAmount;

            if (CurrentKnifeInterval == 2)
                distance += 100;
            else if (CurrentKnifeInterval > 2)
            {
                distance += 140;
                rotationRate += 100;
            }

            for (int i = 0; i < CurrentKnifeInterval; i++)
            {
                rotationRate -= (int)(45 * 0.8f);
                if (rotationRate < 100)
                    rotationRate = 100 + CurrentKnifeInterval * 5;
                distance += 32;
                iterationStart += SuzumiStaff.MinionsPerUse * amtOfKniveSetsPerRing;
                iterationAmount += SuzumiStaff.MinionsPerUse * amtOfKniveSetsPerRing;
                if (iterationAmount > knifeIDs.Count)
                    iterationAmount = knifeIDs.Count;
                radians = MathHelper.TwoPi / (iterationAmount - iterationStart);
            }
            #endregion

            #region position
            for (int i = 0; i < knifeIDs.Count; i++)
            {
                int counterID = knifeIDs[i];
                if (counterID == Projectile.whoAmI)
                {
                    if (CurrentKnifeInterval == 2)
                    {
                        float rotAmt = Utils.AngleLerp(RotationLerp, owner.Center.DirectionTo(Main.MouseWorld).ToRotation(), 0.05f);
                        if (Timer == 0)
                            rotAmt = owner.Center.DirectionTo(Main.MouseWorld).ToRotation();
                        // bcuz of how rotation works moving cursor in one direction makes the ring seem to spin faster, and slower in other direction
                        // this is the best way ive found to mitigate that; guessing
                        float rotCompensateForCursorMovement = rotAmt / 3;
                        float radiansAndExtraRotation = (radians * i) + (MathHelper.TwoPi * (Timer % rotationRate / rotationRate) - rotCompensateForCursorMovement);
                        Vector2 posCircle = Vector2.Normalize(new Vector2(0, -1).RotatedBy(radiansAndExtraRotation));
                        posCircle = new Vector2(posCircle.X * 1.4f, posCircle.Y * 0.4f) * distance;
                        pos += posCircle.RotatedBy(rotAmt);
                        Projectile.rotation = radiansAndExtraRotation + rotAmt;
                        Projectile.rotation += MathHelper.Pi;

                        RotationLerp = rotAmt;
                    }
                    else
                    {
                        float radiansAndExtraRotation = (radians * i) + (MathHelper.TwoPi * (Timer % rotationRate / rotationRate));
                        pos += Vector2.Normalize(new Vector2(0, -1).RotatedBy(radiansAndExtraRotation)) * distance;
                        Projectile.rotation = radiansAndExtraRotation;
                        if (CurrentKnifeInterval == 3)
                            Projectile.rotation += MathHelper.Pi;
                        else if (CurrentKnifeInterval >= 4)
                            Projectile.rotation += currentKnifeIndex + (MathHelper.TwoPi * (Timer % 40 / 40));
                    }

                }
            }

            pos.Y += owner.gfxOffY;
            Projectile.Center = pos;
            #endregion

            #region damage
            if (CurrentKnifeInterval < 2)
                Projectile.damage = (int)(Projectile.damage * 1.33f);
            else if (CurrentKnifeInterval < 4)
                Projectile.damage = Projectile.damage;
            else
                Projectile.damage = (int)(Projectile.damage * 0.66f);
            #endregion

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int knifeFrameWidth = 18;
            int knifeFrameHeight = 38;
            int knifeFrameY = CurrentKnifeInterval >= 4 ? 2 : CurrentKnifeInterval >= 2 ? 1 : 0;
            knifeFrameY *= knifeFrameHeight;
            Rectangle knifeFrame = new Rectangle(0, knifeFrameY, knifeFrameWidth, knifeFrameHeight);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, knifeFrame, lightColor, Projectile.rotation, knifeFrame.Size() * 0.5f, Projectile.scale, SpriteEffects.None);

            return false;
        }
    }

    public class SuzumiStaff_Counter : MinionCounterAbstract<SuzumiStaff_Buff>
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Projectile.hide = false;
        }
    }

    // ------------------------------------------------------------------------------------------------------------

    public abstract class MinionStaffAbstract<TBuff, TMinion> : ModItem where TBuff : ModBuff where TMinion : ModProjectile
    {
        public virtual float MinionSlotsRequired => 1f;
        /// <summary>
        /// A maximum amount of minions you can summon before the weapon refuses to summon more. Set to -1 to disable the cap.
        /// <br/> Defaults to -1.
        /// </summary>
        public virtual int MinionCap => -1;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = MinionSlotsRequired;
        }

        public override void SetDefaults()
        {
            Item.mana = 10; // TODO: remove in 1.4.5.7

            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.reuseDelay = 2;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;

            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<TBuff>();
            Item.shoot = ModContent.ProjectileType<TMinion>();
            Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
        {
            if (MinionCap == -1)
                return true;

            int activeProjs = 0;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == Item.shoot && proj.owner == Main.myPlayer)
                    activeProjs++;
            }

            return activeProjs < MinionCap;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return true;
        }
    }

    public abstract class MinionBuffAbstract<TMinion> : ModBuff where TMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    public abstract class MinionProjAbstract<TBuff> : ModProjectile where TBuff : ModBuff
    {
        public virtual float MinionSlotsRequired => 1f; //TODO: can this pull from buff while still being nice?
        public virtual bool MinionSacrificable => true;
        public virtual bool DoContactDamage => false;
        public virtual float AggroDistance => 700f;
        public virtual bool NeedsLineOfSight => true;
        public virtual bool ForceTargetIfTargetIsTargeted => false;

        public ref float Timer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = MinionSacrificable;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;

            Main.projPet[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;

            Projectile.minionSlots = MinionSlotsRequired;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return DoContactDamage;
        }

        public override sealed void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
                return;

            if (MinionBehavior_PreTargeting(owner))
            {
                SearchForTargets(owner, out bool foundTarget, out NPC target, out float distanceFromTarget, out Vector2 targetCenter);
                MinionBehavior_PostTargeting(owner, foundTarget, target, distanceFromTarget, targetCenter);
            }

            Timer++;
        }

        public bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<TBuff>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<TBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }

        public void SearchForTargets(Player owner, out bool foundTarget, out NPC target, out float distanceFromTarget, out Vector2 targetCenter)
        {
            foundTarget = false;
            target = Main.npc[0];
            distanceFromTarget = AggroDistance;
            targetCenter = Projectile.position;

            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                if (between < 2000f || ForceTargetIfTargetIsTargeted)
                {
                    foundTarget = true;
                    target = npc;
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                }
            }

            if (!foundTarget)
            {
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        if (!NeedsLineOfSight)
                            lineOfSight = true;

                        if (((closest && inRange) || !foundTarget) && lineOfSight)
                        {
                            foundTarget = true;
                            target = npc;
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                        }
                    }
                }
            }

            Projectile.friendly = foundTarget;
        }

        public virtual bool MinionBehavior_PreTargeting(Player owner)
        {
            return true;
        }

        public virtual void MinionBehavior_PostTargeting(Player owner, bool foundTarget, NPC target, float distanceFromTarget, Vector2 targetCenter)
        {

        }
    }

    /// <summary>
    /// A minion "counter", also sometimes referred to as a "gem", is a projectile used for handling weirder minions,
    /// such as singular minions which get stronger for each summon slot consumed or weapons which summon multiple 
    /// minions at once.
    /// </summary>
    public class MinionCounterAbstract<TBuff> : ModProjectile where TBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.StormTigerGem;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;

            Main.projFrames[Type] = 6;
            Main.projPet[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 60;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.HasBuff<TBuff>())
                Projectile.timeLeft = 2;

            Projectile.velocity = Vector2.Zero;
            PlaceCountersAbovePlayerPleasinglyForDebugging();

            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
            }
        }

        private void PlaceCountersAbovePlayerPleasinglyForDebugging()
        {
            Player owner = Main.player[Projectile.owner];
            Vector2 pos = new Vector2(owner.Center.X, owner.position.Y - 14);
            GetAmountOfCountersPlayerHas(owner, out int amtOfCounters, out List<int> counterIDs);

            int distance = Projectile.width + 2;
            for (int i = 0; i < counterIDs.Count; i++)
            {
                int counterID = counterIDs[i];

                if (counterID == Projectile.whoAmI)
                {
                    pos.X += i * distance;

                    int totalWidth = distance * counterIDs.Count;
                    pos.X -= totalWidth / 2;

                    pos.X += distance / 2;
                }
            }

            pos.Y += owner.gfxOffY;
            Projectile.Center = pos;
        }

        /// <summary>
        /// Returns the amount of counters the player has for the specified minion counter.
        /// </summary>
        /// <returns></returns>
        public void GetAmountOfCountersPlayerHas(Player owner, out int amtOfCounters, out List<int> counterIDs)
        {
            amtOfCounters = 0;
            counterIDs = new List<int>();

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.owner == owner.whoAmI && proj.type == Projectile.type)
                {
                    amtOfCounters++;
                    counterIDs.Add(proj.whoAmI);
                }
            }
        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;
    }
}
