using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dotnetapp.Models

{
    public class Ride
{
    public int RideID { get; set; }
    public string DepartureLocation { get; set; }
    public string Destination { get; set; }
    public DateTime DateTime { get; set; }
    public int MaximumCapacity { get; set; }
    public List<Commuter> Commuters { get; set; }
}
}