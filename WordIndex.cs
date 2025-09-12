using System.Collections.Generic;

namespace starterCode
{
    /// <summary>ADT that stores words and their stats.</summary>
    public class WordIndex
    {
        private readonly Dictionary<string, WordStats> _index = new();

        /// <summary>Add one occurrence of a word with the given line number.</summary>
        public void AddOccurrence(string word, int lineNumber)
        {
            if (!_index.ContainsKey(word))
            {
                _index[word] = new WordStats(word);
            }
            _index[word].Count++;
            _index[word].LineNumbers.Add(lineNumber);
        }

        /// <summary>Number of unique words currently stored.</summary>
        public int UniqueCount() => _index.Count;
        // Count how many times a word occurs (case-insensitive)
        public int FrequencyOf(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return 0;
            word = word.ToLowerInvariant();
            return _index.TryGetValue(word, out var stats) ? stats.Count : 0;
        }
        // Day-2:
        // Return all line numbers where the word appears (case-insensitive)
        // Return a copy so callers can't mutate internal state
        public IEnumerable<int> LinesFor(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return Array.Empty<int>();
            word = word.ToLowerInvariant();
            return _index.TryGetValue(word, out var stats)
                ? new List<int>(stats.LineNumbers)
                : Array.Empty<int>();
        }
        // Day-3: 
        // Return the most frequent word and its count
        // Ties are broken by lexicographical order
        //  Return ("", 0) if no words 
        public (string word, int count) MostFrequent()
        {
            int bestCount = 0;
            string bestWord = string.Empty;

            foreach (var kv in _index)
            {
                string w = kv.Key;
                int c = kv.Value.Count;

                if (c > bestCount || (c == bestCount && string.CompareOrdinal(w, bestWord) < 0))
                {
                    bestCount = c;
                    bestWord = w;
                }
            }

            return (bestWord, bestCount);
        }
        // Day-3:
        // Returns the longest word and its count.
        // Ties are broken by frequency, then lexicographical order.
        // Return ("", 0) if no words

        public (string word, int count) LongestWord()
        {
            string bestWord = string.Empty;
            int bestLen = 0;
            int bestCount = 0;

            foreach (var kv in _index)
            {
                string w = kv.Key;
                int cnt = kv.Value.Count;
                int len = w.Length;

                if (len > bestLen ||
                    (len == bestLen && (cnt > bestCount ||
                    (cnt == bestCount && string.CompareOrdinal(w, bestWord) < 0))))
                {
                    bestLen = len;
                    bestCount = cnt;
                    bestWord = w;
                }
            }

            return (bestWord, bestCount);
        }

    }
}