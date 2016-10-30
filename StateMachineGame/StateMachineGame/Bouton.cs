using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombiesVsSurvivors
{
    public class Bouton
    {
        // Le rectangle qui représente le bouton, sa position et sa texture
        // Les dimensions du rectangles sont déterminées par les dimensions de la texture
        private Texture2D texture;
        private Vector2 position;
        private Rectangle bouton;

        // Les 3 types possibles de boutons (mêmes types que les Survivants)
        public enum TypeBouton { Sniper, Soldat, Escrimeur };

        // Détermine si le bouton est actif ou non, donc s'il doit être affiché ou non
        public bool Actif
        {
            get;
            set;
        }


        public TypeBouton Type
        {
            get;
            private set;
        }


        public Bouton(Texture2D texture, Vector2 position, TypeBouton type)
        {
            this.texture = texture;
            this.position = position;
            bouton = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            this.Type = type;
        }


        public void Dessiner(SpriteBatch spriteBatch)
        {
            if(Actif)
                spriteBatch.Draw(texture, bouton, Color.White);
        }

        /// <summary>
        /// Détermine si le bouton a été cliqué en vérifiant si le pointeur de la 
        /// souris est dans le rectangle lors du clic
        /// </summary>
        /// <param name="position">La position du pointeur de la souris</param>
        /// <returns>Vrai si le bouton a été cliqué, faux sinon</returns>
        public bool OnClick(Point position)
        {
            return bouton.Contains(position);
        }
    }
}
