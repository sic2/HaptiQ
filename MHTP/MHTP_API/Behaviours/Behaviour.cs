using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Input_API;

namespace MHTP_API
{
    abstract public class Behaviour : IBehaviour
    {
        public int TIME { get; set; }

        protected double highPosition;
        protected double lowPosition;

        /// <summary>
        /// Return to the MHTP the gesture by defining 
        /// what actuators to move and by how much.
        /// Frequency must be managed by the behaviour. 
        /// Remember that behaviours' play methods are called every 10ms 
        /// (@see const MHTP.BEHAVIOUR_LOOP_MS)
        /// </summary>
        /// <param name="actuators"></param>
        /// <param name="pressureData"></param>
        /// <returns></returns>
        public abstract Dictionary<int, double> play(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData);

        /// <summary>
        /// Shift acts to the left with carry by offset.
        /// Number of bits is determined by numberActuators.
        /// </summary>
        /// <param name="acts"></param>
        /// <param name="offset"></param>
        /// <param name="numberActuators"></param>
        /// <returns></returns>
        protected int LshiftActs(int acts, int offset, int numberActuators)
        {
            int limit = (int)(Math.Pow(2, numberActuators));
            return (acts << offset |
                    acts >> (numberActuators - offset)) & (limit - 1);
        }

        /// <summary>
        /// Shift acts to the right with carry by offset.
        /// Number of bits is determined by numberActuators.
        /// </summary>
        /// <param name="acts"></param>
        /// <param name="offset"></param>
        /// <param name="numberActuators"></param>
        /// <returns></returns>
        protected int RshiftActs(int acts, int offset, int numberActuators)
        {
            int limit = (int)(Math.Pow(2, numberActuators));
            return (acts >> offset |
                    acts << (numberActuators - offset)) & (limit - 1);
        }

        /// <summary>
        /// Convert an binary to a dictionary<actuatorID, position>
        /// </summary>
        /// <param name="actuators"></param>
        /// <param name="activeActuators"></param>
        /// <param name="switchPositionOrder"></param>
        /// <param name="setZeros"></param>
        /// <param name="output"></param>
        protected void bitsToActuators(SerializableDictionary<int, SerializableTuple<int, int>> actuators, 
            int activeActuators, bool switchPositionOrder, bool setZeros,
            ref Dictionary<int, double> output)
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
                if ((activeActuators & 1) != 0)
                {
                    output[i] = actuators[i].Item1 + pos0;
                }
                else if (setZeros)
                {
                    output[i] = actuators[i].Item1 + pos1;
                }
                activeActuators = activeActuators >> 1;
            }
        }

        /// <summary>
        /// This function sets the zero bits to minimum position
        /// </summary>
        /// <param name="actuators"></param>
        /// <param name="zeros"></param>
        /// <param name="output"></param>
        protected void setZerosToMinimum(SerializableDictionary<int, SerializableTuple<int, int>> actuators, int zeros, ref Dictionary<int, double> output)
        {
            for (int i = 0; i < actuators.Count; i++)
            {
                if ((zeros & 1) == 0)
                {
                    output[i] = actuators[i].Item1;
                }
                zeros = zeros >> 1;
            }
        }

        protected int getSector(Tuple<Point, Point> segment, double orientation, int numberActuators, int numberSections)
        {
            // Determines angle between lines in radians
            double angle = Math.PI - Math.Atan2(segment.Item1.Y - segment.Item2.Y,
                                        segment.Item1.X - segment.Item2.X);
            // Map angle to 0-(2*PI)
            angle = (angle > 0 ? angle : (2 * Math.PI + angle));
            angle += orientation; // Total angle between lines and device orientation
            // Normalise angle
            double sectorRange = (2 * Math.PI) / (2.0 * numberActuators);
            double normAngle = angle + (sectorRange / 2.0);
            int sector = (int)Math.Floor(normAngle / sectorRange) % (numberSections);
            return sector;
        }
    }
}
