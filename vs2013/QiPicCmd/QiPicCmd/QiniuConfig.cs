using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QiPicCmd
{
    class QiniuConfig
    {
        public QiniuConfig(string ak, string sk, string baseurl, string bucketname)
        {
            m_access_key = ak;
            m_secret_key = sk;
            m_baseurl = baseurl;
            m_bucketname = bucketname;
        }

        private string m_access_key;
        private string m_secret_key;
        private string m_baseurl;
        private string m_bucketname;

        public string access_key
        {
            set
            {
                m_access_key = value;
            }
            get
            {
                return m_access_key;
            }
        }
        public string secret_key
        {
            set
            {
                m_secret_key = value;
            }
            get
            {
                return m_secret_key;
            }
        }
        public string baseurl
        {
            set
            {
                m_baseurl = value;
            }
            get
            {
                return m_baseurl;
            }
        }
        public string bucketname
        {
            set
            {
                m_bucketname = value;
            }
            get
            {
                return m_bucketname;
            }
        }
    }
}
