/*
 * Sedna .NET API - For use with the Sedna Native XML Server (http://modis.ispras.ru/sedna/index.htm)
 * 
 * C# API Developed By John Wood (http://www.servicestuff.com)
 * 
 * Copyright (C) 2008 MindTouch, Inc.
 * www.mindtouch.com  oss@mindtouch.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Data.Sedna;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Sedna {
    public class MainForm : Form {

        //--- Class Fields ---
        private static readonly string[] _commands = new string[] {
            "UPDATE delete ", 
            "UPDATE delete_undeep ", 
            "UPDATE rename ", 
            "UPDATE insert ", 
            "CREATE DOCUMENT ", 
            "DROP DOCUMENT ", 
            "RETRIEVE METADATA FOR DOCUMENTS", 
            "RETRIEVE DESCRIPTIVE SCHEMA FOR DOCUMENT \"\"", 
            "CREATE COLLECTION ", 
            "DROP COLLECTION ", 
            "CREATE INDEX title ON nodes_path BY key_path AS type", 
            "DROP INDEX "
        };

        //--- Class Methods ---

        [STAThread]
        private static void Main() {
            Application.Run(new MainForm());
        }

        //--- Fields ---
        private TextBox QueryTextBox;
        private TextBox ResultTextBox;
        private Button BulkLoadButton;
        private OpenFileDialog OpenXmlFileDlg;
        private Label label3;
        private Button RunQueryButton;
        private Label label5;
        private Button CommitTransactionButton;
        private Button ExitButton;
        private Label label1;
        private ComboBox CommandsDropDown;
        private CheckBox AlwaysCommitCheckbox;
        private Button RollbackTransaction;
        private System.ComponentModel.Container components;
        private RadioButton sednaCheck;
        private RadioButton raptorCheck;
        private RadioButton sednaRaptorCheck;
        private SednaSession _session;

        //--- Constructors ---
        public MainForm() {
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        //--- Methods ---

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if(disposing) {
                if(components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void ReportError(string title, Exception ex) {
            string message = title + Environment.NewLine + ex.Message + " (" + ex.GetType().Name + ")";
            MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ResultTextBox.Text = "ERROR: " + message;
        }

        private void Login() {
            Login login = new Login();
            if(login.ShowDialog(this) == DialogResult.OK) {
                _session = login.GetSession();
            } else {
                Application.Exit();
            }
        }

        private string ExecuteReadAll(string query) {
            StringBuilder buffer = new StringBuilder();
            foreach(string result in _session.Execute(query)) {
                buffer.AppendLine(result.Replace("\n", Environment.NewLine));
            }
            return buffer.ToString();
        }

        private void InitializeComponent() {
            this.QueryTextBox = new System.Windows.Forms.TextBox();
            this.ResultTextBox = new System.Windows.Forms.TextBox();
            this.BulkLoadButton = new System.Windows.Forms.Button();
            this.OpenXmlFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.RunQueryButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.ExitButton = new System.Windows.Forms.Button();
            this.CommitTransactionButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.CommandsDropDown = new System.Windows.Forms.ComboBox();
            this.AlwaysCommitCheckbox = new System.Windows.Forms.CheckBox();
            this.RollbackTransaction = new System.Windows.Forms.Button();
            this.sednaCheck = new System.Windows.Forms.RadioButton();
            this.raptorCheck = new System.Windows.Forms.RadioButton();
            this.sednaRaptorCheck = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // QueryTextBox
            // 
            this.QueryTextBox.AcceptsReturn = true;
            this.QueryTextBox.AcceptsTab = true;
            this.QueryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.QueryTextBox.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QueryTextBox.Location = new System.Drawing.Point(8, 27);
            this.QueryTextBox.Multiline = true;
            this.QueryTextBox.Name = "QueryTextBox";
            this.QueryTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.QueryTextBox.Size = new System.Drawing.Size(580, 127);
            this.QueryTextBox.TabIndex = 1;
            this.QueryTextBox.WordWrap = false;
            // 
            // ResultTextBox
            // 
            this.ResultTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultTextBox.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResultTextBox.Location = new System.Drawing.Point(8, 215);
            this.ResultTextBox.Multiline = true;
            this.ResultTextBox.Name = "ResultTextBox";
            this.ResultTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ResultTextBox.Size = new System.Drawing.Size(580, 179);
            this.ResultTextBox.TabIndex = 1;
            this.ResultTextBox.WordWrap = false;
            // 
            // BulkLoadButton
            // 
            this.BulkLoadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BulkLoadButton.Location = new System.Drawing.Point(13, 411);
            this.BulkLoadButton.Name = "BulkLoadButton";
            this.BulkLoadButton.Size = new System.Drawing.Size(134, 29);
            this.BulkLoadButton.TabIndex = 2;
            this.BulkLoadButton.Text = "&Upload Document...";
            this.BulkLoadButton.Click += new System.EventHandler(this.BulkLoadButton_Click);
            // 
            // OpenXmlFileDlg
            // 
            this.OpenXmlFileDlg.DefaultExt = "xml";
            this.OpenXmlFileDlg.Filter = "*.xml (Xml Files)|*.xml|*.* (All Files)|*.*";
            this.OpenXmlFileDlg.Title = "Open XML File";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Query Input:";
            // 
            // RunQueryButton
            // 
            this.RunQueryButton.Location = new System.Drawing.Point(10, 164);
            this.RunQueryButton.Name = "RunQueryButton";
            this.RunQueryButton.Size = new System.Drawing.Size(105, 29);
            this.RunQueryButton.TabIndex = 6;
            this.RunQueryButton.Text = "&Run Query";
            this.RunQueryButton.Click += new System.EventHandler(this.RunQueryButton_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(10, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Query Result:";
            // 
            // ExitButton
            // 
            this.ExitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExitButton.Location = new System.Drawing.Point(511, 411);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(77, 29);
            this.ExitButton.TabIndex = 6;
            this.ExitButton.Text = "E&xit";
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // CommitTransactionButton
            // 
            this.CommitTransactionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CommitTransactionButton.Location = new System.Drawing.Point(221, 412);
            this.CommitTransactionButton.Name = "CommitTransactionButton";
            this.CommitTransactionButton.Size = new System.Drawing.Size(135, 29);
            this.CommitTransactionButton.TabIndex = 6;
            this.CommitTransactionButton.Text = "&Commit Transaction";
            this.CommitTransactionButton.Click += new System.EventHandler(this.CommitTransactionButton_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(183, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Common Commands:";
            // 
            // CommandsDropDown
            // 
            this.CommandsDropDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CommandsDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CommandsDropDown.DropDownWidth = 300;
            this.CommandsDropDown.Location = new System.Drawing.Point(296, 4);
            this.CommandsDropDown.Name = "CommandsDropDown";
            this.CommandsDropDown.Size = new System.Drawing.Size(291, 21);
            this.CommandsDropDown.TabIndex = 8;
            this.CommandsDropDown.SelectedIndexChanged += new System.EventHandler(this.CommandsDropDown_SelectedIndexChanged);
            // 
            // AlwaysCommitCheckbox
            // 
            this.AlwaysCommitCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AlwaysCommitCheckbox.Location = new System.Drawing.Point(376, 168);
            this.AlwaysCommitCheckbox.Name = "AlwaysCommitCheckbox";
            this.AlwaysCommitCheckbox.Size = new System.Drawing.Size(211, 16);
            this.AlwaysCommitCheckbox.TabIndex = 10;
            this.AlwaysCommitCheckbox.Text = "Automatically Commit Transactions";
            // 
            // RollbackTransaction
            // 
            this.RollbackTransaction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RollbackTransaction.Location = new System.Drawing.Point(366, 412);
            this.RollbackTransaction.Name = "RollbackTransaction";
            this.RollbackTransaction.Size = new System.Drawing.Size(135, 29);
            this.RollbackTransaction.TabIndex = 6;
            this.RollbackTransaction.Text = "&Rollback Transaction";
            this.RollbackTransaction.Click += new System.EventHandler(this.RollbackTransaction_Click);
            // 
            // sednaCheck
            // 
            this.sednaCheck.AutoSize = true;
            this.sednaCheck.Checked = true;
            this.sednaCheck.Location = new System.Drawing.Point(142, 160);
            this.sednaCheck.Name = "sednaCheck";
            this.sednaCheck.Size = new System.Drawing.Size(71, 17);
            this.sednaCheck.TabIndex = 11;
            this.sednaCheck.TabStop = true;
            this.sednaCheck.Text = "SednaDB";
            this.sednaCheck.UseVisualStyleBackColor = true;
            this.sednaCheck.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // raptorCheck
            // 
            this.raptorCheck.AutoSize = true;
            this.raptorCheck.Location = new System.Drawing.Point(141, 176);
            this.raptorCheck.Name = "raptorCheck";
            this.raptorCheck.Size = new System.Drawing.Size(72, 17);
            this.raptorCheck.TabIndex = 12;
            this.raptorCheck.Text = "RaptorDB";
            this.raptorCheck.UseVisualStyleBackColor = true;
            // 
            // sednaRaptorCheck
            // 
            this.sednaRaptorCheck.AutoSize = true;
            this.sednaRaptorCheck.Location = new System.Drawing.Point(141, 192);
            this.sednaRaptorCheck.Name = "sednaRaptorCheck";
            this.sednaRaptorCheck.Size = new System.Drawing.Size(124, 17);
            this.sednaRaptorCheck.TabIndex = 13;
            this.sednaRaptorCheck.Text = "SednaDB+RaptorDB";
            this.sednaRaptorCheck.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AcceptButton = this.RunQueryButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(604, 459);
            this.Controls.Add(this.sednaRaptorCheck);
            this.Controls.Add(this.raptorCheck);
            this.Controls.Add(this.sednaCheck);
            this.Controls.Add(this.AlwaysCommitCheckbox);
            this.Controls.Add(this.CommandsDropDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RunQueryButton);
            this.Controls.Add(this.BulkLoadButton);
            this.Controls.Add(this.QueryTextBox);
            this.Controls.Add(this.ResultTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.CommitTransactionButton);
            this.Controls.Add(this.RollbackTransaction);
            this.Name = "MainForm";
            this.Text = "Sedna Test";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //--- Event Handlers ---
        private void MainForm_Load(object sender, EventArgs e) {
            Login();

            // try to retrieve the documents in this database...
            try {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ExecuteReadAll("fn:doc(\"auction\")"));
                XmlNodeList list = doc.SelectNodes("//Document");
                if(list != null) {
                    foreach(XmlElement elem in list) {
                        CommandsDropDown.Items.Add("document(\"" + elem.Attributes["name"].Value + "\")");
                    }
                }
            } catch(Exception ex) {
                ReportError("Unable to retrieve metadata for database", ex);
            }

            // add the rest of the common commands...
            foreach(string command in _commands) {
                CommandsDropDown.Items.Add(command);
            }
        }

        private void BulkLoadButton_Click(object sender, EventArgs e) {
            UploadForm form = new UploadForm(_session);
            form.ShowDialog(this);
        }

        private void RunQueryButton_Click(object sender, EventArgs e) {
            //verifica qual radio_button esta selecionado
            if (sednaCheck.Checked)//v
            {
                try
                {
                    ResultTextBox.Text = ExecuteReadAll(QueryTextBox.Text);
                    if (_session.HasTransaction && _session.HasUpdates && AlwaysCommitCheckbox.Checked)
                    {
                        _session.CommitTransaction();
                    }
                }
                catch (Exception ex)
                {
                    ReportError("Unable to execute query or update", ex);
                }
            }
            else
            {
                if (raptorCheck.Checked)
                {
                    ResultTextBox.Text = "TODO raptor selecionado";
                }
                else
                {
                    if(sednaRaptorCheck.Checked)
                    {
                        ResultTextBox.Text = "TODO sedna+raptordb selecionado";
                    }
                }
            }
            QueryTextBox.SelectAll();
            QueryTextBox.Focus();
        }

        private void CommitTransactionButton_Click(object sender, EventArgs e) {
            try {
                _session.CommitTransaction();
                MessageBox.Show(this, "Transaction Committed", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch(Exception ex) {
                ReportError("Unable to commit transaction", ex);
            }
        }

        private void RollbackTransaction_Click(object sender, EventArgs e) {
            try {
                _session.RollbackTransaction();
                MessageBox.Show(this, "Transaction rolled back", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch(Exception ex) {
                ReportError("Unable to rollback transaction", ex);
            }
        }

        private void ExitButton_Click(object sender, EventArgs e) {
            Close();
        }

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if(_session.HasTransaction && _session.HasUpdates) {
                DialogResult res = MessageBox.Show(this, "A transaction is still open in this session. Commit the transaction?", "Transaction Pending", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if(res == DialogResult.Yes) {
                    try {
                        _session.CommitTransaction();
                    } catch(Exception ex) {
                        ReportError("Error committing transaction", ex);
                    }
                } else if(res == DialogResult.Cancel) {
                    e.Cancel = true;
                    return;
                }
            }
            try {
                _session.Close();
            } catch(Exception ex) {
                ReportError("Error closing connection", ex);
            }
        }

        private void CommandsDropDown_SelectedIndexChanged(object sender, EventArgs e) {
            QueryTextBox.Text = CommandsDropDown.Text;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
