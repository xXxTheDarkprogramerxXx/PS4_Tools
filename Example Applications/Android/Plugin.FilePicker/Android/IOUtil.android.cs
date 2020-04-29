using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Webkit;
using System;

namespace Plugin.FilePicker
{
    /// <summary>
    /// Android I/O utility functions
    /// </summary>
    public class IOUtil
    {
        /// <summary>
        /// Tries to find a file system path for given Uri. Note that this isn't always possible,
        /// since the content referenced by the Uri may not be stored on a file system, but is
        /// returned by the responsible app by using a ContentProvider. In this case, the method
        /// returns null, and access to the content is only possible by opening a stream.
        /// </summary>
        /// <param name="context">Android context to access content resolver</param>
        /// <param name="uri">Uri to use</param>
        /// <returns>full file system path, or null</returns>
        public static string GetPath(Context context, Android.Net.Uri uri)
        {
            bool isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

            // DocumentProvider
            if (isKitKat && DocumentsContract.IsDocumentUri(context, uri))
            {
                // ExternalStorageProvider
                if (IsExternalStorageDocument(uri))
                {
                    var docId = DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    var type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }

                    // TODO handle non-primary volumes
                }
                else if (IsDownloadsDocument(uri))
                {
                    // DownloadsProvider
                    string id = DocumentsContract.GetDocumentId(uri);

                    if (!string.IsNullOrEmpty(id) &&
                        id.StartsWith("raw:"))
                    {
                        return id.Substring(4);
                    }

                    string[] contentUriPrefixesToTry = new string[]
                    {
                        "content://downloads/public_downloads",
                        "content://downloads/my_downloads"
                    };

                    foreach (string contentUriPrefix in contentUriPrefixesToTry)
                    {
                        Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                            Android.Net.Uri.Parse(contentUriPrefix), long.Parse(id));

                        try
                        {
                            var path = GetDataColumn(context, contentUri, null, null);
                            if (path != null)
                            {
                                return path;
                            }
                        }
                        catch (Exception)
                        {
                            // ignore exception; path can't be retrieved using ContentResolver
                        }
                    }
                }
                else if (IsMediaDocument(uri))
                {
                    // MediaProvider
                    var docId = DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    var type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }

                    var selection = "_id=?";
                    var selectionArgs = new string[]
                    {
                        split[1]
                    };

                    return GetDataColumn(context, contentUri, selection, selectionArgs);
                }
            }

            // MediaStore (and general)
            if (IsMediaStore(uri.Scheme))
            {
                return GetDataColumn(context, uri, null, null);
            }
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        /// <summary>
        /// Checks if the scheme part of the URL matches the content:// scheme
        /// </summary>
        /// <param name="scheme">scheme part of URL</param>
        /// <returns>true when it matches, false when not</returns>
        public static bool IsMediaStore(string scheme)
        {
            return scheme.StartsWith("content");
        }

        /// <summary>
        /// Returns the "data" column of an Uri from the content resolver.
        /// </summary>
        /// <param name="context">Android context to access content resolver</param>
        /// <param name="uri">content Uri</param>
        /// <param name="selection">selection 'where' clause, or null</param>
        /// <param name="selectionArgs">selection arguments, or null</param>
        /// <returns>data column text, or null when query contained no data column</returns>
        public static string GetDataColumn(Context context, Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            ICursor cursor = null;
            string column = MediaStore.Files.FileColumns.Data;
            string[] projection = { column };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int column_index = cursor.GetColumnIndex(column);
                    if (column_index == -1)
                    {
                        return null;
                    }

                    string path = cursor.GetString(column_index);

                    // When the path has no root (i.e. is relative), better return null so that
                    // the content uri is used and the file contents can be read
                    if (path != null && !System.IO.Path.IsPathRooted(path))
                    {
                        return null;
                    }

                    return path;
                }
            }
            finally
            {
                if (cursor != null)
                {
                    cursor.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Returns if the given Uri is an ExternalStorageProvider Uri
        /// </summary>
        /// <param name="uri">the Uri to check</param>
        /// <returns>whether the Uri authority is an ExternalStorageProvider</returns>
        public static bool IsExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }

        /// <summary>
        /// Returns if the given Uri is a DownloadsProvider Uri
        /// </summary>
        /// <param name="uri">the Uri to check</param>
        /// <returns>whether the Uri authority is a DownloadsProvider</returns>
        public static bool IsDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }

        /// <summary>
        /// Returns if the given Uri is a MediaProvider Uri
        /// </summary>
        /// <param name="uri">the Uri to check</param>
        /// <returns>whether the Uri authority is a MediaProvider</returns>
        public static bool IsMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }

        /// <summary>
        /// Returns MIME type for given Url
        /// </summary>
        /// <param name="url">Url to check</param>
        /// <returns>MIME type, or null when none can be determined</returns>
        public static string GetMimeType(string url)
        {
            string type = null;
            var extension = MimeTypeMap.GetFileExtensionFromUrl(url);

            if (extension != null)
            {
                type = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
            }

            return type;
        }
    }
}
