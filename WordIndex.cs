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
    }
}