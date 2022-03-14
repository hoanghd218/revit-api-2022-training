using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System.Collections.Generic;
using System.Linq;

namespace RevitApiUtils
{
   public static class RebarUtils
   {
      public static void SetPartition(List<Rebar> rebars, string partition)
      {
         foreach (Rebar rebar in rebars)
         {
            Parameter partitionPara = rebar.get_Parameter(BuiltInParameter.NUMBER_PARTITION_PARAM);
            partitionPara.Set(partition);
         }
      }
      public static List<Rebar> CopyRebars(List<Rebar> hostRebars, XYZ translation)
      {
         Document doc = hostRebars.FirstOrDefault().Document;
         List<Rebar> copyRebars = new List<Rebar>();

         var copyIds = ElementTransformUtils.CopyElements(doc, hostRebars.Select(x => x.Id).ToList(), translation);

         foreach (ElementId id in copyIds)
         {
            if (doc.GetElement(id) is Rebar rebar)
            {
               copyRebars.Add(rebar);
            }
         }

         return copyRebars;
      }
      public static Rebar CopyRebar(Rebar hostRebar, XYZ translation)
      {
         Document doc = hostRebar.Document;
         Rebar copyRebar = null;
         var copyIds = ElementTransformUtils.CopyElement(doc, hostRebar.Id, translation);

         foreach (ElementId id in copyIds)
         {
            if (doc.GetElement(id) is Rebar rebar)
            {
               copyRebar = rebar;
               break;
            }
         }

         return copyRebar;
      }
      public static void SetSolidRebarIn3DView(this View view, List<Rebar> rebars)
      {
         foreach (Rebar rebar in rebars)
         {
            if (rebar != null)
            {
               rebar.SetUnobscuredInView(view, true);

               if (view is View3D)
               {
                  rebar.SetSolidInView(view as View3D, true);
               }
            }
         }
      }
      public static List<Rebar> GetAllRebarsInHost(this Element host)
      {
         List<Rebar> rebars = new List<Rebar>();

         if (host.IsValidElement())
         {
            RebarHostData rebarHost = RebarHostData.GetRebarHostData(host);
            if (rebarHost.IsValidHost())
            {
               rebars = rebarHost.GetRebarsInHost().ToList();
            }
         }

         return rebars;
      }
      public static List<Curve> GetRebarCurves(this List<Rebar> rebars)
      {
         List<Curve> curves = new List<Curve>();

         int n, nElements = 0, nCurves = 0;

         foreach (Rebar rebar in rebars)
         {
            ++nElements;

            n = rebar.NumberOfBarPositions;

            nCurves += n;

            for (int i = 0; i < n; ++i)
            {
               IList<Curve> centerlineCurves = rebar.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeAllMultiplanarCurves, i);

               // Move the curves to their position.
#if Version2017
               Transform trf = rebar.GetBarPositionTransform(i);

               foreach (Curve c in centerlineCurves)
               {
                  curves.Add(c.CreateTransformed(trf));
               }
#else
               if (rebar.IsRebarShapeDriven())
               {
                  RebarShapeDrivenAccessor accessor = rebar.GetShapeDrivenAccessor();

                  Transform trf = accessor.GetBarPositionTransform(i);

                  foreach (Curve c in centerlineCurves)
                  {
                     curves.Add(c.CreateTransformed(trf));
                  }
               }
               else
               {
                  // This is a Free Form Rebar

                  foreach (Curve c in centerlineCurves)
                  {
                     curves.Add(c);
                  }
               }
#endif
            }
         }
         return curves;
      }

      public static List<Curve> GetRebarCurves(this Rebar rebar)
      {
         List<Curve> curves = new List<Curve>();

         int n = rebar.NumberOfBarPositions;

         for (int i = 0; i < n; ++i)
         {
            IList<Curve> centerlineCurves = rebar.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeAllMultiplanarCurves,
                i);

            // Move the curves to their position.
#if Version2017
            Transform trf = rebar.GetBarPositionTransform(i);

            foreach (Curve c in centerlineCurves)
            {
               curves.Add(c.CreateTransformed(trf));
            }
#else

            if (rebar.IsRebarShapeDriven())
            {
               RebarShapeDrivenAccessor accessor = rebar.GetShapeDrivenAccessor();

               Transform trf = accessor.GetBarPositionTransform(i);

               foreach (Curve c in centerlineCurves)
               {
                  curves.Add(c.CreateTransformed(trf));
               }
            }
            else
            {
               // This is a Free Form Rebar

               foreach (Curve c in centerlineCurves)
               {
                  curves.Add(c);
               }
            }
#endif
         }

         return curves;
      }

      public static bool IsStirrupOrTie(this Rebar rebar)
      {
         return !IsStandardRebar(rebar);
      }

      public static bool IsStandardRebar(this Rebar rebar)
      {
         var styleParam = rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_HOOK_STYLE);
         //return styleParam.AsInteger() == 0;
         return !styleParam.AsValueString().Contains("Tie");
      }

      public static RebarShape GetRebarShape(this Rebar rebar)
      {
#if Version2017
         return rebar.RebarShapeId.ToElement() as RebarShape;
#elif Version2018
         return rebar.GetShapeId().ToElement() as RebarShape;
#else
         Document doc = rebar.Document;
         try
         {
            var shapeId = rebar.GetShapeId();

            return doc.GetElement(shapeId) as RebarShape;
         }
         catch
         {
            try
            {
               return doc.GetElement(rebar.GetAllRebarShapeIds().FirstOrDefault()) as RebarShape;
            }
            catch
            {
            }
         }
         return null;
#endif
      }

      public static XYZ RebarCenterPoint(this Rebar rebar)
      {
         IList<Curve> centerlineCurves = rebar.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0);
         int num = 0;
         Curve curve1 = centerlineCurves.Where(x => x.Direction().IsHorizontal()).OrderByDescending(x => x.Length).FirstOrDefault();
         foreach (Curve curve2 in centerlineCurves)
         {
            if (num == 0)
            {
               curve1 = curve2;
               num = 1;
            }
            if (num != 0 && curve2.Length > curve1.Length)
               curve1 = curve2;
         }
         return (curve1.GetEndPoint(0) + curve1.GetEndPoint(1)) / 2;
      }

      public static bool IsContainRebar(this Rebar rebar, List<Rebar> listRebar)
      {
         bool flag = false;
         foreach (Rebar element in listRebar)
         {
            if (element.Id == rebar.Id)
               flag = true;
         }
         return flag;
      }

      public static IList<Element> GetAllReinforcements(this Document rvtDoc)
      {
         return GetAllRebars(rvtDoc)
             .Concat(GetAllWWMs(rvtDoc))
             .Concat(GetAllRebarInSystemsReinforcements(rvtDoc))
             .Concat(GetAllRebarContainers(rvtDoc)).ToList();
      }

      public static List<Rebar> GetRebarsFromElementIds(this List<ElementId> elements)
      {
         List<Rebar> list = new List<Rebar>();
         foreach (ElementId elementId in elements)
         {
            list.Add(AC.Document.GetElement(elementId) as Rebar);
         }
         return list;
      }

      //----------------------------------------------------
      public static IList<Element> GetAllRebars(Document rvtDoc)
      {
         return new FilteredElementCollector(rvtDoc, AC.ActiveView.Id).OfClass(typeof(Rebar)).ToElements();
      }

      //----------------------------------------------------
      public static IList<Element> GetAllWWMs(Document rvtDoc)
      {
         return new FilteredElementCollector(rvtDoc, rvtDoc.ActiveView.Id).OfClass(typeof(FabricSheet)).ToElements();
      }

      //----------------------------------------------------
      public static IList<Element> GetAllRebarInSystemsReinforcements(Document rvtDoc)
      {
         return new FilteredElementCollector(rvtDoc, rvtDoc.ActiveView.Id).OfClass(typeof(RebarInSystem)).ToElements();
      }

      //----------------------------------------------------
      public static IList<Element> GetAllRebarContainers(Document rvtDoc)
      {
         return new FilteredElementCollector(rvtDoc, rvtDoc.ActiveView.Id).OfClass(typeof(RebarContainer)).ToElements();
      }
      public static IList<Curve> ComputeRebarDrivingCurves(this Rebar rebar)
      {
#if Version2017
         return rebar.ComputeDrivingCurves();
#else
         return rebar.GetShapeDrivenAccessor().ComputeDrivingCurves();
#endif
      }

      public static void SetRebarLayoutAsFixedNumber(this Rebar rebar, int number, double arrayLength, bool barsOnNormalSide,
          bool includeFirstBar,
          bool includeLastBar)
      {
         if (rebar != null && number > 0 && arrayLength > 0)
         {
#if Version2017
            rebar.SetLayoutAsFixedNumber(number, arrayLength, barsOnNormalSide, includeFirstBar, includeLastBar);
#else
            rebar.GetShapeDrivenAccessor().SetLayoutAsFixedNumber(number, arrayLength, barsOnNormalSide, includeFirstBar, includeLastBar);
#endif
         }
      }

      public static void SetRebarLayoutAsMaximumSpacing(this Rebar rebar, double spacing, double arrayLength, bool barsOnNormalSide,
          bool includeFirstBar,
          bool includeLastBar)
      {
         if (rebar != null && spacing > 0 && arrayLength > 0)
         {
#if Version2017
            rebar.SetLayoutAsMaximumSpacing(spacing, arrayLength, barsOnNormalSide, includeFirstBar, includeLastBar);
#else
            rebar.GetShapeDrivenAccessor().SetLayoutAsMaximumSpacing(spacing, arrayLength, barsOnNormalSide, includeFirstBar, includeLastBar);
#endif
         }
      }

      public static void SetRebarLayoutAsMinimumClearSpacing(this Rebar rebar, double spacing, double arrayLength, bool barsOnNormalSide,
          bool includeFirstBar,
          bool includeLastBar)
      {
         if (rebar != null && spacing > 0 && arrayLength > 0)
         {
#if Version2017
            rebar.SetLayoutAsMinimumClearSpacing(spacing, arrayLength, barsOnNormalSide, includeFirstBar, includeLastBar);
#else
            rebar.GetShapeDrivenAccessor().SetLayoutAsMinimumClearSpacing(spacing, arrayLength, barsOnNormalSide, includeFirstBar, includeLastBar);
#endif
         }
      }

      public static void SetRebarLayoutAsNumberWithSpacing(this Rebar rebar, int number, double spacing, bool barsOnNormalSide,
          bool includeFirstBar,
          bool includeLastBar)
      {
         if (rebar != null && number > 0 && spacing > 0)
         {
#if Version2017
            rebar.SetLayoutAsNumberWithSpacing(number, spacing, barsOnNormalSide, includeFirstBar, includeLastBar);
#else
            rebar.GetShapeDrivenAccessor().SetLayoutAsNumberWithSpacing(number, spacing, barsOnNormalSide, includeFirstBar, includeLastBar);
#endif
         }
      }

      public static void SetRebarLayoutAsSingle(this Rebar rebar)
      {
#if Version2017
         rebar.SetLayoutAsSingle();
#else
         rebar.GetShapeDrivenAccessor().SetLayoutAsSingle();
#endif
      }

      public static void RebarScaleToBox(this Rebar rebar, XYZ origin, XYZ xVec, XYZ yVec)
      {
#if Version2017
         rebar.ScaleToBox(origin, xVec, yVec);
#else
         rebar.GetShapeDrivenAccessor().ScaleToBox(origin, xVec, yVec);
#endif
      }

      public static XYZ RebarNormal(this Rebar rebar)
      {
#if Version2017
         return rebar.Normal;
#else
         try
         {
            return rebar.GetShapeDrivenAccessor().Normal;
         }
         catch (Exception e)
         {
            return rebar.GetFreeFormAccessor().GetCustomDistributionPath().First().Direction();
         }

#endif
      }

      public static double RebarArrayLength(this Rebar rebar)
      {
#if Version2017
         return rebar.ArrayLength;
#else
         return rebar.GetShapeDrivenAccessor().ArrayLength;
#endif
      }

      public static bool RebarBarOnNormalSide(this Rebar rebar)
      {
#if Version2017
         return rebar.BarsOnNormalSide;
#else
         return rebar.GetShapeDrivenAccessor().BarsOnNormalSide;
#endif
      }

      public static Transform GetRebarPositionTransform(this Rebar rebar, int i)
      {
#if Version2017
         return rebar.GetBarPositionTransform(i);
#else
         return rebar.GetShapeDrivenAccessor().GetBarPositionTransform(i);
#endif
      }
   }

   public static class TagUtils
   {
      public static IndependentTag CreateIndependentTag(ElementId tagId, ElementId viewId, Reference rf, bool addLeader, TagOrientation orientation, XYZ point)
      {
         IndependentTag tag = null;
#if Version2017
         tag = AC.Document.Create.NewTag(viewId.ToElement() as View, rf.ToElement(), addLeader, TagMode.TM_ADDBY_CATEGORY,
             orientation, point);
         if (tagId != null)
         {
            tag.ChangeTypeId(tagId);
         }
#elif Version2018
         tag = IndependentTag.Create(AC.Document, viewId, rf, addLeader, TagMode.TM_ADDBY_CATEGORY, orientation, point);
         tag.ChangeTypeId(tagId);
#else
         tag = IndependentTag.Create(AC.Document, tagId, viewId, rf, addLeader, orientation, point);
#endif
         return tag;
      }
   }
}