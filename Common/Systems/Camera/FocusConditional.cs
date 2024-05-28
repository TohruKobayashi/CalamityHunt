using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace CalamityHunt.Common.Systems.Camera;

public class FocusConditional : ICameraModifier
{
    public FocusConditional(int riseTime, int fallTime, Func<bool> condition, string identity = null)
    {
        id = identity;
        this.riseTime = riseTime;
        this.fallTime = fallTime;
        this.condition = condition;
    }

    private string id;
    private int riseTime;
    private int fallTime;
    private int time;
    private Func<bool> condition;

    public static Vector2 focusTarget;

    public string UniqueIdentity => id;

    public bool Finished => time > riseTime + 1 + fallTime;

    public void Update(ref CameraInfo cameraPosition)
    {
        if (!Main.gamePaused) {
            bool shouldHold = condition.Invoke();
            if (!shouldHold || time < riseTime) {
                time++;
            }
        }

        float totalTime = Utils.GetLerpValue(0, riseTime, time, true) * Utils.GetLerpValue(riseTime + 1 + fallTime, riseTime + 1, time, true);
        Vector2 newPosition = Vector2.Lerp(cameraPosition.CameraPosition, focusTarget - Main.ScreenSize.ToVector2() / 2f, (float)MathHelper.SmoothStep(0, 1, totalTime));
        cameraPosition.CameraPosition = newPosition;
    }
}
