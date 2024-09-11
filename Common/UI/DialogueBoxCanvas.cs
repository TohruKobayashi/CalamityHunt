using System.Collections.Generic;
using System.Text;
using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityHunt.Common.UI
{
    public class DialogueBoxCanvas : UIState
    {
        public DialogueBoxButton _dialogueBoxButton;

        public override void OnInitialize()
        {
            Asset<Texture2D> panelBackgroundSquare = AssetDirectory.Textures.UI.PanelBackgroundSquare;
            Asset<Texture2D> panelBorderSquare = AssetDirectory.Textures.UI.PanelBorderSquare;

            UIPanel panel = new UIPanel(panelBackgroundSquare, panelBorderSquare);
            panel.Width.Set(0, 0.4f);
            panel.Height.Set(0, 0.2f);
            panel.HAlign = 0.5f;
            panel.VAlign = 0.25f;
            panel.BorderColor = Color.White;
            panel.BackgroundColor = new Color(5, 20, 30) * 0.95f;
            Append(panel);

            _dialogueBoxButton = new DialogueBoxButton(OnButtonClick);
            _dialogueBoxButton.Width.Set(0, 0.05f);
            _dialogueBoxButton.Height.Set(0, 0.025f);
            _dialogueBoxButton.HAlign = 0.5f;
            _dialogueBoxButton.VAlign = 0.25f;
            panel.Append(_dialogueBoxButton);
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.NewText("god's wrath!!!!");
        }
    }
}
