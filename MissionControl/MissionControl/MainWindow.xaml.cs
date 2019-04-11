using System;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MissionControl {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            RedLabel.Text = "127";
            GreenLabel.Text = "127";
            BlueLabel.Text = "127";
            ColorChange();
            UpdateSerialSelector();
            // Temp...
            SerialPortSelector.Text = "COM6";
        }

        /* TODO:
         * - Set HexBox to current-set RGB  (ColorChange)
         * - On HexBox change, update RGB sliders
         * - Set RGB TextBoxes to mirror sliders
         */

        private void RedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var color = (int)RedSlider.Value;
            RedLabel.Text = color.ToString();
            ColorChange();
            SetLeds();
        }

        private void GreenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            GreenLabel.Text = GreenSlider.Value.ToString();
            ColorChange();
            SetLeds();
        }

        private void BlueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            BlueLabel.Text = BlueSlider.Value.ToString();
            ColorChange();
            SetLeds();
        }

        private void ColorChange() {
            var red = (int)RedSlider.Value;
            var green = (int)GreenSlider.Value;
            var blue = (int)BlueSlider.Value;
            ColorPreviewBox.Fill = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }

        private void ChangeLed_Click(object sender, RoutedEventArgs e) {
            if (SerialPortSelector.Text == "") {
                MessageBox.Show("You need to select a serial port.");
            } else {
                SetLeds();
            }
        }
        private void UpdateSerialSelector() {
            // TODO: get list of serial ports and dump to combo-box
            // https://docs.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.getportnames?view=netframework-4.7.2
            string[] SerialPorts = SerialPort.GetPortNames();
            SerialPortSelector.Items.Clear();
            foreach (var name in SerialPorts) {
                SerialPortSelector.Items.Add(name);
            }
        }

        private void SetLeds() {
            // Set LEDs
            uint red = (uint)RedSlider.Value;
            uint green = (uint)GreenSlider.Value;
            uint blue = (uint)BlueSlider.Value;
            var LEDCount = 136;
            // Map Lower/higher range to ASCII byte
            uint lower = uint.Parse(lbox.Text);
            uint higher = uint.Parse(hbox.Text);
            // Create Serial Command as a String
            string SerialCommand = red + "," + green + "," + blue + "," + lower + "," + higher + "\n";
            if (SerialPortSelector.Text == "") {
                //MessageBox.Show("You need to select a serial port.");
            } else {
                SerialPort Serial = new SerialPort(SerialPortSelector.Text, 115200, Parity.None, 8, StopBits.One);
                Serial.Handshake = Handshake.None;
                Serial.Open();
                Serial.Write(SerialCommand);
                Serial.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            UpdateSerialSelector();
        }

        private void RedLabel_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (RedLabel.Text == "" || !RedLabel.Text.All(char.IsDigit) ) {
                RedSlider.Value = 0.0;
            } else {
                RedSlider.Value = Int32.Parse(RedLabel.Text);
            }
            ColorChange();
        }

        private void GreenLabel_OnTextChanged(object sender, TextChangedEventArgs e) {
            if (GreenLabel.Text == "" || !GreenLabel.Text.All(char.IsDigit) )
            {
                GreenSlider.Value = 0.0;
            }else
            {
                GreenSlider.Value = Int32.Parse(GreenLabel.Text);
            }
            ColorChange();
        }

        private void BlueLabel_OnTextChanged(object sender, TextChangedEventArgs e) {
            if (BlueLabel.Text == "" || !BlueLabel.Text.All(char.IsDigit) ) {
                BlueSlider.Value = 0.0;
            } else {
                BlueSlider.Value = Int32.Parse(BlueLabel.Text);
            }
            ColorChange();
        }

        private void HexBox_OnTextChanged(object sender, TextChangedEventArgs e) {
            // Handle HexColor input
            var isHexColor = Regex.Match(HexBox.Text, "^#([A-Fa-f0-9]{6})$");
            var isHexColorNoHash = Regex.Match(HexBox.Text, "^([A-Fa-f0-9]{6})$");
            if (isHexColor.Success || isHexColorNoHash.Success) {
                // It's a valid HEX color
                var s = HexBox.Text.ToCharArray();
                char[] redChar = new char[2], blueChar = new char[2], greenChar = new char[2];
                int arrayOffset = 2;
                if (s[0] == '#') {
                    arrayOffset = 1;
                }
                redChar[0] = s[2 - arrayOffset];
                redChar[1] = s[3 - arrayOffset];
                greenChar[0] = s[4 - arrayOffset];
                greenChar[1] = s[5 - arrayOffset];
                blueChar[0] = s[6 - arrayOffset];
                blueChar[1] = s[7 - arrayOffset];
                string redString = new string(redChar);
                string greenString = new string(greenChar);
                string blueString = new string(blueChar);
                uint red = Convert.ToUInt32(redString, 16);
                uint green = uint.Parse(greenString, System.Globalization.NumberStyles.HexNumber);
                uint blue = uint.Parse(blueString, System.Globalization.NumberStyles.HexNumber);
                RedLabel.Text = red.ToString();
                GreenLabel.Text = green.ToString();
                BlueLabel.Text = blue.ToString();
                RedSlider.Value = red;
                GreenSlider.Value = green;
                BlueSlider.Value = blue;
                ColorChange();
            }

        }

        private void LedPositionSlider_RangeValueChanged(object sender, RoutedEventArgs e) {
            var LedsSelectedCount = LedPositionSlider.HigherValue - LedPositionSlider.LowerValue;
            totalLabel.Text = LedsSelectedCount.ToString();
        }
    }
}
