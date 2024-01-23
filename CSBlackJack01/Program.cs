using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSBlackJack01
{
    internal class Program
    {
        static int numOfPlayers = 0;

        static List<int> playersOutOfRound = new List<int>();

        static string[] suits = new string[4]  { "Hearts", "Diamonds", "Clubs", "Spades" };
        static string[] ranks = new string[13] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

        static List<string> cards = new List<string>();

        // Key is which, value the number betted if -1 they skipped
        static Dictionary<int, int> playersBetts = new Dictionary<int, int>();

        // Player number, The stack number for that player, the cards for each stack
        static Dictionary<int, Dictionary<int, List<string>>> playerStacks = new Dictionary<int, Dictionary<int, List<string>>>();
        static List<string> dealersHand = new List<string>();
        static string dealersHiddenCard = string.Empty;

        static void Main(string[] args)
        {
            PopulateCards();

            while (numOfPlayers > 7 || numOfPlayers < 1)
            {
                try
                {
                    Console.Write("How many players (Max 7)? ");
                    numOfPlayers = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }

            InitGame();

            while (true)
            {
                // Display player information and options
                foreach (KeyValuePair<int, int> chips in playersBetts)
                {
                    int userInput = -1;

                    // Loop for the current player until '2' (Stand) is pressed
                    while (userInput != 2)
                    {
                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("0 - Exit Program");
                        Console.ResetColor();

                        if (playersOutOfRound.Contains(chips.Key))
                        {
                            Console.WriteLine($"Player {chips.Key}: Is Out.");
                            break;
                        }

                        if (chips.Value == -1)
                        {
                            Console.WriteLine($"\nPlayer {chips.Key}: Sitted Out");
                        }
                        else
                        {
                            Console.WriteLine($"Player {chips.Key}: ${chips.Value}");

                            foreach (string card in playerStacks[chips.Key][1])
                            {
                                Console.WriteLine(card);
                            }
                        }

                        Console.WriteLine("What do you want to do?");

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("  1 - Hit");

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("  2 - Stand");

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("  3 - Double");

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("  4 - Split");

                        Console.ResetColor();

                        // Display dealer's hand
                        Console.WriteLine("\nDealer:");
                        foreach (string dealerCard in dealersHand)
                        {
                            Console.WriteLine(dealerCard);
                            Console.WriteLine("Hidden Card");
                        }

                        userInput:
                        try
                        {
                            userInput = Convert.ToInt32(Console.ReadLine());
                        }
                        catch (FormatException)
                        {
                            goto userInput;
                        }

                        // Process user input and call functions as needed
                        switch (userInput)
                        {
                            case 1:
                                // Handle Hit
                                // ...
                                break;
                            case 2:
                                break;
                            case 3:
                                // Handle Double
                                // ...
                                break;
                            // Add cases for other options as needed
                            // ...
                            default:
                                Console.Clear();
                                goto end;

                        }
                    }
                }
                
                Console.Clear();
                break;

            }

            end:
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void InitGame()
        {
            playersOutOfRound.Clear();
            playerStacks.Clear();
            dealersHand.Clear();
            dealersHiddenCard = string.Empty;

            SetPlayerAmount();

            Deal();

            DealerCheck();
            PlayersCheck();
        }

        static void PopulateCards()
        {
            foreach (string suit in suits)
            {
                foreach (string rank in ranks)
                {
                    cards.Add($"{rank} of {suit}");
                }
            }
        }

        static void SetPlayerAmount() 
        {
            int numBetted;

            for (int i = 0; i < numOfPlayers; i++) 
            {
                Console.WriteLine($"Player {i + 1}:");

                userInput:
                try
                {
                    Console.Write("  How much do you want to bet (-1 to sit out): ");
                    numBetted = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                    goto userInput;
                }

                while (numBetted == 0)
                {
                    try
                    {
                        Console.Write("  How much do you want to bet (-1 to sit out): ");
                        numBetted = Convert.ToInt32(Console.ReadLine());
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                    }
                }

                playersBetts[i + 1] = numBetted;
            }
        }

        static void Deal()
        {
            Random random = new Random();

            Console.Clear();
            for (int i = 0;i < 2;i++)
            {
                foreach (KeyValuePair<int, int> chips in playersBetts)
                {
                    if (chips.Value > 0)
                    {
                        if (!playerStacks.ContainsKey(chips.Key))
                        { 
                            playerStacks[chips.Key] = new Dictionary<int, List<string>>();
                            playerStacks[chips.Key][1] = new List<string>();
                        }
                         
                        int randNumDict = random.Next(cards.Count);
                        playerStacks[chips.Key][1].Add(cards[randNumDict]);
                        cards.RemoveAt(randNumDict);
                    }
                }

                int randNum = random.Next(cards.Count);

                if (i == 0)
                {
                    dealersHand.Add(cards[randNum]);
                    cards.RemoveAt(randNum);
                }
                else
                {
                    dealersHiddenCard = cards[randNum];
                    cards.RemoveAt(randNum);
                }

            }
        }

        static void DealerCheck()
        {
            string cardRank = dealersHand[0].Substring(0, dealersHand[0].IndexOf(' '));
            if (cardRank == "King" || cardRank == "Queen" || cardRank == "Jack" || cardRank == "10" || cardRank == "Ace")
            {
                if (cardRank == "Ace")
                {
                    cardRank = dealersHiddenCard.Substring(0, dealersHiddenCard.IndexOf(' '));
                    if (cardRank == "King" || cardRank == "Queen" || cardRank == "Jack" || cardRank == "10")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Dealer has a blackjack.");
                        Console.ResetColor();

                        foreach (KeyValuePair<int, int> chips in playersBetts)
                        {
                            if (chips.Value != -1)
                            {
                                playersBetts[chips.Key] -= chips.Value;
                                Console.WriteLine($"{(chips.Value == 1 ? $"Player {chips.Key} as lost {chips.Value} dollar" : $"Player {chips.Key} as lost {chips.Value} dollars")}");
                                InitGame();
                            }
                        }
                    }
                }
                else
                {
                    cardRank = dealersHiddenCard.Substring(0, dealersHiddenCard.IndexOf(' '));
                    if (cardRank == "Ace")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Dealer has a blackjack.");
                        Console.ResetColor();

                        foreach (KeyValuePair<int, int> chips in playersBetts)
                        {
                            if (chips.Value != -1)
                            {
                                playersBetts[chips.Key] -= chips.Value;
                                Console.WriteLine($"{(chips.Value == 1 ? $"Player {chips.Key} as lost {chips.Value} dollar" : $"Player {chips.Key} as lost {chips.Value} dollars")}");
                                InitGame();
                            }
                        }
                    }
                }
            }
        }

        static void PlayersCheck()

        {
            foreach (KeyValuePair<int, int> player in playersBetts.ToList())
            {
                if (player.Value == -1)
                {
                    continue;
                }
                foreach (List<string> card in playerStacks[player.Key].Values)
                { 
                    string cardRank = card[0].Substring(0, card[0].IndexOf(' '));
                    if (cardRank == "King" || cardRank == "Queen" || cardRank == "Jack" || cardRank == "10" || cardRank == "Ace")
                    {
                        if (cardRank == "Ace")
                        {
                            cardRank = card[1].Substring(0, card[1].IndexOf(' '));
                            if (cardRank == "King" || cardRank == "Queen" || cardRank == "Jack" || cardRank == "10")
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Player {player.Key} has a blackjack");
                                Console.ResetColor();

                         
                                if (player.Value != -1)
                                {
                                    playersBetts[player.Key] += player.Value;
                                    Console.WriteLine($"{(player.Value == 1 ? $"Player {player.Key} as gain {player.Value} dollar" : $"Player {player.Key} as gain {player.Value} dollars")}");
                                    playersOutOfRound.Add(player.Key);
                                }

                            }
                        }
                        else
                        {
                            cardRank = card[1].Substring(0, card[1].IndexOf(' '));
                            if (cardRank == "Ace")
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Player {player.Key} has a blackjack");
                                Console.ResetColor();


                                if (player.Value != -1)
                                {
                                    playersBetts[player.Key] += player.Value;
                                    Console.WriteLine($"{(player.Value == 1 ? $"Player {player.Key} as gain {player.Value} dollar" : $"Player {player.Key} as gain {player.Value} dollars")}");
                                    playersOutOfRound.Add(player.Key);
                                }
                            }
                        }
                    }
                }
            }
        }
    } // Class
} // Namespace
