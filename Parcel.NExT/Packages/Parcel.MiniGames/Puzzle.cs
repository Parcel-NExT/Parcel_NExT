namespace Parcel.MiniGame
{
    /// <summary>
    /// Mini English puzzle game.
    /// </summary>
    public static class PuzzleGame
    {
        private record Entry(string Question, string Answer, string Hint);

        #region States
        private static Entry? CorrectAnswer = null;
        private static string? PreviousAnswer = null;
        private static int Attempts = 0;
        private const string NewGameCommand = "New Puzzle";
        private static readonly Entry[] Puzzles = [
            new Entry("I speak without a mouth and hear without ears. I have no body, but I come alive with wind. What am I?", "An Echo", "A natural phenomenon"),
            new Entry("I’m light as a feather, yet the strongest man can’t hold me for more than 5 minutes. What am I?", "Breath", "Essential for life"),
            new Entry("I can only live where there is light, but I die if the light shines on me. What am I?", "A Shadow", "Darkness"),
            new Entry("The more you take, the more you leave behind. What am I?", "Footsteps", "Think about walking"),
            new Entry("What has keys but can’t open locks?", "A Piano", "Musical instrument"),
            new Entry("What can travel around the world while staying in a corner?", "A Stamp", "Mail-related"),
            new Entry("What has a heart that doesn’t beat?", "An Artichoke", "A type of vegetable"),
            new Entry("I have branches, but no fruit, trunk, or leaves. What am I?", "A Bank", "Financial institution"),
            new Entry("What can fill a room but takes up no space?", "Light", "Illumination"),
            new Entry("I’m found in socks, scarves, and mittens; and often in the paws of playful kittens. What am I?", "Yarn", "Material for knitting"),
            new Entry("What has many teeth but can’t bite?", "A Comb", "Grooming tool"),
            new Entry("What runs all around a backyard, yet never moves?", "A Fence", "Boundary marker"),
            new Entry("What has words, but never speaks?", "A Book", "Source of knowledge"),
            new Entry("What can you catch, but not throw?", "A Cold", "Illness"),
            new Entry("I’m tall when I’m young, and I’m short when I’m old. What am I?", "A Candle", "Source of light"),
            new Entry("What has a neck but no head?", "A Bottle", "Container"),
            new Entry("What gets wetter as it dries?", "A Towel", "Bathroom item"),
            new Entry("What has one eye, but can’t see?", "A Needle", "Sewing tool"),
            new Entry("What has a thumb and four fingers, but isn’t alive?", "A Glove", "Worn on hands"),
            new Entry("What comes once in a minute, twice in a moment, but never in a thousand years?", "The letter M", "Think letters"),
            new Entry("What goes up but never comes down?", "Your Age", "Time-related"),
            new Entry("What has a head, a tail, but does not have a body?", "A Coin", "Currency"),
            new Entry("What gets bigger when more is taken away?", "A Hole", "Absence of matter"),
            new Entry("What begins with T, ends with T, and has T in it?", "A Teapot", "Kitchen item"),
            new Entry("What is full of holes but still holds water?", "A Sponge", "Cleaning tool"),
            new Entry("What has an end but no beginning, a home but no family, and grows without being alive?", "A Road", "Pathway"),
            new Entry("What is always in front of you but can’t be seen?", "The Future", "Time-related"),
            new Entry("What is so fragile that saying its name breaks it?", "Silence", "Quietness"),
            new Entry("What has cities, but no houses; forests, but no trees; and rivers, but no water?", "A Map", "Navigation tool"),
            new Entry("What can you keep after giving to someone?", "Your Word", "A promise"),
            new Entry("What can you break, even if you never pick it up or touch it?", "A Promise", "Commitment"),
            new Entry("What has a bottom at the top?", "Your Legs", "Body part"),
            new Entry("What belongs to you, but others use it more than you do?", "Your Name", "Identity"),
            new Entry("I shave every day, but my beard stays the same. What am I?", "A Barber", "Profession"),
            new Entry("I have a head, a tail, but no body. What am I?", "A Coin", "Currency"),
            new Entry("I’m light as a feather, yet the strongest man can’t hold me for much longer than a minute. What am I?", "Breath", "Essential for life"),
            new Entry("What has keys but can’t open locks?", "A Piano", "Musical instrument"),
            new Entry("What has a ring but no finger?", "A Telephone", "Communication device"),
            new Entry("What comes down but never goes up?", "Rain", "Weather phenomenon"),
            new Entry("What can run but never walks, has a mouth but never talks, has a head but never weeps, has a bed but never sleeps?", "A River", "Body of water"),
            new Entry("What gets sharper the more you use it?", "Your Brain", "Part of your body"),
            new Entry("What has one eye but can’t see?", "A Needle", "Sewing tool"),
            new Entry("What begins with an E but only has one letter?", "An Envelope", "Mail item"),
            new Entry("What goes up but never goes down?", "Your Age", "Time-related"),
            new Entry("What comes once in a minute, twice in a moment, but never in a thousand years?", "The letter M", "Think letters"),
            new Entry("What has a face and two hands but no arms or legs?", "A Clock", "Time-telling device"),
            new Entry("What is so fragile that saying its name breaks it?", "Silence", "Quietness"),
            new Entry("What kind of room has no doors or windows?", "A Mushroom", "Fungi"),
            new Entry("What is always in front of you but can’t be seen?", "The Future", "Time-related")
        ];
        #endregion

        #region Entrance
        /// <summary>
        /// Start a trivia game. Select topic then enter your reply or "New Game" to start new game.
        /// </summary>
        public static string Puzzle(string reply = NewGameCommand)
        {
            if (reply == NewGameCommand || CorrectAnswer == null)
                return NewGame();

            // Passed
            if (reply == CorrectAnswer.Answer)
            {
                CorrectAnswer = null;
                PreviousAnswer = null;
                Attempts = 0;
                return "You are right!";
            }
            else if (Attempts < 3)
            {
                Attempts++;
                PreviousAnswer = reply;
                return $"Wrong answer. Hint: {CorrectAnswer.Hint}";
            }
            else
            {
                string answer = CorrectAnswer.Answer;
                CorrectAnswer = null;
                PreviousAnswer = null;
                Attempts = 0;
                return $"The correct answer is {answer}. \nTry starting a new game!";
            }
        }
        #endregion

        #region Routines
        private static string NewGame()
        {
            Random rnd = new();
            int draw = rnd.Next(Puzzles!.Length);
            CorrectAnswer = Puzzles[draw];
            return CorrectAnswer.Question;
        }
        #endregion
    }
}
