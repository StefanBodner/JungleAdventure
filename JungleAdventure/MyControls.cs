using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.UI.Forms;

namespace JungleAdventure
{
    internal class MyControls : ControlManager
    {
        public MyControls(Game game) : base(game)
        {
        }
        public override void InitializeComponent()
        {
            var btn1 = new Button()
            {
                Text = "Hallo Steff",
                Size = new Vector2(200, 50),
                BackgroundColor = Color.Green,
                Location = new Vector2(100, 100),
            };
            btn1.Clicked += Btn1_Clicked;
            Controls.Add(btn1);
        }
        private void Btn1_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.Text = "Clicked!";
        }
    }
}