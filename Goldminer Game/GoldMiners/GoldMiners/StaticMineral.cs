using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GoldMiners
{
    // enumerate the game states
    public enum GameState
    {
        Close = 1, Open = 2, Pause = 3, Over = 4
    }

    // state the size and type of mineral
    public enum Kindofm
    {
        stone=1, diamond=2
    }
    public enum Sizeofm
    {
        verylittle = 1, little = 2, large = 3, verylarge=4
    }


    class StaticMineral
    {
        #region Private Fields
        // Mineral coordinate position
        private Point _position = new Point(200, 200);
        // Mineral size
        private Sizeofm _size = Sizeofm.verylittle; // Mineral size ranges from 1 to 4
        // Mineral type
        private Kindofm _kind = Kindofm.stone; // 1 indicates stone, 2 indicates diamond
        
        // storage different sized minera; 
        private Bitmap[] _mineralBmp = new Bitmap[4];
        private Bitmap _staticMineBmp = new Bitmap(16, 16);
        #endregion


        #region Pubilc Fields
        // Mineral coordinate position
        public Point _Position 
        {
          get{return _position;}
          set{_position=value;}
        }
        //mineral size
        public Sizeofm _Size
        {
            get { return _size; }
            set { _size= value; }
        }
        //mineral type
        public Kindofm _Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }
        #endregion


        // Class constructor
        public StaticMineral(int kindInt, int sizeInt)
        {
            //  Record the properties of mineral, to prepare for calculate the specific scores.
            _kind = (Kindofm)kindInt;
            _size = (Sizeofm)sizeInt;

            //  Load the corresponding image
            if (_kind == Kindofm.stone)
            {
                _mineralBmp[0] = new Bitmap("Image\\verylittlestone.gif");
                _mineralBmp[1] = new Bitmap("Image\\littlestone.gif");
                _mineralBmp[2] = new Bitmap("Image\\largestone.gif");
                _mineralBmp[3] = new Bitmap("Image\\verylargestone.gif");
            }
            else
            {
                _mineralBmp[0] = new Bitmap("Image\\verylittlediamand.gif");
                _mineralBmp[1] = new Bitmap("Image\\littlediamand.gif");
                _mineralBmp[2] = new Bitmap("Image\\largediamand.gif");
                _mineralBmp[3] = new Bitmap("Image\\verylargediamand.gif");
            }

            //Set mineral bitmap to transparent color
            for (int i = 0; i < 4; i++)
                _mineralBmp[i].MakeTransparent(Color.Black);
            // The current default is the first picture
            _staticMineBmp = _mineralBmp[0];
        }


        //generate 
        public void GenerateMe(Sizeofm size,int x,int y)
        {
            
            _size = size;
            _position.X = x;
            _position.Y = y;

            
            if (_size == Sizeofm.verylarge)
                _staticMineBmp = _mineralBmp[3];
            else if (_size == Sizeofm.large)
                _staticMineBmp = _mineralBmp[2];
            else if (_size == Sizeofm.little)
                _staticMineBmp = _mineralBmp[1];
            else if (_size == Sizeofm.verylittle)
                _staticMineBmp = _mineralBmp[0];
        }

        //method: draw a mineral
        public void DrawMe(Graphics g)
        {
            g.DrawImage(_staticMineBmp, _position);
        }


    }
}
