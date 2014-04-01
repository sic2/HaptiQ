using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaptiQ_API
{
    public class NotificationBehaviour : Behaviour
    {
        private const int DEFAULT_FREQUENCY = 1;
        private const int DEFAULT_WAITING_MS = 200;
  
        private double _frequency;
        private double[,] _positions;

         /// <summary>
        /// Constructor for a NotificationBehaviour.
        /// </summary>
        /// <param name="haptiQ"></param>
        public NotificationBehaviour(HaptiQ haptiQ)
            : this(haptiQ, DEFAULT_FREQUENCY) { }

        /// <summary>
        /// Constructor for a NotificationBehaviour. 
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <param name="frequency"></param>
        public NotificationBehaviour(HaptiQ haptiQ, double frequency)
            : base(haptiQ)
        {
            TIME = 0;
            _frequency = frequency;

            _positions = new double[_actuators.Count(), 2];
            double position = 0.0;
            for (int i = 0; i < _positions.GetLength(0); i++)
            {
                position += 1.0 / (_positions.GetLength(0) - 1);
             
                _positions[i, 0] = position;
                _positions[i, 1] = 1;
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

            double offset = 1.0 / (_positions.GetLength(0) - 1);
            for (int i = 0; i < _positions.GetLength(0); i++)
            {
                if (_positions[i, 1] == 1)
                {
                    if (_positions[i, 0] + offset <= 1.0)
                    {
                        _positions[i, 0] += offset;
                    }
                    else
                    {
                        _positions[i, 1] = -1;
                        _positions[i, 0] -= offset;
                    }
                }
                else if (_positions[i, 1] == -1)
                {
                    if (_positions[i, 0] - offset >= 0.0)
                    {
                        _positions[i, 0] -= offset;
                    }
                    else
                    {
                        _positions[i, 1] = 1;
                        _positions[i, 0] += offset;
                    }
                }
                // Reduce position by half the pressure percentage
                retval[i] = _positions[i, 0] *
                   (1 - _actuatorsDict[i].pressure / (2.0 * Actuator.MAX_PRESSURE));
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
            NotificationBehaviour notificationBehaviour = behaviour as NotificationBehaviour;
            if (notificationBehaviour != null)
            {
                this._positions = notificationBehaviour._positions;
            }
        }

        /// <summary>
        /// Override equals to allow notification behaviour objects to be compared properly
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false
            NotificationBehaviour p = obj as NotificationBehaviour;
            if ((System.Object)p == null) return false;

            // Return true if the fields match
            return (p._frequency == this._frequency);
        }

        // @see: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this._frequency.GetHashCode();
                return hash;
            }
        }
    }
}
