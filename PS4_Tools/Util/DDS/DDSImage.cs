using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace DDSReader
{
	public class DDSImage
    {
		private readonly Pfim.IImage _image;

		public byte[] Data
		{
			get
			{
				if (_image != null)
					return _image.Data;
				else
					return new byte[0];
			}
		}

		//public DDSImage(string file)
		//{
		//	_image = Pfim.im.FromFile(file);
		//	Process();
		//}

		public DDSImage(Stream stream)
		{
			if (stream == null)
				throw new Exception("DDSImage ctor: Stream is null");

			_image = Pfim.Dds.Create(stream, new Pfim.PfimConfig());
			Process();
		}

		public DDSImage(byte[] data)
		{
			if (data == null || data.Length <= 0)
				throw new Exception("DDSImage ctor: no data");

			_image = Pfim.Dds.Create(data, new Pfim.PfimConfig());
			Process();
		}
        public void Save(Stream file)
        {
            if (_image.Format == Pfim.ImageFormat.Rgba32)
                Save<Bgra32>(file);
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
                Save<Bgr24>(file);
            else
                throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }


        public void Save(string file)
		{
			if (_image.Format == Pfim.ImageFormat.Rgba32)
				Save<Bgra32>(file);
			else if (_image.Format == Pfim.ImageFormat.Rgb24)
				Save<Bgr24>(file);
			else
				throw new Exception("Unsupported pixel format (" + _image.Format + ")");
		}

		private void Process()
		{
			if (_image == null)
				throw new Exception("DDSImage image creation failed");

			if (_image.Compressed)
				_image.Decompress();
		}

        private void Save<TPixel>(Stream file)
            where TPixel : struct, IPixel<TPixel>
        {
            Image<TPixel> image = Image.LoadPixelData<TPixel>(
                _image.Data, _image.Width, _image.Height);

            //image.Save<TPixel>(new FileStream(file,FileMode.OpenOrCreate,FileAccess.ReadWrite));
            image.Save(file, ImageFormats.Png);
            //Encode<TPixel>(image, new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite));
        }

        private void Save<TPixel>(string file)
			where TPixel : struct, IPixel<TPixel>
		{
			Image<TPixel> image = Image.LoadPixelData<TPixel>(
				_image.Data, _image.Width, _image.Height);

            //image.Save<TPixel>(new FileStream(file,FileMode.OpenOrCreate,FileAccess.ReadWrite));
            image.Save(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite), ImageFormats.Png);
           //Encode<TPixel>(image, new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite));
		}

        
    }
}
