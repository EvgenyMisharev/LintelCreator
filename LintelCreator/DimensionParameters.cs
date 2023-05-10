using Autodesk.Revit.DB;

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
