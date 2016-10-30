using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombiesVsSurvivors
{
    public class Camp : Objet2D
    {
       
        public int PointsVie
        {
            get;
            set;
        }

        // Le point vers lequel les personnages convergent pour attaquer le camp
        public Vector2 Cible
        {
            get;
            set;
        }



        public Camp(Texture2D image, Vector2 position, Vector2 cible, SpriteBatch spriteBatch, SpriteFont mainFont) : base(image,position, spriteBatch, mainFont)
        {
            PointsVie = 200;
            TexteVie = "Vie: " + PointsVie;
            Cible = cible;
            positionVie = new Vector2(position.X + image.Width/3, position.Y+image.Height);
        }

        /// <summary>
        /// Réduit le nombre de points de vie du camp quand il se fait attaquer et met 
        /// l'affichage de ses PV restants à jour 
        /// </summary>
        /// <param name="degats">Les dégâts</param>
        public void InfligerDegats(int degats)
        {
            this.PointsVie -= degats;
            if (PointsVie < 0)
                PointsVie = 0;
            TexteVie = "Vie: " + PointsVie;
        }



    }
}
