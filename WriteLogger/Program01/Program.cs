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
    public void Write()
    {

        FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write);

        byte[] bytes = UTF8Encoding.Default.GetBytes(Buffer.data);
        fs.Write(bytes, 0, bytes.Length);

        fs.Close();

    }

}


//Buffer类
class Buffer
{
    public static string data;         //Buffer的数据成员

    //Buffer的构造函数
    public Buffer()
    {
        data = "";
    }

    public void WriteToBuffer(string Content)
    {
        data += Content;
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

            Buffer buffer = new Buffer();
            Logger logger = new Logger(@"D:\log.txt");
            Thread thread = new Thread(new ThreadStart(logger.Write));



            //主线程提示用户输入两个整数，打印这两个整数的和
            //异步线程打印logger


            Console.WriteLine("Please input first number:");
            try
            {
                number1 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Please input second number:");
                try
                {
                    number2 = Convert.ToInt32(Console.ReadLine());
                    sum = number1 + number2;
                    buffer.WriteToBuffer("Do something computing ......");             //输入正确的情况

                }
                catch
                {

                    correctInput = false;
                    buffer.WriteToBuffer("Input Error!!");            //输入错误的情况
                }

            }
            catch
            {
                correctInput = false;
                buffer.WriteToBuffer("Input Error!!");            //输入错误的情况
            }

            if (correctInput)
            {
                Console.WriteLine("sum = " + sum);
            }

            thread.Start();
            thread.Join();

            
            Console.WriteLine(Buffer.data);
            Console.ReadKey();







        }
    }
}
