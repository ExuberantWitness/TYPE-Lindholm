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
using System.Runtime.InteropServices;
using System.Threading;

namespace pc_tool
{
    public partial class Form1 : Form
    {

       

        public static SerialPort comm = CVar.GetComm();
        public int received_count;

        Queue<byte> SerialFifo = new Queue<byte>();

        ToolTip tips1 = new ToolTip();
        ToolTip tips2 = new ToolTip();
        ToolTip tips3 = new ToolTip();


        public Form1()
        {
            InitializeComponent();
            CVar.form1 = this;
        }

        private void InitTips()
        {
            tips1.OwnerDraw = true;
            tips1.SetToolTip(button1,"设置提示的字体及颜色");
            tips1.Draw += new DrawToolTipEventHandler(toolTip1_Draw);
        }

        void toolTip1_Draw(object sender, DrawToolTipEventArgs e)
        {
           // throw new Exception("The method or operation is not implemented.");
            e.DrawBackground( );
    
            e.DrawBorder( );
            using (StringFormat sf = new StringFormat( ))
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                
                using (Font f = new Font("宋体", 12))
                {
                    //e.Graphics.DrawString(e.ToolTipText, f,SystemBrushes.ActiveCaptionText, e.Bounds, sf);
                    e.Graphics.DrawString(e.ToolTipText, f, SystemBrushes.FromSystemColor(Color.Blue), e.Bounds, sf);
                }
            }
        }
 

        private void openPort()
        {
            string str;
            //根据当前串口对象，来判断操作
  
                //关闭时点击，则设置好端口，波特率后打开
                comm.PortName = CommPara.portnum;
                comm.BaudRate = CommPara.baudrate;
                comm.DataBits = 8;
                comm.Parity = Parity.None;
                comm.StopBits = StopBits.One;
                comm.WriteBufferSize = 1024 * 1024 * 5;
                comm.ReadBufferSize = 1024 * 1024 * 5;
                try
                {
                    comm.Open();
                   // MessageBox.Show("串口已打开");
                }
                catch (Exception ex)
                {
                    //捕获到异常信息，创建一个新的comm对象，之前的不能用了。
                    comm = new SerialPort();
                    //现实异常信息给客户。
                    MessageBox.Show(ex.Message);
                }
            

            //设置按钮的状态
            str = comm.PortName + "已打开";
            button1.Text = comm.IsOpen ? str : "串口未打开";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        
            FifoClass.InitFifo();
            CommPara.ParaRead();
            openPort();
            if (comm.IsOpen) comm.DataReceived += comm_DataReceived;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 serialFrom = new Form2();

            CVar.CommBusy = true;


            if (CVar.comm.IsOpen)
            {
                CVar.comm.Close();
            }
            serialFrom.Show();
        }


        /* PID setting */
        private void button3_Click(object sender, EventArgs e)
        {
            para parasetting = new para();
            parasetting.Show();



        }

        private void button2_Click(object sender, EventArgs e)
        {
        

            st_CommParaSet commPara = new st_CommParaSet();
            byte[] buffer = new byte[Marshal.SizeOf(commPara)];

           // CommProtocol.MakeParaSet(ref buffer, CommProtocol.HALL_ANGLE);
           // comm.Write(buffer, 0, Marshal.SizeOf(commPara));
          
        }
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                Application.DoEvents();//可执行某无聊的操作
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comm.IsOpen)
            {
                CommProtocol.SendCmd(CommProtocol.CMD_SET_RESET_MCU);
            }

            Delay(500);

            comm.Close();

            this.Close();
        }


        public UInt16 GetFrame(ref byte[] buffer, ref byte type)
        {
            st_CommParaSet commPara = new st_CommParaSet();

            int length = Marshal.SizeOf(commPara);
            int i;
            while (true)
            {
                if (FifoClass.FifoLen() >= length)
                {
                    FifoClass.CheckFifo(ref buffer, 4);

                    if (buffer[0] == CommProtocol.CMD_HEAD1 && buffer[1] == CommProtocol.CMD_HEAD2 && buffer[2] == CommProtocol.CMD_HEAD3 && buffer[3] == CommProtocol.CMD_HEAD4)
                    {
                        {
                            for (i = 0; i < (length); i++)
                                buffer[i] = FifoClass.DeFifo();

                            commPara = (st_CommParaSet)CommProtocol.BytesToStruct(buffer, commPara.GetType());
                            return (UInt16)length;

                        }
                    }
                    else FifoClass.DeFifo();
                }
                else break;
            }
            return 0;
        }

        public void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int i;

            try
            {
                byte[] framebuffer = new byte[128];
                byte frametype = 0;
                int n = CVar.GetComm().BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                if (n == 0) return;

                byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
                received_count += n;//增加接收计数
                comm.Read(buf, 0, n);//读取缓冲数据


                //Add receive data to que
                for (i = 0; i < n; i++) FifoClass.EnFifo(buf[i]);


                if (GetFrame(ref framebuffer, ref frametype) > 0)
                {
                    st_CommParaSet commPara = new st_CommParaSet();
                    commPara = (st_CommParaSet)CommProtocol.BytesToStruct(framebuffer, commPara.GetType());

                        UploadData.hall_angle = commPara.hall_angle;
                        UploadData.zeroAngle = commPara.hall_zero_angle;
                        UploadData.hall_x = commPara.hall_x;
                        UploadData.hall_y = commPara.hall_y;

                        UploadData.maxDegree = commPara.maxDegree;
                        UploadData.maxCurrent = commPara.maxCurrent;
                        UploadData.Kp = commPara.pid_p;
                        UploadData.Ki = commPara.pid_i;
                        UploadData.Ke1 = commPara.pid_ke1;
                        UploadData.Ke2 = commPara.pid_ke2;


                        UploadData.reversse = commPara.reverse;
                        UploadData.run2zero = commPara.run2zero;
                        UploadData.traceMode = commPara.traceMode;
          
                }
            }
            finally
            {


            }
        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            hall hallsetting = new hall();
            hallsetting.Show();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            about myabout = new about();
            myabout.Show();
        }

    }



    #region my_class
    public static class CVar
    {
        public static SerialPort comm = new SerialPort();
        public static bool CommBusy = false;

        public static Form1 _form1;

        public static Form1 form1
        {
            get { return _form1; }
            set { _form1 = value; }
        }



  

        public static void SetComm(SerialPort commx)
        {
            comm = commx;
        }
        public static SerialPort GetComm()
        {
            return comm;
        }
        public static void SetCommBusy(bool busy)
        {
            CommBusy = busy;
        }
        public static bool GetCommBusy()
        {
            return CommBusy;
        }
    }



    public static class CommPara
    {

        private static int baud_rate = 0 ;
        private static string port_num = null ;


        public const string ParaFile = "commpara.txt";


        public static int baudrate
        {
            get { return baud_rate; }
            set { baud_rate = value; }
        }

        public static string portnum
        {
            get { return port_num; }
            set { port_num = value; }
        }

        public static void ParaRead()
        {
            BinaryReader br;

            if (File.Exists(ParaFile) == false)
            {
                baud_rate = 2000000;
                port_num = "COM1";
                return;
            }

            // 创建文件
            try
            {
                br = new BinaryReader(new FileStream(ParaFile,
                FileMode.Open));
            }
            catch (IOException e)
            {
                MessageBox.Show("Cannot create file, error:" + e.Message);
                //  Console.WriteLine(e.Message + "\n Cannot create file.");
                return;
            }
            // 读取文件
            try
            {
                baud_rate = br.ReadInt32();
                port_num = br.ReadString();
            }
            catch (IOException e)
            {
                MessageBox.Show("Cannot read  para file, error: " + e.Message);
            }
            br.Close();



        }
        public static void ParaWrite()
        {
            BinaryWriter bw;

            // 创建文件
            try
            {
                bw = new BinaryWriter(new FileStream(ParaFile,
                 FileMode.OpenOrCreate));

            }
            catch (IOException e)
            {
                MessageBox.Show("Cannot create file, error:" + e.Message);
                //  Console.WriteLine(e.Message + "\n Cannot create file.");
                return;
            }
            // 写入文件
            try
            {
                bw.Write(baud_rate);
                bw.Write(port_num);
  
            }
            catch (IOException e)
            {
                MessageBox.Show("Cannot write to alert para file, error:" + e.Message);
                return;
            }
            bw.Close();

        }
    }

#endregion



}
