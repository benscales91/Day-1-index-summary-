using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.IO;

namespace starterCode
{
    internal class Program
    {
        static string fileName = "sample.txt"; // file to read
        static string[] linesInFile;
        static readonly HashSet<string> RomanBlacklist = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "i","ii","iii","iv","v","vi","vii","viii","ix","x",
            "xi","xii","xiii","xiv","xv","xvi","xvii","xviii","xix","xx"
        };


        static void Main(string[] args)
        {
            // Build the index from file (silent)
            var (index, totalWords) = BuildIndexFromFile();

            if (index.UniqueCount() == 0)
            {
                Console.WriteLine("No valid words found.");
                return; // stop program early
            }

            // Day-1 summary only
            Console.WriteLine($"{fileName} contains {totalWords} words.");
            Console.WriteLine($"Unique words: {index.UniqueCount()}");

            Console.WriteLine();
            Console.WriteLine("Testing query...");

            // Day-2: test queries
            string testWord = "the"; // choose a word to test
            Console.WriteLine($"Frequency of '{testWord}': {index.FrequencyOf(testWord)}");
            Console.WriteLine($"Lines for '{testWord}': {string.Join(",", index.LinesFor(testWord).Take(10))}...");

            // Day-3: most frequent word and longest word
            var mf = index.MostFrequent();
            Console.WriteLine($"\nMost frequent: '{mf.word}' (count={mf.count})");

            var lw = index.LongestWord();
            Console.WriteLine($"Longest word: '{lw.word}' (count={lw.count})");


            // Day 4: Alphabetical output menu
            while (true)
            {
                // Day 4-5: Alphabetical output menu (plus Day 5 additions)
                Console.WriteLine("\nMenu:");
                Console.WriteLine(" 1) List all words A to Z");
                Console.WriteLine(" 2) List all words Z to A");
                Console.WriteLine(" 3) Preview first 50 words A to Z");
                Console.WriteLine(" 4) Preview first 50 words Z to A");
                Console.WriteLine(" 5) Show longest word");                          // Day 5
                Console.WriteLine(" 6) Search word (frequency with line numbers)");  // Day 5
                Console.WriteLine(" 7) Search loop (keep searching words)");         // Day 5
                Console.WriteLine(" 8) Exit");
                Console.Write("Select: ");
                var option = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(option) || option == "8")
                    break;

                switch (option)
                {
                    case "1":
                        Console.WriteLine("\nAll words A to Z:");
                        ShowWordPages(index.AllSorted(true)); // paged output
                        break;

                    case "2":
                        Console.WriteLine("\nAll words Z to A:");
                        ShowWordPages(index.AllSorted(false)); // paged output
                        break;

                    case "3":
                        
                        Console.WriteLine("\nPreview A to Z (first 30):");
                        foreach (var (word, count) in index.AllSorted(true).Take(30))
                            Console.WriteLine($"{word} : {count}");
                        break;

                    case "4":

                        Console.WriteLine("\nPreview Z to A (first 30):");
                        foreach (var (word, count) in index.AllSorted(false).Take(30))
                            Console.WriteLine($"{word} : {count}");
                        break;

                    case "5": // Day 5: Longest word search
                        {
                            var longestWord = index.LongestWord();
                            Console.WriteLine($"Longest word: '{longestWord.word}' (count={longestWord.count})");
                            break;
                        }

                    case "6": // Day 5: Single search (improved in Day 6)
                        {
                            Console.Write("Type a word: ");
                            string ws = (Console.ReadLine() ?? "").Trim();

                            int f = index.FrequencyOf(ws);

                            // Improved feedback: tell the user if the word doesn’t appear
                            if (f == 0)
                            {
                                Console.WriteLine($"'{ws}' not found in text file.");
                            }
                            else
                            {
                                Console.WriteLine($"Frequency: {f}");

                                // Print up to 30 line numbers for readability
                                int shown = 0, total = 0;
                                foreach (int ln in index.LinesFor(ws))
                                {
                                    total++;
                                    if (shown == 0) Console.Write("Lines: ");
                                    if (shown < 30)
                                    {
                                        if (shown > 0) Console.Write(",");
                                        Console.Write(ln);
                                        shown++;
                                    }
                                }

                                if (total > 30) Console.Write(" ...");
                                Console.WriteLine($"  ({Math.Min(shown, total)} of {total})");
                            }
                            break;
                        }

                    case "7": // Day 5 (improved in Day 6): Lookup loop (multi-search until blank)
                        {
                            while (true)
                            {
                                Console.Write("\nType a word (ENTER to stop): ");
                                string ws = (Console.ReadLine() ?? "").Trim();

                                // stop the inner loop if user presses ENTER
                                if (string.IsNullOrWhiteSpace(ws))
                                    break;

                                // get frequency AFTER trimming / blank check
                                int f = index.FrequencyOf(ws);

                                if (f == 0)
                                {
                                    Console.WriteLine($"'{ws}' not found in text file.");
                                    continue; // go back to ask for the next word
                                }

                                Console.WriteLine($"Frequency: {f}");

                                // Print up to 30 line numbers for readability
                                int shown = 0, total = 0;
                                foreach (int ln in index.LinesFor(ws))
                                {
                                    total++;
                                    if (shown == 0) Console.Write("Lines: ");
                                    if (shown < 30)
                                    {
                                        if (shown > 0) Console.Write(",");
                                        Console.Write(ln);
                                        shown++;
                                    }
                                }

                                if (total > 30) Console.Write(" ...");
                                Console.WriteLine($"  ({Math.Min(shown, total)} of {total})");
                            }
                            break;
                        }

                    default:
                        Console.WriteLine("Please select 1–8.");
                        break;
                }
            }
        }
        // Day 6: show words in pages of chunkLimit (default 30)
        static void ShowWordPages(IEnumerable<(string word, int count)> entries, int chunkSize = 30)
        {
            int shown = 0;

            foreach (var (term, tally) in entries)
            {
                Console.WriteLine($"{term} : {tally}");
                shown++;

                if (shown % chunkSize == 0)
                {
                    Console.Write("Press 'ENTER' to view the next page, or any other key to return to menu...");
                    var key = Console.ReadKey(intercept: true);
                    Console.WriteLine();
                    if (key.Key != ConsoleKey.Enter)
                        break; // stop paging
                }
            }
        }

        // Day-1: build the index from file, return index and total words
        static (WordIndex index, int totalWords) BuildIndexFromFile()
        {
            linesInFile = File.ReadAllLines(fileName);
            int lineNumber = 0;
            int numberWords = 0;

            // delimiters that split words
            char[] delimiters = { ' ', ',', '"', ':', ';', '?', '!', '-', '.', '\'', '*', ')', '(', '[', ']' };

            var index = new WordIndex();

            foreach (string line in linesInFile)
            {
                lineNumber++;
                string[] wordsInLine = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                foreach (string word in wordsInLine)
                {
                    if (isWord(word))
                    {
                        numberWords++;
                        index.AddOccurrence(word.ToLower(), lineNumber);
                    }
                }
            }

            return (index, numberWords);
        }

       // Day 7: Rules for filtering roman numerals and non-words

       // Whitelist of real words/abbreviations that contain Roman numbers
       static readonly HashSet<string> keepIfRomanish = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
       {
           
        "i","mix","div","dix","li","mi","xi","di", "civic",   // everyday/valid words
        "civil", "immix", "livid", "mimic", "villi", "vivid", // everyday/valid words
        "cc","cd","cv","dc","mc","md", // abbreviations
        "cdi","clix","liv" // proper/trade names
        
       };

        // Decide whether a token is a genuine word for indexing
        static bool isWord(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;

            // letters only; allow single-letter a/i
            if (!Regex.IsMatch(token, @"^(?:[A-Za-z]{2,}|[AaIi])$"))
                return false;

            // If it's composed only of I,V,X,L,C,D,M, treat it as a Roman numeral candidate.
            // Keep ONLY if it's on the curated allow-list above.
            if (Regex.IsMatch(token, @"^[ivxlcdm]+$", RegexOptions.IgnoreCase))
                return keepIfRomanish.Contains(token);

            return true;
        }
    }
}

    








