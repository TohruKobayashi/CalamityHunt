//using System;
//using System.Collections.Generic;
//using CalamityHunt.Common.Utilities;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using ReLogic.Content;
//using Terraria;
//using Terraria.ModLoader;
//using Entity = Arch.Core.Entity;

//namespace CalamityHunt.Common.Systems.ParticlesOld;

//[Autoload(Side = ModSide.Client)]
//public abstract class ParticleBehavior : ModTexturedType
//{
//    public int Type { get; private set; }

//    public override string Texture
//    {
//        get
//        {
//            var texture = base.Texture;
//            if (!texture.EndsWith(nameof(ParticleBehavior)))
//                return texture;

//            return !ModContent.RequestIfExists<Texture2D>(texture, out _) ? texture[..^nameof(ParticleBehavior).Length] : texture;
//        }
//    }

//    public virtual void Update(in Entity entity)
//    { }

//    public virtual void Draw(in Entity entity, SpriteBatch spriteBatch)
//    { }

//    public virtual void OnSpawn(in Entity entity)
//    { }

//    protected sealed override void Register()
//    {
//        ModTypeLookup<ParticleBehavior>.Register(this);
//        Type = IDCount++;
//        if (AssetDirectory.Textures.Particle is null)
//            AssetDirectory.Textures.Particle = new Dictionary<int, Asset<Texture2D>>();
//        AssetDirectory.Textures.Particle.Add(Type, AssetUtilities.RequestImmediate<Texture2D>(Texture));
//    }

//    private static int IDCount;

//    public sealed override void SetupContent()
//    {
//        SetStaticDefaults();
//    }

//    public static Entity NewParticle(ParticleBehavior particleBehavior, Vector2 position, Vector2 velocity, Color color, float scale = 1f)
//    {
//        if (Main.gamePaused || Main.dedServ || ModContent.GetInstance<ParticleSystem>() is not { } particleSystem)
//            return Entity.Null;

//        var particle = new Particle { Behavior = particleBehavior };
//        var particlePosition = new ParticlePosition { Value = position };
//        var particleVelocity = new ParticleVelocity { Value = velocity };
//        var particleColor = new ParticleColor { Value = color };
//        var particleScale = new ParticleScale { Value = scale };
//        var particleRotation = new ParticleRotation { Value = velocity.ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f) * MathHelper.TwoPi };
//        var particleActive = new ParticleActive { Value = true };
//        var entity = particleSystem.ParticleWorld.Create(particle, particlePosition, particleVelocity, particleColor, particleScale, particleRotation, particleActive);
//        particleBehavior.OnSpawn(in entity);
//        return entity;
//    }
//}
