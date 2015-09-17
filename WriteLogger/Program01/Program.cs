using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Program01
{
    //Buffer类
    public class Buffer
    {
        public string data;         //Buffer的数据成员

        //Buffer的构造函数
        public Buffer()
        {
            data = "";
        }

        //向Buffer中写的方法
        public void WriteToBuffer(object content)
        {
            data += (string)content;
        }
    }


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

        //读Buffer并且写入Logger的方法
        public void WriteLogger(object buf)
        {
            Buffer buffer = (Buffer)buf;

            Monitor.Enter(buffer);          //获取排它锁
            Monitor.Wait(buffer);           //释放排它锁，阻断当前线程
            FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write);
            string str = "";
            
            for (int i = 0; i < buffer.data.ToString().Length; i++)
            {
                

                str += buffer.data.ToString().ToString()[i];
            }
            byte[] bytes = UTF8Encoding.Default.GetBytes(str);
            fs.Write(bytes, 0, bytes.Length);

            Thread.Sleep(300);
            Monitor.Pulse(buffer);              //写入结束，通知其他线程
            Monitor.Exit(buffer);               //释放排它锁


            fs.Close();

        }

    }


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

            //线程1：读Buffer并写入Logger
            Thread thread1 = new Thread(new ParameterizedThreadStart(logger.WriteLogger));
            thread1.Start(buffer);          //先开启线程1


            //线程2：将第一次运算信息写入Buffer
            Thread thread2 = new Thread(new ParameterizedThreadStart(buffer.WriteToBuffer));

            //线程3：将第二次运算信息写入Buffer
            Thread thread3 = new Thread(new ParameterizedThreadStart(buffer.WriteToBuffer));



            //进行第一次计算
            Console.WriteLine("Please input the first number:");
            
            try
            {
                number1 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Please input the second number:");
                try
                {
                    number2 = Convert.ToInt32(Console.ReadLine());
                   
                    //开启线程2
                    thread2.Start("Do some computing......");             //输入正确的情况     
                    Monitor.Enter(buffer);                              //获取排他锁
                    sum = number1 + number2;
                    Monitor.Pulse(buffer);                              //计算完成，通知其他线程
                }
                catch
                {

                    correctInput = false;

                    thread2.Start("Input Error!!");            //输入错误的情况
                    Monitor.Enter(buffer);
                    Monitor.Pulse(buffer);
                }

            }
            catch
            {
                correctInput = false;

                thread2.Start("Input Error!!");            //输入错误的情况
                Monitor.Enter(buffer);
                Monitor.Pulse(buffer);
            }

            if (correctInput)
            {
                Console.WriteLine("sum = " + sum);
            }

            


            //进行第二次计算
            Console.WriteLine("Please input the first number:");

            try
            {
                number1 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Please input the second number:");
                try
                {
                    number2 = Convert.ToInt32(Console.ReadLine());
                    
                    thread3.Start("Do some computing......");             //输入正确的情况
                    Monitor.Enter(buffer);
                    sum = number1 + number2;
                    Monitor.Pulse(buffer);
                }
                catch
                {

                    correctInput = false;

                    thread3.Start("Input Error!!");            //输入错误的情况
                    Monitor.Enter(buffer);
                    Monitor.Pulse(buffer);
                }

            }
            catch
            {
                correctInput = false;

                thread3.Start("Input Error!!");            //输入错误的情况
                Monitor.Enter(buffer);
                Monitor.Pulse(buffer);
            }

            if (correctInput)
            {
                Console.WriteLine("sum = " + sum);
            }

            Monitor.Wait(buffer, 1000);         //写入Buffer结束，释放排它锁，进入等待
            Monitor.Exit(buffer);               //释放排它锁
            Console.ReadKey();
                   
        }
    }
}
