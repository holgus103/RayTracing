using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using _3DGraphics.Core.Objects.Commons;

namespace RayRracing.Objects
{
    class Sphere : ModelPhongObject
    {
        private Vector3 position;



        public override Model Model { get; }

        protected override string Technique { get; }

        protected override void initEffect(ContentManager ctx)
        {
            base.initEffect(ctx);
        }

        public Sphere(ContentManager ctx, Vector3 position, float xRotation, float yRotation, float zRotation, float scale) : base(ctx, position, xRotation, yRotation, zRotation, scale)
        {
        }
    }
}
