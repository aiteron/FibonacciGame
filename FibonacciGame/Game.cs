using System.Windows.Forms;

namespace FibonacciGame
{
    class Game
    {
        const int FIELD_SIZE = 4;
        GameModel gameModel;
        MyForm1 form;

        public Game()
        {
            gameModel = new GameModel(FIELD_SIZE);
            form = new MyForm1(gameModel);
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
