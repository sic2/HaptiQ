using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Phidgets;

using Input_API;

namespace MHTP_API
{
    /// <summary>
    /// This class describes a Configuration for an MHTP
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// String used to give a name to an MHTP
        /// </summary>
        public String mhtpName;

        /// <summary>
        /// ID of servo board
        /// </summary>
        public int idServoBoard;

        /// <summary>
        /// Name of servo board
        /// </summary>
        public String nameServoBoard;

        /// <summary>
        /// Number of actuators
        /// </summary>
        public int numberActuators;

        /// <summary>
        /// Id of Interface Kit board
        /// </summary>
        public int idInterfaceKit;

        /// <summary>
        /// Name of interface kit board
        /// </summary>
        public String nameInterfaceKit;

        /// <summary>
        /// Number of pressure sensors
        /// </summary>
        public int numberPressureSensors;

        /// <summary>
        /// Dictionary of actuatorIndex -> (min, max) position of actuator
        /// </summary>
        public SerializableDictionary<int, SerializableTuple<int, int>> actuators;

        /// <summary>
        /// List of pressure sensors
        /// </summary>
        public List<int> pressureSensors;
        
        /// <summary>
        /// True if interface kit board is attached
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public bool interfaceKitBoardAttached;

        private InputIdentifier _inputIdentifier;
        /// <summary>
        /// Input identifier for this configuration
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public InputIdentifier inputIdentifier
        {
            get
            {
                return _inputIdentifier;
            }
            set
            {
                _inputIdentifier = value;
            }
        }

        /// <summary>
        /// Wrapper field for input identifier. This is what gets included in the XML file.
        /// </summary>
        public SerializableInputIdentifier serializableInputIdentifier;

        /// <summary>
        /// Creates an empty configuration object
        /// </summary>
        public Configuration()
        {
            actuators = new SerializableDictionary<int, SerializableTuple<int, int>>();
            pressureSensors = new List<int>();
        }

        /// <summary>
        /// Verifies that this configuration is valid.
        /// When an element of this configuration is found in the list
        /// devicesToBeConfigured, the element is removed from the list.
        /// 
        /// Returns true if there is a servo board for this configuration.
        /// Sets interfaceKitBoardAttached to true if there is an interface kit board.
        /// </summary>
        /// <param name="devicesToBeConfigured"></param>
        /// <returns></returns>
        public bool checkConfiguration(ref List<Phidget> devicesToBeConfigured)
        {
            bool retval = false;
            interfaceKitBoardAttached = false;

            // Check if AdvancedServoBoard and/or InterfaceKitBoard is attached
            for (int i = devicesToBeConfigured.Count - 1; i >= 0; i--)
            {
                Phidget phidget = devicesToBeConfigured[i];
                if (!retval && phidget.GetType() == typeof(AdvancedServo) && idServoBoard == phidget.SerialNumber)
                {
                    retval = true;
                    devicesToBeConfigured.Remove(phidget);
                }
                else if (phidget.GetType() == typeof(InterfaceKit) && idInterfaceKit == phidget.SerialNumber)
                {
                    interfaceKitBoardAttached = true;
                    devicesToBeConfigured.Remove(phidget);
                }
            }

            // Return true as long as servoBoard is attached. 
            // This way the device can work even without the pressure sensors.
            return retval;
        }

    }
}
