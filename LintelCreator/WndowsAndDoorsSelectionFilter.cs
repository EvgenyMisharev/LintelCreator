using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace LintelCreator
{
    class WndowsAndDoorsSelectionFilter : ISelectionFilter
    {
		public bool AllowElement(Autodesk.Revit.DB.Element elem)
		{
			if (elem is FamilyInstance
				&& (elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Windows)
				|| elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Doors)))
			{
				return true;
			}
			return false;
		}

		public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
		{
			return false;
		}
	}
}
