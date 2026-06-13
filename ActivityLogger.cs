using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityChatbotWPF.Services
{
    public class ActivityLogger
    {
        private List<string> activityLog;
        private const int MaxLogEntries = 10;

        public ActivityLogger()
        {
            activityLog = new List<string>();
        }

        public void AddLog(string action)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            activityLog.Insert(0, $"[{timestamp}] {action}");

            // Keep only last 10 entries
            if (activityLog.Count > MaxLogEntries)
            {
                activityLog = activityLog.Take(MaxLogEntries).ToList();
            }
        }

        public List<string> GetRecentLogs()
        {
            return activityLog;
        }

        public string GetFormattedLog()
        {
            if (activityLog.Count == 0)
            {
                return "No activity logged yet.";
            }

            string logText = "Recent activities:\n";
            for (int i = 0; i < activityLog.Count; i++)
            {
                logText += $"{i + 1}. {activityLog[i]}\n";
            }
            return logText;
        }

        public void ClearLogs()
        {
            activityLog.Clear();
            AddLog("Activity log cleared");
        }
    }
}
