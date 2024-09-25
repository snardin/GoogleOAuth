using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoogleServiceAccount
{
    public class GoogleManager
    {
        private static string ServiceAccountEmail = "test-service01@black-rhino-316809.iam.gserviceaccount.com";
        private static string ServiceAccountKeyFile = @"C:\black-rhino-316809-cd155a96b56c.json";

        /***********************************************************************/
        /***********************************************************************/
        private static DriveService GetService()
        {
            // Load the Service account credentials and define the scope of its access
            var credential = GoogleCredential.FromFile(ServiceAccountKeyFile)
                .CreateScoped(DriveService.ScopeConstants.Drive);

            //Create the Drive Service
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            return service;
        }

        /***********************************************************************/
        /***********************************************************************/
        private static string GetIdFromUrl(string url)
        {
            Regex r = new Regex(@"\/d\/(.+)\/", RegexOptions.IgnoreCase);
            Match m = r.Match(url);
            return m.ToString().TrimStart('/', 'd').Trim('/');
        }

        /***********************************************************************/
        /***********************************************************************/
        public static GoogleDriveFileInfo GetDriveFileInfo(string url)
        {
            GoogleDriveFileInfo FileInfo = null;

            string FileId = GetIdFromUrl(url);

            DriveService service = GetService();

            // Define parameters of request.
            FilesResource.GetRequest FileRequest = service.Files.Get(FileId);
            FileRequest.Fields = "id, name, size, version, trashed, createdTime, webViewLink, mimeType";

            PermissionsResource.ListRequest PermissionRequest = service.Permissions.List(FileId);
            PermissionRequest.Fields = "permissions(emailAddress)";

            try
            {
                // Get the file info
                Google.Apis.Drive.v3.Data.File fileRes = FileRequest.Execute();

                // Get the permissions
                PermissionList permissions = PermissionRequest.Execute();

                FileInfo = new GoogleDriveFileInfo
                {
                    Id = fileRes.Id,
                    Name = fileRes.Name,
                    Size = fileRes.Size,
                    Version = fileRes.Version,
                    CreatedTime = fileRes.CreatedTime,
                    Link = fileRes.WebViewLink,
                    IsFolder = (fileRes.MimeType == "application/vnd.google-apps.folder"),
                    Permissions = new List<string>()
                };

                foreach (var item in permissions.Permissions)
                {
                    FileInfo.Permissions.Add(item.EmailAddress);
                }
                
            }
            catch (Google.GoogleApiException gex)
            {
                throw;
            }

            return FileInfo;
        }

        /***********************************************************************/
        /***********************************************************************/
        public static bool SetDriveFilePermission(string url, string newEmail)
        {
            bool ret = false;

            string FileId = GetIdFromUrl(url);

            DriveService service = GetService();

            try
            {
                Permission newPermission = new Permission();
                newPermission.EmailAddress = newEmail;

                //The value "user", "group", "domain" or "default"
                newPermission.Type = "user";

                //The value "owner", "writer" or "reader"
                newPermission.Role = "writer";
                                
                service.Permissions.Create(newPermission, FileId).Execute();

                ret = true;
            }
            catch (Google.GoogleApiException gex)
            {
                throw;
            }
            return ret;
        }
    }
}
