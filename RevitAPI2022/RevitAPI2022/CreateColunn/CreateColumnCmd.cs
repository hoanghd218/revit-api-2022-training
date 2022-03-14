using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAPI2022.CreateColunn
{
   [Transaction(TransactionMode.Manual)]
   internal class CreateColumnCmd : IExternalCommand
   {
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {

         TaskDialog.Show("Hello World", "Hello");
         return Result.Succeeded;
      }
   }
}
