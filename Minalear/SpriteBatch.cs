using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Minalear
{
    public class SpriteBatch : IDisposable
    {
        private int vao;
        private Shader shader;

        //Uniform Locations
        private int modelLoc;
        private int projLoc;

        public SpriteBatch(Shader shader, int renderWidth, int renderHeight)
        {
            this.shader = shader;

            float[] vertices = new float[] {
                0f, 0f,     //0f, 0f,
                0f, 1f,     //0f, 1f,
                1f, 0f,     //1f, 0f,

                1f, 0f,     //1f, 0f,
                0f, 1f,     //0f, 1f,
                1f, 1f,     //1f, 1f,
            };

            vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            shader.UseProgram();

            //Attributes
            int posAttrib = GL.GetAttribLocation(shader.ID, "pos");
            GL.EnableVertexAttribArray(posAttrib);
            GL.VertexAttribPointer(posAttrib, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            //int texAttrib = GL.GetAttribLocation(shader.ID, "texCoords");
            //GL.EnableVertexAttribArray(texAttrib);
            //GL.VertexAttribPointer(texAttrib, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            //Locations
            modelLoc = GL.GetUniformLocation(shader.ID, "model");
            projLoc = GL.GetUniformLocation(shader.ID, "proj");

            //Projection Matrix
            Matrix4 projMat4 = Matrix4.CreateOrthographicOffCenter(0f, renderWidth, renderHeight, 0f, -1f, 1f);
            GL.UniformMatrix4(projLoc, false, ref projMat4);
        }

        public void Draw(Texture2D texture, Vector2 position, Vector2 scale)
        {
            shader.UseProgram();
            setUniforms(position, 0f, Vector2.Zero, new Vector2(texture.Width * scale.X, texture.Height * scale.Y));

            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Bind();

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }

        private void setUniforms(Vector2 position, float rotation, Vector2 origin, Vector2 size)
        {
            //Matrix
            Matrix4 model =
                Matrix4.CreateScale(size.X, size.Y, 0f) *
                Matrix4.CreateTranslation(-origin.X, -origin.Y, 0f) *
                Matrix4.CreateRotationZ(rotation) *
                Matrix4.CreateTranslation(position.X, position.Y, 0f);
            GL.UniformMatrix4(modelLoc, false, ref model);
        }

        public void Dispose()
        {
            shader.Dispose();
        }
    }
}
