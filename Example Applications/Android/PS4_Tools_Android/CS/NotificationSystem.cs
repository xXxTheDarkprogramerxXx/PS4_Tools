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
using SQLite;
using System.Data.SqlClient;
using System.Data;
using Android.Media;
using Android.Graphics;

namespace DesignLibrary_Tutorial
{
    class NotificationSystem
    {
        public static void CreateDatabase(string path)
        {
            try
            {
                var connection = new SQLiteConnection(path);
                //now we create the Notification Table
                connection.CreateTable<Notifications>();
                
            }
            catch (SQLiteException ex)
            {
                string error = ex.Message;
            }
        }

        public static void Test_Notification(string path)
        {

            string pathToDatabase = path;
            //test the create method
            CreateDatabase(path);
            //Add the new data
            insert_Data(new Notifications { Title = "ToysRus", Description  = "Your product is on special at ToysRus ",Date = DateTime.Now ,IsSeen = false,ProductRef = 1}, pathToDatabase);
            //we will need to find the data some how

           // Constants.Notification_Count = findNumberRecords(path);
           // List<Notification> DataList = new List<Notification>();
           // Notification test = GetData(path);


        }

        public static void ShowNotification(Context context)
        {
            // Instantiate the builder and set notification elements:
            Notification.Builder builder = new Notification.Builder(context)
                .SetAutoCancel(true)
                .SetContentTitle("Sample Notification")
                .SetContentText("Hello World! This is my first notification!")
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                .SetSmallIcon(Resource.Drawable.ps4_tools)
                .SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.ps4_tools));

            builder.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Ringtone));

            // Setup an intent for SecondActivity:
            Intent secondIntent = new Intent(context, typeof(UserNotification));

            // Pass some information to SecondActivity:
            secondIntent.PutExtra("message", "Greetings from MainActivity!");

            // Create a task stack builder to manage the back stack:
            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(context);

            // Add all parents of SecondActivity to the stack: 
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(UserNotification)));

            // Push the intent that starts SecondActivity onto the stack:
            stackBuilder.AddNextIntent(secondIntent);

            // Obtain the PendingIntent for launching the task constructed by
            // stackbuilder. The pending intent can be used only once (one shot):
            const int pendingIntentId = 0;
            PendingIntent pendingIntent =
                stackBuilder.GetPendingIntent(pendingIntentId, PendingIntentFlags.OneShot);


            int totalunread = 10;
            if (totalunread > 0)
            {
                // Instantiate the Inbox style:
                Notification.InboxStyle inboxStyle = new Notification.InboxStyle();

                // Set the title and text of the notification:
                builder.SetContentIntent(pendingIntent);
                builder.SetContentTitle(totalunread + " new messages");
                builder.SetContentText("WMS Trips");
                //if (dtNotify.Rows.Count > 3)
                //{
                //    for (int i = 0; i < dtNotify.Rows.Count; i++)
                //    {
                //        if (i == 3)
                //        {
                //            i = int.MaxValue;
                //            break;
                //        }
                //        else
                //        {
                //            inboxStyle.AddLine(string.Format("{0}: {1}", dtNotify.Rows[i]["Title"].ToString(), dtNotify.Rows[i]["Description"].ToString()));
                //        }

                //    }

                //    inboxStyle.SetSummaryText("+"+(dtNotify.Rows.Count - 3) + " more");
                //}
                //else
                //{
                //    for (int i = 0; i < dtNotify.Rows.Count; i++)
                //    {
                //        inboxStyle.AddLine(string.Format("{0}: {1}", dtNotify.Rows[i]["Title"].ToString(), dtNotify.Rows[0]["Description"].ToString()));
                //    }
                //}

                // Plug this style into the builder:
                builder.SetStyle(inboxStyle);

                

                // Build the notification:
                Notification notification = builder.Build();

                // Get the notification manager:
                NotificationManager notificationManager =
                    context.GetSystemService(Context.NotificationService) as NotificationManager;

                // Publish the notification:
                const int notificationId = 0;
                notificationManager.Notify(notificationId, notification);

                
            }
        }


        private static Notification GetData(string path)
        {
            try
            {
                var db = new SQLiteConnection(path);
                // this counts all records in the database, it can be slow depending on the size of the database
                //var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Notifications");

                // for a non-parameterless query
                var count = db.Get<Notification>(1);

                return count;
            }
            catch (SQLiteException ex)
            {
                return null;
                //return -1;
            }
        }


        private static int findNumberRecords(string path)
        {
            try
            {
                var db = new SQLiteConnection(path);
                // this counts all records in the database, it can be slow depending on the size of the database
                //var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Notifications");

                // for a non-parameterless query
                 var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Notifications WHERE IsSeen='1'");

                return count;
            }
            catch (SQLiteException)
            {
                return -1;
            }
        }

        private static string insert_Data(Notifications data, string path)
        {
            try
            {
                var db = new SQLiteConnection(path);
                if (db.Insert(data) != 0)
                    db.Update(data);
                return "Single data file inserted or updated";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }

    }


    public class Notifications
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public bool IsSeen { get; set; }

        public int ProductRef {get;set;}

        public override string ToString()
        {
            return string.Format("[Notification: ID={0}, Title={1}, Description={2}, Date={3},IsSeen={4},ProductRef={5}]", ID, Title, Description, Date, IsSeen,ProductRef);
        }
    }
}