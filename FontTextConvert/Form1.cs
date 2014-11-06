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
                MessageBox.Show("You should drag file in dialog first!", "Warning", new MessageBoxButtons(), MessageBoxIcon.Warning);
            }
        }

        List<BMFontDataItem> bmFontItemList = new List<BMFontDataItem>();
        protected Dictionary<string, string> itemDic = new Dictionary<string, string>();
        string[] itemNames = new string[]
        {
        "id","x","y","width","height","xoffset","yoffset","xadvance","page","chnl" 
        };
        string[] itemValues = new string[10];
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
                    textBox2.Text = TextConvert.GetWord("face=\"", "\" size", str);
                    int s2 = str.IndexOf("size=") + 5;
                    int e2 = str.IndexOf(" bold");
                    textBox3.Text = TextConvert.GetWord("size=", " bold", str);
                    f1List.Add(string.Format("#fontFace=\"{0}\" #fontSize={1}", textBox2.Text, textBox3.Text));
                }
                else if (str.Contains("common"))
                {
                    lineHeight = TextConvert.GetWord("lineHeight=", " base", str);
                    baseLine = TextConvert.GetWord("base=", "scaleW", str);

                    textureWidth = TextConvert.GetWord("scaleW=", "scaleH", str);
                    textureHeight = TextConvert.GetWord("scaleH=", "pages", str);

                    f1List.Add(string.Format("#lineHeight={0} #baseLine={1} textureWidth={2} #textureHeight={3}", lineHeight, baseLine, textureWidth, textureHeight)); 
                }
                else if (str.Contains("page id=0 file"))
                {
                    textureFile = TextConvert.GetWord("file=", "", str);
                    f1List.Add(string.Format("#textureFile={0}", textureFile));
                }
                else if (str.Contains("chars count"))
                {
                    charCount = TextConvert.GetWord("count=", "", str); 
                    f1List.Add(string.Format("#charsCount={0}", charCount));
                }
                else if (str.Contains("char id"))//找出charid句
                {
                    itemValues = new string[itemNames.Length];
                    itemValues[0] = TextConvert.GetWord("id=", "x=", str);
                    itemValues[1] = TextConvert.GetWord("x=", "y=", str);
                    itemValues[2] = TextConvert.GetWord("y=", "width=", str);
                    itemValues[3] = TextConvert.GetWord("width=", "height=", str);
                    itemValues[4] = TextConvert.GetWord("height=", "xoffset=", str);
                    itemValues[5] = TextConvert.GetWord("xoffset=", "yoffset=", str);
                    itemValues[6] = TextConvert.GetWord("yoffset=", "xadvance=", str);
                    itemValues[7] = TextConvert.GetWord("xadvance=", "page=", str);
                    BMFontDataItem adataItem = new BMFontDataItem(itemValues[0], itemValues[1], itemValues[2], itemValues[3], itemValues[4], itemValues[5], itemValues[6], itemValues[7], itemValues[8], itemValues[9]);
                    bmFontItemList.Add(adataItem);

                }
                else if (str.Contains("kernings"))
                {
                    string first = TextConvert.GetWord("count=", "", str);
                    f3List.Add(string.Format("#kernings count={0}", first));
                }
                else if (str.Contains("kerning f"))
                {
                    string first = TextConvert.GetWord("first=", "second", str); 
                    string second = TextConvert.GetWord("ond=", "amount", str); 
                    string amount = TextConvert.GetWord("amount=", "", str);
                    f3List.Add(string.Format("#kerning #first={0} #second={1} #amount={2}", first, second, amount));
                }

            }

            MessageBox.Show("Convert complete！");

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
            if(textBox1.Text == "")
            {
                MessageBox.Show("You should convert first!", "Warning", new MessageBoxButtons(), MessageBoxIcon.Warning);
                return;
            }
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
                    for (int a = 0; a < f1List.Count; a++)
                    {
                        str += f1List[a] + "\n";
                    }
                    for (int i = 0; i < bmFontItemList.Count; i++)
                    {//+2 -2操作
                        string endx = (float.Parse(bmFontItemList[i].x) / float.Parse(textureWidth) + float.Parse(bmFontItemList[i].width) / float.Parse(textureWidth)).ToString("0.0000");
                        string endy = (float.Parse(bmFontItemList[i].y) / float.Parse(textureHeight) + float.Parse(bmFontItemList[i].height) / float.Parse(textureHeight)).ToString("0.0000");
                        str += string.Format("#charId={0} #uvStart=({1},{2}) #uvEnd=({3},{4}) #size=({5},{6}) #xOffset={7} #yOffset={8} #xAdvance={9}", bmFontItemList[i].id, (float.Parse(bmFontItemList[i].x) / float.Parse(textureWidth)).ToString("0.0000"), (float.Parse(bmFontItemList[i].y) / float.Parse(textureHeight)).ToString("0.0000"), endx, endy, float.Parse(bmFontItemList[i].width).ToString("0.0"), float.Parse(bmFontItemList[i].height).ToString("0.0"), int.Parse(bmFontItemList[i].xoffset) - 2, int.Parse(bmFontItemList[i].yoffset) + 2, bmFontItemList[i].xadvance) + "\n";
                    }
                    for (int b = 0; b < f3List.Count; b++)
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
         
          
    }


}
