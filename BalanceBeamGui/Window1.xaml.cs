using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using Calculation;

namespace BalanceBeamGui {
    public partial class Window1 : Window {
        #region Fields
        private const Int32 _roundingDigitsMatrixTorque = 2;
        private const Int32 _roundingDigitsMatrixProbability = 3;
        private const Int32 _roundingDigitsToolTips = 4;
        private const Int32 _roundingDigitsMatrixEntropyReduction = 3;
        private const double _canvasPadding = 17;
        private double _arrowTotalRadiusOffset = 4;
        private const Int32 _grdFeatureTorquePositiveColumnIndex = 1;
        private const Int32 _grdFeatureTorqueNegativeColumnIndex = 2;
        private const Int32 _grdFeatureEntropyPairColumnIndex = 3;
        private const Int32 _grdFeatureEntropySetColumnIndex = 4;

        //private byte _transparencyHighlightInBytes = 119;//http://www.easycalculation.com/hex-converter.php
        private readonly Line _potentialTorqueLineUp = new Line();
        private readonly Line _potentialTorqueLineDown = new Line();
        private readonly TextBlock _potentialTorqueTextUp = new TextBlock();
        private readonly TextBlock _potentialTorqueTextDown = new TextBlock();

        private readonly static StudyModule[] _possibleModules = { StudyModule.ChestPain, StudyModule.PediatricDyspnea };
        //private const StudyModule _module = StudyModule.ChestPain;
        private StudyModule _module;//= _possibleModules[0];//StudyModule.PediatricDyspnea;

        private double _maxAbsoluteTorque = double.NaN;
        private byte _diagnosisID1;// = 1; Set in InitializeFeatureGrid
        private byte _diagnosisID2;// = 2;

        private DataTable _dtSensitivities;
        BeamDataSet.tblFeatureDataTable _tblFeature = new BeamDataSet.tblFeatureDataTable();
        private Label _lblColumnGrandHeader = new Label();
        private Label _lblRowGrandHeader = new Label();


        private Grinder _grinder;
        private DataColumn[] _dcEvidences;
        private bool _isWindowLoaded = false;
        #endregion
        #region Working Cosmetics
        
        // 9 colors from colorbrewer2.org>qualitative>paired color scheme plus 9 colors from colorbrewer2.org>sequential>greys color scheme - Jay 101513
        private readonly Color[] _allPossibleColors = { Color.FromArgb(255, 166, 206, 227), Color.FromArgb(255, 31, 120, 180), Color.FromArgb(255, 178, 223, 138), Color.FromArgb(255, 51, 160, 44), Color.FromArgb(255, 251, 154, 153), Color.FromArgb(255, 227, 26, 28), Color.FromArgb(255, 253, 191, 111), Color.FromArgb(255, 255, 127, 0), Color.FromArgb(255, 202, 178, 214), Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 240, 240, 240), Color.FromArgb(255, 217, 217, 217), Color.FromArgb(255, 189, 189, 189), Color.FromArgb(255, 150, 150, 150), Color.FromArgb(255, 115, 115, 115), Color.FromArgb(255, 82, 82, 82), Color.FromArgb(255, 37, 37, 37), Color.FromArgb(255, 0, 0, 0) };

        // private readonly Color[] _allPossibleColors = { Colors.AliceBlue, Colors.AntiqueWhite, Colors.Aqua, Colors.Aquamarine, Colors.Azure, Colors.Beige, Colors.Bisque, Colors.Black, Colors.BlanchedAlmond, Colors.Blue, Colors.BlueViolet, Colors.Brown, Colors.BurlyWood, Colors.CadetBlue, Colors.Chartreuse, Colors.Chocolate, Colors.Coral, Colors.CornflowerBlue, Colors.Cornsilk, Colors.Crimson, Colors.Cyan, Colors.DarkBlue, Colors.DarkCyan, Colors.DarkGoldenrod, Colors.DarkGray, Colors.DarkGreen, Colors.DarkKhaki, Colors.DarkMagenta, Colors.DarkOliveGreen, Colors.DarkOrange, Colors.DarkOrchid, Colors.DarkRed, Colors.DarkSalmon, Colors.DarkSeaGreen, Colors.DarkSlateBlue, Colors.DarkSlateGray, Colors.DarkTurquoise, Colors.DarkViolet, Colors.DeepPink, Colors.DeepSkyBlue, Colors.DimGray, Colors.DodgerBlue, Colors.Firebrick, Colors.FloralWhite, Colors.ForestGreen, Colors.Fuchsia, Colors.Gainsboro, Colors.GhostWhite, Colors.Gold, Colors.Goldenrod, Colors.Gray, Colors.Green, Colors.GreenYellow, Colors.Honeydew, Colors.HotPink, Colors.IndianRed, Colors.Indigo, Colors.Ivory, Colors.Khaki, Colors.Lavender, Colors.LavenderBlush, Colors.LawnGreen, Colors.LemonChiffon, Colors.LightBlue, Colors.LightCoral, Colors.LightCyan, Colors.LightGoldenrodYellow, Colors.LightGray, Colors.LightGreen, Colors.LightPink, Colors.LightSalmon, Colors.LightSeaGreen, Colors.LightSkyBlue, Colors.LightSlateGray, Colors.LightSteelBlue, Colors.LightYellow, Colors.Lime, Colors.LimeGreen, Colors.Linen, Colors.Magenta, Colors.Maroon, Colors.MediumAquamarine, Colors.MediumBlue, Colors.MediumOrchid, Colors.MediumPurple, Colors.MediumSeaGreen, Colors.MediumSlateBlue, Colors.MediumSpringGreen, Colors.MediumTurquoise, Colors.MediumVioletRed, Colors.MidnightBlue, Colors.MintCream, Colors.MistyRose, Colors.Moccasin, Colors.NavajoWhite, Colors.Navy, Colors.OldLace, Colors.Olive, Colors.OliveDrab, Colors.Orange, Colors.OrangeRed, Colors.Orchid, Colors.PaleGoldenrod, Colors.PaleGreen, Colors.PaleTurquoise, Colors.PaleVioletRed, Colors.PapayaWhip, Colors.PeachPuff, Colors.Peru, Colors.Pink, Colors.Plum, Colors.PowderBlue, Colors.Purple, Colors.Red, Colors.RosyBrown, Colors.RoyalBlue, Colors.SaddleBrown, Colors.Salmon, Colors.SandyBrown, Colors.SeaGreen, Colors.SeaShell, Colors.Sienna, Colors.Silver, Colors.SkyBlue, Colors.SlateBlue, Colors.SlateGray, Colors.Snow, Colors.SpringGreen, Colors.SteelBlue, Colors.Tan, Colors.Teal, Colors.Thistle, Colors.Tomato, Colors.Transparent, Colors.Turquoise, Colors.Violet, Colors.Wheat, Colors.White, Colors.WhiteSmoke, Colors.Yellow, Colors.YellowGreen };
        //Modifiable by user
        // default colors for comboBoxes - Jay 101513
        // private SolidColorBrush _brushPalette = Color.FromArgb(255, 255, 255, 255); //white
        // private SolidColorBrush _brushForegroundPositive = Color.FromArgb(255, 51, 160, 44); //green
        // private SolidColorBrush _brushBackgroundPositive = Color.FromArgb(255, 166, 206, 227); //light blue
        // private SolidColorBrush _brushForegroundNegative = Color.FromArgb(255, 227, 26, 28); // red
        // private SolidColorBrush _brushBackgroundNegative = Color.FromArgb(255, 251, 154, 153); // pink
        // private SolidColorBrush _brushHighlight = Color.FromArgb(255, 255, 127, 0); // orange
        
		private SolidColorBrush _brushPalette = Brushes.MintCream;
        private SolidColorBrush _brushForegroundPositive = Brushes.Blue;//
        private SolidColorBrush _brushBackgroundPositive = Brushes.LightBlue;
        private SolidColorBrush _brushForegroundNegative = Brushes.Firebrick;// Coral;
        private SolidColorBrush _brushBackgroundNegative = Brushes.LightPink;// LightCoral;
        private SolidColorBrush _brushHighlight = new SolidColorBrush(Color.FromArgb(119, 255, 255, 0));// (SolidColorBrush)(new BrushConverter().ConvertFromString("#77FFFF00"));//LightGoldenrodYellow; //#77FFFF00;//Brushes.Yellow;

        //Fixed
        private SolidColorBrush _brushForegroundHeader = Brushes.Black;
        private SolidColorBrush _brushDemphasizedText = Brushes.DarkGray;
        private SolidColorBrush _brushBalanceBeam = Brushes.LightGray;
        private byte _alphaTickMarks = 128; //Ranges from 0 to 255;
        #endregion
        #region Default Cosmetics
        private double _arrowSize;
        private double _arrowFontSize;
        private double _comparisonFontSize;
        private SolidColorBrush _brushPaletteDefault;
        private SolidColorBrush _brushForegroundPositiveDefault;//
        private SolidColorBrush _brushBackgroundPositiveDefault;
        private SolidColorBrush _brushForegroundNegativeDefault;
        private SolidColorBrush _brushBackgroundNegativeDefault;
        private SolidColorBrush _brushHighlightDefault;
        private double _alphaHighlight;
        private double _alphaPotential;
        #endregion
        #region Properties
        private double ArrowHeight { get { return Math.Max(canvas1.Height * .5 - _canvasPadding, 0); } }
        private double ArrowLabelOffset { get { return FontSizeArrows * .7; } }
        public SolidColorBrush BrushForegroundPotential { get { return new SolidColorBrush(Color.Add(_brushForegroundPositive.Color, _brushForegroundNegative.Color)); } }
        private double CanvasRadiusX { get { return this.canvas1.ActualWidth * .5; } }
        private double CanvasRadiusY { get { return this.canvas1.ActualHeight * .5; } }
        private string Diagnosis1NameShort { get { return EnumConversion.DiagnosisNameShort(_module, _diagnosisID1); } }
        private string Diagnosis2NameShort { get { return EnumConversion.DiagnosisNameShort(_module, _diagnosisID2); } }
        private byte DiagnosisCountInModule { get { return EnumConversion.MaxDiagnosisID(_module); } }
        private double FontSizeArrows { get { return sldArrowFontSize.Value; } }
        private double FontSizeComparison { get { return sldComparisonFontSize.Value; } }
        private double StrokeThicknessArrow { get { return sldArrowSize.Value; } }
        private double StrokeThicknessArrowTotal { get { return sldArrowSize.Value * 1.3; } }

        #endregion
        #region Constructor, Loading & Initializing Methods
        public Window1( ) {
            InitializeComponent();
            //_dt = DataManager.ChooseCorrectTable(_module);
            //_dcEvidences = DataManager.EvidenceColumns(_module);
        }
        private void Window_Loaded( object sender, RoutedEventArgs e ) {
            this.WindowState = WindowState.Maximized;
            InitializeModuleComboBox();
            _dtSensitivities = DataManager.ChooseCorrectTable(_module);
            _dcEvidences = DataManager.EvidenceColumns(_module, false, _dtSensitivities);
            _grinder = new Grinder(_module);
            InitializeColors();
            InitializeFeatureGrid();
            UpdateFeatureGrid();
            FillComparisonGrid();
            UpdateDiagnosisAndTorqueLabels();
            _maxAbsoluteTorque = FindMaxAbsoluteTorque();
            DrawDiagram();
            ResetGrdFeature();
            SetDefaultValues();
            LoadExampleCases();
            _isWindowLoaded = true;
        }
        private void ResetAfterModuleChanged( ) {
            _isWindowLoaded = false;
            _dtSensitivities = DataManager.ChooseCorrectTable(_module);
            _dcEvidences = DataManager.EvidenceColumns(_module, false, _dtSensitivities);
            _grinder = new Grinder(_module);
            //InitializeColors is not reset;
            InitializeFeatureGrid();
            UpdateFeatureGrid();
            FillComparisonGrid();
            UpdateDiagnosisAndTorqueLabels();
            _maxAbsoluteTorque = FindMaxAbsoluteTorque();
            DrawDiagram();
            ResetGrdFeature();
            SetDefaultValues();
            LoadExampleCases();
            _isWindowLoaded = true;
        }
        private void InitializeModuleComboBox( ) {
            foreach( StudyModule module in _possibleModules ) {
                cboModule.Items.Add(module);
            }
            cboModule.SelectedIndex = 0;
            _module = (StudyModule)cboModule.SelectedItem;
        }
        private void InitializeFeatureGrid( ) {
            _diagnosisID1 = 1;
            _diagnosisID2 = 2;
            GridLengthConverter glc = TypeDescriptor.GetConverter(typeof(GridLength)) as GridLengthConverter;
            grdFeature.RowDefinitions.Clear();
            grdFeature.Children.Clear();

            RowDefinition rowHeader = new RowDefinition();
            rowHeader.Height = new GridLength(1, GridUnitType.Auto);
            grdFeature.RowDefinitions.Add(rowHeader);

            Label lblHeaderCheckBox = new Label();
            lblHeaderCheckBox.Content = "\nFeature";
            Grid.SetColumn(lblHeaderCheckBox, 0);
            grdFeature.Children.Add(lblHeaderCheckBox);

            Label lblHeaderTorquePositive = new Label();
            lblHeaderTorquePositive.Content = "Impact of\nPositive\nResult";
            lblHeaderTorquePositive.ToolTip = "Diagnostic impact equals log(LR),\nwhich corresponds to torque on the balance beam.\n\nA positive number corresponds a counter clockwise rotation of the beam,\nand thus to support of the diagnosis on the left side of the beam.";
            lblHeaderTorquePositive.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(lblHeaderTorquePositive, _grdFeatureTorquePositiveColumnIndex);
            grdFeature.Children.Add(lblHeaderTorquePositive);

            Label lblHeaderTorqueNegative = new Label();
            lblHeaderTorqueNegative.Content = "Impact of\nNegative\nResult";
            lblHeaderTorqueNegative.ToolTip = "Diagnostic impact equals log(LR),\nwhich corresponds to torque on the balance beam.\n\nA positive number corresponds a counter clockwise rotation of the beam,\nand thus to support of the diagnosis on the left side of the beam.";
            lblHeaderTorqueNegative.HorizontalContentAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(lblHeaderTorqueNegative, _grdFeatureTorqueNegativeColumnIndex);
            grdFeature.Children.Add(lblHeaderTorqueNegative);

            Label lblHeaderEntropyReductionPair = new Label();
            lblHeaderEntropyReductionPair.Content = "Uncertainty\nReduction\nfor Pair";
            lblHeaderEntropyReductionPair.HorizontalContentAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(lblHeaderEntropyReductionPair, _grdFeatureEntropyPairColumnIndex);
            grdFeature.Children.Add(lblHeaderEntropyReductionPair);

            Label lblHeaderEntropyReductionSet = new Label();
            lblHeaderEntropyReductionSet.Content = "Uncertainty\nReduction\nfor Set";
            lblHeaderEntropyReductionSet.HorizontalContentAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(lblHeaderEntropyReductionSet, _grdFeatureEntropySetColumnIndex);
            grdFeature.Children.Add(lblHeaderEntropyReductionSet);

            const string numberFormat = "+##.000;-##.000;0";
            _tblFeature.Clear();
            foreach( DataColumn dc in _dcEvidences ) {
                BeamDataSet.tblFeatureRow dr = _tblFeature.NewtblFeatureRow();
                dr.SetPositiveNull();
                dr.FeatureName = dc.ColumnName;
                dr.TorquePositive = _grinder.Torque(dc, true, _diagnosisID1, _diagnosisID2).ToString(numberFormat);
                dr.TorqueNegative = _grinder.Torque(dc, false, _diagnosisID1, _diagnosisID2).ToString(numberFormat);
                dr.SensitivityDiagnosis1 = _grinder.Sensitivity(_diagnosisID1, dc).ToString();
                dr.SensitivityDiagnosis2 = _grinder.Sensitivity(_diagnosisID2, dc).ToString();
                dr.EntropyReductionPair = double.NaN;
                dr.EntropyReductionSet = double.NaN;
                dr.EntropyReductionPairRaw = double.NaN;
                dr.EntropyReductionSetRaw = double.NaN;
                _tblFeature.AddtblFeatureRow(dr);
            }
            Thickness cellPadding = new Thickness(1);

            for( byte i = 0; i < _tblFeature.Rows.Count; i++ ) {
                DataColumn dc = _dcEvidences[i];
                //BeamDataSet.tblFeatureRow dr = _tblFeature[i];
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(1, GridUnitType.Auto);
                grdFeature.RowDefinitions.Add(rowDefinition);

                CheckBox checkBox = new CheckBox();
                checkBox.Name = "chk" + dc.ColumnName;
                checkBox.Template = (ControlTemplate)this.Resources["templateBigCheck"];
                checkBox.IsThreeState = true;
                checkBox.Content = dc.ColumnName;
                checkBox.Tag = dc;
                checkBox.IsChecked = null;
                checkBox.Background = Brushes.Transparent;
                checkBox.BorderBrush = Brushes.Transparent;
                checkBox.ToolTip = DataManager.LongerFeatureName(_module, dc.ColumnName);
                checkBox.Click += new RoutedEventHandler(checkBox_Click);
                Grid.SetRow(checkBox, i + 1);
                Grid.SetColumn(checkBox, 0);
                checkBox.Padding = cellPadding;
                checkBox.MouseEnter += new MouseEventHandler(lblTorquePositiveOrNegative_MouseEnter);
                checkBox.MouseLeave += new MouseEventHandler(lblTorquePositiveOrNegative_MouseLeave);
                grdFeature.Children.Add(checkBox);

                Label lblTorquePositive = new Label();
                lblTorquePositive.Name = "lblPositive" + dc.ColumnName;
                lblTorquePositive.Content = "-pt-"; //dr.TorquePositive;
                lblTorquePositive.Tag = dc;
                lblTorquePositive.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetRow(lblTorquePositive, i + 1);
                Grid.SetColumn(lblTorquePositive, _grdFeatureTorquePositiveColumnIndex);
                lblTorquePositive.Padding = cellPadding;
                lblTorquePositive.MouseEnter += new MouseEventHandler(lblTorquePositiveOrNegative_MouseEnter);
                lblTorquePositive.MouseLeave += new MouseEventHandler(lblTorquePositiveOrNegative_MouseLeave);
                grdFeature.Children.Add(lblTorquePositive);

                Label lblTorqueNegative = new Label();
                lblTorqueNegative.Name = "lblNegative" + dc.ColumnName;
                lblTorqueNegative.Content = "-nt-";// dr.TorqueNegative;
                lblTorqueNegative.Tag = dc;
                lblTorqueNegative.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetRow(lblTorqueNegative, i + 1);
                Grid.SetColumn(lblTorqueNegative, _grdFeatureTorqueNegativeColumnIndex);
                lblTorqueNegative.Padding = cellPadding;
                lblTorqueNegative.MouseEnter += new MouseEventHandler(lblTorquePositiveOrNegative_MouseEnter);
                lblTorqueNegative.MouseLeave += new MouseEventHandler(lblTorquePositiveOrNegative_MouseLeave);
                grdFeature.Children.Add(lblTorqueNegative);

                Label lblEntropyReductionPair = new Label();
                lblEntropyReductionPair.Name = "lblEntropyReductionPair" + dc.ColumnName;
                lblEntropyReductionPair.Content = "-DxPair-";// dr.TorqueNegative;
                lblEntropyReductionPair.Tag = dc;
                lblEntropyReductionPair.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetRow(lblEntropyReductionPair, i + 1);
                Grid.SetColumn(lblEntropyReductionPair, _grdFeatureEntropyPairColumnIndex);
                lblEntropyReductionPair.Padding = cellPadding;
                //lblUncertainty.MouseEnter += new MouseEventHandler(lblTorquePositiveOrNegative_MouseEnter);
                //lblUncertainty.MouseLeave += new MouseEventHandler(lblTorquePositiveOrNegative_MouseLeave);
                grdFeature.Children.Add(lblEntropyReductionPair);

                Label lblEntropyReductionSet = new Label();
                lblEntropyReductionSet.Name = "lblEntropyReductionSet" + dc.ColumnName;
                lblEntropyReductionSet.Content = "-DxSet-";// dr.TorqueNegative;
                lblEntropyReductionSet.Tag = dc;
                lblEntropyReductionSet.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetRow(lblEntropyReductionSet, i + 1);
                Grid.SetColumn(lblEntropyReductionSet, _grdFeatureEntropySetColumnIndex);
                lblEntropyReductionSet.Padding = cellPadding;
                //lblUncertainty.MouseEnter += new MouseEventHandler(lblTorquePositiveOrNegative_MouseEnter);
                //lblUncertainty.MouseLeave += new MouseEventHandler(lblTorquePositiveOrNegative_MouseLeave);
                grdFeature.Children.Add(lblEntropyReductionSet);
            }
            //UpdateEntropies();
        }
        private void FillComparisonGrid( ) {
            Stopwatch sw = new Stopwatch();
            double fontSize = FontSizeComparison;
            GridLengthConverter glc = TypeDescriptor.GetConverter(typeof(GridLength)) as GridLengthConverter;
            grdDiagnosisComparison.ColumnDefinitions.Clear();
            grdDiagnosisComparison.RowDefinitions.Clear();
            grdDiagnosisComparison.Children.Clear();

            ColumnDefinition columnGrandHeader = new ColumnDefinition();
            columnGrandHeader.Width = (GridLength)(new GridLengthConverter().ConvertFromString("Auto"));
            grdDiagnosisComparison.ColumnDefinitions.Add(columnGrandHeader);
            RowDefinition rowGrandHeader = new RowDefinition();
            rowGrandHeader.Height = (GridLength)(new GridLengthConverter().ConvertFromString("Auto"));
            grdDiagnosisComparison.RowDefinitions.Add(rowGrandHeader);

            TextBlock lblDummyUpperLeft = new TextBlock();
            lblDummyUpperLeft.Text = "vs.";
            lblDummyUpperLeft.LayoutTransform = new RotateTransform(-45);
            lblDummyUpperLeft.FontStyle = FontStyles.Italic;
            lblDummyUpperLeft.TextDecorations = TextDecorations.Underline;
            lblDummyUpperLeft.Padding = new Thickness(0);
            lblDummyUpperLeft.FontSize = fontSize;
            Grid.SetRow(lblDummyUpperLeft, 0);
            Grid.SetColumn(lblDummyUpperLeft, 0);
            grdDiagnosisComparison.Children.Add(lblDummyUpperLeft);

            //Label lblColumnGrandHeader = new Label();
            //_lblColumnGrandHeader.Name = "lblColumnGrandHeader";
            //_lblColumnGrandHeader.Content = "dx2 on right side of beam";//Content is set in 'HighlightSelectedComparison'.
            _lblColumnGrandHeader.FontStyle = FontStyles.Italic;
            _lblColumnGrandHeader.HorizontalAlignment = HorizontalAlignment.Center;
            _lblColumnGrandHeader.HorizontalContentAlignment = HorizontalAlignment.Center;
            _lblColumnGrandHeader.Padding = new Thickness(0);
            _lblColumnGrandHeader.FontSize = fontSize;
            Grid.SetRow(_lblColumnGrandHeader, 0);
            Grid.SetColumn(_lblColumnGrandHeader, 1);
            Grid.SetColumnSpan(_lblColumnGrandHeader, _dtSensitivities.Rows.Count + 1);//Add one because of the dx headers.
            grdDiagnosisComparison.Children.Add(_lblColumnGrandHeader);

            //Label lblRowGrandHeader = new Label();
            //_lblRowGrandHeader.Content = "dx1 on left side of beam";//Content is set in 'HighlightSelectedComparison'.
            _lblRowGrandHeader.LayoutTransform = new RotateTransform(-90);
            _lblRowGrandHeader.FontStyle = FontStyles.Italic;
            _lblRowGrandHeader.HorizontalContentAlignment = HorizontalAlignment.Center;
            _lblRowGrandHeader.Padding = new Thickness(0);
            _lblRowGrandHeader.FontSize = fontSize;
            Grid.SetRow(_lblRowGrandHeader, 1);
            Grid.SetColumn(_lblRowGrandHeader, 0);
            Grid.SetRowSpan(_lblRowGrandHeader, _dtSensitivities.Rows.Count + 1);//Add one because of the dx headers.
            grdDiagnosisComparison.Children.Add(_lblRowGrandHeader);

            ColumnDefinition columnDefinitionHeader = new ColumnDefinition();
            columnDefinitionHeader.Width = (GridLength)(new GridLengthConverter().ConvertFromString("*"));
            grdDiagnosisComparison.ColumnDefinitions.Add(columnDefinitionHeader);
            grdDiagnosisComparison.RowDefinitions.Add(new RowDefinition());
            for( byte i = 0; i < _dtSensitivities.Rows.Count; i++ ) { //Cycles through the ~9 diseases for the studyArea
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = (GridLength)(glc.ConvertFromString("60"));
                DataRow dr = DataManager.ChooseDataRow(_module, (byte)(i + 1));
                byte diagnosisID = (byte)dr[ConstantsCalculation.ColumnNameRowID];
                string diagnosisName = (string)dr[ConstantsCalculation.ColumnNameDiagnosis];

                grdDiagnosisComparison.ColumnDefinitions.Add(columnDefinition);
                grdDiagnosisComparison.RowDefinitions.Add(new RowDefinition());

                Label lblColumHeader = new Label();
                lblColumHeader.HorizontalContentAlignment = HorizontalAlignment.Right;
                lblColumHeader.Content = diagnosisName;
                lblColumHeader.FontWeight = FontWeights.Bold;
                //lblColumHeader.ToolTip = "TODO: add more elaborate description of " + diagnosisName;
                lblColumHeader.ToolTip = DataManager.LongerDiagnosisName(_module, diagnosisID);

                lblColumHeader.Padding = new Thickness(0);
                lblColumHeader.FontSize = fontSize;
                Grid.SetRow(lblColumHeader, 1);
                Grid.SetColumn(lblColumHeader, i + 2);
                grdDiagnosisComparison.Children.Add(lblColumHeader);

                Label lblRowHeader = new Label();
                lblRowHeader.Content = diagnosisName;
                lblRowHeader.FontWeight = FontWeights.Bold;
                lblRowHeader.ToolTip = DataManager.LongerDiagnosisName(_module, diagnosisID);
                lblRowHeader.Padding = new Thickness(0);
                lblRowHeader.FontSize = fontSize;
                Grid.SetRow(lblRowHeader, i + 2);
                Grid.SetColumn(lblRowHeader, 1);
                grdDiagnosisComparison.Children.Add(lblRowHeader);
            }

            ResultCollection collection = LoadResultCollection();
            double[] priors = DataManager.Priors(_module);
            sw.Start();

            //Load the torque in the upper triangle
            for( byte diagnosis1ID = 1; diagnosis1ID < DiagnosisCountInModule; diagnosis1ID++ ) {
                //Parallel.For(  1, DiagnosisCountInModule,(intDiagnosis1ID ) => {
                //   byte diagnosis1ID = (byte)intDiagnosis1ID;
                for( byte diagnosis2ID = (byte)(diagnosis1ID + 1); diagnosis2ID <= DiagnosisCountInModule; diagnosis2ID++ ) {
                    //if( diagnosis1ID != diagnosis2ID ) {
                    Label lbl = new Label();
                    lbl.Tag = new byte[] { diagnosis1ID, diagnosis2ID };
                    double torque = collection.TorqueSum(diagnosis1ID, diagnosis2ID);
                    lbl.HorizontalContentAlignment = HorizontalAlignment.Right;
                    lbl.Content = Math.Round(torque, _roundingDigitsMatrixTorque);
                    switch( Math.Sign(torque) ) {
                        case 1: lbl.ToolTip = string.Format("Pairwise Torque = {0}\n(which favors {1} over {2})", Math.Round(torque, _roundingDigitsToolTips), Diagnosis2NameShort, Diagnosis1NameShort); break;
                        case -1: lbl.ToolTip = string.Format("Pairwise Torque = {0}\n(which favors {1} over {2})", Math.Round(torque, _roundingDigitsToolTips), Diagnosis1NameShort, Diagnosis2NameShort); break;
                        default: lbl.ToolTip = string.Format("Pairwise Torque = {0}\n(which favors both diagnoses equally)", Math.Round(torque, _roundingDigitsToolTips)); break;
                    }
                    lbl.FontSize = fontSize;
                    lbl.Padding = new Thickness(0);
                    lbl.MouseDown += new MouseButtonEventHandler(lblComparison_MouseDown);
                    Grid.SetRow(lbl, diagnosis1ID + 1);
                    Grid.SetColumn(lbl, diagnosis2ID + 1);
                    grdDiagnosisComparison.Children.Add(lbl);
                }
            }
            //});

            //Load the probabilities of each diagnosis in the diagonal.
            Posterior posterior = new Posterior(_module);
            double[] posteriorValues = posterior.UpdatePosteriors(collection, priors);
            double[] suprisals = Posterior.Suprisal(posteriorValues);
            double[] entropyUncertainties = Posterior.EntropyUncertainties(posteriorValues);
            double expectedSurprisal = Posterior.ExpectedSuprisal(posteriorValues);
            const string probabilityFormat = ".000;-.000;0";
            for( byte diagnosisID = 1; diagnosisID <= DiagnosisCountInModule; diagnosisID++ ) {
                Label lbl = new Label();
                lbl.Tag = new byte[] { diagnosisID, diagnosisID };
                lbl.HorizontalContentAlignment = HorizontalAlignment.Right;
                lbl.Content = Math.Round(posteriorValues[diagnosisID - 1], _roundingDigitsMatrixProbability).ToString(probabilityFormat);//Subtract 1 for the zero-based index.
                lbl.ToolTip = string.Format("Suprisal(Disease) = -p(Disease)*Log2(p(Disease)) = {0}\nEntropyUncertainty(Disease) = [-p(Disease)*Log2(p(Disease))] - [(1-p(Disease))*Log2(1-p(Disease))]= {1}\nTotal Entropy Uncertainty = Sum(Surprisal(Disease)) = {2}",
                    Math.Round(suprisals[diagnosisID - 1], _roundingDigitsToolTips), Math.Round(entropyUncertainties[diagnosisID - 1], _roundingDigitsToolTips), Math.Round(expectedSurprisal, _roundingDigitsToolTips));

                lbl.FontSize = fontSize;
                lbl.Padding = new Thickness(0); Grid.SetRow(lbl, diagnosisID + 1);
                //The foreground and background are set in 'HighlightSelectedComparison'.
                lbl.BorderThickness = new Thickness(1.0);
                Grid.SetColumn(lbl, diagnosisID + 1);
                grdDiagnosisComparison.Children.Add(lbl);
            }

            //Load the combined probabilities in the lower triangle
            for( byte diagnosis1ID = 1; diagnosis1ID < DiagnosisCountInModule; diagnosis1ID++ ) {
                //Parallel.For(  1, DiagnosisCountInModule,(intDiagnosis1ID ) => {
                //   byte diagnosis1ID = (byte)intDiagnosis1ID;
                for( byte diagnosis2ID = (byte)(diagnosis1ID + 1); diagnosis2ID <= DiagnosisCountInModule; diagnosis2ID++ ) {
                    //if( diagnosis1ID != diagnosis2ID ) {
                    Label lbl = new Label();
                    lbl.Tag = new byte[] { diagnosis2ID, diagnosis1ID };
                    lbl.HorizontalContentAlignment = HorizontalAlignment.Right;
                    double combinedProbability = Math.Round(posteriorValues[diagnosis1ID - 1] + posteriorValues[diagnosis2ID - 1], _roundingDigitsMatrixProbability);//Subtract 1 for the zero-based index.
                    lbl.Content = combinedProbability.ToString(probabilityFormat);
                    lbl.ToolTip = string.Format("The disjoint/combined probability of these two diseases is {0}", combinedProbability);
                    lbl.FontSize = fontSize;
                    lbl.Padding = new Thickness(0);
                    lbl.MouseDown += new MouseButtonEventHandler(lblComparison_MouseDown);
                    Grid.SetRow(lbl, diagnosis2ID + 1);
                    Grid.SetColumn(lbl, diagnosis1ID + 1);
                    grdDiagnosisComparison.Children.Add(lbl);
                }
            }


            sw.Stop();
            Trace.WriteLine("FillComparison nested loop: " + sw.Elapsed.ToString());
            HighlightSelectedComparison();
        }
        private double FindMaxAbsoluteTorque( ) {
            ResultCollection collection = new ResultCollection(_dtSensitivities, _module);
            byte[] diagnoses = new byte[DiagnosisCountInModule];
            for( byte diagnosisID = 1; diagnosisID <= DiagnosisCountInModule; diagnosisID++ ) {
                diagnoses[diagnosisID - 1] = diagnosisID;
            }
            ResultCollection allPossibleCollectionTrue = LoadStraightResultCollection(true);
            double maxOfPositiveTests = LoadStraightResultCollection(true).MaxAbsoluteTorqueComponent(diagnoses);
            double maxOfNegativeTests = LoadStraightResultCollection(false).MaxAbsoluteTorqueComponent(diagnoses);
            return Math.Max(maxOfPositiveTests, maxOfNegativeTests);
        }
        private ResultCollection LoadStraightResultCollection( bool evidenceResult ) {
            ResultCollection collection = new ResultCollection(_dtSensitivities, _module);
            foreach( DataColumn evidence in _dcEvidences ) {
                collection.AddResult(new Result(evidence, (bool?)evidenceResult));
            }
            return collection;
        }
        private ResultCollection LoadResultCollection( ) {
            ResultCollection collection = new ResultCollection(_dtSensitivities, _module);
            Int32 headerRowOffset = grdFeature.ColumnDefinitions.Count;
            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                Int32 childIndex = headerRowOffset + evidenceIndex * grdFeature.ColumnDefinitions.Count;
                CheckBox item = (CheckBox)(grdFeature.Children[childIndex]);
                DataColumn evidence = (DataColumn)item.Tag;

                bool? positiveResult = item.IsChecked;
                Result result = new Result(evidence, positiveResult);
                collection.AddResult(result);
            }
            return collection;
        }
        #endregion
        #region Updating Methods
        private void UpdateFeatureGrid( ) {
            const string numberFormatTorque = "+##.000;-##.000;0";
            Int32 headerRowOffset = grdFeature.ColumnDefinitions.Count;
            const Int32 offsetTorquePositive = _grdFeatureTorquePositiveColumnIndex;
            const Int32 offsetTorqueNegative = _grdFeatureTorqueNegativeColumnIndex;
            _tblFeature.Clear();

            foreach( DataColumn dc in _dcEvidences ) {
                BeamDataSet.tblFeatureRow dr = _tblFeature.NewtblFeatureRow();
                dr.FeatureName = dc.ColumnName;
                dr.TorquePositive = _grinder.Torque(dc, true, _diagnosisID1, _diagnosisID2).ToString(numberFormatTorque);
                dr.TorqueNegative = _grinder.Torque(dc, false, _diagnosisID1, _diagnosisID2).ToString(numberFormatTorque);
                dr.SensitivityDiagnosis1 = _grinder.Sensitivity(_diagnosisID1, dc).ToString();
                dr.SensitivityDiagnosis2 = _grinder.Sensitivity(_diagnosisID2, dc).ToString();
                dr.EntropyReductionPair = double.NaN;
                dr.EntropyReductionSet = double.NaN;
                dr.EntropyReductionPairRaw = double.NaN;
                dr.EntropyReductionSetRaw = double.NaN;
                _tblFeature.AddtblFeatureRow(dr);
            }
            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                BeamDataSet.tblFeatureRow dr = _tblFeature[evidenceIndex];
                Int32 childIndexTorquePositive = headerRowOffset + evidenceIndex * grdFeature.ColumnDefinitions.Count + offsetTorquePositive;
                Int32 childIndexTorqueNegative = headerRowOffset + evidenceIndex * grdFeature.ColumnDefinitions.Count + offsetTorqueNegative;
                Label lblTorquePositive = (Label)(grdFeature.Children[childIndexTorquePositive]);
                lblTorquePositive.Content = dr.TorquePositive;
                Label lblTorqueNegative = (Label)(grdFeature.Children[childIndexTorqueNegative]);
                lblTorqueNegative.Content = dr.TorqueNegative;
                if( dr.FeatureName == "Prior" )
                    lblTorqueNegative.Content = "--";
            }
            UpdateEntropies();
        }
        private void UpdateEntropies( ) {
            const string numberFormat = "##.000";//;##.000;--";
            ResultCollection collection = LoadResultCollection();
            double[] priors = DataManager.Priors(_module);
            Posterior posterior = new Posterior(_module);
            double[] setCurrentProbabilities = posterior.UpdatePosteriors(collection, priors);

            Int32 headerRowOffset = grdFeature.ColumnDefinitions.Count;
            const Int32 offsetEntropyPair = _grdFeatureEntropyPairColumnIndex;
            const Int32 offsetEntropySet = _grdFeatureEntropySetColumnIndex;

            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                BeamDataSet.tblFeatureRow dr = _tblFeature[evidenceIndex];
                DataColumn dc = _dcEvidences[evidenceIndex];
                bool? positive = collection.Positive(dc);
                if( positive.HasValue ) dr.Positive = (bool)positive;
                else dr.SetPositiveNull();
                //dr.EndEdit();

                double[] setSensitivitiesForSingleEvidence = new double[DiagnosisCountInModule];
                for( byte dxIndex = 0; dxIndex < setSensitivitiesForSingleEvidence.Length; dxIndex++ ) {
                    byte dxID = (byte)(dxIndex + 1);
                    setSensitivitiesForSingleEvidence[dxIndex] = _grinder.Sensitivity(dxID, dc);
                }
                if( !positive.HasValue ) {
                    double[] pairCurrentProbabilitiesUnscaled = { setCurrentProbabilities[_diagnosisID1 - 1], setCurrentProbabilities[_diagnosisID2 - 1] };
                    double[] pairCurrentProbabilitiesScaled = Posterior.RescaleToUnity(pairCurrentProbabilitiesUnscaled);
                    double[] pairSensitivitiesForSingleEvidence = { setSensitivitiesForSingleEvidence[_diagnosisID1 - 1], setSensitivitiesForSingleEvidence[_diagnosisID2 - 1] };

                    dr.EntropyReductionPair = Posterior.ExpectedProportionalReductionInEntropyUncertainty(pairCurrentProbabilitiesScaled, pairSensitivitiesForSingleEvidence);
                    dr.EntropyReductionSet = Posterior.ExpectedProportionalReductionInEntropyUncertainty(setCurrentProbabilities, setSensitivitiesForSingleEvidence);

                    dr.EntropyReductionPairRaw = Posterior.ExpectedRawReductionInEntropyUncertainty(pairCurrentProbabilitiesScaled, pairSensitivitiesForSingleEvidence);
                    dr.EntropyReductionSetRaw = Posterior.ExpectedRawReductionInEntropyUncertainty(setCurrentProbabilities, setSensitivitiesForSingleEvidence);
                } else {
                    dr.EntropyReductionPair = 0;
                    dr.EntropyReductionSet = 0;
                }

            }
            //maxReductionSet = Math.Max(entropyReductionSet, maxReductionSet);
            List<Int32> indicesOfMaxEntropyReductionPair = IndicesOfMaxEntropyReductionPair();
            List<Int32> indicesOfTopFiveEntropyReductionPair = IndicesOfTopEntropyReductionPair();
            List<Int32> indicesOfMaxEntropyReductionSet = IndicesOfMaxEntropyReductionSet();
            List<Int32> indicesOfTopFiveEntropyReductionSet = IndicesOfTopFiveEntropyReductionSet();
            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                BeamDataSet.tblFeatureRow dr = _tblFeature[evidenceIndex];
                Int32 childIndexEntropyPair = headerRowOffset + evidenceIndex * grdFeature.ColumnDefinitions.Count + offsetEntropyPair;
                Int32 childIndexEntropySet = headerRowOffset + evidenceIndex * grdFeature.ColumnDefinitions.Count + offsetEntropySet;

                Label lblEntropyPair = (Label)(grdFeature.Children[childIndexEntropyPair]);
                Label lblEntropySet = (Label)(grdFeature.Children[childIndexEntropySet]);
                //double rawPair = Posterior.ExpectedRawReductionInEntropyUncertainty(pairCurrentProbabilities, pairSensitivitiesForSingleEvidence);
                if( dr.IsPositiveNull() && dr.FeatureName != "Prior" ) {
                    lblEntropyPair.Content = Math.Round(dr.EntropyReductionPair, _roundingDigitsMatrixEntropyReduction).ToString(numberFormat);
                    lblEntropySet.Content = Math.Round(dr.EntropyReductionSet, _roundingDigitsMatrixEntropyReduction).ToString(numberFormat);
                } else {
                    lblEntropyPair.Content = "--";
                    lblEntropySet.Content = "--";
                }

                lblEntropyPair.ToolTip = string.Format("Proportional Reduced Entropy = {0}\nRaw Reduced Entropy = {1}", dr.EntropyReductionPair, dr.EntropyReductionPairRaw);
                lblEntropySet.ToolTip = string.Format("Proportional Reduced Entropy = {0}\nRaw Reduced Entropy = {1}", dr.EntropyReductionSet, dr.EntropyReductionSetRaw);

                if( indicesOfMaxEntropyReductionPair.Contains(evidenceIndex) ) lblEntropyPair.FontWeight = FontWeights.Bold;
                else lblEntropyPair.FontWeight = FontWeights.Normal;
                if( indicesOfTopFiveEntropyReductionPair.Contains(evidenceIndex) ) lblEntropyPair.Foreground = Brushes.Black;
                else lblEntropyPair.Foreground = _brushDemphasizedText;

                if( indicesOfMaxEntropyReductionSet.Contains(evidenceIndex) ) lblEntropySet.FontWeight = FontWeights.Bold;
                else lblEntropySet.FontWeight = FontWeights.Normal;
                if( indicesOfTopFiveEntropyReductionSet.Contains(evidenceIndex) ) lblEntropySet.Foreground = Brushes.Black;
                else lblEntropySet.Foreground = _brushDemphasizedText;
            }
        }
        private void HighlightSelectedComparison( ) {
            Brush brushLowCombinedPosterior = Brushes.LightGray;
            double posteriorThreshold = sldPosteriorThreshold.Value;
            bool isUpperSelected = _diagnosisID1 <= _diagnosisID2;
            if( isUpperSelected ) {
                //_lblRowGrandHeader.Content = "dx1 on left side of beam";
                //_lblColumnGrandHeader.Content = "dx2 on right side of beam";
                _lblRowGrandHeader.Content = "Dx 1 on left side of beam";
                _lblColumnGrandHeader.Content = "Diagnosis 2 on right side of beam";

            } else {
                //_lblRowGrandHeader.Content = "dx1 on right side of beam";
                //_lblColumnGrandHeader.Content = "dx2 on left side of beam";
                _lblRowGrandHeader.Content = "Dx 1 on right side of beam";
                _lblColumnGrandHeader.Content = "Diagnosis 2 on left side of beam";
            }
            //lblColumnGrandHeader.Content = "dx2 on right side of beam";
            //lblc
            //   grdDiagnosisComparison.Children["grdDiagnosisComparison.Children"]

            foreach( UIElement control in grdDiagnosisComparison.Children ) {
                if( control.GetType() == typeof(Label) ) {
                    Label lbl = (Label)control;
                    Int32 gridRowIndex = Grid.GetRow(lbl);
                    Int32 gridColumnIndex = Grid.GetColumn(lbl);
                    lbl.Background = Brushes.Transparent;
                    lbl.Foreground = Brushes.Black;
                    lbl.FontWeight = FontWeights.Normal;
                    lbl.BorderThickness = new Thickness(1);
                    lbl.BorderBrush = Brushes.Transparent;

                    if( lbl.Tag == null ) {//The row and column headers;
                        if( gridRowIndex == 1 && (gridColumnIndex == (_diagnosisID2 + 1)) ) {
                            lbl.Background = _brushHighlight;// _colorGridComparisonSelected;
                        } else if( gridColumnIndex == 1 && (gridRowIndex == (_diagnosisID1 + 1)) ) {
                            lbl.Background = _brushHighlight;//_colorGridComparisonSelected;
                        }
                    } else { //The numeric elements of the grid;
                        byte[] tag = (byte[])lbl.Tag;
                        bool isRowEven = (Math.IEEERemainder(gridRowIndex, 2) == 0);
                        bool isCellTorque = tag[0] < tag[1];
                        bool isCellPosterior = tag[0] > tag[1];

                        if( (tag[0] == _diagnosisID1 && tag[1] == _diagnosisID2) || (tag[0] == _diagnosisID2 && tag[1] == _diagnosisID1) ) {
                            lbl.Background = _brushHighlight;
                            if( isCellTorque == isUpperSelected ) lbl.BorderBrush = Brushes.DarkBlue;//Border only the selected cell in the correct triagonle.
                        } else if( tag[0] == tag[1] ) {
                            lbl.Background = Brushes.CadetBlue;
                            lbl.Foreground = Brushes.AntiqueWhite;
                            if( tag[0] == _diagnosisID1 || tag[1] == _diagnosisID2 ) {
                                lbl.Background = Brushes.Blue;
                            }
                        } else if( !isRowEven && isCellTorque ) {
                            lbl.Background = (Brush)Resources["GradientBrushTorque"];
                        } else if( isCellPosterior ) {
                            if( !isRowEven ) lbl.Background = (Brush)Resources["GradientBrushPosterior"];
                        }
                        if( isCellPosterior && (Convert.ToDouble(lbl.Content) < posteriorThreshold) ) lbl.Foreground = brushLowCombinedPosterior;
                    }
                }
            }
            //lbl.FontWeight = FontWeights.Normal;
            //lbl.Background = Brushes.Transparent;//#00FFFFFF;
        }
        private void UpdateDiagnosisAndTorqueLabels( ) {
            ResultCollection collection = LoadResultCollection();
            double torqueSum = collection.TorqueSum(_diagnosisID1, _diagnosisID2);

            string oddsString = ResultCollection.ToOddsString(torqueSum, 2);
            //lblCurrentDiagnoses.Content = string.Format("Diagnosis 1: {0} vs Diagnosis 2: {1}:  Odds are {2} (for {0} to {1})", _diagnosis1.ToString(), _diagnosis2.ToString(), oddsString);
            string diagnosis1LongName = DataManager.LongerDiagnosisName(_module, _diagnosisID1);
            string diagnosis2LongName = DataManager.LongerDiagnosisName(_module, _diagnosisID2);
            lblCurrentDiagnoses.Content = string.Format("Naive Odds for [{1}] to [{2}] are {0}", oddsString, diagnosis1LongName, diagnosis2LongName);

            switch( Math.Sign(torqueSum) ) {
                case 1: lblLeverageSum.Content = string.Format("Naive Torque: {0} (which favors {1} over {2})", Math.Round(torqueSum, _roundingDigitsToolTips), Diagnosis2NameShort, Diagnosis1NameShort); break;
                case -1: lblLeverageSum.Content = string.Format("Naive Torque: {0} (which favors {1} over {2})", Math.Round(torqueSum, _roundingDigitsToolTips), Diagnosis1NameShort, Diagnosis2NameShort); break;
                default: lblLeverageSum.Content = string.Format("Naive Torque: {0} (which favors both diagnoses equally)", Math.Round(torqueSum, _roundingDigitsToolTips)); break;
            }
        }
        private void SaveImage( string filename ) {//http://www.vistax64.com/avalon/103701-using-wpf-generate-image.html
            Control control = this;
            const Int32 dpi = 96;
            RenderTargetBitmap rtb = new RenderTargetBitmap((Int32)control.Width, (Int32)control.Height, dpi, dpi, PixelFormats.Pbgra32);
            control.Arrange(new Rect(new Size((Int32)control.Width, (Int32)control.Height)));
            rtb.Render(control);

            BitmapEncoder encoder;
            switch( System.IO.Path.GetExtension(filename) ) {
                case ".bmp": encoder = new BmpBitmapEncoder(); break;
                case ".png": encoder = new PngBitmapEncoder(); break;
                default: throw new ArgumentOutOfRangeException("filename", filename, "The extension must be either bmp or png.");
            }
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using( Stream fs = File.Create(filename) ) {
                encoder.Save(fs);
            }
        }
        private void SetDefaultValues( ) {
            _arrowSize = sldArrowSize.Value;
            _arrowFontSize = sldArrowFontSize.Value;
            _comparisonFontSize = sldComparisonFontSize.Value;
            _brushPaletteDefault = _brushPalette;
            _brushForegroundPositiveDefault = _brushForegroundPositive;
            _brushBackgroundPositiveDefault = _brushBackgroundPositive;
            _brushForegroundNegativeDefault = _brushForegroundNegative;
            _brushBackgroundNegativeDefault = _brushBackgroundNegative;
            _brushHighlightDefault = _brushHighlight;
            _alphaHighlight = sldAlphaHighlight.Value;
            _alphaPotential = sldAlphaPotential.Value;
        }
        private void ResetDefaultValues( bool redrawForm ) {
            sldArrowSize.Value = _arrowSize;
            sldArrowFontSize.Value = _arrowFontSize;
            sldComparisonFontSize.Value = _comparisonFontSize;
            SetComboBoxColor(cboBrushPalette, _brushPaletteDefault);
            SetComboBoxColor(cboBrushPositiveForeground, _brushForegroundPositiveDefault);
            SetComboBoxColor(cboBrushPositiveBackground, _brushBackgroundPositiveDefault);
            SetComboBoxColor(cboBrushNegativeForeground, _brushForegroundNegativeDefault);
            SetComboBoxColor(cboBrushNegativeBackground, _brushBackgroundNegativeDefault);
            SetComboBoxColor(cboBrushHighlight, _brushHighlightDefault);
            sldAlphaHighlight.Value = _alphaHighlight;
            sldAlphaPotential.Value = _alphaPotential;

            if( redrawForm && _isWindowLoaded ) {
                DrawDiagram();
                UpdateFeatureGrid();
                HighlightSelectedComparison();
            }
        }
        #endregion
        #region Large Entropy Reductions
        private List<Int32> IndicesOfMaxEntropyReductionPair( ) {
            const double toleranceEquivalence = 1e-4;
            List<Int32> indices = new List<Int32>(1);
            double maxValue = 0;
            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                maxValue = Math.Max(_tblFeature[evidenceIndex].EntropyReductionPair, maxValue);
            }
            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                if( Math.Abs(maxValue - _tblFeature[evidenceIndex].EntropyReductionPair) < toleranceEquivalence ) {
                    indices.Add(evidenceIndex);
                }
            }
            return indices;
        }
        private List<Int32> IndicesOfTopEntropyReductionPair( ) {
            const double toleranceEquivalence = 1e-4;
            List<Int32> indices = new List<Int32>(5);
            double[] scores = new double[_dcEvidences.Length];
            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                scores[evidenceIndex] = _tblFeature[evidenceIndex].EntropyReductionPair;
            }
            Array.Sort(scores);
            Array.Reverse(scores);
            Int32 position = (Int32)sldTopReductions.Value - 1;
            double quantileValue = scores[position];

            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                if( _tblFeature[evidenceIndex].EntropyReductionPair > (quantileValue - toleranceEquivalence) ) {
                    indices.Add(evidenceIndex);
                }
            }
            return indices;
        }
        private List<Int32> IndicesOfMaxEntropyReductionSet( ) {
            const double toleranceEquivalence = 1e-4;
            List<Int32> indices = new List<Int32>(1);
            double maxValue = 0;
            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                maxValue = Math.Max(_tblFeature[evidenceIndex].EntropyReductionSet, maxValue);
            }
            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                if( Math.Abs(maxValue - _tblFeature[evidenceIndex].EntropyReductionSet) < toleranceEquivalence ) {
                    indices.Add(evidenceIndex);
                }
            }
            return indices;
        }
        private List<Int32> IndicesOfTopFiveEntropyReductionSet( ) {
            const double toleranceEquivalence = 1e-4;
            List<Int32> indices = new List<Int32>(5);
            double[] scores = new double[_dcEvidences.Length];
            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                scores[evidenceIndex] = _tblFeature[evidenceIndex].EntropyReductionSet;
            }
            Array.Sort(scores);
            Array.Reverse(scores);
            Int32 position = (Int32)sldTopReductions.Value - 1;
            double quantileValue = scores[position];

            for( Int32 evidenceIndex = 0; evidenceIndex < _dcEvidences.Length; evidenceIndex++ ) {
                if( _tblFeature[evidenceIndex].EntropyReductionSet > (quantileValue - toleranceEquivalence) ) {
                    indices.Add(evidenceIndex);
                }
            }
            return indices;
        }
        #endregion
        #region Drawing Methods
        private void DrawDiagram( ) {
            canvas1.Children.Clear();
            ResultCollection collection = LoadResultCollection();
            double torqueSum = collection.TorqueSum(_diagnosisID1, _diagnosisID2);
            DrawBeam(torqueSum);
            foreach( Result result in collection ) {
                DrawArrow(result);
            }
            DrawTotalArrows(torqueSum);
            IntializePotentialTorques();
        }
        private void DrawBeam( double torqueSum ) {
            Line balanceBeam = new Line();
            balanceBeam.SnapsToDevicePixels = true;
            balanceBeam.Stroke = _brushBalanceBeam;
            balanceBeam.StrokeThickness = StrokeThicknessArrow;
            balanceBeam.StrokeStartLineCap = PenLineCap.Triangle;
            balanceBeam.StrokeEndLineCap = PenLineCap.Triangle;
            balanceBeam.X1 = _canvasPadding;
            balanceBeam.X2 = canvas1.ActualWidth - _canvasPadding;
            balanceBeam.Y1 = CanvasRadiusY;
            balanceBeam.Y2 = CanvasRadiusY;
            canvas1.Children.Add(balanceBeam);

            Line fulcrum = new Line();
            fulcrum.SnapsToDevicePixels = true;
            fulcrum.Stroke = _brushBalanceBeam;
            fulcrum.StrokeThickness = StrokeThicknessArrow;
            fulcrum.StrokeStartLineCap = PenLineCap.Triangle;
            fulcrum.X1 = CanvasRadiusX;
            fulcrum.X2 = CanvasRadiusX;
            fulcrum.Y1 = CanvasRadiusY + StrokeThicknessArrow;
            fulcrum.Y2 = fulcrum.Y1 + StrokeThicknessArrow;
            canvas1.Children.Add(fulcrum);

            Label lblLowerLeft = new Label();
            if( torqueSum < 0 ) lblLowerLeft.FontStyle = FontStyles.Oblique;
            else lblLowerLeft.Foreground = _brushDemphasizedText;
            lblLowerLeft.Content = Diagnosis1NameShort + " is more favored as the left side dips lower";
            lblLowerLeft.FontSize = FontSizeArrows;
            lblLowerLeft.Padding = new Thickness(0);
            lblLowerLeft.HorizontalAlignment = HorizontalAlignment.Left;
            Canvas.SetLeft(lblLowerLeft, _canvasPadding);
            Canvas.SetBottom(lblLowerLeft, CanvasRadiusY - FontSizeArrows * .5);
            canvas1.Children.Add(lblLowerLeft);

            Label lblLowerRight = new Label();
            if( torqueSum > 0 ) lblLowerRight.FontStyle = FontStyles.Oblique;
            else lblLowerRight.Foreground = _brushDemphasizedText;
            lblLowerRight.Content = Diagnosis2NameShort + " is more favored as the right side dips lower";
            lblLowerRight.FontSize = FontSizeArrows;
            lblLowerRight.Padding = new Thickness(0);
            Canvas.SetRight(lblLowerRight, _canvasPadding);
            Canvas.SetBottom(lblLowerRight, CanvasRadiusY - FontSizeArrows * .5);
            canvas1.Children.Add(lblLowerRight);

            foreach( double proportion in new double[] { 0, .1, .2, .3, .4, .6, .7, .8, .9, 1 } ) {
                DrawTickMark(balanceBeam, proportion);
            }
        }
        private void DrawTickMark( Line beam, double proportion ) {
            Line tick = new Line();
            double beamLength = beam.X2 - beam.X1;
            tick.Stroke = new SolidColorBrush(ChangeTransparency(_brushPalette.Color, _alphaTickMarks));
            tick.StrokeThickness = 2;
            tick.SnapsToDevicePixels = true;
            tick.X1 = _canvasPadding + beamLength * proportion;
            tick.X2 = tick.X1;
            tick.Y1 = beam.Y1 - (beam.StrokeThickness * .25 - 0);
            tick.Y2 = beam.Y2 + (beam.StrokeThickness * .25 - 0);// +20;
            canvas1.Children.Add(tick);
        }
        public void IntializePotentialTorques( ) {
            byte alpha = Convert.ToByte(sldAlphaPotential.Value);

            //Up Arrow
            _potentialTorqueLineUp.SnapsToDevicePixels = true;
            _potentialTorqueLineUp.Stroke = new SolidColorBrush(ChangeTransparency(_brushBackgroundPositive.Color, alpha));
            _potentialTorqueLineUp.Y1 = 0;
            _potentialTorqueLineUp.Y2 = ArrowHeight;
            _potentialTorqueLineUp.X1 = 0;
            _potentialTorqueLineUp.X2 = _potentialTorqueLineUp.X1;
            _potentialTorqueLineUp.StrokeThickness = StrokeThicknessArrow;
            _potentialTorqueLineUp.StrokeEndLineCap = PenLineCap.Triangle;
            Canvas.SetTop(_potentialTorqueLineUp, CanvasRadiusY);
            Canvas.SetLeft(_potentialTorqueLineUp, CanvasRadiusX);
            _potentialTorqueLineUp.Visibility = Visibility.Hidden;
            canvas1.Children.Add(_potentialTorqueLineUp);

            //Down Arrow
            _potentialTorqueLineDown.SnapsToDevicePixels = true;
            _potentialTorqueLineDown.Stroke = new SolidColorBrush(ChangeTransparency(_brushBackgroundNegative.Color, alpha));
            _potentialTorqueLineDown.Y1 = 0;
            _potentialTorqueLineDown.Y2 = -ArrowHeight;
            _potentialTorqueLineDown.X1 = 0;
            _potentialTorqueLineDown.X2 = _potentialTorqueLineDown.X1;
            _potentialTorqueLineDown.StrokeThickness = StrokeThicknessArrow;
            _potentialTorqueLineDown.StrokeEndLineCap = PenLineCap.Triangle;
            Canvas.SetTop(_potentialTorqueLineDown, CanvasRadiusY);
            Canvas.SetLeft(_potentialTorqueLineDown, CanvasRadiusX);
            _potentialTorqueLineDown.Visibility = Visibility.Hidden;
            canvas1.Children.Add(_potentialTorqueLineDown);

            //Up Textblock
            _potentialTorqueTextUp.FontSize = FontSizeArrows;
            _potentialTorqueTextUp.VerticalAlignment = VerticalAlignment.Bottom;
            _potentialTorqueTextUp.SnapsToDevicePixels = true;
            _potentialTorqueTextUp.Foreground = new SolidColorBrush(ChangeTransparency(BrushForegroundPotential.Color, alpha));
            Canvas.SetTop(_potentialTorqueTextUp, CanvasRadiusY + 1);
            _potentialTorqueTextUp.HorizontalAlignment = HorizontalAlignment.Left;
            _potentialTorqueTextUp.LayoutTransform = new RotateTransform(-90);
            canvas1.Children.Add(_potentialTorqueTextUp);

            //Down Textblock
            _potentialTorqueTextDown.FontSize = FontSizeArrows;
            _potentialTorqueTextDown.VerticalAlignment = VerticalAlignment.Bottom;
            _potentialTorqueTextDown.SnapsToDevicePixels = true;
            _potentialTorqueTextDown.Foreground = new SolidColorBrush(ChangeTransparency(BrushForegroundPotential.Color, alpha));
            Canvas.SetBottom(_potentialTorqueTextDown, CanvasRadiusY + 1);
            _potentialTorqueTextDown.HorizontalAlignment = HorizontalAlignment.Right;
            _potentialTorqueTextDown.LayoutTransform = new RotateTransform(-90);
            canvas1.Children.Add(_potentialTorqueTextDown);
        }
        public void PotentialTorquesShow( DataColumn dc ) {
            //if( !collection.ColumnExistsInCollection(dc) ) {
            //Up Arrow
            double torquePositiveTest = _grinder.Torque(dc, true, _diagnosisID1, _diagnosisID2);
            double xPositive = -TranslateTorqueToX(torquePositiveTest, _maxAbsoluteTorque);
            _potentialTorqueLineUp.X1 = xPositive;
            _potentialTorqueLineUp.X2 = _potentialTorqueLineUp.X1;
            _potentialTorqueLineUp.Visibility = Visibility.Visible;

            //Down Arrow
            double torqueNegativeTest = _grinder.Torque(dc, false, _diagnosisID1, _diagnosisID2);
            double xNegative = TranslateTorqueToX(torqueNegativeTest, _maxAbsoluteTorque);
            _potentialTorqueLineDown.X1 = xNegative;
            _potentialTorqueLineDown.X2 = _potentialTorqueLineDown.X1;
            if( dc.ColumnName != "Prior" )
                _potentialTorqueLineDown.Visibility = Visibility.Visible;

            //Up TextBlock
            _potentialTorqueTextUp.Text = dc.ColumnName;
            _potentialTorqueTextUp.ToolTip = string.Format("Potential\n{0}\nTorque: {1}", dc.ColumnName, torquePositiveTest);
            Canvas.SetLeft(_potentialTorqueTextUp, CanvasRadiusX + xPositive - ArrowLabelOffset);
            _potentialTorqueTextUp.Visibility = Visibility.Visible;

            //Down TextBlock
            _potentialTorqueTextDown.Text = dc.ColumnName;
            _potentialTorqueTextDown.ToolTip = string.Format("Potential\n{0}\nTorque: {1}", dc.ColumnName, torqueNegativeTest);
            Canvas.SetLeft(_potentialTorqueTextDown, CanvasRadiusX + xNegative - ArrowLabelOffset);
            if( dc.ColumnName != "Prior" )
                _potentialTorqueTextDown.Visibility = Visibility.Visible;
        }
        private void PotentialTorquesHide( ) {
            _potentialTorqueLineUp.Visibility = Visibility.Hidden;
            _potentialTorqueLineDown.Visibility = Visibility.Hidden;
            _potentialTorqueTextUp.Visibility = Visibility.Hidden;
            _potentialTorqueTextDown.Visibility = Visibility.Hidden;
        }
        public void DrawArrow( Result result ) {
            double torque = _grinder.Torque(result.Evidence, result.PositiveTest, _diagnosisID1, _diagnosisID2);
            if( Math.Abs(torque) > 0 ) {
                double x = TranslateTorqueToX(torque, _maxAbsoluteTorque);
                string toolTipString = string.Format("{0}\nAdditive Torque: {1}", DataManager.LongerFeatureName(_module, result.Evidence.ColumnName), Math.Round(torque, _roundingDigitsToolTips));
                Line line = new Line();
                line.StrokeThickness = StrokeThicknessArrow;
                line.StrokeEndLineCap = PenLineCap.Triangle;
                line.ToolTip = toolTipString;
                TextBlock textBlock = new TextBlock();
                textBlock.Text = result.Evidence.ColumnName;
                textBlock.Padding = new Thickness(0);
                textBlock.ToolTip = toolTipString;
                textBlock.FontSize = FontSizeArrows;
                textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                textBlock.SnapsToDevicePixels = true;

                if( !result.PositiveTest.Value ) {//&& result.Evidence.ColumnName != "Prior" ) {
                    line.X1 = x;// - arrowRadius;
                    line.X2 = line.X1;
                    line.Y1 = 0;
                    line.Y2 = -ArrowHeight;
                    line.Stroke = _brushBackgroundNegative;
                    Canvas.SetLeft(textBlock, CanvasRadiusX + x - ArrowLabelOffset);
                    Canvas.SetBottom(textBlock, CanvasRadiusY + 1);
                    textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                    textBlock.LayoutTransform = new RotateTransform(-90);
                } else if( result.PositiveTest.Value ) {
                    line.X1 = -x;// - arrowRadius;
                    line.X2 = line.X1;
                    line.Y1 = 0;
                    line.Y2 = ArrowHeight;
                    line.Stroke = _brushBackgroundPositive;
                    Canvas.SetLeft(textBlock, CanvasRadiusX - x - ArrowLabelOffset);
                    Canvas.SetTop(textBlock, CanvasRadiusY + 1);
                    textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    textBlock.LayoutTransform = new RotateTransform(-90);
                }
                Canvas.SetLeft(line, CanvasRadiusX);
                Canvas.SetTop(line, CanvasRadiusY);
                canvas1.Children.Add(line);
                canvas1.Children.Add(textBlock);
            }
        }
        public void DrawTotalArrows( double torqueTotal ) {
            if( this.chkDisplayTotal.IsChecked.HasValue && !this.chkDisplayTotal.IsChecked.Value ) return;
            double x = TranslateTorqueToX(torqueTotal, _maxAbsoluteTorque);
            string toolTipString = string.Format("Total Torque: {0}", Math.Round(torqueTotal, _roundingDigitsToolTips));

            bool isArrowLocationClipped = false;
            byte alpha = Convert.ToByte(sldAlphaHighlight.Value);

            if( CanvasRadiusX < Math.Abs(x) ) {//For clipping.
                isArrowLocationClipped = true;
                switch( Math.Sign(x) ) {
                    case -1: x = -CanvasRadiusX + _arrowTotalRadiusOffset; break;
                    case 1: x = CanvasRadiusX - _arrowTotalRadiusOffset; break;
                }
            }
            //Up Arrow
            Line lineUp = new Line();
            lineUp.StrokeThickness = StrokeThicknessArrowTotal;
            lineUp.StrokeEndLineCap = PenLineCap.Triangle;
            lineUp.ToolTip = toolTipString;
            lineUp.X1 = x;// -arrowRadius;
            lineUp.X2 = lineUp.X1;
            lineUp.Y1 = 0;
            lineUp.Y2 = -ArrowHeight * .5;
            lineUp.Stroke = _brushHighlight;
            Canvas.SetLeft(lineUp, CanvasRadiusX);
            Canvas.SetTop(lineUp, CanvasRadiusY);
            canvas1.Children.Add(lineUp);

            //Down Arrow
            Line lineDown = new Line();
            lineDown.StrokeThickness = StrokeThicknessArrowTotal;
            lineDown.StrokeEndLineCap = PenLineCap.Triangle;
            lineDown.ToolTip = toolTipString;
            lineDown.X1 = -x;// -arrowRadius;
            lineDown.X2 = lineDown.X1;
            lineDown.Y1 = 0;
            lineDown.Y2 = ArrowHeight * .5;
            lineDown.Stroke = _brushHighlight;
            Canvas.SetLeft(lineDown, CanvasRadiusX);
            Canvas.SetTop(lineDown, CanvasRadiusY);
            canvas1.Children.Add(lineDown);

            //Up Label	
            TextBlock lblUp = new TextBlock();
            lblUp.Text = "Total Torque";
            if( isArrowLocationClipped ) lblUp.Text += " (location is clipped)";
            lblUp.ToolTip = toolTipString;
            lblUp.Foreground = new SolidColorBrush(ChangeTransparency(_brushDemphasizedText.Color, alpha));
            lblUp.FontSize = FontSizeArrows;
            lblUp.VerticalAlignment = VerticalAlignment.Bottom;
            lblUp.SnapsToDevicePixels = true;
            lblUp.HorizontalAlignment = HorizontalAlignment.Right;
            lblUp.LayoutTransform = new RotateTransform(-90);
            Canvas.SetLeft(lblUp, CanvasRadiusX + x - ArrowLabelOffset);
            Canvas.SetBottom(lblUp, CanvasRadiusY + 1);
            canvas1.Children.Add(lblUp);

            //Down Label	
            TextBlock lblDown = new TextBlock();
            lblDown.Text = "Total Torque";
            if( isArrowLocationClipped ) lblDown.Text += " (location is clipped)";
            lblDown.ToolTip = toolTipString;
            lblDown.Foreground = new SolidColorBrush(ChangeTransparency(_brushDemphasizedText.Color, alpha));
            lblDown.FontSize = FontSizeArrows;
            lblDown.VerticalAlignment = VerticalAlignment.Bottom;
            lblDown.SnapsToDevicePixels = true;
            lblDown.HorizontalAlignment = HorizontalAlignment.Left;
            lblDown.LayoutTransform = new RotateTransform(-90);
            Canvas.SetLeft(lblDown, CanvasRadiusX - x - ArrowLabelOffset);
            Canvas.SetTop(lblDown, CanvasRadiusY + 1);
            canvas1.Children.Add(lblDown);
        }
        public double TranslateTorqueToX( double torque, double maxAbsoluteTorque ) {
            return -1 * torque * (CanvasRadiusX - _canvasPadding) / maxAbsoluteTorque;
        }
        private void ResetGrdFeature( ) {
            foreach( Control control in grdFeature.Children ) {
                if( Grid.GetRow(control) == 0 ) {
                    control.Foreground = _brushForegroundHeader;
                    control.FontWeight = FontWeights.Bold;
                } else {
                    control.Foreground = _brushDemphasizedText;//_brushForegroundMissing;
                    control.FontWeight = FontWeights.Normal;
                }
                if( control.GetType() == typeof(CheckBox) ) {
                    CheckBox chk = (CheckBox)control;
                    chk.IsChecked = null;
                }
            }
            UpdateEntropies();
        }
        private void HighlightPotentialLabel( DataColumn dc ) {
            foreach( Control control in grdFeature.Children ) {
                if( (DataColumn)control.Tag == dc && control.GetType() == typeof(CheckBox) ) {
                    control.Foreground = BrushForegroundPotential;
                }
            }
        }
        private void ResetPotentialLabel( DataColumn dc ) {
            foreach( Control control in grdFeature.Children ) {
                if( (DataColumn)control.Tag == dc && control.GetType() == typeof(CheckBox) ) {
                    SetCheckBoxColor((CheckBox)control);
                }
            }
        }
        private void SetCheckBoxColor( CheckBox chk ) {
            if( chk.Name == "chkPrior" && chk.IsChecked.HasValue && !chk.IsChecked.Value )
                chk.IsChecked = true;
                
            if( chk.IsChecked == true ) {
                chk.Foreground = _brushForegroundPositive;
                //chk.Background = _brushForegroundPositive;
            } else if( chk.IsChecked == false ) {
                chk.Foreground = _brushForegroundNegative;
                //chk.Background = _brushForegroundNegative;
            } else {
                chk.Foreground = _brushDemphasizedText;//_brushForegroundMissing;
                //chk.Background = _brushDemphasizedText;//_brushForegroundMissing;
            }
            SetGrdFeatureTorqueLabel(chk);
        }
        public void SetGrdFeatureTorqueLabel( CheckBox chk ) {
            bool? isChecked = chk.IsChecked;
            string desiredLabelPositive = "lblPositive" + ((DataColumn)chk.Tag).ColumnName;
            string desiredLabelNegative = "lblNegative" + ((DataColumn)chk.Tag).ColumnName;
            Brush colorLabelPositive;
            Brush colorLabelNegative;
            FontWeight weightLabelPositive;
            FontWeight weightLabelNegative;
            if( isChecked == true ) {
                colorLabelPositive = _brushForegroundPositive;
                colorLabelNegative = _brushDemphasizedText;//_brushForegroundMissing;
                weightLabelPositive = FontWeights.Heavy;
                weightLabelNegative = FontWeights.UltraLight;
            } else if( isChecked == false ) {
                colorLabelPositive = _brushDemphasizedText;//_brushForegroundMissing;
                colorLabelNegative = _brushForegroundNegative;
                weightLabelPositive = FontWeights.UltraLight;
                weightLabelNegative = FontWeights.Heavy;
            } else {
                colorLabelPositive = _brushDemphasizedText;//_brushForegroundMissing;
                colorLabelNegative = _brushDemphasizedText;//_brushForegroundMissing;
                weightLabelPositive = FontWeights.Normal;
                weightLabelNegative = FontWeights.Normal;
            }
            foreach( Control control in grdFeature.Children ) {
                if( control.Name == desiredLabelPositive ) {
                    control.Foreground = colorLabelPositive;
                    control.FontWeight = weightLabelPositive;
                } else if( control.Name == desiredLabelNegative ) {
                    control.Foreground = colorLabelNegative;
                    control.FontWeight = weightLabelNegative;
                }
            }
        }
        //private void HighlightEntropies( ) {
        //}
        private static Color ChangeTransparency( Color solidColor, byte alphaChannel ) {
            byte red = solidColor.R;
            byte green = solidColor.G;
            byte blue = solidColor.B;
            return Color.FromArgb(alphaChannel, red, green, blue);
        }
        #endregion
        #region Responding to User Events
        //private void btnCalculate_Click( object sender, RoutedEventArgs e ) {
        //   //PrintTemplateForLongerFeatures();
        //   //LoadExampleCases();
        //   Posterior posterior = new Posterior(_module);
        //   posterior.UpdatePosteriors(LoadResultCollection());
        //}
        private void cboModule_SelectionChanged( object sender, SelectionChangedEventArgs e ) {
            _module = (StudyModule)cboModule.SelectedItem;
            if( _isWindowLoaded ) ResetAfterModuleChanged();
            lstExamples.SelectedIndex = -1;
            lstExamples_SelectionChanged(lstExamples, null);
        }
        private void chkDisplayTotal_Click( object sender, RoutedEventArgs e ) {
            if( _isWindowLoaded ) DrawDiagram();
        }
        private void checkBox_Click( object sender, RoutedEventArgs e ) {
            SetCheckBoxColor((CheckBox)sender);
            UpdateDiagnosisAndTorqueLabels();
            FillComparisonGrid();
            DrawDiagram();
            UpdateEntropies();

            DataColumn dc = (DataColumn)((Control)sender).Tag;
            PotentialTorquesShow(dc);
        }
        private void lblComparison_MouseDown( object sender, MouseButtonEventArgs e ) {
            _diagnosisID1 = ((byte[])((Label)sender).Tag)[0];
            _diagnosisID2 = ((byte[])((Label)sender).Tag)[1];
            UpdateDiagnosisAndTorqueLabels();
            UpdateFeatureGrid();
            DrawDiagram();
            HighlightSelectedComparison();
        }
        private void btnResetFeatures_Click( object sender, RoutedEventArgs e ) {
            ResetGrdFeature();
            UpdateDiagnosisAndTorqueLabels();
            DrawDiagram();
            FillComparisonGrid();
        }
        private void btnGrayScale_Click( object sender, RoutedEventArgs e ) {
            SetComboBoxColor(cboBrushPalette, _brushPaletteDefault);
            SetComboBoxColor(cboBrushPositiveForeground, new SolidColorBrush(Colors.Black));
            SetComboBoxColor(cboBrushPositiveBackground, new SolidColorBrush(Colors.Silver));
            SetComboBoxColor(cboBrushNegativeForeground, new SolidColorBrush(Colors.Black));
            SetComboBoxColor(cboBrushNegativeBackground, new SolidColorBrush(Colors.Silver));
            SetComboBoxColor(cboBrushHighlight, new SolidColorBrush(Colors.Black));
            sldAlphaHighlight.Value = _alphaHighlight;
            sldAlphaPotential.Value = _alphaPotential;

            if(  _isWindowLoaded ) {
                DrawDiagram();
                UpdateFeatureGrid();
                HighlightSelectedComparison();
            }
        }
        private void lblTorquePositiveOrNegative_MouseEnter( object sender, MouseEventArgs e ) {
            DataColumn dc = (DataColumn)((Control)sender).Tag;
            PotentialTorquesShow(dc);
            HighlightPotentialLabel(dc);
        }
        private void lblTorquePositiveOrNegative_MouseLeave( object sender, MouseEventArgs e ) {
            PotentialTorquesHide();
            DataColumn dc = (DataColumn)((Control)sender).Tag;
            ResetPotentialLabel(dc);
        }
        private void btnSaveImage_Click( object sender, RoutedEventArgs e ) {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "DxBalanceBeam";
            dlg.DefaultExt = ".bmp";
            dlg.Filter = "Windows Bitmap (.bmp)|*.bmp|Portable Network Graphics (.png)|*.png|All files (*.*)|*.*";
            string filename = "";
            if( dlg.ShowDialog() == true ) {
                filename = dlg.FileName;
            }
            if( !string.IsNullOrWhiteSpace(filename) ) SaveImage(filename);
        }
        private void btnDefaultValues_Click( object sender, RoutedEventArgs e ) {
            const bool redrawDiagram = true;
            ResetDefaultValues(redrawDiagram);
        }
        private void lstExamples_SelectionChanged( object sender, SelectionChangedEventArgs e ) {
            if( _isWindowLoaded ) {
                if( lstExamples.SelectedIndex >= 0 ) {
                    ExampleCase exampleCase = (ExampleCase)lstExamples.SelectedItem;
                    txtShortDescription.Text = "Case Title: " + exampleCase.ShortDescription;
                    txtLongDescription.Text = exampleCase.LongDescription;
                    LoadExampleCase(exampleCase);
                } else {
                    txtShortDescription.Text = "No Case Selected";
                    txtLongDescription.Text = "";
                }
            }
        }
        #endregion
        #region Color ComboBoxes & Sliders
        private void InitializeColors( ) {
            InitializeColorComboxBox(cboBrushPalette, _brushPalette);
            InitializeColorComboxBox(cboBrushPositiveForeground, _brushForegroundPositive);
            InitializeColorComboxBox(cboBrushPositiveBackground, _brushBackgroundPositive);
            InitializeColorComboxBox(cboBrushNegativeForeground, _brushForegroundNegative);
            InitializeColorComboxBox(cboBrushNegativeBackground, _brushBackgroundNegative);
            InitializeColorComboxBox(cboBrushHighlight, _brushHighlight);

        }
        private void InitializeColorComboxBox( ComboBox cbo, SolidColorBrush desiredBrush ) {
            foreach( Color color in _allPossibleColors ) {
                cbo.Items.Add(new SolidColorBrush(color));
            }
            SetComboBoxColor(cbo, desiredBrush);
        }
        private void SetComboBoxColor( ComboBox cbo, SolidColorBrush desiredBrush ) {
            byte desiredR = desiredBrush.Color.R;
            byte desiredG = desiredBrush.Color.G;
            byte desiredB = desiredBrush.Color.B;
            for( Int32 i = 0; i < cbo.Items.Count; i++ ) {
                //Color color = ((SolidColorBrush)cbo.Items[i]).Color;
                Color color = ((SolidColorBrush)cbo.Items[i]).Color;
                byte r = color.R;
                byte g = color.G;
                byte b = color.B;
                //if( ((SolidColorBrush)cbo.Items[i]).Color == desiredBrush.Color )
                if( r == desiredR && g == desiredG && b == desiredB )
                    cbo.SelectedIndex = i;
            }
        }
        private void sldArrowSize_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e ) {
            if( _isWindowLoaded ) DrawDiagram();
        }
        private void sldArrowFontSize_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e ) {
            if( _isWindowLoaded ) DrawDiagram();
        }
        private void sldComparisonFontSize_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e ) {
            if( _isWindowLoaded ) FillComparisonGrid();
        }
        private void cboBrushPalette_SelectionChanged( object sender, SelectionChangedEventArgs e ) {
            grid1.Background = (SolidColorBrush)cboBrushPalette.SelectedItem;
        }
        private void cboBrushPositiveForeground_SelectionChanged( object sender, SelectionChangedEventArgs e ) {
            _brushForegroundPositive = (SolidColorBrush)cboBrushPositiveForeground.SelectedItem;
        }
        private void cboBrushPositiveBackground_SelectionChanged( object sender, SelectionChangedEventArgs e ) {
            _brushBackgroundPositive = (SolidColorBrush)cboBrushPositiveBackground.SelectedItem;
            if( _isWindowLoaded ) DrawDiagram();
        }
        private void cboBrushNegativeForeground_SelectionChanged( object sender, SelectionChangedEventArgs e ) {
            _brushForegroundNegative = (SolidColorBrush)cboBrushNegativeForeground.SelectedItem;
        }
        private void cboBrushNegativeBackground_SelectionChanged( object sender, SelectionChangedEventArgs e ) {
            _brushBackgroundNegative = (SolidColorBrush)cboBrushNegativeBackground.SelectedItem;
            if( _isWindowLoaded ) DrawDiagram();
        }
        private void cboBrushHighlight_SelectionChanged( object sender, SelectionChangedEventArgs e ) {
            _brushHighlight = (SolidColorBrush)cboBrushHighlight.SelectedItem;
            _brushHighlight = new SolidColorBrush(ChangeTransparency(_brushHighlight.Color, Convert.ToByte(sldAlphaHighlight.Value)));
            if( _isWindowLoaded ) {
                DrawDiagram();
                HighlightSelectedComparison();
            }
        }
        private void sldAlphaHighlight_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e ) {
            _brushHighlight = new SolidColorBrush(ChangeTransparency(_brushHighlight.Color, Convert.ToByte(sldAlphaHighlight.Value)));
            if( _isWindowLoaded ) {
                DrawDiagram();
                HighlightSelectedComparison();
            }
        }
        private void sldAlphaPotential_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e ) {
            //byte alpha = Convert.ToByte(sldAlphaPotential.Value);
            //_brushBackgroundPotentialPositive = new SolidColorBrush(ChangeTransparency(_brushBackgroundPositive.Color, alpha));
            //_brushBackgroundPotentialNegative = new SolidColorBrush(ChangeTransparency(_brushBackgroundNegative.Color, alpha));
            //_brushForegroundPotential = new SolidColorBrush(ChangeTransparency(_brushForegroundPotential.Color, alpha));
            if( _isWindowLoaded ) DrawDiagram(); //IntializePotentialTorques();// 
        }
        private void sldPosteriorThreshold_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e ) {
            double roundedThreshold = Math.Round(sldPosteriorThreshold.Value, _roundingDigitsMatrixProbability);
            lblPosteriorThreshold.Content = String.Format("Posterior Threshold: {0:#.00}", roundedThreshold);

            lblPosteriorThreshold.ToolTip = String.Format("Threshold for bolding lower traingle is {0}", roundedThreshold);
            sldPosteriorThreshold.ToolTip = String.Format("Threshold for bolding lower traingle is {0}", roundedThreshold);
            HighlightSelectedComparison();
        }
        private void sldTopReductions_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e ) {
            lblTopReductions.Content = String.Format("Top Reductions Highlighted: {0}", (Int32)sldTopReductions.Value);
        }
        #endregion
        #region Development/Temporary Methods
        //private void PrintTemplateForLongerFeatures( ) {
        //   foreach( DataColumn dc in _dcEvidences ) {
        //      string line = string.Format("case \"{0}\": return \"\"; ", dc.ColumnName);
        //      Trace.WriteLine(line);
        //   }
        //}
        #endregion
        #region Example Cases
        private void LoadExampleCases( ) {
            ExampleCases cases = PrebuiltExampleCases.ExamplesCases(_module);
            //this.Resources["exampleCases"] = cases;
            //lstExamples.ItemsSource = cases;
            lstExamples.Items.Clear();
            foreach( ExampleCase exampleCase in cases ) {
                lstExamples.Items.Add(exampleCase);
            }
            lstExamples.DisplayMemberPath = "ShortDescription";
            //lstExamples.ItemTemplate
        }
        private void LoadExampleCase( ExampleCase exampleCase ) {
            //ExampleCase exampleCase = PrebuiltExampleCases.ChestPainExample1();
            _module = exampleCase.Module;
            ResetAfterModuleChanged();
            LoadExampleCaseDiagnoses(exampleCase.Diagnosis1ID, exampleCase.Diagnosis2ID);
            foreach( Result result in exampleCase ) {
                LoadExampleCaseFeature(result);
            }
            FillComparisonGrid();
            UpdateDiagnosisAndTorqueLabels();
            UpdateFeatureGrid();
            DrawDiagram();
            HighlightSelectedComparison();
        }
        private void LoadExampleCaseDiagnoses( byte diagnosis1ID, byte diagnosis2ID ) {
            _diagnosisID1 = diagnosis1ID;
            _diagnosisID2 = diagnosis2ID;
            HighlightSelectedComparison();
        }
        private void LoadExampleCaseFeature( Result result ) {
            string desiredControlName = "chk" + result.Evidence.ColumnName;
            foreach( Control control in grdFeature.Children ) {
                if( control.Name == desiredControlName ) {
                    CheckBox chk = (CheckBox)control;
                    chk.IsChecked = result.PositiveTest;
                    SetCheckBoxColor(chk);
                }
            }
        }
        #endregion


    }
}
