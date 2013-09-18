using System;
using System.Data;
using System.Diagnostics;

namespace Calculation {
    public static class DataManager {
        #region Fields
        private static BeamDataSet _ds;
        private static DataColumnCollection _chestPainTests;
        #endregion
        #region Properties
        public static DataColumnCollection ChestPainTests {
            get {
                if( _chestPainTests == null ) _chestPainTests = BeamDataSet.tblChestPain.Columns;
                return _chestPainTests;
            }
        }
        public static BeamDataSet BeamDataSet {
            get {
                if( _ds == null ) {
                    _ds = new BeamDataSet();
                    LoadChestPainTable(_ds);
                    LoadPediatricDyspneaTable(_ds);
                    InitializePosteriorTable(_ds.tblChestPain, _ds.tblChestPainPosterior);
                    InitializePosteriorTable(_ds.tblPediatricDyspnea, _ds.tblPediatricDyspneaPosterior);
                }
                return _ds;
            }
        }
        #endregion
        #region Public Methods
        public static void VerifyTestExistsInStudyArea( DataColumn evidence, StudyModule area, DataTable dt ) {
            if( evidence.ColumnName == ConstantsCalculation.ColumnNameRowID ) throw new ArgumentOutOfRangeException("evidence", evidence, "The column cannot refer to the table's column for the diagnosis RowID.");
            if( evidence.ColumnName == ConstantsCalculation.ColumnNameDiagnosis ) throw new ArgumentOutOfRangeException("evidence", evidence, "The column cannot refer to the table's column for the diagnosis name.");
            //DataTable dt = ChooseCorrectTable(area, ds);
            foreach( DataColumn dc in dt.Columns ) {
                if( dc == evidence ) return;
            }
            throw new ArgumentException("The evidence column " + evidence.ColumnName + " is not included in the Study Area " + area.ToString() + ".");
        }
        public static DataTable ChooseCorrectTable( StudyModule area ) {
            switch( area ) {
                case StudyModule.ChestPain: return BeamDataSet.tblChestPain;
                case StudyModule.PediatricDyspnea: return BeamDataSet.tblPediatricDyspnea;
                default: throw new ArgumentOutOfRangeException("area", area, "This Study Area was not recognized.");
            }
        }
        public static DataTable ChooseCorrectTablePosterior( StudyModule area ) {
            switch( area ) {
                case StudyModule.ChestPain: return BeamDataSet.tblChestPainPosterior;
                case StudyModule.PediatricDyspnea: return BeamDataSet.tblPediatricDyspneaPosterior;
                default: throw new ArgumentOutOfRangeException("area", area, "This Study Area was not recognized.");
            }
        }
        public static DataRow ChooseDataRow( StudyModule area, byte rowID ) {
            DataTable dt = ChooseCorrectTable(area);
            DataRow[] drs = dt.Select(ConstantsCalculation.ColumnNameRowID + "=" + rowID);
            Trace.Assert(drs.Length == 1, "There should be exactly one row found for the diagnosis in this StudyArea table.");

            return drs[0];
        }
        public static DataColumn[] EvidenceColumns( StudyModule area, bool isPosterior, DataTable dt ) {
            DataTable dtDoubleCheck = ChooseCorrectTable(area);
            if( dtDoubleCheck.TableName != dt.TableName ) throw new ArgumentException("The DataTable does not match the module.");
            DataColumn[] columns;
            //if( isPosterior ) columns = new DataColumn[dt.Columns.Count - 4];
            if( isPosterior ) columns = new DataColumn[dt.Columns.Count - 3];
            else columns = new DataColumn[dt.Columns.Count - 2];
            Int32 tally = 0;
            for( Int32 columnIndex = 0; columnIndex < dt.Columns.Count; columnIndex++ ) {
                DataColumn dcPossible = dt.Columns[columnIndex];
                if( dcPossible.ColumnName != ConstantsCalculation.ColumnNameRowID 
                    && dcPossible.ColumnName != ConstantsCalculation.ColumnNameDiagnosis
                    //&& dcPossible.ColumnName != ConstantsCalculation.ColumnNamePrior 
                    && dcPossible.ColumnName != ConstantsCalculation.ColumnNamePosterior ) {

                    columns[tally] = dcPossible;
                    tally += 1;
                }
            }
            Trace.Assert(tally == columns.Length, "All but two of the table's columns should be used (all but four if it's the posterior table.");
            return columns;
        }
        public static string LongerDiagnosisName( StudyModule module, byte diagnosisID ) {
            switch( module ) {
                case StudyModule.ChestPain:
                    switch( diagnosisID ) {
                        case (byte)ChestPainDiagnosis.Mi: return "Myocardial Infarction";
                        case (byte)ChestPainDiagnosis.Ang: return "Angina";
                        case (byte)ChestPainDiagnosis.Taa: return "Thoracic Aortic Aneurism Dissection";
                        case (byte)ChestPainDiagnosis.Peri: return "Pericarditis";
                        case (byte)ChestPainDiagnosis.Ugi: return "Upper GI";
                        case (byte)ChestPainDiagnosis.Pneum: return "Pneumonia";
                        case (byte)ChestPainDiagnosis.Ptx: return "Pneumothorax";
                        case (byte)ChestPainDiagnosis.Musc: return "Musculoskeletal";
                        case (byte)ChestPainDiagnosis.Pe: return "Pulmonary Embolism";
                        default: throw new ArgumentOutOfRangeException("diagnosisID", diagnosisID, "The diagnosisID was not recognized for the ChestPain module.");
                    }
                case StudyModule.PediatricDyspnea:
                    switch( diagnosisID ) {
                        case (byte)PediatricDyspnea.Crp: return "Croup";
                        case (byte)PediatricDyspnea.Bron: return "Bronchitis";
                        case (byte)PediatricDyspnea.Vpn: return "Viral Pneumonia";
                        case (byte)PediatricDyspnea.Bpn: return "Bacterial Pneumonia";
                        case (byte)PediatricDyspnea.Ast: return "Asthma";
                        case (byte)PediatricDyspnea.Fb: return "Foreign Body";
                        case (byte)PediatricDyspnea.Epig: return "Epiglottitis";
                        default: throw new ArgumentOutOfRangeException("diagnosisID", diagnosisID, "The diagnosisID was not recognized for the PediatricDyspnea module.");
                    }
                default: throw new ArgumentOutOfRangeException("module", module, "The Study Area module was not recognized.");
            }
        }
        public static string LongerFeatureName( StudyModule module, string columnNameForFeature ) {
            switch( module ) {
                case StudyModule.ChestPain: return LongerFeatureNamesForChestPain(columnNameForFeature);
                case StudyModule.PediatricDyspnea: return LongerFeatureNamesForPediatricDyspnea(columnNameForFeature);// "TODO: Add longer names for PediatricDyspnea features";
                default: throw new ArgumentOutOfRangeException("module", module, "The Study Area module was not recognized.");
            }
        }
        public static double[] Priors( StudyModule module ) {
            switch( module ) {
                case StudyModule.ChestPain: return new double[] { 0.050, 0.200, 0.100, 0.150, 0.050, 0.100, 0.150, 0.150, 0.050 };
                case StudyModule.PediatricDyspnea: return new double[] { 1 / 7.0, 1 / 7.0, 1 / 7.0, 1 / 7.0, 1 / 7.0, 1 / 7.0, 1 / 7.0 };
                default: throw new ArgumentOutOfRangeException("module", module, "The module was not recognized.");
            }
        }
        #endregion
        #region Private Methods
        private static void InitializePosteriorTable( DataTable dtSensitivity, DataTable dtPosterior ) {//, double[] priors
            //if( priors.Length != dtSensitivity.Rows.Count ) throw new ArgumentException("The priors array should have the same number of elements as the dtSensitivity has rows.");
            for( byte diagnosisIndex = 0; diagnosisIndex < dtSensitivity.Rows.Count; diagnosisIndex++ ) {
                DataRow drSensitivity = dtSensitivity.Rows[diagnosisIndex];
                DataRow drPosterior = dtPosterior.NewRow();
                drPosterior[ConstantsCalculation.ColumnNameRowID] = drSensitivity[ConstantsCalculation.ColumnNameRowID];
                drPosterior[ConstantsCalculation.ColumnNameDiagnosis] = drSensitivity[ConstantsCalculation.ColumnNameDiagnosis];
                //drPosterior[ConstantsCalculation.ColumnNamePrior] = priors[diagnosisIndex];

                dtPosterior.Rows.Add(drPosterior);
            }
        }
        private static void LoadChestPainTable( BeamDataSet ds ) {//, double[] priors
            //if( priors.Length != 9 ) throw new ArgumentException("The priors array should have the same number of elements as dtSensitivity has rows.");
          
            //Make sure the primary key value and abbreviations match the enum value and string.
            ds.tblChestPain.AddtblChestPainRow((byte)1, "MI", 0.97, 0.5, 0.9, 0, 0, 0.9, 0.1, 0, 0.95, 0.05, 0.98, 0, 0.35, 0, 0.05, 0, 0.02, 0.03, 0, 0, 0, 0.05, 0, 0.5, 0.25, 0.35, 0.25, 0.05, 0, 0.25, 0.3, 0.05, 0, 0, 0.05, 0, 0.2, 0.02, 0, 0, 0, 0.2, 0.25, 0, 0, 0.02, 0.1, 0.03, 0.1);
            ds.tblChestPain.AddtblChestPainRow((byte)2, "ANG", 0.97, 0.5, 0.8, 0.5, 0.4, 0.05, 0.05, 0.05, 0.95, 0, 0.98, 0, 0.15, 0, 0.05, 0, 0.02, 0, 0, 0, 0, 0.3, 0, 0.2, 0.1, 0.05, 0.05, 0.02, 0, 0.05, 0.05, 0.05, 0, 0, 0, 0, 0.05, 0.02, 0, 0, 0, 0.1, 0.1, 0, 0, 0, 0.02, 0.03, 0.1);
            ds.tblChestPain.AddtblChestPainRow((byte)3, "TAA", 0.97, 0.75, 0.9, 0, 0.08, 0.9, 0.1, 0.6, 0.4, 0, 0.6, 0.4, 0.05, 0.5, 0.05, 0.05, 0.02, 0.03, 0.2, 0, 0, 0, 0, 0.1, 0.2, 0.35, 0.5, 0.02, 0, 0, 0, 0, 0, 0, 0, 0, 0.5, 0.02, 0, 0, 0.2, 0.5, 0.05, 0, 0, 0.1, 0, 0.03, 0.5);
            ds.tblChestPain.AddtblChestPainRow((byte)4, "PERI", 0.25, 0.5, 0.5, 0, 0.05, 0.85, 0.05, 0.5, 0.5, 0, 0.95, 0, 0.05, 0.05, 0.05, 0, 0.8, 0.03, 0, 0.02, 0.05, 0, 0, 0.1, 0.05, 0, 0, 0, 0.8, 0, 0, 0, 0.05, 0, 0, 0, 0.1, 0, 0, 0.1, 0, 0.5, 0, 0.4, 0, 0, 0.05, 0.03, 0.05);
            ds.tblChestPain.AddtblChestPainRow((byte)5, "UGI", 0.5, 0.5, 0.4, 0, 0.1, 0.7, 0.2, 0.05, 0.05, 0.9, 0.85, 0.1, 0, 0.1, 0.05, 0, 0.02, 0.5, 0, 0.8, 0, 0, 0, 0.05, 0.05, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.1, 0.98, 0, 0, 0, 0.05, 0, 0, 0, 0, 0, 0.03, 0.05);
            ds.tblChestPain.AddtblChestPainRow((byte)6, "PNEUM", 0.5, 0.5, 0.2, 0, 0.05, 0.7, 0.25, 0.9, 0.1, 0, 0.1, 0.1, 0, 0.05, 0.05, 0.95, 0.9, 0, 0, 0, 0, 0, 0.1, 0.8, 0.2, 0.2, 0.05, 0.05, 0.9, 0.25, 0.9, 0.8, 0.75, 0.02, 0.7, 0, 0.7, 0, 0, 0.25, 0, 0.95, 0.1, 0.05, 0, 0, 0, 0.03, 0.05);
            ds.tblChestPain.AddtblChestPainRow((byte)7, "PTX", 0.1, 0.6, 0.9, 0.05, 0.35, 0.55, 0.05, 0.95, 0.05, 0, 0.05, 0.05, 0, 0.05, 0.05, 0.85, 0.05, 0, 0.2, 0, 0.1, 0.1, 0, 0.1, 0.9, 0, 0.05, 0.05, 0, 0.05, 0.05, 0.05, 0, 0, 0.8, 0.8, 0.9, 0, 0, 0.2, 0.2, 0.85, 0, 0, 0, 0, 0.2, 0.03, 0.05);
            ds.tblChestPain.AddtblChestPainRow((byte)8, "MUSC", 0.1, 0.8, 0.9, 0.9, 0.03, 0.03, 0.05, 0.95, 0.05, 0, 0.02, 0.03, 0, 0.1, 0.95, 0, 0.1, 0, 0.1, 0, 0.75, 0.1, 0, 0.1, 0.05, 0, 0, 0, 0.05, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.98, 0.95, 0.05, 0.05, 0, 0, 0, 0, 0, 0.03, 0);
            ds.tblChestPain.AddtblChestPainRow((byte)9, "PE", 0.8, 0.5, 0.8, 0, 0.05, 0.85, 0.1, 0.5, 0.5, 0, 0.2, 0.1, 0, 0.05, 0.05, 0.95, 0.02, 0, 0, 0, 0.05, 0, 0.4, 0.4, 0.6, 0.35, 0.25, 0.25, 0.2, 0.05, 0.2, 0.05, 0.1, 0.1, 0.1, 0, 0.7, 0, 0, 0.05, 0, 0.4, 0.1, 0, 0.1, 0, 0.2, 0.5, 0.05);

            Int32 priorCount = Priors(StudyModule.ChestPain).Length;
            Trace.Assert(priorCount == ds.tblChestPain.Rows.Count, "The number of sensitivity rows should match the module's number of priors.");
        }
        private static void LoadPediatricDyspneaTable( BeamDataSet ds ) {
            //Make sure the primary key value and abbreviations match the enum value and string.
            ds.tblPediatricDyspnea.AddtblPediatricDyspneaRow((byte)1, "CRP", 0.01, 0.95, 0, 0, 1, 0.9, 0, 0.1, 0.2, 0.1, 0.9, 0.8, 0.05, 0.95, 0, 0, 0, 0, 0, 0, 0, 0.9, 0);
            ds.tblPediatricDyspnea.AddtblPediatricDyspneaRow((byte)2, "BRON", 1, 0.6, 0, 0, 0, 0.9, 0, 0.1, 1, 0.9, 0.9, 0.2, 0.9, 0, 1, 0.2, 0, 0.9, 0.9, 0.2, 0.75, 0, 0);
            ds.tblPediatricDyspnea.AddtblPediatricDyspneaRow((byte)3, "VPN", 0.5, 0.6, 0, 0, 0, 0.7, 0, 0.1, 0.9, 0.9, 0.9, 0.2, 0.9, 0, 0.4, 0.8, 0, 0, 0.4, 1, 0.4, 0, 0);
            ds.tblPediatricDyspnea.AddtblPediatricDyspneaRow((byte)4, "BPN", 0.5, 0.6, 0, 0, 0, 0, 0, 0.9, 0.9, 0.2, 0.1, 0.1, 0.2, 0, 0, 0.95, 0.8, 0, 0, 1, 0.4, 0, 0);
            ds.tblPediatricDyspnea.AddtblPediatricDyspneaRow((byte)5, "AST", 0.05, 0.9, 0, 0.8, 0, 0.1, 0, 0, 0.75, 0.7, 0.1, 0, 0.5, 0, 0.95, 0.1, 0, 0, 0.9, 0.1, 0.75, 0, 0);
            ds.tblPediatricDyspnea.AddtblPediatricDyspneaRow((byte)6, "FB", 0.25, 0.8, 1, 0, 0, 0, 0, 0, 0, 0.1, 0, 0, 0, 0.2, 0, 0, 0, 0, 0.9, 0, 0.25, 0, 0);
            ds.tblPediatricDyspnea.AddtblPediatricDyspneaRow((byte)7, "EPIG", 0.2, 0.5, 0.8, 0.8, 0.75, 0.1, 0.8, 0.95, 0.2, 0.2, 0.05, 0.05, 0.05, 0.9, 0.1, 0, 0.75, 0, 0.05, 0, 0, 0, 1);

            Int32 priorCount = Priors(StudyModule.PediatricDyspnea).Length;
            Trace.Assert(priorCount == ds.tblPediatricDyspnea.Rows.Count, "The number of sensitivity rows should match the module's number of priors.");
        }
        private static string LongerFeatureNamesForChestPain( string columnName ) {
            switch( columnName ) {
                case "Over40": return "Over 40 years old";
                case "Male": return "Male";
                case "SuddenOnset": return "Sudden onset";
                case "TimingAFewSeconds": return "Chest Pain lasting a few seconds";
                case "TimingLessThan30Minutes": return "Chest pain lasting less than 20-30 minutes";
                case "Timing1To3Hours": return "Chest pain lasting 1-3 hours";
                case "TimingMoreThan24Hours": return "Chest pain lasting greater than 24 hours";
                case "QualityStabbing": return "Chest pain sharp/stabbing";
                case "QualityDull": return "Chest pain dull/pressure/squeezing";
                case "QualityBurning": return "Chest pain burning";
                case "LocationSubsternal": return "Location of chest pain substernal/left precordial";
                case "LocationPosteriorThoracic": return "Location of chest pain posterior thoracic";
                case "LocationNeckJawArm": return "Radiation of chest pain to neck/jaw/arm";
                case "LocationBack": return "Radiation of chest pain to back";
                case "LocationSternochondralOrCostrochondral": return "Location of chest pain at sternochondral or costochondral junction";
                case "LocationLateralToCostochondral": return "Location of chest pain lateral to costochondral junction";
                case "FindingsFluLikeExcludingFever": return "Prodromal flu-like symptoms besides fever";
                case "FindingsDysphagia": return "Dysphagia";
                case "FindingsPreceedingTrauma": return "Preceeding trauma";
                case "FindingsPainAlteredByFood": return "Chest pain altered by food";
                case "FindingsPainAlteredByMovement": return "Chest pain altered by movement/posturing";
                case "FindingsPainAlteredByExertion": return "Chest pain altered by exertion";
                case "FindingsRecentImmobility": return "Recent immobility";
                case "FindingsDyspneaGradual": return "Gradual dyspnea";
                case "FindingsDyspneaSudden": return "Sudden dyspnea";
                case "AppearanceDiaphoresis": return "Diaphoresis";
                case "AppearanceSkinCoolPaleMoist": return "Skin cool/pale/moist";
                case "AppearanceCyanosis": return "Cyanosis";
                case "VitalSignsFever": return "Fever";
                case "ThoraxWheezes": return "Wheezes";
                case "ThoraxRales": return "Rales";
                case "ThoraxRonchi": return "Ronchi";
                case "ThoraxProductive": return "Productive";
                case "ThoraxHemoptisys": return "Hemoptysis";
                case "ThoraxUnilateralDecreasedBreathSounds": return "Unilateral decreased breath sounds";
                case "ThoraxUnilateralHyperresonance": return "Unilateral hyperresonance";
                case "ThoraxTachypnea": return "Tachypnea";
                case "ThoraxSubxyphoidEpigastricTenderness": return "Subxyphoid/epigastric tenderness";
                case "ThoraxReproduceablePointTenderness": return "Reproduceable point tenderness";
                case "ThoraxReproduceablePainWithMovement": return "Reproduceable pain with movement/posturing";
                case "ThoraxSoftTissueTrauma": return "Evidence of soft tissue trauma";
                case "CardioTachycardia": return "Tachycardia";
                case "CardioS3S4Gallops": return "S3/S4 gallops";
                case "CardioPrecordialFrictionRubs": return "Precordial friction rubs";
                case "CardioAccentuatedPulmonicSecondSounds": return "Accentuated pulmonic second sound";
                case "CardioAorticRegurgitantMurmur": return "Aortic regurgitant murmur";
                case "CardioNeckVeinDistention": return "Neck vein distention";
                case "CardioDeepVeinThrombosis": return "Deep vein thrombosis";
                case "CardioHypertension": return "Hypertension";
                default: throw new ArgumentOutOfRangeException("columnName", columnName, "The columnName was not recognized for the Chest Pain module.");
            }
        }
        private static string LongerFeatureNamesForPediatricDyspnea( string columnName ) {
            switch( columnName ) {
                case "HistoryUnder12MonthsOld": return "Age less than 12 months";
                case "HistoryCoughWorseAtNight": return "Coughing worse at night";
                case "HistoryCoughSuddenInOnset": return "Cough sudden in onset";
                case "HistoryCoughWorseWithActivity": return "Coughing worse with activity/exercise";
                case "HistoryCoughBarking": return "Barking cough";
                case "HistoryRhinitis": return "Rhinitis";
                case "PhysicalHeadTiltedForward": return "Head Tilted Forward";
                case "PhysicalTempAbove102": return "Temperature > 102";
                case "PhysicalTachypnea": return "Tachypnea";
                case "PhysicalDescreasedO2Sat": return "Decreased Oxygen Saturation";
                case "PhysicalTempBetween100And102": return "Temperature >100 and <102 ";
                case "PhysicalRetractionsSuprasternal": return "Suprasternal retractions";
                case "PhysicalRetractionsIntercostal": return "Intercostal retractions";
                case "PhysicalStridor": return "Stridor";
                case "PhysicalWheezes": return "Wheezes";
                case "PhysicalCrackles": return "Crackles";
                case "LabWbcAbove20k": return "WBC >20,000";
                case "LabNasalSwabRsvPositive": return "Nasal swab RSV positive";
                case "LabCxrHyperinflation": return "CXR reveals hyperinflation";
                case "LabCxrInfiltrate": return "CXR reveals an infiltrate";
                case "LabCxrAtalectesis": return "CXR reveals atalectesis";
                case "LabNeckXrayAirwayNarrowing": return "Neck X-Ray reveals airway narrowing";
                case "LabNeckXrayThumbSign": return "Thumb Sign on Neck X-Ray";
                default: throw new ArgumentOutOfRangeException("columnName", columnName, "The columnName was not recognized for the Pediatric Dyspnea module.");
            }
        }
        #endregion
    }
}
