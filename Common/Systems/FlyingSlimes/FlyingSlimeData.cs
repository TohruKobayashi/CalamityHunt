using System;
using System.Collections.Generic;
using CalamityHunt.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace CalamityHunt.Common.Systems.FlyingSlimes;

public struct FlyingSlimeData
{
    private static int dataCount;

    public FlyingSlimeData(string name, float rarity, Func<bool> specificCondition, int npcType, Color color, Action<FlyingSlimeData, Vector2> dustMethod = null, float speed = 1f, Action<FlyingSlimeData, SpriteBatch, Vector2, float, float, float, Color> specialDraw = null, object extraData = null, bool load = false) : this()
    {
        Type = dataCount++;

        Name = name;
        Rarity = rarity;
        NPCType = npcType;
        DustMethod = dustMethod;
        Speed = speed;
        SpecialDraw = specialDraw;
        SpecificCondition = specificCondition;
        DrawColor = color;
        ExtraData = extraData;

        if (load) {
            AssetDirectory.Textures.FlyingSlime ??= new Dictionary<int, Asset<Texture2D>>();
            AssetDirectory.Textures.FlyingSlime.Add(Type, AssetUtilities.RequestImmediate<Texture2D>(AssetDirectory.AssetPath + "Textures/Extra/FlyingSlimes/Flying" + Name));
        }
    }

    public string Name { get; private set; }

    public int Type { get; private set; }

    public float Rarity { get; private set; }

    public Func<bool> SpecificCondition { get; private set; }

    public int NPCType { get; private set; }

    public Action<FlyingSlimeData, Vector2> DustMethod { get; private set; }

    public float Speed { get; private set; }

    public Color DrawColor { get; private set; }

    public object ExtraData { get; private set; }

    public int RandomValue { get; set; }

    public Action<FlyingSlimeData, SpriteBatch, Vector2, float, float, float, Color> SpecialDraw { get; private set; }
}
