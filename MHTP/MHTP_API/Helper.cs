using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Input_API;
using System.Xml.Serialization;

namespace MHTP_API
{
    /// <summary>
    /// This class defines a set of methods which are used often by the MHTP_API 
    /// and can also be useful to any client application using the MHTP_API
    /// </summary>
    public class Helper
    {
        // This lock must be held when writing to log file to avoid
        // concurrent processes to log at the same time.
        private static readonly Object _logLock = new Object();

        private const double RADS_TO_DEGREES_MULTIPLIER = 57.2957795;
        private const double DEGREES_TO_RADS_MULTIPLIER = 0.01745329251;

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double radsToDegrees(double radians)
        {
            return radians * RADS_TO_DEGREES_MULTIPLIER;
        }

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double degreesToRads(double degrees)
        {
            return degrees * DEGREES_TO_RADS_MULTIPLIER;
        }

        /// <summary>
        /// Find the nearest points between two lists of points.
        /// This function is particularly useful is we want to find the nearest points between
        /// two geometric figures.
        /// </summary>
        /// <param name="srcPoints"></param>
        /// <param name="dstPoints"></param>
        /// <returns></returns>
        public static Tuple<Point, Point> findNearestPoints(List<Point> srcPoints, List<Point> dstPoints)
        {
            if (srcPoints == null || dstPoints == null)
                return null;

            if (srcPoints.Count() == 0 || dstPoints.Count() == 0)
                return null;

            // Greedy search for closest points between the two list of points.
            Point A = new Point(int.MinValue, int.MinValue);
            Point B = new Point(int.MaxValue, int.MaxValue);
            double minDistance = int.MaxValue;
            foreach(Point src in srcPoints)
            {
                foreach (Point dst in dstPoints)
                {
                    double tmp = Helper.distanceBetweenTwoPoints(src, dst);
                    if (tmp < minDistance)
                    {
                        minDistance = tmp;
                        A = src;
                        B = dst;
                    }
                }
            }
            return new Tuple<Point,Point>(A, B);
        }

        /// <summary>
        /// Calculates the distance between two given points.
        /// Returns zero is arguments are null
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static double distanceBetweenTwoPoints(Point src, Point dst)
        {
            if (src == null || dst == null)
                return 0;

            return Math.Sqrt(Math.Pow(src.X - dst.X, 2) + Math.Pow(src.Y - dst.Y, 2));
        }
        
        /// <summary>
        /// Use this function to write a message to log file.
        /// The log file is stored in the current directory of the client application.
        /// The name of the file has the following format:
        ///     "Log-MHTP_API_yyyyMMdd.txt"
        /// where y, M, d are time tags
        /// 
        /// The messages are logged by appending a prefix of the following format:
        ///     "yyyy:MM:dd HH:mm:ss"
        ///     
        /// TODO - introduce levels of logging:
        /// @see http://stackoverflow.com/questions/7839565/logging-levels-logback-rule-of-thumb-to-assign-log-levels
        /// </summary>
        /// <param name="str"></param>
        public static void Logger(String str)
        {
            lock (_logLock)
            {
                String timeStr = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");
                String fileName = "Log-MHTP_API_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                System.IO.StreamWriter file = null;
                try
                {
                    file = new System.IO.StreamWriter(fileName, true);
                    file.WriteLine(timeStr + " :: " + str);
                }
                catch(IOException e)
                {
                    // Write a message to command line and then throw an IOException
                    Console.WriteLine("MHTP_API.Helper.Logger:: Impossible to write Log message " + str + " to file. " + e.Message);
                    throw new IOException("MHTP_API.Helper.Logger:: Impossible to write Log message " + str + " to file. " + e.Message);
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
        }

        /// <summary>
        /// Serialize given configuration to specified file (XML)
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="fileName"></param>
        public static void SerializeToXML(Configuration configuration, String fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
            TextWriter textWriter = new StreamWriter(@fileName);
            serializer.Serialize(textWriter, configuration);
            textWriter.Close();
        }

        /// <summary>
        /// Deserialize configuration stored in specified file.
        /// Return stored configuration.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Configuration DeserializeFromXML(String fileName)
        {
            if (File.Exists(fileName))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Configuration));
                TextReader textReader = new StreamReader(@fileName);
                Configuration configuration = (Configuration)deserializer.Deserialize(textReader);
                configuration.inputIdentifier = configuration.serializableInputIdentifier.getInputIdentifier();
                textReader.Close();
                return configuration;
            }
            else
            {
                return null;
            }
        }
    }
}
