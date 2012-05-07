using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace RHITMobile {
    public class FileHostHandler : PathHandler {
        public static DirectoryInfo Root = new DirectoryInfo(Program.FileHostPath);

        public FileHostHandler() {
            Redirects.Add("upload", new FileHostUploadHandler());
            Redirects.Add("remove", new FileHostRemoveHandler());
            Redirects.Add("download", new FileHostDownloadHandler());
            Redirects.Add("folders", new FileHostFoldersHandler());
        }

        public override IEnumerable<ThreadInfo> VerifyHeaders(ThreadManager TM, NameValueCollection headers, object state) {
            return AdminHandler.VerifyToken(TM, headers);
        }
    }

    public class FileHostUploadHandler : PathHandler {
        public FileHostUploadHandler() {
            UnknownRedirect = new FileHostUploadFolderHandler();
        }

        protected override IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, string path, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, path);
        }

        protected override IEnumerable<ThreadInfo> HandleNoPathExtended(ThreadManager TM, Dictionary<string, string> query, System.Net.HttpListenerContext context, object state) {
            var currentThread = TM.CurrentThread;

            var parser = new MultipartParser(context.Request.InputStream, context.Request.ContentEncoding);
            if (!parser.Success)
                throw new BadRequestException(currentThread, "Could not parse the input stream.");
            string[] fileParts = parser.Filename.Split('.');
            if (fileParts.Length != 2 || fileParts[1].ToLower() != "zip")
                throw new BadRequestException(currentThread, "File is not a .zip file.");
            if (!FileHostHandler.Root.Exists)
                throw new BadRequestException(currentThread, "Root directory {0} does not exist", FileHostHandler.Root.FullName);

            string folderPath = String.Format("{0}\\{1}", FileHostHandler.Root.FullName, fileParts[0]);
            string zipFilePath = String.Format("{0}\\{1}", FileHostHandler.Root.FullName, parser.Filename);

            if (Directory.Exists(folderPath)) {
                yield return TM.Await(currentThread, FileHostRemoveFolderHandler.DeleteFolder(TM, folderPath));
                TM.GetResult<JsonResponse>(currentThread);
            }

            yield return TM.StartNewThread(currentThread, () => {
                File.WriteAllBytes(zipFilePath, parser.FileContents);
                return true;
            });
            TM.GetResult<bool>(currentThread);

            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + "unzip.exe");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.Arguments = String.Format("{0} -d {1}\\{2}", zipFilePath, FileHostHandler.Root.FullName, fileParts[0]);

            processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            yield return TM.StartNewThread(currentThread, () => {
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(processInfo);
                p.WaitForExit();
                return true;
            });
            TM.GetResult<bool>(currentThread);

            yield return TM.StartNewThread(currentThread, () => {
                File.Delete(zipFilePath);
                return true;
            });
            TM.GetResult<bool>(currentThread);

            yield return TM.Return(currentThread, new JsonResponse(new MessageResponse("File successfully uploaded.")));
        }
    }

    public class FileHostUploadFolderHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPathExtended(ThreadManager TM, Dictionary<string, string> query, System.Net.HttpListenerContext context, object state) {
            var currentThread = TM.CurrentThread;

            if (!FileHostHandler.Root.Exists)
                throw new BadRequestException(currentThread, "Root directory {0} does not exist", FileHostHandler.Root.FullName);

            string folderName = (string)state;
            string folderPath = String.Format("{0}\\{1}", FileHostHandler.Root.FullName, folderName);
            string zipFilePath = String.Format("{0}\\{1}.zip", FileHostHandler.Root.FullName, folderName);

            if (Directory.Exists(folderPath)) {
                yield return TM.Await(currentThread, FileHostRemoveFolderHandler.DeleteFolder(TM, folderPath));
                TM.GetResult<JsonResponse>(currentThread);
            }

            yield return TM.StartNewThread(currentThread, () => {
                using (var writer = File.Create(zipFilePath)) {
                    context.Request.InputStream.CopyTo(writer);
                    return true;
                }
            });
            TM.GetResult<bool>(currentThread);

            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + "unzip.exe");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.Arguments = String.Format("\"{0}\" -d \"{1}\\{2}\"", zipFilePath, FileHostHandler.Root.FullName, folderName);

            processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            yield return TM.StartNewThread(currentThread, () => {
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(processInfo);
                p.WaitForExit();
                return true;
            });
            TM.GetResult<bool>(currentThread);

            yield return TM.StartNewThread(currentThread, () => {
                File.Delete(zipFilePath);
                return true;
            });
            TM.GetResult<bool>(currentThread);

            yield return TM.Return(currentThread, new JsonResponse(new MessageResponse("File successfully uploaded.")));
        }
    }

    public class FileHostRemoveHandler : PathHandler {
        public FileHostRemoveHandler() {
            UnknownRedirect = new FileHostRemoveFolderHandler();
        }

        protected override IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, string path, object state) {
            var currentThread = TM.CurrentThread;

            if (!FileHostHandler.Root.Exists)
                throw new BadRequestException(currentThread, "Root directory {0} does not exist", FileHostHandler.Root.FullName);
            string folderPath = String.Format("{0}\\{1}", FileHostHandler.Root.FullName, path);
            if (!Directory.Exists(folderPath))
                throw new BadRequestException(currentThread, "Folder '{0}' does not exist.", path);

            yield return TM.Return(currentThread, folderPath);
        }
    }

    public class FileHostRemoveFolderHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            return DeleteFolder(TM, (string)state);
        }

        public static IEnumerable<ThreadInfo> DeleteFolder(ThreadManager TM, string folderPath) {
            var currentThread = TM.CurrentThread;
            yield return TM.StartNewThread(currentThread, () => {
                Directory.Delete(folderPath, true);
                return true;
            });
            TM.GetResult<bool>(currentThread);

            yield return TM.Return(currentThread, new JsonResponse(new MessageResponse("Folder successfully deleted.")));
        }
    }

    public class FileHostDownloadHandler : PathHandler {
        public FileHostDownloadHandler() {
            UnknownRedirect = new FileHostDownloadFolderHandler();
        }

        protected override IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, string path, object state) {
            var currentThread = TM.CurrentThread;

            if (!FileHostHandler.Root.Exists)
                throw new BadRequestException(currentThread, "Root directory {0} does not exist", FileHostHandler.Root.FullName);
            string folderPath = String.Format("{0}\\{1}", FileHostHandler.Root.FullName, path);
            if (!Directory.Exists(folderPath))
                throw new BadRequestException(currentThread, "Folder '{0}' does not exist.", path);

            yield return TM.Return(currentThread, folderPath);
        }
    }

    public class FileHostDownloadFolderHandler : PathHandler {
        public FileHostDownloadFolderHandler() {
            UnknownRedirect = new FileHostDownloadFolderFileHandler();
        }

        protected override IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, string path, object state) {
            var currentThread = TM.CurrentThread;

            string filePath = String.Format("{0}\\{1}", (string)state, path);
            if (!File.Exists(filePath))
                throw new BadRequestException(currentThread, "File '{0}' does not exist.", path);

            yield return TM.Return(currentThread, filePath);
        }
    }

    public class FileHostDownloadFolderFileHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPathExtended(ThreadManager TM, Dictionary<string, string> query, System.Net.HttpListenerContext context, object state) {
            var currentThread = TM.CurrentThread;

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            using (var reader = File.OpenRead((string)state)) {
                reader.CopyTo(context.Response.OutputStream);
            }
            context.Response.OutputStream.Close();

            yield return TM.Return(currentThread, null);
        }
    }

    public class FileHostFoldersHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;

            if (!FileHostHandler.Root.Exists)
                throw new BadRequestException(currentThread, "Root directory {0} does not exist", FileHostHandler.Root.FullName);

            yield return TM.Return(currentThread, new JsonResponse(new FoldersResponse(
                FileHostHandler.Root.EnumerateDirectories().Select(di => di.Name).ToList())));
        }
    }
}
