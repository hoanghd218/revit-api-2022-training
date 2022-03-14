using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitApiUtils
{
   public class RevitGeometryUtils
   {
      internal static List<Face> GetFacesFromSolid(Solid solid, ElementFacesToUse facesToUse)
      {
         if (facesToUse == ElementFacesToUse.Top)
         {
            return GetTopFacesFromSolid(solid);
         }
         if (facesToUse == ElementFacesToUse.Bottom)
         {
            return GetBottomFacesFromSolid(solid);
         }
         if (facesToUse == ElementFacesToUse.Side)
         {
            return GetSideFacesFromSolid(solid);
         }
         return GetAllFacesFromSolid(solid);
      }

      internal static List<Solid> GetElementSolids(GeometryElement geometryElement)
      {
         List<Solid> list = new List<Solid>();
         foreach (GeometryObject geometryObject in geometryElement)
         {
            if (geometryObject is Solid)
            {
               list.Add(geometryObject as Solid);
            }
            else
            {
               GeometryInstance geometryInstance = geometryObject as GeometryInstance;
               if (geometryInstance != null)
               {
                  foreach (GeometryObject geometryObject2 in geometryInstance.GetInstanceGeometry())
                  {
                     if (geometryObject2 is Solid)
                     {
                        list.Add(geometryObject2 as Solid);
                     }
                  }
               }
            }
         }
         return list;
      }

      internal static List<Solid> GetElementSolids(Element element)
      {
         new List<Solid>();
         Options options = new Options
         {
            ComputeReferences = true,
            DetailLevel = ViewDetailLevel.Fine,
            IncludeNonVisibleObjects = true
         };
         return GetElementSolids(element.get_Geometry(options));
      }

      internal static List<Edge> GetElementEdges(Element element)
      {
         List<Edge> list = new List<Edge>();
         foreach (Solid solid in RevitGeometryUtils.GetElementSolids(element))
         {
            foreach (object obj in solid.Edges)
            {
               Edge item = (Edge)obj;
               list.Add(item);
            }
         }
         return list;
      }

      private static List<Face> GetTopFacesFromSolid(Solid solid)
      {
         List<Face> list = new List<Face>();
         foreach (object obj in solid.Faces)
         {
            PlanarFace planarFace = ((Face)obj) as PlanarFace;
            if (null != planarFace && planarFace.FaceNormal.IsVertical() && DoubleUtils.IsGreater(planarFace.FaceNormal.Z, 0.0))
            {
               list.Add(planarFace);
            }
         }
         return list;
      }

      private static List<Face> GetSideFacesFromSolid(Solid solid)
      {
         return GetAllFacesFromSolid(solid).Except(GetTopFacesFromSolid(solid)).ToList().Except(GetBottomFacesFromSolid(solid)).ToList();
      }

      private static List<Face> GetAllFacesFromSolid(Solid solid)
      {
         List<Face> list = new List<Face>();
         foreach (object obj in solid.Faces)
         {
            Face face = (Face)obj;
            if (face.Area != 0.0)
            {
               list.Add(face);
            }
         }
         return list;
      }

      private static List<Face> GetBottomFacesFromSolid(Solid solid)
      {
         List<Face> list = new List<Face>();
         foreach (object obj in solid.Faces)
         {
            PlanarFace planarFace = ((Face)obj) as PlanarFace;
            if (null != planarFace && planarFace.FaceNormal.IsVertical() && planarFace.FaceNormal.Z.IsSmaller(0.0))
            {
               list.Add(planarFace);
            }
         }
         return list;
      }

      [Flags]
      public enum ElementFacesToUse
      {
         Top = 1,
         Bottom = 2,
         Side = 4,
         All = 7
      }
   }
}