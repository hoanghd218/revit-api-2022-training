using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace RevitApiUtils
{
   public class XYZComparer : IComparer<XYZ>
   {
      int IComparer<XYZ>.Compare(XYZ first, XYZ second)
      {
         if (DoubleUtils.IsEqual(first.Z, second.Z))
         {
            if (!DoubleUtils.IsEqual(first.Y, second.Y))
            {
               if (!first.Y.IsGreater(second.Y))
               {
                  return -1;
               }
               return 1;
            }

            if (first.X.IsEqual(second.X))
            {
               return 0;
            }
            if (first.X.IsGreater(second.X))
            {
               return 1;
            }
            return -1;
         }

         if (!first.Z.IsGreater(second.Z))
         {
            return -1;
         }
         return 1;
      }
   }
}