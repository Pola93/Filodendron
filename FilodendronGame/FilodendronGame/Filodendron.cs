using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FilodendronGame.Interfaces;
using FilodendronGame.Abilities;
using SkinnedModel;
using System.Diagnostics;

namespace FilodendronGame
{
    public class Filodendron : BasicModel
    {
        public AnimationPlayer animationPlayer;
        public SkinningData skinningData;
        public AnimationClip clip;
        public Texture2D avatarTexture;

        public Vector3 avatarOldPosition;
        public Vector3 avatarPosition;
        public Vector3 avatarSpeed;
        public Bullet bullet;

        public MouseState prevMouseState;
        public Effect CustomShader;

        public float rotationSpeed = 1f / 500f;
        public float forwardSpeed;
        public float sideSpeed;
        public float verticalSpeed = 0;
        
        public float avatarYaw;
        public float rotation = 0;
        private float multiplier = 5f;

        // Flag for jump key
        // private bool spacePressed = false;
        // Flag for gravity switch
        private bool gravityPressed = false;
        private bool shootPressed = false;
        // Gravity flag
        public bool allowGravity = false;
        public bool stopPosition { get; set; }

        //Vector3 viewVector; // for specular light

        public Filodendron(Model m, Matrix world) : base(m, world)
        {
            rigidBody = new FilodendronRigidBody(this);
            avatarPosition = World.Translation;
            gravity = new FilodendronGravity();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateAvatarPosition(gameTime);

            World = Matrix.CreateRotationY(avatarYaw) * Matrix.CreateTranslation(avatarPosition); //potem przerzuc nizej do metody
        }
        /// <summary>
        /// Update the position and direction of the avatar.
        /// </summary>

        void UpdateAvatarPosition(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            if (Mouse.GetState().X != prevMouseState.X)
            {
                avatarYaw -= (Mouse.GetState().X - prevMouseState.X) * rotationSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.E))
            {
                if (!shootPressed)
                {
                    shootPressed = true;
                    Shoot();
                }
            }
            else
            {
                shootPressed = false;
            }

            // Switching gravity
            if (keyboardState.IsKeyDown(Keys.G))
            {
                if (!gravityPressed)
                {
                    allowGravity = !allowGravity;
                    gravityPressed = true;

                    // Just in case (for now)
                    if (!allowGravity) {
                        verticalSpeed = 0;
                    }
                }
            }
            else
            {
                gravityPressed = false;
            }

            forwardSpeed = keyboardState.IsKeyDown(Keys.W) ? 200f / 60f * multiplier :
                ((keyboardState.IsKeyDown(Keys.S)) ? -(100f / 60f) * multiplier : 0);

            sideSpeed = keyboardState.IsKeyDown(Keys.A) ? 150f / 60f * multiplier :
                ((keyboardState.IsKeyDown(Keys.D)) ? -(150f / 60f) * multiplier : 0);

            verticalSpeed = (allowGravity && !rigidBody.DetectCollision()) ? verticalSpeed - gravity.UpdateSpeed(gameTime) : 
                ((keyboardState.IsKeyDown(Keys.Space) && (verticalSpeed > -0.0001f && verticalSpeed < 0.0001f)) ? 10f / 2f * multiplier : 0);

            Matrix movement = Matrix.CreateRotationY(avatarYaw);
            Vector3 moveVector = new Vector3(sideSpeed, verticalSpeed, forwardSpeed);
            Vector3 oldPosition = this.avatarPosition;
            moveVector = Vector3.Transform(moveVector, movement);
            this.avatarSpeed = moveVector;
            UpdatePosition(moveVector, oldPosition);
            animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);

            if (!keyboardState.IsKeyDown(Keys.W) && !keyboardState.IsKeyDown(Keys.A)
                && !keyboardState.IsKeyDown(Keys.D) && !keyboardState.IsKeyDown(Keys.S))
            {
                //Stop animation for player walking
                animationPlayer.Update(new TimeSpan(0, 0, 0), true, Matrix.Identity);
            }
            prevMouseState = Mouse.GetState();
        }

        private void Shoot()
        {
            bullet.Shoot(this);
        }

        public void UpdatePosition(Vector3 move, Vector3 oldPosition)
        {
            avatarPosition.X += move.X;
            avatarPosition.Y += move.Y;
            avatarPosition.Z += move.Z;
        }

        public override void Draw(Model model, Matrix world, Texture2D texture, Camera camera, GameTime gameTime, GraphicsDeviceManager graphics)
        {
            //nie usuwać, jeszcze może się przydać!!!
            /*viewVector = Vector3.Transform(avatarPosition - camera.cameraPosition, Matrix.CreateRotationY(avatarYaw));
            viewVector.Normalize();

            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = CustomShader;
                    CustomShader.CurrentTechnique = CustomShader.Techniques["Specular"];
                    CustomShader.Parameters["xWorldViewProjection"].SetValue(
                        world * mesh.ParentBone.Transform * camera.view * camera.proj);
                    CustomShader.Parameters["ViewVector"].SetValue(viewVector);
                    CustomShader.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    CustomShader.Parameters["ModelTexture"].SetValue(texture);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    CustomShader.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                }
                mesh.Draw();
            }
            DrawBoundingBox(camera, graphics);*/
           
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Matrix[] bones = animationPlayer.GetSkinTransforms();

            // Compute camera matrices.
            Matrix view = camera.view;

            Matrix projection = camera.proj;

            // Render the skinned mesh.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.World = world * mesh.ParentBone.Transform;
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
            DrawBoundingBox(camera, graphics);

        }
        public void DrawBoundingBox(Camera camera, GraphicsDeviceManager graphics)
        {
            short[] bBoxIndices = {
                0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
            };
            List<BoundingBox> boundingBoxes = new List<BoundingBox>();
            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                boundingBoxes.Add(BoundingBox.CreateFromSphere(myModelMeshes.BoundingSphere));
            }

            foreach (BoundingBox box in boundingBoxes)
            {
                Vector3[] corners = box.GetCorners();
                VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

                // Assign the 8 box vertices
                for (int i = 0; i < corners.Length; i++)
                {
                    primitiveList[i] = new VertexPositionColor(corners[i], Color.White);
                }

                /* Set your own effect parameters here */
                BasicEffect be = new BasicEffect(graphics.GraphicsDevice);

                be.World = World;
                be.View = camera.view;
                be.Projection = camera.proj;
                be.TextureEnabled = false;

                // Draw the box with a LineList
                foreach (EffectPass pass in be.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.LineList, primitiveList, 0, 8,
                        bBoxIndices, 0, 12);
                }
            }
        }
    }
}
