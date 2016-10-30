using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZombiesVsSurvivors
{
    interface EtatsZombies
    {
        void OnUpdate();
    }

    // À l'état Normal, les zombies se dirigent vers le camp adverse ou 
    // l'attaquent s'ils sont assez près.
    class EtatNormal_Z: EtatsZombies
    {
        private readonly Zombie zombie;

        public EtatNormal_Z(Zombie zombie)
        {
            this.zombie = zombie;
            zombie.CiblePersonnage = null;
            zombie.SymboleEtat = "N";
        }

        public void OnUpdate()
        {
            zombie.AttaquerCampEnnemi();
        }

    }

    // En état Offensif, les Coureurs attaquent un survivant ou le suivent 
    class EtatOffensif_Z: EtatsZombies
    {
        private readonly Zombie zombie;

        public EtatOffensif_Z(Zombie zombie)
        {
            this.zombie = zombie;
            zombie.SymboleEtat = "O";

        }

        public void OnUpdate()
        {
            zombie.Attaquer();
        }

    }


}
