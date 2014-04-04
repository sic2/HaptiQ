using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaptiQ_API
{
    /// <summary>
    /// IAction interface.
    /// Implement this to define custom actions to be executed on pressure gesture events
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Executes the action
        /// </summary>
        /// <param name="id"></param>
        /// <param name="gestureType"></param>
        /// <param name="pressureData"></param>
        void run(uint id, PRESSURE_GESTURE_TYPE gestureType, Dictionary<int, double> pressureData);
    }
}
