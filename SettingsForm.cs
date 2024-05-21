using System;
using System.Windows.Forms;

namespace Lab_21
{
    public partial class SettingsForm : Form
    {
        private ComboBox comboLanguage;
        private Label labelLanguage;
        private Button saveButton;

        public SettingsForm()
        {
            InitializeComponent();
            InitComponents();
            LoadSettings();
        }

        private void InitComponents()
        {
            labelLanguage = new Label
            {
                Text = LocalizationManager.T("Language"),
                AutoSize = true,
                Location = new System.Drawing.Point(20, 20)
            };

            comboLanguage = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new System.Drawing.Point(100, 20),
                Width = 150
            };

            // Populate the ComboBox with language options
            comboLanguage.Items.AddRange(new object[] { Language.English.ToString(), Language.Ukrainian.ToString() });
            comboLanguage.SelectedIndex = (int)LocalizationManager.CurrentLanguage;

            saveButton = new Button
            {
                Text = LocalizationManager.T("Save"),
                Location = new System.Drawing.Point(20, 60)
            };
            saveButton.Click += SaveSettings;

            Controls.Add(labelLanguage);
            Controls.Add(comboLanguage);
            Controls.Add(saveButton);

            this.Text = LocalizationManager.T("Settings");
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new System.Drawing.Size(250, 100);
        }

        private void LoadSettings()
        {
            comboLanguage.SelectedItem = LocalizationManager.CurrentLanguage.ToString();
        }

        private void SaveSettings(object sender, EventArgs e)
        {
            LocalizationManager.CurrentLanguage = (Language)Enum.Parse(typeof(Language), comboLanguage.SelectedItem.ToString());

            if (Owner is Form1 mainForm)
            {
                mainForm.UpdateLocalization();
            }

            Close();
        }
    }
}