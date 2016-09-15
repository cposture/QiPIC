using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace QiPicCmd
{
    class QiPicFileSystem: QiniuFile
    {
        public QiPicFileSystem(QiniuConfig conf):base(conf)
        {
            
        }

        public void CreateDirectory(string dir)
        {
            dealDir(ref dir);
            try
            {
                // 创建文件
                FileStream fs = new FileStream("dir", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(dir);
                sw.Close();
                Upload("dir", false, dir);
                File.Delete("dir");
            }
            catch(Exception e)
            {
                if (File.Exists("dir"))
                    File.Delete("dir");
                throw new Exception("Error: Create directory failed. Please check directory exists or network connection.\n" + e.Message);
            }
        }

        public List<string> getDirectory(string base_dir)
        {
            List<string> dirlist = new List<string>();
            List<string> tmp;

            try
            {
                dealDir(ref base_dir);
                tmp = getFilesWithPrefix(base_dir);
            }
            catch(Exception e)
            {
                throw new Exception("Error: List directory failed. Please check directory exists or network connection.\n" + e.Message);
            }
            
            foreach(var i in tmp)
            {
                if (i.EndsWith(m_spilt_tag.ToString()) 
                    && i.StartsWith(m_spilt_tag.ToString()) 
                    && i != base_dir)
                {
                    dirlist.Add(i);
                }
                    
            }
            return dirlist;
        }

        public void UploadToDir(string filepath, string newname, string dir)
        {
            dealDir(ref dir);

            if (!IsDirExists(dir))
                Upload(filepath, false, dir + newname);
            else
                throw new Exception("Error: Upload file to directory failed. Please check directory exists or network connection.\n");
        }

        private void dealDir(ref string dir)
        {
            dir.Trim();

            // 对路径中的 '\'、'/' 替换为 m_spilt_tag
            dir.Replace('\\', m_spilt_tag);
            dir.Replace('/', m_spilt_tag);

            if (!dir.StartsWith(m_spilt_tag.ToString()))
            {
                dir = m_spilt_tag + dir;
            }

            if (!dir.EndsWith(m_spilt_tag.ToString()))
            {
                dir = dir + m_spilt_tag;
            }
        }

        private bool IsDirExists(string dir)
        {
            return IsFileExists(dir);
        }

        private char m_spilt_tag = '/';
    }
}
