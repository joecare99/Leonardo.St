using System.Reflection;

namespace Leonardo.Models.Interfaces;

public interface ILeonardoSettings
{
#if NET5_0_OR_GREATER
    string this[ELSetting key] { get; set; }
#endif
    string Get(ELSetting key);

}