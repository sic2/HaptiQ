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
    /// <param name="point"></param>
    /// <param name="orientation">In radians</param>
    /// <param name="e"></param>
    public delegate void ChangedEventHandler(object sender, InputIdentifier inputIdentifier, Point point, double orientation, EventArgs e);

    /// <summary>
    /// This interfaces defines what methods must be implemented
    /// so that MHTPsManager is notified about any wanted input.
    /// @see SurfaceInput.cs as an example
    /// </summary>
    public abstract class Input
    {
        #region USER_32_FUNCTIONS

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
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

        /// <summary>
        /// Initialise an Input object for a given window.
        /// </summary>
        /// <param name="windowName"></param>
        public Input(String windowName)
        {
            IntPtr hWnd = FindWindow(null, windowName);
            this._windowHandle = hWnd;
        }
        /// <summary>
        /// Event fired when an input is detected
        /// </summary>
        public event ChangedEventHandler Changed;

        protected virtual void OnChanged(InputIdentifier inputIdentifier, Point point, double orientation, EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, inputIdentifier, point, orientation, e);
            }
        }

        /// <summary>
        /// Cyclically check the input and fire an event of type #ChangedEventHandler 
        /// </summary>
        public abstract void checkInput();

        /// <summary>
        /// Dispose the input. This should stop the system from acquiring anymore input.
        /// </summary>
        public abstract void dispose();
    }
}
