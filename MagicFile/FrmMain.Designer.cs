namespace MagicFile
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemNew = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorNew = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemAddFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddFolders = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorAdd = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemProset = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripFiles = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStripAddFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripClear = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripMove = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripRules = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddFiles = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddFolders = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPreview = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRename = new System.Windows.Forms.ToolStripButton();
            this.menuStripMain.SuspendLayout();
            this.contextMenuStripFiles.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripMain
            // 
            resources.ApplyResources(this.statusStripMain, "statusStripMain");
            this.statusStripMain.Name = "statusStripMain";
            // 
            // menuStripMain
            // 
            this.menuStripMain.BackColor = System.Drawing.SystemColors.Window;
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemFile,
            this.toolStripMenuItemEdit,
            this.toolStripMenuItemProset,
            this.toolStripMenuItemSetting,
            this.toolStripMenuItemLanguage,
            this.toolStripMenuItemHelp});
            resources.ApplyResources(this.menuStripMain, "menuStripMain");
            this.menuStripMain.Name = "menuStripMain";
            // 
            // toolStripMenuItemFile
            // 
            this.toolStripMenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemNew,
            this.toolStripSeparatorNew,
            this.toolStripMenuItemAddFiles,
            this.toolStripMenuItemAddFolders,
            this.toolStripSeparatorAdd,
            this.toolStripMenuItemExit});
            this.toolStripMenuItemFile.Name = "toolStripMenuItemFile";
            resources.ApplyResources(this.toolStripMenuItemFile, "toolStripMenuItemFile");
            // 
            // toolStripMenuItemNew
            // 
            this.toolStripMenuItemNew.Name = "toolStripMenuItemNew";
            resources.ApplyResources(this.toolStripMenuItemNew, "toolStripMenuItemNew");
            // 
            // toolStripSeparatorNew
            // 
            this.toolStripSeparatorNew.Name = "toolStripSeparatorNew";
            resources.ApplyResources(this.toolStripSeparatorNew, "toolStripSeparatorNew");
            // 
            // toolStripMenuItemAddFiles
            // 
            this.toolStripMenuItemAddFiles.Name = "toolStripMenuItemAddFiles";
            resources.ApplyResources(this.toolStripMenuItemAddFiles, "toolStripMenuItemAddFiles");
            // 
            // toolStripMenuItemAddFolders
            // 
            this.toolStripMenuItemAddFolders.Name = "toolStripMenuItemAddFolders";
            resources.ApplyResources(this.toolStripMenuItemAddFolders, "toolStripMenuItemAddFolders");
            // 
            // toolStripSeparatorAdd
            // 
            this.toolStripSeparatorAdd.Name = "toolStripSeparatorAdd";
            resources.ApplyResources(this.toolStripSeparatorAdd, "toolStripSeparatorAdd");
            // 
            // toolStripMenuItemExit
            // 
            this.toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            resources.ApplyResources(this.toolStripMenuItemExit, "toolStripMenuItemExit");
            // 
            // toolStripMenuItemEdit
            // 
            this.toolStripMenuItemEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemUndo});
            this.toolStripMenuItemEdit.Name = "toolStripMenuItemEdit";
            resources.ApplyResources(this.toolStripMenuItemEdit, "toolStripMenuItemEdit");
            // 
            // toolStripMenuItemUndo
            // 
            this.toolStripMenuItemUndo.Name = "toolStripMenuItemUndo";
            resources.ApplyResources(this.toolStripMenuItemUndo, "toolStripMenuItemUndo");
            this.toolStripMenuItemUndo.Click += new System.EventHandler(this.ToolStripMenuItemUndo_Click);
            // 
            // toolStripMenuItemProset
            // 
            this.toolStripMenuItemProset.Name = "toolStripMenuItemProset";
            resources.ApplyResources(this.toolStripMenuItemProset, "toolStripMenuItemProset");
            // 
            // toolStripMenuItemSetting
            // 
            this.toolStripMenuItemSetting.Name = "toolStripMenuItemSetting";
            resources.ApplyResources(this.toolStripMenuItemSetting, "toolStripMenuItemSetting");
            // 
            // toolStripMenuItemLanguage
            // 
            this.toolStripMenuItemLanguage.Name = "toolStripMenuItemLanguage";
            resources.ApplyResources(this.toolStripMenuItemLanguage, "toolStripMenuItemLanguage");
            // 
            // toolStripMenuItemHelp
            // 
            this.toolStripMenuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAbout});
            this.toolStripMenuItemHelp.Name = "toolStripMenuItemHelp";
            resources.ApplyResources(this.toolStripMenuItemHelp, "toolStripMenuItemHelp");
            // 
            // toolStripMenuItemAbout
            // 
            this.toolStripMenuItemAbout.Name = "toolStripMenuItemAbout";
            resources.ApplyResources(this.toolStripMenuItemAbout, "toolStripMenuItemAbout");
            this.toolStripMenuItemAbout.Click += new System.EventHandler(this.ToolStripMenuItemAbout_Click);
            // 
            // contextMenuStripFiles
            // 
            this.contextMenuStripFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuStripAddFiles,
            this.contextMenuStripSelect,
            this.contextMenuStripClear,
            this.contextMenuStripMove});
            this.contextMenuStripFiles.Name = "contextMenuStripFiles";
            resources.ApplyResources(this.contextMenuStripFiles, "contextMenuStripFiles");
            // 
            // contextMenuStripAddFiles
            // 
            this.contextMenuStripAddFiles.Name = "contextMenuStripAddFiles";
            resources.ApplyResources(this.contextMenuStripAddFiles, "contextMenuStripAddFiles");
            // 
            // contextMenuStripSelect
            // 
            this.contextMenuStripSelect.Name = "contextMenuStripSelect";
            resources.ApplyResources(this.contextMenuStripSelect, "contextMenuStripSelect");
            // 
            // contextMenuStripClear
            // 
            this.contextMenuStripClear.Name = "contextMenuStripClear";
            resources.ApplyResources(this.contextMenuStripClear, "contextMenuStripClear");
            // 
            // contextMenuStripMove
            // 
            this.contextMenuStripMove.Name = "contextMenuStripMove";
            resources.ApplyResources(this.contextMenuStripMove, "contextMenuStripMove");
            // 
            // contextMenuStripRules
            // 
            this.contextMenuStripRules.Name = "contextMenuStripRules";
            resources.ApplyResources(this.contextMenuStripRules, "contextMenuStripRules");
            // 
            // toolStripMain
            // 
            resources.ApplyResources(this.toolStripMain, "toolStripMain");
            this.toolStripMain.BackColor = System.Drawing.SystemColors.MenuBar;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddFiles,
            this.toolStripButtonAddFolders,
            this.toolStripButtonPreview,
            this.toolStripButtonRename});
            this.toolStripMain.Name = "toolStripMain";
            // 
            // toolStripButtonAddFiles
            // 
            this.toolStripButtonAddFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonAddFiles, "toolStripButtonAddFiles");
            this.toolStripButtonAddFiles.Name = "toolStripButtonAddFiles";
            // 
            // toolStripButtonAddFolders
            // 
            this.toolStripButtonAddFolders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonAddFolders, "toolStripButtonAddFolders");
            this.toolStripButtonAddFolders.Name = "toolStripButtonAddFolders";
            // 
            // toolStripButtonPreview
            // 
            this.toolStripButtonPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonPreview, "toolStripButtonPreview");
            this.toolStripButtonPreview.Name = "toolStripButtonPreview";
            // 
            // toolStripButtonRename
            // 
            this.toolStripButtonRename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonRename, "toolStripButtonRename");
            this.toolStripButtonRename.Name = "toolStripButtonRename";
            // 
            // FrmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "FrmMain";
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.contextMenuStripFiles.ResumeLayout(false);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFile;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemProset;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetting;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLanguage;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEdit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUndo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorNew;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddFiles;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddFolders;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorAdd;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExit;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFiles;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRules;
        private System.Windows.Forms.ToolStripMenuItem contextMenuStripAddFiles;
        private System.Windows.Forms.ToolStripMenuItem contextMenuStripSelect;
        private System.Windows.Forms.ToolStripMenuItem contextMenuStripClear;
        private System.Windows.Forms.ToolStripMenuItem contextMenuStripMove;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddFiles;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddFolders;
        private System.Windows.Forms.ToolStripButton toolStripButtonPreview;
        private System.Windows.Forms.ToolStripButton toolStripButtonRename;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAbout;
    }
}

