using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FibonacciGame
{
    struct TileAnimation
    {
        internal const int ANIMATION_TIME = 120;
        internal float timeToDelete, timeToIncrease, timeToMoved, xShift, yShift;
        internal bool isMove, isDelete, isIncrease;                     // MAYBE Можно избавиться от этих переменных (но нужно ли?)
        internal int destinationColumn, destinationRow, increaseValue;
    }

    class Tile
    {
        int x, y, size, value, timeInterval;
        Rectangle rect;
        Font font;
        Brush fontBrush, rectBrush;
        StringFormat stringFormat;
        TileAnimation animation;
        public bool isToDelete;

        public Tile(int x, int y, int size, int num, int timeInterval)
        {
            this.x = x;
            this.y = y;
            this.size = size;
            this.value = num;
            this.timeInterval = timeInterval;
            animation = new TileAnimation();
            isToDelete = false;

            rect = new Rectangle(x, y, size, size);
            rectBrush = new SolidBrush(Color.FromArgb(200, Convert.ToInt32((value * 125684) % 256),
                Convert.ToInt32((value * 129075L) % 256),
                Convert.ToInt32((value * 532048L) % 256)));

            font = new Font("Arial", size / 3);
            fontBrush = new SolidBrush(Color.Black);

            this.stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
        }

        public void Draw(PaintEventArgs args)
        {
            args.Graphics.FillRectangle(rectBrush, rect);
            args.Graphics.DrawString(value.ToString(), font, fontBrush, x + size / 2, y + size / 2, stringFormat);
        }

        internal void Move(int nRow, int nColumn)
        {
            if (!animation.isMove)
            {
                animation.isMove = true;
                animation.timeToMoved = TileAnimation.ANIMATION_TIME;
            }
            animation.destinationColumn = nColumn;
            animation.destinationRow = nRow;

            animation.xShift = (animation.destinationRow * size - x) / (TileAnimation.ANIMATION_TIME / timeInterval);
            animation.yShift = (animation.destinationColumn * size - y) / (TileAnimation.ANIMATION_TIME / timeInterval);
        }

        internal void Increase(int value)
        {
            if (!animation.isIncrease)
            {
                animation.isIncrease = true;
                animation.timeToIncrease = TileAnimation.ANIMATION_TIME;
                animation.increaseValue = value;
            }
        }

        internal void Delete()
        {
            if (!animation.isDelete)
            {
                animation.isDelete = true;
                animation.timeToDelete = TileAnimation.ANIMATION_TIME;
            }
        }

        void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
            rect.X = x;
            rect.Y = y;
        }

        public bool CheckAnimation()
        {
            return animation.isDelete || animation.isIncrease || animation.isMove;
        }

        internal void Update(float dt)      // Удаление и увеличение тайла делается с задержкой..
        {
            if (animation.isMove)
            {
                animation.timeToMoved -= dt;
                SetPosition(Convert.ToInt32(x + animation.xShift), Convert.ToInt32(y + animation.yShift));

                if (animation.timeToMoved < 0)
                {
                    animation.timeToMoved = 0;
                    animation.isMove = false;
                    SetPosition(animation.destinationRow * size, animation.destinationColumn * size);
                }
            }
            if (animation.isDelete)
            {
                animation.timeToDelete -= dt;
                if (animation.timeToDelete < 0)
                {
                    animation.isDelete = false;
                    animation.timeToDelete = 0;
                    isToDelete = true;
                }
            }
            if (animation.isIncrease)
            {
                animation.timeToIncrease -= dt;
                if (animation.timeToIncrease < 0)
                {
                    animation.isIncrease = false;
                    animation.timeToIncrease = 0;
                    value = animation.increaseValue;
                    rectBrush = new SolidBrush(Color.FromArgb(200, Convert.ToInt32((value * 125684) % 256),
                        Convert.ToInt32((value * 129075L) % 256), Convert.ToInt32((value * 532048L) % 256)));
                }
            }
        }
    }
}
