using Calculation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CalculationFixture {
    [TestClass()]
    public class DataManagerFixture {
        [TestMethod()]
        public void PriorsChestPainTest( ) {
            const StudyModule module = StudyModule.ChestPain;
            double expectedSum = 1.0;
            double[] actual = DataManager.Priors(module);
            double actualSum = 0;
            foreach( double d in actual ) {
                Assert.IsTrue(d >= 0, "Each probability should be nonnegative.");
                Assert.IsTrue(d <= 1, "Each probability should be not exceed 1.");
                actualSum += d;
            }
            Assert.AreEqual(expectedSum, actualSum, "The probabilities should sum to 1.0.");
        }
        [TestMethod()]
        public void PriorsPediatricDyspneaTest( ) {
            const StudyModule module = StudyModule.PediatricDyspnea;
            const double tolerance = 1e-15;
            double expectedSum = 1.0;
            double[] actual = DataManager.Priors(module);
            double actualSum = 0;
            foreach( double d in actual ) {
                Assert.IsTrue(d >= 0, "Each probability should be nonnegative.");
                Assert.IsTrue(d <= 1, "Each probability should be not exceed 1.");
                actualSum += d;
            }
            Assert.AreEqual(expectedSum, actualSum, tolerance, "The probabilities should sum to 1.0.");
        }
    }
}
