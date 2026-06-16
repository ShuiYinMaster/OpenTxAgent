// TxAgent / Core / KeyStore.cs
// API key 的本地持久化。默认写入插件文件夹 (随程序集)，用 DPAPI 按当前用户加密，不存明文。
// 若插件目录不可写 (例如部署在 Program Files)，自动回退到 %LOCALAPPDATA%\TxAgent。

using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography; // 需引用 System.Security
using System.Text;

namespace TxAgent.Core
{
    public static class KeyStore
    {
        private const string FileName = "deepseek.key";

        /// <summary>读取已保存的 key；没有或解密失败返回 null。</summary>
        public static string Load()
        {
            foreach (var path in CandidatePaths())
            {
                try
                {
                    if (!File.Exists(path)) continue;
                    var blob = Convert.FromBase64String(File.ReadAllText(path, Encoding.ASCII).Trim());
                    var bytes = ProtectedData.Unprotect(blob, null, DataProtectionScope.CurrentUser);
                    var key = Encoding.UTF8.GetString(bytes);
                    if (!string.IsNullOrWhiteSpace(key)) return key;
                }
                catch
                {
                    // 换下一个候选路径
                }
            }
            return null;
        }

        /// <summary>保存 key。返回实际写入的完整路径；全部失败抛异常。</summary>
        public static string Save(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("API key 不能为空。", nameof(key));

            var blob = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(key.Trim()), null, DataProtectionScope.CurrentUser);
            var text = Convert.ToBase64String(blob);

            Exception last = null;
            foreach (var path in CandidatePaths())
            {
                try
                {
                    var dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                    File.WriteAllText(path, text, Encoding.ASCII);
                    return path;
                }
                catch (Exception ex)
                {
                    last = ex; // 尝试下一个
                }
            }
            throw new IOException("无法写入 key 文件。", last);
        }

        public static void Clear()
        {
            foreach (var path in CandidatePaths())
            {
                try { if (File.Exists(path)) File.Delete(path); } catch { }
            }
        }

        // 候选路径，按优先级：插件文件夹 -> 用户本地目录。
        private static string[] CandidatePaths()
        {
            string pluginDir = null;
            try { pluginDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
            catch { /* 忽略 */ }

            var localDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TxAgent");

            if (string.IsNullOrEmpty(pluginDir))
                return new[] { Path.Combine(localDir, FileName) };

            return new[]
            {
                Path.Combine(pluginDir, FileName),
                Path.Combine(localDir, FileName)
            };
        }
    }
}
