using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Joystick_Proxy
{
    class ControllerDevice : IEquatable<ControllerDevice>
    {
        public ControllerDevice(DirectInput di, DeviceInstance deviceInstance)
        {
            DeviceInstance = deviceInstance;
            _usbId = ProductGuidToUSBID(DeviceInstance.ProductGuid);
            Joystick = new Joystick(di, deviceInstance.InstanceGuid);

            Joystick.Properties.BufferSize = 32;
        }

        public delegate void DeviceStateUpdateHandler(object sender, DeviceStateUpdateEventArgs e);
        public event DeviceStateUpdateHandler OnStateUpdated;

        public string Name { get { return DeviceInstance.InstanceName; } }
        public string Guid { get { return DeviceInstance.InstanceGuid.ToString();  } }
        public string UsbId { get => _usbId; }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;

                UpdateState(new List<ControllerInput> { new ControllerInput(_enabled) } );
            }
        }

        private Joystick Joystick { get; set; }

        public SortedDictionary<string, JoystickUpdate> CurrentState { get => _inputStateDictionary; }

        private SortedDictionary<string, JoystickUpdate> _inputStateDictionary = new SortedDictionary<string, JoystickUpdate>();

        public DeviceInstance DeviceInstance { get; private set; }
        public bool Supported { get; internal set; }
        private string _usbId;
        private bool _enabled;
        private bool _notPollable;

        public static string ProductGuidToUSBID(Guid guid)
        {
            return Regex.Replace(guid.ToString(), @"(^....)(....).*$", "$2:$1");
        }

        public void Update() 
        {
            if (_notPollable || _enabled == false)
                return;

            try
            {
                Joystick.Poll();
            }
            catch (Exception)
            {
                _notPollable = true;
                return;
            }

            List<ControllerInput> updatedStates = new List<ControllerInput>();

            foreach (JoystickUpdate joystickUpdate in Joystick.GetBufferedData())
            {
                _inputStateDictionary[joystickUpdate.Offset.ToString()] = joystickUpdate;
                updatedStates.Add(new ControllerInput(joystickUpdate));
            }

            if (updatedStates.Count > 0)
                UpdateState(updatedStates);
        }

        private void UpdateState(List<ControllerInput> updatedStates)
        {
            DeviceStateUpdateEventArgs args = new DeviceStateUpdateEventArgs(this, updatedStates);
            OnStateUpdated?.Invoke(this, args);
        }

        public override int GetHashCode()
        {
            return DeviceInstance.InstanceGuid.GetHashCode();
        }

        public bool Equals(ControllerDevice other)
        {
            return DeviceInstance.InstanceGuid == other.DeviceInstance.InstanceGuid;
        }

        internal void Unacquire()
        {
            try { Joystick.Unacquire(); } catch (Exception) { }
        }

        internal void Acquire()
        {
            Joystick.Acquire();
        }
    }

    internal class DeviceStateUpdateEventArgs
    {
        public List<ControllerInput> UpdatedStates { get; set; }
        public ControllerDevice Device { get; set; }

        public DeviceStateUpdateEventArgs(ControllerDevice device, List<ControllerInput> updatedStates)
        {
            this.Device = device;
            this.UpdatedStates = updatedStates;
        }
    }
}
