using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiPicCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace QiPicCmd.Tests
{
    [TestClass()]
    public class QiniuFileTests
    {
        private static string m_accesskey = "Wege4i-gz1IyWpCEfjhfEjZDj9U7IAhCXwq5FzxP";
        private static string m_secretkey = "l9DlUgST1KhGInpA--QMqeY3sLmaQ6nBCp_HOpH9";
        private static string m_baseurl = "7xosys.com1.z0.glb.clouddn.com";
        private static string m_bucketname = "notebook";



        private QiniuConfig m_good_conf = new QiniuConfig(m_accesskey, m_secretkey, m_baseurl, m_bucketname);
        private QiniuConfig m_badurl_conf = new QiniuConfig(m_accesskey, m_secretkey, "7xosys.com1.z0.glb.clouddn", m_bucketname);
        private QiniuConfig m_badaccesskey_conf = new QiniuConfig("Wege4i-gz1IyWpCEfjhfEjZDj9U7IAhCXwq5Fzxx", m_secretkey, m_baseurl, m_bucketname);
        private QiniuConfig m_badsecretkey_conf = new QiniuConfig(m_accesskey, "l9DlUgST1KhGInpA--QMqeY3sLmaQ6nBCp_HOpH8", m_baseurl, m_bucketname);
        private QiniuConfig m_badbucketname_conf = new QiniuConfig(m_accesskey, m_secretkey, m_baseurl, "noteboo");

        [TestMethod()]
        public void QiniuFileTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void DownloadTest()
        {
            QiniuConfig conf = m_good_conf;
            QiniuFile qiniu = new QiniuFile(conf, ".");

            qiniu.init();

            Download_None(qiniu);
            Download_NotExists(qiniu);
            Download_UrlError();
            Download_Accesskey_Error();
            Download_secretkey_Error();
            Download_bucketname_Error();
            Download_EmptyDirAndFilename_Error();

            return;
        }

        [TestMethod()]
        public void UploadTest()
        {
            QiniuConfig conf = m_good_conf;
            QiniuFile qiniu = new QiniuFile(conf, ".");

            qiniu.init();

            Upload_FileNotExists(qiniu);
            return;
        }

        [TestMethod()]
        public void getFilesWithPrefixTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void IsFileExistsTest()
        {
            //Assert.Fail();
        }

        void Download_None(QiniuFile qiniu)
        {
            try
            {
                qiniu.Download("");
            }
            catch (WebException e)
            {
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        void Download_NotExists(QiniuFile qiniu)
        {
            string error_notExistsFile = "notExistsFile";

            try
            {
                qiniu.Download(error_notExistsFile);
            }
            catch (WebException e)
            {
                if (File.Exists(error_notExistsFile))
                    File.Delete(error_notExistsFile);
                return;
            }

            if (File.Exists(error_notExistsFile))
                File.Delete(error_notExistsFile);

            Assert.Fail("No exception was thrown.");
        }

        void Download_UrlError()
        {
            QiniuConfig conf = m_badurl_conf;
            QiniuFile qiniu = new QiniuFile(conf, "D:/");
            string file = "/test/";


            try
            {
                qiniu.init();
                qiniu.Download(file);
            }
            catch(WebException e)
            {
                if (File.Exists(file))
                    File.Delete(file);
                return;
            }

            if (File.Exists(file))
                File.Delete(file);

            Assert.Fail("No exception was thrown.");
        }

        void Download_Accesskey_Error()
        {
            QiniuConfig conf = m_badaccesskey_conf;
            QiniuFile qiniu = new QiniuFile(conf, ".");

            try
            {
                qiniu.init();
                qiniu.Download("/test/");
            }
            catch (WebException e)
            {
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        void Download_secretkey_Error()
        {
            QiniuConfig conf = m_badsecretkey_conf;
            QiniuFile qiniu = new QiniuFile(conf, ".");

            try
            {
                qiniu.init();
                qiniu.Download("/test/");
            }
            catch (WebException e)
            {
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        void Download_bucketname_Error()
        {
            QiniuConfig conf = m_badbucketname_conf;
            QiniuFile qiniu = new QiniuFile(conf, "D:/");
            string file = "/test/";


            try
            {
                qiniu.init();
                qiniu.Download(file);
            }
            catch (WebException e)
            {
                if (File.Exists(file))
                    File.Delete(file);
                return;
            }

            if (File.Exists(file))
                File.Delete(file);

            Assert.Fail("No exception was thrown.");
        }

        void Download_EmptyDirAndFilename_Error()
        {
            string empty_dir = "";
            string empty_filename = "";
            QiniuConfig conf = new QiniuConfig("Wege4i-gz1IyWpCEfjhfEjZDj9U7IAhCXwq5FzxP", "l9DlUgST1KhGInpA--QMqeY3sLmaQ6nBCp_HOpH9", "7xosys.com1.z0.glb.clouddn.com", "notebook");
            QiniuFile qiniu = new QiniuFile(conf, empty_dir);

            try
            {
                qiniu.init();
                qiniu.Download(empty_filename);
            }
            catch (ArgumentException e)
            {
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        void Upload_FileNotExists(QiniuFile qiniu)
        {
            try
            {
                qiniu.Upload("");
            }
            catch (Exception e)
            {
                StringAssert.Contains(e.Message, "Error: Upload failed. File not Exists");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

    }
}