using System;
using System.Data;
using System.Diagnostics;

namespace Calculation {
	public sealed class Grinder {
		#region Fields
		//private readonly SensitivityDataSet _ds;
		private readonly DataTable _dt;
		private readonly StudyModule _studyArea;
		#endregion
		#region Properties
		#endregion
		#region Constructor
		//public Grinder( StudyArea studyArea, SensitivityDataSet ds ) {
		public Grinder( StudyModule studyArea ) {
			//if( ds == null ) throw new ArgumentNullException("ds");
			_studyArea = studyArea;
			_dt = DataManager.ChooseCorrectTable(_studyArea);
		}
		#endregion
		#region Public Methods
		//public double Sensitivity( ChestPainDiagnosis diagnosis, DataColumn evidence ) {
		public double Sensitivity( byte diagnosisID, DataColumn evidence ) {
			DataManager.VerifyTestExistsInStudyArea(evidence, _studyArea, _dt);
			switch( _studyArea ) {
				case StudyModule.ChestPain:
				case StudyModule.PediatricDyspnea:
					//SensitivityDataSet.tblChestPainRow dr = _dt.FindByDifagnosisID((byte)diagnosis);
					//DataRow[] drs = _dt.Select(ConstantsCalculation.ColumnNameRowID + "=" + (byte)diagnosis);
					//Trace.Assert(drs.Length == 1, "There should be exactly one row found for the diagnosis in this StudyArea table.");

					DataRow dr = DataManager.ChooseDataRow(_studyArea, diagnosisID);
					double unadjustedTableValue = (double)dr[evidence];
					double avoidsZero = Math.Max(unadjustedTableValue, ConstantsCalculation.MinimumSensitivity);
					double avoidsZeroAndOne = Math.Min(avoidsZero, ConstantsCalculation.MaximumSensitivity);
					return avoidsZeroAndOne;
				default:
					throw new ArgumentOutOfRangeException("_studyArea", _studyArea, "This Study Area was not recognized.");
			}
		}
		public double[] Likelihoods( DataColumn evidence, bool? isPositive ) {
			double[] sensitivities = new double[_dt.Rows.Count];
			for( byte diagnosisIndex = 0; diagnosisIndex < sensitivities.Length; diagnosisIndex++ ) {
				byte diagnosisID = (byte)(diagnosisIndex + 1);
				if( !isPositive.HasValue )
					sensitivities[diagnosisIndex] = .5;
				else if( isPositive.Value )
					sensitivities[diagnosisIndex] = Sensitivity(diagnosisID, evidence);
				else
					sensitivities[diagnosisIndex] = 1 - Sensitivity(diagnosisID, evidence);
			}
			return sensitivities;
		}

		public double Odds( DataColumn evidence, bool? positiveFinding, byte diagnosisID1, byte diagnosisID2 ) {
			double probability1 = Sensitivity(diagnosisID1, evidence);
			double probability2 = Sensitivity(diagnosisID2, evidence);
			if( !positiveFinding.HasValue )
				return 1; //ie, 1:1 odds if no information
			else if( positiveFinding.Value ) {
				return (probability1 / probability2);
			}
			else {
				return ((1 - probability1) / (1 - probability2));
			}
		}
		private double Logit( DataColumn evidence, bool? positiveFinding, byte diagnosisID1, byte diagnosisID2 ) {
			double odds = Odds(evidence, positiveFinding, diagnosisID1, diagnosisID2);
			return Math.Log(odds);//This is the Natural Log; The command for base-10 log is 'Math.Log10'.
		}
		public double Torque( DataColumn evidence, bool? positiveFinding, byte diagnosisID1, byte diagnosisID2 ) {
			return -Logit(evidence, positiveFinding, diagnosisID1, diagnosisID2);
		}


		#endregion

	}
}