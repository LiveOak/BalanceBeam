//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace BalanceBeamGui {
//   public class Factor {
//      #region Fields
//      private readonly bool _hanging = true;
//      private readonly double _leverage = double.NaN;
//      private readonly bool? _evidenceState = null;//EvidenceState.NotInitialized;
//      #endregion
//      #region Properties
//      public bool? EvidenceState { get { return _evidenceState; } }
//      public bool Hanging { get { return _hanging; } }
//      public double Leverage { get { return _leverage; } }
//      #endregion
//      #region Constructor
//      public Factor( double leverage, bool hanging, bool? evidenceState ) {
//         if( !(double.NegativeInfinity < leverage && leverage < double.PositiveInfinity) ) throw new ArgumentOutOfRangeException("leverage", leverage, "Leverage should be a real number.");
//         _leverage = leverage;
//         _hanging = hanging;
//         _evidenceState = evidenceState;
//      }
//      #endregion
//      #region Public Methods
//      #endregion
//      #region Private Methods
//      #endregion
//   }
//}