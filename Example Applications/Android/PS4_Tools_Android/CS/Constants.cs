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
using Android.Locations;
using Android.Content.PM;

namespace DesignLibrary_Tutorial
{
    public class Constants
    {

        public static WMSScanner.WMSService _service = new WMSScanner.WMSService();


        public static bool BusyPicking = false;
        public static bool ScannerInUse = false;
        public static string Password = "Password";
        public static string Company = "Company";
        public static string CompanyPassword = "CompanyPassword";
        public static string Warehouse = "Warehouse";
        public static string Operator = "Operator";
        public static string MenuID = "MenuID";
        public static string GUI = "GUI";
        public static string StockCode = "StockCode";
        public static string FactorDescription = "FactorDescription";
        public static string Height = "Height";
        public static string Width = "Width";
        public static string Length = "Length";
        public static string Bin = "Bin";
        public static string FromBin = "FromBin";
        public static string ToBin = "ToBin";
        public static string ScanBin = "ScanBin";
        public static string Quantity = "Quantity";
        public static string QuantityAlt = "QuantityAlt";
        public static string EnterQuantity = "EnterQuantity";
        public static string Barcode = "Barcode";
        public static string ToWarehouse = "ToWarehouse";
        public static string FromWarehouse = "FromWarehouse";
        public static string SCBarcode = "SCBarcode";
        public static string Description = "Description";
        public static string ExpiryDate = "ExpiryDate";
        public static string Lot = "Lot";
        public static string ScanLot = "ScanLot";
        public static string Factor = "Factor";
        public static string Type = "Type";
        public static string Area = "Area";
        public static string Row = "Row";
        public static string BinNumber = "BinNumber";
        public static string Weight = "Weight";
        public static string HasStockBarcode = "HasStockBarcode";
        public static string HasNormalBarcode = "HasNormalBarcode";
        public static string MustScanFlag = "MustScanFlag";
        public static string NoBarCode = "NoBarCode";
        public static string Variance = "Variance";
        public static string Invoice = "Invoice";
        public static string ReasonCode = "ReasonCode";
        public static string Uom = "Uom";
        public static string AltUom = "AltUom";
        public static string NULL = "null";

        public static string Courier = "Courier";

        public static string NrOfLabels = "NrOfLabels";
        public static string NrOfPallets = "NrOfPallets";

        public static string PONumber = "PONumber";
        public static string Serial = "Serial";

        public static string SCTNumber = "SCTNumber";
        public static string Job = "Job";
        public static string TripNumber = "Sales Order Number";

        public static TimeSpan TimeAllowedForScanner = new TimeSpan(0, 0, 0, 1, 0);


        public static object NULLVal = new object();


        public static int DynamicItem = 19001;
        public static int DynamicLabelItem = 18001;
        public static int DynamicEditItem = 17001;
        public static int DynamciLabelInfoItem = 16001;
        public static int DynamicUpItem = 15001;
        public static int DynamicOKItem = 14001;
        public static int DynamicDownItem = 13001;
        public static int DynamicFixedLabelItem = 12001;
    }


    #region << Permisions >>

    /**
 	* Utility class that wraps access to the runtime permissions API in M and provides basic helper
 	* methods.
 	*/
    public abstract class PermissionUtil
    {
        /**
		* Check that all given permissions have been granted by verifying that each entry in the
		* given array is of the value Permission.Granted.
		*
		* See Activity#onRequestPermissionsResult (int, String[], int[])
		*/
        public static bool VerifyPermissions(Permission[] grantResults)
        {
            // At least one result must be checked.
            if (grantResults.Length < 1)
                return false;

            // Verify that each required permission has been granted, otherwise return false.
            foreach (Permission result in grantResults)
            {
                if (result != Permission.Granted)
                {
                    return false;
                }
            }
            return true;
        }
    }

    #endregion << Permisions >>
}