using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace MiniDLX
{

    public class Assembler
    {
        /* [ ] Buat load opcode from file
         * [ ] Buat Regex otomatis
         * [ ] Check Regex untuk Memory
         * [ ] buat rutin untuk cek jump
         * [ ]
         */
        private LabelDictionary ld;
        private LabelDictionary od = new LabelDictionary();

        private string[] AsmErrorMsg = {            
            /*0*/ "Invalid syntax/mnemonic",
            /*1*/ "Duplicate label",
            /*2*/ "Required operand missing",
            /*3*/ "Too many operand",
            /*4*/ "Invalid operand",
            /*5*/ "Register Number out of range",
            /*6*/ "Label is not found",            
            /*7*/ "Address out of range",
            /*8*/ "Bad Memory-Register Syntax"                    
        };

        public Assembler()
        {
            od.Add("ADD", 0x00000000);
            od.Add("SUB", 0x00000002);
            od.Add("OR", 0x00000004);
            od.Add("XOR", 0x00000001);
            od.Add("SLT", 0x00000003);
            od.Add("BNEZ", 0x20000000);
            od.Add("LW", 0x24000000);
            od.Add("SW", 0x28000000);
            od.Add("ADDI", 0x2C000000);
            od.Add("J", 0x80000000);
        }

        public Assembler(string opcodefilename)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private string RemoveWhiteSpace(string s)
        {
            string r = "";
            foreach (char c in s)
            {
                if (char.IsWhiteSpace(c)) continue;
                r += c;
            }
            return r;
        }

        private string DecimalToBinary(UInt32 iDec)
        {
            string strBin = "";
            for (UInt32 divider = 0x80000000; divider > 0; divider = divider >> 1)
            {
                strBin += Convert.ToString(iDec / divider);
                iDec = iDec % divider;
            }
            return strBin;
        }

        private string Pass2(string code)
        {
            UInt32 PC = UInt32.MaxValue - 3; //PC = Program Counter
            int Line = -1; //Line Counter
            StringReader sra = new StringReader(code);
            string nextLine = "";
            string result = ""; // Resulting Code
            while (true)
            {
                UInt32 opCode = 0;
                nextLine = sra.ReadLine();
                Line++;
                PC += 4;

                if ((nextLine == null) || (nextLine == "")) break; // End Of File
                string[] token = nextLine.Split(',');
                string cmd = token[0];
                if (!od.Contains(cmd)) throw new ArgumentException(AsmErrorMsg[0] + "," + Line.ToString()); //Invalid Syntax/Mnemonic
                opCode = od[cmd];
                UInt32 intrType = opCode & 0xFC000000; //masking 6 first bit only to get opCode only not function
                UInt32 offset = 0, rd = 0, rs1 = 0, rs2 = 0, TargetAdd = 0, NPC = 0, RelativeAdd = 0;
                string[] paramtoken;
                switch (intrType)
                {
                    case 0:
                        //register type
                        if (token.Length > 4) throw new ArgumentException(AsmErrorMsg[3] + "," + Line.ToString());
                        if (token.Length < 4) throw new ArgumentException(AsmErrorMsg[2] + "," + Line.ToString());
                        //convert R to UInt32
                        try
                        {
                            rd = Convert.ToUInt32(token[1].Trim('R'));
                            rs1 = Convert.ToUInt32(token[2].Trim('R'));
                            rs2 = Convert.ToUInt32(token[3].Trim('R'));
                        }
                        catch (FormatException ex)
                        {
                            throw new ArgumentException(AsmErrorMsg[4] + "," + Line.ToString());
                        }
                        //check min 0 max 31
                        if (rd > 31) throw new ArgumentException(AsmErrorMsg[5] + "," + Line.ToString());
                        if (rs1 > 31) throw new ArgumentException(AsmErrorMsg[5] + "," + Line.ToString());
                        if (rs2 > 31) throw new ArgumentException(AsmErrorMsg[5] + "," + Line.ToString());
                        //shift to the right position
                        rs1 = rs1 << (32 - 11);
                        rs2 = rs2 << (32 - 16);
                        rd = rd << (32 - 21);

                        //combine into opcode
                        opCode = opCode | rs1 | rs2 | rd;
                        break;
                    case 0x80000000:
                        //jump type
                        if (token.Length > 2) throw new ArgumentException(AsmErrorMsg[3] + "," + Line.ToString());
                        if (token.Length < 2) throw new ArgumentException(AsmErrorMsg[2] + "," + Line.ToString());
                        if (!ld.Contains(token[1])) throw new ArgumentException(AsmErrorMsg[6] + " '" + token[1] + "'," + Line.ToString());
                        TargetAdd = ld[token[1]];
                        NPC = PC + 4;
                        RelativeAdd = TargetAdd - NPC;
                        //check relative address greater than 16 bit??                        
                        RelativeAdd = RelativeAdd & 0x03FFFFFF;
                        //combine into opcode
                        opCode = opCode | RelativeAdd;
                        break;
                    //immediate type
                    case 0x28000000: //SW
                        if (token.Length > 3) throw new ArgumentException(AsmErrorMsg[3] + "," + Line.ToString());
                        if (token.Length < 3) throw new ArgumentException(AsmErrorMsg[2] + "," + Line.ToString());
                        if (!Regex.IsMatch(token[1], @"^[0-9A-F]{1,4}\u0028R\d{1,2}\u0029")) throw new ArgumentException(AsmErrorMsg[8] + "," + Line.ToString());
                        paramtoken = Regex.Split(token[1], @"\u0028|\u0029");
                        try
                        {
                            offset = Convert.ToUInt32("0x" + paramtoken[0], 16);
                            rs1 = Convert.ToUInt32(paramtoken[1].Trim('R'));
                            rd = Convert.ToUInt32(token[2].Trim('R'));
                        }
                        catch (FormatException ex)
                        {
                            throw new ArgumentException(AsmErrorMsg[4] + "," + Line.ToString());
                        }
                        //check min 0 max 31
                        if (rd > 31) throw new ArgumentException(AsmErrorMsg[5] + "," + Line.ToString());
                        if (rs1 > 31) throw new ArgumentException(AsmErrorMsg[5] + "," + Line.ToString());
                        //shift to the right position
                        rs1 = rs1 << (16 + 5);
                        rd = rd << 16;
                        //combine into opcode
                        opCode = opCode | rs1 | rd | offset;
                        break;
                    case 0x24000000: //LW
                        if (token.Length > 3) throw new ArgumentException("Too many parameter," + Line.ToString());
                        if (token.Length < 3) throw new ArgumentException("Lack of parameter," + Line.ToString());
                        if (!Regex.IsMatch(token[2], @"^[0-9A-F]{1,4}\u0028R\d{1,2}\u0029")) throw new ArgumentException(AsmErrorMsg[8] + "," + Line.ToString());
                        try
                        {
                            paramtoken = Regex.Split(token[2], @"\u0028|\u0029");
                            offset = Convert.ToUInt32("0x" + paramtoken[0], 16);
                            rs1 = Convert.ToUInt32(paramtoken[1].Trim('R'));
                            rd = Convert.ToUInt32(token[1].Trim('R'));
                        }
                        catch (FormatException ex)
                        {
                            throw new ArgumentException(AsmErrorMsg[4] + "," + Line.ToString());
                        }
                        //check min 0 max 31
                        if (rd > 31) throw new ArgumentException(AsmErrorMsg[5] + "," + Line.ToString());
                        if (rs1 > 31) throw new ArgumentException(AsmErrorMsg[5] + "," + Line.ToString());
                        //shift to the right position
                        rs1 = rs1 << (16 + 5);
                        rd = rd << 16;
                        //combine into opcode
                        opCode = opCode | rs1 | rd | offset;
                        break;
                    case 0x20000000: //BNEZ
                        if (token.Length > 3) throw new ArgumentException("Too many parameter," + Line.ToString());
                        if (token.Length < 3) throw new ArgumentException("Lack of parameter," + Line.ToString());
                        if (!ld.Contains(token[2])) throw new ArgumentException("Unknown Label '" + token[2] + "'," + Line.ToString());
                        try
                        {
                            rs1 = Convert.ToUInt32(token[1].Trim('R'));
                        }
                        catch (FormatException ex)
                        {
                            throw new ArgumentException(AsmErrorMsg[4] + "," + Line.ToString());
                        }
                        //check min 0 max 31
                        if (rs1 > 31) throw new ArgumentException(AsmErrorMsg[5] + "," + Line.ToString());
                        //Count the relative address
                        TargetAdd = ld[token[2]];
                        NPC = PC + 4;
                        RelativeAdd = TargetAdd - NPC;
                        //check relative address greater than 16 bit??
                        RelativeAdd = RelativeAdd & 0x0000FFFF;
                        //shift to the right position
                        rs1 = rs1 << (16 + 5);
                        //combine into opcode
                        opCode = opCode | rs1 | rd | offset;
                        break;
                    case 0x2C000000: //ADDI                        
                        if (token.Length > 4) throw new ArgumentException("Too many parameter," + Line.ToString());
                        if (token.Length < 4) throw new ArgumentException("Lack of parameter," + Line.ToString());
                        try
                        {
                            offset = Convert.ToUInt32("0x" + token[3].Trim('#'), 16);
                            rs1 = Convert.ToUInt32(token[2].Trim('R'));
                            rd = Convert.ToUInt32(token[1].Trim('R'));
                        }
                        catch (FormatException ex)
                        {
                            throw new ArgumentException(AsmErrorMsg[4] + "," + Line.ToString());
                        }
                        //check min 0 max 31
                        if (rd > 31) throw new ArgumentException(AsmErrorMsg[5] + "," + Line.ToString());
                        if (rs1 > 31) throw new ArgumentException(AsmErrorMsg[5] + "," + Line.ToString());
                        //shift to the right position
                        rs1 = rs1 << (16 + 5);
                        rd = rd << 16;
                        //combine into opcode
                        opCode = opCode | rs1 | rd | offset;
                        break;
                    default:
                        throw new ArgumentException(AsmErrorMsg[0] + "," + Line.ToString()); //Invalid Syntax/Mnemonic                        
                }
                //print the result
                result += DecimalToBinary(opCode) + "\n";
            }
            return result;
        }

        private string Pass1(string code, Boolean isSimulate)
        {
            ld = new LabelDictionary();
            #region Build the Scanner Using Regex
            Regex theReg = new Regex(
            @"((?<LABEL>\w+:)\s*){0,1}" +
            @"(?<KEYWORD>ADD|SUB|OR|XOR|SLT|BNEZ|LW|SW|ADDI|J)\s+" +
            @"(?<PARAM>.+)");
            #endregion

            //----processing per line
            UInt32 PC = 0; //PC = Program Counter
            int Line = -1; //Line Counter
            StringReader sra = new StringReader(code);
            string nextLine = "";
            string result = ""; // Resulting Code
            while (true)
            {
                nextLine = sra.ReadLine();
                Line++;
                if (nextLine == null) break; // End Of File
                if (nextLine.Trim().Equals("")) continue; //Skip Blank Line

                #region Recognize Comment and Command
                string comment = "", cmd = "";
                int iSemicolon = nextLine.IndexOf(';');
                if (iSemicolon != -1)
                {
                    comment = nextLine.Substring(iSemicolon);
                    cmd = nextLine.Substring(0, iSemicolon);
                }
                else
                {
                    cmd = nextLine;
                }
                if (cmd.Trim().Equals("")) continue; //Skip Blank Command
                cmd = cmd.ToUpper(); //Upper Case Command 
                #endregion

                //---- Use the REGEX Scanner - to tokenize
                MatchCollection theMatches = theReg.Matches(cmd);

                if (theMatches.Count != 1)
                {
                    throw new ArgumentException(AsmErrorMsg[0] + "," + Line.ToString()); //Invalid syntax/mnemonic
                }
                else
                {
                    String label = "";
                    if (!theMatches[0].Groups["LABEL"].Captures.Count.Equals(0))
                    {
                        label = theMatches[0].Groups["LABEL"].ToString();
                        label = label.Trim(':');
                        if (ld.Contains(label)) throw new ArgumentException(AsmErrorMsg[1] + "," + Line.ToString()); //Duplicate Label
                        ld.Add(label, PC);
                    }
                    string param = theMatches[0].Groups["PARAM"].ToString();
                    
                    if (isSimulate)
                    {
                        result += String.Format("{0:X4}", PC) + " " + theMatches[0].Groups["LABEL"] + theMatches[0].Groups["KEYWORD"] + " " + RemoveWhiteSpace(param) + "\n";
                    }
                    else
                    {
                        result += theMatches[0].Groups["KEYWORD"] + "," + RemoveWhiteSpace(param) + "\n";
                    }
                    PC += 4;

                }

            }
            if (isSimulate)
            {

                result += String.Format("{0:X4}", PC) + " " + "ADD,R0,R0,R0\n";
                PC = PC + 4;
                result += String.Format("{0:X4}", PC) + " " + "ADD,R0,R0,R0\n";
                PC = PC + 4;
                result += String.Format("{0:X4}", PC) + " " + "ADD,R0,R0,R0\n";
                PC = PC + 4;
            }
            else
            {
                result += "ADD,R0,R0,R0\n";
                result += "ADD,R0,R0,R0\n";
                result += "ADD,R0,R0,R0\n";
            }


            return result;
        }

        public string AssemblerToBin(string code)
        {
            string result = "";
            try
            {
                result = Pass2(Pass1(code,false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string AssemblerToSim(string code)
        {
            string result = "";
            try
            {
                result = Pass1(code, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string AssemblerToHex(string s)
        {
            throw new Exception("The method or operation is not implemented.");
        }

    }

    public class LabelDictionary : DictionaryBase
    {

        public UInt32 this[String key]
        {
            get
            {
                return ((UInt32)Dictionary[key]);
            }
            set
            {
                Dictionary[key] = value;
            }
        }

        public ICollection Keys
        {
            get
            {
                return (Dictionary.Keys);
            }
        }

        public ICollection Values
        {
            get
            {
                return (Dictionary.Values);
            }
        }

        public void Add(String key, UInt32 value)
        {
            Dictionary.Add(key, value);
        }

        public bool Contains(String key)
        {
            return (Dictionary.Contains(key));
        }

        public void Remove(String key)
        {
            Dictionary.Remove(key);
        }

        protected override void OnInsert(Object key, Object value)
        {
            if (key.GetType() != typeof(System.String))
                throw new ArgumentException("key must be of type String.", "key");

            if (value.GetType() != typeof(System.UInt32))
                throw new ArgumentException("value must be of type UInt32.", "value");
        }

        protected override void OnRemove(Object key, Object value)
        {
            if (key.GetType() != typeof(System.String))
                throw new ArgumentException("key must be of type String.", "key");
        }

        protected override void OnSet(Object key, Object oldValue, Object newValue)
        {
            if (key.GetType() != typeof(System.String))
                throw new ArgumentException("key must be of type String.", "key");

            if (newValue.GetType() != typeof(System.UInt32))
                throw new ArgumentException("value must be of type UInt32.", "value");
        }

        protected override void OnValidate(Object key, Object value)
        {
            if (key.GetType() != typeof(System.String))
                throw new ArgumentException("key must be of type String.", "key");

            if (value.GetType() != typeof(System.UInt32))
                throw new ArgumentException("value must be of type UInt32.", "value");
        }

    }

}
