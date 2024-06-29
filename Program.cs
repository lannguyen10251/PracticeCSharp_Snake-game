using System;
using System.Diagnostics;
using System.Threading;
class SnakeGame 
{
    static void Main() 
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"                  _________ _______      _____   ____  __.___________   ________    _____      _____  ___________
                 /   _____/ \      \    /  _  \ |    |/ _|\_   _____/  /  _____/   /  _  \    /     \ \_   _____/
                 \_____  \  /   |   \  /  /_\  \|      <   |    __)_  /   \  ___  /  /_\  \  /  \ /  \ |    __)_ 
                 /        \/    |    \/    |    \    |  \  |        \ \    \_\  \/    |    \/    Y    \|        \
                /_______  /\____|__  /\____|__  /____|__ \/_______  /  \______  /\____|__  /\____|__  /_______  /
                        \/         \/         \/        \/        \/          \/         \/         \/        \/ 
"); 
        Console.ResetColor(); 
        Thread.Sleep(2000);
        Console.Clear();
        ChooseLevel();
        SnakeGame game = new SnakeGame();
        SnakeGame.Start(); 
    }

    static int BOARD_WIDTH = 40;
    static int BOARD_HEIGHT = 20;
    static int FOOD_SCORE = 10;
    static int LEVEL_SCORE = 50;

    static int score;
    static int level;
    static int sleepTime;

    static bool GameOver;
    static bool Pause;
   
    static int[,] board;
    static Random random = new Random();

    static Direction direction;
    static object lockObject = new object();

    private enum Direction
    {
        Left,
        Up,
        Right,
        Down
    }

    private struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    static Point head;
    static Point[] body;

    static SnakeGame()
    {
        score = 0;
        level = 1;
        sleepTime = 200;

        GameOver = false;
        Pause = false;

        board = new int[BOARD_WIDTH, BOARD_HEIGHT];
        direction = Direction.Right;

        head = new Point(BOARD_WIDTH / 2, BOARD_HEIGHT / 2);
        body = new Point[3];
        for (int i = 0; i < body.Length; i++)
        {
            body[i] = new Point(head.X - i - 1, head.Y);
        }

        PlaceFood();
        PlaceObstacle();
    }

    static void PlaceFood()
    {
        int x = random.Next(BOARD_WIDTH);
        int y = random.Next(BOARD_HEIGHT);

        while (board[x, y] != 0)
        {
            x = random.Next(BOARD_WIDTH);
            y = random.Next(BOARD_HEIGHT);
        }

        board[x, y] = -1;
    }
    static void PlaceObstacle()
    {
        int x = random.Next(BOARD_WIDTH);
        int y = random.Next(BOARD_HEIGHT);

        while (board[x, y] != 0 && board[x, y] == -1)
        {
            x = random.Next(BOARD_WIDTH);
            y = random.Next(BOARD_HEIGHT);
        }

        board[x, y] = 2;
    }

    static void DrawBoard()
    {
        Console.SetCursorPosition(0, 0);
         Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        // Draw top border
        for (int i = 0; i < BOARD_WIDTH +1; i++)
        {
            Console.Write("-");
        }
        Console.WriteLine();
        Console.ResetColor();

        // Draw board contents
        for (int y = 0; y < BOARD_HEIGHT; y++)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("||");
            Console.ResetColor();
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                if (board[x, y] == 0)
                {
                    Console.Write(" ");
                }
                else if (board[x, y] == -1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("@");
                    Console.ResetColor();
                }
                else if (board[x, y] == 2)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("X");
                    Console.ResetColor();
                } 
                    
                else
                {
                    Random random = new Random();
                    Console.ForegroundColor = (ConsoleColor)random.Next(1, 12);
                    Console.Write(@"▄");
                    Console.ResetColor();
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("||");
            Console.ResetColor();
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        // Draw bottom border
        for (int i = 0; i < BOARD_WIDTH + 1; i++)
        {
            Console.Write("-");
        }

        Console.SetCursorPosition(0, 0); //trên bên trái
        Console.Write("+");

        Console.SetCursorPosition(BOARD_WIDTH + 1, 0); //trên phải
        Console.Write("--+");

        Console.SetCursorPosition(0, BOARD_HEIGHT + 1); //đặt con trỏ góc dưới bên trái
        Console.Write("+");

        Console.SetCursorPosition(BOARD_WIDTH + 1, BOARD_HEIGHT  +1); //dưới bên phải
        Console.Write("--+"); Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine("Score: {0}", score);
        Console.WriteLine("Level: {0}", level);
    }

    static void ChooseLevel()
    {
        Console.WriteLine("\t \t \t \t GUIDE");
        Console.WriteLine("Enter button number 1 or 2 or button 3 to start playing level 1 or 2 or 3.");
        Console.WriteLine("Enter button number 0 button to start playing from the beginning.");
        Console.Write("\nEnter level (0-3): ");

        int selectedLevel = Convert.ToInt32(Console.ReadLine());
        if (selectedLevel >= 1 && selectedLevel <= 3)
        {
            level = selectedLevel;
        }
        else
        {
            Console.WriteLine("Invalid level, defaulting to level 1.");
            level = 1;
        }
        Console.WriteLine();
        Console.Write(@" 
            GUIDE
Press button ( <- ↓ → ↑ )  to move.
Press SpaceBar to pause or resume the game.

If you are ready then press enter to start the game.
");
        Console.ReadLine();
        Console.Clear();
        switch (level)
        {
            case 1:
                sleepTime = 200;
                break;
            case 2:
                sleepTime = 150;
                break;
            case 3:
                sleepTime = 100;
                break;
            case 0:
                sleepTime = 200;
                break;
        }
    } 
    static void Move()
    {
        int dx = 0, dy = 0;

        switch (direction)
        {
            case Direction.Left:
                dx = -1;
                break;
            case Direction.Up:
                dy = -1;
                break;
            case Direction.Right:
                dx = 1;
                break;
            case Direction.Down:
                dy = 1;
                break;
        }

        Console.Beep(800, 100);

        Thread.Sleep(50);

        Point tail = body[body.Length - 1];

        for (int i = body.Length - 1; i > 0; i--)
        {
            body[i] = body[i - 1];
        }

        body[0] = head;
        head = new Point(head.X + dx, head.Y + dy);

        if (head.X < 0 || head.X >= BOARD_WIDTH || head.Y < 0 || head.Y >= BOARD_HEIGHT || board[head.X, head.Y] > 0 || board[head.X, head.Y] == 2)
        {
            for (int i = 0; i < 5; i++) 
            {
                Console.Beep(300 + i * 100, 200); 
                Thread.Sleep(200); 
            }
            GameOver = true;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Clear();
            Console.Write(@"
 ██████╗  █████╗ ███╗   ███╗███████╗ ██████╗ ██╗   ██╗███████╗██████╗ ██╗
██╔════╝ ██╔══██╗████╗ ████║██╔════╝██╔═══██╗██║   ██║██╔════╝██╔══██╗██║
██║  ███╗███████║██╔████╔██║█████╗  ██║   ██║██║   ██║█████╗  ██████╔╝██║
██║   ██║██╔══██║██║╚██╔╝██║██╔══╝  ██║   ██║╚██╗ ██╔╝██╔══╝  ██╔══██╗╚═╝
╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗╚██████╔╝ ╚████╔╝ ███████╗██║  ██║██╗
 ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝ ╚═════╝   ╚═══╝  ╚══════╝╚═╝  ╚═╝╚═╝
"+ "\n \n SCORE {0}", score);
            Thread.Sleep(3000);
            Console.Clear();
            return;
        }

        if (board[head.X, head.Y] == -1)
        {
            Console.Beep(1000, 200); 
            Thread.Sleep(500);
            score += FOOD_SCORE;
            Array.Resize(ref body, body.Length + 1);
            body[body.Length - 1] = tail;
            PlaceFood();
            PlaceObstacle();
        }
        else
        {
            board[tail.X, tail.Y] = 0;
           
            board[head.X, head.Y] = body.Length + 1;
            
        }

        if (score >= level * LEVEL_SCORE)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(Console.WindowWidth/ 2, Console.WindowHeight/2 - 7);
            Console.WriteLine(@"   
                           ____                            _         _       _   _                 _ 
                          / ___|___  _ __   __ _ _ __ __ _| |_ _   _| | __ _| |_(_) ___  _ __  ___| |
                         | |   / _ \| '_ \ / _` | '__/ _` | __| | | | |/ _` | __| |/ _ \| '_ \/ __| |
                         | |__| (_) | | | | (_| | | | (_| | |_| |_| | | (_| | |_| | (_) | | | \__ \_|
                          \____\___/|_| |_|\__, |_|  \__,_|\__|\__,_|_|\__,_|\__|_|\___/|_| |_|___(_)
                                           |___/                                                     
");
            Console.ResetColor();
            int[] freqs = { 262, 392, 440, 494, 262, 330, 262, 330, 392, 440, 494, 523, 659, 659, 523, 494, 440, 392 };
            int time = 100;
            for (int i = 0; i < freqs.Length; i++)
            {
                Console.Beep(freqs[i], time);
            }
            Thread.Sleep(500);
            for (int i = 0; i < freqs.Length; i++)
            {
                Console.Beep(freqs[i], time);
            }
            Console.SetCursorPosition(Console.WindowWidth / 2 + 1 , Console.WindowHeight / 2 + 2);
            Console.WriteLine("LEVEL {0}", level + 1);
            
            Thread.Sleep(3000);
            Console.Clear();

            level++;
            sleepTime -= 50;
        }

        Thread.Sleep(sleepTime);
    }

    static T[] ResizeArray<T>(T[] array, int size)
    {
        T[] newArray = new T[array.Length + size];
        Array.Copy(array, newArray, array.Length);
        return newArray;
    }

    static void ReadInput()
    {
        while (!GameOver)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (direction != Direction.Right)
                    {
                        direction = Direction.Left;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (direction != Direction.Down)
                    {
                        direction = Direction.Up;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (direction != Direction.Left)
                    {
                        direction = Direction.Right;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (direction != Direction.Up)
                    {
                        direction = Direction.Down;
                    }
                    break;
                case ConsoleKey.Spacebar:
                    Pause = !Pause;
                    break;
            }
        }
    }

    static void GameLoop()
    {
        while (!GameOver)
        {
            lock (lockObject)
            {
                if (!Pause)
                {
                    Move();
                    DrawBoard();
                }
            }
        }
    }

    static void Start()
    {
        Thread inputThread = new Thread(ReadInput);
        inputThread.Start();

        GameLoop();

        inputThread.Join();
    }
}
