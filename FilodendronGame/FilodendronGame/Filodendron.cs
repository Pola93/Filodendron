using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FilodendronGame
{
    public class Filodendron : BasicModel
    {
        public Vector3 avatarPosition = new Vector3(0, 0, -50);
        public float rotationSpeed = 1f / 500f;
        public float forwardSpeed = 200f / 60f;
        public float backwardSpeed = -(100f / 60f);
        public float sideSpeed = 150f / 60f;
        public float avatarYaw;

        public MouseState prevMouseState;
        public Effect CustomShader;

        public Filodendron(Model m, Matrix world)
            : base(m, world)
        {
            
        }

        public override void Update()
        {
            UpdateAvatarPosition();
            
            World = Matrix.CreateRotationY(avatarYaw) *
                Matrix.CreateTranslation(avatarPosition); //potem przerzuc nizej do metody
        }
        /// <summary>
        /// Update the position and direction of the avatar.
        /// </summary>
        void UpdateAvatarPosition()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            if (Mouse.GetState().X != prevMouseState.X)
            {
                avatarYaw -= (Mouse.GetState().X - prevMouseState.X) * rotationSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                Matrix forwardMovement = Matrix.CreateRotationY(avatarYaw);
                Vector3 v = new Vector3(0, 0, forwardSpeed);
                v = Vector3.Transform(v, forwardMovement);
                avatarPosition.Z += v.Z;
                avatarPosition.X += v.X;
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                Matrix backwardMovement = Matrix.CreateRotationY(avatarYaw);
                Vector3 v = new Vector3(0, 0, backwardSpeed);
                v = Vector3.Transform(v, backwardMovement);
                avatarPosition.Z += v.Z;
                avatarPosition.X += v.X;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                Matrix sideMovement = Matrix.CreateRotationY(avatarYaw);
                Vector3 v = new Vector3(sideSpeed, 0, 0);
                v = Vector3.Transform(v, sideMovement);
                avatarPosition.Z += v.Z;
                avatarPosition.X += v.X;
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                Matrix sideMovement = Matrix.CreateRotationY(avatarYaw);
                Vector3 v = new Vector3(-sideSpeed, 0, 0);
                v = Vector3.Transform(v, sideMovement);
                avatarPosition.Z += v.Z;
                avatarPosition.X += v.X;
            }
            prevMouseState = Mouse.GetState();
        }

        public override void Draw(Model model, Matrix world, Texture2D texture, Camera camera)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = CustomShader;
                    CustomShader.CurrentTechnique = CustomShader.Techniques["Diffuse"];
                    CustomShader.Parameters["xWorldViewProjection"].SetValue(
                        world * mesh.ParentBone.Transform * camera.view * camera.proj);
                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    CustomShader.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                    //CustomShader.Parameters["xColoredTexture"].SetValue(texture);
                }
                mesh.Draw();
            }
        }
    }
}
