using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Minalear;

namespace Minalear
{
    public class RenderEngine : IDisposable
    {
        private TextureRenderer renderer;

        private RenderTarget[] renderTargets;
        private List<RenderTask> renderTasks;

        public RenderEngine(Game game, int renderPasses)
        {
            renderTargets = new RenderTarget[renderPasses];
            for (int i = 0; i < renderPasses; i++)
            {
                renderTargets[i] = new RenderTarget(game.Window.Width, game.Window.Height);
            }

            renderTasks = new List<RenderTask>();
            renderer = new TextureRenderer(new Shader("Content/Shaders/tex.vert", "Content/Shaders/tex.frag"), game.Window.Width, game.Window.Height);
        }
        
        public void AddRenderTask(Texture2D texture, Vector2 position, Color4 color)
        {
            RenderTask task = new RenderTask()
            {
                Texture = texture,
                Position = position,
                Size = texture.Size,
                
                Rotation = 0f,
                Origin = Vector2.Zero,

                SourceRect = new RectangleF(Vector2.Zero, texture.Size),
                DrawColor = color
            };

            renderTasks.Add(task);
        }
        public void AddRenderTask(Texture2D texture, Vector2 position, Vector2 size, Color4 color)
        {
            RenderTask task = new RenderTask()
            {
                Texture = texture,
                Position = position,
                Size = size,
                
                Rotation = 0f,
                Origin = Vector2.Zero,

                SourceRect = new RectangleF(Vector2.Zero, texture.Size),
                DrawColor = color
            };

            renderTasks.Add(task);
        }
        public void AddRenderTask(Texture2D texture, Vector2 position, Vector2 size, float rotation, Vector2 origin, Color4 color)
        {
            RenderTask task = new RenderTask()
            {
                Texture = texture,
                Position = position,
                Size = size,
                
                Rotation = rotation,
                Origin = origin,

                SourceRect = new RectangleF(Vector2.Zero, texture.Size),
                DrawColor = color
            };

            renderTasks.Add(task);
        }

        public void ProcessRenderCalls()
        {
            foreach (RenderTask task in renderTasks)
            {
                renderer.Draw(task); 
            }

            renderTasks.Clear();
        }

        public void Dispose()
        {

        }
    }

    public struct RenderTask
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public Color4 DrawColor { get; set; }
        public RectangleF SourceRect { get; set; }
    }
}
