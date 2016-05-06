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
using System.Windows.Forms;

namespace Sedna {

    /// <summary>
    /// Summary description for DocumentNameTextBoxForm.
    /// </summary>
    public class UploadForm : Form {

        //--- Fields ---
        private TabControl Pages;
        private TabPage DocNamePage;
        private TabPage DocFilePage;
        private Label label1;
        private TextBox DocumentNameTextBox;
        private Button NextButton;
        private new Button CancelButton;
        private Label label2;
        private TextBox FileNameTextBox;
        private Button FindFileButton;
        private Button XmlDataSource;
        private OpenFileDialog OpenXmlFileDlg;
        private Timer FocusControlTimer;
        private System.ComponentModel.IContainer components;
        private SednaSession _session = null;

        //--- Constructors ---
        public UploadForm(SednaSession session) {
            InitializeComponent();
            _session = session;
        }

        protected override void Dispose(bool disposing) {
            if(disposing) {
                if(components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void StartUpLoad() {
            string file = FileNameTextBox.Text;
            try {
                Cursor.Current = Cursors.WaitCursor;
                _session.Execute("LOAD \"" + file + "\" " + "\"" + DocumentNameTextBox.Text + "\"");
                MessageBox.Show(this, "Upload was successful");
                Close();
            } catch(Exception ex) {
                MessageBox.Show(this, "Upload failed: " + ex.Message);
            } finally {
                Cursor.Current = Cursors.Default;
            }
        }

        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.Pages = new System.Windows.Forms.TabControl();
            this.DocNamePage = new System.Windows.Forms.TabPage();
            this.DocumentNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DocFilePage = new System.Windows.Forms.TabPage();
            this.FindFileButton = new System.Windows.Forms.Button();
            this.FileNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.NextButton = new System.Windows.Forms.Button();
            this.XmlDataSource = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OpenXmlFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.FocusControlTimer = new System.Windows.Forms.Timer(this.components);
            this.Pages.SuspendLayout();
            this.DocNamePage.SuspendLayout();
            this.DocFilePage.SuspendLayout();
            this.SuspendLayout();
            // 
            // Pages
            // 
            this.Pages.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.Pages.Controls.Add(this.DocNamePage);
            this.Pages.Controls.Add(this.DocFilePage);
            this.Pages.Location = new System.Drawing.Point(5, -21);
            this.Pages.Name = "Pages";
            this.Pages.SelectedIndex = 0;
            this.Pages.Size = new System.Drawing.Size(358, 139);
            this.Pages.TabIndex = 0;
            // 
            // DocNamePage
            // 
            this.DocNamePage.Controls.Add(this.DocumentNameTextBox);
            this.DocNamePage.Controls.Add(this.label1);
            this.DocNamePage.Location = new System.Drawing.Point(4, 25);
            this.DocNamePage.Name = "DocNamePage";
            this.DocNamePage.Size = new System.Drawing.Size(350, 110);
            this.DocNamePage.TabIndex = 0;
            // 
            // DocumentNameTextBox
            // 
            this.DocumentNameTextBox.Location = new System.Drawing.Point(12, 38);
            this.DocumentNameTextBox.Name = "DocumentNameTextBox";
            this.DocumentNameTextBox.Size = new System.Drawing.Size(147, 20);
            this.DocumentNameTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please enter a name for the document:";
            // 
            // DocFilePage
            // 
            this.DocFilePage.Controls.Add(this.FindFileButton);
            this.DocFilePage.Controls.Add(this.FileNameTextBox);
            this.DocFilePage.Controls.Add(this.label2);
            this.DocFilePage.Location = new System.Drawing.Point(4, 25);
            this.DocFilePage.Name = "DocFilePage";
            this.DocFilePage.Size = new System.Drawing.Size(350, 110);
            this.DocFilePage.TabIndex = 1;
            this.DocFilePage.Text = "tabPage1";
            // 
            // FindFileButton
            // 
            this.FindFileButton.Location = new System.Drawing.Point(256, 38);
            this.FindFileButton.Name = "FindFileButton";
            this.FindFileButton.Size = new System.Drawing.Size(46, 25);
            this.FindFileButton.TabIndex = 4;
            this.FindFileButton.Text = "...";
            this.FindFileButton.Click += new System.EventHandler(this.FindFileButton_Click);
            // 
            // FileNameTextBox
            // 
            this.FileNameTextBox.Location = new System.Drawing.Point(10, 40);
            this.FileNameTextBox.Name = "FileNameTextBox";
            this.FileNameTextBox.Size = new System.Drawing.Size(242, 20);
            this.FileNameTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(334, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "Please enter the path and filename of the document to upload:";
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(146, 132);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(83, 25);
            this.NextButton.TabIndex = 1;
            this.NextButton.Text = "Next >>";
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // XmlDataSource
            // 
            this.XmlDataSource.Location = new System.Drawing.Point(53, 132);
            this.XmlDataSource.Name = "XmlDataSource";
            this.XmlDataSource.Size = new System.Drawing.Size(83, 25);
            this.XmlDataSource.TabIndex = 0;
            this.XmlDataSource.Text = "Back";
            this.XmlDataSource.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(263, 132);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(93, 25);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OpenXmlFileDlg
            // 
            this.OpenXmlFileDlg.DefaultExt = "xml";
            this.OpenXmlFileDlg.Filter = "*.xml (Xml Files)|*.xml|*.* (All Files)|*.*";
            this.OpenXmlFileDlg.Title = "Open XML File";
            // 
            // FocusControlTimer
            // 
            this.FocusControlTimer.Interval = 10;
            this.FocusControlTimer.Tick += new System.EventHandler(this.FocusControlTimer_Tick);
            // 
            // UploadForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(364, 164);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.Pages);
            this.Controls.Add(this.XmlDataSource);
            this.Controls.Add(this.CancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UploadForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Upload Document";
            this.Load += new System.EventHandler(this.UploadForm_Load);
            this.Pages.ResumeLayout(false);
            this.DocNamePage.ResumeLayout(false);
            this.DocNamePage.PerformLayout();
            this.DocFilePage.ResumeLayout(false);
            this.DocFilePage.PerformLayout();
            this.ResumeLayout(false);

        }

        //--- Event Handlers ---
        private void NextButton_Click(object sender, System.EventArgs e) {
            if(Pages.SelectedTab == DocFilePage) {
                if(FileNameTextBox.Text == "" || !System.IO.File.Exists(FileNameTextBox.Text)) {
                    MessageBox.Show(this, "Please enter a valid filename");
                } else {
                    // we're done...
                    StartUpLoad();
                }
            } else if(Pages.SelectedTab == DocNamePage) {
                if(DocumentNameTextBox.Text == "") {
                    MessageBox.Show(this, "Please enter a document name");
                } else {
                    Pages.SelectedTab = DocFilePage;
                    NextButton.Text = "Finish";
                }
            }
        }

        private void BackButton_Click(object sender, System.EventArgs e) {
            if(Pages.SelectedTab == DocFilePage) {
                Pages.SelectedTab = DocNamePage;
                NextButton.Text = "Next";
            }
        }

        private void UploadForm_Load(object sender, System.EventArgs e) {
            FocusControlTimer.Enabled = true;
        }

        private void FindFileButton_Click(object sender, System.EventArgs e) {
            OpenXmlFileDlg.FileName = FileNameTextBox.Text;
            if(OpenXmlFileDlg.ShowDialog(this) == DialogResult.OK) {
                string file = OpenXmlFileDlg.FileName;
                FileNameTextBox.Text = file;
            }
        }

        private void CancelButton_Click(object sender, System.EventArgs e) {
            Close();
        }

        private void FocusControlTimer_Tick(object sender, System.EventArgs e) {
            Pages.Focus();
            DocumentNameTextBox.Focus();
            FocusControlTimer.Enabled = false;
        }
    }
}
