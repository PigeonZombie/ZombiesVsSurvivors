using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombiesVsSurvivors
{
    class FabriqueSurvivant
    {
        public enum typeSurvivant { Sniper, Soldat, Escrimeur };

        public static Survivant CreerSurvivant (typeSurvivant type, Vector2 positionDepart, float rotation, Vector2 limiteHautGauche, Vector2 limiteBasDroite, Acteur cible, Camp campAdverse, SpriteBatch batch, SpriteFont font)
        {
            if (type == typeSurvivant.Sniper)
                return new Sniper(positionDepart, rotation, limiteHautGauche, limiteBasDroite, cible, campAdverse, batch, font);
            else if (type == typeSurvivant.Escrimeur)
                return new Escrimeur(positionDepart, rotation, limiteHautGauche, limiteBasDroite, cible, campAdverse, batch, font);
            else
                return new Soldat(positionDepart, rotation, limiteHautGauche, limiteBasDroite, cible, campAdverse, batch, font);
        }
    }
}
