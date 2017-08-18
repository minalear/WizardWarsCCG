using System;
using OpenTK;
using OpenTK.Graphics;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class HandCounter : TextBox
    {
        private Texture2D symbol;
        private string symbolPath;

        private Vector2 textureOffset = new Vector2(-8f, 0f);

        public HandCounter(Control parent, string symbolPath, Vector2 position)
            : base(parent, "7", 12f)
        {
            this.Position = position;
            this.symbolPath = symbolPath;
            this.textColor = Color4.White;
        }

        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            renderer.AddRenderTask(symbol, Position + textureOffset, Color4.White, -0.5f);
            base.Draw(gameTime, renderer);
        }

        public override void LoadContent()
        {
            symbol = Texture2D.LoadFromSource(symbolPath);
            base.LoadContent();
        }
        public override void UnloadContent()
        {
            symbol.Dispose();
            base.UnloadContent();
        }
    }
}
