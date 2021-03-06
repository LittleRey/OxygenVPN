﻿using System;
using System.Net;
using System.Windows.Forms;
using OxygenVPN.Utils;

namespace OxygenVPN.Forms
{
    public partial class GlobalBypassIPForm : Form
    {
        public GlobalBypassIPForm()
        {
            InitializeComponent();
        }

        private void GlobalBypassIPForm_Load(object sender, EventArgs e)
        {
            i18N.TranslateForm(this);

            IPListBox.Items.AddRange(Global.Settings.BypassIPs.ToArray());

            for (var i = 32; i >= 1; i--)
            {
                PrefixComboBox.Items.Add(i);
            }
            PrefixComboBox.SelectedIndex = 0;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(IPTextBox.Text))
            {
                if (IPAddress.TryParse(IPTextBox.Text, out var address))
                {
                    IPListBox.Items.Add(string.Format("{0}/{1}", address, PrefixComboBox.SelectedItem));
                }
                else
                {
                    MessageBoxX.Show(i18N.Translate("Please enter a correct IP address"));
                }
            }
            else
            {
                MessageBoxX.Show(i18N.Translate("Please enter an IP"));
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (IPListBox.SelectedIndex != -1)
            {
                IPListBox.Items.RemoveAt(IPListBox.SelectedIndex);
            }
            else
            {
                MessageBoxX.Show(i18N.Translate("Please select an IP"));
            }
        }

        private void ControlButton_Click(object sender, EventArgs e)
        {
            Global.Settings.BypassIPs.Clear();
            foreach (var ip in IPListBox.Items)
            {
                Global.Settings.BypassIPs.Add(ip as string);
            }

            Configuration.Save();
            MessageBoxX.Show(i18N.Translate("Saved"));
            Close();
        }
    }
}
