using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teach_Qualifications_App
{
    public partial class AddQualifcation : Form
    {
#pragma warning disable IDE0044 // Add readonly modifier
        List<Qualification> qualifications = new List<Qualification>();
#pragma warning restore IDE0044 // Add readonly modifier
        public AddQualifcation()
        {
            InitializeComponent();
        }
        public Form1 OpenerForm { get; set; }
        private void AddQualifcation_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = GetNewQualificationId().ToString();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT teacherid,[name] FROM teachers", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    this.comboBox1.DataSource = dt.DefaultView;
                }
            }
        }
        private int GetNewQualificationId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(qualificationid), 0) FROM qualifications", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    return id + 1;
                }
            }
        }

#pragma warning disable IDE1006 // Naming Styles
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO qualifications 
                                            (qualificationid, degree, institute, result, passingyear, teacherid) VALUES
                                            (@i, @d, @s, @r, @y, @t)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@d", textBox2.Text);
                        cmd.Parameters.AddWithValue("@s", textBox3.Text);
                        cmd.Parameters.AddWithValue("@r", textBox4.Text);
                        cmd.Parameters.AddWithValue("@y", textBox5.Text);
                        cmd.Parameters.AddWithValue("@t", (int)comboBox1.SelectedValue);
                      

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                
                                tran.Commit();
                                qualifications.Add(new Qualification
                                {
                                    qualificationid= int.Parse(textBox1.Text),
                                    degree = textBox2.Text,
                                    institute = textBox3.Text,
                                    result = textBox4.Text,
                                    passingyear = int.Parse(textBox5.Text),
                                    teacherid = (int)comboBox1.SelectedValue
                                });
                                textBox1.Text = GetNewQualificationId().ToString();
                                textBox2.Clear();
                                textBox3.Clear();
                                textBox4.Clear(); 
                                textBox5.Clear();
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

        private void AddQualifcation_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.OpenerForm.QualificationsAdded(qualifications);
        }
    }
}
