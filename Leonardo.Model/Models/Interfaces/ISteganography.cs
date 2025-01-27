using BaseLib.Interfaces;
using System.Drawing;

namespace Leonardo.Models.Interfaces;

public interface ISteganography
{
    string ConvertBinaryToMessage(string binaryMessage);
    string ConvertTextToBinary(string text);
    string Decrypt(Bitmap image);
    Bitmap Encrypt(Bitmap originalImage, string inputString);
    Bitmap NormalizeImage(Bitmap originalImage);
    void SetConsole(IConsole console);
}