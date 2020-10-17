using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pc_tool
{
    public partial class hall : Form
    {
        ToolTip tips1 = new ToolTip();
        System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        int max=0;
        int x_max=0, y_max=0;

        public hall()
        {
            InitializeComponent();

        }


        private void InitTimer()
        {
            myTimer.Tick += new EventHandler(TimerUpdate);
            myTimer.Enabled = true;
            myTimer.Interval = 200;
        }

        private void TimerUpdate(object sender, EventArgs e)
        {
            int mag;
            float a1,a2,a3;
            a1 = UploadData.hall_angle*90/16384.0f;
      
            textBox1.Text = a1.ToString("F1");
            textBox2.Text = UploadData.hall_x.ToString();
            textBox3.Text = UploadData.hall_y.ToString();
            a2 = UploadData.zeroAngle*90/16384.0f;
            textBox4.Text = a2.ToString("F1");

            mag = UploadData.hall_x * UploadData.hall_x + UploadData.hall_y * UploadData.hall_y;

            mag = (int)Math.Sqrt(mag);

            if (mag > max) max = mag;


            textBox5.Text = mag.ToString();
            textBox6.Text = max.ToString();

            a3 = a1 + a2;
            textBox9.Text = a3.ToString("F1");

            if (Math.Abs(UploadData.hall_x) > x_max) x_max = Math.Abs(UploadData.hall_x);
            if (Math.Abs(UploadData.hall_y) > y_max) y_max = Math.Abs(UploadData.hall_y);

            textBox10.Text = x_max.ToString();
            textBox12.Text = y_max.ToString();



        }



        private void InitTips()
        {
           
            tips1.AutoPopDelay = 3000;
            tips1.InitialDelay = 1000;
            tips1.ReshowDelay = 500;
            tips1.ShowAlways = true;
            tips1.IsBalloon = true;

            tips1.SetToolTip(this.button6, "将当前霍尔角度设为0度");
        }

        private void hall_Load(object sender, EventArgs e)
        {
            UploadData.max = 0;
            UploadData.min = 16384 * 10;
            InitTips();
            CommProtocol.SetTraceMode(CommProtocol.TRACE_MODE_PC);
            InitTimer();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CommProtocol.SendCmd(CommProtocol.CMD_SET_ZERO_ANGLE);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            max = 0;
            x_max = 0;
            y_max = 0;
        }
    }
}
