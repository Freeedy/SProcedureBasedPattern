﻿namespace ProcedureExecuter
{
    partial class Contrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtBody = new System.Windows.Forms.TextBox();
            this.chkNullable = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(3, 6);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(37, 13);
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = "Param";
            // 
            // txtBody
            // 
            this.txtBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBody.Location = new System.Drawing.Point(71, 3);
            this.txtBody.Name = "txtBody";
            this.txtBody.Size = new System.Drawing.Size(126, 20);
            this.txtBody.TabIndex = 1;
            this.txtBody.Text = "value";
            // 
            // chkNullable
            // 
            this.chkNullable.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkNullable.AutoSize = true;
            this.chkNullable.Location = new System.Drawing.Point(214, 5);
            this.chkNullable.Name = "chkNullable";
            this.chkNullable.Size = new System.Drawing.Size(54, 17);
            this.chkNullable.TabIndex = 2;
            this.chkNullable.Text = "NULL";
            this.chkNullable.UseVisualStyleBackColor = true;
            this.chkNullable.CheckedChanged += new System.EventHandler(this.chkNullable_CheckedChanged);
            // 
            // Contrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkNullable);
            this.Controls.Add(this.txtBody);
            this.Controls.Add(this.lblDescription);
            this.Name = "Contrl";
            this.Size = new System.Drawing.Size(271, 30);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtBody;
        private System.Windows.Forms.CheckBox chkNullable;
    }
}
