using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Refit;

namespace CoreBot.Fields
{
    public class CepFields
    {
        [JsonProperty("erro")]
        public bool erro { get; set; }
        [JsonProperty("cep")]
        public string cep { get; set; }
        [JsonProperty("logradouro")]
        public string logradouro { get; set; }
        [JsonProperty("complemento")]
        public string complemento { get; set; }
        [JsonProperty("bairro")]
        public string bairro { get; set; }
        [JsonProperty("localidade")]
        public string localidade { get; set; }
        [JsonProperty("uf")]
        public string uf { get; set; }
        [JsonProperty("unidade")]
        public string unidade { get; set; }
        [JsonProperty("ibge")]
        public string ibge { get; set; }
        [JsonProperty("gia")]
        public string gia { get; set; }

    }


    public interface ICepApiService
    {
        [Get("/ws/{cep}/json")]
        Task<CepFields> GetAddressAsync(string cep);
    }


}
