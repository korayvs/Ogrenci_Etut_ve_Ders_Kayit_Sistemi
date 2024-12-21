using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace SQL_Etut_Ders
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection bgl = new SqlConnection(@"Data Source=DESKTOP-F1A12T8\KORAY;Initial Catalog=Ogrenci_Etut_Ders;Integrated Security=True");

        void derslistesi()
        {
            SqlDataAdapter da = new SqlDataAdapter("Select * From TBLDERSLER",bgl);
            DataTable dt = new DataTable();
            da.Fill(dt);
            CmbDers.ValueMember = "DERSID";
            CmbDers.DisplayMember = "DERSAD";
            CmbDers.DataSource = dt;
        }

        //Etüt Listesi
        void etutlistesi()
        {
            SqlDataAdapter da3 = new SqlDataAdapter("Exec Etut", bgl);
            DataTable d3 = new DataTable();
            da3 .Fill(d3);
            dataGridView1.DataSource = d3;
        }

        void dersler()
        {
            SqlDataAdapter da = new SqlDataAdapter("Select ID, TBLDERSLER.AD As 'DERS', (TBLOGRETMEN.AD + ' ' + TBLOGRETMEN.SOYAD) As 'ÖĞRETMEN', TARIH, SAAT, DURUM From TBLETUT Inner Join TBLDERSLER On TBLDERSLER.DERSID = TBLETUT.DERSID Inner Join TBLOGRETMEN On TBLOGRETMEN.OGRTID = TBLETUT.OGRETMENID", bgl);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        void ogretmenler()
        {
            SqlDataAdapter da = new SqlDataAdapter("Select OGRTID, (AD + ' ' + SOYAD) As 'İSİM' From TBLOGRETMEN", bgl);
            DataTable dt = new DataTable();
            da.Fill(dt);
            CmbDersAd.ValueMember = "OGRTID";
            CmbDersAd.DisplayMember = "İSİM";
            CmbDersAd.DataSource = dt;
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            derslistesi();
            etutlistesi();
        }

        private void CmbDers_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlDataAdapter da2 = new SqlDataAdapter("Select OGRTID, (AD + ' ' + SOYAD) As 'OGRETMEN' From TBLOGRETMEN Where BRANSID =" + CmbDers.SelectedValue, bgl);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            CmbOgretmen.ValueMember = "OGRTID";
            CmbOgretmen.DisplayMember = "OGRETMEN";
            CmbOgretmen.DataSource = dt2;
        }

        private void BtnEtutOlustur_Click(object sender, EventArgs e)
        {
            bgl.Open();
            SqlCommand cmd = new SqlCommand("Insert Into TBLETUT (DERSID, OGRETMENID, TARIH, SAAT) Values (@p1, @p2, @p3, @p4)", bgl);
            cmd.Parameters.AddWithValue("@p1", CmbDers.SelectedValue);
            cmd.Parameters.AddWithValue("@p2", CmbOgretmen.SelectedValue);
            cmd.Parameters.AddWithValue("@p3", MskTarih.Text);
            cmd.Parameters.AddWithValue("@p4", MskSaat.Text);
            cmd.ExecuteNonQuery();
            bgl.Close();
            MessageBox.Show("Etüt Oluşturuldu", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int deger = dataGridView1.SelectedCells[0].RowIndex;

            TxtEtutID.Text = dataGridView1.Rows[deger].Cells[0].Value.ToString();
        }

        private void BtnEtutVer_Click(object sender, EventArgs e)
        {
            bgl.Open();
            SqlCommand cmd = new SqlCommand("Update TBLETUT Set OGRENCIID = @p1, DURUM = @p2 Where ID = @p3", bgl);
            cmd.Parameters.AddWithValue("@p1", TxtOgrenciID.Text);
            cmd.Parameters.AddWithValue("@p2", "True");
            cmd.Parameters.AddWithValue("@p3", TxtEtutID.Text);
            cmd.ExecuteNonQuery();
            bgl.Close() ;
            MessageBox.Show("Etüt Öğrenciye Verildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            pictureBox1.ImageLocation = openFileDialog1.FileName;
        }

        private void BtnOgrenciEkle_Click(object sender, EventArgs e)
        {
            //Öğrenci Kayıt ve Mükerrer Kayıt Engelleme
            bgl.Open();
            SqlCommand ogrsorgu = new SqlCommand("Select * From TBLOGRENCI Where AD + ' ' + SOYAD = @p1", bgl);
            ogrsorgu.Parameters.AddWithValue("@p1", TxtAd.Text + " " + TxtSoyad.Text);
            SqlDataReader dr2 = ogrsorgu.ExecuteReader();
            if (dr2.Read())
            {
                MessageBox.Show("Öğrenci Sistemde Kayıtlı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                bgl.Open();
                SqlCommand cmd2 = new SqlCommand("Insert Into TBLOGRENCI (AD, SOYAD, FOTOGRAF, SINIF, TELEFON, MAIL) Values (@p1, @p2, @p3, @p4, @p5, @p6)", bgl);
                cmd2.Parameters.AddWithValue("@p1", TxtAd.Text);
                cmd2.Parameters.AddWithValue("@p2", TxtSoyad.Text);
                if (pictureBox1.ImageLocation == "")
                {
                    cmd2.Parameters.AddWithValue("@p3", "Kaydedilmedi!");
                }
                else
                {
                    cmd2.Parameters.AddWithValue("@p3", pictureBox1.ImageLocation);
                }
                cmd2.Parameters.AddWithValue("@p4", TxtSinif.Text);
                cmd2.Parameters.AddWithValue("@p5", MskTel.Text);
                cmd2.Parameters.AddWithValue("@p6", TxtMail.Text);
                cmd2.ExecuteNonQuery();
                bgl.Close();
                MessageBox.Show("Öğrenci Sisteme Kaydedildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            bgl.Close();
        }

        private void BtnDersEkle_Click(object sender, EventArgs e)
        {
            //Ders Ekleme ve Mükerrer Kayıt Engelleme
            bgl.Open();
            SqlCommand drsorgu = new SqlCommand("Select * From TBLDERSLER Where DERSAD = @p1", bgl);
            drsorgu.Parameters.AddWithValue("@p1", TxtDersAd.Text);
            SqlDataReader dr = drsorgu.ExecuteReader();
            if (dr.Read())
            {
                MessageBox.Show("Bu Ders Sistemde Kayıtlı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                bgl.Open();
                SqlCommand cmd = new SqlCommand("Insert Into TBLDERSLER (DERSAD) Values (@p1)", bgl);
                cmd.Parameters.AddWithValue("@p1", TxtDersAd.Text);
                cmd.ExecuteNonQuery();
                bgl.Close();
                MessageBox.Show("Ders Sisteme Kaydedildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dersler();
            }
            bgl.Close();           
        }

        private void BtnOgrtKaydet_Click(object sender, EventArgs e)
        {
            //Öğretmen Kayıt ve Mükerrer Kayıt Engelleme
            bgl.Open();
            SqlCommand ogrtsorgu = new SqlCommand("Select * From TBLOGRETMEN Where AD + ' ' + SOYAD = @p1", bgl);
            ogrtsorgu.Parameters.AddWithValue("@p1", TxtOgrtAd.Text + " " + TxtOgrtSoyad.Text);
            SqlDataReader dr3 = ogrtsorgu.ExecuteReader();
            if (dr3.Read())
            {
                MessageBox.Show("Öğretmen Sistemde Kayıtlı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                bgl.Open();
                SqlCommand cmd3 = new SqlCommand("Insert Into TBLOGRETMEN (AD, SOYAD, BRANSID) Values (@p1, @p2, @p3)", bgl);
                cmd3.Parameters.AddWithValue("@p1", TxtOgrtAd.Text);
                cmd3.Parameters.AddWithValue("@p2", TxtOgrtSoyad.Text);
                cmd3.Parameters.AddWithValue("@p3", CmbDersAd.SelectedValue);
                cmd3.ExecuteNonQuery();
                bgl.Close();
                MessageBox.Show("Öğretmen Sisteme Kaydedildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ogretmenler();
            }
            bgl.Close();
        }

        private void dataGridView1_CellStyleChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCellStyle stil = new DataGridViewCellStyle();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[5].Value) == true)
                {
                    stil.BackColor = Color.ForestGreen;
                    dataGridView1.Rows[i].DefaultCellStyle = stil;
                }
            }
        }
    }
}
