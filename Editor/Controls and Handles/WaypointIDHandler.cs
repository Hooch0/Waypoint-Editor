using System.Collections.Generic;

namespace Hooch.Waypoint.Editor
{
    public class WaypointIDHandler
    {
        public uint CurrentID { get; private set; }

        private List<uint> _reuseIDs = new List<uint>();

        public void SetupUniqueID(List<WaypointGroup> groups)
        {
            _reuseIDs.Clear();
            List<uint> allIDs = new List<uint>();
            List<uint> missingIds = new List<uint>();

            foreach (WaypointGroup group in groups)
            {
                foreach (Waypoint waypoint in group.Waypoints)
                {
                    allIDs.Add(waypoint.ID);
                }
            }

            if (allIDs.Count > 0)
            {
                //Solution taken from
                //https://www.geeksforgeeks.org/find-all-missing-numbers-from-a-given-sorted-array/
                allIDs.Sort();

                int diff = 0;

                for (int i = 0; i < allIDs.Count; i++)
                {
                    int id = (int)allIDs[i];

                    if (id - i != diff)
                    {
                        
                        while (diff < id - i)
                        {
                            missingIds.Add((uint)(i + diff));
                            diff++;
                        }
                    }
                }

                //Current ID will always be our last used ID + 1
                CurrentID = allIDs[allIDs.Count - 1] + 1;
                _reuseIDs = missingIds;
            }
            else
            {
                CurrentID = 0;
            }
        }

        public uint GetUniqueID()
        {
            if (_reuseIDs.Count > 0)
            {
                _reuseIDs.Sort();
                uint id = _reuseIDs[0];
                
                _reuseIDs.Remove(id);
                return id;
            }

            return CurrentID++;
        }

        public void AddReuseID(uint id)
        {
            _reuseIDs.Add(id);
        }

    }
}
