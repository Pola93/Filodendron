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
using SkinnedModel;
using FilodendronGame.Abilities;


namespace FilodendronGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GeneralModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // Everything with comment "for boxes" will be deleted later
        BasicModel box;
        Texture2D boxTexture;
        BasicModel wall;
		MachineSector sektorMaszyn;
        public static List<BasicModel> allModels = new List<BasicModel>();

        public Filodendron avatar;
        public Matrix World = Matrix.Identity; // for boxes

        //for explosion
        List<ParticleExplosion> explosions = new List<ParticleExplosion>();
        ParticleExplosionSettings particleExplosionSettings = new ParticleExplosionSettings();
        ParticleSettings particleSettings = new ParticleSettings();
        Texture2D explosionTexture;
        Texture2D explosionColorsTexture;
        Effect explosionEffect;

        //for ocean
        Ocean ocean;


        public GeneralModelManager(Game game)
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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            box = new BasicModel(Game.Content.Load<Model>(@"models\box"), Matrix.Identity);//Matrix.CreateTranslation(2000, 0, 185));
            boxTexture = Game.Content.Load<Texture2D>(@"textures/boxtexture");
            box.animation = new PlatformAnimation(new Vector3(20, 0, 185), 100);

            sektorMaszyn = new MachineSector(Game.Content.Load<Model>(@"models\pokoj01-02"), Matrix.Identity);
            sektorMaszyn.boundingBox = true;

            wall = new BasicModel(Game.Content.Load<Model>(@"models\box"), Matrix.Identity);//Matrix.CreateTranslation(20, 0, 265), 33.0f);
            wall.boundingBox = true;

            allModels.Add(wall);
            allModels.Add(box);
            allModels.Add(sektorMaszyn);

            avatar = new Filodendron(Game.Content.Load<Model>(@"models\dude"), Matrix.Identity);//Matrix.CreateTranslation(2500, 2200, 3500));
            avatar.skinningData = avatar.model.Tag as SkinningData;
            avatar.boundingBox = true;
            avatar.bullet = new Bullet(Game.Content.Load<Model>(@"models\box"), Matrix.Identity);

            if (avatar.skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            avatar.animationPlayer = new AnimationPlayer(avatar.skinningData);
            avatar.clip = avatar.skinningData.AnimationClips["Take 001"];
            avatar.avatarTexture = Game.Content.Load<Texture2D>(@"textures/avatartexture");
            avatar.CustomShader = Game.Content.Load<Effect>(@"effects/lightening");
            avatar.animationPlayer.StartClip(avatar.clip);
            avatar.avatarPosition.Y += 40;

            // Load explosion textures and effect  
            explosionTexture = Game.Content.Load<Texture2D>(@"Textures\Particle");
            explosionColorsTexture = Game.Content.Load<Texture2D>(@"Textures\ParticleColors");
            explosionEffect = Game.Content.Load<Effect>(@"effects\particle");

            // Set effect parameters that don't change per particle  
            explosionEffect.CurrentTechnique = explosionEffect.Techniques["Technique1"];
            explosionEffect.Parameters["theTexture"].SetValue(explosionTexture);

            //ocean
            ocean = new Ocean(Game.Content.Load<Model>(@"models\ocean"), Matrix.Identity);
            ocean.oceanEffect = Game.Content.Load<Effect>(@"effects\OceanShader");
            ocean.diffuseOceanTexture = Game.Content.Load<Texture2D>(@"textures\water");
            ocean.normalOceanTexture = Game.Content.Load<Texture2D>(@"textures\wavesbump");
            ocean.SetupOceanShaderParameters();

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            if (((Game1)Game).currentGameState == FilodendronGame.Game1.GameState.Playing)
            {
                avatar.Update(gameTime);
                avatar.bullet.Update(gameTime);
                sektorMaszyn.Update(gameTime);
                if (box != null)
                {
                    box.Update(gameTime);
                }               
                
                ocean.Update(gameTime);
                //if the side boundry of screen reached, set the mouse on the other side
                if (Mouse.GetState().X >= Game.Window.ClientBounds.Width)
                {
                    Mouse.SetPosition(1, Mouse.GetState().Y);// jak wstawia³em 0 to mi ekran przeskakiwa³ lol
                    avatar.prevMouseState = Mouse.GetState();
                }
                if (Mouse.GetState().X <= 0)
                {
                    Mouse.SetPosition(Game.Window.ClientBounds.Width, Mouse.GetState().Y);
                    avatar.prevMouseState = Mouse.GetState();
                }

                // Collision checkout
                if (box != null)
                {
                    if (avatar.rigidBody.CollidesWith(box.model, box.World))
                    {
                        // Collision! add an explosion. 
                        explosions.Add(new ParticleExplosion(GraphicsDevice, avatar,
                            box.World,
                            ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minLife,
                                particleExplosionSettings.maxLife),
                            ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minRoundTime,
                                particleExplosionSettings.maxRoundTime),
                            ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minParticlesPerRound,
                                particleExplosionSettings.maxParticlesPerRound),
                            ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minParticles,
                                particleExplosionSettings.maxParticles),
                                explosionColorsTexture, particleSettings, explosionEffect));
                        // delete the box
                        box = null;
                        ((Game1)Game).numberOfCoins++;
                        ((Game1)Game).Die();
                    }

                }
                // Update explosions
                UpdateExplosions(gameTime);

                base.Update(gameTime);
            }
        }

        protected void UpdateExplosions(GameTime gameTime)
        {
            // Loop through and update explosions  
            for (int i = 0; i < explosions.Count; ++i)
            {
                explosions[i].Update(gameTime);
                // If explosion is finished, remove it   
                if (explosions[i].IsDead)
                {
                    explosions.RemoveAt(i);
                    --i;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {

            if (((Game1)Game).currentGameState == FilodendronGame.Game1.GameState.Playing)
            {
                avatar.Draw(avatar.model, avatar.World, avatar.avatarTexture, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);
                sektorMaszyn.Draw(sektorMaszyn.model, sektorMaszyn.World, null, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);
                avatar.bullet.Draw(avatar.bullet.model, avatar.bullet.World, null, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);
                // Loop through and draw each particle explosion
                foreach (ParticleExplosion pe in explosions)
                {
                    pe.Draw(((Game1)Game).camera);
                }

                // for boxes
                if (box != null)
                {
                    box.Draw(box.model,
                            box.World,
                            boxTexture, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);
                }
                wall.Draw(wall.model,
                            wall.World,
                            boxTexture, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);
                
                ocean.Draw(ocean.model, ocean.World, ocean.diffuseOceanTexture, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);

                base.Draw(gameTime);
            }
        }
    }
}