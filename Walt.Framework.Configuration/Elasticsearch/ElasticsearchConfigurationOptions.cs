using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Walt.Framework.Service.Elasticsearch;
using Walt.Framework.Service.Kafka;

namespace Walt.Framework.Configuration
{
    public class ElasticsearchConfigurationOptions : IConfigureOptions<ElasticsearchOptions>
    {

        private readonly IConfiguration _configuration;


        public ElasticsearchConfigurationOptions(IConfiguration configuration)
        {
           _configuration=configuration;
        }


        public void Configure(ElasticsearchOptions options)
        {
             System.Diagnostics.Debug.WriteLine("kafka配置类，适配方法。"
             +Newtonsoft.Json.JsonConvert.SerializeObject(options));
        }
    }
}