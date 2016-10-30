using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombiesVsSurvivors
{
    class Sniper : Survivant
    {
        /// <summary>
        /// Statistiques propres au Sniper
        /// </summary>
        private const int VIE_MAX_SNIPER = 16;
        private const float VITESSE_SNIPER = 1.5f;
        private const int FORCE_SNIPER = 8;
        private const int RYTHME_ATTAQUE_SNIPER = 240;
        private const int RAYON_ATTAQUE_SNIPER = 600;
        private Vector2 positionDepart;

        public static Texture2D TEXTURE
        {
            get;
            set;
        }

        public Sniper(Vector2 positionDepart, float rotation, Vector2 limiteHautGauche, Vector2 limiteBasDroite, Acteur cible, Camp campAdverse, SpriteBatch batch, SpriteFont font):
            base(TEXTURE, positionDepart, rotation, limiteHautGauche, limiteBasDroite, VIE_MAX_SNIPER, VITESSE_SNIPER, FORCE_SNIPER, cible, campAdverse, batch, font)
        {
            rythmeAttaque = RYTHME_ATTAQUE_SNIPER;
            rayonAttaque = RAYON_ATTAQUE_SNIPER;
            rayonDetection = rayonAttaque;
            PointsDeVieMax = VIE_MAX_SNIPER;
            TexteVie = VIE_MAX_SNIPER + "/" + VIE_MAX_SNIPER;
            this.positionDepart = positionDepart;
        }

        public override void Update()
        {
            prochaineAttaque--;


           // Priorité no2: Attaquer le tank près de la base s'il y en a un
            if (Alerte && !(etat is EtatAlerte))
            {
                etat = new EtatAlerte(this);
            }
            
            else if(!Alerte)
            {
                // Si le sniper n'a plus de cible et qu'il est dans son nid, il choisi 
                // comme nouvelle cible l'ennemi le plus près de la base
                if (CiblePersonnage == null && positionDepart == Position)
                {
                    CiblePersonnage = TrouverEnnemiPlusProche(CampAmi.Cible);
                    if (CiblePersonnage != null)
                    {
                        etat = new EtatOffensif(this);
                    }
                }
            }
            // Priorité no1: Aller se cacher si <25% de point de vie
            if(pointsDeVie <= PointsDeVieMax/4)
            {
                //TrouverCouvertPlusProche();
                etat = new EtatCache(this);
                //CiblePersonnage = null;
            }

            base.Update();

            etat.OnUpdate();
        }

        public override void AttaquerZombie()
        {
            if (CalculerDistance(position, CiblePersonnage.Position) <= rayonAttaque)
            {
                if (prochaineAttaque <= 0)
                {
                    CiblePersonnage.RecevoirDegats(force, this);
                    prochaineAttaque = rythmeAttaque;
                    SonFusil.Play();
                }
                AjusterRotation(CiblePersonnage.Position);
            }


        }

        public override void AttaquerCampEnnemi()
        {
            // Les snipers ne sortent pas de leur nid à moins de devoir se soigner
            if (Position != positionDepart)
                MouvementVersCible(positionDepart);
        }

    }
}
