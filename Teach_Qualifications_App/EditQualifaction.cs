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
    public partial class EditQualifaction : Form
    {
        List<Qualification> qualifications = new List<Qualification>();
        public EditQualifaction()
        {
            InitializeComponent();
        }
        public Form1 OpenerForm { get; set; }
        private void EditQualifaction_Load(object sender, EventArgs e)
        {
            LoadCombo();
        }
        private void LoadCombo()
        {
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter($"SELECT qualificationid,degree FROM qualifications WHERE teacherid=${(int)comboBox1.SelectedValue}", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    this.comboBox2.DataSource = dt.DefaultView;
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand($"SELECT * FROM qualifications WHERE teacherid=@t", con))
                {
                    cmd.Parameters.AddWithValue("@t", (int)comboBox2.SelectedValue);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox2.Text = dr.GetString(1);
                        textBox3.Text = dr.GetString(2);
                        textBox4.Text = dr.GetString(3);
                        textBox5.Text = dr.GetInt32(4).ToString();
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE qualifications 
                                            SET degree=@d, institute=@s, result=@r, passingyear=@y, teacherid=@t 
                                            WHERE qualificationid=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(comboBox2.SelectedValue.ToString()));
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
                                    qualificationid = (int)comboBox1.SelectedValue,
                                    degree = textBox2.Text,
                                    institute = textBox3.Text,
                                    result = textBox4.Text,
                                    passingyear = int.Parse(textBox5.Text),
                                    teacherid = (int)comboBox2.SelectedValue
                                });
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

        private void EditQualifaction_FormClosing(object sender, FormClosingEventArgs e)
        {
            
                this.OpenerForm.QualificationUpdated(qualifications);
        }
    }
}
