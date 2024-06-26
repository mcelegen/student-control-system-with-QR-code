using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;

namespace qrKodOkuma
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        FilterInfoCollection fic; 
        VideoCaptureDevice vcd;

        /* form yüklendiğinde combox içerisine bilgisayarıma bağlı kameraları listelettim. */
        private void Form1_Load(object sender, EventArgs e)
        {
            fic = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in fic)
                comboBox1.Items.Add(filterInfo.Name);
            comboBox1.SelectedIndex = 0;
        }

        /* butona tıkladığımızda combobox içerisinde seçmiş olduğumuz camerayı başlatıp bitene kadar butonu etkisiz hale getirdik, okumayı bitir butonunu etkinleştirdik. */
        private void button1_Click(object sender, EventArgs e)
        {
            vcd = new VideoCaptureDevice(fic[comboBox1.SelectedIndex].MonikerString);
            vcd.NewFrame += CaptureDevice_NewFrama;
            vcd.Start();
            timer1.Start();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        /* butona tıkladığımızda kameraları durdurup okumayı sonlandırdım. */
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
                {
                    vcd.Stop();
                    pictureBox1.Image = null;
                    button1.Enabled = true;
                    button2.Enabled = false;
                    textBox1.Text = "";
                }
            
        }

        /*picturebox içerisine görüntü aktardım*/
        private void CaptureDevice_NewFrama(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        /*form kapanınca video çalıştırma eylemeni kapatıyorum.*/
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (vcd.IsRunning)
                vcd.Stop();
        }

        /* picture box içerisindeki görüntüyi stringe çevirip textboxa gönderiyoruz okuma işlemi. */
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                BarcodeReader okuyucu = new BarcodeReader();
                Result sonuc = okuyucu.Decode((Bitmap)pictureBox1.Image);
                if (sonuc !=null)
                {
                    textBox1.Text = sonuc.ToString();
                    timer1.Stop();
                    if (vcd.IsRunning)
                        vcd.Stop();
                }
            }
        }
    }
}
