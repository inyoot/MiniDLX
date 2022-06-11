using System;
using System.Collections.Generic;
using System.Text;

namespace MiniDLX
{    
    public class PipelineRegister
    {
        public UInt32 NPC = 0;
        public UInt32 IR = 0;
        public UInt32 A = 0;
        public UInt32 B = 0;
        public UInt32 Imm = 0;
        public UInt32 ALUOutput = 0;
        public Boolean Cond = false;
        public UInt32 LMD = 0;

        public PipelineRegister()
        {
            Clear();
        }

        public void Clear()
        {
            NPC = 0;
            IR = 0;
            A = 0;
            B = 0;
            Imm = 0;
            ALUOutput = 0;
            Cond = false;
            LMD = 0;
        }
    }

    public class PipelineMapRow
    {
        public enum CycleStatus
        {
            Run, Stop, Stall, Flush
        }
        public CycleStatus IFStatus = CycleStatus.Stop;
        public CycleStatus IDStatus = CycleStatus.Stop;
        public CycleStatus EXStatus = CycleStatus.Stop;
        public CycleStatus MEMStatus = CycleStatus.Stop;
        public CycleStatus WBStatus = CycleStatus.Stop;
        public Int32 StartingRow;
    }
}
