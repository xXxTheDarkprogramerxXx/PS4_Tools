

using PS4_Tools.LibOrbis.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PS4_Tools.Util;
//using System.IO.MemoryMappedFiles;

namespace PS4_Tools.LibOrbis.PFS
{
    public abstract class FSNode
    {
        public FSDir Parent = null;
        public string name;
        public inode ino;
        public virtual long Size { get; protected set; }
        public virtual long CompressedSize => Size;

        public string FullPath(string suffix = "")
        {
            if (Parent == null) return suffix;
            return Parent.FullPath("/" + name + suffix);
        }
    }

    public class FSDir : FSNode
    {
        public List<FSDir> Dirs = new List<FSDir>();
        public List<FSFile> Files = new List<FSFile>();


        public List<PfsDirent> Dirents = new List<PfsDirent>();

        public override long Size
        {
            get { return Dirents.Sum(d => d.EntSize); }
        }

        public List<FSNode> GetAllChildren()
        {
            var ret = new List<FSNode>(GetAllChildrenDirs().Count);
            //ret.AddRange(GetAllChildrenFiles());
            return ret;
        }

        public List<FSDir> GetAllChildrenDirs()
        {
            var ret = new List<FSDir>(Dirs);
            foreach (var dir in Dirs)
                foreach (var child in dir.GetAllChildrenDirs())
                    ret.Add(child);
            return ret;
        }

        public List<FSFile> GetAllChildrenFiles()
        {
            var ret = new List<FSFile>(Files);
            foreach (var dir in GetAllChildrenDirs())
                foreach (var f in dir.Files)
                    ret.Add(f);
            return ret;
        }

        public FSFile GetFile(string path)
        {
            var breadcrumbs = path.Split('/');
            if (breadcrumbs.Length == 1)
            {
                return Files.Find(f => f.name == path);
            }
            var dir = Dirs.Find(d => d.name == breadcrumbs[0]);
            return dir?.GetFile(path.Substring(path.IndexOf('/') + 1));
        }
    }

    public class FSFile : FSNode
    {
        public FSFile(string origFileName)
        {
            Write = s => { using (var f = File.OpenRead(origFileName)) f.CopyTo(s); };
            Size = new FileInfo(origFileName).Length;
            _compressedSize = Size;
        }
        public FSFile(PfsBuilder b)
        {
            var pfsc = new PFSCWriter(b.CalculatePfsSize());
            Write = s =>
            {
                pfsc.WritePFSCHeader(s);
                b.WriteImage(new OffsetStream(s, s.Position));
            };
            _compressedSize = b.CalculatePfsSize();
            Size = _compressedSize + pfsc.HeaderSize;
            name = "pfs_image.dat";
            Compress = true;
        }
        public FSFile(Action<Stream> writer, string name, long size)
        {
            Write = writer;
            this.name = name;
            Size = size;
            _compressedSize = Size;
        }
        private long _compressedSize;
        public override long CompressedSize => _compressedSize;
        public readonly Action<Stream> Write;
        public bool Compress = false;
    }
}
