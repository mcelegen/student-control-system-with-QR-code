using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace qrKodoluşturma
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\qrKodoluşturma\\qrKodoluşturma\\ogrenciVeriTabani.accdb"); // Global bağlantı yolu belirledik.
        private void Form1_Load(object sender, EventArgs e)
        {
            /*Uygulama yüklendiğinde veri tabanında kayıtlı öğrenci gelsin*/
            baglan.Open(); 
            DataTable tablo = new DataTable();
            OleDbDataAdapter adaptor = new OleDbDataAdapter("Select * from ogrenciBilgi", baglan);
            adaptor.Fill(tablo);
            dataGridView1.DataSource = tablo;
            baglan.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*Eğer textBox1, textBox2, textBox3'te veriler hazırsa çalışsın değilse uyarı versin. */
            if (textBox1.Text !=""  && textBox2.Text != "" && textBox3.Text != "")
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode("Öğrencinin Adı Soyadı:" + Environment.NewLine + textBox2.Text + " " + textBox3.Text + Environment.NewLine + Environment.NewLine + "Öğrencini Numarası:" + Environment.NewLine + textBox1.Text, QRCodeGenerator.ECCLevel.Q); //Environment.NewLine  alt satıra geçer
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                pictureBox1.Image = qrCodeImage;
            }
            else
            {
                MessageBox.Show("Öğrenci Adı Soyadı ve Numara Alanları Boş Geçilemez.");
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {           
            /*baglan.Open();
            OleDbCommand komut = new OleDbCommand("Select * From ogrenciBilgi", baglan);
            OleDbDataReader dr = komut.ExecuteReader();
            if (dr.Read())
            {
                
            }
            baglan.Close();*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*Öğrenci numarasına göre veri tabanından kayıt getirip datagridview2 aktardım.*/
            if (textBox7.Text=="")
            {
                MessageBox.Show("ARAMA ALANI BOŞ.");
            }
            else
            {
                baglan.Open();
                DataSet ds = new DataSet();
                OleDbDataAdapter adtr = new OleDbDataAdapter("Select * From ogrenciBilgi where  OGRENCINUMARASI like '" + textBox7.Text + "%' order by OGRENCINUMARASI", baglan);
                adtr.Fill(ds, "veriler");
                dataGridView2.DataSource = ds.Tables["veriler"];
                dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                baglan.Close();
            }
            
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            /*dataGridView2'ye aktardığımız öğrenci bilgisini kayıt oluşturmak için textboxlara gönderdim.*/
            dataGridView2.CurrentRow.Selected = true;
            textBox1.Text = dataGridView2.Rows[e.RowIndex].Cells["OGRENCINUMARASI"].Value.ToString();
            textBox2.Text = dataGridView2.Rows[e.RowIndex].Cells["ADI"].Value.ToString();
            textBox3.Text = dataGridView2.Rows[e.RowIndex].Cells["SOYADI"].Value.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            /*Eğer textBox1, textBox2, textBox3'te veriler hazırsa yani boş değilse QR kodu oluşturduk dedik değilse uyarı verdirdim.*/
            if (textBox1.Text =="" && textBox2.Text =="" && textBox3.Text=="")
            {
                MessageBox.Show("İlgili Alanlar Boş Geçilemez.");
            }
            else if (pictureBox1.Image == null)
            {
                MessageBox.Show("QR Kod Oluşturulmadan Kimlik Basamazsınız.");
            }
            else
            {
                printPreviewDialog1.ShowDialog();
            }
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            /* printDocument1 de yazdırma alanı oluşturdum. */
            try
            {
                Font font = new Font("Arial", 14);
                SolidBrush firca = new SolidBrush(Color.Black);
                Pen kalem = new Pen(Color.Black);
                font = new Font("arial", 15, FontStyle.Bold);
                e.Graphics.DrawString("ÖĞRENCİ KİMLİK KARTI", font, firca, 350, 75); //ORTALI


                font = new Font("arial", 12, FontStyle.Bold);
                e.Graphics.DrawString("ADI:", font, firca, 50, 150);
                e.Graphics.DrawString("SOYADI:", font, firca, 50, 200);
                e.Graphics.DrawString("ÖĞR.NO:", font, firca, 50, 250);
                
                e.Graphics.DrawLine(kalem, 50, 140, 780, 140); 
                e.Graphics.DrawLine(kalem, 50, 450, 50, 140);
                e.Graphics.DrawLine(kalem, 50, 450, 780, 450);
                e.Graphics.DrawLine(kalem, 780, 140, 780, 450);

                

                Bitmap resimQR = new Bitmap(pictureBox1.Image,125,125); // yeniden boyutlandırmak için bitmap e aktardım.
                Bitmap resimKimlik = new Bitmap(pictureBox2.Image, 100, 100); // yeniden boyutlandırmak için bitmap e aktardım.

                font = new Font("arial",12);
                e.Graphics.DrawString(textBox2.Text, font, firca, 130, 150);
                e.Graphics.DrawString(textBox3.Text, font, firca, 130, 200);
                e.Graphics.DrawString(textBox1.Text, font, firca, 130, 250);
                e.Graphics.DrawImage(resimQR, 350, 300);
                e.Graphics.DrawImage(resimKimlik, 650, 160);

            }
            catch (Exception)
            {

                MessageBox.Show("Alanlar Boş Geçilemez.");
            }
            
        }
    }
}
