using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace PS4_Tools.LibOrbis.PFS
{
    public class PfsProperties
    {
        public FSDir root;
        public long FileTime;
        public uint BlockSize;
        public bool Encrypt;
        public bool Sign;
        public byte[] EKPFS;
        public byte[] Seed;

        public static PfsProperties MakeInnerPFSProps(PKG.PkgProperties props)
        {
            // Generate keystone for GP PKGs if it is not already there
            if (props.VolumeType == GP4.VolumeType.pkg_ps4_app && props.RootDir.GetFile("sce_sys/keystone") == null)
            {
                AddFile(props.RootDir, "sce_sys", "keystone", Util.Crypto.CreateKeystone(props.Passcode));
            }
            var timestamp = props.TimeStamp.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            return new PfsProperties()
            {
                root = props.RootDir,
                BlockSize = 0x10000,
                Encrypt = false,
                Sign = false,
                FileTime = GetTimeStamp(props),
            };
        }

        public static PfsProperties MakeOuterPFSProps(PKG.PkgProperties props, PfsBuilder innerPFS, byte[] EKPFS, bool encrypt = true)
        {
            var root = new FSDir();
            root.Files.Add(new FSFile(innerPFS)
            {
                Parent = root,
            });
            return new PfsProperties()
            {
                root = root,
                BlockSize = 0x10000,
                Encrypt = encrypt,
                Sign = true,
                EKPFS = EKPFS,
                Seed = new byte[16],
                FileTime = GetTimeStamp(props),
            };
        }

        private static long GetTimeStamp(PKG.PkgProperties props)
        {
            // FIXME: This is incorrect when DST of current time and project time are different
            var timestamp = props.TimeStamp.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            return (long)timestamp;
        }

        static void AddFile(FSDir root, string path, string name, byte[] data)
        {
            var dir = FindDir(path, root);
            dir.Files.Add(new FSFile(s => s.Write(data, 0, data.Length), name, data.Length) { Parent = dir });
        }

        public static void AddDirs(FSDir parent, List<GP4.Dir> imgDir)
        {
            foreach (var d in imgDir)
            {
                FSDir dir;
                parent.Dirs.Add(dir = new FSDir { name = d.TargetName, Parent = parent });
                AddDirs(dir, d.Children);
            }
        }

        /// <summary>
        /// Takes a directory and a root node, and recursively makes a filesystem tree.
        /// </summary>
        /// <param name="proj">GP4 Project</param>
        /// <param name="projDir">Directory of GP4 file</param>
        /// <returns>Root directory of the image</returns>
        public static FSDir BuildFSTree(GP4.Gp4Project proj, string projDir)
        {
          
      
            var root = new FSDir();
            AddDirs(root, proj.RootDir);

            foreach (var f in proj.files.Items)
            {
                var lastSlash = f.TargetPath.LastIndexOf('/') + 1;
                var name = f.TargetPath.Substring(lastSlash);
                var source = Path.Combine(projDir, f.OrigPath);
                var parent = lastSlash == 0 ? root : FindDir(f.TargetPath.Substring(0, lastSlash - 1), root);
                parent.Files.Add(new FSFile(source)
                {
                    Parent = parent,
                    name = name,
                });
            }
            return root;
        }

        static FSDir FindDir(string name, FSDir root)
        {
            FSDir dir = root;
            var breadcrumbs = name.Split('/');
            foreach (var crumb in breadcrumbs)
            {
                dir = dir.Dirs.Where(d => d.name == crumb).First();
            }
            return dir;
        }
    }
}