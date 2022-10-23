using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teach_Qualifications_App
{
    public partial class AddTeacher : Form
    {
        string filePath = "";
        string fileName = "";
        List<Teacher> teachers = new List<Teacher>();
        public AddTeacher()
        {
            InitializeComponent();
        }
        public Form1 OpenerForm { get; set; }
        private void AddTeacher_Load(object sender, EventArgs e)
        {
            textBox1.Text = GetNewTeacherId().ToString();
        }
        private int GetNewTeacherId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(teacherid), 0) FROM teachers", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    return id + 1;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.pictureBox1.Image = Image.FromFile(this.filePath);
                this.label6.Text = Path.GetFileName(this.filePath);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO teachers 
                                            (teacherid, [name], joindate, picture, post, basicsalary) VALUES
                                            (@i, @n, @j, @p, @o, @b)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@j", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@o", textBox3.Text);
                        cmd.Parameters.AddWithValue("@b", decimal.Parse(textBox4.Text));
                        string ext = Path.GetExtension(this.filePath);
                        fileName = $"{Guid.NewGuid()}{ext}";
                        string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                        File.Copy(filePath, savePath, true);
                        cmd.Parameters.AddWithValue("@p", fileName);
                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tran.Commit();
                                teachers.Add(new Teacher
                                {
                                    teacherid = int.Parse(textBox1.Text),
                                    name = textBox2.Text,
                                    joindate = dateTimePicker1.Value,
                                    post = textBox3.Text,
                                    basicsalary= decimal.Parse(textBox4.Text),
                                    picture = fileName
                                }); 
                                textBox1.Text = GetNewTeacherId().ToString();
                                textBox2.Text = "";
                                textBox3.Text = "";
                                textBox4.Text = "";
                                dateTimePicker1.Value = DateTime.Now;
                                fileName = "";
                                pictureBox1.Image = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }

                    }
                }

            }
        }

        private void AddTeacher_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.OpenerForm.TeachersAdded(teachers);
        }
    }
}
