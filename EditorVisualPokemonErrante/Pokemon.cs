using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorVisualPokemonErrante
{
    public class Pokemon:Gabriel.Cat.IClauUnicaPerObjecte,IComparable,IComparable<Pokemon>
    {
        int vidaQueTiene;
        int[] vidaXNivel;//vida por nivel
        string nombre;
        Bitmap img;
        int numeroNacional;

        public Pokemon(string nombre, Bitmap img, int numeroNacional)
        {
            this.nombre = nombre.ToUpper();
            this.img = img;
            this.numeroNacional = numeroNacional;
        }
        internal string NombreInternoGif
        {
            get
            {
               return Nombre.Replace('.','0').Replace('-','1').Replace("'","2").Replace('♀', '3').Replace('♂', '4').ToLower();
            }
        }
        public int VidaQueTiene
        {
            get
            {
                return vidaQueTiene;
            }

            set
            {
                vidaQueTiene = value;
            }
        }

        public int[] VidaXNivel
        {
            get
            {
                return vidaXNivel;
            }

            set
            {
                vidaXNivel = value;
            }
        }

        public string Nombre
        {
            get
            {
                return nombre;
            }

            set
            {
                nombre = value;
            }
        }

        public Bitmap Img
        {
            get
            {
                return img;
            }

            set
            {
                img = value;
            }
        }

        public int NumeroNacional
        {
            get
            {
                return numeroNacional;
            }

            set
            {
                numeroNacional = value;
            }
        }

        public IComparable Clau
        {
            get
            {
                return numeroNacional;
            }
        }
        public override string ToString()
        {
            return Nombre;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Pokemon);
        }

        public int CompareTo(Pokemon other)
        {
            int compareTo = -1;
            if (other != null)
                compareTo = Clau.CompareTo(other.Clau);
            return compareTo;
        }
    }
}
