using BaseLib.Interfaces;
using Leonardo.Models.Interfaces;
using System;
using System.Drawing;
using System.Text;

namespace Leonardo.Models;

/// <summary>
/// Provides steganographic functionality for hiding and extracting secret messages within bitmap images.
/// This implementation uses Least Significant Bit (LSB) encoding combined with AES encryption and
/// Caesar cipher obfuscation to securely embed text data into image pixels.
/// </summary>
/// <remarks>
/// <para>
/// <b>Steganography Overview:</b>
/// Steganography is the practice of concealing information within non-secret data or physical objects
/// to avoid detection. Unlike encryption, which makes data unreadable, steganography hides the very
/// existence of the secret message. This class combines both techniques for enhanced security.
/// </para>
/// <para>
/// <b>Encoding Technique:</b>
/// This implementation uses LSB (Least Significant Bit) encoding, which modifies the least significant
/// bits of pixel color values to store message data. Since the LSB changes result in imperceptible
/// color differences (±1 in RGB values ranging 0-255), the visual appearance of the image remains
/// virtually unchanged to the human eye.
/// </para>
/// <para>
/// <b>Security Layers:</b>
/// The message undergoes multiple transformations before embedding:
/// </para>
/// <list type="number">
/// <item><description><b>Caesar Cipher:</b> A simple character shift (+2) provides basic obfuscation</description></item>
/// <item><description><b>AES Encryption:</b> Strong symmetric encryption using <see cref="EncryptionUtility"/></description></item>
/// <item><description><b>Base64 Encoding:</b> Converts binary ciphertext to ASCII for consistent bit representation</description></item>
/// <item><description><b>LSB Embedding:</b> Hides the encoded message in pixel color values</description></item>
/// </list>
/// <para>
/// <b>Pixel Selection Algorithm:</b>
/// Not all pixels are used for data storage. The algorithm uses a heuristic based on pixel similarity
/// with neighbors and position-based patterns to determine which pixels store data. This makes the
/// hidden data more resistant to detection by steganalysis tools.
/// </para>
/// <para>
/// <b>Data Capacity:</b>
/// Each eligible pixel can store up to 3 bits of data (one in each RGB channel). The maximum message
/// size depends on the image dimensions and the number of eligible pixels. Larger images with more
/// color variation can store more data.
/// </para>
/// <para>
/// <b>Supported Image Formats:</b>
/// Works with any <see cref="Bitmap"/> image. For best results, use lossless formats (PNG, BMP)
/// as lossy compression (JPEG) will destroy the embedded data.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Create steganography instance with console logging
/// var stego = new Steganography(new ConsoleWrapper());
/// 
/// // Load an image and hide a message
/// Bitmap originalImage = new Bitmap("photo.png");
/// Bitmap encodedImage = stego.Encrypt(originalImage, "This is a secret message!");
/// encodedImage.Save("photo_with_secret.png", ImageFormat.Png);
/// 
/// // Later, extract the hidden message
/// Bitmap loadedImage = new Bitmap("photo_with_secret.png");
/// string secretMessage = stego.Decrypt(loadedImage);
/// // secretMessage == "This is a secret message!"
/// </code>
/// </example>
/// <seealso cref="ISteganography"/>
/// <seealso cref="EncryptionUtility"/>
/// <seealso cref="Bitmap"/>
public class Steganography : ISteganography
{
    /// <summary>
    /// The console interface used for diagnostic logging during encryption and decryption operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This dependency allows the steganography operations to output debug information and
    /// intermediate results without being coupled to a specific console implementation.
    /// </para>
    /// <para>
    /// Typical output includes:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Binary representations of text during conversion</description></item>
    /// <item><description>Encrypted Base64 strings</description></item>
    /// <item><description>Character-by-character cipher shifts</description></item>
    /// <item><description>Intermediate decryption results</description></item>
    /// </list>
    /// </remarks>
    private IConsole _console;

    /// <summary>
    /// Initializes a new instance of the <see cref="Steganography"/> class with the specified console interface.
    /// </summary>
    /// <param name="console">
    /// The <see cref="IConsole"/> implementation used for diagnostic output during operations.
    /// This parameter is required and should not be <see langword="null"/>.
    /// </param>
    /// <remarks>
    /// <para>
    /// The console dependency enables logging of intermediate processing steps, which is useful
    /// for debugging and understanding the steganographic process. In production scenarios,
    /// a null-object pattern implementation could be provided to suppress output.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Using a real console
    /// var stego = new Steganography(new ConsoleWrapper());
    /// 
    /// // Using a mock console for testing
    /// var mockConsole = new MockConsole();
    /// var testStego = new Steganography(mockConsole);
    /// </code>
    /// </example>
    public Steganography(IConsole console)
    {
        _console = console;
    }

    /// <summary>
    /// Replaces the current console interface with a new implementation.
    /// </summary>
    /// <param name="console">
    /// The new <see cref="IConsole"/> implementation to use for subsequent operations.
    /// </param>
    /// <remarks>
    /// <para>
    /// This method allows runtime switching of the console output destination, which can be
    /// useful for:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Redirecting output to different logging targets</description></item>
    /// <item><description>Enabling/disabling verbose output during execution</description></item>
    /// <item><description>Switching between GUI and console output modes</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// var stego = new Steganography(new QuietConsole());
    /// // ... perform operations silently ...
    /// 
    /// // Enable verbose logging for debugging
    /// stego.SetConsole(new VerboseConsole());
    /// // ... operations now produce detailed output ...
    /// </code>
    /// </example>
    public void SetConsole(IConsole console)
    {
        _console = console;
    }

    /// <summary>
    /// Normalizes an image by setting all color channel LSBs to zero, preparing it for message embedding.
    /// </summary>
    /// <param name="originalImage">
    /// The source <see cref="Bitmap"/> image to normalize. This image is not modified.
    /// </param>
    /// <returns>
    /// A new <see cref="Bitmap"/> instance with all pixel color values adjusted to have even
    /// (LSB = 0) values in all four channels (A, R, G, B).
    /// </returns>
    /// <remarks>
    /// <para>
    /// <b>Purpose:</b>
    /// Image normalization is an essential preprocessing step that creates a "clean slate" for
    /// message embedding. By ensuring all LSBs are zero, the subsequent encoding process can
    /// reliably set bits to 0 or 1 to represent the message data.
    /// </para>
    /// <para>
    /// <b>Algorithm:</b>
    /// For each pixel, each color channel (Alpha, Red, Green, Blue) is checked:
    /// </para>
    /// <list type="bullet">
    /// <item><description>If the value is odd (LSB = 1), subtract 1 to make it even</description></item>
    /// <item><description>If the value is even (LSB = 0), keep it unchanged</description></item>
    /// </list>
    /// <para>
    /// <b>Visual Impact:</b>
    /// The maximum change to any color channel is -1, which is imperceptible to the human eye.
    /// For example, RGB(255, 128, 65) becomes RGB(254, 128, 64) - visually identical.
    /// </para>
    /// <para>
    /// <b>Performance Note:</b>
    /// This method uses <see cref="Bitmap.GetPixel"/> and <see cref="Bitmap.SetPixel"/>,
    /// which are relatively slow for large images. For production use with large images,
    /// consider using <see cref="Bitmap.LockBits"/> for direct memory access.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// Bitmap original = new Bitmap("photo.png");
    /// Bitmap normalized = stego.NormalizeImage(original);
    /// 
    /// // Verify normalization: all LSBs should be 0
    /// Color pixel = normalized.GetPixel(0, 0);
    /// Debug.Assert(pixel.R % 2 == 0);
    /// Debug.Assert(pixel.G % 2 == 0);
    /// Debug.Assert(pixel.B % 2 == 0);
    /// </code>
    /// </example>
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

    /// <summary>
    /// Converts a text string into its binary representation with space-separated bytes.
    /// </summary>
    /// <param name="text">
    /// The text string to convert. Each character is converted to its 8-bit ASCII/Unicode binary representation.
    /// </param>
    /// <returns>
    /// A string containing the binary representation of each character as 8-bit sequences,
    /// separated by spaces. For example, "Hi" returns "01001000 01101001".
    /// </returns>
    /// <remarks>
    /// <para>
    /// <b>Conversion Process:</b>
    /// Each character is converted using <see cref="Convert.ToString(char, int)"/> with base 2,
    /// then left-padded with zeros to ensure exactly 8 bits per character.
    /// </para>
    /// <para>
    /// <b>Character Encoding:</b>
    /// The conversion uses the character's Unicode code point. For ASCII characters (0-127),
    /// this produces standard 7-bit ASCII values padded to 8 bits. Extended Unicode characters
    /// may produce values that don't fit in 8 bits, but the Base64-encoded AES ciphertext
    /// used as input will only contain ASCII characters.
    /// </para>
    /// <para>
    /// <b>Diagnostic Output:</b>
    /// The method logs the complete binary string to the console for debugging purposes.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// string binary = stego.ConvertTextToBinary("ABC");
    /// // Returns: "01000001 01000010 01000011"
    /// // Console output: "TXT2BIN: 01000001 01000010 01000011"
    /// 
    /// string hiBinary = stego.ConvertTextToBinary("Hi");
    /// // Returns: "01001000 01101001"
    /// </code>
    /// </example>
    /// <seealso cref="ConvertBinaryToMessage"/>
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

    /// <summary>
    /// Embeds a binary message string into the LSBs of a normalized image's pixel color values.
    /// </summary>
    /// <param name="normalizedImage">
    /// A <see cref="Bitmap"/> that has been preprocessed by <see cref="NormalizeImage"/>
    /// (all LSBs set to 0).
    /// </param>
    /// <param name="binaryMessage">
    /// The binary string to embed, consisting of '0' and '1' characters, optionally separated
    /// by spaces. Spaces are removed before processing.
    /// </param>
    /// <returns>
    /// A new <see cref="Bitmap"/> with the message embedded in the LSBs of selected pixels.
    /// </returns>
    /// <remarks>
    /// <para>
    /// <b>Pixel Selection Algorithm:</b>
    /// Not every pixel stores data. The algorithm uses a heuristic that considers:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Similarity to the pixel directly to the left (pixel0)</description></item>
    /// <item><description>Similarity to the pixel below (pixel1)</description></item>
    /// <item><description>A position-based pattern using XOR of coordinates: <c>((i ^ j) &amp; 0x3) != 0</c></description></item>
    /// </list>
    /// <para>
    /// <b>Marker Bit (Alpha Channel):</b>
    /// The alpha channel's LSB serves as a marker to indicate whether a pixel contains data:
    /// </para>
    /// <list type="bullet">
    /// <item><description>If <c>(A &amp; 1) == (A > 127 ? 0 : 1)</c>: pixel contains message bits</description></item>
    /// <item><description>Otherwise: pixel is skipped during extraction</description></item>
    /// </list>
    /// <para>
    /// <b>Data Storage:</b>
    /// Each data-carrying pixel stores 3 bits of the message:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Red channel LSB: bit N (XORed with brightness indicator)</description></item>
    /// <item><description>Green channel LSB: bit N+1 (XORed with brightness indicator)</description></item>
    /// <item><description>Blue channel LSB: bit N+2 (XORed with brightness indicator)</description></item>
    /// </list>
    /// <para>
    /// <b>Brightness XOR:</b>
    /// The data bits are XORed with a brightness-based value <c>(channel > 127 ? 1 : 0)</c>
    /// to reduce visual artifacts by maintaining statistical properties of the image.
    /// </para>
    /// </remarks>
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

    /// <summary>
    /// Applies a Caesar cipher encryption by shifting each letter forward by 2 positions in the alphabet.
    /// </summary>
    /// <param name="message">
    /// The plaintext message to encrypt. Can contain any characters; only letters are shifted.
    /// </param>
    /// <returns>
    /// The encrypted message with each letter shifted forward by 2 positions.
    /// Non-letter characters remain unchanged.
    /// </returns>
    /// <remarks>
    /// <para>
    /// <b>Caesar Cipher:</b>
    /// This is a simple substitution cipher where each letter is replaced by a letter
    /// a fixed number of positions down the alphabet. This implementation uses a shift of +2.
    /// </para>
    /// <para>
    /// <b>Alphabet Wrapping:</b>
    /// Letters near the end of the alphabet wrap around:
    /// </para>
    /// <list type="bullet">
    /// <item><description>'Y' → 'A', 'Z' → 'B' (uppercase)</description></item>
    /// <item><description>'y' → 'a', 'z' → 'b' (lowercase)</description></item>
    /// </list>
    /// <para>
    /// <b>Case Preservation:</b>
    /// The original case of each letter is preserved after shifting.
    /// </para>
    /// <para>
    /// <b>Security Note:</b>
    /// The Caesar cipher alone provides minimal security (easily broken). Here it serves
    /// as an additional obfuscation layer before AES encryption, adding defense in depth.
    /// </para>
    /// <para>
    /// <b>Diagnostic Output:</b>
    /// Each character transformation is logged to the console for debugging.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// string encrypted = EncryptMessage("Hello");
    /// // Returns: "Jgnnq"
    /// // H→J, e→g, l→n, l→n, o→q
    /// 
    /// string wrapped = EncryptMessage("XYZ");
    /// // Returns: "ZAB"
    /// // X→Z, Y→A, Z→B
    /// </code>
    /// </example>
    /// <seealso cref="DecryptMessage"/>
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

    /// <summary>
    /// Encrypts a message and embeds it into a bitmap image using steganography.
    /// </summary>
    /// <param name="originalImage">
    /// The source <see cref="Bitmap"/> image to use as the carrier for the hidden message.
    /// This image is not modified; a new image is returned.
    /// </param>
    /// <param name="inputString">
    /// The secret message to hide within the image. Can be any text string.
    /// </param>
    /// <returns>
    /// A new <see cref="Bitmap"/> containing the hidden message, visually nearly identical
    /// to the original image.
    /// </returns>
    /// <remarks>
    /// <para>
    /// <b>Complete Encryption Pipeline:</b>
    /// This method orchestrates the full steganographic encoding process:
    /// </para>
    /// <list type="number">
    /// <item>
    /// <description><b>Image Normalization:</b> Prepares the image by setting all LSBs to 0
    /// using <see cref="NormalizeImage"/></description>
    /// </item>
    /// <item>
    /// <description><b>Caesar Cipher:</b> Shifts letters by +2 using <see cref="EncryptMessage"/></description>
    /// </item>
    /// <item>
    /// <description><b>AES Encryption:</b> Encrypts the shifted text using
    /// <see cref="EncryptionUtility.EncryptStringToBytes_Aes"/></description>
    /// </item>
    /// <item>
    /// <description><b>Base64 Encoding:</b> Converts the binary ciphertext to ASCII text</description>
    /// </item>
    /// <item>
    /// <description><b>Binary Conversion:</b> Converts Base64 string to binary using
    /// <see cref="ConvertTextToBinary"/></description>
    /// </item>
    /// <item>
    /// <description><b>LSB Embedding:</b> Hides the binary data in pixel LSBs using
    /// <see cref="ApplyBinaryMessage"/></description>
    /// </item>
    /// </list>
    /// <para>
    /// <b>Diagnostic Output:</b>
    /// The encrypted Base64 string is logged to the console.
    /// </para>
    /// <para>
    /// <b>Important:</b> Save the resulting image in a lossless format (PNG, BMP).
    /// JPEG or other lossy compression will corrupt the hidden data.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var stego = new Steganography(console);
    /// 
    /// Bitmap original = new Bitmap("vacation_photo.png");
    /// Bitmap withSecret = stego.Encrypt(original, "Meet at noon tomorrow.");
    /// 
    /// // Save as PNG to preserve the hidden data
    /// withSecret.Save("vacation_photo_encoded.png", ImageFormat.Png);
    /// </code>
    /// </example>
    /// <seealso cref="Decrypt"/>
    public Bitmap Encrypt(Bitmap originalImage, string inputString)
    {
        Bitmap normalizedImage = NormalizeImage(originalImage);
        string text = Convert.ToBase64String(EncryptionUtility.EncryptStringToBytes_Aes(EncryptMessage(inputString)));
        _console.WriteLine(string.Format("Encrypted string (Base64): {0}", text));
        string binaryMessage = ConvertTextToBinary(text);
        return ApplyBinaryMessage(normalizedImage, binaryMessage);
    }

    /// <summary>
    /// Extracts and decrypts a hidden message from a steganographic image.
    /// </summary>
    /// <param name="image">
    /// The <see cref="Bitmap"/> image containing a hidden message, previously created
    /// using <see cref="Encrypt"/>.
    /// </param>
    /// <returns>
    /// The decrypted plaintext message extracted from the image.
    /// </returns>
    /// <remarks>
    /// <para>
    /// <b>Extraction and Decryption Pipeline:</b>
    /// This method performs the reverse operations of the encoding process:
    /// </para>
    /// <list type="number">
    /// <item>
    /// <description><b>Message Bits Extraction:</b> Retrieves the embedded bits from the image pixels</description>
    /// </item>
    /// <item>
    /// <description><b>Binary to Text Conversion:</b> Converts the extracted binary data to a text string
    /// using <see cref="ConvertBinaryToMessage"/></description>
    /// </item>
    /// <item>
    /// <description><b>AES Decryption:</b> Decrypts the AES-encrypted message using
    /// <see cref="EncryptionUtility.DecryptStringFromBytes_Aes"/></description>
    /// </item>
    /// <item>
    /// <description><b>Caesar Cipher Decryption:</b> Reverses the Caesar cipher obfuscation
    /// using <see cref="DecryptMessage"/></description>
    /// </item>
    /// </list>
    /// <para>
    /// <b>Performance Note:</b>
    /// This method is optimized for speed: it processes the image pixel data in a single
    /// pass without intermediate image creation. However, this means the output may not
    /// be visually identical to the original image.
    /// </para>
    /// <para>
    /// <b>Diagnostic Output:</b>
    /// Intermediate values are logged to the console for debugging purposes.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Assuming 'stego' is your Steganography instance
    /// Bitmap encodedImage = new Bitmap("vacation_photo_encoded.png");
    /// string secretMessage = stego.Decrypt(encodedImage);
    /// Console.WriteLine("Hidden message: " + secretMessage);
    /// </code>
    /// </example>
    /// <seealso cref="Encrypt"/>
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

    /// <summary>
    /// Decrypts a Caesar cipher encrypted message by shifting each letter backward by 2 positions in the alphabet.
    /// </summary>
    /// <param name="encryptedMessage">
    /// The Caesar cipher encrypted message to decrypt. Can contain any characters; only letters are shifted.
    /// </param>
    /// <returns>
    /// The decrypted plaintext message with each letter shifted backward by 2 positions.
    /// Non-letter characters remain unchanged.
    /// </returns>
    /// <remarks>
    /// <para>
    /// <b>Decryption Process:</b>
    /// This method reverses the Caesar cipher encryption applied by <see cref="EncryptMessage"/>.
    /// Each letter is shifted backward by 2 positions in the alphabet to recover the original character.
    /// </para>
    /// <para>
    /// <b>Alphabet Wrapping:</b>
    /// Letters near the beginning of the alphabet wrap around to the end:
    /// </para>
    /// <list type="bullet">
    /// <item><description>'A' → 'Y', 'B' → 'Z' (uppercase)</description></item>
    /// <item><description>'a' → 'y', 'b' → 'z' (lowercase)</description></item>
    /// </list>
    /// <para>
    /// <b>Case Preservation:</b>
    /// The original case of each letter is preserved after the reverse shift operation.
    /// </para>
    /// <para>
    /// <b>Non-Letter Characters:</b>
    /// Characters that are not letters (digits, punctuation, whitespace, symbols) pass through
    /// unchanged, preserving the structure of the original message.
    /// </para>
    /// <para>
    /// <b>Diagnostic Output:</b>
    /// Each character transformation is logged to the console via <see cref="_console"/>,
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