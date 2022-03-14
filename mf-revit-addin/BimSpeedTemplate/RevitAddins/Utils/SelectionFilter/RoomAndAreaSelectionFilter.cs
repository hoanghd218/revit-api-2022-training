using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;

namespace RevitApiUtils
{
   public class RoomAndAreaSelectionFilter : ISelectionFilter
   {
      public bool AllowElement(Element elem)
      {
         if (elem is Room || elem is Area)
         {
            return true;
         }
         return false;
      }

      public bool AllowReference(Reference reference, XYZ position)
      {
         return false;
      }
   }
}