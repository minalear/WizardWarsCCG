using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

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

        public void AddRenderTask(Texture2D texture, Vector2 position, Color4 color, float drawOrder = 0f)
        {
            RenderTask task = new RenderTask()
            {
                Texture = texture,
                Position = position,
                Size = texture.Size,

                Rotation = 0f,
                Origin = Vector2.Zero,

                SourceRect = new RectangleF(Vector2.Zero, texture.Size),
                DrawColor = color,
                DrawOrder = drawOrder
            };

            renderTasks.Add(task);
        }
        public void AddRenderTask(Texture2D texture, Vector2 position, Vector2 size, Color4 color, float drawOrder = 0f)
        {
            RenderTask task = new RenderTask()
            {
                Texture = texture,
                Position = position,
                Size = size,
                
                Rotation = 0f,
                Origin = Vector2.Zero,

                SourceRect = new RectangleF(Vector2.Zero, texture.Size),
                DrawColor = color,
                DrawOrder = drawOrder
            };

            renderTasks.Add(task);
        }
        public void AddRenderTask(Texture2D texture, Vector2 position, Vector2 size, RectangleF source, Color4 color, float drawOrder = 0f)
        {
            RenderTask task = new RenderTask()
            {
                Texture = texture,
                Position = position,
                Size = size,

                Rotation = 0f,
                Origin = Vector2.Zero,

                SourceRect = source,
                DrawColor = color,
                DrawOrder = drawOrder
            };

            renderTasks.Add(task);
        }
        public void AddRenderTask(Texture2D texture, Vector2 position, Vector2 size, float rotation, Vector2 origin, Color4 color, float drawOrder = 0f)
        {
            RenderTask task = new RenderTask()
            {
                Texture = texture,
                Position = position,
                Size = size,
                
                Rotation = rotation,
                Origin = origin,

                SourceRect = new RectangleF(Vector2.Zero, texture.Size),
                DrawColor = color,
                DrawOrder = drawOrder
            };

            renderTasks.Add(task);
        }

        public void AddOutlineTask(Texture2D texture, Vector2 position, Vector2 size, Color4 drawColor, RectangleF source, Color4 outlineColor, float drawOrder = 0f)
        {
            RenderTask task = new RenderTask()
            {
                Texture = texture,
                Position = position,
                Size = size,
                Rotation = 0f,
                Origin = Vector2.Zero,
                DrawColor = drawColor,

                Outline = true,
                OutlineColor = outlineColor,

                SourceRect = source,
                DrawOrder = drawOrder
            };

            renderTasks.Add(task);
        }

        public void AddOutlineTask(Texture2D texture, Vector2 position, Vector2 size, Color4 drawColor, float rotation, Vector2 origin, Color4 outlineColor, float drawOrder = 0f)
        {
            RenderTask task = new RenderTask()
            {
                Texture = texture,
                Position = position,
                Size = size,
                Rotation = rotation,
                Origin = origin,
                DrawColor = drawColor,

                Outline = true,
                OutlineColor = outlineColor,

                SourceRect = new RectangleF(Vector2.Zero, texture.Size),
                DrawOrder = drawOrder
            };

            renderTasks.Add(task);
        }

        public void ProcessRenderCalls()
        {
            //Sort renderTasks
            renderTasks.Sort((x, y) => -(x.DrawOrder.CompareTo(y.DrawOrder)));

            //First (Main) render pass
            renderTargets[0].Bind();
            GL.ClearColor(Color4.Transparent);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            foreach (RenderTask task in renderTasks)
            {
                renderTargets[0].Bind();
                renderer.Draw(task);

                if (task.Outline)
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
                    renderer.Draw(renderTargets[1].Texture, new Vector2(0, 720), new Vector2(1280, -720), task.OutlineColor);
                    renderer.DrawOutline = false;

                    renderTargets[0].Unbind();
                }
            }
            renderTargets[0].Unbind();

            //Cleanup task lists
            renderTasks.Clear();

            //Render all targets to the screen (negative height and Y offset to draw correctly)
            Vector2 position = new Vector2(0, renderTargets[0].Texture.Height);
            Vector2 size = new Vector2(renderTargets[0].Texture.Width, -renderTargets[0].Texture.Height);

            renderer.Draw(renderTargets[0].Texture, position, size, Color4.White);
        }

        public void Dispose()
        {
            renderTasks.Clear();

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
        public float DrawOrder { get; set; }
        public bool Outline { get; set; }
        public Color4 OutlineColor { get; set; }
    }
}
