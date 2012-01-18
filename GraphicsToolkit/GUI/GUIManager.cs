﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsToolkit.GUI
{
    public class GUIManager : DrawableGameComponent
    {
        public Color BackgroundColor = Color.Red;
        private List<Panel> panels;
        private List<Texture2D> panelTextures;
        private SpriteBatch spriteBatch;
        private bool hasLaoded = false;

        public GUIManager(Game g, GraphicsDeviceManager gManager)
            : base(g)
        {
            panels = new List<Panel>();
            panelTextures = new List<Texture2D>();
            g.Window.AllowUserResizing = true;
            gManager.PreferredBackBufferWidth = Config.ScreenWidth;
            gManager.PreferredBackBufferHeight = Config.ScreenHeight;
            gManager.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(gManager_PreparingDeviceSettings);
            gManager.ApplyChanges();
        }

        void gManager_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            Config.ScreenWidth = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth;
            Config.ScreenHeight = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight;
            ResizePanels();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Panel.Device = Game.GraphicsDevice;

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            foreach (Panel p in panels)
            {
                p.LoadContent(Game.Content);
            }

            hasLaoded = true;
        }

        public void AddPanel(Panel p)
        {
            panels.Add(p);
            panelTextures.Add(null);
            if (hasLaoded)
            {
                p.LoadContent(Game.Content);
            }
        }

        public void RemovePanel(Panel p)
        {
            panels.Remove(p);
        }

        public void ResizePanels()
        {
            foreach (Panel p in panels)
            {
                p.OnUserResize();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (Panel p in panels)
            {
                p.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            //Render the panels to textures
            foreach (Panel p in panels)
            {
                p.BeginDraw();
                p.Draw(gameTime);
                p.EndDraw();
            }

            //Clear the screen
            Game.GraphicsDevice.Clear(BackgroundColor);

            //Draw our panel textures
            spriteBatch.Begin();
            foreach (Panel p in panels)
            {
                spriteBatch.Draw(p.PanelTexture, p.ScreenRect, Color.White);
            }
            spriteBatch.End();
        }
    }
}