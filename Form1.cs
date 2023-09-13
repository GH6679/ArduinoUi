using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//추가
using System.Net.Http;
using System.Net;
using System.Threading;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //-----------------------------------------------------------
        //백그라운드 메세지 수신 스레드 참조 변수
        //----------------------------------------------------------
        private BackgroundWorker recvLedWorker;
        private BackgroundWorker recvTmpWorker;
        private BackgroundWorker recvLightWorker;
        private BackgroundWorker recvdisWorker;


        private void conn_btn_Click(object sender, EventArgs e)
        {
            String port = this.comboBox1.Items[this.comboBox1.SelectedIndex].ToString();
            Console.WriteLine("PORT : " + port);
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try { 
            request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:8080/arduino/conn/" + port);
            request.Method = "GET";
            request.ContentType = "application/json";
            //request.Timeout = 30*1000;

            response = (HttpWebResponse)request.GetResponse();
            
            if(response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("RESPONSE CODE : " + response.StatusCode);
                    //-------------------------------------------------------
                    //수신 스레드 객체 생성 & 스레드 등록
                    //--------------------------------------------------------
                    //LED
                    recvLedWorker = new BackgroundWorker();
                    recvLedWorker.DoWork += recvLedInfo;

                    //TMP
                    recvTmpWorker = new BackgroundWorker();
                    recvTmpWorker.DoWork += recvTmpInfo;

                    //LIGTH
                    recvLightWorker = new BackgroundWorker();
                    recvLightWorker.DoWork += recvLightInfo;

                    //DIS
                    recvdisWorker = new BackgroundWorker();
                    recvdisWorker.DoWork += recvdisInfo;


                    //수신 쓰레드 실행
                    recvLedWorker.RunWorkerAsync();
                    recvTmpWorker.RunWorkerAsync();
                    recvLightWorker.RunWorkerAsync();
                    recvdisWorker.RunWorkerAsync();

                }
            }catch(Exception ex)
            {
                Console.WriteLine("EX : " + ex);
            }
        }

        private void recvLedInfo(object sender,DoWorkEventArgs e)
        {
            while (!recvLedWorker.CancellationPending)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:8080/arduino/ui/led");
                request.Method = "GET";
                request.ContentType = "application/json";
                //request.Timeout = 30*1000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);

                Invoke(new Action(() =>
                {
                    this.led_text.Text = sr.ReadToEnd();
                }));



                Thread.Sleep(1000);
            }
        }

        private void recvTmpInfo(object sender, DoWorkEventArgs e)
        {
            while (!recvTmpWorker.CancellationPending)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:8080/arduino/ui/tmp");
                request.Method = "GET";
                request.ContentType = "application/json";
                //request.Timeout = 30*1000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);

                Invoke(new Action(() =>
                {
                    this.tmp_text.Text = sr.ReadToEnd();
                }));



                Thread.Sleep(1000);
            }
        }

        private void recvLightInfo(object sender, DoWorkEventArgs e)
        {
            while (!recvLightWorker.CancellationPending)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:8080/arduino/ui/light");
                request.Method = "GET";
                request.ContentType = "application/json";
                //request.Timeout = 30*1000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);

                Invoke(new Action(() =>
                {
                    this.light_text.Text = sr.ReadToEnd();
                }));



                Thread.Sleep(1000);
            }
        }

        private void recvdisInfo(object sender, DoWorkEventArgs e)
        {
            while (!recvdisWorker.CancellationPending)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:8080/arduino/ui/dis");
                request.Method = "GET";
                request.ContentType = "application/json";
                //request.Timeout = 30*1000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);

                Invoke(new Action(() =>
                {
                    this.dis_text.Text = sr.ReadToEnd();
                }));



                Thread.Sleep(1000);
            }
        }


        private void led_on_btn_Click(object sender, EventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:8080/arduino/led/1");
            request.Method = "GET";
            request.ContentType = "application/json";
            //request.Timeout = 30*1000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }

        private void led_off_btn_Click(object sender, EventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:8080/arduino/led/0");
            request.Method = "GET";
            request.ContentType = "application/json";
            //request.Timeout = 30*1000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }
    }
}
