namespace MiniDLX
{
    partial class frmSimulator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lsbCode = new System.Windows.Forms.ListBox();
            this.rtbPipelineMap = new System.Windows.Forms.RichTextBox();
            this.MemoryNumber = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSourceCode = new System.Windows.Forms.Label();
            this.lblPipelineMap = new System.Windows.Forms.Label();
            this.RegisterNumber = new System.Windows.Forms.Label();
            this.gbRegister = new System.Windows.Forms.GroupBox();
            this.btnClearRegister = new System.Windows.Forms.Button();
            this.btnSaveRegister = new System.Windows.Forms.Button();
            this.btnLoadRegister = new System.Windows.Forms.Button();
            this.gbMemory = new System.Windows.Forms.GroupBox();
            this.btnClearMemory = new System.Windows.Forms.Button();
            this.btnSaveMemory = new System.Windows.Forms.Button();
            this.btnLoadMemory = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnStep = new System.Windows.Forms.Button();
            this.lblPC = new System.Windows.Forms.Label();
            this.lblCycle = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.rtbSimulator = new System.Windows.Forms.RichTextBox();
            this.lblSimulator = new System.Windows.Forms.Label();
            this.gbSimulator = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.htbRegisters = new HexEdit.HexEditBox();
            this.htbMemory = new HexEdit.HexEditBox();
            this.gbRegister.SuspendLayout();
            this.gbMemory.SuspendLayout();
            this.gbSimulator.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lsbCode
            // 
            this.lsbCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lsbCode.Enabled = false;
            this.lsbCode.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lsbCode.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lsbCode.FormattingEnabled = true;
            this.lsbCode.HorizontalScrollbar = true;
            this.lsbCode.ItemHeight = 14;
            this.lsbCode.Location = new System.Drawing.Point(12, 24);
            this.lsbCode.Name = "lsbCode";
            this.lsbCode.Size = new System.Drawing.Size(232, 312);
            this.lsbCode.TabIndex = 0;
            // 
            // rtbPipelineMap
            // 
            this.rtbPipelineMap.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbPipelineMap.Location = new System.Drawing.Point(262, 40);
            this.rtbPipelineMap.Name = "rtbPipelineMap";
            this.rtbPipelineMap.ReadOnly = true;
            this.rtbPipelineMap.Size = new System.Drawing.Size(649, 277);
            this.rtbPipelineMap.TabIndex = 1;
            this.rtbPipelineMap.Text = "";
            this.rtbPipelineMap.WordWrap = false;
            // 
            // MemoryNumber
            // 
            this.MemoryNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MemoryNumber.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MemoryNumber.Location = new System.Drawing.Point(668, 41);
            this.MemoryNumber.Name = "MemoryNumber";
            this.MemoryNumber.Size = new System.Drawing.Size(50, 296);
            this.MemoryNumber.TabIndex = 7;
            this.MemoryNumber.Text = "0001\r\n2\r\n3\r\n4\r\n5\r\n6\r\n7\r\n8\r\n9\r\n10\r\n11\r\n12\r\n13\r\n14\r\n15\r\n16";
            this.MemoryNumber.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(719, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "00 01 02 03 04 05 06 07";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(721, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 18);
            this.label2.TabIndex = 10;
            this.label2.Text = "MEMORY";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(553, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 18);
            this.label3.TabIndex = 11;
            this.label3.Text = "REGISTER";
            // 
            // lblSourceCode
            // 
            this.lblSourceCode.AutoSize = true;
            this.lblSourceCode.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSourceCode.Location = new System.Drawing.Point(12, 3);
            this.lblSourceCode.Name = "lblSourceCode";
            this.lblSourceCode.Size = new System.Drawing.Size(137, 18);
            this.lblSourceCode.TabIndex = 12;
            this.lblSourceCode.Text = "SOURCE CODE";
            // 
            // lblPipelineMap
            // 
            this.lblPipelineMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPipelineMap.AutoSize = true;
            this.lblPipelineMap.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPipelineMap.Location = new System.Drawing.Point(12, 365);
            this.lblPipelineMap.Name = "lblPipelineMap";
            this.lblPipelineMap.Size = new System.Drawing.Size(137, 18);
            this.lblPipelineMap.TabIndex = 13;
            this.lblPipelineMap.Text = "PIPELINE MAP";
            // 
            // RegisterNumber
            // 
            this.RegisterNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RegisterNumber.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.RegisterNumber.Location = new System.Drawing.Point(506, 40);
            this.RegisterNumber.Name = "RegisterNumber";
            this.RegisterNumber.Size = new System.Drawing.Size(39, 297);
            this.RegisterNumber.TabIndex = 14;
            this.RegisterNumber.Text = "R1\r\nR2\r\nR3\r\nR4\r\nR5\r\nR6\r\nR7\r\nR8\r\nR9\r\nR10\r\nR11\r\nR12\r\n";
            this.RegisterNumber.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // gbRegister
            // 
            this.gbRegister.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRegister.Controls.Add(this.btnClearRegister);
            this.gbRegister.Controls.Add(this.btnSaveRegister);
            this.gbRegister.Controls.Add(this.btnLoadRegister);
            this.gbRegister.Location = new System.Drawing.Point(515, 343);
            this.gbRegister.Name = "gbRegister";
            this.gbRegister.Size = new System.Drawing.Size(157, 43);
            this.gbRegister.TabIndex = 16;
            this.gbRegister.TabStop = false;
            this.gbRegister.Text = "Register Control";
            // 
            // btnClearRegister
            // 
            this.btnClearRegister.Location = new System.Drawing.Point(104, 17);
            this.btnClearRegister.Name = "btnClearRegister";
            this.btnClearRegister.Size = new System.Drawing.Size(43, 23);
            this.btnClearRegister.TabIndex = 2;
            this.btnClearRegister.Text = "Clear";
            this.btnClearRegister.UseVisualStyleBackColor = true;
            this.btnClearRegister.Click += new System.EventHandler(this.btnClearRegister_Click);
            // 
            // btnSaveRegister
            // 
            this.btnSaveRegister.Location = new System.Drawing.Point(55, 17);
            this.btnSaveRegister.Name = "btnSaveRegister";
            this.btnSaveRegister.Size = new System.Drawing.Size(43, 23);
            this.btnSaveRegister.TabIndex = 1;
            this.btnSaveRegister.Text = "Save";
            this.btnSaveRegister.UseVisualStyleBackColor = true;
            this.btnSaveRegister.Click += new System.EventHandler(this.btnSaveRegister_Click);
            // 
            // btnLoadRegister
            // 
            this.btnLoadRegister.Location = new System.Drawing.Point(6, 17);
            this.btnLoadRegister.Name = "btnLoadRegister";
            this.btnLoadRegister.Size = new System.Drawing.Size(43, 23);
            this.btnLoadRegister.TabIndex = 0;
            this.btnLoadRegister.Text = "Load";
            this.btnLoadRegister.UseVisualStyleBackColor = true;
            this.btnLoadRegister.Click += new System.EventHandler(this.btnLoadRegister_Click);
            // 
            // gbMemory
            // 
            this.gbMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMemory.Controls.Add(this.btnClearMemory);
            this.gbMemory.Controls.Add(this.btnSaveMemory);
            this.gbMemory.Controls.Add(this.btnLoadMemory);
            this.gbMemory.Location = new System.Drawing.Point(724, 343);
            this.gbMemory.Name = "gbMemory";
            this.gbMemory.Size = new System.Drawing.Size(160, 43);
            this.gbMemory.TabIndex = 17;
            this.gbMemory.TabStop = false;
            this.gbMemory.Text = "Memory Control";
            // 
            // btnClearMemory
            // 
            this.btnClearMemory.Location = new System.Drawing.Point(106, 16);
            this.btnClearMemory.Name = "btnClearMemory";
            this.btnClearMemory.Size = new System.Drawing.Size(44, 23);
            this.btnClearMemory.TabIndex = 5;
            this.btnClearMemory.Text = "Clear";
            this.btnClearMemory.UseVisualStyleBackColor = true;
            this.btnClearMemory.Click += new System.EventHandler(this.btnClearMemory_Click);
            // 
            // btnSaveMemory
            // 
            this.btnSaveMemory.Location = new System.Drawing.Point(56, 16);
            this.btnSaveMemory.Name = "btnSaveMemory";
            this.btnSaveMemory.Size = new System.Drawing.Size(44, 23);
            this.btnSaveMemory.TabIndex = 4;
            this.btnSaveMemory.Text = "Save";
            this.btnSaveMemory.UseVisualStyleBackColor = true;
            this.btnSaveMemory.Click += new System.EventHandler(this.btnSaveMemory_Click);
            // 
            // btnLoadMemory
            // 
            this.btnLoadMemory.Location = new System.Drawing.Point(6, 16);
            this.btnLoadMemory.Name = "btnLoadMemory";
            this.btnLoadMemory.Size = new System.Drawing.Size(44, 23);
            this.btnLoadMemory.TabIndex = 3;
            this.btnLoadMemory.Text = "Load";
            this.btnLoadMemory.UseVisualStyleBackColor = true;
            this.btnLoadMemory.Click += new System.EventHandler(this.btnLoadMemory_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(102, 14);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(40, 23);
            this.btnStep.TabIndex = 18;
            this.btnStep.Text = "Step";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // lblPC
            // 
            this.lblPC.AutoSize = true;
            this.lblPC.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPC.Location = new System.Drawing.Point(202, 19);
            this.lblPC.Name = "lblPC";
            this.lblPC.Size = new System.Drawing.Size(63, 13);
            this.lblPC.TabIndex = 19;
            this.lblPC.Text = "PC: 0000";
            // 
            // lblCycle
            // 
            this.lblCycle.AutoSize = true;
            this.lblCycle.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCycle.Location = new System.Drawing.Point(281, 19);
            this.lblCycle.Name = "lblCycle";
            this.lblCycle.Size = new System.Drawing.Size(59, 13);
            this.lblCycle.TabIndex = 20;
            this.lblCycle.Text = "Cycle #:";
            this.lblCycle.Click += new System.EventHandler(this.lblCycle_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(148, 14);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(48, 23);
            this.btnReset.TabIndex = 21;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // rtbSimulator
            // 
            this.rtbSimulator.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbSimulator.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbSimulator.Location = new System.Drawing.Point(250, 24);
            this.rtbSimulator.Name = "rtbSimulator";
            this.rtbSimulator.ReadOnly = true;
            this.rtbSimulator.Size = new System.Drawing.Size(239, 320);
            this.rtbSimulator.TabIndex = 22;
            this.rtbSimulator.Text = "";
            // 
            // lblSimulator
            // 
            this.lblSimulator.AutoSize = true;
            this.lblSimulator.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSimulator.Location = new System.Drawing.Point(247, 3);
            this.lblSimulator.Name = "lblSimulator";
            this.lblSimulator.Size = new System.Drawing.Size(116, 18);
            this.lblSimulator.TabIndex = 23;
            this.lblSimulator.Text = "SIMULATOR";
            // 
            // gbSimulator
            // 
            this.gbSimulator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSimulator.Controls.Add(this.btnStop);
            this.gbSimulator.Controls.Add(this.btnStart);
            this.gbSimulator.Controls.Add(this.btnReset);
            this.gbSimulator.Controls.Add(this.btnStep);
            this.gbSimulator.Controls.Add(this.lblPC);
            this.gbSimulator.Controls.Add(this.lblCycle);
            this.gbSimulator.Location = new System.Drawing.Point(148, 343);
            this.gbSimulator.Name = "gbSimulator";
            this.gbSimulator.Size = new System.Drawing.Size(361, 43);
            this.gbSimulator.TabIndex = 24;
            this.gbSimulator.TabStop = false;
            this.gbSimulator.Text = "Simulator Control";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(48, 14);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(48, 23);
            this.btnStop.TabIndex = 23;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(3, 14);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(40, 23);
            this.btnStart.TabIndex = 22;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoScrollMargin = new System.Drawing.Size(5, 5);
            this.panel1.AutoScrollMinSize = new System.Drawing.Size(500, 500);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 404);
            this.panel1.MinimumSize = new System.Drawing.Size(920, 159);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(935, 159);
            this.panel1.TabIndex = 25;
            this.panel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.panel1_Scroll);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.MinimumSize = new System.Drawing.Size(5000, 1000);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(5000, 1000);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;          
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pictureBox1_LoadCompleted);
            // 
            // htbRegisters
            // 
            this.htbRegisters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.htbRegisters.Location = new System.Drawing.Point(547, 38);
            this.htbRegisters.Name = "htbRegisters";
            this.htbRegisters.Size = new System.Drawing.Size(125, 299);
            this.htbRegisters.TabIndex = 15;
            this.htbRegisters.Text = "";
            this.htbRegisters.Resize += new System.EventHandler(this.htbRegisters_Resize);
            this.htbRegisters.VScroll += new System.EventHandler(this.htbRegisters_VScroll);
            this.htbRegisters.TextChanged += new System.EventHandler(this.htbRegisters_TextChanged);
            // 
            // htbMemory
            // 
            this.htbMemory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.htbMemory.Location = new System.Drawing.Point(719, 38);
            this.htbMemory.Name = "htbMemory";
            this.htbMemory.Size = new System.Drawing.Size(216, 299);
            this.htbMemory.TabIndex = 8;
            this.htbMemory.Text = "";
            this.htbMemory.Resize += new System.EventHandler(this.htbMemory_Resize);
            this.htbMemory.VScroll += new System.EventHandler(this.htbMemory_VScroll);
            this.htbMemory.TextChanged += new System.EventHandler(this.htbMemory_TextChanged);
            // 
            // frmSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 563);
            this.Controls.Add(this.rtbPipelineMap);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gbSimulator);
            this.Controls.Add(this.lblSimulator);
            this.Controls.Add(this.rtbSimulator);
            this.Controls.Add(this.gbMemory);
            this.Controls.Add(this.gbRegister);
            this.Controls.Add(this.htbRegisters);
            this.Controls.Add(this.RegisterNumber);
            this.Controls.Add(this.lblPipelineMap);
            this.Controls.Add(this.lblSourceCode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.htbMemory);
            this.Controls.Add(this.MemoryNumber);
            this.Controls.Add(this.lsbCode);
            this.Name = "frmSimulator";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Simulator";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmSimulator_Load);
            this.gbRegister.ResumeLayout(false);
            this.gbMemory.ResumeLayout(false);
            this.gbSimulator.ResumeLayout(false);
            this.gbSimulator.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lsbCode;
        private System.Windows.Forms.RichTextBox rtbPipelineMap;
        private System.Windows.Forms.Label MemoryNumber;
        private HexEdit.HexEditBox htbMemory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSourceCode;
        private System.Windows.Forms.Label lblPipelineMap;
        private System.Windows.Forms.Label RegisterNumber;
        private HexEdit.HexEditBox htbRegisters;
        private System.Windows.Forms.GroupBox gbRegister;
        private System.Windows.Forms.GroupBox gbMemory;
        private System.Windows.Forms.Button btnClearRegister;
        private System.Windows.Forms.Button btnSaveRegister;
        private System.Windows.Forms.Button btnLoadRegister;
        private System.Windows.Forms.Button btnClearMemory;
        private System.Windows.Forms.Button btnSaveMemory;
        private System.Windows.Forms.Button btnLoadMemory;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Label lblPC;
        private System.Windows.Forms.Label lblCycle;
        private System.Windows.Forms.Button btnReset;
        public System.Windows.Forms.RichTextBox rtbSimulator;
        private System.Windows.Forms.Label lblSimulator;
        private System.Windows.Forms.GroupBox gbSimulator;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
    }
}