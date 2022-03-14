using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace RevitApiUtils.ComparerUtils
{
   public class ElementIdComparer : IEqualityComparer<ElementId>
   {
      #region Implementation of IEqualityComparer<in Category>

      public bool Equals(ElementId x, ElementId y)
      {
         if (x == null || y == null) return false;

         return x.IntegerValue.Equals(y.IntegerValue);
      }

      public int GetHashCode(ElementId obj)
      {
         return obj.IntegerValue;
      }

      #endregion Implementation of IEqualityComparer<in Category>
   }

   public class ElementComparer : IEqualityComparer<Element>
   {
      #region Implementation of IEqualityComparer<in Category>

      public bool Equals(Element x, Element y)
      {
         if (x == null || y == null) return false;

         return x.Id.IntegerValue.Equals(y.Id.IntegerValue);
      }

      public int GetHashCode(Element obj)
      {
         return obj.Id.IntegerValue;
      }

      #endregion Implementation of IEqualityComparer<in Category>
   }
}