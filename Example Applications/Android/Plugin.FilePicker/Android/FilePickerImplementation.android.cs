using Android.App;
using Android.Content;
using Android.Runtime;
using Java.IO;
using Plugin.FilePicker.Abstractions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

// Adds permission for READ_EXTERNAL_STORAGE to the AndroidManifest.xml of the app project without
// the user of the plugin having to add it by himself/herself.
[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]

namespace Plugin.FilePicker
{
    /// <summary>
    /// Implementation for file picking on Android
    /// </summary>
    [Preserve(AllMembers = true)]
    public class FilePickerImplementation : IFilePicker
    {
        /// <summary>
        /// Android context to use for picking
        /// </summary>
        private readonly Context context;

        /// <summary>
        /// Request ID for current picking call
        /// </summary>
        private int requestId;

        /// <summary>
        /// Task completion source for task when finished picking
        /// </summary>
        private TaskCompletionSource<FileData> completionSource;

        /// <summary>
        /// Creates a new file picker implementation
        /// </summary>
        public FilePickerImplementation()
        {
            this.context = Application.Context;
        }

        /// <summary>
        /// Implementation for picking a file on Android.
        /// </summary>
        /// <param name="allowedTypes">
        /// Specifies one or multiple allowed types. When null, all file types
        /// can be selected while picking.
        /// On Android you can specify one or more MIME types, e.g.
        /// "image/png"; also wild card characters can be used, e.g. "image/*".
        /// </param>
        /// <returns>
        /// File data object, or null when user cancelled picking file
        /// </returns>
        public async Task<FileData> PickFile(string[] allowedTypes)
        {
            var fileData = await this.PickFileAsync(allowedTypes, Intent.ActionGetContent);

            return fileData;
        }

        /// <summary>
        /// File picking implementation
        /// </summary>
        /// <param name="allowedTypes">list of allowed types; may be null</param>
        /// <param name="action">Android intent action to use; unused</param>
        /// <returns>picked file data, or null when picking was cancelled</returns>
        private Task<FileData> PickFileAsync(string[] allowedTypes, string action)
        {
            var id = this.GetRequestId();

            var ntcs = new TaskCompletionSource<FileData>(id);

            var previousTcs = Interlocked.Exchange(ref this.completionSource, ntcs);
            if (previousTcs != null)
            {
                previousTcs.TrySetResult(null);
            }

            try
            {
                var pickerIntent = new Intent(this.context, typeof(FilePickerActivity));
                pickerIntent.SetFlags(ActivityFlags.NewTask);

                pickerIntent.PutExtra(FilePickerActivity.ExtraAllowedTypes, allowedTypes);

                this.context.StartActivity(pickerIntent);

                EventHandler<FilePickerEventArgs> handler = null;
                EventHandler<FilePickerCancelledEventArgs> cancelledHandler = null;

                handler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref this.completionSource, null);

                    FilePickerActivity.FilePickCancelled -= cancelledHandler;
                    FilePickerActivity.FilePicked -= handler;

                    tcs?.SetResult(new FileData(
                        e.FilePath,
                        e.FileName,
                        () =>
                        {
                            if (IOUtil.IsMediaStore(e.FilePath))
                            {
                                var contentUri = Android.Net.Uri.Parse(e.FilePath);
                                return Application.Context.ContentResolver.OpenInputStream(contentUri);
                            }
                            else
                            {
                                return System.IO.File.OpenRead(e.FilePath);
                            }
                        }));
                };

                cancelledHandler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref this.completionSource, null);

                    FilePickerActivity.FilePickCancelled -= cancelledHandler;
                    FilePickerActivity.FilePicked -= handler;

                    if (e?.Exception != null)
                    {
                        tcs?.SetException(e.Exception);
                    }
                    else
                    {
                        tcs?.SetResult(null);
                    }
                };

                FilePickerActivity.FilePickCancelled += cancelledHandler;
                FilePickerActivity.FilePicked += handler;
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                this.completionSource.SetException(ex);
            }

            return this.completionSource.Task;
        }

        /// <summary>
        /// Returns a new request ID for a new call to PickFile()
        /// </summary>
        /// <returns>new request ID</returns>
        private int GetRequestId()
        {
            int id = this.requestId;

            if (this.requestId == int.MaxValue)
            {
                this.requestId = 0;
            }
            else
            {
                this.requestId++;
            }

            return id;
        }

        /// <summary>
        /// Android implementation of saving a picked file to the external storage directory.
        /// </summary>
        /// <param name="fileToSave">picked file data for file to save</param>
        /// <returns>true when file was saved successfully, false when not</returns>
        public Task<bool> SaveFile(FileData fileToSave)
        {
            try
            {
                var myFile = new File(Android.OS.Environment.ExternalStorageDirectory, fileToSave.FileName);

                if (myFile.Exists())
                {
                    return Task.FromResult(true);
                }

                var fos = new FileOutputStream(myFile.Path);

                fos.Write(fileToSave.DataArray);
                fos.Close();

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Android implementation of opening a file by using ActionView intent.
        /// </summary>
        /// <param name="fileToOpen">file to open in viewer</param>
        public void OpenFile(File fileToOpen)
        {
            var uri = Android.Net.Uri.FromFile(fileToOpen);
            var intent = new Intent();
            var mime = IOUtil.GetMimeType(uri.ToString());

            intent.SetAction(Intent.ActionView);
            intent.SetDataAndType(uri, mime);
            intent.SetFlags(ActivityFlags.NewTask);

            this.context.StartActivity(intent);
        }

        /// <summary>
        /// Android implementation of OpenFile(), opening a file already stored on external
        /// storage.
        /// </summary>
        /// <param name="fileToOpen">relative filename of file to open</param>
        public void OpenFile(string fileToOpen)
        {
            var myFile = new File(Android.OS.Environment.ExternalStorageDirectory, fileToOpen);

            this.OpenFile(myFile);
        }

        /// <summary>
        /// Android implementation of OpenFile(), opening a picked file in an external viewer. The
        /// picked file is saved to external storage before opening.
        /// </summary>
        /// <param name="fileToOpen">picked file data</param>
        public async void OpenFile(FileData fileToOpen)
        {
            var myFile = new File(Android.OS.Environment.ExternalStorageDirectory, fileToOpen.FileName);

            if (!myFile.Exists())
            {
                await this.SaveFile(fileToOpen);
            }

            this.OpenFile(myFile);
        }
    }
}
