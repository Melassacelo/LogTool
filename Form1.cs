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

        private void btn_AddToData_Click(object sender, EventArgs e)
        {
            foreach (var path in filespath)
            {
                var lines = ReadJsonObjects(path);
                foreach (var Json in lines)
                {
                    string fixedJson = FixInvalidJson(Json);
                    var obj = JObject.Parse(fixedJson);

                    EnsureColumnsExist(obj); // Assicura che tutte le colonne esistano

                    string query = JsonToQuery(obj);

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            dataGridView1.DataSource = LoadTable("Logs");
            filespath.Clear();
            lsv_FilesPath.Clear();
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
                    // JSON completo
                    yield return sb.ToString().Trim();
                    sb.Clear();
                }
            }

            // in caso ci fosse qualcosa rimasto senza chiusura corretta
            if (sb.Length > 0)
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

        private void EnsureColumnsExist(JObject obj)
        {
            foreach (var prop in obj.Properties())
            {
                string columnName = prop.Name;

                // Verifica se la colonna esiste già
                using (var cmd = new SQLiteCommand($"PRAGMA table_info(Logs);", conn))
                {
                    var reader = cmd.ExecuteReader();
                    bool exists = false;
                    while (reader.Read())
                    {
                        if (reader["name"].ToString() == columnName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    reader.Close();

                    // Se non esiste, aggiungila
                    if (!exists)
                    {
                        using (var addCmd = new SQLiteCommand($"ALTER TABLE Logs ADD COLUMN [{columnName}] TEXT;", conn))
                        {
                            addCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
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
}
