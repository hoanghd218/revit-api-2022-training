using Autodesk.Revit.DB;

namespace RevitApiUtils
{
   public class SolidData
   {
      public Solid Solid => solid;

      public GeometryInstance GeometryInstance
      {
         get
         {
            return geometryInstance;
         }
      }

      public SolidData(Solid solid, GeometryInstance geometryInstance)
      {
         this.solid = solid;
         this.geometryInstance = geometryInstance;
      }

      private Solid solid;

      private GeometryInstance geometryInstance;
   }
}