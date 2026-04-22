using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityHunt.Common.Players;

public class AuricSoulPlayer : ModPlayer
{
    public bool goozmaSoul;
    public bool yharonSoul;
    public bool olddukeSoul;
    public bool pyrogenSoul;

    public override void PostUpdate()
    {
        if (goozmaSoul) {
            AllStatsUp(0.03f);
        }
        if (yharonSoul) {
            AllStatsUp(0.03f);
        }
        if (olddukeSoul) {
            AllStatsUp(0.03f);
        }
        if (pyrogenSoul) {
            AllStatsUp(0.01f);
        }
    }

    public void AllStatsUp(float val)
    {
        Player.GetDamage(DamageClass.Generic) += val;
        Player.GetCritChance(DamageClass.Generic) += val * 100;
        Player.GetKnockback(DamageClass.Summon) += val;
        Player.moveSpeed += val;
        Player.GetAttackSpeed(DamageClass.Melee) += val;
        Player.endurance += val > 0 ? 0.01f : -0.01f;
        Player.statDefense += (int)(val * 100);
    }

    public override void ResetEffects()
    {
        if (goozmaSoul) {
            Player.statLifeMax2 += 10;
            Player.statManaMax2 += 20;
        }
        if (yharonSoul) {
            Player.statLifeMax2 += 10;
            Player.statManaMax2 += 20;
        }
        if (olddukeSoul) {
            Player.statLifeMax2 += 10;
            Player.statManaMax2 += 20;
        }
        if (pyrogenSoul) {
            Player.statLifeMax2 += 3;
            Player.statManaMax2 += 9;
        }
    }

    public override void SaveData(TagCompound tag)
    {
        if (goozmaSoul) {
            tag["goozmaSoul"] = true;
        }

        if (yharonSoul) {
            tag["yharonSoul"] = true;
        }

        if (olddukeSoul) {
            tag["olddukeSoul"] = true;
        }

        if (pyrogenSoul) {
            tag["pyrogenSoul"] = true;
        }

        base.SaveData(tag);
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey("goozmaSoul")) {
            goozmaSoul = tag.GetBool("goozmaSoul");
        }
        if (tag.ContainsKey("yharonSoul")) {
            yharonSoul = tag.GetBool("yharonSoul");
        }
        if (tag.ContainsKey("olddukeSoul")) {
            olddukeSoul = tag.GetBool("olddukeSoul");
        }
        if (tag.ContainsKey("pyrogenSoul")) {
            pyrogenSoul = tag.GetBool("pyrogenSoul");
        }
        base.LoadData(tag);
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)CalamityHunt.PacketType.SyncPlayer);
        packet.Write((byte)Player.whoAmI);
        packet.Write(goozmaSoul);
        packet.Write(yharonSoul);
        packet.Write(olddukeSoul);
        packet.Write(pyrogenSoul);
        packet.Send(toWho, fromWho);
    }

    // Called in ExampleMod.Networking.cs
    public void ReceivePlayerSync(BinaryReader reader)
    {
        goozmaSoul = reader.ReadBoolean();
        yharonSoul = reader.ReadBoolean();
        olddukeSoul = reader.ReadBoolean();
        pyrogenSoul = reader.ReadBoolean();
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        AuricSoulPlayer clone = (AuricSoulPlayer)targetCopy;
        clone.goozmaSoul = goozmaSoul;
        clone.yharonSoul = yharonSoul;
        clone.olddukeSoul = yharonSoul;
        clone.pyrogenSoul = pyrogenSoul;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        AuricSoulPlayer clone = (AuricSoulPlayer)clientPlayer;

        if (goozmaSoul != clone.goozmaSoul || 
            yharonSoul != clone.yharonSoul ||
            pyrogenSoul != clone.pyrogenSoul ||
            olddukeSoul != clone.olddukeSoul)
            SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
    }
}
