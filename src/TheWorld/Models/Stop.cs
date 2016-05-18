using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheWorld.Models
{
    [Table("Stops")]
    public class Stop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime Arrival { get; set; }
        public int Order { get; set; }

    }
}