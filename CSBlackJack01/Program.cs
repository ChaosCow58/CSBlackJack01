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

        static int[] numOfChips;

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

            numOfChips = new int[numOfPlayers];

            InitGame();
               
            while (true)
            {
                int userInput = -1;
                Console.WriteLine("Exit Program - 0");
                foreach (KeyValuePair<int, int> chips in playersBetts)
                {
                    if (chips.Value == -1)
                    {
                        if (chips.Key > 1)
                        {
                            Console.WriteLine($"\nPlayer {chips.Key}: Sitted Out");
                        }
                        else
                        {
                            Console.WriteLine($"Player {chips.Key}: Sitted Out");
                        }
                    }
                    else
                    {
                        if (chips.Key > 1)
                        {
                            Console.WriteLine($"{(chips.Value == 1 ? $"\nPlayer {chips.Key}: {chips.Value} chip" : $"\nPlayer {chips.Key}: {chips.Value} chips")}");
                        }
                        else
                        {
                            Console.WriteLine($"{(chips.Value == 1 ? $"Player {chips.Key}: {chips.Value} chip" : $"Player {chips.Key}: {chips.Value} chips")}");
                        }
                        foreach (string card in playerStacks[chips.Key][1])
                        {
                            Console.WriteLine(card);
                        }

                        Console.WriteLine("What do you want to do?");
                        Console.WriteLine("  1 - Hit");
                        Console.WriteLine("  2 - Stand");
                        Console.WriteLine("  3 - Double");
                        Console.WriteLine("  4 - Split");
                        userInput = Convert.ToInt32(Console.ReadLine());

                       
                        Console.WriteLine("\nDealer:");
                        foreach (string dealerCard in dealersHand)
                        {
                            Console.WriteLine(dealerCard);
                            Console.WriteLine("Hidden Card");
                        }

                        if (userInput == 0)
                        {
                            Console.Clear();
                            break;
                        }
                        
                    }
                }
                break;
            }


            Console.WriteLine("\nPress any key to exit...");
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

                try
                {
                    Console.Write("  How much do you want to bet (-1 to sit out): ");
                    numBetted = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                    Console.Write("  How much do you want to bet (-1 to sit out): ");
                    numBetted = Convert.ToInt32(Console.ReadLine());
                }

                numOfChips[i] = 5;

                while (numBetted > numOfChips[i] || numBetted == 0)
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
                            if (numOfChips[chips.Key - 1] != -1)
                            { 
                                numOfChips[chips.Key - 1] -= chips.Value;
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
                            if (numOfChips[chips.Key - 1] != -1)
                            {
                                numOfChips[chips.Key - 1] -= chips.Value;
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
            foreach (KeyValuePair<int, int> player in playersBetts)
            {
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

                         
                                if (numOfChips[player.Key - 1] != -1)
                                {
                                    numOfChips[player.Key - 1] += player.Value;
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


                                if (numOfChips[player.Key - 1] != -1)
                                {
                                    numOfChips[player.Key - 1] += player.Value;
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
