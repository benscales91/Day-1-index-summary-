using System;
using System.Collections.Generic;

namespace TextAnalysisTool
{
    // Summary: ADT that stores higher level methods for word statistics.
    public class WordIndex : WordIndexInterface
    {
        private readonly Dictionary<string, WordADT> _wordMap = new();

        public void AddWord(string word, int lineNumber)
        {
            word = word.ToLowerInvariant();
            if (!_wordMap.TryGetValue(word, out var entry)) // TryGetValue is more efficient than ContainsKey + indexer
            {
                entry = new WordADT(word);
                _wordMap[word] = entry;
            }

            entry.Frequency++;
            entry.LineRefs.Add(lineNumber);
        }

        // Number of unique words currently stored.  
        public int UniqueWords() => _wordMap.Count;
        // Count how many times a word occurs (case-insensitive)
        public int GetFrequency(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return 0;
            word = word.ToLowerInvariant();
            return _wordMap.TryGetValue(word, out var stats) ? stats.Frequency : 0;
        }

        // Day-2:
        // Return all line numbers where the word appears (case-insensitive)
        // If the word does not exist, return an empty collection
        public IReadOnlyList<int> GetLines(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return Array.Empty<int>();
            word = word.ToLowerInvariant();

            return _wordMap.TryGetValue(word, out var stats)
                ? new List<int>(stats.LineRefs)
                : Array.Empty<int>();
        }
        // Day-3: 
        // Helper method to determine if the current word is better than the best found so far
        private static bool IsBetter(string w, int c, string bestWord, int bestCount) =>
            c > bestCount || (c == bestCount && string.CompareOrdinal(w, bestWord) < 0);

        // Gets the most frequent word and its count.
        public (string word, int count) GetMostFrequent()
        {
            string bestWord = string.Empty;
            int bestCount = 0;

            foreach (var (w, entry) in _wordMap)
            {
                if (IsBetter(w, entry.Frequency, bestWord, bestCount))
                {
                    bestWord = w;
                    bestCount = entry.Frequency;
                }
            }

            return (bestWord, bestCount);
        }
        // Day-3:
        // Returns the longest word and its count.
        // Ties are broken by frequency, then lexicographical order.
        // Return ("", 0) if no words

        public (string word, int count) GetLongest()
        {
            string bestWord = string.Empty;
            int bestLen = 0, bestCount = 0;

            foreach (var (word, entry) in _wordMap)
            {
                int length = word.Length;
                int count = entry.Frequency;

                if ((length, count, word) // compare tuple
                    .CompareTo((bestLen, bestCount, bestWord)) > 0)
                {
                    bestWord = word;
                    bestLen = length;
                    bestCount = count;
                }
            }

            return (bestWord, bestCount);
        }
        // Day-4:
        // return all (word, count) pairs in unspecified (hash) order.

        public IEnumerable<(string word, int count)> AllEntries()
        {
            foreach (var keyval in _wordMap)
            {
                yield return (keyval.Key, keyval.Value.Frequency);
            }
        }

        // Day-4:
        // return all (word, count) pairs sorted alphabetically by 'word'
        // ascending = true  -> A to Z
        // ascending = false -> Z to A

        public IEnumerable<(string word, int count)> EntriesSorted(bool ascending = true)
        {
            // Copy dictionary entries to a list
            var entries = _wordMap.ToList();

            // Sort by the word (dictionary key)
            entries.Sort((a, b) => string.CompareOrdinal(a.Key, b.Key));

            // Reverse if descending order is requested
            if (!ascending)
            {
                entries.Reverse();
            }

            // Yield results
            foreach (var entry in entries)
            {
                yield return (entry.Key, entry.Value.Frequency);
            }
        }





    }
}