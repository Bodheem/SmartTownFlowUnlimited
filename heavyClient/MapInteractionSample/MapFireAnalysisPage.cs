using Genetec.Sdk.Workspace.Pages;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

// ==========================================================================
// Copyright (C) 2017 by Genetec, Inc.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// ==========================================================================
namespace MapInteractionSample
{
    /// <summary>
    /// This is where you can create a new task in the system.
    /// It is important to inherit from Page and to use the 
    /// Page attribute with the descriptor class.
    /// The View is what you display when the task is clicked in the system.
    /// </summary>
    [Page(typeof(MapFireAnalysisPageDescriptor))]
    public class MapFireAnalysisPage : Page
    {
        #region Constants

        private readonly MapFireAnalysisView m_view = new MapFireAnalysisView();

        #endregion

        #region Constructors

        public MapFireAnalysisPage()
        {
            View = m_view;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Deserializes the data contained by the specified byte array.
        /// This is used to store the page's data if the user wants it back when reopening as a saved task
        /// </summary>
        /// <param name="data">A byte array that contains the data.</param>
        protected override void Deserialize(byte[] data)
        {
        }

        protected override void Initialize()
        {
            m_view.Initialize(Workspace);
        }

        /// <summary>
        /// Serializes the data to a byte array.
        /// This is used to store the page's data if the user wants it back when reopening as a saved task
        /// </summary>
        /// <returns>A byte array that contains the data.</returns>
        protected override byte[] Serialize()
        {
            return null;
        }

        #endregion
    }

    public class MapFireAnalysisPageDescriptor : PageDescriptor
    {
        #region Properties

        public override string Description => "City Hotspots";

        /// <summary>
        /// Gets the page's icon representation
        /// </summary>
        public override ImageSource Icon
        {
            get
            {
                var icon = new BitmapImage();
                icon.BeginInit();
                icon.UriSource = new Uri(@"pack://application:,,,/MapInteractionSample;component/Resources/FireIcon.png", UriKind.RelativeOrAbsolute);
                icon.EndInit();

                return icon;
            }
        }

        /// <summary>
        /// Gets the page's default name.
        /// </summary>
        public override string Name => "Smart Town Flow: Unlimited";

        /// <summary>
        /// Page's thumbnail (low res icon)
        /// </summary>
        public override ImageSource Thumbnail
        {
            get
            {
                var icon = new BitmapImage();
                icon.BeginInit();
                icon.UriSource = new Uri(@"pack://application:,,,/MapInteractionSample;component/Resources/FireIcon.png", UriKind.RelativeOrAbsolute);
                icon.EndInit();

                return icon;
            }
        }

        /// <summary>
        /// Gets the page's unique ID.
        /// </summary>
        public override Guid Type => new Guid("{B51265C9-8B27-4C9D-A225-A37F649F4175}");

        #endregion
    }
}

