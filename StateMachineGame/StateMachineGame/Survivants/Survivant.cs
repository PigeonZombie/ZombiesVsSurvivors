using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace ZombiesVsSurvivors
{
    class Survivant : Acteur
    {
        // Le couvert le plus près du survivant
        protected Objet2D couvertPlusProche;
        
        // Variables pour gérer le rythme de guérison des Survivants
        // Les survivants guérissent d'environ 2 points de vie par seconde
        protected int prochaineGuerison = 0;
        private int rythmeGuerison = 30;

        // Indique si le survivant est Alerté par un tank ou non
        public bool Alerte
        {
            get;
            set;
        }

        public int PointsDeVieMax
        {
            get;
            set;
        }

        // L'État dans lequel se trouve le survivant 
        public EtatsSurvivant etat
        {
            get;
            set;
        }

        // La liste de tous les zombies ennemis
        public static List<Zombie> Ennemis
        {
            get;
            set;
        }

        public static Objet2D[] Couverts
        {
            get;
            set;
        }

        // Son de tir de fusil
        public static SoundEffect SonFusil
        {
            get;
            set;
        }

        // Son qui joue quand le joueur est touché
        public static SoundEffect SonBlesse
        {
            get;
            set;
        }

        public Survivant(Texture2D image, Vector2 positionDepart, float rotation, Vector2 limiteHautGauche, Vector2 limiteBasDroite, int vie, float vitesse, int force, Acteur cible, Camp campAdverse, SpriteBatch batch, SpriteFont font):
            base (image, positionDepart, rotation, limiteHautGauche, limiteBasDroite, vie, vitesse, force, cible, campAdverse, batch, font)
        {
            etat = new EtatNormal(this);
            Alerte = false;
        }

        public virtual void Update()
        {
            if (touche)
            {
                cptFlash++;
            }
            if (cptFlash >= tempsFlashRouge)
            {
                cptFlash = 0;
                couleur = Color.White;
            }
        }


        public virtual void AttaquerZombie()
        { }

        /// <summary>
        /// Diminuer les points de vie et changer la couleur de la texture quand
        /// le survivant est touché. Mets à jour l'affichage des points de vie à l'écran,
        /// fait passer le survivant en état Offensif et fait jouer le son de blessure.
        /// </summary>
        /// <param name="degats">Les dommages infligés</param>
        /// <param name="ennemi">L'ennemi qui inflige les dégâts</param>
        public override void RecevoirDegats(int degats, Acteur ennemi)
        {
            base.RecevoirDegats(degats, ennemi);
            TexteVie = pointsDeVie + "/" + PointsDeVieMax;
            CiblePersonnage = ennemi;
            etat = new EtatOffensif(this);
            SymboleEtat = "O";
            SonBlesse.Play();
        }

        /// <summary>
        /// Trouve l'ennemi le plus près d'un point de référence donné
        /// </summary>
        /// <param name="cible">Le point de référence</param>
        /// <returns>Le zombie le plus près du point</returns>
        public virtual Zombie TrouverEnnemiPlusProche(Vector2 cible)
        {
            Zombie zombieLePlusProche = null;
            float distance = Game1.LARGEUR_ECRAN;
            for (int i = 0; i < Ennemis.Count; i++)
            {
                float nouvelleDistance = CalculerDistance(cible, Ennemis[i].Position);
                if (nouvelleDistance < distance && nouvelleDistance <= rayonDetection)
                {
                    distance = nouvelleDistance;
                    zombieLePlusProche = Ennemis[i];
                }
            }
            return zombieLePlusProche;
        }

        /// <summary>
        /// Trouve le couvert le plus proche et l'assigne dans la variable couvertPlusProche
        /// </summary>
        public void TrouverCouvertPlusProche()
        {
            if (CalculerDistance(position, Couverts[0].Position) > CalculerDistance(position, Couverts[1].Position))
                couvertPlusProche = Couverts[1];
            else
                couvertPlusProche = Couverts[0];
        }

        /// <summary>
        /// Déplace le survivant vers le couvert le plus près de lui ou le guérit s'il y est déjà
        /// </summary>
        public virtual void Fuir()
        {
            Vector2 cibleCouvert = new Vector2(couvertPlusProche.Position.X + couvertPlusProche.Image.Width / 2, couvertPlusProche.Position.Y + couvertPlusProche.Image.Height / 2);
            if (CalculerDistance(position, cibleCouvert) <= vitesse)
            {
                Guerir();
                EstCache = true;
            }
            else
                MouvementVersCible(cibleCouvert);
        }

        /// <summary>
        /// Redonne un point de vie au survivant deux fois par seconde et
        /// mets l'affichage des PV à jour 
        /// </summary>
        private void Guerir()
        {
            prochaineGuerison++;

            int i = prochaineGuerison % rythmeGuerison;
            if (prochaineGuerison % rythmeGuerison == 0)
            {
                pointsDeVie++;
                TexteVie = pointsDeVie + "/" + PointsDeVieMax;
                prochaineGuerison = rythmeGuerison;
            }
                
        }

        /// <summary>
        /// Alerte le survivant actuel qu'un tank est proche de son camp et 
        /// change sa cible pour ce tank
        /// </summary>
        /// <param name="tank">The tank.</param>
        public void Alerter(Zombie tank)
        {
            Alerte = true;
            CiblePersonnage = tank;
        }

    }
}
