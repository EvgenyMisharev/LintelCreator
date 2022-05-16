using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LintelCreator
{
    public class DimensionParameters
    {
        public string Name { get; set; }
        public string ValueString { get; set; }
        public ElementId Id { get; set; }
        public DimensionParameters(string name, string valueString, ElementId id)
        {
            Name = name;
            ValueString = valueString;
            Id = id;
        }
    }
}
