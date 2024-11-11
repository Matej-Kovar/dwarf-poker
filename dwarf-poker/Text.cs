using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dwarf_poker
{
    static class Text
    {
        public enum Align
        {
            left, 
            right, 
            center
        }
        public static string textAlign(int align, int width, string text)
        {
            string result = string.Empty;
            if (width > text.Length)
            {
                switch (align)
                {
                    case 0:
                        result = text + new string(' ', width - text.Length);
                        break;
                    case 1:
                        result = new string(' ', width - text.Length) + text;
                        break;
                    case 2:
                        result = new string(' ', (width - text.Length) / 2) + text + new string(' ', width - text.Length - (width - text.Length) / 2);
                        break;
                }
            }
            return result;
        }

        public static string AddBorder(string text, char border, bool outside = false, int? width = null)
        {
            string result = string.Empty;
            if (width == null || width < 0)
            {
                width = text.Length;
            }
            
            if (outside)
            {
                string offset = new string(' ', (int)width - text.Length - 2);
                result = border + text + offset + border;
            } 
            else
            {
                string offset = new string(' ', (int)width - text.Length);
                text = text.Remove(0, 1);
                text = text.Remove(text.Length - 1);
                result = border + text + offset +  border;
            }
            
            return result;
        }
    }
}
