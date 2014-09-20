[![DOI](https://zenodo.org/badge/4971/LiveOak/BalanceBeam.png)](http://dx.doi.org/10.5281/zenodo.11802)
BalanceBeam
===========

This repository supports two articles:

## Article 1

Hamm, Rob M., Beasley, William Howard, and Johnson, William Jay (2014). [A Balance Beam Aid for Instruction in Clinical Diagnostic Reasoning](http://www.ncbi.nlm.nih.gov/pubmed/24739532). [*Medical Decision Making*](http://mdm.sagepub.com/)
DOI: 10.1177/0272989X14529623. Available on [ResearchGate](https://www.researchgate.net/publication/261748914_A_Balance_Beam_Aid_for_Instruction_in_Clinical_Diagnostic_Reasoning)

Abstract
>We describe a balance beam aid for instruction in diagnosis (BBAID) and demonstrate its potential use in supplementing the training of medical students to diagnose acute chest pain. We suggest the BBAID helps students understand the process of diagnosis because the impact of tokens (weights and helium balloons) attached to a beam at different distances from the fulcrum is analogous to the impact of evidence to the relative support for 2 diseases. The BBAID presents a list of potential findings and allows students to specify whether each is present, absent, or unknown. It displays the likelihood ratios corresponding to a positive (LR+) or negative (LR-) observation for each symptom, for any pair of diseases. For each specified finding, a token is placed on the beam at a location whose distance from the fulcrum is proportional to the finding's log(LR): a downward force (a weight) if the finding is present and a lifting force (a balloon) if it is absent. Combining the physical torques of multiple tokens is mathematically identical to applying Bayes' theorem to multiple independent findings, so the balance beam is a high-fidelity metaphor. Seven first-year medical students and 3 faculty members consulted the BBAID while diagnosing brief patient case vignettes. Student comments indicated the program is usable, helpful for understanding pertinent positive and negative findings' usefulness in particular situations, and welcome as a reference or self-test. All students attended the effect of the tokens on the beam, although some stated they did not use the numerical statistics. Faculty noted the BBAID might be particularly helpful in reminding students of diseases that should not be missed and identifying pertinent findings to ask for.

Keywords: Bayes’ theorem; balance beam metaphor; chest pain diagnosis; computer tutorial; differential diagnosis; medical education

## Article 2
Hamm, Rob M., and Beasley, William Howard (2014). [The Balance Beam Metaphor: A Perspective on Clinical Diagnosis](http://www.ncbi.nlm.nih.gov/pubmed/24739531). [*Medical Decision Making*](http://mdm.sagepub.com/)
DOI: 10.1177/0272989X14528755. Available on [ResearchGate](https://www.researchgate.net/publication/261748694_The_Balance_Beam_Metaphor_A_Perspective_on_Clinical_Diagnosis)

Abstract
>Understanding the impact of clinical findings in discriminating between possible causes of a patient's presentation is essential in clinical judgment. A balance beam is a natural physical analogue that can accurately represent the combination of several pieces of evidence with varying ability to discriminate between disease hypotheses. Calculation of Bayes' theorem using log(posterior odds) as a function of log(prior odds) and the logarithms of the evidence's likelihood ratios maps onto the physical forces affecting objects placed on a balance beam. We describe the rules governing the functioning of tokens representing clinical findings in the comparison of 2 competing diseases. The likelihood ratios corresponding to positive (LR+) or negative (LR-) observations for each symptom determine the lateral position at which the symptom's token is placed on the beam, using a weight if the finding is present and a helium balloon if it is absent. We discuss how a balance beam could represent concepts of dynamic specificity (due to changes in competitor diseases' probabilities) and dynamic sensitivity (due to class-conditional independence). Utility-based thresholds for acting on a diagnosis could be represented by moving the balance beam's fulcrum. It is suggested that a balance beam can be a useful aid for students learning clinical diagnosis, allowing them to build on existing intuitive understanding to develop an appreciation of how evidence combines to influence degree of belief. The balance beam could also facilitate exploration of the potential impact of available questions or investigations.

Keywords: Bayes’ theorem; balance beam metaphor; class-conditional independence; medical diagnosis; medical education; spectrum effects; threshold probability

## Code Repository
The software is written Primarily in C# 4.0, under the Mozilla License.  The DOI of the *repository* (not of the articles) is 10.5281/zenodo.11802
