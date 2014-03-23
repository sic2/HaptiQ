using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Phidgets;
using Phidgets.Events;

using Input_API;

namespace MHTP_API
{
    /***************************/
    /* DELEGATES DEFINITIONS   */
    /***************************/
 
    /// <summary>
    /// Delegate for PressureGesture events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="id">This MHTP id</param>
    /// <param name="position"></param>
    /// <param name="list">list of recent pressure values</param>
    public delegate void PressureGestureEventHandler(object sender, EventArgs e, uint id, Point position, List<Tuple<DateTime, int>> list);

    /// <summary>
    /// Delegate for PressureInput events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="id">This MHTP id</param>
    /// <param name="actuatorId"></param>
    /// <param name="pressureValue"></param>
    public delegate void PressureInputEventHandler(object sender, EventArgs e, uint id, int actuatorId, int pressureValue);

    /// <summary>
    /// Delegate for Position (and Orientation) events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="id">This MHTP id</param>
    /// <param name="position"></param>
    /// <param name="orientation"></param>
    public delegate void PositionEventHandler(object sender, EventArgs e, uint id, Point position, double orientation);

    /// <summary>
    /// Delegate for Actuator position events. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="id"></param>
    /// <param name="actuatorId"></param>
    /// <param name="position"></param>
    public delegate void ActuatorPositionEventHandler(object sender, EventArgs e, uint id, int actuatorId, double position);

    /// <summary>
    /// This class represent an MHTP.
    /// </summary>
    public class MHTP
    {
        /*
         * EVENTS
         */
        /// <summary>
        /// Notify that a relevant input was input
        /// via the pressure sensors
        /// </summary>
        public event PressureGestureEventHandler PressureGesture;

        /// <summary>
        /// Notify that the pressure input of one of the sensors
        /// has changed
        /// </summary>
        public event PressureInputEventHandler PressureInput;

        /// <summary>
        /// Notify that either the position
        /// or the orientation (or both) of this MHTP have changed.
        /// </summary>
        public event PositionEventHandler PositionChanged;

        /// <summary>
        /// Notify that the position of one of the actuators of this MHTP has changed.
        /// </summary>
        public event ActuatorPositionEventHandler ActuatorPositionChanged;
        /*
         * END EVENTS 
         */

        private Point _position;
        /// <summary>
        /// Current position of this MHTP
        /// </summary>
        public Point position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                OnPositionChanged(null);
            }
        }

        private double _orientation;
        /// <summary>
        /// Orientation is in radians
        /// </summary>
        public double orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
                OnPositionChanged(null);
            }
        } 

        private AdvancedServo _advServo;
        private static List<Actuator> _actuators;

        private InterfaceKit _intfKit;
        private const int INPUT_PRESSURE_THRESHOLD = 400;
        private const int NOISE_THRESHOLD = 20;
        // map actuator_id -> (time, pressure value)
        // data is recorded only when above the threshold.
        // When below the threshold the data is removed.
        private Dictionary<int, List<Tuple<DateTime, int>>> _inputData;
        private Dictionary<int, double> _currentPressureData;

        /// <summary>
        /// Indicates how long the MHTP waits before playing the next behaviour
        /// </summary>
        private const int BEHAVIOUR_LOOP_MS = 10;
        private readonly Object _loopLock = new Object();
        private bool _runLoop = true;
        private List<IBehaviour> _behaviours;

        private Configuration _configuration;
        /// <summary>
        /// Configuration of this MHTP
        /// </summary>
        public Configuration configuration
        {
            get { return _configuration; }
        }

        /// <summary>
        /// Unique ID assigned to this MHTP
        /// </summary>
        private uint _id;

        /// <summary>
        /// Initialise the MHTP given a configuration
        /// </summary>
        /// <param name="id">id for this MHTP. 
        /// The MHTPs manager or whoever is creating the MHTP should use an 
        /// id which is unique.</param>
        /// <param name="configuration">Configuration used by this MHTP to setup 
        /// servo and interfacekit boards.</param>
        public MHTP(uint id, Configuration configuration)
        {
            if (configuration == null)
            {
                Helper.Logger("MHTP_API.MHTP.MHTP:: id: " + id + "; configuration is null");
                throw new ArgumentNullException("MHTP_API.MHTP.MHTP:: configuration");
            }

            this._id = id;
            this._configuration = configuration;

            // Initialise data structures
            _inputData = new Dictionary<int, List<Tuple<DateTime, int>>>();
            _currentPressureData = new Dictionary<int, double>();
            _actuators = new List<Actuator>();
            _behaviours = new List<IBehaviour>();

            // Initialise boards
            initialiseServoBoard();
            initialiseInterfaceKitBoard();
           
            // Let this MHTP be an independent thread.
            // Another approach would be to have the MHTPsManager to handle all the MHTPs sequentially,
            // but then the users may lose the feeling of concurrency.
            Thread thread = new Thread(loop);
            thread.Start();
        }

        private void initialiseServoBoard()
        {
            //Declare an Advanced Servo object
            _advServo = new AdvancedServo();
            //Hook the basic event handlers
            _advServo.Attach += new AttachEventHandler(advServo_Attach);
            _advServo.Detach += new DetachEventHandler(advServo_Detach);
            _advServo.Error += new ErrorEventHandler(advServo_Error);
            _advServo.PositionChange += new PositionChangeEventHandler(advServo_PositionChange);

            _advServo.open(configuration.idServoBoard);
            Helper.Logger("MHTP_API.MHTP.MHTP::Waiting for MHTP (" + _id + ") ServoBoard(" + configuration.idServoBoard + ") to be attached.");
            _advServo.waitForAttachment();
            Helper.Logger("MHTP_API.MHTP.MHTP::MHTP (" + _id + ") ServoBoard(" + configuration.idServoBoard + ") Connected");
            setAllActuatorsToMinPosition();
        }

        private void initialiseInterfaceKitBoard()
        {
            // Create an InterfaceKit object if attached and configured for this MHTP
            if (configuration.interfaceKitBoardAttached)
            {
                _intfKit = new InterfaceKit();
                _intfKit.Attach += new AttachEventHandler(intfKit_Attach);
                _intfKit.Detach += new DetachEventHandler(intfKit_Detach);
                _intfKit.Error += new ErrorEventHandler(intfKit_Error);
                _intfKit.SensorChange += new SensorChangeEventHandler(intfKit_SensorChange);
                
                _intfKit.open(configuration.idInterfaceKit);
                Helper.Logger("MHTP_API.MHTP.MHTP::Waiting for MHTP (" + _id + ") IntfKit (" + configuration.idInterfaceKit + ") to be attached.");
                _intfKit.waitForAttachment();
                Helper.Logger("MHTP_API.MHTP.MHTP::MHTP (" + _id + ") IntfKit(" + configuration.idInterfaceKit + ") Connected");
            }
        }

        /// <summary>
        /// Cleanly dispose this object
        /// </summary>
        public void close()
        {
            disableActuators();

            lock (_loopLock)
            {
                _runLoop = false;
            }

            closeServoBoard();
            closeInterfaceKitBoard();
        }

        private void closeServoBoard()
        {
            _advServo.Attach -= new AttachEventHandler(advServo_Attach);
            _advServo.Detach -= new DetachEventHandler(advServo_Detach);
            _advServo.Error -= new ErrorEventHandler(advServo_Error);
            _advServo.PositionChange -= new PositionChangeEventHandler(advServo_PositionChange);
            _advServo.close();
            _advServo = null; //set the object to null to get it out of memory
            Helper.Logger("MHTP_API.MHTP.close::MHTP (" + _id + ") ServoBoard (" + configuration.idServoBoard + ") disconnected");
        }

        private void closeInterfaceKitBoard()
        {
            if (configuration.interfaceKitBoardAttached)
            {
                _intfKit.Attach -= new AttachEventHandler(intfKit_Attach);
                _intfKit.Detach -= new DetachEventHandler(intfKit_Detach);
                _intfKit.Error -= new ErrorEventHandler(intfKit_Error);
                _intfKit.SensorChange -= new SensorChangeEventHandler(intfKit_SensorChange);
                _intfKit.close();
                _intfKit = null;
                Helper.Logger("MHTP_API.MHTP.close::MHTP (" + _id + ") interfaceKit (" + configuration.idInterfaceKit + ") disconnected");
            }
        }

        /// <summary>
        /// Set an actuator to a given position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        public void setActuatorPosition(int index, double position)
        {
            lock (_actuators)
            {
                if (actuatorExist(index))
                    getActuator(index).setHeight(position);
            }
        }

        /// <summary>
        /// Set an actuator to a given position in percentage.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="percentage"></param>
        public void setActuatorPositionByPercentage(int index, double percentage)
        {
            lock (_actuators)
            {
                if (actuatorExist(index))
                    getActuator(index).setHeightByPercentage(percentage);
            }
        }

        /// <summary>
        /// Set a given actuator to its minimum position
        /// </summary>
        /// <param name="index"></param>
        public void setActuatorMinPosition(int index)
        {
            lock (_actuators)
            {
                if (actuatorExist(index))
                    getActuator(index).setToMinimum();
            }
        }

        /// <summary>
        /// Set a given actuator to its maximum position
        /// </summary>
        /// <param name="index"></param>
        public void setActuatorMaxPosition(int index)
        {
            lock (_actuators)
            {
                if (actuatorExist(index))
                    getActuator(index).setToMaximum();
            }
        }

        /// <summary>
        /// Set an actuator to a given position
        /// </summary>
        /// <param name="actuator"></param>
        /// <param name="position"></param>
        public void setActuatorPosition(Actuator actuator, double position)
        {
            if (actuator == null)
            {
                Helper.Logger("MHTP_API.MHTP.setActuatorPosition:: (" + _id + ") actuator is null");
                throw new ArgumentNullException("MHTP_API.MHTP.setActuatorPosition:: actuator");
            }
            else
            {
                lock (_actuators)
                {
                    actuator.setHeight(position);
                }
            }
        }

        public void setActuatorPositionByPercentage(Actuator actuator, double percentage)
        {
            if (actuator == null)
            {
                Helper.Logger("MHTP_API.MHTP.setActuatorPositionByPercentage:: (" + _id + ") actuator is null");
                throw new ArgumentNullException("MHTP_API.MHTP.setActuatorPosition:: actuator");
            }
            else
            {
                lock (_actuators)
                {
                    actuator.setHeightByPercentage(percentage);
                }
            }
        }

        /// <summary>
        /// Set a given actuator to its minimum position
        /// </summary>
        /// <param name="actuator"></param>
        public void setActuatorMinPosition(Actuator actuator)
        {
            if (actuator == null)
            {
                Helper.Logger("MHTP_API.MHTP.setActuatorMinPosition:: (" + _id + ") actuator is null");
                throw new ArgumentNullException("MHTP_API.MHTP.setActuatorMinPosition:: actuator");
            }
            else
            {
                lock (_actuators)
                {
                    actuator.setToMinimum();
                }
            }
        }

        /// <summary>
        /// Set a given actuator to its maximum position
        /// </summary>
        /// <param name="actuator"></param>
        public void setActuatorMaxPosition(Actuator actuator)
        {
            if (actuator == null)
            {
                Helper.Logger("MHTP_API.MHTP.setActuatorMaxPosition:: (" + _id + ") actuator is null");
                throw new ArgumentNullException("MHTP_API.MHTP.setActuatorMaxPosition:: actuator");
            }
            else
            {
                lock (_actuators)
                {
                    actuator.setToMaximum();
                }
            }
        }

        /// <summary>
        /// Sets all actuators of this MHTP to the specified position
        /// </summary>
        /// <param name="position"></param>
        public void setAllActuatorsPosition(double position)
        {
            lock (_actuators)
            {
                Parallel.ForEach(_actuators, actuator =>
                {
                    actuator.setHeight(position);
                });
            }
        }

        /// <summary>
        /// Set all actuators of this MHTP to their minimum positon
        /// </summary>
        public void setAllActuatorsToMinPosition()
        {
            lock (_actuators)
            {
                Parallel.ForEach(_actuators, actuator =>
                {
                    actuator.setToMinimum();
                });
            }
        }

        /// <summary>
        /// Set all actuators of this MHTP to their maximum position
        /// </summary>
        public void setAllActuatorsToMaxPosition()
        {
            lock (_actuators)
            {
                Parallel.ForEach(_actuators, actuator =>
                {
                    actuator.setToMaximum();
                });
            }
        }

        /// <summary>
        /// Get actuator specifying its index as in the Phidget
        /// AdvancedServoBoard.
        /// This method returns null if no actuator exist at that position
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Actuator getActuator(int index)
        {
            if (hasActuators() && index >= 0)
            {
                foreach (Actuator actuator in _actuators)
                {
                    if (actuator.getId() == index)
                        return actuator;
                }
            }
            return null;
        }

        /// <summary>
        /// Return true if there is an actuator at given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool actuatorExist(int index)
        {
            if (!hasActuators() || index < 0)
                return false;
            foreach (Actuator actuator in _actuators)
            {
                if (actuator.getId() == index)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a list of all actuators of this MHTP
        /// </summary>
        /// <returns></returns>
        public List<Actuator> getActuators()
        {
            return _actuators;
        }

        /// <summary>
        /// Disable this MHTP.
        /// Actuators are not engaged anymore.
        /// </summary>
        public void disableActuators()
        {
            lock (_actuators)
            {
                Helper.Logger("MHTP_API.MHTP.disable:: id: (" + _id + ") boards:( " + configuration.idServoBoard + 
                    ", " + configuration.idInterfaceKit + ") Disabling Actuators");
                Parallel.ForEach(_actuators, actuator =>
                {
                    actuator.disable();
                });
            }
        }

        /// <summary>
        /// Enable this MHTP.
        /// Actuators are engaged.
        /// </summary>
        public void enableActuators()
        {
            lock (_actuators)
            {
                Helper.Logger("MHTP_API.MHTP.enable::  id: (" + _id + ") boards:( " + configuration.idServoBoard +
                    ", " + configuration.idInterfaceKit + ") Enabling Actuators");
                Parallel.ForEach(_actuators, actuator =>
                {
                    actuator.enable();
                });
            }
        }

        private bool hasActuators()
        {
            if (_advServo == null || _actuators == null) return false;
            return _advServo.Attached && _actuators.Count > 0;
        }

        /*********************
         * Behaviours Methods
         *********************/

        /// <summary>
        /// Play current haptic behaviour
        /// </summary>
        public void loop()
        {
            while (_runLoop)
            {
                lock (_loopLock)
                {
                    playBehaviours();
                    System.Threading.Thread.Sleep(BEHAVIOUR_LOOP_MS);
                }
            }
        }

        /// <summary>
        /// Add a behaviour to this MHTP
        /// </summary>
        /// <param name="behaviour"></param>
        public void addBehaviour(IBehaviour behaviour)
        {
            lock (_behaviours)
            {
                _behaviours.Add(behaviour);
            }
        }

        /// <summary>
        /// Remove a behaviour from this MHTP
        /// </summary>
        /// <param name="behaviour"></param>
        public void removeBehaviour(IBehaviour behaviour)
        {
            lock (_behaviours)
            {
                _behaviours.Remove(behaviour);
            }
        }

        /// <summary>
        /// Play the current behaviours
        /// </summary>
        public void playBehaviours()
        {
            lock (_behaviours)
            {
                Dictionary<int, Tuple<double, int>> actuators = new Dictionary<int, Tuple<double, int>>();
                foreach(IBehaviour behaviour in _behaviours)
                {
                    Dictionary<int, double> tmp = behaviour.play(_configuration.actuators, _currentPressureData);
                    foreach (KeyValuePair<int, double> pair in tmp)
                    {
                        actuators[pair.Key] = new Tuple<double, int>
                            (actuators.ContainsKey(pair.Key) ? actuators[pair.Key].Item1 + pair.Value : pair.Value, 
                            actuators.ContainsKey(pair.Key) ? actuators[pair.Key].Item2 + 1 : 1);
                    }
                }

                if (actuators != null)
                {
                    Parallel.ForEach(actuators, entry => 
                    {
                        // Normalise actuators positions by averaging values
                        this.setActuatorPositionByPercentage(entry.Key, entry.Value.Item1 / entry.Value.Item2);
                    });
                } 
            }
        }

        /* -------------- */
        /* EVENT METHODS  */ 
        /* ---------------*/

        /// <summary>
        /// This method raises a PressureGestureEventHandler event.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="list">list of all pressure data collected for this event</param>
        private void OnPressureGesture(EventArgs e, List<Tuple<DateTime, int>> list)
        {
            if (PressureGesture != null)
            {
                PressureGesture(this, e, _id, _position, list);
            }
        }

        /// <summary>
        /// This method raises a PressureInputEventHandler event.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="actuatorId"></param>
        /// <param name="pressureValue"></param>
        private void OnPressureInput(EventArgs e, int actuatorId, int pressureValue)
        {
            if (PressureInput != null)
            {
                PressureInput(this, e, _id, actuatorId, pressureValue);
            }
        }

        /// <summary>
        /// This method raises a PositionEventHandler event.
        /// </summary>
        /// <param name="e"></param>
        private void OnPositionChanged(EventArgs e)
        {
            if (PositionChanged != null)
            {
                PositionChanged(this, e, _id, _position, _orientation);
            }
        }

        /// <summary>
        /// This method raises an ActuatorPositionChanged event.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="actuatorId"></param>
        /// <param name="position"></param>
        private void OnActuatorPositionChanged(EventArgs e, int actuatorId, double position)
        {
            if (ActuatorPositionChanged != null)
            {
                ActuatorPositionChanged(this, e, _id, actuatorId, position);
            }
        }

        /* ----------------- */
        /* PHIDGETS HANDLERS */
        /* ----------------- */

        //Attach event handler. Display serial number of the attached servo device
        private void advServo_Attach(object sender, AttachEventArgs e)
        {
            Helper.Logger("MHTP_API.MHTP.advServo_Attach::AdvancedServo " + e.Device.SerialNumber.ToString() + " attached.");
            AdvancedServo attached = (AdvancedServo)sender;

            for (int i = 0; i < attached.servos.Count; i++)
            {
                if (_configuration.actuators.ContainsKey(i))
                {
                    _actuators.Add(new Actuator(attached.servos[i], i, // servo and id
                                                _configuration.actuators[i].Item1, // Min
                                                _configuration.actuators[i].Item2)); // Max
                }
            }
        }

        //Detach event handler...Display the serial number of the detached servo device
        private void advServo_Detach(object sender, DetachEventArgs e)
        {
            Helper.Logger("MHTP_API.MHTP.advServo_Detach::AdvancedServo " + e.Device.SerialNumber.ToString() + " detached.");
        }

        //Error event handler....Display the error description to the console
        private void advServo_Error(object sender, ErrorEventArgs e)
        {
            Helper.Logger("MHTP_API.MHTP.advServo_Error::AdvancedServo Error: " + e.Description);
        }

        private void advServo_PositionChange(object sender, PositionChangeEventArgs e)
        {
            // Do not log, since this could be considered not useful information
            OnActuatorPositionChanged(null, e.Index, e.Position);
        }

        private void intfKit_Attach(object sender, AttachEventArgs e)
        {
            Helper.Logger("MHTP_API.MHTP.intfKit_Attach::InterfaceKit " + e.Device.SerialNumber.ToString() + " attached.");
        }

        private void intfKit_Detach(object sender, DetachEventArgs e)
        {
            Helper.Logger("MHTP_API.MHTP.intfKit_Detach::InterfaceKit " + e.Device.SerialNumber.ToString() + " detached.");
        }

        private void intfKit_Error(object sender, ErrorEventArgs e)
        {
            Helper.Logger("MHTP_API.MHTP.intfKit_Error::InterfaceKit Error: " + e.Description);
        }

        private void intfKit_SensorChange(object sender, SensorChangeEventArgs e)
        {
            _currentPressureData[e.Index] = e.Value;
            if (actuatorExist(e.Index)) 
            {
                getActuator(e.Index).setPressure(e.Value);
                if (e.Value > INPUT_PRESSURE_THRESHOLD)
                {
                    if (!_inputData.ContainsKey(e.Index))
                        _inputData[e.Index] = new List<Tuple<DateTime, int>>();
                    _inputData[e.Index].Add(new Tuple<DateTime, int>(DateTime.Now, e.Value));
                }
                else if (e.Value < INPUT_PRESSURE_THRESHOLD && e.Value > NOISE_THRESHOLD)
                {
                    if (_inputData.ContainsKey(e.Index))
                    {
                        OnPressureGesture(null, _inputData[e.Index]);
                        _inputData[e.Index].Clear();
                        _inputData[e.Index] = null; // deallocated list
                        _inputData.Remove(e.Index);
                    }
                }
                Helper.Logger("MHTP_API.MHTP.intfKit_SensorChange::InterfaceKit Sensors Changed (" + e.Index + "): " + e.Value);
            }

            // Fire an event for this pressure sensor
            OnPressureInput(null, e.Index, e.Value);
        }

    }
}