using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UNO.Cards;
using UNO.Components;

namespace UNO
{
    /// <summary>
    /// Třída nepřítel. Řeší "umělou inteligenci" - AI nepřítele. Definuje pár základních
    /// metod pro umožnění jeho interakce s hráčem a tak dále.
    /// </summary>
    class Enemy
    {
        /// <summary>
        /// Kolekce karet, které nepřítel drží v ruce.
        /// </summary>
        public static List<Card> hand = new List<Card>();

        /// <summary>
        /// Update nepřítele.
        /// </summary>
        public static void Update()
        {
            // Projede všechny karty v ruce
            foreach (Card card in hand)
            {
                // Následně zavolá jejich update metody
                card.Update();
            }
        }

        /// <summary>
        /// Vykreslí protivníkovi karty, které má na ruce. Metoda přijímá vykreslovací kontext a formulář,
        /// aby se měli karty kam vykreslit a správně se zarovanalis výškou a šířkou předaného formuláře.
        /// </summary>
        public static void Draw(Form f, Graphics g)
        {
            // Projede všechny karty v ruce
            foreach (Card card in hand)
            {
                // Vykreslí právě procházenou kartu
                card.Draw(g);
            }
        }

        /// <summary>
        /// Vyřeší pozicování každé karty v ruce nepřítele. Metoda obvykle volána na startu hry nebo při změně velikosti okna.
        /// </summary>
        public static void AlignHand(Form f)
        {
            // Celková šířka všech karet v ruce
            int handWidth = 0;

            // Offset (odsazení) jednotlivých kartiček. Zkrátka o jak velký kus se budou jednotlivé kartičky překrývat.
            // Odsazení roste s přibývajícím počtem karet v ruce, aby se jich na obrazovku vešlo co nejvíce (počet je omezen defaultně na 20)
            int xOffset = -(hand.Count * 7);

            // Spočítá celkovou šířku karet v ruce včetně odsazení
            // Projde všechny karty v ruce
            foreach (Card card in hand)
            {
                // Inkrementuje celkovu šířku ruky o šířku právě procházené kartičky.
                handWidth += card.Dimensions.Width + xOffset;
            }

            // Startovní pozice první karty v ruce
            // Ta zajsití, že kartičky budou vždy pěkně na středu
            int start = (f.ClientRectangle.Width / 2) - (handWidth / 2) + (xOffset / 2);

            // Vykreslí ruku nepřítele
            for (int i = 0; i < hand.Count; i++)
            {
                // Nastaví cílovou pozici karty na nově aktualizovanou polohu založenou na předešlých výpočtech
                // Cílová pozice se od té skutečné liší tak, že se skutečná pozice mění do té doby, dokud se nerovná té cílové.
                // Zkrátka cílová pozice je ta, kde by kartička měla být a ta skutečná ta, kde se fyzicky karta nachází.
                // Díky tomuto systému jsou dosaženy animace pohybu karet.
                hand[i].TargetPosition = new Point(start + ((hand[i].Dimensions.Width + xOffset) * i), 20);
            }
        }

        /// <summary>
        /// Lízací metoda nepřítele. Nejedná se o vykreslovací metodu, ale pouze o kolizi názvů mezi anglickými slovy líznout
        /// a vykreslit. Metoda jako parametr přijímá intové číslo, které představuje počet karet, které si má nepřítel líznout.
        /// </summary>
        public static async void Draw(int count)
        {
            // Zopakuje X-krátm kde X je počet lízaných karet (count)
            foreach (var i in Enumerable.Range(0, count))
            {
                // Zkontrolujeme, jestli v balíčku zbyla nějaká karta
                if (Deck.cards.Count == 0)
                    Deck.Create();

                // Zkontrolujeme, jestli má nepřítel pořád méně, než maximálních 20 karet na ruce
                if (hand.Count == 20)
                    return;

                // Vrchní karta z hracího balíčku
                Card top = Deck.cards[Deck.cards.Count - 1];

                // Odstraní vrchní kartu z balíčku (protože si jí líznul nepřítel)
                Deck.cards.Remove(top);

                // Odstraní rotaci karty
                top.Settle(0);

                // Přidá kartu nepříteli do ruky.
                hand.Add(top);

                // Refresh layoutu zapříčiní, že po odebrání karty z kolekce hracího balíčku
                // a její následné přidání do kolekce karet nepřítele provede animaci přesunu z balíčku do ruky nepřítele.
                Game.container.RefreshLayout();

                // Přidá spoždění, aby se karty nenalízali na ráz ale hezky s odstupem
                await Task.Delay(350);
            }
        }

        /// <summary>
        /// Odehrávací metoda nepřítele. Jeho "AI". Zde se rozhodne, jakou kartu nepřítel odehraje.
        /// </summary>
        public async static void Play()
        {
            // Vytvoří náhodné spoždění mezi 0.75 a 1.5 sekundy aby to vypadalo, že nepřítel "přemýšlí" xD
            await Task.Delay(new Random().Next(750, 1500));

            // Byla zahrána nějaká karta? - začla právě hra?
            if (Pile.cards.Count == 0)
            {
                // Zahraje tu nejpostrádatelnější kartu z ruhy
                PlayCard(ChooseBestCard(hand));
            }
            else
            {
                // Byla již vyložena nějaká karta
                // Získá naposled zahranou kartu
                Card lastPlayed = Pile.cards.Last();

                // List všech karet, které by mohl nepřítel zahrát - zatím pouze jeho založení
                List<Card> playable = new List<Card>();

                // Projde všechny karty v ruce nepřítele
                foreach (Card card in hand)
                {
                    // Porovná, jestli právě procházená karta a naposled zahraná karta jsou spolu kompatibilní.
                    // To znamená, jestli je lze na sebe zahrát.
                    if (Helper.CompareCards(card, lastPlayed))
                    {
                        // Karty lze na sebe zahrát, přidáme jí do kolekce hratelných karet
                        playable.Add(card);
                    }
                }

                // Má nepřítel co zahrát? Máme nějaké karty v kolekci hratelných karet?
                if (playable.Count == 0)
                {
                    // Nepřítel nemá co zahrát, tak si lízne
                    Draw(1);
                }
                else
                {
                    // Nepřítel může něco zahrát.
                    // Má nějaké volby? Může zahrát více jak jednu kartu?
                    if (playable.Count == 1)
                    {
                        // Nemá možnost volby, zahraje proto jedinou kartu, kterou může
                        PlayCard(playable.First());
                    }
                    else
                    {
                        // Nepřítel má možnost volby (může zahrát 2 a více karet)
                        // Předáme list hratelných karet metodě, která rozhodne, jakou bude nejlepší zahrát.
                        PlayCard(ChooseBestCard(playable));
                    }
                }

                // Vyčístí kolekci hratelných karet pro další použití
                playable.Clear();
            }

            // Předá kolo uživateli
            Player.Play();
        }

        /// <summary>
        /// Metoda přijímá list karet, z kterých vybere tu, která bude nejvýhodnější pro zahrání
        /// </summary>
        private static Card ChooseBestCard(IEnumerable<Card> cards)
        {
            // Vytvoří nový slovník, který bude obsahovat počet jednotlivých karet od každé barvy
            Dictionary<Color, int> balance = new Dictionary<Color, int>();

            // Naplní slovník
            balance.Add(Deck.red, hand.Where(item => item.Color == Deck.red).Count());
            balance.Add(Deck.blue, hand.Where(item => item.Color == Deck.blue).Count());
            balance.Add(Deck.green, hand.Where(item => item.Color == Deck.green).Count());
            balance.Add(Deck.yellow, hand.Where(item => item.Color == Deck.yellow).Count());

            // Seřadí slovník podle počtu jednotlivých barev sestupně. To znamená, že nahoře bude taková barva,
            // od které máme nejvíc karet.
            var ordered = balance.OrderBy(item => item.Value).Reverse();

            // Projdeme slovník
            foreach (KeyValuePair<Color, int> entry in ordered)
            {
                // Projde všechny hratelné karty
                foreach (Card card in cards)
                {
                    // Vybere první kartu z hratelných karet, od které máme nejvíc barev
                    if (card.Color == entry.Key)
                    {
                        // Vrátí naší kartičku
                        return card;
                    }
                }
            }

            // AI nepřišla na nejlepší možný play, zahraje tedy první možnou kartu
            return cards.First();
        }

        /// <summary>
        /// Metoda jako parametr bere kartu. Tuto kartu pak nepřítel odehraje.
        /// </summary>
        public static void PlayCard(Card card)
        {
            // Odstraní kartu z ruky nepřítele.
            hand.Remove(card);

            // Přidá kartu na hromádku zahraných karet
            Pile.cards.Add(card);

            // Otočí kartu (s animací) bříškem nahoru tak, aby byla viditelná
            card.Flip();

            // Nastaví náhodnou rotaci karty (jen pro efekt)
            card.Settle(new Random().Next(-180, 180));

            // Zarovná celé hrací pole = přesune kartičky tam, kam patří na základě jejich kolekcí, kde sou umístěny.
            Game.container.RefreshLayout();
        }
    }
}
