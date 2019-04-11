// Mission Control for Quasar LED system
// "MainWindow.xaml.cs"
// Author: Edward J. Green
/*
Copyright 2019 Edward J. Green

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/* TODO:
 * - better sliders (make them dials somehow)
 * - use ini file for setting number of LEDs and last used serial port
 * - make a settings window popup
 */

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

        private void RedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var color = (int)RedSlider.Value;
            RedLabel.Text = color.ToString();
            ColorChange();
            AutoSetLeds();
        }

        private void GreenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            GreenLabel.Text = GreenSlider.Value.ToString();
            ColorChange();
            AutoSetLeds();
        }

        private void BlueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            BlueLabel.Text = BlueSlider.Value.ToString();
            ColorChange();
            AutoSetLeds();
        }

        private void ColorChange() {
            var red = (int)RedSlider.Value;
            var green = (int)GreenSlider.Value;
            var blue = (int)BlueSlider.Value;
            ColorPreviewBox.Fill = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
            HexBox.Text = "#" + red.ToString("X") + green.ToString("X") + blue.ToString("X");
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

        private void AutoSetLeds() {
            // Set LEDs
            if (AutoUpdateLeds == null) {
                return;
            }
            if (AutoUpdateLeds.IsChecked == true) {
                uint red = (uint) RedSlider.Value;
                uint green = (uint) GreenSlider.Value;
                uint blue = (uint) BlueSlider.Value;
                var LEDCount = 136;
                // Map Lower/higher range to ASCII byte
                uint lower = uint.Parse(lbox.Text);
                uint higher = uint.Parse(hbox.Text);
                // Create Serial Command as a String
                string serialCommand = red + "," + green + "," + blue + "," + lower + "," + higher + "\n";
                if (SerialPortSelector.Text == "") {
                    //MessageBox.Show("You need to select a serial port.");
                } else {
                    SerialPort Serial = new SerialPort(SerialPortSelector.Text, 115200, Parity.None, 8, StopBits.One);
                    Serial.Handshake = Handshake.None;
                    Serial.Open();
                    Serial.Write(serialCommand);
                    Serial.Close();
                }
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
            string serialCommand = red + "," + green + "," + blue + "," + lower + "," + higher + "\n";
            if (SerialPortSelector.Text == "") {
                return;
            } else {
                SerialPort Serial = new SerialPort(SerialPortSelector.Text, 115200, Parity.None, 8, StopBits.One);
                Serial.Handshake = Handshake.None;
                Serial.Open();
                Serial.Write(serialCommand);
                Serial.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            UpdateSerialSelector();
        }

        private void RedLabel_OnTextChanged(object sender, TextChangedEventArgs e) {
            if (RedLabel.Text == "" || !RedLabel.Text.All(char.IsDigit) ) {
                RedSlider.Value = 0.0;
            } else {
                RedSlider.Value = Int32.Parse(RedLabel.Text);
            }
            ColorChange();
        }

        private void GreenLabel_OnTextChanged(object sender, TextChangedEventArgs e) {
            if (GreenLabel.Text == "" || !GreenLabel.Text.All(char.IsDigit) ) {
                GreenSlider.Value = 0.0;
            } else {
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
                AutoSetLeds();
            }

        }

        private void LedPositionSlider_RangeValueChanged(object sender, RoutedEventArgs e) {
            var LedsSelectedCount = LedPositionSlider.HigherValue - LedPositionSlider.LowerValue;
            totalLabel.Text = LedsSelectedCount.ToString();
            AutoSetLeds();
        }
    }
}
