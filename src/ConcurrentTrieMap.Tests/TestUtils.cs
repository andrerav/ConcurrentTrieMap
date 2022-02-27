using System.Linq;
using System;

namespace ConcurrentTrieMap.Tests
{
    public static class TestUtils
	{
		public const string Chars = "0123456789bcdefghjkmnpqrstuvwxyz";

		private static Random random = new Random();
		public static string RandomString(int length)
		{
			return new string(Enumerable.Repeat(Chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public static int RandomInt()
		{
			return random.Next();
		}
	}
}