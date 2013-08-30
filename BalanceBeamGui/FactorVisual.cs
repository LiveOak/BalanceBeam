//using System;
//using System.Collections.Generic;
//using System.Windows;
//using System.Windows.Shapes;

//namespace BalanceBeamGui {
//   public sealed class FactorVisual : Factor {
//      public FactorVisual( double leverage, bool hanging , bool? evidenceState)
//         : base(leverage, hanging,evidenceState) {
//      }
//      public FactorVisual( Factor factor )
//         : base(factor.Leverage, factor.Hanging, factor.EvidenceState) {
//      }

//      //public Rectangle Rectangle { get; set; }
//      public Rect Rectangle { get; set; }
//      public double RectangleCenter { get; set; }

//      public String Label { get; set; }
//      public String Description { get; set; }
//   }
//}