using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; 

namespace QQMusicSkinExtract
{
    public partial class frmQQMusicSkinExtract : Form
    {
        public String FileName;
        public String SavePath; 
        public frmQQMusicSkinExtract()
        {
            InitializeComponent();
        }

        private void btnSavePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                SavePath = fd.SelectedPath;
                txtSavePath.Text = SavePath;               
            }
        }

        private void btnResFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                FileName = fd.FileName;
                txtResFile.Text = FileName;

                txtSavePath.Text = Path.GetDirectoryName(FileName) + "\\" + Path.GetFileNameWithoutExtension(FileName) + "\\";
            }
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            ResExtract re = new ResExtract();
            re.onExtractProgress += new ResExtract.dExtractProgress(ResExtract_onExtractProgress);
            re.clear();
            re.LoadFromFile(txtResFile.Text);
            re.ExtractAll(txtSavePath.Text);
            re.Destroy();

            MessageBox.Show("资源提取完成!");
        }

        private void frmKGSkinExtract_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            txtResFile.Text = path; //将获取到的完整路径赋值到textBox1

            txtSavePath.Text = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "\\";
        }

        private void frmKGSkinExtract_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void btnPackage_Click(object sender, EventArgs e)
        {
            ResPackage rp = new ResPackage(txtSavePath.Text);
            rp.onPackageProgress += new ResPackage.dPackageProgress(ResPackage_onPackageProgress);
            rp.Pack(txtResFile.Text);
            MessageBox.Show("资源打包完成!");
        }

        private void ResPackage_onPackageProgress(long total, long current, String filename)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ResPackage.dPackageProgress(ResPackage_onPackageProgress), new object[] { total, current, filename });
            }
            else
            {
                this.progressBar1.Maximum = (int)total;
                this.progressBar1.Value = (int)current;
                this.log.Text = "正在打包：" + filename;
                //System.Threading.Thread.Sleep(1);  
            }        
        }

        private void ResExtract_onExtractProgress(long total, long current, String filename)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ResExtract.dExtractProgress(ResExtract_onExtractProgress), new object[] { total, current, filename });
            }
            else
            {
                this.progressBar1.Maximum = (int)total;
                this.progressBar1.Value = (int)current;
                this.log.Text = "正在提取：" + filename;
                System.Threading.Thread.Sleep(1);  
            }
        }

    }
}
