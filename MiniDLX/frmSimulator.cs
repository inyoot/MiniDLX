using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;

namespace MiniDLX
{
    
    public partial class frmSimulator : Form
    {
        //private PipelineMapRow[] pipelinemaprows;
        Int32 GpipeRow = 0;
        Int32 StallCounter = 0;
        string[] pipeMap;
        /*struct PipeLineMap
        {
            public Pen pen;
            public Point point;
            public Brush brush;
            public string name;           
            public bool forwardingFlag;
        }*/

        public class PipeLineMap
        {
            public Pen pen;
            public Point point;
            public Brush brush;
            public string name;
            public bool forwardingFlag;
        }
        enum CycleStatus
        {
            Run, Stop, Stall, Flush
        }
        private Int32 CycleCounter = 0;
        private UInt32 PC = 0;
        private byte[] DataMem;
        private byte[] R;
        private string[] codeMemory;
           
        private CycleStatus IFStatus;
        private CycleStatus IDStatus;
        private CycleStatus EXStatus;
        private CycleStatus MEMStatus;
        private CycleStatus WBStatus;

        private PipelineRegister IF_ID = new PipelineRegister();
        private PipelineRegister ID_EX = new PipelineRegister();
        private PipelineRegister EX_MEM = new PipelineRegister();
        private PipelineRegister MEM_WB = new PipelineRegister();

        private ArrayList pipeLineMap;
        private const Int32 boxWidth = 50;
        private const Int32 boxHeight = 15;
        private const Int32 verticalDis = 10;

        private bool simRunning = false;
        public Thread simThread;
        private Int32 counterBox = 1;

        public frmSimulator(string opCode, string simCode)
        {
            InitializeComponent();
            rtbPipelineMap.Visible = false;
            pictureBox1.Parent = panel1;
            htbMemory.InitializeComponent();
            htbRegisters.InitializeComponent();
            codeMemory = opCode.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            lsbCode.Items.AddRange(simCode.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            
        }

        #region ScrollingAddressLabel
        private void updateMemoryNumberLabel()
        {
            //we get index of first visible char and number of first visible line
            Point pos = new Point(3, 3);
            int firstIndex = htbMemory.GetCharIndexFromPosition(pos);
            int firstLine = htbMemory.GetLineFromCharIndex(firstIndex);

            //now we get index of last visible char and number of last visible line
            pos.X = htbMemory.ClientRectangle.Width - 3;
            pos.Y = htbMemory.ClientRectangle.Height - 3;
            int lastIndex = htbMemory.GetCharIndexFromPosition(pos);
            int lastLine = htbMemory.GetLineFromCharIndex(lastIndex);

            pos = htbMemory.GetPositionFromCharIndex(firstIndex);
            MemoryNumber.Location = new Point(htbMemory.Location.X - MemoryNumber.Width, htbMemory.Location.Y + pos.Y + 2);
            MemoryNumber.Height = htbMemory.Height - (pos.Y + 2);

            //finally, renumber label
            MemoryNumber.Text = "";
            for (int i = firstLine; i <= lastLine + 1; i++)
            {
                MemoryNumber.Text += string.Format("{0:X4}", i * 8) + "\n";
            }

        }

        private void updateRegisterNumberLabel()
        {
            //we get index of first visible char and number of first visible line
            Point pos = new Point(3, 3);
            int firstIndex = htbRegisters.GetCharIndexFromPosition(pos);
            int firstLine = htbRegisters.GetLineFromCharIndex(firstIndex);

            //now we get index of last visible char and number of last visible line
            pos.X = htbRegisters.ClientRectangle.Width - 3;
            pos.Y = htbRegisters.ClientRectangle.Height - 3;
            int lastIndex = htbRegisters.GetCharIndexFromPosition(pos);
            int lastLine = htbRegisters.GetLineFromCharIndex(lastIndex);

            pos = htbRegisters.GetPositionFromCharIndex(firstIndex);
            RegisterNumber.Location = new Point(htbRegisters.Location.X - RegisterNumber.Width, htbRegisters.Location.Y + pos.Y + 2);
            RegisterNumber.Height = htbRegisters.Height - (pos.Y + 2);

            //finally, renumber label
            RegisterNumber.Text = "";
            for (int i = firstLine; i <= lastLine + 1; i++)
            {
                RegisterNumber.Text += "R" + (i + 1) + "\n";
            }

        }

        private void htbMemory_TextChanged(object sender, EventArgs e)
        {
            updateMemoryNumberLabel();
        }

        private void htbMemory_VScroll(object sender, EventArgs e)
        {
            updateMemoryNumberLabel();
        }

        private void htbMemory_Resize(object sender, EventArgs e)
        {
            updateMemoryNumberLabel();
        }

        private void htbRegisters_TextChanged(object sender, EventArgs e)
        {
            updateRegisterNumberLabel();
        }

        private void htbRegisters_VScroll(object sender, EventArgs e)
        {
            updateRegisterNumberLabel();
        }

        private void htbRegisters_Resize(object sender, EventArgs e)
        {
            updateRegisterNumberLabel();
        }
        #endregion

        #region RegisterMemoryControl
        private void btnLoadRegister_Click(object sender, EventArgs e)
        {
            if (htbRegisters.Modified)
            {
                if (MessageBox.Show("The current document has not been saved, would you like to continue without saving?", "Unsaved Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                else
                {
                    RegisterOpenFile();
                }
            }
            else
            {
                RegisterOpenFile();
            }
        }

        private void RegisterOpenFile()
        {
            openFileDialog1.Title = "MiniDLX - Open File";
            openFileDialog1.DefaultExt = "reg";
            openFileDialog1.Filter = "DLX Register Files|*.reg|All Files|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.FileName = "";

            if ((openFileDialog1.ShowDialog() != DialogResult.OK) || (openFileDialog1.FileName == ""))
            {
                return;
            }
            RegistersLoadFile(openFileDialog1.FileName);
        }

        private void RegistersLoadFile(string p)
        {
            FileStream fs = new FileStream(p, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string s = sr.ReadToEnd();
            sr.Close();
            fs.Close();

            StringReader sra = new StringReader(s);
            string nextLine = "";
            nextLine = sra.ReadLine();//Skip first line
            nextLine = sra.ReadLine();
            int idfs = nextLine.IndexOf(@"\fs20 ");
            rtbPipelineMap.Text += idfs + "\n";
            nextLine = nextLine.Substring(idfs + 6);
            Int32 i = 0;
            while (true)
            {
                if (nextLine[0] == '}') break;// End Of File

                nextLine = nextLine.Substring(0, 11);

                string[] tokens = nextLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string svar in tokens)
                {
                    R[i] = Convert.ToByte("0x" + svar, 16);
                    i++;
                }
                nextLine = sra.ReadLine();
                if (nextLine == null) break; // End Of File
            }
            htbRegisters.UpdateDisplay();
        }

        private void btnSaveRegister_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "MiniDLX - Save File";
            saveFileDialog1.DefaultExt = "reg";
            saveFileDialog1.Filter = "DLX Register Files|*.reg|All Files|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = "";

            if ((saveFileDialog1.ShowDialog() != DialogResult.OK) || (saveFileDialog1.FileName == ""))
            {
                return;
            }

            htbRegisters.SaveFile(saveFileDialog1.FileName);
        }

        private void btnClearRegister_Click(object sender, EventArgs e)
        {
            htbRegisters.NewData(31 * 4);
            R = htbRegisters.ArrayData;
            htbRegisters.UpdateDisplay();
        }

        private void btnClearMemory_Click(object sender, EventArgs e)
        {

            htbMemory.NewData(65536);
            DataMem = htbMemory.ArrayData;
            htbMemory.UpdateDisplay();
        }

        private void btnLoadMemory_Click(object sender, EventArgs e)
        {
            if (htbMemory.Modified)
            {
                if (MessageBox.Show("The current document has not been saved, would you like to continue without saving?", "Unsaved Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                else
                {
                    MemoryOpenFile();
                }
            }
            else
            {
                MemoryOpenFile();
            }
        }

        private void MemoryOpenFile()
        {
            openFileDialog1.Title = "MiniDLX - Open File";
            openFileDialog1.DefaultExt = "mem";
            openFileDialog1.Filter = "DLX Memory Files|*.mem|All Files|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.FileName = "";

            if ((openFileDialog1.ShowDialog() != DialogResult.OK) || (openFileDialog1.FileName == ""))
            {
                return;
            }
            MemoryLoadFile(openFileDialog1.FileName);
            htbMemory.UpdateDisplay();
        }

        private void MemoryLoadFile(string p)
        {
            FileStream fs = new FileStream(p, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string s = sr.ReadToEnd();
            sr.Close();
            fs.Close();

            StringReader sra = new StringReader(s);
            string nextLine = "";
            nextLine = sra.ReadLine();//Skip first line
            nextLine = sra.ReadLine();
            int idfs = nextLine.IndexOf(@"\fs20 ");
            rtbPipelineMap.Text += idfs + "\n";
            nextLine = nextLine.Substring(idfs + 6);
            Int32 i = 0;
            while (true)
            {
                if (nextLine[0] == '}') break;// End Of File

                nextLine = nextLine.Substring(0, 23);

                string[] tokens = nextLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string svar in tokens)
                {
                    DataMem[i] = Convert.ToByte("0x" + svar, 16);
                    i++;
                }
                nextLine = sra.ReadLine();
                if (nextLine == null) break; // End Of File
            }
            htbMemory.UpdateDisplay();
        }

        private void btnSaveMemory_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "MiniDLX - Save File";
            saveFileDialog1.DefaultExt = "mem";
            saveFileDialog1.Filter = "DLX Memory Files|*.mem|All Files|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = "";

            if ((saveFileDialog1.ShowDialog() != DialogResult.OK) || (saveFileDialog1.FileName == ""))
            {
                return;
            }

            htbMemory.SaveFile(saveFileDialog1.FileName);

        }
        #endregion

        #region CPUUtility
        private UInt32 SignExtend16(UInt32 v)
        {
            UInt32 result;

            if ((v & 0x8000) != 0)
            {
                result = v | 0xffff0000;
            }
            else
            {
                result = v & 0x0000ffff;
            }
            return result;
        }

        private UInt32 RegR(UInt32 i)
        {
            if (i == 0)
            {
                return 0;
            }
            else
            {
                i = (i - 1) * 4;

                UInt32 t0 = (UInt32)R[i++] << 24;
                UInt32 t1 = (UInt32)R[i++] << 16;
                UInt32 t2 = (UInt32)R[i++] << 8;
                UInt32 t3 = (UInt32)R[i++];
                return t0 + t1 + t2 + t3;
            }
        }

        private UInt32 MemR(UInt32 i)
        {
            UInt32 t0 = (UInt32)DataMem[i++] << 24;
            UInt32 t1 = (UInt32)DataMem[i++] << 16;
            UInt32 t2 = (UInt32)DataMem[i++] << 8;
            UInt32 t3 = (UInt32)DataMem[i++];
            return t0 + t1 + t2 + t3;
        }

        private void RegW(UInt32 i, UInt32 val)
        {
            if (i == 0)
            {
                return;
            }
            else
            {
                i = (i - 1) * 4;
                R[i++] = (byte)((val & 0xff000000) >> 24);
                R[i++] = (byte)((val & 0x00ff0000) >> 16);
                R[i++] = (byte)((val & 0x0000ff00) >> 8);
                R[i++] = (byte)(val & 0x000000ff);
            }
        }

        private void MemW(UInt32 i, UInt32 val)
        {

            DataMem[i++] = (byte)((val & 0xff000000) >> 24);
            DataMem[i++] = (byte)((val & 0x00ff0000) >> 16);
            DataMem[i++] = (byte)((val & 0x0000ff00) >> 8);
            DataMem[i++] = (byte)(val & 0x000000ff);
        }

        #endregion

        #region CPUCycle
        private Int32 WB_Cycle(Int32 pipeRow)
        {
            rtbSimulator.Text += "-- WB" + CycleCounter + " " + WBStatus.ToString() + " --\n";
            switch (WBStatus)
            {
                case CycleStatus.Run:
                    //Update PipelineMap
                    pipeMap[pipeRow] += "|WB |";

                    PipeLineMap wb = new PipeLineMap();
                    wb.pen = new Pen(Color.Black);
                    wb.name = "WB";
                    wb.brush = Brushes.Blue;
                    wb.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                    wb.forwardingFlag = false;
                    pipeLineMap.Add(wb);

                    pipeRow++;
                    UInt32 iRs = 0;
                    UInt32 opCode = MEM_WB.IR & 0xFC000000;
                    switch (opCode)
                    {
                        case 0x00000000: //register-register(ADD,SUB,OR,XOR,SLT)
                            iRs = (MEM_WB.IR >> 11) & 0x1f;
                            RegW(iRs, MEM_WB.ALUOutput);
                            break;
                        case 0x2C000000: //register-immediate(ADDI)
                            iRs = (MEM_WB.IR >> 16) & 0x1f;
                            RegW(iRs, MEM_WB.ALUOutput);
                            break;
                        case 0x24000000: //load(LW)
                            iRs = (MEM_WB.IR >> 16) & 0x1f;
                            RegW(iRs, MEM_WB.LMD);
                            break;
                        case 0x20000000: //BNEZ
                            IDStatus = CycleStatus.Run;
                            break;
                        case 0x80000000: //J
                            IDStatus = CycleStatus.Run;
                            break;
                        default:
                            //do nothing for other type instruction (SW,BNEZ,J)
                            break;
                    }
                    GpipeRow++;
                    break;
                case CycleStatus.Stop:
                    break;
                case CycleStatus.Stall:
                    MessageBox.Show("WB cannot be in STALL state");
                    break;
                case CycleStatus.Flush:
                    //Update PipelineMap to Flush
                    pipeMap[pipeRow] += "|*U*|";

                    PipeLineMap wb1 = new PipeLineMap(); ;
                    wb1.pen = new Pen(Color.Black);
                    wb1.name = "FLUSH";
                    wb1.brush = Brushes.Cyan;
                    wb1.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                    wb1.forwardingFlag = false;
                    pipeLineMap.Add(wb1);

                    pipeRow++;
                    GpipeRow++;
                    break;
                default:
                    break;
            }
            return pipeRow;
        }

        private Int32 MEM_Cycle(Int32 pipeRow)
        {
            rtbSimulator.Text += "-- MEM" + CycleCounter + " " + MEMStatus.ToString() + " --\n";
            switch (MEMStatus)
            {
                case CycleStatus.Run:
                    //Update PipelineMap
                    pipeMap[pipeRow] += "|MEM";

                    PipeLineMap mem = new PipeLineMap(); ;
                    mem.pen = new Pen(Color.Black);
                    mem.name = "MEM";
                    mem.brush = Brushes.Green;
                    mem.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                    mem.forwardingFlag = false;
                    pipeLineMap.Add(mem);

                    pipeRow++;

                    UInt32 opCode = EX_MEM.IR & 0xFC000000;
                    switch (opCode)
                    {
                        case 0x24000000: //LW
                            MEM_WB.LMD = MemR(EX_MEM.ALUOutput);
                            break;
                        case 0x28000000: //SW
                            MemW(EX_MEM.ALUOutput, EX_MEM.B);
                            break;
                        default:
                            break;
                    }
                    rtbSimulator.Text += " MEM_WB.LMD = " + String.Format("{0:x8}", MEM_WB.LMD) + "\n";

                    MEM_WB.IR = EX_MEM.IR;
                    rtbSimulator.Text += " MEM_WB.IR = " + String.Format("{0:x8}", MEM_WB.IR) + "\n";
                    MEM_WB.ALUOutput = EX_MEM.ALUOutput;
                    rtbSimulator.Text += " MEM_WB.ALUOutput = " + String.Format("{0:x8}", MEM_WB.ALUOutput) + "\n";
                    MEM_WB.A = EX_MEM.A;
                    rtbSimulator.Text += " MEM_WB.A = " + String.Format("{0:x8}", MEM_WB.A) + "\n";
                    MEM_WB.B = EX_MEM.B;
                    rtbSimulator.Text += " MEM_WB.B = " + String.Format("{0:x8}", MEM_WB.B) + "\n";
                    MEM_WB.Cond = EX_MEM.Cond;
                    rtbSimulator.Text += " MEM_WB.Cond = " + MEM_WB.Cond + "\n";

                    WBStatus = CycleStatus.Run; //tidak lihat status WB Dulu???
                    break;
                case CycleStatus.Stop:
                    WBStatus = CycleStatus.Stop;
                    if (EXStatus != CycleStatus.Stop)
                    {
                        //pipeRow++;
                    }
                    break;
                case CycleStatus.Stall:
                    MessageBox.Show("MEM cannot be in STALL state");
                    break;
                case CycleStatus.Flush:
                    //Update PipelineMap to Flush
                    pipeMap[pipeRow] += "|*U*";

                    PipeLineMap mem1 = new PipeLineMap(); ;
                    mem1.pen = new Pen(Color.Black);
                    mem1.name = "FLUSH";
                    mem1.brush = Brushes.Cyan;
                    mem1.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                    mem1.forwardingFlag = false;
                    pipeLineMap.Add(mem1);

                    pipeRow++;

                    //Flushing and Update Next Cycle to Flush
                    MEM_WB.Clear();
                    WBStatus = CycleStatus.Flush;
                    break;
                default:
                    break;
            }
            return pipeRow;
        }

        private Int32 EX_Cycle(Int32 pipeRow)
        {
            rtbSimulator.Text += "-- EX" + CycleCounter + " " + EXStatus.ToString() + " --\n";
            switch (EXStatus)
            {
                case CycleStatus.Run:
                    //Update PipelineMap
                    pipeMap[pipeRow] += "|EX ";

                    PipeLineMap ex = new PipeLineMap(); ;
                    ex.pen = new Pen(Color.Black);
                    ex.name = "EX";
                    ex.brush = Brushes.Yellow;
                    ex.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                    ex.forwardingFlag = false;
                    pipeLineMap.Add(ex);

                    pipeRow++;

                    EX_MEM.Cond = false;

                    UInt32 opCode = ID_EX.IR & 0xFC000000;
                    switch (opCode)
                    {
                        case 0x00000000: //register-register(ADD,SUB,OR,XOR,SLT)

                            UInt32 func = ID_EX.IR & 0xF;
                            switch (func)
                            {
                                case 0x00000000: //ADD
                                    EX_MEM.ALUOutput = (UInt32)((Int32)ID_EX.A + (Int32)ID_EX.B);
                                    break;
                                case 0x00000002: //SUB
                                    EX_MEM.ALUOutput = (UInt32)((Int32)ID_EX.A - (Int32)ID_EX.B);
                                    break;
                                case 0x00000004: //OR
                                    EX_MEM.ALUOutput = ID_EX.A | ID_EX.B;
                                    break;
                                case 0x00000001: //XOR
                                    EX_MEM.ALUOutput = ID_EX.A ^ ID_EX.B;
                                    break;
                                case 0x00000003: //SLT
                                    if ((Int32)(ID_EX.A) < (Int32)(ID_EX.B))
                                    {
                                        EX_MEM.ALUOutput = 1;
                                    }
                                    else
                                    {
                                        EX_MEM.ALUOutput = 0;
                                    }
                                    break;
                                default:
                                    //ERROR... UNRECOGNIZE MNEMONIC
                                    EX_MEM.ALUOutput = 0;
                                    break;
                            }
                            break;
                        case 0x2C000000: //register-immediate(ADDI)
                            EX_MEM.ALUOutput = (UInt32)((Int32)ID_EX.A + (Int32)ID_EX.Imm);
                            break;
                        case 0x24000000: //LW
                            EX_MEM.ALUOutput = (UInt32)((Int32)ID_EX.A + (Int32)ID_EX.Imm);
                            break;
                        case 0x28000000: //SW
                            EX_MEM.ALUOutput = (UInt32)((Int32)ID_EX.A + (Int32)ID_EX.Imm);
                            break;
                        case 0x20000000: //BNEZ                            
                            EX_MEM.Cond = true;
                            if (EX_MEM.A != 0)
                            {
                                EX_MEM.ALUOutput = (UInt32)((Int32)ID_EX.NPC + (Int32)ID_EX.Imm);
                            }
                            else
                            {
                                EX_MEM.ALUOutput = ID_EX.NPC;
                            }
                            break;
                        case 0x80000000: //J
                            EX_MEM.ALUOutput = (UInt32)((Int32)ID_EX.NPC + (Int32)ID_EX.Imm);
                            EX_MEM.Cond = true;
                            break;
                        default:
                            break;
                    }

                    rtbSimulator.Text += " EX_MEM.ALUOutput = " + String.Format("{0:x8}", EX_MEM.ALUOutput) + "\n";
                    rtbSimulator.Text += " EX_MEM.Cond = " + EX_MEM.Cond + "\n";

                    EX_MEM.IR = ID_EX.IR;
                    rtbSimulator.Text += " EX_MEM.IR = " + String.Format("{0:x8}", EX_MEM.IR) + "\n";
                    EX_MEM.A = ID_EX.A;
                    rtbSimulator.Text += " EX_MEM.A = " + String.Format("{0:x8}", EX_MEM.A) + "\n";
                    EX_MEM.B = ID_EX.B;
                    rtbSimulator.Text += " EX_MEM.B = " + String.Format("{0:x8}", EX_MEM.B) + "\n";
                    

                    MEMStatus = CycleStatus.Run;
                    break;
                case CycleStatus.Stop:
                    MEMStatus = CycleStatus.Stop;
                    break;
                case CycleStatus.Stall:
                    //Update PipelineMap to Stall
                    pipeMap[pipeRow] += "|***";

                    PipeLineMap ex1 = new PipeLineMap(); ;
                    ex1.pen = new Pen(Color.Black);
                    ex1.name = "STALL";
                    ex1.brush = Brushes.Brown;
                    ex1.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                    ex1.forwardingFlag = false;
                    pipeLineMap.Add(ex1);

                    pipeRow++;

                    //Just one cycle stall
                    EXStatus = CycleStatus.Run;

                    //Set All Level below to Stall
                    IDStatus = CycleStatus.Stall;
                    IFStatus = CycleStatus.Stall;
                    StallCounter++;
                    //Delay
                    MEMStatus = CycleStatus.Stop;
                    break;
                case CycleStatus.Flush:
                    //Update PipelineMap to Flush
                    pipeMap[pipeRow] += "|*U*";

                    PipeLineMap ex2 = new PipeLineMap();
                    ex2.pen = new Pen(Color.Black);
                    ex2.name = "FLUSH";
                    ex2.brush = Brushes.Cyan;
                    ex2.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                    ex2.forwardingFlag = false;
                    pipeLineMap.Add(ex2);

                    pipeRow++;

                    //Flushing and Update Next Cycle to Flush
                    EX_MEM.Clear();
                    MEMStatus = CycleStatus.Flush;
                    break;
                default:
                    break;
            }
            return pipeRow;
        }

        private Int32 ID_Cycle(Int32 pipeRow)
        {
            rtbSimulator.Text += "-- ID" + CycleCounter + " " + IDStatus.ToString() + " --\n";
            UInt32 opCode;
            switch (IDStatus)
            {
                case CycleStatus.Run:
                    //Update PipelineMap
                    pipeMap[pipeRow] += "|ID ";

                    PipeLineMap id = new PipeLineMap();
                    id.pen = new Pen(Color.Black);
                    id.name = "ID";
                    id.brush = Brushes.Pink;
                    id.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                    pipeLineMap.Add(id);

                    pipeRow++;

                    
                    opCode = IF_ID.IR & 0xFC000000;
                    //rtbSimulator.Text += " opCode = " + String.Format("{0:x8}", opCode) + "\n";
                    if ((opCode == 0x20000000) | (opCode == 0x80000000)) //BNEZ or J
                    {
                        IDStatus = CycleStatus.Flush;
                        rtbSimulator.Text += " NEXT_IDStatus = " + IDStatus + "\n";
                    }

                    //A = Reg[IR6..10]
                    UInt32 rs1 = (IF_ID.IR >> 21) & 0x1f;
                    ID_EX.A = RegR(rs1);
                    rtbSimulator.Text += " ID_EX.A = R(" + rs1 + ") = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                    //B = Reg[IR11..15]
                    UInt32 rs2 = (IF_ID.IR >> 16) & 0x1f;
                    ID_EX.B = RegR(rs2);
                    rtbSimulator.Text += " ID_EX.B = R(" + rs2 + ") = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                    //Imm= (IR16)^16##IR16.31
                    ID_EX.Imm = IF_ID.IR & 0x0000ffff;
                    ID_EX.Imm = SignExtend16(ID_EX.Imm);
                    rtbSimulator.Text += " ID_EX.Imm = " + String.Format("{0:x8}", ID_EX.Imm) + "\n";

                    opCode = ID_EX.IR & 0xFC000000;
                    rtbSimulator.Text += " ID_EX.IR.opCode = " + String.Format("{0:x8}", opCode) + "\n";
                    UInt32 rd = (ID_EX.IR >> 16) & 0x1f;
                    rtbSimulator.Text += " ID_EX.IR.rd = " + String.Format("{0:x8}", rd) + "\n";

                    ID_EX.IR = IF_ID.IR;
                    rtbSimulator.Text += " ID_EX.IR = " + String.Format("{0:x8}", ID_EX.IR) + "\n";

                    ID_EX.NPC = IF_ID.NPC;
                    rtbSimulator.Text += " ID_EX.NPC = " + String.Format("{0:x4}", ID_EX.NPC) + "\n";

                    #region Forwarding
                    UInt32 opCodeEX_MEMIR, opCodeMEM_WBIR;
                    //ambil rs1&r2 dari ID_EX.IR                            
                    rs1 = (ID_EX.IR >> 21) & 0x1f;
                    rtbSimulator.Text += " *rs1 = " + rs1 + "\n";
                    rs2 = (ID_EX.IR >> 16) & 0x1f;
                    rtbSimulator.Text += " *rs2 = " + rs2 + "\n";

                    //ambil EX_MEM.IR chek opCodeEX_MEM.IR
                    opCodeEX_MEMIR = EX_MEM.IR & 0xFC000000;
                    rtbSimulator.Text += " *EX_MEM.IR = " + String.Format("{0:x8}", EX_MEM.IR) + "\n";
                    int idf = 0;
                    switch (opCodeEX_MEMIR)
                    {
                        case 0x00000000: //register-register(ADD,SUB,OR,XOR,SLT)
                            //if opCodeEX_MEM.IR==ALU_R-R then rd = EX_MEM.IR16-20
                            rd = (EX_MEM.IR >> 11) & 0x1f;
                            rtbSimulator.Text += " *ARRrd = " + rs1 + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = EX_MEM.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                idf = pipeLineMap.Count - 2;
                                ((PipeLineMap)(pipeLineMap[idf])).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = EX_MEM.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 2]).forwardingFlag = true;

                            }
                            break;
                        case 0x2C000000: //register-immediate(ADDI)
                            //if opCodeEX_MEM.IR==ALU_R-I then rd = EX_MEM.IR11-15
                            rd = (EX_MEM.IR >> 16) & 0x1f;
                            rtbSimulator.Text += " *ARIrd = " + rd + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = EX_MEM.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 2]).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = EX_MEM.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 2]).forwardingFlag = true;
                            }
                            break;
                        case 0x24000000: //LW
                            //if opCodeEX_MEM.IR==LW then rd = EX_MEM.IR11-15
                            rd = (EX_MEM.IR >> 16) & 0x1f;
                            rtbSimulator.Text += " *LWrd = " + rd + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = EX_MEM.LMD;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 2]).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = EX_MEM.LMD;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 2]).forwardingFlag = true;
                            }
                            break;

                        default:
                            break;
                    }

                    //ambil MEM_WB.IR chek opCodeMEM_WB.IR
                    opCodeMEM_WBIR = MEM_WB.IR & 0xFC000000;
                    rtbSimulator.Text += " *MEM_WB.IR = " + String.Format("{0:x8}", MEM_WB.IR) + "\n";
                    switch (opCodeMEM_WBIR)
                    {
                        case 0x00000000: //register-register(ADD,SUB,OR,XOR,SLT)
                            //if opCodeMEM_WB.IR==ALU_R-R then rd = MEM_WB.IR16-20
                            rd = (MEM_WB.IR >> 11) & 0x1f;
                            rtbSimulator.Text += " *ARRrd = " + rd + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = MEM_WB.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = MEM_WB.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            break;
                        case 0x2C000000: //register-immediate(ADDI)
                            //if opCodeMEM_WB.IR==ALU_R-I then rd = MEM_WB.IR11-15
                            rd = (MEM_WB.IR >> 16) & 0x1f;
                            rtbSimulator.Text += " *ARIrd = " + rd + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = MEM_WB.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = MEM_WB.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            break;
                        case 0x24000000: //LW
                            //if opCodeMEM_WB.IR==LW then rd = MEM_WB.IR11-15
                            rd = (MEM_WB.IR >> 16) & 0x1f;
                            rtbSimulator.Text += " *LWrd = " + rd + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = MEM_WB.LMD;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = MEM_WB.LMD;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion

                    if ((rs1 != 0) && (opCode == 0x24000000) && (rd == rs1)) //load and destinatation equal with source
                    {
                        EXStatus = CycleStatus.Stall;
                        rtbSimulator.Text += " NEXT_EXStatus = " + EXStatus + "\n";
                        break;
                    }

                    if ((rs2 != 0) && (opCode == 0x24000000) && (rd == rs2)) //load and destinatation equal with source
                    {
                        EXStatus = CycleStatus.Stall;
                        rtbSimulator.Text += " NEXT_EXStatus = " + EXStatus + "\n";
                        break;
                    }

                    
                    
                    EXStatus = CycleStatus.Run;
                    break;
                case CycleStatus.Stop:
                    EXStatus = CycleStatus.Stop;
                    break;
                case CycleStatus.Stall:
                    //Update PipelineMap to Stall
                    pipeMap[pipeRow] += "|***";

                    PipeLineMap id1= new PipeLineMap();
                    id1.pen = new Pen(Color.Black);
                    id1.name = "STALL";
                    id1.brush = Brushes.Brown;
                    id1.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                    pipeLineMap.Add(id1);

                    pipeRow++;

                    #region Forwarding
                    
                    //ambil rs1&r2 dari ID_EX.IR                            
                    rs1 = (ID_EX.IR >> 21) & 0x1f;
                    rtbSimulator.Text += " *rs1 = " + rs1 + "\n";
                    rs2 = (ID_EX.IR >> 16) & 0x1f;
                    rtbSimulator.Text += " *rs2 = " + rs2 + "\n";

                    //ambil EX_MEM.IR chek opCodeEX_MEM.IR
                    opCodeEX_MEMIR = EX_MEM.IR & 0xFC000000;
                    rtbSimulator.Text += " *EX_MEM.IR = " + String.Format("{0:x8}", EX_MEM.IR) + "\n";
                    switch (opCodeEX_MEMIR)
                    {
                        case 0x00000000: //register-register(ADD,SUB,OR,XOR,SLT)
                            //if opCodeEX_MEM.IR==ALU_R-R then rd = EX_MEM.IR16-20
                            rd = (EX_MEM.IR >> 11) & 0x1f;
                            rtbSimulator.Text += " *ARRrd = " + rs1 + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = EX_MEM.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 2]).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = EX_MEM.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 2]).forwardingFlag = true;

                            }
                            break;
                        case 0x2C000000: //register-immediate(ADDI)
                            //if opCodeEX_MEM.IR==ALU_R-I then rd = EX_MEM.IR11-15
                            rd = (EX_MEM.IR >> 16) & 0x1f;
                            rtbSimulator.Text += " *ARIrd = " + rd + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = EX_MEM.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 2]).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = EX_MEM.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 2]).forwardingFlag = true;
                            }
                            break;
                        case 0x24000000: //LW
                            //if opCodeEX_MEM.IR==LW then rd = EX_MEM.IR11-15
                            rd = (EX_MEM.IR >> 16) & 0x1f;
                            rtbSimulator.Text += " *LWrd = " + rd + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = EX_MEM.LMD;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 2]).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = EX_MEM.LMD;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 2]).forwardingFlag = true;
                            }
                            break;

                        default:
                            break;
                    }

                    //ambil MEM_WB.IR chek opCodeMEM_WB.IR
                    opCodeMEM_WBIR = MEM_WB.IR & 0xFC000000;
                    rtbSimulator.Text += " *MEM_WB.IR = " + String.Format("{0:x8}", MEM_WB.IR) + "\n";
                    switch (opCodeMEM_WBIR)
                    {
                        case 0x00000000: //register-register(ADD,SUB,OR,XOR,SLT)
                            //if opCodeMEM_WB.IR==ALU_R-R then rd = MEM_WB.IR16-20
                            rd = (MEM_WB.IR >> 11) & 0x1f;
                            rtbSimulator.Text += " *ARRrd = " + rd + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = MEM_WB.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = MEM_WB.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            break;
                        case 0x2C000000: //register-immediate(ADDI)
                            //if opCodeMEM_WB.IR==ALU_R-I then rd = MEM_WB.IR11-15
                            rd = (MEM_WB.IR >> 16) & 0x1f;
                            rtbSimulator.Text += " *ARIrd = " + rd + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = MEM_WB.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = MEM_WB.ALUOutput;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            break;
                        case 0x24000000: //LW
                            //if opCodeMEM_WB.IR==LW then rd = MEM_WB.IR11-15
                            rd = (MEM_WB.IR >> 16) & 0x1f;
                            rtbSimulator.Text += " *LWrd = " + rd + "\n";
                            if ((rs1 != 0) && (rd == rs1))
                            {
                                ID_EX.A = MEM_WB.LMD;
                                rtbSimulator.Text += " *ID_EX.A = " + String.Format("{0:x8}", ID_EX.A) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            if ((rs2 != 0) && (rd == rs2))
                            {
                                ID_EX.B = MEM_WB.LMD;
                                rtbSimulator.Text += " *ID_EX.B = " + String.Format("{0:x8}", ID_EX.B) + "\n";
                                //Mark the Flag Forwarding Here and record the point
                                ((PipeLineMap)pipeLineMap[pipeLineMap.Count - 3]).forwardingFlag = true;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion

                    //Just one cycle stall
                    IDStatus = CycleStatus.Run;
                    break;
                case CycleStatus.Flush:
                    //Update PipelineMap to Flush
                    pipeMap[pipeRow] += "|*U*";

                    PipeLineMap id2 = new PipeLineMap();
                    id2.pen = new Pen(Color.Black);
                    id2.name = "FLUSH";
                    id2.brush = Brushes.Cyan;
                    id2.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                    pipeLineMap.Add(id2);

                    pipeRow++;

                    //Flushing and Update Next Cycle to Flush
                    ID_EX.Clear();
                    EXStatus = CycleStatus.Flush;
                    break;
                default:
                    break;
            }
            return pipeRow;
        }

        private Int32 IF_Cycle(Int32 pipeRow)
        {
            rtbSimulator.Text += "-- IF" + CycleCounter + " " + IFStatus.ToString() + " --\n";
            switch (IFStatus)
            {
                case CycleStatus.Run:
                    //Update PipelineMap
                    if (pipeMap[pipeRow] != null)
                    {
                        pipeMap[pipeRow] += "|IF ";                        
                        PipeLineMap if1 = new PipeLineMap();
                        if1.pen = new Pen(Color.Black);
                        if1.name = "IF";
                        if1.brush = Brushes.Red;
                        if1.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                        pipeLineMap.Add(if1);
                        
                        pipeMap[pipeRow] = lsbCode.SelectedItem.ToString().Substring(0, 4) + pipeMap[pipeRow].Substring(4);
                    }
                    else
                    {
                        PipeLineMap addressBox = new PipeLineMap();
                        addressBox.name = lsbCode.SelectedItem.ToString().Substring(0, 4);
                        addressBox.point = new Point(0, pipeRow * boxHeight + verticalDis);
                        addressBox.pen = new Pen(Color.Black);
                        addressBox.brush = Brushes.White;
                        pipeLineMap.Add(addressBox);
                        PipeLineMap if1 = new PipeLineMap();

                        if1.pen = new Pen(Color.Black);
                        if1.name = "IF";
                        if1.brush = Brushes.Red;
                        if1.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                        pipeLineMap.Add(if1);

                        pipeMap[pipeRow] = lsbCode.SelectedItem.ToString().Substring(0, 4) + " " + GenerateBlank(pipeRow+StallCounter) + "|IF ";
                    }
                    IF_ID.IR = Convert.ToUInt32(codeMemory[PC / 4], 2);

                    rtbSimulator.Text += " IF_ID.IR = " + String.Format("{0:x8}", IF_ID.IR) + "\n";
                    rtbSimulator.Text += " EX_MEM.Cond = " + EX_MEM.Cond + "\n";
                    if (EX_MEM.Cond)
                    {
                        PC = IF_ID.NPC = EX_MEM.ALUOutput;
                        rtbSimulator.Text += " JUMP" + "PC = IF_ID.NPC = " + String.Format("{0:x4}", PC) + "\n";                        
                    }
                    else
                    {
                        PC = IF_ID.NPC = PC + 4;
                        rtbSimulator.Text += " PC = IF_ID.NPC = " + String.Format("{0:x4}", PC) + "\n";
                    }

                    //Update Status
                    if (IDStatus == CycleStatus.Stop)
                    {
                        IDStatus = CycleStatus.Run;
                    }
                    //Peek to the next instruction
                    if ((PC / 4) == codeMemory.Length) //NO MORE INSTRUCTION
                    {
                        IFStatus = CycleStatus.Stop;
                        PC = PC - 4;
                    }
                    break;
                case CycleStatus.Stop:
                    //Update NEXT ID Status
                    IDStatus = CycleStatus.Stop;
                    break;
                case CycleStatus.Stall:
                    //Update PipelineMap to Stall
                    pipeMap[pipeRow] = lsbCode.SelectedItem.ToString().Substring(0, 4) + " " + GenerateBlank(pipeRow) + "|***";
                    PipeLineMap addressBox2 = new PipeLineMap();
                    addressBox2.pen = new Pen(Color.Black);
                    addressBox2.name = lsbCode.SelectedItem.ToString().Substring(0, 4);
                    addressBox2.brush = Brushes.White;
                    addressBox2.point = new Point(0, pipeRow * boxHeight + verticalDis);
                    pipeLineMap.Add(addressBox2);
                    PipeLineMap if2 = new PipeLineMap();
                    if2.pen = new Pen(Color.Black);
                    if2.name = "STALL";
                    if2.brush = Brushes.Brown;
                    if2.point = new Point(counterBox * boxWidth, pipeRow * boxHeight + verticalDis);
                    pipeLineMap.Add(if2);
 
                    pipeRow++;

                    //Just one cycle stall                    
                    IFStatus = CycleStatus.Run;
                    break;
                case CycleStatus.Flush:
                    MessageBox.Show("IF cannot be in FLUSH state");
                    break;
                default:
                    break;
            }
            return pipeRow;
        }
        #endregion

        private void btnStep_Click(object sender, EventArgs e)
        {
            rtbSimulator.Text += "-------- " + String.Format("{0:0000}", CycleCounter) + " --------\n";
            lblPC.Text = "PC : " + String.Format("{0:X4}", PC) + "h";
            lblCycle.Text = "Cycle #: " + String.Format("{0:0000}", CycleCounter);
            lsbCode.SelectedIndex = (Int32)PC / 4;
            //Execute 1 cyle (5 pipeline)
            IF_Cycle(ID_Cycle(EX_Cycle(MEM_Cycle(WB_Cycle(GpipeRow)))));

            //Update Pipeline Map
            rtbPipelineMap.Lines = pipeMap;
            rtbPipelineMap.Refresh();
            //panel1.Refresh();
            pictureBox1.Refresh();
            //Update Memory and Register
            htbMemory.UpdateDisplay();
            htbRegisters.UpdateDisplay();
            //Find out how to set focus to last line of rtbSimulator and Pipeline
            counterBox++;
            CycleCounter++;

            if ((IFStatus == CycleStatus.Stop) && (IDStatus == CycleStatus.Stop) && (EXStatus == CycleStatus.Stop) && (MEMStatus == CycleStatus.Stop) && (WBStatus == CycleStatus.Stop))
            {
                btnStep.Enabled = false;
                btnStart.Enabled = false;
            }
        }

        private string GenerateBlank(int pipeRow)
        {
            string t = "";
            for (int i = 0; i < pipeRow; i++)
            {
                t += "|   ";
            }
            return t;
        }

        private void frmSimulator_Load(object sender, EventArgs e)
        {
            btnReset.PerformClick();
            MemoryNumber.Font = new Font(htbMemory.Font.FontFamily, htbMemory.Font.Size);
            label1.Font = new Font(htbMemory.Font.FontFamily, htbMemory.Font.Size);
            RegisterNumber.Font = new Font(htbRegisters.Font.FontFamily, htbMemory.Font.Size);
            //PipeLineMapCounter = 0;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //PipeLineMapCounter = 0;
            counterBox = 1;
            GpipeRow = 0;
            StallCounter = 0;
            PC = 0;
            CycleCounter = 0;
            lblPC.Text = "PC : " + String.Format("{0:X4}", PC) + "h";
            lblCycle.Text = "Cycle #: " + String.Format("{0:0000}", CycleCounter);
            IFStatus = CycleStatus.Run;
            IDStatus = CycleStatus.Stop;
            EXStatus = CycleStatus.Stop;
            MEMStatus = CycleStatus.Stop;
            WBStatus = CycleStatus.Stop;
            IF_ID.Clear();
            ID_EX.Clear();
            EX_MEM.Clear();
            MEM_WB.Clear();
            btnClearMemory.PerformClick();
            btnClearRegister.PerformClick();
            lsbCode.SelectedIndex = (Int32)PC / 4;
            rtbPipelineMap.Clear();
            rtbSimulator.Clear();
            pipeMap = new string[1024];
            btnStep.Enabled = true;
            btnStart.Enabled = true;
            pipeLineMap = new ArrayList();
            pictureBox1.Refresh();
            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            foreach (PipeLineMap pipe in pipeLineMap)
            {
                Brush b1;
                b1 = Brushes.Black;
                Font newFont = new Font(FontFamily.GenericSerif, 10);

                Rectangle r1 = new Rectangle(pipe.point.X, pipe.point.Y, boxWidth , boxHeight);
                e.Graphics.FillRectangle(pipe.brush, r1);
                e.Graphics.DrawRectangle(pipe.pen, r1);
                e.Graphics.DrawString(pipe.name, newFont, b1, pipe.point);
                for (int i = 4; i < 8; i++)
                {
                    int itempPipe = pipeLineMap.IndexOf(pipe) - i;
                    if (itempPipe < 4)
                    {
                        continue;
                    }
                    PipeLineMap tempPipe = ((PipeLineMap)pipeLineMap[itempPipe]);                    
                    if ((pipe.name == "EX") && (tempPipe.forwardingFlag))
                    {
                        //Draw the line                        
                        Point midSource = new Point((pipe.point.X + boxWidth / 2 - 10), (pipe.point.Y + boxHeight / 2));
                        Point midDest = new Point((tempPipe.point.X + boxWidth / 2 + 10), (tempPipe.point.Y + boxHeight / 2));
                        Pen p1 = new Pen(Color.Black, 2);
                        e.Graphics.DrawLine(p1 , midSource, midDest);                        
                    }
                }
            }
            rtbSimulator.Select(rtbSimulator.Text.Length, 1);
            rtbSimulator.ScrollToCaret();
        }

        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            panel1.Refresh();      
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            simRunning = true;
            while ((simRunning) && (!((IFStatus == CycleStatus.Stop) && (IDStatus == CycleStatus.Stop) && (EXStatus == CycleStatus.Stop) && (MEMStatus == CycleStatus.Stop) && (WBStatus == CycleStatus.Stop))))
            {
                btnStep.PerformClick();
                Application.DoEvents();
                Delay(700);
            }
        }

        private void Delay(int ms)
        {
            int start = Environment.TickCount;
            while ((Environment.TickCount - start) < ms)
            {
                Application.DoEvents();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            simRunning = false;
        }

        private void lblCycle_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            
            for (int i = 0; i < 1000; i++)
            {
                
            }
        }
    }
}