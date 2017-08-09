using System;
using System.IO;
using System.Collections.Generic;
using System.Text;


/*2017.8.9
 * 數位隱寫術範例
 * 讀入1 bmp圖檔 以least significant bit 的方式 將使用者欲隱寫之訊息寫入圖內再行輸出
 * */

namespace SteganographyExample
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("<數位隱寫程式>");
            Console.WriteLine("請輸入欲啟用模式: (1) 寫入模式 (2)讀取模式");

            int success = 0; //作為啟用模式是否正確的旗標
            int mode = 0;    //使用者選擇的模式

            while (success == 0) //確保使用者輸入是合法的
            {
                mode = int.Parse(Console.ReadLine());
                if (mode != 1 && mode != 2)
                {
                    Console.WriteLine("輸入模式有誤 請重新輸入");
                }
                else
                {
                    success = 1;
                }

            }
            if (mode == 1)
            {
                Console.WriteLine("已啟用寫入模式");
                WriteIn();
            }
            if (mode == 2)
            {
                Console.WriteLine("已取用讀取模式");
                Read();
            }

        }
        static void WriteIn() //隱寫模式
        {
            Console.WriteLine("請輸入欲隱寫之圖片(bmp檔)完整路徑(含副檔名)");
            string path = Console.ReadLine();
            FileInfo f = new FileInfo(path);
            long file_size = f.Length;
            long capacity = (file_size - 55) / 8;  //告知可以隱寫多少字母
            byte[] result = File.ReadAllBytes(path);
            Console.WriteLine("檔案大小已偵測完畢，可隱寫之字母數目為:{0}", capacity);
            Console.WriteLine("請輸入欲隱寫之文字檔檔名(要和程式同一個資料夾):");
            string sourse = Console.ReadLine();
            StreamReader sr = new StreamReader(sourse);


            string input = sr.ReadToEnd(); //把要引寫的內容檔案全部讀進來
            List<int> neededToWriteIn = new List<int>();
            for (int i = 0; i < input.Length; i++)
            {
                neededToWriteIn.Add((int)input[i]); //將每一個字母都轉成對應ASCII碼

            }
            int currentBytePos = 56;//BMP前55個位元為標頭檔 因此從第56個位元的LSB開始隱寫
            foreach (int i in neededToWriteIn)
            {
                //將每一個代寫字母換成二進位表示 並把他的每一個位元存到每一個位元組的LSB
                int strPos = 0;
                string temp = Convert.ToString(i, 2).PadLeft(8, '0');
                for (int b = currentBytePos; b <= currentBytePos + 7; b++)
                {
                    string binCurrentBytePos = Convert.ToString(result[b], 2).PadLeft(8, '0');
                    StringBuilder s = new StringBuilder(binCurrentBytePos, 0, 7, 8);
                    s.Append(temp[strPos]);
                    result[b] = Convert.ToByte(s.ToString(), 2);
                    strPos++;

                }
                currentBytePos += 8;
            }

            File.WriteAllBytes("output.bmp", result); //以二進位形式輸出檔案
            Console.WriteLine("隱寫檔案完成 輸出檔名為output.bmp");
            Console.ReadLine();










        }

        

        static void Read()
        {
            Console.WriteLine("請輸入欲讀取之圖片(bmp檔)完整路徑(含副檔名)");
            string path = Console.ReadLine();
            byte[] result = File.ReadAllBytes(path);
            FileInfo f = new FileInfo(path);
            long file_size = f.Length;
            StreamWriter sw = new StreamWriter("result.txt");
            StringBuilder s = new StringBuilder();
            for (long i = 56; i < file_size; i++)
            {
                if (s.Length == 8)
                {
                    byte c = Convert.ToByte(s.ToString(), 2);
                    sw.Write((char)c);
                    s.Clear();
                    i--;
                }
                else
                {
                    string temp = Convert.ToString(result[i], 2).PadLeft(8, '0');
                    s.Append(temp[7]);
                }


            }
            Console.WriteLine("隱寫讀取完成 輸出為result.txt 內容應在檔案開始處");
            Console.ReadLine();

        }

        static byte BinStrToByte(string bin)
        {
            byte result = Convert.ToByte(bin, 2);
            return result;
        }

    }
}
