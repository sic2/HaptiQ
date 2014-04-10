using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using Microsoft.Surface.Core;
using System.Drawing;

namespace Input_API
{
    public class SurfaceGlyphsInput : GlyphsInput
    {
        private Microsoft.Surface.Core.TouchTarget touchTarget;
        private Microsoft.Surface.Core.ImageMetrics normalizedMetrics;
        private byte[] normalizedImage;

        public SurfaceGlyphsInput(String windowName)
            : base(windowName) {}

        // @see: http://msdn.microsoft.com/en-us/library/ff727886.aspx 
        private void OnTouchTargetFrameReceived(object sender, Microsoft.Surface.Core.FrameReceivedEventArgs e)
        {
            lock (imgLock)
            {
                imageAvailable = false;
                if (normalizedImage == null)
                {
                    e.TryGetRawImage(Microsoft.Surface.Core.ImageType.Normalized,
                        Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Left,
                        Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Top,
                        Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Width,
                        Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Height,
                        out normalizedImage,
                        out normalizedMetrics);
                }
                else
                {
                    e.UpdateRawImage(Microsoft.Surface.Core.ImageType.Normalized,
                        normalizedImage,
                        Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Left,
                        Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Top,
                        Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Width,
                        Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Height);
                }

                if (normalizedImage != null)
                    imageAvailable = true;
            }
        }

        protected override void EnableRawImage()
        {
            if (touchTarget != null)
            {
                touchTarget.EnableImage(Microsoft.Surface.Core.ImageType.Normalized);
                touchTarget.FrameReceived += OnTouchTargetFrameReceived;
            }
        }

        protected override void DisableRawImage()
        {
            if (touchTarget != null)
            {
                touchTarget.DisableImage(Microsoft.Surface.Core.ImageType.Normalized);
                touchTarget.FrameReceived -= OnTouchTargetFrameReceived;
            }
        }

        protected override void handleRawInput()
        {
            EnableRawImage();
            // Attach an event handler for the FrameReceived event.
            if (touchTarget != null)
            {
                touchTarget.FrameReceived += new EventHandler<FrameReceivedEventArgs>(OnTouchTargetFrameReceived);
            }
        }

        protected override void unhandleRawInput()
        {
            if (touchTarget != null)
            {
                touchTarget.FrameReceived -= new EventHandler<FrameReceivedEventArgs>(OnTouchTargetFrameReceived);
                touchTarget.Dispose();
            }
        }

        protected override void initialiseWindowTarget()
        {
            // Create a target for surface input.
            touchTarget = new TouchTarget(_windowHandle, EventThreadChoice.OnBackgroundThread);
            touchTarget.EnableInput();
        }

        //Note: need to calculate ratio everytime, because a window might change size during execution
        protected override Bitmap getImage(out double widthRatio, out double heightRatio)
        {
            GCHandle h = GCHandle.Alloc(normalizedImage, GCHandleType.Pinned);
            IntPtr ptr = h.AddrOfPinnedObject();
            widthRatio = Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Width / (normalizedMetrics.Width * 1.0);
            heightRatio = Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Height / (normalizedMetrics.Height * 1.0);
            return new Bitmap(normalizedMetrics.Width,
                                      normalizedMetrics.Height,
                                      normalizedMetrics.Stride,
                                      System.Drawing.Imaging.PixelFormat.Format8bppIndexed,
                                      ptr);
        }
    }
}