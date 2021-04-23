using Gabriel.Cat.S.Binaris;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorVisualPokemonErrante.Core
{
    public class ScriptEVPE:IElementoBinarioComplejo
    {
        public static readonly ElementoBinario Serializador = ElementoBinario.GetSerializador<ScriptEVPE>();

        public ScriptEVPE(short pokemon, short vida, byte nivel, byte estado):this()
        {
            this.Pokemon = pokemon;
            this.Vida = vida;
            this.Nivel = nivel;
            this.Estado = estado;
        }
        public ScriptEVPE() { }


        public short Pokemon { get; set; }

        public short Vida { get; set; }

        public byte Nivel { get; set; }

        public byte Estado { get; set; }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
    }
}
