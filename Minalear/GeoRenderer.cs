using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Minalear
{
    public class GeoRenderer : IDisposable
    {
        private Shader shader;

        private int vao, vbo;
        private int colorLoc;

        public GeoRenderer(Shader shader, int width, int height)
        {
            this.shader = shader;

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            this.createShapeVertexInfo(new float[] { 0f, 0f });

            //Attributes
            shader.UseProgram();
            GL.EnableVertexAttribArray(0); //pos attribute
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            //Uniforms
            int model = GL.GetUniformLocation(shader.ID, "model");
            Matrix4 modelMat = Matrix4.Identity;
            GL.UniformMatrix4(model, false, ref modelMat);

            int proj = GL.GetUniformLocation(shader.ID, "proj");
            Matrix4 projMat = Matrix4.CreateOrthographicOffCenter(0f, width, height, 0f, -1f, 1f);
            GL.UniformMatrix4(proj, false, ref projMat);

            colorLoc = GL.GetUniformLocation(shader.ID, "DrawColor");
            GL.Uniform4(colorLoc, Color4.Black);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
        public void Resize(int width, int height)
        {
            int proj = GL.GetUniformLocation(shader.ID, "proj");
            Matrix4 projMat = Matrix4.CreateOrthographicOffCenter(0f, width, height, 0f, -1f, 1f);
            GL.UniformMatrix4(proj, false, ref projMat);
        }

        public void Begin()
        {
            shader.UseProgram();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        }
        public void End()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void DrawCircle(Vector2 position, float radius, int sides, Color4 color)
        {
            GL.Uniform4(colorLoc, color);

            createCircleVertexInfo(position, radius, sides);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, sides);
        }
        public void DrawRect(Vector2 position, Vector2 size, Color4 color)
        {
            GL.Uniform4(colorLoc, color);

            createRectVertexInfo(position, size);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, 4);
        }
        public void DrawLine(Vector2 pos1, Vector2 pos2, Color4 color)
        {
            GL.Uniform4(colorLoc, color);

            createLineVertexInfo(pos1, pos2);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
        }
        public void DrawArc(Vector2 center, float radius, int sides, float startAngle, float radians, Color4 color)
        {
            //Adjust the sides to match a full circle sides value
            sides = (int)(sides / (MathHelper.TwoPi / radians));
            GL.Uniform4(colorLoc, color);

            createArcVertexInfo(center, radius, sides, startAngle, radians);
            GL.DrawArrays(PrimitiveType.Lines, 0, sides * 2);
        }
        public void DrawShape(float[] vertices, Color4 color)
        {
            GL.Uniform4(colorLoc, color);

            createShapeVertexInfo(vertices);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, vertices.Length / 2);
        }
        public void DrawPixel(Vector2 position, Color4 color)
        {
            GL.Uniform4(colorLoc, color);

            createShapeVertexInfo(new float[] { position.X, position.Y });
            GL.DrawArrays(PrimitiveType.Points, 0, 2);
        }
        public void DrawPixels(float[] vertices, Color4 color)
        {
            GL.Uniform4(colorLoc, color);

            createShapeVertexInfo(vertices);
            GL.DrawArrays(PrimitiveType.Points, 0, vertices.Length / 2);
        }

        public void FillCircle(Vector2 position, float radius, int sides, Color4 color)
        {
            GL.Uniform4(colorLoc, color);

            createCircleVertexInfo(position, radius, sides);
            GL.DrawArrays(PrimitiveType.Polygon, 0, sides);
        }
        public void FillRect(Vector2 position, Vector2 size, Color4 color)
        {
            GL.Uniform4(colorLoc, color);

            createRectVertexInfo(position, size);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
        }
        public void FillShape(float[] vertices, Color4 color)
        {
            GL.Uniform4(colorLoc, color);

            createShapeVertexInfo(vertices);
            GL.DrawArrays(PrimitiveType.Polygon, 0, vertices.Length / 2);
        }

        public void Dispose()
        {
            this.shader.Dispose();
        }

        private void createCircleVertexInfo(Vector2 center, float radius, int sides)
        {
            float[] verts = new float[sides * 2];

            double div = MathHelper.TwoPi / sides;
            for (int i = 0; i < sides; i++)
            {
                verts[0 + i * 2] = (float)(Math.Cos(div * i) * radius) + center.X;
                verts[1 + i * 2] = (float)(Math.Sin(div * i) * radius) + center.Y;
            }

            createShapeVertexInfo(verts);
        }
        private void createArcVertexInfo(Vector2 center, float radius, int sides, float startAngle, float radians)
        {
            float[] verts = new float[sides * 4];

            double div = radians / sides;
            for (int i = 0; i < sides; i++)
            {
                verts[0 + i * 4] = (float)(Math.Cos(div * i + startAngle) * radius) + center.X;
                verts[1 + i * 4] = (float)(Math.Sin(div * i + startAngle) * radius) + center.Y;

                verts[2 + i * 4] = (float)(Math.Cos(div * (i + 1) + startAngle) * radius) + center.X;
                verts[3 + i * 4] = (float)(Math.Sin(div * (i + 1) + startAngle) * radius) + center.Y;
            }

            createShapeVertexInfo(verts);
        }
        private void createRectVertexInfo(Vector2 position, Vector2 size)
        {
            float[] verts = new float[8];

            //0.5 offset to fix rendering issues
            verts[0] = position.X - 0.5f;
            verts[1] = position.Y - 0.5f;

            verts[2] = position.X - 0.5f;
            verts[3] = position.Y + size.Y + 0.5f;

            verts[4] = position.X + size.X + 0.5f;
            verts[5] = position.Y + size.Y + 0.5f;

            verts[6] = position.X + size.X + 0.5f;
            verts[7] = position.Y - 0.5f;

            createShapeVertexInfo(verts);
        }
        private void createLineVertexInfo(Vector2 pos1, Vector2 pos2)
        {
            float[] verts = new float[4];

            verts[0] = pos1.X;
            verts[1] = pos1.Y;

            verts[2] = pos2.X;
            verts[3] = pos2.Y;

            createShapeVertexInfo(verts);
        }
        private void createShapeVertexInfo(float[] verts)
        {
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(verts.Length * sizeof(float)), verts, BufferUsageHint.DynamicDraw);
        }
    }
}
