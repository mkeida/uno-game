using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UNO.Cards;

namespace UNO
{
    /// <summary>
    /// Hrací balík. Metody balíčku jsou statické, protože nepředpokládám, že by ve hře existovalo
    /// víc instancí hracího balíčku = hrací balíček je vždy jen jeden.
    /// </summary>
    class Deck
    {
        /// <summary>
        /// Červená barva. Globálně použitá pro všechny červené karty.
        /// </summary>
        public static Color red = Color.FromArgb(0, 57, 53);

        /// <summary>
        /// Modrá barva. Globálně použitá pro všechny modré karty.
        /// </summary>
        public static Color blue = Color.FromArgb(3, 151, 215);

        /// <summary>
        /// Zelená barva. Globálně použitá pro všechny zelené karty.
        /// </summary>
        public static Color green = Color.FromArgb(3, 167, 81);

        /// <summary>
        /// Žlutá barva. Globálně použitá pro všechny žluté karty.
        /// </summary>
        public static Color yellow = Color.FromArgb(252, 223, 20);

        /// <summary>
        /// Kolekce všech karet v balíčku.
        /// </summary>
        public static List<Card> cards = new List<Card>();

        /// <summary>
        /// Zamíchá balíček - randomizuje pořadí karet v kolekci cards
        /// </summary>
        public static void Shuffle()
        {

        }

        /// <summary>
        /// Vytvoří nový hrací balíček - naplní kolekci cards kartamy (objekty typu Card) podle
        /// pravidel karetní hry UNO.
        /// </summary>
        public static void Create()
        {
            // Resetuje kolekci karet, pokud předtím byla naplněná
            cards.Clear();

            // Vytvoří novou instanci třídy Random.
            Random r = new Random();

            // Cyklus se zopakuje 108-krát. Podle pravidel by měl hrací balíček obsahovat
            // přesně tolik karet.
            foreach (var i in Enumerable.Range(0, 108))
            {
                // Pole všech možných barev, které může karta mít.
                // Pomocné pole nám slouží k jednoduší generaci náhodných barev karet.
                Color[] colors = { red, blue, green, yellow };

                // Vytvoří instanci Karty a zvolí ji náhodnou barvu a číslo od 0 do 7.
                Card card = new Card(colors[r.Next(colors.Count())], r.Next(0, 7));

                // Přiřadí kartě Z-Index. Tento index nám určuje, jak moc blízko po pomyslné ose Z se
                // karta nachází směrem k pozorovateli (hráči / uživateli). Čím větší Z-Index je, tím blíže je karta
                // k pozorovateli. Z-Index je důležitý pro to, abychom věděli, na jakou kartu uživatel klikl, pokud se nachází
                // víc jednotlivých karet na sobě - jako v případě našeho hracího balíčku.
                card.ZIndex = i;

                // Otočí všechny karty v balíčku bříškem dolů. Boolovský parametr metody zajistí, že se neprovede nechtěná animace,
                // která by byla vidět na startu hry.
                card.Flip(true);

                // Nastaví rotaci karty od -180 do 180. Pouze vizuální efekt a detail, který nemá na funkčnost hry vliv.
                card.Rotation = r.Next(-180, 180);

                // Nakonec přidáme naší kartičku do kolekce všech karet v balíku.
                cards.Add(card);
            }
        }

        /// <summary>
        /// Umístí balíček na správnou pozici v hracím okně. Metoda je volána vždy, pokud dojde k změně velikosti okna
        /// a tudíš i potřebě přesunout balíček tak, aby byl vždy na správné pozici kousek od středu.
        /// </summary>
        public static void Align(Form f)
        {
            // Projde všechny kartičky v našem hracím balíčku. Chceme totiž
            // aktualizovat pozici každé kartičky v balíčku.
            foreach (Card card in cards)
            {
                // Aktualizuje pozici kartičky tak, aby byla vždy kousek od středu.
                card.Position = card.TargetPosition = new Point(f.ClientRectangle.Width / 2 - card.Dimensions.Width / 2 - 170, f.ClientRectangle.Height / 2 - card.Dimensions.Height / 2);
            }
        }

        /// <summary>
        /// Update metoda hracího balíčku. Zajistí, že bude zavolána Update metoda
        /// každé kartičky v balíčku.
        /// </summary>
        public static void Update()
        {
            // Projde všechny kartičky v našem hracím balíčku.
            foreach (Card card in cards)
            {
                // Zavolá update metodu právě procházené kartičky.
                card.Update();
            }
        }

        /// <summary>
        /// Vykreslovací metoda balíčku. Zajsití, že náš balíček se vykreslí na hrací obrazovku.
        /// </summary>
        public static void Draw(Graphics g)
        {
            // Projde vždy posledních 5 a méně karet v balíčku. Vykreslujeme zde pouze 5 a méně kartiček z důvodu výkonnostních
            // limitací Windows Forms. Ty jak známo nejsou graficky akcelerované a tudíš k vykreslovaní nepoužívají grafickou kartu,
            // nýbrž pouze procesor.
            for (int i = cards.Count - ((cards.Count - 5 > 5) ? 5 : cards.Count); i < cards.Count; i++)
            {
                // Zavolá Draw metodu právě procházené kartičky = vykreslí kartičku.
                cards[i].Draw(g);
            }
        }

        /// <summary>
        /// Klik metoda balíčku. Řeší, co se stane, když uživatel klikne na balíček.
        /// </summary>
        public static void Click(Form f)
        {
            // Kolekce karet, na které uživatel kliknul. Pamatujeme si, že uživatel může kliknout na více karet současně
            // z důvodu jejich překrývání.
            List<Card> clickedCards = new List<Card>();

            // Iterujeme celou kolekcí po zpátku z důvodu, abychom mohli za běhu modifikovat prvky kolekce, které procházíme.
            for (int i = cards.Count - 1; i >= 0; i--)
            {
                // Získá instanci karty, kterou právě procházíme - pomocná proměnná.
                Card card = cards[i];

                // Získá pozici myší.
                Point p = f.PointToClient(Cursor.Position);

                // Oblast karty (rectangle / obdélník) - opět pomocná proměnná
                Rectangle bounds = new Rectangle(card.Position.X, card.Position.Y, card.Dimensions.Width, card.Dimensions.Height);

                // Zjistí, jestli oblast karty obsahuje myš. Jelikož se nacházíme v Click eventu,
                // znamenalo by to, že uživatel na kartu klikl.
                if (bounds.Contains(p))
                {
                    // Uživatel skutečně klikl na právě procházenou kartu. Přidáme ji proto do kolekce kliknutých karet.
                    clickedCards.Add(card);
                }
            }

            // Zjistí, jestlu živatel vůbec klikl na nějakou kartu. Pokud je kolekce prázná, na žádnou neklikl
            // a můžeme tedy vyskočit z klik eventu.
            if (clickedCards.Count == 0)
                return;

            // Kolekce není prázdná, tudíž je zde alespoň jedna a více karet, na které
            // bylo kliknuto.
            // Seřadí kolekci kliknutých karet sestupně a u první karty z takto seřazené kolekce zavolá Click metodu.
            // Toto provádíme, abychom aktivovali klik jen u karty, která byla na vršku všech ostatních (to znamená nejblíže uživateli)
            clickedCards.OrderByDescending(item => item.ZIndex).First().MouseClick();

            // Nakonec vyčístíme kolekci nakliklých karet, aby byla připravená pro další kliknutí.
            clickedCards.Clear();
        }
    }
}
