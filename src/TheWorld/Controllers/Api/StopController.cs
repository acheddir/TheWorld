using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips/{tripName}/stops")]
    public class StopController : Controller
    {
        public StopController(IWorldRepository repository,
            ICoordService coordService,
            ILogger<StopController> logger)
        {
            _repository = repository;
            _coordService = coordService;
            _logger = logger;
        }

        [HttpGet("")]
        public JsonResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetTripByName(tripName, User.Identity.Name);
                if (trip == null)
                {
                    _logger.LogError($"Trip {tripName}, not found for user {User.Identity.Name}");
                    Response.StatusCode = (int) HttpStatusCode.NotFound;
                    return Json(new { Message = "Failed", Stops = new object[] {} });
                }

                var results = Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.OrderBy(s => s.Order));

                return Json(new { Message = "Success", Stops = results });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get stops in trip {tripName} for user {User.Identity.Name}", ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { Message = "Failed", Stops = new object[] {}, Exception = ex });
            }
        }

        [HttpGet("{stopName}")]
        public JsonResult Get(string tripName, string stopName)
        {
            try
            {
                var trip = _repository.GetTripByName(tripName, User.Identity.Name);
                if (trip == null)
                {
                    _logger.LogError($"Trip \"{tripName}\", not found for user \"{User.Identity.Name}\"");
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Message = "Failed", Stop = new { } });
                }

                var result = Mapper.Map<StopViewModel>(trip.Stops.FirstOrDefault(s => s.Name == stopName));

                return Json(new { Message = "Success", Stop = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get stop \"{stopName}\" in trip \"{tripName}\" for user \"{User.Identity.Name}\"", ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { Message = "Failed", Stops = new { }, Exception = ex });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Post(string tripName, [FromBody] StopViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Map to the entity
                    var newStop = Mapper.Map<Stop>(vm);

                    // Looking up Geocoordinates
                    var coordResult = await _coordService.Lookup(newStop.Name);
                    if (!coordResult.Success)
                    {
                        Response.StatusCode = (int) HttpStatusCode.BadRequest;
                        return Json(new { Message = coordResult.Message });
                    }

                    newStop.Longitude = coordResult.Longitude;
                    newStop.Latitude = coordResult.Latitude;

                    // Save to the Database
                    _repository.AddStop(tripName, User.Identity.Name, newStop);
                    if (_repository.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;
                        return Json(Mapper.Map<StopViewModel>(newStop));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save a new stop for trip {tripName}", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message });
            }

            _logger.LogError("Validation failed on new stop.");
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Validation failed on new stop.", ModelState = ModelState });
        }

        #region Fields

        private IWorldRepository _repository;
        private ILogger<StopController> _logger;
        private ICoordService _coordService;

        #endregion
    }
}