using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculation {
	public sealed class ResultPair {
		private readonly Result _result1;
		private readonly Result _result2;

		public Result Result1 { get { return _result1; } }
		public Result Result2 { get { return _result2; } }

		public ResultPair( Result result1, Result result2 ) {
			//if(result1.PositiveTest != result2.PositiveTest) throw new arg
			_result1 = result1;
			_result2 = result2;
		}
	}
}