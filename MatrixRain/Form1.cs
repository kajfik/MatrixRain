using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MatrixRain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static Bitmap bmp;
        static Graphics g;
        static int width, height;
        static Random rnd = new Random();
        static long frameCount = 0;
        static int symbolSize = 10;
        static float fadeInterval = 1.6F;
        static Font font = new Font(FontFamily.GenericMonospace, symbolSize, FontStyle.Bold);

        List<Stream> streams = new List<Stream>();

        class Symbol
        {
            public int x;
            public int y;
            public int speed;
            public int switchInterval;
            public char symbol;
            public int opacity;
            public bool first;

            public Symbol(int x, int y, int speed, bool first, int opacity)
            {
                this.x = x;
                this.y = y;
                this.speed = speed;
                this.first = first;
                this.opacity = opacity;
                switchInterval = rnd.Next(2, 26);
            }

            public void setToRandomSymbol()
            {
                if (frameCount % switchInterval == 0)
                {
                    int charType = rnd.Next(6);
                    if (charType > 1)
                    {
                        // set it to Katakana
                        symbol = Convert.ToChar(0x30A0 + rnd.Next(0, 97));
                    }
                    else
                    {
                        // set it to numeric
                        symbol = Convert.ToChar('0' + rnd.Next(0, 10));
                    }
                }
            }
            public void rain()
            {
                if(y >= height)
                {
                    y = 0;
                }
                else
                {
                    y += speed;
                }
            }
        }

        class Stream
        {
            List<Symbol> symbols = new List<Symbol>();
            int totalSymbols = rnd.Next(5, 36);
            int speed = rnd.Next(5, 25);

            public void generateSymbols(int x, int y)
            {
                int opacity = 255;
                bool first = (rnd.Next(0, 5) == 1);
                for (var i = 0; i <= totalSymbols; i++)
                {
                    Symbol symbol = new Symbol(x, y, speed, first, opacity);
                    symbol.setToRandomSymbol();
                    symbols.Add(symbol);
                    opacity -= Convert.ToInt32((255 / totalSymbols) / fadeInterval);
                    y -= symbolSize;
                    first = false;
                }
            }

            public void render()
            {
                foreach (Symbol s in symbols)
                {
                    if (s.first)
                    {
                        g.DrawString(s.symbol.ToString(), font, new SolidBrush(Color.FromArgb(s.opacity, 200, 255, 150)), s.x, s.y);
                    }
                    else
                    {
                        g.DrawString(s.symbol.ToString(), font, new SolidBrush(Color.FromArgb(s.opacity, 0, 255, 70)), s.x, s.y);
                    }
                    s.rain();
                    s.setToRandomSymbol();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(175, Color.Black)), 0, 0, width, height);
            foreach (Stream stream in streams)
            {
                stream.render();
            }
            frameCount++;
            pictureBox1.Image = bmp;
            pictureBox1.Update();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Width = 600;// Form1.ActiveForm.Width - 16;
            pictureBox1.Height = 400;// Form1.ActiveForm.Height - 39;
            width = pictureBox1.Width;
            height = pictureBox1.Height;
            bmp = new Bitmap(width, height);
            g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            g.Clear(Color.Black);
            pictureBox1.Image = bmp;

            int x = 0;
            for (var i = 0; x < width - symbolSize; i++)
            {
                var stream = new Stream();
                stream.generateSymbols(x, rnd.Next(-2000, 1));
                streams.Add(stream);
                x += symbolSize;
            }
        }
    }
}
