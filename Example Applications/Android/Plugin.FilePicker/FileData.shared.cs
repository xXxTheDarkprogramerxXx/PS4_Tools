using System;
using System.IO;

namespace Plugin.FilePicker.Abstractions
{
    /// <summary>
    /// File data that specifies a file that was picked by the user.
    /// </summary>
    public sealed class FileData : IDisposable
    {
        /// <summary>
        /// Backing store for the FileName property
        /// </summary>
        private string _fileName;

        /// <summary>
        /// Backing store for the FilePath property
        /// </summary>
        private string _filePath;

        /// <summary>
        /// Indicates if the object is already disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Action to dispose of underlying resources of the picked file.
        /// </summary>
        private readonly Action<bool> _dispose;

        /// <summary>
        /// Function to get a stream to the picked file.
        /// </summary>
        private readonly Func<Stream> _streamGetter;

        /// <summary>
        /// Creates a new and empty file data object
        /// </summary>
        public FileData()
        {
        }

        /// <summary>
        /// Creates a new file data object with property values
        /// </summary>
        /// <param name="filePath">
        /// Full file path to the picked file.
        /// </param>
        /// <param name="fileName">
        /// File name of the picked file.
        /// </param>
        /// <param name="streamGetter">
        /// Function to get a stream to the picked file.
        /// </param>
        /// <param name="dispose">
        /// Action to dispose of the underlying resources of the picked file.
        /// </param>
        public FileData(string filePath, string fileName, Func<Stream> streamGetter, Action<bool> dispose = null)
        {
            _filePath = filePath;
            _fileName = fileName;
            _dispose = dispose;
            _streamGetter = streamGetter;
        }

        /// <summary>
        /// Completely reads all bytes from the input stream and returns it as byte array. Can be
        /// used when the returned file data consists of a stream, not a real filename.
        /// </summary>
        /// <param name="input">input stream</param>
        /// <returns>byte array</returns>
        internal static byte[] ReadFully(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Returns the raw data bytes for the picked file.
        /// Note that due to how this property is implemented, you may only
        /// call this once.  You can always access the underlying stream by
        /// directly calling GetStream().
        /// </summary>
        public byte[] DataArray
        {
            get
            {
                using (var stream = GetStream())
                {
                    return ReadFully(stream);
                }
            }
        }

        /// <summary>
        /// Filename of the picked file, without path
        /// </summary>
        public string FileName
        {
            get
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(null);

                return _fileName;
            }

            set
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(null);

                _fileName = value;
            }
        }

        /// <summary>
        /// Full filepath of the picked file.
        /// Note that on specific platforms this can also contain an URI that
        /// can't be opened with file related APIs. Use DataArray property or
        /// GetStream() method in this cases.
        /// </summary>
        public string FilePath
        {
            get
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(null);

                return _filePath;
            }

            set
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(null);

                _filePath = value;
            }
        }

        /// <summary>
        /// Gets stream to access the picked file.
        /// Note that when DataArray property was already accessed, the stream
        /// must be rewinded to the beginning.
        /// </summary>
        /// <returns>stream object</returns>
        public Stream GetStream()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(null);

            return _streamGetter();
        }

        #region IDispose implementation
        /// <summary>
        /// Disposes of all resources in the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of managed resources
        /// </summary>
        /// <param name="disposing">
        /// True when called from Dispose(), false when called from the destructor
        /// </param>
        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _dispose?.Invoke(disposing);
        }

        /// <summary>
        /// Finalizer for this object
        /// </summary>
        ~FileData()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
