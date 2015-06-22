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
        public Vector3 cameraPosition;
        static int screenWidth = 1366, screenHeight = 768;
        // Set the direction the camera points without rotation.
        public Vector3 thirdPersonReference = new Vector3(0, 400, -400);

        // Set field of view of the camera in radians (pi/4 is 45 degrees).
        static float viewAngle = MathHelper.PiOver4;
        static float FOV = MathHelper.PiOver4;
        static float aspectRatio = (float)screenWidth / (float)screenHeight;
        double time;
        // Set distance from the camera of the near and far clipping planes.
        static float nearClip = 1.0f;
        static float farClip = 15000.0f;
        public float rotationSpeed = 1f / 500f;
        public float cameraPitch = 0;

        MouseState prevMouseState;
        Curve3D cameraCurvePosition = new Curve3D();
        Curve3D cameraCurveLookat = new Curve3D();
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
            InitCurve();
            base.Initialize();
        }
        void InitCurve()
        {
            float time = 0;
            cameraCurvePosition.AddPoint(new Vector3(-4200, 200, 4000), time);  //start
            cameraCurveLookat.AddPoint(new Vector3(-3000, 30, 5000), time); //patrzymy na 1 box
            time += 4000;
            cameraCurvePosition.AddPoint(new Vector3(-3000, 850, 5000), time); // lecimy do gory
            time += 4000;
            cameraCurveLookat.AddPoint(new Vector3(1000, 1600, 3600), time); // patrzymy na br¹zow¹ skrzynie
            cameraCurvePosition.AddPoint(new Vector3(1500, 1650, 500), time); // lecimy na lewo od brazowej skrzyni
            time += 4000;
            cameraCurveLookat.AddPoint(new Vector3(5000, 2400, 3650), time); // patrzymy na cogi i polki po prawej
            cameraCurvePosition.AddPoint(new Vector3(3000, 2500, 3000), time); // lecimy w strone pó³ek
            time += 4000;
            cameraCurveLookat.AddPoint(new Vector3(5000, 0, -1550), time); // patrzymy na dó³
            time += 4000;
            cameraCurveLookat.AddPoint(new Vector3(5000, 2400, -1550), time); // patrzymy w lewo na cogi i polki
            cameraCurvePosition.AddPoint(new Vector3(4000, 2500, 3000), time); // lecimy troche w lewo zeby pokazac naroznik
            time += 4000;
            cameraCurveLookat.AddPoint(new Vector3(-3500, 1000, -2000), time); // patrzymy na ostatni¹ lew¹ œcianê 
            cameraCurvePosition.AddPoint(new Vector3(-3000, 2500, 1000), time); // lecimy w lewo do wyjscia
            time += 4000;
            cameraCurveLookat.AddPoint(new Vector3(-6449, 1000, -3058), time); // patrzymy na wyjscie
            cameraCurvePosition.AddPoint(new Vector3(-5000, 2500, -3058), time); // lecimy powoli w strone wyjscia
            time += 5000;
            
            cameraCurvePosition.SetTangents();
            cameraCurveLookat.SetTangents();
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (((Game1)Game).currentGameState == FilodendronGame.Game1.GameState.CameraRoll)
            {
                UpdateCameraCurve(gameTime);
            }
            // TODO: Add your update code here

            if (((Game1)Game).currentGameState == FilodendronGame.Game1.GameState.Playing)
            {
            
                this.cameraPosition = UpdateCameraThirdPerson();
            }
            if (((Game1)Game).currentGameState == FilodendronGame.Game1.GameState.AvatarRoll)
            {
                UpdateIdleCurve(gameTime);
            }
           
            base.Update(gameTime);
        }
        void UpdateCameraCurve(GameTime gameTime)
        {
            // Calculate the camera's current position.
            Vector3 cameraPosition =
                cameraCurvePosition.GetPointOnCurve((float)time);
            Vector3 cameraLookat =
                cameraCurveLookat.GetPointOnCurve((float)time);

            // Set up the view matrix and projection matrix.
            view = Matrix.CreateLookAt(cameraPosition, cameraLookat,
                new Vector3(0.0f, 1.0f, 0.0f));

            proj = Matrix.CreatePerspectiveFieldOfView(FOV, aspectRatio,
                nearClip, farClip);

            time += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        void UpdateIdleCurve(GameTime gameTime)
        {
            // Calculate the camera's current position.
            Vector3 cameraPosition =
                ((Game1)Game).modelManager.avatar.avatarPosition;
            Vector3 cameraLookat =
                ((Game1)Game).modelManager.avatar.avatarPosition;

            // Set up the view matrix and projection matrix.
            view = Matrix.CreateLookAt(cameraPosition, cameraLookat,
                new Vector3(0.0f, 1.0f, 0.0f));

            proj = Matrix.CreatePerspectiveFieldOfView(FOV, aspectRatio,
                nearClip, farClip);

            time += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        
        
        /// <summary>
        /// Updates the camera when it's in the 3rd person state.
        /// </summary>
        /// 

        Vector3 UpdateCameraThirdPerson()
        {
            // Counting pitch angle to rotate
            if (Mouse.GetState().Y != prevMouseState.Y && this.cameraPosition.Y > ((Game1)Game).modelManager.avatar.avatarPosition.Y)
            {
                cameraPitch -= (Mouse.GetState().Y - prevMouseState.Y) * rotationSpeed;
            }
            Quaternion rotationQuat = Quaternion.CreateFromYawPitchRoll(((Game1)Game).modelManager.avatar.avatarYaw, cameraPitch, 0);
            // Create a vector pointing the direction the camera is facing.
            
            Vector3 transformedReference =
                Vector3.Transform(thirdPersonReference, rotationQuat);

            // Calculate the position the camera is looking from.
            cameraPosition = transformedReference + ((Game1)Game).modelManager.avatar.avatarPosition;
            
            // Set up the view matrix and projection matrix.
            view = Matrix.CreateLookAt(cameraPosition, ((Game1)Game).modelManager.avatar.avatarPosition,
                new Vector3(0.0f, 1.0f, 0.0f));

            Viewport viewport = Game.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;

            proj = Matrix.CreatePerspectiveFieldOfView(viewAngle, aspectRatio,
                nearClip, farClip);

            prevMouseState = Mouse.GetState();

            return cameraPosition;
        }
    }
}
