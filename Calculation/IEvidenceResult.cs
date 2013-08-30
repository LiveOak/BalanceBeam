using System;
using System.Data;

namespace Calculation {
	public interface IEvidenceResult {
		DataColumn Evidence { get; }
		bool  Result { get; }
	}
}
