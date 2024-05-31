using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace kariline
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string server = "172.16.1.72";
            string userId = "katsuchat";
            string password = "ae21215926";
            string databaseName = "db_" + DateTime.Now.ToString("yyyyMMdd");
            string tableName = "chat_" + DateTime.Now.ToString("HH");

            string connStr = $"server={server};user id={userId};password={password};";
            //string connStr = "server=172.16.1.72;user id=katsuchat;password=ae21215926;database=katsuchat";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                //接続開いて取得したデータのテーブルを用意
                conn.Open();


                string checkDbQuery = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{databaseName}'";
                MySqlCommand checkDbCmd = new MySqlCommand(checkDbQuery, conn);
                object dbExists = checkDbCmd.ExecuteScalar();

                if (dbExists == null) 
                {
                    string createDbQuery = $"CREATE DATABASE {databaseName}";
                    MySqlCommand createDb = new MySqlCommand(createDbQuery, conn);
                    createDb.ExecuteNonQuery();
                }

                

                conn.ChangeDatabase(databaseName);

                MySqlCommand verifyDbCmd = new MySqlCommand("SELECT DATABASE()", conn);
                string currentDb = verifyDbCmd.ExecuteScalar().ToString();
                if (currentDb != databaseName)
                {
                    throw new Exception("Failed to switch to the specified database.");
                }

                string checkTbQuery = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{databaseName}' AND TABLE_NAME = '{tableName}'";
                MySqlCommand checkTbCmd = new MySqlCommand(checkDbQuery, conn);
                object TbExists = checkTbCmd.ExecuteScalar();

                if (TbExists == null)
                {
                    string createTableQuery = $"CREATE TABLE `{tableName}` (" +
                              "id INT AUTO_INCREMENT PRIMARY KEY, " +
                              "username VARCHAR(255), " +
                              "messeage TEXT)";
                    MySqlCommand createTableCmd = new MySqlCommand(createTableQuery, conn);
                    createTableCmd.ExecuteNonQuery();
                }

                // username と messeage を挿入する SQL コマンド
                string insertQuery = $"INSERT INTO `{tableName}` (username, messeage) VALUES (@username, @messeage)";
                MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@username", textBox1.Text);
                cmd.Parameters.AddWithValue("@messeage", textBox2.Text);
                cmd.ExecuteNonQuery();

                DataTable tbl = new DataTable();
                //SQL文の実行
                string selectQuery = $"SELECT username, messeage FROM `{tableName}` ";
                MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                MySqlDataReader reader = selectCmd.ExecuteReader();
                richTextBox1.Clear();
                //データの表示
                
                while (reader.Read())
                {
                    string username = reader["username"].ToString();
                    string messeage = reader["messeage"].ToString();
                    richTextBox1.AppendText($"Database: {databaseName} Table: {tableName}\n");
                    richTextBox1.AppendText($"{username}: {messeage}\n\n");
                }
                
                // Scroll to the bottom
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                // リーダーと接続を閉じる
                reader.Close();
                conn.Close();
                textBox2.Clear();
            }
            catch (MySqlException mse)
            {
                MessageBox.Show(mse.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}
