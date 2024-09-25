using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GoogleOAuth
{
    public class GoogleManager
    {
        /***********************************************************************/
        /***********************************************************************/
        private static UserCredential GetUserCredential(string userId, string tokenResponse)
        {
            bool bUpdateToken = false;
            UserCredential credential = null;

            using (var stream = new FileStream("C:\\client_secret_test_oauth.json", FileMode.Open, FileAccess.Read))
            {
                if (String.IsNullOrEmpty(tokenResponse))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        new[] { DriveService.ScopeConstants.Drive },
                        userId,
                        CancellationToken.None,
                        new NullDataStore()).Result;

                    bUpdateToken = true;
                }
                else
                {
                    var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
                    {
                        ClientSecrets = GoogleClientSecrets.FromStream(stream).Secrets,
                        Scopes = new[] { DriveService.ScopeConstants.Drive },
                        DataStore = new NullDataStore()                        
                    });

                    TokenResponse tr = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);

                    if (flow != null && tr != null)
                    {                        
                        credential = new UserCredential(flow, userId, tr);
                        if (credential != null)
                            credential.Token.AccessToken = credential.GetAccessTokenForRequestAsync().Result;

                        if (String.Compare(tr.AccessToken, credential.Token.AccessToken) != 0)
                            bUpdateToken = true;
                    }
                }
            }

            if (bUpdateToken && credential?.Token != null)
            {
                string responseToken = JsonConvert.SerializeObject(credential.Token);
                //Update db with new TokenResponse
            }

            return credential;            
        }

        /***********************************************************************/
        /***********************************************************************/
        public static string RetrieveAccessToken(string userId, string tokenResponse)
        {
            string accessToken = String.Empty;

            UserCredential credential = GetUserCredential(userId, tokenResponse);
            

            return accessToken;
        }
        
        /***********************************************************************/
        /***********************************************************************/
        public static void RevokeAccessToken(string userId, string accessToken)
        {
            //ICredential credential = GetUserCredential(userId, accessToken);
            //credential.Rev
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
        public static void GetDriveFileInfo(string userId)
        {
            string FileId = GetIdFromUrl("https://drive.google.com/file/d/1pMqMmPak77vkdpvr2HzRdWN8NDRKfHfd/view?usp=sharing");

            string tokenResponse = "{ \"access_token\":\"ya29.a0ARrdaM8T0r62tCAuAYFashP-QKwxHjtlS6EPGcRgBFFgCjfyT7_neD_-ZWzp1oaQYpNXk_xRn7u4IdUWo8gxbA_k4FsQGH1b2whN1zefervX6heS1vvkl-AyDJPubC2q_-wFJPL1gRQTMQnTj4RTGvACsr6t\",\"token_type\":\"Bearer\",\"expires_in\":3599,\"refresh_token\":\"1//09MYh_M95JxZUCgYIARAAGAkSNwF-L9Irq4tIwjVIfN6cPy_Z_oCJAjpzVrVg6WXv8UNVhYe1_1m8xwchk6IhVN32DXAwo4nvTeY\",\"scope\":\"https://www.googleapis.com/auth/drive\",\"id_token\":null,\"expireTime\":null,\"Issued\":\"2021-12-17T10:21:10.6547854+01:00\",\"IssuedUtc\":\"2021-12-17T09:21:10.6547854Z\"}";

            UserCredential credential = GetUserCredential(userId, tokenResponse);

            //Create the Drive Service
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

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
            }
            catch (Google.GoogleApiException gex)
            {
                throw;
            }

        }

        /***********************************************************************/
        /***********************************************************************/
        public static void UploadFile(string userId)
        {
            
            string tokenResponse = "{ \"access_token\":\"ya29.a0ARrdaM8T0r62tCAuAYFashP-QKwxHjtlS6EPGcRgBFFgCjfyT7_neD_-ZWzp1oaQYpNXk_xRn7u4IdUWo8gxbA_k4FsQGH1b2whN1zefervX6heS1vvkl-AyDJPubC2q_-wFJPL1gRQTMQnTj4RTGvACsr6t\",\"token_type\":\"Bearer\",\"expires_in\":3599,\"refresh_token\":\"1//09MYh_M95JxZUCgYIARAAGAkSNwF-L9Irq4tIwjVIfN6cPy_Z_oCJAjpzVrVg6WXv8UNVhYe1_1m8xwchk6IhVN32DXAwo4nvTeY\",\"scope\":\"https://www.googleapis.com/auth/drive\",\"id_token\":null,\"expireTime\":null,\"Issued\":\"2021-12-17T10:21:10.6547854+01:00\",\"IssuedUtc\":\"2021-12-17T09:21:10.6547854Z\"}";

            UserCredential credential = GetUserCredential(userId, tokenResponse);

            //Create the Drive Service
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });


            string path = "C:\\test_abc.txt";
            
            var FileMetaData = new Google.Apis.Drive.v3.Data.File();
            FileMetaData.Name = "test_abc.txt";
            FileMetaData.MimeType = MimeMapping.GetMimeMapping(path);
            //FileMetaData.Parents = new List<string>() { GoogleDriveFilesRepository.FolderId };

            using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                FilesResource.CreateMediaUpload fileRequest;
                fileRequest = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                fileRequest.Fields = "id";
                fileRequest.Upload();

                Google.Apis.Drive.v3.Data.Permission permission = new Google.Apis.Drive.v3.Data.Permission();
                permission.Type = "anyone";
                permission.Role = "writer";

                PermissionsResource.CreateRequest permissionRequest;
                permissionRequest = service.Permissions.Create(permission, fileRequest.ResponseBody.Id);
                permissionRequest.Fields = "id";
                permissionRequest.Execute();

                //service.Permissions.Update(permission, fileRequest.ResponseBody.Id, PermissionRequest.);
            }

        }

    }
}
