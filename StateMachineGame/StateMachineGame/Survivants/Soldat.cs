using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombiesVsSurvivors
{
    class Soldat : Survivant
    {    
        /// <summary>
        /// Statistiques propres au Soldat
        /// </summary>
        private const int VIE_MAX_SOLDAT = 22;
        private const float VITESSE_SOLDAT = 2f;
        private const int FORCE_SOLDAT = 3;
        private const int RYTHME_ATTAQUE_SOLDAT = 60;
        private const int RAYON_ATTAQUE_SOLDAT = 200;
        private const int RAYON_DETECTION_SOLDAT = 200;

        public static Texture2D TEXTURE
        {
            get;
            set;
        }

        public Soldat(Vector2 positionDepart, float rotation, Vector2 limiteHautGauche, Vector2 limiteBasDroite, Acteur cible, Camp campAdverse, SpriteBatch batch, SpriteFont font):
            base(TEXTURE, positionDepart, rotation, limiteHautGauche, limiteBasDroite, VIE_MAX_SOLDAT, VITESSE_SOLDAT, FORCE_SOLDAT, cible, campAdverse, batch, font)
        {
            rythmeAttaque = RYTHME_ATTAQUE_SOLDAT;
            TexteVie = VIE_MAX_SOLDAT + "/" + VIE_MAX_SOLDAT;
            rayonAttaque = RAYON_ATTAQUE_SOLDAT;
            rayonDetection = RAYON_DETECTION_SOLDAT;
            PointsDeVieMax = VIE_MAX_SOLDAT;
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
                // Toujours chercher la cible la plus proche si le soldat n'est pas en train de se regénérer
                if (!(etat is EtatCache))
                    CiblePersonnage = TrouverEnnemiPlusProche(position);

                // Attaquer la cible si ce n'est pas déjà le cas
                if (CiblePersonnage != null && !(etat is EtatOffensif))
                {
                    etat = new EtatOffensif(this);
                }
            }
            // Priorité no1: Aller se cacher si <25% de point de vie
            if (pointsDeVie <= VIE_MAX_SOLDAT / 4 && !(etat is EtatCache))
            {
                //TrouverCouvertPlusProche();
                //CiblePersonnage = null;
                etat = new EtatCache(this);
            }

            base.Update();

            etat.OnUpdate();
        }


        public override void AttaquerZombie()
        {
            // Attaquer la cible si elle est dans le rayon d'attaque
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
            // Sinon, si le soldat est en état d'Alerte, se diriger vers la cible quand même
            else if (Alerte)
                MouvementVersCible(CiblePersonnage.Position);
            // Revenir à l'état Normal si aucun zombie à proximité et qu'il n'y a pas d'alerte
            else
            {
                etat = new EtatNormal(this);
                SymboleEtat = "N";
                CiblePersonnage = null;
            } 

        }
    }
}
