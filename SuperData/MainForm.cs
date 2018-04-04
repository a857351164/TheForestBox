using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCWin;
using System.Diagnostics;
using Microsoft;
using Microsoft.Win32;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Net;
using System.Text.RegularExpressions;
using Gecko;
namespace SuperData
{
    public partial class MainFrom : CCSkinMain
    {
        private static string _GamePath = "";
        private static string _FilePath = "";

        private readonly string xulrunnerPath = Application.StartupPath + "\\xulrunner";
        private const string testUrl = "http://localhost/Items.html";
        private GeckoWebBrowser Browser;

        public MainFrom()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://theforest.world5.cn/archiving/banben.php");
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Version v1 = new Version(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Version v2 = new Version(responseString);
            if (v1 < v2)
            {
                MessageBox.Show("此版本为测试版：现在有了一个重大更新请到Q群434779542 获取最新版");
            }
            InitializeComponent();
            //火狐内核浏览器
            Console.WriteLine(xulrunnerPath);
            Xpcom.Initialize(xulrunnerPath);
            geckoWebBrowser1.Navigate(testUrl);
            //this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            //this.skinEngine1.SkinFile = Application.StartupPath + "\\Skins\\Skins\\DeepCyan.ssk";
            skinTabControl1.SelectedTab = skinTabPage1;
            CkDir();
            _GamePath = GetGamePath(0);//读取游戏路径
            GamePath.Text = _GamePath;
            _FilePath = GetFilePath();
            SetUserFile();
            GetUserQq();//读取用户
            GameXiuNo.Hide();

            //UpLoadFile("C:\\Users\\85735\\Desktop\\asd.zip", "http://theforest.world5.cn/upfile.php",false);
            // this.uploadFileByHttp(" http://localhost:1878/UploadFileWebSite/UploadFile.aspx", @"D:/1.txt");
        }
        private void CkDir()
        {
            if (!Directory.Exists(".\\config"))
            {
                Directory.CreateDirectory(".\\config");
            }
            if (!Directory.Exists(".\\exe"))
            {
                Directory.CreateDirectory(".\\exe");
            }
            if (!Directory.Exists(".\\Image"))
            {
                Directory.CreateDirectory(".\\Image");
            }
            if (!Directory.Exists(".\\user"))
            {
                Directory.CreateDirectory(".\\user");
            }
            if (!Directory.Exists(".\\存档缓存"))
            {
                Directory.CreateDirectory(".\\存档缓存");
            }
            if (!Directory.Exists(".\\我保存的存档"))
            {
                Directory.CreateDirectory(".\\我保存的存档");
            }
            if (!Directory.Exists(".\\下载的存档"))
            {
                Directory.CreateDirectory(".\\下载的存档");
            }
            if (!Directory.Exists(".\\修改器"))
            {
                Directory.CreateDirectory(".\\修改器");
            }
        }
         
        /// 最小化窗体
        private void skinAnimatorImg3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        /// 关闭窗体
        private void skinAnimatorImg5_Click(object sender, EventArgs e)
        {
            Close();
        }
        /// 最小化
        private void skinAnimatorImg3_MouseLeave(object sender, EventArgs e)
        {
            skinAnimatorImg3.AnimatorStart = false;
        }

        /// 最大化
        private void skinAnimatorImg5_MouseMove(object sender, MouseEventArgs e)
        {
            skinAnimatorImg5.AnimatorStart = true;
        }

        /// 最大化
        private void skinAnimatorImg5_MouseLeave(object sender, EventArgs e)
        {
            skinAnimatorImg5.AnimatorStart = false;
        }

        /// 最小化
        private void skinAnimatorImg3_MouseMove(object sender, MouseEventArgs e)
        {
            skinAnimatorImg3.AnimatorStart = true;
        }

        private void GameStar_Click(object sender, EventArgs e)
        {
            if (Processes("TheForest")) {
                System.Diagnostics.Process.Start("steam://rungameid/242760");
            }
            else
            {
                MessageBox.Show("游戏正在运行请稍等");
                return;
            }
        }

        //检测进程是否在运行
        private static bool Processes(string e)
        {
            Process[] app = Process.GetProcessesByName(e);
            if (app.Length > 0)
            {
                return false;
            }
            return true;
        }
        //根据注册表检测游戏目录
        private static string GetGamePath(int ifStar)
        {
            if (ifStar != 1) {
                if (File.Exists(".\\config\\dir.txt"))
                {
                    
                    StreamReader sr = new StreamReader(".\\config\\dir.txt", Encoding.Default);
                    String line;
                    if ((line = sr.ReadLine()) != null)
                    {
                        if (GamePathCk(line.ToString()))
                        {
                            return line.ToString();
                        }
                    }
                    sr.Close();
                }
            }
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 242760", false))
            {
                object obj = null;
                if (key != null)
                {
                    obj = key.GetValue("InstallLocation");  //读取注册表内容
                    FileStream fs = new FileStream(".\\config\\dir.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs); // 创建写入流
                    sw.WriteLine(obj.ToString()); // 写入Hello World
                    sw.Close();
                    return obj.ToString();
                }
                return "游戏地址检测失败,请手动选择";
            }
        }
        //自动获取存档根目录
        private static string GetFilePath()
        {
           return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\SKS\\TheForest";
        }
        //自动加载存档用户目录
        private void SetUserFile()
        {
            if (Directory.Exists(_FilePath))
            {
                int pathNum = 0;
                DirectoryInfo dir = new DirectoryInfo(_FilePath);
                DirectoryInfo[] dii = dir.GetDirectories();
                foreach (DirectoryInfo d in dii)
                {
                    CunDir.Items.Add(Path.GetFileNameWithoutExtension(d.FullName));
                    if (pathNum == 0)
                        CunDir.Text = Path.GetFileNameWithoutExtension(d.FullName);
                }
            }
            else {
                CunDir.Text ="请进入游戏后,随便保存一个存档在点击重新读取。";
            }
            
        }
        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlDocument htmlDoc = webBrowser2.Document;
            htmlDoc.Click += new HtmlElementEventHandler(htmlDoc_Click);
        }
        
        //Html点击事件
        private void htmlDoc_Click(object sender, HtmlElementEventArgs e)
        {
            var file = "";
            var name = "";
            var md5 = "";
            var tid = "";
            var filename = "";
            HtmlDocument doc = sender as HtmlDocument;
            HtmlElement ele = doc.GetElementFromPoint(e.ClientMousePosition);
            string str = ele.OuterHtml;

            Regex reg = new Regex("(?<=file=\")(.*?)(?=\")", RegexOptions.IgnoreCase);//[^(<td>))] 
            MatchCollection mc = reg.Matches(str);

            foreach (Match m in mc)
            {
                file = m.Value;
            }
            Regex reg1 = new Regex("(?<=fileName=\")(.*?)(?=\")", RegexOptions.IgnoreCase);
            MatchCollection mc1 = reg1.Matches(str);

            foreach (Match m in mc1)
            {
                name = m.Value;
            }
            Regex reg2 = new Regex("(?<=md5=\")(.*?)(?=\")", RegexOptions.IgnoreCase);
            MatchCollection mc2 = reg2.Matches(str);

            foreach (Match m in mc2)
            {
                md5 = m.Value;
            }

            Regex reg3 = new Regex("(?<=tid=\")(.*?)(?=\")", RegexOptions.IgnoreCase);
            MatchCollection mc3 = reg3.Matches(str);
            foreach (Match m in mc3)
            {
                tid = m.Value;
            }

            if (name != "") {
                filename = ".\\下载的存档\\" + name + "_" + tid + ".zip";
                if (!File.Exists(filename))
                {
                    DownloadFile("http://theforest.world5.cn" + file, filename, DownCun, DownJindu);
                    CunDaoru.Text = filename;
                    skinTabControl1.SelectedTab = skinTabPage3;
                    MessageBox.Show("存档已加载完成,选择挡位击导入即可。");
                }
                else if(md5 != GetMD5HashFromFile(filename))
                {
                    DownloadFile("http://theforest.world5.cn" + file, filename, DownCun, DownJindu);
                    CunDaoru.Text = filename;
                    skinTabControl1.SelectedTab = skinTabPage3;
                    MessageBox.Show("存档已加载完成,选择挡位击导入即可。");
                }
            }
            return;
        }
      
        private void GnameDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            if (GamePathCk(path.SelectedPath))
            {
                GamePath.Text = path.SelectedPath;
                _GamePath = path.SelectedPath;
                FileStream fs = new FileStream(".\\config\\dir.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs); // 创建写入流
                sw.WriteLine(path.SelectedPath); // 写入Hello World
                sw.Close();
            }
            else {
                MessageBox.Show("请选择正确的游戏路径");
            }
            
        }
        //游戏正确目录检测
        private static bool GamePathCk(string dir)
        {
            if (File.Exists(dir+"\\TheForest.exe"))
            {
                return true;
            }
            return false;
        }
        //压缩目录
        public static bool ZipYa(string dirPath, string zipFilePath, out string err)
        {
            err = "";
            if (dirPath == string.Empty)
            {
                err = "要压缩的文件夹不能为空！";
                return false;
            }
            if (!Directory.Exists(dirPath))
            {
                err = "要压缩的文件夹不存在！";
                return false;
            }
            //压缩文件名为空时使用文件夹名＋.zip  
            if (zipFilePath == string.Empty)
            {
                if (dirPath.EndsWith("\\"))
                {
                    dirPath = dirPath.Substring(0, dirPath.Length - 1);
                }
                zipFilePath = dirPath + ".zip";
            }

            try
            {
                string[] filenames = Directory.GetFiles(dirPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    s.SetLevel(9);
                    byte[] buffer = new byte[4096];
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            return true;
        }
        //解压ZIP
        public static bool ZipJie(string zipFilePath, string unZipDir, out string err)
        {
            err = "";
            if (zipFilePath == string.Empty)
            {
                err = "压缩文件不能为空！";
                return false;
            }
            if (!File.Exists(zipFilePath))
            {
                err = "压缩文件不存在！";
                return false;
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹  
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith("\\"))
                unZipDir += "\\";
            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);

            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(unZipDir + directoryName);
                        }
                        if (!directoryName.EndsWith("\\"))
                            directoryName += "\\";
                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                            {

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }//while  
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            return true;
        }//解压结束  
        //重新检测游戏目录
        private void CunDuqu_Click(object sender, EventArgs e)
        {
            CunDir.Items.Clear();
            SetUserFile();
        }

        private void MainFrom_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        //文件拖入程序执行
        private void MainFrom_DragDrop(object sender, DragEventArgs e)
        {
            string FileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            string ExName = Path.GetExtension(FileName);
                if (ExName == ".zip") {
                    CunDaoru.Text = FileName;
                    return;
                }
                else {
                MessageBox.Show("目前只支持zip文件");
            }
        }
        //点击开始写入文档
        private void CunXieru_Click(object sender, EventArgs e)
        {
            string type;
            string dirS;
            string dirBk;
            string err = "";
            int bk_name = 1;

            if (_FilePath == "存档目录获取失败,请手动选择")
            {
                MessageBox.Show("存档目录获取失败.如自动检测失败请手动选择");
                return;
            }
            
            if (CunDaoru.Text == "请将存档拖入到本程序/或者从存档大厅选择")
            {
                MessageBox.Show("请将存档拖入到程序,或者在大厅选择,目前只支持Zip");
                return;
            }
            if (CunDir.Text == "请选择用户目录")
            {
                MessageBox.Show("检测目录内没有任何存档请先进入游戏保存一个存档");
                return;
            }
            if (CunDanji.Checked == true)
            {
                type = "SinglePlayer";
            }
            else if (CunLianji.Checked == true)
            {
                type = "Multiplayer";
            }
            else
            {
                return;
            }
            if (CunWeizhi.Text == "请选择存档位置")
            {
                MessageBox.Show("你得选择一下放到那个位置哈");
                return;
            }
            //数据监测完成,开始执行逻辑
            dirS = _FilePath + "\\" + CunDir.Text + "\\" + type+"\\"+CunWeizhi.Text;
            if (Directory.Exists(dirS))//判断是否存在
            {
                dirBk = _FilePath + "\\" + CunDir.Text + "\\存档备份Q群434779542";
                DialogResult dr = MessageBox.Show("目录已经存在,是否继续执行？如继续执行则会自动备份原来存档。", "提示", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    if (Directory.Exists(dirBk))//判断是否存在
                    {
                        DirectoryInfo bk_dir = new DirectoryInfo(dirBk);
                        bk_name = bk_dir.GetDirectories().Length + 1;
                    }
                    ZipYa(dirS, dirBk + "\\备份存档" + bk_name + ".zip", out err);
                }
                else if (dr == DialogResult.Cancel)
                {
                    return;
                }
            }
            int pathNum = 0;
            int status = 0;
            string huancunPath = ".\\存档缓存";
            string dirZi = "";
            if (ZipJie(CunDaoru.Text, ".\\存档缓存", out err)) {
                if (File.Exists(huancunPath+"\\__RESUME__"))
                {
                    status = 1;
                }
                else
                {
                    DirectoryInfo dir = new DirectoryInfo(huancunPath);
                    DirectoryInfo[] dii = dir.GetDirectories();
                    foreach (DirectoryInfo d in dii)
                    {
                        if (pathNum == 0)
                        {
                            dirZi = Path.GetFileNameWithoutExtension(d.FullName);
                            break;
                        }
                    }
                    if(dirZi != "")
                    {
                        if (File.Exists(huancunPath +"\\"+ dirZi + "\\__RESUME__"))
                        {
                            huancunPath += "\\" + dirZi;
                            status = 1;
                        }
                    }
                }
                if(status == 1)
                {
                    MoveFolder(huancunPath, dirS);
                    if (File.Exists(dirS + "\\thumb.png"))
                    {
                        File.Copy(dirS + "\\thumb.png", ".\\Image\\thumb" + CunWeizhi.Text + ".png", true);
                        System.Drawing.Image img = System.Drawing.Image.FromFile(".\\Image\\thumb" + CunWeizhi.Text + ".png");
                        System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                        img.Dispose();
                        CunYulan.Image = bmp;// System.Drawing.Image.FromFile(".\\Image\\thumb" + CunWeizhi.Text + ".png");
                    }
                    //CunYulan.BackgroundImage = Image.FromFile(dirS + "\\thumb.png");
                    MessageBox.Show("存档载入成功");
                }
                else
                {
                    MessageBox.Show("可能是加载失败了,请检查压缩包");
                }
            }
            else
            {
                MessageBox.Show(err);
            }
        }


    public static void MoveFolder(string sourcePath, string destPath)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destPath))
                {
                    try
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("创建目标目录失败：" + ex.Message);
                    }
                }
                List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                files.ForEach(c =>
                {
                    string destFile = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    if (File.Exists(destFile))
                    {
                        File.Delete(destFile);
                    }
                    File.Move(c, destFile);
                });
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
                folders.ForEach(c =>
                {
                    string destDir = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    MoveFolder(c, destDir);
                });
            }
            else
            {
                throw new DirectoryNotFoundException("源目录不存在！");
            }
        }
        //打开存档文件目录
        private void CunOpenDir_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(".\\我保存的存档"))
            {
                System.Diagnostics.Process.Start(".\\我保存的存档");
            }
        }

        public void DownloadFile(string URL, string filename, System.Windows.Forms.ProgressBar prog, System.Windows.Forms.Label label1)
        {
            float percent = 0;
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                if (prog != null)
                {
                    prog.Maximum = (int)totalBytes;
                }
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);
                    if (prog != null)
                    {
                        prog.Value = (int)totalDownloadedByte;
                    }
                    osize = st.Read(by, 0, (int)by.Length);

                    percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                    label1.Text = "当前补丁下载进度" + percent.ToString() + "%";
                    System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则label1将因为循环执行太快而来不及显示信息
                }
                so.Close();
                st.Close();
                so.Dispose();
                st.Dispose();
            }
            catch (System.Exception)
            {
                label1.Text = "下载地址出错";
                throw;
            }
        }

        private void Xgq073_Click(object sender, EventArgs e)
        {
            XgqDownLab.Text = "0.73版本修改器开始安装,请勿关闭程序";
            if (_GamePath == "游戏地址检测失败,请手动选择")
            {
                MessageBox.Show("自动获取游戏目录失败,请手动选择游戏目录。");
                return;
            }
                if (!Processes("TheForest"))
                {
                MessageBox.Show("请关闭游戏后在点击安装。");
                return;
                }
            
            string err = "";
            if (!File.Exists(".\\修改器\\0.73.zip"))
            {
                DownloadFile("http://theforest.world5.cn/box/%E4%BF%AE%E6%94%B9%E5%99%A8/0.73.zip", ".\\修改器\\0.73.zip", XgqJindu, XgqDownLab);
            }
            else
            {
                string md5File = GetMD5HashFromFile(".\\修改器\\0.73.zip");
                if (md5File != "4652cba985e9ad2d9422508261a91e91")
                    DownloadFile("http://theforest.world5.cn/box/%E4%BF%AE%E6%94%B9%E5%99%A8/0.73.zip", ".\\修改器\\0.73.zip", XgqJindu, XgqDownLab);
            }
            XgqDownLab.Text = "0.73下载完成,正在安装";
            if (ZipJie(".\\修改器\\0.73.zip", _GamePath, out err))
            {
                MessageBox.Show("修改器安装成功,进入游戏按V键开启吧。");
                XgqDownLab.Text = "修改器安装成功,进入游戏按V键开启吧";
            }
            else
            {
                MessageBox.Show("修改器安装失败请联系群主。");
                XgqDownLab.Text = "修改器安装失败请联系群主。";
            }

        }

        private void Xgq062_Click(object sender, EventArgs e)
        {
            if (_GamePath == "游戏地址检测失败,请手动选择")
            {
                MessageBox.Show("自动获取游戏目录失败,请手动选择游戏目录。");
                return;
            }
            if (!Processes("TheForest"))
            {
                MessageBox.Show("请关闭游戏后在点击安装。");
                return;
            }
            XgqDownLab.Text = "0.62版本修改器开始安装,请勿关闭程序";
            string err = "";
            if (!File.Exists(".\\修改器\\0.62.zip"))
            {
                DownloadFile("http://theforest.world5.cn/box/%E4%BF%AE%E6%94%B9%E5%99%A8/0.62.zip", ".\\修改器\\0.62.zip", XgqJindu, XgqDownLab);
            }
            else
            {
                string md5File = GetMD5HashFromFile(".\\修改器\\0.62.zip");
                if (md5File != "fbed6cdd6086d4f4f776d9b576381323")
                DownloadFile("http://theforest.world5.cn/box/%E4%BF%AE%E6%94%B9%E5%99%A8/0.62.zip", ".\\修改器\\0.62.zip", XgqJindu, XgqDownLab);
            }
            XgqDownLab.Text = "0.62下载完成,正在安装";
            if (ZipJie(".\\修改器\\0.62.zip", _GamePath, out err))
            {
                MessageBox.Show("修改器安装成功,进入游戏按V键开启吧。");
                XgqDownLab.Text = "修改器安装成功,进入游戏按V键开启吧";
            }
            else
            {
                MessageBox.Show("修改器安装失败请联系群主。");
                XgqDownLab.Text = "修改器安装失败请联系群主。";
            }
        }

        private void XgqMxiufu_Click(object sender, EventArgs e)
        {
            if (_GamePath == "游戏地址检测失败,请手动选择")
            {
                MessageBox.Show("自动获取游戏目录失败,请手动选择游戏目录。");
                return;
            }
            if (!Processes("TheForest"))
            {
                MessageBox.Show("请关闭游戏后在点击安装。");
                return;
            }
            XgqDownLab.Text = "地图修复开始,请勿关闭程序";
            string err = "";
            if (!File.Exists(".\\修改器\\0.73M.zip"))
            {
                DownloadFile("http://theforest.world5.cn/box/%E4%BF%AE%E6%94%B9%E5%99%A8/0.73M.zip", ".\\修改器\\0.73M.zip", XgqJindu, XgqDownLab);
            }
            else
            {
                string md5File = GetMD5HashFromFile(".\\修改器\\0.73M.zip");
                if (md5File != "9e4af4ff5c3ff36cf2147eeee0c15e50")
                    DownloadFile("http://theforest.world5.cn/box/%E4%BF%AE%E6%94%B9%E5%99%A8/0.73M.zip", ".\\修改器\\0.73M.zip", XgqJindu, XgqDownLab);
            }
            XgqDownLab.Text = "地图修复补丁下载完成,正在安装";
            if (ZipJie(".\\修改器\\0.73M.zip", _GamePath, out err))
            {
                MessageBox.Show("实时地图修复成功,进入游戏按M键开启吧。");
                XgqDownLab.Text = "实时地图修复成功,进入游戏按M键开启吧。";
            }
            else
            {
                MessageBox.Show("实时地图修复失败请联系群主。");
                XgqDownLab.Text = "实时地图修复失败请联系群主。";
            }
        }
        private static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("发生错误:" + ex.Message);
            }
        }
        //读取用户存档
        private void CunDaochu_Click(object sender, EventArgs e)
        {
            string type;
            string dirS;
            string dirBk;
            string err = "";
            int bk_name = 1;

            if (_FilePath == "存档目录获取失败,请手动选择")
            {
                MessageBox.Show("存档目录获取失败.如自动检测失败请手动选择");
                return;
            }
            if (CunDir.Text == "请选择用户目录")
            {
                MessageBox.Show("检测目录内没有任何存档请先进入游戏保存一个存档");
                return;
            }
            if (CunName.Text == "读取存档,在此输入名字")
            {
                MessageBox.Show("保存存档前,要先起个名字哈.");
                    return;
            }
            if (CunDanji.Checked == true)
            {
                type = "SinglePlayer";
            }
            else if (CunLianji.Checked == true)
            {
                type = "Multiplayer";
            }
            else
            {
                return;
            }
            if (CunWeizhi.Text == "请选择存档位置")
            {
                MessageBox.Show("你得选择读取那个挡位哈");
                return;
            }
            dirS = _FilePath + "\\" + CunDir.Text + "\\" + type + "\\" + CunWeizhi.Text;
            //ConvertDateTimeToInt(time).ToString();
            if (Directory.Exists(dirS))
            {
                if (File.Exists(dirS + "\\__RESUME__"))
                {
                    
                    if (ZipYa(dirS, ".\\我保存的存档\\"+CunName.Text+".zip", out err)) {
                        MessageBox.Show("读取存档完成,想要查看请点击打开目录");
                    }
                    else
                    {
                         MessageBox.Show(err);
                    }
                }
                else
                {
                    MessageBox.Show(dirS);
                    MessageBox.Show("你选的存档内好像是空的哦。");
                    return;
                }
                
            }
            else
            {
                MessageBox.Show("没有找到你选的存档");
                return;
            }
            //MessageBox.Show("读取完成存档储存至" + Zmdir + "\\森林存档读取Q群434779542\\读取存档" + bk_name);
        }
        //程序拖动
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);



        bool beginMove = false;//初始化鼠标位置  
        int currentXPosition;
        int currentYPosition;
        private void MainFrom_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                beginMove = true;
                currentXPosition = MousePosition.X;//鼠标的x坐标为当前窗体左上角x坐标  
                currentYPosition = MousePosition.Y;//鼠标的y坐标为当前窗体左上角y坐标  
            }
        }

        private void MainFrom_MouseMove(object sender, MouseEventArgs e)
        {
            if (beginMove)
            {
                this.Left += MousePosition.X - currentXPosition;//根据鼠标x坐标确定窗体的左边坐标x  
                this.Top += MousePosition.Y - currentYPosition;//根据鼠标的y坐标窗体的顶部，即Y坐标  
                currentXPosition = MousePosition.X;
                currentYPosition = MousePosition.Y;
            }
        }

        private void MainFrom_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                currentXPosition = 0; //设置初始状态  
                currentYPosition = 0;
                beginMove = false;
            }
        }
        //开始读取QQ号
        [DllImport("user32.dll", EntryPoint = "FindWindowA", CharSet = CharSet.Ansi)]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(int hwnd, StringBuilder lpString, int cch);
        [DllImport("user32.dll", EntryPoint = "GetWindow")]
        public static extern int GetWindow(int hwnd, int wCmd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(int hWnd, StringBuilder lpClassName, int nMaxCount);
        private void UserNew_Click(object sender, EventArgs e)
        {
            GetUserQq();
        }
        public void GetUserQq()
        {
            int pathNum = 0;
            int hWin = FindWindow("CTXOPConntion_Class", null);
            if (hWin == 0)
            {
                return;
            }
            StringBuilder sbf = new StringBuilder(256);
            StringBuilder sbfClass = new StringBuilder(256);
            while (hWin > 0)
            {
                GetWindowText(hWin, sbf, sbf.Capacity);
                if (sbf.ToString().Contains("OP_"))
                {
                    string str = sbf.ToString();
                    string result = str.Substring(str.IndexOf('_') + 1);
                    UserGetQQ.Items.Add(result);
                    if (pathNum == 0)
                    {
                        UserGetQQ.Text = result;
                        UserName.Text = result;
                        webBrowser2.Url = new Uri("http://theforest.world5.cn/case.php?UQqName=" + result);
                        if (!File.Exists(".\\user\\" + result + ".png"))
                        {
                            if (Download("http://q1.qlogo.cn/g?b=qq&s=100&nk=" + result, ".\\user\\" + result + ".png"))
                            {
                                UserImg.Image = System.Drawing.Image.FromFile(".\\user\\" + result + ".png");
                            }
                        }
                        else
                        {
                            UserImg.Image = System.Drawing.Image.FromFile(".\\user\\" + result + ".png");
                        }
                    }
                    pathNum++;
                }
                do
                {
                    hWin = GetWindow(hWin, 2);
                    if (hWin == 0)
                    {
                        break;
                    }
                    GetClassName(hWin, sbfClass, sbfClass.Capacity);

                } while ("CTXOPConntion_Class".Equals(sbfClass));


            }
        }
        /// <summary>
        /// Http方式下载文件
        /// </summary>
        /// <param name="url">http地址</param>
        /// <param name="localfile">本地文件</param>
        /// <returns></returns>
        public bool Download(string url, string localfile)
        {
            bool flag = false;
            long startPosition = 0; // 上次下载的文件起始位置
            FileStream writeStream; // 写入本地文件流对象

            // 判断要下载的文件夹是否存在
            if (File.Exists(localfile))
            {

                writeStream = File.OpenWrite(localfile);             // 存在则打开要下载的文件
                startPosition = writeStream.Length;                  // 获取已经下载的长度
                writeStream.Seek(startPosition, SeekOrigin.Current); // 本地文件写入位置定位
            }
            else
            {
                writeStream = new FileStream(localfile, FileMode.Create);// 文件不保存创建一个文件
                startPosition = 0;
            }


            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(url);// 打开网络连接

                if (startPosition > 0)
                {
                    myRequest.AddRange((int)startPosition);// 设置Range值,与上面的writeStream.Seek用意相同,是为了定义远程文件读取位置
                }


                Stream readStream = myRequest.GetResponse().GetResponseStream();// 向服务器请求,获得服务器的回应数据流


                byte[] btArray = new byte[512];// 定义一个字节数据,用来向readStream读取内容和向writeStream写入内容
                int contentSize = readStream.Read(btArray, 0, btArray.Length);// 向远程文件读第一次

                while (contentSize > 0)// 如果读取长度大于零则继续读
                {
                    writeStream.Write(btArray, 0, contentSize);// 写入本地文件
                    contentSize = readStream.Read(btArray, 0, btArray.Length);// 继续向远程文件读取
                }

                //关闭流
                writeStream.Close();
                readStream.Close();

                flag = true;        //返回true下载成功
            }
            catch (Exception)
            {
                writeStream.Close();
                flag = false;       //返回false下载失败
            }

            return flag;
        }

        private void UserGetQQ_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!File.Exists(".\\user\\" + UserGetQQ.Text + ".png"))
            {
                if (Download("http://q1.qlogo.cn/g?b=qq&s=100&nk=" + UserGetQQ.Text, ".\\user\\" + UserGetQQ.Text + ".png"))
                {
                    UserImg.Image = System.Drawing.Image.FromFile(".\\user\\" + UserGetQQ.Text + ".png");
                }
            }
            else
            {
                UserImg.Image = System.Drawing.Image.FromFile(".\\user\\" + UserGetQQ.Text + ".png");
            }
            UserGetQQ.Text = UserGetQQ.Text;
            UserName.Text = UserGetQQ.Text;
        }

        private void CunFabu_Click(object sender, EventArgs e)
        {
            if(UserName.Text == "登陆")
            {
                MessageBox.Show("请登陆一个QQ,然后在首页点击重新获取。");
            }
            string type;
            string dirS;
            string err = "";
            if (_FilePath == "存档目录获取失败,请手动选择")
            {
                MessageBox.Show("存档目录获取失败.如自动检测失败请手动选择");
                return;
            }
            if (CunDir.Text == "请选择用户目录")
            {
                MessageBox.Show("检测目录内没有任何存档请先进入游戏保存一个存档");
                return;
            }
            if (CunName.Text == "读取存档,在此输入名字")
            {
                MessageBox.Show("保存存档前,要先起个名字哈.");
                return;
            }
            if (CunDanji.Checked == true)
            {
                type = "SinglePlayer";
            }
            else if (CunLianji.Checked == true)
            {
                type = "Multiplayer";
            }
            else
            {
                return;
            }
            if (CunWeizhi.Text == "请选择存档位置")
            {
                MessageBox.Show("你得选择一下读取那个挡位");
                return;
            }
            dirS = _FilePath + "\\" + CunDir.Text + "\\" + type + "\\" + CunWeizhi.Text;
            //ConvertDateTimeToInt(time).ToString();
            if (Directory.Exists(dirS))
            {
                if (File.Exists(dirS + "\\__RESUME__"))
                {
                    if (File.Exists(".\\我保存的存档\\" + CunName.Text + ".zip"))
                    {
                        MessageBox.Show("存档名称已经存在,可到若要用这个名字请到存档目录删除即可。");
                        return;
                    }
                    MessageBox.Show("开始进行发布操作,请勿关闭程序");
                    if (ZipYa(dirS, ".\\我保存的存档\\" + CunName.Text + ".zip", out err))
                    {
                        PutFile(UserName.Text, dirS+ "\\thumb.png", ".\\我保存的存档\\" + CunName.Text);
                        MessageBox.Show("发布完成");
                    }
                    else
                    {
                        MessageBox.Show(err);
                    }
                }
                else
                {
                    MessageBox.Show(dirS);
                    MessageBox.Show("你选的存档内好像是空的哦。");
                    return;
                }

            }
            else
            {
                MessageBox.Show("没有找到你选的存档");
                return;
            }

        }

        private void CunWeizhi_SelectedIndexChanged(object sender, EventArgs e)
        {
            string type = "";
            if (CunDir.Text == "请选择用户目录")
            {
                MessageBox.Show("检测目录内没有任何存档请先进入游戏保存一个存档");
                return;
            }
            if (CunDanji.Checked == true)
            {
                type = "SinglePlayer";
            }
            else if (CunLianji.Checked == true)
            {
                type = "Multiplayer";
            }
            string  dirS = _FilePath + "\\" + CunDir.Text + "\\" + type + "\\" + CunWeizhi.Text;
            //ConvertDateTimeToInt(time).ToString();
            if (Directory.Exists(dirS))
            {
                if (File.Exists(dirS + "\\thumb.png"))
                {
                    File.Copy(dirS + "\\thumb.png", ".\\Image\\thumb" + CunWeizhi.Text + ".png", true);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(".\\Image\\thumb" + CunWeizhi.Text + ".png");
                    System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                    img.Dispose();
                    CunYulan.Image = bmp;// System.Drawing.Image.FromFile(".\\Image\\thumb" + CunWeizhi.Text + ".png");
                }

            }
        }
        private void PutFile(string UserName,string Img,string File)
        {
            string url = @"http://theforest.world5.cn/upfile.php";//这里就不暴露我们的地址啦
            string UserNameCode = UserName;
            string updateTime = GetMD5HashFromFile(File+".zip");
            string FileCunImg = "data:image/png;base64,"+ImgToBase64String(Img);
            string sta;
            if (CunFaLei.Text == "共享")
            {
                sta = "1";
            }
            else {
                sta = "2";
            }
            string Leixing = sta;
            string filePath = File+ ".zip";
            string fileName = File;

            byte[] fileContentByte = new byte[1024]; // 文件内容二进制

            

            #region 将文件转成二进制

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fileContentByte = new byte[fs.Length]; // 二进制文件
            fs.Read(fileContentByte, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            #endregion


            #region 定义请求体中的内容 并转成二进制

            string boundary = "ceshi";
            string Enter = "\r\n";

            string modelIdStr = "--" + boundary + Enter
                    + "Content-Disposition: form-data; name=\"UserName\"" + Enter + Enter
                    + UserNameCode + Enter;

            string fileContentStr = "--" + boundary + Enter
                    + "Content-Type:application/octet-stream" + Enter
                    + "Content-Disposition: form-data; name=\"fileContent\"; filename=\"" + fileName + "\"" + Enter + Enter;

            string updateTimeStr = Enter + "--" + boundary + Enter
                    + "Content-Disposition: form-data; name=\"updateTime\"" + Enter + Enter
                    + updateTime;

            string encryptStr = Enter + "--" + boundary + Enter
                    + "Content-Disposition: form-data; name=\"FileCunImg\"" + Enter + Enter
                    + FileCunImg + Enter + "--" + boundary + "--";

            string Cunleixing = Enter + "--" + boundary + Enter
                    + "Content-Disposition: form-data; name=\"status\"" + Enter + Enter
                    + Leixing + Enter + "--" + boundary + "--";

            var modelIdStrByte = Encoding.UTF8.GetBytes(modelIdStr);//modelId所有字符串二进制

            var fileContentStrByte = Encoding.UTF8.GetBytes(fileContentStr);//fileContent一些名称等信息的二进制（不包含文件本身）

            var updateTimeStrByte = Encoding.UTF8.GetBytes(updateTimeStr);//updateTime所有字符串二进制

            var encryptStrByte = Encoding.UTF8.GetBytes(encryptStr);//encrypt所有字符串二进制

            var CunleixingEr = Encoding.UTF8.GetBytes(Cunleixing);//encrypt所有字符串二进制
            #endregion


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "multipart/form-data;boundary=" + boundary;

            Stream myRequestStream = request.GetRequestStream();//定义请求流

            #region 将各个二进制 安顺序写入请求流 modelIdStr -> (fileContentStr + fileContent) -> uodateTimeStr -> encryptStr

            myRequestStream.Write(modelIdStrByte, 0, modelIdStrByte.Length);

            myRequestStream.Write(fileContentStrByte, 0, fileContentStrByte.Length);
            myRequestStream.Write(fileContentByte, 0, fileContentByte.Length);

            myRequestStream.Write(updateTimeStrByte, 0, updateTimeStrByte.Length);

            myRequestStream.Write(encryptStrByte, 0, encryptStrByte.Length);

            myRequestStream.Write(CunleixingEr, 0, CunleixingEr.Length);
            
            #endregion

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();//发送

            Stream myResponseStream = response.GetResponseStream();//获取返回值
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));

            string retString = myStreamReader.ReadToEnd();
            MessageBox.Show(retString);
            myStreamReader.Close();
            myResponseStream.Close();
        }
        //图片 转为    base64编码的文本
        protected string ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(Directory.Exists(".\\下载的存档"))
            {
                System.Diagnostics.Process.Start(".\\下载的存档");
            }
        }

    }
}
