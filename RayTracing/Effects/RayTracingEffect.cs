using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using _3DGraphics.Core.Effects;

namespace RayTracing.Effects
{
    class RayTracingEffect : EffectBase
    {
        public RayTracingEffect(ContentManager ctx) : base(ctx, "Shaders//RayTracing")
        {
        }
    }
}
