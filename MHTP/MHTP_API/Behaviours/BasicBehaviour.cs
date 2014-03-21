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
        public int TIME { get; set; }
  
        private double _position;
        private int _numberActuators;
        private double _frequency;

        public int currentActiveActuators { get; set; }
        public int prevActiveActuators { get; set; }

        /// <summary>
        /// This enum is used to specify the type of BasicBehaviour
        /// </summary>
        public enum TYPES { 
            /// <summary>
            /// A flat behaviour puts all actuators to their minimum position
            /// </summary>
            flat,
            /// <summary>
            /// A max behaviour moves all actuators to their maximum position
            /// </summary>
            max,
            /// <summary>
            /// A notification behaviour moves all the actuators alternatively
            /// </summary>
            notification };

        private TYPES _type;

        /// <summary>
        /// Constructor for a BasicBehaviour
        /// </summary>
        /// <param name="type"></param>
        public BasicBehaviour(TYPES type) : this(type, 1, 1, 1) {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        public BasicBehaviour(TYPES type, int current, int prev, double frequency)
        {
            _type = type;
            currentActiveActuators = current;
            prevActiveActuators = prev;
            TIME = 0;
            _position = 40;
            _frequency = frequency;
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
                case TYPES.max:
                    foreach (KeyValuePair<int, SerializableTuple<int, int>> entry in actuators)
                    {
                        retval[entry.Key] = actuators[entry.Key].Item2;
                    }
                    break;
                case TYPES.notification:
                    retval = playNotification(actuators, pressureData);
                    break;
                default:
                    Helper.Logger("MHTP_API.BasicBehaviours.play::type " + _type + "unknown");
                    break;
            }

            System.Threading.Thread.Sleep((int) (200 * _frequency));
            return retval;
        }

        private Dictionary<int, double> playNotification(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData)
        {
            Dictionary<int, double> retval = new Dictionary<int, double>();

            _numberActuators = actuators.Count;
            nextCurrentActiveActuators();
            int tmp = currentActiveActuators;
            for (int i = 0; i < _numberActuators; i++)
            {
                if ((tmp & 1) != 0)
                {
                    _position = _position > actuators[i].Item2 ? actuators[i].Item2 : _position;
                    double adjustedPosition = pressureData == null ? 
                            _position : _position * (1 / 3.0 * Math.Cos(pressureData[i] * Math.PI / 1000) + 2 / 3.0); // XXX - assume pressure range is 1000
                    retval[i] = actuators[i].Item1 + adjustedPosition;    
                }
                else
                {
                    retval[i] = actuators[i].Item1;
                }
                tmp = tmp >> 1;
            }
            return retval;
        }

        private void nextCurrentActiveActuators()
        {
            int limit = (int)(Math.Pow(2, _numberActuators));
            if (TIME % 2 == 0)
            {
                currentActiveActuators = (currentActiveActuators - prevActiveActuators) & (limit - 1);
                prevActiveActuators = currentActiveActuators;
            }
            else
            {
                currentActiveActuators = ((currentActiveActuators << 1) |
                                    (currentActiveActuators >> (_numberActuators - 1)) |
                                    currentActiveActuators) & (limit - 1);
            }
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false
            BasicBehaviour p = obj as BasicBehaviour;
            if ((System.Object)p == null) return false;

            // Return true if the fields match
            return (p._type == this._type &&
                p._frequency == this._frequency);
        }

        // @see: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this._type.GetHashCode();
                hash = hash * 23 + this._frequency.GetHashCode();
                return hash;
            }
        }
         
    }
}
