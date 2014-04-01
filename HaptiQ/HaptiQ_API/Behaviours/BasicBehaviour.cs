using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaptiQ_API
{
    /// <summary>
    /// BasicBehaviours define a set of behaviours general enough to be used by 
    /// any application. 
    /// </summary>
    public class BasicBehaviour : Behaviour
    {
        private const int DEFAULT_FREQUENCY = 1;
        private const double DEFAULT_POS = 0.7;
        private const int DEFAULT_WAITING_MS = 200;
  
        private double _frequency;
        private int _actuatorsToActivate;

        /// <summary>
        /// This enum is used to specify the type of BasicBehaviour
        /// </summary>
        public enum TYPES 
        { 
            /// <summary>
            /// A flat behaviour puts all actuators to their minimum position
            /// </summary>
            flat,
            /// <summary>
            /// A max behaviour moves all actuators to their maximum position
            /// </summary>
            max
        };

        private TYPES _type;

        /// <summary>
        /// Constructor for a BasicBehaviour.
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <param name="type"></param>
        public BasicBehaviour(HaptiQ haptiQ, TYPES type)
            : this(haptiQ, type, DEFAULT_FREQUENCY) { }

        /// <summary>
        /// Constructor for a BasicBehaviour. 
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        public BasicBehaviour(HaptiQ haptiQ, TYPES type, double frequency)
            : base(haptiQ)
        {
            _type = type;
            TIME = 0;
            _frequency = frequency;
            highPosition = DEFAULT_POS;
            lowPosition = MIN_POSITION;

            if (type == TYPES.flat)
            {
                _actuatorsToActivate = 0;
            }
            else if (type == TYPES.max)
            {
                _actuatorsToActivate = (int) (Math.Pow(2, _actuators.Count()) - 1);
            }
            else
            {
                 _actuatorsToActivate = 0;
                Helper.Logger("HaptiQ_API.BasicBehaviour.BasicBehaviour::type " + type + " undefined");
            }
        }

        /// <summary>
        /// Play basic behaviour. 
        /// </summary>
        /// <returns></returns>
        public override Dictionary<int, double> play()
        {
            Dictionary<int, double> retval = new Dictionary<int, double>();
            TIME++;
            bitsToActuators(_actuators.Count, _actuatorsToActivate, false, true, ref retval);
          
            System.Threading.Thread.Sleep((int)(DEFAULT_WAITING_MS * _frequency));
            return retval;
        }

        /// <summary>
        /// Override equals to allow basic behaviour objects to be compared properly
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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
