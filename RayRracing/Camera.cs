using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _3DGraphics
{
    class Camera
    {
        private float speed = 0.1f;
        private Vector3 cameraPosition = new Vector3(0, 0, 50);
        public Vector3 up = new Vector3(0, 1, 0);
        public Vector3 forward = new Vector3(0, 0, 1);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 200f);
        private Matrix view;

        public Matrix View => this.view;
        public Matrix Projection => this.projection;
        public Vector3 Position => this.cameraPosition;

        public Camera(Vector3 init)
        {
            this.cameraPosition = init;
        }

        public void Move(TimeSpan frameTime)
        {

            var keyboard = Keyboard.GetState();

            var step = (float)(this.speed * frameTime.TotalMilliseconds);

            if (keyboard.IsKeyDown(Keys.Up))
            {

                this.cameraPosition -= this.forward * step;
            }

            if (keyboard.IsKeyDown(Keys.Down))
            {
                this.cameraPosition += this.forward * step;
            }

            if (keyboard.IsKeyDown(Keys.Left))
            {
                var mov = Vector3.Cross(this.up, this.forward);
                mov.Normalize();
                this.cameraPosition -= mov * step ;
            }

            if (keyboard.IsKeyDown(Keys.Right))
            {

                var mov = Vector3.Cross(this.up, this.forward);
                mov.Normalize();
                this.cameraPosition += mov * step;
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                this.forward = Vector3.Transform(this.forward, Matrix.CreateFromAxisAngle(this.up, MathHelper.ToRadians(2 * step)));
                this.forward.Normalize();
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                this.forward = Vector3.Transform(this.forward, Matrix.CreateFromAxisAngle(this.up, MathHelper.ToRadians(-2 * step)));
                this.forward.Normalize();
            }

            if (keyboard.IsKeyDown(Keys.W))
            {
                var axis = Vector3.Cross(this.up, this.forward);
                axis.Normalize();
                this.forward = Vector3.Transform(this.forward, Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(2 * step)));
                this.up = Vector3.Transform(this.up, Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(2 * step)));
                this.forward.Normalize();
                this.up.Normalize();
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                var axis = Vector3.Cross(this.up, this.forward);
                axis.Normalize();
                this.forward = Vector3.Transform(this.forward, Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(-2 * step)));
                this.up = Vector3.Transform(this.up, Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(-2 * step)));
                this.forward.Normalize();
                this.up.Normalize();
            }

            if (keyboard.IsKeyDown(Keys.Q))
            {
                this.cameraPosition -= this.up * step;
            }

            if (keyboard.IsKeyDown(Keys.E))
            {
                this.cameraPosition += this.up * step;
            }
            if (keyboard.IsKeyDown(Keys.X))
            {
                this.up = Vector3.Transform(this.up, Matrix.CreateFromAxisAngle(this.forward, MathHelper.ToRadians(2 * step)));
                this.up.Normalize();
            }
            if (keyboard.IsKeyDown(Keys.Z))
            {
                this.up = Vector3.Transform(this.up, Matrix.CreateFromAxisAngle(this.forward, MathHelper.ToRadians(-2 * step)));
                this.up.Normalize();
            }
        }

        public void Draw() => this.view = Matrix.CreateLookAt(this.cameraPosition, this.cameraPosition - this.forward, this.up);
    }
}
