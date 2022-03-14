using System;
using System.Security.Cryptography;
using System.Text;

namespace RevitApiUtils
{
   public static class EncryptUtils
   {
      private static string key = "bimspeed";

      /// <summary>
      /// Giản mã
      /// </summary>
      /// <param name="toDecrypt">Chuỗi đã mã hóa</param>
      /// <returns>Chuỗi giản mã</returns>
      public static string Decrypt(string toDecrypt)
      {
         byte[] keyArray;
         byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

         {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
         }

         TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
         {
            Key = keyArray,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
         };

         ICryptoTransform cTransform = tdes.CreateDecryptor();
         byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

         return Encoding.UTF8.GetString(resultArray);
      }

      /// <summary>
      /// Mã hóa chuỗi có mật khẩu
      /// </summary>
      /// <param name="toEncrypt">Chuỗi cần mã hóa</param>
      /// <returns>Chuỗi đã mã hóa</returns>
      public static string Encrypt(string toEncrypt)
      {
         byte[] keyArray;
         byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

         {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
         }

         TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
         {
            Key = keyArray,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
         };

         ICryptoTransform cTransform = tdes.CreateEncryptor();
         byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

         return Convert.ToBase64String(resultArray, 0, resultArray.Length);
      }
   }
}