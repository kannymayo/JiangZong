﻿/*  MaestroEasyExample:
 *    Simple example GUI for the Maestro USB Servo Controller, written in
 *    Visual C#.
 *    
 *    Features:
 *       Temporary native USB connection using Usc class
 *       Button for disabling channel 0.
 *       Button for setting target of channel 0 to 1000 us.
 *       Button for setting target of channel 0 to 2000 us.
 * 
 *  NOTE: Channel 0 should be configured as a servo channel for this program
 *  to work.  You must also connect USB and servo power, and connect a servo
 *  to channel 0.  If this program does not work, use the Maestro Control
 *  Center to check what errors are occurring.
 */

//using Pololu.Usc;
using Pololu.UsbWrapper;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Pololu.Usc.MaestroEasyExample
{

    public partial class MainWindow : Form
    {
        private ActionRecorder _ar;
        private bool _shouldPlaybackStop;
        private int _targetBindingMode;
        private SerialPort _maestroSerialPort;

        public MainWindow()
        {
            InitializeComponent();
            KeyPreview = true;

            _ar = new ActionRecorder();
            _shouldPlaybackStop = false;
            _targetBindingMode = 1;
            this.mode1RadioButton.Checked = true;
            // tcp
//            String serverAddr = "192.168.1.100";
//            int serverPort = 30015;
//            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            System.Net.IPAddress addr = System.Net.IPAddress.Parse(serverAddr);
//            System.Net.IPEndPoint remoteEP = new System.Net.IPEndPoint(addr, serverPort);
//            soc.Connect(remoteEP);

            //
//            byte[] firstMsg = System.Text.Encoding.ASCII.GetBytes("1");
//            soc.Send(firstMsg);

            // shouldn't these be in designer.cs?
            this.mode1RadioButton.CheckedChanged += new System.EventHandler(this.modeSelectRadioBtnGrp_CheckedChanged);
            this.mode2RadioButton.CheckedChanged += new System.EventHandler(this.modeSelectRadioBtnGrp_CheckedChanged);
            this.mode3RadioButton.CheckedChanged += new System.EventHandler(this.modeSelectRadioBtnGrp_CheckedChanged);

            // scan
            mainTextBox.AppendText("Scanning port names:" + Environment.NewLine);
            foreach (string p in SerialPort.GetPortNames())
            {
                mainTextBox.AppendText("    " + p + Environment.NewLine);
            }

            // configure serial connection
            _maestroSerialPort = new SerialPort(
                Constant.MaestroSerialPortName,
                Constant.MaestroSerialBaudRate);
            _maestroSerialPort.Parity = Parity.None;
            _maestroSerialPort.DataBits = 8;
            _maestroSerialPort.StopBits = StopBits.One;





        }

        void TrySetTargetOverSerial(Byte channel, UInt16 target)
        {
            try
            {
                _maestroSerialPort.Open();
                // convert to byte command codes
                byte[] commands = new byte[4];
                commands[0] = 0x84;
                commands[1] = (byte) channel;
                commands[2] = Convert.ToByte(target & 0x7F);
                commands[3] = Convert.ToByte((target >> 7) & 0x7F);

                // send command
                _maestroSerialPort.Write(commands, 0, commands.Length);

                _maestroSerialPort.Close();
            }
            catch (Exception e)
            {
                mainTextBox.AppendText(e + Environment.NewLine);
            }
        }
       
        /// <summary>
        /// Attempts to set the target (width of pulses sent) of a channel.
        /// </summary>
        /// <param name="channel">Channel number from 0 to 23.</param>
        /// <param name="target">
        ///   Target, in units of quarter microseconds.  For typical servos,
        ///   6000 is neutral and the acceptable range is 4000-8000.
        /// </param>
        void TrySetTarget(Byte channel, UInt16 target)
        {
            try
            {
                using (Usc device = connectToDevice())  // Find a device and temporarily connect.
                {
                    device.setTarget(channel, target);

                    // device.Dispose() is called automatically when the "using" block ends,
                    // allowing other functions and processes to use the device.
                }
            }
            catch (Exception exception)  // Handle exceptions by displaying them to the user.
            {
                displayException(exception);
            }
        }

        /// <summary>
        /// Connects to a Maestro using native USB and returns the Usc object
        /// representing that connection.  When you are done with the
        /// connection, you should close it using the Dispose() method so that
        /// other processes or functions can connect to the device later.  The
        /// "using" statement can do this automatically for you.
        /// </summary>
        Usc connectToDevice()
        {
            // Get a list of all connected devices of this type.
            List<DeviceListItem> connectedDevices = Usc.getConnectedDevices();

            foreach (DeviceListItem dli in connectedDevices)
            {
                // If you have multiple devices connected and want to select a particular
                // device by serial number, you could simply add a line like this:
                //   if (dli.serialNumber != "00012345"){ continue; }

                Usc device = new Usc(dli); // Connect to the device.
                return device;             // Return the device.
            }
            throw new Exception("Could not find device.  Make sure it is plugged in to USB " +
                "and check your Device Manager (Windows) or run lsusb (Linux).");
        }

        /// <summary>
        /// Displays an exception to the user by popping up a message box.
        /// </summary>
        void displayException(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            do
            {
                stringBuilder.Append(exception.Message + "  ");
                if (exception is Win32Exception)
                {
                    stringBuilder.Append("Error code 0x" + ((Win32Exception)exception).NativeErrorCode.ToString("x") + ".  ");
                }
                exception = exception.InnerException;
            }
            while (exception != null);
            MessageBox.Show(stringBuilder.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // start recording
                case Keys.R:
                    mainTextBox.AppendText("Recording started..." + Environment.NewLine);
                    _ar.startRecording();
                    _ar.record(DateTime.Now, e.KeyCode);
                    break;

                // end recording
                case Keys.T:
                    _ar.record(DateTime.Now, e.KeyCode);
                    _ar.endRecording();
                    mainTextBox.AppendText("Recording ended..." + Environment.NewLine);
                    // immediatly playback
                    mainTextBox.AppendText("Playback started..." + Environment.NewLine);
                    handPlayback(_ar, _targetBindingMode);
                    break;

                // record time and key pressed
                case Keys.Q:
                case Keys.W:
                case Keys.E:
                    mainTextBox.AppendText("Key press detected: " + e.KeyCode.ToString() + Environment.NewLine);
                    _ar.record(DateTime.Now, e.KeyCode);
                    handExec(e.KeyCode, _targetBindingMode);
                    break;
                // any other keys, just ignore
                default:
                    break;
            }
        }

        private void handExec(Keys keyPressed, int targetBindingMode)
        {
            // translate mode & keypress to indices
            int idx_keyPressed = 0;
            int idx_targetBindingMode;
            if (keyPressed == Keys.Q)
            {
                idx_keyPressed = 0;
            }
            if (keyPressed == Keys.W)
            {
                idx_keyPressed = 1;
            }
            if (keyPressed == Keys.E)
            {
                idx_keyPressed = 2;
            }
            idx_targetBindingMode = targetBindingMode - 1;

            // send commands to 10 fingers
            for (int i = 10; i < 20; i++)
            {
                TrySetTargetOverSerial((byte)i, (ushort)Constant.arTargetMKF[idx_targetBindingMode, idx_keyPressed, i-10]);
            }
        }
        private void handPlayback(ActionRecorder ar, int mode)
        {
            HandFrame hf;
            while (!_shouldPlaybackStop)
            {
                hf = ar.getFrame();
                // if the newly obtained frame is a valid one, reply it
                if ( hf != null)
                {
                    handExec(hf.action, mode);
                    Thread.Sleep(hf.relTime);
                }
                // if not invalid, aka we've looped beyond the last valid frame - which also means,
                // a full iteration was completed, continue with the while loop, so that playback 
                // will automatically replay
            }
        }

        private void stopPlayback_Click(object sender, EventArgs e)
        {
            _shouldPlaybackStop = true;
        }

        private void modeSelectRadioBtnGrp_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            // Filter events fired by "uncheck"
            if (radioButton.Checked)
            {
                switch (radioButton.Name)
                {
                    case ("mode1RadioButton"):
                        mainTextBox.AppendText("mode1 selected" + Environment.NewLine);
                        _targetBindingMode = 1;
                        break;
                    case ("mode2RadioButton"):
                        mainTextBox.AppendText("mode2 selected" + Environment.NewLine);
                        _targetBindingMode = 2;
                        break;
                    case ("mode3RadioButton"):
                        mainTextBox.AppendText("mode3 selected" + Environment.NewLine);
                        _targetBindingMode = 3;
                        break;
                    default:
                        // do nothing
                        break;
                }
            }
        }
    }

    public class Constant
    {
        public static readonly string MaestroSerialPortName = "COM1";

        public static readonly int MaestroSerialBaudRate = 9600;
        /**
         * 3D array that conatains targets for
         *      3 Modes
         *      3 Keypresses
         *      10 fingers
         * Accessed by arTargetMKF[x][y][z] while
         *      x being the mode,
         *      y being the keypress,
         *      z being the finger
         */
        public static readonly int[,,] arTargetMKF = new int[3, 3, 10]
        {
            {
                // Mode 1, Keypress 1
                {
                    1000,
                    2000,
                    3000,
                    4000,
                    5000,
                    6000,
                    7000,
                    7000,
                    7000,
                    7000
                },
                // Mode 1, Keypress 2
                {
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000
                },
                // Mode 1, Keypress 3
                {
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000
                }
            },
            {
                // Mode 2, Keypress 1
                {
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000
                },
                // Mode 2, Keypress 2
                {
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000
                },
                // Mode 2, Keypress 3
                {
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000
                }
            },
            {
                // Mode 3, Keypress 1
                {
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000
                },
                // Mode 3, Keypress 2
                {
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000
                },
                // Mode 3, Keypress 3
                {
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000,
                    7000
                }
            }
        };
    }
}
