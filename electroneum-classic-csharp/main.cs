using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace electroneum_classic_csharp
{
    public partial class main : Form
    {
        public static main main_panel;
        private panelOpen ctlPanelOpen;
        private panelMain ctlPanelMain;

        public main()
        {
            InitializeComponent();

            main_panel = this;

            menuStrip1.Visible = false;
            statusStrip1.Visible = false;
        }

        private void main_Load(object sender, EventArgs e)
        {
            try
            {

                this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                this.Text = "";

                ctlPanelOpen = new panelOpen();
                ctlPanelOpen.Dock = DockStyle.Fill;
                ctlPanelOpen.OpenSuccess += ControlOpenSuccess;

                panel1.Controls.Add(ctlPanelOpen);
                panel1.Dock = DockStyle.Fill;
                panel1.Visible = true;
                panel1.BringToFront();

            }
            catch (Exception ex)
            {
                log.logger.write(ex.Message, true);
            }

        }

        private void ControlOpenSuccess(object sender, EventArgs e) {

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                //run rpc
                util.wallet.RunRPC(ctlPanelOpen.WalletFilename, ctlPanelOpen.WalletPassword);

                //wait until rpc running complete
                while (!util.wallet.ping("127.0.0.1", "26980"))
                {                    
                    System.Threading.Thread.Sleep(100);
                }

                //get address
                var address = util.wallet.GetWalletAddress();

                Cursor.Current = Cursors.Default;

                panel1.Controls.Clear();

                ctlPanelMain = new panelMain();
                ctlPanelMain.Dock = DockStyle.Fill;
                ctlPanelMain.WalletAddress = address.result.address;

                update_balance();

                update_transactions();

                panel1.Controls.Add(ctlPanelMain);
                panel1.Dock = DockStyle.Fill;
                panel1.Visible = true;
                panel1.BringToFront();

                this.Text = "Electroneum Classic";
                this.FormBorderStyle = FormBorderStyle.Sizable;
                menuStrip1.Visible = true;
                statusStrip1.Visible = true;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                log.logger.write(ex.Message, true);
            }
        }

        public void update_balance()
        {
            //get balance
            var balance = util.wallet.GetBalance();
            ctlPanelMain.Balance = balance.result.balance;
            ctlPanelMain.Unlocked = balance.result.unlocked_balance;
        }

        public void update_transactions()
        {
            //get transaction
            var transaction = util.wallet.GetTransaction();

            List<res.ModelGetTransfersResultItem> results = new List<res.ModelGetTransfersResultItem>();

            if (transaction.result._in != null)
            {
                foreach (res.ModelGetTransfersResultItem item in transaction.result._in)
                {
                    results.Add(item);
                }
            }


            if (transaction.result._out != null)
            {
                foreach (res.ModelGetTransfersResultItem item in transaction.result._out)
                {
                    results.Add(item);
                }
            }



            if (transaction.result.pending != null)
            {
                foreach (res.ModelGetTransfersResultItem item in transaction.result.pending)
                {
                    results.Add(item);
                }
            }


            if (transaction.result.failed != null)
            {
                foreach (res.ModelGetTransfersResultItem item in transaction.result.failed)
                {
                    results.Add(item);
                }
            }


            if (transaction.result.pool != null)
            {
                foreach (res.ModelGetTransfersResultItem item in transaction.result.pool)
                {
                    results.Add(item);
                }
            }

            var sortTxn = results.OrderByDescending(x => x.timestamp);
            ctlPanelMain.Results = sortTxn;
        }

        public void update_toolstrip_balance(string balance)
        {
            toolStripStatusLabel1.Text = balance + " ETNC";
        }
    }
}
