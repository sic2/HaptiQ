using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using System.Windows;

namespace Input_API
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ChangedEventHandler(object sender, InputArgs args);

    /// <summary>
    /// This interfaces defines what methods must be implemented
    /// so that HaptiQsManager is notified about any wanted input.
    /// @see SurfaceInput.cs as an example
    /// </summary>
    public abstract class Input
    {
        #region USER_32_FUNCTIONS

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        protected static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        protected struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(String clazz, String windowName);

        #endregion

        protected IntPtr _windowHandle;
        private String _windowName;

        /// <summary>
        /// Initialise an Input object for a given window.
        /// </summary>
        /// <param name="windowName"></param>
        public Input(String windowName)
        {
            _windowName = windowName;
        }
        /// <summary>
        /// Event fired when an input is detected
        /// </summary>
        public event ChangedEventHandler Changed;

        protected virtual void OnChanged(InputIdentifier inputIdentifier, Point position, double orientation)
        {
            if (Changed != null)
            {
                Console.WriteLine("position " + position.X + ", " + position.Y);
                InputArgs args = new InputArgs(inputIdentifier, position, orientation + Math.PI / 2.0);
                Changed(this, args);
            }
        }

        /// <summary>
        /// Updates the window handle.
        /// </summary>
        protected void updateWindowHandle()
        {
            IntPtr hWnd = FindWindow(null, _windowName);
            this._windowHandle = hWnd;
        }

        public void start()
        {
            while (true)
            {
                if (_windowHandle == IntPtr.Zero)
                {
                    updateWindowHandle();
                }
                else
                {
                    initialiseWindowTarget();
                    break;
                }
                System.Threading.Thread.Sleep(100); // Note - decrease for higher FPS
            }
            checkInput();
        }

        /// <summary>
        /// Cyclically check the input and fire an event of type #ChangedEventHandler 
        /// </summary>
        public abstract void checkInput();

        /// <summary>
        /// Dispose the input. This should stop the system from acquiring anymore input.
        /// </summary>
        public abstract void dispose();

        /// <summary>
        /// Initialise window target
        /// </summary>
        protected abstract void initialiseWindowTarget();
    }
}
