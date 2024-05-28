using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems.Particles;

[Autoload(Side = ModSide.Client)]
public abstract class Particle : ModTexturedType
{
    public int Type { get; private set; }

    public bool ShouldRemove { get; set; } = false;

    public Vector2 position;

    public Vector2 velocity;

    public float rotation;

    public float scale = 1f;

    public Color color = Color.White;

    public virtual void OnSpawn() { }

    public virtual void Update() { }

    public virtual void Draw(SpriteBatch spriteBatch) { }

    protected override void Register()
    {
        Type = ParticleLoader.ReserveID();

        AssetDirectory.Textures.Particle ??= new Dictionary<int, Asset<Texture2D>>();
        AssetDirectory.Textures.Particle.Add(Type, ModContent.Request<Texture2D>(Texture));

        ParticleLoader.Instance ??= new Dictionary<Type, Particle>();
        ParticleLoader.Instance.Add(GetType(), this);
    }

    public static T Create<T>(Action<T> initializer) where T : Particle
    {
        if (Main.dedServ) return null;
        T newParticle = (T)ParticleLoader.Instance[typeof(T)].MemberwiseClone();
        initializer(newParticle);
        newParticle.OnSpawn();
        return newParticle;
    }
}
