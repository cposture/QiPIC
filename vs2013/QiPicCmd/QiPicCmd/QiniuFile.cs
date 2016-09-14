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
        public QiniuFile(string ak, string sk, string baseurl, string bucketname, string savedir = "C:\\QiPic\\")
        {
            m_access_key = ak;
            m_secret_key = sk;
            m_baseurl = baseurl;
            m_bucketname = bucketname;
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
            string baseUrl = GetPolicy.MakeBaseUrl(m_baseurl, filename);
            WebClient web = new WebClient();

            web.DownloadFile(baseUrl, m_save_dir + filename);
        }

        public Entry Upload(string filepath, bool isOverlay)
        {
            // 由外部检查获取成功与否
            filepath.Trim();

            if (false == File.Exists(filepath))
                throw new Exception("Error: Upload failed. File not Exists");

            string filename = getFilenameFromPath(filepath);

            //设置账号的AK和SK
            Qiniu.Conf.Config.ACCESS_KEY = m_access_key;
            Qiniu.Conf.Config.SECRET_KEY = m_secret_key;

            PutPolicy put;
            IOClient target = new IOClient();
            PutExtra extra = new PutExtra();
            Entry entry = null;

            // 判断是否覆盖上传
            if(isOverlay && GetFileInfo(filename, out entry))
            {
                //覆盖上传,<bucket>:<key>，表示只允许用户上传指定key的文件。在这种格式下文件默认允许“修改”，已存在同名资源则会被本次覆盖。
                put = new PutPolicy(m_bucketname + ":" + filename, 3600);
            }
            else if(!GetFileInfo(filename, out entry))
            {
                put = new PutPolicy(m_bucketname, 3600);
            }
            else
            {
                return null;
            }

            // 调用Token()方法生成上传的Token
            string upToken = put.Token();

            // 调用PutFile()方法上传
            PutRet ret = target.PutFile(upToken, filename, filepath, extra);

            // 获取文件上传信息
            GetFileInfo(filename, out entry);

            return entry;
        }

        private string getFilenameFromPath(string filepath)
        {
            // 获取文件名
            string filename;
            int index = filepath.LastIndexOf(@"\");

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
            entry = client.Stat(new EntryPath(m_bucketname, filename));
            return entry.OK;
        }

        private string m_access_key;
        private string m_secret_key;
        private string m_save_dir;
        private string m_baseurl;
        private string m_bucketname;

        public string access_key
        {
            set
            {
                m_access_key = value;
            }
        }
        public string secret_key
        {
            set
            {
                m_secret_key = value;
            }
        }
        public string save_dir
        {
            set
            {
                m_save_dir = value;
            }
        }
        public string baseurl
        {
            set
            {
                m_baseurl = value;
            }
        }
        public string bucketname
        {
            set
            {
                m_bucketname = value;
            }
        }

    }
}
