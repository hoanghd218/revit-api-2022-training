using Autodesk.Revit.UI;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace RevitAddins
{
   public class AppTest : IExternalApplication
   {
      private Stopwatch stopwatch = null;
      public Result OnStartup(UIControlledApplication application)
      {
         stopwatch = new Stopwatch();
         stopwatch.Start();
         CreateRibbons(application);
         return Result.Succeeded;
      }

      public Result OnShutdown(UIControlledApplication application)
      {
         stopwatch.Stop();
         return Result.Succeeded;
      }


      private void CreateRibbons(UIControlledApplication application)
      {
         var path = Assembly.GetExecutingAssembly().Location;
         //var imageLogin = ToBitmapImage(Properties.Resources.user);
         application.CreateRibbonTab("MF BIM");
         var panel = application.CreateRibbonPanel("MF BIM", "MF");
         var pushButton1 =
             (PushButton)panel.AddItem(new PushButtonData("Login", "Login", path,
                 "RevitAddins.Login.LoginCmd"));
         //pushButton1.LargeImage = imageLogin;

         var pushButton2 =
             (PushButton)panel.AddItem(new PushButtonData("Add", "Parameters", path,
                 "RevitAddins.__0TestCmd"));
         //pushButton2.LargeImage = imagePara;

      }

      public static void CreateRibbonLicense(UIControlledApplication application, RibbonPanel panel)
      {
         var path = Assembly.GetExecutingAssembly().Location;
         var imageLogin = ToBitmapImage(Properties.Resources.login);
         var pushButton1 =
            (PushButton)panel.AddItem(new PushButtonData("Login", "Login", path,
               "RevitAddins.Login.LoginCmd"));
         pushButton1.LargeImage = imageLogin;
      }

      public static void CreateRibbonRebar(UIControlledApplication application)
      {
         RibbonPanel panel = application.CreateRibbonPanel("MF Tools", "Rebar");

         var path = Assembly.GetExecutingAssembly().Location;
         var imageRebarOpening = ToBitmapImage(Properties.Resources.rebaropening);
         var pushButton1 = (PushButton)panel.AddItem(new PushButtonData("Rebar Opening", "Rebar Opening", path,
               "RevitAddins.RebarOpeningForSlab.RebarOpeningForSlabCmd"));
         pushButton1.LargeImage = imageRebarOpening;
      }

      public static BitmapImage ToBitmapImage(Bitmap bitmap)
      {
         using var memory = new MemoryStream();
         bitmap.Save(memory, ImageFormat.Png);
         memory.Position = 0;

         var bitmapImage = new BitmapImage();
         bitmapImage.BeginInit();
         bitmapImage.StreamSource = memory;
         bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
         bitmapImage.EndInit();
         bitmapImage.Freeze();

         return bitmapImage;
      }
   }

}
