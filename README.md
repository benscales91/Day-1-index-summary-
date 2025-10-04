Text Analysis Tool (C#)

A C# console application for indexing and analysing text files.
The program demonstrates the use of core data structures (Dictionary, Binary Search Tree, Linked List) by implementing them as interchangeable backends for a word index.

Features:
- Word Indexing
- Stores words, their frequency, and line numbers.
- Case-insensitive (normalises input).
- Roman numeral filter and tokenisation with regex.

Multiple Backends:
- Dictionary: fast build and lookup, alphabetical output requires sorting (O(n log n)).
- Binary Search Tree (BST): slower build but provides ordered traversal in O(n). Implemented with pass-by-ref recursion for robustness.

Interactive Console Menu:
- Search for word frequency and line numbers.
- List words A→Z or Z→A.
- Find longest or most frequent word.
- Switch between Dictionary and BST at runtime.
- Measure build and traversal timings with Stopwatch.

Example Usage
Summary (Dictionary backend):
mobydick.txt contains 215,830 words.
Unique word count: 17,500

Menu:
 1) List all words A to Z
 2) List all words Z to A
 5) Show longest word
 8) Show most frequent word
10) Switch index backend
11) Time build + listing

Technical Highlights
- C# Object-Oriented Design
- Classes: WordADT, WordIndex, BSTIndex.
- Interface: WordIndexInterface allows backend swapping.
- Encapsulation: BSTNode kept private to BSTIndex.
- Properties used for safe access to data.
- Complexity Considerations
- Dictionary lookups: O(1) average.
- Dictionary alphabetical listing: O(n log n) due to sorting.
- BST insertion: O(log n) average, O(n) worst-case (unlikely in natural text).
- BST traversal: O(n) for full ordered output.
- Aggregates (longest, most frequent): O(n) scans.

Learning Outcomes

This project was developed as part of a Data Structures & Algorithms module.
It showcases:
- Understanding of recursion (BST insertion with ref).
- Iterative stack traversal to avoid deep recursion.
- Linking theory (asymptotic complexity) with practice (timing results).
- Clear explanation of why some structures (e.g., stacks, linked lists) are unsuitable as primary backends for this task.

Future Improvements:
- Implement linked list backend fully (for completeness, not efficiency).
- Add persistence (save/load index).
- Extend analysis with top-N frequent words or word clouds.

Structure
/TextAnalysisTool
 ├── Program.cs           # Console UI
 ├── WordADT.cs           # Word abstract data type
 ├── WordIndex.cs         # Dictionary backend
 ├── BSTIndex.cs          # Binary Search Tree backend
 ├── WordIndexInterface.cs# Interface for backends
 ├── LinkedListIndex.cs   # Linked List stub
 └── README.md            # Project overview

Note

This repository is a showcase version of coursework originally completed at university.
It has been adapted and documented for portfolio purposes — all code and explanations here are my own work.
