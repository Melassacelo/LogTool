using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace LogTool
{
    public partial class Form1 : Form
    {
        public List<string> filespath = new List<string>();
        private string dbPath = "logs.db";
        private SQLiteConnection conn;
        public Form1()
        {
            InitializeComponent();
            InitDatabase();
        }


        private void InitDatabase()
        {
            bool createNew = !File.Exists(dbPath);

            conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            conn.Open();

            if (createNew)
            {
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                CREATE TABLE Logs (
                    [time] TEXT
                );";
                    cmd.ExecuteNonQuery();
                }
            }

            dataGridView1.DataSource = LoadTable("Logs");
        }

        private void btn_UploadFolder_Click(object sender, EventArgs e)
        {
            try
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        foreach (var path in Directory.GetFiles(fbd.SelectedPath))
                        {
                            filespath.Add(path);
                            lsv_FilesPath.Items.Add(path);
                        }
                    }
                }
                btn_AddToData.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_RemoveFolder_Click(object sender, EventArgs e)
        {
            filespath.Remove(lsv_FilesPath.SelectedItems[0].Text);
            lsv_FilesPath.Items.Remove(lsv_FilesPath.SelectedItems[0]);
            if (lsv_FilesPath.Items.Count > 0)
            {
                btn_AddToData.Enabled = true;
            }
            else
            {
                btn_AddToData.Enabled = false;
            }
        }

        private void lsv_FilesPath_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsv_FilesPath.SelectedItems.Count > 0) 
            {
                btn_RemoveFolder.Enabled = true;
            }
            else
            {
                btn_RemoveFolder.Enabled = false;
            }

        }

        private async void btn_AddToData_Click(object sender, EventArgs e)
        {
            btn_AddToData.Enabled = false;

            var progressDialog = new ProgressDialog();
            progressDialog.Show(this);

            try
            {
                await Task.Run(() =>
                {
                    int totalFiles = filespath.Count;
                    int processedFiles = 0;

                    foreach (var path in filespath)
                    {
                        var newColumns = new HashSet<string>(); // colonne da aggiungere al file
                        var jsonObjects = ReadJsonObjects(path).Select(FixInvalidJson).Select(JObject.Parse).ToList();
                        int totalLines = jsonObjects.Count;
                        int processedLines = 0;

                        // 1️⃣ Determina tutte le colonne nuove
                        foreach (var obj in jsonObjects)
                        {
                            foreach (var prop in obj.Properties())
                            {
                                string col = prop.Name;
                                if (!ColumnExists(col))
                                    newColumns.Add(col);
                            }
                        }

                        // 2️⃣ Aggiungi le nuove colonne tutte in batch
                        foreach (var col in newColumns)
                        {
                            using (var cmd = new SQLiteCommand($"ALTER TABLE Logs ADD COLUMN [{col}] TEXT;", conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 3️⃣ Inserisci tutti i record in una transazione
                        using (var transaction = conn.BeginTransaction())
                        {
                            foreach (var obj in jsonObjects)
                            {
                                string query = JsonToQuery(obj);
                                using (var cmd = new SQLiteCommand(query, conn, transaction))
                                {
                                    cmd.ExecuteNonQuery();
                                }

                                processedLines++;

                                // aggiorna la progress bar ogni 100 righe
                                if (processedLines % 100 == 0 || processedLines == totalLines)
                                {
                                    int percent = (int)((processedLines / (double)totalLines) * 100);
                                    this.Invoke(new Action(() =>
                                        progressDialog.UpdateProgress(percent,
                                            $"File {processedFiles + 1}/{totalFiles} - Riga {processedLines}/{totalLines}")));
                                }
                            }

                            transaction.Commit();
                        }

                        processedFiles++;
                    }
                });

                // aggiorna DataGridView
                dataGridView1.DataSource = LoadTable("Logs");
            }
            finally
            {
                progressDialog.Close();
                filespath.Clear();
                lsv_FilesPath.Clear();
                btn_AddToData.Enabled = true;
            }
        }

        // controlla se una colonna esiste già nella tabella
        private bool ColumnExists(string columnName)
        {
            using (var cmd = new SQLiteCommand("PRAGMA table_info(Logs);", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["name"].ToString() == columnName)
                            return true;
                    }
                }
            }
            return false;
        }


        private string JsonToQuery(JObject obj)
        {
            var columns = obj.Properties().Select(p => $"[{p.Name}]").ToList();
            var values = obj.Properties().Select(p => p.Value.ToString().Replace("'", "''")).ToList();

            string query = $"INSERT INTO Logs ({string.Join(",", columns)}) VALUES ('{string.Join("','", values)}');";
            return query;
        }

        private IEnumerable<string> ReadJsonObjects(string path)
        {
            var sb = new StringBuilder();
            int braceCount = 0;

            foreach (var line in File.ReadLines(path))
            {
                foreach (char c in line)
                {
                    if (c == '{') braceCount++;
                    if (c == '}') braceCount--;
                }

                sb.AppendLine(line);

                if (braceCount == 0 && sb.Length > 0)
                {
                    yield return sb.ToString().Trim();
                    sb.Clear();
                }
            }

            // se c’è troppo testo non chiuso → scarto invece di freezare
            if (sb.Length > 0 && braceCount == 0)
            {
                yield return sb.ToString().Trim();
            }
        }

        private string FixInvalidJson(string line)
        {
            line = line.Trim();

            if (line.Contains("\"message\":"))
            {
                int idx = line.IndexOf("\"message\":");
                int start = idx + "\"message\":".Length;

                // Prendi tutto ciò che segue fino all'ultima graffa chiusa
                int end = line.LastIndexOf('}');
                if (end < start) end = line.Length;

                string before = line.Substring(0, start);
                string message = line.Substring(start, end - start).Trim();

                // escape eventuali doppi apici interni
                message = message.Replace("\\", "\\\\").Replace("\"", "\\\"");

                // la graffa finale viene inclusa correttamente
                
                string after = "}"; // include la }

                // aggiunge virgolette attorno al valore del messaggio
                line = before + "\"" + message + "\"" + after;
            }
            // -------------------------
            // 2️⃣ Fix per "query-string"
            // -------------------------
            if (line.Contains("\"query-string\":"))
            {
                int idx = line.IndexOf("\"query-string\":");
                int start = idx + "\"query-string\":".Length;

                // trova la prima virgola dopo query-string oppure la fine della riga
                int commaIdx = line.IndexOf(',', start);
                int end = (commaIdx > 0) ? commaIdx : line.Length;

                string before = line.Substring(0, start);
                string qsValue = line.Substring(start, end - start).Trim();

                // escape eventuali doppi apici interni
                qsValue = qsValue.Replace("\\", "\\\\").Replace("\"", "\\\"");

                string after = line.Substring(end);

                // aggiunge virgolette intorno al valore di query-string
                line = before + "\"" + qsValue + "\"" + after;
            }

            return line;
        }


        private DataTable LoadTable(string tableName)
        {
            var dt = new DataTable();

            using (var cmd = new SQLiteCommand($"SELECT * FROM {tableName} ORDER BY [time] LIMIT 100", conn))
            using (var adapter = new SQLiteDataAdapter(cmd))
            {
                adapter.Fill(dt);
            }

            return dt;
        }
        private void btn_SubmitQuery_Click(object sender, EventArgs e)
        {
            string userQuery = textBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(userQuery))
            {
                MessageBox.Show("Inserisci una query.");
                return;
            }

            try
            {
                var dt = new DataTable();

                using (var cmd = new SQLiteCommand(userQuery, conn))
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                // Aggiorna il DataGridView con i risultati
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore nella query: " + ex.Message);
            }
        }
    }

    public class ProgressDialog : Form
    {
        private ProgressBar progressBar;
        private Label lblStatus;

        public ProgressDialog()
        {
            this.Text = "Elaborazione in corso...";
            this.Size = new Size(400, 100);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ControlBox = false; // niente X
            this.TopMost = true;

            progressBar = new ProgressBar()
            {
                Dock = DockStyle.Bottom,
                Style = ProgressBarStyle.Continuous,
                Minimum = 0,
                Maximum = 100,
                Height = 20
            };

            lblStatus = new Label()
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Inizializzazione..."
            };

            this.Controls.Add(lblStatus);
            this.Controls.Add(progressBar);
        }

        public void UpdateProgress(int percent, string status = "")
        {
            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;

            progressBar.Value = percent;
            if (!string.IsNullOrEmpty(status))
                lblStatus.Text = status;

            System.Windows.Forms.Application.DoEvents(); // forza refresh UI
        }
    }
}
