﻿using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using ObservatoryStatScanner.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class RegionRecord : IRecord
    {
        protected readonly StatScannerSettings Settings;
        protected readonly IRecordData Data;
        protected List<LogMonitorState> _disallowedStates = new();

        public RegionRecord(
            StatScannerSettings settings,
            RecordKind recordKind,
            IRecordData data,
            string displayName)
        {
            Settings = settings;
            RecordKind = recordKind;
            Data = data;
            DisplayName = displayName;
        }
        public virtual bool Enabled => false;

        public RecordTable Table => Data.Table;
        public RecordKind RecordKind { get; }
        public List<LogMonitorState> DisallowedLogMonitorStates => _disallowedStates;

        public string VariableName => Data.Variable;

        public string DisplayName { get; }

        public string EDAstroObjectName => Data.EDAstroObjectName;
        public string JournalObjectName => Data.JournalObjectName;
        public virtual string ValueFormat { get => "{0}"; }
        public virtual string Units { get => ""; }

        public bool HasMax => Data.HasMax;
        public string MaxHolder => Data.MaxHolder;
        public long MaxCount => Data.MaxCount;
        public double MaxValue => Data.MaxValue;
        public virtual Function MaxFunction { get => Function.Count; }

        public bool HasMin => Data.HasMin;
        public string MinHolder => Data.MinHolder;
        public long MinCount => Data.MinCount;
        public double MinValue => Data.MinValue;
        public virtual Function MinFunction { get => Function.Minimum; }

        public virtual List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans)
        {
            return new();
        }

        public virtual List<Result> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey)
        {
            return new();
        }

        public virtual List<Result> CheckScan(Scan scan, string currentSystem)
        {
            return new();
        }

        public virtual List<Result> CheckCodexEntry(CodexEntry codexEntry)
        {
            return new();
        }
        public List<Result> Summary()
        {
            var results = new List<Result>();

            if (HasMax)
            {
                results.Add(new(
                    NotificationClass.None,
                    new()
                        {
                            Timestamp = "Summary",
                            ObjectClass = EDAstroObjectName,
                            Variable = DisplayName,
                            Function = MaxFunction.ToString(),
                            RecordValue = String.Format(ValueFormat, MaxValue),
                            Units = Units,
                            RecordHolder = MaxHolder,
                            Details = Constants.UI_CURRENT_PERSONAL_BEST,
                            RecordKind = RecordKind.ToString(),
                        }));
            }
            return results;
        }

        public void MaybeInitForPersonalBest(PersonalBestManager manager)
        {
            Data.Init(manager);
        }

        public virtual void Reset()
        {
            Data.ResetMutable();
        }

        protected List<Result> CheckMax(NotificationClass notificationClass, double observedValue, string timestamp, string objectName, string discoveryStatus = "", string extraData = "")
        {
            List<Result> results = new();

            if (!HasMax || observedValue >= MaxValue)
            {
                StatScannerGrid gridRow = new()
                {
                    Timestamp = timestamp,
                    BodyOrItem = objectName,
                    ObjectClass = EDAstroObjectName,
                    Variable = DisplayName,
                    Function = MaxFunction.ToString(),
                    ObservedValue = String.Format(ValueFormat, observedValue),
                    RecordValue = (HasMax ? String.Format(ValueFormat, MaxValue) : "-"),
                    Units = Units,
                    RecordHolder = (HasMax ? MaxHolder : ""),
                    Details = discoveryStatus,
                    DiscoveryStatus = "-",
                    RecordKind = RecordKind.ToString(),
                };
                results.Add(new Result(notificationClass, gridRow));

                // Update the record *AFTER* generating the GridRow to ensure we have access to the previous value.
                // When there's a tie, this increments the count only.
                Data.SetOrUpdateMax(objectName, observedValue, 1, extraData);
            }
            return results;
        }
    }
}
