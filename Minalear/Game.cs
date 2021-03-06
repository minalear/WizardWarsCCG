﻿using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Minalear
{
    public class Game : IDisposable
    {
        private GameTime gameTime;

        public Color4 ClearColor { get; set; }
        public GameWindow Window { get; private set; }
        public bool UpdateBackground { get; set; }

        public Game(int width, int height, string title)
        {
            Window = new GameWindow(width, height, new GraphicsMode(32, 24, 8, 4), title);
            Window.UpdateFrame += (sender, e) => updateFrame(e);
            Window.RenderFrame += (sender, e) => renderFrame(e);
            Window.Resize += (sender, e) => Resize();

            ClearColor = Color4.Black;
            UpdateBackground = true;

            //Enable Alpha blending
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            gameTime = new GameTime();
        }

        public void Run()
        {
            Initialize();
            LoadContent();

            Window.Run();
        }
        public void Resize()
        {
            GL.ClearColor(ClearColor);
            GL.Viewport(0, 0, Window.Width, Window.Height);
        }

        public virtual void Initialize() { }
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }

        public void Dispose()
        {
            UnloadContent();
            Window.Dispose();
        }

        protected virtual void updateFrame(FrameEventArgs e)
        {
            if (Window.Focused || UpdateBackground)
            {
                gameTime.ElapsedTime = TimeSpan.FromSeconds(e.Time);
                gameTime.TotalTime.Add(gameTime.ElapsedTime);

                Update(gameTime);
            }
        }
        protected virtual void renderFrame(FrameEventArgs e)
        {
            GL.ClearColor(ClearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Draw(gameTime);
            Window.SwapBuffers();
        }
    }
}
