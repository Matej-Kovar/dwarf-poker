using DwarvenPoker;

Player[] players = new Player[0];
Round game = new Round(players);
while (!game.End)
{
    while (game.InMenu)
    {
        Style.Display(game.MenuOutput());
        Console.Write($"  dwarvenPoker: ");
        Style.Display(game.MenuInput(Console.ReadLine()));
        Console.Write("  ");
        Console.ReadLine();
        Console.Clear();
    }
    do
    {
        do
        {
            Style.Display(game.DisplayStatus());
            Console.Write($"  dwarvenPoker@{game.CurrentPlayer.Name}: ");
            Style.Display(game.InGameInputs(Console.ReadLine()));
            Console.Write("  ");
            Console.ReadLine();
            Console.Clear();
        } while (!game.RoundEnd);
    } while (!game.NextPlayer());
    game.InMenu = true;
    Style.Display(game.FinishRound());
    Console.Write($"Press [enter] to continue");
    Console.Write("  ");
    Console.ReadLine();
    if (game.End)
    {
        Environment.Exit(0);
    }
    Console.Clear();

}
