using Genetec.Sdk.Entities;
using Genetec.Sdk.Entities.Maps;
using Genetec.Sdk.Workspace.Components.MapObjectProvider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ==========================================================================
// Copyright (C) 2017 by Genetec, Inc.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// ==========================================================================
namespace MapInteractionSample
{
    /// <summary>
    /// This is where you can build the object to display on the map.
    /// </summary>
    public class FireMapObjectProvider : MapObjectProvider, IDisposable
    {
        #region Constants

        private static ObservableCollection<EventMapObject> m_fires = new ObservableCollection<EventMapObject>();
        private static List<EventMapObject> all_Fires = new List<EventMapObject>();
        public static HashSet<string> all_Tags = new HashSet<string>();
        #endregion

        #region Fields

        private bool m_isMapGeoreferenced;
        private Guid m_mapId = Guid.Empty;

        #endregion

        #region Properties

        public override string Name => "Fire analysis map object provider";

        public override Guid UniqueId => new Guid("{BDC5D2F2-DCA0-4FA4-8BF9-4DB6CA3FD7CE}");

        #endregion

        #region Constructors

        public FireMapObjectProvider()
        {
         
            GenerateHotSpots();

        }

        #endregion

        #region Destructors and Dispose Methods

        public void Dispose()
        {
            m_fires.CollectionChanged -= OnFiresCollectionChanged;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// This is to update the map when an object is added/removed from the @event list.
        /// The Invalidate(...) is important to notify the map of the changes.
        /// </summary>
        private void OnFiresCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var addedItems = new List<MapObject>();
            var removedItems = new List<MapObject>();

            if (e.NewItems != null)
            {
                foreach (MapObject added in e.NewItems)
                {
                    addedItems.Add(added);
                }
            }

            if (e.OldItems != null)
            {
                foreach (MapObject removed in e.OldItems)
                {
                    removedItems.Add(removed);
                }
            }

            Invalidate(Guid.Empty, addedItems, removedItems, null);
        }


        private void GenerateHotSpots()
        {
            m_fires.CollectionChanged += OnFiresCollectionChanged;
            all_Tags.Add("All");
            //string json = System.IO.File.ReadAllText("C:/Users/Nikolay/Downloads/Eventbrite.json");
            string json = System.IO.File.ReadAllText("C:/Users/Nikolay/Downloads/events.json");
            JArray a = JArray.Parse(json);
            dynamic dynObj = JsonConvert.DeserializeObject(json);
            foreach (var item in dynObj)
            {
                EventMapObject newEvent = new EventMapObject();
                newEvent.Name = item.name;
                newEvent.Description = item.description;
                newEvent.Longitude = item.location.longitude;
                newEvent.Latitude = item.location.latitude;
                newEvent.StartTime = Convert.ToDateTime(item.start);
                newEvent.Date = Convert.ToDateTime(item.end);
                newEvent.Population = item.expected;
                newEvent.Logo = item.logo;
                newEvent.Tags = item.type.ToObject<List<string>>();
                if (newEvent.Tags.Count == 0)
                    newEvent.Tags.Insert(0,"None");
                for (int i = 0; i < newEvent.Tags.Count; i++)
                {
                    all_Tags.Add(newEvent.Tags[i]);
                }
                all_Fires.Add((newEvent));
            }
           
        }

        /// <summary>
        /// Simple thread to generate a @event every 5 second on a random location in montreal.
        /// </summary>
        private void OnThreadStart()
        {
          
            return;
            Random random = new Random();

            while (true)
            {
                if (m_isMapGeoreferenced && m_mapId != Guid.Empty)
                {
                    var lat = Convert.ToDouble("45." + random.Next(405052, 682052));
                    var lon = Convert.ToDouble("-73." + random.Next(486714, 981099));

                    var fire = new EventMapObject(lat, lon)
                    {
                        Description = "Fire!",
                        Date = DateTime.Now
                    };

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        m_fires.Add(fire);
                    }));
                }

                Thread.Sleep(5000);
            }
        }

        #endregion

        #region Public Methods

        public static ReadOnlyObservableCollection<EventMapObject> GetFires()
        {
            return new ReadOnlyObservableCollection<EventMapObject>(m_fires);
        }

        public static HashSet<string> GetTags()
        {
            return all_Tags;
        }

        public static ReadOnlyObservableCollection<EventMapObject> GetNEvents(TimeSpan hour, DateTime selectedDate, string tag ="All")
        {
            // Clear everything
            for (int i = m_fires.Count - 1; i >= 0; i--)
            {
                m_fires.RemoveAt(i);
            }

            //Get same dates as today
            //TimeSpan start = new TimeSpan(hour-1,30,0);
            TimeSpan start = hour.Subtract(new TimeSpan(0, 30, 0));
            //TimeSpan end = new TimeSpan(hour,30,0);
            TimeSpan end = hour.Add(new TimeSpan(0, 30, 0));

            //List<EventMapObject> newEvents = new List<EventMapObject>(all_Fires.GetRange(0,10));
            List<EventMapObject> newEvents = new List<EventMapObject>(
                all_Fires.FindAll(x => x.Date.Day == selectedDate.Day && x.Date.Month == selectedDate.Month
                && (x.Date.TimeOfDay >= start && x.Date.TimeOfDay <= end)
                  )
                );
            if (tag != "All")
            {
                newEvents = newEvents.FindAll(x => x.Tags.Contains(tag));
            }
            newEvents.Sort((x, y) => -x.Population.CompareTo(y.Population));
            //if (newEvents.Count == 0)
            //    newEvents.Add(all_Fires[0]);
            for (int i = 0; i < newEvents.Count; i++)
            {
                m_fires.Add(newEvents[i]);
            }

            return new ReadOnlyObservableCollection<EventMapObject>(m_fires);
        }

        public static List<EventMapObject> GetAllFires()
        {
            return all_Fires;
        }

        public static void RemoveFire(EventMapObject @event)
        {
            m_fires.Remove(@event);
        }


        /// <summary>
        /// This is called everytime there is a change to the map
        /// (Zoom, Pan, load, etc.)
        /// </summary>
        public override IList<MapObject> Query(MapObjectProviderContext context)
        {
            var map = Workspace.Sdk.GetEntity(context.MapId) as Map;

            if (m_mapId != context.MapId)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    m_fires.Clear();
                }));
            }

            m_mapId = context.MapId;

            // We only provide accidents for geo referenced maps
            if ((map != null) && map.IsGeoReferenced)
            {
                m_isMapGeoreferenced = true;
                var result = new List<MapObject>(m_fires);
                return result;
            }

            m_isMapGeoreferenced = false;

            return null;
        }

        #endregion
    }
}