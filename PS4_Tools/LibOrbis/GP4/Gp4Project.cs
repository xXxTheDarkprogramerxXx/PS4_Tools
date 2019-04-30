using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PS4_Tools.LibOrbis.GP4
{
    public class ChunkInfo
    {
        [XmlAttribute(AttributeName = "chunk_count")]
        public int chunk_count;

        [XmlAttribute(AttributeName = "scenario_count")]
        public int scenario_count;

        [XmlArrayItem(Type = typeof(Chunk), ElementName = "chunk")]
        [XmlArray(ElementName = "chunks")]
        public List<Chunk> chunks = new List<Chunk>();

        [XmlElement(ElementName = "scenarios")]
        public Scenarios scenarios;

        [XmlArrayItem(Type = typeof(ScenarioInfo), ElementName = "scenario_info")]
        [XmlArray(ElementName = "scenarios_info")]
        public List<ScenarioInfo> ScenariosInfo;
    }
    public class Chunk
    {
        [XmlAttribute(AttributeName = "id")]
        public int id;
        [XmlAttribute(AttributeName = "layer_no")]
        public int layer_no;
        [XmlAttribute(AttributeName = "label")]
        public string label;
    }
    public class Scenarios
    {
        [XmlAttribute(AttributeName = "default_id")]
        public int default_id;

        [XmlElement(Type = typeof(Scenario), ElementName = "scenario")]
        public List<Scenario> scenarios = new List<Scenario>();
    }
    public class Scenario
    {
        [XmlAttribute(AttributeName = "id")]
        public int id;
        [XmlAttribute(AttributeName = "type")]
        public string type;
        [XmlAttribute(AttributeName = "initial_chunk_count")]
        public int initial_chunk_count;
        [XmlAttribute(AttributeName = "label")]
        public string label;
        [XmlText]
        public string chunks;
    }
    public class ScenarioInfo
    {
        [XmlAttribute("id")]
        public int id;
        [XmlAttribute("type")]
        public string type;

        [XmlElement(ElementName = "lang")]
        public List<ScenarioLang> Langs;
    }
    public class ScenarioLang
    {
        [XmlAttribute("type")]
        public string type;
        [XmlAttribute("name")]
        public string name;
    }

    public class Volume
    {
        [XmlElement(ElementName = "volume_type")]
        public string volume_type;
        [XmlIgnore]
        public VolumeType Type
        {
            get { return VolumeTypeUtil.OfString(volume_type); }
            set { volume_type = value.ToString(); }
        }
        [XmlElement(ElementName = "volume_id")]
        public string Id;
        [XmlElement(ElementName = "volume_ts")]
        public string volume_ts;
        [XmlIgnore]
        public DateTime TimeStamp
        {
            get { return DateTime.Parse(volume_ts).ToUniversalTime(); }
            set { volume_ts = value.ToString("s").Replace('T', ' '); }
        }
        [XmlElement(ElementName = "package")]
        public PackageInfo Package;
        [XmlElement(ElementName = "chunk_info")]
        public ChunkInfo chunk_info;
    }

    public class PackageInfo
    {
        [XmlAttribute("content_id")]
        public string ContentId;
        [XmlAttribute("passcode")]
        public string Passcode;
        [XmlAttribute("entitlement_key")]
        public string EntitlementKey;
        [XmlAttribute("storage_type")]
        public string StorageType;
        [XmlAttribute("app_type")]
        public string AppType;
        [XmlAttribute("c_date")]
        public string CreationDate;
        [XmlIgnore]
        public DateTime CreationTimeStamp
        {
            get { return DateTime.Parse(CreationDate).ToUniversalTime(); }
        }
    }

    public enum VolumeType
    {
        bd25,
        bd50,
        bd50_50,
        bd50_25,
        bd25_50,
        exfat,
        pfs_plain,
        pfs_signed,
        pfs_nested,
        pkg_ps4_app,
        pkg_ps4_patch,
        pkg_ps4_remaster,
        pkg_ps4_ac_data,
        pkg_ps4_ac_nodata,
        pkg_ps4_sf_theme,
        pkg_ps4_theme,
    }

    public static class VolumeTypeUtil
    {
        public static VolumeType OfString(string s)
        {
            switch (s)
            {
                case "bd25": return VolumeType.bd25;
                case "bd50": return VolumeType.bd50;
                case "bd50_50": return VolumeType.bd50_50;
                case "bd50_25": return VolumeType.bd50_25;
                case "bd25_50": return VolumeType.bd25_50;
                case "exfat": return VolumeType.exfat;
                case "pfs_plain": return VolumeType.pfs_plain;
                case "pfs_signed": return VolumeType.pfs_signed;
                case "pfs_nested": return VolumeType.pfs_nested;
                case "pkg_ps4_app": return VolumeType.pkg_ps4_app;
                case "pkg_ps4_patch": return VolumeType.pkg_ps4_patch;
                case "pkg_ps4_remaster": return VolumeType.pkg_ps4_remaster;
                case "pkg_ps4_ac_data": return VolumeType.pkg_ps4_ac_data;
                case "pkg_ps4_ac_nodata": return VolumeType.pkg_ps4_ac_nodata;
                case "pkg_ps4_sf_theme": return VolumeType.pkg_ps4_sf_theme;
                case "pkg_ps4_theme": return VolumeType.pkg_ps4_theme;
                default: throw new Exception("Unknown Volume Type: " + s);
            }
        }
    }

    public class Dir
    {
        [XmlAttribute("targ_name")]
        public string TargetName;
        [XmlElement(ElementName = "dir")]
        public List<Dir> Children = new List<Dir>();
        [XmlIgnore]
        public Dir Parent;
        public string Path
        {
            get
            {
                var prefix = "";
                var dir = this;
                while (dir != null)
                {
                    prefix = dir.TargetName + "/" + prefix;
                    dir = dir.Parent;
                }
                return prefix;
            }
        }
    }

    public class Gp4File
    {
        [XmlAttribute("targ_path")]
        public string TargetPath;
        [XmlAttribute("orig_path")]
        public string OrigPath;
        public string FileName => TargetPath.Substring(TargetPath.LastIndexOf('/') + 1);
        public string DirName => TargetPath.Substring(0, TargetPath.LastIndexOf('/') + 1);
    }

    public class Files
    {
        [XmlAttribute("img_no")]
        public int ImageNum;
        [XmlElement(ElementName = "file")]
        public List<Gp4File> Items = new List<Gp4File>();
    }

    [XmlRoot(ElementName = "psproject")]
    public class Gp4Project
    {
        [XmlAttribute("fmt")]
        public string Format;
        [XmlAttribute("version")]
        public int version;
        [XmlElement(ElementName = "volume")]
        public Volume volume;
        [XmlElement(ElementName = "files")]
        public Files files;
        [XmlArrayItem(Type = typeof(Dir), ElementName = "dir")]
        [XmlArray(ElementName = "rootdir")]
        public List<Dir> RootDir = new List<Dir>();

        public static void WriteTo(Gp4Project proj, System.IO.Stream s)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(Gp4Project));
            mySerializer.Serialize(s, proj);
        }

        public static void setParent(List<Dir> dirs, Dir parent)
        {
            foreach (var dir in dirs)
            {
                dir.Parent = parent;
                setParent(dir.Children, dir);
            }
        }

        public static Gp4Project ReadFrom(System.IO.Stream s)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(Gp4Project));
            var proj = (Gp4Project)mySerializer.Deserialize(s);
            if (proj.volume.chunk_info?.ScenariosInfo?.Count() == 0)
            {
                proj.volume.chunk_info.ScenariosInfo = null;
            }
            // Fixup dir tree
           
      
            setParent(proj.RootDir, null);
            foreach (var file in proj.files.Items)
            {
                if (file.OrigPath == null)
                {
                    file.OrigPath = file.TargetPath.Replace('/', '\\');
                }
            }
            if (proj.volume.Package.EntitlementKey == null
              && (proj.volume.Type == VolumeType.pkg_ps4_ac_data
              || proj.volume.Type == VolumeType.pkg_ps4_ac_nodata))
            {
                proj.volume.Package.EntitlementKey = "00000000000000000000000000000000";
            }
            return proj;
        }

        public static Gp4Project Create(VolumeType type)
        {
            var proj = new Gp4Project
            {
                files = new Files(),
                Format = "gp4",
                RootDir = new List<Dir>(),
                version = 1000,
                volume = new Volume
                {
                    volume_ts = DateTime.UtcNow.ToString("s").Replace('T', ' '),
                    Package = new PackageInfo
                    {
                        ContentId = "XXXXXX-CUSA00000_00-ZZZZZZZZZZZZZZZZ",
                        Passcode = "00000000000000000000000000000000"
                    }
                }
            };
            proj.SetType(type);
            return proj;
        }

        #region Modification Functions

        public void RenameFile(Gp4File f, string newName)
        {
            f.TargetPath = f.DirName + newName;
        }

        public void RenameDir(Dir d, string newName)
        {
            var origPath = d.Path;
            d.TargetName = newName;
            var newPath = d.Path;
            foreach (var file in files.Items)
            {
                if (file.TargetPath.StartsWith(origPath))
                {
                    file.TargetPath = newPath + file.FileName;
                }
            }
        }

        /// <summary>
        /// Deletes the given file from this project.
        /// </summary>
        public void DeleteFile(Gp4File f)
        {
            files.Items.Remove(f);
        }

        /// <summary>
        /// Deletes the directory and all files and subdirectories.
        /// </summary>
        public void DeleteDir(Dir d)
        {
            var path = d.Path;
            var deleteQueue = new List<Gp4File>();
            // This covers all children files, too.
            foreach (var f in files.Items)
            {
                if (f.TargetPath.StartsWith(path))
                    deleteQueue.Add(f);
            }
            foreach (var f in deleteQueue)
            {
                files.Items.Remove(f);
            }
            RootDir.Remove(d);
            DeleteDirs(d);
        }

        public Dir AddDir(Dir parent, string name)
        {
            var newDir = new Dir
            {
                TargetName = name,
                Parent = parent,
                Children = new List<Dir>(),
            };
            (parent?.Children ?? RootDir).Add(newDir);
            return newDir;
        }

        /// <summary>
        /// Unlinks all directories in the given directory's subtree.
        /// </summary>
        private static void DeleteDirs(Dir dir)
        {
            dir.Parent?.Children.Remove(dir);
            dir.Parent = null;
            // Work on a copy of the children so we can modify the original list
            foreach (var d2 in dir.Children.ToList())
                DeleteDirs(d2);
            dir.Children.Clear();
            dir.Children = null;
        }

        public void SetType(VolumeType type)
        {
            if (volume.volume_type != null && type == volume.Type) return;
            switch (type)
            {
                case VolumeType.pkg_ps4_app:
                    volume.Package.EntitlementKey = null;
                    volume.Package.StorageType = "digital50";
                    volume.Package.AppType = "full";
                    volume.chunk_info = new ChunkInfo
                    {
                        chunks = new List<Chunk>
            {
              new Chunk
              {
                id = 0,
                layer_no = 0,
                label = "Chunk #0",
              }
            },
                        chunk_count = 1,
                        scenarios = new Scenarios
                        {
                            default_id = 0,
                            scenarios = new List<Scenario>
              {
                new Scenario
                {
                  id = 0,
                  type = "sp",
                  initial_chunk_count = 1,
                  label = "Scenario #0",
                  chunks = "0",
                }
              }
                        },
                        scenario_count = 1
                    };
                    break;
                case VolumeType.pkg_ps4_ac_data:
                    volume.Package.EntitlementKey = "00000000000000000000000000000000";
                    volume.Package.StorageType = null;
                    volume.Package.AppType = null;
                    volume.chunk_info = null;
                    break;
                default:
                    throw new Exception("Sorry, don't know how to make that project type!");
            }
            volume.Type = type;
        }
        #endregion
    }
}