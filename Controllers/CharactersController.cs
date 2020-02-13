using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using webapimarvel.Models;
using System.Web.Http;
using static webapimarvel.Models.GeraHash;
using System.Text;
using System.Security.Cryptography;
using WebApiMarvel.Models;
using Newtonsoft.Json;

namespace WebApiMarvel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CharactersController : ControllerBase
    {

        [HttpGet]
        public async Task<object> SerializeCharacters()
        {
            List<Characters> ListCharacters = new List<Characters>();

            await SerializeCharacters(ListCharacters, "http://gateway.marvel.com/v1/public/characters");

            foreach (var people in ListPeople)
            {
                var especie = await SerializeEspecie(people.Species.FirstOrDefault());

                people.Species = new List<string>()
                {
                    especie
                };

                if (string.IsNullOrWhiteSpace(people.Mass) || people.Mass.ToLower() == "unknown")
                    people.Mass = "0";
            }

            List<Characters> Characters = new List<Characters>();
            Characters.Clear();
            Characters = ListPeople.Where((x => x.Species.Contains("Human"))).ToList();

            var Retorno = new
            {
                QuantHumanos = Characters.Count(),
                PersonagensHumanos = Characters.OrderBy(x => x.Name),
                PesoFinal = Characters.Sum(x => Double.Parse(x.Mass, CultureInfo.InvariantCulture)),
                MediaDePeso = Characters.Sum(x => Double.Parse(x.Mass, CultureInfo.InvariantCulture)) / Characters.Count()

            };

            return Retorno;

        }

        private static async Task<string> SerializeCharacters(string endPoint)
        {
            if (string.IsNullOrEmpty(endPoint))
                return "Unknown";

            var retornoStream = await REST<Characters>("http://gateway.marvel.com/v1/public/characters");

            return retornoStream.Name;
        }
        
        private static async Task SerializeCharacters(List<Characters> CharactersList, string MarvelPoint)
        {
            var retornoStream = await REST<Characters>(MarvelPoint);

            //CharactersList.AddRange(retornoStream.name);

            if (!string.IsNullOrEmpty(retornoStream.name))
                await SerializeCharacters(CharactersList, retornoStream.name);
        }

    }

}

        // private static Characters MontaUrlHash(HttpClient client, double MarvelKey)
        // {
        //     HttpResponseMessage response = client.GetAsync(
        //                 config.GetSection("MarvelComicsAPI:BaseURL").Value +
        //                 $"characters?ts={ts}&apikey={publicKey}&hash={hash}").Result;

        //     response.EnsureSuccessStatusCode();
        //     string conteudo =
        //         response.Content.ReadAsStringAsync().Result;

        //     dynamic resultado = JsonConvert.DeserializeObject(conteudo);

        //     var characters = new Characters();
        //     characters.id = resultado.data.results[0].id;
        //     characters.name = resultado.data.results[0].name;

        //     return characters;
        // }
        // private string GerarHash(string ts, string publicKey, string privateKey)
        // {
        //     byte[] bytes =
        //         Encoding.UTF8.GetBytes(ts + privateKey + publicKey);
        //     var gerador = MD5.Create();
        //     byte[] bytesHash = gerador.ComputeHash(bytes);
        //     return BitConverter.ToString(bytesHash)
        //         .ToLower().Replace("-", String.Empty);
        // }

                // string ts = DateTime.Now.Ticks.ToString();
        // string publicKey = "8170bdf33d2cf70a32842e289c67a882";
        // string privateKey = "66b323fcfd4260de90c894920c2aee10d099f5f4";
        // string hash = GerarHash(ts, publicKey, privateKey);