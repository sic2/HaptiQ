using System;
using System.Collections.Generic;

using System.Threading;
using AForge.Vision.GlyphRecognition;
using AForge;
using System.Drawing;

namespace Input_API
{
    abstract public class GlyphsInput : Input
    {
        /// <summary>
        /// ImageAvailable must be set to true whenever the image from the device has been retrieved
        /// and can be used by Gratf framework to detect any Glyphs
        /// </summary>
        protected bool imageAvailable;

        private Bitmap imageBitmap;
        private static GlyphDatabase glyphDatabase;
        private GlyphRecognizer recognizer;

        /// <summary>
        ///  Hold this lock when acquiring the image to be processed. 
        /// </summary>
        protected readonly Object imgLock = new Object();

        private bool getInput = true;

        /// <summary>
        /// Initialises the GlyphDatabase and the GlyphRecogniser, used
        /// to detect any Glyphs in a given Bitmap image provided by the device used.
        /// </summary>
        /// <param name="windowHandle"></param>
        public GlyphsInput(String windowName)
            : base(windowName)
        {
            glyphDatabase = new GlyphDatabase(5);
            recognizer = new GlyphRecognizer(glyphDatabase);
            imageAvailable = false;
        }

        private void processGlyphs()
        {
            if (imageAvailable)
            {
                // Stop processing raw images so normalizedImage 
                // is not changed while it is saved to a file.
                DisableRawImage();

                // Copy the normalizedImage byte array into a Bitmap object.
                double widthRatio, heightRatio;
                imageBitmap = getImage(out widthRatio, out heightRatio);

                // Process all found glyphs
                List<ExtractedGlyphData> glyphs = recognizer.FindGlyphs(imageBitmap);
                foreach (ExtractedGlyphData glyphData in glyphs)
                {
                    if (glyphData.RecognizedGlyph != null)
                    {
                        List<IntPoint> glyphPoints = glyphData.RecognizedQuadrilateral;
                        OnChanged(new InputIdentifier(InputIdentifier.TYPE.glyph, 5, InputIdentifier.binaryArrayToInt(glyphData.RawData)), 
                            estimateCenter(glyphPoints, widthRatio, heightRatio), estimateRotation(glyphPoints), EventArgs.Empty);
                    }
                    else
                    {
                        // If glyph was never seen before, then register it.
                        registerGlyph(glyphData);
                    }
                }
               
                // Re-enable collecting raw images.
                EnableRawImage();
            }

        }

        /// <summary>
        /// Glyphs are registed with id of the following format:
        ///     "id-#", where # is a positive integer number
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static String registerGlyph(byte[,] data)
        {
            String id = "id-" + glyphDatabase.Count;
            glyphDatabase.Add(new Glyph(id, data));
            return id;
        }

        private String registerGlyph(ExtractedGlyphData glyphData)
        {
            String id = "id-" + glyphDatabase.Count;
            glyphDatabase.Add(new Glyph(id, glyphData.RawData));
            return id;
        }

        /// <summary>
        /// Check the current input in an infinite loop. 
        /// When a registered glyph is detected, an event is fired.
        /// @see #ChangedEventHandler delegate
        /// </summary>
        public override void checkInput()
        {
            Thread thread = new Thread(handleRawInput);
            thread.Start();

            while (getInput)
            {
                lock (imgLock)
                {
                    processGlyphs();
                }
                System.Threading.Thread.Sleep(50); // Note - decrease for higher FPS
            }

            thread.Join();
        }

        /// <summary>
        /// Stop this Input object from retrieving any more points from the input device.
        /// </summary>
        public override void dispose()
        {
            DisableRawImage();
            getInput = false;
            unhandleRawInput();
        }

        /// <summary>
        /// Estimate the rotation of a glyph, assuming this has four corners.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private double estimateRotation(List<IntPoint> points)
        {
            double retval = 0.0;
            return ((retval = Math.Atan2(points[1].Y - points[0].Y, points[1].X - points[0].X)) > 0 ? 
                    retval : (2 * Math.PI + retval));
        }
        
        private Point estimateCenter(List<IntPoint> points, double widthRatio, double heightRatio)
        {
            double x = (points[0].X + points[1].X + points[2].X + points[3].X) / 4.0;
            double y = (points[0].Y + points[1].Y + points[2].Y + points[3].Y) / 4.0;
            return new Point(x * widthRatio, y * heightRatio);
        }

        /*******************
         *  ABSTRACT
         *  METHODS
         *  
         * These methods must be implemented in order to provide 
         * input data to the Glyph recogniser.
         * Two examples are provided: 
         * - SurfaceGlyphsInput
         * - CameraGlyphsInput
         ******************/

        /// <summary>
        /// Implement this method to allow the Graft to detect any registed glyphs
        /// </summary>
        /// <param name="widthRatio"></param>
        /// <param name="heightRatio"></param>
        /// <returns></returns>
        protected abstract Bitmap getImage(out double widthRatio, out double heightRatio);

        /// <summary>
        /// Implement this method to enable the current device to receive raw images.
        /// </summary>
        protected abstract void EnableRawImage();

        /// <summary>
        /// Implement this method to disable the current device from collecting raw images.
        /// </summary>
        protected abstract void DisableRawImage();

        /// <summary>
        /// Initialiser method to start recording raw images
        /// </summary>
        protected abstract void handleRawInput();

        /// <summary>
        /// Stop recording images
        /// </summary>
        protected abstract void unhandleRawInput();
    }
}
