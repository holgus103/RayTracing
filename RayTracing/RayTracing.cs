using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RayTracing.Objects;
using _3DGraphics.Core;

namespace RayTracing
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class RayTracing : GameBase
    {
        protected override void Initialize()
        {
            this.elements.Add(new Sphere(this.Content, new Vector3(0, 0, 0), 4));
            this.camera.Speed = 0.01f;
        }
    }
}
