using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHTP_API
{
    /// <summary>
    /// 
    /// </summary>
    public class PulsationBehaviour : IBehaviour
    {
        private int TIME;

        private int _highFrequencyActuators;
        private double _highFrequency;

        private const double HIGH_POSITION = 40;
        private const double LOW_POSITION = 0;
        private double position;

        // XXX - rather than using matrices, can use logic operations (shift)
        private int[][] pulsationMatrix = new int[][]
                    { new int[]{1, 0, 0, 0},
                      new int[]{1, 1, 0, 0},
                      new int[]{0, 1, 0, 0},
                      new int[]{0, 1, 1, 0},
                      new int[]{0, 0, 1, 0},
                      new int[]{0, 0, 1, 1},
                      new int[]{0, 0, 0, 1},
                      new int[]{1, 0, 0, 1}
                    };

        /// <summary>
        /// Constructor of the pulsation behaviour.
        /// Make the specified actuators to pulse at a constant interval 
        /// </summary>
        /// <param name="highFrequencyActuators">
        /// Values from 0 to k indicate the single actuators
        /// for an MHTP with k actuators</param>
        /// <param name="highFrequency"></param>
        public PulsationBehaviour(int highFrequencyActuators, double highFrequency)
        {
            _highFrequencyActuators = highFrequencyActuators % 8;
            _highFrequency = highFrequency;

            TIME = 0;
            position = HIGH_POSITION;
        }

        public Dictionary<int, double> play(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData)
        {
            TIME++;
            if (TIME % 2 == 0)
            {
                position = HIGH_POSITION;
            }
            else
            {
                position = LOW_POSITION;
            }

            Dictionary<int, double> retval = new Dictionary<int, double>();

            int[] acts = pulsationMatrix[_highFrequencyActuators];
            foreach (KeyValuePair<int, SerializableTuple<int, int>> entry in actuators)
            {
                retval[entry.Key] = actuators[entry.Key].Item1;
                if (acts[entry.Key] == 1)
                {
                    retval[entry.Key] += position > actuators[entry.Key].Item2 ? actuators[entry.Key].Item2 : position;
                }
                else if (acts[entry.Key] == 0)
                {
                    retval[entry.Key] += actuators[entry.Key].Item1;
                }
            }

            System.Threading.Thread.Sleep((int)(10 * _highFrequency));
            return retval;
        }
        
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false
            PulsationBehaviour p = obj as PulsationBehaviour;
            if ((System.Object)p == null) return false;

            // Return true if the fields match
            return (p._highFrequencyActuators == this._highFrequencyActuators &&
                p._highFrequency == this._highFrequency);
        }

        // @see: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this._highFrequencyActuators.GetHashCode();
                hash = hash * 23 + this._highFrequency.GetHashCode();
                return hash;
            }
        }
         
    }
}