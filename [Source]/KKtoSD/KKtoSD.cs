using UnityEngine;
using System.Collections.Generic;
using SigmaDimensionsPlugin;
using KerbalKonstructs.Core;
using System.Linq;


namespace KKtoSDPlugin
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class KKGroups : MonoBehaviour
    {
        static Dictionary<PQSCity, LaunchSite> launchsites = new Dictionary<PQSCity, LaunchSite>();

        void Awake()
        {
            // Version Check
            Debug.Log("Sigma Version Check:   KKtoSD v0.1.2");

            // Check if KK is installed
            if (AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == "KerbalKonstructs") == null) return;

            // Get the groups from KK
            StaticInstance[] KKStatics = StaticDatabase.GetAllStatics();

            // Send the groups to SD
            foreach (StaticInstance KKStatic in KKStatics)
            {
                CelestialBody planet = KKStatic?.CelestialBody;
                string group = KKStatic?.Group;
                PQSCity mod = KKStatic?.pqsCity;

                if (planet == null || string.IsNullOrEmpty(group) || group == "Ungrouped" || mod == null) continue;


                if (!PQSCityGroups.ExternalGroups.ContainsKey(planet))
                    PQSCityGroups.ExternalGroups.Add(planet, new Dictionary<string, List<object>>());

                if (!PQSCityGroups.ExternalGroups[planet].ContainsKey(group))
                    PQSCityGroups.ExternalGroups[planet].Add(group, new List<object>());

                if (!PQSCityGroups.ExternalGroups[planet][group].Contains(mod))
                {
                    PQSCityGroups.ExternalGroups[planet][group].Add(mod);
                }


                LaunchSite spawn = mod.GetComponent<LaunchSite>();
                if (spawn != null && !launchsites.ContainsKey(mod))
                    launchsites.Add(mod, spawn);
            }
        }

        void OnDestroy()
        {
            foreach (PQSCity mod in launchsites.Keys)
            {
                launchsites[mod].transform.localPosition = mod.repositionRadial;
            }
        }
    }
}
