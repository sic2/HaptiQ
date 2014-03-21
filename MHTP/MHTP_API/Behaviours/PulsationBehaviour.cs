using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Input_API;

namespace MHTP_API
{
    // TODO - remove all code duplicated from DirectionBehaviour
    /// <summary>
    /// 
    /// </summary>
    public class PulsationBehaviour : IBehaviour
    {
        public int TIME { get; set; }

        private double highPosition;
        private double lowPosition;
        private double _orientation;
        private double _frequency;

        private const int NUMBER_ACTUATORS_DIVIDER = 4;

        private Tuple<Point, Point> _segment;
        private int[] singleActuatorsMatrix = new int[] 
                         { 2, // 4-MHTP 1-line
                           4}; // 8 -MHTP 1-line

        private int[][] dynamicActuatorsMatrix = new int[][]
                        {
                            new int[]{12, 3}, // 4-MHTP 1-line
                            new int[]{132, 72}// 8-MHTP 1-line
                        };

        /// <summary>
        /// Constructor of the pulsation behaviour.
        /// Make the specified actuators to pulse at a constant interval 
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="orientation"></param>
        /// <param name="frequency"></param>
        public PulsationBehaviour(Tuple<Point, Point> segment, double orientation, double frequency)
        {
            _segment = segment;
            _orientation = orientation;
            _frequency = frequency;
            TIME = 0;
            highPosition = 40;
            lowPosition = 0;
        }

        public Dictionary<int, double> play(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData)
        {
            Dictionary<int, double> retval = new Dictionary<int, double>();
            TIME++;

            int numberActuators = actuators.Count;
            segmentBehaviour(actuators, pressureData, numberActuators, ref retval);
         
            System.Threading.Thread.Sleep((int)(10 * _frequency));
            return retval;
        }

        private void segmentBehaviour(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
           Dictionary<int, double> pressureData, int numberActuators, ref Dictionary<int, double> output)
        {
            // Determines angle between lines in radians
            // (Math.PI - angle) is necessary because y is inverted
            double angle = Math.PI - Math.Atan2(_segment.Item1.Y - _segment.Item2.Y,
                                        _segment.Item1.X - _segment.Item2.X);
            // Map angle to 0-(2*PI)
            angle = (angle > 0 ? angle : (2 * Math.PI + angle));
            angle += _orientation; // Total angle between lines and device orientation
            // Normalise angle
            double sectorRange = (2 * Math.PI) / (2.0 * numberActuators);
            double normAngle = angle + (sectorRange / 2.0);
            int sector = (int)Math.Floor(normAngle / sectorRange) % numberActuators;

            int matrixIndex = numberActuators / NUMBER_ACTUATORS_DIVIDER - 1;
            if (sector % 2 == 0) // Single actuators sector
            {
                int activeActs = singleActuatorsMatrix[matrixIndex];
                activeActs = shiftActs(activeActs, (int)(sector / 2), numberActuators);
                bitsToActuators(actuators, activeActs, TIME % 2 == 0, false, ref output);
            }
            else // Pulsing sector
            {
                int[] acts = dynamicActuatorsMatrix[matrixIndex];
                acts[0] = shiftActs(acts[0], (int)(sector / 2), numberActuators);
                acts[1] = shiftActs(acts[1], (int)(sector / 2), numberActuators);

                bitsToActuators(actuators, acts[0], TIME % 2 == 0, true, ref output);
                bitsToActuators(actuators, acts[1], TIME % 2 != 0, true, ref output);
            }
        }

        /// <summary>
        /// Shift acts to the left with carry by offset.
        /// Number of bits is determined by numberActuators.
        /// </summary>
        /// <param name="acts"></param>
        /// <param name="offset"></param>
        /// <param name="numberActuators"></param>
        /// <returns></returns>
        private int shiftActs(int acts, int offset, int numberActuators)
        {
            int limit = (int)(Math.Pow(2, numberActuators));
            return (acts << offset |
                    acts >> (numberActuators - offset)) & (limit - 1);
        }

        private void bitsToActuators(SerializableDictionary<int, SerializableTuple<int, int>> actuators, int activeActuators, bool switchPositionOrder, bool setZeros, ref Dictionary<int, double> output)
        {
            double pos0 = highPosition;
            double pos1 = lowPosition;
            if (switchPositionOrder)
            {
                double tmp = pos0;
                pos0 = pos1;
                pos1 = tmp;
            }
            for (int i = 0; i < actuators.Count; i++)
            {
                output[i] = actuators[i].Item1;
                if ((activeActuators & 1) != 0)
                {
                    output[i] += pos0;
                }
                else if (setZeros)
                {
                    output[i] += pos1;
                }
                activeActuators = activeActuators >> 1;
            }
        }
        
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false
            PulsationBehaviour p = obj as PulsationBehaviour;
            if ((System.Object)p == null) return false;

            // Return true if the fields match
            return (p._segment == this._segment &&
                p._orientation == this._orientation && 
                p._frequency == this._frequency);
        }

        // @see: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this._segment.GetHashCode();
                hash = hash * 23 + this._orientation.GetHashCode();
                hash = hash * 23 + this._frequency.GetHashCode();
                return hash;
            }
        }
         
    }
}