using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS4_Tools.LibOrbis.PKG
{
    /// <summary>
    /// Collection of data to use when building a PKG.
    /// </summary>
    public class PkgProperties
    {
        /// <summary>
        /// The volume type (should be one of the pkg_* types)
        /// </summary>
        public GP4.VolumeType VolumeType;

        /// <summary>
        /// 36 Character ID for the PKG
        /// </summary>
        public string ContentId;

        /// <summary>
        /// 32 Character Passcode
        /// </summary>
        public string Passcode;

        /// <summary>
        /// The volume timestamp.
        /// </summary>
        public DateTime TimeStamp;

        /// <summary>
        /// 32 Hex Character Entitlement Key (For AC, AL only)
        /// </summary>
        public string EntitlementKey;

        /// <summary>
        /// The creation date/time. Leave as default(DateTime) to disable
        /// </summary>
        public DateTime CreationDate;

        /// <summary>
        /// Set to true to use the creation time in addition to the date
        /// </summary>
        public bool UseCreationTime;

        /// <summary>
        /// The root of the directory tree for the PFS image.
        /// </summary>
        public PFS.FSDir RootDir;

        public static PkgProperties FromGp4(GP4.Gp4Project project, string projDir)
        {
            DateTime CreationDate;
            bool UseCreationTime = false;
            if ((project.volume.Package.CreationDate ?? "") == "")
            {
                CreationDate = DateTime.Now;
            }
            else if (project.volume.Package.CreationDate == "actual_datetime")
            {
                CreationDate = DateTime.Now;
                UseCreationTime = true;
            }
            else
            {
                var split = project.volume.Package.CreationDate.Split(' ');
                UseCreationTime = split.Length == 2; // Date and time specified
                CreationDate = DateTime.Parse(project.volume.Package.CreationDate).ToUniversalTime();
            }
            return new PkgProperties
            {
                ContentId = project.volume.Package.ContentId,
                VolumeType = GP4.VolumeTypeUtil.OfString(project.volume.volume_type),
                Passcode = project.volume.Package.Passcode,
                TimeStamp = project.volume.TimeStamp,
                EntitlementKey = project.volume.Package.EntitlementKey,
                CreationDate = CreationDate,
                UseCreationTime = UseCreationTime,
                RootDir = PFS.PfsProperties.BuildFSTree(project, projDir)
            };
        }
    }
}
