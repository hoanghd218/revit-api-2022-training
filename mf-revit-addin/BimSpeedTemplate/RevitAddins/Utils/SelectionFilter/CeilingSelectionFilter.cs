using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RevitApiUtils.SelectionFilter
{
   public class CeilingSelectionFilter : ISelectionFilter
   {
      public bool AllowElement(Element element)
      {
         if (element is Ceiling)
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