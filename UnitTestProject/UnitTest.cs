using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FibonacciGame;
using System.Windows.Forms;
using System.Drawing;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTestGames
    {
        [TestMethod]
        public void SmallFieldFail()
        {
            bool param = false;

            GameModel gameModel = new GameModel(1);

            gameModel.FullField += () => param = true;

            gameModel.Initialize();
            gameModel.Update(Keys.Up, false);

            Assert.AreEqual(param, true);
        }

        [TestMethod]
        public void AnimationPlay()
        {
            bool param = false;

            GameModel gameModel = new GameModel(1);

            gameModel.FullField += () => param = true;

            gameModel.Initialize();
            gameModel.Update(Keys.Up, true);

            Assert.AreEqual(param, false);
        }

        [TestMethod]
        public void NormalGame()
        {
            bool param = false;

            GameModel gameModel = new GameModel(2);

            gameModel.FullField += () => param = true;
            gameModel.Victory += () => param = true;

            gameModel.Initialize();
            for(int i = 0; i < 50; i++)
            {
                gameModel.Update(Keys.Up, false);
                gameModel.CreateNewTile();
                gameModel.Update(Keys.Left, false);
                gameModel.CreateNewTile();
                gameModel.Update(Keys.Down, false);
                gameModel.CreateNewTile();
                gameModel.Update(Keys.Right, false);
                gameModel.CreateNewTile();
            }
            Assert.AreEqual(param, true);
        }

        [TestMethod]
        public void TileCreated()
        {
            Tuple<int, int, int> param = new Tuple<int, int, int>(-1, -1, -1);

            GameModel gameModel = new GameModel(1);
            gameModel.TileCreated += (row, column, value) =>
            {
                param = new Tuple<int, int, int>(row, column, value);
            };
            gameModel.Initialize();

            Assert.AreEqual(param, new Tuple<int, int, int>(0, 0, 1));
        }

        [TestMethod]
        public void TileMoved()
        {
            bool param = false;

            GameModel gameModel = new GameModel(2);

            gameModel.TileMoved += (int col, int row, int ncol, int nrow) => param = true;

            gameModel.Initialize();
            gameModel.Update(Keys.Up, false);
            gameModel.Update(Keys.Left, false);
            gameModel.Update(Keys.Down, false);
            gameModel.Update(Keys.Right, false);

            Assert.AreEqual(param, true);
        }

        [TestMethod]
        public void TileActions()
        {
            List<Tuple<int, int, int, int>> replaceValues = new List<Tuple<int, int, int, int>>();
            List<Tuple<int, int, int>> increaseValues = new List<Tuple<int, int, int>>();

            GameModel gameModel = new GameModel(2);

            gameModel.TileReplaced += (int col, int row, int ncol, int nrow) => replaceValues.Add(new Tuple<int, int, int, int>(col, row, ncol, nrow));
            gameModel.TileIncrease += (int col, int row, int val) => increaseValues.Add(new Tuple<int, int, int>(col, row, val));

            gameModel.Initialize();
            gameModel.CreateNewTile();
            gameModel.CreateNewTile();
            gameModel.CreateNewTile();
            gameModel.Update(Keys.Up, false);

            Assert.AreEqual(replaceValues[0], new Tuple<int, int, int, int>(0,1,0,0));
            Assert.AreEqual(replaceValues[1], new Tuple<int, int, int, int>(1, 1, 1, 0));

            Assert.AreEqual(increaseValues[0], new Tuple<int, int, int>(0, 1, 2));
            Assert.AreEqual(increaseValues[1], new Tuple<int, int, int>(1, 1, 2));
        }

    }
}
