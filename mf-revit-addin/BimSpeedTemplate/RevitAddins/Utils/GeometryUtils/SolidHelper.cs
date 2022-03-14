using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace RevitApiUtils
{
   public static class SolidHelper
   {
      public static Solid CreateSolidFromBoundingBoxOfSolid(this
          Solid inputSolid)
      {
         BoundingBoxXYZ bbox = inputSolid.GetBoundingBox();

         // Corners in BBox coords

         XYZ pt0 = new XYZ(bbox.Min.X, bbox.Min.Y, bbox.Min.Z);
         XYZ pt1 = new XYZ(bbox.Max.X, bbox.Min.Y, bbox.Min.Z);
         XYZ pt2 = new XYZ(bbox.Max.X, bbox.Max.Y, bbox.Min.Z);
         XYZ pt3 = new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z);

         // Edges in BBox coords

         Line edge0 = Line.CreateBound(pt0, pt1);
         Line edge1 = Line.CreateBound(pt1, pt2);
         Line edge2 = Line.CreateBound(pt2, pt3);
         Line edge3 = Line.CreateBound(pt3, pt0);

         // Create loop, still in BBox coords

         List<Curve> edges = new List<Curve>();
         edges.Add(edge0);
         edges.Add(edge1);
         edges.Add(edge2);
         edges.Add(edge3);

         double height = bbox.Max.Z - bbox.Min.Z;

         CurveLoop baseLoop = CurveLoop.Create(edges);

         List<CurveLoop> loopList = new List<CurveLoop>();
         loopList.Add(baseLoop);

         Solid preTransformBox = GeometryCreationUtilities
             .CreateExtrusionGeometry(loopList, XYZ.BasisZ,
                 height);

         Solid transformBox = SolidUtils.CreateTransformed(
             preTransformBox, bbox.Transform);

         return transformBox;
      }

      public static Solid CreateSolidFromBoundingBox(this
          BoundingBoxXYZ bbox)
      {
         // Corners in BBox coords

         XYZ pt0 = new XYZ(bbox.Min.X, bbox.Min.Y, bbox.Min.Z);
         XYZ pt1 = new XYZ(bbox.Max.X, bbox.Min.Y, bbox.Min.Z);
         XYZ pt2 = new XYZ(bbox.Max.X, bbox.Max.Y, bbox.Min.Z);
         XYZ pt3 = new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z);

         // Edges in BBox coords

         Line edge0 = Line.CreateBound(pt0, pt1);
         Line edge1 = Line.CreateBound(pt1, pt2);
         Line edge2 = Line.CreateBound(pt2, pt3);
         Line edge3 = Line.CreateBound(pt3, pt0);

         // Create loop, still in BBox coords

         List<Curve> edges = new List<Curve>();
         edges.Add(edge0);
         edges.Add(edge1);
         edges.Add(edge2);
         edges.Add(edge3);

         double height = bbox.Max.Z - bbox.Min.Z;

         CurveLoop baseLoop = CurveLoop.Create(edges);

         List<CurveLoop> loopList = new List<CurveLoop>();
         loopList.Add(baseLoop);

         Solid preTransformBox = GeometryCreationUtilities
             .CreateExtrusionGeometry(loopList, XYZ.BasisZ,
                 height);

         Solid transformBox = SolidUtils.CreateTransformed(
             preTransformBox, bbox.Transform);

         return transformBox;
      }
   }
}