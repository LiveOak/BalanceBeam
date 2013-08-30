using System;

namespace Calculation {
	public static class ConstantsCalculation {
		public const string ColumnNameRowID = "DiagnosisID";
		public const string ColumnNameDiagnosis = "DiagnosisName";
		public const string ColumnNamePrior = "Prior";
		public const string ColumnNamePosterior = "Posterior";
		public const double MinimumSensitivity = .001; //0.1%
		public const double MaximumSensitivity = 1 - MinimumSensitivity; //99.9%
	}
}
