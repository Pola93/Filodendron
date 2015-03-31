using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FilodendronGame
{
    class Ocean : BasicModel
    {
        public Effect oceanEffect;
        public Texture2D diffuseOceanTexture;
        public Texture2D normalOceanTexture;

        // Parameters for Ocean shader
        EffectParameter projectionOceanParameter;
        EffectParameter viewOceanParameter;
        EffectParameter worldOceanParameter;
        EffectParameter ambientIntensityOceanParameter;
        EffectParameter ambientColorOceanParameter;

        EffectParameter diffuseIntensityOceanParameter;
        EffectParameter diffuseColorOceanParameter;
        EffectParameter lightDirectionOceanParameter;

        EffectParameter eyePosOceanParameter;
        EffectParameter specularColorOceanParameter;

        EffectParameter colorMapTextureOceanParameter;
        EffectParameter normalMapTextureOceanParameter;
        EffectParameter totalTimeOceanParameter;
        float totalTime = 0.0f;
        
        public Ocean(Model m, Matrix world)
            : base(m, world)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            totalTime += gameTime.ElapsedGameTime.Milliseconds / 5000.0f;
        }

        public void SetupOceanShaderParameters()
        {
            // Bind the parameters with the shader.
            worldOceanParameter = oceanEffect.Parameters["World"];
            viewOceanParameter = oceanEffect.Parameters["View"];
            projectionOceanParameter = oceanEffect.Parameters["Projection"];

            ambientColorOceanParameter = oceanEffect.Parameters["AmbientColor"];
            ambientIntensityOceanParameter = oceanEffect.Parameters["AmbientIntensity"];

            diffuseColorOceanParameter = oceanEffect.Parameters["DiffuseColor"];
            diffuseIntensityOceanParameter = oceanEffect.Parameters["DiffuseIntensity"];
            lightDirectionOceanParameter = oceanEffect.Parameters["LightDirection"];

            eyePosOceanParameter = oceanEffect.Parameters["EyePosition"];
            specularColorOceanParameter = oceanEffect.Parameters["SpecularColor"];

            colorMapTextureOceanParameter = oceanEffect.Parameters["ColorMap"];
            normalMapTextureOceanParameter = oceanEffect.Parameters["NormalMap"];
            totalTimeOceanParameter = oceanEffect.Parameters["TotalTime"];
        }

        public override void Draw(Model model, Matrix world, Texture2D texture, Camera camera, GameTime gameTime, GraphicsDeviceManager graphics)
        {
            ModelMesh mesh = model.Meshes[0];
            ModelMeshPart meshPart = mesh.MeshParts[0];

            // Set parameters
            projectionOceanParameter.SetValue(camera.proj);
            viewOceanParameter.SetValue(camera.view);
            worldOceanParameter.SetValue(
                Matrix.CreateRotationY((float)MathHelper.ToRadians((int)270)) 
                * Matrix.CreateRotationZ((float)MathHelper.ToRadians((int)90)) 
                * Matrix.CreateScale(10.0f) * Matrix.CreateTranslation(0, -60, 0));
            ambientIntensityOceanParameter.SetValue(0.4f);
            ambientColorOceanParameter.SetValue(Color.White.ToVector4());
            diffuseColorOceanParameter.SetValue(Color.White.ToVector4());
            diffuseIntensityOceanParameter.SetValue(0.2f);
            specularColorOceanParameter.SetValue(Color.White.ToVector4());
            eyePosOceanParameter.SetValue(camera.cameraPosition);
            colorMapTextureOceanParameter.SetValue(diffuseOceanTexture);
            normalMapTextureOceanParameter.SetValue(normalOceanTexture);
            totalTimeOceanParameter.SetValue(totalTime);

            Vector3 lightDirection = new Vector3(1.0f, 0.0f, -1.0f);

            //ensure the light direction is normalized, or
            //the shader will give some weird results
            lightDirection.Normalize();
            lightDirectionOceanParameter.SetValue(lightDirection);

            //set the vertex source to the mesh's vertex buffer
            graphics.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);

            //set the current index buffer to the sample mesh's index buffer
            graphics.GraphicsDevice.Indices = meshPart.IndexBuffer;

            graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            oceanEffect.CurrentTechnique = oceanEffect.Techniques["Technique1"];

            for (int i = 0; i < oceanEffect.CurrentTechnique.Passes.Count; i++)
            {
                //EffectPass.Apply will update the device to
                //begin using the state information defined in the current pass
                oceanEffect.CurrentTechnique.Passes[i].Apply();

                //theMesh contains all of the information required to draw
                //the current mesh
                graphics.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList, 0, 0,
                    meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
            }   
        }
    }
}
