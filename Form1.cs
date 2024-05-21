using System;
using System.IO;
using System.Windows.Forms;

namespace Lab_21
{
    public partial class Form1 : Form
    {
        private MenuStrip menuStrip;
        private StatusStrip statusBar;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripMenuItem saveAsMenuItem;

        public Form1()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.IsMdiContainer = true;
            InitMenu();
            InitStatusBar();
            UpdateLocalization();
        }

        private void InitMenu()
        {
            menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem { Tag = "File" };
            fileMenu.DropDownItems.Add(new ToolStripMenuItem { Tag = "New", ShortcutKeys = Keys.Control | Keys.N });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem { Tag = "Open", ShortcutKeys = Keys.Control | Keys.O });
            saveMenuItem = new ToolStripMenuItem { Tag = "Save", ShortcutKeys = Keys.Control | Keys.S };
            saveAsMenuItem = new ToolStripMenuItem { Tag = "Save As" };
            fileMenu.DropDownItems.Add(saveMenuItem);
            fileMenu.DropDownItems.Add(saveAsMenuItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(new ToolStripMenuItem { Tag = "Settings", ShortcutKeys = Keys.Control | Keys.P });
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(new ToolStripMenuItem { Tag = "Exit", ShortcutKeys = Keys.Alt | Keys.F4 });

            var formatMenu = new ToolStripMenuItem { Tag = "Format" };
            formatMenu.DropDownItems.Add(new ToolStripMenuItem { Tag = "Font" });
            formatMenu.DropDownItems.Add(new ToolStripMenuItem { Tag = "Color" });

            var helpMenu = new ToolStripMenuItem { Tag = "Help" };
            helpMenu.DropDownItems.Add(new ToolStripMenuItem { Tag = "About" });

            menuStrip.Items.AddRange(new[] { fileMenu, formatMenu, helpMenu });
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;

            foreach (ToolStripItem mainItem in menuStrip.Items)
            {
                if (mainItem is ToolStripMenuItem menuItem)
                {
                    menuItem.Click += MenuItem_Click;
                    foreach (ToolStripItem subItem in menuItem.DropDownItems)
                    {
                        if (subItem is ToolStripMenuItem subMenuItem)
                        {
                            subMenuItem.Click += MenuItem_Click;
                        }
                    }
                }
            }

            saveMenuItem.Enabled = false;
            saveAsMenuItem.Enabled = false;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                switch (menuItem.Tag)
                {
                    case "New":
                        NewFile_Click(sender, e);
                        break;
                    case "Open":
                        OpenFile_Click(sender, e);
                        break;
                    case "Save":
                        SaveFile_Click(sender, e);
                        break;
                    case "Save As":
                        SaveFileAs_Click(sender, e);
                        break;
                    case "Exit":
                        Application.Exit();
                        break;
                    case "Settings":
                        Settings_Click(sender, e);
                        break;
                    case "About":
                        About_Click(sender, e);
                        break;
                    case "Font":
                        Font_Click(sender, e);
                        break;
                    case "Color":
                        Color_Click(sender, e);
                        break;
                }
            }
        }

        private void InitStatusBar()
        {
            statusBar = new StatusStrip();
            var statusLabel = new ToolStripStatusLabel("Ready");
            statusBar.Items.Add(statusLabel);
            Controls.Add(statusBar);
        }

        public void UpdateLocalization()
        {
            foreach (ToolStripItem mainItem in menuStrip.Items)
            {
                if (mainItem is ToolStripMenuItem menuItem)
                {
                    menuItem.Text = LocalizationManager.T(menuItem.Tag?.ToString() ?? "");

                    foreach (ToolStripItem subItem in menuItem.DropDownItems)
                    {
                        if (subItem is ToolStripMenuItem subMenuItem && subItem.Tag != null)
                        {
                            subMenuItem.Text = LocalizationManager.T(subItem.Tag.ToString());
                        }
                    }
                }
            }

            if (statusBar.Items.Count > 0)
            {
                statusBar.Items[0].Text = LocalizationManager.T("Ready");
            }

            foreach (DocumentForm doc in MdiChildren)
            {
                doc.UpdateLocalization();
            }
        }

        private void NewFile_Click(object sender, EventArgs e)
        {
            var newDocument = new DocumentForm($"Untitled-{MdiChildren.Length + 1}", saveMenuItem, saveAsMenuItem)
            {
                MdiParent = this
            };
            newDocument.Show();
            UpdateSaveMenuItems(true);
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var newDocument = new DocumentForm(Path.GetFileName(openFileDialog.FileName), saveMenuItem, saveAsMenuItem)
                {
                    MdiParent = this
                };
                newDocument.Show();
                newDocument.LoadFile(openFileDialog.FileName);
                UpdateSaveMenuItems(true);
            }
        }

        private void SaveFile_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is DocumentForm activeDocument)
            {
                activeDocument.SaveFile();
            }
        }

        private void SaveFileAs_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is DocumentForm activeDocument)
            {
                activeDocument.SaveFileAs();
            }
        }

        private void Font_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is DocumentForm activeDocument)
            {
                activeDocument.Font_Click(sender, e);
            }
        }

        private void Color_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is DocumentForm activeDocument)
            {
                activeDocument.Color_Click(sender, e);
            }
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
        }

        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show(LocalizationManager.T("About") + "\nMulti-Window Text Editor\nVersion 1.0", LocalizationManager.T("About"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateSaveMenuItems(bool enabled)
        {
            saveMenuItem.Enabled = enabled;
            saveAsMenuItem.Enabled = enabled;
        }
    }
}