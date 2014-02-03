using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tripod.Web.Models;

namespace Tripod.Web
{
    public static class TempDataExtensions
    {
        private const string AlertKey = "Alerts";

        public static void Alerts(this TempDataDictionary tempData, string message, AlertFlavor flavor = AlertFlavor.Info,
            bool isDismissable = false)
        {
            if (tempData == null) throw new ArgumentNullException("tempData");
            var alerts = tempData.Alerts(true) ?? new List<AlertModel>();
            alerts.Add(new AlertModel
            {
                Message = message,
                Flavor = flavor,
                IsDismissable = isDismissable,
            });
            tempData[AlertKey] = alerts;
        }

        public static IList<AlertModel> Alerts(this TempDataDictionary tempData, bool keep = false)
        {
            if (tempData == null) throw new ArgumentNullException("tempData");
            var alerts = keep ? tempData.Peek(AlertKey) as IList<AlertModel> : tempData[AlertKey] as IList<AlertModel>;
            return alerts;
        }
    }
}
