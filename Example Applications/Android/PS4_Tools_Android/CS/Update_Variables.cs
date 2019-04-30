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
using System.Data;

namespace DesignLibrary_Tutorial
{
    public static class Update_Variables
    {
        public static string Bin = string.Empty;
        public static string WMSWarehouse = string.Empty;
        public static string Warehouse = string.Empty;//CVR
        public static string SysproGuid = string.Empty;
        public static Guid WMSGuid = Guid.Empty;
        public static string WebServiceLocation = "";
        public static string CurrentGridItem = "";
        public static string NotFoundCode = "";
        public static string Trolly = "";
        public static DataTable MustScanItems = new DataTable();
        public static string MustScanPassword = "";
        public static string AutoReplace = "";
        public static string Company = string.Empty;
        public static string MustCheckoutInsertPalletNumbering = string.Empty;
        //this is for the must scan barcodes
        public static TimeSpan TimeTakenToScan = new TimeSpan();
        //cant rem why i have this awell it need for some bizzzare reason
        public static string StringPlaceHolder = string.Empty;

        //action screen not the best place but it will do
        public static bool ActionScreenFoundAction = false;
        public static bool ActionScreenUserClosed = false;

        public static bool MustFillLeadingZerosSalesOrder = false;
        public static bool MustFillLeadingZerosInvoice = false;
        public static bool MustFillLeadingZerosPO = false;

        public static int MustFillLeadingZerosSalesOrderAmount = 0;
        public static int MustFillLeadingZerosInvoiceAmount = 0;
        public static int MustFillLeadingZerosPOAmount = 0;

        //moved this to contants
        //public static Crn.WMSService _service = new CransWMSService.WMSService();
    }
}