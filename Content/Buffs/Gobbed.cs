using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Buffs
{
    public class Gobbed : ModBuff
    {
        public static readonly int TagDamage = 50;

        public override void Update(Player player, ref int buffIndex)
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }

    public class GobbedNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<Gobbed>())
                modifiers.FlatBonusDamage += Gobbed.TagDamage * ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
        }
    }
}
