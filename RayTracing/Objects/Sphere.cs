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
        private static Model model;
        private static RayTracingEffect effect;
        public Model Model => Sphere.model;
        public override EffectBase Effect => Sphere.effect;

        protected override string Technique => "BasicColorDrawing";

        protected override void initEffect(ContentManager ctx) 
        {
            Sphere.effect = new RayTracingEffect(ctx);
        }

        public Sphere(ContentManager ctx, Vector3 position, float scale) : base(ctx, position, 0, 0, 0, scale)
        {
            if (Sphere.model == null)
            {
                Sphere.model = ctx.Load<Model>("Models\\sphere");
            }
        }

        public override void Draw(Matrix view, Matrix projection, Vector3 pos)
        {
            base.Draw(view, projection, pos);
            Effect.Effect.CurrentTechnique = effect.Effect.Techniques[this.Technique];
            this.DrawModel(view, projection, pos);
        }
    }
}
