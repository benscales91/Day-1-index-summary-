using System;
using System.Collections.Generic;

namespace TextAnalysisTool
{
    // Binary Search Tree implementation of WordIndexInterface.
    // Words are kept in alphabetical order so an in-order traversal yields A-Z in O(n).
    public class BSTIndex : WordIndexInterface
    {
        private class BSTNode
        {
            public WordADT Data;        // the payload (word, freq, line refs)
            public BSTNode? Left;       // left child
            public BSTNode? Right;      // right child

            // Create explicit constructor
            public BSTNode(WordADT data)
            {
                Data = data;
                Left = null;
                Right = null;
            }
        }

        // Tree state
        private BSTNode? _root;   // root of the BST (null when empty)
        private int _unique;      // number of unique words

        // Explicit constructor. Starts with an empty tree and zero unique words
        public BSTIndex()
        {
            _root = null;
            _unique = 0;
        }

        // Insert (word, line) using pass-by-ref recursion so child links update in place.
        // Average O(log n), worst O(n) if unbalanced (unlikely in natural text).
   
        public void AddWord(string word, int line)
        {
            word = word.ToLowerInvariant();
            InsertItem(ref _root, word, line);
        }

        private void InsertItem(ref BSTNode? tree, string word, int line)
        {
            // Empty spot -> create a new node
            if (tree is null)
            {
                var entry = new WordADT(word) { Frequency = 1 };
                entry.LineRefs.Add(line);
                tree = new BSTNode(entry);
                _unique++;
                return;
            }

            // Recursive case: navigate left or right based on lexicographic order
            int cmp = string.CompareOrdinal(word, tree.Data.Word);
            if (cmp == 0)
            {
                // Found the word -> update the count
                tree.Data.Frequency++;
                tree.Data.LineRefs.Add(line);
            }
            else if (cmp < 0)
            {
                InsertItem(ref tree.Left, word, line); // recursive call left
            }
            else
            {
                InsertItem(ref tree.Right, word, line); // recursive call right
            }
        }

        // Number of unique words stored so far
        public int UniqueWords() => _unique;

        // Lookups
        private BSTNode? Lookup(string target)
        {
            target = target.ToLowerInvariant();
            var cur = _root;
            while (cur != null)
            {
                int cmp = string.CompareOrdinal(target, cur.Data.Word);
                if (cmp == 0) return cur;
                cur = (cmp < 0) ? cur.Left : cur.Right;
            }
            return null;
        }

        public int GetFrequency(string word) => Lookup(word)?.Data.Frequency ?? 0;

        public IReadOnlyList<int> GetLines(string word)
        {
            var hit = Lookup(word);
            return hit is null ? Array.Empty<int>() : hit.Data.LineRefs.ToArray();
        }

        // Stream (word,count) in alphabetical order.
        // Time: O(n); extra space: O(h) for the stack (h = height, log n average).

        public IEnumerable<(string word, int count)> EntriesSorted(bool ascending = true)
            => ascending ? InOrderIter(_root) : ReverseInOrderIter(_root);

        // Iterative in-order traversal (A -> Z)
        private IEnumerable<(string word, int count)> InOrderIter(BSTNode? root)
        {
            var stack = new Stack<BSTNode>();
            var cur = root;

            while (cur != null || stack.Count > 0)
            {
                while (cur != null) { stack.Push(cur); cur = cur.Left; }
                cur = stack.Pop();
                yield return (cur.Data.Word, cur.Data.Frequency);
                cur = cur.Right;
            }
        }

        // Iterative reverse in-order traversal (Z -> A)
        private IEnumerable<(string word, int count)> ReverseInOrderIter(BSTNode? root)
        {
            var stack = new Stack<BSTNode>();
            var cur = root;

            while (cur != null || stack.Count > 0)
            {
                while (cur != null) { stack.Push(cur); cur = cur.Right; }
                cur = stack.Pop();
                yield return (cur.Data.Word, cur.Data.Frequency);
                cur = cur.Left;
            }
        }

        // Most frequent word (ties broken A -> Z). Single O(n) scan of the tree.

        public (string word, int count) GetMostFrequent()
        {
            string bestWord = string.Empty;
            int bestCount = 0;

            var stack = new Stack<BSTNode>();
            var cur = _root;

            while (cur != null || stack.Count > 0)
            {
                while (cur != null) { stack.Push(cur); cur = cur.Left; }
                cur = stack.Pop();

                string w = cur.Data.Word;
                int c = cur.Data.Frequency;
                if (c > bestCount || (c == bestCount && string.CompareOrdinal(w, bestWord) < 0))
                {
                    bestWord = w;
                    bestCount = c;
                }
                cur = cur.Right;
            }
            return (bestWord, bestCount);
        }

        // Longest word (tie-breaks: length -> frequency -> A -> Z). Single O(n) scan.

        public (string word, int count) GetLongest()
        {
            string bestWord = string.Empty;
            int bestLen = 0, bestCount = 0;

            var stack = new Stack<BSTNode>();
            var cur = _root;

            while (cur != null || stack.Count > 0)
            {
                while (cur != null) { stack.Push(cur); cur = cur.Left; }
                cur = stack.Pop();

                string w = cur.Data.Word;
                int len = w.Length;
                int c = cur.Data.Frequency;

                if (len > bestLen ||
                   (len == bestLen && (c > bestCount ||
                   (c == bestCount && string.CompareOrdinal(w, bestWord) < 0))))
                {
                    bestWord = w; bestLen = len; bestCount = c;
                }
                cur = cur.Right;
            }
            return (bestWord, bestCount);
        }
    }
}



