namespace MotionDetector.Models
{
    public class MotionEvent
    {
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } // "MOTION" nebo "CLEAR"
        public string DisplayTime => Timestamp.ToString("HH:mm:ss");
        public string DisplayDate => Timestamp.ToString("dd.MM.yyyy");
    }
}