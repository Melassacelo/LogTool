namespace LogTool
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_UploadFolder = new System.Windows.Forms.Button();
            this.btn_RemoveFolder = new System.Windows.Forms.Button();
            this.btn_AddToData = new System.Windows.Forms.Button();
            this.lsv_FilesPath = new System.Windows.Forms.ListView();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btn_SubmitQuery = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_UploadFolder
            // 
            this.btn_UploadFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_UploadFolder.Location = new System.Drawing.Point(12, 469);
            this.btn_UploadFolder.Name = "btn_UploadFolder";
            this.btn_UploadFolder.Size = new System.Drawing.Size(377, 23);
            this.btn_UploadFolder.TabIndex = 0;
            this.btn_UploadFolder.Text = "Upload";
            this.btn_UploadFolder.UseVisualStyleBackColor = true;
            this.btn_UploadFolder.Click += new System.EventHandler(this.btn_UploadFolder_Click);
            // 
            // btn_RemoveFolder
            // 
            this.btn_RemoveFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_RemoveFolder.Enabled = false;
            this.btn_RemoveFolder.Location = new System.Drawing.Point(12, 500);
            this.btn_RemoveFolder.Name = "btn_RemoveFolder";
            this.btn_RemoveFolder.Size = new System.Drawing.Size(377, 23);
            this.btn_RemoveFolder.TabIndex = 1;
            this.btn_RemoveFolder.Text = "Remove";
            this.btn_RemoveFolder.UseVisualStyleBackColor = true;
            this.btn_RemoveFolder.Click += new System.EventHandler(this.btn_RemoveFolder_Click);
            // 
            // btn_AddToData
            // 
            this.btn_AddToData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_AddToData.Enabled = false;
            this.btn_AddToData.Location = new System.Drawing.Point(12, 529);
            this.btn_AddToData.Name = "btn_AddToData";
            this.btn_AddToData.Size = new System.Drawing.Size(377, 38);
            this.btn_AddToData.TabIndex = 2;
            this.btn_AddToData.Text = "Confirm";
            this.btn_AddToData.UseVisualStyleBackColor = true;
            this.btn_AddToData.Click += new System.EventHandler(this.btn_AddToData_Click);
            // 
            // lsv_FilesPath
            // 
            this.lsv_FilesPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lsv_FilesPath.HideSelection = false;
            this.lsv_FilesPath.Location = new System.Drawing.Point(12, 12);
            this.lsv_FilesPath.MultiSelect = false;
            this.lsv_FilesPath.Name = "lsv_FilesPath";
            this.lsv_FilesPath.Size = new System.Drawing.Size(377, 451);
            this.lsv_FilesPath.TabIndex = 3;
            this.lsv_FilesPath.UseCompatibleStateImageBehavior = false;
            this.lsv_FilesPath.View = System.Windows.Forms.View.List;
            this.lsv_FilesPath.SelectedIndexChanged += new System.EventHandler(this.lsv_FilesPath_SelectedIndexChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(396, 13);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(803, 507);
            this.dataGridView1.TabIndex = 4;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(396, 537);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(662, 20);
            this.textBox1.TabIndex = 5;
            // 
            // btn_SubmitQuery
            // 
            this.btn_SubmitQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_SubmitQuery.Location = new System.Drawing.Point(1064, 529);
            this.btn_SubmitQuery.Name = "btn_SubmitQuery";
            this.btn_SubmitQuery.Size = new System.Drawing.Size(135, 38);
            this.btn_SubmitQuery.TabIndex = 6;
            this.btn_SubmitQuery.Text = "Search";
            this.btn_SubmitQuery.UseVisualStyleBackColor = true;
            this.btn_SubmitQuery.Click += new System.EventHandler(this.btn_SubmitQuery_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1211, 579);
            this.Controls.Add(this.btn_SubmitQuery);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.lsv_FilesPath);
            this.Controls.Add(this.btn_AddToData);
            this.Controls.Add(this.btn_RemoveFolder);
            this.Controls.Add(this.btn_UploadFolder);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_UploadFolder;
        private System.Windows.Forms.Button btn_RemoveFolder;
        private System.Windows.Forms.Button btn_AddToData;
        private System.Windows.Forms.ListView lsv_FilesPath;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btn_SubmitQuery;
    }
}

