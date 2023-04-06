
using System;
using System.Globalization;

namespace KM.GD.PrintInvoices.Helpers
{
   [Serializable]
   public class AppException : Exception
   {
      public AppException() : base() { }

      public AppException(string message) : base(message) { }

      public AppException(string message, params object[] args)
          : base(String.Format(CultureInfo.CurrentCulture, message, args))
      {
      }
   }
}