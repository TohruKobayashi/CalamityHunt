using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityHunt.Common.Players;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using CalamityHunt.Content.Projectiles.Weapons.Summoner;
using Mono.Cecil;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace CalamityHunt.Content.Buffs
{
    public class SlimeCaneBuff : ModBuff
    {
        public override string Texture => $"{Mod.Name}/Assets/Textures/Buffs/SlimeCane_0";

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SlimeCaneGemCounter>()] > 0)
                player.GetModPlayer<SlimeCanePlayer>().slimes = true;

            if (!player.GetModPlayer<SlimeCanePlayer>().slimes)
                player.DelBuff(buffIndex);
            else
                player.buffTime[buffIndex] = 18000;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            bool finalTier = Main.LocalPlayer.GetModPlayer<SlimeCanePlayer>().SlimeRank() == SlimeCanePlayer.HighestRank;
            drawParams.Texture = AssetDirectory.Textures.Buffs.SlimeCane[finalTier ? 1 : 0].Value;
            return true;
        }
    }
}
