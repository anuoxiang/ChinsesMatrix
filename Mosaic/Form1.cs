using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mosaic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Drawing.Text.InstalledFontCollection fonts = new System.Drawing.Text.InstalledFontCollection();
            foreach (System.Drawing.FontFamily family in fonts.Families)
            {
                comboBox1.Items.Add(family.Name);
            }
            comboBox1.Text = "微软雅黑 Light";
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            Bitmap big = new Bitmap(128, 128);
            var rst = GetDot(textBox1.Text,
                (int)numericUpDown1.Value,
                (int)numericUpDown2.Value,
                comboBox1.Text,
                (int)numericUpDown3.Value, out big);
            textBox2.Text = string.Join("\r\n", rst.Select(r => r.maping).ToArray());

            rst.ForEach(r => textBox3.Text += r.Make() + "\r\n");
            pictureBox1.Image = big;
        }

        private List<MatrixWord> GetDot(String txt, int Width, int Height, String FontName, int FontSize, out Bitmap pic)
        {
            List<MatrixWord> words = new List<MatrixWord>();
            char[] a = txt.ToCharArray();
            string Str = "";
            Bitmap big = new Bitmap(128, 128);
            int line = 0;
            int colChar = 100;
            for (int i = 0; i < a.Length; i++)
            {
                var word = new MatrixWord(a[i], Width, Height, FontName, FontSize);
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                        big.SetPixel(x + (i % colChar) * Width, y + (int)(i / colChar) * Height, word.square.GetPixel(x, y));
                if ((i + 1) % colChar * Width >= big.Width)
                    colChar = colChar > (i + 1) ? (i + 1) : colChar;
                words.Add(word);
            }
            pic = big;
            return words;
        }

    }

    public class MatrixWord
    {
        private int width, height, fontsize;
        private string fontname;
        private char _c;

        public MatrixWord()
        {
        }

        public MatrixWord(char c, int Width, int Height, String FontName, int FontSize)
        {
            width = Width;
            height = Height;
            fontsize = FontSize;
            fontname = FontName;
            this._c = c;
            var _t = Convert();
            this.word = c;
            this.maping = _t.maping;
            this.square = _t.square;
            this.hexArr = _t.hexArr;
        }

        public char word { get; set; }
        public Bitmap square { get; set; }
        public String maping { get; set; }
        public List<Int32> hexArr { get; set; }

        public String Make()
        {
            String command = "const uint8_t bitmap[] PROGMEM  = {";
            List<string> arr = new List<string>();
            foreach (int i in hexArr)
                arr.Add(String.Format("0x{0:X}", i));
            command += string.Join(",", arr) + "};";
            return command;
        }

        public MatrixWord Convert()
        {
            return Convert(_c, width, height, fontname, fontsize);
        }

        public MatrixWord Convert(char c)
        {
            return Convert(c, width, height, fontname, fontsize);
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="c"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="FontName"></param>
        /// <param name="FontSize"></param>
        /// <returns></returns>
        public static MatrixWord Convert(char c, int Width, int Height, String FontName, int FontSize)
        {
            Bitmap b = new Bitmap(Width, Height);
            RectangleF rectf = new RectangleF(0, 0, Width, Height);
            Graphics g = Graphics.FromImage(b);

            //g.SmoothingMode = SmoothingMode.AntiAlias;
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(c.ToString(), new Font(FontName, FontSize), Brushes.Black, rectf);
            g.Flush();
            //pictureBox1.Image = new Bitmap(b, 128, 128);
            string DotMartix = "";
            List<Int32> hexList = new List<int>();
            for (int y = 0; y < b.Height; y++)
            {
                string Line = "";
                int hex = 0;
                for (int x = 0; x < b.Width; x++)
                {
                    if (b.GetPixel(x, y).Name != "0")
                    {
                        Line += "● ";
                        hex += 1 << (7 - x % 8);
                    }
                    else
                    {
                        Line += "○ ";
                    }
                    //8个bit分割
                    if ((x + 1) % 8 == 0)
                    {
                        hexList.Add(hex);
                        hex = 0;
                    }
                }

                DotMartix += Line + "\r\n";
            }
            MatrixWord word = new MatrixWord
                {
                    word = c,
                    square = b,
                    maping = DotMartix,
                    hexArr = hexList
                };
                return word;
            }
        }

    }
