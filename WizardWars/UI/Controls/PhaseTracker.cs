using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class PhaseTracker : Control
    {
        private GameState gameState;
        private Texture2D backgroundTexture;

        private int mainCounter = 1;

        public PhaseTracker(Control parent, Vector2 position, GameState gameState)
            : base(parent)
        {
            this.gameState = gameState;
            this.Position = position;

            this.gameState.PhaseChange += GameState_PhaseChange;
        }

        public override void LoadContent()
        {
            backgroundTexture = Texture2D.LoadFromSource(@"Content/Art/Assets/phase_tracker.png");
            Size = backgroundTexture.Size;

            float x = 5f;
            float y = 3f;

            foreach (Phases phase in gameState.PhaseSequence)
            {
                TextBox textBox = new TextBox(this, phase.ToString(), 11f);
                textBox.Position = new Vector2(x, y);

                x += 107f;
            }

            base.LoadContent();
        }
        public override void UnloadContent()
        {
            backgroundTexture.Dispose();

            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            renderer.AddRenderTask(backgroundTexture, Position, Size, Color4.White);

            //Overwrite base.Draw()
            bool ignoreFirstMain = (mainCounter == 1);
            foreach (TextBox textBox in Children)
            {
                if (textBox.Text != gameState.CurrentPhase.ToString())
                {
                    textBox.TextColor = Color4.Black;
                    textBox.Draw(gameTime, renderer);
                }
                else
                {
                    if (gameState.CurrentPhase.ToString() == Phases.Main.ToString())
                    {
                        //Convoluted way to keep track of both main phases
                        if (ignoreFirstMain)
                        {
                            ignoreFirstMain = false;
                            textBox.TextColor = Color4.Black;
                            textBox.Draw(gameTime, renderer);

                            continue;
                        }
                        else
                        {
                            ignoreFirstMain = true;
                            textBox.TextColor = Color4.White;
                            textBox.Draw(gameTime, renderer);
                        }
                    }

                    textBox.TextColor = Color4.White;
                    textBox.Draw(gameTime, renderer);
                }
            }
        }

        private void GameState_PhaseChange(object sender, Phases phase)
        {
            if (phase == Phases.Main)
            {
                mainCounter = (mainCounter == 1) ? 0 : 1;
            }
        }
    }
}
