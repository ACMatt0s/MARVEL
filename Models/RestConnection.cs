using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace webapimarvel.Models
{
    public class RestConnection
    {

        private static readonly HttpClient client = new HttpClient();

        public static async Task<T> REST<T>(string MarvelPoint)
        {
            //Limpa todo o cabeçalho do client
            client.DefaultRequestHeaders.Accept.Clear();

            //Accept ContentType
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                );

            client.DefaultRequestHeaders.Add("User-Agent", "Marvel API");

            var stream = client.GetStreamAsync(MarvelPoint);
            var retornoStream = await JsonSerializer.DeserializeAsync<T>(await stream);

            return retornoStream;
        }
    }
}