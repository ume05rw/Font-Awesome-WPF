using System;
using System.Collections.Generic;
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
    public class FontAwesome : FontIcon
    {
        private const string FontUrl = "ms-appx:///FontAwesome.UWP/FontAwesome.otf#FontAwesome";
        private static readonly Dictionary<int, FontFamily> FontDictionary
            = new Dictionary<int, FontFamily>();

        private static FontFamily GetFontFamily()
        {
            if (!FontAwesome.FontDictionary.ContainsKey(Environment.CurrentManagedThreadId))
                FontAwesome.FontDictionary.Add(
                    Environment.CurrentManagedThreadId,
                    new FontFamily(FontAwesome.FontUrl)
                );

            return FontAwesome.FontDictionary[Environment.CurrentManagedThreadId];
        }

        /// <summary>
        /// IconProperty
        /// </summary>
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

            _ = fontAwesome.RunUI(() =>
            {
                fontAwesome.SetValue(FontFamilyProperty, FontAwesome.GetFontFamily());
                fontAwesome.SetValue(GlyphProperty, char.ConvertFromUtf32((int)fontToSet));
            });
        }

        private readonly int _uiThreadId;

        /// <summary>
        /// Gets or sets the FontAwesome icon
        /// </summary>
        public FontAwesomeIcon Icon
        {
            get => (FontAwesomeIcon)this.GetValue(FontAwesome.IconProperty);
            set => _ = this.RunUI(() =>
            {
                this.SetValue(FontAwesome.IconProperty, value);
            });
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public FontAwesome()
        {
            this._uiThreadId = Environment.CurrentManagedThreadId;
            this.FontFamily = FontAwesome.GetFontFamily();
        }

        /// <summary>
        /// UIスレッドでActionを実行する。
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private async Task RunUI(Action action)
        {
            if (this._uiThreadId == Environment.CurrentManagedThreadId)
            {
                action();
            }
            else
            {
                await this.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    new DispatchedHandler(action)
                );
            }
        }
    }
}
