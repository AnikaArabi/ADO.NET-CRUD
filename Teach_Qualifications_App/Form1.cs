using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teach_Qualifications_App
{
    public partial class Form1 : Form
    {
        DataSet ds; 
        BindingSource bsTeachers = new BindingSource();
        BindingSource bsQualifcations = new BindingSource();
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddTeacher { OpenerForm = this }.ShowDialog();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new AddQualifcation { OpenerForm = this }.ShowDialog();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new EditQualifaction { OpenerForm = this }.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            BindControls();
        }

        private void BindControls()
        {
            bsTeachers.DataSource = ds;
            bsTeachers.DataMember = "teachers";
            bsQualifcations.DataSource = bsTeachers;
            bsQualifcations.DataMember = "FK_teacher_qualification";
            dataGridView1.DataSource = bsQualifcations;
            label5.DataBindings.Add(new Binding("text", bsTeachers, "teacherid"));
            label6.DataBindings.Add(new Binding("text", bsTeachers, "name"));
            var dbDate = new Binding("text", bsTeachers, "joindate", true);
            dbDate.Parse += (s, e) =>
            {
                e.Value = ((DateTime)e.Value).ToString("yyyy-MM-dd");
            };
            label7.DataBindings.Add(dbDate);
            label8.DataBindings.Add(new Binding("text", bsTeachers, "post"));
            
            
            label10.DataBindings.Add(new Binding("text", bsTeachers, "basicsalary"));
            label10.DataBindings["Text"].Format += (s, e) =>
            {
                if (!Convert.IsDBNull(e.Value))
                {
                    e.Value = ((decimal)e.Value).ToString("0.00");
                }
            };
            pictureBox1.DataBindings.Add(new Binding("Image", bsTeachers, "image", true));
        }

        private void LoadData()
        {
            ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM teachers", con))
                {
                    da.Fill(ds, "teachers");
                    ds.Tables["teachers"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for(var i=0; i< ds.Tables["teachers"].Rows.Count; i++)
                    {
                       ds.Tables["teachers"].Rows[i]["image"]= File.ReadAllBytes(Path.Combine(@"..\..\Pictures", ds.Tables["teachers"].Rows[i]["picture"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM qualifications";
                    da.Fill(ds, "qualifications");
                    ds.Relations.Add(new DataRelation(
                            "FK_teacher_qualification",
                            ds.Tables["teachers"].Columns["teacherid"],
                            ds.Tables["qualifications"].Columns["teacherid"]
                        ));
                    ds.AcceptChanges();
                }
            }
        }

        private void firstItem_Click(object sender, EventArgs e)
        {
            bsTeachers.MoveFirst();
        }

        private void lastItem_Click(object sender, EventArgs e)
        {
            bsTeachers.MoveLast();
        }

        private void nextItem_Click(object sender, EventArgs e)
        {
            if(bsTeachers.Position < bsTeachers.Count - 1)
            {
                bsTeachers.MoveNext();
            }
        }

        private void preOtem_Click(object sender, EventArgs e)
        {
            if (bsTeachers.Position > 0)
            {
                bsTeachers.MovePrevious();
            }
        }
        public void TeachersAdded(List<Teacher> teachers)
        {
            foreach (var t in teachers)
            {
                DataRow dr = ds.Tables["teachers"].NewRow();
                dr[0] = t.teacherid; ;
                dr["name"] = t.name;
                dr["joindate"] = t.joindate;
                dr["post"] = t.post;
                dr["basicsalary"] = t.basicsalary;
                dr["picture"] = t.picture;
                dr["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), t.picture));
                ds.Tables["teachers"].Rows.Add(dr);

            }
            ds.AcceptChanges();
        }
        public void TeacherUpdated(Teacher t)
        {
            for (var i = 0; i < ds.Tables["teachers"].Rows.Count; i++)
            {
                if (t.teacherid == (int)ds.Tables["teachers"].Rows[i]["teacherid"])
                {
                    ds.Tables["teachers"].Rows[i]["name"] = t.name;
                    ds.Tables["teachers"].Rows[i]["post"] = t.post;
                    ds.Tables["teachers"].Rows[i]["joindate"] = t.joindate;
                    ds.Tables["teachers"].Rows[i]["basicsalary"] = t.basicsalary;
                    ds.Tables["teachers"].Rows[i]["picture"] = t.picture;
                    ds.Tables["teachers"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), t.picture));

                }
            }


            ds.AcceptChanges();

        }
        public void TeacherDeleted(Teacher t)
        {
            int? index=null;
            for(var i=0; i < ds.Tables["teachers"].Rows.Count; i++)
            {
                if((int)ds.Tables["teachers"].Rows[i]["teacherid"]== t.teacherid)
                {
                    index = i;
                    break;
                }
            }
            if (index.HasValue)
            {
                ds.Tables["teachers"].Rows.RemoveAt(index.Value);
                ds.AcceptChanges();
                bsTeachers.MoveFirst();
            }
        }
        public void QualificationsAdded(List<Qualification> qualifications)
        {
            foreach (var t in qualifications)
            {
                DataRow dr = ds.Tables["qualifications"].NewRow();
                dr[0] = t.qualificationid; ;
                dr["degree"] = t.degree;
                dr["institute"] = t.institute;
                dr["result"] = t.result;
                dr["passingyear"] = t.passingyear;
                dr["teacherid"] = t.teacherid;
                
                ds.Tables["qualifications"].Rows.Add(dr);

            }
            ds.AcceptChanges();
        }
        public void QualificationUpdated(List<Qualification> qs)
        {
            foreach (var q in qs)
            {
                for (var i = 0; i < ds.Tables["qualifications"].Rows.Count; i++)
                {
                    if(q.qualificationid == (int)ds.Tables["qualifications"].Rows[i]["qualificationid"])
                    {
                        ds.Tables["qualifications"].Rows[i]["degree"] = q.degree;
                        ds.Tables["qualifications"].Rows[i]["institute"] = q.institute;
                        ds.Tables["qualifications"].Rows[i]["result"] = q.result;
                        ds.Tables["qualifications"].Rows[i]["passingyear"] = q.passingyear;
                        ds.Tables["qualifications"].Rows[i]["teacherid"] = q.teacherid;
                        break;
                    }
                }

            }
            ds.AcceptChanges();
        }
        public void QualificationDeleted(Qualification t)
        {

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int id = (int)((bsTeachers.Current as DataRowView).Row["teacherid"]);
            new EditTeacher { EditId = id, OpernerForm=this }.ShowDialog();
        }

        
    }
}
