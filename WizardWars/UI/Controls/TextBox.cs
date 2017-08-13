using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class TextBox : Control
    {
        private string text;
        
        protected Texture2D texture;
        protected Font font;
        protected Color4 textColor;

        public Color4 TextColor { get { return textColor; } set { textColor = value; } }

        public TextBox(Control parent, string text)
            : base(parent)
        {
            this.text = text;
            textColor = Color4.Black;
        }

        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            renderer.AddRenderTask(texture, Position, Size, textColor);
        }

        public override void LoadContent()
        {
            font = new Font("Tahoma", 12f, FontStyle.Regular);
            initGLTexture();
        }
        public override void UnloadContent()
        {
            font.Dispose();
            texture.Dispose();
        }

        public void SetText(string text)
        {
            this.text = text;
            initGLTexture();
        }

        protected void initGLTexture()
        {
            Size size = calculateTextSize();
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            this.Size = new Vector2(size.Width, size.Height);

            graphics.Clear(Color.Transparent);
            graphics.DrawString(text, font, Brushes.White, 0f, 0f);

            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (texture == null)
            {
                texture = new Texture2D(bmp.Width, bmp.Height, data.Scan0);
            }
            else if (texture.Width != bmp.Width || texture.Height != bmp.Height)
            {
                texture.Dispose();
                texture = new Texture2D(bmp.Width, bmp.Height, data.Scan0);
            }
            else
            {
                texture.UpdateTexture(data.Scan0);
            }

            bmp.UnlockBits(data);

            graphics.Dispose();
            bmp.Dispose();
        }
        private Size calculateTextSize()
        {
            Bitmap test = new Bitmap(1, 1);
            Graphics gfx = Graphics.FromImage(test);

            SizeF sizeF = gfx.MeasureString(text, font);

            gfx.Dispose();
            test.Dispose();

            return new Size((int)sizeF.Width, (int)sizeF.Height);
        }

        public string Text { get { return text; } set { SetText(value); } }
    }
}
