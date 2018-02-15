﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenMLTD.AllStarsTheater;
using OpenMLTD.AllStarsTheater.Database;
using OpenMLTD.AllStarsTheater.Mltd.Entities;

namespace MltdInfoViewer {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();

            AddListViewHeaders();

            RegisterEventHandlers();
        }

        private void AddListViewHeaders() {
            lvwCards.Columns.Add("Idol");
            lvwCards.Columns.Add("Color");
            lvwCards.Columns.Add("Rarity");
            lvwCards.Columns.Add("Serial");

            lvwCostumes.Columns.Add("Idol");
            lvwCostumes.Columns.Add("Main");
            lvwCostumes.Columns.Add("Variation");
            lvwCostumes.Columns.Add("Category");
            lvwCostumes.Columns.Add("Number");
            lvwCostumes.Columns.Add("Post");
        }

        private void RegisterEventHandlers() {
            btnSelectCardsDatabase.Click += BtnSelectCardsDatabase_Click;
            btnSelectCostumesDatabase.Click += BtnSelectCostumesDatabase_Click;
        }

        private void BtnSelectCostumesDatabase_Click(object sender, EventArgs e) {
            ofd.FileName = string.Empty;
            ofd.CheckFileExists = true;
            ofd.ValidateNames = true;
            ofd.ShowReadOnly = false;
            ofd.ReadOnlyChecked = false;
            ofd.Filter = "Costume Database (chara_costume_release.json)|chara_costume_release.json";

            var r = ofd.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            var filePath = ofd.FileName;
            var text = File.ReadAllText(filePath);

            var obj = JsonConvert.DeserializeObject(text);

            var costumes = new List<(int IdolID, string Idol, string Main, string Variation, string Category, int Number, string DirPost)>();

            int iid = 0;

            try {
                var root = (JObject)obj;
                var costumeArray = (JArray)root["costumes"];

                var idols = MltdConstants.Idols.ToArray();

                foreach (var c in costumeArray) {
                    var category = c["category"].Value<string>();
                    var dirPost = c["dir_post"].Value<string>();
                    var number = Convert.ToInt32(c["number"].Value<string>());

                    var charaBase = (JObject)c["characters"];

                    // Means: "CharacterId","BodyVariation","HeadModelType","HeadCategory","HeadNumber"

                    var charas = (JArray)charaBase["characters"];

                    foreach (var chara in charas) {
                        var values = (JArray)chara["values"];
                        var charaResID = values[0].Value<string>();
                        var variation = values[1].Value<string>();

                        var charaID = Convert.ToInt32(charaResID.Substring(0, 3));
                        var charaAbbr = charaResID.Substring(3, 3);

                        var idolIndex = Array.FindIndex(idols, idol => idol.IdolID == charaID);
                        iid = charaID;
                        var idolName = idols[idolIndex].Name;

                        var entry = (charaID, idolName, charaResID, variation, category, number, dirPost);

                        costumes.Add(entry);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            costumes.Sort((v1, v2) => {
                var cr = v1.IdolID.CompareTo(v2.IdolID);

                if (cr != 0) {
                    return cr;
                }

                cr = string.CompareOrdinal(v1.Main, v2.Main);

                if (cr != 0) {
                    return cr;
                }

                cr = string.CompareOrdinal(v1.Category, v2.Category);

                if (cr != 0) {
                    return cr;
                }

                return v1.Number.CompareTo(v2.Number);
            });

            txtCostumesDatabasePath.Text = filePath;
            _costumeList.AddRange(costumes.Select(c => (c.Idol, c.Main, c.Variation, c.Category, c.Number, c.DirPost)));

            lvwCostumes.Items.Clear();

            foreach (var costume in costumes) {
                var lvi = lvwCostumes.Items.Add(costume.Idol);
                lvi.SubItems.Add(costume.Main);
                lvi.SubItems.Add(costume.Variation);
                lvi.SubItems.Add(costume.Category);
                lvi.SubItems.Add(costume.Number.ToString());
                lvi.SubItems.Add(costume.DirPost);
            }

            lvwCostumes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void BtnSelectCardsDatabase_Click(object sender, EventArgs e) {
            ofd.FileName = string.Empty;
            ofd.CheckFileExists = true;
            ofd.ValidateNames = true;
            ofd.ShowReadOnly = false;
            ofd.ReadOnlyChecked = false;
            ofd.Filter = "Manifest Database (*.data)|*.data";

            var r = ofd.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            var filePath = ofd.FileName;
            var bytes = File.ReadAllBytes(filePath);
            AssetInfoList assetInfoList;

            try {
                assetInfoList = AssetInfoList.Parse(bytes, MltdConstants.Utf8WithoutBom);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtCardsDatabasePath.Text = filePath;

            lvwCards.Items.Clear();

            var cardNameRegex = new Regex(@"^card_list_(?<idolIndex>\d{3})(?<idolAbbr>[a-z]{3})(?<ser>\d{4})\.acb\.unity3d$", RegexOptions.CultureInvariant);
            var cardList = _cardList;
            var idols = MltdConstants.Idols;

            cardList.Clear();

            foreach (var assetInfo in assetInfoList.Assets) {
                var match = cardNameRegex.Match(assetInfo.ResourceName);

                if (!match.Success) {
                    continue;
                }

                var idolID = Convert.ToInt32(match.Groups["idolIndex"].Value);
                var detectedIdolAbbr = match.Groups["idolAbbr"].Value;

                if (idolID < 1 || idolID > idols.Count) {
                    Debug.Print("Warning: idol index out of range: {0} with abbr {1}", idolID, detectedIdolAbbr);
                    continue;
                }

                var idol = idols[idolID - 1];

                if (idol.Abbreviation != detectedIdolAbbr) {
                    Debug.Print("Warning: detected abbr {0} does not match standard abbr {1}.", detectedIdolAbbr, idol.Abbreviation);
                    continue;
                }

                var serial = match.Groups["ser"].Value;
                var serialInt = Convert.ToInt32(serial);
                var rarity = (Rarity)(serialInt % 10);
                serial = TheaterHelper.GetIdolSerialAbbreviation(idolID) + serial;
                var entry = (idol.Name, idol.Color, rarity, serial);

                cardList.Add(entry);
            }

            foreach (var entry in cardList) {
                var lvi = lvwCards.Items.Add(entry.Idol);
                lvi.SubItems.Add(entry.Color.ToString());
                lvi.SubItems.Add(entry.Rarity.ToString());
                lvi.SubItems.Add(entry.Serial);
            }

            lvwCards.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private readonly List<(string Idol, ColorType Color, Rarity Rarity, string Serial)> _cardList = new List<(string Idol, ColorType Color, Rarity Rarity, string Serial)>();
        private readonly List<(string Idol, string Main, string Variation, string Category, int Number, string DirPost)> _costumeList = new List<(string Idol, string Main, string Variation, string Category, int Number, string DirPost)>();

    }
}
