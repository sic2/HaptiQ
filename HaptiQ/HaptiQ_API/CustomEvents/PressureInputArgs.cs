using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaptiQ_API
{
    public class PressureInputArgs : EventArgs
    {
        /// <summary>
        /// Id of the HaptiQ generating the event
        /// </summary>
        public uint ID { get; private set; }

        /// <summary>
        /// ID of the actuator generating the event
        /// </summary>
        public int ActuatorId { get; private set; }

        /// <summary>
        /// Pressure value of the pressure event
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Create a wrapper for the pressure input arguments
        /// </summary>
        /// <param name="id"></param>
        /// <param name="actuatorId"></param>
        /// <param name="value"></param>
        public PressureInputArgs(uint id, int actuatorId, int value)
        {
            ID = id;
            ActuatorId = actuatorId;
            Value = value;
        }
    }
}
