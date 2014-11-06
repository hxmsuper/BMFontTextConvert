using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FontTextConvert
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
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
