using ArenaSimulator.Model;
using System.Net;

namespace ArenaSimulator
{
    public partial class Form1 : Form
    {
        public static DateTime lastpull;
        public Form1()
        {
            InitializeComponent();
            lastpull = DateTime.UtcNow;
            //grab RPC Data from API and pick top one which is the recommended one by NCGM.
            var RPCNodes = API.RPC.LoadRPCALL();
            string host = RPCNodes.Result[0].url;

            //grab agent address via planet.exe
            var agentModel = GQL.PullData.GetAddresses();

            //PullAvatar Data
            var avatarList = GQL.PullData.GetAvatar(agentModel[0].publickey, host);

            comboBox1.DataSource = avatarList.Result;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "address";

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            dataGridView1.RowTemplate.Height = 80;

            DataGridViewTextBoxColumn RankColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn avatarNameColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn avatarAddressColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn scoreAddressColumn = new DataGridViewTextBoxColumn();
            DataGridViewButtonColumn percentageCalulator = new DataGridViewButtonColumn();
            RankColumn.Name = "Rank";
            RankColumn.Width = 50;
            avatarNameColumn.Name = "AvatarName";
            avatarAddressColumn.Name = "AvatarAddress";
            avatarAddressColumn.Width = 150;
            scoreAddressColumn.Name = "Score";
            scoreAddressColumn.Width = 55;
            percentageCalulator.Name = "Win%";
            dataGridView1.Columns.Insert(0, RankColumn);
            dataGridView1.Columns.Insert(1, avatarNameColumn);
            dataGridView1.Columns.Insert(2, avatarAddressColumn);
            dataGridView1.Columns.Insert(3, scoreAddressColumn);
            dataGridView1.Columns.Insert(4, percentageCalulator);

            //Call API for ArenaStats
            var data = API.ArenaStats.GetStats();

            foreach (ArenaLeaderBoard itemEntry in data.Result)
            {
                dataGridView1.Rows.Add(itemEntry.rankid, itemEntry.avatarname, itemEntry.avataraddress, itemEntry.score, "Click Me");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (senderGrid.Rows.Count > 0 && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
                {
                    string enemyAddress = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                    string percentage = API.ArenaStats.Simulate(comboBox1.SelectedValue.ToString(), enemyAddress).Result;
                    dataGridView1.Rows[e.RowIndex].Cells[4].Value = percentage+"%";
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var searchbox = (TextBox)sender;
            var charToFind = searchbox.Text;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[1].Value != null)
                    {
                        if (row.Cells[1].Value.ToString().ToLower().Equals(charToFind.ToLower()))
                        {
                            row.Selected = true;
                            dataGridView1.CurrentCell = row.Cells[1];
                            break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Pressing Refresh updates the leaderboard list.
            //First let's check if user is spamming the button, if so let's stop them and tell them that it's pointless as we don't update it so often.
            var diffOfDates = DateTime.UtcNow - lastpull;
            if(diffOfDates.TotalSeconds < 10)
            {
                MessageBox.Show("There's no need to refresh this quick.\nLeaderboard only updates every 30seconds.");
                return;
            }

            //Let's pull the new data and clear the existing one.
            var data = API.ArenaStats.GetStats();

            dataGridView1.Rows.Clear();

            foreach (ArenaLeaderBoard itemEntry in data.Result)
            {
                dataGridView1.Rows.Add(itemEntry.rankid, itemEntry.avatarname, itemEntry.avataraddress, itemEntry.score, "Click Me");
            }

            //With a new list, if the user has previously has searched for someone, let's re-focus on that target.
            if(textBox1.Text.Length > 0)
            {
                var charToFind = textBox1.Text;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                try
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells[1].Value != null)
                        {
                            if (row.Cells[1].Value.ToString().ToLower().Equals(charToFind.ToLower()))
                            {
                                row.Selected = true;
                                dataGridView1.CurrentCell = row.Cells[1];
                                break;
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }

            //Update last time we pulled.
            lastpull = DateTime.UtcNow;
        }
    }
}