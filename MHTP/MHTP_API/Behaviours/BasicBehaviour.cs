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
    public class BasicBehaviour : Behaviour
    {
        private const int DEFAULT_FREQUENCY = 1;
        private const int INIT_CURRENT_ACTIVE_ACTS = 1;
        private const int INIT_PREV_ACTIVE_ACTS = 1;
        private const double DEFAULT_POS = 0.7;
        private const int DEFAULT_WAITING_MS = 200;
  
        private double _frequency;
        private double[,] positions; // TODO - use underscore for private field

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
        /// Constructor for a BasicBehaviour.
        /// </summary>
        /// <param name="mhtp"></param>
        /// <param name="type"></param>
        public BasicBehaviour(MHTP mhtp, TYPES type) 
            : this(mhtp, type, DEFAULT_FREQUENCY) { }

        /// <summary>
        /// Constructor for a BasicBehaviour. 
        /// </summary>
        /// <param name="mhtp"></param>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        public BasicBehaviour(MHTP mhtp, TYPES type, double frequency) : base(mhtp)
        {
            _type = type;
            TIME = 0;
            _frequency = frequency;

            positions = new double[_actuators.Count(), 2];
            double position = 0.0;
            double direction = 0;
            for (int i = 0; i < positions.GetLength(0); i++)
            {
                switch (type)
                {
                    case TYPES.flat:
                        position = MIN_POSITION;
                        break;
                    case TYPES.max:
                        position = MAX_POSITION;
                        break;
                    case TYPES.notification:
                        position += 1.0 / (positions.GetLength(0) - 1);
                        direction = 1; // Increasing direction
                        break;
                    default:
                        break;
                }
               
                positions[i, 0] = position;
                positions[i, 1] = direction;
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

            double offset = 1.0 / (positions.GetLength(0) - 1);
            for (int i = 0; i < positions.GetLength(0); i++)
            {
                if (positions[i, 1] == 1)
                {
                    if (positions[i, 0] + offset <= 1.0)
                    {
                        positions[i, 0] += offset;
                    }
                    else
                    {
                        positions[i, 1] = -1;
                        positions[i, 0] -= offset;
                    }
                }
                else if (positions[i, 1] == -1)
                {
                    if (positions[i, 0] - offset >= 0.0)
                    {
                        positions[i, 0] -= offset;
                    }
                    else
                    {
                        positions[i, 1] = 1;
                        positions[i, 0] += offset;
                    }
                }
                retval[i] = positions[i, 0]; // TODO - apply pressure input !?
            }

            System.Threading.Thread.Sleep((int)(DEFAULT_WAITING_MS * _frequency));
            return retval;
        }

        /// <summary>
        /// Updates this behaviour based on an another behaviour
        /// </summary>
        /// <param name="behaviour"></param>
        public override void updateNext(IBehaviour behaviour)
        {
            base.updateNext(behaviour);
            BasicBehaviour basicBehaviour = behaviour as BasicBehaviour;
            if (basicBehaviour != null)
            {
                this.positions = basicBehaviour.positions;
            }
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
