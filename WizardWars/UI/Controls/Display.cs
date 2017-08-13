using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class Display : Control
    {
        private Texture2D texture;
        private Font font;
        private string text = "TEST";

        private bool markedForUpdate = false;

        public string Text { get { return text; } set { SetText(value); } }

        public Display(Control parent)
            : base(parent) { }

        public override void LoadContent()
        {
            font = new Font("Tahoma", 10f, FontStyle.Regular);
            initGLTexture();
        }
        public override void UnloadContent()
        {
            font.Dispose();
            texture.Dispose();
        }
        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            if (texture != null)
            {
                Vector2 position = Position;
                renderer.AddRenderTask(texture, position, Size, Color4.White);
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (markedForUpdate)
            {
                initGLTexture();
                markedForUpdate = false;
            }
        }

        public void SetText(string value)
        {
            text = value;

            if (ContentLoaded)
                initGLTexture();
            else
                markedForUpdate = true;
        }

        private void initGLTexture()
        {
            Size size = calculateTextSize(text);
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            this.Size = new Vector2(size.Width, size.Height);

            graphics.Clear(Color.FromArgb(125, 25, 25, 25));
            graphics.DrawString(text, font, Brushes.WhiteSmoke, 0f, 0f);

            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (texture != null)
                texture.Dispose();

            texture = new Texture2D(bmp.Width, bmp.Height, data.Scan0);

            bmp.UnlockBits(data);

            graphics.Dispose();
            bmp.Dispose();
        }
        private Size calculateTextSize(string text)
        {
            Bitmap test = new Bitmap(1, 1);
            Graphics gfx = Graphics.FromImage(test);

            SizeF sizeF = gfx.MeasureString(text, font);

            gfx.Dispose();
            test.Dispose();

            return new Size((int)sizeF.Width, (int)sizeF.Height);
        }
    }
    
}
