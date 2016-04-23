using System;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips")]
    public class TripController : Controller
    {
        public TripController(IWorldRepository repository, ILogger<TripController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]
        public JsonResult Get()
        {
            try
            {
                var trips = _repository.GetUserTripsWithStops(User.Identity.Name);
                var results = Mapper.Map<IEnumerable<TripViewModel>>(trips);

                return Json(new { Message = "Success", Trips = results });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve trips for user \"{User.Identity.Name}\"", ex);
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                return Json(new { Message = "Failed", Trips = new object[] {}, Exception = ex });
            }
        }

        [HttpGet("{tripName}")]
        public JsonResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetTripByName(tripName, User.Identity.Name);
                if (trip == null)
                {
                    _logger.LogError($"Trip \"{tripName}\", not found for user \"{User.Identity.Name}\"");
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Message = "Failed", Trip = new { } });
                }

                var result = Mapper.Map<TripViewModel>(trip);

                return Json(new { Message = "Success", Trip = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve trip {tripName} for user: {User.Identity.Name}", ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { Message = "Failed", Trip = new { }, Exception = ex });
            }
        }

        [HttpPost("")]
        public JsonResult Post([FromBody] TripViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newTrip = Mapper.Map<Trip>(vm);
                    newTrip.UserName = User.Identity.Name;

                    _logger.LogInformation("Attempting to save a new trip...");
                    _repository.AddTrip(newTrip);

                    if (_repository.SaveAll())
                    {
                        var result = Mapper.Map<TripViewModel>(newTrip);
                        Response.StatusCode = (int) HttpStatusCode.Created;
                        return Json(new { Message = "Success", CreatedTrip = result });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save a new trip.", ex);
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                return Json(new { Message = "Failed", CreatedTrip = new { }, Exception = ex });
            }

            _logger.LogError("Validation failed on new trip.");
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Validation Failed", CreatedTrip = new { }, ModelState = ModelState });
        }

        #region Fields

        private IWorldRepository _repository;
        private ILogger<TripController> _logger;

        #endregion
    }
}