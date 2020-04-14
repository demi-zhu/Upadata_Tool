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
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;

namespace IAP_Demo
{
    public partial class IAP_Upgrade : Form
    {
        public class FILEPARTINFO
        {
            public uint PartStartAddress;
            public int PartEndAddress;
            public int PartSize;
            public int PartChecksum;
            public byte[] PartData;
        }

        SerialPort sp = new SerialPort();

        delegate void dShowForm();
        ProgressBarForm frm_Progress = new ProgressBarForm();

        public static object lockIniDevice= new object();
        public bool m_InitFlag = false;

        public int m_PortNameSelectUSB = 0;
        public int m_PortNameSelectUART = 0;

        Thread newth = null;

        uint SPIFSize = 0;
        uint SPIFSectorSize = 0;
        uint SPIFPageSize = 0;

        public Hid oSp = new Hid();
        ushort uPID = 0xAF01;
        ushort uVID = 0x2E3C;

        public const byte USBCOMMOND_MSB = 0x5A;
        public const byte USBCOMMOND_IAPMODE = 0xA0;
        public const byte USBCOMMOND_BEGINDOWN = 0xA1;
        public const byte USBCOMMOND_ADDRESS= 0xA2;
        public const byte USBCOMMOND_SENDDATA = 0xA3;
        public const byte USBCOMMOND_SENDEND = 0xA4;
        public const byte USBCOMMOND_CRC = 0xA5;
        public const byte USBCOMMOND_JUMPAPP = 0xA6;
        public const byte USBCOMMOND_GETAPPADDR = 0xA7;
        public const byte USBCOMMOND_GETSPIFSIZE = 0xA8;
        public const byte USBCOMMOND_SETSPIFADDRESS= 0xA9;
        public const byte USBCOMMOND_SPIFSENDDATA = 0xAA;
        public const byte USBCOMMOND_SPIFCRC = 0xAB;

        public const byte USBCOMMOND_ACK_MSB = 0xFF;
        public const byte USBCOMMOND_ACK_LSB = 0x00;
        public const byte USBCOMMOND_NACK_MSB = 0x00;
        public const byte USBCOMMOND_NACK_LSB = 0xFF;

        public const int USB_PART_LENGTH = 1024;

        static public uint[] crc32table = { 0x00000000, 0x04c11db7, 0x09823b6e, 0x0d4326d9, 0x130476dc, 0x17c56b6b,
                                     0x1a864db2, 0x1e475005, 0x2608edb8, 0x22c9f00f, 0x2f8ad6d6, 0x2b4bcb61,
                                     0x350c9b64, 0x31cd86d3, 0x3c8ea00a, 0x384fbdbd, 0x4c11db70, 0x48d0c6c7,
                                     0x4593e01e, 0x4152fda9, 0x5f15adac, 0x5bd4b01b, 0x569796c2, 0x52568b75,
                                     0x6a1936c8, 0x6ed82b7f, 0x639b0da6, 0x675a1011, 0x791d4014, 0x7ddc5da3,
                                     0x709f7b7a, 0x745e66cd, 0x9823b6e0, 0x9ce2ab57, 0x91a18d8e, 0x95609039,
                                     0x8b27c03c, 0x8fe6dd8b, 0x82a5fb52, 0x8664e6e5, 0xbe2b5b58, 0xbaea46ef,
                                     0xb7a96036, 0xb3687d81, 0xad2f2d84, 0xa9ee3033, 0xa4ad16ea, 0xa06c0b5d,
                                     0xd4326d90, 0xd0f37027, 0xddb056fe, 0xd9714b49, 0xc7361b4c, 0xc3f706fb,
                                     0xceb42022, 0xca753d95, 0xf23a8028, 0xf6fb9d9f, 0xfbb8bb46, 0xff79a6f1,
                                     0xe13ef6f4, 0xe5ffeb43, 0xe8bccd9a, 0xec7dd02d, 0x34867077, 0x30476dc0,
                                     0x3d044b19, 0x39c556ae, 0x278206ab, 0x23431b1c, 0x2e003dc5, 0x2ac12072,
                                     0x128e9dcf, 0x164f8078, 0x1b0ca6a1, 0x1fcdbb16, 0x018aeb13, 0x054bf6a4,
                                     0x0808d07d, 0x0cc9cdca, 0x7897ab07, 0x7c56b6b0, 0x71159069, 0x75d48dde,
                                     0x6b93dddb, 0x6f52c06c, 0x6211e6b5, 0x66d0fb02, 0x5e9f46bf, 0x5a5e5b08,
                                     0x571d7dd1, 0x53dc6066, 0x4d9b3063, 0x495a2dd4, 0x44190b0d, 0x40d816ba,
                                     0xaca5c697, 0xa864db20, 0xa527fdf9, 0xa1e6e04e, 0xbfa1b04b, 0xbb60adfc,
                                     0xb6238b25, 0xb2e29692, 0x8aad2b2f, 0x8e6c3698, 0x832f1041, 0x87ee0df6,
                                     0x99a95df3, 0x9d684044, 0x902b669d, 0x94ea7b2a, 0xe0b41de7, 0xe4750050,
                                     0xe9362689, 0xedf73b3e, 0xf3b06b3b, 0xf771768c, 0xfa325055, 0xfef34de2,
                                     0xc6bcf05f, 0xc27dede8, 0xcf3ecb31, 0xcbffd686, 0xd5b88683, 0xd1799b34,
                                     0xdc3abded, 0xd8fba05a, 0x690ce0ee, 0x6dcdfd59, 0x608edb80, 0x644fc637,
                                     0x7a089632, 0x7ec98b85, 0x738aad5c, 0x774bb0eb, 0x4f040d56, 0x4bc510e1,
                                     0x46863638, 0x42472b8f, 0x5c007b8a, 0x58c1663d, 0x558240e4, 0x51435d53,
                                     0x251d3b9e, 0x21dc2629, 0x2c9f00f0, 0x285e1d47, 0x36194d42, 0x32d850f5,
                                     0x3f9b762c, 0x3b5a6b9b, 0x0315d626, 0x07d4cb91, 0x0a97ed48, 0x0e56f0ff,
                                     0x1011a0fa, 0x14d0bd4d, 0x19939b94, 0x1d528623, 0xf12f560e, 0xf5ee4bb9,
                                     0xf8ad6d60, 0xfc6c70d7, 0xe22b20d2, 0xe6ea3d65, 0xeba91bbc, 0xef68060b,
                                     0xd727bbb6, 0xd3e6a601, 0xdea580d8, 0xda649d6f, 0xc423cd6a, 0xc0e2d0dd,
                                     0xcda1f604, 0xc960ebb3, 0xbd3e8d7e, 0xb9ff90c9, 0xb4bcb610, 0xb07daba7,
                                     0xae3afba2, 0xaafbe615, 0xa7b8c0cc, 0xa379dd7b, 0x9b3660c6, 0x9ff77d71,
                                     0x92b45ba8, 0x9675461f, 0x8832161a, 0x8cf30bad, 0x81b02d74, 0x857130c3,
                                     0x5d8a9099, 0x594b8d2e, 0x5408abf7, 0x50c9b640, 0x4e8ee645, 0x4a4ffbf2,
                                     0x470cdd2b, 0x43cdc09c, 0x7b827d21, 0x7f436096, 0x7200464f, 0x76c15bf8,
                                     0x68860bfd, 0x6c47164a, 0x61043093, 0x65c52d24, 0x119b4be9, 0x155a565e,
                                     0x18197087, 0x1cd86d30, 0x029f3d35, 0x065e2082, 0x0b1d065b, 0x0fdc1bec,
                                     0x3793a651, 0x3352bbe6, 0x3e119d3f, 0x3ad08088, 0x2497d08d, 0x2056cd3a,
                                     0x2d15ebe3, 0x29d4f654, 0xc5a92679, 0xc1683bce, 0xcc2b1d17, 0xc8ea00a0,
                                     0xd6ad50a5, 0xd26c4d12, 0xdf2f6bcb, 0xdbee767c, 0xe3a1cbc1, 0xe760d676,
                                     0xea23f0af, 0xeee2ed18, 0xf0a5bd1d, 0xf464a0aa, 0xf9278673, 0xfde69bc4,
                                     0x89b8fd09, 0x8d79e0be, 0x803ac667, 0x84fbdbd0, 0x9abc8bd5, 0x9e7d9662,
                                     0x933eb0bb, 0x97ffad0c, 0xafb010b1, 0xab710d06, 0xa6322bdf, 0xa2f33668,
                                     0xbcb4666d, 0xb8757bda, 0xb5365d03, 0xb1f740b4
                                     };

        void ShowForm()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new dShowForm(this.ShowForm));
            }
            else
            {
                frm_Progress.ShowDialog(this);
            }
        }
        public IAP_Upgrade()
        {
            InitializeComponent();
            comboBox_PortType.SelectedIndex = 1;
            //InitRS232PortName();
            comboBox_AppSTAddress.SelectedIndex = 0;
            checkBox_CRC.Checked = true;
            m_InitFlag = true;

            Control.CheckForIllegalCrossThreadCalls = false;
            newth = new System.Threading.Thread(new System.Threading.ThreadStart(InitDeviceThread));
            newth.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            newth.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            newth.Start();
        }

        private void InitDeviceThread()
        {
            while(m_InitFlag)
            {
                InitDevice();
                Thread.Sleep(1000);
            }
        }
        private void buttonOpenfile1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Files(*.bin,*.hex)|*.bin;*.hex|IntelBIN Files(*.bin)|*.bin|Hes Files(*.hex)|*.hex|All FIles(*.*)|*.*";
            ofd.Title = "打开文件";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string temp3 = ofd.FileName.Substring(ofd.FileName.LastIndexOf(@"\"));
                string tempType = temp3.ToUpper();

                if (tempType.Substring(tempType.Length - 3, 3) == "HEX")
                {
                    comboBox_AppSTAddress.SelectedIndex = -1;
                    comboBox_AppSTAddress.Enabled = false;
                    label1.Enabled = false;
                }
                else if (tempType.Substring(tempType.Length - 3, 3) == "BIN")
                {
                    //comboBox_AppSTAddress.SelectedIndex = 0;
                    comboBox_AppSTAddress.Enabled = true;
                    label1.Enabled = true;
                }
                else
                {
                    MessageBox.Show("The download file format error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, 0);
                    return;
                }
                textBoxFile1Path.Text = ofd.FileName;
            }

        }

        private void buttonBeginTest_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxFile1Path.Text))
            {
                MessageBox.Show("Can't open the download file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox_PortType.SelectedIndex == 0)
            {
                if (!OpenSerialPort())
                {
                    MessageBox.Show("Open RS232 port error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Control.CheckForIllegalCrossThreadCalls = false;
                Thread newth = new System.Threading.Thread(new System.Threading.ThreadStart(DownloadThreadRS232));
                newth.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                newth.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                newth.Start();
            }
            else
            {
                if (string.IsNullOrEmpty(comboBoxPortName.Text))
                {
                    MessageBox.Show("No USB IAP device!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Control.CheckForIllegalCrossThreadCalls = false;
                Thread newth = new System.Threading.Thread(new System.Threading.ThreadStart(DownloadThreadUSB));
                newth.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                newth.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                newth.Start();
            }
        }

        private void DownloadThreadUSB()
        {
            lock (lockIniDevice)
            {
                List<FILEPARTINFO> FIleInfo = null;

                new System.Threading.Thread(new System.Threading.ThreadStart(ShowForm)).Start();
                frm_Progress.SetOprationInfo("Downloading.......");
                frm_Progress.SetProgress(100, 0);

                uint StartAddr = 0;
                if (!GetFileInfo(out StartAddr, out FIleInfo))
                {
                    frm_Progress.CloseBar();
                    return;
                }

                Hid.HID_RETURN rv = oSp.OpenDevice(uVID, uPID, comboBoxPortName.Text);
                if (rv != Hid.HID_RETURN.SUCCESS)
                {
                    MessageBox.Show("Can't open the usb device", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    frm_Progress.CloseBar();
                    return;
                }

                //Enter IAP Mode
                if (!EnterIAPModeUSB())
                {
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }
                Thread.Sleep(2000);

                //check download address
                uint fileStartAddr = StartAddr + FIleInfo[0].PartStartAddress;
                if (!GetAppAddrFromIAP(fileStartAddr))
                {
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }

                //Begin download
                if (!BeginDownUSB())
                {
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }

                frm_Progress.SetProgress(100, 5);

                //send file data
                if (!DownloadDataToDeviceUSB(StartAddr, FIleInfo))
                {
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }

                //send download end
                if (!DownloadEndUSB())
                {
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }

                //CRC Check
                if (checkBox_CRC.Checked)
                {
                    frm_Progress.SetOprationInfo("CRC Verify.......");

                    if (!CRCVerifyUSB(StartAddr, FIleInfo))
                    {
                        frm_Progress.CloseBar();
                        oSp.CloseDevice();
                        return;
                    }
                }

                //send jump
                if (!JumpToAppUSB())
                {
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }

                frm_Progress.SetProgress(100, 100);
                if (checkBox_CRC.Checked)
                {
                    MessageBox.Show("Download and CRC Verify succeed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
                else
                {
                    MessageBox.Show("Download succeed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }

                oSp.CloseDevice();
            }
        }

        private bool GetFileInfo(out uint StartAddr, out List<FILEPARTINFO>  FIleInfo)
        {
            StartAddr = 0;
            FIleInfo = null;
            string temp3 = textBoxFile1Path.Text.Substring(textBoxFile1Path.Text.LastIndexOf(@"\"));
            string tempType = temp3.ToUpper();

            if (tempType.Substring(tempType.Length - 3, 3) == "HEX")
            {
                FIleInfo = AddValueHex(textBoxFile1Path.Text);
                StartAddr = 0;
            }
            else if (tempType.Substring(tempType.Length - 3, 3) == "BIN")
            {
                FIleInfo = AddValueBin(textBoxFile1Path.Text);
        
                    StartAddr = 0x08008000;
           
            }
            else
            {
                MessageBox.Show("The download file format error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                frm_Progress.CloseBar();
                return false; 
            }

            if (FIleInfo == null)
            {
                MessageBox.Show("The download file format error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                frm_Progress.CloseBar();
                return false; 
            }
            if (FIleInfo.Count == 0)
            {
                MessageBox.Show("The download file format error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                frm_Progress.CloseBar();
                return false;
            }
            return true;
        }

        private bool GetAppAddrFromIAP(uint fileAddr)
        {
            byte[] sendbuff = new byte[2];
            byte[] recvbuff = new byte[8];
            sendbuff[0] = USBCOMMOND_MSB;
            sendbuff[1] = USBCOMMOND_GETAPPADDR;
            if (!PortCommunicationUSB(sendbuff, recvbuff))
            {
                MessageBox.Show("USB port communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return false;
            }
            else
            {
                if ((recvbuff[0] != USBCOMMOND_MSB) || (recvbuff[1] != USBCOMMOND_GETAPPADDR) || (recvbuff[2] != USBCOMMOND_ACK_MSB) || (recvbuff[3] != USBCOMMOND_ACK_LSB))
                {
                    MessageBox.Show("USB device receive NACK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return false;
                }
                else
                {
                    uint appAddr = ((uint)recvbuff[4] << 24) + ((uint)recvbuff[5] << 16) + ((uint)recvbuff[6] << 8) + (uint)recvbuff[7];
                    if (fileAddr != appAddr)
                    {
                        MessageBox.Show("File download address is different from the APP address of IAP", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                      MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        return false;
                    }
                }
            }
            return true;
        }


        private bool EnterIAPModeUSB()
        {
            byte[] sendbuff = new byte[2];
            byte[] recvbuff = new byte[4];
            sendbuff[0] = USBCOMMOND_MSB;
            sendbuff[1] = USBCOMMOND_IAPMODE;
            if (!PortCommunicationUSB(sendbuff, recvbuff))
            {
                MessageBox.Show("USB port communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);   
                return false;
            }
            else
            {
                if ((recvbuff[0] != USBCOMMOND_MSB) || (recvbuff[1] != USBCOMMOND_IAPMODE) || (recvbuff[2] != USBCOMMOND_ACK_MSB) || (recvbuff[3] != USBCOMMOND_ACK_LSB))
                {
                    MessageBox.Show("USB device receive NACK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return false;
                }
            }
            return true;
        }

        private bool BeginDownUSB()
        {
            byte[] sendbuff = new byte[2];
            byte[] recvbuff = new byte[4];
            sendbuff[0] = USBCOMMOND_MSB;
            sendbuff[1] = USBCOMMOND_BEGINDOWN;
            if (!PortCommunicationUSB(sendbuff, recvbuff))
            {
                MessageBox.Show("USB port communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return false;
            }
            else
            {
                if ((recvbuff[0] != USBCOMMOND_MSB) || (recvbuff[1] != USBCOMMOND_BEGINDOWN) || (recvbuff[2] != USBCOMMOND_ACK_MSB) || (recvbuff[3] != USBCOMMOND_ACK_LSB))
                {
                    MessageBox.Show("USB device receive NACK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return false;
                }
            }
            return true;
        }

        private bool CRCVerifyUSBForSPIF(uint StartAddr, List<FILEPARTINFO> FIleInfo, uint PageSize)
        {
            for (int i = 0; i < FIleInfo.Count; i++)
            {
                uint address = StartAddr + FIleInfo[i].PartStartAddress;
                uint pageNum = 0;
                if(FIleInfo[i].PartSize% PageSize == 0)
                {
                    pageNum = (uint)FIleInfo[i].PartSize / PageSize;
                }
                else
                {
                    pageNum = (uint)FIleInfo[i].PartSize / PageSize + 1;
                }

                byte[] sendCRCbuff = new byte[8];
                byte[] recvCRCbuff = new byte[8];
                sendCRCbuff[0] = USBCOMMOND_MSB;
                sendCRCbuff[1] = USBCOMMOND_SPIFCRC;
                sendCRCbuff[2] = (byte)(address >> 24);
                sendCRCbuff[3] = (byte)((address >> 16) & 0x00FF);
                sendCRCbuff[4] = (byte)((address >> 8) & 0x0000FF);
                sendCRCbuff[5] = (byte)((address >> 0) & 0x000000FF);
                sendCRCbuff[6] = (byte)((pageNum >> 8) & 0x0000FF);
                sendCRCbuff[7] = (byte)((pageNum >> 0) & 0x000000FF);

                recvCRCbuff[0] = 0x00;
                recvCRCbuff[1] = 0x00;
                if (!PortCommunicationUSB(sendCRCbuff, recvCRCbuff))
                {
                    MessageBox.Show("USB device communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return false;
                }
                else
                {
                    if ((recvCRCbuff[0] != USBCOMMOND_MSB) || (recvCRCbuff[1] != USBCOMMOND_SPIFCRC) || (recvCRCbuff[2] != USBCOMMOND_ACK_MSB) || (recvCRCbuff[3] != USBCOMMOND_ACK_LSB))
                    {
                        MessageBox.Show("USB device receive NACK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        return false;
                    }
                    else
                    {
                        //CRC Value right?
                        int crcLength = (int)(PageSize * pageNum) ;
                        uint fileCRCCode = GetLoadFileCRCCode(FIleInfo[i].PartData, crcLength);

                        uint fwCRCCode = ((uint)recvCRCbuff[4] << 24) + ((uint)recvCRCbuff[5] << 16) + ((uint)recvCRCbuff[6] << 8) + (uint)recvCRCbuff[7];
                        if (fwCRCCode == fileCRCCode)
                        {
                            continue;
                        }
                        else
                        {
                            MessageBox.Show("USB device CRC verify failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                          MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        private bool CRCVerifyUSB(uint StartAddr,  List<FILEPARTINFO> FIleInfo)
        {
            for (int i = 0; i < FIleInfo.Count; i++)
            {
                uint address = StartAddr + FIleInfo[i].PartStartAddress;
                uint pageNum = 2;

                byte[] sendCRCbuff = new byte[8];
                byte[] recvCRCbuff = new byte[8];
                sendCRCbuff[0] = USBCOMMOND_MSB;
                sendCRCbuff[1] = USBCOMMOND_CRC;
                sendCRCbuff[2] = (byte)(address >> 24);
                sendCRCbuff[3] = (byte)((address >> 16) & 0x00FF);
                sendCRCbuff[4] = (byte)((address >> 8) & 0x0000FF);
                sendCRCbuff[5] = (byte)((address >> 0) & 0x000000FF);
                sendCRCbuff[6] = (byte)((pageNum >> 8) & 0x0000FF);
                sendCRCbuff[7] = (byte)((pageNum >> 0) & 0x000000FF);

                recvCRCbuff[0] = 0x00;
                recvCRCbuff[1] = 0x00;
                if (!PortCommunicationUSB(sendCRCbuff, recvCRCbuff))
                {
                    MessageBox.Show("USB device communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return false;
                }
                else
                {
                    if ((recvCRCbuff[0] != USBCOMMOND_MSB) || (recvCRCbuff[1] != USBCOMMOND_CRC) || (recvCRCbuff[2] != USBCOMMOND_ACK_MSB) || (recvCRCbuff[3] != USBCOMMOND_ACK_LSB))
                    {
                        MessageBox.Show("USB device receive NACK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        return false;
                    }
                    else
                    {
                        //CRC Value right?
                        uint fileCRCCode = GetLoadFileCRCCode(FIleInfo[i].PartData, FIleInfo[i].PartData.Length);

                        uint fwCRCCode = ((uint)recvCRCbuff[4] << 24) + ((uint)recvCRCbuff[5] << 16) + ((uint)recvCRCbuff[6] << 8) + (uint)recvCRCbuff[7];
                        if (fwCRCCode == fileCRCCode)
                        {
                            continue;
                        }
                        else
                        {
                            MessageBox.Show("USB device CRC verify failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                          MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool DownloadEndUSB()
        {
            byte[] sendbuff = new byte[2];
            byte[] recvbuff = new byte[4];

            sendbuff[0] = USBCOMMOND_MSB;
            sendbuff[1] = USBCOMMOND_SENDEND;
            recvbuff[0] = 0x00;
            recvbuff[1] = 0x00;
            if (!PortCommunicationUSB(sendbuff, recvbuff))
            {
                MessageBox.Show("USB device communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return false;
            }
            else
            {
                if ((recvbuff[0] != USBCOMMOND_MSB) || (recvbuff[1] != USBCOMMOND_SENDEND) || (recvbuff[2] != USBCOMMOND_ACK_MSB) || (recvbuff[3] != USBCOMMOND_ACK_LSB))
                {
                    MessageBox.Show("USB device receive NACK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return false;
                }
            }
            return true;
        }

        private bool JumpToAppUSB()
        {
            byte[] sendbuff = new byte[2];
            byte[] recvbuff = new byte[4];
            sendbuff[0] = USBCOMMOND_MSB;
            sendbuff[1] = USBCOMMOND_JUMPAPP;
            recvbuff[0] = 0x00;
            recvbuff[1] = 0x00;
            if (!PortCommunicationUSB(sendbuff, recvbuff))
            {
                MessageBox.Show("USB device communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return false;
            }
            else
            {
                if ((recvbuff[0] != USBCOMMOND_MSB) || (recvbuff[1] != USBCOMMOND_JUMPAPP) || (recvbuff[2] != USBCOMMOND_ACK_MSB) || (recvbuff[3] != USBCOMMOND_ACK_LSB))
                {
                    MessageBox.Show("USB device receive NACK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return false;
                }
            }
            return true;
        }

        private bool DownloadDataToDeviceUSB(uint StartAddr,  List<FILEPARTINFO> FIleInfo)
        {
            byte[] sendbuff = new byte[2];
            byte[] recvbuff = new byte[4];
            string strSend = string.Empty;
            byte[] DataSendBuff = new byte[oSp.OutputReportLength];
            byte[] AddrBuff = new byte[6];

            for (int i = 0; i < FIleInfo.Count; i++)
            {
                for (uint j = 0; j < FIleInfo[i].PartData.Length / USB_PART_LENGTH; j++)
                {
                    //send addr
                    uint address = StartAddr + FIleInfo[i].PartStartAddress + j * USB_PART_LENGTH;
                    AddrBuff[0] = USBCOMMOND_MSB;
                    AddrBuff[1] = USBCOMMOND_ADDRESS;
                    AddrBuff[2] = (byte)(address >> 24);
                    AddrBuff[3] = (byte)((address >> 16) & 0x00FF);
                    AddrBuff[4] = (byte)((address >> 8) & 0x0000FF);
                    AddrBuff[5] = (byte)((address >> 0) & 0x000000FF);
                    if (!PortCommunicationUSB(AddrBuff, recvbuff))
                    {
                        MessageBox.Show("USB device communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        return false ;
                    }
                    else
                    {
                        if ((recvbuff[0] != USBCOMMOND_MSB) || (recvbuff[1] != USBCOMMOND_ADDRESS) || (recvbuff[2] != USBCOMMOND_ACK_MSB) || (recvbuff[3] != USBCOMMOND_ACK_LSB))
                        {
                            MessageBox.Show("USB device receive NACK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return false;
                        }
                    }

                    int sendDataLen = oSp.OutputReportLength - 5; //1byte  00, 2 bytes command, 2 bytes length
                    int sendTimes = 0;
                    if (USB_PART_LENGTH % sendDataLen == 0)
                        sendTimes = USB_PART_LENGTH / sendDataLen;
                    else
                        sendTimes = USB_PART_LENGTH / sendDataLen + 1;

                    DataSendBuff[0] = USBCOMMOND_MSB;
                    DataSendBuff[1] = USBCOMMOND_SENDDATA;

                    int realLen = 0;
                    for (int k = 0; k < sendTimes; k++)
                    {
                        if ((k + 1) * sendDataLen > USB_PART_LENGTH)
                        {
                            realLen = USB_PART_LENGTH - k * sendDataLen;
                        }
                        else
                        {
                            realLen = sendDataLen;
                        }
                        DataSendBuff[2] = (byte)((realLen >> 8) & 0x0000FF);
                        DataSendBuff[3] = (byte)((realLen >> 0) & 0x000000FF);

                        for (int m = 0; m < realLen; m++)
                            DataSendBuff[m + 4] = FIleInfo[i].PartData[j * USB_PART_LENGTH + k * sendDataLen + m];

                        if (!PortSendUSB(DataSendBuff))
                        {
                            MessageBox.Show("USB device send data error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                                   MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return false;
                        }
                    }

                    if (!PortReadUSB(recvbuff))
                    {
                        MessageBox.Show("USB device receive data error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        return false;
                    }
                    else
                    {
                        if ((recvbuff[0] != USBCOMMOND_MSB) || (recvbuff[1] != USBCOMMOND_SENDDATA) || (recvbuff[2] != USBCOMMOND_ACK_MSB) || (recvbuff[3] != USBCOMMOND_ACK_LSB))
                        {
                            MessageBox.Show("USB device receive NACK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return false;
                        }
                    }
                }

                double kkk = 5 + (i + 1) * 1.0 / FIleInfo.Count * 90;
                frm_Progress.SetProgress(100, (int)kkk);
            }

            return true;
        }        

        private uint GetLoadFileCRCCode(byte[] pFileAllData, int reallength)
        {
            int kk = reallength;

            int i;
            UInt32 crc = 0xFFFFFFFF;

            for (i = 0; i < kk; i++)
                crc = (crc << 8) ^ crc32table[((crc >> 24) ^ pFileAllData[i]) & 0xFF];

            return crc;
        }

        private bool PortCommunicationUSB(byte[] sendBuff, byte[] recvBuff)
        {
            if ((sendBuff == null) || (recvBuff == null))
                return false;

            try
            {
                Hid.HID_RETURN hdrtn = oSp.Write(new report(0, sendBuff));

                if (hdrtn != Hid.HID_RETURN.SUCCESS)
                {
                    return false;
                }
            }
            catch 
            {
                return false;
            }

            int timeout = 0;
            while (true)
            {
                try
                {
                    Hid.HID_RETURN re = oSp.Read(recvBuff, recvBuff.Length);
                    if (re == Hid.HID_RETURN.SUCCESS)
                        return true;
                    else
                    {
                        Thread.Sleep(10);
                        timeout++;
                        if (timeout >= 500)
                            return false;
                    }
                }
                catch
                {
                        return false;
                }
            }
        }

        private bool PortSendUSB(byte[] sendBuff)
        {
            if (sendBuff == null)
                return false;

            try
            {
                Hid.HID_RETURN hdrtn = oSp.Write(new report(0, sendBuff));

                if (hdrtn != Hid.HID_RETURN.SUCCESS)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool PortReadUSB(byte[] recvBuff)
        {
            if (recvBuff == null)
                return false;

            int timeout = 0;
            while (true)
            {
                try
                {
                    Hid.HID_RETURN re = oSp.Read(recvBuff, recvBuff.Length);
                    if (re == Hid.HID_RETURN.SUCCESS)
                        return true;
                    else
                    {
                        Thread.Sleep(10);
                        timeout++;
                        if (timeout >= 500)
                            return false;
                    }
                }
                catch
                {
                    return false;
                }
            }        
        }

        private void DownloadThreadRS232()
        {
            lock (lockIniDevice)
            {
                List<FILEPARTINFO> FIleInfo = null;

                new System.Threading.Thread(new System.Threading.ThreadStart(ShowForm)).Start();
                frm_Progress.SetOprationInfo("Downloading.......");
                frm_Progress.SetProgress(100, 0);

                uint StartAddr = 0;
                if (!GetFileInfo(out StartAddr, out FIleInfo))
                {
                    frm_Progress.CloseBar();
                    sp.Close();
                    return;
                }

                byte[] sendbuff = new byte[2];
                byte[] recvbuff = new byte[2];

                //send 5AA5
                sendbuff[0] = 0x5A;
                sendbuff[1] = 0xA5;
                recvbuff[0] = 0x00;
                recvbuff[1] = 0x00;
                if (!PortCommunicationRS232(sendbuff, recvbuff))
                {
                    MessageBox.Show("Serial port communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    frm_Progress.CloseBar();
                    sp.Close();
                    return;
                }
                else
                {
                    if ((recvbuff[0] != 0xCC) || (recvbuff[1] != 0xDD))
                    {
                        MessageBox.Show("Download error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        frm_Progress.CloseBar();
                        sp.Close();
                        return;
                    }
                }
                frm_Progress.SetProgress(100, 5);
                Thread.Sleep(800);
                //send 5A01
                sendbuff[0] = 0x5A;
                sendbuff[1] = 0x01;
                recvbuff[0] = 0x00;
                recvbuff[1] = 0x00;
                if (!PortCommunicationRS232(sendbuff, recvbuff))
                {
                    MessageBox.Show("Serial port communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    frm_Progress.CloseBar();
                    sp.Close();
                    return;
                }
                else
                {
                    if ((recvbuff[0] != 0xCC) || (recvbuff[1] != 0xDD))
                    {
                        MessageBox.Show("Download error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        frm_Progress.CloseBar();
                        sp.Close();
                        return;
                    }
                }
                frm_Progress.SetProgress(100, 10);
                //send file data
                string strSend = string.Empty;
                byte[] DataSendBuff = new byte[2054];
                uint checksum = 0;
                for (int i = 0; i < FIleInfo.Count; i++)
                {
                    DataSendBuff[0] = 0x31;
                    checksum = 0;
                    uint address = StartAddr + FIleInfo[i].PartStartAddress;
                    DataSendBuff[1] = (byte)(address >> 24);
                    checksum += DataSendBuff[1];
                    DataSendBuff[2] = (byte)((address >> 16) & 0x00FF);
                    checksum += DataSendBuff[2];
                    DataSendBuff[3] = (byte)((address >> 8) & 0x0000FF);
                    checksum += DataSendBuff[3];
                    DataSendBuff[4] = (byte)((address >> 0) & 0x000000FF);
                    checksum += DataSendBuff[4];

                    for (int k = 0; k < 2048; k++)
                    {
                        DataSendBuff[5 + k] = FIleInfo[i].PartData[k];
                        checksum += DataSendBuff[5 + k];
                    }

                    DataSendBuff[2053] = (byte)checksum;

                    recvbuff[0] = 0x00;
                    recvbuff[1] = 0x00;
                    if (!PortCommunicationRS232(DataSendBuff, recvbuff))
                    {
                        MessageBox.Show("Serial port communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        frm_Progress.CloseBar();
                        sp.Close();
                        return;
                    }
                    else
                    {
                        if ((recvbuff[0] != 0xCC) || (recvbuff[1] != 0xDD))
                        {
                            MessageBox.Show("Download error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            frm_Progress.CloseBar();
                            sp.Close();
                            return;
                        }
                    }
                    double kkk = 5 + (i + 1) * 1.0 / FIleInfo.Count * 90;
                    frm_Progress.SetProgress(100, (int)kkk);
                }

                //send 5A02
                sendbuff[0] = 0x5A;
                sendbuff[1] = 0x02;
                recvbuff[0] = 0x00;
                recvbuff[1] = 0x00;
                if (!PortCommunicationRS232(sendbuff, recvbuff))
                {
                    MessageBox.Show("Serial port communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    frm_Progress.CloseBar();
                    sp.Close();
                    return;
                }
                else
                {
                    if ((recvbuff[0] != 0xCC) || (recvbuff[1] != 0xDD))
                    {
                        MessageBox.Show("Download error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        frm_Progress.CloseBar();
                        sp.Close();
                        return;
                    }
                }

                frm_Progress.SetProgress(100, 100);
                MessageBox.Show("Download succeed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);

                sp.Close();
            }
        }

        private bool PortCommunicationRS232(byte [] sendBuff, byte [] recvBuff)
        {
            if (!sp.IsOpen)
                return false;

            if ((sendBuff == null) || (recvBuff == null))
                return false;
            
            try
            {
                sp.Write(sendBuff, 0, sendBuff.Length);
            }
            catch
            {
                return false;
            }

            int count = 0;
            int timeout = 0;
            while (true)
            {
                try
                {
                    recvBuff[count] = (byte)sp.ReadByte();
                    count++;
                    if (count >= recvBuff.Length)
                        break;
                }
                catch
                {
                    timeout++;
                    if (timeout >= 5)
                        return  false;
                    else
                        continue;
                }
            }

            return true;
        }

        private void InitRS232PortName()
        {
            comboBoxPortName.Items.Clear();
            string[] str = SerialPort.GetPortNames();
            if (str.Length == 0)
            {
                MessageBox.Show("No SerialPort！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                foreach (string s in str)
                {
                    comboBoxPortName.Items.Add(s);
                }
            }
            if (m_PortNameSelectUART < comboBoxPortName.Items.Count)
                comboBoxPortName.SelectedIndex = m_PortNameSelectUART;
            else
                comboBoxPortName.SelectedIndex = 0;
        }

        private bool OpenSerialPort()
        {
            if (string.IsNullOrEmpty(comboBoxPortName.SelectedItem.ToString()))
            {
                MessageBox.Show("No RS232 port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                sp.BaudRate = 115200;
                sp.PortName = comboBoxPortName.SelectedItem.ToString();
                sp.DataBits = 8;
                sp.Parity = Parity.None; 
                sp.StopBits = StopBits.One; 
                sp.ReadTimeout = 1000;
                sp.Open();
            }
            catch
            {
                return false;
            }

            if (sp.IsOpen)
            {
                return true;
            }
            return false;
        }

        private ArrayList SplitLength(string sourceString, int length)
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < sourceString.Trim().Length; i += length)
            {
                if (sourceString.Trim().Length - i >= length)
                {
                    list.Add(sourceString.Trim().Substring(i, length));
                }
                else
                {
                    list.Add(sourceString.Trim().Substring(i, sourceString.Trim().Length - i));
                }
            }
            return list;
        }

        public List<FILEPARTINFO> AddValueBin(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                return null;
            }

            List<FILEPARTINFO> partInfo;
            partInfo = new List<FILEPARTINFO>();

            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            int fullLength = (int)fs.Length;

            if (fullLength == 0)
                return null;

            int partLength = 0;
            FILEPARTINFO tempInfo = new FILEPARTINFO();
            tempInfo.PartStartAddress = 0;
            tempInfo.PartEndAddress = 0;
            tempInfo.PartSize = 2048;
            tempInfo.PartData = new byte[2048];
            for (int k = 0; k < 2048; k++)
                tempInfo.PartData[k] = 0xFF;
            partInfo.Add(tempInfo);
            partLength = 0;

            for (int i = 0; i < fullLength; i++)
            {
                partInfo[partInfo.Count - 1].PartData[partLength] = br.ReadByte();
                partLength++;
                if(partLength == 0x0800)
                {
                    tempInfo = new FILEPARTINFO();
                    tempInfo.PartStartAddress = partInfo[partInfo.Count - 1].PartStartAddress + 0x0800;
                    tempInfo.PartData = new byte[2048];
                    for (int k = 0; k < 2048; k++)
                        tempInfo.PartData[k] = 0xFF;
                    partInfo.Add(tempInfo);
                    partLength = 0;
                }
            }
            return partInfo;
        }

        public List<FILEPARTINFO> AddValueHex(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                return null;
            }

            List<FILEPARTINFO> partInfo;
            partInfo = new List<FILEPARTINFO>();

            ArrayList arr1 = new ArrayList();

            string dizhi = "";

            string[] readtxt = File.ReadAllLines(FilePath);
            string firstvalue = readtxt[0];
            int qq3 = 0;
            int q1 = 0;
            int partLength = 0;

            for (int qq = 0; qq < readtxt.Length; qq++)
            {
                if (readtxt[qq].Substring(7, 2) == "04")
                {
                    int q = Convert.ToInt32(readtxt[qq].Substring(9, 4), 16);//16进制转10进制，取04后面的数据0800，作为基地址
                    q1 = Convert.ToInt32(q * Math.Pow((double)2, (double)16));
                    qq3 = Convert.ToInt32(readtxt[qq + 1].Substring(3, 4), 16);//取第一行数据的地址
                    int thisPartBenginAddr = qq3 + q1;
                    int lastPartEndAddr = 0;
                    if (dizhi == string.Empty)
                        lastPartEndAddr = 0 + partLength - 16;
                    else
                        lastPartEndAddr = int.Parse(dizhi, System.Globalization.NumberStyles.HexNumber) + partLength - 16;

                    if ((thisPartBenginAddr - lastPartEndAddr > 0x10) || (partLength % 16 != 0) || (partLength == 0x800)) //非连续地址或者上个part地址未写满16字节，才算下一个part开始
                    {
                        dizhi = (qq3 + q1).ToString("X8");//转16进制

                        FILEPARTINFO tempInfo = new FILEPARTINFO();
                        tempInfo.PartStartAddress = uint.Parse(dizhi, System.Globalization.NumberStyles.HexNumber);
                        tempInfo.PartEndAddress = 0;
                        tempInfo.PartSize = 2048;
                        tempInfo.PartData = new byte[2048];
                        for (int k = 0; k < 2048; k++)
                            tempInfo.PartData[k] = 0xFF;
                        partInfo.Add(tempInfo);
                        partLength = 0;
                    }
                }
                else if ((readtxt[qq].Substring(7, 2) == "01") || (qq + 1 == readtxt.Length))
                {
                    break;
                }
                else if (readtxt[qq].Substring(7, 2) != "00")
                {
                    continue;
                }
                else
                {
                    string[] str4 = Regex.Split(readtxt[qq], ":");
                    string str5 = str4[1];
                    string strlength = str5.Substring(0, 2);
                    string str6 = str5.Substring(8, str5.Length - 2 - 8);
                    int ilength = int.Parse(strlength, System.Globalization.NumberStyles.HexNumber);
                    arr1 = SplitLength(str6, 2);

                    if (arr1.Count != ilength)
                    {
                        MessageBox.Show("Error", "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }

                    if (partLength == 0x0800)
                    {
                        FILEPARTINFO tempInfo = new FILEPARTINFO();
                        tempInfo.PartStartAddress = partInfo[partInfo.Count - 1].PartStartAddress + 0x0800;
                        tempInfo.PartEndAddress = 0;
                        tempInfo.PartSize = 2048;
                        tempInfo.PartData = new byte[2048];
                        for (int k = 0; k < 2048; k++)
                            tempInfo.PartData[k] = 0xFF;
                        partInfo.Add(tempInfo);
                        partLength = 0;
                    }

                    if (partInfo.Count > 0)
                    {
                        for (int i = 0; i < arr1.Count; i++)
                        {
                            partInfo[partInfo.Count - 1].PartData[partLength] = Convert.ToByte(arr1[i].ToString(), 16);
                            partLength++;
                        }
                    }
                }
            }

            return partInfo;
        }

        private void SPIFInfoDisplay(bool flag)
        {
            checkBox_CRC.Enabled = flag;
            label4.Enabled = flag;
            textBox_SPIFAddress.Enabled = flag;
            label5.Enabled = flag;
            textBox_SPIFFilePath.Enabled = flag;
            butto_OpenSPIFFile.Enabled = flag;
            button_DownloadSPIF.Enabled = flag;
            checkBox_WriteSPIF.Enabled = flag;
        }
        private void comboBox_PortType_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitDevice();
        }

        private void InitDevice()
        {
            lock (lockIniDevice)
            {
                if (comboBox_PortType.SelectedIndex == 0)
                {
                    label2.Text = "RS232 Port Name";
                    SPIFInfoDisplay(false);
                    InitRS232PortName();
                }
                else
                {
                    label2.Text = "USB Device Serial No.";
                    SPIFInfoDisplay(true);
                    comboBoxPortName.Items.Clear();
                    List<string> deviceList = null;
                    oSp.GetHIDDeviceList(uVID, uPID, out deviceList);
                    if ((deviceList != null) && (deviceList.Count > 0))
                    {
                        foreach (string str in deviceList)
                        {
                            comboBoxPortName.Items.Add(str);
                        }

                        if (m_PortNameSelectUSB < comboBoxPortName.Items.Count)
                            comboBoxPortName.SelectedIndex = m_PortNameSelectUSB;
                        else
                            comboBoxPortName.SelectedIndex = 0;

                        if (checkBox_WriteSPIF.Checked)
                            GetUSBSPIFInfo(comboBoxPortName.Text);
                    }
                    else
                    {
                        //MessageBox.Show("No USB IAP device!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void GetUSBSPIFInfo(string str)
        {
            Hid.HID_RETURN rv = oSp.OpenDevice(uVID, uPID, str);
            if (rv != Hid.HID_RETURN.SUCCESS)
            {
                MessageBox.Show("Can't open the usb device", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            SPIFSize = 0;
            SPIFSectorSize = 0;
            SPIFPageSize = 0;
            if (GetSPIFSize(out SPIFSize, out SPIFSectorSize, out SPIFPageSize))
            {
                label5.Text = "SPIF Size: " + ((uint)SPIFSize / 1024).ToString() + "K bytes";
                oSp.CloseDevice();
                return;
            }
            else
            {
                label5.Text = "SPIF Size: " + " read error";
            }

            oSp.CloseDevice();
        }
private bool DownloadDataToUSBSPIF(uint StartAddr, List<FILEPARTINFO> FIleInfo, uint PageSize)
        {
            byte[] sendbuff = new byte[2];
            byte[] recvbuff = new byte[4];
            string strSend = string.Empty;
            byte[] DataSendBuff = new byte[oSp.OutputReportLength];
            byte[] AddrBuff = new byte[6];
            uint allSentTimes = 0;
            uint allSentTimesStep = 0;

            for (int i = 0; i < FIleInfo.Count; i++)
            {
                if (FIleInfo[i].PartSize % PageSize == 0)
                    allSentTimes += (uint)FIleInfo[i].PartSize / PageSize;
                else
                    allSentTimes += (uint)FIleInfo[i].PartSize / PageSize + 1;
            }

            for (int i = 0; i < FIleInfo.Count; i++)
            {
                uint sendTime = 0;
                if(FIleInfo[i].PartSize % PageSize == 0)
                    sendTime = (uint)FIleInfo[i].PartSize/ PageSize;
                else
                    sendTime = (uint)FIleInfo[i].PartSize / PageSize + 1;
                for (uint j = 0; j < sendTime; j++)
                {
                    //send addr
                    uint address = StartAddr + FIleInfo[i].PartStartAddress + j * PageSize;
                    AddrBuff[0] = USBCOMMOND_MSB;
                    AddrBuff[1] = USBCOMMOND_SETSPIFADDRESS;
                    AddrBuff[2] = (byte)(address >> 24);
                    AddrBuff[3] = (byte)((address >> 16) & 0x00FF);
                    AddrBuff[4] = (byte)((address >> 8) & 0x0000FF);
                    AddrBuff[5] = (byte)((address >> 0) & 0x000000FF);
                    if (!PortCommunicationUSB(AddrBuff, recvbuff))
                    {
                        MessageBox.Show("USB device communication error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        return false;
                    }
                    else
                    {
                        if ((recvbuff[0] != USBCOMMOND_MSB) || (recvbuff[1] != USBCOMMOND_SETSPIFADDRESS) || (recvbuff[2] != USBCOMMOND_ACK_MSB) || (recvbuff[3] != USBCOMMOND_ACK_LSB))
                        {
                            MessageBox.Show("USB device receive NACK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return false;
                        }
                    }

                    uint sendDataLen = (uint)oSp.OutputReportLength - 5; //1byte  00, 2 bytes command, 2 bytes length
                    uint sendTimes = 0;
                    if (PageSize % sendDataLen == 0)
                        sendTimes = PageSize / sendDataLen;
                    else
                        sendTimes = PageSize / sendDataLen + 1;

                    DataSendBuff[0] = USBCOMMOND_MSB;
                    DataSendBuff[1] = USBCOMMOND_SPIFSENDDATA;

                    uint realLen = 0;
                    for (uint k = 0; k < sendTimes; k++)
                    {
                        if ((k + 1) * sendDataLen > PageSize)
                        {
                            realLen = PageSize - k * sendDataLen;
                        }
                        else
                        {
                            realLen = sendDataLen;
                        }
                        DataSendBuff[2] = (byte)((realLen >> 8) & 0x0000FF);
                        DataSendBuff[3] = (byte)((realLen >> 0) & 0x000000FF);

                        for (int m = 0; m < realLen; m++)
                            DataSendBuff[m + 4] = FIleInfo[i].PartData[j * PageSize + k * sendDataLen + m];

                        if (!PortSendUSB(DataSendBuff))
                        {
                            MessageBox.Show("USB device send data error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                                   MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return false;
                        }
                    }                  

                    if (!PortReadUSB(recvbuff))
                    {
                        MessageBox.Show("USB device receive data error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        return false;
                    }
                    else
                    {
                        if ((recvbuff[0] != USBCOMMOND_MSB) || (recvbuff[1] != USBCOMMOND_SPIFSENDDATA) || (recvbuff[2] != USBCOMMOND_ACK_MSB) || (recvbuff[3] != USBCOMMOND_ACK_LSB))
                        {
                            MessageBox.Show("USB device receive NACK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return false;
                        }
                    }

                    allSentTimesStep++;
                    double kkk = allSentTimesStep * 1.0 / allSentTimes * 100;
                    frm_Progress.SetProgress(100, (int)kkk);
                }                
            }

            return true;
        }

        public List<FILEPARTINFO> AddValueBinForSPIF(string FilePath, out uint FileSize)
        {
            FileSize = 0;
            if (!File.Exists(FilePath))
            {
                return null;
            }

            List<FILEPARTINFO> partInfo;
            partInfo = new List<FILEPARTINFO>();

            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            int fullLength = (int)fs.Length;

            if (fullLength == 0)
                return null;

            int partLength = 0;
            FILEPARTINFO tempInfo = new FILEPARTINFO();
            tempInfo.PartStartAddress = 0;
            tempInfo.PartEndAddress = 0;
            tempInfo.PartSize = 0;
            tempInfo.PartData = new byte[16*1024];
            for (int k = 0; k < 16 * 1024; k++)
                tempInfo.PartData[k] = 0xFF;
            partInfo.Add(tempInfo);
            partLength = 0;

            for (int i = 0; i < fullLength; i++)
            {
                partInfo[partInfo.Count - 1].PartData[partLength] = br.ReadByte();
                partInfo[partInfo.Count - 1].PartSize++;
                FileSize++;
                partLength++;
                if ((partLength == 16 * 1024) && (i +1 != fullLength))
                {
                    tempInfo = new FILEPARTINFO();
                    tempInfo.PartStartAddress = partInfo[partInfo.Count - 1].PartStartAddress + 16 * 1024;
                    tempInfo.PartSize = 0;
                    tempInfo.PartData = new byte[16 * 1024];
                    for (int k = 0; k < 16 * 1024; k++)
                        tempInfo.PartData[k] = 0xFF;
                    partInfo.Add(tempInfo);
                    partLength = 0;
                }
            }
            return partInfo;
        }

        public List<FILEPARTINFO> AddValueHexForSPIF(string FilePath, out uint FileSize)
        {
            FileSize = 0;
            if (!File.Exists(FilePath))
            {
                return null;
            }

            List<FILEPARTINFO> partInfo;
            partInfo = new List<FILEPARTINFO>();

            ArrayList arr1 = new ArrayList();

            string dizhi = "";

            string[] readtxt = File.ReadAllLines(FilePath);
            string firstvalue = readtxt[0];
            int qq3 = 0;
            int q1 = 0;
            int partLength = 0;

            for (int qq = 0; qq < readtxt.Length; qq++)
            {
                if (readtxt[qq].Substring(7, 2) == "04")
                {
                    int q = Convert.ToInt32(readtxt[qq].Substring(9, 4), 16);//16进制转10进制，取04后面的数据0800，作为基地址
                    q1 = Convert.ToInt32(q * Math.Pow((double)2, (double)16));
                    qq3 = Convert.ToInt32(readtxt[qq + 1].Substring(3, 4), 16);//取第一行数据的地址
                    int thisPartBenginAddr = qq3 + q1;
                    int lastPartEndAddr = 0;
                    if (dizhi == string.Empty)
                        lastPartEndAddr = 0 + partLength - 16;
                    else
                        lastPartEndAddr = int.Parse(dizhi, System.Globalization.NumberStyles.HexNumber) + partLength - 16;

                    if ((thisPartBenginAddr - lastPartEndAddr > 0x10) || (partLength % 16 != 0) || (partLength == 16 * 1024)) //非连续地址或者上个part地址未写满16字节，才算下一个part开始
                    {
                        dizhi = (qq3 + q1).ToString("X8");//转16进制

                        FILEPARTINFO tempInfo = new FILEPARTINFO();
                        tempInfo.PartStartAddress = uint.Parse(dizhi, System.Globalization.NumberStyles.HexNumber);
                        tempInfo.PartEndAddress = 0;
                        tempInfo.PartSize = 0;
                        tempInfo.PartData = new byte[16 * 1024];
                        for (int k = 0; k < 16 * 1024; k++)
                            tempInfo.PartData[k] = 0xFF;
                        partInfo.Add(tempInfo);
                        partLength = 0;
                    }
                }
                else if ((readtxt[qq].Substring(7, 2) == "01") || (qq + 1 == readtxt.Length))
                {
                    break;
                }
                else if (readtxt[qq].Substring(7, 2) != "00")
                {
                    continue;
                }
                else
                {
                    string[] str4 = Regex.Split(readtxt[qq], ":");
                    string str5 = str4[1];
                    string strlength = str5.Substring(0, 2);
                    string str6 = str5.Substring(8, str5.Length - 2 - 8);
                    int ilength = int.Parse(strlength, System.Globalization.NumberStyles.HexNumber);
                    arr1 = SplitLength(str6, 2);

                    if (arr1.Count != ilength)
                    {
                        MessageBox.Show("Error", "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }

                    if (partLength == 16 * 1024)
                    {
                        FILEPARTINFO tempInfo = new FILEPARTINFO();
                        tempInfo.PartStartAddress = partInfo[partInfo.Count - 1].PartStartAddress + 16 * 1024;
                        tempInfo.PartEndAddress = 0;
                        tempInfo.PartSize = 0;
                        tempInfo.PartData = new byte[16 * 1024];
                        for (int k = 0; k < 16 * 1024; k++)
                            tempInfo.PartData[k] = 0xFF;
                        partInfo.Add(tempInfo);
                        partLength = 0;
                    }

                    if (partInfo.Count > 0)
                    {
                        for (int i = 0; i < arr1.Count; i++)
                        {
                            partInfo[partInfo.Count - 1].PartData[partLength] = Convert.ToByte(arr1[i].ToString(), 16);
                            partInfo[partInfo.Count - 1].PartSize++;
                            FileSize++;
                            partLength++;
                        }
                    }
                }
            }

            return partInfo;
        }


        private bool GetFileInfoForSPIF(out uint StartAddr, out List<FILEPARTINFO> FIleInfo, out uint FileSize)
        {
            StartAddr = 0;
            FIleInfo = null;
            FileSize = 0;
            string temp3 = textBox_SPIFFilePath.Text.Substring(textBox_SPIFFilePath.Text.LastIndexOf(@"\"));
            string tempType = temp3.ToUpper();

            if (tempType.Substring(tempType.Length - 3, 3) == "HEX")
            {
                FIleInfo = AddValueHexForSPIF(textBox_SPIFFilePath.Text, out FileSize);
                StartAddr = 0;
            }
            else if (tempType.Substring(tempType.Length - 3, 3) == "BIN")
            {
                FIleInfo = AddValueBinForSPIF(textBox_SPIFFilePath.Text, out FileSize);
                StartAddr = 0;
            }
            else
            {
                MessageBox.Show("The download file format error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                frm_Progress.CloseBar();
                return false;
            }

            if (FIleInfo == null)
            {
                MessageBox.Show("The download file format error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                frm_Progress.CloseBar();
                return false;
            }
            if (FIleInfo.Count == 0)
            {
                MessageBox.Show("The download file format error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                       MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                frm_Progress.CloseBar();
                return false;
            }
            return true;
        }
        private void DownloadThreadUSBToSPIF()
        {
            lock (lockIniDevice)
            {
                List<FILEPARTINFO> FIleInfo = null;

                new System.Threading.Thread(new System.Threading.ThreadStart(ShowForm)).Start();
                frm_Progress.SetOprationInfo("Downloading.......");
                frm_Progress.SetProgress(100, 0);

                uint StartAddr = 0;
                uint FileSize = 0;
                if (!GetFileInfoForSPIF(out StartAddr, out FIleInfo, out FileSize))
                {
                    frm_Progress.CloseBar();
                    return;
                }

                Hid.HID_RETURN rv = oSp.OpenDevice(uVID, uPID, comboBoxPortName.Text);
                if (rv != Hid.HID_RETURN.SUCCESS)
                {
                    MessageBox.Show("Can't open the usb device", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    frm_Progress.CloseBar();
                    return;
                }

                SPIFSize = 0;
                SPIFSectorSize = 0;
                SPIFPageSize = 0;
                if (!GetSPIFSize(out SPIFSize, out SPIFSectorSize, out SPIFPageSize))
                {
                    MessageBox.Show("Get SPIF size error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }

                if (SPIFSize == 0)
                {
                    MessageBox.Show("No SPIF or SPIF type not support", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }

                if ((SPIFSize == 0) || (SPIFSectorSize == 0) || (SPIFPageSize == 0))
                {
                    MessageBox.Show("Read SPIF Info error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }

                string strAddr = textBox_SPIFAddress.Text;
                uint textStartAddress = StringToUInt32(strAddr, 16);

                if (textStartAddress % SPIFSectorSize != 0)
                {
                    string str = "SPIF Download Address error,address must be a multiple of 0x" + SPIFSectorSize.ToString("X8");
                    MessageBox.Show(str, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }
                if (textStartAddress >= SPIFSize)
                {
                    string str = "SPIF Download Address error,address must be less than SPIF size 0x" + SPIFSize.ToString("X8");
                    MessageBox.Show(str, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }

                if (textStartAddress + FileSize - 1 >= SPIFSize)
                {
                    string str = "Download file size error ,file download range must be less than SPIF size 0x" + SPIFSize.ToString("X8");
                    MessageBox.Show(str, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }

                //send file data
                if (!DownloadDataToUSBSPIF(StartAddr, FIleInfo, SPIFPageSize))
                {
                    frm_Progress.CloseBar();
                    oSp.CloseDevice();
                    return;
                }

                //CRC Check
                if (checkBox_CRC.Checked)
                {
                    frm_Progress.SetOprationInfo("CRC Verify.......");

                    if (!CRCVerifyUSBForSPIF(StartAddr, FIleInfo, SPIFPageSize))
                    {
                        frm_Progress.CloseBar();
                        oSp.CloseDevice();
                        return;
                    }
                }

                frm_Progress.SetProgress(100, 100);
                if (checkBox_CRC.Checked)
                {
                    MessageBox.Show("Download and CRC Verify succeed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
                else
                {
                    MessageBox.Show("Download succeed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information,
                                                               MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }

                oSp.CloseDevice();
            }
        }

        private void button_DownloadSPIF_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_SPIFFilePath.Text))
            {
                MessageBox.Show("Can't open the SPIF download file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(comboBoxPortName.Text))
            {
                MessageBox.Show("No USB IAP device!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread newth = new System.Threading.Thread(new System.Threading.ThreadStart(DownloadThreadUSBToSPIF));
            newth.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            newth.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            newth.Start();
        }

        private void butto_OpenSPIFFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Files(*.bin,*.hex)|*.bin;*.hex|IntelBIN Files(*.bin)|*.bin|Hes Files(*.hex)|*.hex|All FIles(*.*)|*.*";
            ofd.Title = "打开文件";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string temp3 = ofd.FileName.Substring(ofd.FileName.LastIndexOf(@"\"));
                string tempType = temp3.ToUpper();

                if (tempType.Substring(tempType.Length - 3, 3) == "HEX")
                {
                    List<FILEPARTINFO> FIleInfo = null;
                    FIleInfo = AddValueHex(ofd.FileName);
                    if(FIleInfo.Count >0)
                        textBox_SPIFAddress.Text = "0x" + FIleInfo[0].PartStartAddress.ToString("X8");
//                     else
//                         textBox_SPIFAddress.Text = "0x00000000";
                    textBox_SPIFAddress.Enabled = false;
                    label1.Enabled = false;
                }
                else if (tempType.Substring(tempType.Length - 3, 3) == "BIN")
                {
//                     textBox_SPIFAddress.Text = "0x00000000";
                    textBox_SPIFAddress.Enabled = true;
                    label1.Enabled = true;
                }
                else
                {
                    MessageBox.Show("The download file format error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                           MessageBoxDefaultButton.Button1, 0);
                    return;
                }
                textBox_SPIFFilePath.Text = ofd.FileName;
            }
        }

        private bool GetSPIFSize(out uint SPIFSize, out uint SectorSize, out uint PageSize)
        {
            SPIFSize = 0;
            SectorSize = 0;
            PageSize = 0;

            byte[] sendbuff = new byte[2];
            byte[] recvbuff = new byte[16];
            sendbuff[0] = USBCOMMOND_MSB;
            sendbuff[1] = USBCOMMOND_GETSPIFSIZE;
            if (!PortCommunicationUSB(sendbuff, recvbuff))
            {
                return false;
            }
            else
            {
                if ((recvbuff[0] != USBCOMMOND_MSB) || (recvbuff[1] != USBCOMMOND_GETSPIFSIZE) || (recvbuff[2] != USBCOMMOND_ACK_MSB) || (recvbuff[3] != USBCOMMOND_ACK_LSB))
                {
                    return false;
                }
                else
                {
                    SPIFSize = ((uint)recvbuff[4] << 24) + ((uint)recvbuff[5] << 16) + ((uint)recvbuff[6] << 8) + (uint)recvbuff[7];
                    SectorSize = ((uint)recvbuff[8] << 24) + ((uint)recvbuff[9] << 16) + ((uint)recvbuff[10] << 8) + (uint)recvbuff[11];
                    PageSize = ((uint)recvbuff[12] << 24) + ((uint)recvbuff[13] << 16) + ((uint)recvbuff[14] << 8) + (uint)recvbuff[15];
                }
            }
            return true;
        }

        public static uint StringToUInt32(string value, int fromBase)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            else
            {
                try
                {
                    return Convert.ToUInt32(value, fromBase);
                }
                catch
                {
                    return 0;
                }
            }
        }

        private void textBox_SPIFAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = "0123456789ABCDEFabcdef\b".IndexOf(char.ToUpper(e.KeyChar)) < 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void IAPDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_InitFlag = false;
            if (newth != null)
            {
                if (newth.IsAlive)
                {
                    newth.Abort();
                    newth = null;
                }
            }
        }

        private void comboBoxPortName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_PortType.SelectedIndex == 0)
            {
                m_PortNameSelectUART = comboBoxPortName.SelectedIndex;
            }
            else
            {
                m_PortNameSelectUSB= comboBoxPortName.SelectedIndex;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void IAPDemo_Load(object sender, EventArgs e)
        {

        }
    }
}
