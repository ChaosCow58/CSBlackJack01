﻿using System;
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
        static Random random = new Random();

        static int numOfPlayers = 0;
        static int currentPlayer = 0;

        // Player num, money amount
        static Dictionary<int, int> playerMoneyPool = new Dictionary<int, int>();

        static List<int> playersOutOfRound = new List<int>();
        static List<int> playersOutOfGame = new List<int>();

        static string[] suits = new string[4]  { "Hearts", "Diamonds", "Clubs", "Spades" };
        static string[] ranks = new string[13] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

        static List<string> deck = new List<string>();

        // Key is which player, what stack the bet is on ,value the number betted if -1 they skipped
        static Dictionary<int, Dictionary<int, int>> playersBetts = new Dictionary<int,Dictionary<int, int>>();

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
            
            foreach (KeyValuePair<int, Dictionary<int, int>> chips in playersBetts.ToList())
            {
                bool aceChecked = false;
                bool canSplit = true;

                currentPlayer = chips.Key;

                Debug.WriteLine(currentPlayer);

                if (playersOutOfGame.Contains(chips.Key))
                {
                    Console.Clear();
                    Console.WriteLine($"Player {chips.Key} is out of money");
                    Thread.Sleep(1000);
                    continue;
                }

                if (playersBetts[currentPlayer][1] == -1)
                {
                    Console.Clear();
                    Console.WriteLine($"PLayer {currentPlayer} has sitted out.");
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
                    Console.WriteLine($"Player {chips.Key} has ${playerMoneyPool[chips.Key]}\n");

                    Console.ResetColor();

                    if (playersOutOfRound.Contains(chips.Key))
                    {
                        Console.WriteLine($"Player {chips.Key}: Is Out.");
                        break;
                    }

                    PrintCards();

                    if (!aceChecked)
                    {
                        foreach (KeyValuePair<int, List<string>> cardList in playerStacks[chips.Key].ToList())
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

                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("0 - Exit Program");

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Player {chips.Key} has ${playerMoneyPool[chips.Key]}\n");

                        Console.ResetColor();

                        PrintCards();
                    }


                    Console.WriteLine("\nWhat do you want to do?");

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  1 - Hit");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("  2 - Stand");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  3 - Double");

                    if (canSplit)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("  4 - Split");
                    }

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

                            int stackNum = 1;

                            if (playerStacks[currentPlayer].Keys.Count > 1)
                            {
                                Console.Clear();
                                PrintCards();

                                Console.WriteLine("\nWhich stack do you want to hit?");

                            stackTry:
                                try
                                {
                                    stackNum = Convert.ToInt32(Console.ReadLine());

                                    if (stackNum > playerStacks[currentPlayer].Keys.Count)
                                    { 
                                        goto stackTry;
                                    }
                                }
                                catch
                                { 
                                    goto stackTry;
                                }

                                

                                Hit(false, stackNum);
                            }
                            else
                            {
                                Console.Clear();
                                Hit(false, 1);
                            }

                            canSplit = false;
                            // TODO: Check if the player is busted after hitting
                            break;
                        case 2:
                            // Stand
                            break;
                        case 3:
                            int doubleStackNum = 1;

                            if (playerStacks[currentPlayer].Keys.Count > 1)
                            {

                                Console.WriteLine("\nWhich stack do you want to hit?");

                            stackTry:
                                try
                                {
                                    doubleStackNum = Convert.ToInt32(Console.ReadLine());

                                    if (doubleStackNum > playerStacks[currentPlayer].Keys.Count)
                                    {
                                        goto stackTry;
                                    }
                                }
                                catch
                                {
                                    goto stackTry;
                                }

                                Double(doubleStackNum);
                            }
                            else
                            {
                                Double(1);
                            }

                            canSplit = false;

                            break;
                        case 4:
                            if (canSplit)
                            { 
                                Split();
                                canSplit = false;
                            }
                            break;
                        case 0:
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

                while (numBetted == 0 || numBetted > playerMoneyPool[i + 1])
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

                if (!playersBetts.ContainsKey(i + 1))
                {
                    playersBetts[i + 1] = new Dictionary<int, int>();
                }

                playersBetts[i + 1][1] = numBetted;
            }
        }

        static void Deal()
        {

            Console.Clear();
            for (int i = 0;i < 2;i++)
            {
                foreach (KeyValuePair<int, Dictionary<int, int>> chips in playersBetts)
                {
                    if (playersOutOfGame.Contains(chips.Key))
                    {
                        continue;
                    }
                    if (chips.Value[1] > 0)
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

                        foreach (KeyValuePair<int, Dictionary<int, int>> chips in playersBetts.ToList())
                        {
                            if (chips.Value[1] != -1)
                            {
                                if (!playersOutOfGame.Contains(chips.Key))
                                {
                                    playerMoneyPool[chips.Key] -= chips.Value[1];
                                    Console.WriteLine($"{(chips.Value[1] == 1 ? $"Player {chips.Key} as lost {chips.Value[1]} dollar" : $"Player {chips.Key} as lost {chips.Value[1]} dollars")}");
                                }
                              
                            }
                        }
                        InitGame();
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

                        foreach (KeyValuePair<int, Dictionary<int, int>> chips in playersBetts.ToList())
                        {
                            if (chips.Value[1] != -1)
                            {
                                if (!playersOutOfGame.Contains(chips.Key))
                                {
                                    playerMoneyPool[chips.Key] -= chips.Value[1];
                                    Console.WriteLine($"{(chips.Value[1] == 1 ? $"Player {chips.Key} as lost {chips.Value[1]} dollar" : $"Player {chips.Key} as lost {chips.Value[1]} dollars")}");
                                }
                            }
                        }
                       InitGame();
                    }
                }
            }
        }

        static void PlayersCheck()
        {
            foreach (KeyValuePair<int, Dictionary<int, int>> player in playersBetts.ToList())
            {
                if (player.Value[1] == -1)
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

                         
                                if (player.Value[1] != -1)
                                {
                                    if (!playersOutOfGame.Contains(player.Key))
                                    {
                                        playerMoneyPool[player.Key] += player.Value[1];
                                        Console.WriteLine($"{(player.Value[1] == 1 ? $"Player {player.Key} as gain {player.Value[1]} dollar" : $"Player {player.Key} as gain {player.Value[1]} dollars")}");
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


                                if (player.Value[1] != -1)
                                {
                                    if (!playersOutOfGame.Contains(player.Key))
                                    {
                                        playerMoneyPool[player.Key] += player.Value[1];
                                        Console.WriteLine($"{(player.Value[1] == 1 ? $"Player {player.Key} as gain {player.Value[1]} dollar" : $"Player {player.Key} as gain {player.Value[1]} dollars")}");
                                        playersOutOfRound.Add(player.Key);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static void PrintCards()
        {
            foreach (KeyValuePair<int, List<string>> card in playerStacks[currentPlayer].ToList())
            {
                Console.WriteLine($"{(card.Key == 1 ? $"[Stack {card.Key}]: - ${playersBetts[currentPlayer][card.Key]}" : $"\n[Stack {card.Key}]: - ${playersBetts[currentPlayer][card.Key]}")}");
                
                for (int j = 0; j < card.Value.Count; j++)
                {
                    string cardRank = card.Value[j].Substring(0, card.Value[j].IndexOf(' '));

                    if (cardRank == "Ace")
                    {
                        if (aceValues.ContainsKey(currentPlayer))
                        {
                            Console.WriteLine($"{card.Value[j]} - {aceValues[currentPlayer][card.Key][j]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine(card.Value[j]);
                    }
                } 
            }
        }

        static void Hit(bool isMultipleStacks, int numOfStacks)
        {
            int beforeLength = playerStacks[currentPlayer][numOfStacks].Count - 1;
            string cardRank = string.Empty;

            if (!isMultipleStacks)
            {
                playerStacks[currentPlayer][numOfStacks].Add(deck[random.Next(deck.Count)]);
                playerStacks[currentPlayer][numOfStacks].Remove(deck[random.Next(deck.Count)]);

                if (playerStacks[currentPlayer][numOfStacks].Count != beforeLength + 1)
                { 
                    cardRank = playerStacks[currentPlayer][numOfStacks][beforeLength + 1].Substring(0, playerStacks[currentPlayer][numOfStacks][beforeLength + 1].IndexOf(' '));
                }
                
                if (cardRank == "Ace")
                {
                    Console.WriteLine("What value do you want the Ace to be?");

                hitAceTry:
                    try
                    {
                        Console.WriteLine("  1 or 11");
                        int aceValue = Convert.ToInt32(Console.ReadLine());

                        if (aceValue != 1 && aceValue != 11)
                        {
                            goto hitAceTry;
                        }
                        
                        aceValues[currentPlayer][numOfStacks][beforeLength + 1] = aceValue;
                    }
                    catch
                    { 
                        goto hitAceTry;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"You hitted stack {numOfStacks}!");
            }
            else 
            {
                StringBuilder sb = new StringBuilder("You hitted stacks ");

                for (int i = 0; i < numOfStacks; i++)
                {
                    beforeLength = playerStacks[currentPlayer][i + 1].Count;

                    playerStacks[currentPlayer][i + 1].Add(deck[random.Next(deck.Count)]);
                    playerStacks[currentPlayer][i + 1].Remove(deck[random.Next(deck.Count)]);

                    if (playerStacks[currentPlayer][numOfStacks].Count > beforeLength + 1)
                    {
                        cardRank = playerStacks[currentPlayer][numOfStacks][beforeLength + 1].Substring(0, playerStacks[currentPlayer][numOfStacks][beforeLength + 1].IndexOf(' '));
                    }

                    if (cardRank == "Ace")
                    {
                        Console.WriteLine("What value do you want the Ace to be?");

                    hitAceTry:
                        try
                        {
                            Console.WriteLine("  1 or 11");
                            int aceValue = Convert.ToInt32(Console.ReadLine());

                            if (aceValue != 1 && aceValue != 11)
                            {
                                goto hitAceTry;
                            }

                            aceValues[currentPlayer][numOfStacks][beforeLength + 1] = aceValue;
                        }
                        catch
                        {
                            goto hitAceTry;
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Red;

                    sb.Append($"{i + 1}");

                    if (i < numOfStacks - 2)
                    {
                        sb.Append(", ");
                    }
                    else if (i == numOfStacks - 2 && numOfStacks == 2)
                    {
                        sb.Append(" and ");
                    }
                    else if (i == numOfStacks - 2 && numOfStacks > 1)
                    {
                        sb.Append(", and ");
                    }
                }
                
                Console.WriteLine($"{sb}!");

            }

            Console.ResetColor();
            Console.ReadKey();
        }

        static void Double(int stackNum)
        {
            Console.Clear();
  
            playersBetts[currentPlayer][stackNum] *= 2;
            if (playersBetts[currentPlayer][stackNum] > playerMoneyPool[currentPlayer])
            {
                playersBetts[currentPlayer][stackNum] /= 2;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Player {currentPlayer}: Can not double!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Player {currentPlayer}: Bet has been doubled!");
            }

            Console.ResetColor();
            Console.ReadKey();
        }

        static void Split()
        {
            Console.Clear();

            string cardRank1 = playerStacks[currentPlayer][1][0].Substring(0, playerStacks[currentPlayer][1][0].IndexOf(' '));
            string cardRank2 = playerStacks[currentPlayer][1][1].Substring(0, playerStacks[currentPlayer][1][1].IndexOf(' '));

            if (cardRank1 == cardRank2 && playersBetts[currentPlayer][1] * 2 <= playerMoneyPool[currentPlayer])
            {
                if (!playerStacks.ContainsKey(currentPlayer))
                {
                    playerStacks[currentPlayer] = new Dictionary<int, List<string>>();
                }

                if (!playerStacks[currentPlayer].ContainsKey(2))
                {
                    playerStacks[currentPlayer][2] = new List<string>();
                }

                if (!playersBetts.ContainsKey(2))
                {
                    playersBetts[currentPlayer] = new Dictionary<int, int>();
                }

                playerStacks[currentPlayer][2].Add(playerStacks[currentPlayer][1][1]);
                playersBetts[currentPlayer][2] = playersBetts[currentPlayer][1];

                Hit(true, 2);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"PLayer {currentPlayer}: Has splitted!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"PLayer {currentPlayer}: Can not split!");
            }

            Console.ResetColor();
            Console.ReadKey();
        }
    } // Class
} // Namespace
