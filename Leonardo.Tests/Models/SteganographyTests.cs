using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using BaseLib.Interfaces;
using Leonardo.Models;
using Leonardo.Models.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Leonardo.Models.Tests;

[TestClass]
public class SteganographyTests
{
    [TestMethod]
    public void Constructor_WhenConsoleIsNull_SetsNullField()
    {
        var steganography = new Steganography(null!);

        var field = typeof(Steganography).GetField("_console", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        Assert.IsNull(field!.GetValue(steganography));
    }

    [TestMethod]
    public void SetConsole_WhenConsoleIsNull_SetsInternalConsoleToNull()
    {
        var steganography = new Steganography(Substitute.For<IConsole>());

        steganography.SetConsole(null!);

        var field = typeof(Steganography).GetField("_console", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        Assert.IsNull(field!.GetValue(steganography));
    }

    [TestMethod]
    public void SetConsole_WhenConsoleIsProvided_UpdatesInternalConsole()
    {
        var originalConsole = Substitute.For<IConsole>();
        var replacementConsole = Substitute.For<IConsole>();
        var steganography = new Steganography(originalConsole);

        steganography.SetConsole(replacementConsole);

        var field = typeof(Steganography).GetField("_console", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        Assert.AreSame(replacementConsole, field!.GetValue(steganography));
    }

    [TestMethod]
    public void NormalizeImage_WhenImageIsNull_ThrowsNullReferenceException()
    {
        var steganography = new Steganography(Substitute.For<IConsole>());

        Assert.Throws<NullReferenceException>(() => steganography.NormalizeImage(null!));
    }

    [TestMethod]
    public void NormalizeImage_ShouldCreateCopyAndClearLsbs()
    {
        var steganography = new Steganography(Substitute.For<IConsole>());
        var original = CreateImage(5, 4, Color.FromArgb(255, 123, 45, 67), Color.FromArgb(255, 45, 196, 6));

        var normalized = steganography.NormalizeImage(original);

        Assert.AreNotSame(original, normalized);
        Assert.AreEqual(original.Width, normalized.Width);
        Assert.AreEqual(original.Height, normalized.Height);

        for (var y = 0; y < normalized.Height; y++)
        {
            for (var x = 0; x < normalized.Width; x++)
            {
                var pixel = normalized.GetPixel(x, y);
                Assert.AreEqual(0, pixel.A % 2);
                Assert.AreEqual(0, pixel.R % 2);
                Assert.AreEqual(0, pixel.G % 2);
                Assert.AreEqual(0, pixel.B % 2);
            }
        }

        Assert.AreEqual(255, original.GetPixel(0, 0).A);
        Assert.AreEqual(123, original.GetPixel(0, 0).R);
        Assert.AreEqual(45, original.GetPixel(0, 0).G);
        Assert.AreEqual(67, original.GetPixel(0, 0).B);
    }

    [TestMethod]
    public void ConvertTextToBinary_ShouldConvertAsciiToSpaceSeparatedBinary()
    {
        var steganography = new Steganography(Substitute.For<IConsole>());

        var actual = steganography.ConvertTextToBinary("Hi");

        Assert.AreEqual("01001000 01101001", actual);
    }

    [TestMethod]
    public void ConvertBinaryToMessage_ShouldConvertEightBitBlocksWithoutSpaces()
    {
        var steganography = new Steganography(Substitute.For<IConsole>());

        var actual = steganography.ConvertBinaryToMessage("0100100001101001");

        Assert.AreEqual("Hi", actual);
    }

    [TestMethod]
    public void Decrypt_WhenImageIsValid_UsesCurrentImplementationBehavior()
    {
        var steganography = new Steganography(Substitute.For<IConsole>());
        using var image = CreateImage(2, 2, Color.White, Color.Black);

        var exception = Assert.Throws<ArgumentNullException>(() => steganography.Decrypt(image));
        Assert.AreEqual("cipherText", exception.ParamName);
    }

    [TestMethod]
    public void EncryptAndDecrypt_ShouldRoundTripMessage()
    {
        var steganography = new Steganography(Substitute.For<IConsole>());
        using var originalImage = CreateImage(64, 64, Color.WhiteSmoke, Color.DarkGray);
        var message = "Hello, world! 2026";

        var encodedImage = steganography.Encrypt(originalImage, message);
        var decodedMessage = steganography.Decrypt(encodedImage);
        Assert.AreEqual(message, decodedMessage);
    }

    [TestMethod]
    public void Encrypt_WhenImageIsNull_ThrowsNullReferenceException()
    {
        var steganography = new Steganography(Substitute.For<IConsole>());

        Assert.Throws<NullReferenceException>(() => steganography.Encrypt(null!, "secret"));
    }

    [TestMethod]
    public void Encrypt_WhenMessageIsNull_ThrowsNullReferenceException()
    {
        var steganography = new Steganography(Substitute.For<IConsole>());
        using var image = CreateImage(32, 32, Color.White, Color.Black);

        Assert.Throws<NullReferenceException>(() => steganography.Encrypt(image, null!));
    }

    [TestMethod]
    public void Encrypt_WhenMessageExceedsImageCapacity_ThrowsArgumentOutOfRangeException()
    {
        var steganography = new Steganography(Substitute.For<IConsole>());
        using var image = CreateImage(1, 1, Color.White, Color.Black);
        var hugeMessage = new string('A', 2048);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => steganography.Encrypt(image, hugeMessage));
        Assert.AreEqual("y", exception.ParamName);
    }

    [TestMethod]
    public void Decrypt_WhenImageIsNull_ThrowsNullReferenceException()
    {
        var steganography = new Steganography(Substitute.For<IConsole>());

        Assert.Throws<NullReferenceException>(() => steganography.Decrypt(null!));
    }

    private static Bitmap CreateImage(int width, int height, Color color, Color color2)
    {
        var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(color);
        graphics.FillRectangle(new SolidBrush(color2), width/4, height/4, width / 2, height / 2);
        return bitmap;
    }
}
