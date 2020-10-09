using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO; 

    namespace Assembler
{
    public partial class Form1 : Form
    {
        Translation main_obj ;
        string all_code;
        string[] All_lines;
        int start_text_code;
        int start_Data_code; 
       int address_counter_data ; 
       int address_counter_text ;
        bool data_code;
        bool text_code;
        string translate_Result;

        int memory_data_Lines;
        int memory_text_Lines;  
        public Form1()
        {
            InitializeComponent();
            start_text_code = 0;
            start_Data_code = 0; 
            address_counter_data = 0;
            address_counter_text = 0;
            memory_data_Lines = 0;
            memory_text_Lines = 0; 
            data_code = false;
            text_code = false; 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            main_obj = new Translation();
            Translation_instruction.ReadOnly = true;
        }
        
        private void ScanCode_ToSetLabels (string []lines )
        {
            // this code to handle all  comments before .data start  
            string Line = "";
            string label_name = "";
            string loacal = "";
          
            int space_size = 0 ;

                for (int i = 0; i < lines.Length; i++)
                {
          
                    Line =  main_obj.Remove_space(lines[i]);
                if (Line == "") continue;
                if (Line[0] == '#')
                        continue;
                    if (Line[0] == '.' && Line[1] == 'd')
                { data_code = true; start_Data_code = i;  continue;  }
                    if (Line[0] == '.' && Line[1] == 't')
                { text_code = true; data_code = false; start_text_code = i; continue;  }

                    if (data_code)
                    {
                        // loop just for take name 
                        int j = 0;
                        for (; j < Line.Length; j++)
                        {
                            if (Line[j] == ':') { j += 2; break; } // step to start space or word 
                            label_name += Line[j];
                        }
                    //  to get size of spce  
                    if (Line[j] == 's' || Line[j] == 'S')
                    {
                        j += 5; // increment index  by 5 to step to first digit digit 
                        for (; j < Line.Length; j++)
                        {
                            if (Line[j] == '#') break;
                            loacal += Line[j]; // size of varible digit by digit 
                        }
                        space_size = int.Parse(loacal);  // take size of space ; 

                        main_obj.Lables.Add(label_name, Convert.ToString(address_counter_data, 2));
                      
                        address_counter_data += (space_size * 4);

                    }
                    else if (Line[j] == 'w' || Line[j] == 'W')
                    {
                        j += 4; // increment  index by 4 to step to first digit digit
                        string x = Convert.ToString(address_counter_data, 2);
                        int y = x.Length;
                        x = main_obj.Fill_first_Zeros(x, 16);

                            main_obj.Lables.Add(label_name, x);
                       
                        address_counter_data += 4;
                        for (; j < Line.Length; j++)
                        {
                            if (Line[j] == '#') break;
                            if (Line[j] == ',') address_counter_data += 4; // to handle array of words for example >> arr: word 1,2,3,4
                            loacal += Line[j];
                        }

                        main_obj.Words_valueus.Add(label_name, loacal);
                    }
                }
                else if (text_code) {

                    if (main_obj.Lable_Exist(Line))
                    {
                        for (int w = 0; w < Line.Length; w++ )
                        {
                            if (Line[w] == ':')
                                break;
                            label_name += Line[w]; 
                        }
                        main_obj.Lables.Add(label_name, Convert.ToString(address_counter_text, 2));

                       
                    }
                        address_counter_text += 4; 
                }
                Line = "";
                label_name = "";
                loacal = "";
            }
           
            
        }
        
        private void Translate_dataCode(string Line)
        {
           
            string size_space_str="";
            int space_size_int;
            string memory_address_loacal = "";

            string word_value = "";
            int word_tmp = 0;

            // loop just for skip name 
            int j = 0;
            for (; j < Line.Length; j++)
                if (Line[j] == ':')
                      { j += 2; break; } // step to start space or word 
            
            if (Line[j] == 's' || Line[j] == 'S')
            {
                j += 5; // increment index  by 5 to step to first digit digit 
                for (; j < Line.Length; j++)
                {
                    if (Line[j] == '#') break;
                    size_space_str += Line[j]; // size of varible digit by digit 
                }
                space_size_int = int.Parse(size_space_str);  // take size of space ; 
                for (int i = 0; i < space_size_int; i++)
                {
                    memory_address_loacal = "MEMORY(" + memory_data_Lines.ToString() + ") <= \"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\" ;\n";
                    translate_Result += memory_address_loacal; 
                         memory_data_Lines++; 
                }
            }
            else if (Line[j] == 'w' || Line[j] == 'W')
            {
                j += 4; // increment  index by 4 to step to first digit 
                for (; j < Line.Length; j++)
                {
                    if (Line[j] == '#' || Line[j] == ',') break;
                    word_value += Line[j];
                }
                // convert word to int and then to binary then to 32 bit 
                word_tmp = int.Parse(word_value); 
                word_value = Convert.ToString(word_tmp, 2);  //  set binary value for this number 
               word_value = main_obj.Fill_first_Zeros(word_value, 32);  // fill zeros to make it 32 bit 

                memory_address_loacal = "MEMORY(" + memory_data_Lines.ToString() + ") <= \""+word_value+"\" ;\n";
                translate_Result += memory_address_loacal;
                memory_data_Lines++; 
            }

        }


        private void btn_convert_Click(object sender, EventArgs e)
        {
            all_code = mips_instruction_txt.Text; 
            All_lines = all_code.Split('\n');
            string line_code= "" ;
            string binary_line= "";
            string memory_address_loacal = ""; 
            // this function will set all address for all lables in project (.data , .text ) 
            // function will set values of indces  start .data code and .text code 
                 ScanCode_ToSetLabels(All_lines);


            //#################///// to translate data code  
            translate_Result += ("# Translation of Data Segment \n");
            for (int i = start_Data_code +1 ; i < start_text_code; i++)
            {
                line_code = main_obj.Remove_space(All_lines[i]);
                if (line_code == "" || line_code[0] == '#' )
                    continue;

              Translate_dataCode(line_code);
            }
            address_counter_text = 0;

            // ############# /// translate Text Code 
            translate_Result += ("\n#Translation of Code Segment\n");
            for (int i = start_text_code +1; i<All_lines.Length ; i++)
            {
                line_code = main_obj.Remove_space(All_lines[i]);
                if (line_code == "" || line_code[0] == '#')
                    continue;
                binary_line =  main_obj.Translate_Code(line_code, address_counter_text) ;
                memory_address_loacal = "MEMORY(" + memory_text_Lines + ") := \"" + binary_line +"\" ;\n";
                translate_Result += memory_address_loacal;
                address_counter_text += 4;
                memory_text_Lines++;
            }
            Translation_instruction.Text = translate_Result;

             main_obj. Write_in_Files(translate_Result);  

            mips_instruction_txt.ReadOnly = true;
            btn_convert.Enabled = false  ; 

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
