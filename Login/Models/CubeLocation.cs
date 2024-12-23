using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Login.Models
{
    public class CubeLocation
    {
        public int ID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string? SessionID { get; set; }
    }
}
