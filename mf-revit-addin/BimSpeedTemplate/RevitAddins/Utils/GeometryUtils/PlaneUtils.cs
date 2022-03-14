using Autodesk.Revit.DB;
using System;

namespace RevitApiUtils
{
   public static class PlaneUtils
   {
      public static double DistanceTo(this Plane plane, XYZ p)
      {
         double distance = 0;
#if Version2017
         XYZ projectPoint = plane.ProjectOnto(p);
         distance = p.DistanceTo(projectPoint);
#else
         UV uv = null;
         plane.Project(p, out uv, out distance);
#endif
         return distance;
      }
      private static double SignedDistanceToPlaneReal(this Plane plane, XYZ p)
      {
         XYZ v = p - plane.Origin;
         return plane.Normal.DotProduct(v);
      }

      public static double SignedDistanceToPlaneReal(this BPlane plane, XYZ p)
      {
         XYZ v = p - plane.Origin;
         return plane.Normal.DotProduct(v);
      }

      private static double SignedDistanceTo(this Plane plane, XYZ p)
      {
         XYZ v = p - plane.Origin;
         return Math.Abs(plane.Normal.DotProduct(v));
      }

      public static double SignedDistanceTo(this BPlane plane, XYZ p)
      {
         XYZ v = p - plane.Origin;
         return Math.Abs(plane.Normal.DotProduct(v));
      }

      private static double SignedDistanceTo(this Plane plane, XYZ p, bool minus)
      {
         XYZ v = p - plane.Origin;
         if (minus)
         {
            return plane.Normal.DotProduct(v);
         }
         return Math.Abs(plane.Normal.DotProduct(v));
      }

      public static double SignedDistanceTo(this BPlane plane, XYZ p, bool minus)
      {
         XYZ v = p - plane.Origin;
         if (minus)
         {
            return plane.Normal.DotProduct(v);
         }
         return Math.Abs(plane.Normal.DotProduct(v));
      }

      //----------------------------------------------------
      public static XYZ ProjectOnto(this Plane plane, XYZ p)
      {
         XYZ v = p - plane.Origin;
         double d = plane.Normal.DotProduct(v);
         XYZ q = p - d * plane.Normal;
         return q;
      }

      public static XYZ ProjectOnto(this BPlane plane, XYZ p)
      {
         XYZ v = p - plane.Origin;
         double d = plane.Normal.DotProduct(v);
         XYZ q = p - d * plane.Normal;
         return q;
      }

      private static XYZ ProjectOnto(this XYZ p, Plane plane)
      {
         XYZ v = p - plane.Origin;
         double d = plane.Normal.DotProduct(v);
         XYZ q = p - d * plane.Normal;
         return q;
      }

      public static XYZ ProjectOnto(this XYZ p, BPlane plane)
      {
         XYZ v = p - plane.Origin;
         double d = plane.Normal.DotProduct(v);
         XYZ q = p - d * plane.Normal;
         return q;
      }

      //----------------------------------------------------
      private static bool IsPointOnPlane(this Plane plane, XYZ point)
      {
         return Math.Abs(plane.SignedDistanceTo(point)) < 0.0001;
      }

      public static bool IsPointOnPlane(this BPlane plane, XYZ point)
      {
         return Math.Abs(plane.SignedDistanceTo(point)) < 0.0001;
      }

      //----------------------------------------------------

      private static Plane ToPlane(this PlanarFace planarFace, Transform transform = null)
      {
         if (transform == null)
         {
            transform = Transform.Identity;
         }
         return Plane.CreateByNormalAndOrigin(transform.OfVector(planarFace.FaceNormal), transform.OfPoint(planarFace.Origin));
      }

      public static BPlane ToBPlane(this PlanarFace planarFace, Transform transform = null)
      {
         if (transform == null)
         {
            transform = Transform.Identity;
         }

         return BPlane.CreateByNormalAndOrigin(transform.OfVector(planarFace.FaceNormal), transform.OfPoint(planarFace.Origin));
      }

      public static Plane ToPlane(this View view)
      {
         return Plane.CreateByNormalAndOrigin(view.ViewDirection, view.Origin);
      }

      private static BPlane ToPlane(this ViewSection view)
      {
         return BPlane.CreateByNormalAndOrigin(view.ViewDirection, view.Origin);
      }

      public static BPlane ToBPlane(this View view)
      {
         return BPlane.CreateByNormalAndOrigin(view.ViewDirection, view.Origin);
      }

      public static XYZ ProjectPoint(this BPlane plane, XYZ point)
      {
         double num = plane.SignedDistanceTo(point);
         return point - num * plane.Normal;
      }

      private static XYZ TransformAndProjectPoint(this Plane plane, XYZ point, Transform transform)
      {
         XYZ point2 = point;
         if (transform != null)
         {
            point2 = transform.OfPoint(point);
         }
         return plane.ProjectOnto(point2);
      }

      public static XYZ TransformAndProjectPoint(this BPlane plane, XYZ point, Transform transform)
      {
         XYZ point2 = point;
         if (transform != null)
         {
            point2 = transform.OfPoint(point);
         }
         return plane.ProjectPoint(point2);
      }

      public static UV ProjectPointToUV(this Plane plane, XYZ point, Transform transform = null)
      {
         XYZ point2 = plane.TransformAndProjectPoint(point, transform);
         return plane.ConvertXYZToUV(point2);
      }

      public static UV ConvertXYZToUV(this Plane plane, XYZ point)
      {
         double num = point.DotProduct(plane.XVec);
         double num2 = point.DotProduct(plane.YVec);
         return new UV(num, num2);
      }

      public static XYZ ConvertUVToXYZ(this Plane plane, UV point)
      {
         return point.U * plane.XVec + point.V * plane.YVec + plane.Normal * plane.Normal.DotProduct(plane.Origin);
      }
   }

   public class BPlane
   {
      public XYZ Normal { get; set; }
      public XYZ Origin { get; set; }
      public XYZ XVec { get; set; }
      public XYZ YVec { get; set; }

      public BPlane()
      {
      }

      public BPlane(Plane plane)
      {
         Normal = plane.Normal;
         Origin = plane.Origin;
      }

      public BPlane(XYZ normal, XYZ origin)
      {
         Normal = normal.Normalize();
         Origin = origin;
      }

      public static BPlane CreateByNormalAndOrigin(XYZ normal, XYZ origin)
      {
         return new BPlane(normal, origin);
      }

      public static BPlane CreateByThreePoints(XYZ p1, XYZ p2, XYZ p3)
      {
         var v1 = p1 - p2;
         var v2 = p2 - p3;
         return new BPlane(v1.CrossProduct(v2).Normalize(), p1);
      }

      public Plane ToPlane()
      {
         return Plane.CreateByNormalAndOrigin(Normal, Origin);
      }
   }
}