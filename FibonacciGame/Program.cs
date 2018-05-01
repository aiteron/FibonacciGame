using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace FibonacciGame
{
    public class GameModel
    {
        const int VICTORY_VALUE = 55;
        Tuple<int, int>[,] field;
        public readonly int Size;
        Random rand;

        public GameModel(int size)
        {
            Size = size;
            field = new Tuple<int, int>[size, size];
            rand = new Random();
        }

        public void Initialize()
        {
            CreateNewTile();
        }

        public void CreateNewTile()
        {
            if (FieldIsFull())
                return;

            while (true)
            {
                int row = rand.Next() % Size;
                int column = rand.Next() % Size;

                if (field[row, column] == null)
                {
                    field[row, column] = new Tuple<int, int>(1, 1);
                    if (TileCreated != null) TileCreated(row, column, 1);
                    break;
                }
            }
        }

        public void Update(Keys key, bool isAnimation)
        {
            if (isAnimation)
                return;

            int dx = 0, dy = 0;

            switch (key)
            {
                case Keys.Up:
                    dy = -1;
                    for (int column = 0; column < Size; column++)
                        for (int row = 0; row < Size; row++)
                            Shift(column, row, dx, dy);
                    break;
                case Keys.Down:
                    dy = 1;
                    for (int column = 0; column < Size; column++)
                        for (int row = Size - 1; row >= 0; row--)
                            Shift(column, row, dx, dy);
                    break;
                case Keys.Left:
                    dx = -1;
                    for (int row = 0; row < Size; row++)
                        for (int column = 0; column < Size; column++)
                            Shift(column, row, dx, dy);
                    break;
                case Keys.Right:
                    dx = 1;
                    for (int row = 0; row < Size; row++)
                        for (int column = Size - 1; column >= 0; column--)
                            Shift(column, row, dx, dy);
                    break;
                // TODO написать бота
            }
            if (!CanMove())
                FullField();
        }

        bool FieldIsFull()
        {
            for(int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    if (field[i, j] == null)
                        return false;

            return true;
        }
        
        bool CanMove() // Проверка на возможность хода
        {
            if (!FieldIsFull())
                return true;

            // TODO Переписать код компактней

            for (int column = 0; column < Size; column++)
                for (int row = 1; row < Size; row++)
                    if(field[column, row - 1] != null && field[column, row] != null
                        && (field[column, row - 1].Item1 == field[column, row].Item2
                            || field[column, row - 1].Item2 == field[column, row].Item1))
                        return true;

            for (int column = 0; column < Size; column++)
                for (int row = Size - 2; row >= 0; row--)
                    if (field[column, row + 1] != null && field[column, row] != null
                        &&(field[column, row + 1].Item1 == field[column, row].Item2
                            || field[column, row + 1].Item2 == field[column, row].Item1))
                        return true;

            for (int row = 0; row < Size; row++)
                for (int column = 1; column < Size; column++)
                    if (field[column - 1, row] != null && field[column, row] != null
                        && (field[column-1, row].Item1 == field[column, row].Item2
                        || field[column-1, row].Item2 == field[column, row].Item1))
                            return true;

            for (int row = 0; row < Size; row++)
                for (int column = Size - 2; column >= 0; column--)
                    if (field[column + 1, row] != null && field[column, row] != null
                       && (field[column + 1, row].Item1 == field[column, row].Item2
                        || field[column + 1, row].Item2 == field[column, row].Item1))
                        return true;

            return false;
        }

        void Shift(int col, int row, int dx, int dy) 
        {
            if (field[col, row] == null)
                return;

            int shiftCol = col + dx;
            int shiftRow = row + dy;

            if(shiftCol < 0 || shiftCol >= Size || shiftRow < 0 || shiftRow >= Size)
                return;

            if (field[shiftCol, shiftRow] == null)
            {
                field[shiftCol, shiftRow] = field[col, row];
                field[col, row] = null;

                if (TileMoved != null) TileMoved(col, row, shiftCol, shiftRow);

                Shift(shiftCol, shiftRow, dx, dy);
            }
            else
            {
                if(field[col, row].Item2 == field[shiftCol, shiftRow].Item1)
                {
                    int sum = field[col, row].Item2 + field[shiftCol, shiftRow].Item2;
                    field[col, row] = new Tuple<int, int>(field[shiftCol, shiftRow].Item2, sum);
                    if (TileIncrease != null) TileIncrease(col, row, sum);

                    field[shiftCol, shiftRow] = field[col, row];
                    field[col, row] = null;
                    if (TileReplaced != null) TileReplaced(col, row, shiftCol, shiftRow);

                    if (sum == VICTORY_VALUE)
                        Victory();
                }
                else if (field[col, row].Item1 == field[shiftCol, shiftRow].Item2)
                {
                    int sum = field[col, row].Item2 + field[shiftCol, shiftRow].Item2;
                    field[col, row] = new Tuple<int, int>(field[col, row].Item2, sum);
                    if (TileIncrease != null) TileIncrease(col, row, sum);

                    field[shiftCol, shiftRow] = field[col, row];
                    field[col, row] = null;
                    if (TileReplaced != null) TileReplaced(col, row, shiftCol, shiftRow);

                    if (sum == VICTORY_VALUE)
                        Victory();
                }
            }
        }

        public event Action<int, int, int> TileCreated;
        public event Action<int, int, int, int> TileMoved;
        public event Action<int, int, int, int> TileReplaced;
        public event Action<int, int, int> TileIncrease;
        public event Action FullField;
        public event Action Victory;
    }

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

            font = new Font("Arial", size/3);
            fontBrush = new SolidBrush(Color.Black);

            this.stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
        }

        public void Draw(PaintEventArgs args)
        {
            args.Graphics.FillRectangle(rectBrush, rect);
            args.Graphics.DrawString(value.ToString(), font, fontBrush, x + size/2, y+size/2, stringFormat);  
        }

        internal void Move(int nRow, int nColumn)
        {
            if(!animation.isMove)
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
            if(animation.isMove)
            {
                animation.timeToMoved -= dt;
                SetPosition(Convert.ToInt32(x + animation.xShift), Convert.ToInt32(y + animation.yShift));

                if(animation.timeToMoved < 0)
                {
                    animation.timeToMoved = 0;
                    animation.isMove = false;
                    SetPosition(animation.destinationRow*size, animation.destinationColumn * size);
                }
            }
            if(animation.isDelete)
            {
                animation.timeToDelete -= dt;
                if(animation.timeToDelete < 0)
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

    class MyForm : Form
    {
        const int TILE_SIZE = 80;
        const int TIME_INTERVAL = 10;

        GameModel gameModel;
        Tile[,] field;
        List<Tile> buffer;  // Буффер для тайлов, которые сливаются (для корректной анимации)
        bool animationIsPlayed;

        public MyForm(GameModel gameModel)
        {
            ClientSize = new Size(TILE_SIZE * gameModel.Size, TILE_SIZE * gameModel.Size);
            this.Text = "Fibonacci Game";
            this.KeyPreview = true;     // Для перехвата кнопок
            DoubleBuffered = true;

            Timer timer = new Timer();
            timer.Interval = TIME_INTERVAL;
            timer.Tick += (sender, args) => this.TilesUpdate();    
            /* Тут нюанс. Мне нужен таймер только для анимаций. И я его использую в методе "Представления".
             * А для модели я использую event нажатия клавиш. Но в требованиях к игре указано, что должен
             * вызываться метод Update из "Модели". Я могу сделать пустой метод и вызывать, но толку от 
             * этого не будет. Invalidate() вызывается из метода "Представления". Как быть?
             */
            timer.Start();

            this.gameModel = gameModel;
            buffer = new List<Tile>();
            field = new Tile[gameModel.Size, gameModel.Size]; 
            animationIsPlayed = false;


            this.KeyDown += (sender, args) => this.gameModel.Update(args.KeyCode, animationIsPlayed);   // Контроллер
            /* А тут использую переменную animationIsPlayed, чтобы не принимать ввод с клавиатуры
             * во время анимации. Не нарушает ли это требования? Можно просто переделать под
             * событие и привязать "ловца".
             */ 

            AnimationEnds += () => this.gameModel.CreateNewTile();
            /* Тут тоже нюанс. У меня немного отличается от стандартной модели MVC. Тут "Представление" создает
             * событие, которое обрабатывает "Модель". Можно так сделать? Это сделано для корректной анимации.
             * (Чтобы новая плитка появлялась после того, как остальные передвинулись) 
             */

            this.gameModel.TileCreated += (row, column, value) => field[row, column] = new Tile(row*TILE_SIZE, column*TILE_SIZE, TILE_SIZE, value, TIME_INTERVAL);
            this.gameModel.TileMoved += (row, column, nRow, nColumn) =>
            {
                field[row, column].Move(nRow, nColumn);
                field[nRow, nColumn] = field[row, column];
                field[row, column] = null;
            };
            this.gameModel.TileReplaced += (row, column, nRow, nColumn) =>
            {
                field[row, column].Move(nRow, nColumn);
                field[nRow, nColumn].Delete();
                buffer.Add(field[nRow, nColumn]);
                field[nRow, nColumn] = field[row, column];
                field[row, column] = null;
            };
            this.gameModel.TileIncrease += (row, column, value) => field[row, column].Increase(value);
            this.gameModel.FullField += () => 
            {
                MessageBox.Show("YOU LOSE", "-_-", MessageBoxButtons.OK);
                Application.Exit();
            };
            this.gameModel.Victory += () =>
            {
                MessageBox.Show("YOU WIN", "^_^", MessageBoxButtons.OK);
                Application.Exit();
            };

            Paint += (sender, args) =>
            {
                foreach (var tile in field)
                    if(tile != null)
                        tile.Draw(args);

                foreach (var tile in buffer)
                    tile.Draw(args);
            };
        }

        void TilesUpdate()
        {
            foreach (var tile in field)
                if (tile != null)
                    tile.Update(TIME_INTERVAL);

            for(int i = 0; i < buffer.Count; i++)
            {
                buffer[i].Update(TIME_INTERVAL);
                if (buffer[i].isToDelete)
                    buffer.Remove(buffer[i]);
            }

            Invalidate();
            animationIsPlayed = CheckAnimation();
        }

        private bool CheckAnimation()
        {
            foreach (var tile in field)
                if (tile != null && tile.CheckAnimation())
                    return true;

            if (animationIsPlayed && AnimationEnds != null)
                AnimationEnds();

            return false;
        }

        public event Action AnimationEnds;
    }

    class Game
    {
        const int FIELD_SIZE = 4;
        GameModel gameModel;
        MyForm form;

        public Game()
        {
            gameModel = new GameModel(FIELD_SIZE);
            form = new MyForm(gameModel);
        }

        public void Start()
        {
            gameModel.Initialize();
            Application.Run(form);
        }

        public static void Main()
        {
            Game game = new Game();
            game.Start();
        }
    }
}
