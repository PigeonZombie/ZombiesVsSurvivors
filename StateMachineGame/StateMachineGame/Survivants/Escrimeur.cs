using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombiesVsSurvivors
{
    class Escrimeur : Survivant
    {
        /// <summary>
        /// Statistiques propres à l'Escrimeur
        /// </summary>
        private const int VIE_MAX_ESCRIMEUR = 35;
        private const float VITESSE_ESCRIMEUR = 3;
        private const int FORCE_ESCRIMEUR = 5;
        private const int RYTHME_ATTAQUE_ESCRIMEUR = 120;
        private const int RAYON_ATTAQUE_ESCRIMEUR = 40;
        private const int RAYON_DETECTION_ESCRIMEUR = 100;


        public static Texture2D TEXTURE
        {
            get;
            set;
        }

        public Escrimeur(Vector2 positionDepart, float rotation, Vector2 limiteHautGauche, Vector2 limiteBasDroite, Acteur cible, Camp campAdverse, SpriteBatch batch, SpriteFont font):
            base(TEXTURE, positionDepart, rotation, limiteHautGauche, limiteBasDroite, VIE_MAX_ESCRIMEUR, VITESSE_ESCRIMEUR, FORCE_ESCRIMEUR, cible, campAdverse, batch, font)
        {
            rythmeAttaque = RYTHME_ATTAQUE_ESCRIMEUR;
            rayonAttaque = RAYON_ATTAQUE_ESCRIMEUR;
            rayonDetection = RAYON_DETECTION_ESCRIMEUR;
            PointsDeVieMax = VIE_MAX_ESCRIMEUR;
            TexteVie = VIE_MAX_ESCRIMEUR + "/" + VIE_MAX_ESCRIMEUR;
        }


        public override void Update()
        {   
            prochaineAttaque--;

            // Priorité no2: Se replier quand il y a trop de zombies près de la base
            if(Game1.NbZombiesCampSurvivants>=5 && !(etat is EtatRepli))
            {
                etat = new EtatRepli(this);
            }

            else if(CiblePersonnage == null && (etat is EtatNormal))
            {
                CiblePersonnage = TrouverEnnemiPlusProche(position);
                // Attaquer une cible 
                if (CiblePersonnage != null)
                {
                    etat = new EtatOffensif(this);
                }
                // Par défaut: Attaquer le camp ennemi
                else
                {
                    etat = new EtatNormal(this);
                }
                                  
            }
            // Priorité no1: Aller se cacher si <25% de point de vie
            if (pointsDeVie <= VIE_MAX_ESCRIMEUR / 4 && !(etat is EtatCache))
            {
                //TrouverCouvertPlusProche();
                //CiblePersonnage = null;
                etat = new EtatCache(this);
            }

            base.Update();

            etat.OnUpdate();
        }

        /// <summary>
        /// Suis sa cible et l'attaque tant qu'elle est en vie
        /// </summary>
        public override void AttaquerZombie()
        {
            if (CalculerDistance(position, CiblePersonnage.Position) <= rayonAttaque)
            {
                if (prochaineAttaque <= 0)
                {
                    CiblePersonnage.RecevoirDegats(force, this);
                    prochaineAttaque = rythmeAttaque;
                }
                AjusterRotation(CiblePersonnage.Position);
            }
            else
                MouvementVersCible(CiblePersonnage.Position);
        }



    }
}
