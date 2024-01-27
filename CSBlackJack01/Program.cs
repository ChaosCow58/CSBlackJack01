using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSBlackJack01
{
    internal class Program
    {
        static int numOfPlayers = 0;
        static int currentPlayer = 0;

        // Player num, money amount
        static Dictionary<int, int> playerMoneyPool = new Dictionary<int, int>();

        static List<int> playersOutOfRound = new List<int>();
        static List<int> playersOutOfGame = new List<int>();

        static string[] suits = new string[4]  { "Hearts", "Diamonds", "Clubs", "Spades" };
        static string[] ranks = new string[13] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

        static List<string> deck = new List<string>();

        // Key is which, value the number betted if -1 they skipped
        static Dictionary<int, int> playersBetts = new Dictionary<int, int>();

        // Player number, The stack number for that player, the cards for each stack
        static Dictionary<int, Dictionary<int, List<string>>> playerStacks = new Dictionary<int, Dictionary<int, List<string>>>();

        // Player number, Stack number, which card in pile, value
        static Dictionary<int, Dictionary<int, Dictionary<int, int>>> aceValues = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

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
                catch
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }

            for (int i = 1; i <= numOfPlayers; i++)
            {
                playerMoneyPool[i] = 1000;
            }

            InitGame();

         
            // Display player information and options
            foreach (KeyValuePair<int, int> chips in playersBetts)
            {
                bool aceChecked = false;

                currentPlayer = chips.Key;

                if (playersOutOfGame.Contains(chips.Key))
                {
                    Console.Clear();
                    Console.WriteLine($"Player {chips.Key} is out of money");
                    Thread.Sleep(1000);
                    continue;
                }
                int userInput = -1;

                // Loop for the current player until '2' (Stand) is pressed
                while (userInput != 2)
                {
                    Console.Clear();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("0 - Exit Program");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Player {chips.Key} has ${playerMoneyPool[chips.Key]}");

                    Console.ResetColor();
                    if (playersOutOfRound.Contains(chips.Key))
                    {
                        Console.WriteLine($"Player {chips.Key}: Is Out.");
                        break;
                    }

                    Console.WriteLine($"\nPlayer {chips.Key}: ${chips.Value}");

                    foreach (string card in playerStacks[chips.Key][1])
                    {
                        Console.WriteLine(card);
                    }

                    if (!aceChecked)
                    {
                        foreach (KeyValuePair<int, List<string>> cardList in playerStacks[chips.Key])
                        {
                            int aceValue = 0;

                            List<string> cards = playerStacks[chips.Key][1];
                            string cardRank = cards[0].Substring(0, cards[0].IndexOf(' '));

                            if (!aceValues.ContainsKey(chips.Key))
                            {
                                aceValues[chips.Key] = new Dictionary<int, Dictionary<int, int>>();
                                aceValues[chips.Key][1] = new Dictionary<int, int>();
                            }


                            if (cardRank == "Ace")
                            {
                                Console.WriteLine("\nWhat value do you want the Ace to be?");

                            aceTry:
                                try
                                {
                                    Console.WriteLine("  1 or 11");
                                    aceValue = Convert.ToInt32(Console.ReadLine());

                                    if (aceValue == 0)
                                    {
                                        goto end;
                                    }

                                    if (aceValue != 1 && aceValue != 11)
                                    {
                                        goto aceTry;
                                    }
                                }
                                catch
                                {
                                    goto aceTry;
                                }

                                aceValues[chips.Key][1][0] = aceValue;
                                aceChecked = true;
                                Debug.WriteLine(aceValues[chips.Key][1][0]);
                            }

                            cardRank = cards[1].Substring(0, cards[1].IndexOf(' '));  
                            if (cardRank == "Ace")
                            {
                                Console.WriteLine("\nWhat value do you want the Ace to be?");

                            aceTry:
                                try
                                {
                                    Console.WriteLine("  1 or 11");
                                    aceValue = Convert.ToInt32(Console.ReadLine());

                                    if (aceValue == 0)
                                    {
                                        goto end;
                                    }

                                    if (aceValue != 1 && aceValue != 11)
                                    {
                                        goto aceTry;
                                    }

                                }
                                catch
                                {
                                    goto aceTry;
                                }

                                aceValues[chips.Key][1][1] = aceValue;
                                aceChecked = true;
                                Debug.WriteLine(aceValues[chips.Key][1][1]);
                            } 
                        }
                    }
                       
                    Console.WriteLine("\nWhat do you want to do?");

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
                    catch
                    {
                        goto userInput;
                    }

                    // Process user input and call functions as needed
                    switch (userInput)
                    {
                        case 1:
                            Hit();
                            break;
                        case 2:
                            // Stand
                            break;
                        case 3:
                            Double();
                            break;
                        case 4:
                            Split();
                            break;
                        default:
                            goto end;
                    }
                }
            }

            end:
            Console.Clear();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void InitGame()
        {
            for (int i = 0; i < playersOutOfGame.Count; i++)
            {
                playersOutOfRound.Add(playersOutOfGame[i]);
            }

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
                    deck.Add($"{rank} of {suit}");
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
                catch
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
                    catch
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
                    if (playersOutOfGame.Contains(chips.Key))
                    {
                        continue;
                    }
                    if (chips.Value > 0)
                    {
                        if (!playerStacks.ContainsKey(chips.Key))
                        { 
                            playerStacks[chips.Key] = new Dictionary<int, List<string>>();
                            playerStacks[chips.Key][1] = new List<string>();
                        }
                         
                        int randNumDict = random.Next(deck.Count);
                        playerStacks[chips.Key][1].Add(deck[randNumDict]);
                        deck.RemoveAt(randNumDict);
                    }
                }

                int randNum = random.Next(deck.Count);

                if (i == 0)
                {
                    dealersHand.Add(deck[randNum]);
                    deck.RemoveAt(randNum);
                }
                else
                {
                    dealersHiddenCard = deck[randNum];
                    deck.RemoveAt(randNum);
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

                        foreach (KeyValuePair<int, int> chips in playersBetts.ToList())
                        {
                            if (chips.Value != -1)
                            {
                                if (!playersOutOfGame.Contains(chips.Key))
                                {
                                    playerMoneyPool[chips.Key] -= chips.Value;
                                    Console.WriteLine($"{(chips.Value == 1 ? $"Player {chips.Key} as lost {chips.Value} dollar" : $"Player {chips.Key} as lost {chips.Value} dollars")}");
                                    InitGame();
                                }
                              
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

                        foreach (KeyValuePair<int, int> chips in playersBetts.ToList())
                        {
                            if (chips.Value != -1)
                            {
                                if (!playersOutOfGame.Contains(chips.Key))
                                {
                                    playerMoneyPool[chips.Key] -= chips.Value;
                                    Console.WriteLine($"{(chips.Value == 1 ? $"Player {chips.Key} as lost {chips.Value} dollar" : $"Player {chips.Key} as lost {chips.Value} dollars")}");
                                    InitGame();
                                }
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
                                    if (!playersOutOfGame.Contains(player.Key))
                                    {
                                        playerMoneyPool[player.Key] += player.Value;
                                        Console.WriteLine($"{(player.Value == 1 ? $"Player {player.Key} as gain {player.Value} dollar" : $"Player {player.Key} as gain {player.Value} dollars")}");
                                        playersOutOfRound.Add(player.Key);
                                    }

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
                                    if (!playersOutOfGame.Contains(player.Key))
                                    {
                                        playerMoneyPool[player.Key] += player.Value;
                                        Console.WriteLine($"{(player.Value == 1 ? $"Player {player.Key} as gain {player.Value} dollar" : $"Player {player.Key} as gain {player.Value} dollars")}");
                                        playersOutOfRound.Add(player.Key);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static void Hit()
        {
            Console.Clear();
            Console.WriteLine("Hit");
            Console.ReadLine();
        }

        static void Double()
        {
            Console.Clear();
            Console.WriteLine("Double");
            Console.ReadLine();
        }

        static void Split()
        {
            Console.Clear();
            Console.WriteLine("Split");
            Console.ReadLine();
        }
    } // Class
} // Namespace
