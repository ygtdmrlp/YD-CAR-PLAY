using System;
using System.Collections.Generic;

namespace YDCarPlay.Core.Models
{
    public class PhoneContact
    {
        public string Name { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Category { get; set; } = "Lojistik"; // "Lojistik", "Garaj", "Aile", "Acil"
        public string Initials => string.IsNullOrWhiteSpace(Name) ? "?" : Name[0].ToString().ToUpper();
        public string AvatarBgColor { get; set; } = "#34C759";
    }

    public class CallHistoryItem
    {
        public string ContactName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string TimeString { get; set; } = "Bugün 14:22";
        public bool IsMissed { get; set; }
        public string Duration { get; set; } = "3 dk 12 sn";
    }

    public class ChatMessageItem
    {
        public string SenderName { get; set; } = "";
        public string PreviewText { get; set; } = "";
        public string FullMessage { get; set; } = "";
        public string TimeString { get; set; } = "15:30";
        public bool IsUnread { get; set; }
        public bool IsPriority { get; set; }
    }
}
