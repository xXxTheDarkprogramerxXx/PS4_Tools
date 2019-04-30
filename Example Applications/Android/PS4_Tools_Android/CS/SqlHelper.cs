using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Data.SqlClient;
using System.Data;
using Mono.Data.Sqlite;

namespace DesignLibrary_Tutorial
{
    class SqlHelper
    {
        private static object lockThis = new object();


        private static SqlConnection con = null;

        static SqlDataAdapter DA;
        static DataSet DS;
        static DataTable DT;

        private static void SetupConnection()
        {
            lock (lockThis)
            {
                if (con == null)
                {
                    con = new SqlConnection("Data Source=jemtech.dedicated.co.za\\JEMSQL2014;Initial Catalog=Spescial_Mandie;User ID=sa;Password=J3m@T3ch");
                }

                try
                {
                    con.Open();
                }
                catch (InvalidOperationException) // The connection was not closed.
                { }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                System.Threading.Thread.Sleep(50); //Wait for status to change to Connecting
                while (con.State == ConnectionState.Connecting)
                {
                    //Wait for Connection to Connect:
                    //(Error is thrown when using a connection which is still busy connecting)
                }
            }
        }

        /// <summary>
        /// we use this just to get current device information this will be usefull later
        /// </summary>
        /// <returns></returns>
        public static string getDeviceName()
        {
            string manufacturer = Build.Manufacturer;
            string model = Build.Model;
            if (model.StartsWith(manufacturer))
            {
                return model;
            }
            else
            {
                return manufacturer + " " + model; //"Samsung GT-N8010"
            }
        }

        public static SqlDataReader getDataReader(string qry)
        {



            lock (lockThis)
            {
                try
                {
                    SetupConnection();

                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = qry;

                    SqlDataReader dr = cmd.ExecuteReader();
                    return dr;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                    return null;
                }
            }
        }

        public static DataSet getDataSet(String qry)
        {



            lock (lockThis)
            {
                try
                {
                    SetupConnection();

                    DataSet ds = new DataSet();
                    using (SqlDataAdapter da = new SqlDataAdapter(qry, con))
                    {
                        da.Fill(ds);
                    }
                    return ds;
                }
                catch (Exception ex)
                {
                    //Errors.Add(new ErrorMessage("SQL", "DS", ex.Message));
                    return new DataSet();
                }
            }
        }

        public static DataTable getDataTable(String qry)
        {



            lock (lockThis)
            {
                try
                {
                    SetupConnection();

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(qry, con))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    object ob = con.State;
                    //Errors.Add(new ErrorMessage("SQL", "DT", ex.Message));
                    return new DataTable();
                }
            }
        }

        public static DataTable GetDataTable_SQLITE(string sqlqry, string Path)
        {
            SqliteConnection Conn = new SqliteConnection("Data Source=" + Path);
            Conn.Open();
            string stringQuery = sqlqry;
            var SqliteCmd = new SqliteCommand(stringQuery, Conn);
            SqliteCmd.CommandType = CommandType.Text;
            SqliteDataAdapter da = new SqliteDataAdapter(SqliteCmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null)
                return dt;
            else
                return null;
        }

        public static object GetSingleValue_SQLLITE(string sqlqry, string path)
        {
            try
            {
                SqliteConnection Conn = new SqliteConnection("Data Source=" + path);
                // this counts all records in the database, it can be slow depending on the size of the database
                //var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Notifications");
                Conn.Open();
                string stringQuery = sqlqry;
                var SqliteCmd = new SqliteCommand(stringQuery, Conn);
                SqliteCmd.CommandType = CommandType.Text;
                //SqliteDataAdapter da = new SqliteDataAdapter(SqliteCmd);
                // for a non-parameterless query

                object value = SqliteCmd.ExecuteScalar();

                return value;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataTable getDataTable(String qry, System.Data.SqlClient.SqlParameter[] parms)
        {



            lock (lockThis)
            {
                try
                {
                    SetupConnection();

                    SqlCommand cmd = new SqlCommand(qry, con);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(parms);

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    object ob = con.State;
                    //Errors.Add(new ErrorMessage("SQL", "DT", ex.Message));
                    return new DataTable();
                }
            }
        }

        public static object getValue(String qry)
        {



            lock (lockThis)
            {
                try
                {
                    SetupConnection();

                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = qry;

                    object value = cmd.ExecuteScalar();
                    return value;
                }
                catch (Exception ex)
                {
                    //Errors.Add(new ErrorMessage("SQL", "Value", ex.Message));
                    return null;
                }
            }
        }

        public static int executeNonQuery(String qry)
        {


            lock (lockThis)
            {
                try
                {
                    SetupConnection();

                    SqlCommand cmd = con.CreateCommand();

                    cmd.CommandText = qry;
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //Errors.Add(new ErrorMessage("SQL", "NQ", ex.Message));
                    return -1;
                }
            }
        }

        public static int ExecuteNonQuery(string Qry, SqlParameter[] parms)
        {

            lock (lockThis)
            {
                try
                {
                    SetupConnection();

                    SqlCommand cmd = con.CreateCommand();

                    cmd.CommandText = Qry;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(parms);
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                    return -1;
                }
            }
        }

        /// <summary>
        /// Execute a query returning true if successfull, rollback transaction if not
        /// </summary>
        /// <param name="Connstring">Connection string</param>
        /// <param name="Qry">Query to execute</param>
        public static bool ExecuteNonQueryBL1(string Qry, SqlParameter[] parms)
        {
            bool success = false;
            try
            {
                SetupConnection();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = Qry;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parms);
                cmd.ExecuteNonQuery();
                success = true;
            }
            catch (Exception EX)
            {
                success = false;
                throw new Exception(EX.Message);
            }
            finally
            {

            }

            return success;
        }

        /// <summary>
        /// Get a single value returned as an object
        /// </summary>
        /// <param name="ConnectionString">Connection string</param>
        /// <param name="Qry">Query to execute</param>
        /// <returns></returns>
        public static object GetSingleValue(string Qry, SqlParameter[] parms)
        {

            object objValue = null;

            SetupConnection();

            SqlCommand cmd = con.CreateCommand();

            cmd.CommandText = Qry;
            cmd.Parameters.Clear();
            cmd.Parameters.AddRange(parms);

            try
            {

                objValue = (object)cmd.ExecuteScalar();

                string isnull = Convert.ToString(objValue).Trim();

                //if (isnull == "")
                //{
                //    objValue = null;
                //}
            }
            catch (Exception EX)
            {
                throw new Exception(EX.Message);
            }
            finally
            {

                if (cmd != null) cmd.Dispose();
                cmd = null;
            }

            return objValue;
        }

        /// <summary>
        /// Get Data in form of Datatable
        /// </summary>
        /// <param name="sqlqry"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sqlqry, SqlParameter[] _params)
        {
            SetupConnection();

            DA = new SqlDataAdapter(sqlqry, con);
            DA.SelectCommand.CommandTimeout = 800;//800;
            DA.SelectCommand.Parameters.Clear();
            DA.SelectCommand.Parameters.AddRange(_params);
            DT = new DataTable();

            try
            {
                DA.Fill(DT);
            }
            catch (Exception EX)
            {
                throw new Exception(EX.Message);
            }

            return DT;
        }

        /// <summary>
        /// Get a single value returned as an object
        /// </summary>
        /// <param name="ConnectionString">Connection string</param>
        /// <param name="Qry">Query to execute</param>
        /// <returns></returns>
        public static object GetSingleValue(string Qry)
        {

            object objValue = null;

            SetupConnection();

            SqlCommand cmd = con.CreateCommand();

            cmd.CommandText = Qry;

            try
            {
                objValue = (object)cmd.ExecuteScalar();

                string isnull = Convert.ToString(objValue).Trim();

                //if (isnull == "")
                //{
                //    objValue = null;
                //}
            }
            catch (Exception EX)
            {
                throw new Exception(EX.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();

                if (con != null) con.Dispose();
                con = null;

                if (cmd != null) cmd.Dispose();
                cmd = null;
            }

            return objValue;
        }

        //private static void updateTRansactionTime()
        //{
        //    lock (lockThis)
        //    {
        //        if (AppD.AppD1.LastUpdatedTime.AddMinutes(10) < DateTime.Now)
        //        {
        //            try
        //            {
        //                string updateTransTime = "update  [" + AppD.AppD1.WmsDBName + "].dbo.Operator_Login  " +
        //              " set LastTransactionTime =GETDATE(),[LastOperationPerformed]='Dashboard'  " +
        //              " where  WMSGuid ='" + AppD.AppD1.UserWmsGuid + "'";



        //                SetupConnection();

        //                SqlCommand cmd = con.CreateCommand();
        //                cmd.CommandText = updateTransTime;

        //                object value = cmd.ExecuteNonQuery();

        //                AppD.AppD1.LastUpdatedTime = DateTime.Now;


        //            }
        //            catch (Exception ex)
        //            {
        //                // WriteTransLog("Error. Dead Log On WMS Operator within WMS-DB Update Qry " + ex.Message + "Comp : " + comp.Company + " \r\n");
        //                // WriteLog("Error. Dead Log On WMS Operator within WMS-DB Update Qry " + ex.Message + "Comp : " + comp.Company + " \r\n");

        //            }
        //        }
        //    }
        //}
    }
}