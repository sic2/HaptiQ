using System;
using System.Collections.Generic;

namespace MHTP_API
{
    public class DirectionBehaviour : IBehaviour
    {
        private int TIME;
        private int _direction { get; set; }
        private int _orientation;

        private double highPosition;
        private double lowPosition;
        private bool _pulse;

        // First bit in array indicated whether actuators alternate or not
        // -1 means that actuator does not move when pulsing. 
        // Note - this works for MHTP that has 4 actuators only with a precision of 45degrees
        private int[][][] directionMatrix = new int[][][]
                    {  new int[][]{                 // vertical 
                        new int[]{0, 1, 0, 1, 0}, 
                        new int[]{1, 1, 1, 0, 0}, 
                        new int[]{0, 0, 1, 0, 1}, 
                        new int[]{1, 1, 0, 0, 1}, 
                        new int[]{0, 1, 0, 1, 0}, 
                        new int[]{1, 1, 1, 0, 0}, 
                        new int[]{0, 0, 1, 0, 1}, 
                        new int[]{1, 1, 0, 0, 1}}, 
                    new int[][]{                    // horizontal
                        new int[]{0, 0, 1, 0, 1},
                        new int[]{1, 1, 0, 0, 1},
                        new int[]{0, 1, 0, 1, 0},
                        new int[]{1, 1, 1, 0, 0},
                        new int[]{0, 0, 1, 0, 1},
                        new int[]{1, 1, 0, 0, 1},
                        new int[]{0, 1, 0, 1, 0},
                        new int[]{1, 1, 1, 0, 0}},
                    new int[][]{                    // bottom-left
                        new int[]{0, 1, 1, 0, 0},
                        new int[]{1, 1, 0, -1, 0},
                        new int[]{0, 1, 0, 0, 1},
                        new int[]{1, 0, -1, 0, 1}, 
                        new int[]{0, 0, 0, 1, 1},
                        new int[]{1, -1, 0, 1, 0},
                        new int[]{0, 0, 1, 1, 0},
                        new int[]{1, 0, 1, 0, -1}},
                    new int[][]{                    // bottom-right
                        new int[]{0, 1, 0, 0, 1},
                        new int[]{1, 0, -1, 0, 1},
                        new int[]{0, 0, 0, 1, 1},
                        new int[]{1, -1, 0, 1, 0},
                        new int[]{0, 0, 1, 1, 0},
                        new int[]{1, 0, 1, 0, -1},
                        new int[]{0, 1, 1, 0, 0},
                        new int[]{1, 1, 0, -1, 0},}, 
                    new int[][]{                    // top-right
                        new int[]{0, 0, 0, 1, 1},
                        new int[]{1, -1, 0, 1, 0},
                        new int[]{0, 0, 1, 1, 0},
                        new int[]{1, 0, 1, 0, -1},
                        new int[]{0, 1, 1, 0, 0},
                        new int[]{1, 1, 0, -1, 0},
                        new int[]{0, 1, 0, 0, 1},
                        new int[]{1, 0, -1, 0, 1},}, 
                    new int[][]{                    // top-left
                        new int[]{0, 0, 1, 1, 0},
                        new int[]{1, 0, 1, 0, -1},
                        new int[]{0, 1, 1, 0, 0},
                        new int[]{1, 1, 0, -1, 0},
                        new int[]{0, 1, 0, 0, 1},
                        new int[]{1, 0, -1, 0, 1},
                        new int[]{0, 0, 0, 1, 1},
                        new int[]{1, -1, 0, 1, 0}}, 
                    };
                     
        /// <summary>
        /// direction values are:
        /// - 0 vertical
        /// - 1 horizontal
        /// - 2 bottom-left
        /// - 3 bottom-right
        /// - 4 top-right
        /// - 5 top-left
        /// - (-1) all
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="orientation"></param>
        /// <param name="pulse"></param>
        public DirectionBehaviour(int direction, int orientation, bool pulse) 
        {
            _direction = direction;
            _orientation = orientation % 8;
           
            TIME = 0;
            _pulse = pulse;

            highPosition = 160;
            lowPosition = 135;
        }

        // TODO - refactor
        public Dictionary<int, double> play(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData)
        {
            Dictionary<int, double> retval = new Dictionary<int, double>();

            TIME++;

            if (_direction != -1)
            {
                int[] acts = directionMatrix[_direction][_orientation];
                if (acts[0] == 1)
                    _pulse = true;
            }
            else
            {
                _pulse = false;
            }

            if (_pulse)
            {
                if (TIME % 2 == 0)
                {
                    double tmp = highPosition;
                    highPosition = lowPosition;
                    lowPosition = tmp;
                }
            }
            else
            {
                highPosition = 170;
                lowPosition = 135;
            }

            if (_direction == -1)
            {
                for (int i = 0; i < actuators.Count; i++)
                {
                    retval[i] = highPosition;
                }
            }
            else
            {
                int[] acts = directionMatrix[_direction][_orientation]; // TODO - double actuators array if necessary
                foreach (KeyValuePair<int, SerializableTuple<int, int>> entry in actuators)
                {
                    switch (acts[entry.Key + 1])
                    {
                        case -1:
                            retval[entry.Key] = actuators[entry.Key].Item1;
                            break;
                        case 0:
                            retval[entry.Key] = lowPosition;
                            break;
                        case 1:
                            retval[entry.Key] = highPosition;
                            break;
                        default:
                            Helper.Logger("MHTP_API.DirectionBehaviour.Play:: (entry.key + 1: " 
                                + (entry.Key + 1) + ") not valid. Direction: " + _direction +
                                "; orientation: " + _orientation);
                            break;
                    }
                }
            }

            System.Threading.Thread.Sleep(100);
            return retval;
        }

        private int[] extendMatrix(int[] matrix)
        {
            int[] retVal = new int[matrix.Length * 2];

            for (int i = 0; i < matrix.Length; i++)
            {
                retVal[i * 2] = matrix[i];
            }

            return retVal;
        }

        
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false
            DirectionBehaviour p = obj as DirectionBehaviour;
            if ((System.Object)p == null) return false;

            // Return true if the fields match
            return (p._direction == this._direction && 
                p._orientation == this._orientation);
        }

        // @see: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        public override int GetHashCode()
        {
            unchecked 
            {
                int hash = 17;
                hash = hash * 23 + this._direction.GetHashCode();
                hash = hash * 23 + this._orientation.GetHashCode();
                return hash;
            }
        }
         
    }
}
