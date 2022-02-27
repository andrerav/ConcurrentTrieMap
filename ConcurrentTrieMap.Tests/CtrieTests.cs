using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;

namespace ConcurrentTrieMap.Tests
{
    [TestClass]
	public class CtrieTests
	{
        protected static Dictionary<string, int> Fasit = new Dictionary<string, int>();
        private const int TrieWordMaxLength = 12;
        private const int TrieWords = 8000;

		[ClassInitialize]
		public static void CtrieTestsInitalize(TestContext context)
		{
			var r = new Random();
            int i = 1;
			while (true)
			{
				Fasit[TestUtils.RandomString(r.Next(TrieWordMaxLength))] = i;
				if (Fasit.Count >= TrieWords) break;
                i++;
			}
		}

        /// <summary>
        /// Builds a larger trie structure and validates its structure
        /// </summary>
        [TestMethod]
        public void ValidateCtrie01()
        {
            CtrieMap<int> ctrie = BuildLargeCtrie();
            foreach (var kvp in Fasit)
            {
                Assert.IsNotNull(ctrie.GetValue(kvp.Key));
                Assert.AreEqual(kvp.Value, ctrie.GetValue(kvp.Key));
            }
        }

        /// <summary>
        /// Removes arbitrary keys from the trie and then validates that they have been 
        /// removed correctly.
        /// </summary>
        /// <param name="divisor"></param>
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [TestMethod]
        public void ValidateCtrie02(int divisor)
        {
            CtrieMap<int> ctrie = BuildLargeCtrie();
            int i = 0;
            var removedKeys = new HashSet<string>(Fasit.Count / divisor);

            // Remove half the keys
            foreach (var kvp in Fasit)
            {
                if (i % divisor == 0)
                {
                    ctrie.Remove(kvp.Key);
                    removedKeys.Add(kvp.Key);
                }

                i++;
            }

            foreach (var kvp in Fasit)
            {
                if (removedKeys.Contains(kvp.Key))
                {
                    var removedNode = ctrie.GetNodeByKey(kvp.Key);
                    if (removedNode != null)
                    {
                        Assert.IsFalse(removedNode.HasValue);
                    }
                }
                else
                {
                    if (kvp.Value != ctrie.GetValue(kvp.Key))
                    {
                        this.ToString();
                    }
                    Assert.IsNotNull(ctrie.GetNodeByKey(kvp.Key));
                    Assert.AreEqual(kvp.Value, ctrie.GetValue(kvp.Key));
                }
            }
        }

        [TestMethod]
        public void GetNodeByKeyTest01()
        {
            var ctrie = BuildSmallCtrie();
            Assert.AreEqual(1, ctrie.GetNodeByKey("a").Value);
            Assert.AreEqual(2, ctrie.GetNodeByKey("ab").Value);
            Assert.AreEqual(3, ctrie.GetNodeByKey("abc").Value);
            Assert.AreEqual(null, ctrie.GetNodeByKey("abcd")?.Value);
        }

        [TestMethod]
        public void GetNodeByKeyTest02()
        {
            var ctrie = BuildSmallCtrie();
            Assert.IsTrue(ctrie.GetNodeByKey("a").HasValue);
            Assert.IsTrue(ctrie.GetNodeByKey("ab").HasValue);
            Assert.IsTrue(ctrie.GetNodeByKey("abc").HasValue);
            Assert.AreEqual(null, ctrie.GetNodeByKey("abcd")?.Value);
        }

        [TestMethod]
        public void RemoveKeyTest01()
        {
            var ctrie = BuildSmallCtrie();
            ctrie.Remove("ab");
            Assert.IsFalse(ctrie.GetNodeByKey("ab").HasValue);
            Assert.IsTrue(ctrie.GetNodeByKey("abc").HasValue);
            Assert.AreEqual(3, ctrie.GetNodeByKey("abc").Value);
        }

        [TestMethod]
        public void RemoveKeyTest02()
        {
            var ctrie = BuildSmallCtrie();
            ctrie.Remove("abc");
            Assert.AreEqual(null, ctrie.GetNodeByKey("abc"));
            Assert.IsNotNull(ctrie.GetNodeByKey("ab"));
            Assert.AreEqual(1, ctrie.GetNodeByKey("a").Value);
            Assert.AreEqual(2, ctrie.GetNodeByKey("ab").Value);
        }

        [TestMethod]
        public void RemoveKeyTest03()
        {
            var ctrie = BuildSmallCtrie();
            ctrie.Remove("a");
            ctrie.Remove("ab");
            ctrie.Remove("abc");
            Assert.AreEqual(null, ctrie.GetNodeByKey("abc"));
            Assert.AreEqual(null, ctrie.GetNodeByKey("ab"));
            Assert.AreEqual(null, ctrie.GetNodeByKey("a"));
        }

        [TestMethod]
        public void RemoveKeyTest04()
        {
            var ctrie = BuildSmallCtrie();
            ctrie.Add("aba",5);
            ctrie.Remove("a");
            ctrie.Remove("ab");
            ctrie.Remove("abc");
            Assert.IsNull(ctrie.GetNodeByKey("abc"));
            Assert.IsNotNull(ctrie.GetNodeByKey("ab"));
            Assert.IsNotNull(ctrie.GetNodeByKey("a"));
            Assert.AreEqual(5, ctrie.GetNodeByKey("aba").Value);
        }


        [TestMethod]
        public void CountTest01()
        {
            var ctrie = BuildSmallCtrie();
            Assert.AreEqual(3, ctrie.Count);
        }

        [TestMethod]
        public void CountTest02()
        {
            var ctrie = BuildSmallCtrie();
            ctrie.Remove("ab");
            Assert.AreEqual(2, ctrie.Count);
        }

        [TestMethod]
        public void CountTest03()
        {
            var ctrie = BuildSmallCtrie();
            ctrie.Remove("ab");
            ctrie.Add("ab", 2);
            Assert.AreEqual(3, ctrie.Count);
        }


        [TestMethod]
        public void GetNodesByValueTest01()
        {
            var ctrie = BuildSmallCtrie();
            var node1 = ctrie.GetNodesByValue(1).Single();
            var node2 = ctrie.GetNodesByValue(2).Single();
            var node3 = ctrie.GetNodesByValue(3).Single();
            Assert.AreEqual(1, node1.Value);
            Assert.AreEqual(2, node2.Value);
            Assert.AreEqual(3, node3.Value);
        }

        [TestMethod]
        public void GetNodesByValueTest02()
        {
            var ctrie = BuildLargeCtrie();
            var rnd = new Random();
            foreach (var kvp in Fasit.OrderBy(x => rnd.Next()).Take(TrieWords / 10))
            {
                var node = ctrie.GetNodesByValue(kvp.Value).First();
                Assert.AreEqual(node.Value, kvp.Value);
            }
        }

        [TestMethod]
        public void GetNodesByValueTest03()
        {
            var ctrie = BuildLargeCtrie();
            var nodes = ctrie.GetAllNodes().ToList();
            Assert.AreEqual(ctrie.Count, Fasit.Count);
            var rnd = new Random();
            foreach (var kvp in Fasit.OrderBy(x => rnd.Next()).Take(TrieWords / 10))
            {
                var node = nodes.Single(n => n.Value == kvp.Value);
                Assert.AreEqual(node.Value, kvp.Value);
            }
        }

        [TestMethod]
        public void GetKeyTest01()
        {
            var ctrie = BuildSmallCtrie();
            var node1 = ctrie.GetNodesByValue(1).Single();
            var node2 = ctrie.GetNodesByValue(2).Single();
            var node3 = ctrie.GetNodesByValue(3).Single();
            Assert.AreEqual("a", node1.Key);
            Assert.AreEqual("ab", node2.Key);
            Assert.AreEqual("abc", node3.Key);
        }

        [TestMethod]
        public void GetKeyTest02()
        {
            var ctrie = BuildSmallCtrie();
            ctrie.Add("aba", 4);
            var node4 = ctrie.GetNodesByValue(4).Single();
            Assert.AreEqual("aba", node4.Key);
        }

        private static CtrieMap<int> BuildSmallCtrie()
        {
            CtrieMap<int> ctrie = new CtrieMap<int>();
            ctrie.Add("a", 1);
            ctrie.Add("ab", 2);
            ctrie.Add("abc", 3);
            return ctrie;
        }

        private static CtrieMap<int> BuildLargeCtrie()
        {
            var ctrie = new CtrieMap<int>(Environment.ProcessorCount, TestUtils.Chars.Length);
            Parallel.ForEach(Fasit, kvp =>
            {
                ctrie.Add(kvp.Key, kvp.Value);
            });
            return ctrie;
        }
	}
}