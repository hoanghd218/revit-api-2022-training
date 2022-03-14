﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI2022
{
   [Transaction(TransactionMode.Manual)]
   internal class DialogCmd : IExternalCommand
   {
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {

         TaskDialog.Show("Hello World", "Hello");
         return Result.Succeeded;
      }
   }
}
