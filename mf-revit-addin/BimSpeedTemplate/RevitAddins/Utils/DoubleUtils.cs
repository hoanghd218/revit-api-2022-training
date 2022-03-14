using System;

namespace RevitApiUtils
{
   public static class DoubleUtils
   {
      public const double EPSILON = 1E-06;

      public const double PI = Math.PI;
      public static int RoundMultiple(this double d, int i)
      {

         return i * Convert.ToInt32(d / i);
      }
      public static int RoundMultipleUp(this double d, int i)
      {
         if (i == 0)
         {
            i = 1;
         }
         return i * Convert.ToInt32(Math.Ceiling(d / i));
      }
      public static int RoundMultipleDown(this double d, int i)
      {
         if (i == 0)
         {
            i = 1;
         }
         return i * Convert.ToInt32(Math.Floor(d / i));
      }
      public static double RoundMultipleUpFeet(this double d, int i)
      {
         if (i == 0)
         {
            i = 1;
         }
         double mm = d.FootToMm().RoundByDecimalPlace(3);
         double mmdRound = mm.RoundMultipleUp(i);

         return mmdRound.MmToFoot();
      }
      public static double RoundMultipleDownFeet(this double d, int i)
      {
         if (i == 0)
         {
            i = 1;
         }
         double mm = d.FootToMm().RoundByDecimalPlace(3);
         double mmdRound = mm.RoundMultipleDown(i);

         return mmdRound.MmToFoot();
      }
      public static int RoundDivide(this double number, double divideBy)
      {
         return (int)(number.RoundByDecimalPlace(6) / divideBy.RoundByDecimalPlace(6)) + 1;
      }

      public static bool IsEqual(this double A, double B, double tolerance = EPSILON)
      {
         return Math.Abs(B - A) < tolerance;
      }

      public static bool IsZero(this double A)
      {
         return IsEqual(0.0, A);
      }

      public static double NormalizeAngle(this double value)
      {
         double num = value;
         while (num.IsSmallerEqual(-2.0 * Math.PI) || num.IsGreaterEqual(2.0 * Math.PI))
            num = num.IsEqual(2.0 * Math.PI) || num.IsEqual(-2.0 * Math.PI) ? 0.0 : num - Math.Floor(num / (2.0 * Math.PI)) * Math.PI * 2.0;
         return num;
      }
      public static bool IsSmaller(this double A, double B, double tolerance = EPSILON)
      {
         return A + tolerance < B;
      }

      public static bool IsSmallerEqual(this double A, double B, double tolerance = EPSILON)
      {
         if (A + tolerance >= B)
         {
            return Math.Abs(B - A) < tolerance;
         }
         return true;
      }

      public static bool IsGreater(this double A, double B, double tolerance = EPSILON)
      {
         return A > B + tolerance;
      }

      public static bool IsGreaterEqual(this double A, double B, double tolerance = EPSILON)
      {
         if (Math.Abs(B - A) >= tolerance)
         {
            return A > B + tolerance;
         }
         return true;
      }

      public static bool IsBetweenEqual(this double a, double min, double max, double tol = EPSILON)
      {
         if (a.IsGreaterEqual(min, tol) && a.IsSmallerEqual(max, tol))
         {
            return true;
         }
         return false;
      }
      public static bool IsBetween(this double a, double min, double max, double tol = EPSILON)
      {
         if (a.IsGreater(min, tol) && a.IsSmaller(max, tol))
         {
            return true;
         }
         return false;
      }
      public static double Min(this double A, double B, double tolerance = EPSILON)
      {
         if (!IsSmaller(A, B, tolerance))
         {
            return B;
         }
         return A;
      }

      public static double Max(double A, double B, double tolerance = EPSILON)
      {
         if (!IsGreater(A, B, tolerance))
         {
            return B;
         }
         return A;
      }

      public static double MmToFoot(this double mm)
      {
         return mm / 304.79999999999995;
      }

      public static double MmToFoot(this int mm)
      {
         return mm / 304.79999999999995;
      }

      public static double MeterToFoot(this double metter)
      {
         return metter / 0.30479999999999996;
      }

      public static double MeterToFoot(this int metter)
      {
         return metter / 0.30479999999999996;
      }

      public static double FootToMm(this double feet)
      {
         return feet * 304.79999999999995;
      }

      public static double FootToMet(this double feet)
      {
         return feet * 304.79999999999995 / 1000;
      }

      public static double CubicFootToCubicMeter(this double cubicFoot)
      {
         return cubicFoot * 0.02831684659199999;
      }

      public static double SquareFootToSquareMeter(this double squareFoot)
      {
         return squareFoot * 0.092903039999999978;
      }

      public static double RadiansToDegrees(this double rads)
      {
         return rads * 57.295779513082323;
      }

      public static double DegreesToRadians(this double degrees)
      {
         return degrees * 0.017453292519943295;
      }

      public static double RoundByDecimalPlace(this double number, int decimalPlace)
      {
         return Math.Round(number, decimalPlace);
      }

      public static double Round2Number(this double number)
      {
         return Math.Round(number, 2);
      }

      public static double RoundMilimet(this double feet, double roundMm, bool isRoundUp = true)
      {
         if (roundMm.IsEqual(0))
         {
            return feet;
         }
         var mm = feet.FootToMm();
         if (isRoundUp)
         {
            mm = Math.Ceiling(mm / roundMm) * roundMm;
         }
         else
         {
            mm = Math.Floor(mm / roundMm) * roundMm;
         }
         return mm.MmToFoot();
      }

      public static double RoundDown(this double number, int step)
      {
         return Math.Floor(number / step) * step;
      }

      public static double RoundUp(this double number, int step)
      {
         return Math.Ceiling(number / step) * step;
      }
   }
}