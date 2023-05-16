using Android.Locations;
using System;
using System.Collections.Generic;

namespace SensorLab {
	public class GnssStatusReceiver : GnssStatus.Callback {
		internal Dictionary<SatelliteIdentifier, SatelliteStatus> ActiveSatellites
			= new Dictionary<SatelliteIdentifier, SatelliteStatus>();
		public event Action SatelliteStatusChanged;
		public GnssStatusReceiver() { }
		public override void OnStarted() {
			base.OnStarted();
		}
		public override void OnStopped() {
			base.OnStopped();
		}
		public override void OnFirstFix(int ttffMillis) {
			base.OnFirstFix(ttffMillis);
		}
		public override void OnSatelliteStatusChanged(GnssStatus status) {
			base.OnSatelliteStatusChanged(status);
			lock (ActiveSatellites) {
				ActiveSatellites.Clear();
				for (int i = 0; i < status.SatelliteCount; i++) {
					var id = new SatelliteIdentifier {
						Constellation = status.GetConstellationType(i),
						SVID = status.GetSvid(i),
					};
					if (ActiveSatellites.TryGetValue(id, out var ex) && ex.IsUsedInFix) continue;
					ActiveSatellites[id] = new SatelliteStatus {
						Azimuth = status.GetAzimuthDegrees(i),
						Elevation = status.GetElevationDegrees(i),
						IsUsedInFix = status.UsedInFix(i),
						CNR = status.GetCn0DbHz(i),
						BasebandCNR = status.HasBasebandCn0DbHz(i) ? status.GetBasebandCn0DbHz(i) : (float?)null,
						CarrierFrequency = status.HasCarrierFrequencyHz(i) ? status.GetCarrierFrequencyHz(i) : (float?)null,
						HasAlmanacData = status.HasAlmanacData(i),
						HasEphemerisData = status.HasEphemerisData(i),
					};
				}
			}
			SatelliteStatusChanged?.Invoke();
		}
	}
	internal struct SatelliteIdentifier {
		public GnssConstellationType Constellation { get; set; }
		public int SVID { get; set; }
	}
	internal struct SatelliteStatus {
		public float Azimuth { get; set; }
		public float Elevation { get; set; }
		public bool IsUsedInFix { get; set; }
		public float CNR { get; set; }
		public float? BasebandCNR { get; set; }
		public float? CarrierFrequency { get; set; }
		public bool HasAlmanacData { get; set; }
		public bool HasEphemerisData { get; set; }
	}
	internal struct SatelliteStatusEvent {
		public SatelliteIdentifier Identifier { get; set; }
		public SatelliteStatus Status { get; set; }
	}
}