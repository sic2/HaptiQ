using System;
using System.Linq;
using System.Threading;

using System.Reflection;

namespace Input_API
{
    public class InputFactory
    {
        /// <summary>
        /// Returns an Input object for the specified window. 
        /// Return null if inputClass is null, zero length or there is not a class of this type.
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="inputClass">
        /// Specify what type of Input object to use (i.e. "SurfaceGlyphInput")</param>
        /// <returns></returns>
        public static Input getInputObj(String windowName, string inputClass)
        {
            if (inputClass == null || inputClass.Length == 0)
                return null;

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var currentType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == inputClass);
            if (currentType == null)
                return null;

            Input input = (Input)Activator.CreateInstance(currentType, new object[] { windowName });

            Thread oThread = new Thread(new ThreadStart(input.checkInput));
            oThread.Start();

            return input;
        }
    }
}