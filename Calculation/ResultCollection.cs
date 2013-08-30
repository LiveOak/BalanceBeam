using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

namespace Calculation {
	public  class ResultCollection : List<Result> {
		#region Fields
		private readonly DataTable _dt;
		protected readonly StudyModule _module;
		//private ConcurrentBag<Result> _results;
		//private List<Result> _results= new List<Result>();
		#endregion
		#region Properties
		public Int32 ResultCount { get { return base.Count; } }
		#endregion
		#region Constructor
		public ResultCollection( DataTable dt, StudyModule module ) {
			if( dt == null ) throw new ArgumentNullException("dt");
			_dt = dt;
			_module = module;
		}

		#endregion
		#region Public Methods
		public bool? Positive( DataColumn dcEvidence ) {
			if( !ColumnExistsInCollection(dcEvidence) ) return null;

			foreach( Result resultPossible in this ) {
				if( resultPossible.Evidence.ColumnName == dcEvidence.ColumnName ) return resultPossible.PositiveTest;
			}
			throw new InvalidOperationException("Internal Error: Can't find column " + dcEvidence.ColumnName);
		}
		public void AddResult( Result result ) {
			VerifyColumnExistsInTable(result.Evidence, _dt);
			VerifyColumnIsNotDuplicatedInCollection(result.Evidence);
			base.Add(result);
		}
		public static void VerifyColumnExistsInTable( DataColumn dc, DataTable dt ) {
			foreach( DataColumn dcPossible in dt.Columns ) if( dc == dcPossible ) return;
			throw new ArgumentException("dc", "The DataColumn is not a memeber of the DataTable.");
		}
		public bool ColumnExistsInCollection( DataColumn dcProposed ) {
			foreach( Result resultPossible in this ) {
				if( resultPossible.Evidence.ColumnName == dcProposed.ColumnName ) return true;
			}
			return false;
		}
		public void VerifyColumnIsNotDuplicatedInCollection( DataColumn dcProposed ) {
			if( ColumnExistsInCollection(dcProposed) ) throw new ArgumentException("dcProposed", string.Format("A column with the name '{0}' already exists in the collection.", dcProposed.ColumnName));
		}
		//public void VerifyColumnIsNotDuplicatedInCollection( DataColumn dcProposed  ) {
		//   foreach( Result resultPossible in this ) {
		//      if( resultPossible.Evidence.ColumnName == dcProposed.ColumnName ) throw new ArgumentException("dcProposed", string.Format("A column with the name '{0}' already exists in the collection.", dcProposed.ColumnName));
		//   }
		//}
		public double TorqueSum( byte diagnosisID1, byte diagnosisID2 ) {
			Grinder grinder = new Grinder(_module);
			double sum = 0;
			foreach( Result result in this ) {
				sum += grinder.Torque(result.Evidence, result.PositiveTest, diagnosisID1, diagnosisID2);
			}
			return sum;
		}
		//public double TorqueSum( byte diagnosisID1, byte diagnosisID2 ) {
		//   Grinder grinder = new Grinder(_module);
		//   object lockObject = new object();
		//   double sum = 0.0;
		//   List<Result> list = this;
		//   Parallel.ForEach(
		//      list,// The values to be aggregated 
		//      ( ) => 0.0,// The local initial partial result
		//      ( result, loopState, localSum ) => {// The loop body
		//         localSum += grinder.Torque(result.Evidence, result.PositiveTest, diagnosisID1, diagnosisID2);
		//         return localSum;
		//      },
		//      ( localPartialSum ) => {// The final step of each local context 
		//         lock( lockObject ) {
		//            sum += localPartialSum;
		//         }
		//      }
		//   );
		//   return sum;
		//}
		public double MaxAbsoluteTorqueComponent( byte diagnosisID1, byte diagnosisID2 ) {
			Grinder grinder = new Grinder(_module);
			double max = 0;
			foreach( Result result in this ) {
				double current = Math.Abs(grinder.Torque(result.Evidence, result.PositiveTest, diagnosisID1, diagnosisID2));
				if( current > max ) max = current;
			}
			return max;
		}
		public double MaxAbsoluteTorqueComponent( byte[] diagnoses ) {
			Grinder grinder = new Grinder(_module);
			double max = 0;
			for( Int32 diagnosis1Index = 0; diagnosis1Index < diagnoses.Length; diagnosis1Index++ ) {
				for( Int32 diagnosis2Index = diagnosis1Index + 1; diagnosis2Index < diagnoses.Length; diagnosis2Index++ ) {
					foreach( Result result in this ) {
						double current = Math.Abs(grinder.Torque(result.Evidence, result.PositiveTest, diagnoses[diagnosis1Index], diagnoses[diagnosis2Index]));
						if( current > max ) 
							max = current;
					}
				}
			}
			return max;
		}
		public static string ToOddsString( double torqueSum, Int32 roundingDigits ) {
			double odds = Math.Exp(torqueSum);
			string leftSide = "1";
			string rightSide = "1";
			switch( Math.Sign(torqueSum) ) {
				case -1: leftSide = Math.Round(1 / odds, roundingDigits).ToString(); break;
				case 1: rightSide = Math.Round(odds, roundingDigits).ToString(); break;
			}
			return string.Format("{0} to {1}", leftSide, rightSide);
		}
		#endregion
		#region Private Methods
		#endregion
	}
}
//public ResultPair[] ResultPairCombination( ) {
//   ResultPair[] pairs = new ResultPair[(Int32)(ResultCount * (ResultCount - 1) *.5)];
//   Int32 tally = 0;
//   for( Int32 result1Index = 0; result1Index < (ResultCount - 1); result1Index++ ) {
//      for( Int32 result2Index = (result1Index+1); result2Index < (ResultCount); result2Index++ ) {
//         Result result1 = Results[result1Index];
//         Result result2 = Results[result2Index];
//         pairs[tally] = new ResultPair(result1, result2);
//         tally += 1;
//      }
//   }
//   Trace.Assert(tally == pairs.Length, "The tally should match the number of elements.");
//   return pairs;
//}