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
        public DialogueBoxButton _dialogueBoxButton2;
        public DialogueBoxButton _dialogueBoxButton3;

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
            _dialogueBoxButton.Width.Set(0, panel.Width.Precent * 0.2f);
            _dialogueBoxButton.Height.Set(0, panel.Height.Precent * 0.2f);
            _dialogueBoxButton.HAlign = panel.Width.Precent * 0.35f - _dialogueBoxButton.Width.Precent * 0.35f; //0.35f
            _dialogueBoxButton.VAlign = 0.5f;
            Append(_dialogueBoxButton);

            _dialogueBoxButton2 = new DialogueBoxButton(OnButtonClick2);
            _dialogueBoxButton2.Width.Set(0, 0.2f);
            _dialogueBoxButton2.Height.Set(0, 0.2f);
            _dialogueBoxButton2.HAlign = 0.65f;
            _dialogueBoxButton2.VAlign = 1.25f;
            panel.Append(_dialogueBoxButton2);

            _dialogueBoxButton3 = new DialogueBoxButton(OnButtonClick3);
            _dialogueBoxButton3.Width.Set(0, 0.2f);
            _dialogueBoxButton3.Height.Set(0, 0.2f);
            _dialogueBoxButton3.HAlign = 0.95f;
            _dialogueBoxButton3.VAlign = 1.25f;
            panel.Append(_dialogueBoxButton3);
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.NewText("1");
        }

        private void OnButtonClick2(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.NewText("2");
        }

        private void OnButtonClick3(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.NewText("3");
        }
    }
}
