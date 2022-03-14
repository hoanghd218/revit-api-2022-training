using Autodesk.Revit.DB;
using System.IO;

namespace RevitApiUtils
{
   public class DocUtils
   {
      public static string GetProjectName(Document doc)
      {
         string valueString = "";
         if (doc.IsWorkshared)
         {
            ModelPath centralpath = doc.GetWorksharingCentralModelPath();
            if (centralpath != null)
            {
               valueString = ModelPathUtils.ConvertModelPathToUserVisiblePath(centralpath);
               return Path.GetFileNameWithoutExtension(valueString);
            }
         }
         else
         {
            valueString = doc.PathName;
            return Path.GetFileNameWithoutExtension(valueString);
         }
         return "";
      }

      public static string GetModelGUID(Document doc)
      {
         string valueString = "";
#if Version2017 || Version2018 || Version2019
         return valueString;
#else
         if (doc.IsWorkshared)
         {
            valueString = doc.GetCloudModelPath().GetModelGUID().ToString();
         }
         return valueString;
#endif
      }
   }
}