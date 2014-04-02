using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Controls;

using Phidgets;
using Phidgets.Events;

using Input_API;

namespace HaptiQ_API
{
    public partial class ConfigurationForm : Form
    {
        private AdvancedServo _currentAdvancedServoBoard;
        private InterfaceKit _currentInterfaceKitBoard;
        private InputIdentifier _currentInputIdentifier;

        private const int MAX_NUMBER_ACTUATORS = 8;
        private Tuple<int, int>[] _actuatorsValues;

        private List<Phidget> _devicesToBeConfigured;

        private String _windowName;
        private Input _input;

        /// <summary>
        /// Initialises a configuration form
        /// TODO - refactor
        /// </summary>
        public ConfigurationForm(ref List<Phidget> devicesToBeConfigured, String windowName)
        {
            Helper.Logger("HaptiQ_API.ConfigurationForm.ConfigurationForm::Configuration Form started");
            InitializeComponent();
            this.TopMost = true;

            this._devicesToBeConfigured = devicesToBeConfigured;
            this._windowName = windowName;

            _actuatorsValues = new Tuple<int, int>[MAX_NUMBER_ACTUATORS];

            // Initialise combo boxes - XXX move to separate method
            var servoBoards = new List<int>();
            var intfKits = new List<int>();
            intfKits.Add(0); // no intfKit board
            foreach (Phidget phidget in devicesToBeConfigured)
            {
                if (phidget.GetType() == typeof(AdvancedServo))
                {
                    servoBoards.Add(phidget.SerialNumber);
                }
                else if (phidget.GetType() == typeof(InterfaceKit))
                {
                    intfKits.Add(phidget.SerialNumber);
                }
            }
            ServoBoardsComboBox.DataSource = servoBoards;
            InterfaceKitsComboBox.DataSource = intfKits; 
            ServoBoardsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            InterfaceKitsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            ServoBoardsComboBox.SelectedValueChanged += new EventHandler(ServoBoardsComboBox_SelectedValueChanged);
            _currentAdvancedServoBoard = new AdvancedServo();
            _currentAdvancedServoBoard.open((int)ServoBoardsComboBox.SelectedItem);
            _currentAdvancedServoBoard.waitForAttachment();

            InterfaceKitsComboBox.SelectedValueChanged += new EventHandler(InterfaceKitsComboBox_SelectedValueChanged);

            var inputIdentifiers = new List<String>();
            inputIdentifiers.Add("Tag");
            inputIdentifiers.Add("Glyph");
            InputIdentifiersComboBox.DataSource = inputIdentifiers;
            InputIdentifiersComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.TopMost = false;
        }

        // Check configuration is valid
        private void saveForm()
        {
            bool configurationIsValid = true;

            // Create configuration
            Configuration configuration = new Configuration();
            configuration.HaptiQName = configurationName.Text;

            configuration.idServoBoard = _currentAdvancedServoBoard.SerialNumber;
            configuration.nameServoBoard = _currentAdvancedServoBoard.Name;
            configuration.interfaceKitBoardAttached = (int)InterfaceKitsComboBox.SelectedItem == 0 ? false : true;
            if (configuration.interfaceKitBoardAttached)
            {
                configuration.idInterfaceKit = _currentInterfaceKitBoard.SerialNumber;
                configuration.nameInterfaceKit = _currentInterfaceKitBoard.Name;
            }

            configuration.numberActuators = checkedServos(configuration);
            configuration.numberPressureSensors = checkedPressureSensors(configuration);

            if (configuration.numberActuators == 0) // there must be a positive number of actuators
            {
                configurationIsValid = false;
            }

            if (_currentInputIdentifier != null)
            {
                configuration.inputIdentifier = _currentInputIdentifier;
            }
            else
            {
                configurationIsValid = false;
            }

            // Remove chosen boards from devicesToBeConfigured
            // XXX - code duplication
            // XXX - remove from comboBox too
            for (int i = _devicesToBeConfigured.Count - 1; i >= 0; i--)
            {
                if (_devicesToBeConfigured[i].GetType() == typeof(AdvancedServo) && 
                    _devicesToBeConfigured[i].SerialNumber == _currentAdvancedServoBoard.SerialNumber)
                {
                    _devicesToBeConfigured.Remove(_devicesToBeConfigured[i]);
                }
                else if (configuration.interfaceKitBoardAttached &&
                    _devicesToBeConfigured[i].GetType() == typeof(InterfaceKit) &&
                    _devicesToBeConfigured[i].SerialNumber == _currentInterfaceKitBoard.SerialNumber)
                {
                    _devicesToBeConfigured.Remove(_devicesToBeConfigured[i]);
                }
            }

            // Update color of save button 
            changeButtonColor(button1, configurationIsValid);

            ConfigurationManager.addConfiguration(configuration);
        }

        private void changeButtonColor(System.Windows.Forms.Button button, bool condition)
        {
            if (condition)
            {
                button.BackColor = System.Drawing.Color.FromArgb(0, 150, 0);
            }
            else
            {
                button.BackColor = System.Drawing.Color.FromArgb(150, 0, 0);
            }
        }

        private int checkedServos(Configuration configuration)
        {
            if (Actuator1.Checked) 
            { configuration.actuators[0] = new SerializableTuple<int,int>(Actuator1MIN.Value, Actuator1MAX.Value); }
            if (Actuator2.Checked)
            { configuration.actuators[1] = new SerializableTuple<int, int>(Actuator2MIN.Value, Actuator2MAX.Value); }
            if (Actuator3.Checked)
            { configuration.actuators[2] = new SerializableTuple<int, int>(Actuator3MIN.Value, Actuator3MAX.Value); }
            if (Actuator4.Checked)
            { configuration.actuators[3] = new SerializableTuple<int, int>(Actuator4MIN.Value, Actuator4MAX.Value); }
            if (Actuator5.Checked)
            { configuration.actuators[4] = new SerializableTuple<int, int>(Actuator5MIN.Value, Actuator5MAX.Value); }
            if (Actuator6.Checked)
            { configuration.actuators[5] = new SerializableTuple<int, int>(Actuator6MIN.Value, Actuator6MAX.Value); }
            if (Actuator7.Checked)
            { configuration.actuators[6] = new SerializableTuple<int, int>(Actuator7MIN.Value, Actuator7MAX.Value); }
            if (Actuator8.Checked)
            { configuration.actuators[7] = new SerializableTuple<int, int>(Actuator8MIN.Value, Actuator8MAX.Value); }
            return configuration.actuators.Count;
        }

        private int checkedPressureSensors(Configuration configuration)
        {
            if (Pressure1.Checked) { configuration.pressureSensors.Add(0); }
            if (Pressure2.Checked) { configuration.pressureSensors.Add(1); }
            if (Pressure3.Checked) { configuration.pressureSensors.Add(2); }
            if (Pressure4.Checked) { configuration.pressureSensors.Add(3); }
            if (Pressure5.Checked) { configuration.pressureSensors.Add(4); }
            if (Pressure6.Checked) { configuration.pressureSensors.Add(5); }
            if (Pressure7.Checked) { configuration.pressureSensors.Add(6); }
            if (Pressure8.Checked) { configuration.pressureSensors.Add(7); }
            return configuration.pressureSensors.Count;
        }

        /// <summary>
        /// Overriding OnFormClosing to force configuration to be saved
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            const string message = "Are you sure that you would like to exit the configuration form?";
            const string caption = "Cancel Configuration";
            var result = MessageBox.Show(message, caption,
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question);

            e.Cancel = (result == DialogResult.No);

            _currentAdvancedServoBoard.close();
            if (_currentInterfaceKitBoard != null)
                _currentInterfaceKitBoard.close();

            
            if (_input != null)
            {
                _input.Changed -= new ChangedEventHandler(InputChanged);
                _input.dispose();
            }
             

            if (_currentInterfaceKitBoard != null)
            {
                _currentInterfaceKitBoard.SensorChange -= new Phidgets.Events.SensorChangeEventHandler(currentInterfaceKitBoard_SensorChange);
                _currentInterfaceKitBoard.close();
            }
            

            ServoBoardsComboBox.SelectedValueChanged -= new EventHandler(ServoBoardsComboBox_SelectedValueChanged);
            InterfaceKitsComboBox.SelectedValueChanged -= new EventHandler(InterfaceKitsComboBox_SelectedValueChanged);

            // start a new configuration form until all devices are configured
            HaptiQsManager.Instance.startConfigurationForm(); 
        } 

        private void saveButtonClick(object sender, EventArgs e)
        {
            saveForm();
        }

        private void acquireInputIdentifierClick(object sender, EventArgs e)
        {
            
            InputIdentifier.TYPE typeItem = InputIdentifier.stringToType((String)InputIdentifiersComboBox.SelectedItem);
            if (_input != null)
            {
                _input.Changed -= new ChangedEventHandler(InputChanged); 
            }
            switch (typeItem)
            {
                case InputIdentifier.TYPE.tag:
                    _input = InputFactory.getInputObj(_windowName, "SurfaceInput");
                    break;
                case InputIdentifier.TYPE.glyph:
                    _input = InputFactory.getInputObj(_windowName, "SurfaceGlyphsInput");
                    break;
                default:
                    Helper.Logger("HaptiQ_API.ConfigurationForm.acquireInputIdentifierClick:: input type unknown " + typeItem);
                    break;
            }
            if (_input != null)
            {
                _input.Changed += new ChangedEventHandler(InputChanged);
            }
             
        }

        private void InputChanged(object sender, InputIdentifier inputIdentifier, Point point, double orientation, EventArgs e)
        {
            _currentInputIdentifier = inputIdentifier;
            changeButtonColor(button3, _currentInputIdentifier != null);
        }
        private void ServoBoardsComboBox_SelectedValueChanged(Object sender, EventArgs e)
        {  
            _currentAdvancedServoBoard.close();
            _currentAdvancedServoBoard.open((int)ServoBoardsComboBox.SelectedItem);
            _currentAdvancedServoBoard.waitForAttachment();
        }

        private void InterfaceKitsComboBox_SelectedValueChanged(Object sender, EventArgs e)
        {
            if ((int)InterfaceKitsComboBox.SelectedItem != 0)
            {
                if (_currentInterfaceKitBoard == null)
                {
                    _currentInterfaceKitBoard = new InterfaceKit();
                }
                else
                {
                    _currentInterfaceKitBoard.SensorChange -= new Phidgets.Events.SensorChangeEventHandler(currentInterfaceKitBoard_SensorChange);
                    _currentInterfaceKitBoard.close();
                }
                _currentInterfaceKitBoard.SensorChange += new Phidgets.Events.SensorChangeEventHandler(currentInterfaceKitBoard_SensorChange);
                _currentInterfaceKitBoard.open((int)InterfaceKitsComboBox.SelectedItem);
                _currentInterfaceKitBoard.waitForAttachment();
            }
            else if (_currentInterfaceKitBoard != null)
            {
                _currentInterfaceKitBoard.SensorChange -= new Phidgets.Events.SensorChangeEventHandler(currentInterfaceKitBoard_SensorChange);
                _currentInterfaceKitBoard.close();
            }
        }

        // Visualise pressure sensors input
        private void currentInterfaceKitBoard_SensorChange(object sender, SensorChangeEventArgs e)
        {
            switch (e.Index)
            {
                case 0:
                    progressBar1.Value = e.Value;
                    break;
                case 1:
                    progressBar2.Value = e.Value;
                    break;
                case 2:
                    progressBar3.Value = e.Value;
                    break;
                case 3:
                    progressBar4.Value = e.Value;
                    break;
                case 4:
                    progressBar5.Value = e.Value;
                    break;
                case 5:
                    progressBar6.Value = e.Value;
                    break;
                case 6:
                    progressBar7.Value = e.Value;
                    break;
                case 7:
                    progressBar8.Value = e.Value;
                    break;
                default:
                    Console.WriteLine("Sensor at index " + e.Index + " not supported");
                    break;
            }
        }

        /***********************
        * ACTUATORS CHECKBOXES
        * HANDLERS
        ************************/
        private void checkedChanged(int servo, System.Windows.Forms.CheckBox checkBox)
        {
            if (_currentAdvancedServoBoard != null)
            {
                if (checkBox.Checked)
                {
                    this._currentAdvancedServoBoard.servos[servo].Engaged = true;
                }
                else
                {
                    this._currentAdvancedServoBoard.servos[servo].Engaged = false;
                }
            }
            else
            {
                checkBox.Checked = !checkBox.Checked;
            }
        }

        private void Actuator1_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(0, Actuator1);
        }

        private void Actuator2_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(1, Actuator2);
        }

        private void Actuator3_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(2, Actuator3);
        }

        private void Actuator4_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(3, Actuator4);
        }

        private void Actuator5_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(4, Actuator5);
        }

        private void Actuator6_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(5, Actuator6);
        }

        private void Actuator7_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(6, Actuator7);
        }

        private void Actuator8_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(7, Actuator8);
        }

        /******************** 
         * ACTUATORS SCROLLS 
         * CONTROLS HANDLERS
         ********************/

        private void actuatorMINScrollHandler(int servo, TrackBar min, TrackBar max)
        {
            this._currentAdvancedServoBoard.servos[servo].Position = min.Value;
            if (_actuatorsValues[servo] == null)
                _actuatorsValues[servo] = new Tuple<int, int>(min.Value, 0);
            else if (_actuatorsValues[servo].Item1 > _actuatorsValues[servo].Item2)
                _actuatorsValues[servo] = new Tuple<int, int>(min.Value, min.Value);
            else
                _actuatorsValues[servo] = new Tuple<int, int>(min.Value, _actuatorsValues[servo].Item2);
            // Change graphics
            if (_actuatorsValues[servo].Item1 > _actuatorsValues[servo].Item2)
                max.Value = _actuatorsValues[servo].Item1;
        }

        private void actuatorMAXScrollHandler(int servo, TrackBar min, TrackBar max)
        {
            this._currentAdvancedServoBoard.servos[servo].Position = max.Value;
            if (_actuatorsValues[servo] == null)
                _actuatorsValues[servo] = new Tuple<int, int>(0, max.Value);
            else if (_actuatorsValues[servo].Item1 > _actuatorsValues[servo].Item2)
                _actuatorsValues[servo] = new Tuple<int, int>(max.Value, max.Value);
            else
                _actuatorsValues[servo] = new Tuple<int, int>(_actuatorsValues[servo].Item1, max.Value);
            // Change graphics
            if (_actuatorsValues[servo].Item1 > _actuatorsValues[servo].Item2)
                min.Value = _actuatorsValues[servo].Item2;
        }

        private void Actuator1MIN_Scroll(object sender, EventArgs e)
        {
            actuatorMINScrollHandler(0, Actuator1MIN, Actuator1MAX);
        }

        private void Actuator1MAX_Scroll(object sender, EventArgs e)
        {
            actuatorMAXScrollHandler(0, Actuator1MIN, Actuator1MAX);
        }

        private void Actuator2MIN_Scroll(object sender, EventArgs e)
        {
            actuatorMINScrollHandler(1, Actuator2MIN, Actuator2MAX);
        }

        private void Actuator2MAX_Scroll(object sender, EventArgs e)
        {
            actuatorMAXScrollHandler(1, Actuator2MIN, Actuator2MAX);
        }

        private void Actuator3MIN_Scroll(object sender, EventArgs e)
        {
            actuatorMINScrollHandler(2, Actuator3MIN, Actuator3MAX);
        }

        private void Actuator3MAX_Scroll(object sender, EventArgs e)
        {
            actuatorMAXScrollHandler(2, Actuator3MIN, Actuator3MAX);
        }

        private void Actuator4MIN_Scroll(object sender, EventArgs e)
        {
            actuatorMINScrollHandler(3, Actuator4MIN, Actuator4MAX);
        }

        private void Actuator4MAX_Scroll(object sender, EventArgs e)
        {
            actuatorMAXScrollHandler(3, Actuator4MIN, Actuator4MAX);
        }

        private void Actuator5MIN_Scroll(object sender, EventArgs e)
        {
            actuatorMINScrollHandler(4, Actuator5MIN, Actuator5MAX);
        }

        private void Actuator5MAX_Scroll(object sender, EventArgs e)
        {
            actuatorMAXScrollHandler(4, Actuator5MIN, Actuator5MAX);
        }

        private void Actuator6MIN_Scroll(object sender, EventArgs e)
        {
            actuatorMINScrollHandler(5, Actuator6MIN, Actuator6MAX);
        }

        private void Actuator6MAX_Scroll(object sender, EventArgs e)
        {
            actuatorMAXScrollHandler(5, Actuator6MIN, Actuator6MAX);
        }

        private void Actuator7MIN_Scroll(object sender, EventArgs e)
        {
            actuatorMINScrollHandler(6, Actuator7MIN, Actuator7MAX);
        }

        private void Actuator7MAX_Scroll(object sender, EventArgs e)
        {
            actuatorMAXScrollHandler(6, Actuator7MIN, Actuator7MAX);
        }

        private void Actuator8MIN_Scroll(object sender, EventArgs e)
        {
            actuatorMINScrollHandler(7, Actuator8MIN, Actuator8MAX);
        }

        private void Actuator8MAX_Scroll(object sender, EventArgs e)
        {
            actuatorMAXScrollHandler(7, Actuator8MIN, Actuator8MAX);
        }
    }
}
