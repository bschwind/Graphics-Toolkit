﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GraphicsToolkit.GUI
{
    public class Panel
    {
        public static GraphicsDevice Device;
        protected Vector2 upperLeft, bottomRight;
        protected int x, y, width, height;
        private Viewport viewPort;

        public Viewport ViewPort
        {
            get
            {
                return viewPort;
            }

        }

        public Rectangle ScreenRect
        {
            get
            {
                return new Rectangle(x, y, width, height);
            }
        }

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public Panel(Vector2 upLeft, Vector2 botRight)
        {
            upperLeft = upLeft;
            bottomRight = botRight;

            x = (int)(upperLeft.X * Config.ScreenWidth);
            y = (int)(upperLeft.Y * Config.ScreenHeight);
            width = (int)((botRight.X - upLeft.X) * Config.ScreenWidth);
            height = (int)((botRight.Y - upLeft.Y) * Config.ScreenHeight);
        }

        public virtual void LoadContent(ContentManager content)
        {
            Resize(upperLeft, bottomRight);
        }

        public void Resize(Vector2 upLeft, Vector2 botRight)
        {
            upperLeft = upLeft;
            bottomRight = botRight;

            x = (int)(upperLeft.X * Config.ScreenWidth);
            y = (int)(upperLeft.Y * Config.ScreenHeight);
            width = (int)((botRight.X - upLeft.X) * Config.ScreenWidth);
            height = (int)((botRight.Y - upLeft.Y) * Config.ScreenHeight);

            viewPort = new Viewport(x, y, width, height);

            OnRefresh();
        }

        //Called when the user resizes the parent window
        public void OnUserResize()
        {
            Resize(upperLeft, bottomRight);
            OnRefresh();
        }

        protected virtual void OnRefresh()
        {

        }

        public virtual void Update(GameTime g)
        {

        }

        public virtual void Draw(GameTime g)
        {

        }
    }
}