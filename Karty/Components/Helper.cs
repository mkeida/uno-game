using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UNO.Cards;

namespace UNO.Components
{
    /// <summary>
    /// Pomocná třída. Obsahuje to, co se jinam nehodilo.
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Porovná dvě karty. Vrátí true, pokud sou navzájem na sebe hratelné.
        /// </summary>
        public static bool CompareCards(Card c1, Card c2)
        {
            // Porovnání barev
            if (c1.Color == c2.Color)
            {
                return true;
            }

            // Porovnání čísel
            if (c1.Number == c2.Number)
            {
                return true;
            }

            // Karty nemají nic společného, vrátí false
            return false;
        }
    }
}
