﻿//******************************************************************************************************
//  MainWindow.xaml.cs - Gbtc
//
//  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/07/2010 - Stephen C. Wills
//       Generated original version of source code.
//  03/07/2011 - J. Ritchie Carroll
//       Added pre-shutdown validation step.
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using ConfigurationSetupUtility.Screens;
using GSF.Collections;
using GSF.ErrorManagement;
using GSF.Identity;
using GSF.IO;
using GSF.Reflection;

namespace ConfigurationSetupUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region [ Members ]

        // Fields
        private ScreenManager m_screenManager;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Setup screen manager
            m_screenManager = new ScreenManager(this, new WelcomeScreen());

            Loaded += MainWindow_Loaded;
        }

        #endregion

        #region [ Methods ]

        // Make sure window is focused
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Activate();
        }

        // Occurs when the user clicks the "Next" button.
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_screenManager.CurrentScreen.NextScreen != null)
            {
                // Next screen is defined, navigate to it...
                m_screenManager.GoToNextScreen();
            }
            else
            {
                // Attempt pre-shutdown operations
                if (m_screenManager.PerformShutdownOperations())
                    this.Close();
            }
        }

        // Occurs when the user clicks the "Back" button.
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            m_screenManager.GoToPreviousScreen();
        }

        // Occurs when the user clicks the "Cancel" button.
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Occurs when an attempt is made to close the window.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_screenManager.CurrentScreen.NextScreen != null)
            {
                if (!m_screenManager.CurrentScreen.CanCancel)
                    e.Cancel = true;
                else
                {
                    MessageBoxResult result = MessageBox.Show("The setup is not yet complete. Are you sure you want to exit?", this.Title, MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                        e.Cancel = true;
                }
            }
        }

        #endregion
    }
}
