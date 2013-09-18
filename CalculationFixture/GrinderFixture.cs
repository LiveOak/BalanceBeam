using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculation;

namespace CalculationFixture {
    [TestClass()]
    public class GrinderFixture {
        #region Fields
        private const StudyModule _studyArea = StudyModule.ChestPain;
        private BeamDataSet _ds;
        private Grinder _target;
        #endregion
        #region Setup, TearDown and Constructor
        [TestInitialize()]
        public void TestInitialize( ) {
            _ds = DataManager.BeamDataSet;
            _target = new Grinder(_studyArea);
        }
        [TestCleanup()]
        public void TestCleanup( ) {
            _ds = null;
            _target = null;
        }
        #endregion
        #region Tests
        [TestMethod()]
        public void SensitivityFluLikeSymptomsGivenPeri( ) {
            byte diagnosis = (byte)ChestPainDiagnosis.Peri;
            DataColumn evidence = _ds.tblChestPain.FindingsFluLikeExcludingFeverColumn;
            const double expected = .8;
            double actual = _target.Sensitivity(diagnosis, evidence);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void SensitivityFluLikeSymptomsGivenPneumonia( ) {
            byte diagnosis = (byte)ChestPainDiagnosis.Pneum;
            DataColumn evidence = _ds.tblChestPain.FindingsFluLikeExcludingFeverColumn;
            const double expected = .9;
            double actual = _target.Sensitivity(diagnosis, evidence);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void OddsPositiveFluLikeSymptomsGivenPeriOverPneumonia( ) {
            byte diagnosis1 = (byte)ChestPainDiagnosis.Peri;
            byte diagnosis2 = (byte)ChestPainDiagnosis.Pneum;
            DataColumn evidence = _ds.tblChestPain.FindingsFluLikeExcludingFeverColumn;
            bool evidencePositive = true;
            const double expected = .8 / .9;
            double actual = _target.Odds(evidence, evidencePositive, diagnosis1, diagnosis2);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void OddsNegativeFluLikeSymptomsGivenPeriOverPneumonia( ) {
            byte diagnosis1 = (byte)ChestPainDiagnosis.Peri;
            byte diagnosis2 = (byte)ChestPainDiagnosis.Pneum;
            DataColumn evidence = _ds.tblChestPain.FindingsFluLikeExcludingFeverColumn;
            bool evidencePositive = false;
            const double expected = .2 / .1;
            double actual = _target.Odds(evidence, evidencePositive, diagnosis1, diagnosis2);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void LogitPositiveFluLikeSymptomsGivenPeriOverPneumonia( ) {
            const double tolerance = 1e-15;
            byte diagnosis1 = (byte)ChestPainDiagnosis.Peri;
            byte diagnosis2 = (byte)ChestPainDiagnosis.Pneum;
            DataColumn evidence = _ds.tblChestPain.FindingsFluLikeExcludingFeverColumn;
            bool evidencePositive = true;
            const double expected = 0.117783035656383;
            double actual = _target.Torque(evidence, evidencePositive, diagnosis1, diagnosis2);
            Assert.AreEqual(expected, actual, tolerance);
        }
        [TestMethod()]
        public void LogitNegativeFluLikeSymptomsGivenPeriOverPneumonia( ) {
            const double tolerance = 1e-15;
            byte diagnosis1 = (byte)ChestPainDiagnosis.Peri;
            byte diagnosis2 = (byte)ChestPainDiagnosis.Pneum;
            DataColumn evidence = _ds.tblChestPain.FindingsFluLikeExcludingFeverColumn;
            bool evidencePositive = false;
            const double expected = -0.693147180559945;
            double actual = _target.Torque(evidence, evidencePositive, diagnosis1, diagnosis2);
            Assert.AreEqual(expected, actual, tolerance);
        }
        #endregion
        #region Tests
        [TestMethod()]
        public void ProbabilityZeroInTable( ) {
            byte diagnosis = (byte)ChestPainDiagnosis.Mi;
            DataColumn evidence = _ds.tblChestPain.LocationBackColumn;
            const double expected = .001;
            double actual = _target.Sensitivity(diagnosis, evidence);
            Assert.AreEqual(expected, actual);
        }
        //[TestMethod()]
        //public void OddsPeriOverPneumoniaGivenFluLikeSymptomsPositive( ) {
        //   ChestPainDiagnosis diagnosis1 = ChestPainDiagnosis.Peri;
        //   ChestPainDiagnosis diagnosis2 = ChestPainDiagnosis.Pneum;
        //   DataColumn evidence = _ds.tblChestPain.FindingsFluLikeExcludingFeverColumn;
        //   StudyArea area = StudyArea.ChestPain;
        //   bool evidencePositive = true;
        //   const double expected = .8 / .9;
        //   double actual = _target.Odds(evidencePositive, diagnosis1, diagnosis2, evidence, area);
        //   Assert.AreEqual(expected, actual);
        //}
        //[TestMethod()]
        //public void OddsPeriOverPneumoniaGivenFluLikeSymptomsNegative( ) {
        //   ChestPainDiagnosis diagnosis1 = ChestPainDiagnosis.Peri;
        //   ChestPainDiagnosis diagnosis2 = ChestPainDiagnosis.Pneum;
        //   DataColumn evidence = _ds.tblChestPain.FindingsFluLikeExcludingFeverColumn;
        //   StudyArea area = StudyArea.ChestPain;
        //   bool evidencePositive = false;
        //   const double expected = .2 / .1;
        //   double actual = _target.Odds(evidencePositive, diagnosis1, diagnosis2, evidence, area);
        //   Assert.AreEqual(expected, actual);
        //}
        //[TestMethod()]
        //public void LogitPeriOverPneumoniaGivenFluLikeSymptomsPositive( ) {
        //   const double tolerance = 1e-15;
        //   ChestPainDiagnosis diagnosis1 = ChestPainDiagnosis.Peri;
        //   ChestPainDiagnosis diagnosis2 = ChestPainDiagnosis.Pneum;
        //   DataColumn evidence = _ds.tblChestPain.FindingsFluLikeExcludingFeverColumn;
        //   StudyArea area = StudyArea.ChestPain;
        //   bool evidencePositive = true;
        //   const double expected = -0.117783035656383;
        //   double actual = _target.Logit(evidencePositive, diagnosis1, diagnosis2, evidence, area);
        //   Assert.AreEqual(expected, actual, tolerance);
        //}
        //[TestMethod()]
        //public void LogitPeriOverPneumoniaGivenFluLikeSymptomsNegative( ) {
        //   const double tolerance = 1e-15;
        //   ChestPainDiagnosis diagnosis1 = ChestPainDiagnosis.Peri;
        //   ChestPainDiagnosis diagnosis2 = ChestPainDiagnosis.Pneum;
        //   DataColumn evidence = _ds.tblChestPain.FindingsFluLikeExcludingFeverColumn;
        //   StudyArea area = StudyArea.ChestPain;
        //   bool evidencePositive = false;
        //   const double expected = 0.693147180559945;
        //   double actual = _target.Logit(evidencePositive, diagnosis1, diagnosis2, evidence, area);
        //   Assert.AreEqual(expected, actual, tolerance);
        //}
        #endregion
    }
}
