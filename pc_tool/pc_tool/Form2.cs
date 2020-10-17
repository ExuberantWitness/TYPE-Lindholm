using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace pc_tool
{
    public partial class Form2 : Form
    {
        public static SerialPort comm = CVar.GetComm();

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboPortName.Items.AddRange(ports);
            comboPortName.SelectedIndex = comboPortName.Items.Count > 0 ? 0 : -1;
            comboBaudrate.SelectedIndex = comboBaudrate.Items.IndexOf("2000000");
            buttonOpenClose.Text = "打开";
            //comboBaudrate.SelectedIndex = 0;
        }

        private void buttonOpenClose_Click(object sender, EventArgs e)
        {
            if (buttonOpenClose.Text == "关闭")
            {
                if (comm.IsOpen) comm.Close();
                buttonOpenClose.Text = "打开";
                return;
            }
            //根据当前串口对象，来判断操作
            if (comm == null)
            {
                //关闭时点击，则设置好端口，波特率后打开
                comm.PortName = comboPortName.Text;
                comm.BaudRate = int.Parse(comboBaudrate.Text);
                comm.DataBits = 8;
                comm.Parity = Parity.None;
                comm.StopBits = StopBits.One;
                comm.WriteBufferSize = 1024 * 1024 * 5;
                comm.ReadBufferSize = 1024 * 1024 * 5;
                try
                {
                    comm.Open();
                    MessageBox.Show("串口已打开");
                }
                catch (Exception ex)
                {
                    //捕获到异常信息，创建一个新的comm对象，之前的不能用了。
                    comm = new SerialPort();
                    //现实异常信息给客户。
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                if (comm.IsOpen)
                {

                    while (CVar.GetCommBusy()) Application.DoEvents();
                    //打开时点击，则关闭串口
                    comm.Close();
                    MessageBox.Show("串口已关闭");
                }
                else
                {
                                    
                    //关闭时点击，则设置好端口，波特率后打开
                    comm.PortName = comboPortName.Text;
                    comm.BaudRate = int.Parse(comboBaudrate.Text);
                    comm.DataBits = 8;
                    comm.Parity = Parity.Even;
                    comm.StopBits = StopBits.One;
                    comm.WriteBufferSize = 1024 * 1024 * 5;
                    comm.ReadBufferSize = 1024 * 1024 * 5;
                    try
                    {
                        comm.Open();
                        MessageBox.Show("串口已打开");
                    }
                    catch (Exception ex)
                    {
                        //捕获到异常信息，创建一个新的comm对象，之前的不能用了。
                        comm = new SerialPort();
                        //现实异常信息给客户。
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            //设置按钮的状态
            buttonOpenClose.Text = comm.IsOpen ?  "关闭" : "打开" ;

            if (comm.IsOpen)
            {
                CommPara.portnum = comm.PortName;
                CommPara.baudrate = comm.BaudRate;
                CommPara.ParaWrite();
                CVar.form1.button1.Text = CommPara.portnum.ToString() + "已打开";
                comm.DataReceived += CVar.form1.comm_DataReceived;
            }
            
 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            // Update Form1
        }

    }
}
