using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.ZAInstitution.Settings;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.Settings
{
    public class DbBackupService : IDbBackupService
    {
        private readonly IAppSettings _appSettings;
        private readonly string _webRootPath;
        private string ConnectionString;

        public DbBackupService(IAppSettings appSettings, IOptions<AppPaths> options)
        {
            _appSettings = appSettings;
            _webRootPath = options.Value.WebRootPath;
            ConnectionString = _appSettings.ConnectionStrings.DBConnection;
        }

        public string SaveDbBackupFile()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
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
                string folderPath = Path.Combine(_webRootPath, Folder.ToString());
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
            var FullPath = Path.Combine(_webRootPath, ImageFiles.ExportFiles.ToString());
            var FileName = DateTime.Now.ToString("dd-MM-yyyy") + "_" + Folder.ToString() + ".zip";
            return Path.Combine(FullPath, FileName);
        }
    }
}
