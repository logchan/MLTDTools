using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;
using Newtonsoft.Json;
using OpenMLTD.MiriTore;
using OpenMLTD.MiriTore.Database;

namespace OpenMLTD.DanceExport {
    public sealed class MLTDResourceManager {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _data;
        private readonly ILogger _logger;

        public string ResVer { get; private set; }
        public List<AssetInfo> Assets { get; private set; }
        public Dictionary<string, AssetInfo> NameMapping { get; private set; }

        public MLTDResourceManager(string dataFolder, ILogger logger) {
            _data = dataFolder;
            _logger = logger;
        }

        public async Task Init() {
            // pick version
            var versionFile = Path.Combine(_data, "version.txt");
            if (!File.Exists(versionFile)) {
                _logger.Info("version.txt not found, downloading");
                await DownloadFile("https://api.matsurihi.me/mltd/v1/version/latest", versionFile);
            }
            var version = JsonConvert.DeserializeObject<MatsuriHimeVersion>(File.ReadAllText(versionFile)).Res;
            ResVer = version.Version;
            _logger.Info($"res version is {ResVer}");

            // get manifest
            var manifestFile = Path.Combine(_data, version.IndexName);
            if (!File.Exists(manifestFile)) {
                _logger.Info("manifest file not found, downloading");
                await DownloadAsset(version.IndexName, manifestFile);
            }

            // parse manifest
            var list = AssetInfoList.Parse(File.ReadAllBytes(manifestFile), MltdConstants.Utf8WithoutBom);
            Assets = list.Assets;

            NameMapping = new Dictionary<string, AssetInfo>();
            Assets.ForEach(a => NameMapping[a.ResourceName] = a);

            _logger.Info($"resman ready, with {list.Assets.Count} assets");
        }

        public async Task<byte[]> GetFile(string name) {
            var file = await GetFileName(name);
            return file == null ? null : File.ReadAllBytes(file);
        }

        public async Task<string> GetFileName(string name) {
            if (!NameMapping.TryGetValue(name, out var asset)) {
                return null;
            }

            var file = Path.Combine(_data, asset.RemoteName);
            if (!File.Exists(file)) {
                await DownloadAsset(asset.RemoteName, file);
            }

            return file;
        }

        private async Task DownloadFile(string url, string path) {
            var data = await _client.GetByteArrayAsync(url);
            File.WriteAllBytes(path, data);
        }

        private async Task DownloadAsset(string name, string path) {
            _logger.Info($"downloading asset {name}");
            var url = $"https://d2sf4w9bkv485c.cloudfront.net/{ResVer}/production/2018/iOS/{name}";
            await DownloadFile(url, path);
        }
        
        private class MatsuriHimeVersion {
            public VersionInfo Res { get; set; }

            public class VersionInfo {
                public string Version { get; set; }
                public string IndexName { get; set; }
            }
        }
    }
}
