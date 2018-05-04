using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace FibonacciGame
{
    class MyForm1 : Form
    {
        const int TILE_SIZE = 80;
        const int TIME_INTERVAL = 10;

        GameModel gameModel;
        Tile[,] field;
        List<Tile> buffer;  // Буффер для тайлов, которые сливаются (для корректной анимации)
        bool animationIsPlayed;

        public MyForm1(GameModel gameModel)
        {
            ClientSize = new Size(TILE_SIZE * gameModel.Size, TILE_SIZE * gameModel.Size);
            this.Text = "Fibonacci Game";
            this.KeyPreview = true;     // Для перехвата кнопок
            DoubleBuffered = true;

            Timer timer = new Timer();
            timer.Interval = TIME_INTERVAL;
            timer.Tick += (sender, args) => this.TilesUpdate();
            timer.Start();

            this.gameModel = gameModel;
            buffer = new List<Tile>();
            field = new Tile[gameModel.Size, gameModel.Size];
            animationIsPlayed = false;


            this.KeyDown += (sender, args) => this.gameModel.Update(args.KeyCode, animationIsPlayed);   // Контроллер
                                                                                                        

            AnimationEnds += () => this.gameModel.CreateNewTile();

            this.gameModel.TileCreated += (row, column, value) => field[row, column] = new Tile(row * TILE_SIZE, column * TILE_SIZE, TILE_SIZE, value, TIME_INTERVAL);
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
                    if (tile != null)
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

            for (int i = 0; i < buffer.Count; i++)
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
}
