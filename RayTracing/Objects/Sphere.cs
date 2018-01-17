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
    class Sphere : Base
    {

        private static RayTracingEffect effect;
        private VertexPositionNormalColor[] vertices;
        public override EffectBase Effect => Sphere.effect;

        protected override string Technique => "BasicColorDrawing";

        protected override void initEffect(ContentManager ctx)
        {
            Sphere.effect = new RayTracingEffect(ctx);
        }

        private Vector3 getNormalVector(Vector3 point)
        {
            var normal = new Vector3(point.X, point.Y, point.Z);
            normal.Normalize();
            return normal;
        }

        public Sphere(ContentManager ctx, Vector3 position, float scale, int degree, float radius) : base(ctx, position, 0, 0, 0, scale)
        {
            this.vertices = new VertexPositionNormalColor[2 * (360 / degree + 1) * (90 / degree + 1) * 6];
            var fragmentVertices = new List<Vector3>(90 / degree + 1);
            var v1 = new Vector3(radius, 0, 0);
            for (var i = 0; i <= 90 / degree; i++)
            {
                fragmentVertices.Add(Vector3.Transform(v1, Matrix.CreateRotationZ(MathHelper.ToRadians(degree * i))));
            }

            for (var i = 0; i < 360 / degree; i++)
            {
                var f1 = fragmentVertices.Select(e =>
                        Vector3.Transform(e, Matrix.CreateRotationY(MathHelper.ToRadians(i * degree))))
                    .Select(v => new Vector3(v.X, v.Y, v.Z))
                    .ToList();
                var f2 = fragmentVertices.Select(e =>
                        Vector3.Transform(e, Matrix.CreateRotationY(MathHelper.ToRadians((i + 1) * degree))))
                    .Select(v => new Vector3(v.X, v.Y, v.Z))
                    .ToList();

                for (var j = 0; j < 90 / degree; j++)
                {
                    var currentVertexNumber = i * 90 / degree * 6 + j * 6;
                    var normal = this.getNormalVector(f1[0 + j]);
                    this.vertices[currentVertexNumber] =
                        new VertexPositionNormalColor(f1[0 + j], normal, Color.SandyBrown);
                    normal = this.getNormalVector(f2[0 + j]);
                    this.vertices[currentVertexNumber + 2] =
                        new VertexPositionNormalColor(f2[0 + j], normal, Color.SandyBrown);
                    normal = this.getNormalVector(f1[1 + j]);
                    this.vertices[currentVertexNumber + 1] =
                        new VertexPositionNormalColor(f1[1 + j], normal, Color.SandyBrown);

                    normal = this.getNormalVector(f1[1 + j]);
                    this.vertices[currentVertexNumber + 4] =
                        new VertexPositionNormalColor(f1[1 + j], normal, Color.SandyBrown);
                    normal = this.getNormalVector(f2[0 + j]);
                    this.vertices[currentVertexNumber + 3] =
                        new VertexPositionNormalColor(f2[0 + j], normal, Color.SandyBrown);
                    normal = this.getNormalVector(f2[1 + j]);
                    this.vertices[currentVertexNumber + 5] =
                        new VertexPositionNormalColor(f2[1 + j], normal, Color.SandyBrown);
                }
            }
            for (var i = 0; i < (360 / degree + 1) * (90 / degree + 1) * 6; i++)
            {
                var v = this.vertices[i];
                this.vertices[i + (360 / degree + 1) * (90 / degree + 1) * 6] = new VertexPositionNormalColor
                (
                    new Vector3(v.Position.X, -v.Position.Y, v.Position.Z),
                    new Vector3(v.Normal.X, -v.Normal.Y, v.Normal.Z),
                    v.Color
                );
            }

        }

        public override void Draw(Matrix view, Matrix projection, Vector3 pos)
        {
            Effect.GraphicsDevice.RasterizerState = new RasterizerState() {CullMode = CullMode.None};
            base.Draw(view, projection, pos);
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Effect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(PrimitiveType.TriangleList, this.vertices, 0, this.vertices.Length, Enumerable.Range(0, this.vertices.Length).ToArray(), 0, this.vertices.Length / 3);

            }
            Effect.GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.CullCounterClockwiseFace };
        }
    }
}
