using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GoldMiners
{
    //枚举游戏状态
    public enum GameState
    {
        Close = 1, Open = 2, Pause = 3,Over=4
    }
    public enum Kindofm
    {
        stone=1, diamond=2
    }
    public enum Sizeofm
    {
        verylittle = 1, little = 2, large=3, verylarge=4
    }


    class StaticMines
    {
        #region 私有字段
        //矿物坐标位置
        private Point _position = new Point(200, 200);
        //矿物大小
        private Sizeofm  _size = Sizeofm.verylittle; // 矿物大小从1—4不等
        //矿物种类
        private Kindofm  _kind =Kindofm.stone; //0表示石头，1表示钻石

        //矿物各大小图片数组
        private Bitmap[] _mineralBmp = new Bitmap[4];
        private Bitmap _staticMineBmp = new Bitmap(16, 16);
        #endregion
        #region 公有属性
        //矿物坐标位置
        public Point _Position 
        {
          get{return _position;}
          set{_position=value;}
        }
        //矿物大小
        public Sizeofm _Size
        {
            get { return _size; }
            set { _size= value; }
        }
        //矿物种类
        public Kindofm _Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }
        #endregion

        //类构造方法
        public StaticMines(Kindofm  kind)
        {
            _kind = kind;
            // 装载响应的图片
            if (kind == Kindofm.stone)
            {
                _mineralBmp[0] = new Bitmap("Image\\verylittlestone.gif");
                _mineralBmp[1] = new Bitmap("Image\\littlestone.gif");
                _mineralBmp[2] = new Bitmap("Image\\verylargestone.gif");
                _mineralBmp[3] = new Bitmap("Image\\largestone.gif");
            }
            else
            {
                _mineralBmp[0] = new Bitmap("Image\\verylittlediamand.gif");
                _mineralBmp[1] = new Bitmap("Image\\littlediamand.gif");
                _mineralBmp[2] = new Bitmap("Image\\verylargediamand.gif");
                _mineralBmp[3] = new Bitmap("Image\\largediamand.gif");
            }
                
            //设置矿物位图的透明色
            for(int i=0;i<4;i++)
                _mineralBmp[i].MakeTransparent(Color.Black);
            //当前为第一张图
            _staticMineBmp = _mineralBmp[0];
        }

        //生成矿物
        public void GenerateMe(Sizeofm size,int x,int y)
        {
            _size = size;
            _position.X = x;
            _position.Y = y;

            if (_size == Sizeofm.verylarge)
                _staticMineBmp = _mineralBmp[0];
            else if (_size == Sizeofm.large)
                _staticMineBmp = _mineralBmp[1];
            else if (_size == Sizeofm.little)
                _staticMineBmp = _mineralBmp[2];
            else if (_size == Sizeofm.verylittle)
                _staticMineBmp = _mineralBmp[3];
        }

        //（方法）矿物的绘制
        public void DrawMe(Graphics g)
        {
            g.DrawImage(_staticMineBmp, _position);
        }


    }
}
