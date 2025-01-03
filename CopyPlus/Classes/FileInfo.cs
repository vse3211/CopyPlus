using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CopyPlus.Classes
{
    public class FileInfo
    {
        public string BaseFilePath { get; set; }
        public string BaseSourcePath { get; set; }
        public string BaseDestinationPath { get; set; }
        public string SourcePath => Path.Combine(BaseSourcePath, BaseFilePath);
        public string DestinationPath => Path.Combine(BaseDestinationPath, BaseFilePath);
        public string SourceSha256() => GetSHA256(SourcePath);
        public string DestinationSha256() => GetSHA256(DestinationPath);
        private bool copyInProgress = false;
        private bool isJustCheck = false;
        public bool isCopyCompleted => !copyInProgress && File.Exists(SourcePath) && File.Exists(DestinationPath) && SourceSha256() == DestinationSha256();
        public CopyStatus GetStatus {
            get
            {
                if (copyInProgress) return CopyStatus.InProgress;
                if (isJustCheck && File.Exists(DestinationPath)) return CopyStatus.Exists;
                if (isCopyCompleted) return CopyStatus.Success;
                if (File.Exists(DestinationPath)) return CopyStatus.Error;
                return CopyStatus.NotStarted;
            }
        }
        private CopyStatus? LastStatus = null;
        public CopyStatus Status { get { if (LastStatus is null) LastStatus = GetStatus; return (CopyStatus)LastStatus; } set { LastStatus = value; } }

        public FileInfo(bool copyOnCreated = false, bool justCheck = true)
        {
            if (copyOnCreated) _ = Task.Run(() => Copy(this));
            isJustCheck = justCheck;
        }

        public void SetCheckMode(bool justCheck) => isJustCheck = justCheck;

        public Task Copy() => Copy(this);
        public static Task Copy(FileInfo info, int trys = 0)
        {
            info.SetCheckMode(false);
            info.Status = info.GetStatus;
            if (trys >= 5) return Task.FromException(new Exception($"Out of trys! File: {info.BaseFilePath}"));
            if (info.Status == CopyStatus.Success) return Task.CompletedTask;
            if (File.Exists(info.DestinationPath)) File.Delete(info.DestinationPath);
            File.Copy(info.SourcePath, info.DestinationPath);
            trys++;
            Copy(info, trys);
            info.SetCheckMode(true);
            return Task.CompletedTask;
        }
        public static string GetSHA256(string filePath)
        {
            try
            {
                using (FileStream stream = File.OpenRead(filePath))
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);
                    return Convert.ToHexString(hashBytes);
                }
            }
            catch
            {
                return String.Empty;
            }
        }
    }
}
