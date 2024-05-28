using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players
{
    public class MovementModifyPlayer : ModPlayer
    {
        public bool stickyHand;
        public int stickyHandTime;
        public Vector2 preHookVelocity;

        public override void PostUpdateRunSpeeds()
        {
            if (stickyHand) {
                Player.noFallDmg = true;
                Player.gravity = 0f;
                Player.maxFallSpeed = 9999f;
                Player.runSlowdown = 0f;
                Player.GoingDownWithGrapple = true;
            }
            else {
                preHookVelocity = Player.velocity;
            }

            if (stickyHandTime > 0) {
                Player.gravity *= Utils.GetLerpValue(30, 0, stickyHandTime, true);
                Player.runSlowdown *= Utils.GetLerpValue(30, 0, stickyHandTime, true);
            }

            stickyHand = false;
        }

        public override void ResetEffects()
        {
            if (stickyHandTime > 0) {
                stickyHandTime--;
            }
        }
    }
}
