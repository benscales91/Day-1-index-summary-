using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace starterCode
{
    internal class Program
    {
        static string fileName = "sherlock.txt"; // file to read
        static string[] linesInFile;

        static void Main(string[] args)
        {
            // Build the index from file (silent)
            var (index, totalWords) = BuildIndexFromFile();

            // Day-1 summary only
            Console.WriteLine($"{fileName} contains {totalWords} words.");
            Console.WriteLine($"Unique words: {index.UniqueCount()}");

            Console.WriteLine();
            Console.WriteLine("Testing query...");

            // Day-2: test queries
            string testWord = "watson"; // choose a word to test
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
                        foreach (var (word, count) in index.AllSorted(true))
                            Console.WriteLine($"{word} : {count}");
                        break;
                    
                    case "2":
                        Console.WriteLine("\nAll words Z to A:");
                        foreach (var (word, count) in index.AllSorted(false))
                            Console.WriteLine($"{word} : {count}");
                        break;

                    case "3":
                        Console.WriteLine("\nPreview A to Z (first 50):");
                        foreach (var (word, count) in index.AllSorted(true).Take(50))
                            Console.WriteLine($"{word} : {count}");
                        break;

                    case "4":
                        Console.WriteLine("\nPreview Z to A (first 50):");
                        foreach (var (word, count) in index.AllSorted(false).Take(50))
                            Console.WriteLine($"{word} : {count}");
                        break;

                    case "5": // Day 5: Longest word search
                        {
                            var longestWord = index.LongestWord();
                            Console.WriteLine($"Longest word: '{longestWord.word}' (count={longestWord.count})");
                            break;
                        }

                    case "6": // Day 5: Single search
                        {
                            Console.Write("Enter a word: ");
                            string ws = (Console.ReadLine() ?? "").Trim();

                            int f = index.FrequencyOf(ws);
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
                            if (total == 0) Console.WriteLine("Lines: (none)");
                            else { if (total > 30) Console.Write(" ..."); Console.WriteLine($"  ({Math.Min(shown, total)} of {total})"); }
                            break;
                        }

                    case "7": // Day 5: Lookup loop (multi-search until blank)
                        {
                            while (true)
                            {
                                Console.Write("\nEnter a word (ENTER to stop): ");
                                var ws = Console.ReadLine();
                                if (string.IsNullOrWhiteSpace(ws)) break;

                                int f = index.FrequencyOf(ws);
                                Console.WriteLine($"Frequency: {ws}");

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
                                if (total == 0) Console.WriteLine("Lines: (none)");
                                else { if (total > 30) Console.Write(" ..."); Console.WriteLine($"  ({Math.Min(shown, total)} of {total})"); }
                            }
                            break;
                        }

                    default:
                        Console.WriteLine("Please select 1–8.");
                        break;
                }
            }
 
            /* Day-2: interactive queries (Obsolete with Day-5 menu options)
            while (true)
            {
                Console.Write("\nEnter a word to search (or just press ENTER to quit): ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                Console.WriteLine($"Frequency: {index.FrequencyOf(input)}");
                var lines = index.LinesFor(input).ToList();
                Console.WriteLine(lines.Count == 0
                    ? "No occurrences."
                    : $"Lines: {string.Join(",", lines.Take(25))}{(lines.Count > 25 ? "..." : "")}");
            }*/

        }

        // Day-1: build the index from file, return index and total words
        static (WordIndex index, int totalWords) BuildIndexFromFile()
        {
            linesInFile = File.ReadAllLines(fileName);
            int lineNumber = 0;
            int numberWords = 0;

            // delimiters that split words
            char[] delimiters = { ' ', ',', '"', ':', ';', '?', '!', '-', '.', '\'', '*' };

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

        // keep this helper – it filters real words only
        static bool isWord(string str)
        {
            return Regex.IsMatch(str, @"\b(?:[a-z]{2,}|[ai])\b", RegexOptions.IgnoreCase);
        }
    }
}