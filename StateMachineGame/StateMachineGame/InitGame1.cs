using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ZombiesVsSurvivors
{
    /// <summary>
    /// L'équivalent du main pour un jeu XNA
    /// 
    /// Normalement, toutes les classes "systèmes" de XNA sont dans un seul et même fichier
    /// Mais j'aime bien séparer tout ce qui a trait à l'initialisation du jeu de la 
    /// boucle de jeu
    /// </summary>
    public partial class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont mainFont;
        SoundEffect sonZombie;
        SoundEffect sonSurvivant;

        //Modifier ces constantes pour changer la résolution de votre fenêtre de jeu
        //ceci dit, par soucis de rapidité et de simplicité, les sprites ont été pensés pour cette résolution
        public const int LARGEUR_ECRAN = 1280;
        const int HAUTEUR_ECRAN = 720;

        //Les images de fond, les couverts, les nids de sniper et les boutons
        Objet2D champDeBataille;
        Objet2D[] couverts = new Objet2D[2];
        Objet2D[] nidsSniper = new Objet2D[2];
        Bouton[] boutons = new Bouton[3];

        List<Zombie> listeZombies = new List<Zombie>();
        List<Survivant> listeSurvivants = new List<Survivant>();
        
        //Chaque texture devrait être chargée une seule fois par contre, comme des personnages sont crées 
        //après le début de la partie, avoir une référence globale, facile d'accès sera pratique pour 
        //créer les personnages au besoin
        Texture2D imageZombieCoureur;
        Texture2D imageZombieTank;
        Texture2D imageSniper;
        Texture2D imageSoldat;
        Texture2D imageEscrimeur;

        Camp campZombie;
        Camp campSurvivant;

        string equipeGagnante = "";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InitGraphicsMode(LARGEUR_ECRAN, HAUTEUR_ECRAN, false);
            this.IsMouseVisible = true;
            base.Initialize();
        }

        private bool InitGraphicsMode(int width, int height, bool fullScreen)
        {
            // Si on est pas en plein écran, la taille de la fenêtre peut
            // être de n'importe qulle taille plus petite que la surface de l'écran
            if (fullScreen == false)
            {
                if ((width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    graphics.PreferredBackBufferWidth = width;
                    graphics.PreferredBackBufferHeight = height;
                    graphics.IsFullScreen = fullScreen;
                    graphics.ApplyChanges();
                    return true;
                }
            }
            else
            {
                //En plein écran il faut que la résolution qu'on esssai de prendre soit supportée
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    //Si le format est supporté, on fait l'ajustement à l'écran et on retourne vrai.
                    if ((dm.Width == width) && (dm.Height == height))
                    {
                        graphics.PreferredBackBufferWidth = width;
                        graphics.PreferredBackBufferHeight = height;
                        graphics.IsFullScreen = fullScreen;
                        graphics.ApplyChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Changement de la font, pour l'écriture
            mainFont = Content.Load<SpriteFont>("Fonts\\FontPrincipale");

            //Champ de bataille: le fond
            champDeBataille = new Objet2D(Content.Load<Texture2D>("Sprites\\background"), Vector2.Zero, spriteBatch, mainFont);

            //Une texture pour les deux zones de couvert
            Texture2D imageCouvert = Content.Load<Texture2D>("Sprites\\couvert");
            couverts[0] = new Objet2D(imageCouvert, new Vector2(LARGEUR_ECRAN - imageCouvert.Width - 450, 100), spriteBatch, mainFont);
            couverts[1] = new Objet2D(imageCouvert, new Vector2(LARGEUR_ECRAN - imageCouvert.Width - 450, HAUTEUR_ECRAN - imageCouvert.Height - 100), spriteBatch, mainFont);

            // Une texture pour les deux nids de sniper
            Texture2D imageNid = Content.Load<Texture2D>("Sprites\\nid_sniper");
            nidsSniper[0] = new Objet2D(imageNid, new Vector2(LARGEUR_ECRAN - imageNid.Width - 300, 150), spriteBatch, mainFont);
            nidsSniper[1] = new Objet2D(imageNid, new Vector2(LARGEUR_ECRAN - imageNid.Width - 300, HAUTEUR_ECRAN-imageNid.Height-150), spriteBatch, mainFont);
            

            // Deux textures différentes pour les camps
            Texture2D imageCampZombies = Content.Load<Texture2D>("Sprites\\lab");
            Texture2D imageCampSurvivants = Content.Load<Texture2D>("Sprites\\camp");
            campSurvivant = new Camp(imageCampSurvivants, new Vector2(LARGEUR_ECRAN - imageCampSurvivants.Width, HAUTEUR_ECRAN / 3), new Vector2(LARGEUR_ECRAN - imageCampSurvivants.Width -20, HAUTEUR_ECRAN / 2), spriteBatch, mainFont);
            campZombie = new Camp(imageCampZombies, new Vector2(0, HAUTEUR_ECRAN / 3f), new Vector2(imageCampZombies.Width+20, HAUTEUR_ECRAN/2), spriteBatch, mainFont);
            

            // Textures de toutes les sortes de personnages
            imageZombieCoureur = Content.Load<Texture2D>("Sprites\\coureur");
            Coureur.TEXTURE = imageZombieCoureur;
            imageZombieTank = Content.Load<Texture2D>("Sprites\\tank");
            Tank.TEXTURE = imageZombieTank;
            imageSniper = Content.Load<Texture2D>("Sprites\\survivor_rifle");
            Sniper.TEXTURE = imageSniper;
            imageSoldat = Content.Load<Texture2D>("Sprites\\soldat");
            Soldat.TEXTURE = imageSoldat;
            imageEscrimeur = Content.Load<Texture2D>("Sprites\\escrimeur");
            Escrimeur.TEXTURE = imageEscrimeur;

            // Textures pour les boutons de l'interface
            Texture2D iconeSniper = Content.Load<Texture2D>("Sprites\\icone_sniper");
            Texture2D iconeSoldat = Content.Load<Texture2D>("Sprites\\icone_soldat");
            Texture2D iconeEscrimeur = Content.Load<Texture2D>("Sprites\\icone_escrimeur");
            boutons[0] = new Bouton(iconeSniper, new Vector2(campSurvivant.Position.X, campSurvivant.Position.Y - 60), Bouton.TypeBouton.Sniper);
            boutons[1] = new Bouton(iconeSoldat, new Vector2((float)(campSurvivant.Position.X + iconeSoldat.Width*1.5f), campSurvivant.Position.Y-60), Bouton.TypeBouton.Soldat);
            boutons[2] = new Bouton(iconeEscrimeur, new Vector2(campSurvivant.Position.X + iconeSoldat.Width*3, campSurvivant.Position.Y - 60), Bouton.TypeBouton.Escrimeur);


            // Chargement des sons des personnages
            sonZombie = Content.Load<SoundEffect>("Audio\\ZombieDeath");
            sonSurvivant = Content.Load<SoundEffect>("Audio\\SurvivorDeath");
            Survivant.SonFusil = Content.Load<SoundEffect>("Audio\\GunShot");
            Survivant.SonBlesse = Content.Load<SoundEffect>("Audio\\SurvivorHit");

            NbZombiesCampSurvivants = 0;
            RayonDefenseCampSurvivant = 50;

            GenererPersonnagesDepart();            
        }

        /// <summary>
        /// Génère la vague initiale de personnages : 4 survivants aléatoires, 6 zombies coureurs
        /// </summary>
        private void GenererPersonnagesDepart()
        {
            Zombie.Ennemis = listeSurvivants;
            Zombie.CampAmi = campZombie;
            Survivant.Ennemis = listeZombies;
            Survivant.Couverts = couverts;
            Survivant.CampAmi = campSurvivant;

            Random r =  new Random();

            int typeSurvivant;

            for (int i = 0; i < 4; i++)
            {
                typeSurvivant = r.Next(3);
                if (typeSurvivant == 0)
                {
                    GenererTypeSurvivant(0);
                    
                }
                else if (typeSurvivant == 1)
                {
                    GenererTypeSurvivant(1);
                    
                }
                else
                {
                    GenererTypeSurvivant(2);
                    
                }
               
            }

            for(int i=0;i<6;i++)
            {
                GenererTypeZombie(0);
            }


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            imageZombieCoureur.Dispose();
            imageZombieTank.Dispose();
            imageSoldat.Dispose();
            imageSniper.Dispose();
            imageEscrimeur.Dispose();
            
        } 


    }
}
