﻿namespace SubTrans
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lvItems = new System.Windows.Forms.ListView();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnMerge = new System.Windows.Forms.Button();
            this.cmActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiLoadASS = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSep0 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiMerge = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiReplace = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.cmActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvItems
            // 
            this.lvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvItems.FullRowSelect = true;
            this.lvItems.GridLines = true;
            this.lvItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvItems.HideSelection = false;
            this.lvItems.Location = new System.Drawing.Point(12, 12);
            this.lvItems.Name = "lvItems";
            this.lvItems.Size = new System.Drawing.Size(727, 420);
            this.lvItems.TabIndex = 0;
            this.lvItems.UseCompatibleStateImageBehavior = false;
            this.lvItems.View = System.Windows.Forms.View.Details;
            this.lvItems.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvItems_RetrieveVirtualItem);
            this.lvItems.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvItems_KeyUp);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(664, 442);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load ASS";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPaste.Location = new System.Drawing.Point(93, 442);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(75, 23);
            this.btnPaste.TabIndex = 2;
            this.btnPaste.Text = "Paste";
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Location = new System.Drawing.Point(12, 442);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 3;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReplace.Location = new System.Drawing.Point(553, 442);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(105, 23);
            this.btnReplace.TabIndex = 4;
            this.btnReplace.Text = "Replace To...";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnMerge
            // 
            this.btnMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMerge.Location = new System.Drawing.Point(436, 442);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(111, 23);
            this.btnMerge.TabIndex = 5;
            this.btnMerge.Text = "Merge To...";
            this.btnMerge.UseVisualStyleBackColor = true;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // cmActions
            // 
            this.cmActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiLoadASS,
            this.tsmiSep0,
            this.cmiCopy,
            this.cmiPaste,
            this.tsmiSep1,
            this.cmiSaveAs,
            this.cmiMerge,
            this.cmiReplace,
            this.tsmiSep2,
            this.cmiExit});
            this.cmActions.Name = "cmsContext";
            this.cmActions.Size = new System.Drawing.Size(269, 176);
            // 
            // cmiLoadASS
            // 
            this.cmiLoadASS.Name = "cmiLoadASS";
            this.cmiLoadASS.Size = new System.Drawing.Size(268, 22);
            this.cmiLoadASS.Text = "Load ASS";
            this.cmiLoadASS.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // tsmiSep0
            // 
            this.tsmiSep0.Name = "tsmiSep0";
            this.tsmiSep0.Size = new System.Drawing.Size(265, 6);
            // 
            // cmiCopy
            // 
            this.cmiCopy.Name = "cmiCopy";
            this.cmiCopy.Size = new System.Drawing.Size(268, 22);
            this.cmiCopy.Text = "Copy Selected Text To Clipboard";
            this.cmiCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // cmiPaste
            // 
            this.cmiPaste.Name = "cmiPaste";
            this.cmiPaste.Size = new System.Drawing.Size(268, 22);
            this.cmiPaste.Text = "Paste Translated from Clipboard";
            this.cmiPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // tsmiSep1
            // 
            this.tsmiSep1.Name = "tsmiSep1";
            this.tsmiSep1.Size = new System.Drawing.Size(265, 6);
            // 
            // cmiSaveAs
            // 
            this.cmiSaveAs.Name = "cmiSaveAs";
            this.cmiSaveAs.Size = new System.Drawing.Size(268, 22);
            this.cmiSaveAs.Text = "Save As...";
            this.cmiSaveAs.Click += new System.EventHandler(this.cmiSaveAs_Click);
            // 
            // cmiMerge
            // 
            this.cmiMerge.Name = "cmiMerge";
            this.cmiMerge.Size = new System.Drawing.Size(268, 22);
            this.cmiMerge.Text = "Merge Translated As...";
            this.cmiMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // cmiReplace
            // 
            this.cmiReplace.Name = "cmiReplace";
            this.cmiReplace.Size = new System.Drawing.Size(268, 22);
            this.cmiReplace.Text = "Replace Translated As...";
            this.cmiReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // tsmiSep2
            // 
            this.tsmiSep2.Name = "tsmiSep2";
            this.tsmiSep2.Size = new System.Drawing.Size(265, 6);
            // 
            // cmiExit
            // 
            this.cmiExit.Name = "cmiExit";
            this.cmiExit.Size = new System.Drawing.Size(268, 22);
            this.cmiExit.Text = "Exit";
            this.cmiExit.Click += new System.EventHandler(this.cmiExit_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 473);
            this.ContextMenuStrip = this.cmActions;
            this.Controls.Add(this.btnMerge);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lvItems);
            this.MinimumSize = new System.Drawing.Size(640, 400);
            this.Name = "MainForm";
            this.Text = "Subtitle Translator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.MainForm_DragOver);
            this.cmActions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvItems;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.ContextMenuStrip cmActions;
        private System.Windows.Forms.ToolStripMenuItem cmiCopy;
        private System.Windows.Forms.ToolStripMenuItem cmiPaste;
        private System.Windows.Forms.ToolStripSeparator tsmiSep0;
        private System.Windows.Forms.ToolStripMenuItem cmiMerge;
        private System.Windows.Forms.ToolStripMenuItem cmiReplace;
        private System.Windows.Forms.ToolStripSeparator tsmiSep1;
        private System.Windows.Forms.ToolStripMenuItem cmiLoadASS;
        private System.Windows.Forms.ToolStripSeparator tsmiSep2;
        private System.Windows.Forms.ToolStripMenuItem cmiExit;
        private System.Windows.Forms.ToolStripMenuItem cmiSaveAs;
    }
}

