using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace TheWorld.Services
{
    public class BingCoordService : ICoordService
    {
        public BingCoordService(ILogger<BingCoordService> logger)
        {
            _logger = logger;
        }

        #region Implementation of ICoordService

        public async Task<CoordServiceResult> Lookup(string location)
        {
            var result = new CoordServiceResult
            {
                Success = false,
                Message = "Undetermined failure while looking up coordinates"
            };

            // Lookup coordinates
            var bingKey = Startup.Configuration["AppSettings:BingKey"];
            var encodedName = WebUtility.UrlDecode(location);
            var url = $"http://dev.virtualearth.net/REST/v1/Locations?q={encodedName}&key={bingKey}";

            var client = new HttpClient();
            var json = await client.GetStringAsync(url);

            #region Bing Parsing Code

            var results = JObject.Parse(json);
            var resources = results["resourceSets"][0]["resources"];
            if (!resources.HasValues)
            {
                result.Message = $"Could not find '{location}' as a location";
            }
            else
            {
                var confidence = (string) resources[0]["confidence"];
                if (confidence != "High")
                {
                    result.Message = $"Could not find a confident match for '{location}' as a location";
                }
                else
                {
                    var coords = resources[0]["geocodePoints"][0]["coordinates"];
                    result.Latitude = (double)coords[0];
                    result.Longitude = (double)coords[1];
                    result.Success = true;
                    result.Message = "Success";
                }
            }

            #endregion

            return result;
        }

        #endregion

        #region Fields

        private ILogger<BingCoordService> _logger;

        #endregion
    }
}