using Gabriel.Cat;
using Gabriel.Cat.Extension;
using PokemonGBAFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EditorVisualPokemonErrante
{
    /// <summary>
    /// Lógica de interacción para ExportarXSE.xaml
    /// </summary>
    public partial class PrevisualizarScriptXSE : Window
    {
        PokemonGBAFrameWork.PokemonErrante.Pokemon pokemon;

        public PrevisualizarScriptXSE(PokemonGBAFrameWork.PokemonErrante.Pokemon pokemon)
        {
            InitializeComponent();
            Title = "Scirpt Rom version Española";//mas adelante hacerlo universal :D
            this.pokemon = pokemon;
            PonScriptEImagen();
            MainWindow.JuegoUpdated += (s, e) => {
                if (MainWindow.JuegoData.Pokedex.Count > pokemon.PokemonErrante.OrdenPokedexNacional)//si sigue valiendo el script
                {
                    pokemon = new PokemonErrante.Pokemon(MainWindow.JuegoData.Pokedex[pokemon.PokemonErrante.OrdenPokedexNacional], pokemon.Vida, pokemon.Nivel, pokemon.Stats);
                    PonScriptEImagen();
                }
                else Close();
            };


        }

        private void PonScriptEImagen()
        {
            switch (MainWindow.JuegoData.Edicion.AbreviacionRom)
            {
                case Edicion.ABREVIACIONROJOFUEGO:
                    imgVersion.SetImage(Resource1.FireRed); break;
                case Edicion.ABREVIACIONESMERALDA:
                    imgVersion.SetImage(Resource1.Emerald); break;
            }
            txtScript.Text = PokemonGBAFrameWork.PokemonErrante.Pokemon.Script(MainWindow.Juego, MainWindow.JuegoData.Edicion, MainWindow.JuegoData.Compilacion, pokemon);
        }
    }
}
