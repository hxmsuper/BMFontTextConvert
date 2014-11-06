using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FontTextConvert
{
    class TextConvert
    {
        public static string GetWord(string start, string end, string str)
        {
            string result = "";
            if (string.Equals(end, ""))
            {
               result= str.Substring(str.IndexOf(start) + start.Count());
            }
            else
            {
                result = str.Substring(str.IndexOf(start) + start.Count(), str.IndexOf(end) - str.IndexOf(start) - start.Count());
            }
            return result;
        }
    }
}
