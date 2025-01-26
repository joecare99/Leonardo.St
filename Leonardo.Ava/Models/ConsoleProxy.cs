using Leonardo.Models.Interfaces;
using System;
using System.Reflection;

namespace Leonardo.Models;

public class ConsoleProxy : IConsole
{
    MethodInfo _writeLineS = typeof(Console).GetMethod(nameof(Console.WriteLine), [typeof(string)]);
    public void WriteLine(string v) => _writeLineS?.Invoke(null, [v]);
}
