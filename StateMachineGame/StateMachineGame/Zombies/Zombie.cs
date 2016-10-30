using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ZombiesVsSurvivors
{
    abstract class Zombie : Acteur
    {
        // L'état dans lequel se trouve le zombie
        protected EtatsZombies etat;

        protected int pointsDeVieMax;

        // La distance qui sépare le zombie du camp des survivants
        public float DistanceAvecCampSurvivants
        {
            get;
            protected set;
        }

        // La liste de tous les survivants
        public static List<Survivant> Ennemis
        {
            get;
            set;
        }

        

        public Zombie(Texture2D image, Vector2 positionDepart, float rotation, Vector2 limiteHautGauche, Vector2 limiteBasDroite, int vie, float vitesse, int force, Acteur cible, Camp campAdverse, SpriteBatch batch, SpriteFont font) : 
            base(image, positionDepart, rotation, limiteHautGauche, limiteBasDroite, vie, vitesse, force, cible, campAdverse, batch, font)
        {
            etat = new EtatNormal_Z(this);

        }

        public virtual void Update()
        {

            prochaineAttaque--;
            DistanceAvecCampSurvivants = CalculerDistance(position, CampAdverse.Cible);

            
            if(CiblePersonnage!=null)
            {
                // Si le zombie a tué sa cible ou ne la détecte plus, il continue à se diriger vers le camp adverse
                if (CiblePersonnage.PointsdeVie <= 0 || CalculerDistance(position, CiblePersonnage.Position) >= rayonDetection)
                {
                    etat = new EtatNormal_Z(this);
                }

            }
            else
            {
                // Si le zombie n'a pas de cible, il essaie d'en trouver une nouvelle
                Acteur nouvelleCible = TrouverEnnemisAProximite();
                if (nouvelleCible != null)
                {
                    CiblePersonnage = nouvelleCible;
                    etat = new EtatOffensif_Z(this);
                                        
                }
            }

            // Si le zombie a été touché, il clignote rouge 
            if(touche)
            {
                cptFlash++;
            }
            if(cptFlash>=tempsFlashRouge)
            {
                cptFlash = 0;
                couleur = Color.White;
            }


            etat.OnUpdate();
        }


        public void Attaquer()
        {
            // Si le survivant que le zombie suivait est allé se caché, il ne le prend plus comme cible
            if (CiblePersonnage!=null && CiblePersonnage.EstCache)
            {
                etat = new EtatNormal_Z(this);
                return;
            }
                

            if (prochaineAttaque <= 0)
            {
                // Si la cible est un survivant, il l'attaque
                // Sinon, il attaque le camp adverse
                if (CiblePersonnage != null)
                    if (CalculerDistance(position, CiblePersonnage.Position) <= rayonAttaque)
                    {

                        CiblePersonnage.RecevoirDegats(force, this);
                        prochaineAttaque = rythmeAttaque;
                    }
                    else
                        MouvementVersCible(CiblePersonnage.Position);
                else
                {
                    CampAdverse.InfligerDegats(force);
                    prochaineAttaque = rythmeAttaque;
                }
                
            }
        }

        /// <summary>
        /// Trouve le survivant le plus près du zombie
        /// </summary>
        /// <returns>Un survivant</returns>
        private Survivant TrouverEnnemisAProximite()
        {
            for(int i=0;i<Ennemis.Count;i++)
            {
                if (CalculerDistance(position, Ennemis[i].Position) <= rayonDetection && !Ennemis[i].EstCache)
                    return Ennemis[i];
            }
            return null;
        }

        public override void RecevoirDegats(int degats, Acteur ennemi)
        {
            base.RecevoirDegats(degats, this);
            TexteVie = pointsDeVie + "/" + pointsDeVieMax;
            
        }
    }
}
