using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FontAwesome.UWP
{
    /// <summary>
    /// Represents ann icon that uses the FontAwesome font
    /// </summary>
    public class FontAwesome
        : FontIcon
    {
        private static readonly FontFamily FontAwesomeFontFamily = new FontFamily("ms-appx:///FontAwesome.UWP/FontAwesome.otf#FontAwesome");

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(FontAwesome.Icon),
                typeof(FontAwesomeIcon),
                typeof(FontAwesome),
                new PropertyMetadata(
                    FontAwesomeIcon.None,
                    IconPropertyChangedCallback
                )
            );

        private static void IconPropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs
        )
        {
            var fontAwesome = (FontAwesome)dependencyObject;

            var fontToSet = FontAwesomeIcon.None;

            if (dependencyPropertyChangedEventArgs.NewValue != null)
                fontToSet = (FontAwesomeIcon)dependencyPropertyChangedEventArgs.NewValue;

            _ = fontAwesome.SetValueOnUI(FontFamilyProperty, FontAwesomeFontFamily);
            _ = fontAwesome.SetValueOnUI(GlyphProperty, char.ConvertFromUtf32((int)fontToSet));
        }

        private readonly int _uiThreadId;

        public FontAwesome()
        {
            this._uiThreadId = Environment.CurrentManagedThreadId;
            this.FontFamily = FontAwesome.FontAwesomeFontFamily;
        }

        public async Task SetValueOnUI(DependencyProperty dp, object value)
        {
            if (this._uiThreadId == Environment.CurrentManagedThreadId)
            {
                this.SetValue(dp, value);
            }
            else
            {
                await this.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    new DispatchedHandler(() =>
                    {
                        this.SetValue(dp, value);
                    })
                );
            }
        }

        /// <summary>
        /// Gets or sets the FontAwesome icon
        /// </summary>
        public FontAwesomeIcon Icon
        {
            get => (FontAwesomeIcon)this.GetValue(FontAwesome.IconProperty);
            set => _ = this.SetValueOnUI(FontAwesome.IconProperty, value);
        }
    }
}
