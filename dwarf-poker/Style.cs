using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarvenPoker
{
    static class Style
    {
        public enum Align
        {
            left,
            right,
            center
        }
        public enum Direction
        {
            left,
            right,
            top,
            bottom
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

        public static string AddBorder(string text, char border, bool outside = false)
        {
            string result = string.Empty;
            if (outside)
            {
                result = border + text + border;
            }
            else
            {
                text = text.Remove(0, 1);
                text = text.Remove(text.Length - 1);
                result = border + text + border;
            }

            return result;
        }

        public static string[] Table(string[] headers, string[][] content, char border = ' ')
        {
            if (headers.Length != content.Length)
            {
                throw new ArgumentException("Inconsistent collumns");
            }
            foreach (string[] column in content)
            {
                if (content[0].Length != column.Length)
                {
                    throw new ArgumentException("Inconsistent collumns");
                }
            }
            int TableWidth = 0;
            int[] columnWidths = new int[headers.Length];
            for (int i = 0; i < headers.Length; i++)
            {
                columnWidths[i] = headers[i].Length + 6;
                foreach (string line in content[i])
                {
                    if (line.Length > columnWidths[i] - 6)
                    {
                        columnWidths[i] = line.Length + 6;
                    }
                }
            }
            string temp = string.Empty;
            List<string> result = new List<string>();
            foreach (int column in columnWidths)
            {
                TableWidth += column;
            }
            TableWidth += columnWidths.Length * 2;
            TableWidth -= (columnWidths.Length - 1);
            result.Add("┌" + new string('─', TableWidth - 2) + "┐");
            for (int i = 0; i < headers.Length; i++)
            {
                temp += AddBorder(textAlign(2, columnWidths[i], headers[i]), '│', true).Remove(columnWidths[i] + 1);
            }
            temp += "│";
            temp = temp.Remove(0, 1);
            temp = "│" + temp;
            result.Add(temp);
            temp = string.Empty;
            for (int i = 0; i < headers.Length; i++)
            {
                temp += AddBorder(new string('─', columnWidths[i]), '┼', true).Remove(columnWidths[i] + 1);
            }
            temp += "│";
            temp = temp.Remove(0, 1);
            temp = "│" + temp;
            result.Add(temp);
            temp = string.Empty;
            for (int i = 0; i < content[0].Length; i++)
            {
                for (int j = 0; j < content.Length; j++)
                {
                    temp += AddBorder(textAlign(1, columnWidths[j], content[j][i] + " "), '│', true).Remove(columnWidths[j] + 1);
                }
                temp += "│";
                temp = temp.Remove(0, 1);
                temp = "│" + temp;
                result.Add(temp);
                temp = string.Empty;
            }
            result.Add("└" + new string('─', TableWidth - 2) + "┘");
            return result.ToArray();
        }

        public static string[] SectionedArticle(string[][] content, string title = "", char border = '│')
        {
            int width = title.Length;
            foreach (string[] section in content)
            {
                foreach (string line in section)
                {
                    if (line.Length > width)
                    {
                        width = line.Length;
                    }
                }
            }
            width += 2;
            List<string> result = new List<string>();
            result.Add("┌" + new string('─', width - 2) + "┐");
            if (title != "")
            {
                result.Add(Style.AddBorder(Style.textAlign(2, width, title), '│'));
                result.Add(Style.AddBorder(new string('─', width), '│'));
            }
            if (content.Length > 0)
            {
                foreach (string[] section in content)
                {
                    foreach (string line in section)
                    {
                        result.Add(Style.AddBorder(Style.textAlign(0, width, line), '│'));
                    }
                    result.Add(Style.AddBorder(new string('─', width), '│'));
                }
            }
            result[result.Count - 1] = "└" + new string('─', width - 2) + "┘";
            return result.ToArray();
        }

        public static string[] oneLineInfo(string line, char border)
        {
            int width = line.Length + 6;
            string[] result = new string[3];
            result[0] = new string(border, width);
            result[1] = Style.AddBorder(Style.textAlign(2, width - 2, line), '│', true);
            result[2] = new string(border, width);
            return result;
        }

        public static string[] Margin(string[] text, int margin, int direction = 0, char marginChar = ' ')
        {
            List<string> result = new List<string>();
            if (margin < 0)
            {
                throw new ArgumentOutOfRangeException("Argumenen can't be negative");
            }
            switch (direction)
            {
                case 0:
                    foreach (string str in text)
                    {
                        result.Add(str + new string(marginChar, margin));
                    }
                    break;
                case 1:
                    foreach (string str in text)
                    {
                        result.Add(new string(marginChar, margin) + str);
                    }
                    break;
                case 2:
                    for (int i = 0; i < margin; i++)
                    {
                        result.Add(new string(marginChar, text[0].Length));
                    }
                    foreach (string str in text)
                    {
                        result.Add(str);
                    }
                    break;
                case 3:
                    foreach (string str in text)
                    {
                        result.Add(str);
                    }
                    for (int i = 0; i < margin; i++)
                    {
                        result.Add(new string(marginChar, text[0].Length));
                    }
                    break;
            }
            return result.ToArray();
        }

        public static void Format(ref List<string> result, string[] text, int startIndex)
        {
            int maxWidht = 0;
            int endIndex = text.Length + startIndex;
            int toMake = endIndex - result.Count;
            for (int i = 0; i < toMake; i++)
            {
                result.Add(string.Empty);
            }
            for (int i = startIndex; i < text.Length + startIndex; i++)
            {
                if (result[i].Length > maxWidht)
                {
                    maxWidht = result[i].Length;
                }
            }
            for (int i = 0; i < text.Length; i++)
            {
                result[i + startIndex] += new string(' ', maxWidht - result[i + startIndex].Length) + text[i];
            }
        }

        public static void Display(string[] text)
        {
            foreach (string s in text)
            {
                char[] chars = s.ToCharArray();
                bool command = false;
                foreach (char c in chars)
                {
                    if (c == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    if (c == '─' || c == '│' || c == '┘' || c == '└' || c == '┐' || c == '┌' || c == '┼')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    if (c == '<')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        command = true;
                    }
                    if (c == '>')
                    {
                        command = false;
                    }
                    if (c == '$')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                    }
                    if (c == 'ß')
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(" ");
                    }
                    if (c != 'ß')
                    {
                        Console.Write(c);
                        if (!command)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }
                Console.WriteLine(string.Empty);
            }
        }
    }
}
