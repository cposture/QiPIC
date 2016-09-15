using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Qiniu.Conf;
using Qiniu.RSF;
using Qiniu.RS;
using System.Net;
using Qiniu.IO;
using Qiniu.RPC;

namespace QiPicCmd
{
    class QiniuFile
    {
        public QiniuFile(QiniuConfig qiniuconf, string savedir = "")
        {
            m_qiniuconf = qiniuconf;
            m_save_dir = savedir;

            if (!m_save_dir.EndsWith("/"))
                m_save_dir += "/";

            try
            {
                if (!Directory.Exists(m_save_dir))
                    Directory.CreateDirectory(m_save_dir);
            }
            catch(DirectoryNotFoundException e)
            {
                Console.WriteLine("Error: Create directory failed. Invalid savedir path!\n" + e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: Create directory failed. \n" + e.Message);
            }
        }

        public void Download(string filename)
        {
            // 由外部检查获取成功与否
            string baseUrl = GetPolicy.MakeBaseUrl("12", filename);
            WebClient web = new WebClient();

            web.DownloadFile(baseUrl, m_save_dir + filename);
        }

        public Entry Upload(string filepath, bool isOverlay = false, string newname = "")
        {
            // 由外部检查获取成功与否
            filepath.Trim();

            if (false == File.Exists(filepath))
                throw new Exception("Error: Upload failed. File not Exists");

            if (newname == "")
                newname = getFilenameFromPath(filepath);

            //设置账号的AK和SK
            Qiniu.Conf.Config.ACCESS_KEY = m_qiniuconf.access_key;
            Qiniu.Conf.Config.SECRET_KEY = m_qiniuconf.secret_key;

            PutPolicy put;
            IOClient target = new IOClient();
            PutExtra extra = new PutExtra();
            Entry entry = null;

            // 判断是否覆盖上传
            if(isOverlay && GetFileInfo(newname, out entry))
            {
                //覆盖上传,<bucket>:<key>，表示只允许用户上传指定key的文件。在这种格式下文件默认允许“修改”，已存在同名资源则会被本次覆盖。
                put = new PutPolicy(m_qiniuconf.bucketname + ":" + newname, 3600);
            }
            else if(!GetFileInfo(newname, out entry))
            {
                put = new PutPolicy(m_qiniuconf.bucketname, 3600);
            }
            else
            {
                return null;
            }

            // 调用Token()方法生成上传的Token
            string upToken = put.Token();

            // 调用PutFile()方法上传
            PutRet ret = target.PutFile(upToken, newname, filepath, extra);

            // 获取文件上传信息
            GetFileInfo(newname, out entry);

            return entry;
        }

        public List<string> getFilesWithPrefix(string prefix)
        {

            List<string> filelist = new List<string>();
            RSFClient client = new RSFClient(m_qiniuconf.bucketname);
            DumpRet files = client.ListPrefix(m_qiniuconf.bucketname, prefix);
            
            foreach(var i in files.Items)
            {
                filelist.Add(i.Key);
            }
            return filelist;
        }

        public bool IsFileExists(string filename)
        {
            Entry entry;

            return GetFileInfo(filename, out entry);
        }

        private string getFilenameFromPath(string filepath)
        {
            // 获取文件名
            string filename;
            int index = filepath.LastIndexOf(@"/");

            if (index == -1)
            {
                filename = filepath;
            }
            else
            {
                index++;
                filename = filepath.Substring(index, filepath.Length - index);
            }

            return filename;
        }

        private bool GetFileInfo(string filename, out Entry entry)
        {
            //实例化一个RSClient对象，用于操作BucketManager里面的方法
            RSClient client = new RSClient();
            //调用Stat方法获取文件的信息
            entry = client.Stat(new EntryPath(m_qiniuconf.bucketname, filename));
            return entry.OK;
        }

        private QiniuConfig m_qiniuconf; 
        private string m_save_dir;
        public string save_dir
        {
            set
            {
                m_save_dir = value;
            }
        }

    }
}
