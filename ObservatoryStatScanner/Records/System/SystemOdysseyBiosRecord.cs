﻿using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ObservatoryStatScanner.StatScannerSettings;

namespace ObservatoryStatScanner.Records
{
    internal class SystemOdysseyBiosRecord : SystemRecord, IRecord
    {
        private readonly Dictionary<string, int> BodyBioSignals = new();

        public SystemOdysseyBiosRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Odyssey Bio count (System)")
        { }

        public override bool Enabled => Settings.EnableOdysseySurfaceBioRecord;

        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "bios"; }
        public override Function MaxFunction { get => Function.MaxSum; }

        public override List<StatScannerGrid> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans)
        {
            if (!Enabled) return new();

            int systemBioCount = 0;
            foreach (var bodyName in BodyBioSignals.Keys)
            {
                if (bodyName.StartsWith(allBodiesFound.SystemName))
                {
                    systemBioCount += BodyBioSignals[bodyName];
                }
            }
            BodyBioSignals.Clear();

            return CheckMax(systemBioCount, allBodiesFound.Timestamp, allBodiesFound.SystemName);
        }

        public override List<StatScannerGrid> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey)
        {
            if (!Enabled || !isOdyssey) return new();

            List<Signal> bodiesWithBioSignals = bodySignals.Signals.Where(s => s.Type == Constants.FSS_BODY_SIGNAL_BIOLOGICAL).ToList();
            if (bodiesWithBioSignals.Count == 1)
            {
                Signal bioSignal = bodiesWithBioSignals.First();
                if (bioSignal.Count > 0)
                    BodyBioSignals[bodySignals.BodyName] = bioSignal.Count;
            }

            return new();
        }

        public override List<StatScannerGrid> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled) return new();

            TrackIsSystemUndiscovered(scan, currentSystem);

            // Check for atmosphere of any bodies that have bio signals.
            if (BodyBioSignals.ContainsKey(scan.BodyName)
                && (!scan.Landable
                    || scan.AtmosphereType?.Length == 0
                    || scan.Atmosphere == "None"))
            {
                BodyBioSignals.Remove(scan.BodyName);
            }

            return new();
        }
    }
}
