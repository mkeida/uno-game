using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UNO.Cards;
using UNO.Components;

namespace UNO
{
    /// <summary>
    /// Třída hráč. Obstarává jeho karty v ruce, řeší jeho události jako je kliknutí myší a tak dále
    /// </summary>
    class Player
    {
        /// <summary>
        /// Kolekce kartiček, které má hráč v ruce
        /// </summary>
        public static List<Card> hand = new List<Card>();

        /// <summary>
        /// Pravda, pokud je uživatel na řadě a může hrát
        /// </summary>
        public static bool canPlay = true;

        /// <summary>
        /// Update metoda hráče
        /// </summary>
        public static void Update()
        {
            // Projde všechny karty v ruce
            foreach (Card card in hand)
            {
                // Může uživatel hrát?
                if (canPlay)
                {
                    // Je na hromádce nějaká zahraná karta?
                    if (Pile.cards.Count > 0)
                    {
                        // Vrchní karta hromádky
                        Card pileTop = Pile.cards.Last();

                        // Může být karta zahrána?
                        if (Helper.CompareCards(card, pileTop))
                        {
                            // Karta je hratelná
                            card.Playable = true;
                        }
                        else
                        {
                            // Kartu nyní nelze zahrát
                            card.Playable = false;
                        }
                    }
                    else
                    {
                        // Na hromádce není žádná zahraná karta. Pravděpodobně právě začla hra a tudíž můžeme zahrát jakoukoliv kartu.
                        card.Playable = true;
                    }
                }
                else
                {
                    // Uživatel nemůže hrát - vypne rámeček hratelnosti
                    card.Playable = false;
                }

                // Nakonec zavoláme vlastní update právě procházené karty
                card.Update();
            }

            // Počet karet, které můžeme zahrát
            int playable = 0;

            // Projde všechny karty v ruce
            foreach (Card card in hand)
            {
                // Můžeme kartu zahrát?
                if (card.Playable)
                    // Ano - navýšíme celkový počet hratelných karet
                    playable++;
            }

            // Je uživatel na řadě?
            if (canPlay)
            {
                // Pomocná proměnná - vrchní karta hracího balíčku
                Card top = Deck.cards.Last();

                // Pokud nemůžeme zahrát žádnou kartu
                if (playable == 0)
                {
                    // Nemáme co zahrát
                    // Zvýrazníme vrchní kartu hracího balíčku
                    top.Playable = true;
                }
                else
                {
                    // Máme co zahrát
                    // Není důvod zvýrazňovat vrchní kartu hracího balíčku
                    top.Playable = false;
                }

                // Updatujeme vrchní kartu
                top.Update();
            }
        }

        /// <summary>
        /// Vykreslí hráčovu ruku
        /// </summary>
        public static void Draw(Form f, Graphics g)
        {
            // Projde všechny karty v ruce
            for (int i = 0; i < hand.Count; i++)
            {
                // Získá právě procházenou kartu - pomocná proměnná
                Card card = hand[i];

                // Nastaví Z-Index
                card.ZIndex = i;

                // Nakonec kartu vykreslíme - zavoláme její draw metodu
                card.Draw(g);
            }
        }

        /// <summary>
        /// Zarovná hráčovu ruku
        /// </summary>
        public static void AlignHand(Form f)
        {
            // Celková šířka všech karet v ruce
            int handWidth = 0;

            // Offset (odsazení) jednotlivých kartiček. Zkrátka o jak velký kus se budou jednotlivé kartičky překrývat.
            // Odsazení roste s přibývajícím počtem karet v ruce, aby se jich na obrazovku vešlo co nejvíce (počet je omezen defaultně na 20)
            int xOffset = - (hand.Count * 7);

            // Spočítá celkovou šířku karet v ruce včetně odsazení
            // Projde všechny karty v ruce
            foreach (Card card in hand)
            {
                // Inkrementuje celkovu šířku ruky o šířku právě procházené kartičky.
                handWidth += card.Dimensions.Width + xOffset;
            }

            // Startovní pozice první karty v ruce
            // Ta zajistí, že kartičky budou vždy pěkně na středu
            int start = (f.ClientRectangle.Width / 2) - (handWidth / 2) + (xOffset / 2);

            // Vykreslí ruku nepřítele
            for (int i = 0; i < hand.Count; i++)
            {
                // Nastaví cílovou pozici karty na nově aktualizovanou polohu založenou na předešlých výpočtech
                // Cílová pozice se od té skutečné liší tak, že se skutečná pozice mění do té doby, dokud se nerovná té cílové.
                // Zkrátka cílová pozice je ta, kde by kartička měla být a ta skutečná ta, kde se fyzicky karta nachází.
                // Díky tomuto systému jsou dosaženy animace pohybu karet.
                hand[i].TargetPosition = new Point(start + ((hand[i].Dimensions.Width + xOffset) * i), f.ClientRectangle.Height - hand[i].Dimensions.Height - 20);
            }
        }

        /// <summary>
        /// Lízací metoda hráče. Nejedná se o vykreslovací metodu, ale pouze o kolizi názvů mezi anglickými slovy líznout
        /// a vykreslit. Metoda jako parametr přijímá intové číslo, které představuje počet karet, které si má hráč líznout.
        /// </summary>
        public static async void Draw(int count)
        {
            // Zopakuje X-krát kde X je počet lízaných karet (count)
            foreach (var i in Enumerable.Range(0, count))
            {
                // Zkontrolujeme, jestli v balíčku zbyla nějaká karta
                if (Deck.cards.Count == 0)
                    Deck.Create();

                // Zkontrolujeme, jestli má hráč pořád méně, než maximálních 20 karet na ruce
                if (hand.Count == 20)
                    return;

                // Vrchní karta z hracího balíčku
                Card top = Deck.cards[Deck.cards.Count - 1];

                // Odstraní vrchní kartu z balíčku (protože si jí líznul hráč)
                Deck.cards.Remove(top);

                // Odstraní rotaci karty
                top.Settle(0);

                // Otočí kartu bříškem nahoru, abychom ji viděli
                top.Flip();

                // Spustí animaci čísla karty
                top.Zoom();

                // Přidá kartičku do naší ruky
                hand.Add(top);

                // Aktualizuje layout a pošle všechny kartičky tam, kam patří
                Game.container.RefreshLayout();

                // Přidá malé spoždění, abychom kartičky nenalízli všechny naráz
                await Task.Delay(350);
            }
        }

        /// <summary>
        /// Řeší uživatelovi (hráčovi) klik akce (události)
        /// </summary>
        public static void Click(Form f)
        {
            // Stejný postup jako klik event u třídy balíčku (Deck.cs) - podrobněji rozepsáno tam
            // Kolekce karet, na které bylo kliknuto - zatím jen její založení
            List<Card> clickedCards = new List<Card>();

            // Projde všechny karty v ruce
            foreach (Card card in hand)
            {
                // Získá pozici myši
                Point p = f.PointToClient(Cursor.Position);

                // Cards bounds
                Rectangle bounds = new Rectangle(card.Position.X, card.Position.Y, card.Dimensions.Width, card.Dimensions.Height);

                // Klikl uživatel na právě procházenou kartu?
                if (bounds.Contains(p))
                {
                    // Přidá kartu do kolekce
                    clickedCards.Add(card);
                }
            }

            // Klikli jsme na nějakou kartu?
            if (clickedCards.Count == 0)
                // Ne - můžeme se vklídku vrátit
                return;

            // Spustí klik metodu karty nejblíže k uživateli
            clickedCards.OrderByDescending(item => item.ZIndex).First().MouseClick();

            // Vyresetuje kolekci nakliklých karet pro další použití při dalším klik eventu
            clickedCards.Clear();
        }

        // Odehrávací metoda hráče.
        public static void Play()
        {
            // Umožní hráči hrát
            canPlay = true;
        }

        /// <summary>
        /// Ukončí hráčovo kolo
        /// </summary>
        public static void End()
        {
            // Ukončí své kolo
            canPlay = false;
        }
    }
}
