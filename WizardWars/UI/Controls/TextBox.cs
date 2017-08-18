using System.Text;
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
        protected float fontSize;

        public TextBox(Control parent, string text, float fontSize)
            : base(parent)
        {
            this.text = text;
            this.textColor = Color4.Black;
            this.fontSize = fontSize;
        }

        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            renderer.AddRenderTask(texture, Position, Size, textColor, -1f);

            base.Draw(gameTime, renderer);
        }

        public override void LoadContent()
        {
            font = new Font("Tahoma", fontSize, FontStyle.Regular);
            initGLTexture();

            base.LoadContent();
        }
        public override void UnloadContent()
        {
            font.Dispose();
            texture.Dispose();

            base.UnloadContent();
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

            SizeF sizeF = new SizeF(0f, 0f);

            if (WordWrap)
            {
                text = wrapText(gfx, text);
            }
            sizeF = gfx.MeasureString(text, font);

            gfx.Dispose();
            test.Dispose();

            int width = (int)MathHelper.Clamp(sizeF.Width, 1, sizeF.Width + 1);
            int height = (int)MathHelper.Clamp(sizeF.Height, 1, sizeF.Height + 1);

            return new Size(width, height);
        }

        private string wrapText(Graphics graphics, string text)
        {
            /* WORD WRAP CODE BORROWED FROM VISUAL NOVEL (also written by me) */
            StringBuilder stringBuffer = new StringBuilder(text.Length);
            string[] lines = text.Split('\n');

            foreach (string line in lines)
            {
                if (graphics.MeasureString(line, font).Width > BufferWidth)
                {
                    string[] words = line.Split(' ');
                    float bufferWidth = 0f;

                    foreach (string word in words)
                    {
                        SizeF size = graphics.MeasureString(word + " ", font, BufferWidth,
                            new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces });
                        if (size.Width + bufferWidth < BufferWidth)
                        {
                            bufferWidth += size.Width;
                            stringBuffer.Append(word + " ");
                        }
                        else
                        {
                            bufferWidth = size.Width;
                            stringBuffer.Append("\n" + word + " ");
                        }
                    }
                    stringBuffer.Append("\n");
                }
                else
                {
                    stringBuffer.Append(line + "\n");
                }
            }

            return stringBuffer.ToString();
        }

        public Color4 TextColor { get { return textColor; } set { textColor = value; } }
        public string Text { get { return text; } set { SetText(value); } }
        public Texture2D Texture { get { return texture; } }
        public bool WordWrap { get; set; }
        public int BufferWidth { get; set; }
    }
}
