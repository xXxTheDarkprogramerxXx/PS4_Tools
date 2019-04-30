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
using System.Net;

namespace DesignLibrary_Tutorial
{
    public class ServiceWrapper : WMSScanner.WMSService
    {
        string NetworkError = "Network: Can't connect to Service";

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response;
            try
            {
                response = base.GetWebResponse(request);
            }
            catch (Exception)
            {
                bool result = CheckServiceAvailable.CheckServiceAvailble();

                if (result == false)
                {
                    throw new Exception(NetworkError);
                }
                throw;

            }

            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response;
            try
            {
                response = base.GetWebResponse(request, result);

            }
            catch (Exception)
            {

                bool resultg = CheckServiceAvailable.CheckServiceAvailble();

                if (resultg == false)
                {
                    throw new Exception(NetworkError);
                }

                throw;
            }

            return response;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            return base.GetWebRequest(uri);
        }
    }

    public static class CheckServiceAvailable
    {


        private static int timeout = 2000;
        public static bool CheckServiceAvailbleNew()
        {
            var url = Update_Variables.WebServiceLocation;

            try
            {
                var myRequest = (HttpWebRequest)WebRequest.Create(url);

                myRequest.Timeout = timeout;
                var response = (HttpWebResponse)myRequest.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {

                    // return true ;
                    return false;
                }
                else
                {

                    return false;
                }
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static bool CheckServiceAvailble()
        {


            try
            {

                System.Net.WebRequest myRequest = System.Net.WebRequest.Create(Update_Variables.WebServiceLocation);
                myRequest.Timeout = timeout;
                System.Net.WebResponse myResponse = myRequest.GetResponse();
                return true;
            }
            catch (Exception)
            {




                return false;

            }
        }


    }
}