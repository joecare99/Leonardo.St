using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using BaseLib.Interfaces;
using Leonardo.Models;
using Leonardo.Models.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Leonardo.Models.Tests;

[TestClass]
public sealed class LeonardoClassTests
{
    private static LeonardoClass CreateSut(
        ISteganography? steganography = null,
        IConsole? console = null,
        IHttpClient? httpClient = null)
    {
        var actualSteganography = steganography ?? Substitute.For<ISteganography>();
        var actualConsole = console ?? Substitute.For<IConsole>();
        var actualHttpClient = httpClient ?? Substitute.For<IHttpClient>();

        return new LeonardoClass(actualHttpClient, actualSteganography, actualConsole);
    }

    [TestMethod]
    public void SetConsole_WhenInvoked_PropagatesConsoleToSteganography()
    {
        var steganography = Substitute.For<ISteganography>();
        var originalConsole = Substitute.For<IConsole>();
        var nextConsole = Substitute.For<IConsole>();
        var sut = new LeonardoClass(Substitute.For<IHttpClient>(), steganography, originalConsole);

        sut.SetConsole(nextConsole);

        Assert.AreSame(nextConsole, GetPrivateField<IConsole>(sut, "_console"));
        steganography.Received(1).SetConsole(nextConsole);
    }

    [TestMethod]
    public void PromptAndDecrypt_WhenCalled_LoadsTheImageAndDecryptsTheMessage()
    {
        var steganography = Substitute.For<ISteganography>();
        var sut = new LeonardoClass(Substitute.For<IHttpClient>(), steganography, Substitute.For<IConsole>());

        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.png");
        try
        {
            using (var bitmap = new Bitmap(20, 15))
            {
                bitmap.Save(filePath);
            }

            steganography.Decrypt(Arg.Any<Bitmap>()).Returns("Secret message");

            sut.PromptAndDecrypt(filePath);

            Assert.IsNotNull(sut.PicBoxDECImg);
            Assert.AreEqual(20, sut.PicBoxDECImg!.Width);
            Assert.AreEqual(15, sut.PicBoxDECImg.Height);
            Assert.AreEqual("Secret message", sut.MessageText);
        }
        finally
        {
            sut.PicBoxDECImg?.Dispose();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [TestMethod]
    public void ChangeHue_WhenHueOverflowIsApplied_WrapsAroundTheColorWheel()
    {
        var sut = CreateSut();
        var original = Color.Red;

        var result = InvokePrivate<Color>(sut, "ChangeHue", original, 540d);

        Assert.AreEqual(Color.Cyan.ToArgb(), result.ToArgb());
    }

    [TestMethod]
    public void ColorToHslAndHslToColor_WhenCalled_RoundTripsColorWithinTolerance()
    {
        var sut = CreateSut();
        var original = Color.FromArgb(32, 64, 128);

        var args = new object?[] { original, 0d, 0d, 0d };
        var method = typeof(LeonardoClass).GetMethod("ColorToHSL", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(method);

        method.Invoke(sut, args);

        var hue = (double)args[1]!;
        var saturation = (double)args[2]!;
        var lightness = (double)args[3]!;

        var roundTripped = InvokePrivate<Color>(sut, "HSLToColor", hue, saturation, lightness);

        Assert.AreEqual(original.R, roundTripped.R, 1);
        Assert.AreEqual(original.G, roundTripped.G, 1);
        Assert.AreEqual(original.B, roundTripped.B, 1);
    }

    [TestMethod]
    public void ResizeBitmap_WhenCalled_ResizesTheSourceBitmap()
    {
        var sut = CreateSut();

        using var source = new Bitmap(4, 6);
        var result = InvokePrivate<Bitmap>(sut, "ResizeBitmap", source, 8, 12);

        Assert.AreEqual(8, result.Width);
        Assert.AreEqual(12, result.Height);
    }

    [TestMethod]
    public void ApplyStableDiffusion_WhenCalled_ReturnsBitmapWithSameDimensions()
    {
        var sut = CreateSut();

        var images = new List<Bitmap>
        {
            new Bitmap(5, 4),
            new Bitmap(5, 4)
        };

        try
        {
            var result = InvokePrivate<Bitmap>(sut, "ApplyStableDiffusion", images);

            Assert.AreEqual(5, result.Width);
            Assert.AreEqual(4, result.Height);
        }
        finally
        {
            foreach (var image in images)
            {
                image.Dispose();
            }
        }
    }


    private static T GetPrivateField<T>(object instance, string fieldName)
    {
        var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found on {instance.GetType().FullName}.");
        return (T)field.GetValue(instance)!;
    }

    private static T InvokePrivate<T>(object instance, string methodName, params object[] arguments)
    {
        var method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(method, $"Method '{methodName}' not found on {instance.GetType().FullName}.");

        var result = method.Invoke(instance, arguments);
        return result is null ? default! : (T)result;
    }
}