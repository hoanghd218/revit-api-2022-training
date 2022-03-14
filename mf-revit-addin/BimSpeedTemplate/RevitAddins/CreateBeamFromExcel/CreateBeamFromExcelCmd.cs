using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAddins.Helpers;
using RevitAddins.CreateBeamFromExcel.View;
using RevitAddins.CreateBeamFromExcel.ViewModel;
using RevitApiUtils;

namespace RevitAddins.CreateBeamFromExcel
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateBeamFromExcelCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            AC.GetInformation(commandData.Application.ActiveUIDocument);
            var vm = new CreateBeamFromExcelViewModel();
            var view = new CreateBeamFromExcelView() { DataContext = vm };
            view.ShowDialog();

            return Result.Succeeded;
        }
    }
}
