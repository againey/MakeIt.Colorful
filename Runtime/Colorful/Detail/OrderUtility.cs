/******************************************************************************\
* Copyright Andy Gainey                                                        *
*                                                                              *
* Licensed under the Apache License, Version 2.0 (the "License");              *
* you may not use this file except in compliance with the License.             *
* You may obtain a copy of the License at                                      *
*                                                                              *
*     http://www.apache.org/licenses/LICENSE-2.0                               *
*                                                                              *
* Unless required by applicable law or agreed to in writing, software          *
* distributed under the License is distributed on an "AS IS" BASIS,            *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.     *
* See the License for the specific language governing permissions and          *
* limitations under the License.                                               *
\******************************************************************************/

namespace MakeIt.Colorful.Detail
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
