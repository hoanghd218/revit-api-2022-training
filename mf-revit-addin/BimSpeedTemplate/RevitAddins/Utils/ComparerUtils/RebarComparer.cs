using Autodesk.Revit.DB.Structure;
using System.Collections.Generic;

namespace RevitApiUtils.ComparerUtils
{
   public class RebarComparer : IEqualityComparer<Rebar>
   {
      public bool Equals(Rebar x, Rebar y)
      {
         if (x.Id.IntegerValue == y.Id.IntegerValue)
         {
            return true;
         }
         else
         {
            return false;
         }
      }

      public int GetHashCode(Rebar obj)
      {
         return 0;
      }
   }
}