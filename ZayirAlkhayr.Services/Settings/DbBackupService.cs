using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Settings;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.Settings
{
    public class DbBackupService: IDbBackupService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IAppSettings _appSettings;
        public DbBackupService(IWebHostEnvironment environment, IAppSettings appSettings)
        {
            _environment = environment;
            _appSettings = appSettings;
        }
        public string SaveDbBackupFile()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_appSettings.ConnectionStrings.DBConnection))
                {
                    connection.Open();
                    var backupFilePath = "";
                    string backupQuery = $"BACKUP DATABASE [db6936] TO DISK = '{backupFilePath}'";
                    SqlCommand command = new SqlCommand(backupQuery, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                    return backupFilePath;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string DownloadImagesFolder(ImageFiles Folder)
        {
            try
            {
                string folderPath = Path.Combine(_environment.WebRootPath, Folder.ToString());
                string zipFilePath = GetBackupImageFilePath(Folder);
                using (var zipArchive = new ZipArchive(File.Create(zipFilePath), ZipArchiveMode.Create))
                {
                    foreach (var filePath in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = filePath.Substring(folderPath.Length + 1);
                        var zipEntry = zipArchive.CreateEntry(relativePath);

                        using (var sourceStream = File.OpenRead(filePath))
                        using (var entryStream = zipEntry.Open())
                        {
                            sourceStream.CopyTo(entryStream);
                        }
                    }

                    return zipFilePath;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        private string GetBackupImageFilePath(ImageFiles Folder)
        {
            var FullPath = Path.Combine(_environment.WebRootPath, ImageFiles.ExportFiles.ToString());
            var FileName = DateTime.UtcNow.EgyptNow().ToString("dd-MM-yyyy") + "_" + Folder.ToString() + ".zip";
            return Path.Combine(FullPath, FileName);
        }
    }
}
