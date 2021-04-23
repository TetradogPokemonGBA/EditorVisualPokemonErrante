using Gabriel.Cat;
using Gabriel.Cat.S.Extension;
using PokemonGBAFramework.Core;

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

namespace EditorVisualPokemonErrante.Core
{
    /// <summary>
    /// Lógica de interacción para ExportarXSE.xaml
    /// </summary>
    public partial class PrevisualizarScriptXSE : Window
    {
        PokemonErrante.Pokemon pokemon;

        public PrevisualizarScriptXSE(PokemonErrante.Pokemon pokemon)
        {
            InitializeComponent();
            Title = "Scirpt Rom";
            this.pokemon = pokemon;
            PonScriptEImagen();
            MainWindow.JuegoUpdated += (s, e) => {
                if (MainWindow.Total > pokemon.Errante.OrdenNacional.Orden)//si sigue valiendo el script
                {
                    pokemon = new PokemonErrante.Pokemon() { Errante = Pokemon.Get(MainWindow.Juego, pokemon.Errante.OrdenNacional.Orden), Vida = pokemon.Vida, Nivel = pokemon.Nivel, Stats = pokemon.Stats };
                    PonScriptEImagen();
                }
                else Close();
            };


        }

        private void PonScriptEImagen()
        {
            switch (MainWindow.Juego.Edicion.Version)
            {
                case Edicion.Pokemon.RojoFuego:
                    imgVersion.SetImage(Resource1.FireRed); break;
                case Edicion.Pokemon.Esmeralda:
                    imgVersion.SetImage(Resource1.Emerald); break;
                case Edicion.Pokemon.VerdeHoja:
                    imgVersion.SetImage(Resource1.LeafGreen);break;
            }
            txtScript.Text = PokemonErrante.GetScript(MainWindow.Juego, pokemon).GetDeclaracionXSE();
        }
    }
}
