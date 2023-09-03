using ArenaSimulator.Model;
using System.Net;

namespace ArenaSimulator
{
    public partial class Form1 : Form
    {
        public static string currentPath = Environment.CurrentDirectory;
        public static List<AvatarModel> avatars = new List<AvatarModel>();
        public static int passiveSim = 0;
        public Form1()
        {
            InitializeComponent();
            if (!Directory.Exists(currentPath + "/sims"))
                Directory.CreateDirectory(currentPath + "/sims");

            //grab RPC Data from API and pick top one which is the recommended one by NCGM.
            var RPCNodes = API.RPC.LoadRPCALL();
            string host = RPCNodes.Result[0].url;

            //grab agent address via planet.exe
            var agentModel = GQL.PullData.GetAddresses();

            //PullAvatar Data
            var avatarList = GQL.PullData.GetAvatar(agentModel[0].publickey, host);
            avatars = avatarList.Result;
            foreach (AvatarModel avatar in avatarList.Result)
            {
                if (!Directory.Exists(currentPath + "/sims" + "/" + avatar.address))
                    Directory.CreateDirectory(currentPath + "/sims" + "/" + avatar.address);
            }

            comboBox1.DataSource = avatarList.Result;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "address";

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            dataGridView1.RowTemplate.Height = 80;

            DataGridViewTextBoxColumn RankColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn avatarNameColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn avatarAddressColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn avatarCPColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn scoreAddressColumn = new DataGridViewTextBoxColumn();
            DataGridViewButtonColumn percentageCalulator = new DataGridViewButtonColumn();
            RankColumn.Name = "Rank";
            RankColumn.Width = 50;
            avatarNameColumn.Name = "AvatarName";
            avatarAddressColumn.Name = "AvatarAddress";
            avatarAddressColumn.Width = 95;
            avatarCPColumn.Name = "CP";
            avatarCPColumn.Width = 55;
            scoreAddressColumn.Name = "Score";
            scoreAddressColumn.Width = 55;
            percentageCalulator.Name = "Win%";
            dataGridView1.Columns.Insert(0, RankColumn);
            dataGridView1.Columns.Insert(1, avatarNameColumn);
            dataGridView1.Columns.Insert(2, avatarAddressColumn);
            dataGridView1.Columns.Insert(3, avatarCPColumn);
            dataGridView1.Columns.Insert(4, scoreAddressColumn);
            dataGridView1.Columns.Insert(5, percentageCalulator);

            //Call API for ArenaStats
            var data = API.ArenaStats.GetStats();

            foreach (ArenaLeaderBoard itemEntry in data.Result)
            {
                var percentage = Helpers.SimHandler.LoadSim(itemEntry.avataraddress, comboBox1.SelectedValue.ToString());
                if (percentage.Result != "")
                    dataGridView1.Rows.Add(itemEntry.rankid, itemEntry.avatarname, itemEntry.avataraddress, itemEntry.cp, itemEntry.score, percentage.Result);
                else
                    dataGridView1.Rows.Add(itemEntry.rankid, itemEntry.avatarname, itemEntry.avataraddress,itemEntry.cp, itemEntry.score, "Click Me");
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
                    dataGridView1.Rows[e.RowIndex].Cells[5].Value = percentage+"%";
                    var result = Helpers.SimHandler.SaveSim(enemyAddress, percentage, comboBox1.SelectedValue.ToString());
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
            //Let's pull the new data and clear the existing one.
            passiveSim = 0;
            checkBox1.Checked = false;
            var data = API.ArenaStats.GetStats();

            dataGridView1.Rows.Clear();

            foreach (ArenaLeaderBoard itemEntry in data.Result)
            {
                var percentage = Helpers.SimHandler.LoadSim(itemEntry.avataraddress, comboBox1.SelectedValue.ToString());
                if (percentage.Result != "")
                    dataGridView1.Rows.Add(itemEntry.rankid, itemEntry.avatarname, itemEntry.avataraddress, itemEntry.cp, itemEntry.score, percentage.Result);
                else
                    dataGridView1.Rows.Add(itemEntry.rankid, itemEntry.avatarname, itemEntry.avataraddress, itemEntry.cp, itemEntry.score, "Click Me");
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
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(dataGridView1.Columns.Count > 4)
            {
                passiveSim = 0;
                checkBox1.Checked = false;
                comboBox1.SelectedValue = avatars[comboBox1.SelectedIndex].address;
                button1_Click(sender, e);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                passiveSim = 1;
                string aName =  comboBox1.Text;
                string aAddress = comboBox1.SelectedValue.ToString();
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    PassiveSimmer(aName,aAddress);
                }).Start();
            }
            else
            {
                passiveSim = 0;
            }
        }

        private void PassiveSimmer(string avatarName, string avatarAddress)
        {
            List<DataGridViewRow> ListOfRows = new List<DataGridViewRow>();
            while(passiveSim == 1)
            {
                try
                {
                    //find user location
                    DataGridViewRow userRow = null;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells[1].Value != null)
                            ListOfRows.Add(row);
                    }

                    userRow = ListOfRows.Where(s => s.Cells[1].Value.ToString().ToLower() == avatarName.ToLower()).FirstOrDefault();

                    if(userRow != null)
                    {
                        //Got a row and a list of rows we can play around with.
                        int maxSim = userRow.Index + 50;
                        int minSim = userRow.Index - 100;
                        if (minSim < 0)
                            minSim = 0;
                        for (int i = minSim; i < maxSim; i++)
                        {
                            if(passiveSim == 1)
                            {
                                string enemyAddress = dataGridView1.Rows[ListOfRows[i].Index].Cells[2].Value.ToString();
                                string percentage = API.ArenaStats.Simulate(avatarAddress, enemyAddress).Result;
                                dataGridView1.Rows[ListOfRows[i].Index].Cells[5].Value = percentage + "%";
                                var result = Helpers.SimHandler.SaveSim(enemyAddress, percentage, avatarAddress);
                                Thread.Sleep(2000);
                            }
                            else
                                break;
                        }
                    }
                }
                catch(Exception ex) { passiveSim = 0; }
            }
        }
    }
}