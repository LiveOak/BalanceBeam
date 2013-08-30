using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Calculation {
   public static class PrebuiltExampleCases {
      #region Common Methods
      //public static ExampleCase[] ExamplesCases( StudyModule module ) {
      //   switch( module ) {
      //      case StudyModule.ChestPain: return new ExampleCase[] { ChestPainExample1(), ChestPainExample2() };
      //      default: throw new ArgumentOutOfRangeException("module", module, "The Study Module was not recognized.");
      //   }
      //}
      public static ExampleCases ExamplesCases ( StudyModule module ) {
         ExampleCases cases = new ExampleCases();
         switch ( module ) {
            case StudyModule.ChestPain:
               cases.Add(ChestPainExample1());
               cases.Add(ChestPainExample2());
               cases.Add(ChestPainExample3());
               cases.Add(ChestPainExample4());
               cases.Add(ChestPainExample5());
               cases.Add(ChestPainExample6());
               cases.Add(ChestPainExample7());
               cases.Add(ChestPainExample8());
               cases.Add(ChestPainExample9());
               break;
            case StudyModule.PediatricDyspnea:
               cases.Add(PediatricDyspneaExample1());
               break;
            default: throw new ArgumentOutOfRangeException("module", module, "The Study Module was not recognized.");
         }
         return cases;
      }
      #endregion
      #region Chest Pain Examples
      public static ExampleCase ChestPainExample1 ( ) {
			const StudyModule module = StudyModule.ChestPain;
			// mag11W0n-50 - **MI** - Angina - Typ 42.7
			const string shortDescription = "61 year old male, dull chest pain";
			//const string shortDescription = "Not too shabby Male";
         const string longDescription = "HX: A 61 year old male presents with a chief complaint of chest discomfort described as sudden in onset. The patient states that it occurred at home and has been ongoing for the past two hours. The discomfort is described as dull. The patient identifies the location as left precordial. Pain radiation involved the neck. Associated findings: gradual dyspnea. Neither exertion nor food appeared to alter the chest discomfort. No risk factors are noted.\n\nPE/RESPIRATORY findings: tachypnea at 27 breaths per minute, coarse wheezing and, fine basilar crackles. CARDIOVASCULAR findings: tachycardic at 131, an S4 gallop and, neck vein distension. ABDOMINAL findings are unremarkable. EXTREMITY findings are unremarkable. MUSCULOSKELETAL findings: unremarkable. DERMAL findings include diaphoresis and, cool extremities.";
			//const string longDescription = "The 35 male patient reported back pain, but no fever.  The two diagnoses being considered are Pneumonia (on the left side of the beam) and Pneumothorax (on the right).";
			const byte diagnosis1ID = (byte)ChestPainDiagnosis.Ang;
			const byte diagnosis2ID = (byte)ChestPainDiagnosis.Mi;

			BeamDataSet.tblChestPainDataTable dtSensitivities = (BeamDataSet.tblChestPainDataTable)DataManager.ChooseCorrectTable(module);
			ExampleCase exampleCase = new ExampleCase(shortDescription, longDescription, diagnosis1ID, diagnosis2ID, dtSensitivities, module);
			
			exampleCase.Add(new Result(dtSensitivities.Over40Column, true));//Feature: This person is over 40 years old.
			exampleCase.Add(new Result(dtSensitivities.MaleColumn, true));//Feature: This person is male.
			exampleCase.Add(new Result(dtSensitivities.SuddenOnsetColumn, true));
			exampleCase.Add(new Result(dtSensitivities.TimingAFewSecondsColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingLessThan30MinutesColumn, false));
			exampleCase.Add(new Result(dtSensitivities.Timing1To3HoursColumn, true));
			exampleCase.Add(new Result(dtSensitivities.TimingMoreThan24HoursColumn, false));
			exampleCase.Add(new Result(dtSensitivities.QualityStabbingColumn, false));
			exampleCase.Add(new Result(dtSensitivities.QualityDullColumn, false));
         	exampleCase.Add(new Result(dtSensitivities.QualityBurningColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationSubsternalColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationNeckJawArmColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsDyspneaGradualColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByFoodColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByExertionColumn, false));
			exampleCase.Add(new Result(dtSensitivities.ThoraxTachypneaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxRalesColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioTachycardiaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.AppearanceDiaphoresisColumn, true));
			exampleCase.Add(new Result(dtSensitivities.AppearanceSkinCoolPaleMoistColumn, true));
			return exampleCase;
		}
      public static ExampleCase ChestPainExample2 ( ) {
			const StudyModule module = StudyModule.ChestPain;
         // C-Y4PW0080-50 - **Upper GI** - Angina - Typ 63.9
         const string shortDescription = "40 year old female, indigestion";
         //const string shortDescription = "Heart Burn Probably";
         const string longDescription = "HX: A 40 year old female presents with a chief complaint of chest pain described as gradual in onset. The patient relates that it occurred at work and has been ongoing for the past one and a half hours. The discomfort is described as indigestion. The patient identifies the location as slightly towards the left of their chest. Pain radiation extended along the upper back of the chest. Associated findings: difficulty swallowing. Factors affecting the pain: eating appeared to increase the discomfort. No risk factors are noted.\n\nPE/RESPIRATORY findings: unremarkable. CARDIOVASCULAR findings: unremarkable. ABDOMINAL finidngs: subxiphoid tenderness. EXTREMITY findings are unremarkable. MUSCULOSKELETAL findings: unremarkable. DERMAL findings are unremarkable.";
         //const string longDescription = "A patient has sudden-onset, substernal chest pain that burns for two hours.";
         const byte diagnosis1ID = (byte)ChestPainDiagnosis.Ugi;
         const byte diagnosis2ID = (byte)ChestPainDiagnosis.Ang;

         BeamDataSet.tblChestPainDataTable dtSensitivities = (BeamDataSet.tblChestPainDataTable)DataManager.ChooseCorrectTable(module);
         ExampleCase exampleCase = new ExampleCase(shortDescription, longDescription, diagnosis1ID, diagnosis2ID, dtSensitivities, module);

			exampleCase.Add(new Result(dtSensitivities.Over40Column, true));//Feature: This person is over 40 years old.
			exampleCase.Add(new Result(dtSensitivities.MaleColumn, false));//Feature: This person is NOT male 
			exampleCase.Add(new Result(dtSensitivities.SuddenOnsetColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingAFewSecondsColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingLessThan30MinutesColumn, false));
			exampleCase.Add(new Result(dtSensitivities.Timing1To3HoursColumn, true));
			exampleCase.Add(new Result(dtSensitivities.TimingMoreThan24HoursColumn, false));
         	exampleCase.Add(new Result(dtSensitivities.QualityBurningColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationSubsternalColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationBackColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationNeckJawArmColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsDysphagiaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByFoodColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxSubxyphoidEpigastricTendernessColumn, true));
			return exampleCase;
		}
      public static ExampleCase ChestPainExample3 ( ) {
			const StudyModule module = StudyModule.ChestPain;
			// nb501YHmW5 - **DTAA** - pneumothorax - Typ 48.7
         const string shortDescription = "64 year old male, sharp chest pain";
			//const string shortDescription = "Stabbing Pain in 53 Male's Chest";
			const string longDescription = "HX: A 64 year old male presents with a chief complaint of chest discomfort described as sudden in onset. The patient relates that it occurred at the supermarket and has been ongoing for the past three hours. The discomfort is described as sharp. The patient identifies the location as towards the back of their thorax. Pain radiation extended along the upper posterior thoracic region. Associated findings: sudden dyspnea and, preceding trauma. Neither exertion nor food appeared to alter the chest discomfort. No risk factors are noted.\n\nPE/RESPIRATORY findings: tachypnea at 23 breaths per minute. CARDIOVASCULAR findings: tachycardic at 119, moderate hypertension and, an aortic regurgitant murmur. ABDOMINAL findings are unremarkable. EXTREMITY findings are unremarkable. MUSCULOSKELETAL findings: unremarkable. DERMAL findings include evidence of recent thoracic soft tissue ecchymosis, diaphoresis and, cool and moist extremities.";
			//const string longDescription = "HX: A 53 year old male presents with a chief complaint of chest pain described as sudden in onset. The patient states that it occurred while performing household tasks and has been ongoing for the past two hours. The discomfort is described as sharp. The patient identifies the location as towards the back of their thorax. Pain radiation extended along the upper back of the chest. Associated findings: unremarkable. Neither exertion nor food appeared to alter the chest discomfort. No risk factors are noted.\n\nPE/RESPIRATORY findings: unremarkable. CARDIOVASCULAR findings: tachycardic at 130 and moderate hypertension. ABDOMINAL findings are unremarkable. EXTREMITY findings are unremarkable. MUSCULOSKELETAL findings: unremarkable. DERMAL findings include diaphoresis.\n\nhttps://kbit.acdet.com/inst/comp/56/3/1/0";
			const byte diagnosis1ID = (byte)ChestPainDiagnosis.Taa;
			const byte diagnosis2ID = (byte)ChestPainDiagnosis.Ptx;

			BeamDataSet.tblChestPainDataTable dtSensitivities = (BeamDataSet.tblChestPainDataTable)DataManager.ChooseCorrectTable(module);
			ExampleCase exampleCase = new ExampleCase(shortDescription, longDescription, diagnosis1ID, diagnosis2ID, dtSensitivities, module);

			exampleCase.Add(new Result(dtSensitivities.Over40Column, true));//Feature: This person is over 40 years old.
			exampleCase.Add(new Result(dtSensitivities.MaleColumn, true));//Feature: This person is male 
			exampleCase.Add(new Result(dtSensitivities.SuddenOnsetColumn, true));
			exampleCase.Add(new Result(dtSensitivities.TimingAFewSecondsColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingLessThan30MinutesColumn, false));
			exampleCase.Add(new Result(dtSensitivities.Timing1To3HoursColumn, true));
			exampleCase.Add(new Result(dtSensitivities.TimingMoreThan24HoursColumn, false));			
			exampleCase.Add(new Result(dtSensitivities.QualityStabbingColumn, true));
			exampleCase.Add(new Result(dtSensitivities.QualityDullColumn, false));
			exampleCase.Add(new Result(dtSensitivities.LocationBackColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationPosteriorThoracicColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsPreceedingTraumaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsDyspneaGradualColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsDyspneaSuddenColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByFoodColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByMovementColumn, false));
			exampleCase.Add(new Result(dtSensitivities.ThoraxTachypneaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioTachycardiaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioHypertensionColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioAorticRegurgitantMurmurColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxSoftTissueTraumaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.AppearanceDiaphoresisColumn, true));
			exampleCase.Add(new Result(dtSensitivities.AppearanceSkinCoolPaleMoistColumn, true));
			return exampleCase;
		}
      public static ExampleCase ChestPainExample4 ( ) {
			const StudyModule module = StudyModule.ChestPain;
			// C-Cb800e00-50 - **Pericarditis** - Angina - Typ 61.7
         const string shortDescription = "34 year old female, sharp chest pain";
			//const string shortDescription = "Stabbing Pain in 34 Male's Chest";
			const string longDescription = "HX: A 34 year old female presents with a chief complaint of chest discomfort described as sudden in onset. The patient relates that it occurred at home and has been ongoing for the past three hours. The pain is described as sharp. The patient identifies the location as left precordial. There was no radiation. Associated findings: mild flu-like symptoms 3 days ago and, a temperature of 100.3. Neither exertion nor food appeared to alter the chest discomfort. No risk factors are noted.\n\nPE/RESPIRATORY findings: unremarkable. CARDIOVASCULAR findings: tachycardic at 121 and, a precordial friction rub. ABDOMINAL findings are unremarkable. EXTREMITY findings are unremarkable. MUSCULOSKELETAL findings: unremarkable. DERMAL findings are unremarkable.";
			//const string longDescription = "HX: A 34 year old male presents with a chief complaint of chest pain described as sudden in onset. The patient states that it occurred while performing household tasks, lasted approximately 20 minutes and resolved prior to arrival. The discomfort is described as stabbing. The patient identifies the location as lateral to the costochondral junction. There was no radiation. Associated findings: sudden dyspnea. Factors affecting the pain: rest seemed to alleviate the discomfort. No risk factors are noted.\n\nPE/RESPIRATORY findings: tachypnea at 24 breaths per minute, unilateral decreased breath sounds and unilateral hyperresonance. CARDIOVASCULAR findings: tachycardic at 129 and neck vein distension. ABDOMINAL findings are unremarkable. EXTREMITY findings are unremarkable. MUSCULOSKELETAL findings: chest pain is elicited with specific postural changes involving the chest. DERMAL findings include evidence of recent chest wall soft tissue bruising.";
			const byte diagnosis1ID = (byte)ChestPainDiagnosis.Ang;
			const byte diagnosis2ID = (byte)ChestPainDiagnosis.Peri;

			BeamDataSet.tblChestPainDataTable dtSensitivities = (BeamDataSet.tblChestPainDataTable)DataManager.ChooseCorrectTable(module);
			ExampleCase exampleCase = new ExampleCase(shortDescription, longDescription, diagnosis1ID, diagnosis2ID, dtSensitivities, module);

			exampleCase.Add(new Result(dtSensitivities.Over40Column, false));
			exampleCase.Add(new Result(dtSensitivities.MaleColumn, false));//Feature: This person is NOT male 
			exampleCase.Add(new Result(dtSensitivities.SuddenOnsetColumn, true));
			exampleCase.Add(new Result(dtSensitivities.TimingAFewSecondsColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingLessThan30MinutesColumn, false));
			exampleCase.Add(new Result(dtSensitivities.Timing1To3HoursColumn, true));
			exampleCase.Add(new Result(dtSensitivities.TimingMoreThan24HoursColumn, false));
			exampleCase.Add(new Result(dtSensitivities.QualityStabbingColumn, true));
			exampleCase.Add(new Result(dtSensitivities.QualityDullColumn, false));
			exampleCase.Add(new Result(dtSensitivities.LocationSubsternalColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationNeckJawArmColumn, false));
			exampleCase.Add(new Result(dtSensitivities.LocationBackColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsFluLikeExcludingFeverColumn, true));
			exampleCase.Add(new Result(dtSensitivities.VitalSignsFeverColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByFoodColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByMovementColumn, false));
			exampleCase.Add(new Result(dtSensitivities.CardioTachycardiaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioPrecordialFrictionRubsColumn, true));
			return exampleCase;
		}
      public static ExampleCase ChestPainExample5 ( ) {
         const StudyModule module = StudyModule.ChestPain;
			// C-mGe80000-50 - **Angina** - MI - Typ 44.4
			const string shortDescription = "45 year old male, indigestion";
			//const string shortDescription = "Not too shabby Female";
         const string longDescription = "HX: A 45 year old male presents with a chief complaint of chest pain described as gradual in onset. The patient states that it occurred while shopping, lasted approximately 3 minutes and resolved prior to arrival. The pain is described as indigestion. The patient identifies the location as in the middle of their chest. There was no radiation. Associated findings: unremarkable. Factors affecting the pain: rest seemed to aleviate the discomfort. No risk factors are noted.\n\nPE/RESPIRATORY findings: unremarkable. CARDIOVASCULAR findings: unremarkable. ABDOMINAL findings are unremarkable. EXTREMITY findings are unremarkable. MUSCULOSKELETAL findings: unremarkable. DERMAL findings are unremarkable.";
			//const string longDescription = "The 53 female patient reported back pain, but no fever.  The two diagnoses being considered are Myocardial Infarction (on the left side of the beam) and Upper GI (on the right).";
			const byte diagnosis1ID = (byte)ChestPainDiagnosis.Mi;
			const byte diagnosis2ID = (byte)ChestPainDiagnosis.Ang;

			BeamDataSet.tblChestPainDataTable dtSensitivities = (BeamDataSet.tblChestPainDataTable)DataManager.ChooseCorrectTable(module);
			ExampleCase exampleCase = new ExampleCase(shortDescription, longDescription, diagnosis1ID, diagnosis2ID, dtSensitivities, module);

			exampleCase.Add(new Result(dtSensitivities.Over40Column, true));//Feature: This person is over 40 years old.
			exampleCase.Add(new Result(dtSensitivities.MaleColumn, true));//Feature: This person is male.
			exampleCase.Add(new Result(dtSensitivities.SuddenOnsetColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingAFewSecondsColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingLessThan30MinutesColumn, true));
			exampleCase.Add(new Result(dtSensitivities.Timing1To3HoursColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingMoreThan24HoursColumn, false));
			exampleCase.Add(new Result(dtSensitivities.QualityStabbingColumn, false));
			exampleCase.Add(new Result(dtSensitivities.QualityDullColumn, false));
         	exampleCase.Add(new Result(dtSensitivities.QualityBurningColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationSubsternalColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationNeckJawArmColumn, false));
			exampleCase.Add(new Result(dtSensitivities.LocationBackColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByMovementColumn, true));         
        
         return exampleCase;
      }
      public static ExampleCase ChestPainExample6 ( ) {
         const StudyModule module = StudyModule.ChestPain;
         // C-i301ru23-50 - **Pneumonia** - pneumothorax - Typ 62.3
         const string shortDescription = "40 year old female, stabbing pain for 24 hours";
         //const string shortDescription = "Heart Burn Probably";
         const string longDescription = "HX: A 40 year old female presents with a chief complaint of chest discomfort described as gradual in onset. The patient relates that it occurred while outdoors and has been ongoing for the past 24 hours. The pain is described as stabbing. The patient identifies the location as lateral to the costochondral junction. There was no radiation. Associated findings: mild flu-like symptoms 3 days ago, gradual difficulty breathing and, a temperature of 100.2. Neither exertion nor food appeared to alter the chest discomfort. No risk factors are noted.\n\nPE/RESPIRATORY findings: tachypnea at 25 breaths per minute, crackles, coarse ronchi, productive cough and, unilateral decreased breath sounds. CARDIOVASCULAR findings: tachycardic at 118, an S4 gallop and, a precordial friction rub. ABDOMINAL findings are unremarkable. EXTREMITY findings are unremarkable. MUSCULOSKELETAL findings: chest pain is elicited with specific chest wall movements. DERMAL findings are unremarkable.";
         //const string longDescription = "A patient has sudden-onset, substernal chest pain that burns for two hours.";
         const byte diagnosis1ID = (byte)ChestPainDiagnosis.Ptx;
         const byte diagnosis2ID = (byte)ChestPainDiagnosis.Pneum;

         BeamDataSet.tblChestPainDataTable dtSensitivities = (BeamDataSet.tblChestPainDataTable)DataManager.ChooseCorrectTable(module);
         ExampleCase exampleCase = new ExampleCase(shortDescription, longDescription, diagnosis1ID, diagnosis2ID, dtSensitivities, module);

			exampleCase.Add(new Result(dtSensitivities.Over40Column, true));
			exampleCase.Add(new Result(dtSensitivities.MaleColumn, false));//Feature: This person is NOT male 
			exampleCase.Add(new Result(dtSensitivities.SuddenOnsetColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingAFewSecondsColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingLessThan30MinutesColumn, false));
			exampleCase.Add(new Result(dtSensitivities.Timing1To3HoursColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingMoreThan24HoursColumn, true));
			exampleCase.Add(new Result(dtSensitivities.QualityStabbingColumn, true));
			exampleCase.Add(new Result(dtSensitivities.QualityDullColumn, false));
			exampleCase.Add(new Result(dtSensitivities.LocationLateralToCostochondralColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationNeckJawArmColumn, false));
			exampleCase.Add(new Result(dtSensitivities.LocationBackColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsFluLikeExcludingFeverColumn, true));
			exampleCase.Add(new Result(dtSensitivities.VitalSignsFeverColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsDyspneaGradualColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsDyspneaSuddenColumn, false));	
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByFoodColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByMovementColumn, false));
			exampleCase.Add(new Result(dtSensitivities.ThoraxTachypneaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxRalesColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxRonchiColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxProductiveColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxUnilateralDecreasedBreathSoundsColumn, true));		
			exampleCase.Add(new Result(dtSensitivities.CardioTachycardiaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioS3S4GallopsColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioPrecordialFrictionRubsColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxReproduceablePainWithMovementColumn, true));
         return exampleCase;
      }
      public static ExampleCase ChestPainExample7 ( ) {
         const StudyModule module = StudyModule.ChestPain;
         // Hf0O7X3AW5 - **Pneumothorax** - Musculoskeletal/Chest Wall Syndrome - Typ 63.8
         const string shortDescription = "30 year old male, sharp chest pain, difficulty breathing";
         //const string shortDescription = "Heart Burn Probably";
         const string longDescription = "HX: A 30 year old male presents with a chief complaint of chest discomfort described as sudden in onset. The patient relates that it occurred at the supermarket, was present about 20 minutes and has not returned. The pain is described as sharp. The patient identifies the location as to the side of the chest. There was no radiation. Associated findings: sudden difficulty breathing and, a recent auto accident. Factors affecting the pain: rest seemed to aleviate the discomfort. No risk factors are noted.\n\nPE/RESPIRATORY findings: tachypnea at 22 breaths per minute, unilateral diminished breath sounds and, unilateral hyperresonance. CARDIOVASCULAR findings: tachycardic at 122 and, neck vein distension. ABDOMINAL findings are unremarkable. EXTREMITY findings are unremarkable. MUSCULOSKELETAL findings: chest pain is elicited with specific postural changes involving the chest. DERMAL findings include evidence of recent chest wall soft tissue trauma and, cyanosis.";
         //const string longDescription = "A patient has sudden-onset, substernal chest pain that burns for two hours.";
         const byte diagnosis1ID = (byte)ChestPainDiagnosis.Musc;
         const byte diagnosis2ID = (byte)ChestPainDiagnosis.Ptx;

         BeamDataSet.tblChestPainDataTable dtSensitivities = (BeamDataSet.tblChestPainDataTable)DataManager.ChooseCorrectTable(module);
         ExampleCase exampleCase = new ExampleCase(shortDescription, longDescription, diagnosis1ID, diagnosis2ID, dtSensitivities, module);

			exampleCase.Add(new Result(dtSensitivities.Over40Column, false));
			exampleCase.Add(new Result(dtSensitivities.MaleColumn, true));//Feature: This person is male 
			exampleCase.Add(new Result(dtSensitivities.SuddenOnsetColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingAFewSecondsColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingLessThan30MinutesColumn, true));
			exampleCase.Add(new Result(dtSensitivities.Timing1To3HoursColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingMoreThan24HoursColumn, false));
			exampleCase.Add(new Result(dtSensitivities.QualityStabbingColumn, true));
			exampleCase.Add(new Result(dtSensitivities.QualityDullColumn, false));
			exampleCase.Add(new Result(dtSensitivities.LocationLateralToCostochondralColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationNeckJawArmColumn, false));
			exampleCase.Add(new Result(dtSensitivities.LocationBackColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsDyspneaGradualColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsDyspneaSuddenColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsPreceedingTraumaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByMovementColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxTachypneaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxUnilateralDecreasedBreathSoundsColumn, true));		
			exampleCase.Add(new Result(dtSensitivities.ThoraxUnilateralHyperresonanceColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioTachycardiaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioNeckVeinDistentionColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByMovementColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxSoftTissueTraumaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.AppearanceCyanosisColumn, true));
         return exampleCase;
      }
      public static ExampleCase ChestPainExample8 ( ) {
         const StudyModule module = StudyModule.ChestPain;
         // C-Hn1G0064-50 - **Musculoskeletal/Chest Wall Syndrome** - Angina - Typ 81.7
         const string shortDescription = "30 year old male, sharp chest pain";
         //const string shortDescription = "Heart Burn Probably";
         const string longDescription = "HX: A 30 year old male presents with a chief complaint of chest pain described as sudden in onset. The patient relates that it occurred at home, lasted approximately 2 minutes and has not returned. The discomfort is described as sharp. The patient identifies the location as along the costochondral junction. Pain radiation extended along the upper posterior thoracic region. Associated findings: a recent auto accident. Neither exertion nor food appeared to alter the chest discomfort. No risk factors are noted.\n\nPE/RESPIRATORY findings: unremarkable. CARDIOVASCULAR findings: unremarkable. ABDOMINAL findings are unremarkable. EXTREMITY findings are unremarkable. MUSCULOSKELETAL findings: discomfort is produced with palpation at the sternochondral junction and, chest pain is elicited with specific chest wall movements. DERMAL findings are unremarkable.";
         //const string longDescription = "A patient has sudden-onset, substernal chest pain that burns for two hours.";
         const byte diagnosis1ID = (byte)ChestPainDiagnosis.Ang;
         const byte diagnosis2ID = (byte)ChestPainDiagnosis.Musc;

         BeamDataSet.tblChestPainDataTable dtSensitivities = (BeamDataSet.tblChestPainDataTable)DataManager.ChooseCorrectTable(module);
         ExampleCase exampleCase = new ExampleCase(shortDescription, longDescription, diagnosis1ID, diagnosis2ID, dtSensitivities, module);

			exampleCase.Add(new Result(dtSensitivities.Over40Column, false));
			exampleCase.Add(new Result(dtSensitivities.MaleColumn, true));//Feature: This person is male 
			exampleCase.Add(new Result(dtSensitivities.SuddenOnsetColumn, true));
			exampleCase.Add(new Result(dtSensitivities.TimingAFewSecondsColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingLessThan30MinutesColumn, true));
			exampleCase.Add(new Result(dtSensitivities.Timing1To3HoursColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingMoreThan24HoursColumn, false));
			exampleCase.Add(new Result(dtSensitivities.QualityStabbingColumn, true));
			exampleCase.Add(new Result(dtSensitivities.QualityDullColumn, false));
			exampleCase.Add(new Result(dtSensitivities.LocationSternochondralOrCostrochondralColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationNeckJawArmColumn, false));
			exampleCase.Add(new Result(dtSensitivities.LocationBackColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsPreceedingTraumaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByFoodColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByMovementColumn, false));
			exampleCase.Add(new Result(dtSensitivities.ThoraxReproduceablePointTendernessColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxReproduceablePainWithMovementColumn, true));
     return exampleCase;
      }
      public static ExampleCase ChestPainExample9 ( ) {
         const StudyModule module = StudyModule.ChestPain;
         // qaW5PrWwW5 - **PE** - MI - Typ 44.2
         const string shortDescription = "64 year old male, chest tightness, difficulty breathing";
         //const string shortDescription = "Heart Burn Probably";
         const string longDescription = "HX: A 64 year old male presents with a chief complaint of chest pain described as sudden in onset. The patient states that it occurred while performing household tasks and has been ongoing for the past two hours. The pain is described as tightness. The patient identifies the location as to the side of the chest. There was no radiation. Associated findings: sudden difficulty breathing and, a temperature of 100. Neither exertion nor food appeared to alter the chest discomfort. The patient has a history of recent immobility.\n\nPE/RESPIRATORY findings: tachypnea at 28 breaths per minute, crackles, a productive cough with yellow sputum and, hemoptysis. CARDIOVASCULAR findings: tachycardic at 118, an S4 gallop, an accentuated pulmonic second sound and, neck vein distension. ABDOMINAL findings are unremarkable. EXTREMITY findings include unilateral leg pain. MUSCULOSKELETAL findings: unremarkable. DERMAL findings include diaphoresis, cool and moist extremities and, cyanosis.";
         //const string longDescription = "A patient has sudden-onset, substernal chest pain that burns for two hours.";
         const byte diagnosis1ID = (byte)ChestPainDiagnosis.Pe;
         const byte diagnosis2ID = (byte)ChestPainDiagnosis.Mi;

         BeamDataSet.tblChestPainDataTable dtSensitivities = (BeamDataSet.tblChestPainDataTable)DataManager.ChooseCorrectTable(module);
         ExampleCase exampleCase = new ExampleCase(shortDescription, longDescription, diagnosis1ID, diagnosis2ID, dtSensitivities, module);

			exampleCase.Add(new Result(dtSensitivities.Over40Column, true));
			exampleCase.Add(new Result(dtSensitivities.MaleColumn, true));//Feature: This person is male 
			exampleCase.Add(new Result(dtSensitivities.SuddenOnsetColumn, true));
			exampleCase.Add(new Result(dtSensitivities.TimingAFewSecondsColumn, false));
			exampleCase.Add(new Result(dtSensitivities.TimingLessThan30MinutesColumn, false));
			exampleCase.Add(new Result(dtSensitivities.Timing1To3HoursColumn, true));
			exampleCase.Add(new Result(dtSensitivities.TimingMoreThan24HoursColumn, false));
			exampleCase.Add(new Result(dtSensitivities.QualityStabbingColumn, false));
			exampleCase.Add(new Result(dtSensitivities.QualityDullColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationLateralToCostochondralColumn, true));
			exampleCase.Add(new Result(dtSensitivities.LocationNeckJawArmColumn, false));
			exampleCase.Add(new Result(dtSensitivities.LocationBackColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsDyspneaGradualColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsDyspneaSuddenColumn, true));
			exampleCase.Add(new Result(dtSensitivities.VitalSignsFeverColumn, true));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByFoodColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsPainAlteredByMovementColumn, false));
			exampleCase.Add(new Result(dtSensitivities.FindingsRecentImmobilityColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxTachypneaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxRalesColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxProductiveColumn, true));
			exampleCase.Add(new Result(dtSensitivities.ThoraxHemoptisysColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioTachycardiaColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioS3S4GallopsColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioAccentuatedPulmonicSecondSoundsColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioNeckVeinDistentionColumn, true));
			exampleCase.Add(new Result(dtSensitivities.CardioDeepVeinThrombosisColumn, true));
			exampleCase.Add(new Result(dtSensitivities.AppearanceDiaphoresisColumn, true));
			exampleCase.Add(new Result(dtSensitivities.AppearanceSkinCoolPaleMoistColumn, true));
			exampleCase.Add(new Result(dtSensitivities.AppearanceCyanosisColumn, true));
         return exampleCase;
      }
      #endregion
      #region Pediatric Dyspnea Examples
      public static ExampleCase PediatricDyspneaExample1 ( ) {
         const StudyModule module = StudyModule.PediatricDyspnea;
         const string shortDescription = "Bad news bear";
         const string longDescription = "The 7 year old male patient reported back pain, but no fever.  The two diagnoses being considered are Bpn (on the left side of the beam) and Epig (on the right).";
         const byte diagnosis1ID = (byte)PediatricDyspnea.Bpn;
         const byte diagnosis2ID = (byte)PediatricDyspnea.Epig;

         BeamDataSet.tblPediatricDyspneaDataTable dtSensitivities = (BeamDataSet.tblPediatricDyspneaDataTable)DataManager.ChooseCorrectTable(module);
         ExampleCase exampleCase = new ExampleCase(shortDescription, longDescription, diagnosis1ID, diagnosis2ID, dtSensitivities, module);
         exampleCase.Add(new Result(dtSensitivities.PhysicalTempAbove102Column, true));//Feature: high temp
         exampleCase.Add(new Result(dtSensitivities.HistoryUnder12MonthsOldColumn, true));//Feature: under 12
         exampleCase.Add(new Result(dtSensitivities.LabNeckXrayThumbSignColumn, true));//Feature: whatever this is.
         exampleCase.Add(new Result(dtSensitivities.HistoryCoughWorseAtNightColumn, false));//Feature: coughing is NOT worse at night.
         return exampleCase;
      }
      #endregion
   }
}