using System.Collections.Generic;

namespace starterCode
{
    /* summary
    Holds stats for a single word:
    - the line numbers where it appears
    - the word text
    - total count */
    public class WordStats
    {
        public string Word { get; }
        public int Count { get; set; } = 0;
        public List<int> LineNumbers { get; } = new List<int>();

        public WordStats(string word) => Word = word;
    }
}