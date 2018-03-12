using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 

namespace QQMusicSkinExtract
{
    struct KugouResRec 
    {
        public int Index;
        public int FileSize;
        public String FileName;
        public Int64 Position;
        public Byte[] Other;
    };

    class ResExtract
    {
        public List<KugouResRec> FileList;
        public String FileName;
        public FileStream fs;
        public BinaryReader FMemStream;

        //委托
        public delegate void dExtractProgress(long total, long current, String filename);
        //事件
        public event dExtractProgress onExtractProgress;

        public ResExtract()
        {
            FileList = new List<KugouResRec>();
        }

        public void clear()
        {
            FileList.Clear();
        }

        public void Destroy()
        {
            FMemStream.Close();
            fs.Close();
        }

        public void LoadFromFile(String FileName)
        {
            int         FileCount;
            int         ResOffset;
            int         ResStart;
            KugouResRec Item;
            Byte NameLen;
            Byte[] bName;

            Item = new KugouResRec();
            try
            {
                clear();
                this.FileName = FileName;
                this.fs = File.Open(FileName, FileMode.Open);
                this.FMemStream = new BinaryReader(this.fs);
                //跳过文件头的531E98204F8542F0
                this.FMemStream.BaseStream.Seek(0x10, SeekOrigin.Begin);
                //读取文件数
                FileCount = this.FMemStream.ReadInt32();
                ResOffset = this.FMemStream.ReadInt32();
                this.FMemStream.BaseStream.Seek(0x04, SeekOrigin.Current);
                ResStart  = this.FMemStream.ReadInt32();

                this.FMemStream.BaseStream.Seek(ResOffset, SeekOrigin.Begin);
                for (int i = 0; i < FileCount; i++)
                {
                    Item.Index = i;
                    NameLen = 0;
                    Int64 tmp = this.FMemStream.BaseStream.Position;
                    while (true)
                    {
                        if(this.FMemStream.ReadInt16()!=0){
                            NameLen += 2;
                        }
                        else
                        {
                            break;
                        }
                    }
                    this.FMemStream.BaseStream.Seek(tmp, SeekOrigin.Begin);
                    bName = this.FMemStream.ReadBytes(NameLen);
                    Item.FileName = System.Text.Encoding.Unicode.GetString(bName);
                    this.FMemStream.ReadInt16();
                    Item.Position = this.FMemStream.ReadInt32() + ResStart + ResOffset;
                    Item.Other = this.FMemStream.ReadBytes(4);
                    Item.FileSize = this.FMemStream.ReadInt32();
                    this.FMemStream.ReadInt32();
                    FileList.Add(Item);
                }
            }
            catch (Exception) { }
        }

        public void Extract(int Index, byte[] bd)
        {
            KugouResRec Item;
            if(Index >=0 && Index < FileList.Count)
            {
                Item = FileList[Index];
                //bd = new byte[Item.FileSize];
                FMemStream.BaseStream.Seek(Item.Position, SeekOrigin.Begin);
                FMemStream.Read(bd, 0, Item.FileSize);
            }
        }

        public void Extract(String filename, byte[] bd)
        {
            for (int i = 0; i < FileList.Count;i++)
            {
                if(String.Compare(FileList[i].FileName, filename, true) == 0)
                {
                    Extract(i,bd);
                }
            }
        }

        public void ExtractAll(String fileDir)
        {
            try
            {
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                if (fileDir.LastIndexOf('\\') != fileDir.Length - 1)
                {
                    fileDir += "\\";
                }
                for (int i = 0; i < FileList.Count; i++)
                {
                    byte[] bd = new byte[FileList[i].FileSize];
                    Extract(i, bd);

                    String FileName = fileDir + FileList[i].FileName;

                    String FilePath = Path.GetDirectoryName(FileName);
                    if (!Directory.Exists(FilePath))
                        Directory.CreateDirectory(FilePath);

                    FileStream fss = File.Create(FileName);
                    BinaryWriter bw = new BinaryWriter(fss);
                    bw.Write(bd, 0, FileList[i].FileSize);
                    fss.Close();

                    if (onExtractProgress != null)
                        onExtractProgress(FileList.Count, i, FileName);
                }
            }
            catch(Exception){}
        }
    }
}
