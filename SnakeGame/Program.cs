namespace SnakeGame;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public enum GameStatus
{
    Ongoing,
    Won,
    Lost
}

public class GameField
{
    public int Width { get; init; }
    public int Height { get; init; }

    public bool IsInside(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }
}

public class Food
{
    public int X { get; init; }
    public int Y { get; init; }
}

public class Snake
{
    public List<(int x, int y)> Body { get; set; } = [(0, 0)];
    public Direction Direction { get; set; } = Direction.Right;

    public void Move()
    {
        var head = Body[0];

        switch (Direction)
        {
            case Direction.Up:
                head.y--;
                break;
            case Direction.Down:
                head.y++;
                break;
            case Direction.Left:
                head.x--;
                break;
            case Direction.Right:
                head.x++;
                break;
        }

        Body.Insert(0, head);
        Body.RemoveAt(Body.Count - 1);
    }

    public void Grow()
    {
        var tail = Body[^1];
        Body.Add(tail);
    }

    public bool IsSelfEating()
    {
        var head = Body[0];
        return Body.Skip(1).Take(Body.Count - 2).Any(part => part == head);
    }
}


public class Game
{
    private GameField Field { get; set; }
    public Snake Snake { get; init; }
    private GameStatus Status { get; set; }
    private Food Food { get; set; }

    public Game(int width, int height)
    {
        Field = new GameField { Width = width, Height = height };
        Snake = new Snake();
        Status = GameStatus.Ongoing;
    }

    public void GenerateFood()
    {
        var random = new Random();
        int x, y;
        do
        {
            x = random.Next(0, Field.Width);
            y = random.Next(0, Field.Height);
        } while (Snake.Body.Any(part => part.x == x && part.y == y));

        Food = new Food
        {
            X = x,
            Y = y
        };
    }

    private void PrintGameField()
    {
        for (int y = 0; y < Field.Height; y++)
        {
            for (int x = 0; x < Field.Width; x++)
            {
                if (Snake.Body.Any(part => part.x == x && part.y == y))
                {
                    Console.Write("S");
                }
                else if (Food.X == x && Food.Y == y)
                {
                    Console.Write("F");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }
    }

    public void GameLoop()
    {
        while (Status == GameStatus.Ongoing)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        Snake.Direction = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        Snake.Direction = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                        Snake.Direction = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        Snake.Direction = Direction.Right;
                        break;
                }
            }

            Snake.Move();

            if (Snake.Body[0].x == Food.X && Snake.Body[0].y == Food.Y)
            {
                Snake.Grow();
                GenerateFood();
            }

            if (!Field.IsInside(Snake.Body[0].x, Snake.Body[0].y) || Snake.IsSelfEating())
            {
                Status = GameStatus.Lost;
            }
            else if (Snake.Body.Count == (Field.Width * Field.Height)*0.3)
            {
                Status = GameStatus.Won;
            }
            Console.Clear();
            DisplayProgress();
            PrintGameField();
            if (Status == GameStatus.Lost)
            {
                Console.WriteLine("\nGame Over!");
            } else if (Status == GameStatus.Won)
            {
                Console.WriteLine("\nYou won!");
            }
            
            Thread.Sleep(250);
        }
    }

    private void DisplayProgress()
    {
        Console.WriteLine($"\n\nSnake length: {Snake.Body.Count}/{(Field.Width * Field.Height)*0.3}");
    }
}

static class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("x: " + args[0] + " y: " + args[1]);
        var game = new Game(int.Parse(args[0]), int.Parse(args[1]))
        {
            Snake = new Snake()
        };
        game.GenerateFood();
        game.GameLoop();
    }
}