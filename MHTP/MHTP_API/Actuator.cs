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
        private int _pressure;
        private int _id; // used to identify which actuator is of the MHTP
        private double _minPosition;
        private double _maxPosition;

        private AdvancedServoServo _servo;
        private bool _enabled;

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
            _pressure = 0;
            _servo.Engaged = _enabled = true;
            setMinPosition(minPosition);
            setMaxPosition(maxPosition);
         }

        /// <summary>
        /// Return -1 if position cannot be retrieved
        /// </summary>
        public double getHeight()
        {
            double h = -1;
            try
            {
                h = _servo.Position;
            }
            catch (PhidgetException e)
            {
                Helper.Logger("MHTP_API.Actuator.height.get:: (" + _id + ")" +
                            "PhidgetException " + e.Description +
                            ". Cannot get height from servo motor.");
            }
            return h;
        }

        /// <summary>
        /// It is not possible to set an actuator to an height that does not 
        /// satisfies the min/max constraints.
        /// </summary>
        /// <param name="height"></param>
        internal void setHeight(double height)
        {
            if (_enabled && _servo != null && height >= _minPosition && height <= _maxPosition)
            {
                enable();
                _servo.Position = height;
            }
        }

        /// <summary>
        /// Set height by percentage
        /// </summary>
        /// <param name="percentage">Value between 0 and 1</param>
        /// <returns></returns>
        internal void setHeightByPercentage(double percentage)
        {
            if (percentage < 0) percentage = 0.0;
            if (percentage > 1) percentage = 1.0;
            setHeight(_minPosition + (_maxPosition - _minPosition) * percentage);
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
        public int pressure
        {
            get { return _pressure; }
        }

        /// <summary>
        /// Set the current pressure of this actuator.
        /// Note: This function should not be called from other classes other than MHTP
        /// </summary>
        /// <param name="pressure"></param>
        public void setPressure(int pressure)
        {
            _pressure = pressure;
        }

        /// <summary>
        /// Set this actuator to its minimum allowed position
        /// </summary>
        public void setToMinimum()
        {
            setHeight(_minPosition);
        }

        /// <summary>
        /// Set this actuator to its maximum allowed position
        /// </summary>
        public void setToMaximum()
        {
            setHeight(_maxPosition);
        }

        /// <summary>
        /// Set the maximum position allowed for this actuator
        /// </summary>
        /// <param name="maxPosition"></param>
        protected void setMaxPosition(double maxPosition)
        {
            _maxPosition = maxPosition < _servo.PositionMax ? maxPosition : _servo.PositionMax;
        }

        /// <summary>
        /// Set the minimum position allowed for this actuator
        /// </summary>
        /// <param name="minPosition"></param>
        protected void setMinPosition(double minPosition)
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
            if (_enabled)
                _servo.Engaged = _enabled = false;
        }

        /// <summary>
        /// Enable this actuator
        /// </summary>
        public void enable()
        {
            if (!_enabled)
                _servo.Engaged = _enabled = true;
        }
    }
}