using System;
using System.Data;

namespace Calculation {
	public sealed class Result {
		private readonly DataColumn _evidence;
		private readonly bool? _positive;

		public DataColumn Evidence { get { return _evidence; } }
		public bool? PositiveTest { get { return _positive; } }

		public Result( DataColumn evidence, bool? positive ) {
			if( evidence == null ) throw new ArgumentNullException("evidence");
			_evidence = evidence;
			_positive = positive;
		}
	}
}