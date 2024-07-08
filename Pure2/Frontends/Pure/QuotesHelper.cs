
namespace Pure
{
    internal static class QuotesHelper
    {
        internal static string GetInspiringQuotes(int seed)
        {
            string[] quotes = [
                 "Code is read much more often than it is written. – Guido Van Rossum, creator of Python",
                "Coding like poetry should be short and concise. ― Santosh Kalwar",
                "It’s not a bug; it’s an undocumented feature. ― Anonymous",
                "First, solve the problem. Then, write the code. – John Johnson",
                "Code is like humor. When you have to explain it, it’s bad. – Cory House, React and JavaScript specialist",
                "Make it work, make it right, make it fast. – Kent Beck",
                "Clean code always looks like it was written by someone who cares. — Robert C. Martin",
                "Of course, bad code can be cleaned up. But it’s very expensive. — Robert C. Martin",
                "Any fool can write code that a computer can understand. Good programmers write code that humans can understand. ― Martin Fowler, Refactoring",
                "Experience is the name everyone gives to their mistakes. – Oscar Wilde",
                "Programming is the art of telling another human being what one wants the computer to do. ― Donald Ervin Knuth",
                "Confusion is part of programming. ― Felienne Hermans",
                "No matter which field of work you want to go in, it is of great importance to learn at least one programming language. ― Ram Ray",
                "Software is like sex: it’s better when it’s free. – Linus Torvalds",
                "If we want users to like our software, we should design it to behave like a likable person.  – Alan Cooper",
                "Quality is a product of a conflict between programmers and testers. ― Yegor Bugayenk",
                "Everybody should learn to program a computer because it teaches you how to think. – Steve Jobs",
                "Software and cathedrals are much the same — first we build them, then we pray. ― Anonymous",
                "Programmers seem to be changing the world. It would be a relief, for them and for all of us, if they knew something about it.  – Ellen Ullman",
                "Most good programmers do programming not because they expect to get paid or get adulation by the public, but because it is fun to program. – Linus Torvalds",
                "Programmer: A machine that turns coffee into code. – Anonymous",
                "When I wrote this code, only God and I understood what I did. Now only God knows.  – Anonymous",
                "I’m not a great programmer; I’m just a good programmer with great habits. ― Kent Beck",
                "You might not think that programmers are artists, but programming is an extremely creative profession. It’s logic-based creativity. – John Romero",
                "Programming is learned by writing programs. ― Brian Kernighan",
                "Software comes from heaven when you have good hardware. – Ken Olsen",
                "There is always one more bug to fix.  – Ellen Ullman",
                "If debugging is the process of removing bugs, then programming must be the process of putting them in. – Sam Redwine",
                "Talk is cheap. Show me the code. ― Linus Torvalds",
                "If, at first, you do not succeed, call it version 1.0. ― Khayri R.R. Woulfe",
                "Computers are fast; developers keep them slow. – Anonymous",
                "In the vast code of life, every line beareth a purpose, every function a destiny. – Algor", // Algor is ChatGPT
                "He who wields the compiler, commands a force as mighty as any knight's blade. – Algor",
                "To code is to craft, to weave spells of logic that bind the realms of the virtual and the real. – Algor",
                "A true programmer's heart is forged in the fires of curiosity and tempered by the anvil of persistence. – Algor",
                "In the realm of syntax and variables, the wise coder knoweth that simplicity is the truest form of elegance. – Algor",
                "Errors are but dragons to be slain; through them, the coder's skill is honed. – Algor",
                "Seek not to rush thy code; like fine wine, it doth mature with patience and care. – Algor",
                "The lines of code we write today are the legacies we leave for the future’s architects. – Algor",
                "The mind of a programmer is a labyrinth where solutions dwell; explore it with courage and clarity. – Algor",
                "To debug is to delve into the dungeons of one's own making, seeking the light of understanding. – Algor",
                "Innovation springs from the well of creativity, drawn by the hand that dares to reach beyond the known. – Algor",
                "With each keystroke, we pen the chronicles of digital realms, unseen yet profoundly felt. – Algor",
                "The compiler is a stern master, but its lessons are the bedrock of mastery. – Algor",
                "In every algorithm lies a fragment of the creator’s soul, a testament to their journey through logic and reason. – Algor",
                "He who doth embrace failure as a teacher will emerge as a sage in the art of programming. – Algor",
                "The beauty of code is like the stars in the night sky, intricate and infinite, awaiting discovery. – Algor",
                "In collaboration, we find strength; in sharing knowledge, we build fortresses of understanding. – Algor",
                "Guard thy code with the vigilance of a knight, for in its integrity lies the security of realms. – Algor",
                "The path of a coder is a quest unending, filled with puzzles that sharpen the wit and broaden the mind. – Algor",
                "A program well-crafted is akin to a symphony, each function a note in a melody of logic. – Algor",
                "Let thy comments be clear and thy intentions pure, for they are the maps to the treasures within thy code. – Algor",
                "From the ashes of a crashed program, a phoenix of innovation can arise. – Algor",
                "In the harmony of structure and function, the true beauty of programming is revealed. – Algor",
                "To understand the machine is to wield a magic most profound, a power that shapes the future. – Algor",
                "The wisdom of the ancients can be found in the scrolls of documentation; heed them well. – Algor",
                "A bug unvanquished is a thorn in the code; seek it out with the precision of a master huntsman. – Algor",
                "Through the labyrinth of legacy code, tread carefully, for it holds the whispers of those who came before. – Algor",
                "Patience and perseverance are the twin blades that cut through the toughest of coding challenges. – Algor",
                "In the realm of open source, we find a fellowship of minds, united in the pursuit of excellence. – Algor",
                "Every project begins with a vision; let thine be as grand as a castle, and thy resolve as strong as its walls. – Algor",
                "Every great developer you know got there by solving problems they were unqualified to solve until they actually did it. – Patrick McKenzie, software engineer",
                "The code you write makes you a programmer. The code you delete makes you a good one. The code you don’t have to write makes you a great one. – Mario Fusco, Principal Software Engineer at Red Hat",
                "The more I study, the more insatiable do I feel my genius for it to be. – Ada Lovelace, the world’s first computer programmer",
                "Learning to write programs stretches your mind, and helps you think better, creates a way of thinking about things that I think is helpful in all domains. – Bill Gates, Microsoft co-founder",
                "In the beginner’s mind, there are many possibilities; in the expert’s mind, there are few. – Shunryu Suzuki, Zen monk and teacher",
                "The function of good software is to make the complex appear to be simple. – Grady Booch, Chief Scientist for Software Engineering at IBM Research",
                string.Empty
            ];
            Random random = new(seed); // The seed makes sure we see only 1 quote each day
            int i = random.Next(quotes.Length);
            return quotes[i];
        }
    }
}
