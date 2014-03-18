using System;
using System.Collections.Generic;

using Microsoft.Surface.Core;

namespace Input_API
{
    public class SurfaceInput : Input
    {
        private TouchTarget touchTarget;

        public bool getInput = true;

        public SurfaceInput(String windowName) : base(windowName)
        {
            // Create a target for surface input.
            touchTarget = new TouchTarget(_windowHandle, EventThreadChoice.OnBackgroundThread);
            touchTarget.EnableInput();
        }

        public override void checkInput()
        {
            while (getInput)
            {
                ReadOnlyTouchPointCollection touches = touchTarget.GetState();
                manageTouches(touches);
                System.Threading.Thread.Sleep(100); // Add delay to avoid a busy round-robin
            }
        }

        private void manageTouches(ReadOnlyTouchPointCollection touches)
        {
            foreach (TouchPoint touch in touches)
            {
                if (touch.IsTagRecognized)
                {

                    InputIdentifier inputIdentifier = new InputIdentifier(InputIdentifier.TYPE.tag, -1, (ulong)touch.Tag.Value);
                    OnChanged(inputIdentifier, new Point(touch.CenterX, touch.CenterY), touch.Orientation, EventArgs.Empty);  
                }
            }
        }

        public override void dispose()
        {
            // XXX - lock? 
            getInput = false;
        }
    }
}
