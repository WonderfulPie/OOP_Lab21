using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Lab_21
{
    public partial class DocumentForm : Form
    {
        private RichTextBox richTextBox;
        private string currentFilePath = null;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripMenuItem saveAsMenuItem;

        // Define keywords for syntax highlighting
        private HashSet<string> keywords = new HashSet<string>
        {
            "abstract", "as", "base", "bool", "break",
            "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default",
            "delegate", "do", "double", "else", "enum",
            "event", "explicit", "extern", "false", "finally",
            "fixed", "float", "for", "foreach", "goto",
            "if", "implicit", "in", "int", "interface",
            "internal", "is", "lock", "long", "namespace",
            "new", "null", "object", "operator", "out",
            "override", "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed",
            "short", "sizeof", "stackalloc", "static", "string",
            "struct", "switch", "this", "throw", "true",
            "try", "typeof", "uint", "ulong", "unchecked",
            "unsafe", "ushort", "using", "virtual", "void",
            "volatile", "while"
        };

        public DocumentForm(string title, ToolStripMenuItem globalSaveMenuItem, ToolStripMenuItem globalSaveAsMenuItem)
        {
            InitializeComponent();
            Text = title;
            this.saveMenuItem = globalSaveMenuItem;
            this.saveAsMenuItem = globalSaveAsMenuItem;

            InitEditor();
            ApplySettings();
        }

        private void InitEditor()
        {
            richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                WordWrap = true,
                AllowDrop = true,
                AcceptsTab = true,
                Font = new Font("Consolas", 10)
            };
            richTextBox.DragEnter += RichTextBox_DragEnter;
            richTextBox.DragDrop += RichTextBox_DragDrop;
            richTextBox.TextChanged += RichTextBox_TextChanged;
            Controls.Add(richTextBox);
            InitContextMenu();
        }

        private void InitContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(new ToolStripMenuItem("Cut", null, (s, e) => richTextBox.Cut()) { Tag = "Cut" });
            contextMenu.Items.Add(new ToolStripMenuItem("Copy", null, (s, e) => richTextBox.Copy()) { Tag = "Copy" });
            contextMenu.Items.Add(new ToolStripMenuItem("Paste", null, (s, e) => richTextBox.Paste()) { Tag = "Paste" });
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(new ToolStripMenuItem("Font", null, Font_Click) { Tag = "Font" });
            contextMenu.Items.Add(new ToolStripMenuItem("Color", null, Color_Click) { Tag = "Color" });
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(new ToolStripMenuItem("Align Left", null, (s, e) => richTextBox.SelectionAlignment = HorizontalAlignment.Left) { Tag = "Align Left" });
            contextMenu.Items.Add(new ToolStripMenuItem("Align Center", null, (s, e) => richTextBox.SelectionAlignment = HorizontalAlignment.Center) { Tag = "Align Center" });
            contextMenu.Items.Add(new ToolStripMenuItem("Align Right", null, (s, e) => richTextBox.SelectionAlignment = HorizontalAlignment.Right) { Tag = "Align Right" });
            contextMenu.Items.Add(new ToolStripMenuItem("Justify", null, (s, e) => MessageBox.Show("Justify alignment is not supported.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)) { Tag = "Justify" });

            richTextBox.ContextMenuStrip = contextMenu;
        }

        private void RichTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void RichTextBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && IsImageFile(files[0]))
            {
                try
                {
                    var image = Image.FromFile(files[0]);
                    Clipboard.SetImage(image);
                    richTextBox.Paste();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error inserting image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool IsImageFile(string filePath)
        {
            string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            return Array.Exists(validExtensions, ext => filePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        private void RichTextBox_TextChanged(object sender, EventArgs e)
        {
            HighlightSyntax();
        }

        private void HighlightSyntax()
        {
            int selPos = richTextBox.SelectionStart;
            int selLen = richTextBox.SelectionLength;
            Color originalSelectionColor = richTextBox.SelectionColor;

            // Disable redrawing to avoid flickering
            SendMessage(richTextBox.Handle, WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);

            // Highlight keywords
            foreach (string word in keywords)
            {
                int startIndex = 0;
                while (startIndex < richTextBox.TextLength)
                {
                    int wordStartIndex = richTextBox.Find(word, startIndex, RichTextBoxFinds.WholeWord);
                    if (wordStartIndex != -1)
                    {
                        richTextBox.SelectionStart = wordStartIndex;
                        richTextBox.SelectionLength = word.Length;
                        richTextBox.SelectionColor = Color.Blue;
                        startIndex = wordStartIndex + word.Length;
                    }
                    else
                        break;
                }
            }

            // Reset the selection and color
            richTextBox.SelectionStart = selPos;
            richTextBox.SelectionLength = selLen;
            richTextBox.SelectionColor = originalSelectionColor;

            // Re-enable redrawing
            SendMessage(richTextBox.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
            richTextBox.Invalidate(); // Redraw
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private const int WM_SETREDRAW = 0x0b;

        public void SaveFile()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileAs();
            }
            else
            {
                richTextBox.SaveFile(currentFilePath);
                Text = Path.GetFileName(currentFilePath);
                UpdateSaveMenuItems(false);
            }
        }

        public void SaveFileAs()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|All Files (*.*)|*.*",
                FileName = string.IsNullOrEmpty(currentFilePath) ? Text + ".rtf" : Path.GetFileName(currentFilePath)
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = saveFileDialog.FileName;
                richTextBox.SaveFile(currentFilePath);
                Text = Path.GetFileName(currentFilePath);
                UpdateSaveMenuItems(false);
            }
        }

        public void LoadFile(string filePath)
        {
            currentFilePath = filePath;
            richTextBox.LoadFile(currentFilePath);
            Text = Path.GetFileName(currentFilePath);
            UpdateSaveMenuItems(false);
        }

        public void Font_Click(object sender, EventArgs e)
        {
            using (var fontDialog = new FontDialog())
            {
                fontDialog.Font = richTextBox.SelectionFont ?? richTextBox.Font;
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox.SelectionFont = fontDialog.Font;
                }
            }
        }

        public void Color_Click(object sender, EventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = richTextBox.SelectionColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox.SelectionColor = colorDialog.Color;
                }
            }
        }

        public void UpdateLocalization()
        {
            if (richTextBox.ContextMenuStrip is ContextMenuStrip contextMenu)
            {
                foreach (ToolStripItem item in contextMenu.Items)
                {
                    if (item is ToolStripMenuItem menuItem && menuItem.Tag != null)
                    {
                        menuItem.Text = LocalizationManager.T(menuItem.Tag.ToString());
                    }
                }
            }
        }

        private void ApplySettings()
        {
            richTextBox.Font = new Font("Arial", 12); 
            UpdateSaveMenuItems(false);
        }

        private void UpdateSaveMenuItems(bool enabled)
        {
            saveMenuItem.Enabled = enabled;
            saveAsMenuItem.Enabled = enabled;
        }
    }
}