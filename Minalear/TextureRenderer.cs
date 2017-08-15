using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Minalear
{
    public class TextureRenderer : IDisposable
    {
        private int vao;
        private Shader shader;

        //Uniform Locations
        private int modelLoc;
        private int projLoc;
        private int texMatLoc;
        private int colorLoc;
        private int stepLoc;
        private int outlineLoc;

        public Shader Shader { get { return shader; } }
        public bool DrawOutline { get; set; }

        public TextureRenderer(Shader shader, int renderWidth, int renderHeight)
        {
            this.shader = shader;

            float[] vertices = new float[] {
                0f, 0f,     0f, 0f,
                0f, 1f,     0f, 1f,
                1f, 0f,     1f, 0f,

                1f, 0f,     1f, 0f,
                0f, 1f,     0f, 1f,
                1f, 1f,     1f, 1f,
            };

            vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            shader.UseProgram();

            //Attributes
            int posAttrib = /*GL.GetAttribLocation(shader.ID, "pos")*/0;
            GL.EnableVertexAttribArray(posAttrib);
            GL.VertexAttribPointer(posAttrib, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            int uvAttrib = /*GL.GetAttribLocation(shader.ID, "uv")*/1;
            GL.EnableVertexAttribArray(uvAttrib);
            GL.VertexAttribPointer(uvAttrib, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            //Locations
            modelLoc = GL.GetUniformLocation(shader.ID, "model");
            projLoc = GL.GetUniformLocation(shader.ID, "proj");
            texMatLoc = GL.GetUniformLocation(shader.ID, "texMatrix");
            colorLoc = GL.GetUniformLocation(shader.ID, "drawColor");
            stepLoc = GL.GetUniformLocation(shader.ID, "stepSize");
            outlineLoc = GL.GetUniformLocation(shader.ID, "drawOutline");

            //Projection Matrix
            Matrix4 projMat4 = Matrix4.CreateOrthographicOffCenter(0f, renderWidth, renderHeight, 0f, -1f, 1f);
            GL.UniformMatrix4(projLoc, false, ref projMat4);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.DeleteBuffer(vbo);
        }

        public void Draw(Texture2D texture, Vector2 position, Vector2 size, Color4 color)
        {
            shader.UseProgram();
            setUniforms(texture, position, size, Vector2.Zero, 0f, Vector2.Zero, texture.Size, color);

            drawArrays(texture);
        }
        public void Draw(Texture2D texture, Vector2 position, float scale, Color4 color)
        {
            shader.UseProgram();
            Vector2 size = texture.Size * scale;
            setUniforms(texture, position, size, Vector2.Zero, 0f, Vector2.Zero, texture.Size, color);

            drawArrays(texture);
        }
        public void Draw(Texture2D texture, Vector2 position, Vector2 size, float rotation, Color4 color)
        {
            shader.UseProgram();
            Vector2 center = size / 2f;
            setUniforms(texture, position, size, center, rotation, Vector2.Zero, texture.Size, color);

            drawArrays(texture);
        }
        public void Draw(Texture2D texture, Vector2 position, Vector2 size, RectangleF source)
        {
            shader.UseProgram();
            setUniforms(texture, position, size, Vector2.Zero, 0f, new Vector2(source.X, source.Y), new Vector2(source.Width, source.Height), Color4.White);

            drawArrays(texture);
        }
        public void Draw(Texture2D texture, Vector2 position, Vector2 size, float rotation, Vector2 origin, Color4 color)
        {
            shader.UseProgram();
            setUniforms(texture, position, size, size * origin, rotation, Vector2.Zero, texture.Size, color);

            drawArrays(texture);
        }

        public void Draw(RenderTask task)
        {
            shader.UseProgram();
            setUniforms(task.Texture, task.Position, task.Size, task.Size * task.Origin, task.Rotation, task.SourceRect.Position, task.SourceRect.Size, task.DrawColor);

            drawArrays(task.Texture);
        }

        private void setUniforms(Texture2D tex, Vector2 position, Vector2 size, Vector2 origin, float rotation, Vector2 sourcePos, Vector2 sourceSize, Color4 color)
        {
            //Matrices
            Matrix4 model =
                Matrix4.CreateScale(size.X, size.Y, 0f) *
                Matrix4.CreateTranslation(-origin.X, -origin.Y, 0f) *
                Matrix4.CreateRotationZ(rotation) *
                Matrix4.CreateTranslation(origin.X, origin.Y, 0f) *
                Matrix4.CreateTranslation(position.X, position.Y, 0f);
            GL.UniformMatrix4(modelLoc, false, ref model);

            Vector2 textureSize = tex.Size;
            Matrix4 source =
                Matrix4.CreateScale(sourceSize.X / textureSize.X, sourceSize.Y / textureSize.Y, 0f) *
                Matrix4.CreateTranslation(sourcePos.X / textureSize.X, sourcePos.Y / textureSize.Y, 0f);
            GL.UniformMatrix4(texMatLoc, false, ref source);

            //Color
            GL.Uniform4(colorLoc, color);

            //Outline
            if (DrawOutline)
            {
                Vector2 stepSize = new Vector2(1f / textureSize.X, 1f / textureSize.Y);
                GL.Uniform2(stepLoc, ref stepSize);
                GL.Uniform1(outlineLoc, 1);
            }
            else
            {
                GL.Uniform1(outlineLoc, 0);
            }
        }
        private void drawArrays(Texture2D texture)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Bind();

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            shader.Dispose();
        }
    }
}
