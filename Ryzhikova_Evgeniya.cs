using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ryzhikova_Evgeniya
{
    public enum Colors
    {
        Red = 0, Yellow, Blue, White, Green
    }

    public enum Ranks
    {
        One = 1, Two, Three, Four, Five
    }

    public class Card
    {
        public Colors Color { get; private set; }
        public Ranks Rank { get; private set; }
        public bool IsKnowColor { get; set; }
        public bool IsKnowRank { get; set; }

        public SortedSet<Colors> invalidColors;
        public SortedSet<Ranks> invalidRanks;

        private static Dictionary<String, Colors> colorOf = new Dictionary<String, Colors> 
        {
                { "R", Colors.Red },
                { "G", Colors.Green },
                { "B", Colors.Blue },
                { "W", Colors.White },
                { "Y", Colors.Yellow },
                { "Red", Colors.Red },
                { "Green", Colors.Green },
                { "Blue", Colors.Blue },
                { "White", Colors.White },
                { "Yellow", Colors.Yellow }
        };

        private static Dictionary<String, Ranks> rankOf = new Dictionary<String, Ranks> 
        {
                { "1", Ranks.One },
                { "2", Ranks.Two },
                { "3", Ranks.Three },
                { "4", Ranks.Four },
                { "5", Ranks.Five }
        };

        public Card(Colors color, Ranks rank)
        {
            Color = color;
            Rank = rank;
            IsKnowColor = false;
            IsKnowRank = false;
            invalidColors = new SortedSet<Colors>();
            invalidRanks = new SortedSet<Ranks>();
        }

        public Card(String color, String rank)
        {
            Color = colorOf[color];
            Rank = rankOf[rank];
            IsKnowColor = false;
            IsKnowRank = false;
            invalidColors = new SortedSet<Colors>();
            invalidRanks = new SortedSet<Ranks>();
        }

        public bool IsKnownCard()
        {
            return (IsKnowRank == true && IsKnowColor == true);
        }

        public static Colors ColorFromString(string color)
        {
            return colorOf[color];
        }

        public static Ranks RankFromString(string rank)
        {
            return rankOf[rank];
        }

        public override String ToString()
        {
            return String.Format("{0}{1}", Color.ToString(), Rank);
        }
    }

    public class Deck
    {
        private Queue<Card> deck;

        public int CountOfCards { get { return deck.Count; } private set {} }

        public Deck()
        {
            deck = new Queue<Card>();
            CountOfCards = 0;
        }

        public void AddCard(Card currentCard)
        {
            deck.Enqueue(currentCard);
        }

        public Card TakeCard()
        {
            return (deck.Dequeue());
        }
    }

    public class Player
    {
        private List<Card> playersCards;

        public Player()
        {
            playersCards = new List<Card>();
        }

        public Card GetCardByNumber(int numberOfCard)
        {
            return playersCards[numberOfCard];
        }

        public void LayOutCard(int cardNumber)
        {
            playersCards.RemoveAt(cardNumber);
        }

        public void AddCardFromDeck(Card currentCard)
        {
            playersCards.Add(currentCard);
        }

        public void DiscloseCardRank(int cardNumber)
        {
            playersCards[cardNumber].IsKnowRank = true;
        }

        public void DiscloseCardColor(int cardNumber)
        {
            playersCards[cardNumber].IsKnowColor = true;
        }

        public void SupplementInvalidColors(Colors color, List<int> cardsNumbers)
        {
            foreach (var number in cardsNumbers)
            {
                playersCards[number].invalidColors.Add(color);
                if (playersCards[number].invalidColors.Count == 4)
                    playersCards[number].IsKnowColor = true;
            }
        }

        public void SupplementInvalidRanks(Ranks rank, List<int> cardsNumbers)
        {
            foreach (var number in cardsNumbers)
            {
                playersCards[number].invalidRanks.Add(rank);
                if (playersCards[number].invalidRanks.Count == 4)
                    playersCards[number].IsKnowRank = true;
            }
        }

        public bool IsKnownCard(int cardNumber)
        {
            return (playersCards[cardNumber].IsKnownCard() == true);
        }

        public int CountOfCardsWithColor(Colors color)
        {
            int result = 0;

            foreach (Card card in playersCards)
            {
                if (card.Color == color)
                    ++result;
            }

            return result;
        }

        public int CountOfCardsWithRank(Ranks rank)
        {
            int result = 0;

            foreach (Card card in playersCards)
            {
                if (card.Rank == rank)
                    ++result;
            }

            return result;
        }
    }

    public class Table
    {
        public bool IsFull { get; private set; }
        public int CountOfCards { get; private set; }

        Dictionary<Colors, int> rankOfLastCardWithColor;

        public Table()
        {
            rankOfLastCardWithColor = new Dictionary<Colors, int>
            {
                { Colors.Red, 0 },
                { Colors.Green, 0 },
                { Colors.Blue, 0 },
                { Colors.White, 0 },
                { Colors.Yellow, 0 }
            };

            IsFull = false;
            CountOfCards = 0;
        }

        public void PutCard(Card currentCard)
        {
            rankOfLastCardWithColor[currentCard.Color] += 1;
            ++CountOfCards;

            if (CountOfCards == 25)
                IsFull = true;
        }

        public int GetRankOfLastCardWithColor(Colors color)
        {
            return (rankOfLastCardWithColor[color]);
        }

        public SortedSet<Colors> InvalidColorsForRank(Ranks rank)
        {
            SortedSet<Colors> invalidColors = new SortedSet<Colors>();

            foreach (var pair in rankOfLastCardWithColor)
            {
                if (pair.Value != (int)rank - 1)
                    invalidColors.Add(pair.Key);
            }

            return invalidColors;
        }
    }

    public class Game
    {
        public int NumberOfLastMove { get; private set; }
        public int CountOfRiskyMoves { get; private set; }
        public int CountOfPlayedCards { get; private set; }
        public bool IsFinished { get; private set; }

        private int countOfPlayers;
        private int currentPlayerNumber;
        private int receivingPlayerNumber;
        private Deck gamingDeck;
        private Table gamingTable;
        private List<Player> players;

        public Game()
        {
            gamingDeck = null;
            gamingTable = null;
            players = null;
        }

        public void StartNewGame(int countOfGamePlayers)
        {
            NumberOfLastMove = 0;
            CountOfRiskyMoves = 0;
            CountOfPlayedCards = 0;
            currentPlayerNumber = 0;
            receivingPlayerNumber = 1;
            countOfPlayers = countOfGamePlayers;
            IsFinished = false;

            gamingDeck = new Deck();
            gamingTable = new Table();
            players = new List<Player>();

            for (int i = 0; i < countOfPlayers; ++i)
                players.Add(new Player());
        }

        public void AddCardToDeck(Card currentCard)
        {
            gamingDeck.AddCard(currentCard);
        }

        public void DealCardsToPlayers()
        {
            for (int i = 0; i < countOfPlayers; ++i)
                for (int j = 0; j < 5; ++j)
                {
                    Card testCard = gamingDeck.TakeCard();
                    players[i].AddCardFromDeck(testCard);
                }
        }

        private bool IsThisMoveRisky(int numberOfPlayedCard)
        {
            Card playedCard = players[currentPlayerNumber].GetCardByNumber(numberOfPlayedCard);
            SortedSet<Colors> invalidColorsForCardRank = gamingTable.InvalidColorsForRank(playedCard.Rank);

            return (!players[currentPlayerNumber].IsKnownCard(numberOfPlayedCard)
                && !((playedCard.invalidColors.Intersect(invalidColorsForCardRank).SequenceEqual(invalidColorsForCardRank)) 
                && playedCard.IsKnowRank));
        }

        private bool IsCardCorrectlyPlayed(int numberOfPlayedCard)
        {
            Card playedCard = players[currentPlayerNumber].GetCardByNumber(numberOfPlayedCard);
            return (gamingTable.GetRankOfLastCardWithColor(playedCard.Color) == (int)playedCard.Rank - 1);
        }

        private bool IsReportedColorAreTrue(Colors color, int numberOfCard)
        {
            return (players[receivingPlayerNumber].GetCardByNumber(numberOfCard).Color == color);
        }

        private bool IsReportedForAllCardsWithColor(Colors color, int countOfReportedNumbers)
        {
            return (players[receivingPlayerNumber].CountOfCardsWithColor(color) == countOfReportedNumbers);
        }

        private bool IsReportedRankAreTrue(Ranks rank, int numberOfCard)
        {
            return (players[receivingPlayerNumber].GetCardByNumber(numberOfCard).Rank == rank);
        }

        private bool IsReportedForAllCardsWithRank(Ranks rank, int countOfReportedNumbers)
        {
            return (players[receivingPlayerNumber].CountOfCardsWithRank(rank) == countOfReportedNumbers);
        }

        private void SendMoveToNextPlayer()
        {
            currentPlayerNumber = (currentPlayerNumber + 1) % countOfPlayers;
            receivingPlayerNumber = (receivingPlayerNumber + 1) % countOfPlayers;
        }

        public void PlayCard(int numberOfPlayedCard)
        {
            ++NumberOfLastMove;

            if (IsCardCorrectlyPlayed(numberOfPlayedCard))
            {
                ++CountOfPlayedCards;

                if (IsThisMoveRisky(numberOfPlayedCard))
                    ++CountOfRiskyMoves;

                gamingTable.PutCard(players[currentPlayerNumber].GetCardByNumber(numberOfPlayedCard));
                players[currentPlayerNumber].LayOutCard(numberOfPlayedCard);
                players[currentPlayerNumber].AddCardFromDeck(gamingDeck.TakeCard());

                IsFinished = gamingTable.IsFull || gamingDeck.CountOfCards == 0;
            }
            else
                IsFinished = true;

            SendMoveToNextPlayer();
        }

        public void DropCard(int numberOfDroppedCard)
        {
            ++NumberOfLastMove;

            players[currentPlayerNumber].LayOutCard(numberOfDroppedCard);
            players[currentPlayerNumber].AddCardFromDeck(gamingDeck.TakeCard());

            IsFinished = gamingDeck.CountOfCards == 0;
            SendMoveToNextPlayer();
        }

        public void TellColor(Colors color, List<int> numbersOfCards)
        {
            ++NumberOfLastMove;

            foreach (int number in numbersOfCards)
            {
                IsFinished = !(IsReportedColorAreTrue(color, number));
                players[receivingPlayerNumber].DiscloseCardColor(number);
            }

            if (!IsFinished)
                IsFinished = !IsReportedForAllCardsWithColor(color, numbersOfCards.Count);

            List<int> allNumbers = new List<int> { 0, 1, 2, 3, 4 };
            players[receivingPlayerNumber].SupplementInvalidColors(color, allNumbers.Except(numbersOfCards).ToList());
            SendMoveToNextPlayer();
        }

        public void TellRank(Ranks rank, List<int> numbersOfCards)
        {
            ++NumberOfLastMove;

            foreach (int number in numbersOfCards)
            {
                IsFinished = !(IsReportedRankAreTrue(rank, number));
                players[receivingPlayerNumber].DiscloseCardRank(number);
            }

            if (!IsFinished)
                IsFinished = !IsReportedForAllCardsWithRank(rank, numbersOfCards.Count);

            List<int> allNumbers = new List<int> { 0, 1, 2, 3, 4 };
            players[receivingPlayerNumber].SupplementInvalidRanks(rank, allNumbers.Except(numbersOfCards).ToList());
            SendMoveToNextPlayer();
        }
    }

    class Program
    {
        static Regex findStart = new Regex(@"((?<color>R|G|B|Y|W)(?<rank>1|2|3|4|5))+", RegexOptions.Compiled);
        static Regex findPlay = new Regex(@"Play card (?<number>0|1|2|3|4)");
        static Regex findDrop = new Regex(@"Drop card (?<number>0|1|2|3|4)");
        static Regex regExpTellColor = new Regex(@"Tell color (?<color>Red|Green|Blue|White|Yellow) for cards( (?<number>[0-4]))+", RegexOptions.Compiled);
        static Regex regExpTellRank = new Regex(@"Tell rank (?<rank>[1-5]) for cards( (?<number>[0-4]))+", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            Game game = new Game();
            String command;

            while (true)
            {
                command = Console.ReadLine();
                if (command == null)
                    break;

                if (command.Contains(@"Start new game"))
                {
                    game.StartNewGame(2);

                    foreach (Match obj in findStart.Matches(command))
                    {
                        Card currentCard = new Card(obj.Groups["color"].Value, obj.Groups["rank"].Value);
                        game.AddCardToDeck(currentCard);
                    }

                    game.DealCardsToPlayers();
                }

                else if (!game.IsFinished)
                {
                    Match mth = Match.Empty;

                    if ((mth = findPlay.Match(command)) != Match.Empty)
                    {
                        int numberOfCard = int.Parse(mth.Groups["number"].Value);
                        game.PlayCard(numberOfCard);
                    }

                    else if ((mth = findDrop.Match(command)) != Match.Empty)
                    {
                        int numberOfCard = int.Parse(mth.Groups["number"].Value);
                        game.DropCard(numberOfCard);
                    }

                    else if ((mth = regExpTellColor.Match(command)) != Match.Empty) //command.Contains(@"Tell color"))
                    {
                        Colors color = Card.ColorFromString(mth.Groups["color"].Value);
                        List<int> numbersOfCards = new List<int>();

                        foreach (var objNumber in mth.Groups["number"].Captures)
                        {
                            numbersOfCards.Add(int.Parse(objNumber.ToString()));
                        }

                        game.TellColor(color, numbersOfCards);
                    }

                    else if ((mth = regExpTellRank.Match(command)) != Match.Empty)
                    {
                        Ranks rank = Card.RankFromString(mth.Groups["rank"].Value);
                        List<int> numbersOfCards = new List<int>();

                        foreach (var objNumber in mth.Groups["number"].Captures)
                        {
                            numbersOfCards.Add(int.Parse(objNumber.ToString()));
                        }

                        game.TellRank(rank, numbersOfCards);
                    }

                    if (game.IsFinished)
                    {
                        Console.WriteLine("Turn: {0}, cards: {1}, with risk: {2}",
                            game.NumberOfLastMove, game.CountOfPlayedCards, game.CountOfRiskyMoves);
                    }
                }
            }
        }
    }
}
