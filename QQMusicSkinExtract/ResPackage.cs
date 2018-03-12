using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics; 

namespace QQMusicSkinExtract
{
    class ResPackage
    {
        public String Path;
        public ArrayList FileList;
        private DirectoryInfo folder;

        //委托
        public delegate void dPackageProgress(long total, long current, String filename);
        //事件
        public event dPackageProgress onPackageProgress;

        public ResPackage(String Path)
        {
            if ((Path.LastIndexOf('\\') != Path.Length - 1) && (Path.LastIndexOf('/') != Path.Length - 1))
            {
                Path += "\\";
            }
            this.Path = Path;
            this.folder = new DirectoryInfo(Path);
            this.FileList = GetFileNames(folder);
        }

        public void Pack(String FileName)
        {
            FileStream fs;
            BinaryWriter FMemStream;
            String[] fl = (String[])this.FileList.ToArray(typeof(String));
            Int32 Position = 0;

            fs = File.Create(FileName);
            FMemStream = new BinaryWriter(fs);

            String dat = System.Environment.GetEnvironmentVariable("TEMP") + "\\~res.dat";
            FileStream datfile = File.Open(dat, FileMode.Open);
            BinaryWriter datStream = new BinaryWriter(datfile);
            Tools.CmdHelper.ExecuteCmd("echo a 2>\"" + dat + "\"");

            byte[] head = { 0x35, 0x33, 0x31, 0x45, 0x39, 0x38, 0x32, 0x30, 0x34, 0x46, 0x38, 0x35, 0x34, 0x32, 0x46, 0x30 };
            FMemStream.Write(head,0,16);
            FMemStream.Write((Int32)FileList.Count);
            FMemStream.Write((Int32)0x24);
            FMemStream.Write((Int32)0x00);
            FMemStream.Write((Int32)0x00);

            FMemStream.Seek(0x24, SeekOrigin.Begin);



            long current = 0;
            foreach (String file in fl)
            {
                FileStream resfile = File.Open(this.Path + file,FileMode.Open);
                FMemStream.Write(System.Text.Encoding.Unicode.GetBytes(file));
                FMemStream.Write((Int16)0x00);
                FMemStream.Write(Position);
                FMemStream.Write((Int32)0x00);
                FMemStream.Write((Int32)resfile.Length);
                FMemStream.Write((Int32)0x00);
                Position += (Int32)resfile.Length;
                byte[] resdata = new byte[resfile.Length];
                resfile.Read(resdata, 0, (int)resfile.Length);
                datStream.Write(resdata, 0, resdata.Length);

                resfile.Close();                
                
                if (onPackageProgress != null)
                    onPackageProgress(fl.Length, ++current, file);
            }
            Int64 tmp = FMemStream.BaseStream.Position;
            FMemStream.Seek(0x1c, SeekOrigin.Begin);
            FMemStream.Write(fs.Length-0x24);
            FMemStream.Seek((int)tmp, SeekOrigin.Begin);

            datStream.Close();
            datfile.Close();

            datfile = File.Open(dat, FileMode.Open);

            BinaryReader restmp = new BinaryReader(datfile);
            byte[] data = new byte[datfile.Length];
            restmp.Read(data, 0, (int)datfile.Length);
            FMemStream.Write(data, 0, data.Length);
            datfile.Close();
            fs.Close();   
        }

        private ArrayList GetFileNames(DirectoryInfo dirInfo)
        {
            ArrayList fns = new ArrayList();

            foreach (DirectoryInfo subdir in dirInfo.GetDirectories())
            {
                fns.AddRange(GetFileNames(subdir));
            }

            foreach( FileInfo fi in dirInfo.GetFiles())
            {
                fns.Add(fi.FullName.Substring(this.Path.Length));
            }
            return fns;
        }
    }
}
