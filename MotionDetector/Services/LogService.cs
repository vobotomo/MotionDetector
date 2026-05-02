using System.IO;

namespace MotionDetector.Services
{
    public class LogService
    {
        private readonly string _logPath;
    
        public LogService()
        {
            // Ukládej log vždy vedle exe souboru
            string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            _logPath = Path.Combine(exeFolder, "motion_log.txt");
        }

        public void Log(string type)
        {
            string line = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} | {type}";
            File.AppendAllText(_logPath, line + Environment.NewLine);
        }
    }
}