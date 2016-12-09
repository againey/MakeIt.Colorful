/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

namespace Experilous.MakeItColorful.Detail
{
	public static class OrderUtility
	{
		public static bool AreOrdered(float lhs0, float lhs1, float lhs2, float lhs3, float rhs0, float rhs1, float rhs2, float rhs3)
		{
			return lhs0 < rhs0 || lhs0 == rhs0 && (lhs1 < rhs1 || lhs1 == rhs1 && (lhs2 < rhs2 || lhs2 == rhs2 && (lhs3 < rhs3)));
		}

		public static bool AreOrdered(float lhs0, float lhs1, float lhs2, float lhs3, float lhs4, float rhs0, float rhs1, float rhs2, float rhs3, float rhs4)
		{
			return lhs0 < rhs0 || lhs0 == rhs0 && (lhs1 < rhs1 || lhs1 == rhs1 && (lhs2 < rhs2 || lhs2 == rhs2 && (lhs3 < rhs3 || lhs3 == rhs3 && (lhs4 < rhs4))));
		}

		public static int Compare(float lhs0, float lhs1, float lhs2, float lhs3, float rhs0, float rhs1, float rhs2, float rhs3)
		{
			if (lhs0 != rhs0) return lhs0 < rhs0 ? -1 : +1;
			if (lhs1 != rhs1) return lhs1 < rhs1 ? -1 : +1;
			if (lhs2 != rhs2) return lhs2 < rhs2 ? -1 : +1;
			if (lhs3 != rhs3) return lhs3 < rhs3 ? -1 : +1;
			return 0;
		}

		public static int Compare(float lhs0, float lhs1, float lhs2, float lhs3, float lhs4, float rhs0, float rhs1, float rhs2, float rhs3, float rhs4)
		{
			if (lhs0 != rhs0) return lhs0 < rhs0 ? -1 : +1;
			if (lhs1 != rhs1) return lhs1 < rhs1 ? -1 : +1;
			if (lhs2 != rhs2) return lhs2 < rhs2 ? -1 : +1;
			if (lhs3 != rhs3) return lhs3 < rhs3 ? -1 : +1;
			if (lhs4 != rhs4) return lhs4 < rhs4 ? -1 : +1;
			return 0;
		}
	}
}
