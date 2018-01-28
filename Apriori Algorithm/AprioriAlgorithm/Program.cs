/*
 * Implementation of the Apriori Algorithm 
 * Created as part of the Data Mining faculty at the School of Applied Informatics and Mathematics, Warsaw University of Life Sciences
 * Author: Jakub Kowalik
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AprioriAlgorithm
{
    public static class Utility
    {
        public static int[] Union(int[] a, int[] b)
        {
            List<int> c = new List<int>();
            //Make sure a's length is always greater or equal than b's
            if (a.Length < b.Length)
            {
                var tmp = a;
                a = b;
                b = tmp;
            }
            for (int i = 0; i < a.Length; i++)
            {
                c.Add(a[i]);
            }
            for (int i = 0; i < b.Length; i++)
            {
                if (!Contains(c.ToArray(), b[i]))
                {
                    c.Add(b[i]);
                }
            }
            int[] result = c.ToArray();
            Array.Sort(result);
            return result;
        }
        public static bool EqualSets(int[] a, int[] b)
        {
            if (a.Length > b.Length || a.Length < b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static int[] Intersection(int[] a, int[] b)
        {
            List<int> c = new List<int>();
            //Make sure a's length is always greater or equal than b's
            if (a.Length < b.Length)
            {
                var tmp = a;
                a = b;
                b = tmp;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (Contains(b, a[i]) && !Contains(c.ToArray(), a[i]))
                {
                    c.Add(a[i]);
                }
            }
            for (int i = 0; i < b.Length; i++)
            {
                if (Contains(a, b[i]) && !Contains(c.ToArray(), b[i]))
                {
                    c.Add(b[i]);
                }
            }
            int[] result = c.ToArray();
            Array.Sort(result);
            return result;
        }
        public static int[] Subset(int[] set, int start, int end)
        {
            int[] result = new int[end - start + 1];
            if (end > set.Length - 1)
            {
                return null;
            }
            int j = 0;
            int i = start;
            while (i <= end && j < result.Length)
            {
                result[j] = set[i];
                i++;
                j++;
            }
            return result;
        }
        public static bool IsSubset(int[] child, int[] parent)
        {
            if (child.Length > parent.Length)
            {
                return false;
            }
            foreach (int i in child)
            {
                if (!Contains(parent, i))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool Contains(int[] Array, int value)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i] == value)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class Database : IEnumerable
    {
        public List<Item> Transactions;
        public int TransactionCount;
        public int ItemCount;
        public static int[] ItemIDs;

        public Database(int Count, int itemChoiceCount)
        {
            TransactionCount = Count;
            Transactions = new List<Item>();

            ItemIDs = GenerateItems(itemChoiceCount);
            //Generating transactions
            for (int i = 0; i < Count; i++)
            {
                Console.Write("Current transaction " + (i + 1) + "\r");
                Random rand = new Random();
                //Randomizing the number of transactions
                Item t = new Item(rand.Next(1, itemChoiceCount + 1), itemChoiceCount);
                Transactions.Add(t);
            }
            Console.WriteLine();
        }
        public static int[] GenerateItems(int upperBound)
        {
            int[] idSet = new int[upperBound];
            for (int i = 0; i < upperBound; i++)
            {
                idSet[i] = i + 1;
            }
            return idSet;
        }
        //Calculates P(t|u)
        public float CalculateConfidence(Item t, Item u)
        {
            if (t.Support != 0 && u.Support != 0)
            {
                int[] res = Utility.Intersection(t.Array, u.Array);
                float x = CalculateSupport(new Item(res));
                float y = u.Support;
                return x / y;
            }
            return 0;
        }
        //Calculates P(t)
        public float CalculateSupport(Item t)
        {
            float support = 0;
            foreach (Item transaction in Transactions)
            {

                if (Utility.IsSubset(t.Array, transaction.Array))
                {
                    support++;
                }
            }
            t.Support = support / TransactionCount;
            return t.Support;
        }
        public void Show()
        {
            foreach (var item in Transactions)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Transactions).GetEnumerator();
        }
    }

    public class Item
    {
        public int[] Array;
        public float Support;
        public readonly int itemCount;

        public Item(int itemCount, int itemChoiceCount)
        {
            this.itemCount = itemCount;
            Array = GenerateTransaction(itemCount, itemChoiceCount);
        }
        public Item(params int[] transaction)
        {
            Array = transaction;
            itemCount = transaction.Length;
        }
        public Item(int[] transaction, float Support)
        {
            this.Support = Support;
            Array = transaction;
            itemCount = transaction.Length;
        }
        public int this[int i] => Array[i];

        private int[] GenerateTransaction(int itemChoiceCount, int SleepTime = 14)
        {
            int[] itemChoice = Database.ItemIDs;    //Our array of available products
            if (itemCount > itemChoice.Length)
            {
                throw new Exception("Cannot create a proper shopping basket!");
            }

            //Transaction generation
            //Thread.Sleep(SleepTime);
            Random rand = new Random();
            int[] basket = new int[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                int item = itemChoice[rand.Next(0, itemChoice.Length)];
                while (Utility.Contains(basket, item))
                {
                    item = itemChoice[rand.Next(0, itemChoice.Length)];
                }
                basket[i] = item;
            }
            System.Array.Sort(basket);
            return basket;
        }
        public override string ToString()
        {
            string array = "[ ";
            for (int i = 0; i < itemCount; i++)
            {
                if (i < itemCount - 1)
                {
                    array += this[i] + ", ";
                }
                else
                {
                    array += this[i];
                }
            }
            array += " ]";
            return array;
        }
    }

    public class Program
    {
        static List<List<Item>> Apriori(Database DB, float minSupport, float minConfidence)
        {
            List<List<Item>> result = new List<List<Item>>();
            //Scanning DB once to get frequent 1 - itemsets
            result.Add(GetL1FrequentItems(DB, minSupport));

            for (int i = 1; result[i - 1].Count != 0; i++)
            {
                List<Item> candidates = new List<Item>();
                candidates = GenerateCandidates(result[i - 1], DB);
                List<Item> LiResult = new List<Item>();
                foreach (Item item in candidates)
                {
                    if (item.Support >= minSupport)
                    {
                        LiResult.Add(item);
                    }
                }
                result.Add(LiResult);
            }
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine("Frequent transaction of length " + (i + 1));
                foreach (var TList in result[i])
                {
                    Console.WriteLine(TList.ToString() + " with support = " + TList.Support);
                }
            }
            return result;
        }
        static List<Item> GenerateCandidates(List<Item> previous, Database DB)
        {
            List<Item> candidates = new List<Item>();
            for (int i = 0; i < previous.Count - 1; i++)
            {
                int[] firstItem = previous[i].Array;

                for (int j = i + 1; j < previous.Count; j++)
                {
                    int[] secondItem = previous[j].Array;
                    int[] candidate = GenerateCandidate(firstItem, secondItem);

                    if (candidate != null)
                    {
                        Item t = new Item(candidate);
                        float support = DB.CalculateSupport(t);
                        candidates.Add(t);
                    }
                }
            }
            return candidates;
        }
        static int[] GenerateCandidate(int[] firstItem, int[] secondItem)
        {
            int length = firstItem.Length;

            if (length == 1)
            {
                return Utility.Union(firstItem, secondItem);
            }
            else
            {
                int[] firsSubSet = Utility.Subset(firstItem, 0, length - 1);
                int[] secondSubSet = Utility.Subset(secondItem, 0, length - 1);
                if (Utility.EqualSets(firsSubSet, secondSubSet))
                {
                    int[] temp = { secondItem[length] };
                    return Utility.Union(firstItem, temp);
                }
                return null;
            }
        }
        static List<Item> GetL1FrequentItems(Database DB, float minSupport)
        {
            List<Item> frequentL1Items = new List<Item>();
            int transactionCount = DB.TransactionCount;

            foreach (int item in Database.ItemIDs)
            {
                float support = DB.CalculateSupport(new Item(item));
                if (support >= minSupport)
                {
                    //Console.WriteLine(item.ToString() + " " + support);
                    int[] tmp = new int[1];
                    tmp[0] = item;
                    frequentL1Items.Add(new Item(tmp, support));
                }
            }
            return frequentL1Items;
        }
        static void Main(string[] args)
        {
            Database db = new Database(10000 , 20);
            //var L1 = GetL1FrequentItems(db, 0.6f);
            List<List<Item>> result = Apriori(db, 0.5f, 0);
            Console.ReadKey();
        }
    }
}
