using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitApiUtils
{
   public static class BoundingBoxXYZUtils
   {
      public static BoundingBoxXYZ GetBoundingBoxFromSolid(List<Solid> solids)
      {
         BoundingBoxXYZ finalBbox = null;
         if (solids != null && solids.Count > 0)
         {
            foreach (Solid solid in solids)
            {
               BoundingBoxXYZ sectionBox = solid.GetBoundingBox();
               if (finalBbox == null)
               {
                  finalBbox = new BoundingBoxXYZ();
                  finalBbox.Max = sectionBox.Max;
                  finalBbox.Min = sectionBox.Min;
               }
               else
               {
                  double maxX = finalBbox.Max.X;
                  double maxY = finalBbox.Max.Y;
                  double maxZ = finalBbox.Max.Z;
                  double minX = finalBbox.Min.X;
                  double minY = finalBbox.Min.Y;
                  double minZ = finalBbox.Min.Z;
                  if (sectionBox.Max.X > maxX)
                  {
                     maxX = sectionBox.Max.X;
                  }
                  if (sectionBox.Max.Y > maxY)
                  {
                     maxY = sectionBox.Max.Y;
                  }
                  if (sectionBox.Max.Z > maxZ)
                  {
                     maxZ = sectionBox.Max.Z;
                  }

                  if (sectionBox.Min.X < minX)
                  {
                     minX = sectionBox.Min.X;
                  }
                  if (sectionBox.Min.Y < minY)
                  {
                     minY = sectionBox.Min.Y;
                  }
                  if (sectionBox.Min.Z < minZ)
                  {
                     minZ = sectionBox.Min.Z;
                  }
                  finalBbox.Max = new XYZ(maxX, maxY, maxZ);
                  finalBbox.Min = new XYZ(minX, minY, minZ);
               }
            }
            if (finalBbox != null)
            {
               int step = 1;
               finalBbox.Max += new XYZ(step, step, step);
               finalBbox.Min -= new XYZ(step, step, step);
            }
         }
         return finalBbox;
      }

      public static double Width(this BoundingBoxXYZ bb)
      {
         return bb.Max.X - bb.Min.X;
      }

      public static double Length(this BoundingBoxXYZ bb)
      {
         return bb.Max.Y - bb.Min.Y;
      }

      public static double Height(this BoundingBoxXYZ bb)
      {
         return bb.Max.Z - bb.Min.Z;
      }

      public static double DistAlongDir(this BoundingBoxXYZ bb, XYZ dir)
      {
         XYZ pt = bb.Transform.OfPoint(bb.Max);
         XYZ xyz = bb.Transform.OfPoint(bb.Min);
         BPlane plane = BPlane.CreateByNormalAndOrigin(dir, xyz);
         return Math.Abs(plane.SignedDistanceTo(pt));
      }

      public static XYZ CenterPoint(this BoundingBoxXYZ bb)
      {
         return bb.Min.Add(0.5 * bb.Max.Subtract(bb.Min));
      }

      public static bool IsPointInBox(this BoundingBoxXYZ bb, XYZ ptGlobal, double tol = 0.0)
      {
         XYZ xyz = bb.Transform.Inverse.OfPoint(ptGlobal);
         return xyz.X.IsBetweenEqual(bb.Min.X, bb.Max.X, tol) && xyz.Y.IsBetweenEqual(bb.Min.Y, bb.Max.Y, tol) && xyz.Z.IsBetweenEqual(bb.Min.Z, bb.Max.Z, tol);
      }

      public static bool IsBoxContained(this BoundingBoxXYZ bb, BoundingBoxXYZ bbContained)
      {
         return bb.IsPointInBox(bbContained.Min, 0.0) && bb.IsPointInBox(bbContained.Max, 0.0);
      }

      public static bool Intersects(this BoundingBoxXYZ bb, BoundingBoxXYZ someOtherbb, double tolerance = 0.0001)
      {
         if (bb == null || someOtherbb == null)
         {
            return false;
         }
         XYZ[] boundaryPoints = bb.GetBoundaryPoints();
         if (boundaryPoints == null)
         {
            return false;
         }
         foreach (XYZ ptGlobal in boundaryPoints)
         {
            if (someOtherbb.IsPointInBox(ptGlobal, tolerance))
            {
               return true;
            }
         }
         return false;
      }

      public static XYZ[] GetBoundaryPoints(this BoundingBoxXYZ bb)
      {
         if (bb == null)
         {
            return null;
         }
         XYZ min = bb.Min;
         XYZ max = bb.Max;
         return new XYZ[]
         {
                bb.Transform.OfPoint(new XYZ(min.X, min.Y, min.Z)),
                bb.Transform.OfPoint(new XYZ(min.X, max.Y, min.Z)),
                bb.Transform.OfPoint(new XYZ(min.X, max.Y, max.Z)),
                bb.Transform.OfPoint(new XYZ(min.X, min.Y, max.Z)),
                bb.Transform.OfPoint(new XYZ(max.X, min.Y, min.Z)),
                bb.Transform.OfPoint(new XYZ(max.X, max.Y, min.Z)),
                bb.Transform.OfPoint(new XYZ(max.X, max.Y, max.Z)),
                bb.Transform.OfPoint(new XYZ(max.X, min.Y, max.Z))
         };
      }

      public static void Inflate(this BoundingBoxXYZ bb, double inflateQuantityInFeet, bool inflateX = true, bool inflateY = true, bool inflateZ = true)
      {
         if (bb == null)
         {
            return;
         }
         XYZ xyz = new XYZ(inflateX ? inflateQuantityInFeet : 0.0, inflateY ? inflateQuantityInFeet : 0.0, inflateZ ? inflateQuantityInFeet : 0.0);
         bb.Max += xyz;
         bb.Min -= xyz;
      }

      public static Solid SolidFromBoundingbox(this BoundingBoxXYZ bb)
      {
         var min = bb.Min;
         var max = bb.Max;
         var a = min;
         var b = new XYZ(min.X, max.Y, min.Z);
         var c = new XYZ(max.X, max.Y, min.Z);
         var d = new XYZ(max.X, min.Y, min.Z);
         var ab = a.LineByPoints(b);
         var bc = b.LineByPoints(c);
         var cd = c.LineByPoints(d);
         var da = d.LineByPoints(a);
         var cl = new CurveLoop();
         cl.Append(ab);
         cl.Append(bc);
         cl.Append(cd);
         cl.Append(da);
         return GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { cl }, XYZ.BasisZ, bb.Height());
      }
   }
}