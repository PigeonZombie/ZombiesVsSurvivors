using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombiesVsSurvivors
{
    class Tank : Zombie
    {
        /// <summary>
        /// Statistiques propres au Tank
        /// </summary>
        private const int VIE_MAX_TANK = 70;
        private const int VITESSE_TANK = 1;
        private const int FORCE_TANK = 5;
        private const int RYTHME_ATTAQUE_TANK = 120;
        private const int RAYON_DETECTION_TANK = 150;
        private const int RAYON_ATTAQUE_TANK = 40;
        public static Texture2D TEXTURE
        {
            get;
            set;
        }

        public Tank(Vector2 positionDepart, float rotation, Vector2 limiteHautGauche, Vector2 limiteBasDroite, Acteur cible, Camp campAdverse, SpriteBatch batch, SpriteFont font):
            base(TEXTURE, positionDepart, rotation, limiteHautGauche, limiteBasDroite, VIE_MAX_TANK, VITESSE_TANK, FORCE_TANK, cible, campAdverse, batch, font)
        {
            rythmeAttaque = RYTHME_ATTAQUE_TANK;
            TexteVie = VIE_MAX_TANK + "/" + VIE_MAX_TANK;
            pointsDeVieMax = VIE_MAX_TANK;
            rayonAttaque = RAYON_ATTAQUE_TANK;
            rayonDetection = RAYON_DETECTION_TANK;
            
        }

        public override void Update()
        {
            // Par défaut, le tank ne fait que se diriger vers la base adverse pour l'attaquer

            prochaineAttaque--;
            DistanceAvecCampSurvivants = CalculerDistance(position, CampAdverse.Cible);

            // Si le tank est proche du camp de survivants, tous ces derniers sont alertés et
            // le tank devient bleu
            if (DistanceAvecCampSurvivants <= 200)
            {
                for(int i=0;i<Ennemis.Count;i++)
                {
                    if(!Ennemis[i].Alerte)
                        Ennemis[i].Alerter(this);
                }
                this.couleur = Color.Blue;
              
            }

            /// <summary>
            /// Statistiques propres au Coureur
            /// </summary>
            if (touche)
            {
                cptFlash++;
            }
            if (cptFlash >= tempsFlashRouge)
            {
                cptFlash = 0;
                couleur = Color.White;
            }

            etat.OnUpdate();
        }




    }
}
