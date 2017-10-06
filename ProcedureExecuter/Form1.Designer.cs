namespace ProcedureExecuter
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.chkState = new System.Windows.Forms.CheckBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtConstr = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.rTxtresult = new System.Windows.Forms.RichTextBox();
            this.btnExecuteNonQ = new System.Windows.Forms.Button();
            this.btnExecuteDs = new System.Windows.Forms.Button();
            this.btnAddreturnparam = new System.Windows.Forms.Button();
            this.lstProcs = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dataGridSet = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatues = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSet)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chkState);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Controls.Add(this.txtConstr);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1006, 55);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DbAgent ";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(428, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Connection String";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Name";
            // 
            // chkState
            // 
            this.chkState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkState.AutoSize = true;
            this.chkState.Checked = true;
            this.chkState.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkState.Location = new System.Drawing.Point(791, 31);
            this.chkState.Name = "chkState";
            this.chkState.Size = new System.Drawing.Size(51, 17);
            this.chkState.TabIndex = 3;
            this.chkState.Text = "State";
            this.chkState.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(925, 26);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtConstr
            // 
            this.txtConstr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConstr.Location = new System.Drawing.Point(226, 29);
            this.txtConstr.Name = "txtConstr";
            this.txtConstr.Size = new System.Drawing.Size(526, 20);
            this.txtConstr.TabIndex = 1;
            this.txtConstr.Text = "Data Source=";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(21, 29);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(156, 20);
            this.txtName.TabIndex = 1;
            this.txtName.Text = "myDbAgent";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnClear);
            this.groupBox2.Controls.Add(this.rTxtresult);
            this.groupBox2.Controls.Add(this.btnExecuteNonQ);
            this.groupBox2.Controls.Add(this.btnExecuteDs);
            this.groupBox2.Controls.Add(this.btnAddreturnparam);
            this.groupBox2.Controls.Add(this.lstProcs);
            this.groupBox2.Location = new System.Drawing.Point(12, 88);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1006, 215);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Procedures";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(226, 182);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 23);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // rTxtresult
            // 
            this.rTxtresult.AcceptsTab = true;
            this.rTxtresult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rTxtresult.AutoWordSelection = true;
            this.rTxtresult.BackColor = System.Drawing.Color.DarkGray;
            this.rTxtresult.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rTxtresult.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.rTxtresult.Location = new System.Drawing.Point(368, 19);
            this.rTxtresult.Name = "rTxtresult";
            this.rTxtresult.ReadOnly = true;
            this.rTxtresult.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rTxtresult.Size = new System.Drawing.Size(632, 186);
            this.rTxtresult.TabIndex = 3;
            this.rTxtresult.Text = "";
            this.rTxtresult.TextChanged += new System.EventHandler(this.rTxtresult_TextChanged);
            // 
            // btnExecuteNonQ
            // 
            this.btnExecuteNonQ.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnExecuteNonQ.Location = new System.Drawing.Point(226, 77);
            this.btnExecuteNonQ.Name = "btnExecuteNonQ";
            this.btnExecuteNonQ.Size = new System.Drawing.Size(120, 23);
            this.btnExecuteNonQ.TabIndex = 1;
            this.btnExecuteNonQ.Text = "ExecuteNonQuery";
            this.btnExecuteNonQ.UseVisualStyleBackColor = true;
            this.btnExecuteNonQ.Click += new System.EventHandler(this.btnExecuteNonQ_Click);
            // 
            // btnExecuteDs
            // 
            this.btnExecuteDs.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnExecuteDs.Location = new System.Drawing.Point(226, 48);
            this.btnExecuteDs.Name = "btnExecuteDs";
            this.btnExecuteDs.Size = new System.Drawing.Size(120, 23);
            this.btnExecuteDs.TabIndex = 1;
            this.btnExecuteDs.Text = "ExecuteDataSet";
            this.btnExecuteDs.UseVisualStyleBackColor = true;
            this.btnExecuteDs.Click += new System.EventHandler(this.btnExecuteDs_Click);
            // 
            // btnAddreturnparam
            // 
            this.btnAddreturnparam.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnAddreturnparam.Location = new System.Drawing.Point(226, 19);
            this.btnAddreturnparam.Name = "btnAddreturnparam";
            this.btnAddreturnparam.Size = new System.Drawing.Size(120, 23);
            this.btnAddreturnparam.TabIndex = 1;
            this.btnAddreturnparam.Text = "AddReturnParameter";
            this.btnAddreturnparam.UseVisualStyleBackColor = true;
            this.btnAddreturnparam.Click += new System.EventHandler(this.btnAddreturnparam_Click);
            // 
            // lstProcs
            // 
            this.lstProcs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lstProcs.FormattingEnabled = true;
            this.lstProcs.Location = new System.Drawing.Point(21, 19);
            this.lstProcs.Name = "lstProcs";
            this.lstProcs.Size = new System.Drawing.Size(156, 186);
            this.lstProcs.TabIndex = 0;
            this.lstProcs.SelectedIndexChanged += new System.EventHandler(this.lstProcs_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.dataGridSet);
            this.groupBox3.Location = new System.Drawing.Point(12, 309);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1006, 227);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Data Grid";
            // 
            // dataGridSet
            // 
            this.dataGridSet.AllowUserToAddRows = false;
            this.dataGridSet.AllowUserToDeleteRows = false;
            this.dataGridSet.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridSet.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridSet.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridSet.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridSet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridSet.Location = new System.Drawing.Point(3, 16);
            this.dataGridSet.Name = "dataGridSet";
            this.dataGridSet.ReadOnly = true;
            this.dataGridSet.RowHeadersWidth = 15;
            this.dataGridSet.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridSet.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.dataGridSet.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Blue;
            this.dataGridSet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridSet.Size = new System.Drawing.Size(1000, 208);
            this.dataGridSet.TabIndex = 4;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatues});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1030, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatues
            // 
            this.lblStatues.Name = "lblStatues";
            this.lblStatues.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1030, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1030, 561);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1046, 599);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSet)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkState;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox txtConstr;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lstProcs;
        private System.Windows.Forms.Button btnExecuteNonQ;
        private System.Windows.Forms.Button btnExecuteDs;
        private System.Windows.Forms.Button btnAddreturnparam;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox rTxtresult;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatues;
        private System.Windows.Forms.DataGridView dataGridSet;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

