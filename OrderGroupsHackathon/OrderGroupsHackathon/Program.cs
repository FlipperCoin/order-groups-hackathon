using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace OrderGroupsHackathon
{
    class Program
    {
        static int[,] InitMatrix(int n)
        {
            int[,] m = new int[n, n];

            for (var i = 1; i < m.GetLength(0); i++)
            {
                m[0, i] = i;
                m[i, 0] = i;
            }

            return m;
        }
        static List<int[,]> GetAbelianBoardsParallel(int[,] m, int i, int j)
        {
            bool[] invalidValues = new bool[m.GetLength(0)];
            invalidValues[m[0, j]] = true;
            for (int k = 0; k < j; k++)
            {
                invalidValues[m[j, k]] = true;
                invalidValues[m[i, k]] = true;
            }

            for (int k = j; k < i; k++)
            {
                invalidValues[m[k, j]] = true;
            }

            var boards = new List<int[,]>();
            var l = new object();
            Parallel.For(0, m.GetLength(0), (val, s) =>
            {
                if (invalidValues[val]) return;

                m = (int[,])m.Clone();
                m[i, j] = val;

                // advance in board
                if (j < i)
                {
                    lock (l) { boards.AddRange(GetAbelianBoardsParallel(m, i, j + 1)); }
                }
                else // j == i
                {
                    if (i < (m.GetLength(0) - 1))
                    {
                        lock (l) { boards.AddRange(GetAbelianBoardsParallel(m, i + 1, 1)); }
                    }
                    else
                    {
                        if (!IsAbelianBoardAssociative(m)) return;
                        lock (l) { boards.Add(m); }
                    }
                }
            });

            return boards;
        }

        static List<int[,]> GetAbelianBoards(int[,] m, int i, int j)
        {
            bool[] invalidValues = new bool[m.GetLength(0)];
            invalidValues[m[0, j]] = true;
            for (int k = 0; k < j; k++)
            {
                invalidValues[m[j, k]] = true;
                invalidValues[m[i, k]] = true;
            }

            for (int k = j; k < i; k++)
            {
                invalidValues[m[k, j]] = true;
            }

            var boards = new List<int[,]>();
            for (int val = 0; val < m.GetLength(0); val++)
            {
                if (invalidValues[val]) continue;

                m = (int[,])m.Clone();
                m[i, j] = val;

                // advance in board
                if (j < i)
                {
                    boards.AddRange(GetAbelianBoards(m, i, j + 1));
                }
                else // j == i
                {
                    if (i < (m.GetLength(0) - 1))
                    {
                        boards.AddRange(GetAbelianBoards(m, i + 1, 1));
                    }
                    else
                    {
                        if (!IsAbelianBoardAssociative(m)) continue;
                        boards.Add(m);
                    }
                }
            }

            return boards;
        }

        private static int IndexAbelianBoard(int[,] m, int i, int j)
        {
            if (j <= i)
            {
                return m[i, j];
            }
            else
            {
                return m[j, i];
            }
        }

        private static bool IsAbelianBoardAssociative(int[,] m)
        {
            int n = m.GetLength(0);
            for (int i = 1; i < n; i++)
            {
                for (int j = 1; j < n; j++)
                {
                    for (int k = 1; k < n; k++)
                    {
                        if (IndexAbelianBoard(m,IndexAbelianBoard(m, i, j),k) 
                            != IndexAbelianBoard(m,i,IndexAbelianBoard(m,j,k)))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        static List<int[,]> GetAbelianBoards(int n)
        {
            var m = InitMatrix(n);

            return GetAbelianBoards(m, 1, 1);
        }

        static bool AbelianMatrixEquals(int[,] m1, int[,] m2)
        {
            for (int i = 0; i < m1.GetLength(0); i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    if (m1[i, j] != m2[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static bool SmartAbelianMatrixEquals(int[,] m1, int[,] m2)
        {
            for (int val1 = 0; val1 < m1.GetLength(0); val1++)
            {
                for (int val2 = 0; val2 <= val1; val2++)
                {
                    int m1col = 0;
                    int m1row = 0;
                    int m2col = 0;
                    int m2row = 0;

                    for (int i = 0; i < m1.GetLength(0); i++)
                    {
                        if (m1[i, 0] == val1)
                        {
                            m1row = i;
                        }

                        if (m1[i, 0] == val2)
                        {
                            m1col = i;
                        }

                        if (m2[i, 0] == val1)
                        {
                            m2row = i;
                        }

                        if (m2[i, 0] == val2)
                        {
                            m2col = i;
                        }
                    }

                    if (IndexAbelianBoard(m1,m1row,m1col) != IndexAbelianBoard(m2,m2row,m2col))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static bool CompareAbelianBoards(int[,] m1, int[,] m2, IEnumerable<Dictionary<int,int>> permutations)
        {
            // same size boards & both abelian

            if (AbelianMatrixEquals(m1, m2)) return true;

            foreach (var permutation in permutations)
            {
                var m1perm = GetPermutation(m1, permutation);
                if (SmartAbelianMatrixEquals(m1perm, m2)) return true;
            }

            return false;
        }

        private static int[,] GetPermutation(int[,] m1, Dictionary<int, int> permutation)
        {
            m1 = (int[,])m1.Clone();
            for (int i = 0; i < m1.GetLength(0); i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    m1[i, j] = permutation[m1[i, j]];
                }
            }

            return m1;
        }

        static void Main(string[] args)
        {
            int n = 8;
            Console.WriteLine("Getting boards");
            var boards = GetAbelianBoards(n);
            var permutations = StupidGetPermutations(n);
            var uniqueBoards = new List<int[,]>();

            Console.WriteLine("Comparing boards");
            foreach (var board in boards)
            {
                var found = false;
                foreach (var uniqueBoard in uniqueBoards)
                {
                    if (CompareAbelianBoards(board, uniqueBoard, permutations))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    uniqueBoards.Add(board);
                }
            }

            foreach (var board in uniqueBoards)
            {
                PrintAbelianBoard(board);
                Console.WriteLine();
            }
        }

        private static IEnumerable<Dictionary<int, int>> GetPermutations(int n)
        {
            var permutation = new Dictionary<int,int>(n);
            var values = new bool[n];
            var permutations = GetPermutations(permutation, 0, values);
            foreach (var subper in permutations)
            {
                yield return subper;
            }
        }

        private static IEnumerable<Dictionary<int, int>> GetPermutations(Dictionary<int, int> permutation, int i,
            bool[] values)
        {
            for (int j = 0; j < values.Length; j++)
            {
                if (values[j] == false)
                {
                    var newperm = new Dictionary<int, int>(permutation);
                    newperm[i] = j;
                    var newvalues = (bool[])values.Clone();
                    newvalues[j] = true;
                    var permutations = GetPermutations(newperm, i+1, newvalues);
                    foreach (var subper in permutations)
                    {
                        yield return subper;
                    }
                }
            }
        }

        private static IEnumerable<Dictionary<int, int>> StupidGetPermutations(int n)
        {
            var permutation = new Dictionary<int,int>(n);
            if (n == 3)
            {
                for (int i = 0; i < n; i++)
                {
                    permutation[0] = i;
                    for (int j = 0; j < n; j++)
                    {
                        if (j == i) continue;
                        permutation[1] = j;
                        for (int k = 0; k < n; k++)
                        {
                            if (k==i||k==j) continue;
                            permutation[2] = k;
                            yield return permutation;
                        }
                    }
                }

                yield break;
            }

            if (n == 4)
            {
                for (int i = 0; i < n; i++)
                {
                    permutation[0] = i;
                    for (int j = 0; j < n; j++)
                    {
                        if (j == i) continue;
                        permutation[1] = j;
                        for (int k = 0; k < n; k++)
                        {
                            if (k == i || k == j) continue;
                            permutation[2] = k;
                            for (int l = 0; l < n; l++)
                            {
                                if (l == i || l == j || l == k) continue;
                                permutation[3] = l;
                                yield return permutation;
                            }
                        }
                    }
                }

                yield break;
            }

            if (n == 5)
            {
                for (int i = 0; i < n; i++)
                {
                    permutation[0] = i;
                    for (int j = 0; j < n; j++)
                    {
                        if (j == i) continue;
                        permutation[1] = j;
                        for (int k = 0; k < n; k++)
                        {
                            if (k == i || k == j) continue;
                            permutation[2] = k;
                            for (int l = 0; l < n; l++)
                            {
                                if (l == i || l == j || l == k) continue;
                                permutation[3] = l;
                                for (int t = 0; t < n; t++)
                                {
                                    if (t == i || t == j || t == k || t == l) continue;
                                    permutation[4] = t;
                                    yield return permutation;
                                }
                            }
                        }
                    }
                }

                yield break;
            }

            if (n == 6)
            {
                for (int i = 0; i < n; i++)
                {
                    permutation[0] = i;
                    for (int j = 0; j < n; j++)
                    {
                        if (j == i) continue;
                        permutation[1] = j;
                        for (int k = 0; k < n; k++)
                        {
                            if (k == i || k == j) continue;
                            permutation[2] = k;
                            for (int l = 0; l < n; l++)
                            {
                                if (l == i || l == j || l == k) continue;
                                permutation[3] = l;
                                for (int t = 0; t < n; t++)
                                {
                                    if (t == i || t == j || t == k || t == l) continue;
                                    permutation[4] = t;
                                    for (int r = 0; r < n; r++)
                                    {
                                        if (r == i || r == j || r == k || r == l || r == t) continue;
                                        permutation[5] = r;
                                        yield return permutation;
                                    }
                                }
                            }
                        }
                    }
                }

                yield break;
            }

            if (n == 7)
            {
                for (int i = 0; i < n; i++)
                {
                    permutation[0] = i;
                    for (int j = 0; j < n; j++)
                    {
                        if (j == i) continue;
                        permutation[1] = j;
                        for (int k = 0; k < n; k++)
                        {
                            if (k == i || k == j) continue;
                            permutation[2] = k;
                            for (int l = 0; l < n; l++)
                            {
                                if (l == i || l == j || l == k) continue;
                                permutation[3] = l;
                                for (int t = 0; t < n; t++)
                                {
                                    if (t == i || t == j || t == k || t == l) continue;
                                    permutation[4] = t;
                                    for (int r = 0; r < n; r++)
                                    {
                                        if (r == i || r == j || r == k || r == l || r == t) continue;
                                        permutation[5] = r;
                                        for (int s = 0; s < n; s++)
                                        {
                                            if (s == i || s == j || s == k || s == l || s == t || s == r) continue;
                                            permutation[6] = s;
                                            yield return permutation;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                yield break;
            }

            if (n == 8)
            {
                for (int i = 0; i < n; i++)
                {
                    permutation[0] = i;
                    for (int j = 0; j < n; j++)
                    {
                        if (j == i) continue;
                        permutation[1] = j;
                        for (int k = 0; k < n; k++)
                        {
                            if (k == i || k == j) continue;
                            permutation[2] = k;
                            for (int l = 0; l < n; l++)
                            {
                                if (l == i || l == j || l == k) continue;
                                permutation[3] = l;
                                for (int t = 0; t < n; t++)
                                {
                                    if (t == i || t == j || t == k || t == l) continue;
                                    permutation[4] = t;
                                    for (int r = 0; r < n; r++)
                                    {
                                        if (r == i || r == j || r == k || r == l || r == t) continue;
                                        permutation[5] = r;
                                        for (int s = 0; s < n; s++)
                                        {
                                            if (s == i || s == j || s == k || s == l || s == t || s == r) continue;
                                            permutation[6] = s;
                                            for (int o = 0; o < n; o++)
                                            {
                                                if (o == i || o == j || o == k || o == l || o == t || o == r || o == s) continue;
                                                permutation[7] = o;
                                                yield return permutation;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                yield break;
            }

            throw new ArgumentException();
        }

        private static void PrintMatrix(int[,] m)
        {
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(0); j++)
                {
                    if (j < m.GetLength(0) - 1)
                        Console.Write($"{m[i,j]} ");
                    else
                        Console.WriteLine($"{m[i,j]}");
                }
            }
        }

        private static void PrintAbelianBoard(int[,] m)
        {
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(0); j++)
                {
                    if (j < m.GetLength(0)-1)
                        Console.Write($"{(j < i ? m[i,j].ToString() : m[j,i].ToString())} ");
                    else
                        Console.WriteLine($"{(j < i ? m[i, j].ToString() : m[j, i].ToString())}");
                }
            }
        }
    }
}
