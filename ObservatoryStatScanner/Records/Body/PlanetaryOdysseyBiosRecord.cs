﻿using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class PlanetaryOdysseyBiosRecord : BodyRecord
    {
        private readonly Dictionary<string, int> BodyBioSignals = new();

        public PlanetaryOdysseyBiosRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Odyssey Bio count")
        { }

        public override bool Enabled => Settings.EnableOdysseySurfaceBioRecord;

        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "bios"; }

        public override List<StatScannerGrid> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || !BodyBioSignals.ContainsKey(scan.BodyName)) return new();
            int bioCount = BodyBioSignals[scan.BodyName];
            if (!scan.Landable
                || scan.AtmosphereType?.Length == 0
                || scan.Atmosphere == "None"
                || bioCount == 0)
            {
                if (BodyBioSignals.ContainsKey(scan.BodyName)) BodyBioSignals.Remove(scan.BodyName);
                return new();
            }

            BodyBioSignals.Remove(scan.BodyName);
            var results = CheckMax(bioCount, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            if (results.Count > 0)
            {
                Data.SetOrUpdateMax(scan.BodyName, bioCount);
            }
            return results;
        }

        public override List<StatScannerGrid> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey)
        {
            if (!Enabled || !isOdyssey) return new();

            var bioSignals = bodySignals.Signals.Where(s => s.Type == Constants.FSS_BODY_SIGNAL_BIOLOGICAL).ToList();
            if (bioSignals.Count() == 1)
            {
                var bioSignal = bioSignals.First();
                if (bioSignal.Count > 0)
                    BodyBioSignals[bodySignals.BodyName] = bioSignal.Count;
            }

            return new();
        }
        public override List<StatScannerGrid> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans)
        {
            BodyBioSignals.Clear();

            return new();
        }
    }
}
