using UNO.Cards;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UNO;

namespace UNO
{
    /// <summary>
    /// Main game class
    /// </summary>
    public partial class Game : Form
    {
        /// <summary>
        /// Game container (form)
        /// </summary>
        public static Game container = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public Game()
        {
            // Initialize form's controls
            InitializeComponent();

            // Assign game container
            container = this;

            // Turn on DoubleBuffer to prevent flickering
            DoubleBuffered = true;

            // Start in full screen
            WindowState = FormWindowState.Maximized;

            // Initialize timer
            mainTimer.Start();

            // Create new deck
            Deck.Create();

            // Align deck
            Deck.Align(this);

            // Creates new pile
            Pile.Create();

            // Align pile
            Pile.Align(this);

            // Align enemy hand
            Enemy.AlignHand(this);

            // Initializes enemy
            // Enemy.Init();

            // Setup game
            // Draw 7 cards for player
            Player.Draw(7);

            // Draw 7 cards for enemy
            Enemy.Draw(7);

            // Předá kolo hráči
            Player.Play();
        }

        /// <summary>
        /// Update method / timer tick
        /// </summary>
        private void Update(object sender, EventArgs e)
        {
            // Updates player
            Player.Update();

            // Update enemy
            Enemy.Update();

            // Updates deck
            Deck.Update();

            // Updates pile
            Pile.Update();

            // Redraw whole scene
            Invalidate();
        }

        /// <summary>
        /// Draw method. Paints whole game scene
        /// </summary>
        private void Draw(object sender, PaintEventArgs e)
        {
            // Draw player's stuff
            Player.Draw(this, e.Graphics);

            // Draw enemy
            Enemy.Draw(this, e.Graphics);

            // Draw deck
            Deck.Draw(e.Graphics);

            // Draw pile
            Pile.Draw(e.Graphics);
        }

        /// <summary>
        /// Form's click event
        /// </summary>
        private void OnClick(object sender, EventArgs e)
        {
            // Handles players click actions
            Player.Click(this);

            // Handles deck click actions
            Deck.Click(this);
        }

        /// <summary>
        /// Form's resize event
        /// </summary>
        private void OnResize(object sender, EventArgs e)
        {
            // Align player's hand
            Player.AlignHand(this);

            // Align deck
            Deck.Align(this);

            // Align pile
            Pile.Align(this);

            // Align enemy hand
            Enemy.AlignHand(this);
        }

        /// <summary>
        /// Calls form's resize event
        /// </summary>
        public void RefreshLayout()
        {
           OnResize(EventArgs.Empty);
        }
    }
}
