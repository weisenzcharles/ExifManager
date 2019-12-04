using System;

namespace MagicFile
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintMenu();
            while (true)
            {
                Console.Write("选择你要执行的功能模块：");
                var command = Console.ReadLine();
                Console.WriteLine();

                if (command == "1" && Confirm())
                {
                    Console.WriteLine("正在执行中......");

                    Console.WriteLine("执行结束");
                    PrintMenu();
                }
            }

        }
        static bool Confirm()
        {
            Console.WriteLine("是否确认执行此操作? ");
            Console.WriteLine("确认请输入[Y]，按任意键返回菜单");
            var key = Console.ReadKey();
            Console.WriteLine("");
            if (key.KeyChar.ToString().ToUpper() == "Y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 打印菜单。
        /// </summary>
        static void PrintMenu()
        {
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("---------------------------- MagicFile v1.0.1 -----------------------------");
            Console.WriteLine("1、自动处理文件信息");
            Console.WriteLine("2、手动处理文件信息");
            Console.WriteLine("3、手动校准文件元数据");
            Console.WriteLine("4、自动根据元数据命名文件");
            Console.WriteLine("---------------------------------------------------------------------------");
        }
    }
}
