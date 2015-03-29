using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace FilodendronGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Matrix view;
        public Matrix proj;

        // Set the direction the camera points without rotation.
        Vector3 thirdPersonReference = new Vector3(0, 200, -200);

        // Set field of view of the camera in radians (pi/4 is 45 degrees).
        static float viewAngle = MathHelper.PiOver4;

        // Set distance from the camera of the near and far clipping planes.
        static float nearClip = 1.0f;
        static float farClip = 1000.0f;

        public float rotationSpeed = 1f / 500f;
        public float cameraPitch = 0;

        MouseState prevMouseState;

        public Camera(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            UpdateCameraThirdPerson();

            base.Update(gameTime);
        }
        
        /// <summary>
        /// Updates the camera when it's in the 3rd person state.
        /// </summary>
        void UpdateCameraThirdPerson()
        {
            // Counting pitch angle to rotate
            if (Mouse.GetState().Y != prevMouseState.Y)
            {
                cameraPitch -= (Mouse.GetState().Y - prevMouseState.Y) * rotationSpeed;
            }

            Quaternion rotationQuat = Quaternion.CreateFromYawPitchRoll(((Game1)Game).modelManager.avatar.avatarYaw, cameraPitch, 0);
            // Create a vector pointing the direction the camera is facing.
            
            Vector3 transformedReference =
                Vector3.Transform(thirdPersonReference, rotationQuat);

            // Calculate the position the camera is looking from.
            Vector3 cameraPosition = transformedReference + ((Game1)Game).modelManager.avatar.avatarPosition;
            
            // Set up the view matrix and projection matrix.
            view = Matrix.CreateLookAt(cameraPosition, ((Game1)Game).modelManager.avatar.avatarPosition,
                new Vector3(0.0f, 1.0f, 0.0f));

            Viewport viewport = Game.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;

            proj = Matrix.CreatePerspectiveFieldOfView(viewAngle, aspectRatio,
                nearClip, farClip);

            prevMouseState = Mouse.GetState();
        }
    }
}
