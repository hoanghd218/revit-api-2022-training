using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RevitApiUtils
{
   public class ViewportSelectionFilter : ISelectionFilter
   {
      public bool AllowElement(Element elem)
      {
         if (elem is Viewport vp)
         {
            if (vp.ViewId.ToElement() is ViewSection)
            {
               return true;
            }
         }
         return false;
      }

      public bool AllowReference(Reference reference, XYZ position)
      {
         return false;
      }
   }
}