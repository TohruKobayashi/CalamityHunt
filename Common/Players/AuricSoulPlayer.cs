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
    public bool avatarSoul;
    public bool namelessSoul;

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
        if (avatarSoul) {
            AllStatsUp(-0.09f);
        }
        if (namelessSoul) {
            AllStatsUp(0.091f);
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
        if (avatarSoul) {
            Player.statLifeMax2 -= 30;
            Player.statManaMax2 -= 60;
        }
        if (namelessSoul) {
            Player.statLifeMax2 += 31;
            Player.statManaMax2 += 61;
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

        if (avatarSoul) {
            tag["avatarSoul"] = true;
        }

        if (namelessSoul) {
            tag["deitySoul"] = true;
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
        if (tag.ContainsKey("avatarSoul")) {
            avatarSoul = tag.GetBool("avatarSoul");
        }
        if (tag.ContainsKey("deitySoul")) {
            namelessSoul = tag.GetBool("deitySoul");
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
        packet.Write(avatarSoul);
        packet.Write(namelessSoul);
        packet.Send(toWho, fromWho);
    }

    // Called in ExampleMod.Networking.cs
    public void ReceivePlayerSync(BinaryReader reader)
    {
        goozmaSoul = reader.ReadBoolean();
        yharonSoul = reader.ReadBoolean();
        olddukeSoul = reader.ReadBoolean();
        pyrogenSoul = reader.ReadBoolean();
        avatarSoul = reader.ReadBoolean();
        namelessSoul = reader.ReadBoolean();
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        AuricSoulPlayer clone = (AuricSoulPlayer)targetCopy;
        clone.goozmaSoul = goozmaSoul;
        clone.yharonSoul = yharonSoul;
        clone.olddukeSoul = yharonSoul;
        clone.pyrogenSoul = pyrogenSoul;
        clone.avatarSoul = avatarSoul;
        clone.namelessSoul = namelessSoul;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        AuricSoulPlayer clone = (AuricSoulPlayer)clientPlayer;

        if (goozmaSoul != clone.goozmaSoul || 
            yharonSoul != clone.yharonSoul ||
            pyrogenSoul != clone.pyrogenSoul ||
            avatarSoul != clone.avatarSoul ||
            namelessSoul != clone.namelessSoul ||
            olddukeSoul != clone.olddukeSoul)
            SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
    }
}
