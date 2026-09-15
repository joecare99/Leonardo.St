using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Leonardo.Models.Interfaces;

public interface IHttpClient
{
#if NET5_0_OR_GREATER || NET462
    HttpRequestHeaders DefaultRequestHeaders { get; }

    Task<HttpResponseMessage> GetAsync(string text);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage val2);
#else
    HttpRequestHeader DefaultRequestHeaders { get; }
    Task<HttpWebResponse> GetAsync(string text);
    Task<HttpWebResponse> SendAsync(HttpWebRequest val2);
#endif
    Task<byte[]> GetByteArrayAsync(string imageUrl);
}