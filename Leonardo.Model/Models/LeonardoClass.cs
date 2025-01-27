using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Leonardo.Models.Interfaces;
using BaseLib.Interfaces;

namespace Leonardo.Models;

public partial class LeonardoClass : ObservableObject , ILeonardoClass
{
    private readonly IHttpClient _httpClient;
    private readonly ISteganography _steganography;
    private IConsole _console;
    private readonly ILeonardoSettings appSettings;

    [ObservableProperty]
    private Bitmap? _picBoxDECImg;

    [ObservableProperty]
    private string _messageText = "< >";

    [ObservableProperty]
    private ECursor _cursorCurrent;

    public Func<string,string?>? SaveFileQuery { get; set; }
    public Action<string>? MessageBoxShow { get; set; }
    public Func<string, string>? InputQuery { get; set; }
    public Action? ShowGeneratingMessage { get; set; }
    public Action? HideGeneratingMessage { get; set; }
    public async Task<List<byte[]>> GetDogImages(string breed, int maxImages)
    {
        List<byte[]> imageDatas = new List<byte[]>();
        try
        {
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true,
                MustRevalidate = true,
                MaxAge = TimeSpan.Zero
            };
            string text = $"https://dog.ceo/api/breed/{breed}/images";
            var obj = await _httpClient.GetAsync(text);
            obj.EnsureSuccessStatusCode();
            JObject val = JObject.Parse(await obj.Content.ReadAsStringAsync());
            List<string>? imageUrls = val["message"]?.ToObject<List<string>>();
            int numImagesToFetch = Math.Min(maxImages, imageUrls?.Count ?? 0);
            for (int i = 0; i < numImagesToFetch; i++)
            {
                imageDatas.Add(await DownloadImage(imageUrls![i]));
            }
            return imageDatas;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching dog images: " + ex.Message);
        }
    }

    private Size GetImageSize(byte[] imageData)
    {
        using MemoryStream stream = new MemoryStream(imageData);
        using Bitmap bitmap = new Bitmap(stream);
        return new Size(bitmap.Width, bitmap.Height);
    }

    private Bitmap ResizeImage(byte[] imageData, Size targetSize)
    {
        using MemoryStream stream = new MemoryStream(imageData);
        using Bitmap original = new Bitmap(stream);
        return new Bitmap(original, targetSize);
    }

    private async Task<byte[]> DownloadImage(string imageUrl)
    {
        try
        {
            IHttpClient httpClient = Ioc.Default.GetRequiredService<IHttpClient>();
            try
            {
                _console.WriteLine($"Downloading image from URL: {imageUrl}");
                return await httpClient.GetByteArrayAsync(imageUrl);
            }
            finally
            {
                ((IDisposable)httpClient)?.Dispose();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error downloading image: " + ex.Message);
        }
    }

    private Bitmap ResizeBitmap(Bitmap bitmap, int width, int height)
    {
        try
        {
            _console.WriteLine($"Before resizing - Width: {bitmap.Width}, Height: {bitmap.Height}");
            Bitmap bitmap2 = new Bitmap(bitmap, new Size(width, height));
            _console.WriteLine($"After resizing - Width: {bitmap2.Width}, Height: {bitmap2.Height}");
            return bitmap2;
        }
        catch (Exception ex)
        {
            _console.WriteLine("Error resizing bitmap: " + ex.Message);
            throw;
        }
    }

    private Bitmap ApplyStableDiffusion(List<Bitmap> bitmaps)
    {
        Random random = new Random();
        int num = bitmaps[0].Width;
        int num2 = bitmaps[0].Height;
        Bitmap bitmap = new Bitmap(num, num2);
        string text = ((random.Next(2) == 0) ? "average" : "weighted");
        for (int i = 0; i < num2; i++)
        {
            for (int j = 0; j < num; j++)
            {
                int num3 = 0;
                int num4 = 0;
                int num5 = 0;
                foreach (Bitmap bitmap2 in bitmaps)
                {
                    Color pixel = bitmap2.GetPixel(j, i);
                    num3 += pixel.R;
                    num4 += pixel.G;
                    num5 += pixel.B;
                }
                int red;
                int green;
                int blue;
                if (text == "average")
                {
                    red = num3 / bitmaps.Count;
                    green = num4 / bitmaps.Count;
                    blue = num5 / bitmaps.Count;
                }
                else
                {
                    int count = bitmaps.Count;
                    int[] source = (from _ in Enumerable.Range(1, count)
                                    select random.Next(256)).ToArray();
                    int num6 = source.Sum();
                    red = (int)Math.Round((double)num3 * source.Average() / (double)num6);
                    green = (int)Math.Round((double)num4 * source.Average() / (double)num6);
                    blue = (int)Math.Round((double)num5 * source.Average() / (double)num6);
                }
                Color color = ChangeHue(Color.FromArgb(red, green, blue), random.NextDouble() * 360.0);
                bitmap.SetPixel(j, i, color);
            }
        }
        return bitmap;
    }

    private Color ChangeHue(Color color, double hueChange)
    {
        ColorToHSL(color, out var hue, out var saturation, out var lightness);
        hue += hueChange;
        if (hue < 0.0)
        {
            hue += 360.0;
        }
        else if (hue >= 360.0)
        {
            hue -= 360.0;
        }
        return HSLToColor(hue, saturation, lightness);
    }

    private void ColorToHSL(Color color, out double hue, out double saturation, out double lightness)
    {
        double num = (double)(int)color.R / 255.0;
        double num2 = (double)(int)color.G / 255.0;
        double num3 = (double)(int)color.B / 255.0;
        double num4 = Math.Max(num, Math.Max(num2, num3));
        double num5 = Math.Min(num, Math.Min(num2, num3));
        double num6 = num4 - num5;
        hue = 0.0;
        if (num6 != 0.0)
        {
            if (num4 == num)
            {
                hue = 60.0 * ((num2 - num3) / num6 % 6.0);
            }
            else if (num4 == num2)
            {
                hue = 60.0 * ((num3 - num) / num6 + 2.0);
            }
            else if (num4 == num3)
            {
                hue = 60.0 * ((num - num2) / num6 + 4.0);
            }
        }
        lightness = (num4 + num5) / 2.0;
        saturation = ((num6 == 0.0) ? 0.0 : (num6 / (1.0 - Math.Abs(2.0 * lightness - 1.0))));
    }

    private Color HSLToColor(double hue, double saturation, double lightness)
    {
        double num = (1.0 - Math.Abs(2.0 * lightness - 1.0)) * saturation;
        double num2 = num * (1.0 - Math.Abs(hue / 60.0 % 2.0 - 1.0));
        double num3 = lightness - num / 2.0;
        double num4;
        double num5;
        double num6;
        if (hue >= 0.0 && hue < 60.0)
        {
            num4 = num;
            num5 = num2;
            num6 = 0.0;
        }
        else if (hue >= 60.0 && hue < 120.0)
        {
            num4 = num2;
            num5 = num;
            num6 = 0.0;
        }
        else if (hue >= 120.0 && hue < 180.0)
        {
            num4 = 0.0;
            num5 = num;
            num6 = num2;
        }
        else if (hue >= 180.0 && hue < 240.0)
        {
            num4 = 0.0;
            num5 = num2;
            num6 = num;
        }
        else if (hue >= 240.0 && hue < 300.0)
        {
            num4 = num2;
            num5 = 0.0;
            num6 = num;
        }
        else
        {
            num4 = num;
            num5 = 0.0;
            num6 = num2;
        }
        num4 = (num4 + num3) * 255.0;
        num5 = (num5 + num3) * 255.0;
        num6 = (num6 + num3) * 255.0;
        return Color.FromArgb((int)num4, (int)num5, (int)num6);
    }

    public async Task HuggingRequest()
    {
        IHttpClient val = Ioc.Default.GetRequiredService<IHttpClient>();
        string text = "https://api-inference.huggingface.co/models/Maheshmarathe/my-favourite-dog";
        string text2 = "{\"inputs\": \"Corgi with a banana hat\"}";
        string text3 = appSettings.Get(ELSetting.ApiToken);
        try
        {
            HttpRequestMessage val2 = new HttpRequestMessage(HttpMethod.Post, text);
            ((HttpHeaders)val2.Headers).Add("Authorization", "Bearer " + text3);
            val2.Content = (HttpContent)new StringContent(text2, Encoding.UTF8, "application/json");
            HttpResponseMessage val3 = await val.SendAsync(val2);
            if (val3.IsSuccessStatusCode)
            {
                byte[] bytes = await val3.Content.ReadAsByteArrayAsync();
                string text4 = "image.jpg";
                File.WriteAllBytes(text4, bytes);
                _console.WriteLine("saved to: " + text4);
            }
            else
            {
                _console.WriteLine("Request failed with status code: " + val3.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _console.WriteLine("Error: " + ex.Message);
        }
    }

    public async Task HuggingRequest2ENC(string inputString)
    {
        CursorCurrent = ECursor.WaitCursor;
        IHttpClient val = Ioc.Default.GetRequiredService<IHttpClient>();
        string text = "https://api-inference.huggingface.co/models/Maheshmarathe/my-favourite-dog";
        string text2 = $"{{\"inputs\": \"{inputString}\"}}";
        string obj = appSettings.Get(ELSetting.ApiToken);
        SecureString secureString = new SecureString();
        string text3 = obj;
        if (obj != null)
        foreach (char c in text3)
        {
            secureString.AppendChar(c);
        }
        try
        {
            HttpRequestMessage val2 = new HttpRequestMessage(HttpMethod.Post, text);
            if (secureString != null)
            {
                IntPtr intPtr = IntPtr.Zero;
                string text4 = null;
                try
                {
                    intPtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                    text4 = Marshal.PtrToStringUni(intPtr);
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
                }
                ((HttpHeaders)val2.Headers).Add("Authorization", "Bearer " + text4);
            }
            else
            {
                CursorCurrent = ECursor.Default;
                MessageBoxShow("Token parsing failed");
            }
            val2.Content = (HttpContent)new StringContent(text2, Encoding.UTF8, "application/json");
            HttpResponseMessage val3 = await val.SendAsync(val2);
            ShowGeneratingMessage();
            if (val3.IsSuccessStatusCode)
            {
                byte[] bytes = await val3.Content.ReadAsByteArrayAsync();
                string text5 = "mskxnsdknfo30d821jx93x29138x10.jpg";
                CursorCurrent = ECursor.Default;
                File.WriteAllBytes(text5, bytes);
                _console.WriteLine("saved to: " + text5);
                using (Bitmap image = new Bitmap(text5))
                {
                    PromptAndEncryptDOG(image);
                }
                File.Delete(text5);
                _console.WriteLine("president secured.");
            }
            else
            {
                _console.WriteLine("Request failed with status code: " + val3.StatusCode);
                MessageBoxShow("Sorry... \n There was a server connection issue: " + val3.StatusCode.ToString() + " \n This is common when first starting up. \n Please restart the application and try again");
            }
        }
        catch (Exception ex)
        {
            _console.WriteLine("Error: " + ex.Message);
            MessageBoxShow(ex.ToString());
        }
        finally
        {
            HideGeneratingMessage();
        }
    }

    public LeonardoClass(IHttpClient httpClient, ISteganography steganography,IConsole console)
    {
        _httpClient = httpClient;
        _steganography = steganography;
        _console = console;
    }

    public void SetConsole(IConsole console)
    {
        _console = console;
        _steganography.SetConsole(console);
    }
    public void PromptAndEncrypt(string imagePath)
    {
        string inputString = InputQuery?.Invoke("Enter string to encrypt");
        Bitmap bitmap = _steganography.Encrypt(new Bitmap(imagePath), inputString);

        string? fileName;
        if (null != (fileName = SaveFileQuery?.Invoke("Image Files (*.png, *.jpg)|*.png;*.jpg;")) )
        {
            bitmap.Save(fileName);
            MessageBoxShow?.Invoke("Encryption completed successfully! Image saved at: " + fileName);
        }
    }

    private void PromptAndEncryptDOG(Bitmap image)
    {
        string inputString = InputQuery?.Invoke("Enter string to encrypt");
        Bitmap bitmap = _steganography.Encrypt(image, inputString);

        string? fileName;
        if (null != (fileName = SaveFileQuery?.Invoke("Image Files (*.png, *.jpg)|*.png;*.jpg;")))
        {
            bitmap.Save(fileName);
            MessageBoxShow("Encryption completed successfully! Image saved at: " + fileName);
        }
    }

    private void PromptAndEncryptAI(Bitmap image)
    {
        string inputString = InputQuery?.Invoke("Enter string to encrypt");
        Bitmap bitmap = _steganography.Encrypt(image, inputString);
        string? fileName;
        if (null != (fileName = SaveFileQuery?.Invoke("Image Files (*.png, *.jpg)|*.png;*.jpg;")))
        {
            bitmap.Save(fileName);
            MessageBoxShow("Encryption completed successfully! Image saved at: " + fileName);
        }
    }

    public void PromptAndDecrypt(string fileName)
    {
        Bitmap image = new Bitmap(fileName);
            PicBoxDECImg = image;
        string text = _steganography.Decrypt(image);
        MessageText = text;

    }
}
