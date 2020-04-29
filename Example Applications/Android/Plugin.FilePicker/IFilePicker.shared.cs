using System;
using System.Threading.Tasks;

namespace Plugin.FilePicker.Abstractions
{
    /// <summary>
    /// Interface for FilePicker plugin. Access the platform specific instance
    /// of this interface by using CrossFilePicker.Current.
    /// </summary>
    public interface IFilePicker
    {
        /// <summary>
        /// Starts file picking and returns file data for picked file. File
        /// types can be specified in order to limit files that can be
        /// selected. Note that this method may throw exceptions that occured
        /// during file picking.
        /// </summary>
        /// <param name="allowedTypes">
        /// Specifies one or multiple allowed types. When null, all file types
        /// can be selected while picking.
        /// On Android you can specify one or more MIME types, e.g.
        /// "image/png"; also wild card characters can be used, e.g. "image/*".
        /// On iOS you can specify UTType constants, e.g. UTType.Image.
        /// On UWP, specify a list of extensions, like this: ".jpg", ".png".
        /// On WPF, specify strings like this: "Data type (*.ext)|*.ext", which
        /// corresponds how the Windows file open dialog specifies file types.
        /// </param>
        /// <returns>
        /// File data object, or null when user cancelled picking file
        /// </returns>
        Task<FileData> PickFile(string[] allowedTypes = null);

        /// <summary>
        /// Saves the file that was picked to external storage.
        /// </summary>
        /// <param name="fileToSave">
        /// File data from a call to PickFile() that should be saved.
        /// </param>
        /// <returns>
        /// True when file was saved successfully, false when there was an
        /// error
        /// </returns>
        [Obsolete("The SaveFile() method is obsolete; store the picked file with system functions from System.IO or read from stream directly")]
        Task<bool> SaveFile(FileData fileToSave);

        /// <summary>
        /// Opens the file with given filename in an external application that
        /// is registered for this file type.
        /// </summary>
        /// <param name="fileToOpen">
        /// Full filename of the file to open
        /// </param>
        [Obsolete("The OpenFile() method is obsolete; please use Xamarin.Essentials Launcher plugin. The FilePicker plugins just concentrates on picking files")]
        void OpenFile(string fileToOpen);

        /// <summary>
        /// Opens the file specified by first storing the file to external
        /// storage, then opening it in an external application that is
        /// registered for this file type. This is a combination of SaveFile()
        /// and OpenFile() above.
        /// </summary>
        /// <param name="fileToOpen">
        /// File data from a call to PickFile() that should be opened.
        /// </param>
        [Obsolete("The OpenFile() method is obsolete; please use Xamarin.Essentials Launcher plugin. The FilePicker plugins just concentrates on picking files")]
        void OpenFile(FileData fileToOpen);
    }
}
