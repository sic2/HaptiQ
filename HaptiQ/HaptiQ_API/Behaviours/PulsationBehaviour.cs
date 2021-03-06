﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Input_API;

namespace HaptiQ_API
{
    /// <summary>
    /// Pulsation behaviour defines a set of behaviours where 
    /// an actuator or a set of actuators pulses
    /// </summary>
    public class PulsationBehaviour : Behaviour
    { 
        private double _frequency;

        private const int NUMBER_ACTUATORS_DIVIDER = 4;
        private const double HIGH_POSITION_PERCENTAGE = 0.8;

        private Tuple<Point, Point> _segment;
        private int[] singleActuatorsMatrix = new int[] 
                         { 2, // 4-HaptiQ 1-line
                           4}; // 8 -HaptiQ 1-line

        private int[][] dynamicActuatorsMatrix = new int[][]
                        {
                            new int[]{1, 2}, // 4-HaptiQ 1-line
                            new int[]{2, 4}// 8-HaptiQ 1-line
                        };

        /// <summary>
        /// Constructor of the pulsation behaviour.
        /// Make the specified actuators to pulse at a constant interval 
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <param name="segment"></param>
        /// <param name="frequency"></param>
        public PulsationBehaviour(HaptiQ haptiQ, Tuple<Point, Point> segment, double frequency)
            : base(haptiQ)
        {
            _segment = segment;
            _frequency = frequency;
            TIME = 0;
            highPosition = HIGH_POSITION_PERCENTAGE;
            lowPosition = 0;
        }

        /// <summary>
        /// Plays this behaviour
        /// </summary>
        /// <returns></returns>
        public override Dictionary<int, double> play()
        {
            Dictionary<int, double> retval = new Dictionary<int, double>();
            TIME++;

            segmentBehaviour(ref retval);
         
            System.Threading.Thread.Sleep((int)(10 * _frequency));
            return retval;
        }

        /// <summary>
        /// Segment behaviour with pulsation
        /// </summary>
        /// <param name="output"></param>
        private void segmentBehaviour(ref Dictionary<int, double> output)
        {
            int sector = getSector(_segment, orientation, actuators.Count, actuators.Count * 2);
            int matrixIndex = actuators.Count / NUMBER_ACTUATORS_DIVIDER - 1;
            if (sector % 2 == 0) // Single pulsating actuators sector
            {
                int activeActs = singleActuatorsMatrix[matrixIndex];
                activeActs = RshiftActs(activeActs, (int)(sector / 2), actuators.Count);
                bitsToActuators(actuators.Count, activeActs, TIME % 2 == 0, false, ref output);
                setZerosToMinimum(actuators.Count, activeActs, ref output); 
            }
            else // Double pulsating actuators sector
            {
                int[] acts = (int[]) dynamicActuatorsMatrix[matrixIndex].Clone();
                acts[0] = RshiftActs(acts[0], (int)((sector - 1) / 2), actuators.Count);
                acts[1] = RshiftActs(acts[1], (int)((sector - 1) / 2), actuators.Count);
                bitsToActuators(actuators.Count, acts[0] | acts[1], TIME % 2 == 0, false, ref output);
                setZerosToMinimum(actuators.Count, (acts[0] | acts[1]), ref output); 
            }
        }
        
        /// <summary>
        /// Override equals to allow PulsationBehaviour to be compared correctly.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false
            PulsationBehaviour p = obj as PulsationBehaviour;
            if ((System.Object)p == null) return false;

            // Return true if the fields match
            return (p._segment == this._segment &&
                p.orientation == this.orientation && 
                p._frequency == this._frequency);
        }

        // @see: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this._segment.GetHashCode();
                hash = hash * 23 + this.orientation.GetHashCode();
                hash = hash * 23 + this._frequency.GetHashCode();
                return hash;
            }
        }
    }
}