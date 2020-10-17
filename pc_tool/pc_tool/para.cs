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
using System.Runtime.InteropServices;
using System.Threading;




namespace pc_tool
{
    public partial class para : Form
    {

        ToolTip tips_refresh = new ToolTip();
        ToolTip tips_save = new ToolTip();
        ToolTip tips_load = new ToolTip();


        public para()
        {
            InitializeComponent();
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.Close();
        }




        private void InitTips()
        {

            tips_refresh.AutoPopDelay = 3000;
            tips_refresh.InitialDelay = 1000;
            tips_refresh.ReshowDelay = 500;
            tips_refresh.ShowAlways = true;
            tips_refresh.IsBalloon = true;
            tips_refresh.SetToolTip(this.button3, "读取MCU中储存数据");



            tips_save.AutoPopDelay = 3000;
            tips_save.InitialDelay = 1000;
            tips_save.ReshowDelay = 500;
            tips_save.ShowAlways = true;
            tips_save.IsBalloon = true;
            tips_save.SetToolTip(this.button7, "保存当前设定值到文件");



            tips_load.AutoPopDelay = 3000;
            tips_load.InitialDelay = 1000;
            tips_load.ReshowDelay = 500;
            tips_load.ShowAlways = true;
            tips_load.IsBalloon = true;
            tips_load.SetToolTip(this.button6, "从文件读取设定值");


        }








        private void para_Load(object sender, EventArgs e)
        {
            CommProtocol.SetTraceMode(CommProtocol.TRACE_MODE_PC);
           // Refresh data
            textBox1.Text = UploadData.Kp.ToString();
            textBox5.Text = UploadData.Ki.ToString();
            textBox8.Text = UploadData.Ke1.ToString();
            textBox11.Text = UploadData.Ke2.ToString();

            textBox17.Text = UploadData.maxCurrent.ToString();
            textBox14.Text = UploadData.maxDegree.ToString();

            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton3.Checked = false;

            if (UploadData.traceMode == CommProtocol.TRACE_MODE_LOG)
            {
                radioButton5.Checked = true;
            }
            else if (UploadData.traceMode == CommProtocol.TRACE_MODE_PC)
            {
                radioButton4.Checked = true;
            }
            else if (UploadData.traceMode == CommProtocol.TRACE_MODE_NONE)
            {
                radioButton3.Checked = true;
            }

            if (UploadData.run2zero > 0) checkBox2.Checked = true;
            if (UploadData.reversse > 0) checkBox1.Checked = true;


            InitTips();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MotorPara.traceMode = CommProtocol.TRACE_MODE_NONE;
            if (radioButton4.Checked) MotorPara.traceMode = CommProtocol.TRACE_MODE_PC;
            else if (radioButton5.Checked) MotorPara.traceMode = CommProtocol.TRACE_MODE_LOG;
            else if (radioButton3.Checked) MotorPara.traceMode = CommProtocol.TRACE_MODE_NONE;

            CommProtocol.SendCmd(CommProtocol.CMD_SET_TRACE_MODE);
           
        }
        /* read para from comm */
        private void button3_Click(object sender, EventArgs e)
        {

            DialogResult dr= MessageBox.Show("读取并覆盖当前参数，确认?","注意！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                textBox1.Text = UploadData.Kp.ToString();
                textBox5.Text = UploadData.Ki.ToString();
                textBox8.Text = UploadData.Ke1.ToString();
                textBox11.Text = UploadData.Ke2.ToString();

                textBox17.Text = UploadData.maxCurrent.ToString();
                textBox14.Text = UploadData.maxDegree.ToString();

                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton3.Checked = false;

                if (UploadData.traceMode == CommProtocol.TRACE_MODE_LOG) 
                {
                    radioButton5.Checked = true;
                }
                else  if (UploadData.traceMode == CommProtocol.TRACE_MODE_PC) 
                {
                    radioButton4.Checked = true;
                }
                else if (UploadData.traceMode == CommProtocol.TRACE_MODE_NONE)
                {
                    radioButton3.Checked = true;
                }

                if (UploadData.run2zero>0) checkBox2.Checked = true;
                if (UploadData.reversse>0) checkBox1.Checked = true;

            }
         else
             {      
            }           

        }

        // set control para
        private void button5_Click(object sender, EventArgs e)
        {
            MotorPara.maxCurrent = Convert.ToUInt16(textBox17.Text);
            MotorPara.maxDegree = Convert.ToByte(textBox14.Text);

            if (checkBox1.Checked) MotorPara.reversse = 1;
            else MotorPara.reversse = 0;

            if (checkBox2.Checked) MotorPara.run2zero = 1;
            else MotorPara.run2zero = 0;

            CommProtocol.SendCmd(CommProtocol.CMD_SET_CONTROL_PARA); 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MotorPara.Kp = Convert.ToUInt16(textBox1.Text);
            MotorPara.Ki = Convert.ToUInt16(textBox5.Text);
            MotorPara.Ke1 = Convert.ToUInt16(textBox8.Text);
            MotorPara.Ke2 = Convert.ToUInt16(textBox11.Text);
            CommProtocol.SendCmd(CommProtocol.CMD_SET_PID_PARA); 
        }

       // read para from file
        private void button6_Click(object sender, EventArgs e)
        {

        }
        // Save para to file
        private void button7_Click(object sender, EventArgs e)
        {

        }


    }

    /* Motor para we want to set */
    public static class MotorPara
    {
        private static string ParaFile = "motorpara.txt";
        private static UInt16 max_current = 0;
        private static Int32 zero_angle = 0;
        private static UInt16 p;
        private static UInt16 i;
        private static UInt16 ke1;
        private static UInt16 ke2;
        private static byte _reverse;
        private static byte _run2zero;
        private static byte _tracemode;
        private static byte _maxDegree;

        public static UInt16 maxCurrent
        {
            get { return max_current; }
            set { max_current = value; }
        }
        public static Int32 zeroAngle
        {
            get { return zero_angle; }
            set { zero_angle = value; }
        }
        public static byte reversse
        {
            get { return _reverse; }
            set { _reverse = value; }
        }
        public static byte run2zero
        {
            get { return _run2zero; }
            set { _run2zero = value; }
        }
        public static byte traceMode
        {
            get { return _tracemode; }
            set { _tracemode = value; }
        }
        public static byte maxDegree
        {
            get { return _maxDegree; }
            set { _maxDegree = value; }
        }


        public static UInt16 Kp
        {
            get { return p; }
            set { p = value; }
        }
        public static UInt16 Ki
        {
            get { return i; }
            set { i = value; }
        }
        public static UInt16 Ke1
        {
            get { return ke1; }
            set { ke1 = value; }
        }
        public static UInt16 Ke2
        {
            get { return ke2; }
            set { ke2 = value; }
        }




        public static void ParaRead()
        {
            BinaryReader br;

            if (File.Exists(ParaFile) == false)
            {
                max_current = 0;
                zero_angle = 0;
                p = 0;
                i = 0;
                ke1 = 0;
                ke2 = 0;
                _reverse = 0;
                _run2zero = 0;
                _tracemode = 0;
                _maxDegree = 0;
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

                max_current = br.ReadUInt16();
                zero_angle = br.ReadInt32();
                p = br.ReadUInt16();
                i = br.ReadUInt16();
                ke1 = br.ReadUInt16();
                ke2 = br.ReadUInt16();
                _reverse = br.ReadByte();
                _run2zero = br.ReadByte();
                _tracemode = br.ReadByte();
                _maxDegree = br.ReadByte();

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
                bw.Write(max_current);
                bw.Write(zero_angle);
                bw.Write(p);
                bw.Write(i);
                bw.Write(ke1);
                bw.Write(ke2);
                bw.Write(_reverse);
                bw.Write(_run2zero);
                bw.Write(_tracemode);
                bw.Write(_maxDegree);

            }
            catch (IOException e)
            {
                MessageBox.Show("Cannot write to alert para file, error:" + e.Message);
                return;
            }
            bw.Close();

        }





    }

    public static class UploadData
    {
        private static UInt16 max_current = 0;
        private static Int32 zero_angle = 0;
        private static UInt16 p;
        private static UInt16 i;
        private static UInt16 ke1;
        private static UInt16 ke2;
        private static byte _reverse;
        private static byte _run2zero;
        private static byte _tracemode;
        private static byte _maxDegree;
        private static Int16 _hall_x;
        private static Int16 _hall_y;
        private static Int32 _hall_angle;
        private static Int32 _max;
        private static Int32 _min;




        public static UInt16 maxCurrent
        {
            get { return max_current; }
            set { max_current = value; }
        }
        public static Int32 zeroAngle
        {
            get { return zero_angle; }
            set { zero_angle = value; }
        }
        public static byte reversse
        {
            get { return _reverse; }
            set { _reverse = value; }
        }
        public static byte run2zero
        {
            get { return _run2zero; }
            set { _run2zero = value; }
        }
        public static byte traceMode
        {
            get { return _tracemode; }
            set { _tracemode = value; }
        }
        public static byte maxDegree
        {
            get { return _maxDegree; }
            set { _maxDegree = value; }
        }
        public static UInt16 Kp
        {
            get { return p; }
            set { p = value; }
        }
        public static UInt16 Ki
        {
            get { return i; }
            set { i = value; }
        }
        public static UInt16 Ke1
        {
            get { return ke1; }
            set { ke1 = value; }
        }
        public static UInt16 Ke2
        {
            get { return ke2; }
            set { ke2 = value; }
        }

        public static Int16 hall_x
        {
            get { return _hall_x; }
            set { _hall_x = value; }
        }
        public static Int16 hall_y
        {
            get { return _hall_y; }
            set { _hall_y = value; }
        }
        public static Int32 hall_angle
        {
            get { return _hall_angle; }
            set { _hall_angle = value; }
        }

        public static Int32 max
        {
            get { return _max; }
            set { _max = value; }
        }

        public static Int32 min
        {
            get { return _min; }
            set { _min = value; }
        }
    }




#region struct_define

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct st_header
    {
        public UInt32 head;
        public UInt16 crc16;
        public byte cmd;
        public byte len;
    };

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct st_CommParaSet
    {
	    public st_header header;
        public UInt16 pid_p;
        public UInt16 pid_i;
        public UInt16 pid_ke1;
        public UInt16 pid_ke2;

        public byte reverse;
        public byte run2zero;
        public byte traceMode;
        public byte maxDegree;

        public UInt16 maxCurrent;
        public Int16 hall_x;
        public Int16 hall_y;

        public Int32 hall_angle;
        public Int32 hall_zero_angle;

    };

#endregion

    public static class CommProtocol
    {
        public const UInt32 CMD_HEAD = 0x12345678;

        public const UInt32 CMD_HEAD1 = 0x78;
        public const UInt32 CMD_HEAD2 = 0x56;
        public const UInt32 CMD_HEAD3 = 0x34;
        public const UInt32 CMD_HEAD4 = 0x12;

        public const byte CMD_SET_PARA = 0x01;
        public const byte CMD_READ_PARA = 0x02;

        public const byte CMD_SET_ZERO_ANGLE = 0x01;
        public const byte CMD_SET_PID_PARA = 0x02;
        public const byte CMD_SET_TRACE_MODE = 0x04;
        public const byte CMD_SET_CONTROL_PARA = 0x08;
        public const byte CMD_PERIOD_UPLOAD  = 0x10;
        public const byte CMD_SET_RESET_MCU = 0x20;

        public const byte TRACE_MODE_LOG = 0x01;
        public const byte TRACE_MODE_PC = 0x02;
        public const byte TRACE_MODE_NONE = 0x03;

        public static  byte frame_type ;


        private static st_CommParaSet commPara = new st_CommParaSet();

        private static byte[] FrameBuffer = new byte[Marshal.SizeOf(commPara)];


        public static Byte[] StructToBytes(Object structure)
        {
            Int32 size = Marshal.SizeOf(structure);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, buffer, false);
                Byte[] bytes = new Byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        //2、Byte[]转换为struct
        public static Object BytesToStruct(Byte[] bytes, Type strcutType)
        {
            Int32 size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.Copy(bytes, 0, buffer, size);

                return Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static void MakeFrame(byte Cmd)
        {
         //   st_CommParaSet commPara = new st_CommParaSet();

            commPara = (st_CommParaSet)BytesToStruct(FrameBuffer, commPara.GetType());
            commPara.header.head = CMD_HEAD;
            commPara.header.cmd = Cmd;

            commPara.hall_x = 0;
            commPara.hall_y = 0;
            commPara.hall_angle = 0;
            commPara.hall_zero_angle = 0;
    
            if ((Cmd & CMD_SET_ZERO_ANGLE) == CMD_SET_ZERO_ANGLE)
            {
                // Do nothing
            }
            else if ((Cmd & CMD_SET_PID_PARA) == CMD_SET_PID_PARA)
            {
                    commPara.pid_p = MotorPara.Kp;
                    commPara.pid_i = MotorPara.Ki;
                    commPara.pid_ke1 = MotorPara.Ke1;
                    commPara.pid_ke2 = MotorPara.Ke2;
            }
            else if ((Cmd & CMD_SET_CONTROL_PARA) == CMD_SET_CONTROL_PARA)
            {
                    commPara.maxDegree = MotorPara.maxDegree;
                    commPara.maxCurrent = MotorPara.maxCurrent;
                    commPara.reverse = MotorPara.reversse;
                    commPara.run2zero = MotorPara.run2zero;
            } 
            else if ((Cmd & CMD_SET_TRACE_MODE) == CMD_SET_TRACE_MODE)
            {
                    commPara.traceMode = MotorPara.traceMode;
            } 

            FrameBuffer = StructToBytes(commPara);
        }

        public static void SendCmd(byte Cmd)
        {
            MakeFrame(Cmd);
            CVar.GetComm().Write(FrameBuffer, 0, Marshal.SizeOf(commPara));
        }

        public static void SetTraceMode(byte mode)
        {
            MotorPara.traceMode = mode;
            SendCmd(CommProtocol.CMD_SET_TRACE_MODE);
        }




    }

    public static class FifoClass
    {
        public static  UInt32 buffersize = 1024*10;
        public static byte [] fifoBuffer = new byte [buffersize];
        public static UInt32 head, rear, len;

        public static void InitFifo()
        {
            head = rear = 0;
            len = buffersize;
        }


        public static int FifoIsFull()
        {
            if (head < rear)
            {
                if ((rear - head) == 1) return (1);
                else return (0);
            } 
            else
            {
                if (rear == 0 && head == (len-1))  return (1);
                else return (0);
            }  
        }

        public static int FifoIsEmpty()
        {
            if (head == rear) return (1);
            else return (0);
        }

        public static int FifoLen()
        {
            if (head >= rear) return (int)(head - rear);
            // head < rear
            else  return (int)(len - rear + head);
        }

        public static int FifoFreeSize()
        {
	        int ret;
	        ret = (int)(len - FifoLen());
	        if (ret == 0) return 0;
	        else return (ret - 1);
        }


        public static void EnFifo(byte dat)
        {
            fifoBuffer[head] = dat;
            head++;
            if (head >= len) 
               head = 0;
        }

        public static byte DeFifo()
        {
            byte tmp;
            tmp = fifoBuffer[rear];
            rear ++;
            if (rear >= len) 
                rear = 0;
            return tmp;
        }


        public static int CheckFifo(ref byte[] buffer, UInt16 len)
        {
	        UInt32 tmp,i;
	        tmp = rear;
	        for (i=0;i<len;i++) buffer[i] = DeFifo();
	        i = rear;
	        rear = tmp;
	        return (int)i;
        }

        public static void FifoClear()
        {
            head = rear;
        }


    }



}
