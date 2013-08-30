using System;

public enum StudyModule : byte {
	ChestPain,
	PediatricDyspnea,
}

public enum ChestPainDiagnosis : byte {
	Mi = 1,//make sure these values match the primary key value set in the Singleton's LoadData function.
	Ang = 2,
	Taa = 3,
	Peri = 4,
	Ugi = 5,
	Pneum = 6,
	Ptx = 7,
	Musc = 8,
	Pe = 9,
	MAX = 9,
}
public enum PediatricDyspnea : byte {
	Crp = 1,//make sure these values match the primary key value set in the Singleton's LoadData function.
	Bron = 2,
	Vpn = 3,
	Bpn = 4,
	Ast = 5,
	Fb = 6,
	Epig = 7,
	MAX = 7,
}
public static class EnumConversion {
	public static byte MaxDiagnosisID( StudyModule module ) {
		switch( module ) {
			case StudyModule.ChestPain: return (byte)global::ChestPainDiagnosis.MAX;
			case StudyModule.PediatricDyspnea: return (byte)global::PediatricDyspnea.MAX;
			default: throw new ArgumentOutOfRangeException("module", module, "The value of StudyModule was not recognized.");
		}
	}
	public static string DiagnosisNameShort( StudyModule module, byte diagnosisID ) {
		switch( module ) {
			case StudyModule.ChestPain: return ChestPainDiagnosis(diagnosisID).ToString();
			case StudyModule.PediatricDyspnea: return PediatricDyspnea(diagnosisID).ToString();
			default: throw new ArgumentOutOfRangeException("module", module, "The value of StudyModule was not recognized.");
		}
	}
	public static ChestPainDiagnosis ChestPainDiagnosis( byte value ) {
		switch( value ) {
			case 1: return global::ChestPainDiagnosis.Mi;
			case 2: return global::ChestPainDiagnosis.Ang;
			case 3: return global::ChestPainDiagnosis.Taa;
			case 4: return global::ChestPainDiagnosis.Peri;
			case 5: return global::ChestPainDiagnosis.Ugi;
			case 6: return global::ChestPainDiagnosis.Pneum;
			case 7: return global::ChestPainDiagnosis.Ptx;
			case 8: return global::ChestPainDiagnosis.Musc;
			case 9: return global::ChestPainDiagnosis.Pe;
			default: throw new ArgumentOutOfRangeException("value", value, "The value for ChestPainDiagnosis was not recognized.");
		}
	}
	public static PediatricDyspnea PediatricDyspnea( byte value ) {
		switch( value ) {
			case 1: return global::PediatricDyspnea.Crp;
			case 2: return global::PediatricDyspnea.Bron;
			case 3: return global::PediatricDyspnea.Vpn;
			case 4: return global::PediatricDyspnea.Bpn;
			case 5: return global::PediatricDyspnea.Ast;
			case 6: return global::PediatricDyspnea.Fb;
			case 7: return global::PediatricDyspnea.Epig;
			default: throw new ArgumentOutOfRangeException("value", value, "The value for PediatricDyspnea was not recognized.");
		}
	}
}