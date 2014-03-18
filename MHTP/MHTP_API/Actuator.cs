using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Phidgets;

namespace MHTP_API
{
    /// <summary>
    /// The Actuator class represents a physical actuator of the MHTP.
    /// An actuator has a position (output) and a current applied pressure (input)
    /// </summary>
    public class Actuator
    {
        private double _pressure;
        private int _id; // used to identify which actuator is of the MHTP
        private double _minPosition;
        private double _maxPosition;

        private AdvancedServoServo _servo;

        /// <summary>
        /// Creates an actuator
        /// </summary>
        /// <param name="servo"></param>
        /// <param name="id"></param>
        /// <param name="minPosition"></param>
        /// <param name="maxPosition"></param>
        public Actuator(AdvancedServoServo servo, int id, int minPosition, int maxPosition)
        {
            if (servo == null)
            {
                Helper.Logger("MHTP_API.Actuator.Actuator:: (" + id + ") servo is null");
                throw new ArgumentNullException("MHTP_API.Actuator.Actuator:: servo");
            }

            _servo = servo;
            _id = id;
            setMinPosition(minPosition);
            setMaxPosition(maxPosition);
         }

        /// <summary>
        /// Get/Set height of this actuator.
        /// It is not possible to set an actuator to an height that does not 
        /// satisfies the min/max constraints.
        /// </summary>
        public double height
        {
            get { return _servo.Position; } 
            set 
            {
                if (value >= _minPosition && value < _maxPosition)
                    {
                        enable();
                        _servo.Position = value;
                    }
            }
        }

        /// <summary>
        /// Returns the ID of this actuator relative to the MHTP it belongs to
        /// </summary>
        /// <returns></returns>
        public int getId()
        {
            return _id;
        }

        /// <summary>
        /// Get the current pressure of this actuator
        /// </summary>
        public double pressure
        {
            get { return _pressure; }
        }

        /// <summary>
        /// Set the current pressure of this actuator.
        /// Note: This function should not be called from other classes other than MHTP
        /// </summary>
        /// <param name="pressure"></param>
        public void setPressure(double pressure)
        {
            _pressure = pressure;
        }

        /// <summary>
        /// Set this actuator to its minimum allowed position
        /// </summary>
        public void setToMinimum()
        {
            this.height = _minPosition;
        }

        /// <summary>
        /// Set this actuator to its maximum allowed position
        /// </summary>
        public void setToMaximum()
        {
            this.height = _maxPosition;
        }

        /// <summary>
        /// Set the maximum position allowed for this actuator
        /// </summary>
        /// <param name="maxPosition"></param>
        public void setMaxPosition(double maxPosition)
        {
            _maxPosition = maxPosition < _servo.PositionMax ? maxPosition : _servo.PositionMax;
        }

        /// <summary>
        /// Set the minimum position allowed for this actuator
        /// </summary>
        /// <param name="minPosition"></param>
        public void setMinPosition(double minPosition)
        {
            _minPosition = minPosition > _servo.PositionMin ? minPosition : _servo.PositionMin;
        }

        /// <summary>
        /// Set the current acceleration for this actuator
        /// </summary>
        /// <param name="acceleration"></param>
        public void setAcceleration(double acceleration)
        {
            if (acceleration >= _servo.AccelerationMin && acceleration < _servo.AccelerationMax)
                _servo.Acceleration = acceleration;
        }

        /// <summary>
        /// Get the current acceleration for this actuator
        /// </summary>
        /// <returns></returns>
        public double getAcceleration()
        {
            return _servo.Acceleration;
        }

        /// <summary>
        /// Set the current velocity limit for this actuator
        /// </summary>
        /// <param name="velocity"></param>
        public void setVelocity(double velocity)
        {
            if (velocity >= _servo.VelocityMin && velocity < _servo.VelocityMax)
                _servo.VelocityLimit = velocity;
        }

        /// <summary>
        /// Get the current velocity limit for this actuator
        /// </summary>
        /// <returns></returns>
        public double getVelocity()
        {
            return _servo.VelocityLimit;
        }

        /// <summary>
        /// Disable this actuator
        /// </summary>
        public void disable()
        {
            _servo.Engaged = false;
        }

        /// <summary>
        /// Enable this actuator
        /// </summary>
        public void enable()
        {
            _servo.Engaged = true;
        }
    }
}