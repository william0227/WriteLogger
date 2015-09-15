using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

//Logger类
public class Logger
{
    private string path;


    public void SetPath(string Path)
    {
        path = Path;
    }


    //Logger的构造函数
    public Logger(string Path)
    {
        path = Path;
    }


    //写Logger的方法
    public void Write(object Content)
    {
        string content = (string)Content;
        FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write);
        byte[] bytes = UTF8Encoding.Default.GetBytes(content);
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();

    }
}

namespace Program01
{

    class Program
    {
        static void Main(string[] args)
        {
            int number1 = 0;
            int number2 = 0;
            int sum = 0;
            bool correctInput = true;

            Logger logger = new Logger(@"D:\logger.txt");
            Thread thread = new Thread(new ParameterizedThreadStart(logger.Write));     


            //主线程提示用户输入两个整数，打印这两个整数的和
            //异步线程打印logger
            Console.WriteLine("Please input a number:");
            try
            {
                number1 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Please input another number:");
                try
                {
                    number2 = Convert.ToInt32(Console.ReadLine());
                    sum = number1 + number2;
                    thread.Start("Do some computing .....");        //输入正确的情况
                }
                catch
                {
                    correctInput = false;
                    thread.Start("Input Error!!");              //输入错误的情况
                }

            }
            catch
            {
                correctInput = false;
                thread.Start("Input Error!!");          //输入错误的情况
            }

            if (correctInput)
            {
                Console.WriteLine("sum = " + sum);
            }
           
            Console.ReadKey();
        }
    }
}
