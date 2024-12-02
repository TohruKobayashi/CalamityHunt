using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Reflection.Metadata;
using System.Text;
using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace CalamityHunt.Common.UI
{
    public class DialogueBoxCanvas : UIState
    {
        private UIPanel container;
        private DialogueBoxButton nameplate;
        private UIPanel infobox;
        private UIImage lordOfWeedsPortrait;
        private UIPanel panel;
        private DialogueBoxButton button1;
        private DialogueBoxButton button2;
        private DialogueBoxButton button3;

        private float BUTTON_HEIGHT = 0.1f;
        private float CONTAINER_MAX_SIZE = 2.5f;

        public float containerExpansion = 1f;
        public bool doContainerExpansion = false;

        public override void OnInitialize()
        {
            Asset<Texture2D> panelBackgroundSquare = AssetDirectory.Textures.UI.PanelBackgroundSquare;
            Asset<Texture2D> panelBorderSquare = AssetDirectory.Textures.UI.PanelBorderSquare;

            // the container which wraps around EVERYTHING you need to click on
            container = new UIPanel(panelBackgroundSquare, panelBorderSquare);
            container.Width.Set(Main.screenHeight * 0.4f, 0f);
            container.Height.Set((Main.screenHeight * 0.4f), 0f);
            container.HAlign = 0.5f;
            container.Top.Set(Main.screenHeight - (Main.screenHeight * 0.7f), 0f);
            container.BorderColor = container.BackgroundColor = Color.Red;
            //container.BorderColor = container.BackgroundColor = Color.White * 0; // comment out to see where box should be

            container.MarginBottom = 0;
            container.MarginTop = 0;
            container.MarginLeft = 0;
            container.MarginRight = 0;
            container.SetPadding(0);
            Append(container);

            // lord of weeds swag portrait
            Asset<Texture2D> lordOfWeedsPortraitSprite = AssetDirectory.Textures.UI.LordOfWeedsPortraitSprite;
            lordOfWeedsPortrait = new UIImage(lordOfWeedsPortraitSprite);
            lordOfWeedsPortrait.HAlign = container.HAlign;
            lordOfWeedsPortrait.Top.Set(-lordOfWeedsPortrait.Height.Pixels, container.Top.Percent);

            // the base panel for talking
            panel = new UIPanel(panelBackgroundSquare, panelBorderSquare);
            panel.Width.Set(0f, 1f);
            panel.Height.Set(0f, 0.45f);
            panel.Top.Set(0f, 0f); // vert alignment
            panel.Left.Set(0f, 0f); // horiz alignment
            panel.BorderColor = Color.White;
            panel.BackgroundColor = new Color(5, 20, 30) * 0.95f;

            panel.MarginBottom = 0;
            panel.MarginTop = 0;
            panel.MarginLeft = 0;
            panel.MarginRight = 0;
            panel.SetPadding(0);

            //TODO: use localization files for everything below this
            //TODO: make not a button
            // the nameplate that displays the lord of weeds' name 
            nameplate = new DialogueBoxButton(OnButtonClick, new UIText("Lord of Weeds"));
            nameplate.Width.Set(0f, 0.3f);
            nameplate.Height.Set(Main.screenHeight * 0.03f, 0f);
            nameplate.Top.Set(panel.Top.Pixels - (nameplate.Height.Pixels * 0.8f), 0f);

            // context sensisitve textbox
            infobox = new UIPanel(panelBackgroundSquare, panelBorderSquare);
            infobox.Width.Set(container.Width.Pixels * 0.98f, 0f);
            infobox.Height.Set(container.Height.Pixels - panel.Height.Pixels, 0f);
            infobox.HAlign = 0.5f;
            infobox.Top.Set(0f, container.Top.Percent - (1 - panel.Height.Percent));

            // 
            button1 = new DialogueBoxButton(OnButtonClick, new UIText("Left", 0.75f));
            button1.Width.Set(0f, 0.2f);
            button1.HAlign = 0.15f;
            button1.Height.Set(0f, BUTTON_HEIGHT);
            button1.Top.Set(infobox.Height.Pixels - infobox.Top.Pixels, 0f); // MAKE WORM -(BUTTON_HEIGHT * 0.5f)

            // 
            button2 = new DialogueBoxButton(OnButtonClick, new UIText("Middle", 0.75f));
            button2.Width.Set(0f, 0.2f);
            button2.HAlign = 0.5f;
            button2.Height.Set(0f, BUTTON_HEIGHT);
            button2.Top.Set(0f, 1f - BUTTON_HEIGHT);

            // 
            button3 = new DialogueBoxButton(OnButtonClick, new UIText("Right", 0.75f));
            button3.Width.Set(0f, 0.2f);
            button3.HAlign = 0.85f;
            button3.Height.Set(0f, BUTTON_HEIGHT);
            button3.Top.Set(0f, 1f - BUTTON_HEIGHT);

            container.Append(lordOfWeedsPortrait);
            //infobox
            container.Append(nameplate);
            container.Append(panel);
            container.Append(infobox); //
            container.Append(button1);
            container.Append(button2);
            container.Append(button3);
            
        }

        private float ORIGINAL_TIME_INCREMENT = 0.01f;
        private float increaseTimeIncrement = 0.01f;
        private int numberOfBounces = 2;
        public override void Update(GameTime gameTime)
        {
            //container.Height.Set((Main.screenHeight * 0.2f) * CONTAINER_MAX_SIZE, 0);
            //panel.Height.Set(0f, (1f - BUTTON_HEIGHT * 0.5f) / containerExpansion);
            //button1.Height.Set(0f, BUTTON_HEIGHT / containerExpansion);
            //button1.Top.Set(0f, 1 - BUTTON_HEIGHT / containerExpansion);

            //button2.Height.Set(0f, BUTTON_HEIGHT / containerExpansion);
            //button2.Top.Set(0f, 1 - BUTTON_HEIGHT / containerExpansion);

            //button3.Height.Set(0f, BUTTON_HEIGHT / containerExpansion);
            //button3.Top.Set(0f, 1 - BUTTON_HEIGHT / containerExpansion);

            //infobox.Top.Set(-(Main.screenHeight * 0.2f), 1 - BUTTON_HEIGHT * 0.5f / containerExpansion);
            Main.NewText(infobox.Height.Pixels); // HEIGHT ZERO???? HEIGHT IS ZERO?????????? OF COURSE! WHAT A FOOL I WAS, TO THINK HEIGHT WOULD BE ANYTHING ELSE
            Main.NewText(infobox.Top.Pixels);
            Main.NewText(button1.Top.Pixels);

            if (doContainerExpansion) {
                // make button fall exponentially
                containerExpansion += increaseTimeIncrement;
                increaseTimeIncrement *= 1.5f;

                // bounces when hitting lower threshold
                if (containerExpansion >= CONTAINER_MAX_SIZE) {
                    containerExpansion = CONTAINER_MAX_SIZE;
                    increaseTimeIncrement = ORIGINAL_TIME_INCREMENT;
                    doContainerExpansion = false;
                }
            }

        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            doContainerExpansion = true;

            // treeset
            if (containerExpansion > 1f) {
                containerExpansion = 1f;
                doContainerExpansion = false;
            }
        }
    }
}
