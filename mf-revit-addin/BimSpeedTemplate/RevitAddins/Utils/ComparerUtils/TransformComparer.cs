using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace RevitApiUtils.ComparerUtils
{
   public class TransformComparer : IEqualityComparer<Transform>
   {
      public bool Equals(Transform x, Transform y)
      {
         return (x.Origin - y.Origin).GetLength() < Constants.MINIMUMDISTANCE;
      }

      public int GetHashCode(Transform obj)
      {
         int length = Convert.ToInt32(obj.Origin.GetLength() * 100);
         int hashCode = length.GetHashCode();
         return hashCode;
      }
   }
}