using System.Linq;
using System.Windows;

// ==========================================================================
// Copyright (C) 2017 by Genetec, Inc.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// ==========================================================================
namespace MapInteractionSample
{
    /// <summary>
    /// This class handle the display of the FireMapObject on the map.
    /// The view must inherit from MapObjectView.
    /// You can display what you want in the xaml at this point.
    /// </summary>
    public partial class FireMapObjectView
    {
        #region Constants

        public static readonly DependencyProperty IsFireSelectedProperty = DependencyProperty.Register(
                    "IsFireSelected", typeof(bool), typeof(FireMapObjectView), new PropertyMetadata(default(bool)));

        #endregion

        #region Properties

        public bool IsFireSelected
        {
            get { return (bool)GetValue(IsFireSelectedProperty); }
            set { SetValue(IsFireSelectedProperty, value); }
        }

        #endregion

        #region Constructors

        public FireMapObjectView()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void OnFireImageMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
        }

        #endregion

        private void Image_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IsFireSelected = true;
            textBlock.Visibility = IsFireSelected ? Visibility.Visible : Visibility.Hidden;
            if (textBlock.Visibility == Visibility.Visible)
            {
                string tags = ((FireMapObject) MapObject).Tags.Aggregate((i, j) => i + "," + j);
                textBlock.Text =
                    "Name " + ((FireMapObject)MapObject).Name +
                    "\nStart Time : " + ((FireMapObject)MapObject).StartTime +
                    "\nEnd Time : " + ((FireMapObject)MapObject).Date +
                    "\nPopulation " + ((FireMapObject)MapObject).Population.ToString() +
                    "\nTags : " + tags;
            }
        }

        private void Image_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IsFireSelected = false;
            textBlock.Visibility = IsFireSelected ? Visibility.Visible : Visibility.Hidden;

        }
    }
}

