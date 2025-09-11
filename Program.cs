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