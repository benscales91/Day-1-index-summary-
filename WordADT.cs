using System.Collections.Generic;

namespace TextAnalysisTool
{
    // Summary
    // ADT that stores words and their stats.
    // Day-1: Store a word and how many times it occurs.

    public class WordADT
    {
        public List<int> LineRefs { get; } = new List<int>(); // Line numbers where the word appears
        public int Frequency { get; set; } = 0; // How many times the word occurs
        public string Word { get; } // The word itself

        public WordADT(string word) => Word = word; // Constructor to initialize the word
    }
}