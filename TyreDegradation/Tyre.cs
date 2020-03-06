using System;
using System.Collections.Generic;
using System.Text;

namespace TyreDegradation
{
    public class Tyre
    {
        public string Name { set; get; }
        public string Placement { get; set; }
        public string Type { get; set; }
        public string Family { get; set; }
        public int DegredationCoefficient { get; set; }
        public Tyre(string name, string placement, string type, string family, int degredationCoefficient)
        {
            Name = name;
            Placement = placement;
            Type = type;
            Family = family;
            DegredationCoefficient = degredationCoefficient;
        }
    }
}
