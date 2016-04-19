using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorVisualPokemonErrante
{
    public class scriptEVPE : Gabriel.Cat.Binaris.ElementoComplejoBinario
    {
        private static scriptEVPE scriptSaver = new scriptEVPE();
        short pokemon;
        short vida;
        byte nivel;
        byte estado;


        public scriptEVPE(short pokemon, short vida, byte nivel, byte estado):this()
        {
            this.pokemon = pokemon;
            this.vida = vida;
            this.nivel = nivel;
            this.estado = estado;
        }
        private scriptEVPE()
        {

            base.PartesElemento.Afegir(Gabriel.Cat.Binaris.ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.Short));
            base.PartesElemento.Afegir(Gabriel.Cat.Binaris.ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.Short));
            base.PartesElemento.Afegir(Gabriel.Cat.Binaris.ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.Byte));
            base.PartesElemento.Afegir(Gabriel.Cat.Binaris.ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.Byte));
        }



        public short Pokemon
        {
            get
            {
                return pokemon;
            }

            set
            {
                pokemon = value;
            }
        }

        public short Vida
        {
            get
            {
                return vida;
            }

            set
            {
                vida = value;
            }
        }

        public byte Nivel
        {
            get
            {
                return nivel;
            }

            set
            {
                nivel = value;
            }
        }

        public byte Estado
        {
            get
            {
                return estado;
            }

            set
            {
                estado = value;
            }
        }


        public override object GetObject(object[] parts)
        {
            return new scriptEVPE(Convert.ToInt16(parts[0]), Convert.ToInt16(parts[1]), Convert.ToByte(parts[2]), Convert.ToByte(parts[3]));
        }



        public override byte[] GetBytes(object obj)
        {
            List<byte> bytes = new List<byte>();
            scriptEVPE script = obj as scriptEVPE;
            if (script != null)
            {
                bytes.AddRange(base.PartesElemento[0].GetBytes(script.pokemon));
                bytes.AddRange(base.PartesElemento[1].GetBytes(script.vida));
                bytes.AddRange(base.PartesElemento[2].GetBytes(script.nivel));
                bytes.AddRange(base.PartesElemento[3].GetBytes(script.estado));
            }
            else
                bytes.Add((byte)0x0);
            return bytes.ToArray();
        }
        public static byte[] GetByteArray(scriptEVPE script)
        {
            if (script == null)
                throw new ArgumentNullException();
            return  scriptSaver.GetBytes(script);
        }
        public static scriptEVPE GetScriptEVPE(Stream stream)
        {
            return (scriptEVPE)scriptSaver.GetObject(stream);
        }
    }
}
