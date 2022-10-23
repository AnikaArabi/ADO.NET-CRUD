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
    public partial class EditTeacher : Form
    {
        string filePath="", fileName="", oldPic="";
        Teacher t;
        public EditTeacher()
        {
            InitializeComponent();
        }
        public Form1 OpernerForm { get; set; }
        public int EditId { get; set; }

        private void EditTeacher_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(t != null)
                this.OpernerForm.TeacherUpdated(t);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (CanDelete(this.EditId)) 
            {
                using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
                {
                    con.Open();
                    using (SqlTransaction tran = con.BeginTransaction())
                    {

                        using (SqlCommand cmd = new SqlCommand(@"DELETE teachers 
                                            WHERE teacherid=@i", con, tran))
                        {
                            cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                            


                            try
                            {
                                if (cmd.ExecuteNonQuery() > 0)
                                {
                                    MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    tran.Commit();
                                    t = new Teacher
                                    {
                                        teacherid = int.Parse(textBox1.Text),
                                        name = textBox2.Text,
                                        joindate = dateTimePicker1.Value,
                                        post = textBox3.Text,
                                        basicsalary = decimal.Parse(textBox4.Text),
                                        picture = fileName == "" ? oldPic : fileName
                                    };
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
                                this.OpernerForm.TeacherDeleted(t);
                                this.Dispose();
                            }

                        }
                    }

                }
            }
            else
            {
                MessageBox.Show("This record has related child. Remove child entries first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CanDelete(int editId)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM qualifications WHERE teacherid = @i", con))
                {
                    cmd.Parameters.AddWithValue("@i", editId);
                    con.Open();
                    int n = (int)cmd.ExecuteScalar();
                    con.Close();
                    return n == 0;
                }
            }
        }

        private void EditTeacher_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand($"SELECT * FROM teachers WHERE teacherid=@t", con))
                {
                    cmd.Parameters.AddWithValue("@t", this.EditId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(dr.GetOrdinal("teacherid")).ToString();
                        textBox2.Text = dr.GetString(dr.GetOrdinal("name")).ToString();
                        dateTimePicker1.Value = dr.GetDateTime(dr.GetOrdinal("joindate"));
                        textBox3.Text = dr.GetString(dr.GetOrdinal("post")).ToString();
                        textBox4.Text = dr.GetDecimal(dr.GetOrdinal("basicsalary")).ToString("0.00");
                        //label6.Text = dr.GetString(dr.GetOrdinal("picture")).ToString();
                        oldPic = dr.GetString(dr.GetOrdinal("picture")).ToString();
                        pictureBox1.Image = Image.FromFile(Path.Combine(@"..\..\Pictures", oldPic));
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE teachers 
                                            SET [name]=@n, joindate=@j, post=@p, basicsalary=@b, picture = @pic
                                            WHERE teacherid=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse( textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@j", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@p", textBox3.Text);
                        cmd.Parameters.AddWithValue("@b", decimal.Parse( textBox4.Text));
                        if(label6.Text == "")
                        {
                            cmd.Parameters.AddWithValue("@pic", oldPic);
                        }
                        else
                        {
                            string ext = Path.GetExtension(this.filePath);
                            fileName = $"{Guid.NewGuid()}{ext}";
                            string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                            File.Copy(filePath, savePath, true);
                            cmd.Parameters.AddWithValue("@pic", fileName);
                        }


                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                tran.Commit();
                                t = new Teacher
                                {
                                    teacherid = int.Parse(textBox1.Text),
                                    name = textBox2.Text,
                                    joindate = dateTimePicker1.Value,
                                    post = textBox3.Text,
                                    basicsalary = decimal.Parse(textBox4.Text),
                                    picture = fileName == "" ? oldPic: fileName
                                };
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
    }
}

