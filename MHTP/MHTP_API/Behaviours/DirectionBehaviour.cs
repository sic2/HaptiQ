using System;
using System.Collections.Generic;

using Input_API;

namespace MHTP_API
{
    public class DirectionBehaviour : IBehaviour
    {
        private int TIME;
        private int _direction { get; set; }
        private double _orientation;

        private double highPosition;
        private double lowPosition;

        private const int NUMBER_ACTUATORS_DIVIDER = 4;

        private List<Tuple<Point, Point>> _lines;

        private int[] staticActuatorsMatrix = new int[] 
                         { 10, // 4-MHTP 1-line
                           68}; // 8 -MHTP 1-line

        private int[][] dynamicActuatorsMatrix = new int[][]
                        {
                            new int[]{12, 3}, // 4-MHTP 1-line
                            new int[]{132, 72}// 8-MHTP 1-line
                        };

        public DirectionBehaviour(List<Tuple<Point, Point>> lines, double orientation)
        {
            _lines = lines;
            _orientation = orientation;

            TIME = 0;

            // TODO - do not use these absolute values
            highPosition = 160;
            lowPosition = 135;
        }

        /// <summary>
        /// Play this behaviour. 
        /// </summary>
        /// <param name="actuators"></param>
        /// <param name="pressureData"></param>
        /// <returns></returns>
        public Dictionary<int, double> play(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData)
        {
            Dictionary<int, double> retval = new Dictionary<int, double>();
            TIME++;

            // Normalise number of actuators. This is necessary, otherwise the methods used 
            // below do not behave correctly
            int numberActuators = actuators.Count <= 4 ? 4 : 8; // TODO - do not use magic numbers
            bool isCorner = _lines.Count == 2 ? true : false;
            if (isCorner)
            {
                cornerBehaviour(actuators, pressureData, numberActuators, ref retval);
            }
            else
            {
                segmentBehaviour(actuators, pressureData, numberActuators, ref retval);
            }

            System.Threading.Thread.Sleep(100);
            return retval;
        }

        // TODO - rename method 
        private void segmentBehaviour(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData, int numberActuators, ref Dictionary<int, double> output)
        {
            int limit = (int)(Math.Pow(2, numberActuators));

            // Determines angle between lines in radians
            double angle = Math.Atan2(_lines[0].Item1.Y - _lines[0].Item2.Y,
                                        _lines[0].Item1.X - _lines[0].Item2.X);
            // Map angle to 0-(2*PI)
            angle = (angle > 0 ? angle : (2 * Math.PI + angle));
            angle += _orientation; // Total angle between lines and device orientation
            // Normalise angle
            double sectorRange = (2 * Math.PI) / (2.0 * numberActuators);
            double normAngle = angle + (sectorRange / 2.0);
            int sector = (int)Math.Floor(normAngle / sectorRange) % numberActuators;

            int matrixIndex = numberActuators / NUMBER_ACTUATORS_DIVIDER - 1;
            if (sector % 2 == 0) // Static sector
            {
                int activeActs = staticActuatorsMatrix[matrixIndex];
                activeActs = shiftActs(activeActs, sector, numberActuators, limit);
                bitsToActuators(activeActs, numberActuators, false, ref output);
            }
            else // Pulsing sector
            {
                int[] acts = dynamicActuatorsMatrix[matrixIndex];
                acts[0] = shiftActs(acts[0], sector, numberActuators, limit);
                acts[1] = shiftActs(acts[1], sector, numberActuators, limit);

                bitsToActuators(acts[0], numberActuators, TIME % 2 == 0, ref output);
                bitsToActuators(acts[1], numberActuators, TIME % 2 != 0, ref output);
            }
        }

        private void cornerBehaviour(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData, int numberActuators, ref Dictionary<int, double> output)
        {
            // http://stackoverflow.com/questions/3365171/calculating-the-angle-between-two-lines-without-having-to-calculate-the-slope
            int limit = (int)(Math.Pow(2, numberActuators));

            // Determines angle between lines in radians
            double angle1 = Math.Atan2(_lines[0].Item1.Y - _lines[0].Item2.Y,
                                        _lines[0].Item1.X - _lines[0].Item2.X);
            double angle2 = Math.Atan2(_lines[1].Item1.Y - _lines[1].Item2.Y,
                                        _lines[1].Item1.X - _lines[1].Item2.X);
            double angle = angle1 - angle2;
           
            // Map angle to 0-(2*PI)
            angle = (angle > 0 ? angle : (2 * Math.PI + angle));
            Console.WriteLine("Angle is {0:f}", angle);
            angle += _orientation; // Total angle between lines and device orientation
            // Normalise angle
            double sectorRange = (2 * Math.PI) / (2.0 * numberActuators);
            double normAngle = angle + (sectorRange / 2.0);
            int sector = (int)Math.Floor(normAngle / sectorRange) % numberActuators;

            int matrixIndex = numberActuators / NUMBER_ACTUATORS_DIVIDER - 1;

            // TODO

        }

        private int detectCorner()
        {
            return -1;
        }

        private int shiftActs(int acts, int sector, int numberActuators, int limit)
        {
            return (acts << (int)(sector / 2) |
                    acts >> (numberActuators - (int)(sector / 2))) & (limit - 1);
        }

        private void bitsToActuators(int activeActuators, int numberActuators, bool switchPositionOrder, ref Dictionary<int, double> output)
        {
            double pos0 = highPosition;
            double pos1 = lowPosition;
            if (switchPositionOrder)
            {
                double tmp = pos0;
                pos0 = pos1;
                pos1 = tmp;
            }
            for (int i = 0; i < numberActuators; i++)
            {
                if ((activeActuators & 1) != 0)
                {
                    output[i] = pos0;
                }
                else
                {
                    output[i] = pos1;
                }
                activeActuators = activeActuators >> 1;
            }
        }


        // TODO - refactor
        /*
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
         * */
        
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
