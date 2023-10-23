﻿using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using ObservatoryStatScanner.DB;

namespace ObservatoryStatScanner.Records
{
    public enum RecordTable
    {
        Stars,
        Planets,
        Rings,
        Systems,
        Regions,
        Belts,
        Unknown,
    };

    enum Function
    {
        Minimum,
        Maximum,
        Sum,  // No Min -- would always be 0.
        Count, // No Min -- would always be 0.
    }

    enum Outcome
    {
        None,
        NearRecord,
        Tie,
        PotentialNew,
        PersonalNew
    }

    enum RecordKind
    {
        Galactic,
        GalacticProcGen,
        Personal,
    }

    internal interface IRecord
    {
        bool Enabled { get; }
        RecordTable Table { get; }
        RecordKind RecordKind { get; }
        List<LogMonitorState> DisallowedLogMonitorStates { get; }

        string VariableName { get; }
        string DisplayName { get; }
        string EDAstroObjectName { get; }
        string JournalObjectName { get; }
        string ValueFormat { get; }
        string Units { get; }

        bool HasMax { get; }
        string MaxHolder { get; }
        long MaxCount { get; }
        double MaxValue { get; }
        Function MaxFunction { get; }

        bool HasMin { get; }
        string MinHolder { get; }
        long MinCount { get; }
        double MinValue { get; }
        Function MinFunction { get; }

        List<Result> Summary();
        List<Result> CheckFSSAllBodiesFound(FSSAllBodiesFound allBodiesFound, List<Scan> scans);
        List<Result> CheckFSSBodySignals(FSSBodySignals bodySignals, bool isOdyssey);
        List<Result> CheckScan(Scan scan, string currentSystem);
        List<Result> CheckCodexEntry(CodexEntry codexEntry);

        void MaybeInitForPersonalBest(PersonalBestManager manager);

        void Reset();
    }
}