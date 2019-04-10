using System;
using System.IO.Ports;
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
        }

        private void GreenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            GreenLabel.Text = GreenSlider.Value.ToString();
            ColorChange();
        }

        private void BlueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            BlueLabel.Text = BlueSlider.Value.ToString();
            ColorChange();
        }

        private void ColorChange() {
            var red = (int)RedSlider.Value;
            var green = (int)GreenSlider.Value;
            var blue = (int)BlueSlider.Value;
            ColorPreviewBox.Fill = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }

        private void ChangeLed_Click(object sender, RoutedEventArgs e) {
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
            
            // TODO: Fix Int -> Byte

            if (SerialPortSelector.Text == "")
            {
                MessageBox.Show("You need to select a serial port.");
            }
            else
            {
                SerialPort Serial = new SerialPort(SerialPortSelector.Text, 115200, Parity.None, 8, StopBits.One);
                Serial.Handshake = Handshake.None;
                Serial.Open();
                Serial.Write(SerialCommand);
                Serial.Close();
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

        private void Button_Click(object sender, RoutedEventArgs e) {
            UpdateSerialSelector();
        }

        private void RedLabel_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (RedLabel.Text == "") {
                RedSlider.Value = 0.0;
            } else {
                RedSlider.Value = Int32.Parse(RedLabel.Text);
            }
            ColorChange();
        }

        private void GreenLabel_OnTextChanged(object sender, TextChangedEventArgs e) {
            if (GreenLabel.Text == "")
            {
                GreenSlider.Value = 0.0;
            }else
            {
                GreenSlider.Value = Int32.Parse(GreenLabel.Text);
            }
            ColorChange();
        }

        private void BlueLabel_OnTextChanged(object sender, TextChangedEventArgs e) {
            if (BlueLabel.Text == "") {
                BlueSlider.Value = 0.0;
            } else {
                BlueSlider.Value = Int32.Parse(BlueLabel.Text);
            }
            ColorChange();
        }
    }
}
