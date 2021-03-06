﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using CommandLine;
using CommandLine.Text; // if you want text formatting helpers (recommended)

namespace QiPicCmd
{
    class Options
    {
        [Option('d', "dir", HelpText = "as")]
        public string SaveDir { get; set; }

        [Option('h', "help", HelpText = "Input directory path to save files")]
        public string HelpText { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Options options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                QiniuConfig conf = new QiniuConfig("Wege4i-gz1IyWpCEfjhfEjZDj9U7IAhCXwq5FzxP", "l9DlUgST1KhGInpA--QMqeY3sLmaQ6nBCp_HOpH9", "7xosys.com1.z0.glb.clouddn.com", "notebook");
                QiniuFile qiniu = new QiniuFile(conf, "D:/");

                qiniu.init();

                try
                {
                    // 创建文件
                    FileStream fs = new FileStream("uploadtest", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("uploadtest");
                    sw.Close();
                    qiniu.Upload("uploadtest", true);
                    File.Delete("uploadtest");
                }
                catch (Exception e)
                {
                    if (File.Exists("uploadtest"))
                        File.Delete("uploadtest");
                    return;
                }
                Console.WriteLine(options.SaveDir);
            }
            else 
            {
                ;
            }
        }
    }
}
