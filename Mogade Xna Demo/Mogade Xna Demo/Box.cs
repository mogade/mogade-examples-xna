using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Mogade_Xna_Demo
{
    public class Box
    {
        public Rectangle rectangle;
        public Color colour;
        public float changetimer;
        public float clicktimer;
        public bool isgreen;

        public Box(int xpos, int ypos, int width, int height, Color incolour, float inchangetimer)
        {
            rectangle = new Rectangle(
                xpos, ypos,
                width,
                height);

            colour = incolour;
            changetimer = inchangetimer;
            clicktimer = 2.0f;
            isgreen = false;
        }

        public void changecolour()
        {
            isgreen = true;
            colour = Color.Green;
        }

        public void update(float elapsedTime)
        {
            changetimer -= elapsedTime;
            if (changetimer <= 0 && !isgreen)
            {
                changecolour();
            }

            if (isgreen)
            {
                clicktimer -= elapsedTime;
            }
        }
    }
}
