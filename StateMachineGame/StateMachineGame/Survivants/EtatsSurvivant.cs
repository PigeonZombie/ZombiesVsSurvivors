using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZombiesVsSurvivors
{
    interface EtatsSurvivant
    {
        void OnUpdate();

    }
    
    // À l'état Normal, les survivants se dirigent vers la base
    // adverse ou l'attaquent s'ils sont assez proche
    class EtatNormal : EtatsSurvivant
    {
        readonly Survivant survivant;
        public EtatNormal(Survivant survivant)
        {
            this.survivant = survivant;
            survivant.SymboleEtat = "N";
        }

        public void OnUpdate()
        {
            survivant.AttaquerCampEnnemi();
        }

    }

    // À l'état Offensif, les survivants attaquent un zombie ou se dirigent vers lui
    class EtatOffensif : EtatsSurvivant
    {
        readonly Survivant survivant;
        public EtatOffensif(Survivant survivant)
        {
            this.survivant = survivant;
            survivant.SymboleEtat = "O";
        }

        public void OnUpdate()
        {
            if (survivant.CiblePersonnage == null || survivant.CiblePersonnage.PointsdeVie <= 0)
            {
                survivant.etat = new EtatNormal(survivant);
                survivant.CiblePersonnage = null;
                survivant.SymboleEtat = "N";
            }

            else
                survivant.AttaquerZombie();
        }


    }

    // À l'état Caché, les survivants se dirigent vers le couvert le plus proche
    // d'eux et guérissent pour quelques seconde quand ils y sont. Ils arrêtent d'attaquer.
    class EtatCache : EtatsSurvivant
    {
        readonly Survivant survivant;
        private int tempsGuerisonMax = 480;
        private int tempsGuerisonActuel = 0;

        public EtatCache(Survivant survivant)
        {
            this.survivant = survivant;
            survivant.SymboleEtat = "C";
            survivant.CiblePersonnage = null;
            survivant.TrouverCouvertPlusProche();
        }

        public void OnUpdate()
        {
            tempsGuerisonActuel++;
            if (survivant.PointsdeVie == survivant.PointsDeVieMax || tempsGuerisonActuel==tempsGuerisonMax)
            {
                survivant.etat = new EtatNormal(survivant);
                survivant.CiblePersonnage = null;
                survivant.SymboleEtat = "N";
                survivant.EstCache = false;
            }
            else
                survivant.Fuir();

        }

    }

    // En état d'Alerte, les survivants se dirigent vers le zombie Tank qui
    // est à proximité de la base et l'attaquent
    class EtatAlerte : EtatsSurvivant
    {
        readonly Survivant survivant;
        public EtatAlerte(Survivant survivant)
        {
            this.survivant = survivant;
            survivant.SymboleEtat = "A";
            
        }

        public void OnUpdate()
        {
            if (survivant.CiblePersonnage == null || survivant.CiblePersonnage.PointsdeVie <= 0)
            {
                survivant.etat = new EtatNormal(survivant);
                survivant.CiblePersonnage = null;
                survivant.SymboleEtat = "N";
                survivant.Alerte = false;
            }
            else
                survivant.AttaquerZombie();
        }

    }

    // En état de Repli, les escrimeurs se dirigent vers leur camp pour attaquer
    // les zombies qui y sont
    class EtatRepli: EtatsSurvivant
    {
        private readonly Survivant survivant;

        public EtatRepli(Survivant survivant)
        {
            this.survivant = survivant;
            survivant.SymboleEtat = "R";
        }

        public void OnUpdate()
        {
            if (Game1.NbZombiesCampSurvivants >= 5)
            {
                Zombie nouvelleCible = survivant.TrouverEnnemiPlusProche(survivant.Position);
                if (nouvelleCible == null)
                    survivant.MouvementVersCible(Survivant.CampAmi.Cible);
                else
                {
                    survivant.CiblePersonnage = nouvelleCible;
                    survivant.AttaquerZombie();
                }
            }
            else
            {
                survivant.etat = new EtatNormal(survivant);
                survivant.SymboleEtat = "N";
            }
        }

    }


}
