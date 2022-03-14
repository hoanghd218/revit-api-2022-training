using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RevitApiUtils.SelectionFilter
{
   public class ScopeBoxSelectionFilter : ISelectionFilter
   {
      public bool AllowElement(Element element)
      {
         if (element.Category.ToBuiltinCategory() == BuiltInCategory.OST_VolumeOfInterest)
         {
            return true;
         }
         return false;
      }

      public bool AllowReference(Reference refer, XYZ point)
      {
         return false;
      }
   }
}