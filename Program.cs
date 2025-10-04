using System;
using System.Collections.Generic; // Day 14: for HashSet
using System.Diagnostics;          // Day 14: Stopwatch for simple timing
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextAnalysisTool
{
    internal class Program
    {
        // config/helpers 

        // Day 7: roman numeral handling
        // Roman numerals are whitelisted for hybrid whitelist/regex filtering.
        static readonly HashSet<string> RomanWhitelist = new(StringComparer.OrdinalIgnoreCase)
        {
            "i","mix","div","dix","li","mi","xi","di",
            "cc","cd","cv","dc","mc","md",
            "cdi","clix","liv",
        };

        // LettersOnly regex (compiled once for speed)
        // Matches either 2+ letters or the single words 'a' or 'i'.
        // Using static readonly + RegexOptions.Compiled is more efficient than creating a new regex object on every isWord() call.
     
        static readonly Regex LettersOnly = new(@"^(?:[A-Za-z]{2,}|[AaIi])$", RegexOptions.Compiled);
        static readonly Regex RomanLetters = new(@"^[ivxlcdm]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        static bool IsWord(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;
            if (!LettersOnly.IsMatch(token)) return false;
            return RomanLetters.IsMatch(token) ? RomanWhitelist.Contains(token) : true;
        }

        // Day 9: file chooser
        static string SelectFile()
        {
            Console.Write("Type file to read and press 'ENTER': (sample/sherlock/mobydick): ");
            var option = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            return option switch
            {
                "sample" => "sample.txt",
                "mobydick" => "mobydick.txt",
                "sherlock" => "sherlock.txt",
                _ => "sample.txt" // default to sample.txt if unrecognized
            };
        }

        // Day 14: backend toggle
        enum Backend { Dictionary, Bst }
        static Backend currentBackend = Backend.Dictionary;

        static string BackendLabel() =>
            currentBackend == Backend.Dictionary ? "Dictionary" : "BST";

        // factory to swap the implementation
        static WordIndexInterface CreateIndex(Backend which) =>
            which == Backend.Dictionary ? new WordIndex() : new BSTIndex();

        // program state 
        static string fileName = SelectFile();
        static string[]? linesInFile;

        static void Main(string[] args)
        {
            // initial build
            var (index, totalWords) = BuildIndexFromFile();

            if (index.UniqueWords() == 0)
            {
                Console.WriteLine("No valid words found.");
                return;
            }

            const int PageSize = 30;
            const int PreviewCount = 50;

            Console.WriteLine($"\nSummary ({BackendLabel()} backend):");
            Console.WriteLine($"{fileName} contains {totalWords} words.");
            Console.WriteLine($"Unique word count: {index.UniqueWords()}");
            Console.WriteLine();

            // quick test (kept from Day 2)
            string testWord = "labubu";
            Console.WriteLine($"Number of times '{testWord}' appears in text: {index.GetFrequency(testWord)}");
            Console.WriteLine($"'{testWord}' appears in lines: {string.Join(",", index.GetLines(testWord).Take(PageSize))}.");

            // Menu 
            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine(" 0) Change file");
                Console.WriteLine(" 1) List all words A to Z");
                Console.WriteLine(" 2) List all words Z to A");
                Console.WriteLine(" 3) Preview first 50 words A to Z");
                Console.WriteLine(" 4) Preview first 50 words Z to A");
                Console.WriteLine(" 5) Show longest word");
                Console.WriteLine(" 6) Search word (frequency with line numbers)");
                Console.WriteLine(" 7) Search loop (keep searching words)");
                Console.WriteLine(" 8) Show most frequent word");
                Console.WriteLine(" 9) Exit");
                // Day 14 additions
                Console.WriteLine($"10) Switch index backend (currently: {BackendLabel()})");
                Console.WriteLine("11) Time build + A to Z listing (current backend)");
                Console.Write("Select: ");

                var option = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(option) || option == "9")
                    break;

                switch (option)
                {
                    case "0":
                        fileName = SelectFile();
                        (index, totalWords) = BuildIndexFromFile();
                        if (index.UniqueWords() == 0)
                        {
                            Console.WriteLine("No valid words found.");
                            return;
                        }
                        Console.WriteLine($"\nLoaded {fileName} with {totalWords} words, {index.UniqueWords()} unique ({BackendLabel()}).");
                        break;

                    case "1":
                        Console.WriteLine("\nAll words A to Z:");
                        PagedWordList(index.EntriesSorted(true)); // paged output
                        break;

                    case "2":
                        Console.WriteLine("\nAll words Z to A:");
                        PagedWordList(index.EntriesSorted(false)); // paged output
                        break;

                    case "3":
                        Console.WriteLine("\nPreview A to Z (first 50):");
                        foreach (var (word, count) in index.EntriesSorted(true).Take(PreviewCount))
                            Console.WriteLine($"{word} : {count}");
                        break;

                    case "4":
                        Console.WriteLine("\nPreview Z to A (first 50):");
                        foreach (var (word, count) in index.EntriesSorted(false).Take(PreviewCount))
                            Console.WriteLine($"{word} : {count}");
                        break;

                    case "5":
                        {
                            var longest = index.GetLongest();
                            Console.WriteLine($"Longest word: '{longest.word}' (count={longest.count})");
                            break;
                        }

                    case "6":
                        {
                            Console.Write("Type a word: ");
                            string query = (Console.ReadLine() ?? "").Trim();
                            int freq = index.GetFrequency(query);

                            if (freq == 0)
                            {
                                Console.WriteLine($"'{query}' not found in text file.");
                            }
                            else
                            {
                                Console.WriteLine($"Frequency: {freq}");
                                PagedLineNumbers(index.GetLines(query), PageSize); // paged output
                            }
                            break;
                        }

                    case "7":
                        {
                            while (true)
                            {
                                Console.Write("\nType a word (ENTER to stop): ");
                                string query = (Console.ReadLine() ?? "").Trim();
                                if (string.IsNullOrWhiteSpace(query)) break;

                                int freq = index.GetFrequency(query);
                                if (freq == 0)
                                {
                                    Console.WriteLine($"'{query}' not found in text file.");
                                    continue;
                                }

                                Console.WriteLine($"Frequency: {freq}");
                                PagedLineNumbers(index.GetLines(query), PageSize);
                            }
                            break;
                        }

                    case "8":
                        {
                            var mf = index.GetMostFrequent();
                            string mfw = mf.word;
                            Console.WriteLine($"Most frequent word: '{mfw}' (frequency = {mf.count})");
                            PagedLineNumbers(index.GetLines(mfw), PageSize);
                            break;
                        }

                    // Day 14 features
                    case "10":
                        currentBackend = currentBackend == Backend.Dictionary ? Backend.Bst : Backend.Dictionary;
                        Console.WriteLine($"\nSwitching backend to: {BackendLabel()}");
                        (index, totalWords) = BuildIndexFromFile();
                        Console.WriteLine($"Rebuilt {fileName} → {index.UniqueWords()} unique ({BackendLabel()}).");
                        break;

                    case "11":
                        {
                            Console.WriteLine($"\nTiming ({BackendLabel()}):");
                            var sw = Stopwatch.StartNew();
                            (index, totalWords) = BuildIndexFromFile();
                            sw.Stop();
                            Console.WriteLine($"Build time: {sw.ElapsedMilliseconds} ms " +
                                              $"({totalWords} tokens, {index.UniqueWords()} unique)");

                            sw.Restart();
                            int shown = 0;
                            foreach (var _ in index.EntriesSorted(true))
                            {
                                shown++;
                                if (shown >= 300_000) break; // safety for huge inputs
                            }
                            sw.Stop();
                            Console.WriteLine($"Traverse A to Z time: {sw.ElapsedMilliseconds} ms");
                            break;
                        }

                    default:
                        Console.WriteLine("Please select 1–11.");
                        break;
                }
            }
        }

        // paging helpers
        static void PagedWordList(IEnumerable<(string word, int count)> entries, int chunkSize = 30)
        {
            int shown = 0;

            foreach (var (term, tally) in entries)
            {
                Console.WriteLine($"{term} : {tally}");
                shown++;

                if (shown % chunkSize == 0)
                {
                    Console.Write("Press 'ENTER' to view the next page, or any other key to return to menu.");
                    var key = Console.ReadKey(intercept: true);
                    Console.WriteLine();
                    if (key.Key != ConsoleKey.Enter)
                        break;
                }
            }
        }

        static void PagedLineNumbers(IEnumerable<int> lineNumbers, int pageSize = 30)
        {
            var all = (lineNumbers as IList<int>) ?? new List<int>(lineNumbers);
            int total = all.Count;
            if (total == 0)
            {
                Console.WriteLine("No line positions to display.");
                return;
            }

            int pages = (total + pageSize - 1) / pageSize;

            for (int p = 0; p < pages; p++)
            {
                var slice = all.Skip(p * pageSize).Take(pageSize);

                if (p == 0) Console.Write("Lines: ");
                Console.Write(string.Join(",", slice));

                int shown = Math.Min((p + 1) * pageSize, total);
                if (p < pages - 1)
                {
                    Console.Write(" .");
                    Console.WriteLine($"  ({shown} of {total})");
                    Console.Write("Press 'ENTER' for more lines, or any other key to stop.");
                    var key = Console.ReadKey(intercept: true);
                    Console.WriteLine();
                    if (key.Key != ConsoleKey.Enter) break;
                }
                else
                {
                    Console.WriteLine($"  ({total} of {total})");
                }
            }
        }

        // Day 14: backend-aware build 
        static (WordIndexInterface index, int totalWords) BuildIndexFromFile()
        {
            int lineNumber = 0, tokenCount = 0;
            char[] delimiters = { ' ', ',', '"', ':', ';', '?', '!', '-', '.', '\'', '*', ')', '(', '[', ']' };

            var index = CreateIndex(currentBackend);

            foreach (var line in File.ReadLines(fileName))
            {
                lineNumber++;
                foreach (var raw in line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (IsWord(raw))
                    {
                        tokenCount++;
                        index.AddWord(raw.ToLowerInvariant(), lineNumber);
                    }
                }
            }
            return (index, tokenCount);
        }
    }
}















