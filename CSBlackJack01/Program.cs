using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


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

        static string[] suits = new string[4] { "Hearts", "Diamonds", "Clubs", "Spades" };
        static string[] ranks = new string[13] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

        static List<string> deck = new List<string>();

        // Key is which player, what stack the bet is on ,value the number betted if -1 they skipped
        static Dictionary<int, Dictionary<int, int>> playersBetts = new Dictionary<int, Dictionary<int, int>>();

        // Player number, The stack number for that player, the cards for each stack
        static Dictionary<int, Dictionary<int, List<string>>> playerStacks = new Dictionary<int, Dictionary<int, List<string>>>();

        // Player number, Stack number, which card in pile, value
        static Dictionary<int, Dictionary<int, Dictionary<int, int>>> aceValues = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

        // Player number, what stack the score is on ,value the score
        static Dictionary<int, Dictionary<int, int>> playerScore = new Dictionary<int, Dictionary<int, int>>();

        static List<string> dealersHand = new List<string>();
        static string dealersHiddenCard = string.Empty;

        static void Main(string[] args)
        {
            int startUser = 0;
            int round = 0;

            while (startUser != 1)
            {
                Console.Clear();

                Console.WriteLine("===========================");
                Console.Write("|  Welcome to Black");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Jack");
                Console.ResetColor();

                Console.Write("   |\n");

                Console.WriteLine("===========================");

                Console.WriteLine("\nWhat you like to do?");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("(0) - Exit Program");
                Console.ResetColor();

                Console.WriteLine("(1) - Play");
                Console.WriteLine("(2) - Rules");

            beginningTry:
                try
                {
                    startUser = int.Parse(Console.ReadKey().KeyChar.ToString());

                    if (startUser != 1 && startUser != 2 && startUser != 0)
                    {
                        goto beginningTry;
                    }
                }
                catch
                {
                    goto beginningTry;
                }

                Thread.Sleep(50);

                switch (startUser)
                {
                    case 1:
                        break;
                    case 2:
                        Rules();
                        break;
                    default:
                        goto end;
                }

            }

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All players have $1000 to start!\n");
            Console.ResetColor();

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

            while (playersOutOfGame.Count != playerMoneyPool.Count)
            {
                // Display player information and options
                foreach (KeyValuePair<int, Dictionary<int, int>> chips in playersBetts.ToList())
                {
                    bool aceChecked = false;
                    bool canSplit = true;

                    currentPlayer = chips.Key;

                    if (playerMoneyPool[currentPlayer] <= 0)
                    {
                        playerMoneyPool[currentPlayer] = 0;
                        playersOutOfGame.Add(currentPlayer);
                        playersOutOfRound.Add(currentPlayer);
                    }

                    if (playersOutOfGame.Contains(chips.Key))
                    {
                        Console.Clear();
                        Console.WriteLine($"Player {chips.Key} is out of money");
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (playersBetts[currentPlayer][1] == -1)
                    {
                        Console.Clear();                            Console.WriteLine($"Player {chips.Key}
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
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine($"Player {chips.Key} is out of the round!");
                            Console.ResetColor();
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
                            userInput = int.Parse(Console.ReadKey().KeyChar.ToString());
                        }
                        catch
                        {
                            goto userInput;
                        }

                        Thread.Sleep(100);

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

                                CountPlayerCards(false);

                                break;
                            case 2:
                                // Stand
                                break;
                            case 3:
                                int doubleStackNum = 1;

                                if (playerStacks[currentPlayer].Keys.Count > 1)
                                {
                                    Console.Clear();
                                    PrintCards();

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
                            default:
                                goto end;
                        }
                    }
                }

                CountPlayerCards(true);
                DealersHand();

                round++;

                Console.Clear();

                Console.WriteLine($"Round {round} has been completed!");
                Console.WriteLine("Press an key to contuine to the next round!");

                Console.ReadKey();

                Console.Clear();

                InitGame();
            }

        end:
            Console.Clear();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void InitGame()
        {
            foreach (KeyValuePair<int, int> player in playerMoneyPool)
            {
                if (playerMoneyPool[player.Key] <= 0)
                {
                    playerMoneyPool[player.Key] = 0;
                    playersOutOfGame.Add(player.Key);
                }
            }

            for (int i = 0; i < playersOutOfGame.Count; i++)
            {
                playersOutOfRound.Add(playersOutOfGame[i]);
            }

            playersOutOfRound.Clear();
            playerStacks.Clear();
            dealersHand.Clear();
            dealersHiddenCard = string.Empty;

            deck.Clear();

            PopulateCards();

            SetPlayerAmount();

            Deal();

            DealerCheck();
            PlayersCheck();
        }

        /// <summary>
        /// The function "PopulateCards" populates a deck of cards by iterating through each suit and rank
        /// and adding the corresponding card to the deck.
        /// </summary>
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

        static void Rules()
        {
            int rulesInput = -1;

            Console.Clear();

            while (rulesInput != 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("(0) - Back");
                Console.ResetColor();

                Console.WriteLine("1. \x1b[1mDoubling Down:\x1b[0m");
                Console.WriteLine("   - \x1b[1mWhen:\x1b[0m You can double down after being dealt the first two cards or any split stacks.");
                Console.WriteLine("   - \x1b[1mHow:\x1b[0m Place an additional bet (equal to your original bet).");
                Console.WriteLine("   - \x1b[1mStrategy:\x1b[0m Can split as much as you want as long you have enough money.");

                Console.WriteLine("\n2. \x1b[1mHitting:\x1b[0m");
                Console.WriteLine("   - \x1b[1mWhen:\x1b[0m After receiving the first two cards, choose to 'hit' and get additional cards.");
                Console.WriteLine("   - \x1b[1mHow:\x1b[0m Continue hitting until you decide to stand, reach 21, or bust (exceed 21).");
                Console.WriteLine("   - \x1b[1mStrategy:\x1b[0m Typically hit when hand is below 17.");

                Console.WriteLine("\n3. \x1b[1mSplitting:\x1b[0m");
                Console.WriteLine("   - \x1b[1mWhen:\x1b[0m If first two cards are of the same rank, you can split them into two hands.");
                Console.WriteLine("   - \x1b[1mHow:\x1b[0m Place an additional bet for the second hand, and play each hand independently.");
                Console.WriteLine("   - \x1b[1mStrategy:\x1b[0m Often split pairs of 8s and Aces. Avoid splitting 10s, 5s, and 4s.");

                rulesInput = int.Parse(Console.ReadKey().KeyChar.ToString());

                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// The function allows players to set the amount they want to bet, with the option to sit out by
        /// entering -1.
        /// </summary>
        static void SetPlayerAmount()
        {
            int numBetted;

            for (int i = 0; i < numOfPlayers; i++)
            {
                Console.Write($"Player {i + 1}: - ");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"${playerMoneyPool[i + 1]}\n");
                Console.ResetColor();

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
            for (int i = 0; i < 2; i++)
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

        /// <summary>
        /// The function `DealerCheck` checks if the dealer has a blackjack based on their hand and hidden card.
        /// </summary>
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

       /// <summary>
       /// The function "PlayersCheck" checks if any player has a blackjack based on their card ranks
       /// and updates their money pool accordingly.
       /// </summary>
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
        
       /// <summary>
       /// The function "Hit" adds a card to a player's stack(s) and removes a card from the deck, and
       /// also handles the case when an Ace card is drawn.
       /// </summary>
       /// <param name="isMultipleStacks">A boolean value indicating whether the player is hitting
       /// multiple stacks or not.</param>
       /// <param name="numOfStacks">The `numOfStacks` parameter represents the number of stacks that
       /// the player wants to hit. It is used to determine how many stacks the player will hit in the
       /// `Hit` method.</param>
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
                Console.WriteLine($"Player {currentPlayer} hit stack {numOfStacks}!");
            }
            else
            {
                StringBuilder sb = new StringBuilder($"Player {currentPlayer} hit stacks ");

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
            Thread.Sleep(1000);
        }

     /// <summary>
     /// The function "Double" allows a player to double their bet on a specific stack if they have
     /// enough money in their money pool.
     /// </summary>
     /// <param name="stackNum">The parameter `stackNum` represents the stack number that the player
     /// wants to double their bet on.</param>
        static void Double(int stackNum)
        {
            Console.Clear();

            if (playersBetts[currentPlayer][stackNum] * 2 > playerMoneyPool[currentPlayer])
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Player {currentPlayer} can not double!");
            }
            else
            {
                playersBetts[currentPlayer][stackNum] *= 2;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Player {currentPlayer} has doubled stack {stackNum}!");
            }

            Console.ResetColor();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// The Split function checks if a player's two cards have the same rank and if they have enough
        /// money to split, and if so, it splits the cards into two separate stacks and updates the bet
        /// accordingly.
        /// </summary>
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
                playerStacks[currentPlayer][1].Remove(playerStacks[currentPlayer][1][1]);

                playersBetts[currentPlayer][2] = playersBetts[currentPlayer][1];

                Hit(true, 2);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"PLayer {currentPlayer} has splitted!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"PLayer {currentPlayer} can not split!");
            }

            Console.ResetColor();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// The function "CountPlayerCards" calculates the score of each player's cards and checks if any player
        /// has busted.
        /// </summary>
        /// <param name="isEndOfRound">The parameter `isEndOfRound` is a boolean value that indicates whether it
        /// is the end of the round or not. It is used to determine whether to skip players other than the
        /// current player when calculating the player scores. If `isEndOfRound` is `true`, the scores for
        /// all</param>
        static void CountPlayerCards(bool isEndOfRound)
        {
            foreach (KeyValuePair<int, Dictionary<int, List<string>>> playerEntry in playerStacks)
            {
                int currentPlayer1 = playerEntry.Key;

                if (!isEndOfRound && currentPlayer1 != currentPlayer)
                {
                    continue; // Skip players other than the current player when not at the end of the round
                }

                foreach (KeyValuePair<int, List<string>> card in playerEntry.Value)
                {
                    if (!playerScore.ContainsKey(currentPlayer1))
                    {
                        playerScore[currentPlayer1] = new Dictionary<int, int>();
                    }

                    if (!playerScore[currentPlayer1].ContainsKey(card.Key))
                    {
                        playerScore[currentPlayer1][card.Key] = 0;
                    }

                    for (int j = 0; j < card.Value.Count; j++)
                    {
                        string cardRank = card.Value[j].Substring(0, card.Value[j].IndexOf(' '));

                        if (cardRank == "King" || cardRank == "Queen" || cardRank == "Jack" || cardRank == "10")
                        {
                            playerScore[currentPlayer1][card.Key] += 10;
                        }
                        else if (cardRank == "Ace")
                        {
                            playerScore[currentPlayer1][card.Key] += aceValues[currentPlayer1][card.Key][j];
                        }
                        else
                        {
                            playerScore[currentPlayer1][card.Key] += int.Parse(cardRank);
                        }
                    }
                }

                Debug.WriteLine(playerScore[currentPlayer1][1]);

                if (playerScore[currentPlayer1][1] > 21)
                {
                    Console.Clear();
                    PrintCards();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nPlayer {currentPlayer1} has busted!");

                    playerMoneyPool[currentPlayer1] -= playersBetts[currentPlayer][1];

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Player {currentPlayer1} has lost {playersBetts[currentPlayer1][1]} dollars.");
                    Console.ResetColor();

                    playersOutOfRound.Add(currentPlayer1);
                }
                else if (playerScore[currentPlayer1].ContainsKey(2) && playerScore[currentPlayer1][2] > 21)
                {
                    Console.Clear();
                    PrintCards();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nPlayer {currentPlayer1} has busted!");

                    playerMoneyPool[currentPlayer1] -= playersBetts[currentPlayer1][2];

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Player {currentPlayer1} has lost {playersBetts[currentPlayer1][2]} dollars.");
                    Console.ResetColor();

                    playersOutOfRound.Add(currentPlayer1);
                }

                if (!isEndOfRound)
                {
                    playerScore[currentPlayer1][1] = 0;

                    if (playerScore[currentPlayer1].ContainsKey(2))
                    {
                        playerScore[currentPlayer1][2] = 0;
                    }
                }
            }

            Console.ReadKey();
        }

       /// <summary>
       /// The `DealersHand` function calculates the score of the dealer's hand in a blackjack game and
       /// determines the outcome of the round for each player.
       /// </summary>
        static void DealersHand()
        {
            int dealersScore = 0;

            dealersHand.Add(dealersHiddenCard);

            while (dealersHand.Count < 4 && dealersScore < 16)
            {
                dealersScore = 0;

                foreach (string dealerCard in dealersHand)
                {
                    string cardRank = dealerCard.Substring(0, dealerCard.IndexOf(' '));


                    if (cardRank == "King" || cardRank == "Queen" || cardRank == "Jack" || cardRank == "10")
                    {
                        dealersScore += 10;
                    }

                    if (ConatinsNumber2To9(cardRank))
                    {
                        dealersScore += int.Parse(cardRank);
                    }

                    if (cardRank == "Ace")
                    {
                        Debug.WriteLine("Ace");
                        if (dealersScore + 11 < 21)
                        {
                            Debug.WriteLine("Ace 1");
                            dealersScore++;
                        }
                        else if (dealersScore + 1 < 21)
                        {
                            Debug.WriteLine("Ace 11");
                            dealersScore += 11;
                        }
                        else
                        {
                            Debug.WriteLine("Ace Bust");
                            goto dealerEnd;
                        }
                    }

                }

                if (dealersScore < 16)
                {
                    int randNum = random.Next(deck.Count);

                    dealersHand.Add(deck[randNum]);
                    deck.RemoveAt(randNum);
                }

                Console.Clear();

                Console.WriteLine("Dealer's Hand:");
                foreach (string dealerCard in dealersHand)
                {
                    Console.WriteLine($"  {dealerCard}");
                }

            }

        dealerEnd:
            if (dealersScore > 21)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nThe Dealer has busted!");

                foreach (KeyValuePair<int, Dictionary<int, int>> player in playersBetts.ToList())
                {
                    if (playersOutOfRound.Contains(player.Key))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"Player {player.Key} is out of the round!");
                        continue;
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Player {player.Key} has gained ${playersBetts[player.Key][1]} on stack 1.");
                    playerMoneyPool[player.Key] += playersBetts[player.Key][1];
                    

                    if (playerScore[player.Key].ContainsKey(2))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Player {player.Key} has gained ${playersBetts[player.Key][2]} on stack 2.");
                        playerMoneyPool[player.Key] += playersBetts[player.Key][2];
                    }
                }

                Console.ResetColor();
            }
            else
            {
                foreach (KeyValuePair<int, Dictionary<int, int>> player in playersBetts.ToList())
                {
                    if (playersOutOfRound.Contains(player.Key))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"Player {player.Key} is out of the round!");
                        continue;
                    }

                    if (playerScore[player.Key][1] > dealersScore)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Player {player.Key} has gained ${playersBetts[player.Key][1]} on stack 1.");
                        playerMoneyPool[player.Key] += playersBetts[player.Key][1];
                    }
                    else if (playerScore[player.Key][1] < dealersScore)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"Player {player.Key} has lost ${playersBetts[player.Key][1]} on stack 1.");
                        playerMoneyPool[player.Key] -= playersBetts[player.Key][1];
                    }
                    else if (playerScore[player.Key].ContainsKey(2) && playerScore[player.Key][2] > dealersScore)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Player {player.Key} has gained ${playersBetts[player.Key][2]} on stack 2.");
                        playerMoneyPool[player.Key] += playersBetts[player.Key][2];
                    }
                    else if (playerScore[player.Key].ContainsKey(2) && playerScore[player.Key][2] < dealersScore)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"Player {player.Key} has lost ${playersBetts[player.Key][2]} on stack 2.");
                        playerMoneyPool[player.Key] -= playersBetts[player.Key][2];
                    }
                    else if (playerScore[player.Key][1] == dealersScore)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Player {player.Key} has tied on stack 1.");
                    }
                    else if (playerScore[player.Key].ContainsKey(2) && playerScore[player.Key][2] == dealersScore)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Player {player.Key} has tied on stack 2.");
                    }
                }

                Console.ResetColor();
            }

            Console.ReadKey();

        }

        static bool ConatinsNumber2To9(string input)
        {
            Regex regex = new Regex("[2-9]");
            return regex.IsMatch(input);
        }
    } // Class
} // Namespace
