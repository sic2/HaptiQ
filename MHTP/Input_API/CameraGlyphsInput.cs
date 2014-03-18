using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using AForge.Video;
using AForge.Video.DirectShow;

namespace Input_API
{
    public class CameraGlyphsInput : GlyphsInput
    {
        private VideoCaptureDevice cam = null;
        private FilterInfoCollection usbCams;

        private Bitmap currentFrame;

        public CameraGlyphsInput(String windowName)
            : base(windowName)
        {
            usbCams = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo f in usbCams)
            {
                // FIXME - get default camera, unless specified
                cam = new VideoCaptureDevice(f.MonikerString);
            }

            currentFrame = null;
        }

        /// <summary>
        /// Implement this method to allow the Graft to detect any registed glyphs
        /// </summary>
        /// <param name="widthRatio"></param>
        /// <param name="heightRatio"></param>
        /// <returns></returns>
        protected override Bitmap getImage(out double widthRatio, out double heightRatio)
        {
            // TODO - need to find a way to calculate the width and height ratios
            // compared to the application
            widthRatio = 0;
            heightRatio = 0;
            return currentFrame;
        }

        /// <summary>
        /// Implement this method to enable the current device to receive raw images.
        /// </summary>
        protected override void EnableRawImage()
        {
            cam.NewFrame += new NewFrameEventHandler(camNewFrame);
            cam.Start();
        }

        /// <summary>
        /// Implement this method to disable the current device from collecting raw images.
        /// </summary>
        protected override void DisableRawImage()
        {
            cam.NewFrame -= new NewFrameEventHandler(camNewFrame);
        }

        private void camNewFrame(object sender, NewFrameEventArgs args)
        {
            lock (imgLock)
            {
                currentFrame = (Bitmap)args.Frame.Clone();
                imageAvailable = true;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        protected override void handleRawInput()
        {
            EnableRawImage();
        }

        protected override void unhandleRawInput()
        {
            cam.Stop();
        }

    }
}
