using BaseLib.Interfaces;
using Leonardo.Models.Interfaces;
using System;
using System.Drawing;
using System.Text;

namespace Leonardo.Models;

public class Steganography : ISteganography
{
    private IConsole _console;

    public Steganography(IConsole console)
    {
        _console = console;
    }

    public void SetConsole(IConsole console)
    {
        _console = console;
    }

    public Bitmap NormalizeImage(Bitmap originalImage)
    {
        Bitmap bitmap = new Bitmap(originalImage.Width, originalImage.Height);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.DrawImage(originalImage, Point.Empty);
        }
        for (int i = 0; i < bitmap.Height; i++)
        {
            for (int j = 0; j < bitmap.Width; j++)
            {
                Color pixel = bitmap.GetPixel(j, i);
                Color color = Color.FromArgb(
                    pixel.A % 2 != 0 ? pixel.A - 1 : pixel.A,
                    pixel.R % 2 != 0 ? pixel.R - 1 : pixel.R,
                    pixel.G % 2 != 0 ? pixel.G - 1 : pixel.G,
                    pixel.B % 2 != 0 ? pixel.B - 1 : pixel.B);
                bitmap.SetPixel(j, i, color);
            }
        }
        return bitmap;
    }

    public string ConvertTextToBinary(string text)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            string text2 = Convert.ToString(text[i], 2);
            text2 = text2.PadLeft(8, '0');
            stringBuilder.Append(text2).Append(' ');
        }
        if (stringBuilder.Length > 0)
        {
            stringBuilder.Length--;
        }
        _console.WriteLine("TXT2BIN: " + stringBuilder.ToString());
        return stringBuilder.ToString();
    }

    private static Bitmap ApplyBinaryMessage(Bitmap normalizedImage, string binaryMessage)
    {
        Bitmap bitmap = new Bitmap(normalizedImage);

        binaryMessage = binaryMessage.Replace(" ", "");
        int num = 0;
        for (int i = 0; i < bitmap.Height; i++)
        {
            Color pixel0 = bitmap.GetPixel(0, i);
            for (int j = 0; j < bitmap.Width; j++)
            {
                Color pixel1 = bitmap.GetPixel(j, Math.Min(i + 1, bitmap.Height));
                if (num < binaryMessage.Length)
                {
                    Color pixel = bitmap.GetPixel(j, i);
                    Color color;
                    if (pixel0 == pixel && pixel1 == pixel
                         || (pixel0 == pixel
                                || pixel1 == pixel)
                              && ((i ^ j) & 0x3) != 0)
                        color = Color.FromArgb(pixel.A & 0xFE | (pixel.A > 127 ? 1 : 0), pixel);
                    else
                    {
                        color = Color.FromArgb(
                            pixel.A & 0xFE | (pixel.A > 127 ? 0 : 1),
                            pixel.R & 0xFE | (pixel.R > 127 ? 1 : 0) ^ (num < binaryMessage.Length ? binaryMessage[num] - 48 & 1 : 0),
                            pixel.G & 0xFE | (pixel.G > 127 ? 1 : 0) ^ (num + 1 < binaryMessage.Length ? binaryMessage[num + 1] - 48 & 1 : 0),
                            pixel.B & 0xFE | (pixel.B > 127 ? 1 : 0) ^ (num + 2 < binaryMessage.Length ? binaryMessage[num + 2] - 48 & 1 : 0));
                        num += 3;
                    }
                    bitmap.SetPixel(j, i, color);
                    continue;
                }
                return bitmap;
            }
        }
        return bitmap;
    }

    private string EncryptMessage(string message)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (char c in message)
        {
            char c2;
            if (char.IsLetter(c))
            {
                c2 = (char)(c + 2);
                if (char.IsLower(c) && c2 > 'z' || char.IsUpper(c) && c2 > 'Z')
                {
                    c2 = (char)(c - 24);
                }
            }
            else
            {
                c2 = c;
            }
            _console.WriteLine($"Character: {c}, Shifted: {c2}");
            stringBuilder.Append(c2);
        }
        _console.WriteLine(stringBuilder.ToString());
        return stringBuilder.ToString();
    }

    public Bitmap Encrypt(Bitmap originalImage, string inputString)
    {
        Bitmap normalizedImage = NormalizeImage(originalImage);
        string text = Convert.ToBase64String(EncryptionUtility.EncryptStringToBytes_Aes(EncryptMessage(inputString)));
        _console.WriteLine(string.Format("Encrypted string (Base64): {0}", text));
        string binaryMessage = ConvertTextToBinary(text);
        return ApplyBinaryMessage(normalizedImage, binaryMessage);
    }

    public string Decrypt(Bitmap image)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < image.Height; i++)
        {
            for (int j = 0; j < image.Width; j++)
            {
                Color pixel = image.GetPixel(j, i);
                if ((pixel.A & 1) == (pixel.A > 127 ? 0 : 1))
                {
                    stringBuilder.Append(pixel.R & 1 ^ (pixel.R > 127 ? 1 : 0));
                    stringBuilder.Append(pixel.G & 1 ^ (pixel.G > 127 ? 1 : 0));
                    stringBuilder.Append(pixel.B & 1 ^ (pixel.B > 127 ? 1 : 0));
                }
            }
        }
        string text = stringBuilder.ToString();
        _console.WriteLine(text);
        string text2 = ConvertBinaryToMessage(text);
        _console.WriteLine("msg: " + text2);
        text2 = text2.ToString();
        int num = text2.IndexOf('=');
        if (num != -1)
        {
            int num2 = text2.IndexOf('=', num + 1);
            text2 = num2 == -1 ? text2.Substring(0, num + 1) : text2.Substring(0, num2 + 1);
        }
        _console.WriteLine("msg after manip: " + text2);
        string text3 = EncryptionUtility.DecryptStringFromBytes_Aes(Convert.FromBase64String(text2));
        _console.WriteLine(string.Format("Decrypted string: {0}", text3));
        string text4 = DecryptMessage(text3);
        _console.WriteLine(text4);
        return text4;
    }

    private string DecryptMessage(string encryptedMessage)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (char c in encryptedMessage)
        {
            char c2;
            if (char.IsLetter(c))
            {
                c2 = (char)(c - 2);
                if (char.IsLower(c) && c2 < 'a' || char.IsUpper(c) && c2 < 'A')
                {
                    c2 = (char)(c + 24);
                }
            }
            else
            {
                c2 = c;
            }
            _console.WriteLine($"Character: {c}, Original: {c2}");
            stringBuilder.Append(c2);
        }
        return stringBuilder.ToString();
    }

    public string ConvertBinaryToMessage(string binaryMessage)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < binaryMessage.Length && i + 8 <= binaryMessage.Length; i += 8)
        {
            char value = (char)Convert.ToInt32(binaryMessage.Substring(i, 8), 2);
            stringBuilder.Append(value);
        }
        _console.WriteLine("BIN2MSG: " + binaryMessage.ToString());
        _console.WriteLine("BIN2MSG2: " + stringBuilder.ToString());
        return stringBuilder.ToString();
    }
}
