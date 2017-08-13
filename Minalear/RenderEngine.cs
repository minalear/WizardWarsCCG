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
        private List<RenderTask> renderTasks, effectsTasks;

        public RenderEngine(Game game, int renderPasses)
        {
            renderTargets = new RenderTarget[renderPasses];
            for (int i = 0; i < renderPasses; i++)
            {
                renderTargets[i] = new RenderTarget(game.Window.Width, game.Window.Height);
            }

            renderTasks = new List<RenderTask>();
            effectsTasks = new List<RenderTask>();
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

        public void AddOutlineTask(Texture2D texture, Vector2 position, Vector2 size, float rotation, Vector2 origin, Color4 color)
        {
            //Texture will be the first Framebuffer
            RenderTask task = new RenderTask()
            {
                Texture = texture,
                Position = position,
                Size = size,
                Rotation = rotation,
                Origin = origin,
                DrawColor = color,

                SourceRect = new RectangleF(Vector2.Zero, texture.Size),
            };

            effectsTasks.Add(task);
        }

        public void ProcessRenderCalls()
        {
            //First (Main) render pass
            renderTargets[0].Bind();
            GL.ClearColor(Color4.Transparent);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            foreach (RenderTask task in renderTasks)
            {
                renderer.Draw(task); 
            }
            renderTargets[0].Unbind();

            //Effects pass
            foreach (RenderTask task in effectsTasks)
            {
                //Clear renderTargets[1]
                renderTargets[1].Bind();
                GL.ClearColor(Color4.Transparent);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                //Draw card art to it
                renderer.Draw(task);

                //Bind renderTargets[0] and draw outline on top
                renderTargets[0].Bind();

                renderer.DrawOutline = true;
                RectangleF source = new RectangleF(task.Position, task.Size);
                renderer.Draw(renderTargets[1].Texture, new Vector2(0, 720), new Vector2(1280, -720), task.DrawColor);
                renderer.DrawOutline = false;

                renderTargets[0].Unbind();
            }

            //Cleanup task lists
            renderTasks.Clear();
            effectsTasks.Clear();

            //Render all targets to the screen (negative height and Y offset to draw correctly)
            Vector2 position = new Vector2(0, renderTargets[0].Texture.Height);
            Vector2 size = new Vector2(renderTargets[0].Texture.Width, -renderTargets[0].Texture.Height);

            renderer.Draw(renderTargets[0].Texture, position, size, Color4.White);
        }

        public void Dispose()
        {
            renderTasks.Clear();
            effectsTasks.Clear();

            renderer.Dispose();
            for (int i = 0; i < renderTargets.Length; i++)
                renderTargets[i].Dispose();
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
