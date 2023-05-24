using Newtonsoft.Json;

namespace CodeAnalyzerTool.API.ConfigModel;

public class GlobalConfig
{

    [JsonProperty("projectName", Required = Required.Always)]
    public string ProjectName { get; set; }
    [JsonProperty("apiUrl", Required = Required.Default)]
    public string ApiUrl { get; set; }

    [JsonProperty("pluginsPath", Required = Required.Default)]
    public string PluginsPath { get; set; }

    [JsonProperty("plugins", Required = Required.Always)]
    public IEnumerable<PluginConfig> Plugins { get; set; }

    public GlobalConfig( string projectName, string apiUrl, string pluginsPath, IEnumerable<PluginConfig> plugins)
    {
        ProjectName = projectName;
        ApiUrl = apiUrl;
        PluginsPath = pluginsPath;
        Plugins = plugins;
    }
}