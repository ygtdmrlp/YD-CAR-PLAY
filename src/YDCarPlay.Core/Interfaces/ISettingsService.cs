using System;
using YDCarPlay.Core.Models;

namespace YDCarPlay.Core.Interfaces
{
    public interface ISettingsService
    {
        AppSettings Settings { get; }
        event EventHandler<AppSettings>? SettingsChanged;

        void Load();
        void Save();
        void ResetToDefaults();
    }
}
