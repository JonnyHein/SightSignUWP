using System;
using Windows.Devices.Input.Preview;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Foundation;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using System.IO;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml.Shapes;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SightSignUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Reference to the user's eyes and head as detected
        /// by the eye-tracking device.
        /// </summary>
        private GazeInputSourcePreview gazeInputSource;

        /// <summary>
        /// Dynamic store of eye-tracking devices.
        /// </summary>
        /// <remarks>
        /// Receives event notifications when a device is added, removed, 
        /// or updated after the initial enumeration.
        /// </remarks>
        private GazeDeviceWatcherPreview gazeDeviceWatcher;

        /// <summary>
        /// Eye-tracking device counter.
        /// </summary>
        private int deviceCounter = 0;

        /// <summary>
        /// Timer for gaze focus on RadialProgressBar.
        /// </summary>
        DispatcherTimer timerGaze = new DispatcherTimer();

        /// <summary>
        /// Tracker used to prevent gaze timer restarts.
        /// </summary>
        bool timerStarted = false;

        public RobotArm RobotArm { get; }
        private readonly Settings _settings;


        /// <summary>
        /// Initialize the app.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // Set application window to full screen & get its dimensions.
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            var bounds = Window.Current.Bounds;
            double appHeight = bounds.Height;
            double appWidth = bounds.Width;

            // instantiate Robot Arm & settings
            RobotArm = new RobotArm(
                appWidth / 2.0,
                appHeight / 2.0,
                Math.Min(appWidth, appHeight) / 2.0,
                new UArmSwiftPro());

            _settings = new Settings(RobotArm);
            DataContext = _settings;

            if (_settings.RobotControl)
            {
                RobotArm.Connect();
                RobotArm.ArmDown(false); // Lift the arm.
            }

            // set the default drawing attributes
            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            SetDrawingAttributesFromSettings(drawingAttributes);
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            inkCanvas.InkPresenter.InputDeviceTypes =
                    CoreInputDeviceTypes.Mouse |
                    CoreInputDeviceTypes.Pen |
                    CoreInputDeviceTypes.Touch;

            LoadInkOnStartup();
        }

        private void SetDrawingAttributesFromSettings(InkDrawingAttributes defaultDrawingAttributes)
        {
            defaultDrawingAttributes.Color = Windows.UI.Colors.DarkOliveGreen;

            // set Stroke Size through with same width and height
            Size strokeSize;
            strokeSize.Width = strokeSize.Height = _settings.InkWidth;
            defaultDrawingAttributes.Size = strokeSize;

            defaultDrawingAttributes.PenTip = PenTipShape.Circle;
            
        }


        /// <summary>
        /// There is more function in the WPF before this one...
        /// Just integrating this in so that it can load in a signature from a file.
        /// </summary>
        

        ///<summary>
        /// Creating the Write button functionality [[Testing]]
        ///</summary>
        private void WriteButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("write ink ");
        }


        // Load up ink based on the ink that was shown when the app was last run.
        private void LoadInkOnStartup()
        {
            var filename = Settings1.Default.LoadedInkLocation;
            if (string.IsNullOrEmpty(filename))
            {
                // Look for default ink if we can find it in the same folder as the exe.
                filename = AppDomain.CurrentDomain.BaseDirectory + "Signature.isf";
            }

            if (File.Exists(filename))
            {
                AddInkFromFile(filename);
            }
        }


        /// <summary>
        /// Add ink to the InkCanvas, based on the contents of the supplied ISF file.
        /// </summary>
        private async void AddInkFromFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            // Remove any existing ink first.
            inkCanvas.InkPresenter.StrokeContainer.Clear();

            // Assume the file is valid and accessible
            /* TODO :: parse the incoming filename and replace "Signature.isf" */
           
            //Windows.Storage.StorageFile file = await 
            //    Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Signature.isf"));

            // TODO :: Unable to load ink from a file onto the inkCanvas.
            //         Have tried several ways to load a .isf file onto the canvas,
            //         I am thinking of not support .isf files and just using .gif files.
            //         GIF files are the UWP standard for loading and saving inkCanvases. It 
            //         does say that LoadAsync() is backwasrd compatible to .isf,but no luck yet.
            var file = new FileStream(filename, FileMode.Open, FileAccess.Read); // new FileStream(filename, FileMode.Open, FileAccess.Read);
            var stream = file.AsInputStream();
            await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(stream);


            //file.Close();

            
            //if (file != null)
            //{
            //    var stream = await file.OpenReadAsync();

            //    using (var inputStream = stream.GetInputStreamAt(0))
            //    {
            //        await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(inputStream);
            //    }

            //    stream.Dispose();
            //}
            //else
            //{
            //    // opeartion cancelled
            //}

            //if (strokeCollection.Count > 0)
            //{
            //    // Add ink to the InkCanvas, similar to the ink loaded from the supplied file,
            //    // but with evenly distributed points along the strokes.
            //    GenerateStrokesWithEvenlyDistributedPoints(strokeCollection);

            //    ApplySettingsToInk();
            //}
        }
        //    /// <summary>
        //    /// Override of OnNavigatedTo page event starts GazeDeviceWatcher.
        //    /// </summary>
        //    /// <param name="e">Event args for the NavigatedTo event</param>
        //    protected override void OnNavigatedTo(NavigationEventArgs e)
        //    {
        //        // Start listening for device events on navigation to eye-tracking page.
        //        StartGazeDeviceWatcher();
        //    }

        //    /// <summary>
        //    /// Override of OnNavigatedFrom page event stops GazeDeviceWatcher.
        //    /// </summary>
        //    /// <param name="e">Event args for the NavigatedFrom event</param>
        //    protected override void OnNavigatedFrom(NavigationEventArgs e)
        //    {
        //        // Stop listening for device events on navigation from eye-tracking page.
        //        StopGazeDeviceWatcher();
        //    }

        //    //********** STEP 3: https://docs.microsoft.com/en-us/windows/uwp/design/input/gaze-interactions?utm_source=developer.tobii.com *********

        //    /// <summary>
        //    /// Start gaze watcher and declare watcher event handlers.
        //    /// </summary>
        //    private void StartGazeDeviceWatcher()
        //    {
        //        if (gazeDeviceWatcher == null)
        //        {
        //            gazeDeviceWatcher = GazeInputSourcePreview.CreateWatcher();
        //            gazeDeviceWatcher.Added += this.DeviceAdded;
        //            gazeDeviceWatcher.Updated += this.DeviceUpdated;
        //            gazeDeviceWatcher.Removed += this.DeviceRemoved;
        //            gazeDeviceWatcher.Start();
        //        }
        //    }

        //    /// <summary>
        //    /// Shut down gaze watcher and stop listening for events.
        //    /// </summary>
        //    private void StopGazeDeviceWatcher()
        //    {
        //        if (gazeDeviceWatcher != null)
        //        {
        //            gazeDeviceWatcher.Stop();
        //            gazeDeviceWatcher.Added -= this.DeviceAdded;
        //            gazeDeviceWatcher.Updated -= this.DeviceUpdated;
        //            gazeDeviceWatcher.Removed -= this.DeviceRemoved;
        //            gazeDeviceWatcher = null;
        //        }
        //    }

        //    /// <summary>
        //    /// Eye-tracking device connected (added, or available when watcher is initialized).
        //    /// </summary>
        //    /// <param name="sender">Source of the device added event</param>
        //    /// <param name="e">Event args for the device added event</param>
        //    private void DeviceAdded(GazeDeviceWatcherPreview source,
        //        GazeDeviceWatcherAddedPreviewEventArgs args)
        //    {
        //        if (IsSupportedDevice(args.Device))
        //        {
        //            deviceCounter++;
        //            TrackerCounter.Text = deviceCounter.ToString();
        //        }
        //        // Set up gaze tracking.
        //        TryEnableGazeTrackingAsync(args.Device);
        //    }

        //    /// <summary>
        //    /// Initial device state might be uncalibrated, 
        //    /// but device was subsequently calibrated.
        //    /// </summary>
        //    /// <param name="sender">Source of the device updated event</param>
        //    /// <param name="e">Event args for the device updated event</param>
        //    private void DeviceUpdated(GazeDeviceWatcherPreview source,
        //        GazeDeviceWatcherUpdatedPreviewEventArgs args)
        //    {
        //        // Set up gaze tracking.
        //        TryEnableGazeTrackingAsync(args.Device);
        //    }

        //    /// <summary>
        //    /// Handles disconnection of eye-tracking devices.
        //    /// </summary>
        //    /// <param name="sender">Source of the device removed event</param>
        //    /// <param name="e">Event args for the device removed event</param>
        //    private void DeviceRemoved(GazeDeviceWatcherPreview source,
        //        GazeDeviceWatcherRemovedPreviewEventArgs args)
        //    {
        //        // Decrement gaze device counter and remove event handlers.
        //        if (IsSupportedDevice(args.Device))
        //        {
        //            deviceCounter--;
        //            TrackerCounter.Text = deviceCounter.ToString();

        //            if (deviceCounter == 0)
        //            {
        //                gazeInputSource.GazeEntered -= this.GazeEntered;
        //                gazeInputSource.GazeMoved -= this.GazeMoved;
        //                gazeInputSource.GazeExited -= this.GazeExited;
        //            }
        //        }
        //    }

        //    //********** STEP 4: https://docs.microsoft.com/en-us/windows/uwp/design/input/gaze-interactions?utm_source=developer.tobii.com *********
        //    /// <summary>
        //    /// Initialize gaze tracking.
        //    /// </summary>
        //    /// <param name="gazeDevice"></param>
        //    private async void TryEnableGazeTrackingAsync(GazeDevicePreview gazeDevice)
        //    {
        //        // If eye-tracking device is ready, declare event handlers and start tracking.
        //        if (IsSupportedDevice(gazeDevice))
        //        {
        //            timerGaze.Interval = new TimeSpan(0, 0, 0, 0, 20);
        //            timerGaze.Tick += TimerGaze_Tick;

        //            SetGazeTargetLocation();

        //            // This must be called from the UI thread.
        //            gazeInputSource = GazeInputSourcePreview.GetForCurrentView();

        //            gazeInputSource.GazeEntered += GazeEntered;
        //            gazeInputSource.GazeMoved += GazeMoved;
        //            gazeInputSource.GazeExited += GazeExited;
        //        }
        //        // Notify if device calibration required.
        //        else if (gazeDevice.ConfigurationState ==
        //                    GazeDeviceConfigurationStatePreview.UserCalibrationNeeded ||
        //                    gazeDevice.ConfigurationState ==
        //                    GazeDeviceConfigurationStatePreview.ScreenSetupNeeded)
        //        {
        //            // Device isn't calibrated, so invoke the calibration handler.
        //            System.Diagnostics.Debug.WriteLine(
        //                "Your device needs to calibrate. Please wait for it to finish.");
        //            await gazeDevice.RequestCalibrationAsync();
        //        }
        //        // Notify if device calibration underway.
        //        else if (gazeDevice.ConfigurationState ==
        //            GazeDeviceConfigurationStatePreview.Configuring)
        //        {
        //            // Device is currently undergoing calibration.  
        //            // A device update is sent when calibration complete.
        //            System.Diagnostics.Debug.WriteLine(
        //                "Your device is being configured. Please wait for it to finish");
        //        }
        //        // Device is not viable.
        //        else if (gazeDevice.ConfigurationState == GazeDeviceConfigurationStatePreview.Unknown)
        //        {
        //            // Notify if device is in unknown state.  
        //            // Reconfigure/recalbirate the device.  
        //            System.Diagnostics.Debug.WriteLine(
        //                "Your device is not ready. Please set up your device or reconfigure it.");
        //        }
        //    }

        //    /// <summary>
        //    /// Check if eye-tracking device is viable.
        //    /// </summary>
        //    /// <param name="gazeDevice">Reference to eye-tracking device.</param>
        //    /// <returns>True, if device is viable; otherwise, false.</returns>
        //    private bool IsSupportedDevice(GazeDevicePreview gazeDevice)
        //    {
        //        TrackerState.Text = gazeDevice.ConfigurationState.ToString();
        //        return (gazeDevice.CanTrackEyes &&
        //                    gazeDevice.ConfigurationState ==
        //                    GazeDeviceConfigurationStatePreview.Ready);
        //    }

        //    //********** STEP 5: https://docs.microsoft.com/en-us/windows/uwp/design/input/gaze-interactions?utm_source=developer.tobii.com *********
        //    /// <summary>
        //    /// GazeEntered handler.
        //    /// </summary>
        //    /// <param name="sender">Source of the gaze entered event</param>
        //    /// <param name="e">Event args for the gaze entered event</param>
        //    private void GazeEntered(
        //        GazeInputSourcePreview sender,
        //        GazeEnteredPreviewEventArgs args)
        //    {
        //        // Show ellipse representing gaze point.
        //        eyeGazePositionEllipse.Visibility = Visibility.Visible;

        //        // Mark the event handled.
        //        args.Handled = true;
        //    }

        //    /// <summary>
        //    /// GazeExited handler.
        //    /// Call DisplayRequest.RequestRelease to conclude the 
        //    /// RequestActive called in GazeEntered.
        //    /// </summary>
        //    /// <param name="sender">Source of the gaze exited event</param>
        //    /// <param name="e">Event args for the gaze exited event</param>
        //    private void GazeExited(
        //        GazeInputSourcePreview sender,
        //        GazeExitedPreviewEventArgs args)
        //    {
        //        // Hide gaze tracking ellipse.
        //        eyeGazePositionEllipse.Visibility = Visibility.Collapsed;

        //        // Mark the event handled.
        //        args.Handled = true;
        //    }

        //    /// <summary>
        //    /// GazeMoved handler translates the ellipse on the canvas to reflect gaze point.
        //    /// </summary>
        //    /// <param name="sender">Source of the gaze moved event</param>
        //    /// <param name="e">Event args for the gaze moved event</param>
        //    private void GazeMoved(GazeInputSourcePreview sender, GazeMovedPreviewEventArgs args)
        //    {
        //        // Update the position of the ellipse corresponding to gaze point.
        //        if (args.CurrentPoint.EyeGazePosition != null)
        //        {
        //            double gazePointX = args.CurrentPoint.EyeGazePosition.Value.X;
        //            double gazePointY = args.CurrentPoint.EyeGazePosition.Value.Y;

        //            double ellipseLeft =
        //                gazePointX -
        //                (eyeGazePositionEllipse.Width / 2.0f);
        //            double ellipseTop =
        //                gazePointY -
        //                (eyeGazePositionEllipse.Height / 2.0f) -
        //                (int)Header.ActualHeight;

        //            // Translate transform for moving gaze ellipse.
        //            TranslateTransform translateEllipse = new TranslateTransform
        //            {
        //                X = ellipseLeft,
        //                Y = ellipseTop
        //            };

        //            eyeGazePositionEllipse.RenderTransform = translateEllipse;

        //            // The gaze point screen location.
        //            Point gazePoint = new Point(gazePointX, gazePointY);

        //            // Basic hit test to determine if gaze point is on progress bar.
        //            bool hitRadialProgressBar =
        //                DoesElementContainPoint(
        //                    gazePoint,
        //                    GazeRadialProgressBar.Name,
        //                    GazeRadialProgressBar);

        //            // Use progress bar thickness for visual feedback.
        //            if (hitRadialProgressBar)
        //            {
        //                GazeRadialProgressBar.Thickness = 10;
        //            }
        //            else
        //            {
        //                GazeRadialProgressBar.Thickness = 4;
        //            }

        //            // Mark the event handled.
        //            args.Handled = true;
        //        }
        //    }

        //    //********** STEP 6: https://docs.microsoft.com/en-us/windows/uwp/design/input/gaze-interactions?utm_source=developer.tobii.com
        //    /// <summary>
        //    /// Return whether the gaze point is over the progress bar.
        //    /// </summary>
        //    /// <param name="gazePoint">The gaze point screen location</param>
        //    /// <param name="elementName">The progress bar name</param>
        //    /// <param name="uiElement">The progress bar UI element</param>
        //    /// <returns></returns>
        //    private bool DoesElementContainPoint(
        //        Point gazePoint, string elementName, UIElement uiElement)
        //    {
        //        // Use entire visual tree of progress bar.
        //        IEnumerable<UIElement> elementStack =
        //            VisualTreeHelper.FindElementsInHostCoordinates(gazePoint, uiElement, true);
        //        foreach (UIElement item in elementStack)
        //        {
        //            //Cast to FrameworkElement and get element name.
        //            if (item is FrameworkElement feItem)
        //            {
        //                if (feItem.Name.Equals(elementName))
        //                {
        //                    if (!timerStarted)
        //                    {
        //                        // Start gaze timer if gaze over element.
        //                        timerGaze.Start();
        //                        timerStarted = true;
        //                    }
        //                    return true;
        //                }
        //            }
        //        }

        //        // Stop gaze timer and reset progress bar if gaze leaves element.
        //        timerGaze.Stop();
        //        GazeRadialProgressBar.Value = 0;
        //        timerStarted = false;
        //        return false;
        //    }

        //    /// <summary>
        //    /// Tick handler for gaze focus timer.
        //    /// </summary>
        //    /// <param name="sender">Source of the gaze entered event</param>
        //    /// <param name="e">Event args for the gaze entered event</param>
        //    private void TimerGaze_Tick(object sender, object e)
        //    {
        //        // Increment progress bar.
        //        GazeRadialProgressBar.Value += 1;

        //        // If progress bar reaches maximum value, reset and relocate.
        //        if (GazeRadialProgressBar.Value == 100)
        //        {
        //            SetGazeTargetLocation();
        //        }
        //    }

        //    /// <summary>
        //    /// Set/reset the screen location of the progress bar.
        //    /// </summary>
        //    private void SetGazeTargetLocation()
        //    {
        //        // Ensure the gaze timer restarts on new progress bar location.
        //        timerGaze.Stop();
        //        timerStarted = false;

        //        // Get the bounding rectangle of the app window.
        //        Rect appBounds = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds;

        //        // Translate transform for moving progress bar.
        //        TranslateTransform translateTarget = new TranslateTransform();

        //        // Calculate random location within gaze canvas.
        //        Random random = new Random();
        //        int randomX =
        //            random.Next(
        //                0,
        //                (int)appBounds.Width - (int)GazeRadialProgressBar.Width);
        //        int randomY =
        //            random.Next(
        //                0,
        //                (int)appBounds.Height - (int)GazeRadialProgressBar.Height - (int)Header.ActualHeight);

        //        translateTarget.X = randomX;
        //        translateTarget.Y = randomY;

        //        GazeRadialProgressBar.RenderTransform = translateTarget;

        //        // Show progress bar.
        //        GazeRadialProgressBar.Visibility = Visibility.Visible;
        //        GazeRadialProgressBar.Value = 0;
        //    }
    }

}