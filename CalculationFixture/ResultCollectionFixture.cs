using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculation;


namespace CalculationFixture {
	[TestClass()]
	public class ResultCollectionFixture {
		#region Fields
		private const StudyModule _area = StudyModule.ChestPain;
		private BeamDataSet.tblChestPainDataTable _dt;
		private ResultCollection _collection;
		#endregion
		#region Test Attributes
		//private TestContext testContextInstance;
		//public TestContext TestContext {
		//get { return testContextInstance; }
		//set { testContextInstance = value; }
		//}
		#endregion
		#region Setup, TearDown and Constructor
		//[ClassInitialize()]
		//public static void ClassInitialize(TestContext testContext) {
		//}
		//[ClassCleanup()]
		//public static void ClassCleanup() {
		//}
		[TestInitialize()]
		public void TestInitialize( ) {
			_dt = DataManager.BeamDataSet.tblChestPain;
			_collection = new ResultCollection(_dt, _area);
		}
		[TestCleanup()]
		public void TestCleanup( ) {
			_collection = null;
			_dt = null;
		}
		#endregion
		[TestMethod()]
		public void MaxAbsoluteTorqueComponentTest( ) {
			const byte diagnosis1 = (byte)ChestPainDiagnosis.Pe;
			const byte diagnosis2 = (byte)ChestPainDiagnosis.Pneum;
			const double expected = 0F; // TODO: Initialize to an appropriate value
			double actual = _collection.MaxAbsoluteTorqueComponent(diagnosis1, diagnosis2);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod()]
		public void TorqueSumTest( ) {
			const byte diagnosis1 = (byte)ChestPainDiagnosis.Pe;
			const byte diagnosis2 = (byte)ChestPainDiagnosis.Pneum;
			const double expected = 0F; // TODO: Initialize to an appropriate value
			double actual = _collection.TorqueSum(diagnosis1, diagnosis2);
			Assert.AreEqual(expected, actual);
		}
		#region ResultPair Combination Tests
		//[TestMethod()]
		//public void ResultPairCombinationTestK2( ) {
		//   Result result1 = new Result(_dt.LocationBackColumn, true);
		//   Result result2 = new Result(_dt.LocationNeckJawArmColumn, false);
		//   _collection.AddResult(result1);
		//   _collection.AddResult(result2);

		//   ResultPair[] expected = { new ResultPair(result1, result2) };
		//   ResultPair[] actual = _collection.ResultPairCombination();
		//   CompareResultPairs(expected, actual);
		//}
		//[TestMethod()]
		//public void ResultPairCombinationTestK3( ) {
		//   Result result1 = new Result(_dt.LocationBackColumn, true);
		//   Result result2 = new Result(_dt.LocationNeckJawArmColumn, false);
		//   Result result3 = new Result(_dt.LocationPosteriorThoracicColumn, false);
		//   _collection.AddResult(result1);
		//   _collection.AddResult(result2);
		//   _collection.AddResult(result3);

		//   ResultPair[] expected = { new ResultPair(result1, result2), new ResultPair(result1, result3), new ResultPair(result2, result3) };
		//   ResultPair[] actual = _collection.ResultPairCombination();
		//   CompareResultPairs(expected, actual);
		//}
		//[TestMethod()]
		//public void ResultPairCombinationTestK4( ) {
		//   Result result1 = new Result(_dt.LocationBackColumn, true);
		//   Result result2 = new Result(_dt.LocationNeckJawArmColumn, false);
		//   Result result3 = new Result(_dt.LocationPosteriorThoracicColumn, false);
		//   Result result4 = new Result(_dt.ThoraxRonchiColumn, false);
		//   _collection.AddResult(result1);
		//   _collection.AddResult(result2);
		//   _collection.AddResult(result3);
		//   _collection.AddResult(result4);

		//   ResultPair[] expected = { new ResultPair(result1, result2), new ResultPair(result1, result3), new ResultPair(result1, result4), new ResultPair(result2, result3), new ResultPair(result2, result4), new ResultPair(result3, result4) };
		//   ResultPair[] actual = _collection.ResultPairCombination();
		//   CompareResultPairs(expected, actual);
		//}
		//[TestMethod()]
		//public void ResultPairCombinationTestK5( ) {
		//   Result result1 = new Result(_dt.LocationBackColumn, true);
		//   Result result2 = new Result(_dt.LocationNeckJawArmColumn, false);
		//   Result result3 = new Result(_dt.LocationPosteriorThoracicColumn, false);
		//   Result result4 = new Result(_dt.ThoraxRonchiColumn, true);
		//   Result result5 = new Result(_dt.CardioDeepVeinThrombosisColumn, false);
		//   _collection.AddResult(result1);
		//   _collection.AddResult(result2);
		//   _collection.AddResult(result3);
		//   _collection.AddResult(result4);
		//   _collection.AddResult(result5);

		//   ResultPair[] expected = { new ResultPair(result1, result2), new ResultPair(result1, result3), new ResultPair(result1, result4), new ResultPair(result1, result5), new ResultPair(result2, result3), new ResultPair(result2, result4), new ResultPair(result2, result5), new ResultPair(result3, result4), new ResultPair(result3, result5), new ResultPair(result4, result5) };
		//   ResultPair[] actual = _collection.ResultPairCombination();
		//   CompareResultPairs(expected, actual);
		//}
		#endregion
		#region Tests that throw exceptions
		[TestMethod(), ExpectedException(typeof(ArgumentException))]
		public void DuplicateColumn( ) {
			Result result1 = new Result(_dt.LocationBackColumn, true);
			Result result2 = new Result(_dt.LocationBackColumn, true);
			_collection.AddResult(result1);
			_collection.AddResult(result2);
		}
		[TestMethod(), ExpectedException(typeof(ArgumentException))]
		public void DuplicateColumnSameReference( ) {
			Result result1 = new Result(_dt.LocationBackColumn, true);
			_collection.AddResult(result1);
			_collection.AddResult(result1);
		}
		#endregion
		#region Helpers
		public static void CompareResultPairs( ResultPair[] expectedArray, ResultPair[] actualArray ) {
			Assert.AreEqual(expectedArray.Length, actualArray.Length, "The array should have the correct length.");
			for( Int32 i = 0; i < expectedArray.Length; i++ ) {
				ResultPair expectedPair = expectedArray[i];
				Result expectedResult1 = expectedPair.Result1;
				Result expectedResult2 = expectedPair.Result2;
				ResultPair actualPair = actualArray[i];
				Result actualResult1 = actualPair.Result1;
				Result actualResult2 = actualPair.Result2;

				Assert.AreEqual(expectedResult1.Evidence, actualResult1.Evidence, "The DataColumn for Result1 should be correct.");
				Assert.AreEqual(expectedResult2.Evidence, actualResult2.Evidence, "The DataColumn for Result2 should be correct.");
				Assert.AreEqual(expectedResult1.PositiveTest, actualResult1.PositiveTest, "The PositiveTest for Result1 should be correct.");
				Assert.AreEqual(expectedResult2.PositiveTest, actualResult2.PositiveTest, "The PositiveTest for Result2 should be correct.");
			}
		}
		#endregion
	}
}