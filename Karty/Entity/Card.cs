using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Threading.Tasks;

namespace UNO.Cards
{
    /// <summary>
    /// Stav karty.
    /// Normal - karta je klasicky viditelná
    /// Back - karta je otočená břískem dolů (vidíme pouze zadní stranu)
    /// </summary>
    public enum State
    {
        Normal,
        Back
    }

    /// <summary>
    /// Možné typy karet
    /// Classic - klasická karta s barvou a číslem
    /// </summary>
    public enum Type
    {
        Classic
    }

    /// <summary>
    /// Třída karty
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Stav karty
        /// </summary>
        public State State { get; set; }

        /// <summary>
        /// Typ karty
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Barva karty
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Číslo karty
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Skutečná pozice karty
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Z index karty - čím větší, tím blíže po ose Z k uživateli
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// Cílová pozice karty
        /// </summary>
        public Point TargetPosition { get; set; }

        /// <summary>
        /// Rozměry karty
        /// </summary>
        public Size Dimensions { get; set; }

        /// <summary>
        /// Rotace karty
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Můžeme kartu zahrát? - Rámeček hratelnosti
        /// </summary>
        public bool Playable { get; set; }

        /// <summary>
        /// Pravda, pokud je karta bříškem dolů
        /// </summary>
        public bool Flipped { get; set; }

        /// <summary>
        /// Pravda, pokud by se karta měla otočit bříškem dolů - řídí animaci
        /// </summary>
        private bool flipDown = false;

        /// <summary>
        /// Pravda, pokud by se karta měla otočit bříškem nahoru - řídí animaci
        /// </summary>
        private bool flipUp = false;

        /// <summary>
        /// Scale karty po ose X
        /// </summary>
        private float flipX = 1;

        /// <summary>
        /// Pravda, pokud by měla kartička vynulovat svou rotaci
        /// </summary>
        private bool settleRotation = false;

        /// <summary>
        /// Rychlost animací
        /// </summary>
        private int speed = 25;

        /// <summary>
        /// Pravda, pokud by se měla spustit animace zvětšední a zmenšení čísla
        /// </summary>
        private bool zoom = false;

        /// <summary>
        /// Maximum zvětšení
        /// </summary>
        private int zoomMaximum = 30;

        /// <summary>
        /// Přírůstek velikosti fontu po zvětšení
        /// </summary>
        private int zoomScale = -20;

        /// <summary>
        /// Cílový úhel
        /// </summary>
        private int targetAngle = 0;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Card(Color color, int number)
        {
            // Inicializuje počáteční vnitřní stav objektu 
            Color = color;
            Number = number;

            // Definuje standardní rozměry karty
            Dimensions = new Size(200, 250);

            // Defautlní stav
            State = State.Normal;

            // Defautlní typ
            Type = Type.Classic;
        }

        /// <summary>
        /// Update metoda kartičky
        /// </summary>
        public void Update()
        {
            // Má karta být otočena bříškem dolů?
            if (flipDown)
            {
                // Ještě stále není plně otočená?
                if (flipX > -1)
                {
                    // Otočíme jí zas o něco víc
                    flipX -= 0.1f;

                    // Překročila akrtička mrtvý úhel viditelnosti? = je k nám úplně kolmo ? 
                    if (flipX < 0)
                    {
                        // Otočí kartičku bříškem dolů
                        State = State.Back;
                    }
                }
                else
                {
                    // Otočení dokončeno
                    flipDown = false;

                    // Aktualizuje stav otočení
                    Flipped = true;
                }
            }

            // Má karta být otočena bříškem nahoru?
            if (flipUp)
            {
                // Ještě stále není plně otočená?
                if (flipX < 1)
                {
                    // Otočíme jí zas o něco víc
                    flipX += 0.1f;

                    // Překročila akrtička mrtvý úhel viditelnosti? = je k nám úplně kolmo ? 
                    if (flipX > 0)
                    {
                        // Otočí kartičku bříškem nahoru
                        State = State.Normal;
                    }
                }
                else
                {
                    // Otočení dokončeno
                    flipUp = false;

                    // Aktualizuje stav otočení
                    Flipped = false;
                }
            }

            // Aktualizuje pozici karty. Pokud se skutečná pozice nerovná té cílové, posune se skutečná pozice zas o něco
            // blíž té cílové - nejdříve o rychlost animace a poté, když je kartička už hodně blízko, pouze po pixelu aby došlo
            // k perfektní shodě obou pozic. Opakuje se pro všechny 4 možné směry pohybu.
            if (Position.X > TargetPosition.X)
                Position = new Point(Position.X - ((Position.X - TargetPosition.X > speed) ? speed : 1), Position.Y);

            if (Position.X < TargetPosition.X)
                Position = new Point(Position.X + ((TargetPosition.X - Position.X > speed) ? speed : 1), Position.Y);

            if (Position.Y > TargetPosition.Y)
                Position = new Point(Position.X, Position.Y - ((Position.Y - TargetPosition.Y > speed) ? speed : 1));

            if (Position.Y < TargetPosition.Y)
                Position = new Point(Position.X, Position.Y + ((TargetPosition.Y - Position.Y > speed) ? speed : 1));

            // Vyřeší rotaci
            // Má se karta vyrovnat?
            if (settleRotation)
            {
                // Je rotace větší jak cílový úhel?
                if (Rotation > targetAngle)
                {
                    Rotation -= (Math.Abs(Rotation-targetAngle) > 15) ? 15 : 1;
                }
                // Je rotace menší jak cílový úhel
                else if (Rotation < targetAngle)
                {
                    Rotation += (Math.Abs(Rotation-targetAngle) > 15) ? 15 : 1;
                }
                // Cílová a skutečná rotace se shodují, proces je u konce
                else
                {
                    settleRotation = false;
                }
            }

            // Vyřeší zoom animacu
            if (zoom)
            {
                // Je zoom stále menší jak zoom maximum?
                if (zoomScale < zoomMaximum)
                {
                    // Přičte scale
                    zoomScale += 2;
                }
                else
                {
                    // Dosaženo maxima
                    // Ukončí zoom
                    zoom = false;

                    // Obrátí zoom scale
                    zoomScale = -zoomMaximum;
                }
            }
        }

        /// <summary>
        /// Vykreslovací metoda. Postará se o vykreslení kartičky na základě její hodnot.
        /// </summary>
        public void Draw(Graphics g)
        {
            // Karta otočená bříškem dolů
            if (State == State.Back)
            {
                // Odsazení bílého okraje
                int borderPadding = 15;

                // Posune počáteční vykreslovací pozici na střed karty
                g.TranslateTransform(Position.X + (Dimensions.Width / 2), Position.Y + (Dimensions.Height / 2));

                // Otočí vykreslovací plátno
                g.RotateTransform(Rotation);

                // Roztáhne vykreslovací plátno
                g.ScaleTransform(flipX, 1);

                // Zapne anti-aliasing
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Playable border size - šířka efektu karty, když jde zahrát
                int pbs = 10;

                // Můžeme kartu zahrát?
                if (Playable)
                {
                    // Vykreslí efekt kolem karty
                    using (LinearGradientBrush brush = new LinearGradientBrush(new Point(-(Dimensions.Width / 2) - pbs, -(Dimensions.Height / 2) - pbs), new Point(Dimensions.Width + (pbs * 2), Dimensions.Height + (pbs * 2)), Color.FromArgb(255, 217, 0), Color.FromArgb(231, 66, 0)))
                        g.FillPath(brush, Paint.RoundedRectangle(new Rectangle(-(Dimensions.Width / 2) - pbs, -(Dimensions.Height / 2) - pbs, Dimensions.Width + (pbs * 2), Dimensions.Height + (pbs * 2)), 10));
                }


                // Vykreslí obrys / stín karty
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, Color.Black)))
                    g.FillPath(brush, Paint.RoundedRectangle(new Rectangle(-(Dimensions.Width / 2) - 1, -(Dimensions.Height / 2) - 1, Dimensions.Width + 2, Dimensions.Height + 2), 10));

                // Býli okraj kolem karty
                g.FillPath(Brushes.White, Paint.RoundedRectangle(new Rectangle(-(Dimensions.Width / 2), -(Dimensions.Height / 2), Dimensions.Width, Dimensions.Height), 10));

                // Barevná výplň
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(45, 45, 45)))
                    g.FillPath(brush, Paint.RoundedRectangle(new Rectangle(-(Dimensions.Width / 2) + borderPadding, -(Dimensions.Height / 2) + borderPadding, Dimensions.Width - (borderPadding * 2), Dimensions.Height - (borderPadding * 2)), 10));

                // Rotace
                g.RotateTransform(-45f);

                // Vnitřní bílá elipsa
                g.FillPath(Brushes.White, Paint.Ellipse(new Point(0, 0), 75, 105));

                // Nastaví nazpátek scale pro správné vykreslní nápisu "UNO"
                g.ScaleTransform(flipX, 1);

                // Vytvoří formát řetězce pro psaní na střed
                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;

                // Zapne antialiasing textu
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Vykreslí UNO text
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(45, 45, 45)))
                    g.DrawString("UNO", new Font("Segoe UI", 36, FontStyle.Bold), brush, new Point(), format);

                // Vypne anti-aliasing
                g.SmoothingMode = SmoothingMode.Default;

                //  Vyresetuje transformace
                g.ResetTransform();

                // Vykreslení je ukonce - vrátíme se
                return;
            }

            // Klasická karta
            if (Type == Type.Classic)
            {
                // Býlý okraj kolem karty
                int borderPadding = 15;

                // Posune počáteční vykreslovací pozici na střed karty
                g.TranslateTransform(Position.X + (Dimensions.Width / 2), Position.Y + (Dimensions.Height / 2));

                // Orotuje karty
                g.RotateTransform(Rotation);

                // ROztáhne kartu
                g.ScaleTransform(flipX, 1);

                // Vypne anti-aliasing
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Playable border size - šířka efektu karty, když jde zahrát
                int pbs = 10;

                // Můžeme kartu zahrát?
                if (Playable)
                {
                    // Vykreslí efekt kolem karty
                    using (LinearGradientBrush brush = new LinearGradientBrush(new Point(-(Dimensions.Width / 2) - pbs, -(Dimensions.Height / 2) - pbs), new Point(Dimensions.Width + (pbs * 2), Dimensions.Height + (pbs * 2)), Color.FromArgb(255, 217, 0), Color.FromArgb(231, 66, 0)))
                        g.FillPath(brush, Paint.RoundedRectangle(new Rectangle(-(Dimensions.Width / 2) - pbs, -(Dimensions.Height / 2) - pbs, Dimensions.Width + (pbs * 2), Dimensions.Height + (pbs * 2)), 10));
                }

                // Vykreslí stín / okraj kolem karty
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, Color.Black)))
                    g.FillPath(brush, Paint.RoundedRectangle(new Rectangle(-(Dimensions.Width / 2) - 1, -(Dimensions.Height / 2) - 1, Dimensions.Width + 2, Dimensions.Height + 2), 10));

                // Vykreslí býlý okraj
                g.FillPath(Brushes.White, Paint.RoundedRectangle(new Rectangle(-(Dimensions.Width / 2), -(Dimensions.Height / 2), Dimensions.Width, Dimensions.Height), 10));

                // Barevná výplň
                using (SolidBrush brush = new SolidBrush(Color))
                    g.FillPath(brush, Paint.RoundedRectangle(new Rectangle(-(Dimensions.Width / 2) + borderPadding, -(Dimensions.Height / 2) + borderPadding, Dimensions.Width - (borderPadding * 2), Dimensions.Height - (borderPadding * 2)), 10));

                // Rotace pro vykreslení zkosené elipsy
                g.RotateTransform(45f);

                // Vykreslí elipsu
                g.FillPath(Brushes.White, Paint.Ellipse(new Point(0, 0), 75, 105));

                // Vytvoří formát řetězce pro psaní na střed
                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;

                // Zapne textový anti-aliasing
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Resetuje rotaci
                g.RotateTransform(-45f);

                // Vykreslí stín pod číslem karty
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(45, 45, 45)))
                    g.DrawString(Number.ToString(), new Font("Segoe UI", 76 - Math.Abs(zoomScale) + zoomMaximum, FontStyle.Bold | FontStyle.Italic), brush, new Point(-3, -3), format);

                // Vykreslí číslo karty
                using (SolidBrush brush = new SolidBrush(Color))
                    g.DrawString(Number.ToString(), new Font("Segoe UI", 76 - Math.Abs(zoomScale) + zoomMaximum, FontStyle.Bold | FontStyle.Italic), brush, new Point(), format);

                // Vykreslí levé horní šíslo
                if (Number == 6)
                    g.DrawString(Number.ToString(), new Font("Segoe UI", 18, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline), Brushes.White, new Point(-76, -105));
                else
                    g.DrawString(Number.ToString(), new Font("Segoe UI", 18, FontStyle.Bold | FontStyle.Italic), Brushes.White, new Point(-76, -105));
                
                // Rotace vzůru nohama
                g.RotateTransform(-180f);

                // Vykreslí pravé dolní číslo
                if (Number == 6)
                    g.DrawString(Number.ToString(), new Font("Segoe UI", 18, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline), Brushes.White, new Point(-76, -105));
                else
                    g.DrawString(Number.ToString(), new Font("Segoe UI", 18, FontStyle.Bold | FontStyle.Italic), Brushes.White, new Point(-76, -105));

                // Vypne anti-aliasing
                g.SmoothingMode = SmoothingMode.Default;

                // Resetuje transformace
                g.ResetTransform();
            }
        }

        /// <summary>
        /// Animace otočení
        /// </summary>
        public void Flip(bool skipAnimation = false)
        {
            // Je karta bříškem dolů?
            if (Flipped)
            {
                // Započne animaci otočení bříškem vzůru
                flipUp = true;

                // Přeskočí animaci, pokud je parameter metody na true
                if (skipAnimation)
                {
                    // Zastaví průběh animace
                    flipUp = false;

                    // Aktualizuje stav karty na normální
                    State = State.Normal;

                    // Nastaví horizontální scale karty na 1
                    flipX = 1;

                    // Karta již není otočená bříškem dolů
                    Flipped = false;
                }
            }
            else
            {
                // Karta není bříškem dolů
                // Započne animaci otočení bříškem dolů
                flipDown = true;

                // Přeskočí animaci, pokud je parameter metody na true
                if (skipAnimation)
                {
                    // Zastaví průběh animace
                    flipDown = false;

                    // Aktualizuje stav karty na otočenou směrem dolů
                    State = State.Back;

                    // Nastaví horizontální scale karty na -1
                    flipX = -1;

                    // Karta již není otočená bříškem dolů
                    Flipped = true;
                }
            }
        }

        /// <summary>
        /// Nastaví úhel karty s animací
        /// </summary>
        public void Settle(int angle)
        {
            // AKtualizuje cílový úhel na předaný úhel
            targetAngle = angle;

            // Započne animaci otočení
            settleRotation = true;
        }

        /// <summary>
        /// Animace zvětšení. Spuštěna při líznutí nové karty
        /// </summary>
        public async void Zoom()
        {
            // Chvíli počká - prodleva přidána, aby se animace spustila v momentně, kdy karta
            // dorazí z hracího balíčku do ruky
            await Task.Delay(400);

            // Spustí animaci zvětšení
            zoom = true;
        }

        /// <summary>
        /// Klik event karty
        /// </summary>
        public void MouseClick()
        {
            // Může hráč hrát? Pokud ne, nemá smysl řešit jeho kliky
            if (!Player.canPlay)
                return;

            // Je karta v hráčově ruce?
            if (Player.hand.Contains(this))
            {
                // Je karta hratelná?
                if (Playable)
                {
                    // Kartu lze zahrát
                    // Removneme kartu z ruky
                    Player.hand.Remove(this);

                    // Přidáme ji na hromádku zahraných karet
                    Pile.cards.Add(this);

                    // Nastavíme náhodnou rotaci - jen pro efekt
                    Settle(new Random().Next(-180, 180));

                    // Refreshnem layout aby se vše posunulo na své nové místo
                    Game.container.RefreshLayout();
                }
                else
                {
                    // Karta není hratelná - vrátíme se zpět a počkáme, až hráč zkusí kliknout na kartu, kterou lze zahrát
                    return;
                }
            }

            // Karta není v hráčově ruce.
            // Je karta v hracím balíčku?
            if (Deck.cards.Contains(this))
            {
                // Ano je - lízneme hráči jednu kartu
                Player.Draw(1);
            }

            // Vypne rámeček hratelnosti - aby dále nebyl vidět na hromádce odehraných karet
            Playable = false;

            // Updatuje kartu
            Update();

            // End player's turn
            Player.End();

            // Pass turn to enemy
            Enemy.Play();
        }
    }
}
