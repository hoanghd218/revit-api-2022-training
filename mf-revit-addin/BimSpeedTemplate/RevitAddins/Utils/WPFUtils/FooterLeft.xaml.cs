using System.Windows;
using System.Windows.Controls;

namespace RevitApiUtils.WPFUtils
{
   /// <summary>
   /// Interaction logic for FooterLeft.xaml
   /// </summary>
   public partial class FooterLeft : UserControl
   {
      public FooterLeft()
      {
         InitializeComponent();
      }

      private void BtnFeedBack_OnClick(object sender, RoutedEventArgs e)
      {

         System.Diagnostics.Process.Start("https://www.mf-tools.info/question-answer");
      }


      private void BtnHomePage_OnClick(object sender, RoutedEventArgs e)
      {
         System.Diagnostics.Process.Start("https://www.mf-tools.info/");
      }
   }
}
