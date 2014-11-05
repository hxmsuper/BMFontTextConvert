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
namespace FontTextConvert
{
    
    public partial class Form1 : Form
    { 
        StreamReader reader;
        List<string> f1List = new List<string>();
        List<string> readerList = new List<string>();
        List<string> f3List = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                string filePath = textBox1.Text;
                reader = new StreamReader(filePath);
                string line = reader.ReadLine();
                readerList.Clear();

                while (line != null)
                {
                    readerList.Add(line);
                    line = reader.ReadLine();//不断读取直到结束
                }
                reader.Close();
                DoConvert();

            }
            else
            {
                MessageBox.Show("No file !");
            }
        }

        List<BMFontDataItem> bmFontItemList = new List<BMFontDataItem>();
        protected Dictionary<string, string> itemDic = new Dictionary<string, string>(); 
        string[] itemNames = new string[]
        {
        "id","x","y","width","height","xoffset","yoffset","xadvance","page","chnl" 
        };
        string[] itemValues = new string[10] ;
        string lineHeight;
        string baseLine;
        string textureWidth;
        string textureHeight;
        string textureFile;
        string charCount;
        private void DoConvert()
        {
            f1List.Clear();
            f3List.Clear();
            for (int i = 0; i < readerList.Count; i++)//遍历每一
            {
                string str = readerList[i];
                if (str.Contains("info"))
                {
                    int s1 = str.IndexOf("face=\"") + 6;
                    int e1 = str.IndexOf("\" size");
                    textBox2.Text = str.Substring(s1, e1 - s1);
                    int s2 = str.IndexOf("size=") + 5;
                    int e2 = str.IndexOf(" bold");
                    textBox3.Text = str.Substring(s2, e2 - s2);
                    f1List.Add(string.Format("#fontFace=\"{0}\" #fontSize={1}",textBox2.Text,textBox3.Text));
                }
                else if(readerList[i].Contains("common"))
                {
                    
                    int s1 = str.IndexOf("lineHeight=") + 11;
                    int e1 = str.IndexOf(" base");
                    lineHeight = str.Substring(s1, e1 - s1);
                    int s2 = str.IndexOf("base=") + 5;
                    int e2 = str.IndexOf(" scaleW");
                    baseLine = str.Substring(s2, e2 - s2);
                    textureWidth = str.Substring(str.IndexOf("scaleW=") + 7, str.IndexOf(" scaleH") - str.IndexOf("scaleW=") - 7);
                    textureHeight = str.Substring(str.IndexOf("scaleH=") + 7, str.IndexOf(" pages") - str.IndexOf("scaleH=") - 7);
                    f1List.Add(string.Format("#lineHeight={0} #baseLine={1} textureWidth={2} #textureHeight={3}", lineHeight, baseLine,textureWidth,textureHeight));
                    
                }
                else if (readerList[i].Contains("page id=0 file"))
                {
                    textureFile = str.Substring(str.IndexOf("file=") + 5);
                    f1List.Add(string.Format("#textureFile={0}",textureFile));
                }
                else if(readerList[i].Contains("chars count"))
                {
                    charCount = str.Substring(str.IndexOf("count=") + 6 );
                    f1List.Add(string.Format("#charsCount={0}", charCount));
                }
                else if (readerList[i].Contains("char id"))//找出charid句
                {
                    string str2 = readerList[i].Substring(readerList[i].IndexOf("id"));
                    
                    string[] items = str2.Split(new char[]{' '});  //切分每一句
                    itemValues = new string[itemNames.Length];
                    itemDic.Clear();//清理临时字典 
                    for (int m = 0; m < items.Length; m++)//析出各个元素
                    {
                        string currentStr = items[m];
                        for (int n = 0;n<itemNames.Length;n++)
                        {
                            string aname = itemNames[n];
                            if (currentStr.Contains(aname))
                            { 
                              string result = currentStr.Substring(currentStr.IndexOf("=")+1);
                              itemValues[m] = result;
                               break;
                            }
                        }  
                    }
                    

                    BMFontDataItem adataItem = new BMFontDataItem(itemValues[0], itemValues[1], itemValues[2], itemValues[3], itemValues[4], itemValues[5], itemValues[6], itemValues[7], itemValues[8], itemValues[9]);
                    bmFontItemList.Add(adataItem);

                }
                else if(str.Contains("kernings"))
                {
                    string first = str.Substring(str.IndexOf("count=") + 6); 
                    f3List.Add(string.Format("#kernings count={0}", first));
                }
                else if(str.Contains("kerning f"))
                {
                    string first = str.Substring(str.IndexOf("first=") + 6, str.IndexOf(" second") - str.IndexOf("first=") - 6);
                    string second = str.Substring(str.IndexOf("ond=") +4, str.IndexOf(" amount") - str.IndexOf("ond=") - 4);
                    string amount = str.Substring(str.IndexOf("amount=") + 7);
                    f3List.Add(string.Format("#kerning #first={0} #second={1} #amount={2}", first, second, amount));
                }

            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            string filename = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (filename.Contains("txt") == true)
            {
                this.textBox1.Text = filename;

            }
            else
            {
                MessageBox.Show("不支持当前拖拽的文件格式，请拖拽文本文件！");
            }
        }
        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All; 
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {

            //保存
            saveFileDialog1.Filter = "文本文档(*.txt)|*.txt|Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.CreatePrompt = true;
                Stream fileStream = saveFileDialog1.OpenFile();
                StreamWriter fileWriter = new StreamWriter(fileStream);
                string str = "";
                try
                {
                    for (int a = 0; a < f1List.Count;a++ )
                    {
                        str += f1List[a]+"\n";
                    }
                    for (int i = 0; i < bmFontItemList.Count; i++)
                    {//+2 -2操作
                        string endx = (float.Parse(bmFontItemList[i].x) / float.Parse(textureWidth)+float.Parse(bmFontItemList[i].width)/float.Parse(textureWidth)).ToString("0.0000");
                        string endy =  (float.Parse(bmFontItemList[i].y) / float.Parse(textureHeight)+float.Parse(bmFontItemList[i].height)/float.Parse(textureHeight)).ToString("0.0000");
                        str += string.Format("#charId={0} #uvStart=({1},{2}) #uvEnd=({3},{4}) #size=({5},{6}) #xOffset={7} #yOffset={8} #xAdvance={9}", bmFontItemList[i].id, (float.Parse(bmFontItemList[i].x) / float.Parse(textureWidth)).ToString("0.0000"), (float.Parse(bmFontItemList[i].y) / float.Parse(textureHeight)).ToString("0.0000"), endx,endy, float.Parse(bmFontItemList[i].width).ToString("0.0"), float.Parse(bmFontItemList[i].height).ToString("0.0"), int.Parse(bmFontItemList[i].xoffset)-2, int.Parse(bmFontItemList[i].yoffset)+2, bmFontItemList[i].xadvance) + "\n";
                    }
                    for (int b = 0; b < f3List.Count;b++ )
                    {
                        str += f3List[b] + "\n";
                    }
                        fileWriter.Write(str);
                    fileWriter.Dispose();
                    fileStream.Dispose();

                }
                catch (Exception f)
                {
                    MessageBox.Show(f.Message);
                }
                finally
                {
                    fileWriter.Dispose();
                    fileStream.Dispose();
                }


            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

    }

    class BMFontDataItem
    {
        public string id;
        public string x;
        public string y;
        public string width;
        public string height;
        public string xoffset;
        public string yoffset;
        public string xadvance;
        public string page;
        public string chnl;
        public BMFontDataItem(string id1, string x1, string y1, string width1, string height1, string xoffset1, string yoffset1, string xadvance1, string page1, string chnl1)
        {
            id = id1;
            x = x1;
            y = y1;
            width = width1;
            height = height1;
            xoffset = xoffset1;
            yoffset = yoffset1;
            xadvance = xadvance1;
            page = page1;
            chnl = chnl1;
        }
    }
}
