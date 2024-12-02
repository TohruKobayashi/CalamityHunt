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
    public class DialogueBoxButton : UIElement
    {
        private object _text;
        private MouseEvent _clickAction;
        private UIPanel _uiPanel;
        private UIText _uiText;

        public DialogueBoxButton(MouseEvent clickAction, UIText uIText) : base()
        { 
            _clickAction = clickAction;
            _uiText = uIText;
        }

        public override void OnInitialize()
        {
            Asset<Texture2D> panelBackgroundSquare = AssetDirectory.Textures.UI.PanelBackgroundSquare;
            Asset<Texture2D> panelBorderSquare = AssetDirectory.Textures.UI.PanelBorderSquare;

            _uiPanel = new UIPanel(panelBackgroundSquare, panelBorderSquare);
            _uiPanel.Width = StyleDimension.Fill;
            _uiPanel.Height = StyleDimension.Fill;
            _uiPanel.BorderColor = Color.White;
            _uiPanel.BackgroundColor = new Color(5, 20, 30) * 0.9f;
            Append(_uiPanel);

            //_uiText = new UIText("More", 0.75f);
            _uiText.VAlign = _uiText.HAlign = 0.5f;
            _uiPanel.Append(_uiText);

            _uiPanel.OnLeftClick += _clickAction;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // Propagate update to child elements.
            if (_text != null) {
                _uiText.SetText(_text.ToString());
                _text = null;
                Recalculate();
                base.MinWidth = _uiText.MinWidth;
                base.MinHeight = _uiText.MinHeight;
            }
        }
    }
}
