using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace electroneum_classic_csharp
{
    public partial class panelMain : UserControl
    {
        //timer balance, transactions, blocked sync
        public string WalletAddress;
        public string Balance;
        public string Unlocked;
        public IEnumerable<res.ModelGetTransfersResultItem> Results;

        public panelMain()
        {
            InitializeComponent();
            startTimerOnce();
        }

        void startTimerOnce()
        {
            Timer tmrOnce = new Timer();
            tmrOnce.Tick += tmrOnce_Tick;
            tmrOnce.Interval = 30000;
            tmrOnce.Start();
        }

        void tmrOnce_Tick(object sender, EventArgs e)
        {
            ((Timer)sender).Dispose();

            redraw_balance();
            redraw_transactions();

            startTimerOnce();
        }

        public void redraw_balance()
        {
            util.wallet.GetBalance();
            double balance = electroneum_classic_csharp.util.wallet.balance;
            double unlocked = electroneum_classic_csharp.util.wallet.unlocked;

            txtWalletAddress.Text = WalletAddress;
            lblBalance.Text = "BALANCE : " + balance.ToString("0.00");
            lblUnlocked.Text = "UNLOCKED : " + unlocked.ToString("0.00");

            main.main_panel.Invoke(new Action(()=> main.main_panel.update_toolstrip_balance(balance.ToString("0.00"))) );
        }

        public void redraw_transactions()
        {
            util.wallet.GetTransaction();
            dgvTransaction.Rows.Clear();
            foreach (res.ModelGetTransfersResultItem item in electroneum_classic_csharp.util.wallet.txResult.OrderByDescending(x => x.timestamp))
            {
                var dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                dateTime = dateTime.AddSeconds(double.Parse(item.timestamp));

                dgvTransaction.Rows.Add(dateTime.ToString("yyyy-MM-dd hh:mm:ss"),
                    item.txid,
                    (double.Parse(item.amount) / 100).ToString("0.00"),
                    item.type);
            }
        }

        private void panelMain_Load(object sender, EventArgs e)
        {
            var nodes = treeView1.Nodes;
            foreach (TreeNode node in nodes)
            {
                if (node.Name == "nodeHome")
                {
                    treeView1.SelectedNode = node;
                }
            }

            redraw_balance();
            redraw_transactions();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Name)
            {
                case "nodeHome":
                    panelHome.Visible = true;
                    panelHome.BringToFront();

                    panelSend.Visible = false;
                    panelMiner.Visible = false;
                    break;
                case "nodeSend":
                    panelSend.Visible = true;
                    panelSend.BringToFront();

                    panelHome.Visible = false;
                    panelMiner.Visible = false;

                    break;

                case "nodeMiner":
                    panelMiner.Visible = true;
                    panelMiner.BringToFront();

                    panelHome.Visible = false;
                    panelSend.Visible = false;

                    break;

                default:
                    panelHome.Visible = true;
                    panelHome.BringToFront();

                    panelSend.Visible = false;
                    panelMiner.Visible = false;

                    break;
            }
     
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            redraw_balance();
            redraw_transactions();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var address = this.textBox1.Text; //Pay To
            var amount = this.textBox2.Text; //Amount
            var pay_id = this.textBox4.Text; //Pay Id
            if (pay_id.Equals(""))
            {
                pay_id = null;
            }

            var amount_str = (int)(double.Parse(amount) * 100);

            var confirmResult = MessageBox.Show(String.Format("Are you sure you want to send {0} ETNC to {1}?", amount, address),
                "",
                MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.No)
            {
                return;
            }

            var tx_hash = util.wallet.Transfer(address, amount_str, pay_id);
            if (tx_hash != null) {
                MessageBox.Show(String.Format("TX_HASH is {0}", tx_hash));
            }
            else {
                MessageBox.Show("Send Failed :(");
            }

            redraw_balance();
            redraw_transactions();
        }
    }
}
