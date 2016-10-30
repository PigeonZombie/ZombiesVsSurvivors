using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombiesVsSurvivors
{
    class FabriqueZombie
    {
        public enum typeZombie { Coureur, Tank };

        public static Zombie CreerZombie(typeZombie zombie, Vector2 positionDepart, float rotation, Vector2 limiteHautGauche, Vector2 limiteBasDroite, Acteur cible, Camp campAdverse, SpriteBatch batch, SpriteFont font)
        {
            if (zombie == typeZombie.Coureur)
                return new Coureur(positionDepart, rotation, limiteHautGauche, limiteBasDroite, cible, campAdverse, batch, font);
            else
                return new Tank(positionDepart, rotation, limiteHautGauche, limiteBasDroite, cible, campAdverse, batch, font);
        }
    }
}
