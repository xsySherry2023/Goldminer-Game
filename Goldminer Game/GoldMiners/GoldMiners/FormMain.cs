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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Runtime.CompilerServices;

namespace GoldMiners
{
    public partial class FormMain : Form
    {
        // define the three states of the hook
        enum HookState
        {
            Shaking=1, Using=2, Back=3
        }

        DateTime OneM = new DateTime();

        #region Private field
        // Game state
        private GameState _gameState = GameState.Close;
        // Generated static minerals
        private List<StaticMineral> _listMineral = new List<StaticMineral>();
        private Bitmap anchor = new Bitmap(16, 16);
        // Background image
        private Bitmap deskbmp = new Bitmap("Image\\desktop.jpg");
        // the properity of current mineral being captured
        private int _num = -1;
        private Kindofm _caughtKind;
        private Sizeofm _caughtSize;

        // Current player score, and current score
        private int scores = 0;
        private int currentscores = 0;
        #endregion

        #region the hook part
        private Point _p1, _p2;
        private Pen pen = new Pen(Color.Black);
        //initial two point  for the hook
        private float xf1 = 497;
        private float xf2 = 517;
        private float yf1 = 121;
        private float yf2 = 121;
        private double r = 20;
        private double angle = 0;
        private double degrees = 0;
        private HookState _hookState = HookState.Shaking;  //initialize the swing state of the hook
        private int endflag = 0;
        #endregion


        public FormMain()
        {
            InitializeComponent();
            //the timer of global game: timer1, which is connected to label2
            timer1.Tick += new EventHandler(timer1_Tick);
            OneM = DateTime.Parse("00:02:00");
            timer1.Interval = 1000;
            label1.Text = "GoldMiner game, let's start!";

        }


        // timer1: the timer for the global game,demanding pass level1 within a given time (2min)
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
                // Modify the hook to vertical position (default)
                xf2 = 517;
                yf2 = 121;
                _num = -1;
                timer1.Stop();
                endflag = 1;
                timer1.Enabled = false;
                timer2.Enabled = false;
                timer3.Enabled = false;
                // empty the mineral list
                _listMineral.Clear();
                _gameState = GameState.Over;
                label4.Text = "The game is over and your score is:" + scores;
                label4.ForeColor = Color.Black;
                // refresh
                pictureBox1.Invalidate();
            }

            //game score
            label1.Text = scores.ToString();
            label1.BackColor = Color.Transparent;
            //Parent control, otherwise the background will be white after starting
            label1.Parent = pictureBox1;    

        }



        // response method to paint window events
        private void FormMain_Paint(object sender, PaintEventArgs e)
        {

        }

        // response method for start menu 
        private void BeginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // set the limited time
            OneM = DateTime.Parse("00:01:30");
            _gameState = GameState.Open;
            timer1.Enabled = true;
            timer2.Enabled = true;
            timer3.Enabled = true;
            // play background music
            Thread thread = new Thread(new ThreadStart(PlayThread));
            thread.Start();
        }

        // response method to end menu
        private void EndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _gameState = GameState.Close;
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
        }


        // timer2: design to generate mineral function
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                if (_gameState == GameState.Open)
                {
                    // generate one mineral every 2 seconds.
                    Random rand1 = new Random(DateTime.Now.Second);
                    int newKind2 = rand1.Next(1, 3);
                    int newSize2 = rand1.Next(1, 5);
                    StaticMineral staticmine = new StaticMineral(newKind2, newSize2);
                    // add the new obejctive to the lists
                    _listMineral.Add(staticmine);

                    // add different seeds each time, ensuring that the output position is random
                    Random rand2 = new Random(DateTime.Now.Second + rand1.Next(0,60));
                    double newx2 = rand2.Next(0, 40) * 0.25;
                    double newy2 = rand2.Next(18, 35) * 0.25;

                    _listMineral[_listMineral.Count - 1].GenerateMe((Sizeofm)newSize2, (int)(newx2 * 90), (int)(newy2 * 50));

                    // forcefully refresh the pictureBox1 control
                    pictureBox1.Invalidate();
                }
            }
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // draw hook
            g.DrawLine(pen, xf1, yf1, xf2, yf2);
            anchor.MakeTransparent(Color.White);
            g.DrawImage(anchor, new Point((int)xf2 - 18, (int)yf2 - 18));

            // drawing minerals
            for (int i = 0; i <= _listMineral.Count - 1; i++)
            {
                _listMineral[i].DrawMe(g);
            }
            
            if(timer1.Enabled==false)
            {
                Bitmap endpic=new Bitmap("Image\\endpicture.tif");
                g.DrawImage(endpic,0,0);
            }
            // Wrap long text
            if (timer1.Enabled == false && endflag==0)
                label4.Text = "Click on the game menu\nand select Start Game!";
            else if(timer1.Enabled == false && endflag==1)
            {
                label4.Text = "Time out!\nand your score is: " + scores;
                label4.ForeColor=Color.Black;
            }
            else
                label4.Text = "";
            label4.BackColor = Color.Transparent;
            label4.Parent = pictureBox1;    //set a parent control, otherwise the background will be white after starting
            if (_gameState == GameState.Over)
            {
                SolidBrush bru = new SolidBrush(Color.Black);
                Font font = new Font("Arial", 50, FontStyle.Regular, GraphicsUnit.Pixel);

                if (scores < 650)
                    g.DrawString("Level Failed!", font, bru, 240, 417);
                else
                    g.DrawString("Level Succeeded!", font, bru, 240, 417);
            }


        }


        // timer3: design to control the hook
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                if (_gameState == GameState.Open)
                {
                    // dram a straight line running in circular path
                    if (_hookState == HookState.Shaking)
                    {
                        label3.Text="";   //clear label3
                        angle = Math.PI * degrees / 360.0;
                        xf2 = (int)(xf1 + Math.Cos(angle) * r);
                        yf2 = (int)(yf1 + Math.Abs(Math.Sin(angle) * r));
                        degrees += 3;
                    }
                    else if (_hookState == HookState.Using)
                    {
                        //go straight
                        MoveGo();
                        for (int i = 0; i <= _listMineral.Count - 1; i++)
                        {
                            //radius for different type
                            int R;
                            //radius for different sizes
                            int multipleForSize = (int)_listMineral[i]._Size;
                            // judge the type
                            if (_listMineral[i]._Kind == Kindofm.stone)
                                R = 90 * (multipleForSize+2)/6 ;
                            else
                                R = 80 * (multipleForSize + 2) / 6;
                            // if the distance is smaller than the radius,
                                // set -10 will avoid the hook to adsorb so well.
                            if (Math.Sqrt(Math.Pow(_listMineral[i]._Position.X + R - xf2, 2) +
                                          Math.Pow(_listMineral[i]._Position.Y + R - yf2, 2)) < R-10)
                            {
                                // record  the minerals being caught
                                _num = i;
                                _caughtKind = _listMineral[i]._Kind;
                                _caughtSize = _listMineral[i]._Size;
                                _hookState = HookState.Back;
                                break;
                            }
                            
                        }
                        // the hook will come back when it exceeds the boundary
                        if (xf2 > 980 || yf2 > 520 || xf2 < 0 || yf2 < 0)
                            _hookState = HookState.Back;
                    }
                    // come back
                    else if (_hookState == HookState.Back)
                    {

                        MoveBack();
                        int R;
                        //judge the type
                        if (_caughtKind == Kindofm.stone)
                            R = 40 * (int)_caughtSize / 4;
                        else
                            R = 90 * (int)_caughtSize / 4;
                        for (int i = 0; i <= _listMineral.Count - 1; i++)
                        {
                            if (_num == i)
                            {
                                // change the coordinates to hook coordinates, and the captured minerals will be hooked up with the hook.
                                _listMineral[i]._Position = new Point((int)(xf2 - R), (int)(yf2 - R));
                                break;
                            }
                        }
                        // if the hook comes back to the ground
                        if (Math.Sqrt(Math.Pow(xf1 - xf2, 2) + Math.Pow(yf1 - yf2, 2)) < 4)
                        {
                            // change hook status
                            _hookState = HookState.Shaking;
                            // add scores
                            // set up _num to prevent the previous empty hook
                            if (_caughtKind == Kindofm.stone && _num != -1)
                                currentscores = 40 * (int)_caughtSize/4;
                            else if (_caughtKind == Kindofm.diamond && _num != -1)
                                currentscores = 100 * (int)_caughtSize/4;
                            else
                                currentscores = 0;

                            scores+=currentscores;
                            label3.Text = "$" + currentscores;

                            // if a mineral is caught, clear the mineral
                            if (_num != -1)
                                _listMineral.RemoveAt(_num);
                            // change _num to original statement
                            _num = -1;
                        }
                    }

                    pictureBox1.Invalidate();

                }
            }
        }

        // the process of hook moving forward
        public void MoveGo( )
        { 
            float changex;
            float changey;
            // move in original direction
            changex = Math.Abs(xf1 - xf2);
            changey = Math.Abs(yf1 - yf2);
            // calculate the the third line(in the triangle) hypotenuse. preparing for the next step
            float changez = (float)Math.Sqrt((double)(changex * changex + changey * changey));

            //set movement forward step size is 5 units
            if (xf2<xf1)
                xf2 -= 5 * changex/changez;
            else
                xf2 += 5 * changex / changez;
            yf2+=5*changey/changez;
        }

        // the process of hook moving backward
        public void MoveBack()
        {
            float changex;
            float changey;
            // move in original direction
            changex = Math.Abs(xf1 - xf2);
            changey = Math.Abs(yf1 - yf2);
            // calculate the the third line(in the triangle) hypotenuse. preparing for the next step
            float changez = (float)Math.Sqrt((double)(changex * changex + changey * changey));

            //set movement backward step size is 2 units, slower compared to forward movement
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
                //If the hook is shaking and "Enter" is pressed
                if (e.KeyCode == Keys.Enter&&_hookState==HookState.Shaking)
                {
                   _hookState = HookState.Using;
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            MediaPlayer1.URL = "songs//Denver, the Last Dinosaur.mp3"; //bakcground music files
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
            //change hook position
            xf2 = 517;
            yf2 = 121;
            _num = -1;
            timer1.Stop();
            endflag = 1;
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
            // empty the mineral list
            _listMineral.Clear();
            // refresh
            pictureBox1.Invalidate();
            label4.Text = "The game is over\nYour score is: " + scores;
            label4.ForeColor = Color.Black;
            MediaPlayer1.Ctlcontrols.pause();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
            }
            else {
                timer1.Start();
            }
                
        }

        //play the bakcground music
        private void PlayThread()
        {
            
            MediaPlayer1.Ctlcontrols.play();
        }

    }
}
