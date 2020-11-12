namespace Veneka.Indigo.CardManagement.objects
{
    public class AvailableCardsInfo
    {
        private readonly string _biggestCardNumber;
        private readonly string _cardBin;
        private readonly string _cardProgram;
        private readonly int _numberOfCards;
        private readonly string _smallestCardNumber;


        public AvailableCardsInfo(string smallestCardNumber, string biggestCardNumber, string cardProgram,
                                  string cardBin, int numberOfCards)
        {
            _smallestCardNumber = smallestCardNumber;
            _biggestCardNumber = biggestCardNumber;
            _cardProgram = cardProgram;
            _cardBin = cardBin;
            _numberOfCards = numberOfCards;
        }

        public string SmallestCardNumber
        {
            get { return _smallestCardNumber; }
        }

        public string BiggestCardNumber
        {
            get { return _biggestCardNumber; }
        }

        public string CardProgram
        {
            get { return _cardProgram; }
        }

        public string CardBin
        {
            get { return _cardBin; }
        }

        public int NumberOfCards
        {
            get { return _numberOfCards; }
        }
    }
}