using System;
using System.Collections.Generic;
using System.Data;

namespace Calculation {
	public class ExampleCases : List<ExampleCase> {
	}
	public class ExampleCase : ResultCollection {
		#region Fields
		private readonly string _shortDescription = "Short Description Not Set Yet";
		private readonly string _longDescription = "Long Description Not Set Yet";
		private readonly byte _diagnosis1ID = byte.MaxValue;
		private readonly byte _diagnosis2ID = byte.MaxValue;
		#endregion
		#region Properties
		public string ShortDescription { get { return _shortDescription; } }
		public string LongDescription { get { return _longDescription; } }
		public byte Diagnosis1ID { get { return _diagnosis1ID; } }
		public byte Diagnosis2ID { get { return _diagnosis2ID; } }
		public StudyModule Module { get { return base._module; } }
		#endregion
		#region Constructor
		public ExampleCase( ):base(new DataTable(), StudyModule.ChestPain) {
		}
		public ExampleCase( string shortDescription, string longDescription, byte diagnosis1ID, byte diagnosis2ID, DataTable dt, StudyModule module )
			: base(dt, module) {
			if( string.IsNullOrWhiteSpace(shortDescription) ) throw new ArgumentNullException("shortDescription");
			if( string.IsNullOrWhiteSpace(longDescription) ) throw new ArgumentNullException("longDescription");
			_shortDescription = shortDescription;
			_longDescription = longDescription;
			_diagnosis1ID = diagnosis1ID;
			_diagnosis2ID = diagnosis2ID;
		}
		#endregion
		#region Public Methods
		public override string ToString( ) {
			return ShortDescription;
		}
		#endregion
		#region Private Methods
		#endregion

	}
}