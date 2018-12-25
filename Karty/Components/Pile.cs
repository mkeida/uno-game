using UNO.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace UNO
{
    /// <summary>
    /// Hromádka naposled zahraných karet.
    /// </summary>
    class Pile
    {
        /// <summary>
        /// Kolekce naposled zahraných karet.
        /// </summary>
        public static List<Card> cards = new List<Card>();

        /// <summary>
        /// Vytvoří novou hromádku.
        /// </summary>
        public static void Create()
        {
        }

        /// <summary>
        /// Zarovná hromádku karet na svou správnou pozici.
        /// </summary>
        public static void Align(Form f)
        {
            // Instance třídy Random
            Random r = new Random();

            // Projde všechny karty na hromádce
            foreach (Card card in cards)
            {
                // Nastaví jejich pozici na správné místo kousek od středu
                card.TargetPosition = new Point(f.ClientRectangle.Width / 2 - card.Dimensions.Width / 2 + 170, f.ClientRectangle.Height / 2 - card.Dimensions.Height / 2);
            }
        }

        /// <summary>
        /// Update hromádky naposled zahraných karet.
        /// </summary>
        public static void Update()
        {
            // Projde všechny kartičky
            foreach (Card card in cards)
            {
                // Aktualzuje právě procházenou kartičku
                card.Update();
            }
        }

        /// <summary>
        /// Vykreslí hromádku
        /// </summary>
        public static void Draw(Graphics g)
        {
            // Z výkonostních důvodů vykreslí pouze 4 a méně karet.
            for (int i = cards.Count - ((cards.Count - 4 > 4) ? 4 : cards.Count); i < cards.Count; i++)
            {
                // Vykreslí právě procházenou kartičku - zavolá její draw mwtodu.
                cards[i].Draw(g);
            }
        }
    }
}
