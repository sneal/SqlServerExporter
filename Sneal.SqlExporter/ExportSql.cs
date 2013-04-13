using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sneal.SqlExporter.Core;

namespace Sneal.SqlExporter
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : Form
    {
        private Button btnExportButton;
        private Button btnSelectExportDir;
        private CheckBox chkConstraints;
        private CheckBox chkDataSql;
        private CheckBox chkDataXml;
        private CheckBox chkIndex;
        private CheckBox chkIntegratedAuthentication;
        private CheckBox chkSchema;
        private IContainer components;
        private ProgressBar exportProgressBar;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label lblExportDir;
        private Label lblHelp;
        private Label lblSproc;
        private Label lblTable;
        private Label lblView;
        private ListBox lstSproc;
        private ListBox lstTable;
        private ListBox lstView;
        private ComboBox selExportDatabaseList;
        private ToolTip toolTip1;
        private TextBox txtExportRootDir;
        private TextBox txtPassword;
        private TextBox txtServer;
        private TextBox txtUserName;

        private IExportSession exportSession;
        private RadioButton rdoSingleFile;
        private RadioButton rdoMultipleFiles;
        private ExportSessionFactory exportFactory;

        public Form1()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
        }

        /// <summary>
        /// Gets the name of the selected database.
        /// </summary>
        /// <value>The name of the selected database.</value>
        private string SelectedDatabaseName
        {
            get { return selExportDatabaseList.SelectedItem.ToString(); }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        [STAThread]
        private static void Main()
        {
            Application.Run(new Form1());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            selExportDatabaseList.Focus();
            LoadUserPreferences();
        }

        private void LoadUserPreferences()
        {
            UserPrefsRepository prefsRepository = new UserPrefsRepository();
            UserPrefs prefs = prefsRepository.LoadUserPrefs();

            if (!string.IsNullOrEmpty(prefs.ExportDirectory))
                txtExportRootDir.Text = prefs.ExportDirectory;
            else
                txtExportRootDir.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!string.IsNullOrEmpty(prefs.Server))
                txtServer.Text = prefs.Server;
            else
                txtServer.Text = Environment.MachineName;
        }

        /// <summary>
        /// Handles the Click event of the ExportButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExportButton_Click(object sender, EventArgs e)
        {
            // need a database selected
            if (selExportDatabaseList.SelectedIndex < 0)
            {
                MessageBox.Show("You need to first select a database before exporting");
                return;
            }

            SaveUserPreferences();
            ExportScripts();
        }

        private void ExportScripts()
        {
            ExportParams exportParams = new ExportParams();
            exportParams.UseMultipleFiles = false;  // for now this always must be true

            exportParams.ScriptDataAsSql = chkDataSql.Checked;
            exportParams.ScriptDataAsXml = chkDataXml.Checked;
            exportParams.ScriptTableSchema = chkSchema.Checked;
            exportParams.ScriptTableConstraints = chkConstraints.Checked;
            exportParams.ScriptTableIndexes = chkIndex.Checked;

            if (exportParams.ScriptDataAsSql || exportParams.ScriptDataAsXml)
            {
                foreach (string table in lstTable.SelectedItems)
                    exportParams.TablesToScriptData.Add(table);
            }

            foreach (string sproc in lstSproc.SelectedItems)
                exportParams.SprocsToScript.Add(sproc);

            foreach (string table in lstTable.SelectedItems)
                exportParams.TablesToScript.Add(table);

            foreach (string view in lstView.SelectedItems)
                exportParams.ViewsToScript.Add(view);

            exportParams.UseMultipleFiles = rdoMultipleFiles.Checked;
            exportParams.DatabaseName = exportSession.DatabaseName;
            exportParams.ExportDirectory = txtExportRootDir.Text;

            try
            {
                exportSession.Export(txtExportRootDir.Text, exportParams);
            }
            catch (SqlExporterException ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveUserPreferences()
        {
            UserPrefs prefs = new UserPrefs();
            prefs.Server = txtServer.Text;
            prefs.ExportDirectory = txtExportRootDir.Text;

            UserPrefsRepository prefsRepository = new UserPrefsRepository();
            prefsRepository.SaveUserPrefs(prefs);
        }

        private static void ShowError(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Fills the table list with table names from
        /// the selected database.
        /// </summary>
        private void FillTableList()
        {
            foreach (string table in exportSession.GetUserTables())
                lstTable.Items.Add(table);            
        }

        /// <summary>
        /// Fills the sproc list with sproc names
        /// from the selected database.
        /// </summary>
        private void FillSprocList()
        {
            foreach (string sproc in exportSession.GetUserSprocs())
                lstSproc.Items.Add(sproc);
        }

        /// <summary>
        /// Fills the view list.
        /// </summary>
        private void FillViewList()
        {
            foreach (string view in exportSession.GetUserViews())
                lstView.Items.Add(view);
        }

        /// <summary>
        /// Handles the Enter event of the selExportDatabaseList control.
        /// </summary>
        private void selExportDatabaseList_Enter(object sender, EventArgs e)
        {
            selExportDatabaseList.Items.Clear();

            ConnectionSettings settings = new ConnectionSettings();
            settings.ServerName = txtServer.Text;
            settings.Password = txtPassword.Text;
            settings.UserName = txtUserName.Text;
            settings.UseIntegratedAuthentication = chkIntegratedAuthentication.Checked;

            try
            {
                exportFactory = new ExportSessionFactory(settings);
            }
            catch (SqlExporterConnectionException ex)
            {
                ShowError(ex.Message);
                return;
            }

            foreach (string db in exportFactory.GetDatabaseNames())
            {
                selExportDatabaseList.Items.Add(db);
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the selExportDatabaseList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void selExportDatabaseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstView.Items.Clear();
            lstSproc.Items.Clear();
            lstTable.Items.Clear();

            try
            {
                if (exportSession != null)
                    exportSession.Dispose();

                exportSession = exportFactory.CreateExportSession(SelectedDatabaseName);
                exportSession.ProgressEvent += ExportSession_ProgressEvent;
            }
            catch (SqlExporterException ex)
            {
                ShowError(ex.Message);
                return;
            }

            FillSprocList();
            FillTableList();
            FillViewList();
        }

        private void ExportSession_ProgressEvent(object sender, ProgressEventArgs e)
        {
            exportProgressBar.Value = e.PercentDone;

            if (e.PercentDone >= 100)
            {
                MessageBox.Show("Scripting is complete.", "Done", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSelectExportDir control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSelectExportDir_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.SelectedPath = txtExportRootDir.Text;

            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtExportRootDir.Text = folderBrowserDialog.SelectedPath;
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.selExportDatabaseList = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnExportButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.exportProgressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSelectExportDir = new System.Windows.Forms.Button();
            this.lblExportDir = new System.Windows.Forms.Label();
            this.txtExportRootDir = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.chkIntegratedAuthentication = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstTable = new System.Windows.Forms.ListBox();
            this.lstSproc = new System.Windows.Forms.ListBox();
            this.lblTable = new System.Windows.Forms.Label();
            this.lblSproc = new System.Windows.Forms.Label();
            this.chkDataSql = new System.Windows.Forms.CheckBox();
            this.chkDataXml = new System.Windows.Forms.CheckBox();
            this.chkSchema = new System.Windows.Forms.CheckBox();
            this.chkConstraints = new System.Windows.Forms.CheckBox();
            this.lblHelp = new System.Windows.Forms.Label();
            this.lblView = new System.Windows.Forms.Label();
            this.lstView = new System.Windows.Forms.ListBox();
            this.chkIndex = new System.Windows.Forms.CheckBox();
            this.rdoSingleFile = new System.Windows.Forms.RadioButton();
            this.rdoMultipleFiles = new System.Windows.Forms.RadioButton();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // selExportDatabaseList
            // 
            this.selExportDatabaseList.Location = new System.Drawing.Point(8, 20);
            this.selExportDatabaseList.Name = "selExportDatabaseList";
            this.selExportDatabaseList.Size = new System.Drawing.Size(212, 21);
            this.selExportDatabaseList.TabIndex = 3;
            this.selExportDatabaseList.SelectedIndexChanged += new System.EventHandler(this.selExportDatabaseList_SelectedIndexChanged);
            this.selExportDatabaseList.Enter += new System.EventHandler(this.selExportDatabaseList_Enter);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "UserName";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(68, 56);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(156, 20);
            this.txtUserName.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 16);
            this.label5.TabIndex = 12;
            this.label5.Text = "Password";
            // 
            // btnExportButton
            // 
            this.btnExportButton.Location = new System.Drawing.Point(68, 108);
            this.btnExportButton.Name = "btnExportButton";
            this.btnExportButton.Size = new System.Drawing.Size(80, 24);
            this.btnExportButton.TabIndex = 0;
            this.btnExportButton.Text = "Export";
            this.btnExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "Server";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(68, 12);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(156, 20);
            this.txtServer.TabIndex = 0;
            this.txtServer.Text = ".";
            // 
            // exportProgressBar
            // 
            this.exportProgressBar.Location = new System.Drawing.Point(4, 72);
            this.exportProgressBar.Name = "exportProgressBar";
            this.exportProgressBar.Size = new System.Drawing.Size(216, 23);
            this.exportProgressBar.TabIndex = 18;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.selExportDatabaseList);
            this.groupBox2.Location = new System.Drawing.Point(4, 124);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(232, 64);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Databases";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnSelectExportDir);
            this.groupBox3.Controls.Add(this.lblExportDir);
            this.groupBox3.Controls.Add(this.txtExportRootDir);
            this.groupBox3.Controls.Add(this.exportProgressBar);
            this.groupBox3.Controls.Add(this.btnExportButton);
            this.groupBox3.Location = new System.Drawing.Point(4, 196);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(232, 156);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Actions";
            // 
            // btnSelectExportDir
            // 
            this.btnSelectExportDir.Location = new System.Drawing.Point(200, 40);
            this.btnSelectExportDir.Name = "btnSelectExportDir";
            this.btnSelectExportDir.Size = new System.Drawing.Size(24, 20);
            this.btnSelectExportDir.TabIndex = 21;
            this.btnSelectExportDir.Text = "...";
            this.btnSelectExportDir.Click += new System.EventHandler(this.btnSelectExportDir_Click);
            // 
            // lblExportDir
            // 
            this.lblExportDir.Location = new System.Drawing.Point(8, 20);
            this.lblExportDir.Name = "lblExportDir";
            this.lblExportDir.Size = new System.Drawing.Size(152, 16);
            this.lblExportDir.TabIndex = 20;
            this.lblExportDir.Text = "Export root directory";
            // 
            // txtExportRootDir
            // 
            this.txtExportRootDir.Location = new System.Drawing.Point(8, 40);
            this.txtExportRootDir.Name = "txtExportRootDir";
            this.txtExportRootDir.Size = new System.Drawing.Size(192, 20);
            this.txtExportRootDir.TabIndex = 19;
            this.txtExportRootDir.Text = "c:\\";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(68, 80);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(156, 20);
            this.txtPassword.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtPassword, "Enter Password and TAB out to fill DataBase listbox.");
            // 
            // chkIntegratedAuthentication
            // 
            this.chkIntegratedAuthentication.Checked = true;
            this.chkIntegratedAuthentication.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIntegratedAuthentication.Location = new System.Drawing.Point(68, 36);
            this.chkIntegratedAuthentication.Name = "chkIntegratedAuthentication";
            this.chkIntegratedAuthentication.Size = new System.Drawing.Size(156, 16);
            this.chkIntegratedAuthentication.TabIndex = 23;
            this.chkIntegratedAuthentication.Text = "Integrated Authentication";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(4, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(232, 120);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Login";
            // 
            // lstTable
            // 
            this.lstTable.HorizontalScrollbar = true;
            this.lstTable.Location = new System.Drawing.Point(244, 24);
            this.lstTable.Name = "lstTable";
            this.lstTable.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstTable.Size = new System.Drawing.Size(140, 199);
            this.lstTable.TabIndex = 25;
            // 
            // lstSproc
            // 
            this.lstSproc.HorizontalScrollbar = true;
            this.lstSproc.Location = new System.Drawing.Point(392, 24);
            this.lstSproc.Name = "lstSproc";
            this.lstSproc.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSproc.Size = new System.Drawing.Size(140, 199);
            this.lstSproc.TabIndex = 26;
            // 
            // lblTable
            // 
            this.lblTable.Location = new System.Drawing.Point(248, 4);
            this.lblTable.Name = "lblTable";
            this.lblTable.Size = new System.Drawing.Size(100, 16);
            this.lblTable.TabIndex = 28;
            this.lblTable.Text = "Tables";
            // 
            // lblSproc
            // 
            this.lblSproc.Location = new System.Drawing.Point(392, 4);
            this.lblSproc.Name = "lblSproc";
            this.lblSproc.Size = new System.Drawing.Size(100, 16);
            this.lblSproc.TabIndex = 29;
            this.lblSproc.Text = "Sprocs";
            // 
            // chkDataSql
            // 
            this.chkDataSql.Location = new System.Drawing.Point(244, 232);
            this.chkDataSql.Name = "chkDataSql";
            this.chkDataSql.Size = new System.Drawing.Size(124, 16);
            this.chkDataSql.TabIndex = 30;
            this.chkDataSql.Text = "Export Data as SQL";
            // 
            // chkDataXml
            // 
            this.chkDataXml.Enabled = false;
            this.chkDataXml.Location = new System.Drawing.Point(244, 256);
            this.chkDataXml.Name = "chkDataXml";
            this.chkDataXml.Size = new System.Drawing.Size(124, 16);
            this.chkDataXml.TabIndex = 31;
            this.chkDataXml.Text = "Export Data as XML";
            // 
            // chkSchema
            // 
            this.chkSchema.Location = new System.Drawing.Point(244, 280);
            this.chkSchema.Name = "chkSchema";
            this.chkSchema.Size = new System.Drawing.Size(124, 16);
            this.chkSchema.TabIndex = 32;
            this.chkSchema.Text = "Export Schema";
            // 
            // chkConstraints
            // 
            this.chkConstraints.Location = new System.Drawing.Point(244, 300);
            this.chkConstraints.Name = "chkConstraints";
            this.chkConstraints.Size = new System.Drawing.Size(136, 32);
            this.chkConstraints.TabIndex = 33;
            this.chkConstraints.Text = "Export Defaults, Constraints, and FKs";
            // 
            // lblHelp
            // 
            this.lblHelp.Location = new System.Drawing.Point(548, 236);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(132, 116);
            this.lblHelp.TabIndex = 34;
            this.lblHelp.Text = "Note: Each table option you select will create a separate SQL or XML file to be i" +
    "ntegrated into source control.  All indexes are exported to a single SQL script " +
    "file.";
            // 
            // lblView
            // 
            this.lblView.Location = new System.Drawing.Point(536, 4);
            this.lblView.Name = "lblView";
            this.lblView.Size = new System.Drawing.Size(100, 16);
            this.lblView.TabIndex = 35;
            this.lblView.Text = "Views";
            // 
            // lstView
            // 
            this.lstView.HorizontalScrollbar = true;
            this.lstView.Location = new System.Drawing.Point(540, 24);
            this.lstView.Name = "lstView";
            this.lstView.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstView.Size = new System.Drawing.Size(140, 199);
            this.lstView.TabIndex = 36;
            // 
            // chkIndex
            // 
            this.chkIndex.Location = new System.Drawing.Point(244, 332);
            this.chkIndex.Name = "chkIndex";
            this.chkIndex.Size = new System.Drawing.Size(124, 20);
            this.chkIndex.TabIndex = 37;
            this.chkIndex.Text = "Export Indexes";
            // 
            // rdoSingleFile
            // 
            this.rdoSingleFile.AutoSize = true;
            this.rdoSingleFile.Location = new System.Drawing.Point(392, 236);
            this.rdoSingleFile.Name = "rdoSingleFile";
            this.rdoSingleFile.Size = new System.Drawing.Size(115, 17);
            this.rdoSingleFile.TabIndex = 38;
            this.rdoSingleFile.Text = "Export as single file";
            this.rdoSingleFile.UseVisualStyleBackColor = true;
            // 
            // rdoMultipleFiles
            // 
            this.rdoMultipleFiles.AutoSize = true;
            this.rdoMultipleFiles.Checked = true;
            this.rdoMultipleFiles.Location = new System.Drawing.Point(392, 260);
            this.rdoMultipleFiles.Name = "rdoMultipleFiles";
            this.rdoMultipleFiles.Size = new System.Drawing.Size(156, 17);
            this.rdoMultipleFiles.TabIndex = 39;
            this.rdoMultipleFiles.TabStop = true;
            this.rdoMultipleFiles.Text = "Export as one file per object";
            this.rdoMultipleFiles.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(700, 362);
            this.Controls.Add(this.rdoMultipleFiles);
            this.Controls.Add(this.rdoSingleFile);
            this.Controls.Add(this.chkIndex);
            this.Controls.Add(this.lstView);
            this.Controls.Add(this.lblView);
            this.Controls.Add(this.lblHelp);
            this.Controls.Add(this.chkConstraints);
            this.Controls.Add(this.chkSchema);
            this.Controls.Add(this.chkDataXml);
            this.Controls.Add(this.chkDataSql);
            this.Controls.Add(this.lblSproc);
            this.Controls.Add(this.lblTable);
            this.Controls.Add(this.lstSproc);
            this.Controls.Add(this.lstTable);
            this.Controls.Add(this.chkIntegratedAuthentication);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "DataBase Export Utility";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}