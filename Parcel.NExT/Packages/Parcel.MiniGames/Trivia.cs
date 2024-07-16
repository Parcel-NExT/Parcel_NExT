namespace Parcel.MiniGame
{
    /// <summary>
    /// Mini English trivia game.
    /// </summary>
    public static class TriviaGame
    {
        // Entry type
        private record Entry(string Question, string Answer, string Elaboration); // Elaboration provides some additional information of interest

        #region States
        private static Entry? CorrectAnswer = null;
        private static string? PreviousAnswer = null;
        private static int Attempts = 0;
        private const string NewGameCommand = "New Game";

        // From topic to questions
        private static readonly Dictionary<string, Entry[]> QuestionLibrary = new()
        {
            { "General Knowledge", new Entry[] {
                new("Who, in 1903, was the first woman to win a Nobel Prize?", "Marie Curie", "Maria Salomea Skłodowska-Curie, known simply as Marie Curie, was a Polish and naturalised-French physicist and chemist who conducted pioneering research on radioactivity."),
                new("What is the tallest mountain in the world?", "Mount Everest", "Mount Everest is Earth's highest mountain above sea level, located in the Mahalangur Himal sub-range of the Himalayas."),
                new("What is the capital of France?", "Paris", "Paris is the capital and most populous city of France, known for its art, fashion, and culture.")
            } },
            { "United States History", new Entry[] {
                new("Who's the first president of the United States?", "George Washington", "On April 30, 1789, George Washington, standing on the balcony of Federal Hall on Wall Street in New York, took his oath of office as the first President of the United States."),
                new("What year did the United States declare independence?", "1776", "The United States declared its independence from Britain on July 4, 1776."),
                new("Who was the main author of the Declaration of Independence?", "Thomas Jefferson", "Thomas Jefferson, the third President of the United States, was the principal author of the Declaration of Independence.")
            } },
            { "Animal World", new Entry[] {
                new("What's the largest land animal in Australia?", "Red Kangaroo", "The Red Kangaroo is Australia's largest native land mammal."),
                new("Which bird is known for its impressive ability to mimic sounds, including human speech?", "Lyrebird", "The Lyrebird is known for its extraordinary ability to mimic natural and artificial sounds from its environment."),
                new("What is the fastest land animal?", "Cheetah", "The cheetah is the fastest land animal, capable of running up to 75 miles per hour.")
            } },
            { "Science", new Entry[] {
                new("What is the chemical symbol for water?", "H2O", "Water is composed of two hydrogen atoms and one oxygen atom."),
                new("Who developed the theory of relativity?", "Albert Einstein", "Albert Einstein was a German-born theoretical physicist who developed the theory of relativity, one of the two pillars of modern physics."),
                new("What planet is known as the Red Planet?", "Mars", "Mars is known as the Red Planet due to its reddish appearance, caused by iron oxide on its surface.")
            } },
            { "Literature", new Entry[] {
                new("Who wrote 'Romeo and Juliet'?", "William Shakespeare", "William Shakespeare was an English playwright, widely regarded as one of the greatest writers in the English language."),
                new("What is the title of the first Harry Potter book?", "Harry Potter and the Philosopher's Stone", "The first book in the Harry Potter series by J.K. Rowling is 'Harry Potter and the Philosopher's Stone,' known as 'Harry Potter and the Sorcerer's Stone' in the United States."),
                new("Who wrote '1984'?", "George Orwell", "George Orwell, born Eric Arthur Blair, was an English novelist and essayist, known for his works '1984' and 'Animal Farm'.")
            } },
            { "Geography", new Entry[] {
                new("What is the largest desert in the world?", "Sahara Desert", "The Sahara Desert is the largest hot desert in the world, covering much of North Africa."),
                new("Which country has the largest population?", "China", "China has the largest population of any country in the world, with over 1.4 billion people."),
                new("What is the capital city of Japan?", "Tokyo", "Tokyo is the capital city of Japan and one of the most populous metropolitan areas in the world.")
            } },
            { "Movies", new Entry[] {
                new("Who directed the movie 'Titanic'?", "James Cameron", "James Cameron is a Canadian filmmaker known for directing 'Titanic,' which became the highest-grossing film of its time."),
                new("Which movie features the song 'Let It Go'?", "Frozen", "'Let It Go' is a song from Disney's animated movie 'Frozen,' released in 2013."),
                new("What is the highest-grossing film of all time?", "Avatar", "As of 2023, 'Avatar,' directed by James Cameron, is the highest-grossing film of all time.")
            } },
            { "Music", new Entry[] {
                new("Who is known as the 'King of Pop'?", "Michael Jackson", "Michael Jackson is known as the 'King of Pop' for his significant contributions to music, dance, and popular culture."),
                new("What band was Freddie Mercury the lead singer of?", "Queen", "Freddie Mercury was the lead vocalist of the British rock band Queen."),
                new("Who wrote the opera 'The Marriage of Figaro'?", "Wolfgang Amadeus Mozart", "Wolfgang Amadeus Mozart, an influential composer of the Classical era, wrote 'The Marriage of Figaro'.")
            } },
            { "Technology", new Entry[] {
                new("What does 'HTTP' stand for?", "HyperText Transfer Protocol", "HTTP stands for HyperText Transfer Protocol, which is the foundation of data communication for the World Wide Web."),
                new("Who is the founder of Microsoft?", "Bill Gates", "Bill Gates, along with Paul Allen, founded Microsoft in 1975."),
                new("What year was the first iPhone released?", "2007", "The first iPhone, created by Apple Inc., was released on June 29, 2007.")
            } },
            { "Sports", new Entry[] {
                new("Which country won the FIFA World Cup in 2018?", "France", "France won the FIFA World Cup in 2018, held in Russia."),
                new("Who holds the record for the most home runs in a single MLB season?", "Barry Bonds", "Barry Bonds holds the record for the most home runs in a single Major League Baseball season, with 73 home runs in 2001."),
                new("What sport is known as 'the beautiful game'?", "Soccer", "Soccer, also known as football, is often referred to as 'the beautiful game'.")
            } },
            { "History", new Entry[] {
                new("In which year did the Titanic sink?", "1912", "The RMS Titanic sank on April 15, 1912, after hitting an iceberg during her maiden voyage."),
                new("Who was the first man to step on the moon?", "Neil Armstrong", "Neil Armstrong became the first man to step on the moon on July 20, 1969, during NASA's Apollo 11 mission."),
                new("Which war was fought between the North and South regions in the United States?", "The Civil War", "The American Civil War was fought between the Northern states (Union) and Southern states (Confederacy) from 1861 to 1865.")
            } },
            { "Art", new Entry[] {
                new("Who painted the Mona Lisa?", "Leonardo da Vinci", "Leonardo da Vinci, an Italian Renaissance artist, painted the Mona Lisa, one of the most famous portraits in the world."),
                new("What is the art movement associated with artists like Claude Monet?", "Impressionism", "Impressionism is an art movement that originated in France in the late 19th century, characterized by small, thin brush strokes and an emphasis on light and movement."),
                new("What museum is the 'Starry Night' by Vincent van Gogh housed in?", "The Museum of Modern Art (MoMA)", "'Starry Night' is housed in the Museum of Modern Art (MoMA) in New York City.")
            } },
            { "Food & Drink", new Entry[] {
                new("What is the main ingredient in guacamole?", "Avocado", "Avocado is the main ingredient in guacamole, a traditional Mexican dip."),
                new("Which country is famous for the dish sushi?", "Japan", "Sushi, a dish made with vinegared rice and various other ingredients, originates from Japan."),
                new("What is the world's most expensive spice by weight?", "Saffron", "Saffron, derived from the flower of Crocus sativus, is the world's most expensive spice by weight.")
            } },
            { "Mythology", new Entry[] {
                new("Who is the Greek god of the sea?", "Poseidon", "Poseidon is the Greek god of the sea, earthquakes, and horses."),
                new("What is the name of the Norse god of thunder?", "Thor", "Thor is the Norse god of thunder, lightning, and storms."),
                new("In Egyptian mythology, who is the god of the underworld?", "Osiris", "Osiris is the Egyptian god of the underworld and the afterlife.")
            } },
            { "Television", new Entry[] {
                new("What is the longest-running TV show in the United States?", "The Simpsons", "'The Simpsons' is the longest-running American sitcom and animated television show."),
                new("Who is the main character in 'Breaking Bad'?", "Walter White", "Walter White, played by Bryan Cranston, is the main character in the television series 'Breaking Bad'."),
                new("In which TV show would you find characters named Ross, Rachel, Monica, Chandler, Joey, and Phoebe?", "Friends", "'Friends' is a popular American sitcom that aired from 1994 to 2004.")
            } },
            { "Nature", new Entry[] {
                new("What is the largest rainforest in the world?", "Amazon Rainforest", "The Amazon Rainforest, located in South America, is the largest rainforest in the world."),
                new("Which is the only mammal capable of true flight?", "Bat", "Bats are the only mammals capable of true flight, using their wings to fly."),
                new("What phenomenon causes the northern lights?", "Aurora Borealis", "The northern lights, or aurora borealis, are caused by the interaction of solar wind with the Earth's magnetosphere.")
            } },
            { "Astronomy", new Entry[] {
                new("What is the closest planet to the Sun?", "Mercury", "Mercury is the closest planet to the Sun in our solar system."),
                new("What is the largest planet in our solar system?", "Jupiter", "Jupiter is the largest planet in our solar system, known for its Great Red Spot."),
                new("Which planet is known as the Earth's twin?", "Venus", "Venus is often called Earth's twin because of their similar size, mass, and proximity to the Sun.")
            } },
            { "World History", new Entry[] {
                new("Who was the first emperor of Rome?", "Augustus", "Augustus, also known as Octavian, was the first emperor of Rome, reigning from 27 BC until his death in AD 14."),
                new("In which year did World War I begin?", "1914", "World War I began in 1914, following the assassination of Archduke Franz Ferdinand."),
                new("What was the name of the ship that carried the Pilgrims to America in 1620?", "Mayflower", "The Mayflower was the ship that transported the Pilgrims from England to the New World in 1620.")
            } },
            { "Language", new Entry[] {
                new("What is the most spoken language in the world?", "Mandarin Chinese", "Mandarin Chinese is the most spoken language in the world by number of native speakers."),
                new("What is the official language of Brazil?", "Portuguese", "Portuguese is the official language of Brazil."),
                new("What language is primarily spoken in Australia?", "English", "English is the primary language spoken in Australia.")
            } },
            { "Economics", new Entry[] {
                new("What does GDP stand for?", "Gross Domestic Product", "GDP stands for Gross Domestic Product, which measures the economic performance of a country."),
                new("Which country uses the Yen as its currency?", "Japan", "Japan uses the Yen as its official currency."),
                new("Who is known as the father of modern economics?", "Adam Smith", "Adam Smith, a Scottish economist and philosopher, is known as the father of modern economics for his work 'The Wealth of Nations'.")
            } }
        };
        #endregion

        #region Entrance
        /// <summary>
        /// Start a trivia game. Select topic then enter your reply or "New Game" to start new game.
        /// </summary>
        public static string Trivia(string topic = "General Knowledge", string reply = NewGameCommand)
        {
            if (reply == NewGameCommand || CorrectAnswer == null)
                return NewGame(topic);

            // Passed
            if (reply == CorrectAnswer.Answer)
            {
                string elaboration = CorrectAnswer.Elaboration;
                CorrectAnswer = null;
                PreviousAnswer = null;
                Attempts = 0;
                return $"You are right! {elaboration}";
            }
            else if (Attempts < 3)
            {
                Attempts++;
                PreviousAnswer = reply;
                return "Wrong answer";
            }
            else
            {
                string elaboration = CorrectAnswer.Elaboration;
                string answer = CorrectAnswer.Answer;
                CorrectAnswer = null;
                PreviousAnswer = null;
                Attempts = 0;
                return $"The correct answer is {answer}. {elaboration}\nTry starting a new game!";
            }
        }
        #endregion

        #region Routines
        private static string NewGame(string topic)
        {
            if (!QuestionLibrary.TryGetValue(topic, out Entry[]? questions))
                return $"Nonexistent topic. Available topics: {string.Join(", ", QuestionLibrary.Keys)}";

            Random rnd = new();
            int draw = rnd.Next(questions!.Length);
            var question = questions[draw];
            CorrectAnswer = question;
            return question.Question;
        }
        #endregion
    }
}
