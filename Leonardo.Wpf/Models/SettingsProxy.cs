using Leonardo.Models.Interfaces;

namespace Leonardo.Models
{
    internal class SettingsProxy : ILeonardoSettings
    {
        public string this[ELSetting key] { get => Get(key); set => throw new System.NotImplementedException(); }

        public string Get(ELSetting key)
        {
            return key switch
            {
                ELSetting.ApiToken => "YourApiTokenHere",
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}