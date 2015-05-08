using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.UI.StartScreen;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using System.Diagnostics;

namespace TileUpdateAfterDeactivation
{
    public partial class Sceanrio1_Inline : PhoneApplicationPage
    {
        public Sceanrio1_Inline()
        {
            InitializeComponent();
        }

        const string SCENARIO1_TILEID = "Scenario1.Tile";

        private async void Clean_Click(object sender, RoutedEventArgs e)
        {
            // Find all secondary tiles
            var secondaryTiles = await SecondaryTile.FindAllAsync();
            foreach (var secondaryTile in secondaryTiles)
            {
                // We'll be good citizens and only remove the secondary tile belonging
                // to this scenario. To remove all secondary tiles, remove this check.
                if (secondaryTile.TileId == SCENARIO1_TILEID)
                {
                    // Delete the secondary tile.
                    // Note: On Windows Phone, the call to RequestDeleteAsync deletes the tile without prompting the user
                    await secondaryTile.RequestDeleteAsync();
                }
            }
        }

        private async void PinAndUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Create the original Square150x150 tile
            var tile = new SecondaryTile(SCENARIO1_TILEID, "Scenario 1", "/MainPage.xaml?scenario=Scenario1", new Uri("ms-appx:///Assets/originalTileImage.png"), TileSize.Default);
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            await tile.RequestCreateAsync();

            // When a new tile is created, the app will be deactivated and the new tile will be displayed to the user on the start screen.
            // Any code after the call to RequestCreateAsync is not guaranteed to run. 
            // For example, a common scenario is to associate a push channel with the newly created tile,
            // which involves a call to WNS to get a channel using the CreatePushNotificationChannelForSecondaryTileAsync() asynchronous operation. 
            // Another example is updating the secondary tile with data from a web service. Both of these are examples of actions that may not
            // complete before the app is deactivated. To illustrate this, we'll create a delay and then attempt to update our secondary tile.

            
            // If the app is deactivated before reaching this point, the following code will never run.

            // Update the tile we created using a notification.
            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Image);

            // The TileSquare150x150Image template only contains one image entry, so retrieve it.
            var imageElement = tileXml.GetElementsByTagName("image").Single();

            // Set the src propertry on the image entry.
            imageElement.Attributes.GetNamedItem("src").NodeValue = "ms-appx:///Assets/updatedTileImage.png";

            // Create a new tile notification.
            var notification = new Windows.UI.Notifications.TileNotification(tileXml);

            // Create a tile updater.
            var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(SCENARIO1_TILEID);

            // Send the update notification for the tile. 
            updater.Update(notification);
        }
    }
}
