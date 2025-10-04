using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalysisTool
{
    public interface WordIndexInterface
    {
        // Insert an occurrence of a word at a given line number.
        void AddWord(string word, int line);

        // Return total occurrences of a word (0 if absent).
        int GetFrequency(string word);

        // Return the line numbers where the word appears (read-only list or snapshot).
        IReadOnlyList<int> GetLines(string word);

        // Return the most frequent word (with its count); ties broken lexicographically.
        (string word, int count) GetMostFrequent();

        // Return the longest word (with its count); ties broken by frequency, then lexicographically.
        (string word, int count) GetLongest();

        // Number of unique words stored.
        int UniqueWords();

        // Return all words with their counts, in alphabetical order (A to Z or Z to A).
        IEnumerable<(string word, int count)> EntriesSorted(bool ascending = true);
    }
}
