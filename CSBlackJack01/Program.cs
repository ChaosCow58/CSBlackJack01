using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSBlackJack01
{
    internal class Program
    {
        static int numOfPlayers = 1;

        // Number of chips they have
        static int[] numOfChips;

        static string[] suits = new string[4]  { "Hearts", "Diamonds", "Clubs", "Spades" };
        static string[] ranks = new string[13] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

        static List<string> cards = new List<string>();
        // Key is which, value the number betted if -1 they skipped
        static Dictionary<int, int> playersBetts = new Dictionary<int, int>();

        static void Main(string[] args)
        {
            PopulateCards();
            SetPlayerAmount();

            for (int i = 0; i < numOfPlayers; i++) 
            {
                Console.WriteLine($"Player {i + 1}:");
                Console.Write("    How much do you want to bet (-1 to sit out): ");
                int numBetted = Convert.ToInt32(Console.ReadLine());

                while (numBetted > numOfChips[i] || numBetted == 0)
                {
                    Console.Write("    How much do you want to bet (-1 to sit out): ");
                    numBetted = Convert.ToInt32(Console.ReadLine());
                }

                playersBetts[i + 1] = numBetted;
            }

            Deal();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
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
            Console.Write("How many players? ");
            numOfPlayers = Convert.ToInt32(Console.ReadLine());

            numOfChips = new int[numOfPlayers];

            for (int i = 0; i < numOfPlayers; i++) 
            {
                numOfChips[i] = 5;
            }
        }

        static void Deal()
        {
            Console.Clear();
            foreach (KeyValuePair<int, int> chips in playersBetts)
            {
                if (chips.Value == -1)
                {
                    Console.WriteLine($"Player {chips.Key}: Sitted Out");                 
                }
                else 
                {
                    Console.WriteLine($"{(chips.Value == 1 ? $"Player {chips.Key}: {chips.Value} chip" : $"Player {chips.Key}: {chips.Value} chips")}");
                }
            }
        }
    }
}
