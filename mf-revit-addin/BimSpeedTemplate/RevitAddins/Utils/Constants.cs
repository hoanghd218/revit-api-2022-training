using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace RevitApiUtils
{
   public class Constants
   {
      public static string SettingFolder
      {
         get
         {
            string settingFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Constants.AppName + "Setting";
            if (Directory.Exists(settingFolder) == false)
            {
               Directory.CreateDirectory(settingFolder);
            }
            return settingFolder;
         }
      }

      public static string VersionFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      public static string ContentsFolder => Directory.GetParent(Constants.VersionFolder).FullName;
      public static string ResourcesFolder => Path.Combine(Constants.ContentsFolder, "Resources");
      public static string AppName = "BimSpeed";

      private static string LocalAppdataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      public const double EPS = 1.0e-9;
      public static double MINIMUMVOLUME => UnitConverter.CubicMeterToCubicFoot(1e-6);
      public static double MINIMUMAREA => UnitConverter.SquareMeterToSquareFoot(1e-6);
      public const double MINIMUMANGLE = 0.3;
      public const double MINIMUMDISTANCE = 0.01;
      public const string FAMILYZFITTINGNAME = "00.REC_Offset";
      public const string APPNAME = "BIMSpeedMEP";
#if RELEASE
      public static int CURRENT_VERSION = Convert.ToInt32(DateTime.Today.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
#else
      public const int CURRENT_VERSION = 20210808;
#endif
      public static bool IS_ENGLISH = false;
      public static bool IS_STUDENT = false;

      public static string REVITVERSION
      {
         get
         {
#if Version2018
            return "2018";
#elif Version2019
            return "2019";
#elif Version2020
            return "2020";
#elif Version2021
            return "2021";
#elif Version2022
            return "2022";
#else
            return "";
#endif
         }
      }

      //public static SelectionChangedWatcher SELECTION_CHANGED { get; set; }



      #region Message

      public const string NO_DUCT_SELECTION = "There is no Duct in current selections.";
      public const string PICK_DUCT_SPLIT = "Pick a Duct to split";
      public const string PICK_FIRST_ELEMENT = "Pick first Element";
      public const string PICK_FIRST_ELEMENT_ERROR = "User has to pick first Element.";
      public const string PICK_START_POINT = "Pick start point";
      public const string PICK_FAMILYINSTANCE_COPY = "Pick FamilyInstance to copy";
      public const string PICK_DESTINATION_OBJECT = "Pick destination Object";
      public const string PICK_MEPCURVE_MOVE = "Pick MEPCurve will be moved";

      #endregion Message
   }
}