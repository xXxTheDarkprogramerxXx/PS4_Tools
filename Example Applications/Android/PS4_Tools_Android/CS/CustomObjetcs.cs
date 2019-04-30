using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DesignLibrary_Tutorial
{
    public class TripMasterObjects
    {
        #region Attributes

        public String Status { get; set; }

        public DateTime ReqDate { get; set; }

        public DateTime LastPrintDate { get; set; }

        public DateTime DeLastPrint { get; set; }

        public DateTime DateTimeTruckLeft { get; set; }

        public DateTime HandedOutTime { get; set; }

        public DateTime StartLoadTime { get; set; }

        public DateTime EndLoadTime { get; set; }

        public DateTime TripCompletedTime { get; set; }

        public DateTime TripCreateTime { get; set; }

        public Decimal TripMass { get; set; }

        public Decimal TripVolume { get; set; }

        public Decimal CostPerTrip { get; set; }

        public Int32 PrintCount { get; set; }

        public Int32 DePrintCount { get; set; }

        public Int32 Sequence { get; set; }

        public String TripSupervisor { get; set; }

        public String TripNo { get; set; }

        public String Company { get; set; }

        public String WMSWarehouse { get; set; }

        public String Vehicle { get; set; }

        public String Area { get; set; }

        public String Cage { get; set; }

        public String Driver { get; set; }

        public String State { get; set; }

        public String Comment { get; set; }

        public String CoDriver { get; set; }
        #endregion Attributes
    }
}