using Newtonsoft.Json;

namespace SaulGoodmanBot;

internal struct ConfigJSON {
    [JsonProperty("Token")]
    public string Token { get; private set; }

    [JsonProperty("Prefix")]
    public string Prefix { get; set; }
}