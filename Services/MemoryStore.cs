namespace CyberSecurityChatbotWPF.Services
{
    public class MemoryStore
    {
        public string UserName { get; set; }
        public string Interest { get; set; }
        public string LastTopic { get; set; }

        public void RememberInterest(string interest)
        {
            Interest = interest;
        }

        public string RecallPersonalizedMessage()
        {
            if (!string.IsNullOrEmpty(Interest))
            {
                return $"Since you're interested in {Interest}, here's a relevant tip: ";
            }
            return "";
        }

        public string GetUserInfo()
        {
            string info = "";
            if (!string.IsNullOrEmpty(UserName))
            {
                info += $"Your name is {UserName}. ";
            }
            if (!string.IsNullOrEmpty(Interest))
            {
                info += $"You're interested in {Interest}. ";
            }
            if (string.IsNullOrEmpty(info))
            {
                return "I don't know much about you yet. Tell me your name or what cybersecurity topic interests you!";
            }
            return info;
        }
    }
}