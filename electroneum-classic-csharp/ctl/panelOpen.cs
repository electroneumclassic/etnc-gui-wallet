﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace electroneum_classic_csharp
{
    public partial class panelOpen : UserControl
    {
        public string WalletFilename;
        public string WalletPassword;

        public event EventHandler OpenSuccess;
        //public event EventHandler Status;

        public panelOpen()
        {
            InitializeComponent();
        }

        private void Success()
        {

            EventHandler handler = OpenSuccess;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string password = "";
            if (InputBox.Show("Set up a wallet password (default no-password)",
                "", ref password) != DialogResult.OK)
            {
                return;
            }

            try
            {
                //browse
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                sfd.RestoreDirectory = true;
                sfd.OverwritePrompt = false;
                sfd.Filter = "Electroneum Classic Wallet (*.wallet)|*.wallet";
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                if (File.Exists(sfd.FileName))
                {
                    MessageBox.Show("File already exist", "etnc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;

                //create wallet
                util.wallet.Create(sfd.FileName, password);

                Cursor.Current = Cursors.Default;

                WalletFilename = sfd.FileName;
                WalletPassword = password;

                this.Success();
            }
            catch (Exception ex)
            {

                Cursor.Current = Cursors.Default;
                log.logger.write(ex.Message, true);
                MessageBox.Show(ex.Message, "etnc", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string password = "";
            if (InputBox.Show("Enter your password (default no-password)",
                "", ref password) != DialogResult.OK)
            {
                return;
            }

            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                ofd.RestoreDirectory = true;
                ofd.Filter = "Electroneum Classic Wallet (*.wallet)|*.wallet|All files (*.*)|*.*";
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                WalletFilename = ofd.FileName;
                WalletPassword = password;

                this.Success();
            }
            catch (Exception ex)
            {
                log.logger.write("Open : " + ex.Message, true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string mnemonic_seed = null;
            if (InputBox.Show("Enter your Mnemonic Seed",
                "", ref mnemonic_seed) != DialogResult.OK)
            {
                return;
            }

            string restore_height = "0";
            if (InputBox.Show("Enter your restore height (default 0)",
                "This should be height of approx your first tx. Restoration can take LONG from 0, look inside wallet.log for progress", ref restore_height) != DialogResult.OK)
            {
                return;
            }

            string password = "";
            if (InputBox.Show("Set a wallet password (default no-password)",
                "", ref password) != DialogResult.OK)
            {
                return;
            }

            try
            {
                //browse
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                sfd.RestoreDirectory = true;
                sfd.OverwritePrompt = false;
                sfd.Filter = "Electroneum Classic Wallet (*.wallet)|*.wallet";
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                if (File.Exists(sfd.FileName))
                {
                    MessageBox.Show("File already exist", "etnc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;

                //create wallet
                util.wallet.CreateFromMnemonic(sfd.FileName, password, mnemonic_seed, restore_height);

                Cursor.Current = Cursors.Default;

                WalletFilename = sfd.FileName;
                WalletPassword = password;

                this.Success();
            }
            catch (Exception ex)
            {

                Cursor.Current = Cursors.Default;
                log.logger.write(ex.Message, true);
                MessageBox.Show(ex.Message, "etnc", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
