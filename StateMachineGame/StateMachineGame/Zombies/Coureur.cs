using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombiesVsSurvivors
{
    class Coureur : Zombie
    {

        /// <summary>
        /// Statistiques propres au Coureur
        /// </summary>
        private const int VIE_MAX_COUREUR = 5;
        private const int FORCE_COUREUR = 2;
        private const float VITESSE_COUREUR = 1.5f;
        private const int RYTHME_ATTAQUE_COUREUR = 20;
        private const int RAYON_DETECTION_COUREUR = 150;
        private const int RAYON_ATTAQUE_COUREUR = 35;
        public static Texture2D TEXTURE
        {
            get;
            set;
        }

        public Coureur(Vector2 positionDepart, float rotation, Vector2 limiteHautGauche, Vector2 limiteBasDroite, Acteur cible, Camp campAdverse, SpriteBatch batch, SpriteFont font):
            base(TEXTURE, positionDepart, rotation, limiteHautGauche, limiteBasDroite, VIE_MAX_COUREUR, VITESSE_COUREUR, FORCE_COUREUR, cible, campAdverse, batch, font)
        {
            rythmeAttaque = RYTHME_ATTAQUE_COUREUR;
            TexteVie = VIE_MAX_COUREUR + "/" + VIE_MAX_COUREUR;
            pointsDeVieMax = VIE_MAX_COUREUR;
            rayonDetection = RAYON_DETECTION_COUREUR;
            rayonAttaque = RAYON_ATTAQUE_COUREUR;
        }


    }
}
