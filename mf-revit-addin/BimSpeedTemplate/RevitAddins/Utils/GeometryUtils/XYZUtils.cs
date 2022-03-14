using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitApiUtils
{
   public static class XYZUtils
   {
      public static bool IsLeftSide(this XYZ point, Curve curve)
      {
         bool isLeft = false;
         if (point != null && curve != null)
         {
            XYZ start = curve.GetEndPoint(0);
            XYZ end = curve.GetEndPoint(1);
            XYZ dir = end - start;
            XYZ v = point - start;
            double distance = curve.Project(point).Distance;
            double angle = dir.AngleOnPlaneTo(v, XYZ.BasisZ);
            if (distance < 0.01)
            {
               throw new Exception("Point is coincided with curve!");
            }
            if (angle < Math.PI)
            {
               isLeft = true;
            }
         }

         return isLeft;
      }
      public static bool IsVerticalUp(this XYZ vector)
      {
         return vector.Z.IsGreater(0);
      }
      public static bool IsVerticalDown(this XYZ vector)
      {
         return vector.Z.IsSmaller(0);
      }
      public static double AngleBetweenTwoVectors(this XYZ vectorOne, XYZ vectorTwo, bool absolute)
      {
         double num = vectorOne.X * vectorTwo.X + vectorOne.Y * vectorTwo.Y + vectorOne.Z * vectorTwo.Z;

         double num2 = VectorLength(vectorOne) * VectorLength(vectorTwo);

         if (absolute)
         {
            return Math.Acos(Math.Round(num / num2, 6)).RadiansToDegrees();
         }

         return Math.Acos(Math.Round(Math.Abs(num) / num2, 6)).RadiansToDegrees();
      }

      public static double VectorLength(XYZ vector)
      {
         return Math.Sqrt(Math.Pow(vector.X, 2.0) + Math.Pow(vector.Y, 2.0) + Math.Pow(vector.Z, 2.0));
      }
      public static bool IsEqual(this XYZ point1, XYZ point2)
      {
         double length = (point2 - point1).GetLength();
         return length < Constants.EPS;
      }

      public static bool IsNegative(this XYZ p, XYZ q)
      {
         return p.IsParallel(q) && p.DotProduct(q) < 0;
      }

      public static bool IsSameDirection(this XYZ p, XYZ q)
      {
         return p.IsParallel(q) && p.DotProduct(q) > 0;
      }

      public static XYZ FindXyzFromLengthVector(XYZ start, XYZ end, double leng)
      {
         XYZ value = null;
         var vector = (end - start).Normalize();
         value = start.Add(leng * vector);
         return value;
      }

      public static XYZ EditY(this XYZ p, double y)
      {
         return new XYZ(p.X, y, p.Z);
      }

      public static XYZ EditX(this XYZ p, double x)
      {
         return new XYZ(x, p.Y, p.Z);
      }

      private const double PrecisionComparison = 1E-06;

      public static bool IsPerpendicular(this XYZ v, XYZ w)
      {
         return 1E-09 < v.GetLength() && 1E-09 < w.GetLength() && 1E-09 > Math.Abs(v.DotProduct(w));
      }

      public static bool IsParallel(this XYZ p, XYZ q)
      {
         return p.CrossProduct(q).GetLength() < 0.01;
      }

      public static bool IsCodirectionalTo(this XYZ vecThis, XYZ vecTo)
      {
         if (vecTo == null)
         {
            throw new ArgumentNullException();
         }
         return Math.Abs(1.0 - vecThis.Normalize().DotProduct(vecTo.Normalize())) < 1E-06;
      }

      public static XYZ GetClosestPoint(this XYZ pt, List<XYZ> pts)
      {
         XYZ xyz = new XYZ();
         double num1 = 0.0;
         foreach (XYZ pt1 in pts)
         {
            if (!pt.Equals((object)pt1))
            {
               double num2 = Math.Sqrt(Math.Pow(pt.X - pt1.X, 2.0) + Math.Pow(pt.Y - pt1.Y, 2.0) + Math.Pow(pt.Z - pt1.Z, 2.0));
               if (xyz.IsZeroLength())
               {
                  num1 = num2;
                  xyz = pt1;
               }
               else if (num2 < num1)
               {
                  num1 = num2;
                  xyz = pt1;
               }
            }
         }
         return xyz;
      }

      public static bool Iscontains(this XYZ point, List<XYZ> listPoint)
      {
         bool result = false;
         foreach (XYZ item in listPoint)
         {
            if (item.IsAlmostEqualTo(point))
            {
               result = true;
            }
         }
         return result;
      }

      public static XYZ LastPointByDirection(this XYZ direction, List<XYZ> points)
      {
         var max = double.MinValue;
         var p = points.FirstOrDefault();
         foreach (var point in points)
         {
            var m = point.DotProduct(direction);
            if (m > max)
            {
               max = m;
               p = point;
            }
         }
         return p;
      }

      public static XYZ FirstPointByDirection(this XYZ direction, List<XYZ> points)
      {
         var min = double.MaxValue;
         var p = points.FirstOrDefault();
         foreach (var point in points)
         {
            var m = point.DotProduct(direction);
            if (m < min)
            {
               min = m;
               p = point;
            }
         }
         return p;
      }

      public static bool IsOppositeDirectionTo(this XYZ vecThis, XYZ vecTo)
      {
         return DoubleUtils.IsEqual(-1.0, vecThis.Normalize().DotProduct(vecTo.Normalize()));
      }

      public static bool IsOrthogonalTo(this XYZ vecThis, XYZ vecTo)
      {
         return DoubleUtils.IsEqual(0.0, vecThis.Normalize().DotProduct(vecTo.Normalize()));
      }

      public static bool IsHorizontal(this XYZ vecThis)
      {
         return vecThis.IsPerpendicular(XYZ.BasisZ);
      }

      public static bool IsHorizontal(this XYZ vecThis, View view)
      {
         return vecThis.IsPerpendicular(view.UpDirection);
      }

      public static bool IsVertical(this XYZ vecThis)
      {
         return vecThis.IsParallel(XYZ.BasisZ);
      }

      public static bool IsVertical(this XYZ vecThis, View view)
      {
         return vecThis.IsPerpendicular(view.RightDirection);
      }

      public static XYZ GetElementCenter(this Element element, View v = null)
      {
         BoundingBoxXYZ boundingBoxXyz = element.get_BoundingBox(v);
         XYZ xYZ = boundingBoxXyz.Max - boundingBoxXyz.Min;
         return new XYZ(boundingBoxXyz.Min.X + xYZ.X / 2.0, boundingBoxXyz.Min.Y + xYZ.Y / 2.0, boundingBoxXyz.Min.Z + xYZ.Z / 2.0);
      }

      public static XYZ RotateRadians(this XYZ v, double radians)
      {
         var ca = Math.Cos(radians);
         var sa = Math.Sin(radians);
         return new XYZ(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y, v.Z);
      }

      public static XYZ RotateDegree(this XYZ v, double degrees)
      {
         return v.RotateRadians(degrees * 0.0174532925);
      }

      public static XYZ Direction(this Curve curve)
      {
         return (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize();
      }

      public static XYZ Direction(this Edge edge)
      {
         var curve = edge.AsCurve();
         return curve.Direction();
      }

      public static XYZ GetClosestPt(XYZ pt, List<XYZ> pts)
      {
         XYZ xYZ = new XYZ();
         double num = 0.0;
         foreach (XYZ pt2 in pts)
         {
            if (!pt.Equals(pt2))
            {
               double num2 = Math.Sqrt(Math.Pow(pt.X - pt2.X, 2.0) + Math.Pow(pt.Y - pt2.Y, 2.0) + Math.Pow(pt.Z - pt2.Z, 2.0));
               if (xYZ.IsZeroLength())
               {
                  num = num2;
                  xYZ = pt2;
               }
               else if (num2 < num)
               {
                  num = num2;
                  xYZ = pt2;
               }
            }
         }
         return xYZ;
      }

      public static XYZ Intersection(Curve c1, Curve c2)
      {
         var l = c1 as Line;
         var ll = c2 as Line;
         IntersectionResultArray resultArray;
         if (Line.CreateBound(l.Origin + 10000.0 * l.Direction, l.Origin - 10000.0 * l.Direction).Intersect((Curve)Line.CreateBound(ll.Origin + 10000.0 * ll.Direction, ll.Origin - 10000.0 * ll.Direction), out resultArray) != SetComparisonResult.Overlap)
            throw new InvalidOperationException("Input lines did not intersect.");
         if (resultArray == null || resultArray.Size != 1)
            throw new InvalidOperationException("Could not extract line intersection point.");
         return resultArray.get_Item(0).XYZPoint;
      }

      public static XYZ Midpoint(this XYZ p, XYZ q)
      {
         return 0.5 * (p + q);
      }

      public static XYZ Midpoint(this Curve curve)
      {
         double endParameter = curve.GetEndParameter(0);
         double num = (curve.GetEndParameter(1) - endParameter) * 0.5;
         num += endParameter;
         return curve.Evaluate(num, false);
      }

      public static XYZ Midpoint(this Edge edge)
      {
         var curve = edge.AsCurve();
         return Midpoint(curve);
      }

      public static List<XYZ> GetExtremePoints(this List<XYZ> points)
      {
         double num2;
         double num = num2 = points[0].X;
         double num4;
         double num3 = num4 = points[0].Y;
         double num6;
         double num5 = num6 = points[0].Z;
         foreach (XYZ xyz in points)
         {
            num2 = Math.Min(xyz.X, num2);
            num4 = Math.Min(xyz.Y, num4);
            num6 = Math.Min(xyz.Z, num6);
            num = Math.Max(xyz.X, num);
            num3 = Math.Max(xyz.Y, num3);
            num5 = Math.Max(xyz.Z, num5);
         }
         XYZ item = new XYZ(num2, num4, num6);
         XYZ item2 = new XYZ(num, num3, num5);
         return new List<XYZ>
            {
                item,
                item2
            };
      }

      public static XYZ ModifyVector(this XYZ vector, double num, XyzEnum e)
      {
         var x = vector.X;
         var y = vector.Y;
         var z = vector.Z;
         if (e == XyzEnum.X)
         {
            x = num;
         }
         if (e == XyzEnum.Y)
         {
            y = num;
         }
         if (e == XyzEnum.Z)
         {
            z = num;
         }
         return new XYZ(x, y, z);
      }

      public static XYZ EditZ(this XYZ p, double z)
      {
         return new XYZ(p.X, p.Y, z);
      }

      public enum XyzEnum
      {
         X,
         Y,
         Z
      }
   }
}