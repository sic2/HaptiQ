using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using HapticClientAPI;
using Input_API;

using Phidgets;
using Phidgets.Events;

namespace MHTP_API
{
    // Singleton pattern
    // see http://stackoverflow.com/questions/4203634/singleton-with-parameters
    /// <summary>
    /// The MHTPsManager handles any number of MHTPs, making sure
    /// that they are configured and no error occurs. 
    /// </summary>
    public class MHTPsManager
    {
        private String _windowName;
        private String _inputClass;

        // Phidget Manager, used to handle multiple boards.
        private Manager _manager;

        private static MHTPsManager _mInstance;
        private Dictionary<UInt32, MHTP> _mhtpsDictionary;
        private Dictionary<InputIdentifier, UInt32> _inputIdentifiersToMHTPs;
        private UInt32 _nextID;

        private List<IHapticObject> _hapticObjectObservers;
        private readonly Object _syncObj = new Object();

        private List<Phidget> _devicesToBeConfigured;

        private Input _input;

        /// <summary>
        /// TimeOut for detecting the hardware (PhidgetBoards)
        /// TimeOut is expressed in seconds.
        /// </summary>
        private const int TIME_OUT = 1;

        // Note: Should never be used
        private MHTPsManager()
        {
            Helper.Logger("MHTP_API.MHTPsManager.MHTPsManager()::Constructor with no params called - INVALID");
            throw new Exception("MHTPsManager - Constructor with no params");
        }

        private MHTPsManager(String windowName, String inputClass)
        {
            _windowName = windowName;
            _inputClass = inputClass;

            _mhtpsDictionary = new Dictionary<UInt32, MHTP>();
            _inputIdentifiersToMHTPs = new Dictionary<InputIdentifier, UInt32>();
            _devicesToBeConfigured = new List<Phidget>();
            
            // Initialise list of - haptic - observers which are notified when
            // MHTP position changes or pressure inputs change 
            _hapticObjectObservers = new List<IHapticObject>();

            initialisePhidgetManager();
            // Configure MHTPs
            configure();
        }

        private void initialisePhidgetManager()
        {
            // create and setup the Phidget Manager
            _manager = new Manager();
            _manager.Attach += new AttachEventHandler(manager_Attach);
            _manager.Detach += new DetachEventHandler(manager_Detach);
            _manager.Error += new ErrorEventHandler(manager_Error);
            _manager.open();
        }

        /// <summary>
        /// Return an instance of MHTPsManager.
        /// An MHTPsManager must be created first using the Create method,
        /// otherwise an exception is thrown.
        /// </summary>
        public static MHTPsManager Instance
        {
            get
            {
                if (_mInstance == null)
                {
                    Helper.Logger("MHTP_API.MHTPsManager.Instance:: Object not created");
                    throw new Exception("MHTPsManager - Object not created");
                }
                return _mInstance;
            }
        }

        /// <summary>
        /// Creates an object of type MHTPsManager.
        /// MHTPsManager is a singleton object, so there can only be one instance for the entire program.
        /// If Create is called twice, then an Exception is thrown.
        /// 
        /// The creation of MHTPsManager includes the detection of Phidgets boards
        /// and the configuration of MHTPs.
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="inputClass"></param>
        /// <returns></returns>
        public static MHTPsManager Create(String windowName, String inputClass)
        {
            if (inputClass == null)
            {
                Helper.Logger("MHTP_API.MHTPsManager.Create:: inputClass is null");
                throw new ArgumentNullException("MHTP_API.MHTPsManager.Create:: inputClass");
            }
            if (_mInstance != null)
            {
                Helper.Logger("MHTP_API.MHTPsManager.Create:: Object already created");
                throw new Exception("MHTP_API.MHTPsManager.Create:: Object already created");
            }
            _mInstance = new MHTPsManager(windowName, inputClass);
            return _mInstance;
        }

        /// <summary>
        /// Deletes the only instance of MHTPsManager
        /// </summary>
        public void delete()
        {
            Helper.Logger("MHTP_API.MHTPsManager.delete:: Deleting MHTPsManager instance");
            foreach (KeyValuePair<UInt32, MHTP> mhtp in _mhtpsDictionary)
            {
                mhtp.Value.close();
            }

            disposePhidgetManager();
            disposeInput();
            _mInstance = null;
        }

        private void disposePhidgetManager()
        {
            _manager.Attach -= new AttachEventHandler(manager_Attach);
            _manager.Detach -= new DetachEventHandler(manager_Detach);
            _manager.Error -= new ErrorEventHandler(manager_Error);
            _manager.close();
        }

        /// <summary>
        /// Configure the MHTPs. 
        /// This method is implicitly called any time an MHTPsManager is created.
        /// </summary>
        public void configure()
        {
            Helper.Logger("MHTP_API.MHTPsManager.configure:: Configuring MHTPs");

            // Wait for the hardware devices to be attached for TIME_OUT amount of time.
            int currentSec = DateTime.Now.Second;
            int startSec = currentSec;
            while ((Math.Abs(currentSec - startSec) <= TIME_OUT))
            {
                currentSec = DateTime.Now.Second;
            }

            if (_devicesToBeConfigured.Count == 0)
            {
                Helper.Logger("MHTP_API.MHTPsManager.configure:: No devices attached and to be configured. Throwing exception");
                throw new Exception("No devices attached and/or to be configured");
            }
            // Check configuration file.
            // TODO - check all .xml files
            Configuration conf = Helper.DeserializeFromXML("test.xml");
            if (conf != null)
            {
                conf.checkConfiguration(ref _devicesToBeConfigured);
                List<Configuration> configurations = new List<Configuration>();
                configurations.Add(conf);
                createMHTPs(configurations);
            }
            startConfigurationForm();
        }

        /// <summary>
        /// Start an MHTP configuration form if there are still 
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
                createMHTPs(configurations);
                
                initialiseInput();
            }
        }

        private void createMHTPs(List<Configuration> configurations)
        {
            if (configurations != null)
            {
                foreach (Configuration configuration in configurations)
                {
                    MHTP mhtp = new MHTP(_nextID, configuration);
                    if (configuration.interfaceKitBoardAttached)
                        mhtp.Changed += new PressureInputEventHandler(mhtp_Changed);
                    addMHTP(mhtp); // Add this MHTP to MHTPsManager appropriate data structure
                }
            }
        }

        private void initialiseInput()
        {
            // Initialise the input object used to get the MHTPs positions in space
            _input = InputFactory.getInputObj(_windowName, _inputClass);
            if (_input == null)
            {
                Helper.Logger("MHTP_API.MHTPsManager.initialiseInput:: Cannot create a valid object of type " + _inputClass);
                throw new NullReferenceException("MHTP_API.MHTPsManager.initialiseInput:: Cannot create a valid object of type " + _inputClass);
            }

            // [ HACK ] - Possible to register glyph not from here? 
            foreach (KeyValuePair<uint, MHTP> entry in _mhtpsDictionary)
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
                Helper.Logger("MHTP_API.MHTPsManager.disposeInput:: Cannot dispose input of type: " + _inputClass + ". Input object is null");
            }
            else
            {
                Helper.Logger("MHTP_API.MHTPsManager.disposeInput:: Disposing input of type: " + _inputClass);
                _input.Changed -= new ChangedEventHandler(InputChanged);
                _input.dispose();
            }
        }

        /// <summary>
        /// Registers an IHapticObject.
        /// This will allow observers to be notified when:
        ///     - the position of an MHTP changes
        ///     - there is an input (via pressure sensors)
        /// </summary>
        /// <param name="hapticObject"></param>
        public void addObserver(IHapticObject hapticObject)
        {
            if (hapticObject == null)
            {
                Helper.Logger("MHTP_API.MHTPsManager.addObserver:: hapticObject is null");
                throw new ArgumentNullException("MHTP_API.MHTPsManager.addObserver:: hapticObject");
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
                Helper.Logger("MHTP_API.MHTPsManager.removeObserver:: hapticObject is null");
                throw new ArgumentNullException("MHTP_API.MHTPsManager.removeObserver:: hapticObject");
            }
            lock (_syncObj)
            {
                _hapticObjectObservers.Remove(hapticObject);
            }

        }

        // This will be called whenever the position of one of the MHTPs actuators changes.
        private void InputChanged(object sender, InputIdentifier inputIdentifier, Point point, double orientation, EventArgs e)
        {
            if (inputIdentifier == null)
            {
                Helper.Logger("MHTP_API.MHTPsManager.InputChanged:: inputIdentifier is null");
            }
            else if (!_inputIdentifiersToMHTPs.ContainsKey(inputIdentifier))
            {
                Helper.Logger("MHTP_API.MHTPsManager.InputChanged:: inputIdentifier not in dictionary");
            }
            else if (!_mhtpsDictionary.ContainsKey(_inputIdentifiersToMHTPs[inputIdentifier]))
            {
                Helper.Logger("MHTP_API.MHTPsManager.InputChanged:: MHTP not in dictionary");
            }
            else
            {
                MHTP mhtp = _mhtpsDictionary[_inputIdentifiersToMHTPs[inputIdentifier]];
                handleBehaviours(mhtp, point, orientation);
            }
        }

        private void handleBehaviours(MHTP mhtp, Point point, double orientation)
        {
            // Notify observers
            lock (_syncObj)
            {
                Parallel.ForEach(_hapticObjectObservers, observer =>
                {
                    Tuple<int, IBehaviour, IBehaviour> behaviour = observer.handleInput(point, orientation);
                    switch (behaviour.Item1)
                    {
                        case 0:
                            mhtp.addBehaviour(behaviour.Item2);
                            break;
                        case 1:
                            mhtp.removeBehaviour(behaviour.Item2);
                            break;
                        case 2:
                            // Substitute behaviours
                            mhtp.removeBehaviour(behaviour.Item3);
                            mhtp.addBehaviour(behaviour.Item2);
                            break;
                        case 3:
                            // Do nothing
                            break;
                        default:
                            Helper.Logger("MHTP_API.MHTPsManager.handleBehaviours:: observed returned unknown rule for behaviour");
                            break;
                    }
                });
            }
        }

        private void mhtp_Changed(object sender, EventArgs e, uint id, List<Tuple<DateTime, double>> list)
        {
            // Notify observers
            lock (_syncObj)
            {
                Parallel.ForEach(_hapticObjectObservers, observer => 
                    observer.handlePress());
            }
        }

        /// <summary>
        /// Registers an MHTP.
        /// Returns not updated id if mhtp could not be added.
        /// </summary>
        /// <param name="mhtp"></param>
        /// <returns> unique id assigned to MHTP </returns>
        public UInt32 addMHTP(MHTP mhtp)
        {
            if (mhtp == null)
            {
                Helper.Logger("MHTP_API.MHTPsManager.addMHTP:: mhtp is null");
                return _nextID;
            }
            else
            {
                _mhtpsDictionary.Add(_nextID, mhtp);
                _inputIdentifiersToMHTPs.Add(mhtp.configuration.inputIdentifier, _nextID);
                return _nextID++;
            }
        }

        /// <summary>
        /// Removes (unregisters) an MHTP
        /// </summary>
        /// <param name="mhtpID"></param>
        public void removeMHTP(UInt32 mhtpID)
        {
            if (!_mhtpsDictionary.ContainsKey(mhtpID))
            {
                Helper.Logger("MHTP_API.MHTPsManager.removeMHTP:: mhtp has never been added");
            }
            else
            {
                _mhtpsDictionary.Remove(mhtpID);
            }
        }

        /// <summary>
        /// Get a dictionary of all the MHTPs 
        /// with a mapping:
        ///     mhtpID -> MHTP
        /// </summary>
        /// <returns></returns>
        public Dictionary<UInt32, MHTP> getAllMHTPs()
        {
            return _mhtpsDictionary;
        }

        /// <summary>
        /// Get a MHTP given its ID
        /// </summary>
        /// <param name="mhtpID"></param>
        /// <returns></returns>
        public MHTP getMHTP(UInt32 mhtpID)
        {
            if (_mhtpsDictionary.ContainsKey(mhtpID))
                return _mhtpsDictionary[mhtpID];
            return null;
        }

        /****************************
         * Phidget Manager Callbacks
         ****************************/

        private void manager_Attach(object sender, AttachEventArgs e)
        {
            if (e.Device.GetType() == typeof(AdvancedServo) || e.Device.GetType() == typeof(InterfaceKit))
            {
                Helper.Logger("MHTP_API.MHTPsManager.manager_Attach::Device " + e.Device.Name + " (" + e.Device.SerialNumber.ToString() + ") attached");
                _devicesToBeConfigured.Add(e.Device);
            }
            else
            {
                Helper.Logger("MHTP_API.MHTPsManager.manager_Attach::Device " + e.Device.Name + " not supported (" + e.Device.SerialNumber.ToString() + ") attached");
            }
        }

        private void manager_Detach(object sender, DetachEventArgs e)
        {
            Helper.Logger("MHTP_API.MHTPsManager.manager_Detach::Device " + e.Device.Name + " (" + e.Device.SerialNumber.ToString() + ") detached");
            // TODO - remove board from list / dictionary
        }

        private void manager_Error(object sender, ErrorEventArgs e)
        {
            Helper.Logger("MHTP_API.MHTPsManager.manager_Error:: " + e.Description);
        }
    }
}
