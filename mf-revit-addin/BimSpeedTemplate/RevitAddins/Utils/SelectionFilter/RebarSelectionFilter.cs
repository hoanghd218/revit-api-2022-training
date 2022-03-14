using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;

namespace RevitApiUtils.SelectionFilter
{
   public class RebarSelectionFilter : ISelectionFilter
   {
      public bool AllowElement(Element element)
      {
         if (element is Rebar)
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