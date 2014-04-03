using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Input_API;

namespace HaptiQ_API
{
    /// <summary>
    /// Behaviour abstract class
    /// </summary>
    abstract public class Behaviour : IBehaviour
    {
        /// <summary>
        /// Minimum position allowed for the actuators
        /// </summary>
        protected const double MIN_POSITION = 0.0;

        /// <summary>
        /// Maximum position allowed for the actuators
        /// </summary>
        protected const double MAX_POSITION = 1.0;
        
        /// <summary>
        /// Internal timer of this behaviour
        /// </summary>
        public int TIME { get; set; }

        /// <summary>
        /// High position of this behaviour
        /// </summary>
        protected double highPosition;
        /// <summary>
        /// Low position of this behaviour
        /// </summary>
        protected double lowPosition;

        /// <summary>
        /// List of actuators for this behaviour
        /// </summary>
        protected List<Actuator> actuators;

        /// <summary>
        /// Mapping id->actuator
        /// This information is duplicated from _actuators, but allows fast retrieval
        /// </summary>
        protected Dictionary<int, Actuator> actuatorsDict;

        /// <summary>
        /// Position of the HaptiQ that created this Behaviour
        /// </summary>
        protected Point position;

        /// <summary>
        /// Orientation of the HaptiQ that created this Behaviour
        /// </summary>
        protected double orientation;

        /// <summary>
        /// Behaviour constructor
        /// Note that this constructor should never be called.
        /// It exists only 
        /// </summary>
        //public Behaviour()
        //{ }

        /// <summary>
        /// Behaviour constructor
        /// </summary>
        /// <param name="haptiQ"></param>
        public Behaviour(HaptiQ haptiQ)
        {
            this.actuators = haptiQ.getActuators();
            this.position = haptiQ.position;
            this.orientation = haptiQ.orientation;

            actuatorsDict = new Dictionary<int, Actuator>();
            foreach (Actuator actuator in actuators)
            {
                actuatorsDict[actuator.getId()] = actuator;
            }
        }

        /// <summary>
        /// Return to the HaptiQ the gesture by defining 
        /// what actuators to move and by how much.
        /// Frequency must be managed by the behaviour. 
        /// Remember that behaviours' play methods are called every 10ms 
        /// (@see const HaptiQ.BEHAVIOUR_LOOP_MS)
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<int, double> play();

        /// <summary>
        /// Updates the clock of this behaviour based on an another behaviour
        /// </summary>
        /// <param name="behaviour"></param>
        public virtual void updateNext(IBehaviour behaviour)
        {
            if (behaviour != null && behaviour.GetType() == this.GetType())
            {
                this.TIME = behaviour.TIME;
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
        /// Convert an binary to a dictionary (actuatorID -> position>)
        /// </summary>
        /// <param name="numberActuators"></param>
        /// <param name="activeActuators"></param>
        /// <param name="switchPositionOrder"></param>
        /// <param name="setZeros"></param>
        /// <param name="output"></param>
        protected void bitsToActuators(int numberActuators, int activeActuators,
            bool switchPositionOrder, bool setZeros, 
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
            for (int i = 0; i < numberActuators; i++)
            {
                if ((activeActuators & 1) != 0)
                {
                    output[i] = pos0;
                }
                else if (setZeros)
                {
                    output[i] = pos1;
                }
                activeActuators = activeActuators >> 1;
            }
        }

        /// <summary>
        /// This function sets the zero bits to minimum position
        /// </summary>
        /// <param name="numberActuators"></param>
        /// <param name="zeros"></param>
        /// <param name="output"></param>
        protected void setZerosToMinimum(int numberActuators, int zeros, ref Dictionary<int, double> output)
        {
            for (int i = 0; i < numberActuators; i++)
            {
                if ((zeros & 1) == 0)
                {
                    output[i] = MIN_POSITION;
                }
                zeros = zeros >> 1;
            }
        }

        /// <summary>
        /// Return sector of the HaptiQ given a segment and orientation of the HaptiQ.
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="orientation"></param>
        /// <param name="numberActuators"></param>
        /// <param name="numberSections"></param>
        /// <returns></returns>
        protected int getSector(Tuple<Point, Point> segment, double orientation, int numberActuators, int numberSections)
        {
            if (orientation < 0) orientation = Math.PI * 2 + orientation;

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
