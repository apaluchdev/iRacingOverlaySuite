using GameOverlay.Drawing;
using GameOverlay.Windows;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Windows.Interop;
using System.Windows;

namespace iRacingOverlaySuite
{
    public enum Location
    {
        TopLeft,
        TopMiddle,
        TopRight,
        BottomLeft,
        BottomRight,
        Center
    }

    public class Overlay : IDisposable
    {
        private const string IRACING_WINDOW_NAME = "iRacing.com Simulator";

        protected readonly GraphicsWindow _window;

        protected bool _attachedToWindow = false;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int X1 { get; private set; }
        public int Y1 { get; private set; }
        public int X2 { get; private set; }
        public int Y2 { get; private set; }

        private RECT _gameWindow;

        private Location _location;

        public Overlay(OverlayParams overlayParams)
        {
            Width = overlayParams.Width;
            Height = overlayParams.Height;

            _location = overlayParams.Location;

            var gfx = new Graphics()
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true
            };

            _window = new GraphicsWindow(overlayParams.X, overlayParams.Y, overlayParams.Width, overlayParams.Height, gfx)
            {
                FPS = 50,
                IsTopmost = true,
                IsVisible = true,
            };

            UpdateCoordinates();
        }

        public void AttachToWindow(string windowName = IRACING_WINDOW_NAME)
        {
            IntPtr windowHandle = Win32Helper.FindWindow(null, windowName);

            if (windowHandle != IntPtr.Zero)
            {
                POINT lpPoint;
                Win32Helper.GetCursorPos(out lpPoint);

                Win32Helper.GetWindowRect(windowHandle, out _gameWindow);

                _gameWindow.width = _gameWindow.right - _gameWindow.left;
                _gameWindow.height = _gameWindow.bottom - _gameWindow.top;

                if (_location == Location.Center)
                    _window.Move
                    (
                        _gameWindow.left + (_gameWindow.width / 2) - Width / 2,
                        _gameWindow.top + (_gameWindow.height / 2) - Height / 2
                    );
                else if (_location == Location.TopLeft)
                    _window.Move(
                        _gameWindow.left,
                        _gameWindow.top);
                else if (_location == Location.TopMiddle)
                    _window.Move(
                        _gameWindow.left + (_gameWindow.width / 2) - Width / 2,
                        _gameWindow.top);
                else
                    _window.Move(_gameWindow.left, _gameWindow.top);

                UpdateCoordinates();

                _attachedToWindow = true;
            }
        }

        private void UpdateCoordinates()
        {
            X1 = _window.X;
            Y1 = _window.Y;

            X2 = _window.X + _window.Width;
            Y2 = _window.Y + _window.Height;

            Width = X2 - X1;
            Height = Y2 - Y1;
        }

        public void Run()
        {
            _window.Create();
            _window.Join();
        }

        #region IDisposable

        ~Overlay()
        {
            Dispose(false);
        }

        private bool disposedValue;

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
