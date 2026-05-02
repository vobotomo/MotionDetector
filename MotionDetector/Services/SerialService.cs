using System.IO.Ports;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace MotionDetector.Services
{
    [SupportedOSPlatform("windows")]
    public class SerialService
    {
        private SerialPort? _port;
        public event Action<string>? DataReceived;
        public bool IsConnected => _port?.IsOpen ?? false;

        public bool Connect(string portName)
        {
            try
            {
                _port = new SerialPort(portName, 9600);
                _port.DataReceived += (s, e) =>
                {
                    string data = _port.ReadLine().Trim();
                    DataReceived?.Invoke(data);
                };
                _port.Open();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Chyba: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            _port?.Close();
        }

        public static string[] GetAvailablePorts()
        {
            try
            {
                using var key = Registry.LocalMachine
                    .OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM");
                if (key == null) return Array.Empty<string>();
                return key.GetValueNames()
                    .Select(n => key.GetValue(n)?.ToString() ?? "")
                    .Where(s => !string.IsNullOrEmpty(s))
                    .OrderBy(s => s)
                    .ToArray();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }
    }
}