using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.Settings
{
    public interface IDbBackupService
    {
        string SaveDbBackupFile();
        string DownloadImagesFolder(ImageFiles Folder);
    }
}
