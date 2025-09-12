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

            // Day-2: interactive queries
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
            }
            
        }


        // Day-1: build index, no queries, no per-word printing
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