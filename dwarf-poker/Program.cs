using dwarf_poker;

Player[] players = new Player[5];
for (int i = 0; i < 5; i++)
{
    Dice[] dices = new Dice[5];
    for (int j = 0; j < 5; j++)
    {
        dices[j] = new Dice([1, 2, 3, 4, 5, 6]);
    }
    players[i] = new Player(i.ToString(), 100, 2, dices);
}
Game game = new Game(players);

do
{
    do
    {
        string[] display = game.DisplayStatus();
        foreach (string s in display)
        {
            Console.WriteLine(s);
        }
        display = game.InputHandling(Console.ReadLine());
        foreach (string s in display)
        {
            Console.WriteLine(s);
        }
        Console.ReadLine();
        Console.Clear();
    } while (!game.RoundEnd);
    /*Console.WriteLine("Your round has ended");
    Thread.Sleep(1500);
    Console.Clear();*/
} while (!game.NextPlayer());
//decide winner



