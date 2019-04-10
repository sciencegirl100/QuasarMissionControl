using System.Windows;
using System.Windows.Media;

namespace MissionControl {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            RedSlider.Value = 127;
            GreenSlider.Value = 127;
            BlueSlider.Value = 127;
            ColorChange();
            UpdateSerialSelector();
        }

        /* TODO:
         * - Set HexBox to current-set RGB  (ColorChange)
         * - On HexBox change, update RGB sliders
         * - Set RGB TextBoxes to mirror sliders
         */

        private void RedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            ColorChange();
        }

        private void GreenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            ColorChange();
        }

        private void BlueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            ColorChange();
        }

        private void ColorChange() {
            int red = (int)RedSlider.Value;
            int green = (int)GreenSlider.Value;
            int blue = (int)BlueSlider.Value;
            ColorPreviewBox.Fill = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }

        private void ChangeLed_Click(object sender, RoutedEventArgs e) {
            // Set LEDs
            int red = (int)RedSlider.Value;
            int green = (int)GreenSlider.Value;
            int blue = (int)BlueSlider.Value;
            int lower = (int)LedPositionSlider.LowerValue;
            int higher = (int)LedPositionSlider.HigherValue;
            int LEDCount = 136;
            // Map Lower/higher range to ASCII byte
            lower = Map(lower, 0, LEDCount, 0, 127);
            higher = Map(higher, 0, LEDCount, 0, 127);
            // Create Serial Command as a String
            string SerialCommand = (char)(byte)red + "" + (char)(byte)green + "" + (char)(byte)blue + "" + (char)(byte)lower + "" + (char)(byte)higher + "\n";
            // MessageBox.Show("Red: " + (char)(byte)red + "\n Green: " + (char)(byte)green + "\n Blue: " + (char)(byte)blue);
            // MessageBox.Show(SerialCommand);

            // TODO: Pull from Selector Box and push to that serial port
            // https://www.c-sharpcorner.com/UploadFile/eclipsed4utoo/communicating-with-serial-port-in-C-Sharp/
            // https://docs.microsoft.com/en-us/dotnet/api/system.io.ports.serialport?view=netframework-4.7.2

        }
        private void UpdateSerialSelector() {
            // TODO: get list of serial ports and dump to combo-box
            // https://docs.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.getportnames?view=netframework-4.7.2
        }

        private int Map(int value, int fromSource, int toSource, int fromTarget, int toTarget) {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            UpdateSerialSelector();
        }
    }
}
