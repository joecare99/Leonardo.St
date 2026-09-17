using BaseLib.Interfaces;
using CommunityToolkit.Mvvm.DependencyInjection;
using Leonardo.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Leonardo.Models;

public class HuggingFaceApi(IHttpClient _httpClient, IConsole _console, ILeonardoSettings appSettings, ISteganography _steganography, ILeonardoClass leonardoClass) : IHuggingFaceApi
{

    public async Task HuggingRequest()
    {
        IHttpClient val = Ioc.Default.GetRequiredService<IHttpClient>();
        string text = "https://api-inference.huggingface.co/models/Maheshmarathe/my-favourite-dog";
        string text2 = "{\"inputs\": \"Corgi with a banana hat\"}";
        string text3 = appSettings.Get(ELSetting.ApiToken);
        try
        {
#if NET5_0_OR_GREATER || NET462
            ((HttpHeaders)val.DefaultRequestHeaders).Add("Authorization", "Bearer " + text3);
            HttpRequestMessage val2 = new HttpRequestMessage(HttpMethod.Post, text);
            ((HttpHeaders)val2.Headers).Add("Authorization", "Bearer " + text3);
            val2.Content = (HttpContent)new StringContent(text2, Encoding.UTF8, "application/json");
#else
            HttpWebRequest val2 = (HttpWebRequest)WebRequest.Create(text);
            val2.Headers.Add("Authorization", "Bearer " + text3);
#endif
            var val3 = await val.SendAsync(val2);
#if NET5_0_OR_GREATER || NET462
            if (val3.IsSuccessStatusCode)
#else
            if (val3.StatusCode == HttpStatusCode.OK)
#endif
            {
#if NET5_0_OR_GREATER || NET462
                byte[] bytes = await val3.Content.ReadAsByteArrayAsync();
#else
                using var responseStream = val3.GetResponseStream();
                using var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream);
                byte[] bytes = memoryStream.ToArray();
#endif
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
        leonardoClass.CursorCurrent = ECursor.WaitCursor;
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
                leonardoClass.CursorCurrent = ECursor.Default;
                leonardoClass.MessageBoxShow?.Invoke("Token parsing failed");
            }
            val2.Content = (HttpContent)new StringContent(text2, Encoding.UTF8, "application/json");
            HttpResponseMessage val3 = await val.SendAsync(val2);
            leonardoClass.ShowGeneratingMessage?.Invoke();
            if (val3.IsSuccessStatusCode)
            {
                byte[] bytes = await val3.Content.ReadAsByteArrayAsync();
                string text5 = "mskxnsdknfo30d821jx93x29138x10.jpg";
                leonardoClass.CursorCurrent = ECursor.Default;
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
                leonardoClass.MessageBoxShow?.Invoke("Sorry... \n There was a server connection issue: " + val3.StatusCode.ToString() + " \n This is common when first starting up. \n Please restart the application and try again");
            }
        }
        catch (Exception ex)
        {
            _console.WriteLine("Error: " + ex.Message);
            leonardoClass.MessageBoxShow?.Invoke(ex.ToString());
        }
        finally
        {
            leonardoClass.HideGeneratingMessage?.Invoke();
        }
    }

    private void PromptAndEncryptDOG(Bitmap image)
    {
        string inputString = leonardoClass.InputQuery?.Invoke("Enter string to encrypt");
        Bitmap bitmap = _steganography.Encrypt(image, inputString);

        string? fileName;
        if (null != (fileName = leonardoClass.SaveFileQuery?.Invoke("Image Files (*.png, *.jpg)|*.png;*.jpg;")))
        {
            bitmap.Save(fileName);
            leonardoClass.MessageBoxShow?.Invoke("Encryption completed successfully! Image saved at: " + fileName);
        }
    }

}
