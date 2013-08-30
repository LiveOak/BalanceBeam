using Calculation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CalculationFixture
{
    
    
	[TestClass()]
	public sealed class PosteriorFixture {



		[TestMethod()]
		public void ExpectedProportionalReductionInEntropyUncertainty( ) {
			const double tolerance = 1e-15;
			double[] currentProbabilities = { .9, .0125, .0125, .0125, .0125, .0125, .0125, .0125, .0125};
			double[] sensitivities = { .8, .7, .7, .7, .7, .7, .7, .7, .7};
			//double expected = 0.003618364609578100;//Unproportional
			double expected = 0.0047053125398150;//Proportional
			double actual= Posterior.ExpectedProportionalReductionInEntropyUncertainty(currentProbabilities, sensitivities);
			Assert.AreEqual(expected, actual, tolerance);
		}

[TestMethod()]
public void ExpectedProportionalReductionExtremeCase1( ) {
	const double tolerance = 1e-12;
	double[] currentProbabilities = { 0.9999840,0.000002,0.000002,0.000002,0.000002,0.000002,0.000002,0.000002,0.000002 };
	double[] sensitivities = { 0.97, 0.97, 0.97, 0.25, 0.5, 0.5, 0.1, 0.1, 0.8 };
	double expectedRaw= 0.0000291957893046;
	double expectedProportional = 0.0895609235319499;
	double actualRaw = Posterior.ExpectedRawReductionInEntropyUncertainty(currentProbabilities, sensitivities);
	double actualProportional = Posterior.ExpectedProportionalReductionInEntropyUncertainty(currentProbabilities, sensitivities);
	Assert.AreEqual(expectedRaw, actualRaw, tolerance);
	Assert.AreEqual(expectedProportional, actualProportional, tolerance);
}
				
[TestMethod()]
public void ExpectedProportionalReductionExtremeCase2( ) {
	const double tolerance = 1e-12;
	double[] currentProbabilities = { 0.00000000000051272399102566666, 0.0000000000028179448076085058, 0.99974822726247836, 0.0000000058705231666032978, 0.000000032627400850283105, 0.000024132131419987528, 0.0000001297816101535053, 0.0000000020266377764906643, 0.00022747029659905932 };
	double[] sensitivities = { 0.97, 0.97, 0.97, 0.25, 0.5, 0.5, 0.1, 0.1, 0.8 };
	double expectedRaw = 0.0001119128710007;
	double expectedProportional = 0.0320649456221081;
	double actualRaw = Posterior.ExpectedRawReductionInEntropyUncertainty(currentProbabilities, sensitivities);
	double actualProportional = Posterior.ExpectedProportionalReductionInEntropyUncertainty(currentProbabilities, sensitivities);
	Assert.AreEqual(expectedRaw, actualRaw, tolerance);
	Assert.AreEqual(expectedProportional, actualProportional, tolerance);
}
	}
}

		
