using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Input_API;

using Phidgets;
using Phidgets.Events;
using System.IO;

namespace HaptiQ_API
{
    /// <summary>
    /// Delegate for PressureInput events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void HaptiQPressureInputEventHandler(object sender, PressureInputArgs args);

    /// <summary>
    /// Delegate for Position (and Orientation) events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void HaptiQPositionEventHandler(object sender, HaptiQPositionArgs args);

    /// <summary>
    /// Delegate for Actuator position events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void HaptiQActuatorPositionEventHandler(object sender, ActuatorPositionArgs args);


    /// <summary>
    /// Behaviour enum for the rules used when adding new/removing behaviours
    /// </summary>
    public enum BEHAVIOUR_RULES { 
        /// <summary>
        /// Adds a behaviour
        /// </summary>
        ADD, 
        /// <summary>
        /// Removes a behaviour
        /// </summary>
        REMOVE, 
        /// <summary>
        /// Substitute a behaviour with another one
        /// </summary>
        SUBS, 
        /// <summary>
        /// Do nothing
        /// </summary>
        NOPE };

    // Singleton pattern
    // see http://stackoverflow.com/questions/4203634/singleton-with-parameters
    /// <summary>
    /// The HaptiQsManager handles any number of HaptiQs, making sure
    /// that they are configured and no error occurs. 
    /// 
    /// Also, the HaptiQsManager automatically subscribes to HaptiQs events:
    /// - PressureInputEventHandler
    /// - PositionEventHandler
    /// So, a client application need to subscribe only to the events:
    /// - HaptiQPressureInputEventHandler
    /// - HaptiQPositionEventHandler
    /// to get information about any HaptiQ. 
    /// However, the API also allows users to subscribe to these events
    /// from the HaptiQ object themselves.
    /// In this latter case, it is suggested to call #removeHaptiQsEventsHandlers
    /// Call #addHaptiQsEventsHandlers to restore it.
    /// </summary>
    public class HaptiQsManager
    {
        /*
        * EVENTS - fired whenever an HaptiQ changes position, orientation or pressure input values
        */
        /// <summary>
        /// Pressure event
        /// </summary>
        public event HaptiQPressureInputEventHandler PressureInput;
        /// <summary>
        /// Position and orientation event
        /// </summary>
        public event HaptiQPositionEventHandler PositionChanged;
        /// <summary>
        /// Actuator position event
        /// </summary>
        public event HaptiQActuatorPositionEventHandler ActuatorPositionChanged;
        /*
        * END EVENTS 
        */

        private String _windowName;
        private String _inputClass;

        // Phidget Manager, used to handle multiple boards.
        private Manager _manager;

        private static HaptiQsManager _mInstance;
        private Dictionary<UInt32, HaptiQ> _HaptiQsDictionary;
        private Dictionary<InputIdentifier, UInt32> _inputIdentifiersToHaptiQs;
        private UInt32 _nextID;

        private List<IHapticObject> _hapticObjectObservers;
        private readonly Object _syncObj = new Object(); // hold lock on this object for _hapticObjectObservers list

        private List<IHapticObject> _selectedHapticObjects;

        private const String CURRENT_DIR = ".";
        private List<Phidget> _devicesToBeConfigured;

        private Input _input;

        /// <summary>
        /// TimeOut for detecting the hardware (PhidgetBoards)
        /// TimeOut is expressed in seconds.
        /// </summary>
        private const int TIME_OUT = 1;

        // Note: Should never be used
        private HaptiQsManager()
        {
            Helper.Logger("HaptiQ_API.HaptiQsManager.HaptiQsManager()::Constructor with no params called - INVALID");
            throw new Exception("HaptiQsManager - Constructor with no params");
        }

        private HaptiQsManager(String windowName, String inputClass)
        {
            _windowName = windowName;
            _inputClass = inputClass;

            _HaptiQsDictionary = new Dictionary<UInt32, HaptiQ>();
            _inputIdentifiersToHaptiQs = new Dictionary<InputIdentifier, UInt32>();
            _devicesToBeConfigured = new List<Phidget>();
            
            // Initialise list of - haptic - observers which are notified when
            // HaptiQ position changes or pressure inputs change 
            _hapticObjectObservers = new List<IHapticObject>();
            // List of haptic objects selected by OnTouchEnter in HapticShape
            // or by other means for other types of IHapticObjects
            _selectedHapticObjects = new List<IHapticObject>();

            initialisePhidgetManager();
            // Configure HaptiQs
            configure();
        }

        private void initialisePhidgetManager()
        {
            // create and setup the Phidget Manager
            _manager = new Manager();
            _manager.Attach += new AttachEventHandler(manager_Attach);
            _manager.Detach += new DetachEventHandler(manager_Detach);
            _manager.Error += new Phidgets.Events.ErrorEventHandler(manager_Error);
            _manager.open();
        }

        /// <summary>
        /// Return an instance of HaptiQsManager.
        /// An HaptiQsManager must be created first using the Create method,
        /// otherwise an exception is thrown.
        /// </summary>
        public static HaptiQsManager Instance
        {
            get
            {
                if (_mInstance == null)
                {
                    Helper.Logger("HaptiQ_API.HaptiQsManager.Instance:: Object not created");
                    throw new Exception("HaptiQsManager - Object not created");
                }
                return _mInstance;
            }
        }

        /// <summary>
        /// Creates an object of type HaptiQsManager.
        /// HaptiQsManager is a singleton object, so there can only be one instance for the entire program.
        /// If Create is called twice, then an Exception is thrown.
        /// 
        /// The creation of HaptiQsManager includes the detection of Phidgets boards
        /// and the configuration of HaptiQs.
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="inputClass"></param>
        /// <returns></returns>
        public static HaptiQsManager Create(String windowName, String inputClass)
        {
            if (inputClass == null)
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.Create:: inputClass is null");
                throw new ArgumentNullException("HaptiQ_API.HaptiQsManager.Create:: inputClass");
            }
            if (_mInstance != null)
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.Create:: Object already created");
                throw new Exception("HaptiQ_API.HaptiQsManager.Create:: Object already created");
            }
            _mInstance = new HaptiQsManager(windowName, inputClass);
            return _mInstance;
        }

        /// <summary>
        /// Deletes the only instance of HaptiQsManager
        /// </summary>
        public void delete()
        {
            Helper.Logger("HaptiQ_API.HaptiQsManager.delete:: Deleting HaptiQsManager instance");
            foreach (KeyValuePair<UInt32, HaptiQ> HaptiQ in _HaptiQsDictionary)
            {
                HaptiQ.Value.close();
            }

            disposePhidgetManager();
            disposeInput();
            _mInstance = null;
        }

        private void disposePhidgetManager()
        {
            _manager.Attach -= new AttachEventHandler(manager_Attach);
            _manager.Detach -= new DetachEventHandler(manager_Detach);
            _manager.Error -= new Phidgets.Events.ErrorEventHandler(manager_Error);
            _manager.close();
        }

        /// <summary>
        /// Configure the HaptiQs.
        /// This method is implicitly called any time an HaptiQsManager is created.
        /// </summary>
        public void configure()
        {
            Helper.Logger("HaptiQ_API.HaptiQsManager.configure:: Configuring HaptiQs");

            // Wait for the hardware devices to be attached for TIME_OUT amount of time.
            int currentSec = DateTime.Now.Second;
            int startSec = currentSec;
            while ((Math.Abs(currentSec - startSec) <= TIME_OUT))
            {
                currentSec = DateTime.Now.Second;
            }

            if (_devicesToBeConfigured.Count == 0)
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.configure:: No devices attached and to be configured. Throwing exception");
                throw new Exception("No devices attached and/or to be configured");
            }

            // Check all configuration files in current directory
            List<Configuration> configurations = new List<Configuration>();
            DirectoryInfo dir = new DirectoryInfo(CURRENT_DIR);
            foreach (var file in dir.GetFiles(Configuration.CONFIGURATION_FILENAME_PATTERN))
            {
                Configuration conf = Helper.DeserializeFromXML(file.Name);
                if (conf != null && conf.checkConfiguration(ref _devicesToBeConfigured))
                {
                    configurations.Add(conf);
                }
            }
            createHaptiQs(configurations);
            startConfigurationForm();
        }

        /// <summary>
        /// Start an HaptiQ configuration form if there are still 
        /// Phidget devices to be configured
        /// </summary>
        public void startConfigurationForm()
        {
            if (_devicesToBeConfigured.Count != 0)
            {
                new ConfigurationForm(ref _devicesToBeConfigured, _windowName).Show();
            }
            else
            {
                List<Configuration> configurations = ConfigurationManager.getConfigurations();
                createHaptiQs(configurations);
                initialiseInput();
            }
        }

        private void createHaptiQs(List<Configuration> configurations)
        {
            if (configurations != null)
            {
                foreach (Configuration configuration in configurations)
                {
                    HaptiQ haptiQ = new HaptiQ(_nextID, configuration);
                    addHaptiQ(haptiQ); // Add this HaptiQ to HaptiQsManager appropriate data structure
                }
            }
        }

        private void initialiseInput()
        {
            // Initialise the input object used to get the HaptiQs positions in space
            _input = InputFactory.getInputObj(_windowName, _inputClass);
            if (_input == null)
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.initialiseInput:: Cannot create a valid object of type " + _inputClass);
                throw new NullReferenceException("HaptiQ_API.HaptiQsManager.initialiseInput:: Cannot create a valid object of type " + _inputClass);
            }
            else
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.initialiseInput:: Input object created");
            }

            // [ HACK ] - Possible to register glyph not from here? 
            foreach (KeyValuePair<uint, HaptiQ> entry in _HaptiQsDictionary)
            {
                if (entry.Value.configuration.inputIdentifier.getType() == InputIdentifier.TYPE.glyph)
                {
                    GlyphsInput.registerGlyph(InputIdentifier.intToBinaryArray(entry.Value.configuration.inputIdentifier.getID(), entry.Value.configuration.inputIdentifier.getDim()));
                }
            }
 
            _input.Changed += new ChangedEventHandler(InputChanged);
        }

        private void disposeInput()
        {
            if (_input == null)
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.disposeInput:: Cannot dispose input of type: " + _inputClass + ". Input object is null");
            }
            else
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.disposeInput:: Disposing input of type: " + _inputClass);
                _input.Changed -= new ChangedEventHandler(InputChanged);
                _input.dispose();
            }
        }

        /// <summary>
        /// Registers an IHapticObject.
        /// This will allow observers to be notified when:
        ///     - the position of an HaptiQ changes
        ///     - there is an input (via pressure sensors)
        /// </summary>
        /// <param name="hapticObject"></param>
        public void addObserver(IHapticObject hapticObject)
        {
            if (hapticObject == null)
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.addObserver:: hapticObject is null");
                throw new ArgumentNullException("HaptiQ_API.HaptiQsManager.addObserver:: hapticObject");
            }
            lock (_syncObj)
            {
                _hapticObjectObservers.Add(hapticObject); 
            }
        }

        /// <summary>
        /// Unregisters an IHapticObject
        /// </summary>
        /// <param name="hapticObject"></param>
        public void removeObserver(IHapticObject hapticObject)
        {
            if (hapticObject == null)
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.removeObserver:: hapticObject is null");
                throw new ArgumentNullException("HaptiQ_API.HaptiQsManager.removeObserver:: hapticObject");
            }
            lock (_syncObj)
            {
                _hapticObjectObservers.Remove(hapticObject);
            }

        }

        /// <summary>
        /// Return all haptic objects registered with the HaptiQsManager
        /// </summary>
        /// <returns></returns>
        public List<IHapticObject> getAllObservers()
        {
            return _hapticObjectObservers;
        }

        /// <summary>
        /// Add this haptic object to the list of selected ones
        /// </summary>
        /// <param name="hapticObject"></param>
        public void selectObject(IHapticObject hapticObject)
        {
            lock (_selectedHapticObjects)
            {
                _selectedHapticObjects.Add(hapticObject);
            }
        }

        /// <summary>
        /// Remove this object from the list of selected objects
        /// </summary>
        /// <param name="hapticObject"></param>
        public void deselectObject(IHapticObject hapticObject)
        {
            lock (_selectedHapticObjects)
            {
                _selectedHapticObjects.Remove(hapticObject);
            }
        }

        /// <summary>
        /// Get the list of selected objects
        /// </summary>
        /// <returns></returns>
        public List<IHapticObject> getSelectedObjects()
        {
            return _selectedHapticObjects;
        }

        // This will be called whenever the position of one of the HaptiQs actuators changes.
        private void InputChanged(object sender, InputArgs args)
        {
            if (args.InputIdentifier == null)
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.InputChanged:: inputIdentifier is null");
            }
            else if (!_inputIdentifiersToHaptiQs.ContainsKey(args.InputIdentifier))
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.InputChanged:: inputIdentifier not in dictionary");
            }
            else if (!_HaptiQsDictionary.ContainsKey(_inputIdentifiersToHaptiQs[args.InputIdentifier]))
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.InputChanged:: HaptiQ not in dictionary");
            }
            else
            {
                HaptiQ haptiQ = _HaptiQsDictionary[_inputIdentifiersToHaptiQs[args.InputIdentifier]];
                haptiQ.position = args.Position;
                haptiQ.orientation = args.Orientation;
                handleBehaviours(haptiQ, args.Position, args.Orientation);
            }
        }

        private void handleBehaviours(HaptiQ haptiQ, Point point, double orientation)
        {
            // Notify observers
            lock (_syncObj)
            {
                Parallel.ForEach(_hapticObjectObservers, observer =>
                {
                    Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> behaviour = observer.handleInput(haptiQ);
                    switch (behaviour.Item1)
                    {
                        case BEHAVIOUR_RULES.ADD:
                            haptiQ.addBehaviour(behaviour.Item2);
                            break;
                        case BEHAVIOUR_RULES.REMOVE:
                            haptiQ.removeBehaviour(behaviour.Item2);
                            break;
                        case BEHAVIOUR_RULES.SUBS:
                            // Substitute behaviours
                            haptiQ.removeBehaviour(behaviour.Item3);
                            haptiQ.addBehaviour(behaviour.Item2);
                            break;
                        case BEHAVIOUR_RULES.NOPE:
                            // Do nothing
                            break;
                        default:
                            Helper.Logger("HaptiQ_API.HaptiQsManager.handleBehaviours:: observed returned unknown rule for behaviour");
                            break;
                    }
                });
            }
        }

        /// <summary>
        /// Notifies haptic observers about a relevant pressure input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void pressureGestureHaptiQChanged(object sender, PressureGestureArgs args)
        {
            // Notify observers
            lock (_syncObj)
            {
                Parallel.ForEach(_hapticObjectObservers, observer =>
                    observer.handlePress(_HaptiQsDictionary.ContainsKey(args.ID) ? _HaptiQsDictionary[args.ID] : null, args.GestureType));
            }
        }

        /// <summary>
        /// Registers an HaptiQ.
        /// Returns not updated id if HaptiQ could not be added.
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <returns> unique id assigned to HaptiQ </returns>
        public UInt32 addHaptiQ(HaptiQ haptiQ)
        {
            if (haptiQ == null)
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.addHaptiQ:: HaptiQ is null");
                return _nextID;
            }
            else
            {
                haptiQ.PositionChanged += new PositionEventHandler(HaptiQ_PositionChanged);
                haptiQ.PressureInput += new PressureInputEventHandler(HaptiQ_PressureInput);
                haptiQ.ActuatorPositionChanged += new ActuatorPositionEventHandler(HaptiQ_ActuatorPositionChanged);
                haptiQ.PressureGesture += new PressureGestureEventHandler(pressureGestureHaptiQChanged);

                _HaptiQsDictionary.Add(_nextID, haptiQ);
                _inputIdentifiersToHaptiQs.Add(haptiQ.configuration.inputIdentifier, _nextID);
                return _nextID++;
            }
        }

        /// <summary>
        /// Removes (unregisters) an HaptiQ
        /// </summary>
        /// <param name="HaptiQID"></param>
        public void removeHaptiQ(UInt32 HaptiQID)
        {
            if (!_HaptiQsDictionary.ContainsKey(HaptiQID))
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.removeHaptiQ:: HaptiQ has never been added");
            }
            else
            {
                _HaptiQsDictionary[HaptiQID].PositionChanged -= new PositionEventHandler(HaptiQ_PositionChanged);
                _HaptiQsDictionary[HaptiQID].PressureInput -= new PressureInputEventHandler(HaptiQ_PressureInput);
                _HaptiQsDictionary[HaptiQID].ActuatorPositionChanged -= new ActuatorPositionEventHandler(HaptiQ_ActuatorPositionChanged);
                _HaptiQsDictionary[HaptiQID].PressureGesture -= new PressureGestureEventHandler(pressureGestureHaptiQChanged);

                _HaptiQsDictionary.Remove(HaptiQID);
            }
        }

        /// <summary>
        /// Get a dictionary of all the HaptiQs 
        /// with a mapping:
        ///     HaptiQID -> HaptiQ
        /// </summary>
        /// <returns></returns>
        public Dictionary<UInt32, HaptiQ> getAllHaptiQs()
        {
            return _HaptiQsDictionary;
        }

        /// <summary>
        /// Get a HaptiQ given its ID
        /// </summary>
        /// <param name="HaptiQID"></param>
        /// <returns></returns>
        public HaptiQ getHaptiQ(UInt32 HaptiQID)
        {
            if (_HaptiQsDictionary.ContainsKey(HaptiQID))
                return _HaptiQsDictionary[HaptiQID];
            return null;
        }

        /// <summary>
        /// Subscribe to the Position and Pressure events for all HaptiQs
        /// </summary>
        public void addHaptiQsEventsHandlers()
        {
            foreach (KeyValuePair<UInt32, HaptiQ> entry in _HaptiQsDictionary)
            {
                entry.Value.PositionChanged += new PositionEventHandler(HaptiQ_PositionChanged);
                entry.Value.PressureInput += new PressureInputEventHandler(HaptiQ_PressureInput);
            }
        }

        /// <summary>
        /// Unsubscribe to the Position and Pressure events for all HaptiQs
        /// </summary>
        public void removeHaptiQsEventsHandlers()
        {
            foreach (KeyValuePair<UInt32, HaptiQ> entry in _HaptiQsDictionary)
            {
                entry.Value.PositionChanged -= new PositionEventHandler(HaptiQ_PositionChanged);
                entry.Value.PressureInput -= new PressureInputEventHandler(HaptiQ_PressureInput);
            }
        }

        /********************/
        /* Manage HaptiQs   */
        /* position and     */    
        /* pressure events  */
        /********************/
        private void HaptiQ_PositionChanged(object sender, HaptiQPositionArgs args)
        {
            if (PositionChanged != null)
            {
                PositionChanged(this, args);
            }
        }

        private void HaptiQ_PressureInput(object sender, PressureInputArgs args)
        {
            if (PressureInput != null)
            {
                PressureInput(this, args);
            }
        }

        private void HaptiQ_ActuatorPositionChanged(object sender, ActuatorPositionArgs args)
        {
            if (ActuatorPositionChanged != null)
            {
                ActuatorPositionChanged(this, args);
            }
        }

        /****************************
         * Phidget Manager Callbacks
         ****************************/

        private void manager_Attach(object sender, AttachEventArgs e)
        {
            if (e.Device.GetType() == typeof(AdvancedServo) || e.Device.GetType() == typeof(InterfaceKit))
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.manager_Attach::Device " + e.Device.Name + " (" + e.Device.SerialNumber.ToString() + ") attached");
                // TODO - do not add if device already configured
                _devicesToBeConfigured.Add(e.Device);
            }
            else
            {
                Helper.Logger("HaptiQ_API.HaptiQsManager.manager_Attach::Device " + e.Device.Name + " not supported (" + e.Device.SerialNumber.ToString() + ") attached");
            }
        }

        private void manager_Detach(object sender, DetachEventArgs e)
        {
            Helper.Logger("HaptiQ_API.HaptiQsManager.manager_Detach::Device " + e.Device.Name + " (" + e.Device.SerialNumber.ToString() + ") detached");
            if (_devicesToBeConfigured.Contains(e.Device))
            {
                _devicesToBeConfigured.Remove(e.Device);
            }
        }

        private void manager_Error(object sender, Phidgets.Events.ErrorEventArgs e)
        {
            Helper.Logger("HaptiQ_API.HaptiQsManager.manager_Error:: " + e.Description);
        }
    }
}
