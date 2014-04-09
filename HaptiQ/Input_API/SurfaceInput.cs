using System;
using System.Collections.Generic;

using Microsoft.Surface.Core;

namespace Input_API
{
    /// <summary>
    /// This class defines the methods to get input through ByteTags as 
    /// defined in the SurfaceSDK
    /// </summary>
    public class SurfaceInput : Input
    {
        private TouchTarget touchTarget;

        public bool getInput = true;

        public SurfaceInput(String windowName) : base(windowName)
        {
            touchTarget = null;
        }

        public override void checkInput()
        {
            while (getInput)
            {
                ReadOnlyTouchPointCollection touches = touchTarget.GetState();
                manageTouches(touches);
                System.Threading.Thread.Sleep(50); // Add delay to avoid a busy round-robin
            }
        }

        private void manageTouches(ReadOnlyTouchPointCollection touches)
        {
            foreach (TouchPoint touch in touches)
            {
                if (touch.IsTagRecognized)
                {
                    InputIdentifier inputIdentifier = new InputIdentifier(InputIdentifier.TYPE.tag, -1, (ulong)touch.Tag.Value);
                    OnChanged(inputIdentifier, new Point(touch.CenterX, touch.CenterY), touch.Orientation);  
                }
            }
        }

        public override void dispose()
        {
            getInput = false;
        }

        protected override void initialiseWindowTarget()
        {
            // Create a target for surface input.
            touchTarget = new TouchTarget(_windowHandle, EventThreadChoice.OnBackgroundThread);
            touchTarget.EnableInput();
        }
    }
}
