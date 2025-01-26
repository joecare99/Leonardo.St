using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Leonardo.Models.Interfaces;

public interface IHttpClient
{
    HttpRequestHeaders DefaultRequestHeaders { get; }

    Task<HttpResponseMessage> GetAsync(string text);
    Task<byte[]> GetByteArrayAsync(string imageUrl);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage val2);
}