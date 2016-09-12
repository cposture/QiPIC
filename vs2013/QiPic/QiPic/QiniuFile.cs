using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qiniu.Conf;
using Qiniu.RSF;
using Qiniu.RS;
using System.Net;
using Qiniu.IO;
using Qiniu.RPC;


namespace QiPic
{
    class QiniuFile
    {
        string m_access_key;
        string m_secret_key;

        public QiniuFile(string ak, string sk)
        {
            m_access_key = ak;
            m_secret_key = sk;
        }

        public void Download(string filename, string dir)
        {
            
        }
    }
}
