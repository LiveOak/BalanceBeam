using System;
using System.Data;
using System.Diagnostics;

namespace Calculation {
    public sealed class Posterior {
        #region Fields
        //private readonly SensitivityDataSet _ds;
        //private readonly DataTable _dt;
        private readonly StudyModule _studyArea;
        private readonly Grinder _grinder;
        #endregion
        #region Properties
        #endregion
        #region Constructor
        //public Grinder( StudyArea studyArea, SensitivityDataSet ds ) {
        public Posterior( StudyModule studyArea ) {
            _studyArea = studyArea;
            //_dt = DataManager.ChooseCorrectTable(_studyArea);
            _grinder = new Grinder(_studyArea);
        }
        #endregion
        #region Public Methods
        public double[] UpdatePosteriors( ResultCollection results ) {
            DataTable dtPosterior = DataManager.ChooseCorrectTablePosterior(_studyArea);

            Int32 diagnosisCount = dtPosterior.Rows.Count;
            double[] updatedPosteriors = new double[diagnosisCount];

            //Load the priors
            double[] priors = DataManager.Priors(_studyArea);
            for( Int32 diseaseIndex = 0; diseaseIndex < diagnosisCount; diseaseIndex++ ) {

                //DataRow drPosterior = dtPosterior.Rows[diseaseIndex];//Consider: .Rows.Find(diagnosisID);
                //drPosterior[ConstantsCalculation.ColumnNamePrior] = priors[diseaseIndex];
                dtPosterior.Rows[diseaseIndex][ConstantsCalculation.ColumnNamePrior] = priors[diseaseIndex];
                updatedPosteriors[diseaseIndex] = priors[diseaseIndex];//Load the posterior for the first evidence.
            }

            //Update each evidence/column
            foreach( Result result in results ) {//Cycles through the evidences/columns.
                double[] sensitivities = _grinder.Likelihoods(result.Evidence, result.PositiveTest);
                double posteriorDenominator = PosteriorDenominator(sensitivities, updatedPosteriors);
                DataColumn dcPosterior = dtPosterior.Columns[result.Evidence.ColumnName];
                for( Int32 diagnosisIndex = 0; diagnosisIndex < diagnosisCount; diagnosisIndex++ ) {
                    double newPosterior = (sensitivities[diagnosisIndex] * updatedPosteriors[diagnosisIndex]) / posteriorDenominator;
                    updatedPosteriors[diagnosisIndex] = newPosterior;
                    dtPosterior.Rows[diagnosisIndex][dcPosterior] = newPosterior;
                }
            }

            //Load the final posterior values
            for( Int32 diseaseIndex = 0; diseaseIndex < diagnosisCount; diseaseIndex++ ) {
                //DataRow drSensitivity = dtSensitivity.Rows[diseaseIndex];//Consider: .Rows.Find(diagnosisID);
                //drSensitivity[ConstantsCalculation.ColumnNamePosterior] = updatedPosteriors[diseaseIndex];
                dtPosterior.Rows[diseaseIndex][ConstantsCalculation.ColumnNamePosterior] = updatedPosteriors[diseaseIndex];
            }
            return updatedPosteriors;
        }
        //public double[] CurrentPosteriors( ) {

        //}
        #endregion
        #region Static Public Methods
        public static double PosteriorDenominator( double[] likelihoods, double[] priors ) {
            if( likelihoods.Length != priors.Length ) throw new ArgumentException("The likelihoods and priors must have the same number of elements.");
            double sum = 0;
            for( Int32 diagnosisIndex = 0; diagnosisIndex < likelihoods.Length; diagnosisIndex++ ) {
                sum += likelihoods[diagnosisIndex] * priors[diagnosisIndex];
            }
            return sum;
        }
        public static double[] UnnamedVariable( double[] posteriors ) {
            if( posteriors == null ) throw new ArgumentNullException("posteriors");
            const Int32 logBase = 2;
            double[] uncertainties = new double[posteriors.Length];
            for( Int32 i = 0; i < uncertainties.Length; i++ ) {
                uncertainties[i] = -Math.Log(posteriors[i], logBase);
            }
            return uncertainties;
        }
        public static double[] Suprisal( double[] posteriors ) {
            if( posteriors == null ) throw new ArgumentNullException("posteriors");
            const Int32 logBase = 2;
            double[] surprisals = new double[posteriors.Length];
            for( Int32 i = 0; i < surprisals.Length; i++ ) {
                surprisals[i] = -posteriors[i] * Math.Log(posteriors[i], logBase);
            }
            return surprisals;
        }
        public static double[] EntropyUncertainties( double[] posteriors ) {
            if( posteriors == null ) throw new ArgumentNullException("posteriors");
            const Int32 logBase = 2;
            double[] uncertainties = new double[posteriors.Length];
            for( Int32 i = 0; i < uncertainties.Length; i++ ) {
                double thisDisease = -posteriors[i] * Math.Log(posteriors[i], logBase);
                double notThisDisease = (1 - posteriors[i]) * Math.Log(1 - posteriors[i], logBase);
                uncertainties[i] = thisDisease - notThisDisease;
            }
            return uncertainties;
        }
        public static double ExpectedSuprisal( double[] posteriors ) {
            if( posteriors == null ) throw new ArgumentNullException("posteriors");
            double[] entropyUncertainties = Suprisal(posteriors);
            double sum = 0;
            for( Int32 i = 0; i < posteriors.Length; i++ ) {
                sum += entropyUncertainties[i];
            }
            return sum;
        }
        public static double ExpectedRawReductionInEntropyUncertainty( double[] currentProbabilities, double[] sensitivitiesForSingleEvidence ) {
            if( currentProbabilities == null ) throw new ArgumentNullException("posteriors");
            if( sensitivitiesForSingleEvidence == null ) throw new ArgumentNullException("sensitivities");
            if( currentProbabilities.Length != sensitivitiesForSingleEvidence.Length ) throw new ArgumentOutOfRangeException("sensitivities", sensitivitiesForSingleEvidence, "The sensitivity array should have the same number of elements as the posterior array.");
            const double logBase = 2.0;

            double[] probPositiveTest = new double[currentProbabilities.Length];
            double probPositiveTestSum = 0;
            double[] probNegativeTest = new double[currentProbabilities.Length];
            double probNegativeTestSum = 0;
            double entropyUncertaintyBeforeTestSum = 0;  //ie, Individual surprises.
            for( Int32 dxIndex = 0; dxIndex < currentProbabilities.Length; dxIndex++ ) {
                probPositiveTest[dxIndex] = currentProbabilities[dxIndex] * sensitivitiesForSingleEvidence[dxIndex];
                probPositiveTestSum += probPositiveTest[dxIndex];

                probNegativeTest[dxIndex] = currentProbabilities[dxIndex] * (1 - sensitivitiesForSingleEvidence[dxIndex]);
                probNegativeTestSum += probNegativeTest[dxIndex];

                entropyUncertaintyBeforeTestSum += currentProbabilities[dxIndex] * -Math.Log(currentProbabilities[dxIndex], logBase);
            }

            double surprisalIfPositiveTestSum = 0;
            double surprisalIfNegativeTestSum = 0;
            for( Int32 dxIndex = 0; dxIndex < currentProbabilities.Length; dxIndex++ ) {
                double probDxIfPositiveTest = probPositiveTest[dxIndex] / probPositiveTestSum;
                surprisalIfPositiveTestSum += -probDxIfPositiveTest * Math.Log(probDxIfPositiveTest, logBase);

                double probDxIfNegativeTest = probNegativeTest[dxIndex] / probNegativeTestSum;
                surprisalIfNegativeTestSum += -probDxIfNegativeTest * Math.Log(probDxIfNegativeTest, logBase);
            }

            double surprisalIfPositiveTestSumWeighted = surprisalIfPositiveTestSum * probPositiveTestSum;
            double surprisalIfNegativeTestSumWeighted = surprisalIfNegativeTestSum * probNegativeTestSum;

            double expectedReductionInEntropyUncertaintyIfEvidenceCollected = surprisalIfPositiveTestSumWeighted + surprisalIfNegativeTestSumWeighted;
            double expectedReductionInEntropyUncertainty = entropyUncertaintyBeforeTestSum - expectedReductionInEntropyUncertaintyIfEvidenceCollected;

            return expectedReductionInEntropyUncertainty;
            //double expectedProportionalReductionInEntropyUncertainty;
        }
        public static double ExpectedProportionalReductionInEntropyUncertainty( double[] currentProbabilities, double[] sensitivitiesForSingleEvidence ) {
            if( currentProbabilities == null ) throw new ArgumentNullException("posteriors");
            if( sensitivitiesForSingleEvidence == null ) throw new ArgumentNullException("sensitivities");
            if( currentProbabilities.Length != sensitivitiesForSingleEvidence.Length ) throw new ArgumentOutOfRangeException("sensitivities", sensitivitiesForSingleEvidence, "The sensitivity array should have the same number of elements as the posterior array.");
            const double logBase = 2.0;

            double[] probPositiveTest = new double[currentProbabilities.Length];
            double probPositiveTestSum = 0;
            double[] probNegativeTest = new double[currentProbabilities.Length];
            double probNegativeTestSum = 0;
            double entropyUncertaintyBeforeTestSum = 0;  //ie, Individual surprises.
            for( Int32 dxIndex = 0; dxIndex < currentProbabilities.Length; dxIndex++ ) {
                probPositiveTest[dxIndex] = currentProbabilities[dxIndex] * sensitivitiesForSingleEvidence[dxIndex];
                probPositiveTestSum += probPositiveTest[dxIndex];

                probNegativeTest[dxIndex] = currentProbabilities[dxIndex] * (1 - sensitivitiesForSingleEvidence[dxIndex]);
                probNegativeTestSum += probNegativeTest[dxIndex];

                entropyUncertaintyBeforeTestSum += currentProbabilities[dxIndex] * -Math.Log(currentProbabilities[dxIndex], logBase);
            }

            double surprisalIfPositiveTestSum = 0;
            double surprisalIfNegativeTestSum = 0;
            for( Int32 dxIndex = 0; dxIndex < currentProbabilities.Length; dxIndex++ ) {
                double probDxIfPositiveTest = probPositiveTest[dxIndex] / probPositiveTestSum;
                surprisalIfPositiveTestSum += -probDxIfPositiveTest * Math.Log(probDxIfPositiveTest, logBase);

                double probDxIfNegativeTest = probNegativeTest[dxIndex] / probNegativeTestSum;
                surprisalIfNegativeTestSum += -probDxIfNegativeTest * Math.Log(probDxIfNegativeTest, logBase);
            }

            double surprisalIfPositiveTestSumWeighted = surprisalIfPositiveTestSum * probPositiveTestSum;
            double surprisalIfNegativeTestSumWeighted = surprisalIfNegativeTestSum * probNegativeTestSum;

            double expectedReductionInEntropyUncertaintyIfEvidenceCollected = surprisalIfPositiveTestSumWeighted + surprisalIfNegativeTestSumWeighted;
            double expectedReductionInEntropyUncertainty = entropyUncertaintyBeforeTestSum - expectedReductionInEntropyUncertaintyIfEvidenceCollected;

            double expectedProportionalReductionInEntropyUncertainty = expectedReductionInEntropyUncertainty / entropyUncertaintyBeforeTestSum;
            return expectedProportionalReductionInEntropyUncertainty;
            //double expectedProportionalReductionInEntropyUncertainty;
        }
        public static double[] RescaleToUnity( double[] unscaledProbabilities ) {
            double sum = 0;
            double[] scaled = new double[unscaledProbabilities.Length];
            for( Int32 i = 0; i < unscaledProbabilities.Length; i++ ) {
                sum += unscaledProbabilities[i];
            }
            for( Int32 i = 0; i < scaled.Length; i++ ) {
                scaled[i] = unscaledProbabilities[i] / sum;
            }
            return scaled;
        }
        #endregion
        #region Private Methods
        private double[] ExtractUpdatedPosterior( DataTable dtPosterior, DataColumn dcUpdatedEvidence ) {
            double[] posteriors = new double[dtPosterior.Rows.Count];
            for( Int32 diseaseIndex = 0; diseaseIndex < dtPosterior.Rows.Count; diseaseIndex++ ) {
                DataRow dr = dtPosterior.Rows[diseaseIndex];
                object value = dr[dcUpdatedEvidence];
                Trace.Assert(!DBNull.Value.Equals(value), "The datatable's cell should not be null.");
                posteriors[diseaseIndex] = (double)value;
            }
            return posteriors;
        }
        #endregion
    }
}
//private double[] ExtractSensitivities( DataColumn dcEvidence , DataTable dtSensitivity) {
//   double[] sensitivies = new double[dtSensitivity.Rows.Count];
//   for( Int32 diagnosisIndex = 0; diagnosisIndex < sensitivies.Length; diagnosisIndex++ ) {
//      sensitivies[diagnosisIndex]
//   }
//   throw new NotImplementedException();
//}
