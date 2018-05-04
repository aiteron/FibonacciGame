using System;
using System.Windows.Forms;

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
}
