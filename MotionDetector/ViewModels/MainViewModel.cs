using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using MotionDetector.Models;
using MotionDetector.Services;

namespace MotionDetector.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SerialService _serial = new();
        private readonly LogService _log = new();

        public ObservableCollection<MotionEvent> Events { get; } = new();
        public ObservableCollection<string> Ports { get; } = new();

        private string? _selectedPort;
        public string? SelectedPort
        {
            get => _selectedPort;
            set { _selectedPort = value; OnPropertyChanged(nameof(SelectedPort)); }
        }

        private string _status = "Odpojeno";
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        private string _statusColor = "#FF4444";
        public string StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; OnPropertyChanged(nameof(StatusColor)); }
        }

        private int _motionCount = 0;
        public int MotionCount
        {
            get => _motionCount;
            set { _motionCount = value; OnPropertyChanged(nameof(MotionCount)); }
        }

        private bool _motionActive = false;
        public bool MotionActive
        {
            get => _motionActive;
            set { _motionActive = value; OnPropertyChanged(nameof(MotionActive)); }
        }

        // Graf
        private readonly ObservableCollection<int> _chartValues = new();
        private readonly ObservableCollection<string> _chartLabels = new();

        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        public MainViewModel()
        {
            Series = new ISeries[]
            {
                new LineSeries<int>
                {
                    Values = _chartValues,
                    Name = "Pohyby",
                    Stroke = new SolidColorPaint(SKColors.Cyan, 2),
                    Fill = new SolidColorPaint(new SKColor(0, 212, 255, 40)),
                    GeometrySize = 8,
                    GeometryStroke = new SolidColorPaint(SKColors.Cyan, 2),
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = _chartLabels,
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(80, 80, 80)),
                    TextSize = 10
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(80, 80, 80))
                }
            };

            _serial.DataReceived += OnDataReceived;
            RefreshPorts();
        }

        public void RefreshPorts()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Ports.Clear();
                var ports = SerialService.GetAvailablePorts();
                foreach (var p in ports)
                    Ports.Add(p);

                if (Ports.Count > 0)
                    SelectedPort = Ports[0];
            });
        }

        public void Connect()
        {
            if (SelectedPort == null) return;
            bool ok = _serial.Connect(SelectedPort);
            if (ok)
            {
                Status = $"Připojeno ({SelectedPort})";
                StatusColor = "#44FF44";
            }
            else
            {
                Status = "Chyba připojení";
                StatusColor = "#FF4444";
            }
            OnPropertyChanged(nameof(Status));
        }

        public void Disconnect()
        {
            _serial.Disconnect();
            Status = "Odpojeno";
            StatusColor = "#FF4444";
        }

        private void OnDataReceived(string data)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var type = data.Split(';')[0];
                var ev = new MotionEvent { Timestamp = DateTime.Now, Type = type };

                Events.Insert(0, ev);
                _log.Log(type);

                if (type == "MOTION")
                {
                    MotionCount++;
                    MotionActive = true;
                    System.Media.SystemSounds.Hand.Play();

                    // Aktualizuj graf
                    _chartValues.Add(MotionCount);
                    _chartLabels.Add(DateTime.Now.ToString("HH:mm:ss"));

                    // Max 20 záznamů
                    if (_chartValues.Count > 20)
                    {
                        _chartValues.RemoveAt(0);
                        _chartLabels.RemoveAt(0);
                    }
                }
                else if (type == "CLEAR")
                {
                    MotionActive = false;
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}