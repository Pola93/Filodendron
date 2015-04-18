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
    public class GeneralModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // Everything with comment "for boxes" will be deleted later
        BasicModel box;
        Texture2D boxTexture;
        Texture2D avatarTexture; // ##########czy textury trzymac w modelmanager czy w klasach poszczegolnych modeli?

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
            box = new BasicModel(Game.Content.Load<Model>(@"models\box"), Matrix.CreateTranslation(20,0,185));
            boxTexture = Game.Content.Load<Texture2D>(@"textures/boxtexture");
            
            avatar = new Filodendron(Game.Content.Load<Model>(@"models\spaceship"), Matrix.Identity);
            avatarTexture = Game.Content.Load<Texture2D>(@"textures/avatartexture");
            avatar.CustomShader = Game.Content.Load<Effect>(@"effects/lightening");

            // Load explosion textures and effect  
            explosionTexture = Game.Content.Load<Texture2D>(@"Textures\Particle"); 
            explosionColorsTexture = Game.Content.Load<Texture2D>(@"Textures\ParticleColors");
            explosionEffect = Game.Content.Load<Effect>(@"effects\particle");

            // Set effect parameters that don't change per particle  
            explosionEffect.CurrentTechnique = explosionEffect.Techniques["Technique1"];  
            explosionEffect.Parameters["theTexture"].SetValue(explosionTexture);

            //ocean
            ocean = new Ocean(Game.Content.Load<Model>(@"models\ocean"), Matrix.Identity );
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
            // TODO: Add your update code here
            avatar.Update(gameTime);
            ocean.Update(gameTime);
            //if the side boundry of screen reached, set the mouse on the other side
            if (Mouse.GetState().X >= Game.Window.ClientBounds.Width)
            {
                Mouse.SetPosition(1, Mouse.GetState().Y);// jak wstawia�em 0 to mi ekran przeskakiwa� lol
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
                }
            }
            // Update explosions
            UpdateExplosions(gameTime);

            base.Update(gameTime);
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
            avatar.Draw(avatar.model, avatar.World, avatarTexture, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);

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

            ocean.Draw(ocean.model, ocean.World, ocean.diffuseOceanTexture, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);
                
            base.Draw(gameTime);
        }
    }
}
