using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace InsuranceSystemDemo.Views;

//
// Summary:
//     Implements the animation where text scrolls from left to right and reappears from the left after
//         exiting the right border in the «OnLoaded» and «StartScrollingTextAnimation» methods.
public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    #region Text Scrolling Animation
    private void OnLoaded(object sender, RoutedEventArgs e) =>
        StartScrollingTextAnimation();

    private void StartScrollingTextAnimation()
    {
        ScrollingText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        ScrollingText.Arrange(new Rect(0, 0, ScrollingText.DesiredSize.Width, ScrollingText.DesiredSize.Height));

        double canvasWidth = ((Canvas)ScrollingText.Parent).ActualWidth;
        double textWidth = ScrollingText.ActualWidth;

        if (canvasWidth == 0 || textWidth == 0)
            return;

        var animation = new DoubleAnimation
        {
            From = -textWidth,
            To = canvasWidth,
            Duration = new Duration(TimeSpan.FromSeconds(10)),
            RepeatBehavior = RepeatBehavior.Forever
        };

        ScrollingText.BeginAnimation(Canvas.LeftProperty, animation);
    }
    #endregion
}
