using BaseLib.Interfaces;
using System.Drawing;

namespace Leonardo.Models.Interfaces;

/// <summary>
/// Defines the contract for steganography operations, enabling the hiding and extraction
/// of secret messages within digital images using least significant bit (LSB) manipulation
/// or similar techniques.
/// </summary>
/// <remarks>
/// Steganography is the practice of concealing information within non-secret data or a physical object
/// to avoid detection. This interface provides methods for encoding text messages into bitmap images
/// and decoding hidden messages from images that contain embedded data.
/// </remarks>
public interface ISteganography
{
    /// <summary>
    /// Converts a binary string representation back into its original text message.
    /// </summary>
    /// <param name="binaryMessage">
    /// A string containing binary digits (0s and 1s) representing the encoded message.
    /// Each character in the original message is typically represented by 8 bits (one byte).
    /// </param>
    /// <returns>
    /// The decoded plain text message extracted from the binary representation.
    /// </returns>
    /// <example>
    /// Input: "0100100001101001" would return "Hi" (H = 01001000, i = 01101001).
    /// </example>
    string ConvertBinaryToMessage(string binaryMessage);

    /// <summary>
    /// Converts a plain text string into its binary representation.
    /// </summary>
    /// <param name="text">
    /// The plain text message to be converted into binary format.
    /// Each character will be converted to its 8-bit binary equivalent.
    /// </param>
    /// <returns>
    /// A string containing the binary representation of the input text,
    /// where each character is represented as an 8-bit binary sequence.
    /// </returns>
    /// <example>
    /// Input: "Hi" would return "0100100001101001" (H = 01001000, i = 01101001).
    /// </example>
    string ConvertTextToBinary(string text);

    /// <summary>
    /// Extracts and decrypts a hidden message from a steganographic image.
    /// </summary>
    /// <param name="image">
    /// The bitmap image containing the hidden steganographic message.
    /// This image must have been previously encoded using a compatible encryption method.
    /// </param>
    /// <returns>
    /// The decrypted plain text message that was hidden within the image.
    /// Returns an empty string if no valid message is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="image"/> is <c>null</c>.
    /// </exception>
    string Decrypt(Bitmap image);

    /// <summary>
    /// Encrypts and embeds a secret message into a bitmap image using steganographic techniques.
    /// </summary>
    /// <param name="originalImage">
    /// The carrier image that will contain the hidden message.
    /// The image should have sufficient pixel capacity to store the entire message.
    /// </param>
    /// <param name="inputString">
    /// The secret text message to be hidden within the image.
    /// The message length is limited by the image dimensions and encoding method.
    /// </param>
    /// <returns>
    /// A new <see cref="Bitmap"/> instance containing the original image with the secret message embedded.
    /// The returned image appears visually identical to the original but contains the hidden data.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="originalImage"/> or <paramref name="inputString"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the message is too long to fit within the provided image.
    /// </exception>
    Bitmap Encrypt(Bitmap originalImage, string inputString);

    /// <summary>
    /// Normalizes an image to ensure consistent encoding and decoding of steganographic data.
    /// </summary>
    /// <param name="originalImage">
    /// The bitmap image to be normalized. This process may adjust pixel values
    /// to create a clean baseline for steganographic operations.
    /// </param>
    /// <returns>
    /// A new <see cref="Bitmap"/> instance with normalized pixel values,
    /// suitable for reliable steganographic encoding and decoding.
    /// </returns>
    /// <remarks>
    /// Normalization may include operations such as clearing the least significant bits
    /// of pixel color channels to prepare the image for embedding data.
    /// </remarks>
    Bitmap NormalizeImage(Bitmap originalImage);

    /// <summary>
    /// Sets the console interface for outputting status messages and progress information
    /// during steganographic operations.
    /// </summary>
    /// <param name="console">
    /// An implementation of <see cref="IConsole"/> that handles output messages.
    /// This allows for flexible logging and user feedback during encoding/decoding operations.
    /// </param>
    void SetConsole(IConsole console);
}