using Leonardo.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Leonardo.Properties;

public class SettingsProxy : ILeonardoSettings
{
    public string this[ELSetting key] { get => Get(key); set => Set(key, value); }

    private void Set(ELSetting key, string value)
    {
        throw new NotImplementedException();
    }

    public string Get(ELSetting key)
    {
        return Settings.Default[key.ToString()].ToString();
    }
}
