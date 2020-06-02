using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using OpenMLTD.MiriTore;
using OpenMLTD.MiriTore.Database;

namespace OpenMLTD.DanceExport {
    public partial class FMain : Form, ILogger {
        private const string ConfigFile = "dance-export.json";

        private readonly Config _config;
        private MLTDResourceManager _res;
        private readonly List<IdolSelector> _selectors;
        private readonly List<IdolInfo> _idols = new List<IdolInfo>();

        public FMain() {
            InitializeComponent();

            T MakeControl<T>(T reference) where T : Control, new() {
                var c = new T {
                    Width = reference.Width,
                    Height = reference.Height
                };
                if (c is PictureBox pic) {
                    pic.BorderStyle = BorderStyle.FixedSingle;
                }
                if (c is ComboBox cb) {
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                mainGroup.Controls.Add(c);
                return c;
            }

            _selectors = new List<IdolSelector>(5);
            for (var i = 0; i < 5; ++i) {
                var selector = new IdolSelector {
                    AvatarBox = MakeControl(avatar1),
                    IdolCombo = MakeControl(idolCombo1),
                    CostumeBox = MakeControl(avatar1),
                    CostumeCombo = MakeControl(idolCombo1)
                };
                var idx = i;
                selector.IdolCombo.SelectedIndexChanged += (sender, ev) => IdolComboSelectedIndexChanged(idx);
                _selectors.Add(selector);
            }
            RearrangeSelectors();

            if (File.Exists(ConfigFile)) {
                _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFile));
                _config.LastWindow.RestoreForm(this);
            }
            else {
                _config = new Config();
            }
        }

        private void IdolComboSelectedIndexChanged(int selectorIndex) {
            var selector = _selectors[selectorIndex];
            selector.CostumeCombo.Items.Clear();

            var idol = _idols[selector.IdolCombo.SelectedIndex];
            idol.Costumes.ForEach(costume => { selector.CostumeCombo.Items.Add(costume.Name); });
        }

        private void RearrangeSelectors() {
            var left = avatar1.Left;
            var top = avatar1.Top;
            var width = avatar1.Width;

            var xMargin = (mainGroup.Width - 2 * left - width) / 4;
            var yMargin = idolCombo1.Height + 20;

            for (var i = 0; i < 5; ++i) {
                var selector = _selectors[i];
                var sLeft = left + i * xMargin;
                selector.AvatarBox.Top = top;
                selector.AvatarBox.Left = sLeft;
                selector.IdolCombo.Top = idolCombo1.Top;
                selector.IdolCombo.Left = sLeft;
                selector.CostumeBox.Top = idolCombo1.Top + yMargin;
                selector.CostumeBox.Left = sLeft;
                selector.CostumeCombo.Top = selector.CostumeBox.Top + (idolCombo1.Top - avatar1.Top);
                selector.CostumeCombo.Left = sLeft;
            }
        }

        #region Logging
        private void AddLog(string lvl, string msg, Color color) {
            if (InvokeRequired) {
                Invoke(new Action(() => AddLog(lvl, msg, color)));
                return;
            }

            var time = DateTime.Now.ToString("HH:mm:ss");
            msg = $"{time} [{lvl}] {msg}";
            if (logBox.Text.Length > 0) {
                logBox.AppendText(Environment.NewLine);
            }
            logBox.AppendText(msg);
            logBox.Select(logBox.Text.Length - msg.Length, msg.Length);
            logBox.SelectionColor = color;
            logBox.Select(logBox.Text.Length, 0);
            logBox.ScrollToCaret();
        }

        public void Info(string msg) {
            AddLog("INF", msg, Color.Black);
        }

        public void Error(string msg) {
            AddLog("ERR", msg, Color.Red);
        }

        public void Warning(string msg) {
            AddLog("WRN", msg, Color.SandyBrown);
        }
        #endregion

        private void FMain_FormClosing(object sender, FormClosingEventArgs e) {
            _config.LastWindow.SetFrom(this);
            var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
            File.WriteAllText(ConfigFile, json);
        }

        private static string PickFolder(string title) {
            var dialog = new CommonOpenFileDialog {
                EnsurePathExists = true,
                AllowNonFileSystemItems = false,
                IsFolderPicker = true,
                Title = title
            };

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok) {
                return dialog.FileName;
            }
            return null;
        }

        private async void FMain_Shown(object sender, EventArgs e) {
            if (String.IsNullOrWhiteSpace(_config.CacheFolder) || !Directory.Exists(_config.CacheFolder)) {
                _config.CacheFolder = PickFolder("Pick cache folder");
                if (_config.CacheFolder == null) {
                    Environment.Exit(0);
                }
            }

            _res = new MLTDResourceManager(_config.CacheFolder, this);
            await _res.Init();
            await Task.Run(InitCostumes);
            _idols.ForEach(idol => { _selectors.ForEach(selector => { selector.IdolCombo.Items.Add(idol.Name); }); });
        }

        private void InitCostumes() {
            foreach (var info in MltdConstants.Idols) {
                _idols.Add(new IdolInfo {
                    Name = info.Name,
                    Id = $"{info.IdolID:000}{info.Abbreviation}"
                });
            }

            var mapping = new Dictionary<string, CostumeInfo>();
            var idMapping = new Dictionary<string, List<CostumeInfo>>();
            var chRegex = new Regex(@"^ch_(?<id>[a-z0-9]+)_(?<idol>[a-z0-9]+)(_v2)?\.unity3d$");
            _res.Assets.ForEach(asset => {
                if (!asset.ResourceName.StartsWith("ch_")) {
                    return;
                }

                var m = chRegex.Match(asset.ResourceName);
                if (!m.Success) {
                    return;
                }

                var id = m.Groups["id"].Value;
                var idolId = m.Groups["idol"].Value;
                var isV2 = asset.ResourceName.Contains("_v2");
                var key = id + idolId;

                if (mapping.ContainsKey(key) && !isV2) {
                    return;
                }

                var costume = new CostumeInfo {
                    Id = id,
                    Name = id, // TODO
                    ChAsset = asset
                };
                mapping[key] = costume;
                _idols.First(i => i.Id == idolId).Costumes.Add(costume);

                if (!idMapping.TryGetValue(id, out var list)) {
                    idMapping[id] = list = new List<CostumeInfo>();
                }
                list.Add(costume);
            });

            var cbRegex = new Regex(@"^cb_(?<id>[a-z0-9]+)_(?<idol>[a-z0-9]+)(_v2)?\.unity3d$");
            _res.Assets.ForEach(asset => {
                var m = cbRegex.Match(asset.ResourceName);
                if (!m.Success) {
                    return;
                }

                var id = m.Groups["id"].Value;
                var idolId = m.Groups["idol"].Value;
                var isV2 = asset.ResourceName.Contains("_v2");
                var key = id + idolId;

                // unseen id
                if (!idMapping.TryGetValue(id, out var list)) {
                    Warning($"unseen id {id} in cb stage");
                    return;
                }

                // generic id
                if (!Char.IsDigit(id[0])) {
                    foreach (var costume in list) {
                        costume.CbAsset ??= asset;
                    }
                }
                else {
                    if (!mapping.TryGetValue(key, out var costume)) {
                        Warning($"unseen key {key} in cb stage");
                        return;
                    }

                    if (isV2 || costume.CbAsset == null) {
                        costume.CbAsset = asset;
                    }
                }
            });

            var totalCount = 0;
            var validCount = 0;
            _idols.ForEach(idol => {
                idol.Costumes.ForEach(costume => {
                    totalCount += 1;
                    if (costume.IsValid()) {
                        validCount += 1;
                    }
                    else {
                        costume.Name += " [X]";
                    }
                });
            });
            Info($"init done, found {totalCount} costumes, {validCount} are valid");
        }

        private void clearLogBtn_Click(object sender, EventArgs e) {
            logBox.Text = "";
        }

        private class IdolSelector {
            public PictureBox AvatarBox { get; set; }
            public ComboBox IdolCombo { get; set; }
            public PictureBox CostumeBox { get; set; }
            public ComboBox CostumeCombo { get; set; }
        }

        private class IdolInfo {
            public string Name { get; set; }
            public string Id { get; set; }
            public List<CostumeInfo> Costumes { get; set; } = new List<CostumeInfo>();
        }

        private class CostumeInfo {
            public string Id { get; set; }
            public string Name { get; set; }
            public AssetInfo ChAsset { get; set; }
            public AssetInfo CbAsset { get; set; }

            public bool IsValid() {
                return ChAsset != null && CbAsset != null;
            }
        }
    }
}
