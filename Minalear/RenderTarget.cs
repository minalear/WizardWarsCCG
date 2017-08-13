using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Minalear
{
    public sealed class RenderTarget : IDisposable
    {
        private int fbo;
        private Texture2D texture;

        public RenderTarget(int width, int height)
        {
            //Generate framebuffer object
            fbo = GL.GenFramebuffer();

            //Bind framebuffer and create a blank texture
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            texture = new Texture2D(width, height, IntPtr.Zero);

            //Attach texture to the framebuffer
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture.ID, 0);

            //Unbind both framebuffer and texture
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
        }
        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        
        public void Dispose()
        {
            GL.DeleteFramebuffer(fbo);
            texture.Dispose();
        }

        public int FBO { get { return fbo; } }
        public Texture2D Texture { get { return texture; } }
    }
}
