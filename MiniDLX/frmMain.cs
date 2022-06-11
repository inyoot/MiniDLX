using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace MiniDLX
{
    public partial class frmMain : Form
    {
        private string currentFileName;

        public frmMain()
        {
            InitializeComponent();
            Thread th = new Thread(new ThreadStart(DoSplash));
            th.Start();
            Thread.Sleep(700);
            th.Abort();
            Thread.Sleep(1000);
            toolStripStatusLabel1.Text = rtbEdit.GetLineFromCharIndex(rtbEdit.GetFirstCharIndexOfCurrentLine()).ToString();
            toolStripStatusLabel3.Text = "";
            newToolStripMenuItem.PerformClick();
        }

        private void DoSplash()
        {
            Splash sp = new Splash();
            sp.ShowDialog();
        }
       

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbEdit.Modified)
            {
                if (MessageBox.Show("The current document has not been saved, would you like to continue without saving?", "Unsaved Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                else
                {
                    OpenFile();
                }
            }
            else
            {
                    OpenFile();
            }
        }

        private void OpenFile()
        {
            openFileDialog1.Title = "MiniDLX - Open File";
            openFileDialog1.DefaultExt = "dlx";
            openFileDialog1.Filter = "DLX Assembly Files|*.dlx|All Files|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.FileName = "";

            if ((openFileDialog1.ShowDialog() != DialogResult.OK) || (openFileDialog1.FileName == ""))
            {
                return;
            }
            
            System.IO.StreamReader txtReader;
            txtReader = new System.IO.StreamReader(openFileDialog1.FileName);
            rtbEdit.Text = txtReader.ReadToEnd();
            txtReader.Close();            
            rtbEdit.SelectionStart = 0;
            rtbEdit.SelectionLength = 0;
                
            currentFileName = openFileDialog1.FileName;
            rtbEdit.Modified = false;
            this.Text = "MiniDLX - Editor: " + currentFileName;

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbEdit.Modified)
            {

                if (MessageBox.Show("The current document has not been saved, would you like to continue without saving?", "Unsaved Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                else
                {
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFileName == "")
            {
                saveAsToolStripMenuItem_Click(this, e);
                return;
            }
            System.IO.StreamWriter txtWriter;
            txtWriter = new System.IO.StreamWriter(currentFileName);
            txtWriter.Write(rtbEdit.Text);
            txtWriter.Close();                        
            rtbEdit.Modified = false;
            this.Text = "MiniDLX - Editor: " + currentFileName;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "MiniDLX - Save File";
            saveFileDialog1.DefaultExt = "dlx";
            saveFileDialog1.Filter = "DLX Assembly Files|*.dlx|All Files|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = "";

            if ((saveFileDialog1.ShowDialog() != DialogResult.OK) || (saveFileDialog1.FileName == ""))
            {
                return;
            }
            
            StreamWriter txtWriter;
            txtWriter = new StreamWriter(saveFileDialog1.FileName);
            txtWriter.Write(rtbEdit.Text);
            txtWriter.Close();
                        
            currentFileName = saveFileDialog1.FileName;
            rtbEdit.Modified = false;
            this.Text = "MiniDLX - Editor: " + currentFileName.ToString();
        }

        private void UpdateTitle(string info)
        {

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbEdit.Modified)
            {
                if (MessageBox.Show("The current document has not been saved, would you like to continue without saving?", "Unsaved Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    rtbEdit.Clear();
                }
                else
                {
                    return;
                }
            }
            else
            {
                rtbEdit.Clear();
            }
            currentFileName = "";
            this.Text = "MiniDLX - Editor: New Document";
        }

        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            toolStripStatusLabel1.Text =  rtbEdit.GetLineFromCharIndex(rtbEdit.GetFirstCharIndexOfCurrentLine()).ToString();            
        }

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            toolStripStatusLabel1.Text = rtbEdit.GetLineFromCharIndex(rtbEdit.GetFirstCharIndexOfCurrentLine()).ToString();
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFileName == "")
            {
                MessageBox.Show("The new document must be saved first", "Unsaved Document", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (rtbEdit.Modified)
            {
                saveToolStripMenuItem.PerformClick();
            }

            Assembler asm = new Assembler();
            try
            {                
                string opcode = asm.AssemblerToBin(rtbEdit.Text);

                StreamWriter txtWriter;

                string opFileName = System.IO.Path.GetFileNameWithoutExtension(currentFileName) + ".s";
                txtWriter = new StreamWriter(opFileName);
                txtWriter.Write(opcode);
                txtWriter.Close();
                toolStripStatusLabel3.ForeColor = Color.Green;
                toolStripStatusLabel3.Text = "SUCCESS: " + opFileName + " is created";
            }
            catch (ArgumentException ex)
            {
                string[] errtoken = ex.Message.Split(',');                                
                Int32 errlinenumber = Convert.ToInt32(errtoken[1]);
                toolStripStatusLabel3.ForeColor = Color.Red;
                toolStripStatusLabel3.Text = "ERROR: " + errtoken[0];
                rtbEdit.Select(rtbEdit.GetFirstCharIndexFromLine(errlinenumber), rtbEdit.Lines[errlinenumber].Length);
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFileName == "")
            {
                MessageBox.Show("The new document must be saved and compiled first", "Unsaved Document", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (rtbEdit.Modified)
            {
                saveToolStripMenuItem.PerformClick();
            }

            Assembler asm = new Assembler();
            string opcode;
            string simcode;

            try
            {
                opcode = asm.AssemblerToBin(rtbEdit.Text);
                simcode = asm.AssemblerToSim(rtbEdit.Text);

                StreamWriter txtWriter;

                string opFileName = System.IO.Path.GetFileNameWithoutExtension(currentFileName) + ".s";
                txtWriter = new StreamWriter(opFileName);
                txtWriter.Write(opcode);
                txtWriter.Close();
                toolStripStatusLabel3.ForeColor = Color.Green;
                toolStripStatusLabel3.Text = "SUCCESS: " + opFileName + " is created";
            }
            catch (ArgumentException ex)
            {
                string[] errtoken = ex.Message.Split(',');                
                Int32 errlinenumber = Convert.ToInt32(errtoken[1]);
                rtbEdit.Select(rtbEdit.GetFirstCharIndexFromLine(errlinenumber), rtbEdit.Lines[errlinenumber].Length);
                toolStripStatusLabel1.Text = rtbEdit.GetLineFromCharIndex(rtbEdit.GetFirstCharIndexOfCurrentLine()).ToString();
                toolStripStatusLabel3.ForeColor = Color.Red;
                toolStripStatusLabel3.Text = "ERROR: " + errtoken[0];
                return;
            }

            frmSimulator fSimulator = new frmSimulator(opcode,simcode);
            fSimulator.ShowDialog(this);
        }

        private void rtbEdit_KeyDown(object sender, KeyEventArgs e)
        {
            toolStripStatusLabel3.Text = "";
        }

    }
}