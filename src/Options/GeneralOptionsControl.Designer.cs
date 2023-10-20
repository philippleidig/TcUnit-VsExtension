
using System.Drawing;
using System.Windows.Forms;

namespace TcUnit.Options
{
    partial class GeneralOptionsControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.nameConventionGroup = new System.Windows.Forms.GroupBox();
            this.testCaseNamingRegex = new System.Windows.Forms.TextBox();
            this.testSuiteNamingRegex = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.nameConventionGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBox1);
            this.groupBox1.Location = new System.Drawing.Point(19, 143);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(439, 229);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test Case Template";
            // 
            // richTextBox1
            // 
            this.richTextBox1.AcceptsTab = true;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 16);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(10);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.richTextBox1.Size = new System.Drawing.Size(433, 210);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            this.richTextBox1.WordWrap = false;
            // 
            // nameConventionGroup
            // 
            this.nameConventionGroup.Controls.Add(this.testCaseNamingRegex);
            this.nameConventionGroup.Controls.Add(this.testSuiteNamingRegex);
            this.nameConventionGroup.Controls.Add(this.label2);
            this.nameConventionGroup.Controls.Add(this.label1);
            this.nameConventionGroup.Location = new System.Drawing.Point(22, 14);
            this.nameConventionGroup.Name = "nameConventionGroup";
            this.nameConventionGroup.Size = new System.Drawing.Size(433, 116);
            this.nameConventionGroup.TabIndex = 2;
            this.nameConventionGroup.TabStop = false;
            this.nameConventionGroup.Text = "Naming Conventions";
            // 
            // testCaseNamingRegex
            // 
            this.testCaseNamingRegex.Location = new System.Drawing.Point(19, 80);
            this.testCaseNamingRegex.Name = "testCaseNamingRegex";
            this.testCaseNamingRegex.Size = new System.Drawing.Size(201, 20);
            this.testCaseNamingRegex.TabIndex = 3;
            // 
            // testSuiteNamingRegex
            // 
            this.testSuiteNamingRegex.Location = new System.Drawing.Point(19, 37);
            this.testSuiteNamingRegex.Name = "testSuiteNamingRegex";
            this.testSuiteNamingRegex.Size = new System.Drawing.Size(201, 20);
            this.testSuiteNamingRegex.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Test Case Naming Regex";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Test Suite Naming Regex";
            // 
            // GeneralOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nameConventionGroup);
            this.Controls.Add(this.groupBox1);
            this.Name = "GeneralOptionsControl";
            this.Size = new System.Drawing.Size(481, 395);
            this.Load += new System.EventHandler(this.richTextBox1_Load);
            this.groupBox1.ResumeLayout(false);
            this.nameConventionGroup.ResumeLayout(false);
            this.nameConventionGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private GroupBox groupBox1;
        private RichTextBox richTextBox1;
        private GroupBox nameConventionGroup;
        private TextBox testCaseNamingRegex;
        private TextBox testSuiteNamingRegex;
        private Label label2;
        private Label label1;
    }
}
