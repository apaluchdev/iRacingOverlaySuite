﻿using GameOverlay.Drawing;
using GameOverlay.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingOverlaySuite
{
    internal class OverlayCanvas : Overlay
    {
        const float TRANSPARENCY = 0.8f;

        public bool DrawGrid { get; set; }

        public int CenterX => (this.X2 - this.X1) / 2;
        public int CenterY => (this.Y2 - this.Y1) / 2;

        #region Resources

        public readonly Dictionary<string, SolidBrush> Brushes;
        public readonly Dictionary<string, Font> Fonts;
        public readonly Dictionary<string, Image> Images;

        #endregion

        private List<Action<Graphics>> _drawActions = new List<Action<Graphics>>();

        public event EventHandler? SetupCompleted;

        public IOverlayDrawer Drawer;

        public OverlayCanvas(OverlayParams overlayParams, IOverlayDrawer drawer) : base (overlayParams)
        {
            Drawer = drawer;

            Brushes = new Dictionary<string, SolidBrush>();
            Fonts = new Dictionary<string, Font>();
            Images = new Dictionary<string, Image>();

            _window.DestroyGraphics += _window_DestroyGraphics;
            _window.DrawGraphics += _window_DrawGraphics;
            _window.SetupGraphics += _window_SetupGraphics;
        }

        public void AddDrawActions(List<Action<Graphics>> drawActions)
        {
            foreach (var drawAction in drawActions)
            {
                _drawActions.Add(drawAction);
            }
        }

        public void AddDrawAction(Action<Graphics> drawAction)
        {
            _drawActions.Add(drawAction); 
        }

        #region SetupGraphics

        private void SetupBrushes(Graphics gfx)
        {
            Brushes["black"] = gfx.CreateSolidBrush(0, 0, 0, TRANSPARENCY);
            Brushes["white"] = gfx.CreateSolidBrush(255, 255, 255, TRANSPARENCY);
            Brushes["grid"] = gfx.CreateSolidBrush(255, 255, 255, 0.1f);


            Brushes["red"] = gfx.CreateSolidBrush(255, 0, 0, TRANSPARENCY);
            Brushes["green"] = gfx.CreateSolidBrush(51, 204, 51, TRANSPARENCY);
            Brushes["blue"] = gfx.CreateSolidBrush(0, 51, 204, TRANSPARENCY);

            Brushes["orange"] = gfx.CreateSolidBrush(255, 153, 0, TRANSPARENCY);
            Brushes["yellow"] = gfx.CreateSolidBrush(255, 255, 0, TRANSPARENCY);
            Brushes["purple"] = gfx.CreateSolidBrush(102, 0, 255, TRANSPARENCY);
            Brushes["aqua"] = gfx.CreateSolidBrush(0, 255, 255, TRANSPARENCY); 

            Brushes["clear"] = gfx.CreateSolidBrush(0, 0, 0, 0.00f);
            Brushes["gray"] = gfx.CreateSolidBrush(128, 128, 128, TRANSPARENCY);
            Brushes["transparentGray"] = gfx.CreateSolidBrush(128, 128, 128, 0.25f);
            Brushes["darkGray"] = gfx.CreateSolidBrush(64, 64, 64, TRANSPARENCY);
        }

        private void _window_SetupGraphics(object? sender, SetupGraphicsEventArgs? e)
        {
            AttachToWindow();

            var gfx = e?.Graphics;

            if (gfx == null) return;

            if (e?.RecreateResources ?? false)
            {
                foreach (var pair in Brushes) pair.Value.Dispose();
                foreach (var pair in Images) pair.Value.Dispose();
                foreach (var pair in Fonts) pair.Value.Dispose();
            }

            SetupBrushes(gfx);

            Fonts["arial"] = gfx.CreateFont("Arial", 12);
            Fonts["consolas"] = gfx.CreateFont("Consolas", 14);
            Fonts["calibri"] = gfx.CreateFont("Calibri", 14);
            Fonts["courier"] = gfx.CreateFont("Courier New", 14);


            if (e?.RecreateResources ?? false) return;

            SetupCompleted?.Invoke(this, e);

            Drawer.SetupOverlay();
        }

        #endregion

        #region DrawGraphics

        private void _window_DrawGraphics(object? sender, DrawGraphicsEventArgs? e)
        {
            var gfx = e.Graphics;

            // If we are not attached to the window, attempt to do so every 5 seconds
            if (!_attachedToWindow)
            {
                if (DateTime.Now.Second % 5 == 0)
                    AttachToWindow();
            }

            // Fetch latest iRacing data
            IRData.UpdateData();

            // Draw list of actions generated by child class
            Paint(gfx);
        }

        private void Paint(Graphics gfx)
        {
            gfx.ClearScene(Brushes["clear"]);

#if !DEBUG
            if (!_attachedToWindow) return;
#endif

            foreach (Action<Graphics> action in _drawActions)
            {
                action(gfx);
            }

            if (DrawGrid)
            {
                for (int i = 0; i < Width; i += 10)
                {
                    gfx.DrawLine(Brushes["grid"], i, 0, i, Height, 1f);
                }

                for (int i = 0; i < Height; i += 10)
                {
                    gfx.DrawLine(Brushes["grid"], 0, i, Width, i, 1f);
                }
            }
        }

        #endregion

        #region DestroyGraphics

        private void _window_DestroyGraphics(object? sender, DestroyGraphicsEventArgs? e)
        {
            foreach (var pair in Brushes) pair.Value.Dispose();
            foreach (var pair in Fonts) pair.Value.Dispose();
            foreach (var pair in Images) pair.Value.Dispose();
        }

        #endregion
    }
}
