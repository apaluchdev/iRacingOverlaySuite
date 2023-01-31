﻿using GameOverlay.Drawing;
using GameOverlay.Windows;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace iRacingOverlaySuite.Overlays
{
    public class Overlay : IDisposable
    {
        private readonly GraphicsWindow _window;
        private List<Action<Graphics>> DrawActions = new List<Action<Graphics>>();

        private const string IRACING_WINDOW_NAME = "iRacing.com Simulator";
        private bool _attachedToWindow = false;

        protected readonly Dictionary<string, SolidBrush> _brushes;
        protected readonly Dictionary<string, Font> _fonts;
        protected readonly Dictionary<string, Image> _images;

        protected IRData IRData = new IRData();

        public event EventHandler SetupCompleted;

        public int WindowXPos { get; private set; }
        public int WindowYPos { get; private set; }

        private int _offsetX;
        private int _offsetY;

        public Overlay(int x, int y, int width, int height)
        {
            _brushes = new Dictionary<string, SolidBrush>();
            _fonts = new Dictionary<string, Font>();
            _images = new Dictionary<string, Image>();

            _offsetX = x;
            _offsetY = y;

            var gfx = new Graphics()
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true
            };

            _window = new GraphicsWindow(x, y, width, height, gfx)
            {
                FPS = 50,
                IsTopmost = true,
                IsVisible = true,
            };

            WindowXPos = _window.X;
            WindowYPos = _window.Y;

            _window.DestroyGraphics += _window_DestroyGraphics;
            _window.DrawGraphics += _window_DrawGraphics;
            _window.SetupGraphics += _window_SetupGraphics;
        }

        #region SetupGraphics

        private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            AttachToWindow();

            var gfx = e.Graphics;          

            if (e.RecreateResources)
            {
                foreach (var pair in _brushes) pair.Value.Dispose();
                foreach (var pair in _images) pair.Value.Dispose();
            }

            _brushes["black"] = gfx.CreateSolidBrush(0, 0, 0, 0.25f);
            _brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
            _brushes["transparentWhite"] = gfx.CreateSolidBrush(255, 255, 255,0.75f);
            _brushes["red"] = gfx.CreateSolidBrush(255, 0, 0, 0.5f);
            _brushes["yellow"] = gfx.CreateSolidBrush(255, 255, 0, 0.5f);
            _brushes["green"] = gfx.CreateSolidBrush(0, 255, 0, 0.25f);
            _brushes["blue"] = gfx.CreateSolidBrush(0, 0, 255);
            _brushes["background"] = gfx.CreateSolidBrush(0x33, 0x36, 0x3F, 0.00f);
            _brushes["backgroundGray"] = gfx.CreateSolidBrush(0x33, 0x36, 0x3F, 0.8f);
            _brushes["grid"] = gfx.CreateSolidBrush(255, 255, 255, 0.2f);
            _brushes["transparent"] = gfx.CreateSolidBrush(0, 0, 0, 0.25f);
            _brushes["random"] = gfx.CreateSolidBrush(0, 0, 0);

            if (e.RecreateResources) return;

            _fonts["arial"] = gfx.CreateFont("Arial", 12);
            _fonts["consolas"] = gfx.CreateFont("Consolas", 14);

            SetupCompleted?.Invoke(this, e);
        }

        #endregion

        protected void AddDrawAction(Action<Graphics> drawAction)
        {
            DrawActions.Add(drawAction);
        }

        private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            // If we are not attached to the window, attempt to do so every 5 seconds
            if (!_attachedToWindow)
            {
                if (DateTime.Now.Second % 5 == 0)
                    AttachToWindow();
            }

            IRData.ProcessData();

            RefreshData();
            
            // Draw list of actions generated by child class
            Draw(gfx);
        }

        private void Draw(Graphics gfx)
        {
            foreach (var action in DrawActions)
            {
                action(gfx);
            }   
        }

        public void Run()
        {
            _window.Create();
            _window.Join();
        }

        public virtual void RefreshData()
        {

        }

        public void AttachToWindow(string windowName = IRACING_WINDOW_NAME)
        {
            IntPtr windowHandle = Win32Helper.FindWindow(null, windowName);

            if (windowHandle != IntPtr.Zero)
            {
                // TODO - Allow for moving overlays via a click-drag
                POINT lpPoint;
                Win32Helper.GetCursorPos(out lpPoint);

                RECT rect;
                Win32Helper.GetWindowRect(windowHandle, out rect);
                
                var left = ((Math.Abs(rect.right + rect.left) / 2) - (_window.Width / 2)) + _window.X;
                var top = (((rect.top + rect.bottom) / 2) - _window.Height) + _window.Y;

                _window.Move(left, top);
                _attachedToWindow = true;
            }

            WindowXPos = _window.X;
            WindowYPos = _window.Y; 
        }

        ~Overlay()
        {
            Dispose(false);
        }

        #region IDisposable

        private bool disposedValue;

        private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            foreach (var pair in _brushes) pair.Value.Dispose();
            foreach (var pair in _fonts) pair.Value.Dispose();
            foreach (var pair in _images) pair.Value.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _window.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
