using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHTP_API
{
    /// <summary>
    /// BasicBehaviours define a set of behaviours general enough to be used by 
    /// any application. 
    /// </summary>
    public class BasicBehaviour : IBehaviour
    {
        private int TIME;
        private double position;
        private int _currentActiveActuators;
        private int _prevActiveActuators;

        /// <summary>
        /// This enum is used to specify the type of BasicBehaviour
        /// </summary>
        public enum TYPES { 
            /// <summary>
            /// A flat behaviour puts all actuators to their minimum position
            /// </summary>
            flat, 
            /// <summary>
            /// A notification behaviour moves all the actuators alternatively
            /// </summary>
            notification };

        private TYPES _type;

        /// <summary>
        /// Constructor for a BasicBehaviour
        /// </summary>
        /// <param name="type"></param>
        public BasicBehaviour(TYPES type)
        {
            _type = type;
            _currentActiveActuators = 1;
            TIME = 0;
            position = 170;
        }

        /// <summary>
        /// Play basic behaviour. 
        /// </summary>
        /// <param name="actuators"></param>
        /// <returns></returns>
        public Dictionary<int, double> play(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData)
        {
            Dictionary<int, double> retval = new Dictionary<int, double>();

            TIME++;
            switch (_type)
            {
                case TYPES.flat:
                    foreach (KeyValuePair<int, SerializableTuple<int, int>> entry in actuators)
                    {
                        retval[entry.Key] = actuators[entry.Key].Item1;
                    }
                    break;
                case TYPES.notification:
                    retval = playNotification(actuators, pressureData);
                    break;
                default:
                    Helper.Logger("MHTP_API.BasicBehaviours.play::type " + _type + "unknown");
                    break;
            }

            System.Threading.Thread.Sleep(100);
            return retval;
        }

        private Dictionary<int, double> playNotification(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData)
        {
            Dictionary<int, double> retval = new Dictionary<int, double>();

            int numberActuators = actuators.Count;
            int limit = (int)(Math.Pow(2, numberActuators));
            if (TIME % 2 == 0)
            {
                _currentActiveActuators = (_currentActiveActuators - _prevActiveActuators) & ~limit;
                _prevActiveActuators = _currentActiveActuators;
            }
            else
            {
                _currentActiveActuators = ((_currentActiveActuators << 1) |
                                    (_currentActiveActuators >> (numberActuators - 1)) |
                                    _currentActiveActuators) & ~limit;
            }

            int tmp = _currentActiveActuators;
            for (int i = 0; i < numberActuators; i++)
            {
                if ((tmp & 1) != 0)
                {
                    position = position > actuators[i].Item2 ? actuators[i].Item2 : position;
                    // XXX - only if pressure data exist !?
                    double adjustedPosition = position * (1 / 3.0 * Math.Cos(pressureData[i] * Math.PI / 1000) + 2 / 3.0); // XXX - assume pressure range is 1000
                    retval[i] = adjustedPosition;    
                }
                else
                {
                    retval[i] = actuators[i].Item1;
                }
                tmp = tmp >> 1;
            }
            return retval;
        }


        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false
            BasicBehaviour p = obj as BasicBehaviour;
            if ((System.Object)p == null) return false;

            // Return true if the fields match
            return (p._type == this._type);
        }

        // @see: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this._type.GetHashCode();
                return hash;
            }
        }
         
    }
}
