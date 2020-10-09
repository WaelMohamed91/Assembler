using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 
namespace Assembler
{
  public  class Translation
    {
        Dictionary<string, string> Registers;
        Dictionary<string, string> R_type;
        Dictionary<string, string> I_type;
        Dictionary<string, string> J_type;
       public Dictionary<string, string> Words_valueus ;

        public  Dictionary<string, string> Lables ;
        public Dictionary<string, string> words; 

        public Translation ()
        {
            Words_valueus = new Dictionary<string, string>();
            Lables = new Dictionary<string, string>();
            words = new Dictionary<string, string>(); 

            Registers = new Dictionary<string, string>();
            Set_Registers_Dic();

            R_type = new Dictionary<string, string>();
            Set_R_Type_Dic();

            I_type = new Dictionary<string, string>();
            Set_I_Type_Dic(); 

            J_type = new Dictionary<string, string>(); 
            Set_J_Type_Dic(); 
        }
     
        private void Set_Registers_Dic() {
            Registers.Add("$zero", "00000"); //0
            Registers.Add("$at", "00001"); //1
            Registers.Add("$v0", "00010"); //2
            Registers.Add("$v1", "00011"); //3
            Registers.Add("$a0", "00100"); //4
            Registers.Add("$a1", "00101"); //5
            Registers.Add("$a2", "00110"); //6
            Registers.Add("$a3", "00111"); //7
            Registers.Add("$t0", "01000"); //8
            Registers.Add("$t1", "01001"); //9
            Registers.Add("$t2", "01010"); //10
            Registers.Add("$t3", "01011"); //11
            Registers.Add("$t4", "01100"); //12
            Registers.Add("$t5", "01101"); //13
            Registers.Add("$t6", "01110"); //14
            Registers.Add("$t7", "01111"); //15
            Registers.Add("$s0", "10000"); //16
            Registers.Add("$s1", "10001"); //17
            Registers.Add("$s2", "10010"); //18
            Registers.Add("$s3", "10011"); //19
            Registers.Add("$s4", "10100"); //20
            Registers.Add("$s5", "10101"); //21
            Registers.Add("$s6", "10110"); //22
            Registers.Add("$s7", "10111"); //23
            Registers.Add("$t8", "11000"); //24
            Registers.Add("$t9", "11001"); //25
            Registers.Add("$k0", "11010"); //26
            Registers.Add("$k1", "11011"); //27
            Registers.Add("$gp", "11100"); //28
            Registers.Add("$sp", "11101"); //29
            Registers.Add("$fp", "11110"); //30
            Registers.Add("$ra", "11111"); //31
        }
        private void Set_R_Type_Dic()
        {
            R_type.Add("add", "100000"); //32
            R_type.Add("and", "100100"); //36
            R_type.Add("sub", "100010"); //34
            R_type.Add("nor", "100111"); //39
            R_type.Add("or", "100101"); //37
            R_type.Add("slt", "101010"); //42
            R_type.Add("sll", "000000"); //0
            R_type.Add("srl", "000010"); //2
            R_type.Add("jr", "001000"); //8
            R_type.Add("xor", "100110"); //26
            R_type.Add("sltu", "101011"); //42
        }
        private void Set_I_Type_Dic ()
        {
            I_type.Add("addi", "001000"); //0x08
            I_type.Add("lw", "100011"); //0x023
            I_type.Add("sw", "101011"); //0x02B
            I_type.Add("beq", "000100"); //0x04
            I_type.Add("bne", "000101"); //0x05
            I_type.Add("addiu", "001001"); //0x09
            I_type.Add("slti", "001010"); //0x0A
            I_type.Add("andi", "001100"); //0x0C
            I_type.Add("ori", "001101"); //0x0D
            I_type.Add("lui", "001111"); //0x0F
            I_type.Add("lb", "010100"); //0x020
            I_type.Add("sb", "011100"); //0x028
        }
        private void Set_J_Type_Dic ()
        {
            J_type.Add("j", "000010"); //2
            J_type.Add("jal", "000011"); //3
        }
        // Function Remove All Spaces in Line 
        public string Remove_space(string str)
        {
            string without_space = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == ' ')
                    continue;
                without_space += str[i];

            }
            return without_space;
        }
        // Function take String , and Size string after fill zeroes in begin of string 
        public string Fill_first_Zeros (string str , int count )
        {
            string full = "";
            int diff = count - str.Length; 
            for (int i = 0; i < diff ; i++)
                full += '0';
            full += str;
            return full; 
        }
   //Function Return true when label exist in Line 
        public bool Lable_Exist (string str )
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] == ':')
                    return true;

            return false; 
        }
        private string split_comments(string str)
        {
            string line_split = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '#')
                    break;
                else
                    line_split += str[i];
            }
            return line_split;
        }


        public string Translate_Code (string Line , int line_address ) 
        {

            char Type =  Chooser(Line) ; // To Check line have which Type 
            if (Type == 'R') 
              return Translate_R_Type(Line);
            if (Type == 'I')
                return Translate_I_Type(Line , line_address);

          else if (Type == 'J')
             return Translate_J_Type(Line, line_address) ;

            return "Not Implemented"; 
        }
        private char Chooser(String Instruction_Line)
        {
            // To Get instruction name from line
            string Instruction = "";
           
            for ( int i = 0; i < Instruction_Line.Length; i++)
            {
                if (Instruction_Line[i] == '$')
                {
                    if (R_type.ContainsKey(Instruction))
                        return 'R';
                    if (I_type.ContainsKey(Instruction))
                        return 'I'; 
                }
                Instruction += Instruction_Line[i];

                if (Instruction_Line[i] == ':')
                    Instruction = ""; 
            }

            return 'J';
        }
        public void Write_in_Files(string translate_Result )
        {
            FileStream F = new FileStream("dataSegment.txt", FileMode.OpenOrCreate);
            StreamWriter datasegment = new StreamWriter(F);

            FileStream F2 = new FileStream("CodeSegment.txt", FileMode.OpenOrCreate);
            StreamWriter codesegment = new StreamWriter(F2);

            bool data_code = true;
            string[] ALL_output;
            ALL_output = translate_Result.Split('\n');
            for (int i = 0; i < ALL_output.Count(); i++)
            {
                if (ALL_output[i] == "#Translation of Code Segment")
                    data_code = false;

                if (data_code)
                    datasegment.WriteLine(ALL_output[i]);
                else
                    codesegment.WriteLine(ALL_output[i]);
            }
            datasegment.Close();
            codesegment.Close();
            F.Close();
            F2.Close();
        }

        private string Translate_R_Type(String Instruction_Line) {
            // write your code here  
            // Just Retutn 32 bit which not contain any other char 
            // load R - egisters Dictionary  
            string stl_line = "add$t1,$t0,$s2"; // we will replace this line with the parameter !
            string special = "000000"; // first 6 bits
            string Reg1 = "00000";    // fourth  5 bits  
            string Reg2 = "00000";   // third 5 bits 
            string Reg3 = "00000";  // second 5 bits 
            string shift = "";     // shift bits    
            string function = "000000"; // last 6 bits 
            string instruction = "";
            string label = "";

            string instruction_line = Instruction_Line;

            string line = split_comments(instruction_line);
            string[] array_registers = line.Split(',');// for example [0]:add $s0 , [1]:$t1 , [2]:$t2
            int j; // lenght of [0]

            string register1 = "";
            // separate the first register from the instruction --> [0]
            for (j = 0; j < array_registers[0].Length; j++)
            {
                if (array_registers[0][j] == '$')
                {
                    // outer loop fill the instruction 
                    for (int l = j; l < array_registers[0].Length; l++)
                    {
                        // inner loop fill the register
                        register1 += array_registers[0][j];
                        j++;
                    }
                    break;
                }

                else
                    instruction += array_registers[0][j];

            }
            array_registers[0] = register1;
            string[] first_word;

            if (instruction.Length > 4)
            {
                first_word = instruction.Split(':');
                label = first_word[0];
                instruction = first_word[1];
            }
            // just (slt , sll , sltu) : accesed the shift 
            if ( instruction == "sll")
            {
                // we fill nurmaly the op-code and function and the two (2 , 3) registers 
                R_type.TryGetValue(instruction, out function);
                Registers.TryGetValue(array_registers[0], out Reg2);
                Registers.TryGetValue(array_registers[1], out Reg3);
                Reg1 = "00000"; // the third register set with zeros
                //get the shift binary from third register 
                if (Registers.TryGetValue(array_registers[2], out shift))
                {
                    Console.WriteLine(shift);
                }
                else
                {
                    int dec_shift;
                    //convert the string to integer 
                    Int32.TryParse(array_registers[2], out dec_shift);
                    //convert the integer first to binary and to string 
                    shift = Convert.ToString(dec_shift, 2);
                    //fill the remain bits by zeros from left 
                    shift = Fill_first_Zeros(shift, 5);
                    Console.WriteLine(shift);

                }
            }

            else
            {
                //get values of keys
                Console.WriteLine(instruction + " " + array_registers[0] + " " + array_registers[1] + " " + array_registers[2]);
                R_type.TryGetValue(instruction, out function);
                Registers.TryGetValue(array_registers[0], out Reg1);
                Registers.TryGetValue(array_registers[1], out Reg2);
                Registers.TryGetValue(array_registers[2], out Reg3);
                shift = "00000";

            }
            //Console.WriteLine(special + " " + Reg3 + " " + Reg2 + " " + Reg1 + " " + shift + " " + function);

            string return_Binary_code = special + Reg2 + Reg3 + Reg1 + shift + function;

            return  return_Binary_code ;

        }

        private string Translate_I_Type(String instruction_input, int line_address) 
        {
            string out_Reg1 = ""; // rs
            string out_Reg2 = ""; // rt
            string out_offset = ""; // offset
            string outt = ""; // the output
            string function = ""; // op code
            string label = ""; // label name
            int len=  0;

            Console.WriteLine(instruction_input);
            string instruction = "";
            string register1 = "";
            string register2 = "";
            string offset = "";
            string offset_binary = "";

            string cop1 = instruction_input;

            string[] arr1 = instruction_input.Split(',');

            if (Lable_Exist(instruction_input)) // To seperate the label and get the instruction name
            {
                string[] arr2 = arr1[0].Split(':');
                label = arr2[0];
                string[] arr3 = arr2[1].Split('$');
       
                instruction = arr3[0];
            }
            else 
            {
                string[] arr2 = arr1[0].Split('$');
             
                instruction = arr2[0];
            }

        
            Int32 integer_adress;
            string instruction2 = "";
            //bool found_reg1 = false;
            //bool found_instruction = false;
            string[] line_inst = instruction_input.Split(',');

            string reg1 = "";
            instruction2 = "";



            for (int i = 0; i < line_inst[0].Length; i++)
            {
                if (line_inst[0][i] == '$')
                {
                    for (int j = i; j < line_inst[0].Length; j++)
                    {
                        reg1 += line_inst[0][j];
                    }
                    break;
                }
                else
                    instruction2 += line_inst[0][i];
                

            }

         
            if (instruction2 == "beq" || instruction2 == "bne")
            {

                string reg2 = line_inst[1];
                string jump_label = line_inst[2];
                I_type.TryGetValue(instruction2, out function);//  OP Code
                Registers.TryGetValue(reg1, out out_Reg1);// rt
                Registers.TryGetValue(reg2, out out_Reg2);// rs
                Lables.TryGetValue(jump_label, out out_offset);

                integer_adress = Convert.ToInt32(out_offset, 2);
                integer_adress = integer_adress / 4;
                integer_adress += 1;
                line_address = line_address / 4;


                Int16 res = (Int16)(integer_adress - line_address - 2);

                string str = Convert.ToString(res, 2);

                str = Fill_first_Zeros(str, 16);
                outt = function + out_Reg1 + out_Reg2 + str;


            }

            else if (instruction == "lw" || instruction == "sw") // Load and Store instructions
            {
              

                string[] ar1 = cop1.Split(',');
             

                if (ar1[0].Length > 7) // to get the rt Register
                {
                    string[] ar2 = ar1[0].Split(':');
                    // ar2[0] = label , arr2[1] =  lw$t0
                    string[] ar3 = ar2[1].Split('$');
                    // ar3[0] = lw
                    // ar3[1] = t0
                    ar1[0] = ar3[1];
                    ar1[0] = "$" + ar1[0];
                    register1 = ar1[0];
                }
                else if (ar1[0].Length < 7)
                {
                    string[] ar2 = ar1[0].Split('$');
                    //ar2[0] = lw
                    //ar2[1] = t0
                    ar1[0] = ar2[1];
                    ar1[0] = "$" + ar1[0];
                    register1 = ar1[0];
                }


                ////////////////////////////////////////////////////// to get the rs register and the offset
                for (int i = 0; i < ar1[1].Length; i++)
                {
                    while (ar1[1][i] != '(')
                    {
                        offset += ar1[1][i];
                        i++;
                    }
                    i++;
                    while (ar1[1][i] != ')')
                    {
                        register2 += ar1[1][i];
                        i++;
                    }
                    break;
                }

                I_type.TryGetValue(instruction, out function);//  OP Code
                Registers.TryGetValue(register1, out out_Reg1);// rt
                Registers.TryGetValue(register2, out out_Reg2);// rs

                // to check if the offset immediate or variable
                double Num;
                bool isNum = double.TryParse(offset , out Num);

                if (isNum)
                {
                    int offs = 0;
                    Int32.TryParse(offset, out offs);
                    offset_binary = Convert.ToString(offs, 2);
                    offset_binary = Fill_first_Zeros(offset_binary, 16);
                }
                else
                {
                    Lables.TryGetValue(offset, out offset_binary); // offset
                }
               
                outt = function + out_Reg2 + out_Reg1 + offset_binary;
            }


          
            else if (instruction == "addi")// addi Instruction
            {

                // label:addi$t0,$t1,4
                // addi$t0,$t1,4

                string[] ar1 = cop1.Split(',');
                // ar1[0] = labe:addi$t0 || ar1[0] = addi$t0
                // ar1[1] = $t1
                // ar1[2] = 4

                // to get the rs and rt registers and the offset

                if (ar1[0].Length > 7)
                {
                    string[] ar2 = ar1[0].Split(':');
                    // ar2[0] = label , ar2[1] =  addi$t0
                    string[] ar3 = ar2[1].Split('$');
                    // ar3[0] = addi
                    // ar3[1] = t0
                    ar1[0] = ar3[1];
                    ar1[0] = "$" + ar1[0];
                    register1 = ar1[0];
                    register2 = ar1[1];
                    offset = ar1[2];
                }
                else if (ar1[0].Length <= 7)
                {
                    string[] ar2 = ar1[0].Split('$');
                    //ar2[0] = addi
                    //ar2[1] = t0
                    ar1[0] = ar2[1];
                    ar1[0] = "$" + ar1[0];
                    register1 = ar1[0];
                    register2 = ar1[1];
                    offset = ar1[2];
                }

                I_type.TryGetValue(instruction, out function);//  OP Code
                Registers.TryGetValue(register1, out out_Reg1);// rt
                Registers.TryGetValue(register2, out out_Reg2);// rs

                int offs = 0;
                Int32.TryParse(offset , out offs);

                 offset_binary = Convert.ToString(offs, 2);

                len = offset_binary.Length;

                offset_binary = Fill_first_Zeros(offset_binary, len);

                outt = function + out_Reg2  + out_Reg1  + offset_binary;

            }
            return outt;

        }



        private string Translate_J_Type(String Instruction_Line, int line_address) {
            // write your code here  
            // Just Retutn 32 bit which not contain any other char 
            string label = "";
            string instruction = "";
            string jar = "jal";
            string value = "";
            int counter = 0;
           
            // to skip label from begin of line 
            if (Lable_Exist(Instruction_Line))
            {
                string[] temp;
                temp = Instruction_Line.Split(':');
                Instruction_Line = temp[1];
            }

            for (int i = 0; i < 3; i++)
                if (jar[i] == Instruction_Line[i])
                    counter++;

            // if the counter = 3 that main that the line contain "jal" jump
            if (counter == 3)
                instruction = "jal";

            // if the counter = 1 that main that the line contain "j" jump
            else 
                instruction = "j";
              
          
            for (int j = counter ; j < Instruction_Line.Length; j++)
            {
                if (Instruction_Line[j] == '#')
                    break; 
                label += Instruction_Line[j];   
            }
          

            

            Lables.TryGetValue(label, out value);

           
            string binary_instruction = "";
            J_type.TryGetValue(instruction, out binary_instruction);
        

            int integer = Convert.ToInt32(value, 2);
            integer = integer / 4;

            string binary_address = Convert.ToString(integer, 2);
           
            binary_address = Fill_first_Zeros(binary_address, 26);
            
            string return_J_binary_code = binary_instruction + binary_address;
            return return_J_binary_code;
        }
        

    }
}
