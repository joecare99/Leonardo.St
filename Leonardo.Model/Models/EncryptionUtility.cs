using System;
using System.IO;
using System.Security.Cryptography;

namespace Leonardo.Models;

/// <summary>
/// Provides utility methods for symmetric encryption and decryption of strings
/// using the AES (Advanced Encryption Standard) algorithm.
/// </summary>
/// <remarks>
/// <para>
/// This utility class implements AES-256 symmetric encryption with a fixed key and
/// initialization vector (IV). It is designed for simple, application-internal
/// encryption scenarios where data needs to be protected but does not require
/// the complexity of key management systems.
/// </para>
/// <para>
/// <b>Security Considerations:</b>
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// <b>Fixed Key/IV:</b> This implementation uses hardcoded key and IV values.
/// While this simplifies usage, it means that anyone with access to the source code
/// or compiled assembly can potentially decrypt the data. This approach is suitable
/// for obfuscation but not for high-security requirements.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Key Length:</b> The 32-byte (256-bit) key provides strong encryption when
/// properly used with unique IVs per encryption operation.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>IV Reuse:</b> Using a fixed IV with the same key for multiple encryptions
/// can weaken security. For sensitive data, consider generating a random IV for
/// each encryption and storing it alongside the ciphertext.
/// </description>
/// </item>
/// </list>
/// <para>
/// <b>Algorithm Details:</b>
/// </para>
/// <list type="bullet">
/// <item><description>Algorithm: AES (Rijndael)</description></item>
/// <item><description>Key Size: 256 bits (32 bytes)</description></item>
/// <item><description>Block Size: 128 bits (16 bytes)</description></item>
/// <item><description>IV Size: 128 bits (16 bytes)</description></item>
/// <item><description>Mode: CBC (Cipher Block Chaining) - default for <see cref="Aes.Create"/></description></item>
/// <item><description>Padding: PKCS7 - default for <see cref="Aes.Create"/></description></item>
/// </list>
/// <para>
/// <b>Typical Use Cases:</b>
/// </para>
/// <list type="bullet">
/// <item><description>Encrypting configuration values or connection strings</description></item>
/// <item><description>Protecting sensitive user data in local storage</description></item>
/// <item><description>Obfuscating data that should not be easily readable</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Encrypt a string
/// string sensitiveData = "This is secret information";
/// byte[] encrypted = EncryptionUtility.EncryptStringToBytes_Aes(sensitiveData);
/// 
/// // Store or transmit the encrypted bytes...
/// 
/// // Later, decrypt the data
/// string decrypted = EncryptionUtility.DecryptStringFromBytes_Aes(encrypted);
/// // decrypted == "This is secret information"
/// </code>
/// </example>
/// <seealso cref="Aes"/>
/// <seealso cref="SymmetricAlgorithm"/>
public class EncryptionUtility
{
    /// <summary>
    /// The 256-bit (32-byte) secret key used for AES encryption and decryption operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This key is decoded from a Base64 string during static initialization.
    /// The 32-byte length corresponds to AES-256, providing the strongest
    /// encryption level available in the AES algorithm family.
    /// </para>
    /// <para>
    /// <b>Security Note:</b> In production environments with high security requirements,
    /// consider loading this key from a secure configuration source, environment variable,
    /// Azure Key Vault, or other secure key management system rather than embedding it
    /// in the source code.
    /// </para>
    /// </remarks>
    private static readonly byte[] Key;

    /// <summary>
    /// The 128-bit (16-byte) initialization vector used for AES encryption and decryption.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The IV (Initialization Vector) ensures that encrypting the same plaintext multiple
    /// times produces different ciphertext (when combined with different IVs). This is
    /// crucial for semantic security.
    /// </para>
    /// <para>
    /// <b>Security Note:</b> Using a fixed IV with a fixed key means that identical
    /// plaintexts will always produce identical ciphertexts, which can leak information
    /// about the data. For enhanced security, generate a random IV for each encryption
    /// operation and prepend it to the ciphertext.
    /// </para>
    /// </remarks>
    private static readonly byte[] IV;

    /// <summary>
    /// Initializes the static members of the <see cref="EncryptionUtility"/> class
    /// by decoding the Base64-encoded key and IV strings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This static constructor is called automatically before any static members
    /// are accessed or any instances are created. It converts the Base64-encoded
    /// key and IV strings into their byte array representations.
    /// </para>
    /// <para>
    /// The decoded values are:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Key: 32 bytes (256 bits) for AES-256</description></item>
    /// <item><description>IV: 16 bytes (128 bits) matching AES block size</description></item>
    /// </list>
    /// </remarks>
    static EncryptionUtility()
    {
        Key = Convert.FromBase64String("J5V+chcqR4BiKdEcMsC0wVjaW94ESa8/NLa28D3+ChI=");
        IV = Convert.FromBase64String("GyN51+n33DLArsVvF0GhNw==");
    }

    /// <summary>
    /// Generates an array of cryptographically secure random bytes.
    /// </summary>
    /// <param name="length">
    /// The number of random bytes to generate. Must be a positive integer.
    /// </param>
    /// <returns>
    /// A byte array of the specified length filled with cryptographically
    /// secure random values.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method uses <see cref="RNGCryptoServiceProvider"/> to generate
    /// random bytes suitable for cryptographic purposes. The generated bytes
    /// are unpredictable and suitable for use as keys, IVs, salts, or nonces.
    /// </para>
    /// <para>
    /// <b>Note:</b> While this method is currently private and unused in the
    /// public API, it provides infrastructure for generating random keys or IVs
    /// if the implementation is enhanced to support per-encryption random IVs.
    /// </para>
    /// <para>
    /// <b>Modern Alternative:</b> In .NET 6+, consider using
    /// <see cref="RandomNumberGenerator.GetBytes(int)"/> or
    /// <see cref="RandomNumberGenerator.Fill(Span{byte})"/> instead of
    /// <see cref="RNGCryptoServiceProvider"/>, which is marked as obsolete
    /// in newer .NET versions.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Generate a random 256-bit key
    /// byte[] randomKey = GenerateRandomBytes(32);
    /// 
    /// // Generate a random 128-bit IV
    /// byte[] randomIV = GenerateRandomBytes(16);
    /// </code>
    /// </example>
    /// <seealso cref="RNGCryptoServiceProvider"/>
    /// <seealso cref="RandomNumberGenerator"/>
    private static byte[] GenerateRandomBytes(int length)
    {
        byte[] array = new byte[length];
        using RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
        rNGCryptoServiceProvider.GetBytes(array);
        return array;
    }

    /// <summary>
    /// Encrypts a plaintext string using AES-256 encryption and returns the ciphertext as a byte array.
    /// </summary>
    /// <param name="plainText">
    /// The string to encrypt. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <returns>
    /// A byte array containing the AES-encrypted ciphertext. The length of this array
    /// depends on the input string length and will be padded to a multiple of 16 bytes
    /// (the AES block size) using PKCS7 padding.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs symmetric encryption using the AES algorithm with the
    /// class-level <see cref="Key"/> and <see cref="IV"/>. The encryption process:
    /// </para>
    /// <list type="number">
    /// <item><description>Creates a new AES algorithm instance</description></item>
    /// <item><description>Configures it with the predefined key and IV</description></item>
    /// <item><description>Creates an encryptor transform</description></item>
    /// <item><description>Writes the plaintext through a <see cref="CryptoStream"/></description></item>
    /// <item><description>Returns the resulting encrypted bytes</description></item>
    /// </list>
    /// <para>
    /// <b>Output Format:</b> The returned byte array contains only the ciphertext.
    /// It does not include the IV, so the same IV must be used for decryption.
    /// </para>
    /// <para>
    /// <b>Resource Management:</b> All cryptographic resources (<see cref="Aes"/>,
    /// <see cref="ICryptoTransform"/>, streams) are properly disposed using
    /// <see langword="using"/> statements to prevent resource leaks.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="plainText"/> is <see langword="null"/> or has zero length.
    /// </exception>
    /// <exception cref="CryptographicException">
    /// Thrown if the encryption operation fails due to invalid key/IV sizes or
    /// other cryptographic errors.
    /// </exception>
    /// <example>
    /// <code>
    /// string password = "MySecretPassword123";
    /// byte[] encryptedPassword = EncryptionUtility.EncryptStringToBytes_Aes(password);
    /// 
    /// // Store encryptedPassword in database or file
    /// // Can be converted to Base64 for text storage:
    /// string base64Encrypted = Convert.ToBase64String(encryptedPassword);
    /// </code>
    /// </example>
    /// <seealso cref="DecryptStringFromBytes_Aes"/>
    public static byte[] EncryptStringToBytes_Aes(string plainText)
    {
        if (plainText == null || plainText.Length <= 0)
        {
            throw new ArgumentNullException("plainText");
        }
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;
        ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
        using MemoryStream memoryStream = new MemoryStream();
        using CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
        using (StreamWriter streamWriter = new StreamWriter(stream))
        {
            streamWriter.Write(plainText);
        }
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Decrypts an AES-encrypted byte array and returns the original plaintext string.
    /// </summary>
    /// <param name="cipherText">
    /// The encrypted byte array to decrypt. Must not be <see langword="null"/> or empty.
    /// This should be data that was previously encrypted using <see cref="EncryptStringToBytes_Aes"/>.
    /// </param>
    /// <returns>
    /// The decrypted plaintext string. Returns the original string that was passed
    /// to <see cref="EncryptStringToBytes_Aes"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs symmetric decryption using the AES algorithm with the
    /// same <see cref="Key"/> and <see cref="IV"/> used for encryption. The decryption process:
    /// </para>
    /// <list type="number">
    /// <item><description>Creates a new AES algorithm instance</description></item>
    /// <item><description>Configures it with the predefined key and IV</description></item>
    /// <item><description>Creates a decryptor transform</description></item>
    /// <item><description>Reads the ciphertext through a <see cref="CryptoStream"/></description></item>
    /// <item><description>Returns the decrypted string</description></item>
    /// </list>
    /// <para>
    /// <b>Symmetric Requirement:</b> The same key and IV used for encryption must be
    /// used for decryption. Since this class uses fixed values, any data encrypted
    /// with this class can be decrypted by it.
    /// </para>
    /// <para>
    /// <b>Resource Management:</b> All cryptographic resources are properly disposed
    /// after use to prevent resource leaks and ensure secure cleanup of sensitive data.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="cipherText"/> is <see langword="null"/> or has zero length.
    /// </exception>
    /// <exception cref="CryptographicException">
    /// Thrown if decryption fails, which can occur when:
    /// <list type="bullet">
    /// <item><description>The ciphertext was not encrypted with matching key/IV</description></item>
    /// <item><description>The ciphertext has been corrupted or tampered with</description></item>
    /// <item><description>The padding is invalid (indicates wrong key or corrupted data)</description></item>
    /// </list>
    /// </exception>
    /// <example>
    /// <code>
    /// // Retrieve encrypted data from storage
    /// byte[] encryptedData = GetEncryptedDataFromDatabase();
    /// 
    /// // Decrypt to get original string
    /// string originalText = EncryptionUtility.DecryptStringFromBytes_Aes(encryptedData);
    /// 
    /// // Or decrypt from Base64 string
    /// string base64Data = "..."; // Base64-encoded ciphertext
    /// byte[] cipherBytes = Convert.FromBase64String(base64Data);
    /// string decrypted = EncryptionUtility.DecryptStringFromBytes_Aes(cipherBytes);
    /// </code>
    /// </example>
    /// <seealso cref="EncryptStringToBytes_Aes"/>
    public static string DecryptStringFromBytes_Aes(byte[] cipherText)
    {
        if (cipherText == null || cipherText.Length == 0)
        {
            throw new ArgumentNullException("cipherText");
        }
        string text = null;
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;
        ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
        using MemoryStream stream = new MemoryStream(cipherText);
        using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
        using StreamReader streamReader = new StreamReader(stream2);
        return streamReader.ReadToEnd();
    }
}
