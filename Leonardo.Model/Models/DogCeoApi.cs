using BaseLib.Interfaces;
using CommunityToolkit.Mvvm.DependencyInjection;
using Leonardo.Models.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Leonardo.Models;

public class DogCeoApi(IHttpClient _httpClient,IConsole _console)
{
    public async Task<List<byte[]>> GetDogImages(string breed, int maxImages)
    {
        List<byte[]> imageDatas = new List<byte[]>();
        try
        {
#if NET5_0_OR_GREATER || NET462
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
#else
            string text = $"https://dog.ceo/api/breed/{breed}/images";
            var obj = await _httpClient.GetAsync(text);
            JObject val = JObject.Parse(await new StreamReader(obj.GetResponseStream()).ReadToEndAsync());
#endif
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
}
