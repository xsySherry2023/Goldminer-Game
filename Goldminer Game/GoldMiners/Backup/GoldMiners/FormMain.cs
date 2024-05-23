using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading;

namespace GoldMiners
{
    public partial class FormMain : Form
    {
        //钩子的三个状态
        enum HookState
        {
            Shaking=1, Using=2, Back=3
        }

        DateTime OneM = new DateTime();

        #region 私有字段
        //游戏状态
        private GameState _gameState = GameState.Close;
        //生成的静态矿物
        private List<StaticMines> _listMineral = new List<StaticMines>();
        private Bitmap anchor = new Bitmap(16, 16);
        //背景图片
        private Bitmap deskbmp = new Bitmap("Image\\desktop.jpg");
        //当前抓住的矿物
        private int _num = -1;
        private Kindofm _caughtKine;
        //当前玩家得分、以及当前得分
        private int scores = 0;
        private int currentscores = 0;
        #endregion

        #region 钩子部分
        private Point _p1, _p2;
        private Pen pen = new Pen(Color.Black);
        private float xf1 = 497;//钩子两个端点值
        private float xf2 = 517;
        private float yf1 = 121;
        private float yf2 = 121;
        private double r = 20;
        private double angle = 0;
        private double degrees = 0;
        private HookState _hookState = HookState.Shaking;  //初始化钩子摆动状态
        private int endflag = 0;
        #endregion


        public FormMain()
        {
            InitializeComponent();
            //游戏计时器  timer1 连的是label2
            timer1.Tick += new EventHandler(timer1_Tick);
            OneM = DateTime.Parse("00:02:00");
            timer1.Interval = 1000;
            label1.Text = "哥哥，钓我！     哥哥来惹!";

        }

        //随机产生 两个参数，形成一个固定矿物

        //  给定时间内通关（2min）
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (OneM != Convert.ToDateTime("00:00:00"))
            {
                OneM = OneM.AddSeconds(-0.5);
                label2.Text = OneM.Minute.ToString("00") + ":" + OneM.Second.ToString("00");
            }
            else
            {
                
                MediaPlayer1.Ctlcontrols.pause();
                //修改钩子位置
                xf2 = 517;
                yf2 = 121;
                _num = -1;
                timer1.Stop();
                endflag = 1;
                timer1.Enabled = false;
                timer2.Enabled = false;
                timer3.Enabled = false;
                //清空矿物
                _listMineral.Clear();
                _gameState = GameState.Over;
                label4.Text = "游戏结束，您的得分为：" + scores;
                label4.ForeColor = Color.Black;
                //重绘
                pictureBox1.Invalidate();
            }

            //游戏得分
            label1.Text = scores.ToString();
            label1.BackColor = Color.Transparent;
            label1.Parent = pictureBox1;    //父控件，否则开始以后背景是白色的

  
        }



        //paint窗口事件响应
        private void FormMain_Paint(object sender, PaintEventArgs e)
        {

        }

        //开始 菜单响应方法
        private void BeginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OneM = DateTime.Parse("00:01:00");
            _gameState = GameState.Open;
            timer1.Enabled = true;
            timer2.Enabled = true;
            timer3.Enabled = true;
            //音乐播放
            Thread thread = new Thread(new ThreadStart(PlayThread));
            thread.Start();
        }

        //结束 菜单响应方法
        private void EndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _gameState = GameState.Close;
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
        }



        private void timer2_Tick(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                if (_gameState == GameState.Open)
                {
                    //先产生一个矿物
                    StaticMines staticmine1 = new StaticMines(Kindofm.stone);

                    //加入链表中
                    _listMineral.Add(staticmine1);

                    Random kindRand = new Random(DateTime.Now.Second);
                    Random sizeRand = new Random(DateTime.Now.Second);
                    Random x = new Random(DateTime.Now.Second + 1);
                    Random y = new Random(DateTime.Now.Second + 9);

                    int newKind = kindRand.Next(1, 3);
                    int newSize = sizeRand.Next(1, 4);
                    int newx = x.Next(1, 10);
                    int newy = y.Next(1, 6);

                    _listMineral[_listMineral.Count - 1].GenerateMe((Sizeofm)newSize, (int)newx * 130, (int)newy * 50);

                    //再产生一个矿物
                    StaticMines staticmine2 = new StaticMines(Kindofm.diamond);
                    //加入链表中

                    _listMineral.Add(staticmine2);

                    Random kindRand2 = new Random(DateTime.Now.Second);
                    Random sizeRand2 = new Random(DateTime.Now.Second);
                    Random x2 = new Random(DateTime.Now.Second + 7);
                    Random y2 = new Random(DateTime.Now.Second + 32);

                    int newKind2 = kindRand.Next(1, 3);
                    int newSize2 = sizeRand.Next(1, 4);
                    int newx2 = x.Next(1, 10);
                    int newy2 = y.Next(1, 6);

                    _listMineral[_listMineral.Count - 1].GenerateMe((Sizeofm)newSize2, (int)newx2 * 220, (int)newy2 * 100);

                    //强制刷新pictureBox1控件
                    pictureBox1.Invalidate();
                }
            }
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //画钩子
            g.DrawLine(pen, xf1, yf1, xf2, yf2);
            anchor.MakeTransparent(Color.White);
            g.DrawImage(anchor, new Point((int)xf2 - 18, (int)yf2 - 18));

            //画矿物
            for (int i = 0; i <= _listMineral.Count - 1; i++)
            {
                _listMineral[i].DrawMe(g);
            }
            
            if(timer1.Enabled==false)
            {
                Bitmap endpic=new Bitmap("Image\\endpicture.tif");
                g.DrawImage(endpic,0,0);
            }

            if (timer1.Enabled == false && endflag==0)
                label4.Text = "点击游戏菜单，选择开始";
            else if(timer1.Enabled == false && endflag==1)
            {
                label4.Text = "游戏结束，您的得分为："+scores;
                label4.ForeColor=Color.Black;
            }
            else
                label4.Text = "";
            label4.BackColor = Color.Transparent;
            label4.Parent = pictureBox1;    //父控件，否则开始以后背景是白色的
            if (_gameState == GameState.Over)
            {
                SolidBrush bru = new SolidBrush(Color.Black);
                Font font = new Font("宋体", 100, FontStyle.Regular, GraphicsUnit.Pixel);

                if (scores < 650)
                    g.DrawString("一败涂地", font, bru, 240, 417);
                else
                    g.DrawString("恭喜顺利通关", font, bru, 240, 417);
            }
            
        }


        //控制钩子的计时工具
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                if (_gameState == GameState.Open)
                {
                    //画圆周运动的直线，（借鉴百度、作业帮）
                    if (_hookState == HookState.Shaking)
                    {
                        label3.Text="";   //清空label3
                        angle = Math.PI * degrees / 360.0;
                        xf2 = (int)(xf1 + Math.Cos(angle) * r);
                        yf2 = (int)(yf1 + Math.Abs(Math.Sin(angle) * r));
                        degrees += 3;
                    }
                    else if (_hookState == HookState.Using)
                    {
                        //直行
                        MoveGo();
                        for (int i = 0; i <= _listMineral.Count - 1; i++)
                        {
                            //半径
                            int R;
                            //判断种类
                            if (_listMineral[i]._Kind == Kindofm.stone)
                                R = 40;
                            else
                                R = 90;
                            //如果距离小于半径,-10为了钩子吸附性不那么好
                            if (Math.Sqrt(Math.Pow(_listMineral[i]._Position.X + R - xf2, 2) +
                                          Math.Pow(_listMineral[i]._Position.Y + R - yf2, 2)) < R-10)
                            {
                                //把抓到了的矿物记录保存下来
                                _num = i;
                                _caughtKine = _listMineral[i]._Kind;
                                _hookState = HookState.Back;
                                break;
                            }

                        }
                        //超出边界就回来
                        if (xf2 > 980 || yf2 > 520 || xf2 < 0 || yf2 < 0)
                            _hookState = HookState.Back;
                    }
                    //回来
                    else if (_hookState == HookState.Back)
                    {

                        MoveBack();
                        //半径
                        int R;
                        //判断种类
                        if (_caughtKine == Kindofm.stone)
                            R = 40;
                        else
                            R = 90;
                        for (int i = 0; i <= _listMineral.Count - 1; i++)
                        {
                            if (_num == i)
                            {
                                //更改坐标为钩子坐标，抓到的矿物跟着钩子勾上来
                                _listMineral[i]._Position = new Point((int)(xf2 - R), (int)(yf2 - R));
                                break;
                            }
                        }
                        //如果钩子回到了地面上
                        if (Math.Sqrt(Math.Pow(xf1 - xf2, 2) + Math.Pow(yf1 - yf2, 2)) < 4)
                        {
                            //更改钩子状态
                            _hookState = HookState.Shaking;
                            //加分数    设立_num防止上一个为空钩子
                            if (_caughtKine == Kindofm.stone && _num != -1)
                                currentscores = 10;
                            else if (_caughtKine == Kindofm.diamond && _num != -1)
                                currentscores = 100;
                            else
                                currentscores = 0;

                            scores+=currentscores;
                            label3.Text = "$" + currentscores;

                            //如果抓了矿物，清除矿物
                            if (_num != -1)
                                _listMineral.RemoveAt(_num);
                            //修改_num
                            _num = -1;
                        }
                    }

                    pictureBox1.Invalidate();

                }
            }
        }

        //钩子伸长过程
        public void MoveGo( )
        { 
            float changex;
            float changey;
            //按原来的方向移动
            changex = Math.Abs(xf1 - xf2);
            changey = Math.Abs(yf1 - yf2);
            //两线段斜边长，为求单位步长，为下一步做准备
            float changez = (float)Math.Sqrt((double)(changex * changex + changey * changey));

            //移动步长为5个单位
            if(xf2<xf1)
                xf2-=5*changex/changez;
            else
                xf2 += 5 * changex / changez;
            yf2+=5*changey/changez;
        }

        //钩子缩短
        public void MoveBack()
        {
            float changex;
            float changey;
            //按原来的方向移动
            changex = Math.Abs(xf1 - xf2);
            changey = Math.Abs(yf1 - yf2);
            //两线段斜边长，为求单位步长，为下一步做准备
            float changez = (float)Math.Sqrt((double)(changex * changex + changey * changey));

            //回来慢一点，移动步长为2个单位
            if (xf2 < xf1)
                xf2 += 2 * changex / changez;
            else
                xf2 -= 2 * changex / changez;
            yf2 -= 2 * changey / changez;
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (_gameState == GameState.Open)
            {
                //如果钩子在晃动 并按下了回车键
                if (e.KeyCode == Keys.Enter&&_hookState==HookState.Shaking)
                {
                   _hookState = HookState.Using;
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            MediaPlayer1.URL = "songs//Denver, the Last Dinosaur.mp3"; //音乐文件
            MediaPlayer1.Ctlcontrols.stop();
            anchor = new Bitmap("Image\\anchor.tif");
            label2.Text = "";
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripStatusLabel1.Text = e.X + "  " + e.Y;
        }

        private void EndToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //修改钩子位置
            xf2 = 517;
            yf2 = 121;
            _num = -1;
            timer1.Stop();
            endflag = 1;
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
            //清空矿物
            _listMineral.Clear();
            //重绘
            pictureBox1.Invalidate();
            label4.Text = "游戏结束，您的得分为：" + scores;
            label4.ForeColor = Color.Black;
            MediaPlayer1.Ctlcontrols.pause();
        }

        //播放音乐
        private void PlayThread()
        {
            
            MediaPlayer1.Ctlcontrols.play();
        }






 

    }
}
