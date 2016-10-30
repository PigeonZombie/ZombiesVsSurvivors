using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace ZombiesVsSurvivors
{
    /// <summary>
    /// Classe simple qui réunit le minimum afin de pouvoir afficher un sprite à l'écran
    /// </summary>
    /// <remarks>
    /// La classe Acteur hérite de Objet2D
    /// </remarks>
    public class Objet2D
    {
        protected Texture2D image;
        protected Vector2 position;
        protected Vector2 positionTexte;
        protected Vector2 positionVie;
        protected SpriteBatch spriteBatch;
        protected SpriteFont mainFont;
       

        public String TexteVie
        {
            get;
            set;
        }

        public Texture2D Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public Objet2D(Texture2D image, Vector2 position, SpriteBatch spriteBatch, SpriteFont mainFont)
        {
            this.image = image;
            this.position = position;
            this.positionTexte = new Vector2(position.X, position.Y - 30);
            this.positionVie = new Vector2(position.X/10, position.Y + image.Height-10);
            this.spriteBatch = spriteBatch;
            this.mainFont = mainFont;
            TexteVie = "";
        }

        /// <summary>
        /// Dessine la texture et ses points de vie à l'écran
        /// </summary>
        /// <param name="spriteBatch">Batch d'écriture à l'écran en cours, on le passe en 
        /// référence afin de contrôler l'affichage du sprite directement dans la classe</param>
        public virtual void Dessiner(SpriteBatch spriteBatch)
        {
            //Ici la position représente le coin haut à gauche du sprite            
            //Color.White fait en sorte qu'on ne mat aucun filtre de couleur sur notre image
            spriteBatch.Draw(image, position, Color.White);
            spriteBatch.DrawString(mainFont, TexteVie, positionVie, Color.Black);

        }
    }
}
