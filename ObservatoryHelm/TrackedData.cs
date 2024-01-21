﻿using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryHelm
{
    class TrackedData
    {
        private NavRouteFile _lastNavRoute = null;

        public TrackedData()
        {
            Scans = new();
        }

        public bool IsOdyssey { get; set; }
        public string Commander { get; set; }
        public double FuelCapacity { get; set; }
        public double FuelRemaining { get; set; }
        public double MaxFuelPerJump { get; set; }
        public string? CurrentSystem { get; set; }
        public double DistanceTravelled { get; set; }
        public long JumpsCompleted { get; set; }
        public long DockedCarrierJumpsCompleted { get; set; }
        public StartJump LastStartJumpEvent { get; set; }
        public FSDJump LastJumpEvent { get; set; }
        public NavRouteFile LastNavRoute
        {
            get => _lastNavRoute;
            set {
                _lastNavRoute = value;
                Destination = "";
                JumpsRemainingInRoute = 0;
                if (_lastNavRoute != null)
                {
                    Destination = value.Route[value.Route.Count - 1].StarSystem;
                    JumpsRemainingInRoute = value.Route.Count;
                }
            }
        }
        public string Destination {  get; set; }
        public int JumpsRemainingInRoute { get; set; }
        public string? NeutronPrimarySystem { get; set; }
        public Scan? ScoopableSecondaryCandidateScan { get; set; }
        public string? NeutronPrimarySystemNotified { get; set; }
        public string? FuelWarningNotifiedSystem { get; set; }
        public bool IsDockedOnCarrier { get; set; }
        public Dictionary<string, Scan> Scans { get; private set; }
        public bool AllBodiesFound { get; set; }
        public bool UndiscoveredSystem { get; set; }

        public void SystemReset(string systemName, double fuelLevel, double jumpDist)
        {
            Scans.Clear();
            CurrentSystem = systemName;
            FuelRemaining = fuelLevel;
            DistanceTravelled += jumpDist;
            JumpsCompleted++;
            AllBodiesFound = false;
            UndiscoveredSystem = false;
        }

        public void SystemResetDockedOnCarrier(string systemName, double distanceTravelled = 0)
        {
            Scans.Clear();
            CurrentSystem = systemName;
            IsDockedOnCarrier = true;
            DockedCarrierJumpsCompleted++;
            DistanceTravelled += distanceTravelled;
            AllBodiesFound = false;
            UndiscoveredSystem = false;
        }

        public void SessionReset(bool isOdyssey, string commander, double fuelCapacity, double fuelLevel)
        {
            IsOdyssey = isOdyssey;
            Commander = commander;
            FuelCapacity = fuelCapacity;
            FuelRemaining = fuelLevel;
            JumpsRemainingInRoute = 0;
            DistanceTravelled = 0;
            JumpsCompleted = 0;
            DockedCarrierJumpsCompleted = 0;
            LastNavRoute = null;
        }
    }
}
