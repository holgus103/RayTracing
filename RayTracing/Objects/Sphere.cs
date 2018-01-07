using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RayTracing.Effects;
using _3DGraphics.Core.Effects;
using _3DGraphics.Core.Objects;
using _3DGraphics.Core.Objects.Commons;

namespace RayTracing.Objects
{
    class Sphere : Base, IModelObject
    {
        class MyIsland : Island
        {
            public MyIsland(ContentManager ctx, float noise, float curvyness, int degree, float radius, GraphicsDevice dev, Vector3 position, float xRotation, float yRotation, float zRotation)
                : base(ctx, noise, curvyness, degree, radius, dev, position, xRotation, yRotation, zRotation)
            {
            }

            protected override string Technique => "BasicColorDrawing";
            public override EffectBase Effect => Sphere.effect;

            public override void Draw(Matrix view, Matrix projection, Vector3 pos)
            {
                Effect.GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
                Effect.CurrentTechnique = Effect.Techniques[this.Technique];
                Effect.CameraPosition = pos;
                Effect.World = this.getWorldMatrix();
                Effect.View = view;
                Effect.Projection = projection;
                foreach (var pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Effect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(PrimitiveType.TriangleList, this.vertices, 0, this.vertices.Length, Enumerable.Range(0, this.vertices.Length).ToArray(), 0, this.vertices.Length / 3);

                }
                Effect.GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.CullClockwiseFace };
            }

            protected override void initEffect(ContentManager ctx)
            {
            }
        }
        private static Model model;
        private static RayTracingEffect effect;
        private Island i2;
        private Island i1;
        public Model Model => Sphere.model;
        public override EffectBase Effect => Sphere.effect;

        protected override string Technique => "BasicColorDrawing";

        protected override void initEffect(ContentManager ctx) 
        {
            Sphere.effect = new RayTracingEffect(ctx);
        }

        public Sphere(ContentManager ctx, Vector3 position, float scale) : base(ctx, position, 0, 0, 0, scale)
        {
            this.i1 = new MyIsland(ctx, 0, 0, 10, 4, effect.GraphicsDevice, new Vector3(0, 0, 0), 0, 0, 0);
            this.i2 = new MyIsland(ctx, 0, 0, 10, 4, effect.GraphicsDevice, new Vector3(0, 0, 0), 180, 0, 0);
            //if (Sphere.model == null)
            //{
            //    Sphere.model = ctx.Load<Model>("Models\\sphere");
            //}
        }

        public override void Draw(Matrix view, Matrix projection, Vector3 pos)
        {
            this.i1.Draw(view, projection, pos);
            this.i2.Draw(view, projection, pos);
        }
    }
}
