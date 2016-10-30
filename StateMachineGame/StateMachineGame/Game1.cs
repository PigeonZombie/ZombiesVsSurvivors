using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// This is the main type for your game
    /// </summary>
    public partial class Game1 : Microsoft.Xna.Framework.Game
    {
        bool partieTerminee = false;

        // La position pour le texte à afficher quand la partie est terminée
        Vector2 positionTexte = new Vector2((LARGEUR_ECRAN / 2)-100, 20);
        
        // Variables pour gérer le rythme de génération des personnages et le 
        // nombre maximal de ceux-ci
        int rythmeGenerationZombie = 60;
        int prochaineGenerationZombie = 0;
        int rythmeGenerationSurvivants = 240;
        int prochaineGenerationSurvivant = 0;
        int maxSurvivants = 9;
        int cptSurvivants = 4;
        int maxZombies = 15;
        int cptZombies = 6;

        // Indique quel type de personnage le joueur veux faire apparaître
        Bouton.TypeBouton typeSurvivantAGenerer;
        bool choixFait;



        Random r = new Random();

        // Indique le nombre de zombies qui sont proche du camp de survivant (à l'intérieur
        // de son rayon de défense)
        public static int NbZombiesCampSurvivants
        {
            get;
            set;
        }
        // La distance à partir de laquelle on considère qu'un zombie est dans la zone
        // du camp de survivants
        public static int RayonDefenseCampSurvivant
        {
            get;
            private set;
        }

        /// <summary>
        /// À la base, les inputs dans XNA sont gérés au début de la méthode input
        /// Mais juste pour respecter parfaitement le cycle entrées/traitement/affichage
        /// on ajoute cette méthode
        /// </summary>    
        private void ManageInputs()
        {
            GamePadState padOneState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape) || (padOneState.Buttons.Back == ButtonState.Pressed))
            {
                this.Exit();
            }

            MouseState mouseState = Mouse.GetState();
            if(mouseState.LeftButton == ButtonState.Pressed)
            {
                Point coord = new Point(mouseState.X, mouseState.Y);
                for(int i=0; i<boutons.Length;i++)
                {
                    if(boutons[i].OnClick(coord))
                    {
                        typeSurvivantAGenerer = boutons[i].Type;
                        for (int j = 0; j < boutons.Length; j++)
                        {
                            boutons[j].Actif = false;
                        }
                        choixFait = true;
                        break;
                    }
                }

            }
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            if (!partieTerminee)
            {
                ManageInputs();

                GenererZombies();

                GenererSurvivant();

                RetirerMorts();

                CompterZombiesDansZoneSurvivants();

                for (int i = 0; i < listeZombies.Count; i++)
                {
                    listeZombies[i].Update();
                }

                for (int i = 0; i < listeSurvivants.Count;i++ )
                {
                    listeSurvivants[i].Update();
                }


                if (campSurvivant.PointsVie <= 0)
                {
                    partieTerminee = true;
                    equipeGagnante = "Les zombies gagnent!";
                }
                else if (campZombie.PointsVie <= 0)
                {
                    partieTerminee = true;
                    if (equipeGagnante != "")
                        equipeGagnante = "Égalité!";
                    else
                        equipeGagnante = "Les survivants gagnent!";
                }

                base.Update(gameTime);
            }


        }

        /// <summary>
        /// Génère un nouveau survivant en se basant sur le choix du joueur
        /// </summary>
        private void GenererSurvivant()
        {
            prochaineGenerationSurvivant++;

            if (prochaineGenerationSurvivant >= rythmeGenerationSurvivants && cptSurvivants < maxSurvivants)
            {
                for (int i = 0; i < boutons.Length; i++)
                    boutons[i].Actif = true;
                    if (choixFait)
                    {
                        cptSurvivants++;
                        prochaineGenerationSurvivant = 0;

                        switch (typeSurvivantAGenerer)
                        {
                            case Bouton.TypeBouton.Sniper:
                                GenererTypeSurvivant(0);
                                break;

                            case Bouton.TypeBouton.Soldat:
                                GenererTypeSurvivant(1);
                                break;

                            case Bouton.TypeBouton.Escrimeur:
                                GenererTypeSurvivant(2);
                                break;
                        }

                        if(cptSurvivants==maxSurvivants)
                            for (int i = 0; i < boutons.Length; i++)
                                boutons[i].Actif = false;
                        choixFait = false;
                    }

            }

        }

        /// <summary>
        /// Génère un zombie de manière aléatoire. 
        /// Un coureur a 90% de chances d'être généré, un tank 10%. 
        /// </summary>
        private void GenererZombies()
        {
            prochaineGenerationZombie++;

            if ((prochaineGenerationZombie >= rythmeGenerationZombie && cptZombies<maxZombies) || listeZombies.Count<=5)
            {
                cptZombies++;
                int prochainType = r.Next(10);
                if (prochainType <= 8)
                    GenererTypeZombie(0);
                
                else
                    GenererTypeZombie(1);
                    
                prochaineGenerationZombie = 0;
            }

        }

        /// <summary>
        /// Enlève tous les Survivants et Zombies morts de leur liste respective.
        /// </summary>
        private void RetirerMorts()
        {
            for (int i = 0; i < listeZombies.Count; i++)
                if (listeZombies[i].PointsdeVie <= 0)
                {
                    listeZombies.RemoveAt(i);
                    cptZombies--;
                    sonZombie.Play();
                }

            for (int i = 0; i < listeSurvivants.Count; i++)
                if (listeSurvivants[i].PointsdeVie <= 0)
                {
                    listeSurvivants.RemoveAt(i);
                    cptSurvivants--;
                    sonSurvivant.Play();
                }
                    
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        ///<remarks>
        /// Remarquez que chaque objet 2D du projet a la responsabilité de se dessiner soit même
        /// à l'aide du spritebatch.  Ce qui est dessiné à la fin est dessiné par dessus ce qui fut dessiné avant
        /// </remarks>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LawnGreen);

            spriteBatch.Begin();

            champDeBataille.Dessiner(spriteBatch);

            for (int i = 0; i < boutons.Length;i++ )
                boutons[i].Dessiner(spriteBatch);


            couverts[0].Dessiner(spriteBatch);
            couverts[1].Dessiner(spriteBatch);

            nidsSniper[0].Dessiner(spriteBatch);
            nidsSniper[1].Dessiner(spriteBatch);

            for (int i = 0; i < listeZombies.Count;i++ )
            {
                listeZombies[i].Dessiner(spriteBatch);
            }

            for (int i = 0; i < listeSurvivants.Count; i++)
            {
                listeSurvivants[i].Dessiner(spriteBatch);
            }

            campSurvivant.Dessiner(spriteBatch);

            campZombie.Dessiner(spriteBatch);



            if(partieTerminee)
                spriteBatch.DrawString(mainFont, equipeGagnante, positionTexte, Color.Black);


            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        public void CompterZombiesDansZoneSurvivants()
        {
            NbZombiesCampSurvivants=0;
            for(int i=0; i<listeZombies.Count; i++)
            {
                if (listeZombies[i].DistanceAvecCampSurvivants <= RayonDefenseCampSurvivant)
                    NbZombiesCampSurvivants++;
            }
        }

        public void GenererTypeSurvivant(int type)
        {
            if(type==0)
            {
                int nidDepart = r.Next(2);
                int positionX = (int)nidsSniper[nidDepart].Position.X + imageSniper.Width;
                int positionY = (int)nidsSniper[nidDepart].Position.Y + imageSniper.Height;
                listeSurvivants.Add(FabriqueSurvivant.CreerSurvivant(FabriqueSurvivant.typeSurvivant.Sniper, 
                    new Vector2(positionX, positionY), 0, new Vector2(imageSniper.Width / 2, imageSniper.Height / 2), 
                    new Vector2(LARGEUR_ECRAN - imageSniper.Width / 2, HAUTEUR_ECRAN - imageSniper.Height / 2), null, 
                    campZombie, spriteBatch, mainFont));
            }
            else if(type == 1)
            {
                int positionX = r.Next(LARGEUR_ECRAN / 2, (int)(campSurvivant.Position.X - imageSoldat.Width));
                int positionY = r.Next(imageSoldat.Height, LARGEUR_ECRAN - imageSoldat.Height);
                listeSurvivants.Add(FabriqueSurvivant.CreerSurvivant(FabriqueSurvivant.typeSurvivant.Soldat, 
                    new Vector2(positionX, positionY), 0, new Vector2(imageSoldat.Width / 2, imageSoldat.Height / 2), 
                    new Vector2(LARGEUR_ECRAN - imageSoldat.Width / 2, HAUTEUR_ECRAN - imageSoldat.Height / 2), null, 
                    campZombie, spriteBatch, mainFont));
            }
            else if(type == 2)
            {
                int positionX = r.Next(LARGEUR_ECRAN / 2, (int)(campSurvivant.Position.X - imageEscrimeur.Width));
                int positionY = r.Next(imageEscrimeur.Height, LARGEUR_ECRAN - imageEscrimeur.Height);
                listeSurvivants.Add(FabriqueSurvivant.CreerSurvivant(FabriqueSurvivant.typeSurvivant.Escrimeur, 
                    new Vector2(positionX, positionY), 0, new Vector2(imageEscrimeur.Width / 2, imageEscrimeur.Height / 2), 
                    new Vector2(LARGEUR_ECRAN - imageEscrimeur.Width / 2, HAUTEUR_ECRAN - imageEscrimeur.Height / 2), null, 
                    campZombie, spriteBatch, mainFont));
            }
        }

        public void GenererTypeZombie(int type)
        {
            int xPosition = r.Next(0, (LARGEUR_ECRAN / 2)-imageZombieCoureur.Width);
            int yPosition = r.Next(0, HAUTEUR_ECRAN - imageZombieCoureur.Height);
            if(type==0)
            {             
                listeZombies.Add(FabriqueZombie.CreerZombie(FabriqueZombie.typeZombie.Coureur, new Vector2(xPosition, yPosition), 
                    0, new Vector2(imageZombieCoureur.Width / 2, imageZombieCoureur.Height / 2), 
                    new Vector2(LARGEUR_ECRAN - imageZombieCoureur.Width / 2, HAUTEUR_ECRAN - imageZombieCoureur.Height / 2), null, 
                    campSurvivant, spriteBatch, mainFont));
            }
            else
                listeZombies.Add(FabriqueZombie.CreerZombie(FabriqueZombie.typeZombie.Tank, new Vector2(xPosition,yPosition),
                    0, new Vector2(imageZombieTank.Width / 2, imageZombieTank.Height / 2), 
                    new Vector2(LARGEUR_ECRAN - imageZombieTank.Width / 2, HAUTEUR_ECRAN - imageZombieTank.Height / 2), null, 
                    campSurvivant, spriteBatch, mainFont));
        }
    }
}
