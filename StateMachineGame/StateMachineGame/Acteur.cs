using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombiesVsSurvivors
{

    /// <summary>
    /// Classe qui représente les acteurs de la partie.  Il est clair que pour le moment, elle regroupe trop de chose.
    /// Les tours ont des points de vie, mais ne bougent jamais.  Il est clair que l'enchainement des classes est à revoir.
    /// </summary>
    /// <seealso cref="ZombiesVsSurvivors.Objet2D" />
    abstract class Acteur : Objet2D
    {

        // Lettre qui indique l'état dans lequel se trouve un personnage
        // N: Normal
        // O: Offensif
        // A: Alerté
        // C: Caché
        // R: Repli
        public String SymboleEtat
        {
            get;
            set;
        }

        /// <summary>
        /// Rotation de l'acteur.  Il est normalement oreienté dans l'angle où il avance
        /// </summary>
        private float rotation;

        /// <summary>
        /// vector de offset.  Le pivot d'un sprite est normalement en haut à gauche.  On veut le ramener au centre.
        /// </summary>        
        private Vector2 centrageDuPivot;


        /// <summary>
        /// Voici deux objets de XNA qui permettent facilment de gérer les collisions.  Regardez les méthodes intersects, 
        /// elles peuvent tester les collisions avec pas mal toutes les formes possibles.  Une boite de collision avec une
        /// hauteur de zéro (axe des z) est une boite de collision et une sphère avec le pivot sur le plan zéro en z verra son rayon
        /// maximal être sur cette axe également, ce qui fait qu'on peut le traiter comme un cercle de collision.
        /// </summary>
        private BoundingBox boiteCollision;
        private BoundingSphere sphereCollision;

        //Boite limite dans lequel un acteur peut évoluer.  Normalement il s'agit de l'écran, mais ça peut être ce que vous voulez.
        protected Vector2 limiteHautGauche;
        protected Vector2 limiteBasDroit;

        /// <summary>
        /// Caractéristiques de l'acteur
        /// </summary> 
        protected int pointsDeVie;
        protected float vitesse;
        protected int force;
        protected int rythmeAttaque;
        protected int rayonAttaque;
        protected int rayonDetection;
        protected int prochaineAttaque;
        protected bool touche;
        protected int tempsFlashRouge = 30;
        protected int cptFlash = 0;

        /// <summary>
        /// Cible vers lequel l'acteur se dirige, ou qu'il va éventuellement attaquer.
        /// </summary>     
        private Acteur cible;

        // Indique si le joueur est dans la zone de régénération 
        public bool EstCache
        {
            get;
            set;
        }

        // Le camp de l'équipe adverse qu'il faut détruire
        protected Camp CampAdverse
        {
            get;
            set;
        }

        // Le propre camp de cette instance d'Acteur
        public static Camp CampAmi
        {
            get;
            set;
        }

        /// <summary>
        /// Filtre de couleur qui se place par dessus le sprite de l'acteur.  Imaginez un papier transparent teinté.
        /// Pratique pour identifier les camps.
        /// </summary>
        protected Color couleur;

        // La rotation de l'acteur
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }
        public Vector2 CentrageDuPivot
        {
            get
            {
                return centrageDuPivot;
            }
            set
            {
                centrageDuPivot = value;
            }
        }
        public BoundingBox BoiteCollision
        {
            get
            {
                return boiteCollision;
            }
            set
            {
                boiteCollision = value;
            }
        }
        public BoundingSphere SphereCollision
        {
            get
            {
                return sphereCollision;
            }
            set
            {
                sphereCollision = value;
            }
        }

        // Le nombre de points de vie actuel de l'Acteur
        public int PointsdeVie
        {
            get
            {
                return pointsDeVie;
            }
            set
            {
                pointsDeVie = value;
            }
        }
        // La vitesse de déplacement de l'Acteur
        public float Vitesse
        {
            get
            {
                return vitesse;
            }
            set
            {
                vitesse = value;
            }
        }
        // La cible ennemie 
        public Acteur CiblePersonnage
        {
            get
            {
                return cible;
            }
            set
            {
                cible = value;
            }
        }


        public Acteur(Texture2D image, Vector2 position, float rotation, Vector2 limiteHautGauche, Vector2 limiteBasDroit, int pointsdeVie, 
            float vitesse, int force, Acteur cible, Camp campAdverse, SpriteBatch batch, SpriteFont font) : base(image, position, batch, font)
        {
            this.rotation = rotation;
            this.centrageDuPivot = new Vector2(image.Width / 2, image.Height / 2);
            this.boiteCollision = new BoundingBox(new Vector3(position.X - centrageDuPivot.X, position.Y - centrageDuPivot.Y, 0), new Vector3(position.X + centrageDuPivot.X, position.Y + centrageDuPivot.Y, 0));
            this.sphereCollision = new BoundingSphere(new Vector3(position.X, position.Y, 0), centrageDuPivot.X);
            this.limiteHautGauche = limiteHautGauche + centrageDuPivot;
            this.limiteBasDroit = limiteBasDroit - centrageDuPivot;
            this.pointsDeVie = pointsdeVie;
            this.vitesse = vitesse;
            this.force = force;
            this.couleur = Color.White;
            this.cible = cible;
            this.CampAdverse = campAdverse;
            prochaineAttaque = rythmeAttaque;
            touche = false;
        }


        /// <summary>
        /// Algorithme de déplacement standard.  L'acteur détecte sa cible et détermine quel est l'angle pour s'orienter vers la cible.
        /// Si la distance entre l'acteur et sa cible est plus petite que la vitesse, on le place sur la cible, sinon il se déplace de 
        /// sa vitesse vers elle.
        /// </summary>
        public void MouvementVersCible(Vector2 cibleActuelle)
        {
            //Recherche de la distance
            float distanceX = cibleActuelle.X - position.X;
            float distanceY = cibleActuelle.Y - position.Y;
            float distance = CalculerDistance(position, cibleActuelle);

            //Si la distance entre l'acteur et sa cible est plus petite que la vitesse, on le place sur la cible
            if (distance < vitesse)
            {
                Deplacement(distanceX, distanceY);
                return;
            }

            //Sinon on calcul l'angle d'orientation de l'acteur vers la cible
            //rotation = (float)Math.Atan((cibleActuelle.Position.Y - position.Y) / (cibleActuelle.Position.X - position.X));
            rotation = (float)Math.Atan((cibleActuelle.Y - position.Y) / (cibleActuelle.X - position.X));

            //Nécessaire car si la clible est plus petite en x, l'acteur fera dos à la cible
            if (cibleActuelle.X < position.X)
            {
                rotation += (float)(Math.PI);
            }

            //On déplace le personnage
            Deplacement(vitesse * (float)Math.Cos(rotation), vitesse * (float)Math.Sin(rotation));
        }

        /// <summary>
        /// Déplacements de l'acteur.  On le fait à l'intérieur du cadre de ses limites (les limites nulles ne sont pas encore codées)
        /// On déplace aussi la boite et la sphère de collision, qui ne suivent pas automatiquement
        /// 
        /// Méthode toujours utilisé par mouvement, mais on la garde publique, au cas où vous voudriez placer directement / téléporter un acteur.
        /// </summary>
        /// <param name="deplacementX">Le déplacement selon l'axe des X</param>
        /// <param name="deplacementY">Le déplacement selon l'axe des Y</param>
        public void Deplacement(float deplacementX, float deplacementY)
        {
            //Déplacement à l'intérieur des limites en X
            if (position.X + deplacementX < limiteHautGauche.X)
            {
                position.X = limiteHautGauche.X;
            }
            else if (position.X + deplacementX > limiteBasDroit.X)
            {
                this.position.X = limiteBasDroit.X;
            }
            else
            {
                this.position.X += deplacementX;
            }

            //Déplacement à l'intérieur des limites en Y
            if (this.position.Y + deplacementY < limiteHautGauche.Y)
            {
                this.position.Y = limiteHautGauche.Y;
            }
            else if (this.position.Y + deplacementY > limiteBasDroit.Y)
            {
                this.position.Y = limiteBasDroit.Y;
            }
            else
            {
                this.position.Y += deplacementY;
            }

            //Déplacement du symbole
            positionTexte = new Vector2(position.X, position.Y-50);

            //Déplacement de la boite de collision
            boiteCollision.Min.X = position.X - centrageDuPivot.X;
            boiteCollision.Min.Y = position.Y - centrageDuPivot.Y;
            boiteCollision.Max.X = position.X + centrageDuPivot.X;
            boiteCollision.Max.Y = position.Y + centrageDuPivot.Y;
       
            //Déplacement de la sphère de collision
            sphereCollision.Center.X = position.X;
            sphereCollision.Center.Y = position.Y;
        }

        public override void Dessiner(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                image,              //L'image à afficher
                position,           //sa position
                null,               //Rectangle pour un affichage partiel: null signifit qu'on affiche tout le sprite
                couleur,            //couleur du filtre.  Blanc veut dire aucun filtre
                rotation,           //rotation de l'image
                centrageDuPivot,    //point de pivot.  0,0 par défaut
                1f,                 //Mise à l'échelle: 1 = taille de base
                SpriteEffects.None, //effets de mirroir
                0                   //Échelles de profondeur.  Si tout est égal, le dernier affiché dans l'ordre du
                );                         //code sera celui "le plus sur le dessus"
           
            positionVie = new Vector2(position.X-image.Width/2, position.Y + image.Height/2);
            spriteBatch.DrawString(mainFont, TexteVie, positionVie, Color.Black);
        }

        /// <summary>
        /// Diminuer les points de vie et changer la couleur de la texture quand 
        /// l'Acteur est touché.
        /// </summary>
        /// <param name="degats">Les dommages infligés</param>
        /// <param name="ennemi">L'ennemi qui inflige les dégâts</param>
        public virtual void RecevoirDegats(int degats, Acteur ennemi)
        {
            pointsDeVie -= degats;
            touche = true;
            this.couleur = Color.Red;
            cptFlash = 0;
        }

        /// <summary>
        /// Calcule la distance entre deux points
        /// </summary>
        /// <param name="a">Le premier point.</param>
        /// <param name="a">Le deuxième point.</param>
        /// <returns></returns>
        protected float CalculerDistance(Vector2 a, Vector2 b)
        {
            float distanceX = a.X - b.X;
            float distanceY = a.Y - b.Y;
            float distance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            return distance;
        }


        /// <summary>
        /// Se déplacer vers le camp adverse ou l'attaquer si l'Acteur est assez près
        /// </summary>
        public virtual void AttaquerCampEnnemi()
        {
            if (CalculerDistance(position, CampAdverse.Cible) <= rayonAttaque)
            {
                if (prochaineAttaque <= 0)
                {
                    prochaineAttaque = rythmeAttaque;
                    CampAdverse.InfligerDegats(force);
                }
            }
            else
                MouvementVersCible(CampAdverse.Cible);
        }

        /// <summary>
        /// Modifier la rotation de l'acteur pour faire face à une cible
        /// </summary>
        /// <param name="cible">La position de la cible</param>
        public void AjusterRotation(Vector2 cible)
        {
            rotation = (float)Math.Atan((cible.Y - position.Y) / (cible.X - position.X));

            //Nécessaire car si la clible est plus petite en x, l'acteur fera dos à la cible
            if (cible.X < position.X)
            {
                rotation += (float)(Math.PI);
            }
        }

       
        
    }
}
