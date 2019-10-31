using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using System.Xml;
using System.ComponentModel;
using System.Diagnostics;

namespace laungh
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public class s_ver
    {
        public string ver_string;
        public int ver;
        public string file;
    }
    public partial class MainWindow : Window
    {
        s_ver m_ver = new s_ver();
        s_ver m_gver;
        List<s_ver> m_other_vers = new List<s_ver>();
        public MainWindow()
        {
            InitializeComponent();
            WebBrowserOverlay wbo = new WebBrowserOverlay(l_ie, 0, 0); 
            System.Windows.Controls.WebBrowser w = wbo.WebBrowser; 
            w.Navigate(new Uri("http://mario.web.yymoon.com/client.html"));
        }

        public void DragWindow(object sender, MouseButtonEventArgs argss)
        {
            this.DragMove();
        }

        public int get_ver(string s)
        {
            string[] ss = s.Split('.');
            int ver = int.Parse(ss[0]) * 10000 + int.Parse(ss[1]);
            return ver;
        }

        private void l_start_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.CurrentDirectory + "/main/boxmaker.exe");
            this.Close();
        }

        private void l_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void l_min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            l_start.IsEnabled = false;
            try
            {
                FileStream fs = new FileStream("ver", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                string str = sr.ReadToEnd();
                sr.Close();
                fs.Close();
                m_ver.ver = get_ver(str);
                m_ver.ver_string = str;
            }
            catch (Exception)
            {
                l_state.Content = "找不到本地版本文件";
                MessageBox.Show("找不到本地版本文件");
                return;
            }
            l_state.Content = "正在获取版本信息";
            l_ver.Content = m_ver.ver_string;
            Random r = new Random();
            int rr = r.Next();
            String s = "http://mario.oss.yymoon.com/win/version.xml?v" + rr.ToString();
            HttpGet(s, ver_callback);
        }

        private void ver_callback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)(result.AsyncState);
                StreamReader sr = new StreamReader(request.EndGetResponse(result).GetResponseStream());
                string data = sr.ReadToEnd();
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(data);
                XmlNodeList xnl = xmldoc.SelectNodes("vers/ver");
                for (int i = 0; i < xnl.Count; i++)
                {
                    XmlNode xn = xnl.Item(i);
                    s_ver v = new s_ver();
                    v.ver_string = xn.Attributes["v"].Value;
                    v.ver = get_ver(v.ver_string);
                    v.file = xn.Attributes["file"].Value;
                    if (v.ver > m_ver.ver)
                    {
                        m_other_vers.Add(v);
                    }
                }
            }
            catch
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    l_state.Content = "获取版本信息失败";
                    MessageBox.Show("获取版本信息失败");
                }));
                return;
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                check_ver();
            }));
        }

        private void HttpGet(string url, AsyncCallback callback)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.BeginGetResponse(callback, request);
        }

        private void check_ver()
        {
            l_ver.Content = m_ver.ver_string;
            if (m_other_vers.Count > 0)
            {
                m_gver = m_other_vers[0];
                m_other_vers.RemoveAt(0);
                l_state.Content = "正在更新版本到 " + m_gver.ver_string;
                WebClient wc = new WebClient();
                if (!Directory.Exists("download"))
                {
                    Directory.CreateDirectory("download");
                }
                string name = m_gver.file.Substring(m_gver.file.LastIndexOf("/") + 1);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
                wc.DownloadFileAsync(new Uri(m_gver.file), "download/" + name);
                l_progress.Value = 0;
            }
            else
            {
                l_state.Content = "更新完成";
                l_start.IsEnabled = true;
            }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                l_progress.Value = e.ProgressPercentage;
                l_state.Content = "正在更新版本到 " + m_gver.ver_string + string.Format(", 当前进度 {0}/{1} (byte)", e.BytesReceived, e.TotalBytesToReceive);
            }));
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                string path = m_gver.file.Substring(m_gver.file.LastIndexOf("/") + 1);
                path = "download/" + path;
                if (utils.UnZip(path, "./main"))
                {
                    l_progress.Value = 100;
                    m_ver = m_gver;
                    FileStream fs = new FileStream("ver", FileMode.Open, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(m_ver.ver_string);
                    sw.Close();
                    fs.Close();
                    check_ver();
                }
                else
                {
                    l_state.Content = "解压失败";
                    MessageBox.Show("解压失败");
                }
            }));
        }

        
    }
}
