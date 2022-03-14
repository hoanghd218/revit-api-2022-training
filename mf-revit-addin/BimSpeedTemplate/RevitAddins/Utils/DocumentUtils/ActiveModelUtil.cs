using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace RevitApiUtils
{
   public class AC
   {
      public static UIDocument UiDoc;
      public static Document Document;
      public static Application Application;
      public static UIApplication UiApplication;
      public static Autodesk.Revit.UI.Selection.Selection Selection;
      public static View ActiveView;
      public static string Username;
      public static BPlane ViewPlane;
      public static string BimSpeedSettingPath;
      public static string BimSpeedInstallPath;
      public static string CurrentCommand;
      public static Dictionary<string, string> DicCommandYoutubeLink;
      public static string Version;
      private static ExternalEvent externalEvent;
      private static ExternalEventHandler externalEventHandler;
      private static ExternalEventHandlers externalEventHandlers;
      public static string ErrorLog = "";

      public static ExternalEvent ExternalEvent
      {
         get
         {
            if (externalEvent == null)
            {
               externalEvent = ExternalEvent.Create(ExternalEventHandler);
            }
            return externalEvent;
         }
         set => externalEvent = value;
      }

      public static ExternalEventHandler ExternalEventHandler
      {
         get
         {
            if (externalEventHandler == null)
            {
               externalEventHandler = new ExternalEventHandler();
            }
            return externalEventHandler;
         }
         set => externalEventHandler = value;
      }

      public static ExternalEventHandlers ExternalEventHandlers
      {
         get
         {
            if (externalEventHandlers == null)
            {
               externalEventHandlers = new ExternalEventHandlers();
            }
            return externalEventHandlers;
         }
         set => externalEventHandlers = value;
      }

      public static void GetInformation(UIDocument uidoc)
      {
         UiDoc = uidoc;
         Document = uidoc.Document;
         Application = uidoc.Application.Application;
         UiApplication = uidoc.Application;
         Selection = uidoc.Selection;
         Username = Application.Username;
         ActiveView = Document.ActiveView;
         try
         {
            ViewPlane = BPlane.CreateByNormalAndOrigin(ActiveView.ViewDirection, ActiveView.Origin);
         }
         catch (Exception e)
         {
            Log(e.Message);
         }
         ErrorLog = String.Empty;
         SetPath();
      }

      public static void GetInformation(ExternalCommandData data, string currentCommand)
      {
         CurrentCommand = currentCommand;
         var uidoc = data.Application.ActiveUIDocument;
         UiDoc = uidoc;
         Document = uidoc.Document;
         Application = uidoc.Application.Application;
         UiApplication = uidoc.Application;
         Selection = uidoc.Selection;
         Username = Application.Username;
         ActiveView = Document.ActiveView;
         ViewPlane = BPlane.CreateByNormalAndOrigin(ActiveView.ViewDirection, ActiveView.Origin);
         ErrorLog = String.Empty;
         SetPath();
      }

      private static void SetPath()
      {
         //Setting Path
         Version = UiApplication.Application.VersionNumber;
         BimSpeedSettingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BimSpeedSetting";
         BimSpeedInstallPath = @"C:\ProgramData\Autodesk\ApplicationPlugins\BIMSpeed.bundle\Contents";
         BimSpeedInstallPath = @"C:\ProgramData\Autodesk\ApplicationPlugins\BIMSpeed.bundle\Contents";
      }

      public static string Log(string log, object obj = null)
      {
         ErrorLog += Environment.NewLine + "- " + log;
         if (obj != null)
         {
            ErrorLog += Environment.NewLine + obj.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
         }
         return ErrorLog;
      }

      public static string Log(string log, Exception e, object obj = null)
      {
         ErrorLog += Environment.NewLine + "- " + log + Environment.NewLine + e.Message;
         if (obj != null)
         {
            ErrorLog += Environment.NewLine + obj.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
         }
         return ErrorLog;
      }
   }

   public class ActiveData : AC
   {
   }
}